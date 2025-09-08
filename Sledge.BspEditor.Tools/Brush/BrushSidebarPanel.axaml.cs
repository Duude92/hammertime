using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Sledge.BspEditor.Tools.Brush;
using Sledge.BspEditor.Tools.Brush.Brushes.Controls;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Hooks;
using Sledge.Common.Translations;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Sledge.Shell;
using System.Threading.Tasks;
using System.Linq;
using LogicAndTrick.Oy;
using Sledge.Common.Shell.Context;
using Avalonia.Interactivity;

namespace Sledge.BspEditor.Tools;

[Export(typeof(ISidebarComponent))]
[Export(typeof(IInitialiseHook))]
[OrderHint("F")]
[AutoTranslate]
public partial class BrushSidebarPanel : UserControl, ISidebarComponent, IInitialiseHook
{
	[ImportMany] private IEnumerable<Lazy<IBrush>> _brushes;
	[Import] private BrushTool _tool;

	private IBrush _selectedBrush;
	private readonly List<BrushControl> _currentControls;

	public string Title { get; set; } = "Brush";
	public object Control => this;

	#region Translations

	public string BrushType { set => this.InvokeLater(() => BrushTypeLabel.Content = value); }
	public string RoundVertices { set => this.InvokeLater(() => RoundCreatedVerticesCheckbox.Content = value); }

	#endregion

	public async Task OnInitialise()
	{
		_selectedBrush = null;
		this.InvokeLater(() =>
		{
			//BrushTypeList.BeginUpdate();
			BrushTypeList.Items.Clear();
			foreach (var brush in _brushes.OrderBy(x => OrderHintAttribute.GetOrderHint(x.Value.GetType())))
			{
				if (_selectedBrush == null) _selectedBrush = brush.Value;
				BrushTypeList.Items.Add(new BrushWrapper(brush.Value));
			}

			BrushTypeList.SelectedIndex = 0;
			//BrushTypeList.EndUpdate();

			UpdateControls();
		});
	}

	public BrushSidebarPanel()
	{
		InitializeComponent();
		//CreateHandle();

		_currentControls = new List<BrushControl>();

		Oy.Subscribe<BrushTool>("BrushTool:ResetBrushType", ResetBrushType);
	}

	private async Task ResetBrushType(BrushTool bt)
	{
		this.InvokeLater(() =>
		{
			if (BrushTypeList.Items.Count > 0)
			{
				BrushTypeList.SelectedIndex = 0;
				UpdateControls();
			}
		});
	}

	public bool IsInContext(IContext context)
	{
		return context.TryGet("ActiveTool", out BrushTool _);
	}

	private void UpdateControls()
	{
		_currentControls.ForEach(x => x.ValuesChanged -= ControlValuesChanged);
		_currentControls.ForEach(x => MainPanel.Children.Remove(x));
		_currentControls.Clear();

		if (_selectedBrush == null) return;

		RoundCreatedVerticesCheckbox.IsEnabled = _selectedBrush.CanRound;

		_currentControls.AddRange(_selectedBrush.GetControls().Reverse());
		for (var i = 0; i < _currentControls.Count; i++)
		{
			var ctrl = _currentControls[i];
			//ctrl.Dock = DockStyle.Top;
			ctrl.ValuesChanged += ControlValuesChanged;
			MainPanel.Children.Add(ctrl);
			//MainPanel.Children.SetChildIndex(ctrl, i);
		}

		OnBrushChange();
	}

	private void OnBrushChange()
	{
		Oy.Publish("Context:Add", new ContextInfo("BrushTool:ActiveBrush", _selectedBrush));
	}

	private void OnValuesChange()
	{
		Oy.Publish("BrushTool:ValuesChanged", new object());
	}

	private void ControlValuesChanged(object sender, IBrush brush)
	{
		OnValuesChange();
	}
	
	private void RoundCreatedVerticesChanged(object sender, RoutedEventArgs e)
	{
		_tool.RoundVertices = RoundCreatedVerticesCheckbox.IsChecked.Value;
		OnValuesChange();
	}

	private void SelectedBrushTypeChanged(object sender, RoutedEventArgs e)
	{
		_selectedBrush = (BrushTypeList.SelectedItem as BrushWrapper)?.Brush;
		UpdateControls();
	}

	private class BrushWrapper
	{
		public IBrush Brush { get; set; }

		public BrushWrapper(IBrush brush)
		{
			Brush = brush;
		}

		public override string ToString()
		{
			return Brush.Name;
		}
	}
}
