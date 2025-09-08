using System;
using System.Linq;
using System.Numerics;
using Avalonia;
using Avalonia.Input;

namespace Sledge.BspEditor.Rendering.Viewport
{
	public class ViewportEvent : EventArgs
	{
		public MapViewport Sender { get; set; }
		public bool Handled { get; set; }

		// Key
		public Key Modifiers { get; set; }
		public bool Control { get; set; }
		public bool Shift { get; set; }
		public bool Alt { get; set; }
		public Key KeyCode { get; set; }
		public int KeyValue { get; set; }
		public char KeyChar { get; set; }

		// Mouse
		public MouseButton Button { get; set; }
		public int Clicks { get; set; }
		public int X { get; set; }
		public int Y { get; set; }
		public int Delta { get; set; }

		public Vector3 Location => new Vector3(X, Y, 0);

		// Mouse movement
		public int LastX { get; set; }
		public int LastY { get; set; }

		public int DeltaX => X - LastX;
		public int DeltaY => Y - LastY;

		// Click and drag
		public bool Dragging { get; set; }
		public int StartX { get; set; }
		public int StartY { get; set; }

		// 2D Camera
		public Vector3 CameraPosition { get; set; }
		public decimal CameraZoom { get; set; }

		public ViewportEvent(MapViewport sender, EventArgs e = null)
		{
			Sender = sender;
		}

		public ViewportEvent(MapViewport sender, KeyEventArgs e)
		{
			Sender = sender;
			KeyChar = e.KeySymbol.First();
			Control = e.KeyModifiers.HasFlag(KeyModifiers.Control);
			Shift = e.KeyModifiers.HasFlag(KeyModifiers.Shift);
			Alt = e.KeyModifiers.HasFlag(KeyModifiers.Alt);
			KeyCode = e.Key;
			//KeyValue = e.ke;
		}

		public ViewportEvent(MapViewport sender, PointerEventArgs e)
		{
			var buttons = MouseButton.None;
			var position = e.GetPosition(sender.Control);
			var clicks = 0;
			var delta = 0;
			if (e is PointerPressedEventArgs e1)
			{
				//buttons = e1.
				clicks = e1.ClickCount;
			}
			if (e is PointerWheelEventArgs wheel)
			{
				delta = (int)wheel.Delta.Length;
			}
			Sender = sender;
			//         Button = e.
			Clicks = clicks;
			X = (int)position.X;
			Y = (int)position.Y;
			Delta = delta;
		}
	}
}