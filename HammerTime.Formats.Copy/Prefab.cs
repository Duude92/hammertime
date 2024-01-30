﻿using Sledge.BspEditor.Primitives;
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
		public static IEnumerable<Sledge.BspEditor.Primitives.MapData.Visgroup> Visgroups { get; private set; }
		public static IEnumerable<IMapObject> GetPrefab(MapFile mapFile, UniqueNumberGenerator ung, Map map)
		{
			List<IMapObject> content = new List<IMapObject>();
			var Visgroups = mapFile.Visgroups.Select(x => new Sledge.BspEditor.Primitives.MapData.Visgroup() { 
				ID = x.ID,
				Name = x.Name,
				Colour = x.Color,
				Visible = x.Visible
			});

			foreach (var item in mapFile.Worldspawn.Children)
            {
                content.Add(MapObject.GetMapObject(item, ung));
            }
			map.Data.AddRange(Visgroups);
            return content;
		}
	}
}