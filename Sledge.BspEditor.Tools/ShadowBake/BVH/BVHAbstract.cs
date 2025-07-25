
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.DataStructures.Geometric;
using System.Collections.Generic;

namespace Sledge.BspEditor.Tools.ShadowBake.BVH
{
	internal class BVHAbstract
	{
		public Box Bounds;
		public static int GroupId = 0;

		public void GetLeafs(List<List<Solid>> groups, int maxDepth, int currentDepth = 0)
		{
			if (this is BVHLeaf leaf)
			{
				if (groups.Count == 0)
					groups.Add(new List<Solid>());
				groups[^1].Add(leaf.Solid);
			}
			else if (this is BVHNode node)
			{
				if (currentDepth >= maxDepth)
				{
					var group = new List<Solid>();
					node.CollectAllLeafs(group);
					groups.Add(group);
				}
				else
				{
					node.Left.GetLeafs(groups, maxDepth, currentDepth + 1);
					node.Right.GetLeafs(groups, maxDepth, currentDepth + 1);
				}
			}
		}
		private void CollectAllLeafs(List<Solid> solids)
		{
			if (this is BVHLeaf leaf)
				solids.Add(leaf.Solid);
			else if (this is BVHNode node)
			{
				node.Left.CollectAllLeafs(solids);
				node.Right.CollectAllLeafs(solids);
			}
		}
	}
}