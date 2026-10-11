using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
