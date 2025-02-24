using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Transport;
using System;
using System.Numerics;
using System.Runtime.Serialization;

namespace Sledge.BspEditor.Primitives.MapData
{
	public class DisplayData : IMapData
	{
		public bool AffectsRendering => true;
		public string SkyboxName { get; set; } = null;
		public Vector3 LightDirection { get; set; }

		public IMapElement Clone()
		{
			return new DisplayData() { SkyboxName = SkyboxName };
		}

		public IMapElement Copy(UniqueNumberGenerator numberGenerator)
		{
			return Clone();
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			throw new NotImplementedException();
		}

		public SerialisedObject ToSerialisedObject()
		{
			throw new NotImplementedException();
		}
	}
}
