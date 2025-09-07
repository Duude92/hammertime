using Avalonia.Media;
using IBrush = Sledge.BspEditor.Tools.Brush.IBrush;
using System.Linq;
using Avalonia.Interactivity;

namespace Sledge.BspEditor.Tools;

public partial class FontChooserControl : BrushControl
{
	private string _value = "";

	public string FontName
	{
		get => FontPicker.SelectedItem as string;
		set => FontPicker.SelectedItem = _value = value;
	}

	public string LabelText
	{
		get => Label.Content as string;
		set => Label.Content = value;
	}
	public FontChooserControl(IBrush brush) : base(brush)
	{
		InitializeComponent();

		FontPicker.Items.Clear();
		var a = FontManager.Current.SystemFonts;
		var b = FontManager.FontCollectionScheme;
		var c = FontManager.SystemFontScheme;

		foreach (var item in FontManager.Current.SystemFonts.Select(x => x))
		{
			FontPicker.Items.Add(item);
		}
		FontPicker.SelectedItem = _value = GetFontFamily().Name;
	}

	public FontFamily GetFontFamily()
	{
		return FontManager.Current.SystemFonts.FirstOrDefault(x => x.Name == _value) ?? FontManager.Current.DefaultFontFamily;
	}

	private void ValueChanged(object sender, RoutedEventArgs e)
	{
		_value = (FontPicker.SelectedItem as FontFamily).Name;
		OnValuesChanged(Brush);
	}
}
