using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Environment;
using Sledge.BspEditor.Primitives;
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

namespace Sledge.BspEditor.Providers
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

				var result = new BspFileLoadResult();
				var (bspFile, _) = HalfLife.UnifiedSdk.MapDecompiler.Serialization.BspSerialization.Deserialize(stream);


				return result;

			});

		}

		public Task Save(Stream stream, Map map, MapDocument document = null)
		{
			throw new NotImplementedException();
		}
	}
}
