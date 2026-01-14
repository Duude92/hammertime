using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using LogicAndTrick.Oy;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Viewports;
using Sledge.Shell.Registers;

namespace Sledge.BspEditor.Rendering.Viewport
{
	public class MapViewport
	{
		public List<IViewportEventListener> Listeners { get; private set; }
		public IViewport Viewport { get; private set; }

		public Control Control => Viewport.Control;
		public int Height => (int)Control.Height;
		public int Width => (int)Control.Width;

		public bool Is2D => Viewport.Camera.Type == CameraType.Orthographic;
		public bool Is3D => Viewport.Camera.Type == CameraType.Perspective;

		#region Input Locking

		private object _inputLock;

		public bool IsUnlocked(object context)
		{
			return _inputLock == null || _inputLock == context;
		}

		public bool AquireInputLock(object context)
		{
			if (_inputLock == null) _inputLock = context;
			return _inputLock == context;
		}

		public bool ReleaseInputLock(object context)
		{
			if (_inputLock == context) _inputLock = null;
			return _inputLock == null;
		}

		#endregion

		public MapViewport(IViewport viewport)
		{
			Viewport = viewport;
			Listeners = new List<IViewportEventListener>();

			viewport.Control.PointerWheelChanged += OnMouseWheel;
			viewport.Control.PointerEntered += OnMouseEnter;
			viewport.Control.PointerExited += OnMouseLeave;
			viewport.Control.AttachedToLogicalTree += (s, e) =>
			{
				var top = e.Root as TopLevel;
				if (top != null)
				{
					//top.PointerPressed += OnTopPointerPressed;
					top.PointerMoved += (s1, e1) => OnMouseMove(s, e1);
					top.PointerReleased += (s1, e1) => OnMouseUp(s, e1);
					top.PointerPressed += (s1, e1) => OnMouseDown(s, e1);
					top.KeyDown += OnKeyDown;
					//top.MouseDoubleClick += OnMouseDoubleClick;
					top.KeyUp += OnKeyUp;
					//top.OnUpdate += OnUpdate;
				}
			};

			//viewport.Control.PointerMoved += OnMouseMove;
			//viewport.Control.MouseUp += OnMouseUp;
			//viewport.Control.MouseDown += OnMouseDown;
			//viewport.Control.MouseDoubleClick += OnMouseDoubleClick;
			//viewport.Control.KeyDown += OnKeyDown;
			//viewport.Control.KeyDown += (s, e) => { }
			//viewport.Control.KeyUp += OnKeyUp;
			viewport.OnUpdate += OnUpdate;
			Oy.Subscribe("BspEditor:Viewport:Paste", async () =>
			{

				if (viewport.IsFocused) await Oy.Publish<string>("BspEditor:Edit:PasteFromView", (Viewport.Camera is OrthographicCamera camera) ?
					camera.ViewType == OrthographicCamera.OrthographicType.Top ? "Z" :
					camera.ViewType == OrthographicCamera.OrthographicType.Front ? "X" : "Y" : "3D");
			});
		}

		#region Listeners

		public delegate void ListenerExceptionEventHandler(object sender, Exception exception);
		public event ListenerExceptionEventHandler ListenerException;

		private void OnListenerException(Exception ex)
		{
			if (ListenerException != null)
			{
				var st = new StackTrace();
				var frames = st.GetFrames() ?? new StackFrame[0];
				var msg = "Listener exception: " + ex.Message;
				foreach (var frame in frames)
				{
					var method = frame.GetMethod();
					msg += "\r\n    " + method.ReflectedType.FullName + "." + method.Name;
				}
				ListenerException(this, new Exception(msg, ex));
			}
		}

		private void ListenerDo(Action<IViewportEventListener> action)
		{
			foreach (var listener in Listeners.Where(x => x.IsActive()).OrderBy(x => x.OrderHint))
			{
				try
				{
					action(listener);
				}
				catch (Exception ex)
				{
					OnListenerException(ex);
				}
			}
		}

		private void ListenerDoEvent(ViewportEvent e, Action<IViewportEventListener, ViewportEvent> action)
		{
			foreach (var listener in Listeners.Where(x => x.IsActive()).OrderBy(x => x.OrderHint))
			{
				try
				{
					action(listener, e);
				}
				catch (Exception ex)
				{
					OnListenerException(ex);
				}
				if (e.Handled)
				{
					break;
				}
			}
		}


		private void OnMouseWheel(object sender, PointerWheelEventArgs e)
		{
			ListenerDoEvent(new ViewportEvent(this, e), (l, v) => l.MouseWheel(v));
		}

		private void OnMouseEnter(object sender, EventArgs e)
		{
			if (!DialogRegister.IsAnyDialogFocused())
				Viewport.Control.Focus();

			ListenerDoEvent(new ViewportEvent(this, e), (l, v) => l.MouseEnter(v));
		}

		private void OnMouseLeave(object sender, EventArgs e)
		{
			ListenerDoEvent(new ViewportEvent(this, e), (l, v) => l.MouseLeave(v));
			_lastMouseLocationKnown = false;
			_lastMouseLocation = new Point(-1, -1);
		}

		private bool _dragging = false;
		private MouseButton _dragButton;
		private bool _lastMouseLocationKnown = false;
		private Point _lastMouseLocation = new Point(-1, -1);
		private Point _mouseDownLocation = new Point(-1, -1);

		private void OnMouseMove(object sender, PointerEventArgs ev)
		{
			var e = ev.GetPosition(sender as Control);
			if (!_lastMouseLocationKnown)
			{
				_lastMouseLocation = new Point(e.X, e.Y);
			}
			var ve = new ViewportEvent(this, ev)
			{
				Dragging = _dragging,
				StartX = (int)_mouseDownLocation.X,
				StartY = (int)_mouseDownLocation.Y,
				LastX = (int)_lastMouseLocation.X,
				LastY = (int)_lastMouseLocation.Y,
			};
			if (!_dragging
				&& (Math.Abs(_mouseDownLocation.X - e.X) > 1
					|| Math.Abs(_mouseDownLocation.Y - e.Y) > 1)
				&& _mouseDownLocation.X >= 0 && _mouseDownLocation.Y >= 0)
			{
				_dragging = ve.Dragging = true;
				ve.Button = _dragButton;
				ListenerDoEvent(ve, (l, v) => l.DragStart(v));
			}
			ListenerDoEvent(ve, (l, v) => l.MouseMove(v));
			if (_dragging)
			{
				ve.Button = _dragButton;
				ListenerDoEvent(ve, (l, v) => l.DragMove(v));
			}
			_lastMouseLocationKnown = true;
			_lastMouseLocation = new Point(e.X, e.Y);
		}

		private void OnMouseUp(object sender, PointerReleasedEventArgs ev)
		{
			var e = ev.GetPosition(sender as Control);
			if (!_lastMouseLocationKnown)
			{
				_lastMouseLocation = new Point(e.X, e.Y);
			}
			var ve = new ViewportEvent(this, ev)
			{
				Dragging = _dragging,
				StartX = (int)_mouseDownLocation.X,
				StartY = (int)_mouseDownLocation.Y,
				LastX = (int)_lastMouseLocation.X,
				LastY = (int)_lastMouseLocation.Y,
			};
			if (_dragging && ve.Button == _dragButton)
			{
				ListenerDoEvent(ve, (l, v) => l.DragEnd(v));
			}
			ListenerDoEvent(ve, (l, v) => l.MouseUp(v));
			if (!_dragging
				&& Math.Abs(_mouseDownLocation.X - e.X) <= 1
				&& Math.Abs(_mouseDownLocation.Y - e.Y) <= 1)
			{
				// Mouse hasn't moved very much, trigger the click event
				ListenerDoEvent(ve, (l, v) => l.MouseClick(v));
			}
			if (_dragging && ve.Button == _dragButton)
			{
				_dragging = false;
			}
			if (!_dragging)
			{
				_mouseDownLocation = new Point(-1, -1);
			}
			_lastMouseLocationKnown = true;
			_lastMouseLocation = new Point(e.X, e.Y);
		}

		private void OnMouseDown(object sender, PointerPressedEventArgs ev)
		{
			var e = ev.GetPosition(sender as Control);
			Viewport.Control.Focus();

			if (!_lastMouseLocationKnown)
			{
				_lastMouseLocation = new Point(e.X, e.Y);
			}
			if (!_dragging)
			{
				_mouseDownLocation = new Point(e.X, e.Y);
				_dragging = false;
				_dragButton = ev.Properties.IsLeftButtonPressed ? MouseButton.Left : ev.Properties.IsRightButtonPressed ? MouseButton.Right : ev.Properties.IsMiddleButtonPressed ? MouseButton.Middle : MouseButton.None;
			}
			ListenerDoEvent(new ViewportEvent(this, ev), (l, v) => l.MouseDown(v));
			_lastMouseLocationKnown = true;
			_lastMouseLocation = new Point(e.X, e.Y);
		}

		//private void OnMouseDoubleClick(object sender, EventArgs e)
		//{
		//	ListenerDoEvent(new ViewportEvent(this, e), (l, v) => l.MouseDoubleClick(v));
		//}

		private void OnKeyDown(object sender, KeyEventArgs e)
		{
			ListenerDoEvent(new ViewportEvent(this, e), (l, v) => l.KeyDown(v));
		}

		private void OnKeyUp(object sender, KeyEventArgs e)
		{
			ListenerDoEvent(new ViewportEvent(this, e), (l, v) => l.KeyUp(v));
			e.Handled = false;
		}

		private void OnUpdate(object sender, long frame)
		{
			ListenerDo(x => x.UpdateFrame(frame));
		}

		#endregion
	}
}