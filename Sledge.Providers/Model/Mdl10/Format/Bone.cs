using System.Numerics;
using System.Runtime.InteropServices;

namespace Sledge.Providers.Model.Mdl10.Format
{
	[StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
	public struct Bone
    {
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
		public string Name;
        public int Parent;
        public int Flags;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public int[] Controllers;
        public Vector3 Position;
        public Vector3 Rotation;
        public Vector3 PositionScale;
        public Vector3 RotationScale;
    }
}