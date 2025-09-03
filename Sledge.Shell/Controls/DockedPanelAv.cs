using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using System.Drawing;
using System;

namespace Sledge.Shell
{

	public partial class DockedPanelAv : DockPanel
	{
		public int DockDimension
		{
			get
			{
				switch (GetDock(this))
				{
					case Dock.Top:
					case Dock.Bottom:
						return (int)Bounds.Height;
					case Dock.Left:
					case Dock.Right:
						return (int)Bounds.Width;
					default:
						return 0;
				}
			}
			set
			{
				switch (GetDock(this))
				{
					case Dock.Top:
					case Dock.Bottom:
						Bounds = new Rect(Bounds.X, Bounds.Y, Bounds.Width, value);
						break;
					case Dock.Left:
					case Dock.Right:
						Bounds = new Rect(Bounds.X, Bounds.Y, value, Bounds.Height);
						break;
				}
			}
		}

		private bool _hidden;
		public bool Hidden
		{
			get { return _hidden; }
			set
			{
				_hidden = value;
				if (value)
				{
					_savedDimension = DockDimension;
					DockDimension = HandleWidth;
				}
				else
				{
					DockDimension = Math.Max(_savedDimension, 10);
				}
				InvalidateVisual();
			}
		}

		private int _savedDimension = 100;
		private bool _resizing;

		public int MaxSize { get; set; } = 500;

		public int MinSize { get; set; } = 10;

		private const int ResizeHandleSize = 4;

		public DockedPanelAv()
		{
			MinSize = 10;
			MaxSize = 500;
			_resizing = false;
			this.Height = 500;
			this.Width = 500;
			//this.Children.Add(new Button { });
			//InitializeComponent();

		}

		private bool IsInResizeArea(PointerEventArgs e)
		{
			switch (GetDock(this))
			{
				case Dock.Left:
					return Bounds.Width >= e.GetPosition(this).X && Bounds.Width - ResizeHandleSize <= e.GetPosition(this).X;
				case Dock.Right:
					return e.GetPosition(this).X >= 0 && e.GetPosition(this).X <= ResizeHandleSize;
				case Dock.Top:
					return Bounds.Height >= e.GetPosition(this).Y && Bounds.Height - ResizeHandleSize <= e.GetPosition(this).Y;
				case Dock.Bottom:
					return e.GetPosition(this).Y >= 0 && e.GetPosition(this).Y <= ResizeHandleSize;
			}
			return false;
		}

		private bool IsInButtonArea(PointerEventArgs e)
		{
			switch (GetDock(this))
			{
				case Dock.Left:
					return Bounds.Width >= e.GetPosition(this).X && Bounds.Width - ButtonHeight <= e.GetPosition(this).X && e.GetPosition(this).Y <= ButtonHeight;
				case Dock.Right:
					return e.GetPosition(this).X >= 0 && e.GetPosition(this).X <= ButtonHeight && e.GetPosition(this).Y <= ButtonHeight;
				case Dock.Top:
					return Bounds.Height >= e.GetPosition(this).Y && Bounds.Height - ButtonHeight <= e.GetPosition(this).Y && e.GetPosition(this).X <= ButtonHeight;
				case Dock.Bottom:
					return e.GetPosition(this).Y >= 0 && e.GetPosition(this).Y <= ButtonHeight && e.GetPosition(this).X <= ButtonHeight;
			}
			return false;
		}

		private void SetDockSize(PointerEventArgs e)
		{
			double width = Bounds.Width, height = Bounds.Height;
			switch (GetDock(this))
			{
				case Dock.Left:
					width = e.GetPosition(this).X;
					break;
				case Dock.Right:
					width -= e.GetPosition(this).X;
					break;
				case Dock.Top:
					height = e.GetPosition(this).Y;
					break;
				case Dock.Bottom:
					height -= e.GetPosition(this).Y;
					break;
			}

			Bounds = new Rect(Bounds.X, Bounds.Y,
			DockedPanelAv.Clamp<double>(width, Math.Max(MinSize, ResizeHandleSize + 1), MaxSize),
						DockedPanelAv.Clamp<double>(height, Math.Max(MinSize, ResizeHandleSize + 1), MaxSize));

		}
		public static T Clamp<T>(T val, T min, T max) where T : IComparable<T>
		{
			if (val.CompareTo(min) < 0) return min;
			else if (val.CompareTo(max) > 0) return max;
			else return val;
		}
		protected override void OnPointerMoved(PointerEventArgs e)
		{
			this.Children.Add(new TextBox { Text = "This is DockedPanelAv" });
			if (_resizing)
			{
				SetDockSize(e);
			}
			else
			{
				var ba = IsInButtonArea(e);
				var ra = IsInResizeArea(e);
				if (ba || (_hidden && ra))
				{
					Cursor = new Cursor(StandardCursorType.Hand);
				}
				else if (ra && !_hidden)
				{
					Cursor = GetDock(this) == Dock.Left || GetDock(this) == Dock.Right
						? new Cursor(StandardCursorType.SizeWestEast)
						: new Cursor(StandardCursorType.SizeNorthSouth);
				}
				else
				{
					Cursor = new Cursor(StandardCursorType.Arrow);
				}
			}
			base.OnPointerMoved(e);


			const int padding = 4;
			var rect = new Rect();
			var rotflip = RotateFlipType.RotateNoneFlipNone;
			int buttonX = 0, buttonY = 0;
			switch (GetDock(this))
			{
				case Dock.Top:
					rect = new Rect(padding + ButtonHeight, Bounds.Height - RenderHandleWidth - 1, Bounds.Width - padding - padding - ButtonHeight, RenderHandleWidth);
					rotflip = _hidden ? RotateFlipType.RotateNoneFlipNone : RotateFlipType.RotateNoneFlipY;
					buttonX = padding;
					buttonY = (int)Bounds.Height - HandleWidth;
					break;
				case Dock.Bottom:
					rect = new Rect(padding + ButtonHeight, 1, Bounds.Width - padding - padding - ButtonHeight, RenderHandleWidth);
					rotflip = !_hidden ? RotateFlipType.RotateNoneFlipNone : RotateFlipType.RotateNoneFlipY;
					buttonX = padding;
					break;
				case Dock.Left:
					rect = new Rect(Bounds.Width - RenderHandleWidth - 1, padding + ButtonHeight, RenderHandleWidth, Bounds.Height - padding - padding - ButtonHeight);
					rotflip = _hidden ? RotateFlipType.Rotate90FlipX : RotateFlipType.Rotate90FlipNone;
					buttonY = padding;
					buttonX = (int)Bounds.Width - HandleWidth;
					break;
				case Dock.Right:
					rect = new Rect(1, padding + ButtonHeight, RenderHandleWidth, Bounds.Height - padding - padding - ButtonHeight);
					rotflip = !_hidden ? RotateFlipType.Rotate90FlipX : RotateFlipType.Rotate90FlipNone;
					buttonY = padding;
					break;
			}
			//if (rect.Size)
			//{
			//	context.FillRectangle(Brushes.Darken(_hidden ? 10 : 40), rect);
			//	using (var cl = new Bitmap(_arrow))
			//	{
			//		cl.RotateFlip(rotflip);
			//		context.DrawImage(cl, new Rect(buttonX, buttonY, cl.PixelWidth, cl.PixelHeight));
			//	}
			//}
		}
		//protected override void OnPointerLeave(PointerEventArgs e)
		//{
		//	if (!_resizing) Cursor = new Cursor(StandardCursorType.Arrow);
		//	base.OnPointerLeave(e);
		//}

		protected override void OnPointerPressed(PointerPressedEventArgs e)
		{
			var ba = IsInButtonArea(e);
			var ra = IsInResizeArea(e);
			if (ba || (ra && _hidden)) Hidden = !Hidden;
			else if (!_hidden && IsInResizeArea(e)) _resizing = true;
			base.OnPointerPressed(e);
		}

		protected override void OnPointerReleased(PointerReleasedEventArgs e)
		{
			_resizing = false;
			base.OnPointerReleased(e);
		}

		private const int ButtonHeight = 12;
		private const int HandleWidth = 8;
		private const int RenderHandleWidth = 3;



	}
}