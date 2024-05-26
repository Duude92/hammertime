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
using Sledge.BspEditor.Primitives.MapObjectData;
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

		public override Vector3 Origin => _sphereHandles.First?.Value.Origin ?? Vector3.Zero;
		public PathState Next { get; set; }
		public PathProperty Property { get; set; }


		private Vector3 _pointDragStart;
		private Vector3 _pointDragGridOffset;

		public bool IsSelected { get => _sphereHandles.FirstOrDefault(h => h.IsSelected) != null; }


		LinkedList<SphereHandle> _sphereHandles = new LinkedList<SphereHandle>();
		public SphereHandle Head => _sphereHandles.First();
		private PathTool.PathTool _pathTool;

		public PathState(Vector3 start, PathTool.PathTool pathTool) : this(pathTool)
		{
			_sphereHandles.AddFirst(new SphereHandle(start, this, pathTool) { ID = 0 });
		}
		public PathState(PathTool.PathTool pathTool)
		{
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
					var nextHandle = new SphereHandle(position, this, _pathTool) { ID = _sphereHandles.Last.Value.ID + 1 };
					nextHandle.IsSelected = true;
					_sphereHandles.AddBefore(node, nextHandle);
				}
				node = node.Next;
			}
		}
		public void AddNode(Vector3 location)
		{
			_sphereHandles.Last().IsSelected = false;
			_sphereHandles.AddLast(new SphereHandle(location, this, _pathTool) { ID = _sphereHandles.Last.Value.ID + 1 });
			_sphereHandles.Last().IsSelected = true;
		}
		public PathState AddRange(IEnumerable<SphereHandle> handles)
		{
			foreach (SphereHandle handle in handles)
			{
				handle.Path = this;
				_sphereHandles.AddLast(handle);
			}
			return this;
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


		public override bool CanDrag(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 position) => false;
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

		public override void Render(MapDocument document, BufferBuilder builder)
		{
			var path = this;

			var color = Color.Goldenrod.ToVector4();

			var vertices = path._sphereHandles.Select(n => new VertexStandard { Position = n.Origin, Colour = color });
			var nodeCount = path._sphereHandles.Count;
			var indices = Enumerable.Range(0, nodeCount).SelectMany(i => (i == 0 || i == nodeCount) ? new[] { (uint)i } : new[] { (uint)i, (uint)i });


			//uint sectorCount = 8;
			//uint stackCount = 8;
			//var vertices = GenerateSphereVertices(20, sectorCount, stackCount);
			//var vertexStandart = vertices.Select(v => new VertexStandard { Position = v + pathNode.Position, Colour = color, Tint = color }).ToArray();
			//var indices = GenerateSphereIndices(sectorCount, stackCount).ToArray();
			var groups = new List<BufferGroup>();
			//groups.Add(new BufferGroup(PipelineType.TexturedOpaque, CameraType.Both, (uint)0, (uint)indices.Length));
			groups.Add(new BufferGroup(PipelineType.Wireframe, CameraType.Both, (uint)0, (uint)indices.Count()));


			builder.Append(vertices, indices, groups);
			builder.Complete();

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
		public struct PathProperty
		{
			public Path.PathDirection Direction;
			public string Name;
			public string ClassName;
			public Vector3 Position;
		}
	}
}
