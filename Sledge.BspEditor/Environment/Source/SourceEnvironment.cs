using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Environment.Goldsource;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.Common;
using Sledge.DataStructures.GameData;
using Sledge.FileSystem;
using Sledge.Providers.GameData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Path = System.IO.Path;

namespace Sledge.BspEditor.Environment.Source
{
	internal class SourceEnvironment : GoldsourceEnvironment
	{
		public override string Engine => "Source";

		//public string ID { get; set; }

		//public string Name { get; set; }

		private readonly Lazy<Task<GameData>> _gameData;
		private readonly Lazy<Task<TextureCollection>> _textureCollection;
		private readonly IGameDataProvider _fgdProvider;
		private readonly List<IEnvironmentData> _data;


		private IFile _root;

		public override IFile Root
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

		//public string BaseDirectory { get; set; }
		//public string GameDirectory { get; set; }
		//public string ModDirectory { get; set; }

		public override IEnumerable<string> Directories
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
			//_wadProvider = Container.Get<ITexturePackageProvider>("Wad3");
			//_spriteProvider = Container.Get<ITexturePackageProvider>("Spr");
			_fgdProvider = Container.Get<IGameDataProvider>("Fgd");
			//_envProvider = Container.Get<ITexturePackageProvider>("Env");


			_textureCollection = new Lazy<Task<TextureCollection>>(MakeTextureCollectionAsync);
			_gameData = new Lazy<Task<GameData>>(MakeGameDataAsync);
			_data = new List<IEnvironmentData>();
			FgdFiles = new List<string>();
			AdditionalTextureFiles = new List<string>();
			ExcludedWads = new List<string>();
			IncludeToolsDirectoryInEnvironment = IncludeToolsDirectoryInEnvironment = true;
		}



		//public string DefaultBrushEntity => throw new NotImplementedException();

		//public string DefaultPointEntity => throw new NotImplementedException();

		//public decimal DefaultTextureScale => throw new NotImplementedException();

		//public float DefaultGridSize => throw new NotImplementedException();

		//public string CordonTexture { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		//public string[] NonRenderableTextures { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		//public void AddData(IEnvironmentData data)
		//{
		//	throw new NotImplementedException();
		//}

		//public Task<Batch> CreateBatch(IEnumerable<BatchArgument> arguments, BatchOptions options)
		//{
		//	throw new NotImplementedException();
		//}

		//public IEnumerable<AutomaticVisgroup> GetAutomaticVisgroups()
		//{
		//	throw new NotImplementedException();
		//}

		//public IEnumerable<T> GetData<T>() where T : IEnvironmentData
		//{
		//	throw new NotImplementedException();
		//}

		public override Task<GameData> GetGameData()
		{
			return _gameData.Value;
		}


		public override Task<TextureCollection> GetTextureCollection()
		{
			return _textureCollection.Value;
		}

		public override async Task UpdateDocumentData(MapDocument document)
		{
			// Ensure that worldspawn has the correct entity data
			var ed = document.Map.Root.Data.GetOne<EntityData>();

			if (ed == null)
			{
				ed = new EntityData();
				document.Map.Root.Data.Add(ed);
			}

			ed.Name = "worldspawn";

			// Set the wad usage - this required when exporting to map for compile

			var tc = await GetTextureCollection();

			// Get the list of used packages - the packages are abstracted away from the file system, so we don't know where they are located yet
			//var usedPackages = GetUsedTexturePackages(document, tc).Select(x => x.Location).ToHashSet(StringComparer.InvariantCultureIgnoreCase);

			// Get the list of wad locations - for the wad texture provider, this is a quick operation
			//var wads = _wadProvider.GetPackagesInFile(Root).Select(x => x.File.GetPathOnDisk()).Where(x => x != null).ToList();

			// Get the list of wads that are in the used set
			//var usedWads = wads.Where(x => usedPackages.Contains(Path.GetFileName(x))).ToList();

			//document.Map.Root.Data.GetOne<EntityData>()?.Set("wad", string.Join(";", usedWads));
		}
	}
}
