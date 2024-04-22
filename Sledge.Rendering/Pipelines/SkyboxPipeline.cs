using SixLabors.ImageSharp.PixelFormats;
using Sledge.Rendering.Engine;
using Sledge.Rendering.Primitives;
using Sledge.Rendering.Renderables;
using Sledge.Rendering.Resources;
using Sledge.Rendering.Viewports;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using Veldrid;
using Veldrid.ImageSharp;
using Texture = Sledge.Rendering.Resources.Texture;

namespace Sledge.Rendering.Pipelines
{
	public class SkyboxPipeline : IPipeline
	{
		public PipelineType Type => PipelineType.Skybox;

		public PipelineGroup Group => PipelineGroup.Opaque;

		public float Order => 11;

		private Shader _vertex;
		private Shader _fragment;
		private Pipeline _pipeline;
		private DeviceBuffer _projectionBuffer;
		private ResourceSet _projectionResourceSet;



		private SixLabors.ImageSharp.Image<Rgba32> _right, _left, _top, _bottom, _back, _front;
		private Veldrid.Texture _texture;
		private TextureView _view;


		public void Bind(RenderContext context, CommandList cl, string binding)
		{
			var tex = context.ResourceLoader.GetTexture("sky");
			tex?.BindTo(cl, 1);
			cl.SetGraphicsResourceSet(0, _projectionResourceSet);
		}

		public void Create(RenderContext context)
		{
			var factory = context.Device.ResourceFactory;

			//CubeMap tx = new CubeMap(context, 256, 256, null, TextureSampleType.Point);
			//context.ResourceLoader.UploadTexture("sky", tx, TextureSampleType.Standard);


			//_right = SixLabors.ImageSharp.Image.Load<Rgba32>	("d:/env/officert.tga");
			//_left = SixLabors.ImageSharp.Image.Load<Rgba32>	("d:/env/officelf.tga");
			//_top = SixLabors.ImageSharp.Image.Load<Rgba32>	("d:/env/officeup.tga");
			//_bottom = SixLabors.ImageSharp.Image.Load<Rgba32>	("d:/env/officedn.tga");
			//_back = SixLabors.ImageSharp.Image.Load<Rgba32>	("d:/env/officebk.tga");
			//_front = SixLabors.ImageSharp.Image.Load<Rgba32>	("d:/env/officeft.tga");

			//ImageSharpCubemapTexture imageSharpCubemapTexture = new ImageSharpCubemapTexture(_right, _left, _top, _bottom, _back, _front, false);
			//ImageSharpCubemapTexture imageSharpCubemapTexture = new ImageSharpCubemapTexture(_back, _front, _right, _left, _top, _bottom,  false);





			ImageSharpCubemapTexture imageSharpCubemapTexture = new ImageSharpCubemapTexture("d:/env/officert.tga", "d:/env/officelf.tga", "d:/env/officeup.tga", "d:/env/officedn.tga", "d:/env/officebk.tga", "d:/env/officeft.tga");




			_texture = imageSharpCubemapTexture.CreateDeviceTexture(context.Device, factory);
			_texture.Name = "SkyTexture";

			TextureView textureView = factory.CreateTextureView(new TextureViewDescription(_texture));

			(_vertex, _fragment) = context.ResourceLoader.LoadShaders(Type.ToString());

			var pd = new GraphicsPipelineDescription
			{
				BlendState = BlendStateDescription.SingleAlphaBlend,
				DepthStencilState = DepthStencilStateDescription.DepthOnlyLessEqual,
				RasterizerState = RasterizerStateDescription.Default,

				PrimitiveTopology = PrimitiveTopology.TriangleList,
				ResourceLayouts = new ResourceLayout[] { context.ResourceLoader.ProjectionLayout, context.ResourceLoader.TextureLayout },
				ShaderSet = new ShaderSetDescription(new[] { context.ResourceLoader.VertexStandardLayoutDescription }, new[] { _vertex, _fragment }),
				Outputs = new OutputDescription
				{
					ColorAttachments = new[] { new OutputAttachmentDescription(PixelFormat.B8_G8_R8_A8_UNorm) },
					DepthAttachment = new OutputAttachmentDescription(PixelFormat.R32_Float),
					SampleCount = TextureSampleCount.Count1
				},
			};

			_pipeline = context.Device.ResourceFactory.CreateGraphicsPipeline(ref pd);

			_projectionBuffer = context.Device.ResourceFactory.CreateBuffer(
				new BufferDescription((uint)Unsafe.SizeOf<UniformProjection>(), BufferUsage.UniformBuffer)
			);

			_projectionResourceSet = context.Device.ResourceFactory.CreateResourceSet(
				new ResourceSetDescription(context.ResourceLoader.ProjectionLayout, _projectionBuffer)
			);

			_view = context.Device.ResourceFactory.CreateTextureView(_texture);

			var sampler = context.ResourceLoader.TextureSampler;

			var textureResourceSet = context.Device.ResourceFactory.CreateResourceSet(
				new ResourceSetDescription(context.ResourceLoader.TextureLayout, textureView, sampler)
				);
			var tx = new Texture(context, _texture, TextureSampleType.Standard, textureResourceSet);
			tx.MipGenerated();
			context.ResourceLoader.UploadTexture("sky", tx, TextureSampleType.Standard);

		}
		public void SetupFrame(RenderContext context, IViewport target)
		{
			context.Device.UpdateBuffer(_projectionBuffer, 0, new UniformProjection
			{
				Selective = context.SelectiveTransform,
				Model = Matrix4x4.Identity,
				View = target.Camera.View,
				Projection = target.Camera.Projection,
			});
		}
		public void Dispose()
		{
			_projectionResourceSet?.Dispose();
			_projectionBuffer?.Dispose();
			_pipeline?.Dispose();
			_vertex?.Dispose();
			_fragment?.Dispose();
		}

		public void Render(RenderContext context, IViewport target, CommandList cl, IEnumerable<IRenderable> renderables)
		{
			cl.SetPipeline(_pipeline);
			cl.SetGraphicsResourceSet(0, _projectionResourceSet);

			foreach (var r in renderables)
			{
				r.Render(context, this, target, cl);
			}
		}

		public void Render(RenderContext context, IViewport target, CommandList cl, IRenderable renderable, ILocation locationObject)
		{
			cl.SetPipeline(_pipeline);
			cl.SetGraphicsResourceSet(0, _projectionResourceSet);

			renderable.Render(context, this, target, cl, locationObject);
		}
	}
}
