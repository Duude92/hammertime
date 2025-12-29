using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common;
using Sledge.Common.Transport;
using Sledge.DataStructures.Geometric;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Numerics;
using System.Runtime.Serialization;

namespace Sledge.BspEditor.Primitives.MapObjectData
{
	public class EntityData : IMapObjectData, ITransformable
	{
		public string Name { get; set; }
		public int Flags { get; set; }
		public Dictionary<string, string> Properties { get; set; }

		public EntityData()
		{
			Name = "";
			Properties = new Dictionary<string, string>();
		}

		public EntityData(SerialisedObject obj)
		{
			Name = "";
			Properties = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
			foreach (var prop in obj.Properties)
			{
				if (prop.Key == "Name") Name = prop.Value;
				else if (prop.Key == "Flags") Flags = Convert.ToInt32(prop.Value, CultureInfo.InvariantCulture);
				else Properties[prop.Key] = prop.Value;
			}
		}

		[Export(typeof(IMapElementFormatter))]
		public class ActiveTextureFormatter : StandardMapElementFormatter<EntityData> { }

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Properties", Properties);
		}

		public Vector3? GetVector3(string key)
		{
			if (!Properties.ContainsKey(key)) return null;

			var spl = (Properties[key] ?? "").Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			if (spl.Length < 3) return null;

			if (float.TryParse(spl[0], NumberStyles.Float, CultureInfo.InvariantCulture, out var x)
				&& float.TryParse(spl[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var y)
				&& float.TryParse(spl[2], NumberStyles.Float, CultureInfo.InvariantCulture, out var z))
			{
				return new Vector3(x, y, z);
			}
			return null;
		}

		public T Get<T>(string key, T defaultValue = default(T))
		{
			if (!Properties.ContainsKey(key)) return defaultValue;
			try
			{
				var val = Properties[key];
				var conv = TypeDescriptor.GetConverter(typeof(T));
				return (T)conv.ConvertFromString(null, CultureInfo.InvariantCulture, val);
			}
			catch
			{
				return defaultValue;
			}
		}
		public string GetStringProperty(string key, string defaultValue)
		{
			if (!Properties.ContainsKey(key)) return defaultValue;
			return Properties[key];
		}

		public void Set<T>(string key, T value)
		{
			var conv = TypeDescriptor.GetConverter(typeof(T));
			var v = conv.ConvertToString(null, CultureInfo.InvariantCulture, value);
			Properties[key] = v;
		}

		public void Unset(string key)
		{
			if (Properties.ContainsKey(key)) Properties.Remove(key);
		}

		public IMapElement Clone()
		{
			var ed = new EntityData();
			ed.Name = Name;
			ed.Flags = Flags;
			ed.Properties = new Dictionary<string, string>(Properties);
			return ed;
		}

		public IMapElement Copy(UniqueNumberGenerator numberGenerator)
		{
			return Clone();
		}

		public SerialisedObject ToSerialisedObject()
		{
			var so = new SerialisedObject("EntityData");
			foreach (var p in Properties)
			{
				so.Set(p.Key, p.Value);
			}
			so.Set("Name", Name);
			so.Set("Flags", Flags);
			return so;
		}

		public void Transform(Matrix4x4 matrix)
		{
			if (Properties.TryGetValue("angles", out var angleString))
			{
				var initialAngles = angleString.Split(' ');
				var initial = NumericsExtensions.Parse(initialAngles[0], initialAngles[1], initialAngles[2], System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture);

				Vector3 previousLocalRotationRadians = new Vector3(
					MathHelper.DegreesToRadians(initial.X),
					MathHelper.DegreesToRadians(initial.Y),
					MathHelper.DegreesToRadians(initial.Z));
				Matrix4x4 yawMatrix = Matrix4x4.CreateRotationY(-previousLocalRotationRadians.X);
				Matrix4x4 pitchMatrix = Matrix4x4.CreateRotationX(previousLocalRotationRadians.Z);
				Matrix4x4 rollMatrix = Matrix4x4.CreateRotationZ(previousLocalRotationRadians.Y);

				Matrix4x4 rotationMatrix = pitchMatrix * yawMatrix * rollMatrix;

				rotationMatrix *= matrix;

				Vector3 deltaEuler = MathHelper.ExtractEulerAngles(rotationMatrix);

				var addition = new Vector3(
					MathHelper.RadiansToDegrees(deltaEuler.Y),
					MathHelper.RadiansToDegrees(-deltaEuler.Z),
					MathHelper.RadiansToDegrees(-deltaEuler.X));


				Properties["angles"] =
					$"{Math.Round(addition.X)} {Math.Round(addition.Y)} {Math.Round(addition.Z)}";
			}
		}
	}
}