using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Newtonsoft.Json.Linq;
using Sledge.Common.Shell.Settings;
using System;
using System.Linq;

namespace Sledge.Shell.Settings.Editors;

public partial class DropdownEditor : UserControl, ISettingEditor
{
	private SettingKey _key;
	public event EventHandler<SettingKey> OnValueChanged;

	string ISettingEditor.Label
	{
		get => Label.Content as string;
		set => Label.Content = value;
	}

	public object Value
	{
		get => ((Combobox.SelectedItem as ComboBoxItem)?.Tag as DropdownValue).Value;
		set => Combobox.SelectedItem = Combobox.Items.OfType<ComboBoxItem>().FirstOrDefault(x => (x.Tag as DropdownValue).Value == Convert.ToString(value));
	}

	public object Control => this;

	public SettingKey Key
	{
		get => _key;
		set
		{
			_key = value;
			SetHint(value?.EditorHint);
		}
	}

	public DropdownEditor()
	{
		InitializeComponent();

		Combobox.SelectionChanged += (o, e) => OnValueChanged?.Invoke(this, Key);
	}

	private void SetHint(string hint)
	{
		var spl = (hint ?? "").Split(',');
		foreach (var item in spl.Select(x => new DropdownValue(x)))
		{
			var cb = new ComboBoxItem
			{
				Content = item.Value,
				Tag = item

			};
			Combobox.Items.Add(cb);
		}
	}

	public void Dispose()
	{
		throw new NotImplementedException();
	}

	private class DropdownValue
	{
		public string Value { get; }

		public DropdownValue(string value)
		{
			Value = value;
		}
	}
}
