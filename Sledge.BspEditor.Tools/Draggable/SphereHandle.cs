using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.DataStructures.Geometric;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Overlay;
using Sledge.Rendering.Resources;
using Sledge.Rendering.Viewports;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.AxHost;

namespace Sledge.BspEditor.Tools.Draggable
{
	public class SphereHandle : BaseDraggable
	{
		private Vector3 _position;
		public override Vector3 Origin => _position;
		public bool IsSelected { get; set; }
		public bool IsDragging { get; set; } = false;
		public bool IsHighlighted { get; private set; }

		public SphereHandle(Vector3 position)
		{
			_position = position;
		}

		public override bool CanDrag(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 position)
		{
			const int width = 5;
			var screenPosition = camera.WorldToScreen(_position);
			var diff = (e.Location - screenPosition).Absolute();
			return diff.X < width && diff.Y < width;
		}

		public override void Click(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 position)
		{
		}

		public override void Highlight(MapDocument document, MapViewport viewport)
		{
			IsHighlighted = true;
			viewport.Control.Cursor = Cursors.SizeAll;

		}
		public override void Unhighlight(MapDocument document, MapViewport viewport)
		{
			IsHighlighted = false;
			viewport.Control.Cursor = Cursors.Default;

		}

		public override void Render(MapDocument document, BufferBuilder builder)
		{
			return;
		}
		public void MoveTo(Vector3 position)
		{
			_position = position;
		}
		protected (Vector3, Vector3) GetWorldPositionAndScreenOffset(ICamera camera)
		{
			const int distance = 6;
			var mid = camera.Flatten(_position);
			Vector3 center;
			Vector3 offset;
			center = new Vector3(mid.X, mid.Y, 0);
			offset = new Vector3(distance, distance, 0);

			return (camera.Expand(center), offset);
		}

		public override void Render(IViewport viewport, OrthographicCamera camera, Vector3 worldMin, Vector3 worldMax, I2DRenderer im)
		{
			//if (State.State.Action != BoxAction.Drawn) return;

			var (wpos, soff) = GetWorldPositionAndScreenOffset(camera);
			var spos = camera.WorldToScreen(wpos) + soff;

			const int size = 4;

			const int boxOffset = 3;

			im.AddRectFilled(new Vector2(spos.X - size - boxOffset, spos.Y - size - boxOffset), new Vector2(spos.X - size + boxOffset, spos.Y - size + boxOffset), IsSelected ? Color.Red : Color.Bisque);
		}

		public override void Render(IViewport viewport, PerspectiveCamera camera, I2DRenderer im)
		{
			return;
		}

		public override void StartDrag(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 position)
		{
			IsDragging = true;
			base.StartDrag(document, viewport, camera, e, position);
		}
		public override void Drag(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 lastPosition, Vector3 position)
		{
			if (IsDragging)
			{
				_position = camera.Expand( position);
			}
			base.Drag(document, viewport, camera, e, lastPosition, position);
		}
		public override void EndDrag(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 position)
		{
			IsDragging = false;	
			base.EndDrag(document, viewport, camera, e, position);
		}
	}
}
