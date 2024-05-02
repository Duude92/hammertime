using System.Runtime.InteropServices;

namespace Sledge.Providers.Model.Mdl10.Format
{
	[StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
	public struct BodyPart
    {
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
		public string Name;
        public int NumModels;
        public int Base;
        public int ModelIndex;

        public Model[] Models {  get; set; }
    }
}