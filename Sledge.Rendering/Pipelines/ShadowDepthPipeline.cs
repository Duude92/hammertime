using Sledge.Rendering.Engine;
using Sledge.Rendering.Primitives;
using Sledge.Rendering.Renderables;
using Sledge.Rendering.Viewports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Veldrid;

namespace Sledge.Rendering.Pipelines
{
	public class ShadowDepthPipeline : IPipeline
	{
		public PipelineType Type => PipelineType.ShadowDepth;

		public PipelineGroup Group => PipelineGroup.Opaque;

		public float Order => 10;

		private Shader _vertex;
		private Shader _fragment;
		private Pipeline _pipeline;
		private DeviceBuffer _projectionBuffer;
		private ResourceSet _projectionResourceSet;
		public Resources.Texture NearShadowResourceTexture { get; private set; }
		public Texture NearShadowMap { get; private set; }
		public TextureView NearShadowMapView { get; private set; }
		public Framebuffer NearShadowMapFramebuffer { get; private set; }

		private DeviceBuffer _worldAndInverseBuffer;
		private uint _uniformOffset = 0;

		public void Bind(RenderContext context, CommandList cl, string binding)
		{
			//var tex = context.ResourceLoader.GetTexture(binding);
			//tex?.BindTo(cl, 1);
			NearShadowResourceTexture?.BindTo(cl,1);
			cl.SetGraphicsResourceSet(0, _projectionResourceSet);
		}

		public void Create(RenderContext context)
		{
			var gd = context.Device;
			_uniformOffset = gd.UniformBufferMinOffsetAlignment;

			var factory = gd.ResourceFactory;
			TextureDescription desc = TextureDescription.Texture2D(2048, 2048, 1, 1, PixelFormat.D32_Float_S8_UInt, TextureUsage.DepthStencil | TextureUsage.Sampled);
			NearShadowMap = factory.CreateTexture(desc);
			NearShadowMap.Name = "Near Shadow Map";
			NearShadowMapView = factory.CreateTextureView(NearShadowMap);
			NearShadowMapFramebuffer = factory.CreateFramebuffer(new FramebufferDescription(
				new FramebufferAttachmentDescription(NearShadowMap, 0), Array.Empty<FramebufferAttachmentDescription>()));


			VertexLayoutDescription[] shadowDepthVertexLayouts = new VertexLayoutDescription[]
			{
				new VertexLayoutDescription(
					new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3),
					new VertexElementDescription("Normal", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3),
					new VertexElementDescription("TexCoord", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2))
			};

			(_vertex, _fragment) = context.ResourceLoader.LoadShaders(Type.ToString());

			ResourceLayout projViewCombinedLayout = context.Device.ResourceFactory.CreateResourceLayout(
				new ResourceLayoutDescription(
					new ResourceLayoutElementDescription("Projection", ResourceKind.UniformBuffer, ShaderStages.Vertex)
				)
			);

			ResourceLayout worldLayout = context.Device.ResourceFactory.CreateResourceLayout(new ResourceLayoutDescription(
				new ResourceLayoutElementDescription("WorldAndInverse", ResourceKind.UniformBuffer, ShaderStages.Vertex, ResourceLayoutElementOptions.DynamicBinding)));



			_projectionResourceSet = factory.CreateResourceSet(new ResourceSetDescription(
				worldLayout,
				new DeviceBufferRange(_worldAndInverseBuffer, _uniformOffset, 128)));



			GraphicsPipelineDescription depthPD = new GraphicsPipelineDescription(
				BlendStateDescription.Empty,
				gd.IsDepthRangeZeroToOne ? DepthStencilStateDescription.DepthOnlyGreaterEqual : DepthStencilStateDescription.DepthOnlyLessEqual,
				RasterizerStateDescription.Default,
				PrimitiveTopology.TriangleList,
				new ShaderSetDescription(
					shadowDepthVertexLayouts,
					new[] { _vertex, _fragment },
					new[] { new SpecializationConstant(100, gd.IsClipSpaceYInverted) }),
				new ResourceLayout[] { projViewCombinedLayout, worldLayout },
				new OutputDescription
				{
					ColorAttachments = new[] { new OutputAttachmentDescription(PixelFormat.B8_G8_R8_A8_UNorm) },
					DepthAttachment = new OutputAttachmentDescription(PixelFormat.R32_Float),
					SampleCount = TextureSampleCount.Count1
				});


			NearShadowResourceTexture = new Resources.Texture(context, NearShadowMap, Resources.TextureSampleType.Standard, _projectionResourceSet);

		}

		public void Dispose()
		{
			_vertex.Dispose();
			_fragment.Dispose();
			_pipeline.Dispose();
			_projectionBuffer.Dispose();
			_projectionResourceSet.Dispose();
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
	}
}
