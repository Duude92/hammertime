using Avalonia.Controls;
using Avalonia.Media;
using Sledge.Common.Shell.Settings;
using System;
using System.Text.RegularExpressions;
using Sledge.Common;
using Avalonia.Controls.Converters;

namespace Sledge.Shell.Settings.Editors;

public partial class ColorEditor : UserControl, ISettingEditor
{
	public event EventHandler<SettingKey> OnValueChanged;

	string ISettingEditor.Label
	{
		get => Label.Content as string;
		set => Label.Content = value;
	}

	public object Value
	{
		get => ColorPanel.Color;
		set
		{
			var systemColor = value as System.Drawing.Color?;
			var v = systemColor?.ToAvaloniaColor() ?? Colors.Black;
			ColorPanel.Color = v;
			var c = ColorPanel.Color;
			HexBox.Text = $@"{c.R:X2}{c.G:X2}{c.B:X2}";
		}
	}

	public object Control => this;
	public SettingKey Key { get; set; }
	private ColorToHexConverter converter = new();

	public ColorEditor()
	{
		InitializeComponent();
		ColorPanel.ColorChanged += PickColor;
		HexBox.LostFocus += HexUnfocused;
		HexBox.TextChanged += UpdateHex;
	}

	public void Dispose()
	{
		throw new NotImplementedException();
	}

	private void PickColor(object sender, EventArgs e)
	{
		var c = ColorPanel.Color;
		HexBox.Text = $@"{c.R:X2}{c.G:X2}{c.B:X2}";
		OnValueChanged?.Invoke(this, Key);
	}

	private void UpdateHex(object sender, EventArgs e)
	{
		// Match #000 and #000000
		if (Regex.IsMatch(HexBox.Text, "^([0-9A-Fa-f]{3}){1,2}$"))
		{
			var color = converter.ConvertBack(HexBox.Text, typeof(Color), null, System.Globalization.CultureInfo.CurrentCulture);
			ColorPanel.Color = (color as Color?) ?? Colors.Black;
			OnValueChanged?.Invoke(this, Key);
		}
	}

	private void HexUnfocused(object sender, EventArgs e)
	{
		if (!Regex.IsMatch(HexBox.Text, "^[0-9A-Fa-f]{6}$"))
		{
			var c = ColorPanel.Color;
			HexBox.Text = $@"{c.R:X2}{c.G:X2}{c.B:X2}";
		}
	}
}
