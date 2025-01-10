using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;

namespace Sledge.Providers.Model.Mdl10.Format
{
	public struct MeshHeader
	{
		public int NumTriangles;
		public int TriangleIndex;
		public int SkinRef;
		public int NumNormals;
		public int NormalIndex;
	}
	public struct Trivert
	{
		public short vertindex;
		public short normindex;
		public short s;
		public short t;
	}
	public struct TriSequence
	{
		public short TriCountDir;
		public Trivert[] TriVerts;
	}
	public struct Mesh
	{
		public MeshHeader Header;

		public MeshVertex[] Vertices { get; set; }
		public TriSequence[] Sequences { get; set; }
	}
}