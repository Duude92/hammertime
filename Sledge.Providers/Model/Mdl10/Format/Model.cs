using System.Numerics;
using System.Runtime.InteropServices;

namespace Sledge.Providers.Model.Mdl10.Format
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct ModelHeader
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string Name;
        public int Type;
        public float Radius;

        public int NumMesh;
        public int MeshIndex;

        public int NumVerts;
        public int VertInfoIndex;
        public int VertIndex;

        public int NumNormals;
        public int NormalInfoIndex;
        public int NormalIndex;

        public int NumGroups; // Not used
        public int GroupIndex;
    }
	public struct Model
	{
        public ModelHeader Header;
		public Mesh[] Meshes {  get; set; }
        public MeshVertex[] Vertices { get; set; }
    }
}