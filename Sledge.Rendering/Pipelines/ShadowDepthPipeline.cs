using Sledge.Rendering.Engine;
using Sledge.Rendering.Renderables;
using Sledge.Rendering.Viewports;
using System;
using System.Collections.Generic;
using System.Linq;
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
		public void Bind(RenderContext context, CommandList cl, string binding)
		{
			var tex = context.ResourceLoader.GetTexture(binding);
			tex?.BindTo(cl, 1);
			cl.SetGraphicsResourceSet(0, _projectionResourceSet); ;
		}

		public void Create(RenderContext context)
		{
			var gd = context.Device;

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
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}

		public void Render(RenderContext context, IViewport target, CommandList cl, IEnumerable<IRenderable> renderables)
		{
			throw new NotImplementedException();
		}

		public void Render(RenderContext context, IViewport target, CommandList cl, IRenderable renderable, ILocation locationObject)
		{
			throw new NotImplementedException();
		}

		public void SetupFrame(RenderContext context, IViewport target)
		{
			throw new NotImplementedException();
		}
	}
}
