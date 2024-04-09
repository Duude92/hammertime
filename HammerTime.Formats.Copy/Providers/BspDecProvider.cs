using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Environment;
using SledgePrimitives = Sledge.BspEditor.Primitives;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Shell.Documents;
using Sledge.DataStructures.GameData;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sledge.BspEditor.Providers;
using HalfLife.UnifiedSdk.MapDecompiler;
using Serilog;
using System.Threading;
using MapFormats = HammerTime.Formats.Map;
using Sledge.Common.Shell.Commands;
using Sledge.BspEditor.Commands;
using Sledge.BspEditor.Environment.Goldsource;
using LogicAndTrick.Oy;
using System.Security.Cryptography;


namespace HammerTime.Formats.Providers
{
	[Export(typeof(ICommand))]
	[CommandID("Tool:DecompileTool")]
	internal class BspDecProvider : BaseCommand, IBspSourceProvider
	{
		private static readonly IEnumerable<Type> SupportedTypes = new List<Type>
		{
            // Map Object types
            typeof(Solid),
			typeof(Entity),

            // Map Object Data types
            typeof(VisgroupID),
			typeof(EntityData),
		};
		public IEnumerable<Type> SupportedDataTypes => SupportedTypes;

		public IEnumerable<FileExtensionInfo> SupportedFileExtensions => new[] { new FileExtensionInfo("BSP v29 & v30 files", ".bsp") };

		public bool CanSave => false;

		public override string Name { get; set; } = "Decompiler";
		public override string Details { get; set; } = "Decompiler tool";

		private static GameData _gameData;
		private DecompilerOptions _decompilerOptions;
		private DecompilerStrategy _strategy;
		protected override async Task Invoke(MapDocument document, CommandParameters parameters)
		{
			var path = parameters.Get<string>("Path");
			_strategy = DecompilerStrategies.Strategies[parameters.Get<int>("Strategy")];
			_decompilerOptions = new DecompilerOptions
			{
				AlwaysGenerateOriginBrushes = parameters.Get<bool>("GenerateOrigin"),
				ApplyNullToGeneratedFaces = parameters.Get<bool>("ApplyNull"),
				BrushOptimization = (BrushOptimization)parameters.Get<int>("Optimization"),
				IncludeLiquids = parameters.Get<bool>("IncludeLiquids"),
				MergeBrushes = parameters.Get<bool>("MergeBrushes"),
				TriggerEntityWildcards = System.Collections.Immutable.ImmutableList.Create(parameters.Get<string>("TriggerEntityWildcards").Split('\n')),
			};
			await Oy.Publish<IBspSourceProvider>("Internal:RegisterDocumentLoader", this);

			await Oy.Publish("Command:Run", new CommandMessage("Internal:OpenDocument", new
			{
				Path = path
			}));

			await Oy.Publish<IBspSourceProvider>("Internal:RemoveDocumentLoader", this);


		}
		public async Task<BspFileLoadResult> Load(Stream stream, IEnvironment environment)
		{
			_gameData = await environment.GetGameData();
			return await Task.Factory.StartNew(() =>
			{
				var logger = new LoggerConfiguration()
				.MinimumLevel.Information()
				.CreateLogger();

				var result = new BspFileLoadResult();
				var map = new SledgePrimitives.Map();
				var (bspFile, _) = HalfLife.UnifiedSdk.MapDecompiler.Serialization.BspSerialization.Deserialize(stream);
				var strategy = DecompilerStrategies.FaceToBrushDecompilerStrategy;
				var mapFile = strategy.Decompile(logger, bspFile, new DecompilerOptions
				{
					BrushOptimization = BrushOptimization.FewestBrushes,
					AlwaysGenerateOriginBrushes = true,
					MergeBrushes = true,
					ApplyNullToGeneratedFaces = true,
				}, CancellationToken.None);

				map.Root.Data.Replace(MapFormats.Entity.FromFmt(mapFile.Worldspawn, map.NumberGenerator).EntityData);

				var objects = MapFormats.Prefab.GetPrefab(mapFile, map.NumberGenerator, map, false);


				foreach (var obj in objects)
				{
					obj.Hierarchy.Parent = map.Root;
				}

				result.Map = map;
				result.Map.Root.DescendantsChanged();
				return result;

			});

		}

		public Task Save(Stream stream, SledgePrimitives.Map map, MapDocument document = null)
		{
			throw new NotImplementedException();
		}


	}
}
