using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.BspEditor.Tools.Draggable;
using Sledge.BspEditor.Tools.PathTool.Forms;
using Sledge.BspEditor.Tools.Properties;
using Sledge.BspEditor.Tools.Vertex;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Hotkeys;
using Sledge.Common.Shell.Settings;
using Sledge.Common.Translations;
using Sledge.DataStructures.Geometric;
using Sledge.Formats.Bsp.Lumps;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Renderables;
using Sledge.Shell.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Windows.Forms;
using static System.Windows.Forms.AxHost;

namespace Sledge.BspEditor.Tools.PathTool
{
	[Export(typeof(ITool))]
	[OrderHint("H")]
	[AutoTranslate]
	[DefaultHotkey("Shift+P")]
	public class PathTool : BaseDraggableTool
	{
		private BoxDraggableState box;

		private bool _shiftPressed;

		public PathTool()
		{
			box = new BoxDraggableState(this);
			box.BoxColour = Color.Turquoise;
			box.FillColour = Color.FromArgb(1, Color.Aqua);
			//box.State.Changed += BoxChanged;
			States.Add(box);
		}
		protected override IEnumerable<Subscription> Subscribe()
		{
			yield return Oy.Subscribe<PathProperty>("PathTool:NewPath", p => CreatePath(p));

		}

		private void CreatePath(PathProperty property)
		{
			var loc = property.Position;
			var state = new SpherePointState(loc);
			States.Insert(0, state);
			state.IsSelected = true;
		}

		public override Image GetIcon() => Resources.Tool_VM;

		public override string GetName() => "Path Tool";
		protected override void MouseDown(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e)
		{
			if (_shiftPressed)
			{
				if (e.Button != MouseButtons.Left && e.Button != MouseButtons.Right) return;
				var hl = States.OfType<SpherePointState>().Where(s => s.IsSelected).ToList();
				var loc = SnapIfNeeded(camera.ScreenToWorld(e.X, e.Y));

				if (!hl.Any())
				{
					PathProperties dialog = new PathProperties(loc);
					var result = dialog.ShowDialog();
					if (result == DialogResult.Cancel) return;
					return;
				}

				if (hl.Count > 1) return; // Ignore on multiple selected
				var parent = hl.FirstOrDefault();
				if (parent?.Next) return; // Ignore if selected node has Next item

				var state = new SpherePointState(loc);
				if (parent)
				{
					parent.Next = state;
					parent.IsSelected = false;
				}
				States.Insert(0, state);
				state.IsSelected = true;
			}
			else
			{
				var toggle = false;

				var spheres = States.OfType<SpherePointState>();

				var l = camera.ScreenToWorld(e.X, e.Y);
				var pos = l;
				var p = new Vector3(e.X, e.Y, 0);

				const int d = 5;


				var clicked = (from point in spheres
							   let c = viewport.Viewport.Camera.WorldToScreen(point.Origin)
							   where c.Z <= 1
							   where p.X >= c.X - d && p.X <= c.X + d && p.Y >= c.Y - d && p.Y <= c.Y + d
							   orderby (pos - point.Origin).LengthSquared()
							   select point).ToList();



				Select(clicked, toggle);

			}
			base.MouseDown(document, viewport, camera, e);
		}

		private void Select(List<SpherePointState> points, bool toggle)
		{
			var spheres = States.OfType<SpherePointState>().ToList();
			spheres.ForEach(x => x.IsSelected = points.Contains(x));
		}

		protected override void KeyDown(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e)
		{
			_shiftPressed = e.KeyCode == Keys.ShiftKey;
			if (e.KeyCode == Keys.Enter) ConfirmSelection(document, viewport);
			else if (e.KeyCode == Keys.Back) DeleteSelection();
			//else if (e.KeyCode == Keys.Escape) Cancel(document, viewport);

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

		private void DeleteSelection()
		{
			States.RemoveAll(x => x is SpherePointState state && state.IsSelected);
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
			var inBox = States.OfType<SpherePointState>().Where(x => box.Vector3IsInside(x.Origin)).ToList();
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

			base.KeyUp(document, viewport, camera, e);
		}

		protected override void DragStart(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e)
		{
			base.DragStart(document, viewport, camera, e);
		}
		internal enum PathDirection
		{
			ONE_WAY,
			CIRCULAR,
			PING_PONG
		}
		internal struct PathProperty
		{
			public PathDirection Direction;
			public string Name;
			public string ClassName;
			public Vector3 Position;
		}
	}
}
