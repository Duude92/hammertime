
using Sledge.BspEditor.Primitives.MapObjects;

namespace Sledge.BspEditor.Tools.ShadowBake.BVH
{
	internal class BVHLeaf : BVHAbstract
	{
		public readonly Solid Solid;

		public BVHLeaf(Solid solid)
		{
			this.Solid = solid;
			this.Bounds = solid.BoundingBox;
		}
	}
}