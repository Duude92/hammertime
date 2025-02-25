using Sledge.Common.Shell.Context;
using Sledge.Rendering.Engine;
using Sledge.Rendering.Renderables;
using Sledge.Rendering.Viewports;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using Veldrid;


namespace Sledge.Rendering.Pipelines
{
	public class ShadowmapDrawer : IRenderable, IPipeline
    {

        private DeviceBuffer _vb;
        private DeviceBuffer _ib;
        private DeviceBuffer _orthographicBuffer;
        private DeviceBuffer _sizeInfoBuffer;
        private Pipeline _pipeline;
        private ResourceSet _resourceSet;

        private Vector2 _position = Vector2.One;
        private Vector2 _size = new Vector2(0.01f, 0.01f);

        private readonly Func<TextureView> _bindingGetter;
        private SizeInfo? _si;
        private Matrix4x4? _ortho;

        public Vector2 Position { get => _position; set { _position = value; UpdateSizeInfoBuffer(); } }

        public Vector2 Size { get => _size; set { _size = value; UpdateSizeInfoBuffer(); } }

        private void UpdateSizeInfoBuffer()
        {
            _si = new SizeInfo { Size = _size, Position = _position };
        }

        public ShadowmapDrawer(Func<TextureView> bindingGetter)
        {
            _bindingGetter = bindingGetter;
        }

        public  void CreateDeviceObjects(GraphicsDevice gd, RenderContext context)
        {
			_ortho = Matrix4x4.CreateOrthographicOffCenter(0, 20, 20, 0, -1, 1);


			ResourceFactory factory = gd.ResourceFactory;
            _vb = factory.CreateBuffer(new BufferDescription((uint)s_quadVerts.Length * sizeof(float), BufferUsage.VertexBuffer));
            _ib = factory.CreateBuffer(new BufferDescription((uint)s_quadIndices.Length * sizeof(ushort), BufferUsage.IndexBuffer));

			gd.UpdateBuffer(_vb, 0, s_quadVerts);
			gd.UpdateBuffer(_ib, 0, s_quadIndices);
			VertexLayoutDescription[] vertexLayouts = new VertexLayoutDescription[]
            {
                new VertexLayoutDescription(
                    new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2),
                    new VertexElementDescription("TexCoord", VertexElementSemantic.TextureCoordinate,  VertexElementFormat.Float2))
            };
			(Shader _vertex, Shader _fragment) = context.ResourceLoader.LoadShaders("ShadowmapPreviewShader");

            ResourceLayout layout = factory.CreateResourceLayout(new ResourceLayoutDescription(
                new ResourceLayoutElementDescription("Projection", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                new ResourceLayoutElementDescription("SizePos", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                new ResourceLayoutElementDescription("Tex", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
                new ResourceLayoutElementDescription("TexSampler", ResourceKind.Sampler, ShaderStages.Fragment)));

            GraphicsPipelineDescription pd = new GraphicsPipelineDescription(
                BlendStateDescription.SingleOverrideBlend,
                new DepthStencilStateDescription(false, true, ComparisonKind.Always),
                RasterizerStateDescription.Default,
                PrimitiveTopology.TriangleList,
                new ShaderSetDescription(
                    vertexLayouts,
                    new[] { _vertex, _fragment }),
                //ShaderHelper.GetSpecializations(gd)),
                new ResourceLayout[] { layout },
                new OutputDescription
                {
                    ColorAttachments = new[] { new OutputAttachmentDescription(PixelFormat.B8_G8_R8_A8_UNorm) },
                    DepthAttachment = new OutputAttachmentDescription(PixelFormat.R32_Float),
                    SampleCount = TextureSampleCount.Count1
                });
			//sc.MainSceneFramebuffer.OutputDescription);


			_pipeline = factory.CreateGraphicsPipeline(ref pd);


            _sizeInfoBuffer = factory.CreateBuffer(new BufferDescription((uint)Unsafe.SizeOf<SizeInfo>(), BufferUsage.UniformBuffer));
            UpdateSizeInfoBuffer();
            _orthographicBuffer = factory.CreateBuffer(new BufferDescription((uint)Unsafe.SizeOf<Matrix4x4>(), BufferUsage.UniformBuffer));

            _resourceSet = factory.CreateResourceSet(new ResourceSetDescription(
                layout,
                _orthographicBuffer,
                _sizeInfoBuffer,
                _bindingGetter(),
                gd.PointSampler));


        }

		public PipelineGroup Group => PipelineGroup.Overlay;

        public PipelineType Type => PipelineType.Overlay;

        public float Order => 9998;

		public  void UpdatePerFrameResources(GraphicsDevice gd)
        {
            var cl = gd.ResourceFactory.CreateCommandList();
            cl.Begin();
			cl.UpdateBuffer(_vb, 0, s_quadVerts);
			cl.UpdateBuffer(_ib, 0, s_quadIndices);

			cl.UpdateBuffer(_sizeInfoBuffer, 0, _si.Value);
			cl.UpdateBuffer(_orthographicBuffer, 0, _ortho.Value);

			//if (_si.HasValue)
   //         {
			//	cl.UpdateBuffer(_sizeInfoBuffer, 0, _si.Value);
   //             _si = null;
   //         }

   //         if (_ortho.HasValue)
   //         {
			//	cl.UpdateBuffer(_orthographicBuffer, 0, _ortho.Value);
   //             _ortho = null;
   //         }
            cl.End();
			gd.SubmitCommands(cl);

		}

		public IEnumerable<ILocation> GetLocationObjects(IPipeline pipeline, IViewport viewport)
		{
            return new ILocation[0];
		}

		public bool ShouldRender(IPipeline pipeline, IViewport viewport)
		{
            return viewport.Camera.Type == Sledge.Rendering.Cameras.CameraType.Perspective;
		}

		public void Render(RenderContext context, IPipeline pipeline, IViewport viewport, CommandList cl)
		{
			cl.SetVertexBuffer(0, _vb);
			cl.SetIndexBuffer(_ib, IndexFormat.UInt16);
			cl.SetPipeline(_pipeline);
			cl.SetGraphicsResourceSet(0, _resourceSet);
			cl.DrawIndexed((uint)s_quadIndices.Length, 1, 0, 0, 0);
		}

		public void Render(RenderContext context, IPipeline pipeline, IViewport viewport, CommandList cl, ILocation locationObject)
		{
			cl.SetVertexBuffer(0, _vb);
			cl.SetIndexBuffer(_ib, IndexFormat.UInt16);
			cl.SetPipeline(_pipeline);
			cl.SetGraphicsResourceSet(0, _resourceSet);
			cl.DrawIndexed((uint)s_quadIndices.Length, 1, 0, 0, 0);
		}

		public void Dispose()
		{
            return;
		}

		public void Create(RenderContext context, TextureSampleCount sampleCount)
		{
            CreateDeviceObjects(context.Device, context);
		}

		public void SetupFrame(RenderContext context, IViewport target)
		{
            UpdatePerFrameResources(context.Device);
		}

		public void Render(RenderContext context, IViewport target, CommandList cl, IEnumerable<IRenderable> renderables)
		{
			this.Render(context, _pipeline as IPipeline, target, cl);
		}

		public void Render(RenderContext context, IViewport target, CommandList cl, IRenderable renderable, ILocation locationObject)
		{
			this.Render(context, _pipeline as IPipeline, target, cl);
		}

		public void Bind(RenderContext context, CommandList cl, string binding)
		{
            return;
		}

		private static float[] s_quadVerts = new float[]
        {
            0, 0, 0, 0,
            1, 0, 1, 0,
            1, 1, 1, 1,
            0, 1, 0, 1
        };

        private static ushort[] s_quadIndices = new ushort[] { 0, 1, 2, 0, 2, 3 };

        public struct SizeInfo
        {
            public Vector2 Position;
            public Vector2 Size;
        }
    }
}
