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
	public class DefaultPathNodeConverter : IMapObjectSceneConverter
	{
		public MapObjectSceneConverterPriority Priority => MapObjectSceneConverterPriority.BelowDefaultLowest;

		public Task Convert(BufferBuilder builder, MapDocument document, IMapObject obj, ResourceCollector resourceCollector)
		{
			var pathNode = obj as Path.PathNode;
			uint sectorCount = 10;
			uint stackCount = 10;
			var vertices = GenerateSphereVertices(20, sectorCount, stackCount);
			var color = Color.Green.ToVector4();
			var vertexStandart = vertices.Select(v => new VertexStandard { Position = v + pathNode.Position, Tint = color }).ToArray();
			var indices = GenerateSphereIndices(sectorCount, stackCount).ToArray();
			var groups = new List<BufferGroup>();

			var vertices1 = GenerateSphereVertices(20, 8, stackCount).Select(v=>new VertexStandard { Position = v + pathNode.Position, Colour = color, Tint = color });
			var indices1 = GenerateSphereIndices(8, stackCount).ToArray();


			groups.Add(new BufferGroup(PipelineType.TexturedOpaque, CameraType.Perspective, (uint)0, (uint)indices.Length));

			builder.Append(vertices1, indices1, new[] { new BufferGroup(PipelineType.Wireframe, CameraType.Orthographic, (uint)0, (uint)indices1.Length) });


			builder.Append(vertexStandart, indices, groups);
			builder.Complete();
			return Task.CompletedTask;
		}

		public bool ShouldStopProcessing(MapDocument document, IMapObject obj)
		{
			return false;
		}

		public bool Supports(IMapObject obj)
		{
			return obj is Path.PathNode;
		}

		static List<Vector3> GenerateSphereVertices(float radius, uint sectorCount, uint stackCount)
		{
			List<Vector3> vertices = new List<Vector3>();

			float x, y, z, xy; // vertex position
			float sectorStep = 2 * (float)Math.PI / sectorCount;
			float stackStep = (float)Math.PI / stackCount;
			float sectorAngle, stackAngle;

			for (int i = 0; i <= stackCount; ++i)
			{
				stackAngle = (float)Math.PI / 2 - i * stackStep; // starting from pi/2 to -pi/2
				xy = radius * (float)Math.Cos(stackAngle); // r * cos(u)
				z = radius * (float)Math.Sin(stackAngle);  // r * sin(u)

				// add (sectorCount+1) vertices per stack
				// the first and last vertices have same position and normal, but different tex coords
				for (int j = 0; j <= sectorCount; ++j)
				{
					sectorAngle = j * sectorStep; // starting from 0 to 2pi

					// vertex position (x, y, z)
					x = xy * (float)Math.Cos(sectorAngle); // r * cos(u) * cos(v)
					y = xy * (float)Math.Sin(sectorAngle); // r * cos(u) * sin(v)
					vertices.Add(new Vector3(x, y, z));
				}
			}

			return vertices;
		}

		static List<uint> GenerateSphereIndices(uint sectorCount, uint stackCount)
		{
			List<uint> indices = new List<uint>();
			uint k1, k2;

			for (uint i = 0; i < stackCount; ++i)
			{
				k1 = i * (sectorCount + 1); // beginning of current stack
				k2 = k1 + sectorCount + 1;  // beginning of next stack

				for (int j = 0; j < sectorCount; ++j, ++k1, ++k2)
				{
					// 2 triangles per sector excluding the first and last stacks
					if (i != 0)
					{
						indices.Add(k1 + 1);
						indices.Add(k2 + 1);
						indices.Add(k1);
					}

					if (i != (stackCount - 1))
					{
						indices.Add(k1 + 0);
						indices.Add(k2 + 1);
						indices.Add(k2);
					}

				}
			}

			return indices;
		}


	}
}
