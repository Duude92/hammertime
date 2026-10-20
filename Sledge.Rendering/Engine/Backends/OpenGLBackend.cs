using Sledge.Common.Logging;
using Sledge.Common.Shell.Context;
using Sledge.Rendering.Shaders;
using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Veldrid;
using Veldrid.SPIRV;
using Vortice.Dxc;

namespace Sledge.Rendering.Engine.Backends
{
	internal class OpenGLBackend : IGraphicBackend
	{
		private readonly RenderContext _context;
		public OpenGLBackend(RenderContext context)
		{
			_context = context;
		}
		public VertexLayoutDescription VertexStandardLayoutDescription { get; } = new VertexLayoutDescription(
				new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3),
				new VertexElementDescription("Normal", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3),
				new VertexElementDescription("Colour", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float4),
				new VertexElementDescription("Texture", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2),
				new VertexElementDescription("Tint", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float4),
				new VertexElementDescription("Flags", VertexElementSemantic.TextureCoordinate, VertexElementFormat.UInt1)
			);

		public VertexLayoutDescription VertexModel3LayoutDescription { get; } = new VertexLayoutDescription(
				new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3),
				new VertexElementDescription("Normal", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3),
				new VertexElementDescription("Texture", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2),
				new VertexElementDescription("Bone", VertexElementSemantic.TextureCoordinate, VertexElementFormat.UInt1),
				new VertexElementDescription("Flags", VertexElementSemantic.TextureCoordinate, VertexElementFormat.UInt1),
				new VertexElementDescription("TextureLayer", VertexElementSemantic.TextureCoordinate, VertexElementFormat.UInt1)
			);

		public VertexLayoutDescription ImGUILayoutDescription => new VertexLayoutDescription(
				new VertexElementDescription("in_position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2),
				new VertexElementDescription("in_texCoord", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2),
				new VertexElementDescription("in_color", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Byte4_Norm));

		public RasterizerStateDescription RasterizerStateDescription => new RasterizerStateDescription
		{
			CullMode = FaceCullMode.Front,
			FillMode = PolygonFillMode.Solid,
			FrontFace = FrontFace.CounterClockwise,
			DepthClipEnabled = true,
			ScissorTestEnabled = false
		};

		private byte[] GetShader(string name)
		{
			using (var s = ResourceAssembly.GetManifestResourceStream(typeof(Scope), name))
			{
				if (s != null)
				{
					using (var ms = new MemoryStream())
					{
						s.CopyTo(ms);
						return ms.ToArray();
					}
				}
			}
			return null;
		}
		public (Shader, Shader) LoadShaders(string name)
		{
			var options = new CrossCompileOptions
			{
				FixClipSpaceZ = true,
				InvertVertexOutputY = false,
			};
			var vCode = GetShader(name + ".vert.glsl");
			var fCode = GetShader(name + ".frag.glsl");

			var vertex = _context.Device.ResourceFactory.CreateShader(new ShaderDescription(ShaderStages.Vertex, vCode, "main"));
			var fragment = _context.Device.ResourceFactory.CreateShader(new ShaderDescription(ShaderStages.Fragment, fCode, "main"));
			return (vertex, fragment);
		}

		public (Shader, Shader, Shader) LoadShadersGeometry(string name)
		{
			var options = new CrossCompileOptions
			{
				FixClipSpaceZ = true,
				InvertVertexOutputY = false,
			};

			var vCode = GetShader(name + ".vert.glsl");
			var fCode = GetShader(name + ".frag.glsl");
			var gCode = GetShader(name + ".geom.glsl");


			var vertex = _context.Device.ResourceFactory.CreateShader(new ShaderDescription(ShaderStages.Vertex, vCode, "main"));
			var fragment = _context.Device.ResourceFactory.CreateShader(new ShaderDescription(ShaderStages.Fragment, fCode, "main"));
			var geom = _context.Device.ResourceFactory.CreateShader(new ShaderDescription(ShaderStages.Geometry, gCode, "main"));

			return (vertex, geom, fragment);
		}
		private static readonly Assembly ResourceAssembly = Assembly.GetExecutingAssembly();
		private OpenglSwapchainAdapter _swapchain;

		public Swapchain CreateSwapchain(Control control, GraphicsDeviceOptions options)
		{
			_swapchain = new OpenglSwapchainAdapter(_context.Device);
			return _swapchain;
		}

		public void SetScissors(CommandList cl, Viewport vp)
		{
			vp.Y = _swapchain.Framebuffer.Height - (vp.Y + vp.Height);
			cl.SetScissorRect(0, (uint)vp.X, (uint)vp.Y, (uint)vp.Width, (uint)vp.Height);
		}
	}
	internal class OpenglSwapchainAdapter : Swapchain
	{
		private GraphicsDevice _device;
		private Framebuffer _framebuffer;
		public override Framebuffer Framebuffer => _framebuffer;

		public override bool SyncToVerticalBlank { get; set; }
		public override string Name { get; set; }

		public override bool IsDisposed => false;
		public OpenglSwapchainAdapter(GraphicsDevice device)
		{
			_device = device;
			_framebuffer = device.SwapchainFramebuffer;
		}

		public override void Dispose()
		{
			_framebuffer?.Dispose();
		}

		public override void Resize(uint width, uint height)
		{
			_device.ResizeMainWindow(width, height);
		}
	}
}
