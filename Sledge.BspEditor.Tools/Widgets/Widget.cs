using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Windows.Forms;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.DataStructures.Geometric;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Overlay;
using Sledge.Rendering.Viewports;
using static Sledge.BspEditor.Tools.Selection.SelectionBoxDraggableState;
using Plane = Sledge.DataStructures.Geometric.Plane;


namespace Sledge.BspEditor.Tools.Widgets
{
	public abstract class Widget : BaseTool
	{
		protected bool _autoPivot;
		protected AxisType _mouseDown;
		protected Vector3? _mouseDownPoint;
		protected Vector3? _mouseMovePoint;
		protected AxisType _mouseOver;
		protected readonly List<CachedLines> _cachedLines = new List<CachedLines>();
		public abstract TransformationMode WidgetTransformationMode { get; }

		public class CachedLines
		{
			public int Width { get; set; }
			public int Height { get; set; }
			public Vector3 CameraLocation { get; set; }
			public Vector3 CameraLookAt { get; set; }
			public Vector3 PivotPoint { get; set; }
			public IViewport Viewport { get; set; }
			public Dictionary<AxisType, List<Line>> Cache { get; set; }

			public CachedLines(IViewport viewport)
			{
				Viewport = viewport;
				Cache = new Dictionary<AxisType, List<Line>>
				{
					{AxisType.Outer, new List<Line>()},
					{AxisType.X, new List<Line>()},
					{AxisType.Y, new List<Line>()},
					{AxisType.Z, new List<Line>()}
				};
			}
		}

		public Vector3 Pivot { get; private set; }

		protected MapViewport ActiveViewport { get; private set; }

		public delegate void TransformEventHandler(Widget sender, Matrix4x4? transformation);
		public event TransformEventHandler Transforming;
		public event TransformEventHandler Transformed;

		public abstract bool IsUniformTransformation { get; }
		public abstract bool IsScaleTransformation { get; }

		public void SetViewport(MapViewport viewport)
		{
			if (viewport?.Viewport.Camera is PerspectiveCamera)
				ActiveViewport = viewport;
		}

		public Vector3 GetPivotPoint()
		{
			return Pivot;
		}

		public void SetPivotPoint(Vector3 point)
		{
			Pivot = point;
		}

		protected void OnTransforming(Matrix4x4? transformation)
		{
			Transforming?.Invoke(this, transformation);
		}

		protected void OnTransformed(Matrix4x4? transformation)
		{
			Transformed?.Invoke(this, transformation);
		}

		public override Image GetIcon() { return null; }
		public override string GetName() { return "Widget"; }

		protected override void MouseEnter(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e)
		{
			ActiveViewport = viewport;
			base.MouseEnter(document, viewport, camera, e);
		}

		protected override void MouseEnter(MapDocument document, MapViewport viewport, PerspectiveCamera camera, ViewportEvent e)
		{
			ActiveViewport = viewport;
			base.MouseEnter(document, viewport, camera, e);
		}

		protected override void MouseLeave(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e)
		{
			ActiveViewport = null;
			base.MouseLeave(document, viewport, camera, e);
		}

		protected override void MouseLeave(MapDocument document, MapViewport viewport, PerspectiveCamera camera, ViewportEvent e)
		{
			ActiveViewport = null;
			viewport.Control.Cursor = Cursors.Default;

			base.MouseLeave(document, viewport, camera, e);
		}

		public virtual void SelectionChanged()
		{
			var document = GetDocument();
			if (document != null && document.Selection.IsEmpty) _autoPivot = true;
			if (!_autoPivot) return;

			var bb = document?.Selection.GetSelectionBoundingBox();
			Pivot = bb?.Center ?? Vector3.Zero;
		}
		protected void RenderLine(Vector3 start, Vector3 end, Plane plane, Color color, ICamera camera, I2DRenderer im)
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


		private bool MouseOver(AxisType type, ViewportEvent ev, MapViewport viewport)
		{
			var cache = _cachedLines.FirstOrDefault(x => x.Viewport == viewport.Viewport);
			if (cache == null) return false;
			var lines = cache.Cache[type];
			var point = new Vector3(ev.X, ev.Y, 0);
			return lines.Any(x => (x.ClosestPoint(point) - point).Length() <= 8);
		}



		protected override void MouseMove(MapDocument document, MapViewport viewport, PerspectiveCamera camera, ViewportEvent e)
		{
			if (viewport != ActiveViewport) return;

			if (document.Selection.IsEmpty || !viewport.IsUnlocked(this)) return;

			if (_mouseDown != AxisType.None)
			{
				_mouseMovePoint = new Vector3(e.X, e.Y, 0);
				e.Handled = true;
				var tform = GetTransformationMatrix(viewport);
				OnTransforming(tform);
			}
			else
			{
				UpdateCache(viewport.Viewport, camera);

				if (MouseOver(AxisType.Z, e, viewport)) _mouseOver = AxisType.Z;
				else if (MouseOver(AxisType.Y, e, viewport)) _mouseOver = AxisType.Y;
				else if (MouseOver(AxisType.X, e, viewport)) _mouseOver = AxisType.X;
				else if (MouseOver(AxisType.Outer, e, viewport)) _mouseOver = AxisType.Outer;
				else _mouseOver = AxisType.None;
			}
		}

		protected override void MouseDown(MapDocument document, MapViewport viewport, PerspectiveCamera camera, ViewportEvent e)
		{
			if (viewport != ActiveViewport) return;

			if (e.Button != MouseButtons.Left || _mouseOver == AxisType.None) return;
			_mouseDown = _mouseOver;
			_mouseDownPoint = new Vector3(e.X, e.Y, 0);
			_mouseMovePoint = null;
			e.Handled = true;
			viewport.AquireInputLock(this);
		}

		protected override void MouseUp(MapDocument document, MapViewport viewport, PerspectiveCamera camera, ViewportEvent e)
		{
			if (viewport != ActiveViewport) return;

			if (_mouseDown != AxisType.None && _mouseMovePoint != null) e.Handled = true;

			var transformation = GetTransformationMatrix(viewport);
			OnTransformed(transformation);
			_mouseDown = AxisType.None;
			_mouseMovePoint = null;
			viewport.ReleaseInputLock(this);
		}

		protected override void MouseWheel(MapDocument document, MapViewport viewport, PerspectiveCamera camera, ViewportEvent e)
		{
			if (viewport != ActiveViewport) return;
			if (_mouseDown != AxisType.None) e.Handled = true;
		}

		protected abstract Matrix4x4? GetTransformationMatrix(MapViewport viewport);
		protected abstract void UpdateCache(IViewport viewport, PerspectiveCamera camera);

		protected void AddLine(Widget.AxisType type, Vector3 start, Vector3 end, Plane test, CachedLines cache)
		{
			var line = new Line(start, end);
			var cls = line.ClassifyAgainstPlane(test);
			if (cls == PlaneClassification.Back) return;
			if (cls == PlaneClassification.Spanning)
			{
				var isect = test.GetIntersectionPoint(line, true);
				var first = test.OnPlane(line.Start) > 0 ? line.Start : line.End;
				if (isect.HasValue) line = new Line(first, isect.Value);
			}
			cache.Cache[type].Add(new Line(cache.Viewport.Camera.WorldToScreen(line.Start), cache.Viewport.Camera.WorldToScreen(line.End)));
		}
		public enum AxisType
		{
			None,
			Outer,
			X,
			Y,
			Z
		}

	}
}