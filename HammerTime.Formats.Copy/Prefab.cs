using Sledge.BspEditor.Primitives;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Formats.Map.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HammerTime.Formats
{
	public class Prefab
	{
		public static IEnumerable<IMapObject> GetPrefab(MapFile mapFile, UniqueNumberGenerator ung)
		{
			List<IMapObject> content = new List<IMapObject>();

            foreach (var item in mapFile.Worldspawn.Children)
            {
                content.Add(MapObject.GetMapObject(item, ung));
            }

            return content;
		}
	}
}
