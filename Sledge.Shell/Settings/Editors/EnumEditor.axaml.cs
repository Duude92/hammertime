using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Sledge.Common.Shell.Settings;
using System;
using System.Linq;
using System.Reflection;

namespace Sledge.Shell.Settings.Editors;

public partial class EnumEditor : UserControl, ISettingEditor
{
	private readonly Type _enumType;
	public event EventHandler<SettingKey> OnValueChanged;


	string ISettingEditor.Label
	{
		get => Label.Content as String;
		set => Label.Content = value;
	}

	public object Value
	{
		get => (Combobox.SelectedItem as EnumValue)?.Value;
		set => Combobox.SelectedItem = Combobox.Items.OfType<EnumValue>().FirstOrDefault(x => x.Value == Convert.ToString(value));
	}

	public object Control => this;
	public SettingKey Key { get; set; }

	public EnumEditor(Type enumType)
	{
		_enumType = enumType;
		InitializeComponent();

		//Combobox.DisplayMember = "Label";
		//Combobox.ValueMember = "Value";

		var values = enumType.GetFields(BindingFlags.Static | BindingFlags.Public);

		foreach (var item in values.Select(x => new EnumValue(x)).OfType<object>())
		{
			Combobox.Items.Add(item);
		}

		Combobox.SelectionChanged += (o, e) => OnValueChanged?.Invoke(this, Key);
	}

	private class EnumValue
	{
		public string Label { get; set; }
		public string Value { get; set; }
		public EnumValue(FieldInfo field)
		{
			Label = field.Name;
			Value = field.Name;
		}
	}

	public void Dispose()
	{
		throw new NotImplementedException();
	}
}
