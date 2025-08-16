using Sledge.FileSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sledge.Providers.Texture.Vmt
{
	internal class VmtMaterialPackage : TexturePackage
	{
		private ICollection<MaterialTexturePackageReference> _references;

		public VmtMaterialPackage(ICollection<MaterialTexturePackageReference> references) : base("Materials", "vmt")
		{
			_references = references;
			foreach (var reference in references)
			{
				Textures.Add(reference.Name);
			}
		}

		public override ITextureStreamSource GetStreamSource()
		{
			return new VtfTextureStreamSourceProvider(this);
		}
		public MaterialTexturePackageReference GetTextureReference(string name)
		{
			var texture = _references.FirstOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
			if (texture == null) return null;
			return texture;
		}

		public override async Task<TextureItem> GetTexture(string name)
		{
			//if (!_file.NameWithoutExtension.Equals(name, StringComparison.InvariantCultureIgnoreCase)) return null;
			var texture = _references.FirstOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
			if(texture == null) return null;

			//var texturePath = ReadMaterialBaseTexture(texture.Material);
			//var file = _file as CompositeFile;
			//var fFile = file.FirstFile as InlinePackageFile;
			return new TextureItem(name, TextureFlags.None, 100, 100, name);
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

		public override async Task<IEnumerable<TextureItem>> GetTextures(IEnumerable<string> names)
		{
			var textures = new HashSet<string>(names);
			textures.IntersectWith(Textures);
			if (!textures.Any()) return new TextureItem[0];

			var list = new List<TextureItem>();

			foreach (var name in textures)
			{
				var entry = await GetTexture(name);
				if (entry == null) continue;
				list.Add(entry);
			}

			return list;
		}
	}
}
