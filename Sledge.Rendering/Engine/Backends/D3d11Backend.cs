using Sledge.Common.Logging;
using Sledge.Common.Shell.Context;
using Sledge.Rendering.Shaders;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Veldrid;
using Veldrid.SPIRV;
using Vortice.Dxc;
using static System.Windows.Forms.Design.AxImporter;

namespace Sledge.Rendering.Engine.Backends
{
	internal class D3d11Backend : IGraphicBackend
	{
#if DEBUG
		private const bool DebugMode = true;
#else
		private const bool DebugMode = false;
#endif
		private readonly RenderContext _context;
		public D3d11Backend(RenderContext context)
		{
			_context = context;
		}

		public VertexLayoutDescription VertexStandardLayoutDescription =>
			new VertexLayoutDescription(
				new VertexElementDescription("Position", VertexElementSemantic.Position, VertexElementFormat.Float3),
				new VertexElementDescription("Normal", VertexElementSemantic.Normal, VertexElementFormat.Float3),
				new VertexElementDescription("Colour", VertexElementSemantic.Color, VertexElementFormat.Float4),
				new VertexElementDescription("Texture", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2),
				new VertexElementDescription("Tint", VertexElementSemantic.Color, VertexElementFormat.Float4),
				new VertexElementDescription("Flags", VertexElementSemantic.Position, VertexElementFormat.UInt1)
			);
		public VertexLayoutDescription VertexModel3LayoutDescription =>
			 new VertexLayoutDescription(
				new VertexElementDescription("Position", VertexElementSemantic.Position, VertexElementFormat.Float3),
				new VertexElementDescription("Normal", VertexElementSemantic.Normal, VertexElementFormat.Float3),
				new VertexElementDescription("Texture", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2),
				new VertexElementDescription("Bone", VertexElementSemantic.Color, VertexElementFormat.UInt1),
				new VertexElementDescription("Flags", VertexElementSemantic.TextureCoordinate, VertexElementFormat.UInt1),
				new VertexElementDescription("TextureLayer", VertexElementSemantic.Color, VertexElementFormat.UInt1)
			);

		public VertexLayoutDescription ImGUILayoutDescription => new VertexLayoutDescription(
				new VertexElementDescription("in_position", VertexElementSemantic.Position, VertexElementFormat.Float2),
				new VertexElementDescription("in_texCoord", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2),
				new VertexElementDescription("in_color", VertexElementSemantic.Color, VertexElementFormat.Byte4_Norm));

		public RasterizerStateDescription RasterizerStateDescription => RasterizerStateDescription.Default;

		public (Shader, Shader) LoadShaders(string name)
		{
			return (
				_context.Device.ResourceFactory.CreateShader(new ShaderDescription(ShaderStages.Vertex, GetEmbeddedShader(name + ".vert.hlsl"), "main", DebugMode)),
				_context.Device.ResourceFactory.CreateShader(new ShaderDescription(ShaderStages.Fragment, GetEmbeddedShader(name + ".frag.hlsl"), "main", DebugMode))
			);
		}

		public (Shader, Shader, Shader) LoadShadersGeometry(string name)
		{
			return (
			_context.Device.ResourceFactory.CreateShader(new ShaderDescription(ShaderStages.Vertex, GetEmbeddedShader(name + ".vert.hlsl"), "main")),
			_context.Device.ResourceFactory.CreateShader(new ShaderDescription(ShaderStages.Geometry, GetEmbeddedShader(name + ".geom.hlsl"), "main")),
			_context.Device.ResourceFactory.CreateShader(new ShaderDescription(ShaderStages.Fragment, GetEmbeddedShader(name + ".frag.hlsl"), "main"))
			);
		}

		private static readonly Assembly ResourceAssembly = Assembly.GetExecutingAssembly();
		private static byte[] GetEmbeddedShader(string name)
		{
			var names = new[] { name + ".bytes", name };
#if DEBUG
			// Compiling shaders manually is a pain!
			if (!Features.DirectX11OrHigher) Log.Debug("ResourceLoader", "If you're debugging on DX10 you'll need to manually compile shaders.");
			else names = new[] { name };
#endif
			foreach (var n in names)
			{
				using (var s = ResourceAssembly.GetManifestResourceStream(typeof(Scope), n))
				{
					if (s == null) continue;
					using (var ms = new MemoryStream())
					{
						s.CopyTo(ms);
						return Encoding.UTF8.GetBytes(Regex.Replace(Encoding.UTF8.GetString(ms.ToArray()), @"\:\s+register\(.\d+, space\d+\)", ""));
					}
				}
			}
			throw new FileNotFoundException($"The `{name}` shader could not be found.", name);
		}
		public Swapchain CreateSwapchain(Control control, GraphicsDeviceOptions options)
		{
			IntPtr HInstance = Process.GetCurrentProcess().Handle;
			var source = SwapchainSource.CreateWin32(control.Handle, HInstance);
			uint w = (uint)control.Width, h = (uint)control.Height;
			if (w <= 0) w = 1;
			if (h <= 0) h = 1;
			var desc = new SwapchainDescription(source, w, h, options.SwapchainDepthFormat, options.SyncToVerticalBlank);

			return _context.Device.ResourceFactory.CreateSwapchain(desc);
		}

		public void SetScissors(CommandList cl, Viewport vp)
		{
			cl.SetScissorRect(0, (uint)vp.X, (uint)vp.Y, (uint)vp.Width, (uint)vp.Height);
		}
	}
}
