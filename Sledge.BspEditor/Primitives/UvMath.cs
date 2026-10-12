using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.DataStructures.Geometric;

namespace Sledge.BspEditor.Primitives
{
    /// <summary>
    /// Engine limits that texture mapping has to respect. The defaults are the GoldSrc / Counter-Strike 1.6 ones;
    /// other games can use a different instance (bigger limits) without touching the maths below.
    /// </summary>
    public sealed class UvLimits
    {
        /// <summary>Size of one lightmap luxel, in texels (texture space).</summary>
        public float LuxelSize { get; init; } = 16;

        /// <summary>Maximum number of luxels a single face may span along s or t ("Bad surface extents" above this).</summary>
        public int MaxLuxelSpan { get; init; } = 16;

        /// <summary>Largest texture side the engine accepts.</summary>
        public int MaxTextureSize { get; init; } = 512;

        /// <summary>Texture sides must be a multiple of this.</summary>
        public int TextureSizeMultiple { get; init; } = 16;

        /// <summary>Smallest absolute texture scale we allow tools to produce.</summary>
        public float MinScale { get; init; } = 0.01f;

        public static UvLimits GoldSrc { get; } = new UvLimits();
    }

    /// <summary>The texture-space (texel) bounds of a face, and how many luxels that covers.</summary>
    public readonly struct SurfaceExtents
    {
        public float MinS { get; }
        public float MaxS { get; }
        public float MinT { get; }
        public float MaxT { get; }

        /// <summary>Luxels covered along s, aligned to the luxel grid (this is what the compiler checks).</summary>
        public int LuxelsS { get; }

        /// <summary>Luxels covered along t, aligned to the luxel grid (this is what the compiler checks).</summary>
        public int LuxelsT { get; }

        public SurfaceExtents(float minS, float maxS, float minT, float maxT, int luxelsS, int luxelsT)
        {
            MinS = minS;
            MaxS = maxS;
            MinT = minT;
            MaxT = maxT;
            LuxelsS = luxelsS;
            LuxelsT = luxelsT;
        }

        public float SpanS => MaxS - MinS;
        public float SpanT => MaxT - MinT;

        public bool Exceeds(UvLimits limits) => LuxelsS > limits.MaxLuxelSpan || LuxelsT > limits.MaxLuxelSpan;
    }

    /// <summary>
    /// Pure maths for texture mapping. Nothing in here touches the UI or the document, so it can be reused by
    /// texture lock, linked-object syncing, the alignment commands and the UV editor.
    ///
    /// Convention (matches Face.GetTextureCoordinates): for a world point P,
    ///   s = P.UAxis / XScale + XShift
    ///   t = P.VAxis / YScale + YShift
    /// both in texels.
    /// </summary>
    public static class UvMath
    {
        /// <summary>The (s, t) texel coordinates of a world point.</summary>
        public static Vector2 GetST(Texture tex, Vector3 point)
        {
            var s = Math.Abs(tex.XScale) > 1e-6f ? point.Dot(tex.UAxis) / tex.XScale + tex.XShift : 0;
            var t = Math.Abs(tex.YScale) > 1e-6f ? point.Dot(tex.VAxis) / tex.YScale + tex.YShift : 0;
            return new Vector2(s, t);
        }

        /// <summary>
        /// The texel bounds of the given face vertices, plus the luxel span the compiler will measure.
        /// The luxel span is shift dependent: it is measured on the luxel grid (floor of the minimum to ceil of the
        /// maximum), so the same face can pass or fail depending on its shift.
        /// </summary>
        public static SurfaceExtents GetExtents(Texture tex, IEnumerable<Vector3> vertices, UvLimits limits = null)
        {
            limits ??= UvLimits.GoldSrc;

            var minS = float.MaxValue;
            var minT = float.MaxValue;
            var maxS = float.MinValue;
            var maxT = float.MinValue;
            var any = false;

            foreach (var v in vertices)
            {
                var st = GetST(tex, v);
                minS = Math.Min(minS, st.X);
                maxS = Math.Max(maxS, st.X);
                minT = Math.Min(minT, st.Y);
                maxT = Math.Max(maxT, st.Y);
                any = true;
            }

            if (!any) return new SurfaceExtents(0, 0, 0, 0, 0, 0);

            var luxel = limits.LuxelSize;
            var luxelsS = (int)(MathF.Ceiling(maxS / luxel) - MathF.Floor(minS / luxel));
            var luxelsT = (int)(MathF.Ceiling(maxT / luxel) - MathF.Floor(minT / luxel));
            return new SurfaceExtents(minS, maxS, minT, maxT, luxelsS, luxelsT);
        }

        /// <summary>
        /// Re-fits the texture so that every point of the face keeps exactly the (s, t) it had before the face moved.
        /// This is what "texture lock" means, solved exactly for any transform of the vertices (translation, rotation,
        /// flip, non-uniform scale, skew, vertex edits), as long as the face stays planar.
        /// <paramref name="oldVertices"/> and <paramref name="newVertices"/> are the same vertices before and after, in the same order.
        /// The texture is only modified when this returns true. Rotation, ULength and VLength are left alone.
        /// </summary>
        public static bool TryPreserveMapping(Texture tex, IReadOnlyList<Vector3> oldVertices, IReadOnlyList<Vector3> newVertices)
        {
            if (tex == null || oldVertices == null || newVertices == null) return false;
            if (oldVertices.Count < 3 || oldVertices.Count != newVertices.Count) return false;

            if (!PickBasis(newVertices, out var i, out var j, out var k)) return false;

            var q0 = newVertices[i];
            var e1 = newVertices[j] - q0;
            var e2 = newVertices[k] - q0;
            var n = e1.Cross(e2).Normalise();

            // The texture as it was, evaluated at the old positions of the three basis vertices
            var s0 = GetST(tex, oldVertices[i]);
            var s1 = GetST(tex, oldVertices[j]);
            var s2 = GetST(tex, oldVertices[k]);

            // Gradient of s (and t) over the new face: a.e1 = ds1, a.e2 = ds2, a.n = 0 (the gradient lies in the plane).
            // Solved with the dual basis of (e1, e2, n).
            var e2n = e2.Cross(n);
            var ne1 = n.Cross(e1);
            var triple = e1.Dot(e2n);
            if (Math.Abs(triple) < 1e-6f) return false;

            var gs = (e2n * (s1.X - s0.X) + ne1 * (s2.X - s0.X)) / triple;
            var gt = (e2n * (s1.Y - s0.Y) + ne1 * (s2.Y - s0.Y)) / triple;

            var ls = gs.Length();
            var lt = gt.Length();
            if (!(ls > 1e-9f) || !(lt > 1e-9f) || float.IsInfinity(ls) || float.IsInfinity(lt)) return false;

            // Keep the handedness the texture already had (negative scale = mirrored)
            var signS = tex.XScale < 0 ? -1f : 1f;
            var signT = tex.YScale < 0 ? -1f : 1f;

            var xShift = s0.X - gs.Dot(q0);
            var yShift = s0.Y - gt.Dot(q0);
            if (float.IsNaN(xShift) || float.IsNaN(yShift) || float.IsInfinity(xShift) || float.IsInfinity(yShift)) return false;

            tex.UAxis = gs * (signS / ls);
            tex.VAxis = gt * (signT / lt);
            tex.XScale = Denoise(signS / ls);
            tex.YScale = Denoise(signT / lt);
            tex.XShift = Denoise(xShift);
            tex.YShift = Denoise(yShift);
            return true;
        }

        /// <summary>
        /// For a face whose vertices have already been moved by <paramref name="applied"/>, re-fits the texture so the
        /// face keeps the mapping it had before the move (exact texture lock for any transform).
        /// </summary>
        public static bool TryLockAfterTransform(Texture tex, IReadOnlyList<Vector3> currentVertices, Matrix4x4 applied)
        {
            if (currentVertices == null) return false;
            if (!Matrix4x4.Invert(applied, out var inverse)) return false;
            var before = currentVertices.Select(x => Vector3.Transform(x, inverse)).ToList();
            return TryPreserveMapping(tex, before, currentVertices);
        }

        /// <summary>
        /// True if the matrix only moves, rotates or mirrors (no scaling or skewing), so a texture can follow it
        /// by just carrying its axes and shifts along.
        /// </summary>
        public static bool IsRigid(Matrix4x4 m)
        {
            var x = Vector3.TransformNormal(Vector3.UnitX, m);
            var y = Vector3.TransformNormal(Vector3.UnitY, m);
            var z = Vector3.TransformNormal(Vector3.UnitZ, m);
            const float eps = 1e-4f;
            return Math.Abs(x.Length() - 1) < eps && Math.Abs(y.Length() - 1) < eps && Math.Abs(z.Length() - 1) < eps
                   && Math.Abs(x.Dot(y)) < eps && Math.Abs(y.Dot(z)) < eps && Math.Abs(x.Dot(z)) < eps;
        }

        /// <summary>
        /// Removes float noise: a value within 1e-5 (relative) of a 4-decimal number becomes that number,
        /// so clean values (shift 0, scale 1, scale 0.5) stay clean after being solved for.
        /// </summary>
        private static float Denoise(float v)
        {
            var r = MathF.Round(v, 4);
            return Math.Abs(v - r) <= 1e-5f * Math.Max(1f, Math.Abs(v)) ? r : v;
        }

        /// <summary>
        /// Same as the overload above, for a face whose vertices were moved by <paramref name="transform"/>.
        /// </summary>
        public static bool TryPreserveMapping(Texture tex, IReadOnlyList<Vector3> oldVertices, Matrix4x4 transform)
        {
            if (oldVertices == null) return false;
            var moved = oldVertices.Select(x => Vector3.Transform(x, transform)).ToList();
            return TryPreserveMapping(tex, oldVertices, moved);
        }

        /// <summary>
        /// Unfolds a group of connected faces into one continuous, undistorted layout.
        /// The least distorted face keeps its mapping; the others are reached across shared edges, each one rotated about
        /// the edge into the plane of the face it was reached from, so the shared edge gets identical (s, t) on both sides
        /// and the scale stays the same. Only faces with the same texture are joined. Returns the new texture for every face
        /// that was reached, by face ID (faces that aren't connected to the reference are left out).
        /// </summary>
        public static Dictionary<long, Texture> Unfold(IReadOnlyList<Face> faces)
        {
            var result = new Dictionary<long, Texture>();
            if (faces == null || faces.Count == 0) return result;

            // The reference is the face that is currently the least distorted (biggest one wins a tie)
            Face reference = null;
            var bestDistortion = float.MaxValue;
            var bestArea = 0f;
            foreach (var face in faces)
            {
                var distortion = Distortion(face);
                var area = Area(face.Vertices);
                if (reference == null || distortion < bestDistortion - 1e-3f || (Math.Abs(distortion - bestDistortion) <= 1e-3f && area > bestArea))
                {
                    reference = face;
                    bestDistortion = distortion;
                    bestArea = area;
                }
            }

            result[reference.ID] = reference.Texture.Clone();
            var queue = new Queue<Face>();
            queue.Enqueue(reference);

            while (queue.Count > 0)
            {
                var parent = queue.Dequeue();
                var parentTexture = result[parent.ID];

                foreach (var next in faces)
                {
                    if (result.ContainsKey(next.ID)) continue;
                    if (!string.Equals(next.Texture.Name, parent.Texture.Name, StringComparison.InvariantCultureIgnoreCase)) continue;
                    if (!TryGetSharedEdge(parent, next, out var a, out var b)) continue;

                    // Where the next face's vertices would be if it were folded flat into the parent's plane
                    var flat = UnfoldInto(parent, next, a, b);
                    if (flat == null) continue;

                    // The parent's mapping, read at those flattened positions, becomes the next face's mapping
                    var solved = parentTexture.Clone();
                    if (!TryPreserveMapping(solved, flat, next.Vertices.ToList())) continue;

                    var texture = next.Texture.Clone();
                    texture.UAxis = solved.UAxis;
                    texture.VAxis = solved.VAxis;
                    texture.ULength = solved.ULength;
                    texture.VLength = solved.VLength;
                    texture.XScale = solved.XScale;
                    texture.YScale = solved.YScale;
                    texture.XShift = solved.XShift;
                    texture.YShift = solved.YShift;
                    texture.Rotation = solved.Rotation;

                    result[next.ID] = texture;
                    queue.Enqueue(next);
                }
            }

            return result;
        }

        /// <summary>
        /// How stretched or sheared a face's mapping is: 1 means the texture keeps its shape on the face
        /// (equal scale, right angle between the axes as seen on the face), larger is worse.
        /// </summary>
        public static float Distortion(Face face)
        {
            var t = face.Texture;
            if (Math.Abs(t.XScale) < 1e-6f || Math.Abs(t.YScale) < 1e-6f) return float.MaxValue;

            var n = face.Plane.Normal;
            var gs = t.UAxis / t.XScale;
            var gt = t.VAxis / t.YScale;
            gs -= n * gs.Dot(n);
            gt -= n * gt.Dot(n);

            var a = gs.Dot(gs);
            var b = gs.Dot(gt);
            var c = gt.Dot(gt);
            var mean = (a + c) / 2f;
            var diff = MathF.Sqrt(((a - c) / 2f) * ((a - c) / 2f) + b * b);
            var min = mean - diff;
            var max = mean + diff;
            return min <= 1e-12f ? float.MaxValue : MathF.Sqrt(max / min);
        }

        private static float Area(IList<Vector3> v)
        {
            var area = 0f;
            for (var i = 1; i < v.Count - 1; i++)
            {
                area += (v[i] - v[0]).Cross(v[i + 1] - v[0]).Length() / 2f;
            }
            return area;
        }

        /// <summary>
        /// The edge two faces share, as the stretch of it that both have (the faces may be cut differently along it).
        /// </summary>
        private static bool TryGetSharedEdge(Face fa, Face fb, out Vector3 a, out Vector3 b)
        {
            a = b = default;
            var va = fa.Vertices;
            var vb = fb.Vertices;
            const float tolerance = 0.05f;
            var bestLength = tolerance;
            var found = false;

            for (var i = 0; i < va.Count; i++)
            {
                var p0 = va[i];
                var p1 = va[(i + 1) % va.Count];
                var length = (p1 - p0).Length();
                if (length < 1e-4f) continue;
                var dir = (p1 - p0) / length;

                for (var j = 0; j < vb.Count; j++)
                {
                    var q0 = vb[j];
                    var q1 = vb[(j + 1) % vb.Count];
                    if (DistanceToLine(q0, p0, dir) > tolerance || DistanceToLine(q1, p0, dir) > tolerance) continue;

                    var t0 = (q0 - p0).Dot(dir);
                    var t1 = (q1 - p0).Dot(dir);
                    var start = Math.Max(0f, Math.Min(t0, t1));
                    var end = Math.Min(length, Math.Max(t0, t1));
                    if (end - start > bestLength)
                    {
                        bestLength = end - start;
                        a = p0 + dir * start;
                        b = p0 + dir * end;
                        found = true;
                    }
                }
            }

            return found;
        }

        private static float DistanceToLine(Vector3 p, Vector3 origin, Vector3 direction)
        {
            var v = p - origin;
            return (v - direction * v.Dot(direction)).Length();
        }

        /// <summary>
        /// Rotates the next face's vertices about the shared edge a-b into the parent's plane, on the far side of the edge.
        /// </summary>
        private static List<Vector3> UnfoldInto(Face parent, Face next, Vector3 a, Vector3 b)
        {
            var edge = b - a;
            if (edge.LengthSquared() < 1e-8f) return null;
            var axis = Vector3.Normalize(edge);

            var parentNormal = parent.Plane.Normal;
            var nextNormal = next.Plane.Normal;
            var angle = MathF.Atan2(axis.Dot(nextNormal.Cross(parentNormal)), nextNormal.Dot(parentNormal));
            var rotation = Matrix4x4.CreateFromAxisAngle(axis, angle);

            return next.Vertices.Select(v => a + Vector3.Transform(v - a, rotation)).ToList();
        }

        /// <summary>
        /// Picks three well spread vertices (the farthest pair, then the vertex farthest from that line) so the solve is stable.
        /// </summary>
        private static bool PickBasis(IReadOnlyList<Vector3> v, out int a, out int b, out int c)
        {
            a = b = c = -1;
            var best = 0f;

            for (var x = 0; x < v.Count; x++)
            {
                for (var y = x + 1; y < v.Count; y++)
                {
                    var d = (v[y] - v[x]).LengthSquared();
                    if (d > best)
                    {
                        best = d;
                        a = x;
                        b = y;
                    }
                }
            }

            if (a < 0 || !(best > 1e-6f)) return false;

            var line = v[b] - v[a];
            var bestArea = 0f;
            for (var z = 0; z < v.Count; z++)
            {
                if (z == a || z == b) continue;
                var area = line.Cross(v[z] - v[a]).LengthSquared();
                if (area > bestArea)
                {
                    bestArea = area;
                    c = z;
                }
            }

            return c >= 0 && bestArea > 1e-6f;
        }
    }
}
