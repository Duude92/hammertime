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

			var refs = new List<TexturePackageReference>()
			{
				new TexturePackageReference("Materials", materialsRoot)
			};
			return refs;
		}
		private IFile GetTextureFile(IFile m, IDictionary<string, IFile> textures)
		{
			var tName = ReadMaterialBaseTexture(m);
			if (string.IsNullOrEmpty(tName)) return null;
			tName = Path.GetRelativePath(".", tName.ToLower());
			if (textures.TryGetValue(tName, out var tex))
			{
				return tex;
			}
			return null;
		}
		HashSet<string> types = new HashSet<string>
			{
				"unlitgeneric",
				"lightmappedgeneric",
				"lightmappedreflective",
				"water",
				"sprite",
				"decalmodulate",
				"modulate",
				"subrect",
				"worldvertextransition",
				"lightmapped_4wayblend",
				"unlittwotexture",
				"worldtwotextureblend",
				"skyfog"
			};
		private string ReadMaterialBaseTexture(IFile material)
		{
			var mName = material.NameWithoutExtension;
			using (var stream = material.Open())
			{
				using (var reader = new StreamReader(stream, Encoding.UTF8))
				{
					string line = reader.ReadLine();
					if (line != null && !types.Contains(line.Trim().Trim('\"').ToLowerInvariant()))
					{
						return null;
					}
					while ((line = reader.ReadLine()) != null)
					{
						if (line.Trim().StartsWith("\"$basetexture\"", StringComparison.InvariantCultureIgnoreCase))
						{
							var parts = line.Trim('\t').Split(new[] { ' ', '\t' }, 2);
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
				return references.AsParallel().Select(reference => reference).Select(reference =>
				{
					var files = reference.File.GetFiles("\\.v((tf)|(mt))$", true).GroupBy(x => x.Extension);
					var materials = files.Single(x => x.Key.Equals("vmt", StringComparison.InvariantCultureIgnoreCase)).ToList();
					var textures = files.Single(x => x.Key.Equals("vtf", StringComparison.InvariantCultureIgnoreCase)).ToDictionary(t => GetRelativeName(t));
					var refs = materials.Select(m => new MaterialTexturePackageReference(GetRelativeName(m), GetTextureFile(m, textures), m)).ToList();

					static string GetRelativeName(IFile file)
					{
						var pName = file.FullPathName.Split(':')[1];
						pName = pName.Substring(MATERIALS_INDEX, pName.Length - MATERIALS_INDEX - EXTENSION_COUNT);
						pName = Path.GetRelativePath(".", pName);
						return pName;
					}

					return new VmtMaterialPackage(refs);

				});
			});
		}
	}
}
