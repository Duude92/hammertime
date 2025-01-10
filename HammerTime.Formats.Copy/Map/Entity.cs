using SledgeRegular = Sledge.BspEditor.Primitives.MapObjects;
using SledgeFormats = Sledge.Formats.Map.Objects;
using Sledge.BspEditor.Primitives;
using Sledge.BspEditor.Primitives.MapObjectData;
using System.Collections.Generic;
using System.Numerics;
using Sledge.DataStructures.Geometric;
using System.Globalization;
using System.Linq;


namespace HammerTime.Formats.Map
{
    public class Entity
    {
        public static SledgeRegular.Entity FromFmt(SledgeFormats.Entity Entity, UniqueNumberGenerator uniqueNumberGenerator)
        {
            var properties = new Dictionary<string, string>(Entity.Properties);
            properties.Remove("origin");
            var entity = new SledgeRegular.Entity(uniqueNumberGenerator.Next("MapObject"))
            {
                Data =
                {
                    new EntityData
                    {
                        Properties = properties,
                        Name = Entity.ClassName,
                        Flags = Entity.SpawnFlags,
                    },
                    new ObjectColor(Entity.Color)
                }
            };
            if (Entity.Properties.TryGetValue("origin", out var origin))
            {
                string[] originValues = origin.Split(' ');
                entity.Origin = NumericsExtensions.Parse(originValues[0], originValues[1], originValues[2], NumberStyles.Float, CultureInfo.InvariantCulture);
            }

            foreach (var children in Entity.Children)
            {
                MapObject.GetMapObject(children, uniqueNumberGenerator).Hierarchy.Parent = entity;
            }

            entity.DescendantsChanged();


            return entity;
        }
        public static SledgeFormats.Entity WriteEntity(SledgeRegular.Entity entity)
        {
            var newEntity = new SledgeFormats.Entity()
            {
                Color = entity.Color.Color,
                ClassName = entity.EntityData.Name,
                SpawnFlags = entity.EntityData.Flags,
                Children = entity.Hierarchy.Select(x => MapObject.WriteMapObject(x)).ToList(),

            };
            newEntity.Properties.Add("origin", $"{entity.Origin.X} {entity.Origin.Y} {entity.Origin.Z}");
            foreach (var property in entity.EntityData.Properties)
            {
                newEntity.Properties.Add(property);
            }

            return newEntity;
        }
    }
}
