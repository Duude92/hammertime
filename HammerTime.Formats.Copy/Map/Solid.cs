using System;
using System.Linq;
using SledgeRegular = Sledge.BspEditor.Primitives.MapObjects;
using SledgeFormats = Sledge.Formats.Map.Objects;
using Sledge.BspEditor.Primitives;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.Formats.Bsp.Lumps;
using Sledge.DataStructures.Geometric;
namespace HammerTime.Formats.Map
{
	internal class Solid
	{
		public static SledgeRegular.Solid FromFmt(SledgeFormats.Solid solid, UniqueNumberGenerator ung)
		{
			var newSolid = new SledgeRegular.Solid(ung.Next("MapObject"));
			newSolid.Data.AddRange(solid.Faces.Select(x => Face.FromFmt(x, ung)));
			newSolid.Data.Add(new ObjectColor(solid.Color));


			var poly = new Polyhedron(newSolid.Faces.Select(x => x.Plane));

			foreach (var face in newSolid.Faces)
			{
				try
				{
					var pg = poly.Polygons.FirstOrDefault(x => x.Plane is not null ? x.Plane.Normal.EquivalentTo(face.Plane.Normal, 0.0075f) : false); // Magic number that seems to match VHE

					if (pg != null)
					{
						face.Vertices.Clear();
						face.Vertices.AddRange(pg.Vertices);
					}
					else
					{
						face.Vertices.Clear();
						face.Plane = new Plane(System.Numerics.Vector3.Zero, 0);
					}
				}
				catch (Exception ex)
				{
					//TODO: fix polyhedron creation, some polys have only 2 vertices
					Console.WriteLine(ex.ToString());
				}

			}



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
