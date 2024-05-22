using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.BspEditor.Tools.Draggable;
using Sledge.BspEditor.Tools.Properties;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Hotkeys;
using Sledge.Common.Shell.Settings;
using Sledge.Common.Translations;
using Sledge.Formats.Bsp.Lumps;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Renderables;
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

		private List<SpherePointState> _highlightedStates = new List<SpherePointState>();
		
		private bool _shiftPressed;

		public PathTool()
		{
			box = new BoxDraggableState(this);
			box.BoxColour = Color.Turquoise;
			box.FillColour = Color.FromArgb(1, Color.Aqua);
			//box.State.Changed += BoxChanged;
			States.Add(box);
		}
		public override Image GetIcon() => Resources.Tool_VM;

		public override string GetName() => "Path Tool";
		protected override void MouseDown(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e)
		{
			if (_shiftPressed)
			{
				if (e.Button != MouseButtons.Left && e.Button != MouseButtons.Right) return;


				var loc = SnapIfNeeded(camera.ScreenToWorld(e.X, e.Y));
				//_location = camera.GetUnusedCoordinate(_location) + loc;

				var state = new SpherePointState(loc);
				States.Add(state);
				if (_highlightedStates.Count == 1)
				{
					_highlightedStates.First().Next = state;
				}
				_highlightedStates.Add(state);
				//_sphereHandle = new SphereHandle(camera.Location);
			}
			else
			{
				var toggle = false;

				var spheres = States.OfType<SpherePointState>();

				var l = camera.ScreenToWorld(e.X, e.Y);

				var pos = new Vector3((float)l.X, (float)l.Y, (float)l.Z);
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
			if (!points.Any()) return;
			var first = points[0];
			points.ForEach(x => x.IsSelected = true);
		}

		protected override void KeyDown(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e)
		{
			_shiftPressed = e.KeyCode == Keys.ShiftKey;
			//if (e.KeyCode == Keys.Enter) Confirm(document, viewport);
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
		protected override void KeyUp(MapDocument document, MapViewport viewport, PerspectiveCamera camera, ViewportEvent e)
		{
			base.KeyUp(document, viewport, camera, e);
		}
		protected override void KeyUp(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e)
		{
			_shiftPressed = !((e.KeyCode == Keys.ShiftKey) && _shiftPressed);

			base.KeyUp(document, viewport, camera, e);
		}
	}
}
