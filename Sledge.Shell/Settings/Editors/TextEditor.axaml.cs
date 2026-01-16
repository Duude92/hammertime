using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Sledge.Common.Shell.Settings;
using System;

namespace Sledge.Shell.Settings.Editors;

public partial class TextEditor : UserControl, ISettingEditor
{
	private SettingKey _key;
	public event EventHandler<SettingKey> OnValueChanged;

	string ISettingEditor.Label
	{
		get => Label.Content as String;
		set => Label.Content = value;
	}

	public object Value
	{
		get => Textbox.Text;
		set => Textbox.Text = Convert.ToString(value);
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

	private void SetHint(string hint)
	{
		switch (hint)
		{
			case "Directory":
			case "File":
				BrowseButton.IsVisible = true;
				//Textbox.Width = BrowseButton.Bounds.Left - Textbox.Bounds.Left - 5;
				break;
			default:
				BrowseButton.IsVisible = false;
				//Textbox.Width = BrowseButton.Bounds.Right - Textbox.Bounds.Left;
				break;
		}
	}

	public TextEditor()
	{
		InitializeComponent();
		Textbox.TextChanged += (o, e) => OnValueChanged?.Invoke(this, Key);
	}

	private async void BrowseButtonClicked(object sender, RoutedEventArgs e)
	{
		if (Key.EditorHint == "Directory") BrowseDirectory();
		if (Key.EditorHint == "File") BrowseFile();
	}

	private async void BrowseDirectory()
	{
		var topLevel = TopLevel.GetTopLevel(this);
		var storageProvider = topLevel?.StorageProvider;

		var startFolder = await storageProvider.TryGetFolderFromPathAsync(Textbox.Text);


		var gameFolder = await storageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions { Title = "Select game base folder", SuggestedStartLocation = startFolder });
		if (gameFolder != null && gameFolder.Count > 0)
		{
			Textbox.Text = gameFolder[0].Path.LocalPath;
			return;
		}
	}

	private async void BrowseFile()
	{
		var topLevel = TopLevel.GetTopLevel(this);
		var storageProvider = topLevel?.StorageProvider;

		var startFolder = await storageProvider.TryGetFolderFromPathAsync(Textbox.Text);

		var file = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions { Title = "Select game base folder", SuggestedStartLocation = startFolder, AllowMultiple = false });

		if(file != null && file.Count > 0)
		{
			Textbox.Text = file[0].Path.LocalPath;
			return;
		}
	}

	public void Dispose()
	{
		throw new NotImplementedException();
	}
}
