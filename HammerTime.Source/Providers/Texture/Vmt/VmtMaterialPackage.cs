using Sledge.Formats.Texture.Vtf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sledge.Providers.Texture.Vmt
{
	internal class VmtMaterialPackage : TexturePackage
	{
		private ICollection<MaterialTexturePackageReference> _references;
		private HashSet<string> _allTextures = new HashSet<string>();

		public VmtMaterialPackage(string name, ICollection<MaterialTexturePackageReference> references) : base(name, "vmt")
		{
			_references = references;
			foreach (var reference in references)
			{
				if (!reference.HideFromList)
					Textures.Add(reference.Name);
				_allTextures.Add(reference.Name);
			}
		}
		public override bool HasTexture(string name)
		{
			return _allTextures.Contains(name.ToLowerInvariant());
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
			if (texture == null) return null;
			if (texture.File != null)
			{

				var vtf = new VtfFile(texture.File.Open());
				var im = vtf.Images.Last();

				//var texturePath = ReadMaterialBaseTexture(texture.Material);
				//var file = _file as CompositeFile;
				//var fFile = file.FirstFile as InlinePackageFile;
				return new TextureItem(name, TextureFlags.None, im.Width, im.Height, name);
			}
			return new TextureItem(name, TextureFlags.None, 1, 1, name);
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
			textures.IntersectWith(_allTextures);
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
