using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using LogicAndTrick.Oy;
using Sledge.Common.Translations;
using System;
using System.ComponentModel.Composition;
using Sledge.Shell;
using Avalonia.Interactivity;

namespace Sledge.BspEditor.Tools;

[AutoTranslate]
[Export]
public partial class VertexEditFaceControl : UserControl
{
	#region Translations

	public string WithSelectedFaces { set => this.InvokeLater(() => WithSelectedFacesLabel.Content = value); }
	public string Units { set => this.InvokeLater(() => { UnitsLabel1.Content = value; UnitsLabel2.Content = value; }); }
	public string PokeBy { set => this.InvokeLater(() => PokeByLabel.Content = value); }
	public string BevelBy { set => this.InvokeLater(() => BevelByLabel.Content = value); }
	public string Poke { set => this.InvokeLater(() => PokeFaceButton.Content = value); }
	public string Bevel { set => this.InvokeLater(() => BevelButton.Content = value); }

	#endregion

	public VertexEditFaceControl()
	{
		InitializeComponent();
		//CreateHandle();
	}

	private void BevelButtonClicked(object sender, RoutedEventArgs e)
	{
		Oy.Publish("VertexEditFaceTool:Bevel", (int)BevelValue.Value);
	}

	private void PokeFaceButtonClicked(object sender, RoutedEventArgs e)
	{
		Oy.Publish("VertexEditFaceTool:Poke", (int)PokeFaceCount.Value);
	}

	private void ExtrudeButtonClicked(object sender, RoutedEventArgs e)
	{
		Oy.Publish("VertexEditFaceTool:Extrude", (int)ExtrudeValue.Value);

	}
}