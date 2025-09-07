using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Sledge.BspEditor.Tools.Brush;
using Sledge.BspEditor.Tools.Brush.Brushes.Controls;
using System;

namespace Sledge.BspEditor.Tools;

public partial class TextControl : BrushControl
{
	private string _value;

	public string EnteredText
	{
		get => TextBox.Text;
		set => TextBox.Text = _value = value;
	}

	public string LabelText
	{
		get => Label.Content as string;
		set => Label.Content = value;
	}

	public TextControl(IBrush brush) : base(brush)
	{
		InitializeComponent();
		_value = TextBox.Text;
	}

	public string GetValue()
	{
		return _value;
	}

	private void ValueChanged(object sender, RoutedEventArgs e)
	{
		_value = TextBox.Text;
		OnValuesChanged(Brush);
	}
}