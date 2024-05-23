using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.DataStructures.Geometric;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Overlay;
using Sledge.Rendering.Pipelines;
using Sledge.Rendering.Primitives;
using Sledge.Rendering.Resources;
using Sledge.Rendering.Viewports;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;

namespace Sledge.BspEditor.Tools.Draggable
{
	public class PathState : BaseDraggable, IDraggableState
	{
		public IEnumerable<SphereHandle> Handles => _sphereHandles;

		public BoxAction Action { get; set; }

		public override Vector3 Origin => _sphereHandles.First.Value.Origin;
		public PathState Next { get; set; }
		public PathProperty Property { get; set; }


		private Vector3 _pointDragStart;
		private Vector3 _pointDragGridOffset;

		public bool IsSelected { get => _sphereHandles.FirstOrDefault(h => h.IsSelected) != null; }


		LinkedList<SphereHandle> _sphereHandles = new LinkedList<SphereHandle>();
		public SphereHandle Head => _sphereHandles.First();
		private PathTool.PathTool _pathTool;

		public PathState(Vector3 start, PathTool.PathTool pathTool)
		{
			_sphereHandles.AddFirst(new SphereHandle(start, this, pathTool));
			_pathTool = pathTool;
			Oy.Subscribe("PathTool:InsertNode", new Action(InsertNode));
			Oy.Subscribe("PathTool:Delete", new Action(DeleteSelection));

		}
		private void DeleteSelection()
		{
			var selection = _sphereHandles.Where(h => h.IsSelected).ToList();
			selection.ForEach(s => _sphereHandles.Remove(s));
		}
		private void InsertNode()
		{
			var node = _sphereHandles.First;
			while (node != null)
			{
				if (node.Value.IsSelected)
				{
					var prev = node.Previous ?? node;
					var position = (prev.Value.Origin + node.Value.Origin) / 2;
					node.Value.IsSelected = false;
					var nextHandle = new SphereHandle(position, this, _pathTool);
					nextHandle.IsSelected = true;
					_sphereHandles.AddBefore(node, nextHandle);
				}
				node = node.Next;
			}
		}
		public void AddNode(Vector3 location)
		{
			_sphereHandles.Last().IsSelected = false;
			_sphereHandles.AddLast(new SphereHandle(location, this, _pathTool));
			_sphereHandles.Last().IsSelected = true;
		}
		public override void StartDrag(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 position)
		{
			//_head.IsDragging = true;


			//_pointDragStart = camera.ZeroUnusedCoordinate(position);

			//base.StartDrag(document, viewport, camera, e, position);
			return;
		}
		public override void Drag(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 lastPosition, Vector3 position)
		{
			//_head.MoveTo(position);
			//base.Drag(document, viewport, camera, e, lastPosition, position);
		}


		public override bool CanDrag(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 position) => true;
		//_head.CanDrag(document, viewport, camera, e, position);


		public override void Click(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 position)
		{
			return;
			throw new NotImplementedException();
		}

		public IEnumerable<IDraggable> GetDraggables()
		{
			//if (Action == BoxAction.Idle || Action == BoxAction.Drawing) yield break;
			return _sphereHandles;
		}

		public override void Highlight(MapDocument document, MapViewport viewport)
		{
			return;
			throw new NotImplementedException();
		}

		// Method to generate circle vertices in 3D space
		private static List<Vector3> GenerateCircleVertices(Vector3 center, float radius, Vector3 normal, int segments)
		{
			List<Vector3> vertices = new List<Vector3>();

			// Create a basis with the normal vector as one of the axes
			Vector3 tangent1, tangent2;
			if (normal == Vector3.UnitZ || normal == -Vector3.UnitZ)
			{
				tangent1 = Vector3.UnitY;
			}
			else
			{
				tangent1 = Vector3.Cross(normal, Vector3.UnitZ).Normalise();
			}
			tangent2 = Vector3.Cross(normal, tangent1).Normalise();

			// Generate vertices
			for (int i = 0; i < segments; i++)
			{
				float angle = (float)i / segments * 2.0f * MathF.PI;
				float x = MathF.Cos(angle) * radius;
				float y = MathF.Sin(angle) * radius;

				// Convert 2D circle point to 3D
				Vector3 point = center + tangent1 * x + tangent2 * y;
				vertices.Add(point);
			}

			return vertices;
		}

		public override void Render(MapDocument document, BufferBuilder builder)
		{
			if (true)
			{
				// Draw a box around the point
				var c = Origin;

				const uint numVertices = 8 * 3;
				const uint numWireframeIndices = numVertices * 2;

				var points = new VertexStandard[8];
				var indices = new uint[numWireframeIndices];

				var col = Color.BlueViolet;
				var colour = new Vector4(col.R, col.G, col.B, 255) / 255;

				var vi = 0u;
				var wi = 0u;
				foreach (var vertex in GenerateCircleVertices(c, 20, Vector3.UnitZ, 8))
				{
					var offs = vi;


					points[vi++] = new VertexStandard
					{
						Position = vertex,
						Colour = colour,
						Tint = Vector4.One
					};

					// Lines - [0 1] ... [n-1 n] [n 0]
					for (uint i = 0; i < 3; i++)
					{
						indices[wi++] = offs + i;
						indices[wi++] = offs + (i == 4 - 1 ? 0 : i + 1);
					}
				}

				var groups = new[]
				{
					new BufferGroup(PipelineType.Wireframe, CameraType.Perspective, 0, numWireframeIndices)
				};

				builder.Append(points, indices, groups);
			}
		}

		public override void Render(IViewport viewport, OrthographicCamera camera, Vector3 worldMin, Vector3 worldMax, I2DRenderer im)
		{
			var @enum = _sphereHandles.GetEnumerator();
			@enum.MoveNext();
			var current = @enum.Current;
			while (@enum.MoveNext())
			{
				var next = @enum.Current;
				im.AddLine(camera.WorldToScreen(current.Origin).ToVector2(), camera.WorldToScreen(next.Origin).ToVector2(), Color.YellowGreen);
				current = next;
			}
		}

		protected (Vector3, Vector3) GetWorldPositionAndScreenOffset(ICamera camera)
		{
			const int distance = 6;
			var mid = camera.Flatten(Origin);
			Vector3 center;
			Vector3 offset;
			center = new Vector3(mid.X, mid.Y, 0);
			offset = new Vector3(distance, distance, 0);

			return (camera.Expand(center), offset);
		}
		public override void Render(IViewport viewport, PerspectiveCamera camera, I2DRenderer im)
		{
			return;
		}

		public override void Unhighlight(MapDocument document, MapViewport viewport)
		{
			return;
		}
		public static implicit operator bool(PathState sphere) => sphere != null;
		public enum PathDirection
		{
			ONE_WAY,
			CIRCULAR,
			PING_PONG
		}
		public struct PathProperty
		{
			public PathDirection Direction;
			public string Name;
			public string ClassName;
			public Vector3 Position;
		}
	}
}
