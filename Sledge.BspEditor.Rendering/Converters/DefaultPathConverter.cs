using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Rendering.Resources;
using Sledge.DataStructures.Geometric;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Pipelines;
using Sledge.Rendering.Primitives;
using Sledge.Rendering.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Sledge.BspEditor.Rendering.Converters
{
	[Export(typeof(IMapObjectSceneConverter))]
	public class DefaultPathConverter : IMapObjectSceneConverter
	{
		public MapObjectSceneConverterPriority Priority => MapObjectSceneConverterPriority.BelowDefaultLowest;

		public Task Convert(BufferBuilder builder, MapDocument document, IMapObject obj, ResourceCollector resourceCollector)
		{
			var path = obj as Path;

			var color = Color.Goldenrod.ToVector4();

			var vertices = path.Nodes.Select(n => new VertexStandard { Position = n.Position, Colour = color });
			var nodeCount = path.Nodes.Count;
			var indices = Enumerable.Range(0, nodeCount).SelectMany(i => (i == 0 || i == nodeCount) ? new[] { (uint)i } : new[] { (uint)i, (uint)i });


			//uint sectorCount = 8;
			//uint stackCount = 8;
			//var vertices = GenerateSphereVertices(20, sectorCount, stackCount);
			//var vertexStandart = vertices.Select(v => new VertexStandard { Position = v + pathNode.Position, Colour = color, Tint = color }).ToArray();
			//var indices = GenerateSphereIndices(sectorCount, stackCount).ToArray();
			var groups = new List<BufferGroup>();
			//groups.Add(new BufferGroup(PipelineType.TexturedOpaque, CameraType.Both, (uint)0, (uint)indices.Length));
			groups.Add(new BufferGroup(PipelineType.Wireframe, CameraType.Both, (uint)0, (uint)indices.Count()));


			builder.Append(vertices, indices, groups);
			builder.Complete();
			return Task.CompletedTask;
		}

		public bool ShouldStopProcessing(MapDocument document, IMapObject obj)
		{
			return false;
		}

		public bool Supports(IMapObject obj)
		{
			return obj is Path;
		}
	}
}
