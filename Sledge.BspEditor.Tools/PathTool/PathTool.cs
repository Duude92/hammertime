using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.BspEditor.Tools.Draggable;
using Sledge.BspEditor.Tools.PathTool.Forms;
using Sledge.BspEditor.Tools.Properties;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Hotkeys;
using Sledge.Common.Translations;
using Sledge.DataStructures.Geometric;
using Sledge.Rendering.Cameras;
using Sledge.Shell.Input;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Sledge.BspEditor.Tools.Draggable.PathState;

namespace Sledge.BspEditor.Tools.PathTool
{
	[Export(typeof(ITool))]
	[OrderHint("S")]
	[AutoTranslate]
	[DefaultHotkey("Shift+P")]
	public class PathTool : BaseDraggableTool
	{
		private BoxDraggableState box;

		private bool _shiftPressed;
		private bool _controlPressed;
		private MapDocument _lastDocument;
		public PathTool()
		{
			RenderedByDefault = true;
			box = new BoxDraggableState(this);
			box.BoxColour = Color.Turquoise;
			box.FillColour = Color.FromArgb(1, Color.Aqua);
			//box.State.Changed += BoxChanged;
			States.Add(box);
			Oy.Subscribe<MapDocument>("Document:Activated", document =>
			{
				_lastDocument = document;
				var states = States.OfType<PathState>();
				States.RemoveAll(state => states.Contains(state));
				var path = document.Map.Root.Data.Get<Path>();
				States.InsertRange(0, path.Select(p => new PathState(this)
				{
					Property = new PathProperty
					{
						Name = p.Name,
						Direction = p.Direction,
						ClassName = p.Type
					}
				}.AddRange(p.Nodes.Select(n => new SphereHandle(n.Position, this)
				{
					ID = n.ID,
				}))));
			});
		}
		public override Task ToolDeselected()
		{
			var states = States.OfType<PathState>().Where(p => p.Handles.Any());
			var path = _lastDocument.Map.Root.Data.Get<Path>();
			_lastDocument.Map.Root.Data.Remove(data => path.Contains(data));
			_lastDocument.Map.Root.Data.AddRange(states.Select(p => new Path
			{
				Direction = p.Property.Direction,
				Name = p.Property.Name,
				Type = p.Property.ClassName,
				Nodes = p.Handles.Select(h => new Path.PathNode
				{
					ID = h.ID.HasValue ? h.ID.Value : 0,
					Name = h.Name,
					Position = h.Origin,
					Properties = h.Properties
				}).ToList()
			}));

			return base.ToolDeselected();
		}
		protected override IEnumerable<Subscription> Subscribe()
		{
			yield return Oy.Subscribe<PathProperty>("PathTool:SavePathProperties", p => ApplyPathSettings(p));
		}
		private void ApplyPathSettings(PathProperty property)
		{
			var hl = States.OfType<PathState>().Where(s => s.IsSelected).ToList();
			hl.First().Property = property;
		}

		public override Image GetIcon() => Resources.Tool_VM;

		public override string GetName() => "Path Tool";
		protected override void MouseDown(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e)
		{
			if (e.Button == MouseButtons.Left)
			{
			if (_shiftPressed)
			{
				if (e.Button != MouseButtons.Left && e.Button != MouseButtons.Right) return;
				var hl = States.OfType<PathState>().Where(s => s.IsSelected).ToList();
				var loc = SnapIfNeeded(camera.ScreenToWorld(e.X, e.Y));

				if (!hl.Any())
				{
					PathProperties dialog = new PathProperties(loc);
						_shiftPressed = false;
						var state = new PathState(loc, this);
						States.Insert(0, state);
						state.Head.IsSelected = true;

					var result = dialog.ShowDialog();
					if (result == DialogResult.Cancel) return;

					return;
				}

				if (hl.Count > 1) return; // Ignore on multiple selected
				var parent = hl.FirstOrDefault();
				if (parent?.Next) return; // Ignore if selected node has Next item

				parent.AddNode(loc);
			}
			else
			{
					var toggle = _controlPressed;

					List<SphereHandle> clicked = GetClicked(viewport, camera, e);

					Select(clicked, toggle);

				}
			}
			base.MouseDown(document, viewport, camera, e);
		}

		private List<SphereHandle> GetClicked(MapViewport viewport, OrthographicCamera camera, ViewportEvent e)
		{
				var spheres = States.OfType<PathState>();

				var l = camera.ScreenToWorld(e.X, e.Y);
				var pos = l;
				var p = new Vector3(e.X, e.Y, 0);

				const int d = 5;

			List<SphereHandle> clicked = GetClickedAt(viewport, spheres, pos, p, d);
			return clicked;
		}

		private static List<SphereHandle> GetClickedAt(MapViewport viewport, IEnumerable<PathState> spheres, Vector3 pos, Vector3 p, int d)
		{
			return (from point in spheres.SelectMany(s => s.Handles)
							   let c = viewport.Viewport.Camera.WorldToScreen(point.Origin)
							   where c.Z <= 1
							   where p.X >= c.X - d && p.X <= c.X + d && p.Y >= c.Y - d && p.Y <= c.Y + d
							   orderby (pos - point.Origin).LengthSquared()
							   select point).ToList();

				Select(clicked, toggle);

			}
			base.MouseDown(document, viewport, camera, e);
		}

		private void Select(List<SphereHandle> points, bool toggle)
		{
			var spheres = States.OfType<PathState>().ToList();
			spheres.SelectMany(s => s.Handles).ToList().ForEach(x => x.IsSelected = (points.Contains(x) || (toggle && x.IsSelected)));
		}

		protected override void KeyDown(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e)
		{
			_shiftPressed = e.KeyCode == Keys.ShiftKey;
			_controlPressed = e.KeyCode == Keys.ControlKey;
			if (e.KeyCode == Keys.Enter) ConfirmSelection(document, viewport);

			base.KeyDown(document, viewport, camera, e);

			var nudge = GetNudgeValue(e.KeyCode);

			if (nudge != null && box.State.Action == BoxAction.Drawn)
			{
				var translate = camera.Expand(nudge.Value);
				var transformation = Matrix4x4.CreateTranslation(translate.X, translate.Y, translate.Z);
				var matrix = transformation;
				box.State.Start += translate;
				box.State.End += translate;
			}
		}


		private void ConfirmSelection(MapDocument document, MapViewport viewport)
		{
			if (box.State.Action != BoxAction.Drawn) return;
			var bbox = box.State.GetSelectionBox();
			if (bbox != null && !bbox.IsEmpty())
			{
				SelectPointsInBox(bbox, KeyboardState.Ctrl);
				box.RememberedDimensions = bbox;
			}
			box.State.Action = BoxAction.Idle;

		}
		public bool SelectPointsInBox(Box box, bool toggle)
		{
			var inBox = States.OfType<PathState>().SelectMany(s => s.Handles).Where(x => box.Vector3IsInside(x.Origin)).ToList();
			Select(inBox, toggle);
			return inBox.Any();
		}
		protected override void KeyUp(MapDocument document, MapViewport viewport, PerspectiveCamera camera, ViewportEvent e)
		{
			base.KeyUp(document, viewport, camera, e);
		}
		protected override void KeyUp(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e)
		{
			_shiftPressed = (!(e.KeyCode == Keys.ShiftKey) && _shiftPressed);
			_controlPressed = (!(e.KeyCode == Keys.ControlKey) && _controlPressed);

			base.KeyUp(document, viewport, camera, e);
		}

		protected override void DragStart(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e)
		{
			base.DragStart(document, viewport, camera, e);
		}
	}
}
