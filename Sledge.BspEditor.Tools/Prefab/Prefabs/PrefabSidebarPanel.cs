using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Documents;
using Sledge.Common.Shell.Hooks;
using Sledge.Common.Translations;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sledge.Formats.Map.Formats;
using System.IO;
using System.Drawing;
using System.Numerics;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Primitives;
using Sledge.Rendering.Engine;
using Sledge.Rendering.Viewports;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Renderables;
using Sledge.Rendering.Resources;
using Sledge.Rendering.Pipelines;
using Sledge.Rendering.Primitives;
using Sledge.Providers.Texture;
using Sledge.BspEditor.Rendering.Scene;
using Sledge.BspEditor.Rendering.Resources;

namespace Sledge.BspEditor.Tools.Prefab
{
	[AutoTranslate]
	[Export(typeof(ISidebarComponent))]
	[Export(typeof(IInitialiseHook))]
	[OrderHint("A")]


	public partial class PrefabSidebarPanel : UserControl, ISidebarComponent, IInitialiseHook
	{
		private WeakReference<MapDocument> _activeDocument = new WeakReference<MapDocument>(null);

		public string Title => "Prefabs";

		public object Control => this;
		private string[] _files = null;
		private WorldcraftPrefabLibrary _activeWorldcraftPrefabLibrary;
		private Control _viewport;
		private IViewport _renderTarget;
		[Import] private Lazy<EngineInterface> _engine;

		private Scene _scene = new Scene();

		private List<IMapObject> _preview = new List<IMapObject>();
		private MapDocument _previewDocument;

		private Veldrid.GraphicsDevice Device;
		private RenderContext RenderContext;

		public PrefabSidebarPanel()
		{
			InitializeComponent();
			CreateHandle();

			InitPrefabLibraries();


		}

		private void InitPrefabLibraries()
		{
			_files = Directory.GetFiles("./prefabs/");

			FileContainer.Items.Clear();

			FileContainer.Items.AddRange(_files.Select(x => System.IO.Path.GetFileNameWithoutExtension(x)).ToArray());
			FileContainer.SelectedIndex = 0;

			UpdatePrefabList();

			Task.Delay(3000).ContinueWith(t =>
			{
				Oy.Publish("Context:Add", new ContextInfo("PrefabTool:ActiveLibrary", _files[0])); //Should be delayed until PrefabTool is created
			});
		}

		private void UpdatePrefabList(int index = 0)
		{
			PrefabList.Items.Clear();
			_activeWorldcraftPrefabLibrary = WorldcraftPrefabLibrary.FromFile(_files[index]);

			PrefabList.Text = null;

			PrefabList.Items.AddRange(_activeWorldcraftPrefabLibrary.Prefabs.Select(x => x.Name).ToArray());
			if (PrefabList.Items.Count > 0)
				PrefabList.SelectedIndex = 0;


		}

		public bool IsInContext(IContext context)
		{
			return context.TryGet("ActiveTool", out PrefabTool _);
		}

		public Task OnInitialise()
		{
			Oy.Subscribe<IDocument>("Document:Activated", DocumentActivated);
			Oy.Subscribe<Change>("MapDocument:Changed", DocumentChanged);


			if (this.InvokeRequired)
				this.Invoke(new Action(() =>
				{
					_renderTarget = _engine.Value.CreateViewport(false);
					_viewport = (Control)_renderTarget;

					this.Controls.Add(_viewport);


					_viewport.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
					_viewport.Location = new Point(4, 254);
					_viewport.Size = new Size(250, 250);


					var cam = new PerspectiveCamera();
					_renderTarget.Camera = cam;
				}));

			Device = _engine.Value.Device;
			RenderContext = new RenderContext(Device);

			_scene.Add(RenderContext);

			var txap = new TexturedAlphaPipeline();
			txap.Create(RenderContext);

			var txop = new TexturedOpaquePipeline();
			txop.Create(RenderContext);

			_pipelines.Add(txap);
			_pipelines.Add(txop);

			return Task.FromResult(0);
		}

		private async Task DocumentActivated(IDocument doc)
		{
			var md = doc as MapDocument;

			_activeDocument = new WeakReference<MapDocument>(md);
			Update(md);
		}

		private async Task DocumentChanged(Change change)
		{
			if (_activeDocument.TryGetTarget(out MapDocument t) && change.Document == t)
			{
				if (change.HasObjectChanges)
				{
					Update(change.Document);
				}
			}
		}

		private void Update(MapDocument document)
		{
			Task.Factory.StartNew(() =>
			{

			});
		}


		private async void CreateButton_Click(object sender, EventArgs e)
		{
			await Oy.Publish("PrefabTool:CreatePrefab", PrefabList.SelectedIndex);
		}

		private void FileContainer_SelectionChangeCommitted(object sender, EventArgs e)
		{
			UpdatePrefabList(FileContainer.SelectedIndex);
		}

		private void NewPrefab_Click(object sender, EventArgs e)
		{
			var name = NewPrefabName.Text.Trim();
			if (String.IsNullOrEmpty(name)) throw new Exception($"Prefab name cannot be empty.\r\nPrefab name: {name}");
			if (_activeDocument.TryGetTarget(out var mapDocument))
			{
				var selection = mapDocument.Selection;
				HammerTime.Formats.Prefab.WriteObjects(_activeWorldcraftPrefabLibrary, selection, name);

				_activeWorldcraftPrefabLibrary.WriteToFile(_files[FileContainer.SelectedIndex]);

				UpdatePrefabList(FileContainer.SelectedIndex);


			}
		}
		private void PrefabList_SelectedValueChanged(object sender, EventArgs e)
		{
			Oy.Publish("Context:Add", new ContextInfo("PrefabTool:PrefabIndex", PrefabList.SelectedIndex));

			var worldspawn = _activeWorldcraftPrefabLibrary.Prefabs[PrefabList.SelectedIndex].Map;
			if (!_activeDocument.TryGetTarget(out var mapDocument)) return;
			_previewDocument = new MapDocument(new Map(), mapDocument.Environment);
			_preview = HammerTime.Formats.Prefab.GetPrefab(worldspawn, _previewDocument.Map.NumberGenerator, _previewDocument.Map);
		}

		private void FileContainer_SelectedIndexChanged(object sender, EventArgs e)
		{
			Oy.Publish("Context:Add", new ContextInfo("PrefabTool:ActiveLibrary", _files[FileContainer.SelectedIndex]));
		}

		private void CreateLib_Click(object sender, EventArgs e)
		{
			var name = NewLibName.Text.Trim();
			if (String.IsNullOrEmpty(name)) throw new Exception($"Prefab name cannot be empty.\r\nPrefab name: {name}");
			var lib = new WorldcraftPrefabLibrary() { Description = name };
			lib.WriteToFile($"./prefabs/{name}.ol");
			InitPrefabLibraries();
		}
		private List<IPipeline> _pipelines = new List<IPipeline>();
		protected override void OnPaint(PaintEventArgs e)
		{
			if (_renderTarget == null) return;
			base.OnPaint(e);

			if (_previewDocument == null) return;
			var _commandList = Device.ResourceFactory.CreateCommandList();
			_commandList.Begin();
			_commandList.SetFramebuffer(_renderTarget.Swapchain.Framebuffer);
			_commandList.ClearDepthStencil(1);
			_commandList.ClearColorTarget(0, Veldrid.RgbaFloat.CornflowerBlue);

			var resourceCollector = new ResourceCollector();


			var builder = _engine.Value.CreateBufferBuilder(BufferSize.Small);
			var _scenebuilder = new SceneBuilder(_engine.Value);
			var renderable = new BufferBuilderRenderable(builder);

			foreach (var obj in _preview)
			{
				Convert(builder, _previewDocument, obj, resourceCollector);
			}

			builder.Complete();
			_scene.Add(renderable);

			foreach (var pipeline in _pipelines)
			{
				pipeline.SetupFrame(RenderContext, _renderTarget);
				pipeline.Render(RenderContext, _renderTarget, _commandList, _scene.GetRenderables(pipeline, _renderTarget));
			}


			_commandList.End();

			Device.SubmitCommands(_commandList);
			Device.SwapBuffers(_renderTarget.Swapchain);

			_scene.Remove(renderable);

		}
		public virtual IEnumerable<Tuple<Vector3, float, float>> GetTextureCoordinates(Sledge.Formats.Map.Objects.Face face, int width, int height)
		{
			if (width <= 0 || height <= 0 || face.XScale == 0 || face.YScale == 0)
			{
				return face.Vertices.Select(x => Tuple.Create(x, 0f, 0f));
			}

			var udiv = width * face.XScale;
			var uadd = face.XShift / width;
			var vdiv = height * face.YScale;
			var vadd = face.YShift / height;

			return face.Vertices.Select(x => Tuple.Create(x, Vector3.Dot(face.UAxis, x) / udiv + uadd, Vector3.Dot(face.VAxis, x) / vdiv + vadd));
		}
		private void Convert(BufferBuilder builder, MapDocument document, IMapObject obj, ResourceCollector resourceCollector)
		{
			if (obj is Primitives.MapObjects.Solid solid)
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

				var tc = document.Environment.GetTextureCollection().Result;

				var vi = 0u;
				var si = 0u;
				var wi = numSolidIndices;
				foreach (var face in faces)
				{
					var t = tc.GetTextureItem(face.Texture.Name).Result;
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
					var t = tc.GetTextureItem(f.Texture.Name).Result;
					var transparent = opacity < 0.95f || t?.Flags.HasFlag(TextureFlags.Transparent) == true;

					var texture = t == null ? string.Empty : $"{document.Environment.ID}::{f.Texture.Name}";

					groups.Add(transparent
						? new BufferGroup(PipelineType.TexturedAlpha, CameraType.Perspective, f.Origin, texture, texOffset, texInd)
						: new BufferGroup(PipelineType.TexturedOpaque, CameraType.Perspective, texture, texOffset, texInd)
					);

					texOffset += texInd;

					if (t != null) resourceCollector.RequireTexture(t.Name);
				}

				//groups.Add(new BufferGroup(PipelineType.Wireframe, solid.IsSelected ? CameraType.Both : CameraType.Orthographic, numSolidIndices, numWireframeIndices));

				builder.Append(points, indices, groups);
			}
			foreach (var child in obj.Hierarchy)
			{
				//await Convert(builder, document, child, resourceCollector);
				Convert(builder, document, child, resourceCollector);
			}
		}
	}
}
