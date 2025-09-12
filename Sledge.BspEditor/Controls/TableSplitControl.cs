using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using Sledge.Shell;
using Avalonia.Input;
using Avalonia;
using Avalonia.Media;

namespace Sledge.BspEditor.Controls
{
	public sealed class TableSplitControl : Avalonia.Controls.Grid
	{
		private int _inH;
		private int _inV;

		private bool _resizing;

		public int MinimumViewSize { get; set; }
		private int MaximumViewSize => 100 - MinimumViewSize;

		private TableSplitConfiguration _configuration;

		public TableSplitConfiguration Configuration
		{
			get => _configuration;
			set
			{
				if (!value.IsValid()) return;
				_configuration = value;
				ResetLayout();
			}
		}

		public IEnumerable<float> RowSizes
		{
			get => RowDefinitions.OfType<RowDefinition>().Select(x => (float)x.Height.Value);
			set
			{
				//while (vals.Count < RowDefinitions.Count) vals.Add((float)RowDefinitions[vals.Count].Height.Value);
				//var total = vals.Aggregate(0f, (a, b) => a + b);
				var vals = value.ToList();
				var total = vals.Sum();
				for (var i = 0; i < vals.Count; i++)
				{
					if (i < RowDefinitions.Count)
					{
						// Convert percentage to proportional star
						var proportion = vals[i] / total;
						RowDefinitions[i].Height = new GridLength(proportion, GridUnitType.Star);
					}
				}
				//var width = double.IsNaN(Width) ? 0 : (float)Width;
				//vals = vals.Select(x => x / total * (float)width).ToList();
				//for (var i = 0; i < vals.Count; i++)
				//{
				//	if (i < RowDefinitions.Count) RowDefinitions[i].Height = new GridLength(vals[i], GridUnitType.Pixel);
			}
		}

		public IEnumerable<float> ColumnSizes
		{
			get => ColumnDefinitions.OfType<ColumnDefinition>().Select(x => (float)x.Width.Value);
			set
			{
				//var vals = value.ToList();
				//while (vals.Count < ColumnDefinitions.Count) vals.Add((float)ColumnDefinitions[vals.Count].Width.Value);
				//var total = vals.Aggregate(0f, (a, b) => a + b);
				//var height = double.IsNaN(Height) ? 0 : (float)Height;
				//vals = vals.Select(x => x / total * (float)height).ToList();
				//for (var i = 0; i < vals.Count; i++)
				//{
				//	if (i < ColumnDefinitions.Count) ColumnDefinitions[i].Width = new GridLength(vals[i], GridUnitType.Pixel);
				//}
				var vals = value.ToList();
				var total = vals.Sum();
				for (var i = 0; i < vals.Count; i++)
				{
					if (i < ColumnDefinitions.Count)
					{
						// Convert percentage to proportional star
						var proportion = vals[i] / total;
						ColumnDefinitions[i].Width = new GridLength(proportion, GridUnitType.Star);
					}
				}

			}
		}

		public int RowCount { get; private set; }
		public int ColumnCount { get; private set; }

		public struct GridPosition
		{
			public int Row;
			public int Column;
		}
		public GridPosition GetPositionFromControl(Control cc) => new GridPosition
		{
			Row = GetRow(cc),
			Column = GetColumn(cc)
		};

		private void ResetLayout()
		{
			//SuspendLayout();

			// Remove any controls that aren't in the layout anymore
			var recs = _configuration.Rectangles.ToList();
			foreach (var cc in Children.OfType<Control>().ToList())
			{
				var pos = GetPositionFromControl(cc);
				if (recs.Any(x => x.Y == pos.Row && x.X == pos.Column)) continue;

				Children.Remove(cc);
				//cc.Dispose();
			}

			// Set the new layout
			RowCount = _configuration.Rows;
			ColumnCount = _configuration.Columns;
			//ColumnDefinitions.Clear();
			//RowDefinitions.Clear();
			//for (var i = 0; i < ColumnCount; i++) ColumnDefinitions.Add(new ColumnDefinition((int)(100m / ColumnCount) * this.Width, GridUnitType.Pixel)); //FIXME: percent
			//for (var i = 0; i < RowCount; i++) RowDefinitions.Add(new RowDefinition((int)(100m / RowCount) * this.Height, GridUnitType.Pixel));

			// Make sure there's at least an empty control in every cell
			foreach (var rec in recs)
			{
				var i = rec.X;
				var j = rec.Y;
				var c = GetControlFromPosition(i, j);
				if (c == null) Children.Add(c = new Panel { MinHeight = 200, MinWidth = 200 /*BackColor = SystemColors.ControlDark, Dock = DockStyle.Fill */});
				SetRow(c, rec.Y);
				SetColumn(c, rec.X);
				SetRowSpan(c, rec.Height);
				SetColumnSpan(c, rec.Width);
			}

			//ResumeLayout();
			ResetViews();
		}
		public Control GetControlFromPosition(int x, int y)
		{
			foreach (var control in Children)
			{
				var row = GetRow(control);
				var col = GetColumn(control);
				if (x == col && y == row) return control;
			}
			return null;
		}

		public TableSplitControl()
		{
			ColumnDefinitions.Add(new ColumnDefinition(1, GridUnitType.Star));
			ColumnDefinitions.Add(new ColumnDefinition(1, GridUnitType.Star));
			RowDefinitions.Add(new RowDefinition(1, GridUnitType.Star));
			RowDefinitions.Add(new RowDefinition(1, GridUnitType.Star));
			Background = Brushes.Transparent;
			ColumnSpacing = 10;
			RowSpacing = 10;
			Margin = new Thickness(4);
			MinimumViewSize = 2;
			_resizing = false;
			_inH = _inV = -1;
			_configuration = TableSplitConfiguration.Default();
			ResetLayout();
		}
		protected override void OnLoaded(RoutedEventArgs e)
		{
			var r = GetPositionFromControl((Control)e.Source);
			var parentSize = ((Control)Parent).Bounds;
			//Width = parentSize.Width;
			//Height = parentSize.Height;
			HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
			VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch;
			var rec = _configuration.Rectangles.FirstOrDefault(t => t.X <= r.Column && t.X + t.Width > r.Column && t.Y <= r.Row && t.Y + t.Height > r.Row);
			if (!rec.IsEmpty)
			{
				SetRow((Control)e.Source, rec.Y);
				SetColumn((Control)e.Source, rec.X);
				SetRowSpan((Control)e.Source, rec.Height);
				SetColumnSpan((Control)e.Source, rec.Width);
			}
			base.OnLoaded(e);
		}

		public void ReplaceControl(Control oldControl, Control newControl)
		{
			int col = GetColumn(oldControl),
				row = GetRow(oldControl),
				csp = GetColumnSpan(oldControl),
				rsp = GetRowSpan(oldControl);

			Children.Remove(oldControl);
			SetColumnSpan(newControl, csp);
			SetRowSpan(newControl, rsp);
		}

		public void ResetViews()
		{
			ColumnDefinitions.Clear();
			RowDefinitions.Clear();
			ColumnDefinitions.Add(new ColumnDefinition(1, GridUnitType.Star));
			ColumnDefinitions.Add(new ColumnDefinition(1, GridUnitType.Star));
			RowDefinitions.Add(new RowDefinition(1, GridUnitType.Star));
			RowDefinitions.Add(new RowDefinition(1, GridUnitType.Star));
			return;
			var c = (int)Math.Floor(100m / _configuration.Columns);
			var r = (int)Math.Floor(100m / _configuration.Rows);
			for (var i = 0; i < ColumnCount; i++) ColumnDefinitions[i].Width = new GridLength(i == 0 ? 100 - (c * (ColumnCount - 1)) : c, GridUnitType.Pixel);
			for (var i = 0; i < RowCount; i++) RowDefinitions[i].Height = new GridLength(i == 0 ? 100 - (r * (RowCount - 1)) : r, GridUnitType.Pixel);
		}

		public void FocusOn(Control ctrl)
		{
			if (ctrl == null || !Children.Contains(ctrl)) return;
			var row = GetRow(ctrl);
			var col = GetColumn(ctrl);
			FocusOn(row, col);
		}

		public void FocusOn(int rowIndex, int columnIndex)
		{
			if (rowIndex < 0 || rowIndex > 1 || columnIndex < 0 || columnIndex > 1) return;
			RememberFocus();
			ColumnDefinitions[columnIndex].Width = new GridLength(MaximumViewSize);
			ColumnDefinitions[(columnIndex + 1) % 2].Width = new GridLength(MinimumViewSize);
			RowDefinitions[rowIndex].Height = new GridLength(MaximumViewSize);
			RowDefinitions[(rowIndex + 1) % 2].Height = new GridLength(MinimumViewSize);
		}

		private void RememberFocus()
		{
			_memoryWidth = new float[ColumnDefinitions.Count];
			_memoryHeight = new float[RowDefinitions.Count];
			for (var i = 0; i < ColumnDefinitions.Count; i++)
			{
				_memoryWidth[i] = (float)ColumnDefinitions[i].Width.Value;
			}
			for (var i = 0; i < RowDefinitions.Count; i++)
			{
				_memoryHeight[i] = (float)RowDefinitions[i].Height.Value;
			}
		}

		private void ForgetFocus()
		{
			_memoryWidth = _memoryHeight = null;
		}

		public void Unfocus()
		{
			for (var i = 0; i < ColumnDefinitions.Count; i++)
			{
				ColumnDefinitions[i].Width = new GridLength(_memoryWidth[i]);
			}
			for (var i = 0; i < RowDefinitions.Count; i++)
			{
				RowDefinitions[i].Height = new GridLength(_memoryHeight[i]);
			}
			ForgetFocus();
		}

		public bool IsFocusing()
		{
			return _memoryWidth != null;
		}

		private float[] _memoryWidth;
		private float[] _memoryHeight;
		private int[] GetColumnWidths() => ColumnDefinitions.Select(x => (int)x.Width.Value).ToArray();
		private int[] GetRowHeights() => RowDefinitions.Select(x => (int)x.Height.Value).ToArray();
		protected override void OnPointerMoved(PointerEventArgs ev)
		{
			var e = ev.GetCurrentPoint(this).Position;
			if (_resizing)
			{
				if (_inH >= 0 && Width > 0)
				{
					ForgetFocus();
					var mp = e.Y / (float)Height * 100;
					SetHorizontalSplitPosition(_inH, (float)mp);
				}
				if (_inV >= 0 && Height > 0)
				{
					ForgetFocus();
					var mp = e.X / (float)Width * 100;
					SetVerticalSplitPosition(_inV, (float)mp);
				}
			}
			else
			{
				var cw = GetColumnWidths();
				var rh = GetRowHeights();
				_inH = _inV = -1;
				int hval = 0, vval = 0;

				//todo: rowspan checks

				for (var i = 0; i < rh.Length - 1; i++)
				{
					hval += rh[i];
					var top = hval - Margin.Bottom;
					var bottom = hval + Margin.Top;
					if (e.X <= Margin.Left || e.X >= Width - Margin.Right || e.Y <= top || e.Y >= bottom) continue;
					_inH = i;
					break;
				}

				for (var i = 0; i < cw.Length - 1; i++)
				{
					vval += cw[i];
					var left = vval - Margin.Right;
					var right = vval + Margin.Left;
					if (e.Y <= Margin.Top || e.Y >= Height - Margin.Bottom || e.X <= left || e.X >= right) continue;
					_inV = i;
					break;
				}

				if (_inH >= 0 && _inV >= 0) Cursor = new Cursor(StandardCursorType.SizeAll);
				else if (_inV >= 0) Cursor = new Cursor(StandardCursorType.SizeWestEast);
				else if (_inH >= 0) Cursor = new Cursor(StandardCursorType.SizeNorthSouth);
				else Cursor = Cursor.Default;
			}

			base.OnPointerMoved(ev);
		}

		protected override void OnPointerExited(PointerEventArgs e)
		{
			if (!_resizing) Cursor = Cursor.Default;
			base.OnPointerExited(e);
		}
		protected override void OnPointerPressed(PointerPressedEventArgs e)
		{
			if (_inV >= 0 || _inH >= 0) _resizing = true;
			base.OnPointerPressed(e);
		}
		protected override void OnPointerReleased(PointerReleasedEventArgs e)
		{
			_resizing = false;
			base.OnPointerReleased(e);
		}
		protected void OnMouseDoubleClick(PointerEventArgs e) // TODO: ??
		{
			if (_inH >= 0 && _inH < RowCount - 1)
			{
				ForgetFocus();
				SetHorizontalSplitPosition(_inH, (_inH + 1f) / RowCount * 100);
			}
			if (_inV >= 0)
			{
				ForgetFocus();
				SetVerticalSplitPosition(_inV, (_inV + 1f) / ColumnCount * 100);
			}
		}

		private void SetVerticalSplitPosition(int index, float percentage)
		{
			percentage = Math.Min(100, Math.Max(0, percentage));
			if (ColumnCount == 0 || index < 0 || index >= ColumnCount - 1 || Width <= 0) return;

			var widths = ColumnDefinitions.OfType<ColumnDefinition>().Select(x => x.Width).ToList();
			var currentPercent = widths.GetRange(0, index + 1).Sum(x => x.Value);
			if (percentage < currentPercent)
			{
				// <--
				var diff = currentPercent - percentage;
				for (var i = index; i >= 0 && diff > 0; i--)
				{
					var w = widths[i];
					var nw = Math.Max(MinimumViewSize, w.Value - diff);
					widths[i] = new GridLength(nw);
					widths[index + 1] = new GridLength(widths[index + 1].Value + (w.Value - nw));
					diff -= (w.Value - nw);
				}
			}
			else if (percentage > currentPercent)
			{
				// -->
				var diff = percentage - currentPercent;
				for (var i = index + 1; i < widths.Count && diff > 0; i++)
				{
					var w = widths[i];
					var nw = Math.Max(MinimumViewSize, w.Value - diff);
					widths[i] = new GridLength(nw);
					widths[index + 1] = new GridLength(widths[index + 1].Value + (w.Value - nw));
					diff -= (w.Value - nw);
				}
			}
			for (var i = 0; i < ColumnCount; i++)
			{
				widths[i] = new GridLength((float)Math.Round(widths[i].Value * 10) / 10);
				ColumnDefinitions[i].Width = widths[i];
			}
		}

		private void SetHorizontalSplitPosition(int index, float percentage)
		{
			percentage = Math.Min(100, Math.Max(0, percentage));
			if (RowCount == 0 || index < 0 || index >= RowCount - 1 || Height <= 0) return;

			var heights = RowDefinitions.OfType<RowDefinition>().Select(x => x.Height).ToList();
			var currentPercent = heights.GetRange(0, index + 1).Sum(x => x.Value);
			if (percentage < currentPercent)
			{
				// <--
				var diff = currentPercent - percentage;
				for (var i = index; i >= 0 && diff > 0; i--)
				{
					var h = heights[i];
					var nh = Math.Max(MinimumViewSize, h.Value - diff);

					heights[i] = new GridLength(nh);
					heights[index + 1] = new GridLength(heights[index + 1].Value + (h.Value - nh));
					diff -= (h.Value - nh);
				}
			}
			else if (percentage > currentPercent)
			{
				// -->
				var diff = percentage - currentPercent;
				for (var i = index + 1; i < heights.Count && diff > 0; i++)
				{
					var h = heights[i];
					var nh = Math.Max(MinimumViewSize, h.Value - diff);
					heights[i] = new GridLength(nh);
					heights[index + 1] = new GridLength(heights[index + 1].Value + (h.Value - nh));
					diff -= (h.Value - nh);
				}
			}
			for (var i = 0; i < RowCount; i++)
			{
				heights[i] = new GridLength((float)Math.Round(heights[i].Value * 10) / 10);
				RowDefinitions[i].Height = heights[i];
			}
		}
	}
}
