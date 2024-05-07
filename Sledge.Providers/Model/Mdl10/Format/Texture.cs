using System;
using System.Runtime.InteropServices;

namespace Sledge.Providers.Model.Mdl10.Format
{
	[StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
	public struct TextureHeader
	{
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
		public string Name;
		public TextureFlags Flags;
		public int Width;
		public int Height;
		public int Index;
	}
	public struct Texture
	{
		public TextureHeader Header;
		public byte[] Data { get; set; }
        public byte[] Palette { get; set; }
        public Texture((byte[], byte[]) tuple, TextureHeader header)
        {
			Data = tuple.Item1;
			Header = header;
			Palette = tuple.Item2;
        }
    }
}