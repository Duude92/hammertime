using Sledge.Common.Logging;
using Sledge.FileSystem;
using Sledge.Providers.Texture.Wad;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sledge.Providers.Texture.Vmt
{
	[Export("Vmt", typeof(ITexturePackageProvider))]
	internal class VmtMaterialPackageProvider : ITexturePackageProvider
	{
		private const int MATERIALS_INDEX = 11; // \\materials\\

		private const int EXTENSION_COUNT = 4; // .vmt

		public IEnumerable<TexturePackageReference> GetPackagesInFile(IFile file)
		{
			IFile materialsRoot = null;
			try
			{
				materialsRoot = file.GetChild("materials");
			}
			catch
			{
				return new TexturePackageReference[0];
			}
			if (materialsRoot == null || !materialsRoot.Exists) return new TexturePackageReference[0];
			var files = materialsRoot.GetFiles("\\.v((tf)|(mt))$", true).GroupBy(x => x.Extension);
			var materials = files.Single(x => x.Key.Equals("vmt", StringComparison.InvariantCultureIgnoreCase)).ToList();
			var textures = files.Single(x => x.Key.Equals("vtf", StringComparison.InvariantCultureIgnoreCase)).ToDictionary(t =>
			{
				var pName = t.FullPathName.Split(':')[1];
				pName = pName.Substring(MATERIALS_INDEX, pName.Length - MATERIALS_INDEX - EXTENSION_COUNT);
				pName = Path.GetRelativePath(".", pName);
				return pName;
			});
			var refs = materials.Select(m => new MaterialTexturePackageReference(m.NameWithoutExtension, GetTextureFile(m, textures), m)).ToList();
			return refs;
		}
		private IFile GetTextureFile(IFile m, IDictionary<string, IFile> textures)
		{
			var tName = ReadMaterialBaseTexture(m);
			if (string.IsNullOrEmpty(tName)) return null;
			tName = Path.GetRelativePath(".", tName);
			if (textures.TryGetValue(tName, out var tex))
			{
				return tex;
			}
			return null;
		}
		private string ReadMaterialBaseTexture(IFile material)
		{
			using (var stream = material.Open())
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
		public Task<TexturePackage> GetTexturePackage(TexturePackageReference reference)
		{
			throw new NotImplementedException();
		}

		public async Task<IEnumerable<TexturePackage>> GetTexturePackages(IEnumerable<TexturePackageReference> references)
		{
			return await Task.Factory.StartNew(() =>
			{
				return references.AsParallel().Select(reference => reference as MaterialTexturePackageReference).Select(reference =>
				{
					if (!reference.Material.Exists || !string.Equals(reference.Material.Extension, "vmt", StringComparison.InvariantCultureIgnoreCase)) return null;
					try
					{
						return new VmtMaterialPackage(reference);
					}
					catch (Exception ex)
					{
						Log.Debug(nameof(VmtMaterialPackageProvider), $"Invalid VMT file: {reference.Material.Name} - {ex.Message}");
						return null;
					}
				}).Where(x => x != null);
			});
		}
	}
}
