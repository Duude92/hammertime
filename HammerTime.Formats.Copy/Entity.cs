using SledgeRegular = Sledge.BspEditor.Primitives.MapObjects;
using SledgeFormats = Sledge.Formats.Map.Objects;
using Sledge.BspEditor.Primitives;
using Sledge.BspEditor.Primitives.MapObjectData;
using System.Collections.Generic;


namespace HammerTime.Formats
{
	internal class Entity
	{
		public static SledgeRegular.Entity FromFmt(SledgeFormats.Entity Entity, UniqueNumberGenerator uniqueNumberGenerator)
		{
			var entity = new SledgeRegular.Entity(uniqueNumberGenerator.Next("MapObject"))
			{
				Data =
				{
					new EntityData
					{
						Properties = new Dictionary<string, string>( Entity.Properties),
						Name = Entity.ClassName,
						Flags = Entity.SpawnFlags,
					},
					new ObjectColor(Entity.Color)
				}
			};


			foreach (var children in Entity.Children)
			{
				MapObject.GetMapObject(children, uniqueNumberGenerator).Hierarchy.Parent = entity;
			}

			entity.DescendantsChanged();


			return entity;
		}
	}
}
