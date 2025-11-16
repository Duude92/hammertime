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
		public class KVNode
		{
			public readonly string Node;
			public readonly string? Value;
			public readonly List<KVNode> Children = new();
			public KVNode(StreamReader reader)
			{
				bool objectMode = false;
				string line;
				while (true)
				{
					if (objectMode)
					{
						var child = new KVNode(reader);
						if (!string.IsNullOrEmpty(child.Node))
							Children.Add(child);
						else
							objectMode = false;
						continue;
					}

					line = reader.ReadLine();
					if (line == null) return;
					var trimmed = line?.Trim()?.Trim('\"').ToLowerInvariant();
					if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("//")) continue;
					if (trimmed.StartsWith("{"))
					{
						objectMode = true;
						continue;
					}

					if (trimmed.StartsWith("}")) return;
					var spl = trimmed.Split(new[] { ' ', '\t' }, 2);
					if (spl.Length < 2)
					{
						Node = trimmed;
						continue;
					}

					var split = trimmed.Split(new[] { ' ', '\t' }, 2);

					Node = split[0].Replace("\"", "");
					Value = split[1].Replace("\"", "");
					return;
				}
			}
			public string? GetValue(string key) => Children.FirstOrDefault(x => x.Node.Equals(key))?.Value;
			public float GetFloat(string key)
			{
				if (float.TryParse(Children.FirstOrDefault(x => x.Node.Equals(key))?.Value, out var result)) return result;
				return 0;
			}
		}
		public KVNode? ReadMaterial(IFile material)
		{
			var mName = material.NameWithoutExtension;
			using (var stream = material.Open())
			{
				using (var reader = new StreamReader(stream, Encoding.UTF8))
				{
					var root = new KVNode(reader);
					return root;
				}
			}
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
						bool hide = false;
						var mat = ReadMaterial(m);

						var baseTexture = mat?.GetValue("$basetexture");

						if (string.IsNullOrEmpty(baseTexture))
							return new MaterialTexturePackageReference(GetRelativeName(m), null, mat, true);

						var texturePath = Path.GetRelativePath(".", baseTexture.ToLower());
						texturePath = texturePath.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

						if (!textures.TryGetValue(texturePath, out var tex))
						{
							return new MaterialTexturePackageReference(GetRelativeName(m), null, mat, true);
						}

						return new MaterialTexturePackageReference(GetRelativeName(m), tex, mat, !_allowedTypes.Contains(mat?.Node!));
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
