using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Documents;
using Sledge.Common.Shell.Hooks;
using Sledge.Common.Translations;
using Sledge.Providers.Texture;
using Sledge.Shell;
using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Sledge.BspEditor.Tools;

[AutoTranslate]
[Export(typeof(ISidebarComponent))]
[Export(typeof(IInitialiseHook))]
[OrderHint("B")]
public partial class TextureSidebarPanel : UserControl, ISidebarComponent, IInitialiseHook
{
	public Task OnInitialise()
	{
		Oy.Subscribe<IDocument>("Document:Activated", DocumentActivated);
		Oy.Subscribe<Change>("MapDocument:Changed", DocumentChanged);
		return Task.FromResult(0);
	}

	public string Title { get; set; } = "Texture";
	public object Control => this;

	private string _currentTexture;
	private WeakReference<MapDocument> _activeDocument;

	public string Apply
	{
		set => this.InvokeLater(() => ApplyButton.Content = value);
	}

	public string Browse
	{
		set => this.InvokeLater(() => BrowseButton.Content = value);
	}

	public string Replace
	{
		set => this.InvokeLater(() => ReplaceButton.Content = value);
	}

	public TextureSidebarPanel()
	{
		//CreateHandle();
		InitializeComponent();

		SizeLabel.Content = "";
		NameLabel.Content = "";
		_activeDocument = new WeakReference<MapDocument>(null);
	}

	private void ApplyButtonClicked(object sender, RoutedEventArgs e)
	{
		Oy.Publish("Command:Run", new CommandMessage("BspEditor:ApplyActiveTexture"));
	}

	private void BrowseButtonClicked(object sender, RoutedEventArgs e)
	{
		Oy.Publish("Command:Run", new CommandMessage("BspEditor:BrowseActiveTexture"));
	}

	private void ReplaceButtonClicked(object sender, RoutedEventArgs e)
	{
		Oy.Publish("Command:Run", new CommandMessage("BspEditor:ReplaceTextures"));
	}

	private async Task DocumentActivated(IDocument doc)
	{
		var md = doc as MapDocument;

		_activeDocument = new WeakReference<MapDocument>(md);
		_currentTexture = null;

		this.Invoke(() =>
		{
			//var dis = SelectionPictureBox.Image;
			SelectionPictureBox.Source = null;
			//dis?.Dispose();
		});

		if (md != null)
		{
			await TextureSelected(md.Map.Data.GetOne<ActiveTexture>()?.Name);
		}
	}

	private async Task DocumentChanged(Change change)
	{
		if (_activeDocument.TryGetTarget(out MapDocument t) && change.Document == t)
		{
			await TextureSelected(t.Map.Data.GetOne<ActiveTexture>()?.Name);
		}
	}

	public bool IsInContext(IContext context)
	{
		return context.TryGet("ActiveDocument", out MapDocument _);
	}

	private async Task TextureSelected(string selection)
	{
		if (selection == _currentTexture) return;
		_currentTexture = selection;

		if (!_activeDocument.TryGetTarget(out MapDocument doc)) return;

		Bitmap bmp = null;
		TextureItem texItem = null;

		if (selection != null)
		{
			var tc = await doc.Environment.GetTextureCollection();
			texItem = await tc.GetTextureItem(selection);

			if (texItem != null)
			{
				using (var ss = tc.GetStreamSource())
				{
					var sysbmp = (await ss.GetImage(selection, 256, 256)).First();
					using var ms = new MemoryStream();
					sysbmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
					ms.Seek(0, SeekOrigin.Begin);
					bmp = new Bitmap(ms);
				}
			}
		}

		this.InvokeLater(() =>
		{
			//if (bmp != null)
			//{
			//	if (bmp.Width > SelectionPictureBox.Width || bmp.Height > SelectionPictureBox.Height)
			//	{
			//		SelectionPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
			//	}
			//	else
			//	{
			//		SelectionPictureBox.SizeMode = PictureBoxSizeMode.CenterImage;
			//	}
			//}

			SelectionPictureBox.Source = null;

			SelectionPictureBox.Source = bmp;
			NameLabel.Content = texItem?.Name ?? "";
			SizeLabel.Content = texItem == null ? "" : $"{texItem.Width} x {texItem.Height}";
		});
	}
}
