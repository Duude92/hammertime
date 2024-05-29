using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification.Operations.Mutation;
using Sledge.BspEditor.Modification.Operations.Selection;
using Sledge.BspEditor.Modification.Operations.Tree;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Primitives;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Rendering.Resources;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.BspEditor.Tools.Draggable;
using Sledge.BspEditor.Tools.Properties;
using Sledge.Common.Shell.Components;
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
using System.Threading.Tasks;
using static Sledge.BspEditor.Primitives.MapObjects.MapObjectExtensions;
using Sledge.Common.Shell.Context;
using Sledge.Formats.Map.Formats;
using System.Windows.Forms;
using System.IO;

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

		private string _activeLibraryPath = "";

		private int _selectedPrefabIndex = 0;

		private MapDocument _previewDocument = null;

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

			Oy.Subscribe<int>("PrefabTool:CreatePrefab", CreatePrefab);

		}

		private List<IMapObject> GetPrefab(int prefabId, UniqueNumberGenerator ung, Map map)
		{
			if (String.IsNullOrEmpty(_activeLibraryPath)) return null;
			WorldcraftPrefabLibrary lib;
			try
			{
				lib = WorldcraftPrefabLibrary.FromFile(_activeLibraryPath);
			}
			catch { 
				lib = new WorldcraftPrefabLibrary();
			}
			if (lib.Prefabs.Count > prefabId)
				return HammerTime.Formats.Map.Prefab.GetPrefab(lib.Prefabs[prefabId].Map, ung, map);
			return null;
		}
		protected override void ContextChanged(IContext context)
		{
			var libPath = context.Get<string>("PrefabTool:ActiveLibrary");
			_activeLibraryPath = String.IsNullOrEmpty(libPath) ? _activeLibraryPath : libPath;
			_selectedPrefabIndex = context.Get<int>("PrefabTool:PrefabIndex");
			_updatePreview = true;

			base.ContextChanged(context);
		}
		private async void CreatePrefab(int index)
		{
			await CreatePrefab(index, default);
		}
		private void DeletePreview()
		{
			if (_previewDocument != null)
				_previewDocument.RequestClose();
			//FIXME: need to research how Document is disposed
			//MEMORY LEAK

		}
		private async Task CreatePrefab(int index, Vector3 position)
		{
			if (GetDocument() is MapDocument mapDocument)
			{
				if (_preview == null)
				{
					var ung = mapDocument.Map.NumberGenerator;


					var contents = GetPrefab(index, ung, mapDocument.Map);
					if (contents == null) return;

					var transaction = new Transaction();

					var contentCenter = contents.Select(x => x.BoundingBox.Center).Aggregate((acc, x) => (acc + x) / 2);
					position -= contentCenter;
					var translation = Matrix4x4.CreateTranslation(position);

					transaction.Add(new Attach(mapDocument.Map.Root.ID, contents));
					//transaction.Add(new Transform(translation, contents));
					//transaction.Add(new TransformTexturesUniform(translation, contents.SelectMany(x => x.FindAll())));
					transaction.Add(new Deselect(mapDocument.Selection));
					transaction.Add(new Select(contents));


					await MapDocumentOperation.Perform(mapDocument, transaction);
				}
				else
				{
					var items = _preview.Select(x => (IMapObject)x.Copy(mapDocument.Map.NumberGenerator));
					var transaction = new Transaction();
					transaction.Add(new Attach(mapDocument.Map.Root.ID, items));
					transaction.Add(new Deselect(mapDocument.Selection));
					transaction.Add(new Select(items));

					await MapDocumentOperation.Perform(mapDocument, transaction);
					_preview = null;
					DeletePreview();
				}
			}
			await Task.CompletedTask;
		}
		//protected override IEnumerable<Subscription> Subscribe()
		//{
		//	//yield return null;
		//	//yield return Oy.Subscribe<RightClickMenuBuilder>("MapViewport:RightClick", b =>
		//	//{
		//	//	b.AddCommand("PrefabTool:CreatePrefab");
		//	//});
		//}
		private void BoxChanged(object sender, EventArgs e)
		{
			_updatePreview = true;
		}
		private Object _lockObject = new Object();

		private List<IMapObject> GetPreview(MapDocument document, Vector3 position)
		{

			DeletePreview();

			if (_updatePreview)
			{
				lock (_lockObject)
				{
					var md = new MapDocument(new Map(), document.Environment);
					md.Map.Root.Unclone(md.Map.Root);
					//var bbox = new Box(_state.State.Start, _state.State.End);
					_preview = GetPrefab(_selectedPrefabIndex, md.Map.NumberGenerator, md.Map);
					if (_preview == null) return new List<IMapObject>();
					//var brush = GetBrush(document, bbox, new UniqueNumberGenerator()).FindAll();
					//_preview = brush;
					var contentCenter = _preview.Select(x => x.BoundingBox.Center).Aggregate((acc, x) => (acc + x) / 2);
					position -= contentCenter;
					var transaction = new Transaction();
					var translation = Matrix4x4.CreateTranslation(position);

					transaction.Add(new Attach(md.Map.Root.ID, _preview));
					transaction.Add(new Transform(translation, _preview));
					transaction.Add(new TransformTexturesUniform(translation, _preview.SelectMany(x => x.FindAll())));


					MapDocumentOperation.Perform(md, transaction);
				}
			}

			_updatePreview = false;
			return _preview ?? new List<IMapObject>();
		}

		protected override void Render(MapDocument document, BufferBuilder builder, ResourceCollector resourceCollector)
		{
			//if (_state.State.Action != BoxAction.Idle)
			if (_preview != null)
			{
				lock (_lockObject)
				{
					// Force this work to happen on a new thread so waiting on it won't block the context
					Task.Run(async () =>
					{
						foreach (var obj in _preview)
						{
							await Convert(builder, document, obj, resourceCollector);
						}
					}).Wait();
				}
			}

			base.Render(document, builder, resourceCollector);
		}
		private async Task Convert(BufferBuilder builder, MapDocument document, IMapObject obj, ResourceCollector resourceCollector)
		{
			if (obj is Solid solid)
			{
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
			foreach (var child in obj.Hierarchy)
			{
				await Convert(builder, document, child, resourceCollector);
			}
		}
		protected override void MouseDown(MapDocument document, MapViewport viewport, PerspectiveCamera camera, ViewportEvent e)
		{
			var tf = document.Map.Data.GetOne<DisplayFlags>() ?? new DisplayFlags();
			IgnoreOptions iopt = (tf.HideClipTextures ? IgnoreOptions.IgnoreClip : IgnoreOptions.None) | (tf.HideNullTextures ? IgnoreOptions.IgnoreNull : IgnoreOptions.None);

			if (e.Button == MouseButtons.Right) return;

			var (rayStart, rayEnd) = camera.CastRayFromScreen(new Vector3(e.X, e.Y, 0));
			var ray = new Line(rayStart, rayEnd);

			// Grab all the elements that intersect with the ray
			var (_, intersectingPoint) = document.Map.Root.GetIntersectionPointOnSurface(ray);
			Vector3 spawnPosition = rayStart;

			if (intersectingPoint != null)
			{
				spawnPosition = intersectingPoint.Value;
			}

			_updatePreview = true;

			_preview = GetPreview(document, spawnPosition);
			//((Action)(async () => await CreatePrefab(_selectedPrefabIndex, spawnPosition)))();



			base.MouseDown(document, viewport, camera, e);
		}
	}
}
