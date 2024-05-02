using System.Runtime.InteropServices;

namespace Sledge.Providers.Model.Mdl10.Format
{
	[StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
	public struct Texture
    {
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
		public string Name;
        public TextureFlags Flags;
        public int Width;
        public int Height;
        public int Index;

		public byte[] Data { get; set; }
        public byte[] Palette { get; set; }
    }
}