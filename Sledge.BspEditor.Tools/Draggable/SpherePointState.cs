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
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.AxHost;

namespace Sledge.BspEditor.Tools.Draggable
{
	internal class SpherePointState : BaseDraggable, IDraggableState
	{
		public BoxAction Action { get; set; }

		public override Vector3 Origin => _sphereHandle.Origin;
		public SpherePointState Next;
		public bool IsSelected { get => _sphereHandle.IsSelected; set => _sphereHandle.IsSelected = value; }

		private SphereHandle _sphereHandle { get; set; }
		public SpherePointState(Vector3 start)
		{
			_sphereHandle = new SphereHandle(start);
		}

		public override bool CanDrag(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 position)
		{
			throw new NotImplementedException();
		}

		public override void Click(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 position)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<IDraggable> GetDraggables()
		{
			//if (Action == BoxAction.Idle || Action == BoxAction.Drawing) yield break;
			yield return _sphereHandle;
		}

		public override void Highlight(MapDocument document, MapViewport viewport)
		{
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
			{
				DrawCircle(camera, im);
				if (Next)
				{
					im.AddLine(camera.WorldToScreen(Origin).ToVector2(), camera.WorldToScreen(Next.Origin).ToVector2(), Color.YellowGreen);
				}
			}
		}

		private void DrawCircle(ICamera camera, I2DRenderer im)
		{
			var (wpos, soff) = GetWorldPositionAndScreenOffset(camera);
			var spos = camera.WorldToScreen(wpos) + soff;

			const int size = 4;

			im.AddCircle(new Vector2(spos.X - size, spos.Y - size), 10, Color.Bisque);
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
			throw new NotImplementedException();
		}
		public static implicit operator bool(SpherePointState sphere) => sphere != null;
	}
}
