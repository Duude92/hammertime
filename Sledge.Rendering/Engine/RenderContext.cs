using System;
using System.Numerics;
using Sledge.Rendering.Engine.Backends;
using Sledge.Rendering.Interfaces;
using Veldrid;

namespace Sledge.Rendering.Engine
{
	public class RenderContext : IUpdateable
	{
		public ResourceLoader ResourceLoader { get; }
		public GraphicsDevice Device { get; }
		public Matrix4x4 SelectiveTransform { get; set; } = Matrix4x4.Identity;
		internal IGraphicBackend GraphicBackend { get; }

		public RenderContext(GraphicsDevice device)
		{
			GraphicBackend = device.BackendType switch
			{
				GraphicsBackend.Vulkan => new VulkanBackend(this),
				GraphicsBackend.Direct3D11 => new D3d11Backend(this),
				//GraphicsBackend.Metal => new MetalBackend(this),
				//GraphicsBackend.OpenGL => new OpenGLBackend(this),
				//GraphicsBackend.OpenGLES => new OpenGLESBackend(this),
				_ => throw new NotSupportedException($"The graphics backend {device.BackendType} is not supported."),
			};
			Device = device;
			ResourceLoader = new ResourceLoader(this);
		}

		public void Update(long frame)
		{

		}
	}
}
