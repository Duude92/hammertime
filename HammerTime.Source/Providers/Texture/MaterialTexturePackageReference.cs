using Sledge.FileSystem;
using Sledge.Providers.Texture;

namespace HammerTime.Source.Providers.Texture
{
	internal class MaterialTexturePackageReference : TexturePackageReference
	{
		public IFile Material { get; private set; }
		public bool HideFromList { get; set; }

		public MaterialTexturePackageReference(string name, IFile file) : base(name, file)
		{
		}
		public MaterialTexturePackageReference(string name, IFile file, IFile material, bool hideFromList) : this(name, file)
		{
			Material = material;
			HideFromList = hideFromList;
		}
	}
}
