using Sledge.FileSystem;
using Sledge.Providers.Texture;
using System.ComponentModel.Composition;
using System.Text;

namespace HammerTime.Source.Providers.Texture.Vmt
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
		private (bool hide, IFile file) GetTextureFile(IFile m, IDictionary<string, IFile> textures)
		{
			var texture = ReadMaterialBaseTexture(m);
			if (string.IsNullOrEmpty(texture.path)) return (true, null);
			var texturePath = Path.GetRelativePath(".", texture.path.ToLower());
			texturePath = texturePath.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
			if (textures.TryGetValue(texturePath, out var tex))
			{
				return (texture.hide, tex);
			}
			return (true, null);
		}
		private HashSet<string> _allowedTypes = new HashSet<string>
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
				"skyfog",
			};
		private (bool hide, string path) ReadMaterialBaseTexture(IFile material)
		{
			var mName = material.NameWithoutExtension;
			using (var stream = material.Open())
			{
				using (var reader = new StreamReader(stream, Encoding.UTF8))
				{
					string line = reader.ReadLine();
					var shaderType = line?.Trim()?.Trim('\"').ToLowerInvariant();

						while ((line = reader.ReadLine()) != null)
					{
						if (line.Trim().Trim('\t').StartsWith("\"$basetexture\"", StringComparison.InvariantCultureIgnoreCase))
						{
							var parts = line.Trim('\t').Split(new[] { ' ', '\t' }, 2);
							if (parts.Length > 1)
							{
								if (!_allowedTypes.Contains(shaderType))
								{
									return (true, parts[1].Trim('"'));
								}
								return (false, parts[1].Trim('"'));
							}
						}
					}
				}
			}
			return (false, null);
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
					var refs = materials.Select(m =>
					{
						var (hide, f) = GetTextureFile(m, textures);
						return new MaterialTexturePackageReference(GetRelativeName(m), f, m, hide);
					}).ToList();
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
