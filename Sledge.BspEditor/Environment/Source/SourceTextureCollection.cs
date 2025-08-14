using Sledge.Providers.Texture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sledge.BspEditor.Environment.Source
{
	internal class SourceTextureCollection : TextureCollection
	{
		public SourceTextureCollection(IEnumerable<TexturePackage> packages) : base(packages)
		{
		}
		public override IEnumerable<string> GetBrowsableTextures()
		{
			var hs = new HashSet<string>();
			foreach (var pack in Packages.Where(x => x.Type == "Wad3")) hs.UnionWith(pack.Textures);
			return Packages.SelectMany(x=>x.Textures);
		}

		public override IEnumerable<string> GetDecalTextures()
		{
			return new string[0];
		}

		public override float GetOpacity(string name)
		{
			return 1;
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
