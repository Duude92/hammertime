using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Sledge.BspEditor.Environment;
using System;
using System.Collections.Generic;
using System.Linq;
using DialogResult = System.Windows.Forms.DialogResult;
using System.Xml;
using Avalonia.Interactivity;

namespace Sledge.BspEditor;

public partial class EnvironmentSelectionForm : Window, IDisposable
{
	private readonly List<SerialisedEnvironment> _environments;

	public SerialisedEnvironment SelectedEnvironment { get; private set; }

	public EnvironmentSelectionForm(List<SerialisedEnvironment> environments)
	{
		_environments = environments;
		InitializeComponent();
		SelectedEnvironment = null;

		GameTable.Children.Clear();
		foreach (var g in _environments.GroupBy(x => x.Type).OrderBy(x => x.Key))
		{
			//GameTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));
			var esp = new StackPanel { Orientation = Avalonia.Layout.Orientation.Horizontal, Margin = new Thickness(5) };
			esp.Children.Add(new Label { Content = g.Key, FontWeight = Avalonia.Media.FontWeight.Bold /*Font = new Font(Font, FontStyle.Bold), Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft */});
			esp.Children.Add(new Label { Content = "" });
			GameTable.Children.Add(esp);
			foreach (var game in g)
			{
				var gsp = new StackPanel { Orientation = Avalonia.Layout.Orientation.Horizontal };

				//GameTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
				gsp.Children.Add(new Label { Content = game.Name,/* Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft*/ });
				var btn = new Button { Content = @">>", Width = 40, Margin = new Thickness(5,2,5,2)};
				var btnGame = game;
				btn.Click += (s, ev) => SelectGame(btnGame);
				gsp.Children.Add(btn);
				GameTable.Children.Add(gsp);
			}
		}

		if (!_environments.Any())
		{
			SelectedEnvironment = null;
			Close();
		}
	}

	private void SelectGame(SerialisedEnvironment environment)
	{
		SelectedEnvironment = environment;
		Close(DialogResult.OK);
	}

	public void Dispose()
	{
		return;
	}
}