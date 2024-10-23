using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Rendering.Resources;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Pipelines;
using Sledge.Rendering.Primitives;
using Sledge.Rendering.Resources;
using System.ComponentModel.Composition;
using System.Numerics;
using System.Threading.Tasks;

namespace Sledge.BspEditor.Rendering.Converters
{
	[Export(typeof(IMapObjectSceneConverter))]
	public class SkyboxConverter : IMapObjectSceneConverter
	{
		public MapObjectSceneConverterPriority Priority => MapObjectSceneConverterPriority.OverrideLowest;

		public Task Convert(BufferBuilder builder, MapDocument document, IMapObject obj, ResourceCollector resourceCollector)
		{
			var displayFlags = document.Map.Data.GetOne<DisplayFlags>();
			var displayData = document.Map.Data.GetOne<DisplayData>() ?? new DisplayData();
			var skybox = displayFlags?.ToggleSkybox == true;

			if (skybox)
			{
				builder.Append(_skyVertices, _skyIndices, new[] {new BufferGroup(
						PipelineType.Skybox,
						CameraType.Perspective, false, Vector3.Zero, displayData.SkyboxName, 0, 36)
					});
			}
			builder.Complete();
			return Task.CompletedTask;
		}

		public bool ShouldStopProcessing(MapDocument document, IMapObject obj) => false;

		public bool Supports(IMapObject obj) => false;

		private static VertexStandard[] _skyVertices = new VertexStandard[]
		{
		   new VertexStandard{ Position =new Vector3(-1, -1,  0.5f) }, //0
		   new VertexStandard{ Position =new Vector3( 1, -1,  0.5f) }, //1
		   new VertexStandard{ Position =new Vector3(-1,  1,  0.5f) }, //2
		   new VertexStandard{ Position =new Vector3( 1,  1,  0.5f) }, //3
		   new VertexStandard{ Position =new Vector3(-1, -1, -0.5f) }, //4
		   new VertexStandard{ Position =new Vector3( 1, -1, -0.5f) }, //5
		   new VertexStandard{ Position =new Vector3(-1,  1, -0.5f) }, //6
		   new VertexStandard{ Position =new Vector3( 1,  1, -0.5f)  }  //7															
		};
		private static uint[] _skyIndices = new uint[]
		{
			//Top
			2, 6, 7,
			2, 3, 7,
			//Bottom
			0, 4, 5,
			0, 1, 5,
			//Left
			0, 2, 6,
			0, 4, 6,
			//Right
			1, 3, 7,
			1, 5, 7,
			//Front
			0, 2, 3,
			0, 1, 3,
			//Back
			4, 6, 7,
			4, 5, 7
		};
	}
}
