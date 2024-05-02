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
    public struct Mesh
    {
		public MeshHeader Header;

        public MeshVertex[] Vertices { get; set; }
    }
}