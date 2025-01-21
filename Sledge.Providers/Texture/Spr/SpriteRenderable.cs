using Sledge.Rendering.Engine;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.Pipelines;
using Sledge.Rendering.Renderables;
using Sledge.Rendering.Viewports;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Primitives;
using Sledge.Rendering.Resources;
using Veldrid;
using Buffer = Sledge.Rendering.Resources.Buffer;

namespace Sledge.Providers.Texture.Spr
{
	public class SpriteRenderable : IModelRenderable
	{
		public IModel Model => null;
		private readonly Rendering.Resources.Texture _texture;
		private ResourceLayout _uvLayout;
		private DeviceBuffer _uvBuffer;
		private ResourceSet _uvProjectionSet;
		private readonly TextureItem _textureItem;
		private Buffer _buffer;
		private double _interframePercent;
		public Vector3 Origin
		{
			get => _location.Location;
			set => _location.Location = value;
		}
		public Vector3 Angles { get; set; }
		public int Sequence { get; set; }
		public int Framerate { get; set; } = 10;
		public float Scale { get; set; } = 1;
		public Vector4 Tint { get; set; } = Vector4.One;

		private SpriteLocation _location = new SpriteLocation();
		private long _lastFrameTime;

		public SpriteRenderable(Rendering.Resources.Texture texture, TextureItem item)
		{
			_texture = texture;
			_textureItem = item;
		}

		public void CreateResources(EngineInterface engine, RenderContext context)
		{
			_uvLayout = context.Device.ResourceFactory.CreateResourceLayout(new ResourceLayoutDescription(
				new ResourceLayoutElementDescription("UVS", ResourceKind.UniformBuffer, ShaderStages.Geometry)));
			_uvBuffer = context.Device.ResourceFactory.CreateBuffer(
				new BufferDescription((uint)Unsafe.SizeOf<BillboardUV>(), BufferUsage.UniformBuffer)
			);

			_uvProjectionSet = context.Device.ResourceFactory.CreateResourceSet(
				new ResourceSetDescription(_uvLayout, _uvBuffer)
			);
			context.Device.UpdateBuffer(_uvBuffer, 0, new BillboardUV
			{
				FrameCount = 1,
				CurrentFrame = 0,
				UniformPadding = Vector2.Zero
			});
			_buffer = Engine.Interface.CreateBuffer();
		}

		public void DestroyResources()
		{
			_uvProjectionSet?.Dispose();
			_uvBuffer = null;
		}

		public void Dispose()
		{
			//
		}

		public (Vector3, Vector3) GetBoundingBox()
		{
			throw new NotImplementedException();
		}

		public IEnumerable<ILocation> GetLocationObjects(IPipeline pipeline, IViewport viewport)
		{
			yield return _location;
		}

		public Matrix4x4 GetModelTransformation()
		{
			throw new NotImplementedException();
		}

		public void Render(RenderContext context, IPipeline pipeline, IViewport viewport, CommandList cl)
		{
			if (_uvBuffer == null) return;
			var frameCount = _texture == null ? 1 : (float)_texture.FrameCount;

			var uv = new BillboardUV
			{
				FrameCount = frameCount,
				CurrentFrame = (float)(Sequence % frameCount),
			};

			cl.UpdateBuffer(_uvBuffer, 0, uv);
			cl.SetGraphicsResourceSet(1, _uvProjectionSet);

			_buffer.Update(
				new[]
				{
					new VertexStandard
					{
						Position = Origin, Normal = new Vector3(_textureItem.Width * Scale, _textureItem.Height * Scale, 0),
						Colour = Vector4.One, Tint = Tint, Flags = VertexFlags.None
					}
				},
				new[] { 0u }
			);
			_buffer.Bind(cl, 0);


			_texture.BindTo(cl, 2);

			cl.DrawIndexed((uint)_buffer.IndexCount, 1, 0, 0, 0);
		}

		public void Render(RenderContext context, IPipeline pipeline, IViewport viewport, CommandList cl,
			ILocation locationObject)
		{
			Render(context, pipeline, viewport, cl);
			return;
			var frameCount = _texture == null ? 1 : (float)_texture.FrameCount;

			context.Device.UpdateBuffer(_uvBuffer, 0, new BillboardUV
			{
				FrameCount = frameCount,
				CurrentFrame = (float)(Sequence % frameCount),
			});
			cl.SetGraphicsResourceSet(1, _uvProjectionSet);
		}

		public bool ShouldRender(IPipeline pipeline, IViewport viewport)
		{
			if (pipeline.Type == PipelineType.BillboardAlpha)
			{
				return viewport.Camera.Type == CameraType.Perspective;
			}

			return false;
		}

		public void Update(long frame)
		{
			double targetFps = 1000 / Framerate;
			double diff = frame - _lastFrameTime;
			_interframePercent += diff / targetFps;

			var skip = (int)_interframePercent;
			_interframePercent -= skip;

			Sequence = (Sequence + skip) % _texture.FrameCount;

			_lastFrameTime = frame;
		}
		public class SpriteLocation : ILocation
		{
			public Vector3 Location { get; set; }
		}
	}
}