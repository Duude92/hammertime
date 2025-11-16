using HammerTime.Source.Providers.Texture.Vmt;
using Sledge.BspEditor.Environment;
using Sledge.Providers.Texture;

namespace HammerTime.Source.BspEditor.Environment.Source
{
	internal class SourceTextureCollection : TextureCollection
	{
		public SourceTextureCollection(IEnumerable<TexturePackage> packages) : base(packages)
		{
		}
		public override IEnumerable<string> GetBrowsableTextures()
		{
			var hs = new HashSet<string>();
			foreach (var pack in Packages.Where(x => x.Type == "vmt")) hs.UnionWith(pack.Textures);
			return hs;
		}

		public override IEnumerable<string> GetDecalTextures()
		{
			return new string[0];
		}

		public override float GetOpacity(string name)
		{
			var hasTransparency = Packages.OfType<VmtMaterialPackage>().Select(p => p.TranslucentTextures.FirstOrDefault(t => t.Equals(name, StringComparison.InvariantCultureIgnoreCase))).Where(x => x != null).Any();
			return hasTransparency ? 0.5f : 1;
		}

		public override IEnumerable<string> GetSpriteTextures()
		{
			return new string[0];
		}

		public override bool IsClipTexture(string name)
		{
			return false;
		}

		public override bool IsNullTexture(string name)
		{
			return false;
		}
	}
}
