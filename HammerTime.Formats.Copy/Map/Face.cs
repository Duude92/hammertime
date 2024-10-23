using SledgeRegular = Sledge.BspEditor.Primitives.MapObjects;
using SledgeFormats = Sledge.Formats.Map.Objects;
using SledgeFace = Sledge.BspEditor.Primitives.MapObjectData.Face;
using Sledge.BspEditor.Primitives;
using System.Linq;
using System.Numerics;


namespace HammerTime.Formats.Map
{
	internal class Face
	{
		public static SledgeFace FromFmt(SledgeFormats.Face face, UniqueNumberGenerator ung)
		{
			var newFace = new SledgeFace(ung.Next("Face"))
			{
				Texture = { Name = face.TextureName,
				UAxis = face.UAxis,
				VAxis = face.VAxis,
				XScale = face.XScale,
				YScale = face.YScale,
				XShift = Prefab.IsRmf?face.YShift : face.XShift,
				YShift = Prefab.IsRmf?face.Rotation : face.YShift,
				Rotation = Prefab.IsRmf?face.XShift : face.Rotation,
				}
			};
			newFace.Plane = new Sledge.DataStructures.Geometric.Plane(face.Plane.Normal, face.Plane.D);

			newFace.Vertices.AddRange(face.Vertices.ToArray().Reverse());

			return newFace;
		}

		public static SledgeFormats.Face WriteFace(SledgeFace face)
		{
			return new SledgeFormats.Face()
			{
				TextureName = face.Texture.Name,
				Plane = new Plane(face.Plane.Normal, face.Plane.D),
				Vertices = face.Vertices.Reverse().ToList(),
				UAxis = face.Texture.UAxis,
				VAxis = face.Texture.VAxis,
				XScale = face.Texture.XScale,
				YScale = face.Texture.YScale,
				XShift = Prefab.IsRmf ? face.Texture.Rotation : face.Texture.XShift,
				YShift = Prefab.IsRmf ? face.Texture.XShift : face.Texture.YShift,
				Rotation = Prefab.IsRmf ? face.Texture.YShift : face.Texture.Rotation,
			};

		}

	}
}
