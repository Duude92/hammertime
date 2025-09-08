using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Input;

namespace Sledge.Shell.Controls
{
	public class SidebarPanel : StackPanel
	{
		private bool _hidden;

		public bool Hidden
		{
			get { return _hidden; }
			set
			{
				_hidden = value;
				if (_hidden)
				{
					Children.Remove(_panel);
				}
				else
				{
					Children.Add(_panel);
					//Controls.SetChildIndex(_panel, 0);
				}
			}
		}

		public string Text
		{
			get { return (string)_header.Content; }
			set { _header.Content = value; }
		}

		private readonly SidebarHeader _header;
		private readonly Panel _panel;

		public SidebarPanel()
		{
			//AutoSize = true;
			//AutoSizeMode = AutoSizeMode.GrowAndShrink;

			_hidden = false;
			_header = new SidebarHeader { Content = "This is a test", Expanded = !_hidden };//, Dock = DockStyle.Top};
																							//_header.Click += HeaderClicked;
			_header.PointerPressed += HeaderClicked;

			_panel = new Panel();// {Dock = DockStyle.Top, AutoSize = true};
			//Children.Add(_panel);
			Children.Add(_header);

			//CreateHandle();
		}


		public void AddControl(Control c)
		{
			//c.Dock = DockStyle.Top;
			_panel.Children.Add(c);
		}

		private void HeaderClicked(object sender, EventArgs e)
		{
			Hidden = !Hidden;
			_header.Expanded = !Hidden;
		}

		private class SidebarHeader : Label
		{
			private bool _expanded;

			public bool Expanded
			{
				get { return _expanded; }
				set
				{
					_expanded = value;
					//Refresh();
				}
			}

			public SidebarHeader()
			{
				//DoubleBuffered = true;
				_expanded = false;
				Height = 24;
				//Height = Font.Height + 12;
				//AutoSize = false;
				Padding = new Thickness(16, 5, 3, 1);
				//SetStyle(ControlStyles.StandardDoubleClick, false);
				//Cursor = Cursors.Hand;
			}
			public override void Render(DrawingContext context)
			{

				context.DrawLine(new Pen(Brushes.DarkGray), new Point(0, 0), new Point(Width, 0));
				context.DrawLine(new Pen(Brushes.LightGray), new Point(0, 1), new Point(Width, 1));
				if (_mouseIn)
				{
					{
						context.FillRectangle(Brushes.DimGray, new Rect(0, 3, Width, Height - Padding.Bottom));
					}
				}
				else
				{
					{
						context.FillRectangle(Brushes.DimGray, new Rect(0, Height - Padding.Bottom + 2, Width, 1));

					}
				}
				//using (var brush = new SolidBrush(ForeColor))
				{
					//e.Graphics.FillPolygon(brush, GetTrianglePoints());
				}
				base.Render(context);
			}

			private bool _mouseIn;


			protected override void OnPointerEntered(PointerEventArgs e)
			{
				_mouseIn = true;
				//Refresh();
				base.OnPointerEntered(e);
			}

			protected override void OnPointerExited(PointerEventArgs e)
			{
				_mouseIn = false;
				//Refresh();
				base.OnPointerExited(e);
			}

			private Point[] GetTrianglePoints()
			{
				if (_expanded)
				{
					var left = 4;
					var top = 5 + Padding.Top;
					return new[]
					{
						new Point(left, top),
						new Point(left + 8, top),
						new Point(left + 4, top + 4),
						new Point(left + 3, top + 4)
					};
				}
				else
				{
					var left = 6;
					var top = 2 + Padding.Top;
					return new[]
					{
						new Point(left, top),
						new Point(left + 4, top + 4),
						new Point(left + 4, top + 5),
						new Point(left, top + 9)
					};
				}
			}
		}
	}
}
