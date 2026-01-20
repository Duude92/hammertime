using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Engine;
using Sledge.Rendering.Primitives;
using Sledge.Rendering.Renderables;
using Sledge.Rendering.Viewports;
using Veldrid;
using static Sledge.Rendering.Engine.Engine;

namespace Sledge.Rendering.Pipelines
{
	public class ShadowOverlayPipeline : IPipeline
	{
		public PipelineType Type => PipelineType.ShadowOverlay;
		public PipelineGroup Group => PipelineGroup.Transparent;
		public float Order => 10;

		private Shader _vertex;
		private Shader _fragment;
		private Pipeline _pipeline;
		private DeviceBuffer _projectionBuffer;
		private ResourceSet _projectionResourceSet;
		private ViewProjectionBuffer _lightData;

		private ResourceSet _lightDirectionSet;
		private DeviceBuffer _lightProjection;
		private ResourceSet _lightProjectionSet;
		private DeviceBuffer _lightDirection;

		public ShadowOverlayPipeline(Engine.Engine.ViewProjectionBuffer lightData)
		{
			_lightData = lightData;
		}


		public void Create(RenderContext context, TextureSampleCount sampleCount)
		{
			(_vertex, _fragment) = context.ResourceLoader.LoadShaders(Type.ToString());

			var pDesc = new GraphicsPipelineDescription
			{
				BlendState = new BlendStateDescription
				{
					AttachmentStates = new[]
					{
						new BlendAttachmentDescription
						{
							BlendEnabled = true,
							SourceColorFactor = BlendFactor.Zero,  // Ignore the source color
							DestinationColorFactor = BlendFactor.SourceColor, // Multiply existing color by shadow
							ColorFunction = BlendFunction.Add, // Multiply result

							SourceAlphaFactor = BlendFactor.One,
							DestinationAlphaFactor = BlendFactor.One,
							AlphaFunction = BlendFunction.Add,
						}
					}
				},
				DepthStencilState = DepthStencilStateDescription.DepthOnlyLessEqual,
				RasterizerState = context.GraphicBackend.RasterizerStateDescription,
				PrimitiveTopology = PrimitiveTopology.TriangleList,
				ResourceLayouts = new[] { context.ResourceLoader.ProjectionLayout, context.ResourceLoader.TextureLayout,
					context.Device.ResourceFactory.CreateResourceLayout(
						new ResourceLayoutDescription(
						new ResourceLayoutElementDescription("LightView", ResourceKind.UniformBuffer, ShaderStages.Vertex|ShaderStages.Fragment)
					)),
					context.Device.ResourceFactory.CreateResourceLayout(
						new ResourceLayoutDescription(
						new ResourceLayoutElementDescription("LightProjection", ResourceKind.UniformBuffer, ShaderStages.Vertex)
					))
},
				ShaderSet = new ShaderSetDescription(new[] { context.ResourceLoader.VertexStandardLayoutDescription }, new[] { _vertex, _fragment }),
				Outputs = new OutputDescription
				{
					ColorAttachments = new[] { new OutputAttachmentDescription(PixelFormat.B8_G8_R8_A8_UNorm) },
					DepthAttachment = new OutputAttachmentDescription(PixelFormat.R32_Float),
					SampleCount = sampleCount
				}
			};

			_lightProjection = context.Device.ResourceFactory.CreateBuffer(
new BufferDescription((uint)Unsafe.SizeOf<Matrix4x4>(), BufferUsage.UniformBuffer)
);
			_lightProjection.Name = "lightProjectionbuffer";
			_lightProjectionSet = context.Device.ResourceFactory.CreateResourceSet(
	new ResourceSetDescription(context.ResourceLoader.ProjectionLayout, _lightProjection)
);

			_lightDirection = context.Device.ResourceFactory.CreateBuffer(
new BufferDescription((uint)Unsafe.SizeOf<Matrix4x4>(), BufferUsage.UniformBuffer)
);
			_lightDirection.Name = "lightViewbuffer";

			_lightDirectionSet = context.Device.ResourceFactory.CreateResourceSet(
				new ResourceSetDescription(context.ResourceLoader.ProjectionLayout, _lightDirection)
			);

			_pipeline = context.Device.ResourceFactory.CreateGraphicsPipeline(ref pDesc);
			_pipeline.Name = "Shadow Overlay Pipeline";

			_projectionBuffer = context.Device.ResourceFactory.CreateBuffer(
				new BufferDescription((uint)Unsafe.SizeOf<UniformProjection>(), BufferUsage.UniformBuffer)
			);

			_projectionResourceSet = context.Device.ResourceFactory.CreateResourceSet(
				new ResourceSetDescription(context.ResourceLoader.ProjectionLayout, _projectionBuffer)
			);
		}

		public void SetupFrame(RenderContext context, Engine.Engine.ViewProjectionBuffer viewProjectionBuffer)
		{
			context.Device.UpdateBuffer(_projectionBuffer, 0, new UniformProjection
			{
				Selective = context.SelectiveTransform,
				Model = Matrix4x4.Identity,
				View = viewProjectionBuffer.View,
				Projection = viewProjectionBuffer.Projection,
			});

			context.Device.UpdateBuffer(_lightDirection, 0, _lightData.View);
			context.Device.UpdateBuffer(_lightProjection, 0, _lightData.Projection);
		}

		public void SetupFrame(RenderContext context, CommandList cl, Engine.Engine.ViewProjectionBuffer viewProjectionBuffer)
		{
			cl.UpdateBuffer(_projectionBuffer, 0, new UniformProjection
			{
				Selective = context.SelectiveTransform,
				Model = Matrix4x4.Identity,
				View = viewProjectionBuffer.View,
				Projection = viewProjectionBuffer.Projection,
			});

			cl.UpdateBuffer(_lightDirection, 0, _lightData.View);
			cl.UpdateBuffer(_lightProjection, 0, _lightData.Projection);
		}
		public void Render(RenderContext context, IViewport target, CommandList cl, IEnumerable<IRenderable> renderables) {
			if (target.Camera is not PerspectiveCamera) return;

			cl.SetPipeline(_pipeline);
			cl.SetGraphicsResourceSet(0, _projectionResourceSet);

			cl.SetGraphicsResourceSet(2, _lightDirectionSet);
			cl.SetGraphicsResourceSet(3, _lightProjectionSet);

			foreach (var r in renderables)
			{
				r.Render(context, this, target, cl);
			}
		}

		public void Render(RenderContext context, IViewport target, CommandList cl, IRenderable renderable, ILocation locationObject)
		{
			if (target.Camera is not PerspectiveCamera) return;
			cl.SetPipeline(_pipeline);
			cl.SetGraphicsResourceSet(0, _projectionResourceSet);

			cl.SetGraphicsResourceSet(2, _lightDirectionSet);
			cl.SetGraphicsResourceSet(3, _lightProjectionSet);

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
			_vertex?.Dispose();
			_fragment?.Dispose();
		}
	}
}