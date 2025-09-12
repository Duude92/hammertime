using System;
using System.Linq;
using System.Numerics;
using Avalonia.Input;
using LogicAndTrick.Oy;
using Sledge.Rendering.Cameras;
using Sledge.Shell;
using Sledge.Shell.Input;

namespace Sledge.BspEditor.Rendering.Viewport
{
	public class OrthographicCameraViewportListener : IViewportEventListener
	{
		public string OrderHint => "W";
		public MapViewport Viewport { get; set; }

		private OrthographicCamera Camera => Viewport.Viewport.Camera as OrthographicCamera;

		public OrthographicCameraViewportListener(MapViewport viewport)
		{
			Viewport = viewport;
		}

		private const float ScrollStart = 1;
		private const float ScrollIncrement = 0.025f;
		private const int ScrollMaximum = 200;
		private const int ScrollPadding = 40;

		public void UpdateFrame(long frame)
		{
			Viewport.Control.InvokeLater(() =>
			{
				if (Viewport.Viewport.IsFocused && _mouseDown != null && KeyboardState.MouseButtons.HasFlag(MouseButton.Left) && !KeyboardState.IsKeyDown(Key.Space))
				{
					var pt = _mousePosition;// Viewport.Control.PointToClient(Control.MousePosition);
					var pos = Camera.Position;
					if (pt.X < ScrollPadding)
					{
						var mx = ScrollStart + ScrollIncrement * Math.Min(ScrollMaximum, ScrollPadding - pt.X);
						mx = mx * mx + ScrollStart;
						pos.X -= mx / Camera.Zoom;
					}
					else if (pt.X > Viewport.Control.Bounds.Width - ScrollPadding)
					{
						var mx = ScrollStart + ScrollIncrement * Math.Min(ScrollMaximum, pt.X - (Viewport.Control.Bounds.Width - ScrollPadding));
						mx = mx * mx + ScrollStart;
						pos.X += (float)mx / Camera.Zoom;
					}
					if (pt.Y < ScrollPadding)
					{
						var my = ScrollStart + ScrollIncrement * Math.Min(ScrollMaximum, ScrollPadding - pt.Y);
						my = my * my + ScrollStart;
						pos.Y += my / Camera.Zoom;
					}
					else if (pt.Y > Viewport.Control.Bounds.Height - ScrollPadding)
					{
						var my = ScrollStart + ScrollIncrement * Math.Min(ScrollMaximum, pt.Y - (Viewport.Control.Bounds.Height - ScrollPadding));
						my = my * my + ScrollStart;
						pos.Y -= (float)my / Camera.Zoom;
					}
					Camera.Position = pos;
				}
			});
		}

		public bool IsActive()
		{
			return Viewport != null && Camera != null;
		}

		public void KeyUp(ViewportEvent e)
		{
			if (e.KeyCode == Key.Space)
			{
				//Viewport.Control.Cursor = Cursors.Default;
				//Viewport.Control.Capture = false;
				e.Handled = true;
			}
		}

		public void KeyDown(ViewportEvent e)
		{
			if (e.KeyCode == Key.Space)
			{
				//Viewport.Control.Cursor = Cursors.SizeAll;
				if (!CameraNavigationViewportSettings.Camera2DPanRequiresMouseClick)
				{
					//Viewport.Control.Capture = true;
					//var p = e.Sender.Control.PointToClient(Cursor.Position);
					//_mouseDown = new Vector3(p.X, Viewport.Height - p.Y, 0);
				}
				e.Handled = true;
			}

			if (KeyboardState.Shift)
			{
				var shift = new Vector3(0, 0, 0);

				switch (e.KeyCode)
				{
					case Key.Left:
						shift.X = -Viewport.Width / Camera.Zoom / 4;
						break;
					case Key.Right:
						shift.X = Viewport.Width / Camera.Zoom / 4;
						break;
					case Key.Up:
						shift.Y = Viewport.Height / Camera.Zoom / 4;
						break;
					case Key.Down:
						shift.Y = -Viewport.Height / Camera.Zoom / 4;
						break;
				}

				Camera.Position += shift;
			}

			var str = e.KeyCode.ToString();
			if (str.StartsWith("NumPad") || str.StartsWith("D"))
			{
				var last = str.Last();
				if (Char.IsDigit(last))
				{
					var press = (int)Char.GetNumericValue(last);
					if (press >= 0 && press <= 9)
					{
						if (press == 0) press = 10;
						var num = Math.Max(press - 6, 6 - press);
						var pow = (float)Math.Pow(2, num);
						var zoom = press < 6 ? 1 / pow : pow;
						Camera.Zoom = zoom;
						Oy.Publish("MapDocument:ViewportZoomStatus:UpdateValue", Camera.Zoom);
					}
				}
			}
		}

		public void MouseMove(ViewportEvent e)
		{
			_mousePosition = new Vector2(e.X, e.Y);
			var lmouse = false;
			var mmouse = false;
			//var lmouse = Control.MouseButton.HasFlag(MouseButton.Left);
			//       var mmouse = Control.MouseButton.HasFlag(MouseButton.Middle);
			var space = KeyboardState.IsKeyDown(Key.Space);
			if (space || mmouse)
			{
				//Viewport.Control.Cursor = Cursors.SizeAll;
				if (lmouse || mmouse || !CameraNavigationViewportSettings.Camera2DPanRequiresMouseClick)
				{
					var point = new Vector3(e.X, Viewport.Height - e.Y, 0);
					if (_mouseDown != null)
					{
						var difference = _mouseDown.Value - point;
						Camera.Position += (difference / Camera.Zoom);
					}
					_mouseDown = point;
					e.Handled = true;
				}
			}

			Oy.Publish<Vector3?>("MapDocument:ViewportMouseLocationStatus:UpdateValue", Camera.ScreenToWorld(e.X, e.Y));
		}

		public void MouseWheel(ViewportEvent e)
		{
			var before = Camera.Flatten(Camera.ScreenToWorld(new Vector3(e.X, e.Y, 0)));
			Camera.Zoom *= (float)Math.Pow((double)CameraNavigationViewportSettings.MouseWheelZoomMultiplier, (e.Delta < 0 ? -1 : 1));
			var after = Camera.Flatten(Camera.ScreenToWorld(new Vector3(e.X, e.Y, 0)));
			Camera.Position -= (after - before);

			Oy.Publish("MapDocument:ViewportZoomStatus:UpdateValue", Camera.Zoom);
			//if (KeyboardState.IsKeyDown(Key.ControlKey))
			//{
			//    Oy.Publish("MapDocument:Viewport:SetZoom", Camera.Zoom);
			//}
		}

		public void MouseUp(ViewportEvent e)
		{
			var space = KeyboardState.IsKeyDown(Key.Space);
			var req = CameraNavigationViewportSettings.Camera2DPanRequiresMouseClick;
			if (space && (!req || e.Button == MouseButton.Left))
			{
				e.Handled = true;
			}
			else if (e.Button == MouseButton.Middle)
			{
				e.Handled = true;
				//Viewport.Control.Cursor = Cursors.Default;
			}
		}

		private Vector3? _mouseDown;
		private Vector2 _mousePosition;

		public void MouseDown(ViewportEvent e)
		{
			var space = KeyboardState.IsKeyDown(Key.Space);
			var req = CameraNavigationViewportSettings.Camera2DPanRequiresMouseClick;
			if (space && (!req || e.Button == MouseButton.Left))
			{
				e.Handled = true;
			}
			else if (e.Button == MouseButton.Middle)
			{
				e.Handled = true;
				//Viewport.Control.Cursor = Cursors.SizeAll;
			}
			_mouseDown = new Vector3(e.X, Viewport.Height - e.Y, 0);
		}

		public void MouseClick(ViewportEvent e)
		{

		}

		public void MouseDoubleClick(ViewportEvent e)
		{

		}

		public void DragStart(ViewportEvent e)
		{

		}

		public void DragMove(ViewportEvent e)
		{

		}

		public void DragEnd(ViewportEvent e)
		{

		}

		public void MouseEnter(ViewportEvent e)
		{
			if (KeyboardState.IsKeyDown(Key.Space))
			{
				//Viewport.Control.Cursor = Cursors.SizeAll;
			}

			Oy.Publish("MapDocument:ViewportZoomStatus:UpdateValue", Camera.Zoom);
		}

		public void MouseLeave(ViewportEvent e)
		{
			//Viewport.Control.Cursor = Cursors.Default;

			Oy.Publish("MapDocument:ViewportZoomStatus:UpdateValue", 0f);
			Oy.Publish<Vector3?>("MapDocument:ViewportMouseLocationStatus:UpdateValue", null);
		}

		public bool Filter(string hotkey, int keys)
		{
			return false;
		}

		public void Dispose()
		{
			// 
		}
	}
}
