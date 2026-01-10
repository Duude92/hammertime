using Microsoft.Extensions.DependencyModel;
using Sledge.Common.Logging;
using Sledge.Rendering.Shaders;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using Veldrid;
using Veldrid.SPIRV;
using Vortice.Dxc;

namespace Sledge.Rendering.Engine.Backends
{
	internal class VulkanBackend : IGraphicBackend
	{
		private readonly RenderContext _context;
		public VulkanBackend(RenderContext context)
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

		public RasterizerStateDescription RasterizerStateDescription => new RasterizerStateDescription {
			CullMode = FaceCullMode.Front,
			FillMode = PolygonFillMode.Solid,
			FrontFace = FrontFace.Clockwise,
			DepthClipEnabled = true,
			ScissorTestEnabled = false
		};

		private byte[] CompileShadersToSpirV(byte[] shaderCode, DxcShaderStage stage)
		{
			var vShader = shaderCode;
			var str = System.Text.Encoding.Default.GetString(vShader);

			var options = new DxcCompilerOptions
			{
				GenerateSpirv = true,
				SpirvTargetEnvMinor = 0,
				SpvTargetEnvMajor = 1,
				OptimizationLevel = 0,
			};

			IDxcUtils Utils = Dxc.CreateDxcUtils();
			var includeHandler = Utils.CreateDefaultIncludeHandler();

			var result = DxcCompiler.Compile(stage, str, "main", options, includeHandler: includeHandler);
			if (result.GetStatus() != 0)
			{
				var errors = result.GetErrors();
				throw new Exception($"Error compiling shader\n{errors}");
			}
			var resultArray = result.GetObjectBytecodeArray();
			result.Dispose();
			return resultArray;
		}
		public (Shader, Shader) LoadShaders(string name)
		{
			var options = new CrossCompileOptions
			{
				FixClipSpaceZ = true,
				InvertVertexOutputY = false,
			};
			var vertex = _context.Device.ResourceFactory.CreateFromSpirv(new ShaderDescription(ShaderStages.Vertex, CompileShadersToSpirV(GetEmbeddedShader(name + ".vert.hlsl"), DxcShaderStage.Vertex), "main"), options);
			var fragment = _context.Device.ResourceFactory.CreateFromSpirv(new ShaderDescription(ShaderStages.Fragment, CompileShadersToSpirV(GetEmbeddedShader(name + ".frag.hlsl"), DxcShaderStage.Pixel), "main"), options);
			return (vertex, fragment);

		}

		public (Shader, Shader, Shader) LoadShadersGeometry(string name)
		{
			var options = new CrossCompileOptions
			{
				FixClipSpaceZ = true,
				InvertVertexOutputY = false,
			};
			var vertex = _context.Device.ResourceFactory.CreateFromSpirv(new ShaderDescription(ShaderStages.Vertex, CompileShadersToSpirV(GetEmbeddedShader(name + ".vert.hlsl"), DxcShaderStage.Vertex), "main"), options);
			var fragment = _context.Device.ResourceFactory.CreateFromSpirv(new ShaderDescription(ShaderStages.Fragment, CompileShadersToSpirV(GetEmbeddedShader(name + ".frag.hlsl"), DxcShaderStage.Pixel), "main"), options);
			var geom = _context.Device.ResourceFactory.CreateFromSpirv(new ShaderDescription(ShaderStages.Geometry, CompileShadersToSpirV(GetEmbeddedShader(name + ".geom.hlsl"), DxcShaderStage.Geometry), "main"), options);
			return (vertex, geom, fragment);
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
						return ms.ToArray();
					}
				}
			}
			throw new FileNotFoundException($"The `{name}` shader could not be found.", name);
		}

	}
}
