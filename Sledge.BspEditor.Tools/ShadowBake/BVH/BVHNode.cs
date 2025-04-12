using Sledge.BspEditor.Primitives;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.DataStructures.Geometric;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sledge.BspEditor.Tools.ShadowBake.BVH
{
	partial class BVHNode : BVHAbstract
	{
		public  BVHAbstract Left;
		public  BVHAbstract Right;
		public BVHNode()
		{
			
		}
		public BVHNode(Box bounds, BVHAbstract left, BVHAbstract right)
		{
			this.Bounds = bounds;
			this.Left = left;
			this.Right = right;
		}

		public static Box GetBoundingBox(IEnumerable<Solid> solids)
		{
			return new Box(solids.Select(x => x.BoundingBox).Where(x => x != null).DefaultIfEmpty(Box.Empty));
		}
	}
}
