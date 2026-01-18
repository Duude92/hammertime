using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using DialogResult = System.Windows.Forms.DialogResult;

namespace Sledge.Shell;

public partial class AddLanguageForm : Window
{
	private Window _parent;

	public string Code => txtCode.Text;
	public string Description => txtDescription.Text;

	public DialogResult DialogResult { get; private set; }
	public AddLanguageForm(Window parent)
	{
		_parent = parent;
		InitializeComponent();
		btnOK.IsEnabled = false;
		DialogResult = DialogResult.Cancel;
	}
	private void OKClicked(object sender, RoutedEventArgs e)
	{
		DialogResult = DialogResult.OK;
		Close();
	}

	private void CancelClicked(object sender, RoutedEventArgs e)
	{
		Close();
	}

	private void FormTextChanged(object sender, TextChangedEventArgs e)
	{
		btnOK.IsEnabled = Code.Length > 0 && Description.Length > 0;
	}
	public Task<DialogResult> ShowDialog()
	{
		return ShowDialog<DialogResult>(_parent);
	}
}
