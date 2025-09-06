using Avalonia.Controls;
using LogicAndTrick.Oy;
using Sledge.Common.Translations;
using System;
using Sledge.Shell;
using System.ComponentModel.Composition;
using Avalonia.Interactivity;

namespace Sledge.BspEditor.Tools;

[AutoTranslate]
[Export]
public partial class VertexScaleControl : UserControl
{
	#region Translations

	public string ScaleDistance { set => this.InvokeLater(() => ScaleDistanceLabel.Content = value); }
	public string Reset { set => this.InvokeLater(() => ResetDistanceButton.Content = value); }
	public string ResetOrigin { set => this.InvokeLater(() => ResetOriginButton.Content = value); }

	#endregion

	private bool _freeze;

	public VertexScaleControl()
	{
		_freeze = true;
		InitializeComponent();
		_freeze = false;

		//CreateHandle();
	}

	public void ResetValue()
	{
		_freeze = true;
		DistanceValue.Value = 100;
		Oy.Publish("VertexScaleTool:ValueReset", DistanceValue.Value);
		_freeze = false;
	}

	private void DistanceValueChanged(object sender, RoutedEventArgs e)
	{
		if (_freeze) return;
		Oy.Publish("VertexScaleTool:ValueChanged", DistanceValue.Value);
	}

	private void ResetDistanceClicked(object sender, RoutedEventArgs e)
	{
		ResetValue();
	}

	private void ResetOriginClicked(object sender, RoutedEventArgs e)
	{
		Oy.Publish("VertexScaleTool:ResetOrigin");
	}
}