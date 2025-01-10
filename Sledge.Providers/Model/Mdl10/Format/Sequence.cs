using System.Numerics;
using System.Runtime.InteropServices;

namespace Sledge.Providers.Model.Mdl10.Format
{
	[StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
	public struct SequenceHeader
	{
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
		public string Name;

		public float Framerate;
		public int Flags;

		public int Activity;
		public int ActivityWeight;

		public int NumEvents;
		public int EventIndex;

		public int NumFrames;

		public int NumPivots;
		public int PivotIndex;

		public int MotionType;
		public int MotionBone;
		public Vector3 LinearMovement;
		public int AutoMovePositionIndex;
		public int AutoMoveAngleIndex;

		public Vector3 Min;
		public Vector3 Max;

		public int NumBlends;
		public int AnimationIndex;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
		public int[] BlendType;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
		public float[] BlendStart;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
		public float[] BlendEnd;
		public int BlendParent;

		public int SequenceGroup;

		public int EntryNode;
		public int ExitNode;
		public int NodeFlags;

		public int NextSequence;
	}
	public struct Sequence
	{
		public SequenceHeader Header;
		public Blend[] Blends;
	}
}