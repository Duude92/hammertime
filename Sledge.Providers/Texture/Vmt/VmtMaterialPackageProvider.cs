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
		private const int MATERIALS_INDEX = 10; // materials\\

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

			var refs = (materialsRoot as CompositeFile).GetCompositeFiles().Select(x => new TexturePackageReference(x.Parent?.Name ?? "Materials", x));
			return refs;
		}
		private IFile GetTextureFile(IFile m, IDictionary<string, IFile> textures)
		{
			var tName = ReadMaterialBaseTexture(m);
			if (string.IsNullOrEmpty(tName)) return null;
			tName = Path.GetRelativePath(".", tName.ToLower());
			tName = tName.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
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
						if (line.Trim().Trim('\t').StartsWith("\"$basetexture\"", StringComparison.InvariantCultureIgnoreCase))
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
				return references.AsParallel().Select(reference =>
				{
					var files = reference.File.GetFiles("\\.v((tf)|(mt))$", true).GroupBy(x => x.Extension);
					var materials = files.Single(x => x.Key.Equals("vmt", StringComparison.InvariantCultureIgnoreCase)).ToList();
					var textures = files.Single(x => x.Key.Equals("vtf", StringComparison.InvariantCultureIgnoreCase)).GroupBy(t => GetRelativeName(t)).ToDictionary(g => g.Key, g => g.First());
					var refs = materials.Select(m => new MaterialTexturePackageReference(GetRelativeName(m), GetTextureFile(m, textures), m)).ToList();
					refs.Sort((p, n) => p.Name.CompareTo(n.Name));

					string GetRelativeName(IFile file)
					{
						var pName = file.GetRelativePath(reference.File);
						pName = pName.Substring(MATERIALS_INDEX, pName.Length - MATERIALS_INDEX - EXTENSION_COUNT);

						var rPath = Path.GetRelativePath(".", pName);
						return rPath.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
					}

					return new VmtMaterialPackage(reference.Name, refs);

				});
			});
		}
	}
}
