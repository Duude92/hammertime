using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using Sledge.Rendering.Engine;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.Primitives;
using Sledge.Rendering.Renderables;
using Sledge.Rendering.Viewports;
using Veldrid;

namespace Sledge.Rendering.Pipelines
{
	public class BillboardAlphaPipeline : IPipeline
	{
		public PipelineType Type => PipelineType.BillboardAlpha;
		public PipelineGroup Group => PipelineGroup.Transparent;
		public float Order => 6;

		private Shader _vertex;
		private Shader _geometry;
		private Shader _fragment;
		private Pipeline _pipeline;
		private DeviceBuffer _projectionBuffer;
		private ResourceSet _projectionResourceSet;
		private ResourceLayout _uvLayout;

		public void Create(RenderContext context, TextureSampleCount sampleCount)
		{
			(_vertex, _geometry, _fragment) = context.ResourceLoader.LoadShadersGeometry("Billboard");
			_uvLayout = context.Device.ResourceFactory.CreateResourceLayout(new ResourceLayoutDescription(
						new ResourceLayoutElementDescription("UVS", ResourceKind.UniformBuffer, ShaderStages.Geometry)));

			var pDesc = new GraphicsPipelineDescription
			{
				BlendState = BlendStateDescription.SingleAlphaBlend,
				DepthStencilState = DepthStencilStateDescription.DepthOnlyLessEqual,
				RasterizerState = RasterizerStateDescription.Default,
				PrimitiveTopology = PrimitiveTopology.PointList,
				ResourceLayouts = new[] { context.ResourceLoader.ProjectionLayout,
					_uvLayout,
					context.ResourceLoader.TextureLayout },
				ShaderSet = new ShaderSetDescription(new[] { context.ResourceLoader.VertexStandardLayoutDescription }, new[] { _vertex, _geometry, _fragment }),
				Outputs = new OutputDescription
				{
					ColorAttachments = new[] { new OutputAttachmentDescription(PixelFormat.B8_G8_R8_A8_UNorm) },
					DepthAttachment = new OutputAttachmentDescription(PixelFormat.R32_Float),
					SampleCount = sampleCount
				}
			};

			_pipeline = context.Device.ResourceFactory.CreateGraphicsPipeline(ref pDesc);

			_projectionBuffer = context.Device.ResourceFactory.CreateBuffer(
				new BufferDescription((uint)Unsafe.SizeOf<UniformProjection>(), BufferUsage.UniformBuffer)
			);

			_projectionResourceSet = context.Device.ResourceFactory.CreateResourceSet(
				new ResourceSetDescription(context.ResourceLoader.ProjectionLayout, _projectionBuffer)
			);
		}

		public void SetupFrame(RenderContext context, IViewport target)
		{
			var view = target.Camera.View;
			if (!Matrix4x4.Invert(view, out var invView)) invView = Matrix4x4.Identity;

			context.Device.UpdateBuffer(_projectionBuffer, 0, new UniformProjection
			{
				Selective = context.SelectiveTransform,
				Model = invView,
				View = view,
				Projection = target.Camera.Projection,
			});
		}

		public void Render(RenderContext context, IViewport target, CommandList cl, IEnumerable<IRenderable> renderables)
		{
			return;
			cl.SetPipeline(_pipeline);
			cl.SetGraphicsResourceSet(0, _projectionResourceSet);
			
			foreach (var r in renderables.OfType<IModelRenderable>())
			{
				r.Render(context, this, target, cl);
			}
		}

		public void Render(RenderContext context, IViewport target, CommandList cl, IRenderable renderable, ILocation locationObject)
		{
			if (renderable is not IModelRenderable) return;
			cl.SetPipeline(_pipeline);
			cl.SetGraphicsResourceSet(0, _projectionResourceSet);

			renderable.Render(context, this, target, cl, locationObject);
		}
		public void Bind(RenderContext context, CommandList cl, string binding)
		{
			var tex = context.ResourceLoader.GetTexture(binding);
			tex?.BindTo(cl, 2);
		}

		public void Dispose()
		{
			_projectionResourceSet?.Dispose();
			_uvLayout?.Dispose();
			_projectionBuffer?.Dispose();
			_pipeline?.Dispose();
			_vertex?.Dispose();
			_geometry?.Dispose();
			_fragment?.Dispose();
		}
	}
}