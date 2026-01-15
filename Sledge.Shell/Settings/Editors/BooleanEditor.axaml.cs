using Avalonia.Controls;
using Sledge.Common.Shell.Settings;
using System;

namespace Sledge.Shell.Settings.Editors;

public partial class BooleanEditor : UserControl, ISettingEditor
{
	public event EventHandler<SettingKey> OnValueChanged;

	string ISettingEditor.Label
	{
		get => Checkbox.Content as string;
		set => Checkbox.Content = value;
	}

	public object Value
	{
		get => Checkbox.IsChecked;
		set => Checkbox.IsChecked = Convert.ToBoolean(value);
	}

	public object Control => this;
	public SettingKey Key { get; set; }

	public BooleanEditor()
	{
		InitializeComponent();

		Checkbox.IsCheckedChanged += (o, e) => OnValueChanged?.Invoke(this, Key);
	}

	public void Dispose()
	{
		throw new NotImplementedException();
	}
}
