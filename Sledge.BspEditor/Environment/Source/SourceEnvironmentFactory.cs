using Sledge.BspEditor.Environment.Goldsource;
using System;
using System.ComponentModel.Composition;
using System.Linq;

namespace Sledge.BspEditor.Environment.Source
{
	[Export(typeof(IEnvironmentFactory))]

	internal class SourceEnvironmentFactory : GoldsourceEnvironmentFactory
	{
		public override Type Type => typeof(SourceEnvironment);
		public override string TypeName => "SourceEnvironment";
		public override string Description { get; set; } = "Source";

		public override IEnvironment Deserialise(SerialisedEnvironment environment)
		{
			var gse = new SourceEnvironment()
			{
				ID = environment.ID,
				Name = environment.Name,
				BaseDirectory = GetVal(environment.Properties, "BaseDirectory", ""),
				GameDirectory = GetVal(environment.Properties, "GameDirectory", ""),
				ModDirectory = GetVal(environment.Properties, "ModDirectory", ""),
				GameExe = GetVal(environment.Properties, "GameExe", ""),
				LoadHdModels = GetVal(environment.Properties, "LoadHdModels", true),

				FgdFiles = GetVal(environment.Properties, "FgdFiles", "").Split(';').Where(x => !String.IsNullOrWhiteSpace(x)).ToList(),
				DefaultPointEntity = GetVal(environment.Properties, "DefaultPointEntity", ""),
				DefaultBrushEntity = GetVal(environment.Properties, "DefaultBrushEntity", ""),
				OverrideMapSize = GetVal(environment.Properties, "OverrideMapSize", false),
				MapSizeLow = GetVal(environment.Properties, "MapSizeLow", -4096m),
				MapSizeHigh = GetVal(environment.Properties, "MapSizeHigh", 4096m),
				IncludeFgdDirectoriesInEnvironment = GetVal(environment.Properties, "IncludeFgdDirectoriesInEnvironment", true),

				ToolsDirectory = GetVal(environment.Properties, "ToolsDirectory", ""),
				IncludeToolsDirectoryInEnvironment = GetVal(environment.Properties, "IncludeToolsDirectoryInEnvironment", true),
				BspExe = GetVal(environment.Properties, "BspExe", ""),
				CsgExe = GetVal(environment.Properties, "CsgExe", ""),
				VisExe = GetVal(environment.Properties, "VisExe", ""),
				RadExe = GetVal(environment.Properties, "RadExe", ""),

				GameCopyBsp = GetVal(environment.Properties, "GameCopyBsp", true),
				GameRun = GetVal(environment.Properties, "GameRun", true),
				GameAsk = GetVal(environment.Properties, "GameAsk", true),

				MapCopyBsp = GetVal(environment.Properties, "MapCopyBsp", false),
				MapCopyMap = GetVal(environment.Properties, "MapCopyMap", false),
				MapCopyLog = GetVal(environment.Properties, "MapCopyLog", false),
				MapCopyErr = GetVal(environment.Properties, "MapCopyErr", false),
				MapCopyRes = GetVal(environment.Properties, "MapCopyRes", false),

				DefaultTextureScale = GetVal(environment.Properties, "DefaultTextureScale", 1m),
				DefaultGridSize = GetVal(environment.Properties, "DefaultGridSize", 16f),
				ExcludedWads = GetVal(environment.Properties, "ExcludedWads", "").Split(';').Where(x => !String.IsNullOrWhiteSpace(x)).ToList(),
				AdditionalTextureFiles = GetVal(environment.Properties, "AdditionalTextureFiles", "").Split(';').Where(x => !String.IsNullOrWhiteSpace(x)).ToList(),
				CordonTexture = GetVal(environment.Properties, "CordonWrapTexture", "BLACK"),
				NonRenderableTextures = GetVal(environment.Properties, "NonRenderableTexture", "aaatrigger;sky;null;clip;hint;skip;origin;bevel").Split(';').Select(texture => texture.Trim()).Where(texture => !String.IsNullOrEmpty(texture)).ToArray()

			};
			return gse;
		}

	}
}
