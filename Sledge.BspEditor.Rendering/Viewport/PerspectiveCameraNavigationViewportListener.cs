using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.Common;
using Sledge.Common.Easings;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Settings;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Overlay;
using Sledge.Rendering.Viewports;
using Sledge.Shell;
using Sledge.Shell.Input;

namespace Sledge.BspEditor.Rendering.Viewport
{
	public class PerspectiveCameraNavigationViewportListener : IViewportEventListener, IOverlayRenderable
	{
        public string OrderHint => FreeLook ? "A" : "W";
        public MapViewport Viewport { get; set; }

        private int LastKnownX { get; set; }
        private int LastKnownY { get; set; }
        private bool PositionKnown { get; set; }
        private bool FreeLook { get; set; }
        private bool FreeLookToggle { get; set; }
        private bool CursorVisible { get; set; }
        private Rectangle CursorClip { get; set; }
        private bool Focus { get; set; }
        private PerspectiveCamera Camera => Viewport.Viewport.Camera as PerspectiveCamera;
		private long _downMillis;
        private long _lastMillis;
        private readonly Easing _easing;
        private readonly List<Keys> _downKeys;
        private readonly IContext _context;

        // Free look toggled from a dialog (texture application) that keeps the keyboard focus: the viewport is
        // neither focused nor hovered then, so these keep it alive and let the same key (or Escape) turn it off
        private bool _dialogEngaged;
        private Point _dialogCursorPos;
        private System.Windows.Forms.Timer _pollTimer;
        private Subscription _focusSubscription;

        // Every 3D viewport listener hears the dialog's event; only one may act on it
        private static PerspectiveCameraNavigationViewportListener _dialogOwner;
        private static readonly List<WeakReference<PerspectiveCameraNavigationViewportListener>> Instances = new List<WeakReference<PerspectiveCameraNavigationViewportListener>>();
        private static WeakReference<PerspectiveCameraNavigationViewportListener> _lastHovered;

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

		private ViewportSettingContainer _settingsContainer;

		private Keys _forwardKey => _settingsContainer.ForwardKey;
        private Keys _backwardKey => _settingsContainer.BackwardKey;
        private Keys _leftKey => _settingsContainer.LeftKey;
        private Keys _rightKey => _settingsContainer.RightKey;
        private Keys _upKey => _settingsContainer.UpKey;
        private Keys _downKey => _settingsContainer.DownKey;
		private Keys _panRightKey => _settingsContainer.PanRightKey;
		private Keys _panLeftKey => _settingsContainer.PanLeftKey;
		private Keys _tiltUpKey => _settingsContainer.TiltUpKey;
		private Keys _tiltDownKey => _settingsContainer.TiltDownKey;
        private Keys _freeLookKey => _settingsContainer.FreeLookKey;

		public PerspectiveCameraNavigationViewportListener(MapViewport vp)
        {
            LastKnownX = 0;
            LastKnownY = 0;
            PositionKnown = false;
            FreeLook = false;
            FreeLookToggle = false;
            CursorVisible = true;
            Focus = false;
            Viewport = vp;
            _downKeys = new List<Keys>();
            _downMillis = _lastMillis = 0;
            _easing = Easing.FromType(EasingType.Sinusoidal, EasingDirection.Out);

            _context = Container.Get<IContext>();
            Oy.Subscribe<ITool>("Tool:Activated", ToolSelected);
            // Allow dialogs (e.g. the texture application) to hand keyboard focus over
            // to the 3D viewport when they have it instead of the viewport
            _focusSubscription = Oy.Subscribe("BspEditor:Viewport:Focus3DView", Focus3DViewFromDialog);
            lock (Instances)
            {
                Instances.RemoveAll(x => !x.TryGetTarget(out _));
                Instances.Add(new WeakReference<PerspectiveCameraNavigationViewportListener>(this));
            }
			_settingsContainer = Common.Container.GetMany<ISettingsContainer>().OfType<ViewportSettingContainer>().FirstOrDefault();
        }

        private Task ToolSelected(ITool tool)
        {
            if (FreeLook || FreeLookToggle)
            {
                FreeLook = FreeLookToggle = false;
                ClearDialogEngaged();
                SetFreeLook();
            }
            return Task.CompletedTask;
        }

        public void UpdateFrame(long frame)
        {
            var currMillis = _lastMillis;
            _lastMillis = frame;

            if (currMillis == 0) return;
            // Toggled from a dialog, the cursor is not hovering the viewport, so hover must not cancel free look
            var engaged = _dialogEngaged && FreeLookToggle;
            if (!Viewport.IsUnlocked(this) || (!engaged && (!Focus || !Viewport.Viewport.IsFocused)))
            {
                if (FreeLook || FreeLookToggle)
                {
                    FreeLook = FreeLookToggle = false;
                    ClearDialogEngaged();
                    SetFreeLook();
                }
                return;
            }

            var seconds = (frame - currMillis) / 1000f;
            var units = CameraNavigationViewportSettings.ForwardSpeed * seconds;

            var down = AnyKeyDownNow(Keys.W, Keys.A, Keys.S, Keys.D);
            if (!down) _downMillis = 0;
            else if (_downMillis == 0) _downMillis = currMillis;

            if (CameraNavigationViewportSettings.TimeToTopSpeed > 0)
            {
                var downFor = (frame - _downMillis) / (CameraNavigationViewportSettings.TimeToTopSpeed * 1000);
                if (downFor >= 0 && downFor < 1) units *= (float) _easing.Evaluate((double) downFor);
            }

            if (FreeLook)
            {
                if (KeyDownNow(Keys.ShiftKey)) units *= 2;
                if (KeyDownNow(Keys.ControlKey)) units /= 2;
            }

            if (float.IsNaN(units) || float.IsInfinity(units) || units < 0.001f) units = 0;

            var move = units;
            var tilt = 2f;

            // These keys are used for hotkeys, don't want the 3D view to move about when trying to use hotkeys.
            var ignore = !FreeLook && AnyKeyDownNow(Keys.ShiftKey, Keys.ControlKey, Keys.Alt);

            // Movement keys (WASD/QE) work while Z-toggle is active,
            // while Space is held, or while the Camera tool is the active tool.
            var cameraToolActive = _context.Get<ITool>("ActiveTool")?.GetType().Name == "CameraTool";
            if (FreeLookToggle || cameraToolActive || KeyDownNow(Keys.Space))
            {
                IfKey(_forwardKey, () => Camera.Advance(move), ignore);
                IfKey(_backwardKey, () => Camera.Advance(-move), ignore);
                IfKey(_leftKey, () => Camera.Strafe(-move), ignore);
                IfKey(_rightKey, () => Camera.Strafe(move), ignore);
                IfKey(_upKey, () => Camera.AscendAbsolute(move), ignore);
                IfKey(_downKey, () => Camera.AscendAbsolute(-move), ignore);
            }
            else
            {
                _downKeys.Remove(_forwardKey);
                _downKeys.Remove(_backwardKey);
                _downKeys.Remove(_leftKey);
                _downKeys.Remove(_rightKey);
                _downKeys.Remove(_upKey);
                _downKeys.Remove(_downKey);
            }

            // Arrow keys are not really used for hotkeys all that much, so we allow shift+arrows to match Hammer's keys
            var shiftDown = KeyDownNow(Keys.ShiftKey);
            var otherDown = AnyKeyDownNow(Keys.ControlKey, Keys.Alt);

            IfKey(_panRightKey, () => { if (shiftDown) Camera.Strafe(move); else Camera.Pan(-tilt); }, otherDown);
            IfKey(_panLeftKey, () => { if (shiftDown) Camera.Strafe(-move); else Camera.Pan(tilt); }, otherDown);
            IfKey(_tiltUpKey, () => { if (shiftDown) Camera.Ascend(move); else Camera.Tilt(-tilt); }, otherDown);
            IfKey(_tiltDownKey, () => { if (shiftDown) Camera.Ascend(-move); else Camera.Tilt(tilt); }, otherDown);
        }

        private void IfKey(Keys key, Action action, bool ignoreKeyboard)
        {
            if (!KeyDownNow(key))
            {
                _downKeys.Remove(key);
            }
            else if (ignoreKeyboard)
            {
                if (_downKeys.Contains(key)) action();
            }
            else
            {
                if (!_downKeys.Contains(key)) _downKeys.Add(key);
                action();
            }
        }

        public bool IsActive()
        {
            return Viewport != null && Camera != null;
        }

        public void KeyUp(ViewportEvent e)
        {
            SetFreeLook();
        }

        public void KeyDown(ViewportEvent e)
        {
            if (!Focus || !Viewport.IsUnlocked(this)) return;
            // Backup exit for toggled free look: Escape always releases it, so the cursor can't get stuck
            if (e.KeyCode == Keys.Escape && FreeLookToggle && !e.Alt && !e.Control && !e.Shift)
            {
                ExitToggledFreeLook();
                e.Handled = true;
                return;
            }
            if (e.KeyCode == _freeLookKey && !e.Alt && !e.Control && !e.Shift)
            {
                ToggleFreeLook();
                if (!FreeLookToggle) ClearDialogEngaged();
            }
            else
            {
                SetFreeLook();
            }
            if (FreeLook)
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Toggles the free look state.
        /// </summary>
        public void ToggleFreeLook()
        {
            if (!Viewport.IsUnlocked(this)) return;
            FreeLookToggle = !FreeLookToggle;
            SetFreeLook();
            PositionKnown = false;
        }

        // While a dialog holds the keyboard focus the thread's keyboard state is not the viewport's, so read the physical key state
        private bool KeyDownNow(Keys key)
        {
            return _dialogEngaged ? (GetAsyncKeyState((int) key) & 0x8000) != 0 : KeyboardState.IsKeyDown(key);
        }

        private bool AnyKeyDownNow(params Keys[] keys)
        {
            return keys.Any(KeyDownNow);
        }

        private void ClearDialogEngaged()
        {
            _dialogEngaged = false;
            if (ReferenceEquals(_dialogOwner, this)) _dialogOwner = null;
        }

        /// <summary>
        /// Turns toggled free look off and restores the cursor (to where it was, if free look was started from a dialog).
        /// </summary>
        private void ExitToggledFreeLook()
        {
            var wasEngaged = _dialogEngaged;
            FreeLookToggle = false;
            ClearDialogEngaged();
            StopPoll();
            SetFreeLook();
            PositionKnown = false;
            if (wasEngaged) Cursor.Position = _dialogCursorPos;
        }

        private bool IsUsable()
        {
            var c = Viewport?.Control;
            return c != null && !c.IsDisposed && c.IsHandleCreated && c.Visible && Camera != null;
        }

        // The last hovered visible 3D viewport, else the first visible one
        private bool IsDialogTarget()
        {
            if (!IsUsable()) return false;
            PerspectiveCameraNavigationViewportListener target = null;
            lock (Instances)
            {
                if (_lastHovered != null && _lastHovered.TryGetTarget(out var hovered) && hovered.IsUsable())
                {
                    target = hovered;
                }
                else
                {
                    foreach (var w in Instances)
                    {
                        if (w.TryGetTarget(out var l) && l.IsUsable()) { target = l; break; }
                    }
                }
            }
            return ReferenceEquals(target, this);
        }

        private void StartPoll()
        {
            if (_pollTimer == null)
            {
                _pollTimer = new System.Windows.Forms.Timer { Interval = 30 };
                _pollTimer.Tick += PollDialogEngaged;
            }
            _pollTimer.Start();
        }

        private void StopPoll()
        {
            _pollTimer?.Stop();
        }

        // UI thread, only while free look was toggled from a dialog: Escape exits even though the viewport has no keyboard focus
        private void PollDialogEngaged(object sender, EventArgs e)
        {
            if (!FreeLookToggle || !_dialogEngaged)
            {
                StopPoll();
                return;
            }
            if (!IsUsable())
            {
                ExitToggledFreeLook();
                return;
            }
            if (Form.ActiveForm == null) return; // only while this application is in the foreground

            var mods = (GetAsyncKeyState((int) Keys.ControlKey) & 0x8000) != 0
                       || (GetAsyncKeyState((int) Keys.ShiftKey) & 0x8000) != 0
                       || (GetAsyncKeyState((int) Keys.Menu) & 0x8000) != 0;
            if (!mods && (GetAsyncKeyState((int) Keys.Escape) & 0x8000) != 0) ExitToggledFreeLook();
        }

        /// <summary>
        /// Handles the free look key pressed while a dialog (the texture application) has the keyboard focus:
        /// toggles 3D free look right there, without moving the cursor or the focus to the 3D view first.
        /// The same key pressed in the dialog again (or Escape) turns it off.
        /// </summary>
        private void Focus3DViewFromDialog()
        {
            // A viewport already toggled from the dialog owns the event: it exits, every other one ignores it
            var owner = _dialogOwner;
            if (owner != null && owner._dialogEngaged && owner.FreeLookToggle)
            {
                if (ReferenceEquals(owner, this)) Viewport.Control.InvokeSync(ExitToggledFreeLook);
                return;
            }

            // Hidden or other 3D views must not react as well (each would hide the cursor)
            if (!IsDialogTarget()) return;

            Viewport.Control.InvokeSync(() =>
            {
                _dialogCursorPos = Cursor.Position;
                LastKnownX = Viewport.Width / 2;
                LastKnownY = Viewport.Height / 2;
                PositionKnown = false;
                Focus = true;

                // Set before toggling so the per-frame hover check can't cancel free look straight away.
                // SetFreeLook itself saves the cursor clip, confines the cursor to the viewport and hides it,
                // and restores all of that when free look is turned off.
                _dialogEngaged = true;
                _dialogOwner = this;
                ToggleFreeLook();

                if (FreeLookToggle) StartPoll();
                else ClearDialogEngaged();
            });
        }

        private void SetFreeLook()
        {
            if (!Viewport.IsUnlocked(this)) return;
            FreeLook = false;
            
            if (FreeLookToggle)
            {
                FreeLook = true;
            }
            else
            {
                var left = Control.MouseButtons.HasFlag(MouseButtons.Left);
                var right = Control.MouseButtons.HasFlag(MouseButtons.Right);
                var mid = Control.MouseButtons.HasFlag(MouseButtons.Middle);

                var activeTool = _context.Get<ITool>("ActiveTool");
                if (activeTool != null && activeTool.GetType().Name == "CameraTool")
                {
                    FreeLook = left || right;
                }
                else
                {
                    var space = KeyboardState.IsKeyDown(Keys.Space);
                    var req = CameraNavigationViewportSettings.Camera3DPanRequiresMouseClick;
                    FreeLook = (space && (!req || left || right))||(mid);
                }
            }

            if (FreeLook && CursorVisible)
            {
                CursorClip = Cursor.Clip;
                Cursor.Clip = Viewport.Control.RectangleToScreen(new Rectangle(0, 0, Viewport.Width, Viewport.Height));
                SetCapture(true);
                Viewport.AquireInputLock(this);
            }
            else if (!FreeLook && !CursorVisible)
            {
                Cursor.Clip = CursorClip;
                CursorClip = Rectangle.Empty;
                SetCapture(false);
                Viewport.ReleaseInputLock(this);
            }
        }

        private void SetCapture(bool capture)
        {
            Viewport.Control.InvokeSync(() =>
            {
                Viewport.Control.Capture = capture;
                if (capture && CursorVisible)
                {
                    CursorVisible = false;
                    Cursor.Hide();
                }
                else if (!capture && !CursorVisible)
                {
                    CursorVisible = true;
                    Cursor.Show();
                }
            });
        }

        public void KeyPress(ViewportEvent e)
        {
            if (FreeLook)
            {
                e.Handled = true;
            }
        }

        public void MouseMove(ViewportEvent e)
        {
            if (!Focus) return;
            if (PositionKnown && FreeLook)
            {
                var dx = LastKnownX - e.X;
                var dy = e.Y - LastKnownY;
                if (dx != 0 || dy != 0)
                {
                    MouseMoved(e, dx, dy);
                    return;
                }
            }
            LastKnownX = e.X;
            LastKnownY = e.Y;
            PositionKnown = true;
        }

        private void MouseMoved(ViewportEvent e, int dx, int dy)
        {
            if (!FreeLook) return;

            var left = Control.MouseButtons.HasFlag(MouseButtons.Left);
            var right = Control.MouseButtons.HasFlag(MouseButtons.Right);
            var updown = !left && right;
            var forwardback = left && right;

            if (CameraNavigationViewportSettings.InvertX) dx = -dx;
            if (CameraNavigationViewportSettings.InvertY) dy = -dy;

            if (updown)
            {
                Camera.Strafe(-dx);
                Camera.Ascend(-dy);
            }
            else if (forwardback)
            {
                Camera.Strafe(-dx);
                Camera.Advance(-dy);
            }
            else // left mouse or z-toggle
            {
                // adjust is an arbitrary and tunable parameter to get setting
                // values that are sensible and not too high or too low.
                const float adjust = 100.0f;

                // Scale with FOV for consistency
                float fovscale = CameraNavigationViewportSettings.FOV / 60f;
                float scale = fovscale * ( (float)CameraNavigationViewportSettings.Sensitivity / adjust );
                Camera.Pan(dx * scale);
                Camera.Tilt(dy * scale);
            }

            LastKnownX = Viewport.Width/2;
            LastKnownY = Viewport.Height/2;
            Cursor.Position = Viewport.Control.PointToScreen(new Point(LastKnownX, LastKnownY));
        }

        public void MouseWheel(ViewportEvent e)
        {
            if (!Viewport.IsUnlocked(this) || e.Delta == 0) return;
            Camera.Advance((e.Delta / (float) Math.Abs(e.Delta)) * (float) CameraNavigationViewportSettings.MouseWheelMoveDistance);
        }

        public void MouseUp(ViewportEvent e)
        {
            SetFreeLook();
        }

        public void MouseDown(ViewportEvent e)
        {
			if (e.Button == MouseButtons.Middle)
			{
                FreeLook = true;
				PositionKnown = false;
			}

			if (FreeLook && KeyboardState.IsKeyDown(Keys.Space)) e.Handled = true;
            SetFreeLook();
        }

        public void MouseClick(ViewportEvent e)
        {
            if (FreeLook) e.Handled = true;
        }

        public void MouseDoubleClick(ViewportEvent e)
        {
            if (FreeLook) e.Handled = true;
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
            Focus = true;
            _lastHovered = new WeakReference<PerspectiveCameraNavigationViewportListener>(this);
        }

        public void MouseLeave(ViewportEvent e)
        {
            if (FreeLook)
            {
                LastKnownX = Viewport.Width/2;
                LastKnownY = Viewport.Height/2;
                Cursor.Position = Viewport.Control.PointToScreen(new Point(LastKnownX, LastKnownY));
            }
            else
            {
                if (!CursorVisible)
                {
                    Cursor.Clip = CursorClip;
                    CursorClip = Rectangle.Empty;
                    SetCapture(false);
                    Viewport.ReleaseInputLock(this);
                }
                PositionKnown = false;
                Focus = false;
            }
        }

        public bool Filter(string hotkey, int keys)
        {
            // If we're freelooking, consume any hotkeys using WASD/QE and ctrl or shift
            if (FreeLook)
            {
                var k = (Keys) keys;
                if (
                    k.HasFlag(Keys.W) || k.HasFlag(Keys.A) ||
                    k.HasFlag(Keys.S) || k.HasFlag(Keys.D) ||
                    k.HasFlag(Keys.Q) || k.HasFlag(Keys.E)
                )
                {
                    return !k.HasFlag(Keys.Alt);
                }
            }
            return false;
        }

        public void Dispose()
        {
            try { if (_focusSubscription != null) Oy.Unsubscribe(_focusSubscription); } catch { }
            _focusSubscription = null;
            ClearDialogEngaged();
            _pollTimer?.Dispose();
            _pollTimer = null;
            lock (Instances) Instances.RemoveAll(x => !x.TryGetTarget(out var l) || ReferenceEquals(l, this));
        }

        public void Render(IViewport viewport, OrthographicCamera camera, Vector3 worldMin, Vector3 worldMax, I2DRenderer im)
        {
            // 
        }

        public void Render(IViewport viewport, PerspectiveCamera camera, I2DRenderer im)
        {
            if (CursorVisible) return;
            if (Viewport.Viewport != viewport) return;

            var x = viewport.Width / 2;
            var y = viewport.Height / 2;
            const int size = 3;

            im.AddRectFilled(new Vector2(x - 1, y - size - 1), new Vector2(x + 2, y + size + 2), Color.Black);
            im.AddRectFilled(new Vector2(x - size - 1, y - 1), new Vector2(x + size + 2, y + 2), Color.Black);

            im.AddLine(new Vector2(x, y - size), new Vector2(x, y + size + 1), Color.White, 1, false);
            im.AddLine(new Vector2(x - size, y), new Vector2(x + size + 1, y), Color.White, 1, false);
        }
	}
}
