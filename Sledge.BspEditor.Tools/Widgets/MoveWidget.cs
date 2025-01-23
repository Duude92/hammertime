using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Rendering.Resources;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.DataStructures.Geometric;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Overlay;
using Sledge.Rendering.Pipelines;
using Sledge.Rendering.Primitives;
using Sledge.Rendering.Resources;
using Sledge.Rendering.Viewports;
using System;
using System.Drawing;
using System.Numerics;
using Plane = Sledge.DataStructures.Geometric.Plane;

namespace Sledge.BspEditor.Tools.Widgets
{
	public class MoveWidget : Widget
	{
		public override bool IsUniformTransformation => true;

		public override bool IsScaleTransformation => false;

		public MoveWidget(MapDocument document)
		{
			SetDocument(document);
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
						RenderMoveTypeNone(camera, im);
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

		private void RenderMoveTypeNone(PerspectiveCamera camera, I2DRenderer im)
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

			var radius = 0.3f * distance;

			var normal = Vector3.Normalize(Vector3.Subtract(camera.EyeLocation, origin));
			var right = Vector3.Normalize(Vector3.Cross(normal, Vector3.UnitZ));
			var up = Vector3.Normalize(Vector3.Cross(normal, right));

			const int sides = 3;
			var plane = new Plane(normal, Vector3.Dot(origin, normal));

			for (var i = 0; i < sides; i++)
			{

				RenderLine(
					(origin - Vector3.UnitX),
					(origin + Vector3.UnitX * radius),
					plane,
					_mouseOver == AxisType.Z ? Color.Blue : Color.DarkBlue,
					camera, im);

				RenderLine(
					(origin - Vector3.UnitY),
					(origin + Vector3.UnitY * radius),
					plane,
					_mouseOver == AxisType.X ? Color.Red : Color.DarkRed,
					camera, im);

				RenderLine(
					(origin - Vector3.UnitZ),
					(origin + Vector3.UnitZ * radius),
					plane,
					_mouseOver == AxisType.Y ? Color.Lime : Color.LimeGreen,
					camera, im);
			}
		}

		protected override Matrix4x4? GetTransformationMatrix(MapViewport viewport)
		{
			throw new NotImplementedException();
		}

		protected override void UpdateCache(IViewport viewport, PerspectiveCamera camera)
		{
			throw new NotImplementedException();
		}

	}
}
