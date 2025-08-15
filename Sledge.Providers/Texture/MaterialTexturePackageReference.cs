using Sledge.FileSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sledge.Providers.Texture
{
	internal class MaterialTexturePackageReference : TexturePackageReference
	{
		public IFile Material { get; private set; }

		public MaterialTexturePackageReference(string name, IFile file) : base(name, file)
		{
		}
		public MaterialTexturePackageReference(string name, IFile file, IFile material) : this(name, file)
		{
			Material = material;
		}
	}
}
