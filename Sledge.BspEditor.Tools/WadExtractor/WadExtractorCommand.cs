using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sledge.BspEditor.Commands;
using Sledge.BspEditor.Documents;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Menu;
using Sledge.Formats.Bsp;
using Sledge.Formats.Id;
using Sledge.Formats.Texture.Wad;
using Sledge.Formats.Texture.Wad.Lumps;

namespace Sledge.BspEditor.Tools.WadExtractor
{
	[Export(typeof(ICommand))]
	[CommandID("Tools:ExtractWad")]

	[MenuItem("Tools", "", "ExtractWad", "N")]

	internal class WadExtractorCommand : ICommand
	{
		public string Name { get; set; } = "Extract WAD Textures";
		public string Details { get; set; } = "Extracts the textures from a BSP file and saves them to a new WAD file.";

		public Task Invoke(IContext context, CommandParameters parameters)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Filter = "BSP Files|*.bsp";
			ofd.Title = "Select a WAD file to extract";
			ofd.Multiselect = false;
			if (ofd.ShowDialog() != DialogResult.OK) return Task.CompletedTask;
			var stream = File.OpenRead(ofd.FileName);
			BspFile bsp = new BspFile(stream);
			WadFile wad = new WadFile(Formats.Texture.Wad.Version.Wad3);
			var fileName = Path.GetFileNameWithoutExtension(ofd.FileName);
			foreach (var tex in bsp.Textures)
			{
				using (var texStream = new MemoryStream())
				{
					if (string.IsNullOrEmpty(tex.Name) || tex.NumMips == 0) continue;
					MipTexture.Write(new BinaryWriter(texStream), true, tex);
					texStream.Seek(0, SeekOrigin.Begin);
					var br = new BinaryReader(texStream);
					wad.AddLump(tex.Name, new MipTextureLump(br));
				}
			}
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Filter = "WAD Files|*.wad";
			saveFileDialog.Title = "Save the extracted WAD file";
			saveFileDialog.FileName = fileName;
			if (saveFileDialog.ShowDialog() != DialogResult.OK) return Task.CompletedTask;
			using (var saveStream = File.OpenWrite(saveFileDialog.FileName))
			{
				wad.Write(saveStream);
			}
			return Task.CompletedTask;
		}



		public bool IsInContext(IContext context) => true;

	}
}
