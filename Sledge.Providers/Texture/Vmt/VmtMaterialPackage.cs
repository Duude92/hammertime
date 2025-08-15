using Sledge.FileSystem;
using Sledge.Providers.Texture.Wad.Format;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sledge.Formats;
using Sledge.Formats.Texture.Vtf;

namespace Sledge.Providers.Texture.Vmt
{
	internal class VmtMaterialPackage : TexturePackage
	{
		private TexturePackageReference _reference;
		private IFile _file;

		public class VtfTextureStreamSourceProvider : ITextureStreamSource
		{
			private IFile _file;
			private VtfTextureStreamSourceProvider _streamSource;
			private VtfFile _vtfFile;

			public VtfTextureStreamSourceProvider(IFile file)
			{
				if (file != null)
					_vtfFile = new VtfFile(file.Open());
			}
			public void Dispose()
			{
				_vtfFile = null;
			}

			public async Task<ICollection<Bitmap>> GetImage(string item, int maxWidth, int maxHeight)
			{
				return await Task.Factory.StartNew(() => _vtfFile.Images.Select(x =>
				{
					Stream stream = new MemoryStream(x.Data);
					return new Bitmap(stream);
				}).ToList());
			}

			public bool HasImage(string item)
			{
				return _vtfFile != null;
			}
		}
		public VmtMaterialPackage(TexturePackageReference reference) : base(reference.File.Name, "vmt")
		{
			_reference = reference;
			_file = reference.File;
			Textures.Add(reference.File.Name);
		}

		public override ITextureStreamSource GetStreamSource()
		{
			return new VtfTextureStreamSourceProvider(_reference.Material);
		}

		public override Task<TextureItem> GetTexture(string name)
		{
			if (!_file.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)) return null;

			var texturePath = ReadMaterialBaseTexture();
			var file = _file as CompositeFile;
			var fFile = file.FirstFile as InlinePackageFile;
			return null;
			//var textureFile = new NativeFile(_file.Parent, texturePath);


			//var entry = wp.GetEntry(name);
			//if (entry == null) return null;
			//string wadname;
			//try
			//{
			//	wadname = wp.File.Name.Substring(0, wp.File.Name.LastIndexOf('.'));
			//}
			//catch (Exception ex)
			//{
			//	wadname = wp.File.Name;
			//}
			//return new TextureItem(entry.Name, TextureFlags.None, (int)entry.Width, (int)entry.Height, wp.File.Name);
		}
		private string ReadMaterialBaseTexture()
		{
			using (var stream = _file.Open())
			{
				using (var reader = new StreamReader(stream, Encoding.UTF8))
				{
					string line;
					while ((line = reader.ReadLine()) != null)
					{
						if (line.Trim().StartsWith("\"$basetexture\"", StringComparison.InvariantCultureIgnoreCase))
						{
							var parts = line.Split(new[] { ' ' }, 2);
							if (parts.Length > 1)
							{
								return parts[1].Trim('"');
							}
						}
					}
				}
			}
			return null;
		}

		public override async Task<IEnumerable<TextureItem>> GetTextures(IEnumerable<string> names)
		{
			var textures = new HashSet<string>(names);
			textures.IntersectWith(Textures);
			if (!textures.Any()) return new TextureItem[0];

			var wp = new WadPackage(_file);
			var list = new List<TextureItem>();

			foreach (var name in textures)
			{
				var entry = wp.GetEntry(name);
				if (entry == null) continue;
				var item = new TextureItem(entry.Name, TextureFlags.None, (int)entry.Width, (int)entry.Height);
				list.Add(item);
			}

			return list;
		}
	}
}
