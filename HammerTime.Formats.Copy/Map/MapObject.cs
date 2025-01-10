using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Formats.Map.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SledgeRegular = Sledge.BspEditor.Primitives.MapObjects;
using SledgeFormats = Sledge.Formats.Map.Objects;
using Sledge.BspEditor.Primitives;

namespace HammerTime.Formats.Map
{
    internal class MapObject
    {
        public static IMapObject GetMapObject(SledgeFormats.MapObject MapObject, UniqueNumberGenerator ung)
        {
            IMapObject result = null;
            if (MapObject is SledgeFormats.Solid)
            {
                result = Solid.FromFmt(MapObject as SledgeFormats.Solid, ung);
            }
            else if (MapObject is SledgeFormats.Group)
            {
                result = Group.FromFmt(MapObject as SledgeFormats.Group, ung);
            }
            else if (MapObject is SledgeFormats.Entity)
            {
                result = Entity.FromFmt(MapObject as SledgeFormats.Entity, ung);
            }
            else
            {
                throw new Exception($"Prefab type is: {MapObject.GetType()}");
            }
            foreach (var vg in MapObject.Visgroups)
            {
                var vis = Prefab.Visgroups.FirstOrDefault(x => x.ID == vg);
                if (vis != null)
                {
                    vis.Objects.Add(result);
                }
            }
            return result;
        }
        public static SledgeFormats.MapObject WriteMapObject(IMapObject mapObject)
        {
            SledgeFormats.MapObject result = null;
            if (mapObject is SledgeRegular.Solid solid)
            {
                result = Solid.WriteSolid(solid);
            }
            else if (mapObject is SledgeRegular.Group group)
            {
                result = Group.WriteGroup(group);
            }
            else if (mapObject is SledgeRegular.Entity entity)
            {
                result = Entity.WriteEntity(entity);
            }
            return result;
        }
    }
}
