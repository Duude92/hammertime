﻿using Sledge.BspEditor.Compile;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common;
using Sledge.DataStructures.GameData;
using Sledge.FileSystem;
using Sledge.Providers.GameData;
using Sledge.Providers.Texture;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Path = System.IO.Path;

namespace Sledge.BspEditor.Environment.Source
{
	internal class SourceEnvironment : IEnvironment
	{
		public string Engine => "Source";

		public string ID { get; set; }

		public string Name { get; set; }

		private readonly Lazy<Task<GameData>> _gameData;
		private readonly Lazy<Task<TextureCollection>> _textureCollection;
		private readonly ITexturePackageProvider _materialsProvider;
		private readonly IGameDataProvider _fgdProvider;
		private readonly List<IEnvironmentData> _data;

		private IFile _root;

		public IFile Root
		{
			get
			{
				if (_root == null)
				{
					var dirs = Directories.Where(Directory.Exists).ToList();
					if (dirs.Any()) _root = new RootFile(Name, dirs.Select(x => new NativeFile(x)));
					else _root = new VirtualFile(null, "");
				}
				return _root;
			}
		}

		public string BaseDirectory { get; set; }
		public string GameDirectory { get; set; }
		public string ModDirectory { get; set; }
		public bool IncludeToolsDirectoryInEnvironment { get; set; }
		public string ToolsDirectory { get; set; }
		public bool IncludeFgdDirectoriesInEnvironment { get; set; }
		public List<string> FgdFiles { get; set; }
		public List<string> AdditionalTextureFiles { get; set; }

		public IEnumerable<string> Directories
		{
			get
			{
				yield return Path.Combine(BaseDirectory, ModDirectory);
				yield return Path.Combine(BaseDirectory, Path.Combine(ModDirectory, "download"));


				if (!String.Equals(GameDirectory, ModDirectory, StringComparison.CurrentCultureIgnoreCase))
				{
					yield return Path.Combine(BaseDirectory, Path.Combine(GameDirectory, "download"));
					yield return Path.Combine(BaseDirectory, GameDirectory);
				}

				if (IncludeToolsDirectoryInEnvironment && !String.IsNullOrWhiteSpace(ToolsDirectory) && Directory.Exists(ToolsDirectory))
				{
					yield return ToolsDirectory;
				}

				if (IncludeFgdDirectoriesInEnvironment)
				{
					foreach (var file in FgdFiles)
					{
						if (File.Exists(file)) yield return Path.GetDirectoryName(file);
					}
				}

				// Editor location to the path, for sprites and the like
				yield return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
			}
		}

		public SourceEnvironment()
		{
			_materialsProvider = Container.Get<ITexturePackageProvider>("Vmt");
			//_spriteProvider = Container.Get<ITexturePackageProvider>("Spr");
			_fgdProvider = Container.Get<IGameDataProvider>("Fgd");
			//_envProvider = Container.Get<ITexturePackageProvider>("Env");


			_textureCollection = new Lazy<Task<TextureCollection>>(MakeTextureCollectionAsync);
			_gameData = new Lazy<Task<GameData>>(MakeGameDataAsync);
			_data = new List<IEnvironmentData>();
			FgdFiles = new List<string>();
			AdditionalTextureFiles = new List<string>();
			IncludeToolsDirectoryInEnvironment = IncludeToolsDirectoryInEnvironment = true;
		}
		private Task<GameData> MakeGameDataAsync()
		{
			Func<GameData> fgdFunc = () =>
			{
				var fgd = _fgdProvider.GetGameDataFromFiles(FgdFiles);
				//_skyTextures = _envProvider.GetPackagesInFile(Root);

				var worldSpawn = fgd.Classes.FirstOrDefault(c => c.Name.Equals("worldspawn"));
				//if (worldSpawn != null)
				//{
				//	var skyProperty = worldSpawn.Properties.FirstOrDefault(c => c.Name.Equals("skyname"));
				//	skyProperty.VariableType = DataStructures.GameData.VariableType.StringChoices;

				//	skyProperty.Options = new List<DataStructures.GameData.Option>(_skyTextures.Select(x => new Option { Key = x.Name }).ToList());
				//}
				return fgd;
			};

			return Task.FromResult(fgdFunc());
		}

		private async Task<TextureCollection> MakeTextureCollectionAsync()
		{
			var matRefs = _materialsProvider.GetPackagesInFile(Root);
			var extraWads = AdditionalTextureFiles.SelectMany(x => _materialsProvider.GetPackagesInFile(new NativeFile(x)));
			var wads = await _materialsProvider.GetTexturePackages(matRefs.Union(extraWads));

			//var spriteRefs = _spriteProvider.GetPackagesInFile(Root);
			//var sprites = await _spriteProvider.GetTexturePackages(spriteRefs);

			return new SourceTextureCollection(wads);
		}

		public string DefaultBrushEntity { get; set; }

		public string DefaultPointEntity { get; set; }

		public decimal DefaultTextureScale { get; set; } = 1;
		public float DefaultGridSize { get; set; } = 16;


		public string CordonTexture { get; set; }
		public string[] NonRenderableTextures { get; set; }
		public string GameExe { get; internal set; }
		public bool OverrideMapSize { get; internal set; }
		public decimal MapSizeLow { get; internal set; }
		public decimal MapSizeHigh { get; internal set; }
		public string BspExe { get; internal set; }
		public string VisExe { get; internal set; }
		public string RadExe { get; internal set; }
		public bool GameCopyBsp { get; internal set; }
		public bool GameRun { get; internal set; }
		public bool GameAsk { get; internal set; }
		public bool MapCopyBsp { get; internal set; }
		public bool MapCopyMap { get; internal set; }
		public bool MapCopyLog { get; internal set; }
		public bool MapCopyErr { get; internal set; }
		public bool MapCopyRes { get; internal set; }

		public void AddData(IEnvironmentData data)
		{
			if (!_data.Contains(data)) _data.Add(data);
		}

		public Task<Batch> CreateBatch(IEnumerable<BatchArgument> arguments, BatchOptions options)
		{
			throw new NotImplementedException();
		}
		private static readonly string AutoVisgroupPrefix = typeof(SourceEnvironment).Namespace + ".AutomaticVisgroups";

		public IEnumerable<AutomaticVisgroup> GetAutomaticVisgroups()
		{
			// Entities
			yield return new AutomaticVisgroup(x => x is Entity && x.Hierarchy.HasChildren)
			{
				Path = $"{AutoVisgroupPrefix}.Entities",
				Key = $"{AutoVisgroupPrefix}.BrushEntities"
			};
			yield return new AutomaticVisgroup(x => x is Entity && !x.Hierarchy.HasChildren)
			{
				Path = $"{AutoVisgroupPrefix}.Entities",
				Key = $"{AutoVisgroupPrefix}.PointEntities"
			};
			yield return new AutomaticVisgroup(x => x is Entity e && e.EntityData.Name.StartsWith("light", StringComparison.InvariantCultureIgnoreCase))
			{
				Path = $"{AutoVisgroupPrefix}.Entities",
				Key = $"{AutoVisgroupPrefix}.Lights"
			};
			yield return new AutomaticVisgroup(x => x is Entity e && e.EntityData.Name.StartsWith("trigger_", StringComparison.InvariantCultureIgnoreCase))
			{
				Path = $"{AutoVisgroupPrefix}.Entities",
				Key = $"{AutoVisgroupPrefix}.Triggers"
			};
			yield return new AutomaticVisgroup(x => x is Entity e && e.EntityData.Name.IndexOf("_node", StringComparison.InvariantCultureIgnoreCase) >= 0)
			{
				Path = $"{AutoVisgroupPrefix}.Entities",
				Key = $"{AutoVisgroupPrefix}.Nodes"
			};

			// Tool brushes
			yield return new AutomaticVisgroup(x => x is Solid s && s.Faces.Any(f => string.Equals(f.Texture.Name, "bevel", StringComparison.InvariantCultureIgnoreCase)))
			{
				Path = $"{AutoVisgroupPrefix}.ToolBrushes",
				Key = $"{AutoVisgroupPrefix}.Bevel"
			};
			yield return new AutomaticVisgroup(x => x is Solid s && s.Faces.Any(f => string.Equals(f.Texture.Name, "hint", StringComparison.InvariantCultureIgnoreCase)))
			{
				Path = $"{AutoVisgroupPrefix}.ToolBrushes",
				Key = $"{AutoVisgroupPrefix}.Hint"
			};
			yield return new AutomaticVisgroup(x => x is Solid s && s.Faces.Any(f => string.Equals(f.Texture.Name, "origin", StringComparison.InvariantCultureIgnoreCase)))
			{
				Path = $"{AutoVisgroupPrefix}.ToolBrushes",
				Key = $"{AutoVisgroupPrefix}.Origin"
			};
			yield return new AutomaticVisgroup(x => x is Solid s && s.Faces.Any(f => string.Equals(f.Texture.Name, "skip", StringComparison.InvariantCultureIgnoreCase)))
			{
				Path = $"{AutoVisgroupPrefix}.ToolBrushes",
				Key = $"{AutoVisgroupPrefix}.Skip"
			};
			yield return new AutomaticVisgroup(x => x is Solid s && s.Faces.Any(f => string.Equals(f.Texture.Name, "aaatrigger", StringComparison.InvariantCultureIgnoreCase)))
			{
				Path = $"{AutoVisgroupPrefix}.ToolBrushes",
				Key = $"{AutoVisgroupPrefix}.Trigger"
			};

			// World geometry
			yield return new AutomaticVisgroup(x => x is Solid s && s.FindClosestParent(p => p is Entity) == null)
			{
				Path = $"{AutoVisgroupPrefix}.WorldGeometry",
				Key = $"{AutoVisgroupPrefix}.Brushes"
			};
			yield return new AutomaticVisgroup(x => x is Solid s && s.FindClosestParent(p => p is Entity) == null && s.Faces.Any(f => string.Equals(f.Texture.Name, "null", StringComparison.InvariantCultureIgnoreCase)))
			{
				Path = $"{AutoVisgroupPrefix}.WorldGeometry",
				Key = $"{AutoVisgroupPrefix}.Null"
			};
			yield return new AutomaticVisgroup(x => x is Solid s && s.FindClosestParent(p => p is Entity) == null && s.Faces.Any(f => string.Equals(f.Texture.Name, "sky", StringComparison.InvariantCultureIgnoreCase)))
			{
				Path = $"{AutoVisgroupPrefix}.WorldGeometry",
				Key = $"{AutoVisgroupPrefix}.Sky"
			};
			yield return new AutomaticVisgroup(x => x is Solid s && s.FindClosestParent(p => p is Entity) == null && s.Faces.Any(f => f.Texture.Name.StartsWith("!")))
			{
				Path = $"{AutoVisgroupPrefix}.WorldGeometry",
				Key = $"{AutoVisgroupPrefix}.Water"
			};
		}

		public IEnumerable<T> GetData<T>() where T : IEnvironmentData
		{
			return _data.OfType<T>();
		}
		public Task<GameData> GetGameData()
		{
			return _gameData.Value;
		}
		public Task<TextureCollection> GetTextureCollection()
		{
			return _textureCollection.Value;
		}

		public async Task UpdateDocumentData(MapDocument document)
		{
			// Ensure that worldspawn has the correct entity data
			var ed = document.Map.Root.Data.GetOne<EntityData>();

			if (ed == null)
			{
				ed = new EntityData();
				document.Map.Root.Data.Add(ed);
			}

			ed.Name = "worldspawn";
		}
	}
}
