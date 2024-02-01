using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Rendering.Resources;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.BspEditor.Tools.Draggable;
using Sledge.BspEditor.Tools.Properties;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Hotkeys;
using Sledge.Common.Shell.Settings;
using Sledge.Common.Translations;
using Sledge.DataStructures.Geometric;
using Sledge.Providers.Texture;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Pipelines;
using Sledge.Rendering.Primitives;
using Sledge.Rendering.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static Sledge.BspEditor.Primitives.MapObjects.MapObjectExtensions;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace Sledge.BspEditor.Tools.Prefab
{
	[Export(typeof(ITool))]
	[OrderHint("I")]
	[AutoTranslate]
	internal class PrefabTool : BaseDraggableTool
	{
		private int _selectionBoxBackgroundOpacity = 64;

		private BoxDraggableState _state;

		private List<IMapObject> _preview;

		private bool _updatePreview = false;

		public override Image GetIcon() => Resources.Tool_Prefab;

		public override string GetName() => "PrefabTool";

		public PrefabTool()
		{
			_state = new BoxDraggableState(this);

			_state.BoxColour = Color.Turquoise;
			_state.FillColour = Color.FromArgb(_selectionBoxBackgroundOpacity, Color.Green);
			_state.State.Changed += BoxChanged;
			States.Add(_state);
			Usage = ToolUsage.Both;

		}
		protected override IEnumerable<Subscription> Subscribe()
		{
			yield return Oy.Subscribe<RightClickMenuBuilder>("MapViewport:RightClick", b =>
			{
				b.AddCommand("PrefabTool:CreatePrefab");
			});
		}
		private void BoxChanged(object sender, EventArgs e)
		{
			_updatePreview = true;
		}


		private List<IMapObject> GetPreview(MapDocument document)
		{
			if (_updatePreview)
			{
				var bbox = new Box(_state.State.Start, _state.State.End);
				//var brush = GetBrush(document, bbox, new UniqueNumberGenerator()).FindAll();
				//_preview = brush;
			}

			_updatePreview = false;
			return _preview ?? new List<IMapObject>();
		}
		protected override void Render(MapDocument document, BufferBuilder builder, ResourceCollector resourceCollector)
		{
			if (_state.State.Action != BoxAction.Idle)
			{
				// Force this work to happen on a new thread so waiting on it won't block the context
				Task.Run(async () =>
				{
					foreach (var obj in GetPreview(document).OfType<Solid>())
					{
						await Convert(builder, document, obj, resourceCollector);
					}
				}).Wait();
			}

			base.Render(document, builder, resourceCollector);
		}
		private async Task Convert(BufferBuilder builder, MapDocument document, IMapObject obj, ResourceCollector resourceCollector)
		{
			var solid = (Solid)obj;
			var faces = solid.Faces.Where(x => x.Vertices.Count > 2).ToList();

			// Pack the vertices like this [ f1v1 ... f1vn ] ... [ fnv1 ... fnvn ]
			var numVertices = (uint)faces.Sum(x => x.Vertices.Count);

			// Pack the indices like this [ solid1 ... solidn ] [ wireframe1 ... wireframe n ]
			var numSolidIndices = (uint)faces.Sum(x => (x.Vertices.Count - 2) * 3);
			var numWireframeIndices = numVertices * 2;

			var points = new VertexStandard[numVertices];
			var indices = new uint[numSolidIndices + numWireframeIndices];

			var c = Color.Turquoise;
			var colour = new Vector4(c.R, c.G, c.B, c.A) / 255f;

			c = Color.FromArgb(192, Color.Turquoise);
			var tint = new Vector4(c.R, c.G, c.B, c.A) / 255f;

			var tc = await document.Environment.GetTextureCollection();

			var vi = 0u;
			var si = 0u;
			var wi = numSolidIndices;
			foreach (var face in faces)
			{
				var t = await tc.GetTextureItem(face.Texture.Name);
				var w = t?.Width ?? 0;
				var h = t?.Height ?? 0;

				var offs = vi;
				var numFaceVerts = (uint)face.Vertices.Count;

				var textureCoords = face.GetTextureCoordinates(w, h).ToList();

				var normal = face.Plane.Normal;
				for (var i = 0; i < face.Vertices.Count; i++)
				{
					var v = face.Vertices[i];
					points[vi++] = new VertexStandard
					{
						Position = v,
						Colour = colour,
						Normal = normal,
						Texture = new Vector2(textureCoords[i].Item2, textureCoords[i].Item3),
						Tint = tint,
						Flags = t == null ? VertexFlags.FlatColour : VertexFlags.None
					};
				}

				// Triangles - [0 1 2]  ... [0 n-1 n]
				for (uint i = 2; i < numFaceVerts; i++)
				{
					indices[si++] = offs;
					indices[si++] = offs + i - 1;
					indices[si++] = offs + i;
				}

				// Lines - [0 1] ... [n-1 n] [n 0]
				for (uint i = 0; i < numFaceVerts; i++)
				{
					indices[wi++] = offs + i;
					indices[wi++] = offs + (i == numFaceVerts - 1 ? 0 : i + 1);
				}
			}

			var groups = new List<BufferGroup>();

			uint texOffset = 0;
			foreach (var f in faces)
			{
				var texInd = (uint)(f.Vertices.Count - 2) * 3;

				var opacity = tc.GetOpacity(f.Texture.Name);
				var t = await tc.GetTextureItem(f.Texture.Name);
				var transparent = opacity < 0.95f || t?.Flags.HasFlag(TextureFlags.Transparent) == true;

				var texture = t == null ? string.Empty : $"{document.Environment.ID}::{f.Texture.Name}";

				groups.Add(transparent
					? new BufferGroup(PipelineType.TexturedAlpha, CameraType.Perspective, f.Origin, texture, texOffset, texInd)
					: new BufferGroup(PipelineType.TexturedOpaque, CameraType.Perspective, texture, texOffset, texInd)
				);

				texOffset += texInd;

				if (t != null) resourceCollector.RequireTexture(t.Name);
			}

			groups.Add(new BufferGroup(PipelineType.Wireframe, solid.IsSelected ? CameraType.Both : CameraType.Orthographic, numSolidIndices, numWireframeIndices));

			builder.Append(points, indices, groups);
		}
		protected override void MouseDown(MapDocument document, MapViewport viewport, PerspectiveCamera camera, ViewportEvent e)
		{
			var tf = document.Map.Data.GetOne<DisplayFlags>() ?? new DisplayFlags();
			IgnoreOptions iopt = (tf.HideClipTextures ? IgnoreOptions.IgnoreClip : IgnoreOptions.None) | (tf.HideNullTextures ? IgnoreOptions.IgnoreNull : IgnoreOptions.None);


			var (rayStart, rayEnd) = camera.CastRayFromScreen(new Vector3(e.X, e.Y, 0));
			var ray = new Line(rayStart, rayEnd);

			// Grab all the elements that intersect with the ray
			var (_,intersectingPoint) = document.Map.Root.GetIntersectionPointOnSurface(ray);
			Vector3 spawnPosition = rayStart;

			if (intersectingPoint != null)
			{
				spawnPosition = intersectingPoint.Value;
			}


			Oy.Publish("PrefabTool:CreatePrefab", spawnPosition);

			base.MouseDown(document, viewport, camera, e);
		}
	}
}
