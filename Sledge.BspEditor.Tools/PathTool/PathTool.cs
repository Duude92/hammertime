using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations.Data;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.BspEditor.Tools.Draggable;
using Sledge.BspEditor.Tools.Properties;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Hotkeys;
using Sledge.Common.Translations;
using Sledge.DataStructures.Geometric;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Overlay;
using Sledge.Rendering.Viewports;
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
		public override Image GetIcon() => Resources.Tool_Path;
		public override string GetName() => "Path Tool";

		public PathTool()
		{
			Usage = ToolUsage.Both;
			RenderedByDefault = true;
			box = new BoxDraggableState(this);
			box.BoxColour = Color.Turquoise;
			box.FillColour = Color.FromArgb(1, Color.Aqua);
			States.Add(box);
			Oy.Subscribe<MapDocument>("Document:Activated", document =>
			{
				_lastDocument = document;
				UpdateNodes();
			});
			Oy.Subscribe<Change>("MapDocument:Changed:Late", change =>
			{
				UpdateNodes();
			});
		}
		public override Task ToolSelected()
		{
			UpdateNodes();
			return base.ToolSelected();
		}

		private void UpdateNodes()
		{
			var states = States.OfType<PathState>();
			States.RemoveAll(state => states.Contains(state));
			var path = _lastDocument.Map.Root.Data.Get<Path>();
			States.InsertRange(0, path.Select(p => new PathState(this)
			{
				Property = new PathProperty
				{
					Name = p.Name,
					Direction = p.Direction,
					ClassName = p.Type
				}
			}.AddRange(p.Nodes.Select(n => new PathNodeHandle(n.Position, this)
			{
				ID = n.ID,
				Name = n.Name,
				Properties = n.Properties,
			}))));
		}

		public override async Task ToolDeselected()
		{
			var transaction = new Transaction();

			box.State.Action = BoxAction.Idle;

			var states = States.OfType<PathState>().Where(p => p.Handles.Any());

			var path = _lastDocument.Map.Root.Data.Get<Path>();


			transaction.Add(new RemoveMapObjectData(_lastDocument.Map.Root.ID, path));

			var newPath = (states.Select(p => new Path
			{
				Direction = p.Property.Direction,
				Name = p.Property.Name,
				Type = p.Property.ClassName,
				Nodes = p.Handles.Select((h, count) => new Path.PathNode
				{
					ID = count,
					Name = h.Name,
					Position = h.Origin,
					Properties = h.Properties
				}).ToList()
			}));
			transaction.Add(new AddMapObjectData(_lastDocument.Map.Root.ID, newPath));

			await MapDocumentOperation.Perform(_lastDocument, transaction);

			await base.ToolDeselected();
		}
		protected override IEnumerable<Subscription> Subscribe()
		{
			yield break;
		}
		protected override void MouseDown(MapDocument document, MapViewport viewport, PerspectiveCamera camera, ViewportEvent e)
		{
			if (e.Button == MouseButtons.Left)
			{
				var clicked = GetClicked(viewport, camera, e);
				Select(clicked, false);

			}
			base.MouseDown(document, viewport, camera, e);
		}
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
						_shiftPressed = false;
						var state = new PathState(loc, this);
						Oy.Publish("Command:Run", new CommandMessage("BspEditor:PathProperties", new { state }));

						States.Insert(0, state);
						state.Head.IsSelected = true;
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

					List<PathNodeHandle> clicked = GetClicked(viewport, camera, e);

					Select(clicked, toggle);

				}
			}
			base.MouseDown(document, viewport, camera, e);
		}

		private List<PathNodeHandle> GetClicked(MapViewport viewport, PerspectiveCamera camera, ViewportEvent e)
		{
			var l = camera.EyeLocation;
			var pos = new Vector3((float)l.X, (float)l.Y, (float)l.Z);
			var p = new Vector3(e.X, e.Y, 0);
			const int d = 5;
			var clicked = (from point in States.OfType<PathState>().SelectMany(p=>p.Handles)
						   let c = viewport.Viewport.Camera.WorldToScreen(point.Origin)
						   where c.Z <= 1
						   where p.X >= c.X - d && p.X <= c.X + d && p.Y >= c.Y - d && p.Y <= c.Y + d
						   orderby (pos - point.Origin).LengthSquared()
						   select point).ToList();

			return clicked.ToList();

		}
		private List<PathNodeHandle> GetClicked(MapViewport viewport, OrthographicCamera camera, ViewportEvent e)
		{
			var spheres = States.OfType<PathState>();

			var l = camera.ScreenToWorld(e.X, e.Y);
			var pos = l;
			var p = new Vector3(e.X, e.Y, 0);

			const int d = 5;

			List<PathNodeHandle> clicked = GetClickedAt(viewport, spheres, pos, p, d);
			return clicked;
		}

		private static List<PathNodeHandle> GetClickedAt(MapViewport viewport, IEnumerable<PathState> spheres, Vector3 pos, Vector3 p, int d)
		{
			return (from point in spheres.SelectMany(s => s.Handles)
					let c = viewport.Viewport.Camera.WorldToScreen(point.Origin)
					where c.Z <= 1
					where p.X >= c.X - d && p.X <= c.X + d && p.Y >= c.Y - d && p.Y <= c.Y + d
					orderby (pos - point.Origin).LengthSquared()
					select point).ToList();
		}

		private void Select(List<PathNodeHandle> points, bool toggle)
		{
			var spheres = States.OfType<PathState>().ToList();
			spheres.SelectMany(s => s.Handles).ToList().ForEach(x => x.IsSelected = points.Contains(x) ? !x.IsSelected : (toggle && x.IsSelected));
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
