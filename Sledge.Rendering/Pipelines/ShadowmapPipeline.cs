using Sledge.Rendering.Engine;
using Sledge.Rendering.Renderables;
using Sledge.Rendering.Viewports;
using System;
using System.Collections.Generic;
using Veldrid;

namespace Sledge.Rendering.Pipelines
{
	public class ShadowmapPipeline : IPipeline
	{
		public PipelineType Type => PipelineType.Shadowmap;

		public PipelineGroup Group => PipelineGroup.Opaque;

		public float Order => 20;

		private Shader _vertex;
		private Shader _fragment;
		private Pipeline _pipeline;
		private ResourceSet _shadowBlendResourceSet;
		private DeviceBuffer _deviceBuffer;
		private DeviceBuffer _orthographicBuffer;
		private DeviceBuffer _projectionBuffer;
		private ResourceSet _projectionResourceSet;

		private ResourceSet _mainProjViewRS;
		private Sampler _shadowSampler;
		private ResourceSet _mainSharedRS;
		private ResourceSet _mainPerObjectRS;
		private readonly Func<Resources.Texture> _shadowmapGetter;
		private readonly Func<TextureView> _viewGetter;

		public ShadowmapPipeline(Func<Resources.Texture> bindingGetter, Func<TextureView> viewGetter)
		{
			_shadowmapGetter = bindingGetter;
			_viewGetter = viewGetter;
		}
		public void Bind(RenderContext context, CommandList cl, string binding)
		{
			//var tex = context.ResourceLoader.GetTexture(binding);
			//tex?.BindTo(cl, 1);
			//cl.SetGraphicsResourceSet(0, _projectionResourceSet);
		}

		public void Create(RenderContext context, TextureSampleCount sampleCount)
		{
			var gd = context.Device;
			var factory = gd.ResourceFactory;

			(_vertex, _fragment) = context.ResourceLoader.LoadShaders(Type.ToString());



			var ResourceLayouts = new ResourceLayout[] { gd.ResourceFactory.CreateResourceLayout(new ResourceLayoutDescription(
				new ResourceLayoutElementDescription("ShadowMap", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
				new ResourceLayoutElementDescription("ShadowSampler", ResourceKind.Sampler, ShaderStages.Fragment)
				))};

			GraphicsPipelineDescription shadowBlendPipelineDesc = new()
			{
				BlendState = BlendStateDescription.SingleDisabled,

				//BlendState = BlendStateDescription.SingleOverrideBlend, // Blend with existing color
				DepthStencilState = DepthStencilStateDescription.DepthOnlyLessEqual,

				//DepthStencilState = DepthStencilStateDescription.Disabled,
				RasterizerState = RasterizerStateDescription.Default,
				PrimitiveTopology = PrimitiveTopology.TriangleList, // For fullscreen quad
				ResourceLayouts =  ResourceLayouts ,
				ShaderSet = new ShaderSetDescription(
		vertexLayouts: Array.Empty<VertexLayoutDescription>(), // No vertex input needed
					shaders: new[] { _vertex, _fragment }),
				Outputs = new OutputDescription
				{
					ColorAttachments = new[] { new OutputAttachmentDescription(PixelFormat.B8_G8_R8_A8_UNorm) },
					DepthAttachment = new OutputAttachmentDescription(PixelFormat.R32_Float),
					SampleCount = TextureSampleCount.Count1
				},
			};

			_pipeline = factory.CreateGraphicsPipeline(ref shadowBlendPipelineDesc);


			_shadowBlendResourceSet = factory.CreateResourceSet(new ResourceSetDescription(
				ResourceLayouts[0],
				_viewGetter(), // This should be your shadow depth texture
				gd.PointSampler // A sampler, like linear or nearest filtering
			));

	//		_deviceBuffer = factory.CreateBuffer(new BufferDescription(
	//3 * 2 * sizeof(float), // 3 vertices, 2 floats (XY) per vertex
	//BufferUsage.VertexBuffer
	//		));




			//			var applyShadowPipelineDesc = new GraphicsPipelineDescription
			//			{
			//				BlendState = BlendStateDescription.SingleOverrideBlend,
			//				DepthStencilState = DepthStencilStateDescription.Disabled,
			//				RasterizerState = RasterizerStateDescription.Default,
			//				PrimitiveTopology = PrimitiveTopology.TriangleList,
			//				ResourceLayouts = new ResourceLayout[] { context.ResourceLoader.ProjectionLayout, context.ResourceLoader.TextureLayout },
			//				ShaderSet = new ShaderSetDescription(
			//		vertexLayouts: new VertexLayoutDescription[] { context.ResourceLoader.VertexStandardLayoutDescription },
			//		shaders: new[] { _vertex, _fragment }),
			//				Outputs = new OutputDescription
			//				{
			//					ColorAttachments = new[] { new OutputAttachmentDescription(PixelFormat.B8_G8_R8_A8_UNorm) },
			//					DepthAttachment = new OutputAttachmentDescription(PixelFormat.R32_Float),
			//					SampleCount = TextureSampleCount.Count1
			//				},

			//				ResourceBindingModel = ResourceBindingModel.Improved
			//			};
			//			_pipeline = factory.CreateGraphicsPipeline(ref applyShadowPipelineDesc);

			//			_orthographicBuffer = factory.CreateBuffer(new BufferDescription((uint)Unsafe.SizeOf<UniformProjection>(), BufferUsage.UniformBuffer));



			//			_mainProjViewRS = gd.ResourceFactory.CreateResourceSet(new ResourceSetDescription(
			//				context.ResourceLoader.ProjectionLayout,
			//				_orthographicBuffer
			//				/*_viewGetter(),
			//				gd.PointSampler*/));

			//			var sampler = context.ResourceLoader.TextureSampler;

			//			_mainPerObjectRS = gd.ResourceFactory.CreateResourceSet(new ResourceSetDescription(
			//	context.ResourceLoader.TextureLayout, _viewGetter(), sampler
			//));

			//			_shadowSampler = factory.CreateSampler(new SamplerDescription(
			//				SamplerAddressMode.Clamp,
			//				SamplerAddressMode.Clamp,
			//				SamplerAddressMode.Clamp,
			//				/*SamplerFilter.ComparisonMinLinear_MagLinear_MipLinear*/
			//				SamplerFilter.MinLinear_MagLinear_MipLinear,
			//				ComparisonKind.LessEqual,
			//				0,
			//				0,
			//				0,
			//				0,
			//				SamplerBorderColor.OpaqueBlack));


			//			_projectionBuffer = context.Device.ResourceFactory.CreateBuffer(
			//	new BufferDescription((uint)Unsafe.SizeOf<UniformProjection>(), BufferUsage.UniformBuffer)
			//);
			//			_projectionBuffer.Name = "projection buffer";

			//			_projectionResourceSet = context.Device.ResourceFactory.CreateResourceSet(
			//				new ResourceSetDescription(context.ResourceLoader.ProjectionLayout, _projectionBuffer)
			//			);
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}

		public void Render(RenderContext context, IViewport target, CommandList cl, IEnumerable<IRenderable> renderables)
		{

			cl.SetPipeline(_pipeline);

			//if (target.Camera is OrthographicCamera) return;
			cl.PushDebugGroup("Shadow Map - Apply shadow");

			cl.SetGraphicsResourceSet(0, _shadowBlendResourceSet);
			_shadowmapGetter().BindTo(cl, 0);
			//cl.SetGraphicsResourceSet(1, _mainPerObjectRS);
			//var count = renderables.Count();
			//foreach (var r in renderables)
			//{

			//	r.Render(context, this, target, cl);
			//}
			cl.Draw(3, 1, 0, 0); // Draw a full-screen triangle with 3 vertices

			cl.PopDebugGroup();
		}

		public void Render(RenderContext context, IViewport target, CommandList cl, IRenderable renderable, ILocation locationObject)
		{
			return;
			throw new NotImplementedException();
		}

		public void SetupFrame(RenderContext context, IViewport target)
		{
			//context.Device.UpdateBuffer(_deviceBuffer, 0, new float[]
			//{
			//	-1f, -1f, // Bottom-left
			//	3f, -1f, // Bottom-right (oversized for fullscreen triangle trick)
			//	-1f,  3f  // Top-left (oversized)
			//});

			//context.Device.UpdateBuffer(_projectionBuffer, 0, new UniformProjection
			//{
			//	Selective = context.SelectiveTransform,
			//	Model = Matrix4x4.Identity,
			//	View = target.Camera.View,
			//	Projection = target.Camera.Projection,
			//});
		}
	}
}
