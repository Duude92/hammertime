using Sledge.Formats.Texture.Vtf;
using Sledge.Providers.Texture;

namespace HammerTime.Source.Providers.Texture.Vmt
{
	internal class VmtMaterialPackage : TexturePackage
	{
		private ICollection<MaterialTexturePackageReference> _references;
		private HashSet<string> _allTextures = new HashSet<string>();
		public readonly HashSet<string> TranslucentTextures = new HashSet<string>();

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
			var materialReference = _references.FirstOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
			if (materialReference == null) return null;
			var textureFlags = TextureFlags.None;
			if (materialReference.File != null)
			{
				var vtf = new VtfFile(materialReference.File.Open());
				var im = vtf.Images.Last();
				var translucent = materialReference.Material.GetFloat("$translucent");
				textureFlags |= (translucent > 0) ? TextureFlags.Transparent : TextureFlags.None;
				if (translucent > 0)
					TranslucentTextures.Add(name);

				return new TextureItem(name, textureFlags, im.Width, im.Height, name);
			}
			return new TextureItem(name, textureFlags, 1, 1, name);
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
