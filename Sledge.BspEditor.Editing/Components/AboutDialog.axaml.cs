using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Sledge.BspEditor.Documents;
using System.Diagnostics;

namespace Sledge.BspEditor.Editing;

public partial class AboutDialog : Window
{
	public AboutDialog()
	{
		InitializeComponent();
		VersionLabel.Text = FileVersionInfo.GetVersionInfo(typeof(MapDocument).Assembly.Location).FileVersion;

	}
	public void OkButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
	{
		this.Close();
	}


}