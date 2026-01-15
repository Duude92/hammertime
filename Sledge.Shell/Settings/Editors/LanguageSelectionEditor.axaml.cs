using Avalonia.Controls;
using Sledge.Common.Shell.Settings;
using Sledge.Common.Translations;
using System;
using System.Linq;

namespace Sledge.Shell.Settings.Editors;

public partial class LanguageSelectionEditor : UserControl, ISettingEditor
{
	public event EventHandler<SettingKey> OnValueChanged;

	string ISettingEditor.Label
	{
		get => Label.Content as string;
		set => Label.Content = value;
	}

	public object Value
	{
		get => ((Combobox.SelectedItem as ComboBoxItem)?.Tag as LanguageValue ).Value;
		set => Combobox.SelectedItem = Combobox.Items.OfType<ComboBoxItem>().FirstOrDefault(x => (x.Tag as LanguageValue).Value == Convert.ToString(value));
	}

	public object Control => this;
	public SettingKey Key { get; set; }

	public LanguageSelectionEditor(TranslationStringsCatalog catalog)
	{
		InitializeComponent();

		var values = catalog.Languages.Values;
		foreach (var item in values.Select(x => new LanguageValue(x)))
		{
			var cb = new ComboBoxItem
			{
				Content = item.Label,
				Tag = item
				
			};
			Combobox.Items.Add(cb);
		}

		Combobox.SelectionChanged += (o, e) => OnValueChanged?.Invoke(this, Key);
	}

	private class LanguageValue
	{
		public string Label { get; set; }
		public string Value { get; set; }
		public LanguageValue(Language lang)
		{
			Label = String.IsNullOrWhiteSpace(lang.Description) ? lang.Code : lang.Description;
			Value = lang.Code;
		}
	}

	public void Dispose()
	{
		throw new NotImplementedException();
	}
}
