using System.Runtime.InteropServices;

namespace Sledge.Providers.Model.Mdl10.Format
{
	[StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
	public struct SequenceGroup
    {
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
		public string Label;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
		public string Name;
		public int unused1;
		public int unused2;

	}
}