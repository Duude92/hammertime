using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Rendering.ChangeHandlers;
using Sledge.BspEditor.Rendering.Resources;
using Sledge.Common;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Engine;
using Sledge.Rendering.Pipelines;
using Sledge.Rendering.Primitives;
using Sledge.Rendering.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static Sledge.DataStructures.Geometric.NumericsExtensions;

namespace Sledge.BspEditor.Rendering.Converters
{
	[Export(typeof(IMapObjectSceneConverter))]
	public class LightEnvConverter : IMapObjectSceneConverter
	{
		[Import] private EngineInterface _engine;
		public MapObjectSceneConverterPriority Priority => MapObjectSceneConverterPriority.DefaultLow;

		public bool ShouldStopProcessing(MapDocument document, IMapObject obj)
		{
			return false;
		}

		public bool Supports(IMapObject obj)
		{
			return obj is Entity e && e.EntityData.Name.ToLower() == "light_environment";
		}

		public async Task Convert(BufferBuilder builder, MapDocument document, IMapObject obj, ResourceCollector resourceCollector)
		{
			var entity = (Entity)obj;
			var angled = entity.EntityData.GetVector3("angles").Value;
			var pitch = entity.EntityData.Get<float>("pitch", angled.X) * MathF.PI / 180;
			var yaw = entity.EntityData.Get<float>("angle", angled.Y) * MathF.PI / 180;
			var colourProp = entity.EntityData.GetVector3("_light").Value; // FIXME: Vector4
			var colour = new Vector4(colourProp, 1);

			var rad = angled;

			var mat_x = Matrix4x4.CreateRotationY(-pitch);
			var mat_y = Matrix4x4.CreateRotationX(yaw);
			var mat_z = Matrix4x4.CreateRotationZ(angled.Z);
			var rot = mat_x * mat_y * mat_z;
			var lightRotation = Quaternion.CreateFromRotationMatrix(rot);
			Vector3 lightDirection = Vector3.Transform(Vector3.UnitX, lightRotation);
			var forward = lightDirection;

			var verts = new List<VertexStandard>();
			var indices = new List<int>();
			var groups = new List<BufferGroup>();
			var start = entity.Origin;
			var end = entity.Origin + forward * 50;

			for (int i = -1; i < 2; i++)
			{
				verts.Add(new VertexStandard { Colour = colour, Position = start + i * Vector3.UnitZ * 10, Tint = Vector4.One });
				verts.Add(new VertexStandard { Colour = colour, Position = end + i * Vector3.UnitZ * 10, Tint = Vector4.One });
				indices.Add((i + 1) * 2);
				indices.Add((i + 1) * 2 + 1);
			}

			groups.Add(new BufferGroup(PipelineType.Wireframe, CameraType.Perspective, (uint)0, (uint)(indices.Count)));
			builder.Append(verts, indices.Select(x => (uint)x), groups);
		}
	}
}
