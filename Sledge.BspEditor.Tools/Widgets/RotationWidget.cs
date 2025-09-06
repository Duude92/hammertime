using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Windows.Forms;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Rendering.Resources;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.BspEditor.Tools.Selection;
using Sledge.DataStructures.Geometric;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Overlay;
using Sledge.Rendering.Pipelines;
using Sledge.Rendering.Primitives;
using Sledge.Rendering.Resources;
using Sledge.Rendering.Viewports;
using Sledge.Shell.Input;
using Plane = Sledge.DataStructures.Geometric.Plane;

namespace Sledge.BspEditor.Tools.Widgets
{
    public class RotationWidget : Widget
    {
        public RotationWidget(MapDocument document)
        {
            SetDocument(document);
        }


        public override bool IsUniformTransformation => true;
        public override bool IsScaleTransformation => false;

		public override SelectionBoxDraggableState.TransformationMode WidgetTransformationMode => SelectionBoxDraggableState.TransformationMode.Rotate;

		#region Line cache

		protected override void UpdateCache(IViewport viewport, PerspectiveCamera camera)
        {
            var ccl = camera.EyeLocation;
            var ccla = camera.Position + camera.Direction;

            var cache = _cachedLines.FirstOrDefault(x => x.Viewport == viewport);
            if (cache == null)
            {
                cache = new CachedLines(viewport);
                _cachedLines.Add(cache);
            }
            if (ccl == cache.CameraLocation && ccla == cache.CameraLookAt && cache.PivotPoint == Pivot && cache.Width == viewport.Width && cache.Height == viewport.Height) return;

            var origin = Pivot;
            var distance = (ccl - origin).Length();

            if (distance <= 1) return;

            cache.CameraLocation = ccl;
            cache.CameraLookAt = ccla;
            cache.PivotPoint = Pivot;
            cache.Width = (int)viewport.Width;
            cache.Height = (int)viewport.Height;

            var normal = (ccl - origin).Normalise();
            var right = normal.Cross(Vector3.UnitZ).Normalise();
            var up = normal.Cross(right).Normalise();

            var plane = new Plane(normal, origin.Dot(normal));

            const float sides = 32;
            var diff = (2 * Math.PI) / sides;

            var radius = 0.15f * distance;

            cache.Cache[AxisType.Outer].Clear();
            cache.Cache[AxisType.X].Clear();
            cache.Cache[AxisType.Y].Clear();
            cache.Cache[AxisType.Z].Clear();

            for (var i = 0; i < sides; i++)
            {
                var cos1 = (float) Math.Cos(diff * i);
                var sin1 = (float) Math.Sin(diff * i);
                var cos2 = (float) Math.Cos(diff * (i + 1));
                var sin2 = (float) Math.Sin(diff * (i + 1));

                // outer circle
                AddLine(AxisType.Outer,
                    origin + right * cos1 * radius * 1.2f + up * sin1 * radius * 1.2f,
                    origin + right * cos2 * radius * 1.2f + up * sin2 * radius * 1.2f,
                    plane, cache);

                cos1 *= radius;
                sin1 *= radius;
                cos2 *= radius;
                sin2 *= radius;

                // X/Y plane = Z axis
                AddLine(AxisType.Z,
                    origin + Vector3.UnitX * cos1 + Vector3.UnitY * sin1,
                    origin + Vector3.UnitX * cos2 + Vector3.UnitY * sin2,
                    plane, cache);

                // Y/Z plane = X axis
                AddLine(AxisType.X,
                    origin + Vector3.UnitY * cos1 + Vector3.UnitZ * sin1,
                    origin + Vector3.UnitY * cos2 + Vector3.UnitZ * sin2,
                    plane, cache);

                // X/Z plane = Y axis
                AddLine(AxisType.Y,
                    origin + Vector3.UnitZ * cos1 + Vector3.UnitX * sin1,
                    origin + Vector3.UnitZ * cos2 + Vector3.UnitX * sin2,
                    plane, cache);
            }
        }

        #endregion

        protected override Matrix4x4? GetTransformationMatrix(MapViewport viewport)
        {
            if (_mouseMovePoint == null || _mouseDownPoint == null) return null;

            var originPoint = viewport.Viewport.Camera.WorldToScreen(Pivot);
            var origv = Vector3.Normalize(_mouseDownPoint.Value - originPoint);
            var newv = Vector3.Normalize(_mouseMovePoint.Value - originPoint);
            var angle = Math.Acos(Math.Max(-1, Math.Min(1, origv.Dot(newv))));
            if ((origv.Cross(newv).Z < 0)) angle = 2 * Math.PI - angle;

            var shf = KeyboardState.Shift;
            // var def = Select.RotationStyle;
            var snap = true; // (def == RotationStyle.SnapOnShift && shf) || (def == RotationStyle.SnapOffShift && !shf);
            if (snap)
            {
                var deg = angle * (180 / Math.PI);
                var rnd = Math.Round(deg / 15) * 15;
                angle = rnd * (Math.PI / 180);
            }

            Vector3 axis;
            var dir = Vector3.Normalize(viewport.Viewport.Camera.Location - Pivot);
            switch (_mouseDown)
            {
                case AxisType.Outer:
                    axis = dir;
                    break;
                case AxisType.X:
                    axis = Vector3.UnitX;
                    break;
                case AxisType.Y:
                    axis = Vector3.UnitY;
                    break;
                case AxisType.Z:
                    axis = Vector3.UnitZ;
                    break;
                default:
                    return null;
            }
            var dirAng = Math.Acos(Vector3.Dot(dir, axis)) * 180 / Math.PI;
            if (dirAng > 90) angle = -angle;

            var rotm = Matrix4x4.CreateFromAxisAngle(axis, (float) -angle);
            var mov = Matrix4x4.CreateTranslation(-Pivot);
            var rot = Matrix4x4.Multiply(mov, rotm);
            var inv = Matrix4x4.Invert(mov, out var i) ? i : Matrix4x4.Identity;
            return Matrix4x4.Multiply(rot, inv);
        }


        protected override void Render(MapDocument document, BufferBuilder builder, ResourceCollector resourceCollector)
        {
            if (_mouseMovePoint.HasValue && _mouseDown != AxisType.None)
            {
                var axis = Vector3.One;
                var c = Color.White;

                switch (_mouseDown)
                {
                    case AxisType.X:
                        axis = Vector3.UnitX;
                        c = Color.Red;
                        break;
                    case AxisType.Y:
                        axis = Vector3.UnitY;
                        c = Color.Lime;
                        break;
                    case AxisType.Z:
                        axis = Vector3.UnitZ;
                        c = Color.Blue;
                        break;
                    case AxisType.Outer:
                        if (ActiveViewport == null || !(ActiveViewport.Viewport.Camera is PerspectiveCamera pc)) return;
                        axis = pc.Direction;
                        c = Color.White;
                        break;
                }

                var start = Pivot - axis * 1024 * 1024;
                var end = Pivot + axis * 1024 * 1024;

                var col = new Vector4(c.R, c.G, c.B, c.A) / 255;

                builder.Append(
                    new[]
                    {
                        new VertexStandard {Position = start, Colour = col, Tint = Vector4.One},
                        new VertexStandard {Position = end, Colour = col, Tint = Vector4.One},
                    },
                    new uint[] { 0, 1 },
                    new[]
                    {
                        new BufferGroup(PipelineType.Wireframe, CameraType.Perspective, 0, 2)
                    }
                );
            }

            base.Render(document, builder, resourceCollector);
        }

        protected override void Render(MapDocument document, IViewport viewport, PerspectiveCamera camera, I2DRenderer im)
        {
            if (!document.Selection.IsEmpty)
            {
                switch (_mouseMovePoint == null ? AxisType.None : _mouseDown)
                {
                    case AxisType.None:
                        RenderCircleTypeNone(camera, im);
                        break;
                    case AxisType.Outer:
                    case AxisType.X:
                    case AxisType.Y:
                    case AxisType.Z:
                        RenderAxisRotating(viewport, camera, im);
                        break;
                }
            }
            base.Render(document, viewport, camera, im);
        }

        private void RenderAxisRotating(IViewport viewport, PerspectiveCamera camera, I2DRenderer im)
        {
            if (ActiveViewport.Viewport != viewport || !_mouseDownPoint.HasValue || !_mouseMovePoint.HasValue) return;

            var st = camera.WorldToScreen(Pivot);
            var en = _mouseDownPoint.Value;
            im.AddLine(st.ToVector2(), en.ToVector2(), Color.Gray);

            en = _mouseMovePoint.Value;
            im.AddLine(st.ToVector2(), en.ToVector2(), Color.LightGray);
        }

        private void RenderCircleTypeNone(PerspectiveCamera camera, I2DRenderer im)
        {
            var center = Pivot;
            var origin = new Vector3(center.X, center.Y, center.Z);

            var distance = (camera.EyeLocation - origin).Length();
            if (distance <= 1) return;

            // Ensure points that can't be projected properly don't get rendered
            var screenOrigin = camera.WorldToScreen(origin);
            var sop = new PointF(screenOrigin.X, screenOrigin.Y);
            var rec = new RectangleF(-200, -200, camera.Width + 400, camera.Height + 400);
            if (!rec.Contains(sop)) return;

            var radius = 0.15f * distance;

            var normal = Vector3.Normalize(Vector3.Subtract(camera.EyeLocation, origin));
            var right = Vector3.Normalize(Vector3.Cross(normal, Vector3.UnitZ));
            var up = Vector3.Normalize(Vector3.Cross(normal, right));

            const int sides = 32;
            const float diff = (float)(2 * Math.PI) / sides;

            for (var i = 0; i < sides; i++)
            {
                var cos1 = (float)Math.Cos(diff * i);
                var sin1 = (float)Math.Sin(diff * i);
                var cos2 = (float)Math.Cos(diff * (i + 1));
                var sin2 = (float)Math.Sin(diff * (i + 1));

                var line = new Line(
                    origin + right * cos1 * radius + up * sin1 * radius,
                    origin + right * cos2 * radius + up * sin2 * radius
                );

                var st = camera.WorldToScreen(line.Start);
                var en = camera.WorldToScreen(line.End);

                im.AddLine(st.ToVector2(), en.ToVector2(), Color.DarkGray);

                line = new Line(
                    origin + right * cos1 * radius * 1.2f + up * sin1 * radius * 1.2f,
                    origin + right * cos2 * radius * 1.2f + up * sin2 * radius * 1.2f
                );

                st = camera.WorldToScreen(line.Start);
                en = camera.WorldToScreen(line.End);

                var c = _mouseOver == AxisType.Outer ? Color.White : Color.LightGray;
                im.AddLine(st.ToVector2(), en.ToVector2(), c);
            }

            var plane = new Plane(normal, Vector3.Dot(origin, normal));

            for (var i = 0; i < sides; i++)
            {
                var cos1 = (float)Math.Cos(diff * i) * radius;
                var sin1 = (float)Math.Sin(diff * i) * radius;
                var cos2 = (float)Math.Cos(diff * (i + 1)) * radius;
                var sin2 = (float)Math.Sin(diff * (i + 1)) * radius;

                RenderLine(
                    (origin + Vector3.UnitX * cos1 + Vector3.UnitY * sin1),
                    (origin + Vector3.UnitX * cos2 + Vector3.UnitY * sin2),
                    plane,
                    _mouseOver == AxisType.Z ? Color.Blue : Color.DarkBlue,
                    camera, im);

                RenderLine(
                    (origin + Vector3.UnitY * cos1 + Vector3.UnitZ * sin1),
                    (origin + Vector3.UnitY * cos2 + Vector3.UnitZ * sin2),
                    plane,
                    _mouseOver == AxisType.X ? Color.Red : Color.DarkRed,
                    camera, im);

                RenderLine(
                    (origin + Vector3.UnitZ * cos1 + Vector3.UnitX * sin1),
                    (origin + Vector3.UnitZ * cos2 + Vector3.UnitX * sin2),
                    plane,
                    _mouseOver == AxisType.Y ? Color.Lime : Color.LimeGreen,
                    camera, im);
            }
        }
    }
}
