using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.DataStructures.Geometric;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Overlay;
using Sledge.Rendering.Pipelines;
using Sledge.Rendering.Resources;
using Sledge.Rendering.Viewports;
using System.Collections.Generic;
using System;
using System.Drawing;
using System.Numerics;
using System.Windows.Forms;
using System.Linq;
using Sledge.Rendering.Primitives;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations.Mutation;

namespace Sledge.BspEditor.Tools.Draggable
{
	public class PathNodeHandle : BaseDraggable
	{
		private Vector3 _position;
		public override Vector3 Origin => _position;
		public bool IsSelected { get; set; }
		public bool IsDragging { get; set; } = false;
		public string Name { get; set; }
		public int? ID { get; set; } = null;
		public Dictionary<string, string> Properties { get; set; }

		private Vector3 _draggingPosion;

		public bool IsHighlighted { get; private set; }
		public PathState Path { get; set; }
		private PathTool.PathTool _pathTool;
		public PathNodeHandle(Vector3 position, PathState path, PathTool.PathTool pathTool) : this(position, pathTool)
		{
			Path = path;
		}
		public PathNodeHandle(Vector3 position, PathTool.PathTool pathTool)
		{
			_pathTool = pathTool;
			_position = position;
			Properties = new Dictionary<string, string>();
		}
		private Subscription _subscription;
		private void Subscribe() => _subscription = Oy.Subscribe<RightClickMenuBuilder>("MapViewport:RightClick", b =>
			{
				b.Clear();
				b.AddCommand("BspEditor:PathToolDeleteNodes");
				b.AddCommand("BspEditor:PathInsertNode");
				b.AddCommand("BspEditor:NodeProperties", new[] { this });

				b.AddSeparator();
				b.AddCommand("BspEditor:PathProperties", new { Path });
				b.AddCommand("BspEditor:PathToolDeletePath", new[] { Path });
				b.AddCommand("BspEditor:PathToolSelectPath", new[] { Path });

				b.AddSeparator();
				b.AddCommand("BspEditor:PathToolConvertPath", new[] { Path });
			});

		public override bool CanDrag(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 position)
		{
			const int width = 5;
			var screenPosition = camera.WorldToScreen(_position);
			var diff = (e.Location - screenPosition).Absolute();
			return diff.X < width && diff.Y < width;
		}

		public override void Click(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 position)
		{
		}

		public override void Highlight(MapDocument document, MapViewport viewport)
		{
			Subscribe();
			IsHighlighted = true;
			viewport.Control.Cursor = Cursors.SizeAll;

		}
		public override void Unhighlight(MapDocument document, MapViewport viewport)
		{
			_subscription.Dispose();
			IsHighlighted = false;
			viewport.Control.Cursor = Cursors.Default;
		}

		public override void Render(MapDocument document, BufferBuilder builder)
		{
			var pathNode = this;
			uint sectorCount = 10;
			uint stackCount = 10;
			var vertices = GenerateSphereVertices(20, sectorCount, stackCount);
			var color = Color.Green.ToVector4();
			var vertexStandart = vertices.Select(v => new VertexStandard { Position = v + pathNode.Origin, Tint = color }).ToArray();
			var indices = GenerateSphereIndices(sectorCount, stackCount).ToArray();
			var groups = new List<BufferGroup>();

			groups.Add(new BufferGroup(PipelineType.TexturedOpaque, CameraType.Perspective, (uint)0, (uint)indices.Length));

			builder.Append(vertexStandart, indices, groups);
			builder.Complete();
		}
		static List<Vector3> GenerateSphereVertices(float radius, uint sectorCount, uint stackCount)
		{
			List<Vector3> vertices = new List<Vector3>();

			float x, y, z, xy; // vertex position
			float sectorStep = 2 * (float)Math.PI / sectorCount;
			float stackStep = (float)Math.PI / stackCount;
			float sectorAngle, stackAngle;

			for (int i = 0; i <= stackCount; ++i)
			{
				stackAngle = (float)Math.PI / 2 - i * stackStep; // starting from pi/2 to -pi/2
				xy = radius * (float)Math.Cos(stackAngle); // r * cos(u)
				z = radius * (float)Math.Sin(stackAngle);  // r * sin(u)

				// add (sectorCount+1) vertices per stack
				// the first and last vertices have same position and normal, but different tex coords
				for (int j = 0; j <= sectorCount; ++j)
				{
					sectorAngle = j * sectorStep; // starting from 0 to 2pi

					// vertex position (x, y, z)
					x = xy * (float)Math.Cos(sectorAngle); // r * cos(u) * cos(v)
					y = xy * (float)Math.Sin(sectorAngle); // r * cos(u) * sin(v)
					vertices.Add(new Vector3(x, y, z));
				}
			}

			return vertices;
		}

		static List<uint> GenerateSphereIndices(uint sectorCount, uint stackCount)
		{
			List<uint> indices = new List<uint>();
			uint k1, k2;

			for (uint i = 0; i < stackCount; ++i)
			{
				k1 = i * (sectorCount + 1); // beginning of current stack
				k2 = k1 + sectorCount + 1;  // beginning of next stack

				for (int j = 0; j < sectorCount; ++j, ++k1, ++k2)
				{
					// 2 triangles per sector excluding the first and last stacks
					if (i != 0)
					{
						indices.Add(k1 + 1);
						indices.Add(k2 + 1);
						indices.Add(k1);
					}

					if (i != (stackCount - 1))
					{
						indices.Add(k1 + 0);
						indices.Add(k2 + 1);
						indices.Add(k2);
					}

				}
			}

			return indices;
		}
		public void MoveTo(Vector3 position)
		{
			_position = position;
		}
		protected (Vector3, Vector3) GetWorldPositionAndScreenOffset(ICamera camera)
		{
			const int distance = 6;
			var mid = camera.Flatten(_position);
			Vector3 center;
			Vector3 offset;
			center = new Vector3(mid.X, mid.Y, 0);
			offset = new Vector3(distance, distance, 0);

			return (camera.Expand(center), offset);
		}

		public override void Render(IViewport viewport, OrthographicCamera camera, Vector3 worldMin, Vector3 worldMax, I2DRenderer im)
		{
			//if (State.State.Action != BoxAction.Drawn) return;

			var (wpos, soff) = GetWorldPositionAndScreenOffset(camera);
			var spos = camera.WorldToScreen(wpos) + soff;

			const int size = 4;

			const int boxOffset = 3;

			im.AddCircle(new Vector2(spos.X - size, spos.Y - size), 10, Color.Bisque);

			im.AddRectFilled(new Vector2(spos.X - size - boxOffset, spos.Y - size - boxOffset), new Vector2(spos.X - size + boxOffset, spos.Y - size + boxOffset), IsSelected ? Color.Red : Color.Bisque);
		}
		public override void Render(IViewport viewport, PerspectiveCamera camera, I2DRenderer im)
		{
			return;
		}

		public override void StartDrag(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 position)
		{
			IsDragging = true;
			_draggingPosion = camera.ZeroUnusedCoordinate(position);
			base.StartDrag(document, viewport, camera, e, position);
		}
		public override void Drag(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 lastPosition, Vector3 position)
		{
			if (IsDragging)
			{
				var draggingPosition = _pathTool.SnapIfNeeded(camera.Expand(position));
				var diff = new Vector3(draggingPosition.X != 0 ? 0 : Origin.X, draggingPosition.Y != 0 ? 0 : Origin.Y, draggingPosition.Z != 0 ? 0 : Origin.Z);
				_position = draggingPosition + diff;
			}
			base.Drag(document, viewport, camera, e, lastPosition, position);
		}
		public override void EndDrag(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 position)
		{
			IsDragging = false;
			base.EndDrag(document, viewport, camera, e, position);
		}
	}
}
