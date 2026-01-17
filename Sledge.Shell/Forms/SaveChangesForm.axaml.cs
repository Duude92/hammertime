using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Sledge.Common.Shell.Documents;
using Sledge.Common.Translations;
using System;
using System.Collections.Generic;
using DialogResult = System.Windows.Forms.DialogResult;

namespace Sledge.Shell.Forms;

public partial class SaveChangesForm : Window
{
	public DialogResult DialogResult { get; private set; }
	public SaveChangesForm()
	{
		//Used for designer
	}
	public SaveChangesForm(List<IDocument> unsaved)
	{
		InitializeComponent();
		DialogResult = DialogResult.Cancel;

		foreach (var document in unsaved)
		{
			DocumentList.Items.Add(document.Name + " *");
		}

	}

	private void SaveAllClicked(object sender, EventArgs e)
	{
		DialogResult = DialogResult.Yes;
		Close();
	}

	private void DiscardAllClicked(object sender, EventArgs e)
	{
		DialogResult = DialogResult.No;
		Close();
	}

	private void CancelClicked(object sender, EventArgs e)
	{
		DialogResult = DialogResult.Cancel;
		Close();
	}

	public void Translate(ITranslationStringProvider translation)
	{
		this.InvokeLater(() => {
			SaveAllButton.Content = translation.GetString(typeof(SaveChangesForm).FullName + ".SaveAll");
			DiscardButton.Content = translation.GetString(typeof(SaveChangesForm).FullName + ".DiscardAll");
			CancelButton.Content = translation.GetString(typeof(SaveChangesForm).FullName + ".Cancel");
			UnsavedChangesLabel.Text = translation.GetString(typeof(SaveChangesForm).FullName + ".UnsavedChangesMessage");
			Title = translation.GetString(typeof(SaveChangesForm).FullName + ".Title");
		});
	}
}
