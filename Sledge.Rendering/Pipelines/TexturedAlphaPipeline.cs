using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using Sledge.Rendering.Engine;
using Sledge.Rendering.Primitives;
using Sledge.Rendering.Renderables;
using Sledge.Rendering.Viewports;
using Veldrid;

namespace Sledge.Rendering.Pipelines
{
    public class TexturedAlphaPipeline : IPipeline
    {
        public PipelineType Type => PipelineType.TexturedAlpha;
        public PipelineGroup Group => PipelineGroup.Transparent;
        public float Order => 5;

        private Shader _vertex;
        private Shader _fragment;
        private Pipeline _pipeline;
        private DeviceBuffer _projectionBuffer;
        private ResourceSet _projectionResourceSet;

        public void Create(RenderContext context, TextureSampleCount sampleCount)
        {
            (_vertex, _fragment) = context.ResourceLoader.LoadShaders(Type.ToString());

            var pDesc = new GraphicsPipelineDescription
            {
                BlendState = BlendStateDescription.SingleAlphaBlend,
                DepthStencilState = DepthStencilStateDescription.DepthOnlyLessEqualRead,
                RasterizerState = context.GraphicBackend.RasterizerStateDescription,
                PrimitiveTopology = PrimitiveTopology.TriangleList,
                ResourceLayouts = new[] { context.ResourceLoader.ProjectionLayout, context.ResourceLoader.TextureLayout },
                ShaderSet = new ShaderSetDescription(new[] { context.ResourceLoader.VertexStandardLayoutDescription }, new[] { _vertex, _fragment }),
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
            context.Device.UpdateBuffer(_projectionBuffer, 0, new UniformProjection
            {
                Selective = context.SelectiveTransform,
                Model = Matrix4x4.Identity,
                View = viewProjectionBuffer.View,
                Projection = viewProjectionBuffer.Projection,
            });
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
			cl.SetPipeline(_pipeline);
			cl.SetGraphicsResourceSet(0, _projectionResourceSet);
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