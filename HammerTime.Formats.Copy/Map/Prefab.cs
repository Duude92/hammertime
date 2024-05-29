using Sledge.BspEditor.Primitives;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Formats.Map.Formats;
using Sledge.Formats.Map.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SledgePrimitives = Sledge.BspEditor.Primitives;


namespace HammerTime.Formats.Map
{
    public class Prefab
    {
        internal static bool IsRmf = true;
        public static IEnumerable<Sledge.BspEditor.Primitives.MapData.Visgroup> Visgroups { get; private set; }
        public static List<IMapObject> GetPrefab(MapFile mapFile, UniqueNumberGenerator ung, SledgePrimitives.Map map, bool isRmf = true)
        {
            IsRmf = isRmf;
            List<IMapObject> content = new List<IMapObject>();
            var Visgroups = mapFile.Visgroups.Select(x => new Sledge.BspEditor.Primitives.MapData.Visgroup()
            {
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
        public static IEnumerable<Sledge.Formats.Map.Objects.MapObject> WriteObjects(WorldcraftPrefabLibrary prefabLibrary, IEnumerable<IMapObject> mapObjects, string prefabName)
        {
            List<Sledge.Formats.Map.Objects.MapObject> content = new List<Sledge.Formats.Map.Objects.MapObject>();


            foreach (var item in mapObjects)
            {
                content.Add(MapObject.WriteMapObject(item));
            }


            var mapFile = new MapFile();
            mapFile.Worldspawn.Children.AddRange(content);

            var newPrefab = new Sledge.Formats.Map.Objects.Prefab()
            {
                Map = mapFile,
                Name = prefabName,
                Description = "test description",
            };


            prefabLibrary.Prefabs.Add(newPrefab);


            return content;

        }
    }
}
