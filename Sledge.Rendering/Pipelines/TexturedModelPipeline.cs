﻿using System.Collections.Generic;
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
	public class TexturedModelPipeline : IPipeline
	{
		public PipelineType Type => PipelineType.TexturedModel;
		public PipelineGroup Group => PipelineGroup.Opaque;
		public float Order => 7;

		private Shader _vertex;
		private Shader _fragment;
		private Pipeline _pipeline;
		private DeviceBuffer _projectionBuffer;
		private ResourceSet _projectionResourceSet;
		private ResourceLayout _transformsLayout;
		private Engine.Engine.ViewProjectionBuffer _lastProjectionBuffer;

		public void Create(RenderContext context, TextureSampleCount sampleCount)
		{
			(_vertex, _fragment) = context.ResourceLoader.LoadShaders(Type.ToString());

			_transformsLayout = context.Device.ResourceFactory.CreateResourceLayout(
				new ResourceLayoutDescription(
					new ResourceLayoutElementDescription("uTransforms", ResourceKind.UniformBuffer, ShaderStages.Vertex)
				)
			);

			var pDesc = new GraphicsPipelineDescription
			{
				BlendState = BlendStateDescription.SingleDisabled,
				DepthStencilState = DepthStencilStateDescription.DepthOnlyLessEqual,
				RasterizerState = RasterizerStateDescription.Default,
				PrimitiveTopology = PrimitiveTopology.TriangleList,
				ResourceLayouts = new[] { context.ResourceLoader.ProjectionLayout, context.ResourceLoader.TextureLayout, _transformsLayout },
				ShaderSet = new ShaderSetDescription(new[] { context.ResourceLoader.VertexModel3LayoutDescription }, new[] { _vertex, _fragment }),
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

		public void SetupFrame(RenderContext context, Engine.Engine.ViewProjectionBuffer viewProjectionBuffer)
		{
			_lastProjectionBuffer = viewProjectionBuffer;
		}

		public void Render(RenderContext context, IViewport target, CommandList cl, IEnumerable<IRenderable> renderables)
		{
			cl.SetPipeline(_pipeline);
			cl.SetGraphicsResourceSet(0, _projectionResourceSet);

			foreach (var r in renderables.OfType<IModelRenderable>())
			{
				cl.UpdateBuffer(_projectionBuffer, 0, new UniformProjection
				{
					Selective = r.Flags.HasFlag(VertexFlags.SelectiveTransformed) ? context.SelectiveTransform : Matrix4x4.Identity,
					Model = r.GetModelTransformation(),
					View = _lastProjectionBuffer.View,
					Projection = _lastProjectionBuffer.Projection,
				});

				r.Render(context, this, target, cl);
			}
		}

		public void Render(RenderContext context, IViewport target, CommandList cl, IRenderable renderable, ILocation locationObject)
		{
			cl.SetPipeline(_pipeline);
			cl.SetGraphicsResourceSet(0, _projectionResourceSet);

			if (renderable is IModelRenderable r)
			{
				cl.UpdateBuffer(_projectionBuffer, 0, new UniformProjection
				{
					Selective = context.SelectiveTransform,
					Model = r.GetModelTransformation(),
					View = _lastProjectionBuffer.View,
					Projection = _lastProjectionBuffer.Projection,
				});
			}

			renderable.Render(context, this, target, cl, locationObject);
		}

		public void Bind(RenderContext context, CommandList cl, string binding)
		{
			var tex = context.ResourceLoader.GetTexture(binding);
			tex?.BindTo(cl, 1);
		}

		public void Dispose()
		{
			_projectionResourceSet?.Dispose();
			_projectionBuffer?.Dispose();
			_pipeline?.Dispose();
			_transformsLayout?.Dispose();
			_vertex?.Dispose();
			_fragment?.Dispose();
		}
	}
}