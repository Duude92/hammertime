
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.DataStructures.Geometric;
using System.Collections.Generic;

namespace Sledge.BspEditor.Tools.ShadowBake.BVH
{
	internal class BVHAbstract
	{
		public Box Bounds;
		public static int GroupId = 0;

		public void GetLeafs(int nestLevel, int group, List<List<Solid>>solids)
		{
			if (this is BVHLeaf leaf)
			{
				solids[group].Add(leaf.Solid);
			}
			if(this is BVHNode node)
			{
				if(nestLevel!=0)
				{
					node.Left.GetLeafs(nestLevel - 1, group, solids);
					node.Right.GetLeafs(nestLevel - 1, group, solids);
				}
				if(nestLevel == 0)
				{
					solids.Add(new List<Solid>());
					solids.Add(new List<Solid>());

					node.Left.GetLeafs(nestLevel - 1, GroupId, solids);
					node.Right.GetLeafs(nestLevel - 1, ++GroupId, solids);
					GroupId++;
				}
			}

		}
	}
}