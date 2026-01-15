using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Sledge.Common.Shell.Settings;
using Sledge.Shell.Registers;
using System;

namespace Sledge.Shell.Settings.Editors;

public partial class FileAssociationsEditor : UserControl, ISettingEditor
{
	public event EventHandler<SettingKey> OnValueChanged;

	public string Label { get; set; }

	private DocumentRegister.FileAssociations _bindings;

	public object Value
	{
		get => _bindings;
		set
		{
			_bindings = ((DocumentRegister.FileAssociations)value).Clone();
			UpdateAssociationsList();
		}
	}

	public object Control => this;
	public SettingKey Key { get; set; }

	public FileAssociationsEditor()
	{
		InitializeComponent();
		//Anchor = AnchorStyles.Top | AnchorStyles.Bottom;
	}

	private void UpdateAssociationsList()
	{
		CheckboxPanel.Children.Clear();

		if (_bindings == null) return;

		foreach (var b in _bindings)
		{
			var checkbox = new CheckBox
			{
				Content = b.Key,
				IsChecked = b.Value,
				Tag = b.Key,
				Margin = new Thickness(2)
			};
			checkbox.IsCheckedChanged += SetAssociation;
			CheckboxPanel.Children.Add(checkbox);
		}
	}

	private void SetAssociation(object sender, EventArgs e)
	{
		var assoc = (sender as CheckBox)?.IsChecked ?? false;
		_bindings[(sender as CheckBox)?.Tag as string ?? ""] = assoc;
		OnValueChanged?.Invoke(this, Key);
	}

	public void Dispose()
	{
		throw new NotImplementedException();
	}
}