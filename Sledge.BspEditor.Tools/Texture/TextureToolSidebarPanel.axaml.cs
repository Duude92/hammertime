using Avalonia.Controls;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations.Data;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.BspEditor.Tools.Texture;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Context;
using Sledge.Common.Translations;
using Sledge.DataStructures.Geometric;
using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Sledge.Shell;
using Avalonia.Interactivity;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives;

namespace Sledge.BspEditor.Tools;

[AutoTranslate]
[Export(typeof(ISidebarComponent))]
[OrderHint("F")]
public partial class TextureToolSidebarPanel : UserControl, ISidebarComponent
{
	[Import] private TextureTool _tool;

	public string Title { get; set; } = "Texture Power Tools";
	public object Control => this;

	#region Translations

	public string RandomiseShiftValues
	{
		set => this.InvokeLater(() => RandomiseShiftValuesGroup.Header = value);
	}

	public string Min
	{
		set => this.InvokeLater(() => MinLabel.Content = value);
	}

	public string Max
	{
		set => this.InvokeLater(() => MaxLabel.Content = value);
	}

	public string RandomiseX
	{
		set => this.InvokeLater(() => RandomShiftXButton.Content = value);
	}

	public string RandomiseY
	{
		set => this.InvokeLater(() => RandomShiftYButton.Content = value);
	}

	public string FitToMultipleTiles
	{
		set => this.InvokeLater(() => FitGroup.Header = value);
	}

	public string TimesToTile
	{
		set => this.InvokeLater(() => TimesToTileLabel.Content = value);
	}

	public string Fit
	{
		set => this.InvokeLater(() => TileFitButton.Content = value);
	}

	#endregion

	public TextureToolSidebarPanel()
	{
		InitializeComponent();
		//CreateHandle();
	}

	public bool IsInContext(IContext context)
	{
		return context.TryGet("ActiveTool", out TextureTool _);
	}
	private void RandomShiftXButtonClicked(object sender, RoutedEventArgs e)
	{
		var document = _tool.GetDocument();
		var fs = document?.Map.Data.GetOne<FaceSelection>();
		if (fs == null || fs.IsEmpty) return;

		var min = (int)RandomShiftMin.Value;
		var max = (int)RandomShiftMax.Value;

		var rand = new Random();

		var edit = new Transaction();
		foreach (var it in fs.GetSelectedFaces())
		{
			var clone = (Face)it.Value.Clone();
			clone.Texture.XShift = rand.Next(min, max + 1); // Upper bound is exclusive

			edit.Add(new RemoveMapObjectData(it.Key.ID, it.Value));
			edit.Add(new AddMapObjectData(it.Key.ID, clone));
		}

		MapDocumentOperation.Perform(document, edit);
	}

	private void RandomShiftYButtonClicked(object sender, RoutedEventArgs e)
	{
		var document = _tool.GetDocument();
		var fs = document?.Map.Data.GetOne<FaceSelection>();
		if (fs == null || fs.IsEmpty) return;

		var min = (int)RandomShiftMin.Value;
		var max = (int)RandomShiftMax.Value;

		var rand = new Random();

		var edit = new Transaction();
		foreach (var it in fs.GetSelectedFaces())
		{
			var clone = (Face)it.Value.Clone();
			clone.Texture.YShift = rand.Next(min, max + 1); // Upper bound is exclusive

			edit.Add(new RemoveMapObjectData(it.Key.ID, it.Value));
			edit.Add(new AddMapObjectData(it.Key.ID, clone));
		}

		MapDocumentOperation.Perform(document, edit);
	}

	private void TileFitButtonClicked(object sender, RoutedEventArgs e)
	{
		ApplyFit();
	}

	private async Task ApplyFit()
	{
		var document = _tool.GetDocument();
		var fs = document?.Map.Data.GetOne<FaceSelection>();
		if (fs == null || fs.IsEmpty) return;

		var tc = await document.Environment.GetTextureCollection();
		if (tc == null) return;

		var tileX = (int)TileFitX.Value;
		var tileY = (int)TileFitY.Value;

		var edit = new Transaction();
		foreach (var it in fs.GetSelectedFaces())
		{
			var clone = (Face)it.Value.Clone();

			var tex = await tc.GetTextureItem(clone.Texture.Name);
			if (tex == null) continue;

			clone.Texture.FitToPointCloud(tex.Width, tex.Height, new Cloud(clone.Vertices), tileX, tileY);

			edit.Add(new RemoveMapObjectData(it.Key.ID, it.Value));
			edit.Add(new AddMapObjectData(it.Key.ID, clone));
		}

		if (!edit.IsEmpty) await MapDocumentOperation.Perform(document, edit);
	}
}
