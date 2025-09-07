using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Sledge.BspEditor.Tools.Brush;
using System;

namespace Sledge.BspEditor.Tools;

public partial class NumericControl : BrushControl
{
	private decimal _value;

	public decimal Minimum
	{
		get => Numeric.Minimum;
		set => Numeric.Minimum = value;
	}

	public decimal Maximum
	{
		get => Numeric.Maximum;
		set => Numeric.Maximum = value;
	}

	public decimal Value
	{
		get => Numeric.Value.Value;
		set => Numeric.Value = _value = value;
	}

	public string LabelText
	{
		get => Label.Content as string;
		set => Label.Content = value;
	}

	public bool ControlEnabled
	{
		get => Numeric.IsEnabled;
		set => Numeric.IsEnabled = value;
	}

	public int Precision
	{
		get => 2;
		set { return; }
	}
	//public int Precision
	//{
	//	get => Numeric.DecimalPlaces;
	//	set => Numeric.DecimalPlaces = value;
	//}

	public decimal Increment
	{
		get => Numeric.Increment;
		set => Numeric.Increment = value;
	}

	public NumericControl(IBrush brush) : base(brush)
	{
		InitializeComponent();
		_value = Numeric.Value.Value;
	}

	public decimal GetValue()
	{
		return _value;
	}

	private void ValueChanged(object sender, RoutedEventArgs e)
	{
		_value = Numeric?.Value.Value ?? 0;
		OnValuesChanged(Brush);
	}
}
