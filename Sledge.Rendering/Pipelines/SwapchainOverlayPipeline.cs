using Sledge.Rendering.Engine;
using Sledge.Rendering.Primitives;
using Sledge.Rendering.Renderables;
using Sledge.Rendering.Viewports;
using System;
using System.Collections.Generic;
using System.Numerics;
using Veldrid;

namespace Sledge.Rendering.Pipelines
{
	internal class SwapchainOverlayPipeline : IPipeline
	{
		private Pipeline _quadPipeline;
		private Shader _vertex;
		private Shader _fragment;
		private DeviceBuffer _quadVertexBuffer;
		private DeviceBuffer _quadIndexBuffer;

		public PipelineType Type => PipelineType.TexturedOpaque;

		public PipelineGroup Group => PipelineGroup.Overlay;

		public float Order => 1;
		private static VertexStandard[] _overlayVertices = new VertexStandard[]
		{
			new VertexStandard{ Position = new Vector3(-1, -1, 0f), Texture = new Vector2(0, 1) },
			new VertexStandard{ Position = new Vector3( 1, -1, 0f), Texture = new Vector2(1, 1) },
			new VertexStandard{ Position = new Vector3(-1,  1, 0f), Texture = new Vector2(0, 0) },
			new VertexStandard{ Position = new Vector3( 1,  1, 0f), Texture = new Vector2(1, 0) },
		};
		UInt32[] _quadIndices = { 0, 1, 2, 2, 1, 3 };

		public void Bind(RenderContext context, CommandList cl, string binding)
		{
			var tex = context.ResourceLoader.GetTexture(binding);
			tex?.BindTo(cl, 1);
		}

		public void Create(RenderContext context, TextureSampleCount sampleCount)
		{
			(_vertex, _fragment) = context.ResourceLoader.LoadShaders("SwapchainOverlay");
			_vertex.Name = "SwapchainOverlayVertexShader";
			_fragment.Name = "SwapchainOverlayFragmentShader";

			var _gd = context.Device;

			GraphicsPipelineDescription quadPipelineDesc = new GraphicsPipelineDescription
			{
				BlendState = BlendStateDescription.SingleAlphaBlend,
				DepthStencilState = new DepthStencilStateDescription(false, false, ComparisonKind.Always),
				RasterizerState = new RasterizerStateDescription(FaceCullMode.None, PolygonFillMode.Solid, FrontFace.Clockwise, false, false),
				PrimitiveTopology = PrimitiveTopology.TriangleList,
				ResourceLayouts = new[] { context.ResourceLoader.TextureLayout },
				ShaderSet = new ShaderSetDescription(new[] { context.ResourceLoader.VertexStandardLayoutDescription }, new[] { _vertex, _fragment }),
				Outputs = new OutputDescription
				{
					ColorAttachments = new[] { new OutputAttachmentDescription(PixelFormat.B8_G8_R8_A8_UNorm) },
					DepthAttachment = new OutputAttachmentDescription(PixelFormat.R32_Float),
					SampleCount = TextureSampleCount.Count1
				}
			};
			_quadPipeline = _gd.ResourceFactory.CreateGraphicsPipeline(ref quadPipelineDesc);
			_quadPipeline.Name = "SwapchainOverlayPipeline";

			BufferDescription vbDesc = new BufferDescription(
				(uint)(_overlayVertices.Length * VertexStandard.SizeInBytes),
				BufferUsage.VertexBuffer);

			_quadVertexBuffer = _gd.ResourceFactory.CreateBuffer(vbDesc);
			_gd.UpdateBuffer(_quadVertexBuffer, 0, _overlayVertices);

			BufferDescription ibDesc = new BufferDescription(
				(uint)(_quadIndices.Length * sizeof(UInt32)),
				BufferUsage.IndexBuffer);
			_quadIndexBuffer = _gd.ResourceFactory.CreateBuffer(ibDesc);


			_gd.UpdateBuffer(_quadIndexBuffer, 0, _quadIndices);
		}

		public void Dispose()
		{
			_quadPipeline.Dispose();
			_vertex.Dispose();
			_fragment.Dispose();
			_quadVertexBuffer.Dispose();
			_quadIndexBuffer.Dispose();
		}

		public void Render(RenderContext context, IViewport target, CommandList cl, IEnumerable<IRenderable> renderables)
		{
			cl.SetPipeline(_quadPipeline);
			cl.SetVertexBuffer(0, _quadVertexBuffer);
			cl.SetIndexBuffer(_quadIndexBuffer, IndexFormat.UInt32);

			target.ViewportRenderTexture.BindTo(cl, 0);
			cl.DrawIndexed(6, 1, 0, 0, 0);
		}

		public void Render(RenderContext context, IViewport target, CommandList cl, IRenderable renderable, ILocation locationObject)
		{
		}

		public void SetupFrame(RenderContext context, Engine.Engine.ViewProjectionBuffer viewProjectionBuffer)
		{
		}
	}
}
