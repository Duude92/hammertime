using Sledge.Common.Logging;
using Sledge.FileSystem;
using Sledge.Providers.Texture.Wad;
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
			var envRoot = file.GetChild("gfx");
			envRoot = envRoot.GetChild("env");




			if (envRoot==null || !envRoot.Exists) return new TexturePackageReference[0];

			var files = envRoot.GetFiles();
			var groupsInitial = files.GroupBy(x => x.Extension);
			IEnumerable<IGrouping<string, IFile>> targetGroups = null;
            foreach (var group in groupsInitial)
            {
				if (targetGroups == null) targetGroups = group.GroupBy(x => x.NameWithoutExtension.Substring(0, x.NameWithoutExtension.Length - 2));
				else targetGroups = targetGroups.Union(group.GroupBy(x => x.NameWithoutExtension.Substring(0, x.NameWithoutExtension.Length - 2)));
            }



			return targetGroups.Select(x => new TexturePackageReference(x.Key, new CompositeFile(envRoot, x.Select(y=>((CompositeFile)y).FirstFile))));

		}

		public Task<TexturePackage> GetTexturePackage(TexturePackageReference reference)
		{
			throw new NotImplementedException();
		}

		public async Task<IEnumerable<TexturePackage>> GetTexturePackages(IEnumerable<TexturePackageReference> references)
		{
			return null;
		}
	}
}
