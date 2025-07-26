using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.DataStructures.Geometric;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Sledge.BspEditor.Tools.ShadowBake.BVH
{
	partial class BVHNode
	{
		public class BVHBuilder
		{
			public BVHNode BuildBVHIterative(List<Solid> solids, int leafThreshold = 4)
			{
				var root = new BVHNode();
				var workStack = new Stack<(BVHNode Node, List<Solid> Solids)>();
				workStack.Push((root, solids));

				while (workStack.Count > 0)
				{
					var (node, currentSolids) = workStack.Pop();
					if (currentSolids.Count == 0) continue;
					node.Bounds = GetBoundingBox(currentSolids);



					int axis = GetLongestAxis(node.Bounds);
					float mid = axis switch
					{
						0 => (node.Bounds.Start.X + node.Bounds.End.X) * 0.5f,
						1 => (node.Bounds.Start.Y + node.Bounds.End.Y) * 0.5f,
						_ => (node.Bounds.Start.Z + node.Bounds.End.Z) * 0.5f
					};

					var leftList = new List<Solid>();
					var rightList = new List<Solid>();

					foreach (var solid in currentSolids)
					{
						float centerVal = axis switch
						{
							0 => solid.BoundingBox.Center.X,
							1 => solid.BoundingBox.Center.Y,
							_ => solid.BoundingBox.Center.Z
						};

						if (centerVal < mid)
							leftList.Add(solid);
						else
							rightList.Add(solid);
					}

					// In rare degenerate cases where all objects fall to one side,
					// force a balanced split to prevent infinite recursion
					if (leftList.Count == 0 || rightList.Count == 0)
					{
						int half = currentSolids.Count / 2;
						leftList = currentSolids.Take(half).ToList();
						rightList = currentSolids.Skip(half).ToList();
					}


					node.Left = leftList.Count == 1 ? new BVHLeaf(leftList.First()) : new BVHNode();
					node.Right = rightList.Count == 1 ? new BVHLeaf(rightList.First()) : new BVHNode();
					if (node.Right is BVHNode) workStack.Push((node.Right as BVHNode, rightList));
					if (node.Left is BVHNode) workStack.Push((node.Left as BVHNode, leftList));
				}

				return root;
			}

			public BVHAbstract BuildBVH(List<Solid> solids)
			{
				if (solids.Count == 0)
					return null;
				if (solids.Count == 1)
					return new BVHLeaf(solids[0]);
				var bounds = BVHNode.GetBoundingBox(solids);
				int axis = GetLongestAxis(bounds);
				float splitPosition = axis switch
				{
					0 => (bounds.Start.X + bounds.End.X) * 0.5f,
					1 => (bounds.Start.Y + bounds.End.Y) * 0.5f,
					_ => (bounds.Start.Z + bounds.End.Z) * 0.5f,
				};

				var center = bounds.Center;
				var leftSolids = new List<Solid>();
				var rightSolids = new List<Solid>();
				foreach (var solid in solids)
				{
					float axisValue = axis switch
					{
						0 => solid.BoundingBox.Center.X,
						1 => solid.BoundingBox.Center.Y,
						_ => solid.BoundingBox.Center.Z,
					};

					if (axisValue <= splitPosition)
						leftSolids.Add(solid);
					else
						rightSolids.Add(solid);
				}

				var left = BuildBVH(leftSolids);
				var right = BuildBVH(rightSolids);

				return new BVHNode(bounds, left, right);
			}
			private static int GetLongestAxis(Box box)
			{
				Vector3 size = box.End - box.Start;
				if (size.X > size.Y && size.X > size.Z) return 0;
				if (size.Y > size.Z) return 1;
				return 2;
			}
		}

	}
}
