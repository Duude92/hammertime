using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sledge.Common.Shell.Hooks;
using System.ComponentModel.Composition;
using System.Linq;
using LogicAndTrick.Oy;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Context;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.Media;

namespace Sledge.Shell.Registers
{
	[Export(typeof(IInitialiseHook))]
	public class StatusRegister : IInitialiseHook
	{
		// The menu register needs direct access to the shell
		[Import] private Forms.ShellAv _shell;

		[ImportMany] private IEnumerable<Lazy<IStatusItem>> _statusItems;

		public Task OnInitialise()
		{
			foreach (var si in _statusItems.OrderBy(x => OrderHintAttribute.GetOrderHint(x.Value.GetType())))
			{
				Add(si.Value);
			}

			// Subscribe to context changes
			Oy.Subscribe<IContext>("Context:Changed", ContextChanged);

			return Task.FromResult(0);
		}

		private List<StatusBarItem> _items;

		public StatusRegister()
		{
			_items = new List<StatusBarItem>();
		}

		public void Add(IStatusItem item)
		{
			var si = new StatusBarItem(item);
			_items.Add(si);
			_shell.InvokeLater(() => _shell.StatusStrip.Children.Add(si.Label));
		}

		private Task ContextChanged(IContext context)
		{
			foreach (var si in _items)
			{
				si.ContextChanged(context);
			}
			return Task.FromResult(0);
		}

		private class StatusBarItem
		{
			public IStatusItem Item { get; set; }
			public Label Label { get; set; }

			public StatusBarItem(IStatusItem item)
			{
				Item = item;
				item.TextChanged += TextChanged;
				Label = new Label
				{
					Content = item.Text ?? "",

					Width = item.Width > 0 ? Math.Max(1, item.Width) : double.NaN,
					HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Left,
					BorderThickness = item.HasBorder ? new Avalonia.Thickness(1) : new Avalonia.Thickness(0),
					BorderBrush = Brushes.Black
				};
				//Label = new ToolStripStatusLabel
				//{
				//	Text = item.Text ?? "",
				//	BorderSides = item.HasBorder ? ToolStripStatusLabelBorderSides.All : ToolStripStatusLabelBorderSides.None,
				//	AutoSize = item.Width <= 0,
				//	Spring = item.Width <= 0,
				//	TextAlign = item.Width <= 0 ? ContentAlignment.MiddleLeft : ContentAlignment.MiddleCenter,
				//	Width = Math.Max(1, item.Width)
				//};
			}

			private void TextChanged(object sender, string text)
			{
				Label.InvokeLater(() =>
				{
					Label.Content = text;
				});
			}

			public void ContextChanged(IContext context)
			{
				Label.InvokeLater(() =>
				{
					Label.IsVisible = Item.IsInContext(context);
				});
			}
		}
	}
}
