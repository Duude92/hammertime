using System.Numerics;
using System.Runtime.InteropServices;

namespace Sledge.Providers.Model.Mdl10.Format
{
	[StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
	public struct Header
    {
        public ID ID;
        public Version Version;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
		public string Name;
        public int Size;

        public Vector3 EyePosition;
        public Vector3 HullMin;
        public Vector3 HullMax;
        public Vector3 BoundingBoxMin;
        public Vector3 BoundingBoxMax;

        public int Flags;
    }
}