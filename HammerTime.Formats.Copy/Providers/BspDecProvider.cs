﻿using Sledge.BspEditor.Documents;
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


namespace HammerTime.Formats.Providers
{
	[Export(typeof(IBspSourceProvider))]
	internal class BspDecProvider : IBspSourceProvider
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
		private static GameData _gameData;

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
				var strategy = DecompilerStrategies.TreeDecompilerStrategy;
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
