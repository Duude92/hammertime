using SixLabors.ImageSharp.PixelFormats;
using Sledge.Common.Logging;
using Sledge.Rendering.Engine.Backends;
using Sledge.Rendering.Resources;
using Sledge.Rendering.Shaders;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Veldrid;
using Veldrid.SPIRV;
using Vortice.Dxc;
using Texture = Sledge.Rendering.Resources.Texture;
namespace Sledge.Rendering.Engine
{
	public class ResourceLoader
	{
		private readonly RenderContext _context;
		public ResourceLayout ProjectionLayout { get; }
		public ResourceLayout TextureLayout { get; }
		public Sampler TextureSampler { get; }
		public Sampler OverlaySampler { get; }

		public VertexLayoutDescription VertexStandardLayoutDescription { get => _backend.VertexStandardLayoutDescription; }
		public VertexLayoutDescription VertexModel3LayoutDescription { get => _backend.VertexModel3LayoutDescription; }
		public VertexLayoutDescription ImGUILayoutDescription { get => _backend.ImGUILayoutDescription; }


		private Lazy<Texture> MissingTexture { get; }
		private IGraphicBackend _backend;

		public ResourceLoader(RenderContext context)
		{
			_context = context;
			_backend = context.GraphicBackend;

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
		public (Shader, Shader) LoadShaders(string name) => _backend.LoadShaders(name);

		public (Shader, Shader, Shader) LoadShadersGeometry(string name) => _backend.LoadShadersGeometry(name);

	}
}