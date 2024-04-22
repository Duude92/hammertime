using Sledge.FileSystem;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sledge.Providers.Texture.Env
{
	[Export("Env", typeof(ITexturePackageProvider))]
	public class EnvTextureProvider : ITexturePackageProvider
	{
		public IEnumerable<TexturePackageReference> GetPackagesInFile(IFile file)
		{
			var envRoot = file.GetChild("/gfx/env");




			if (envRoot==null || !envRoot.Exists) return new TexturePackageReference[0];

			if (!file.IsContainer) return file.Extension == "wad" ? new[] { new TexturePackageReference(file.Name, file) } : new TexturePackageReference[0];
			return file.GetFilesWithExtension("wad").Select(x => new TexturePackageReference(x.Name, x));
		}

		public Task<TexturePackage> GetTexturePackage(TexturePackageReference reference)
		{
			throw new NotImplementedException();
		}

		public Task<IEnumerable<TexturePackage>> GetTexturePackages(IEnumerable<TexturePackageReference> references)
		{
			throw new NotImplementedException();
		}
	}
}
