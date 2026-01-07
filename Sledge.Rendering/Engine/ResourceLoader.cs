using SixLabors.ImageSharp.PixelFormats;
using Sledge.Common.Logging;
using Sledge.Rendering.Resources;
using Sledge.Rendering.Shaders;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Veldrid;
using Veldrid.SPIRV;
using Vortice.Dxc;
using Texture = Sledge.Rendering.Resources.Texture;
namespace Sledge.Rendering.Engine
{
	public class ResourceLoader
	{
		private readonly RenderContext _context;
#if DEBUG
		private const bool DebugMode = true;
#else
		private const bool DebugMode = false;
#endif
		public ResourceLayout ProjectionLayout { get; }
		public ResourceLayout TextureLayout { get; }
		public Sampler TextureSampler { get; }
		public Sampler OverlaySampler { get; }

		public VertexLayoutDescription VertexStandardLayoutDescription { get; }
		public VertexLayoutDescription VertexModel3LayoutDescription { get; }

		private Lazy<Texture> MissingTexture { get; }

		public ResourceLoader(RenderContext context)
		{
			_context = context;

			ProjectionLayout = context.Device.ResourceFactory.CreateResourceLayout(
				new ResourceLayoutDescription(
					new ResourceLayoutElementDescription("Projection", ResourceKind.UniformBuffer, ShaderStages.Vertex | ShaderStages.Fragment | ShaderStages.Geometry)
				)
			);
			TextureLayout = context.Device.ResourceFactory.CreateResourceLayout(
				new ResourceLayoutDescription(
					new ResourceLayoutElementDescription("Texture", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
					new ResourceLayoutElementDescription("Sampler", ResourceKind.Sampler, ShaderStages.Fragment)
				)
			);

			VertexStandardLayoutDescription = new VertexLayoutDescription(
				new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3),
				new VertexElementDescription("Normal", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3),
				new VertexElementDescription("Colour", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float4),
				new VertexElementDescription("Texture", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2),
				new VertexElementDescription("Tint", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float4),
				new VertexElementDescription("Flags", VertexElementSemantic.TextureCoordinate, VertexElementFormat.UInt1)
			);

			VertexModel3LayoutDescription = new VertexLayoutDescription(
				new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3),
				new VertexElementDescription("Normal", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3),
				new VertexElementDescription("Texture", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2),
				new VertexElementDescription("Bone", VertexElementSemantic.TextureCoordinate, VertexElementFormat.UInt1),
				new VertexElementDescription("Flags", VertexElementSemantic.TextureCoordinate, VertexElementFormat.UInt1),
				new VertexElementDescription("TextureLayer", VertexElementSemantic.TextureCoordinate, VertexElementFormat.UInt1)
			);

			TextureSampler = context.Device.Aniso4xSampler;
			OverlaySampler = context.Device.PointSampler;

			MissingTexture = new Lazy<Texture>(() => UploadTexture("", 1, 1, new byte[] { 255, 255, 255, 255 }, TextureSampleType.Standard));
		}

		// Textures
		private readonly ConcurrentDictionary<string, Texture> _textures = new ConcurrentDictionary<string, Texture>(StringComparer.InvariantCultureIgnoreCase);

		internal Texture UploadTexture(string name, int width, int height, byte[] data, TextureSampleType sampleType, int frameNum)
		{
			return _textures.GetOrAdd(name, n => new Texture(_context, width, height, data, sampleType, frameNum));
		}
		internal Texture UploadTexture(string name, int width, int height, byte[] data, TextureSampleType sampleType)
		{
			return _textures.GetOrAdd(name, n => new Texture(_context, width, height, data, sampleType));
		}
		internal Texture UploadTexture(string name, int width, int height, byte[][] data, TextureSampleType sampleType, uint layerCount)
		{
			return _textures.GetOrAdd(name, n => new Texture(_context, width, height, data, sampleType, layerCount));
		}
		internal Texture UploadTexture(string name, Texture texture, TextureSampleType sampleType)
		{
			return _textures.GetOrAdd(name, texture);
		}
		internal Texture UploadCubemap(string name, IEnumerable<SixLabors.ImageSharp.Image<Rgba32>> images, TextureSampleType sampleType)
		{
			return _textures.GetOrAdd(name, n => new CubeMap(_context, images, sampleType));
		}

		internal void DestroyTexture(Texture texture)
		{
			var keys = _textures.Where(x => x.Value == texture).ToList();
			if (keys.Any()) _textures.TryRemove(keys[0].Key, out _);
			texture.Dispose();
		}

		internal Texture GetTexture(string name)
		{
			return _textures.TryGetValue(name, out var tex) ? tex : MissingTexture.Value;
		}

		// Shaders
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

			return result.GetObjectBytecodeArray();
		}
		public (Shader, Shader) LoadShaders(string name)
		{
			if (_context.Device.BackendType == GraphicsBackend.Direct3D11)
			{
				return (
					_context.Device.ResourceFactory.CreateShader(new ShaderDescription(ShaderStages.Vertex, GetEmbeddedShader(name + ".vert.hlsl"), "main", DebugMode)),
					_context.Device.ResourceFactory.CreateShader(new ShaderDescription(ShaderStages.Fragment, GetEmbeddedShader(name + ".frag.hlsl"), "main", DebugMode))
				);
			}


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
			if (_context.Device.BackendType == GraphicsBackend.Direct3D11)
			{
				return (
				_context.Device.ResourceFactory.CreateShader(new ShaderDescription(ShaderStages.Vertex, GetEmbeddedShader(name + ".vert.hlsl"), "main")),
				_context.Device.ResourceFactory.CreateShader(new ShaderDescription(ShaderStages.Geometry, GetEmbeddedShader(name + ".geom.hlsl"), "main")),
				_context.Device.ResourceFactory.CreateShader(new ShaderDescription(ShaderStages.Fragment, GetEmbeddedShader(name + ".frag.hlsl"), "main"))
				);
			}
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