using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Transport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Sledge.BspEditor.Primitives.MapData
{
	[Serializable]
	public class DisplayData : IMapData
	{
		public bool AffectsRendering => true;
		public string SkyboxName { get; set; } = null;

		public IMapElement Clone()
		{
			return Clone();
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
