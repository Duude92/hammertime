using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Rendering.Resources;
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

		private Vector3? _mouseMovePoint;
		private Vector3 _pivotPoint = Vector3.Zero;
		private MoveType _mouseOver;
		private MoveType _mouseDown;
		private Vector3? _mouseDownPoint;
		private bool _autoPivot;

		public MoveWidget(MapDocument document)
		{
			SetDocument(document);
		}

		public override void SelectionChanged()
		{
			var document = GetDocument();
			if (document != null && document.Selection.IsEmpty) _autoPivot = true;
			if (!_autoPivot) return;

			var bb = document?.Selection.GetSelectionBoundingBox();
			_pivotPoint = bb?.Center ?? Vector3.Zero;
		}

		protected override void Render(MapDocument document, BufferBuilder builder, ResourceCollector resourceCollector)
		{
			if (_mouseMovePoint.HasValue && _mouseDown != MoveType.None)
			{
				var axis = Vector3.One;
				var c = Color.White;

				switch (_mouseDown)
				{
					case MoveType.X:
						axis = Vector3.UnitX;
						c = Color.Red;
						break;
					case MoveType.Y:
						axis = Vector3.UnitY;
						c = Color.Lime;
						break;
					case MoveType.Z:
						axis = Vector3.UnitZ;
						c = Color.Blue;
						break;
					case MoveType.Outer:
						if (ActiveViewport == null || !(ActiveViewport.Viewport.Camera is PerspectiveCamera pc)) return;
						axis = pc.Direction;
						c = Color.White;
						break;
				}

				var start = _pivotPoint - axis * 1024 * 1024;
				var end = _pivotPoint + axis * 1024 * 1024;

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
				switch (_mouseMovePoint == null ? MoveType.None : _mouseDown)
				{
					case MoveType.None:
						RenderMoveTypeNone(camera, im);
						break;
					case MoveType.Outer:
					case MoveType.X:
					case MoveType.Y:
					case MoveType.Z:
						RenderAxisRotating(viewport, camera, im);
						break;
				}
			}
			base.Render(document, viewport, camera, im);
		}
		private void RenderAxisRotating(IViewport viewport, PerspectiveCamera camera, I2DRenderer im)
		{
			if (ActiveViewport.Viewport != viewport || !_mouseDownPoint.HasValue || !_mouseMovePoint.HasValue) return;

			var st = camera.WorldToScreen(_pivotPoint);
			var en = _mouseDownPoint.Value;
			im.AddLine(st.ToVector2(), en.ToVector2(), Color.Gray);

			en = _mouseMovePoint.Value;
			im.AddLine(st.ToVector2(), en.ToVector2(), Color.LightGray);
		}

		private void RenderMoveTypeNone(PerspectiveCamera camera, I2DRenderer im)
		{
			var center = _pivotPoint;
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
					_mouseOver == MoveType.Z ? Color.Blue : Color.DarkBlue,
					camera, im);

				RenderLine(
					(origin - Vector3.UnitY),
					(origin + Vector3.UnitY * radius),
					plane,
					_mouseOver == MoveType.X ? Color.Red : Color.DarkRed,
					camera, im);

				RenderLine(
					(origin - Vector3.UnitZ),
					(origin + Vector3.UnitZ * radius),
					plane,
					_mouseOver == MoveType.Y ? Color.Lime : Color.LimeGreen,
					camera, im);
			}
		}

		private void RenderLine(Vector3 start, Vector3 end, Plane plane, Color color, ICamera camera, I2DRenderer im)
		{
			var line = new Line(start, end);
			var cls = line.ClassifyAgainstPlane(plane);
			if (cls == PlaneClassification.Back) return;
			if (cls == PlaneClassification.Spanning)
			{
				var isect = plane.GetIntersectionPoint(line, true);
				var first = plane.OnPlane(line.Start) > 0 ? line.Start : line.End;
				if (!isect.HasValue) return;
				line = new Line(first, isect.Value);
			}

			var st = camera.WorldToScreen(line.Start);
			var en = camera.WorldToScreen(line.End);

			im.AddLine(st.ToVector2(), en.ToVector2(), color, 2);
		}


		private enum MoveType
		{
			None,
			Outer,
			X,
			Y,
			Z
		}


	}
}
