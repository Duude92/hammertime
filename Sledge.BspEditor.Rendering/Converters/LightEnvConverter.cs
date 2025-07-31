using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Rendering.Resources;
using Sledge.DataStructures.GameData;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Pipelines;
using Sledge.Rendering.Primitives;
using Sledge.Rendering.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace Sledge.BspEditor.Rendering.Converters
{
	[Export(typeof(IMapObjectSceneConverter))]
	public class LightEnvConverter : IMapObjectSceneConverter
	{
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
			var data = entity.EntityData;
			var angles = data.GetVector3("angles");
			if (!angles.HasValue)
			{
				// Probably somewhere there is function to convert string to Vector3, but I don't remember for sure.
				var gameData = await document.Environment.GetGameData();
				var light_default = gameData.Classes.FirstOrDefault(x => x.ClassType != ClassType.Base && (x.Name ?? "").ToLower() == "light_environment");
				var angleString = light_default.Properties.FirstOrDefault(x => x.Name == "angles").DefaultValue;
				var pitchString = light_default.Properties.FirstOrDefault(x => x.Name == "pitch")?.DefaultValue;
				var yawString = light_default.Properties.FirstOrDefault(x => x.Name == "angle")?.DefaultValue;

				var spl = (angleString ?? "").Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
				var resultAngle = Vector3.Zero;

				if (float.TryParse(spl[0], NumberStyles.Float, CultureInfo.InvariantCulture, out var x)
					&& float.TryParse(spl[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var y)
					&& float.TryParse(spl[2], NumberStyles.Float, CultureInfo.InvariantCulture, out var z))
				{
					resultAngle = new Vector3(x, y, z);
				}
				if (!String.IsNullOrEmpty(pitchString))
					resultAngle.X = int.Parse(pitchString);
				if (!String.IsNullOrEmpty(yawString))
					resultAngle.Y = int.Parse(yawString);
				angles = resultAngle;
			}
			var anglesValue = angles.Value;
			var pitch = data.Get<float>("pitch", angles.Value.X);
			if (pitch != float.NaN)
				anglesValue.X = pitch;
			var yaw = data.Get<float>("angle", angles.Value.Y);
			if (yaw != float.NaN)
				anglesValue.Y = yaw;


			var colourProp = entity.EntityData.GetVector3("_light").Value; // FIXME: Vector4
			var colour = new Vector4(colourProp, 1);

			var rad = anglesValue;

			var mat_x = Matrix4x4.CreateRotationY(-pitch);
			var mat_y = Matrix4x4.CreateRotationX(yaw);
			var mat_z = Matrix4x4.CreateRotationZ(anglesValue.Z);
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
