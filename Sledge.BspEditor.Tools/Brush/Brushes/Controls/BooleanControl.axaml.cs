using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Sledge.BspEditor.Tools.Brush;
using System;

namespace Sledge.BspEditor.Tools;

public partial class BooleanControl : BrushControl
{
	private bool _value;

	public bool Checked
	{
		get => _value;
		set => Checkbox.IsChecked = _value = value;
	}

	public string LabelText
	{
		get => Checkbox.Content as string;
		set => Checkbox.Content = value;
	}

	public bool ControlEnabled
	{
		get => Checkbox.IsEnabled;
		set => Checkbox.IsEnabled = value;
	}

	public BooleanControl(IBrush brush) : base(brush)
	{
		InitializeComponent();
		_value = Checkbox.IsChecked.Value;
	}

	public bool GetValue()
	{
		return _value;
	}

	private void ValueChanged(object sender, EventArgs e)
	{
		_value = Checkbox.IsChecked.Value;
		OnValuesChanged(Brush);
	}
}
