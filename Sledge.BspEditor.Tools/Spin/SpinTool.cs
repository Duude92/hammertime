using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations.Tree;
using Sledge.BspEditor.Primitives;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.BspEditor.Tools.Properties;
using Sledge.Common.Shell;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Documents;
using Sledge.DataStructures.Geometric;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Overlay;
using Sledge.Rendering.Viewports;
using Sledge.Shell.Input;
using Plane = Sledge.DataStructures.Geometric.Plane;

namespace Sledge.BspEditor.Tools.Spin
{
    /// <summary>
    /// Spin tool: duplicates the selected solids around an axis running through
    /// the rotation gizmo pivot (Selection.RotationPivot, falling back to the
    /// selection centre) and bisects every piece against the radial half-space
    /// planes of its own angular slot, so the result forms a clean arch with
    /// radial seams. The interaction is similar to Blender's spin tool: drag in
    /// a 2D viewport around the pivot to sweep out the total angle.
    ///
    /// Textures: the texture axes of every duplicate are rotated along with the
    /// geometry (like texture lock). Every second piece then has its texture
    /// mapping mirrored about the mid-radial plane of its slot. Together these
    /// two steps make the texture on the visible arc surfaces continue across
    /// the seams between adjacent pieces without a jump - the pattern is
    /// mirrored per segment ("flipped scale") so the faces tile up seamlessly.
    /// </summary>
    [Export(typeof(ITool))]
    [Export]
    [OrderHint("M")]
    public class SpinTool : BaseTool
    {
        public static readonly Capability SpinToolCapability = Capability.Create("SpinTool");

        /// <summary>Number of segments (pieces) in the resulting arch.</summary>
        public int Steps { get; set; } = 8;

        /// <summary>Total sweep angle in degrees, used when applying from the panel or with Enter.</summary>
        public float AngleDegrees { get; set; } = 90f;

        /// <summary>When true, every piece is cut by the radial planes bounding its slot.</summary>
        public bool BisectPieces { get; set; } = true;

        /// <summary>When true, every second piece gets its texture mapping mirrored so the pattern tiles seamlessly.</summary>
        public bool MirrorUvs { get; set; } = true;

        // Drag state
        private bool _dragging;
        private bool _movingPivot;
        private Vector3 _dragPivot;
        private float _dragStartAngle;
        private float _dragSweep;

        /// <summary>Screen-space grab radius for the pivot (gizmo) handle.</summary>
        private const int PivotDragSize = 8;

        // The 2D view the tool was last used in; the spin axis is derived from it
        private OrthographicCamera.OrthographicType _viewType = OrthographicCamera.OrthographicType.Top;

        public SpinTool()
        {
            Usage = ToolUsage.Both;
        }

        public override Image GetIcon()
        {
            return IconLoader.Load(typeof(Resources), nameof(Resources.Tool_Spin));
        }

        public override string GetName()
        {
            return "Spin Tool";
        }

        public override Capability ToolCapability => SpinToolCapability;

        #region Viewport interaction

        protected override void MouseDown(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e)
        {
            if (e.Button != MouseButtons.Left) return;

            _viewType = camera.ViewType;

            if (!document.Selection.OfType<Solid>().Any()) return;

            var pivot = GetPivot(document);

            // Dragging the pivot marker moves the gizmo itself
            var screenPivot = camera.WorldToScreen(pivot);
            if (Math.Abs(screenPivot.X - e.X) <= PivotDragSize && Math.Abs(screenPivot.Y - e.Y) <= PivotDragSize)
            {
                _movingPivot = true;
                _dragging = false;
                _dragPivot = pivot;
                return;
            }

            GetSpinFrame(camera.ViewType, out var u, out var v, out _);

            // Clicks too close to the pivot can't define a meaningful angle
            var point = camera.ScreenToWorld(e.X, e.Y);
            if ((point - pivot).Length() < 1f) return;

            _dragging = true;
            _movingPivot = false;
            _dragPivot = pivot;
            _dragStartAngle = AngleAt(point, pivot, u, v);
            _dragSweep = 0f;
        }

        protected override void MouseMove(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e)
        {
            _viewType = camera.ViewType;

            if (_movingPivot)
            {
                // Move the gizmo pivot, keeping its depth (the axis the current
                // view looks down) and snapping to the grid
                var point = camera.GetUnusedCoordinate(GetPivot(document)) + SnapIfNeeded(camera.ScreenToWorld(e.X, e.Y));
                document.Selection.RotationPivot = point;
                _dragPivot = point;
                return;
            }

            if (!_dragging) return;

            GetSpinFrame(camera.ViewType, out var u, out var v, out _);
            var p = camera.ScreenToWorld(e.X, e.Y);
            var sweep = NormalizeDegrees(AngleAt(p, _dragPivot, u, v) - _dragStartAngle);
            if (KeyboardState.Shift) sweep = (float) Math.Round(sweep / 45f) * 45f;
            _dragSweep = sweep;
        }

        protected override void MouseUp(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e)
        {
            if (_movingPivot)
            {
                // The pivot is already stored on the selection while dragging
                _movingPivot = false;
                return;
            }

            if (!_dragging) return;
            _dragging = false;
            if (e.Button != MouseButtons.Left) return;

            // A real drag uses the dragged sweep angle; a plain click uses the panel's angle
            var dragged = Math.Abs(_dragSweep) >= 1f;
            var angle = dragged ? _dragSweep : AngleDegrees;
            if (dragged)
            {
                // Keep the panel in sync with the angle that was actually used
                AngleDegrees = _dragSweep;
                Oy.Publish("SpinTool:SetAngle", _dragSweep.ToString("0.#", System.Globalization.CultureInfo.InvariantCulture));
            }
            PerformSpin(document, angle);
        }

        protected override void KeyDown(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e)
        {
            HandleKey(document, e);
        }

        protected override void KeyDown(MapDocument document, MapViewport viewport, PerspectiveCamera camera, ViewportEvent e)
        {
            HandleKey(document, e);
        }

        private void HandleKey(MapDocument document, ViewportEvent e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                _dragging = false;
                _movingPivot = false;
            }
            else if (e.KeyCode == Keys.Enter)
            {
                PerformSpin(document, AngleDegrees);
            }
        }

        #endregion

        #region Spin operation

        /// <summary>
        /// Called by the sidebar panel's Spin button: apply the spin with the panel's angle.
        /// </summary>
        public void ApplySpin()
        {
            var document = GetDocument();
            if (document == null) return;
            PerformSpin(document, AngleDegrees);
        }

        private void PerformSpin(MapDocument document, float totalAngle)
        {
            var solids = document.Selection.OfType<Solid>().Where(x => x.Hierarchy.Parent != null).ToList();
            if (!solids.Any()) return;

            var steps = Math.Max(1, Math.Min(64, Steps));
            totalAngle = NormalizeDegrees(totalAngle);
            if (Math.Abs(totalAngle) < 0.01f) return;

            GetSpinFrame(_viewType, out var u, out var v, out var axis);

            var pivot = GetPivot(document);
            var delta = totalAngle / steps;

            // A slot wider than ~179 degrees can't be represented as the
            // intersection of two half-spaces, so skip cutting in that case.
            var bisect = BisectPieces && Math.Abs(delta) < 179f;

            var generator = document.Map.NumberGenerator;
            var transaction = new Transaction();
            var changed = false;

            foreach (var solid in solids)
            {
                var parent = solid.Hierarchy.Parent.ID;

                // Slots are anchored to the solid's own angular extent: positive
                // sweeps align with the leading (minimum) edge, negative sweeps
                // with the trailing (maximum) edge, so a slot always covers the
                // rotated copy that belongs to it.
                AngularExtent(solid, pivot, u, v, out var minAngle, out var maxAngle);
                var startAngle = delta > 0 ? minAngle : maxAngle;

                // Piece 0 is the original: trim it down to its own slot
                if (bisect)
                {
                    var piece = CutToSlot(document, solid, pivot, axis, u, v, startAngle, delta);
                    if (piece != null)
                    {
                        transaction.Add(new Detatch(parent, solid));
                        transaction.Add(new Attach(parent, piece));
                        changed = true;
                    }
                }

                // One duplicate per step. The last copy sits on the final slot
                // boundary and is not needed, so steps - 1 duplicates are made.
                for (var j = 1; j < steps; j++)
                {
                    var copy = (Solid) solid.Copy(generator);
                    copy.IsSelected = true;

                    var rotation = Matrix4x4.CreateTranslation(-pivot)
                                   * Matrix4x4.CreateFromAxisAngle(axis, delta * j * MathF.PI / 180f)
                                   * Matrix4x4.CreateTranslation(pivot);
                    copy.Transform(rotation);
                    foreach (var face in copy.Faces)
                    {
                        face.Texture.TransformUniform(rotation);
                    }

                    var current = copy;
                    if (bisect)
                    {
                        current = CutToSlot(document, copy, pivot, axis, u, v, startAngle + delta * j, delta) ?? copy;
                    }

                    if (MirrorUvs && j % 2 == 1)
                    {
                        // Mirror the texture mapping about the mid-radial plane of
                        // this piece's slot. Combined with the rotated axes this
                        // makes the pattern continue seamlessly over each seam.
                        var normal = TangentAt(axis, u, v, startAngle + delta * (j + 0.5f));
                        var mirror = CreateReflection(normal, pivot);
                        foreach (var face in current.Faces)
                        {
                            face.Texture.TransformUniform(mirror);
                        }
                    }

                    transaction.Add(new Attach(parent, current));
                    changed = true;
                }
            }

            if (!changed || transaction.IsEmpty) return;
            MapDocumentOperation.Perform(document, transaction);
        }

        /// <summary>
        /// Cuts a solid down to the angular slot [start, start + sweep] around the
        /// pivot by intersecting it with the two half-spaces bounded by the radial
        /// planes through the pivot. Returns the cut solid, or null when the solid
        /// doesn't intersect the slot or the cut fails (the caller then keeps the
        /// uncut original).
        /// </summary>
        private static Solid CutToSlot(MapDocument document, Solid solid, Vector3 pivot, Vector3 axis, Vector3 u, Vector3 v, float startDegrees, float sweepDegrees)
        {
            if (Math.Abs(sweepDegrees) < 0.01f || Math.Abs(sweepDegrees) >= 179f) return null;

            var sign = sweepDegrees > 0 ? 1f : -1f;
            var endDegrees = startDegrees + sweepDegrees;

            // Front of the start plane = the swept side of the start boundary,
            // front of the end plane = the swept side of the end boundary.
            // Note: Plane's constructor is (normal, pointOnPlane).
            var startPlane = new Plane(TangentAt(axis, u, v, startDegrees) * sign, pivot);
            var endPlane = new Plane(TangentAt(axis, u, v, endDegrees) * -sign, pivot);

            Solid current = solid;

            current.Split(document.Map.NumberGenerator, startPlane, out _, out var front);
            if (front == null) return null;
            current = front;

            current.Split(document.Map.NumberGenerator, endPlane, out _, out front);
            if (front == null) return null;
            current = front;

            // Nothing was cut away; keep the original object untouched
            if (ReferenceEquals(current, solid)) return null;

            if (!current.IsValid()) return null;

            return current;
        }

        #endregion

        #region Spin frame helpers

        private static void GetSpinFrame(OrthographicCamera.OrthographicType view, out Vector3 u, out Vector3 v, out Vector3 axis)
        {
            switch (view)
            {
                case OrthographicCamera.OrthographicType.Front:
                    axis = Vector3.UnitX;
                    u = Vector3.UnitY;
                    v = Vector3.UnitZ;
                    break;
                case OrthographicCamera.OrthographicType.Side:
                    // Negated so (u, v, axis) stays right-handed in this view
                    axis = -Vector3.UnitY;
                    u = Vector3.UnitX;
                    v = Vector3.UnitZ;
                    break;
                default:
                    axis = Vector3.UnitZ;
                    u = Vector3.UnitX;
                    v = Vector3.UnitY;
                    break;
            }
        }

        private static Vector3 GetPivot(MapDocument document)
        {
            // The rotation widget's pivot is the authoritative pivot: when a pivot
            // has been placed on the rotation gizmo it overrides the selection centre.
            if (document.Selection.RotationPivot.HasValue) return document.Selection.RotationPivot.Value;
            return document.Selection.GetSelectionBoundingBox().Center;
        }

        private static float AngleAt(Vector3 point, Vector3 pivot, Vector3 u, Vector3 v)
        {
            var diff = point - pivot;
            return (float) (Math.Atan2(diff.Dot(v), diff.Dot(u)) * 180d / Math.PI);
        }

        private static float NormalizeDegrees(float degrees)
        {
            return ((degrees % 360f) + 540f) % 360f - 180f;
        }

        /// <summary>
        /// The unit tangent direction at the given angle in the spin plane: the
        /// direction in which the angle increases when rotating about the axis.
        /// </summary>
        private static Vector3 TangentAt(Vector3 axis, Vector3 u, Vector3 v, float degrees)
        {
            var radians = degrees * MathF.PI / 180f;
            var radial = u * MathF.Cos(radians) + v * MathF.Sin(radians);
            return Vector3.Cross(axis, radial).Normalise();
        }

        /// <summary>
        /// The minimum and maximum angular positions of the solid's vertices in
        /// the spin plane, relative to the pivot. The circular mean is used as
        /// the reference so vertices that wrap around the +/-180 degree boundary
        /// don't break the extremes.
        /// </summary>
        private static void AngularExtent(Solid solid, Vector3 pivot, Vector3 u, Vector3 v, out float min, out float max)
        {
            min = max = 0f;
            var angles = solid.Faces.SelectMany(x => x.Vertices).Select(x => AngleAt(x, pivot, u, v)).ToList();
            if (angles.Count == 0) return;

            double sin = 0, cos = 0;
            foreach (var angle in angles)
            {
                var radians = angle * Math.PI / 180d;
                sin += Math.Sin(radians);
                cos += Math.Cos(radians);
            }

            var mean = (float) (Math.Atan2(sin, cos) * 180d / Math.PI);
            var unwrapped = angles.Select(x => NormalizeDegrees(x - mean)).ToList();
            min = NormalizeDegrees(mean + unwrapped.Min());
            max = NormalizeDegrees(mean + unwrapped.Max());
        }

        /// <summary>
        /// A matrix reflecting everything about the plane with the given normal
        /// running through the given point. Determinant is -1, which
        /// Texture.TransformUniform supports.
        /// </summary>
        private static Matrix4x4 CreateReflection(Vector3 normal, Vector3 point)
        {
            var reflection = Matrix4x4.Identity;
            reflection.M11 -= 2f * normal.X * normal.X;
            reflection.M12 -= 2f * normal.X * normal.Y;
            reflection.M13 -= 2f * normal.X * normal.Z;
            reflection.M21 -= 2f * normal.Y * normal.X;
            reflection.M22 -= 2f * normal.Y * normal.Y;
            reflection.M23 -= 2f * normal.Y * normal.Z;
            reflection.M31 -= 2f * normal.Z * normal.X;
            reflection.M32 -= 2f * normal.Z * normal.Y;
            reflection.M33 -= 2f * normal.Z * normal.Z;

            return Matrix4x4.CreateTranslation(-point) * reflection * Matrix4x4.CreateTranslation(point);
        }

        #endregion

        #region Rendering

        protected override void Render(MapDocument document, IViewport viewport, OrthographicCamera camera, Vector3 worldMin, Vector3 worldMax, I2DRenderer im)
        {
            base.Render(document, viewport, camera, worldMin, worldMax, im);

            if (!document.Selection.OfType<Solid>().Any()) return;

            GetSpinFrame(camera.ViewType, out var u, out var v, out _);
            var pivot = GetPivot(document);
            var radius = Math.Max(64f, document.Selection.GetSelectionBoundingBox().LargestDimension);

            // The spin circle
            const int sides = 48;
            var previous = default(Vector3);
            for (var i = 0; i <= sides; i++)
            {
                var radians = i * 2f * MathF.PI / sides;
                var point = pivot + (u * MathF.Cos(radians) + v * MathF.Sin(radians)) * radius;
                if (i > 0)
                {
                    im.AddLine(camera.WorldToScreen(previous).ToVector2(), camera.WorldToScreen(point).ToVector2(), Color.DarkGray);
                }
                previous = point;
            }

            // The pivot (gizmo) marker
            var cross = radius * 0.04f;
            im.AddLine(camera.WorldToScreen(pivot - u * cross).ToVector2(), camera.WorldToScreen(pivot + u * cross).ToVector2(), Color.Red);
            im.AddLine(camera.WorldToScreen(pivot - v * cross).ToVector2(), camera.WorldToScreen(pivot + v * cross).ToVector2(), Color.Red);

            // The sector that a spin would create: the dragged sweep while
            // dragging, otherwise the angle configured in the side panel
            var sweep = _dragging ? _dragSweep : AngleDegrees;
            var startAngle = _dragging
                ? _dragStartAngle
                : AngleAt(document.Selection.GetSelectionBoundingBox().Center, pivot, u, v);

            DrawRadial(im, camera, pivot, u, v, startAngle, radius, Color.White);
            if (Math.Abs(sweep) < 0.01f) return;

            DrawRadial(im, camera, pivot, u, v, startAngle + sweep, radius, Color.Turquoise);

            // The swept arc
            var arcSides = Math.Max(2, (int) (Math.Abs(sweep) / 360f * 96));
            previous = default(Vector3);
            for (var i = 0; i <= arcSides; i++)
            {
                var angle = startAngle + sweep * i / arcSides;
                var radians = angle * MathF.PI / 180f;
                var point = pivot + (u * MathF.Cos(radians) + v * MathF.Sin(radians)) * radius;
                if (i > 0)
                {
                    im.AddLine(camera.WorldToScreen(previous).ToVector2(), camera.WorldToScreen(point).ToVector2(), Color.Turquoise);
                }
                previous = point;
            }

            // The step boundaries inside the sector
            var steps = Math.Max(1, Math.Min(64, Steps));
            var delta = sweep / steps;
            for (var k = 1; k < steps; k++)
            {
                DrawRadial(im, camera, pivot, u, v, startAngle + delta * k, radius, Color.LightGray);
            }
        }

        private static void DrawRadial(I2DRenderer im, OrthographicCamera camera, Vector3 pivot, Vector3 u, Vector3 v, float degrees, float radius, Color colour)
        {
            var radians = degrees * MathF.PI / 180f;
            var point = pivot + (u * MathF.Cos(radians) + v * MathF.Sin(radians)) * radius;
            im.AddLine(camera.WorldToScreen(pivot).ToVector2(), camera.WorldToScreen(point).ToVector2(), colour);
        }

        #endregion
    }
}
