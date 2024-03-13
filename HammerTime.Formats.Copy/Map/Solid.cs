using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SledgeRegular = Sledge.BspEditor.Primitives.MapObjects;
using SledgeFormats = Sledge.Formats.Map.Objects;
using Sledge.BspEditor.Primitives;
using Sledge.BspEditor.Primitives.MapObjectData;
namespace HammerTime.Formats.Map
{
    internal class Solid
    {
        public static SledgeRegular.Solid FromFmt(SledgeFormats.Solid solid, UniqueNumberGenerator ung)
        {
            var newSolid = new SledgeRegular.Solid(ung.Next("MapObject"));
            newSolid.Data.AddRange(solid.Faces.Select(x => Face.FromFmt(x, ung)));
            newSolid.Data.Add(new ObjectColor(solid.Color));


            foreach (var children in solid.Children)
            {
                MapObject.GetMapObject(children, ung).Hierarchy.Parent = newSolid;
            }


            newSolid.DescendantsChanged();

            return newSolid;
        }
        public static SledgeFormats.Solid WriteSolid(SledgeRegular.Solid solid)
        {
            return new SledgeFormats.Solid()
            {
                Color = solid.Color.Color,
                Faces = solid.Faces.Select(x => Face.WriteFace(x)).ToList(),
                Children = solid.Hierarchy.Select(x => MapObject.WriteMapObject(x)).ToList(),
            };
        }
    }
}
