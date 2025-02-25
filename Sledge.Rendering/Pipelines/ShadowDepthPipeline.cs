using LogicAndTrick.Oy;
using Sledge.Common.Shell.Context;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Engine;
using Sledge.Rendering.Primitives;
using Sledge.Rendering.Renderables;
using Sledge.Rendering.Resources;
using Sledge.Rendering.Viewports;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
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
		private ResourceSet _textureSet;
		public Resources.Texture NearShadowResourceTexture { get; private set; }
		public Veldrid.Texture NearShadowMap { get; private set; }
		public TextureView NearShadowMapView { get; private set; }
		public Framebuffer NearShadowMapFramebuffer { get; private set; }

		private DeviceBuffer _worldAndInverseBuffer;
		private uint _uniformOffset = 0;

		//private ResourceSet _shadowmapResource;
		private ResourceSet _shadowWorld;
		private Bitmap _bitmap;
		private Veldrid.Texture stagingTexture;

		public void Bind(RenderContext context, CommandList cl, string binding)
		{
			//var tex = context.ResourceLoader.GetTexture(binding);
			//tex?.BindTo(cl, 1);
			//NearShadowResourceTexture?.BindTo(cl,1);
			cl.SetGraphicsResourceSet(0, _projectionResourceSet);
			//cl.SetGraphicsResourceSet(1, _shadowWorld);
			//cl.SetGraphicsResourceSet(2, _textureSet);
		}

		public void Create(RenderContext context)
		{

			var gd = context.Device;
			_uniformOffset = gd.UniformBufferMinOffsetAlignment;

			var factory = gd.ResourceFactory;

			TextureDescription desc = TextureDescription.Texture2D(2048, 2048, 1, 1, PixelFormat.R32_Float, TextureUsage.DepthStencil | TextureUsage.Sampled, TextureSampleCount.Count1);
			NearShadowMap = factory.CreateTexture(ref desc);

			//TextureDescription desc1 = TextureDescription.Texture2D(2048, 2048, 1, 1, PixelFormat.B8_G8_R8_A8_UNorm, TextureUsage.Sampled | TextureUsage.RenderTarget);
			//var ColorShadowMap = factory.CreateTexture(desc1);

			//gd.UpdateTexture(NearShadowMap, new byte[2048 * 2048 * 8], 0, 0,0, 2048, 2048, 1, 0, 0);

			NearShadowMap.Name = "Near Shadow Map";
			NearShadowMapView = factory.CreateTextureView(NearShadowMap);
			NearShadowMapFramebuffer = factory.CreateFramebuffer(new FramebufferDescription(
				new FramebufferAttachmentDescription(NearShadowMap, 0), Array.Empty<FramebufferAttachmentDescription>() /*new FramebufferAttachmentDescription[] { new FramebufferAttachmentDescription(ColorShadowMap,1) }*/   ));
			NearShadowMapFramebuffer.Name = "near shadowmap";

			(_vertex, _fragment) = context.ResourceLoader.LoadShaders(Type.ToString());

			ResourceLayout projViewCombinedLayout = context.ResourceLoader.ProjectionLayout;

			//ResourceLayout worldLayout = context.Device.ResourceFactory.CreateResourceLayout(new ResourceLayoutDescription(
			//	new ResourceLayoutElementDescription("WorldAndInverse", ResourceKind.UniformBuffer, ShaderStages.Vertex)));


			//VertexLayoutDescription[] shadowDepthVertexLayouts = new VertexLayoutDescription[]
			//{
			//	new VertexLayoutDescription(
			//		new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3),
			//		new VertexElementDescription("Normal", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3),
			//		new VertexElementDescription("TexCoord", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2))
			//};

			var pd = new GraphicsPipelineDescription
			{
				BlendState = BlendStateDescription.Empty,
				DepthStencilState = DepthStencilStateDescription.DepthOnlyLessEqual,
				RasterizerState = RasterizerStateDescription.Default,

				PrimitiveTopology = PrimitiveTopology.TriangleList,
				ResourceLayouts = new ResourceLayout[] { projViewCombinedLayout },// ,context.ResourceLoader.TextureLayout/*, worldLayout*/ },
				ShaderSet = new ShaderSetDescription(new[] { context.ResourceLoader.VertexStandardLayoutDescription }, new[] { _vertex }),
				Outputs = NearShadowMapFramebuffer.OutputDescription,
			};

			_pipeline = context.Device.ResourceFactory.CreateGraphicsPipeline(ref pd);
			_pipeline.Name = "shadowdepthpipeline";

			_projectionBuffer = context.Device.ResourceFactory.CreateBuffer(
				new BufferDescription((uint)Unsafe.SizeOf<UniformProjection>(), BufferUsage.UniformBuffer)
			);
			_projectionBuffer.Name = "projection buffer";

			_projectionResourceSet = context.Device.ResourceFactory.CreateResourceSet(
				new ResourceSetDescription(context.ResourceLoader.ProjectionLayout, _projectionBuffer)
			);


			_textureSet = gd.ResourceFactory.CreateResourceSet(new ResourceSetDescription(
				context.ResourceLoader.TextureLayout, NearShadowMapView, context.ResourceLoader.TextureSampler
			));

			NearShadowResourceTexture = new Resources.Texture(context, NearShadowMap, Resources.TextureSampleType.Standard, _textureSet);
			NearShadowResourceTexture.GenerateMips = false;


			_bitmap = new Bitmap((int)10, (int)10, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
			// Step 1: Create a staging texture
			stagingTexture = gd.ResourceFactory.CreateTexture(TextureDescription.Texture2D(NearShadowMap.Width, NearShadowMap.Height, 1, 1, NearShadowMap.Format, TextureUsage.Staging));

			Oy.Subscribe("GetDepthImage", () => Oy.Publish("Context:Add", new ContextInfo("DepthImage", _bitmap)));
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
			if (target.Camera is not PerspectiveCamera) return;

			SetupFrame(context, target);
			cl.PushDebugGroup("Shadow Map - Near Cascade");
			cl.SetFramebuffer(NearShadowMapFramebuffer);

			cl.SetFullViewports();
			cl.ClearDepthStencil(1);

			cl.SetPipeline(_pipeline);
			//cl.SetGraphicsResourceSet(0, _shadowmapResource);

			//cl.SetGraphicsResourceSet(1, _shadowWorld);

			cl.SetGraphicsResourceSet(0, _projectionResourceSet);
			//cl.SetGraphicsResourceSet(1, _shadowWorld);
			//cl.SetGraphicsResourceSet(2, _textureSet);
			var count = renderables.Count();

			foreach (var r in renderables)
			{
				r.Render(context, this, target, cl);
			}

			cl.PopDebugGroup();
		}

		public void Render(RenderContext context, IViewport target, CommandList cl, IRenderable renderable, ILocation locationObject)
		{
			cl.SetPipeline(_pipeline);
			cl.SetGraphicsResourceSet(0, _textureSet);
			cl.SetGraphicsResourceSet(1, _textureSet);

			renderable.Render(context, this, target, cl, locationObject); 
		}
		public void SetupFrame(RenderContext context, IViewport target)
		{
			if (target.Camera is not PerspectiveCamera) return;

			float orthoSize = 5000f; // Adjust based on scene size
			float nearPlane = 1f;
			float farPlane = 5000f;

			Matrix4x4 lightProjection = Matrix4x4.CreateOrthographic(
				orthoSize, // Width
				orthoSize, // Height
				nearPlane,
				farPlane
			);

			Vector3 lightPosition = new Vector3(0, 0, 1000); // Light high above the scene
			Vector3 lightTarget = new Vector3(100, 0, 0); // Looking at the scene center
			Vector3 upVector = Vector3.UnitZ; // Adjust if needed


			Matrix4x4 lightView = Matrix4x4.CreateLookAt(
	lightPosition,
	lightTarget,
	upVector
);


			context.Device.UpdateBuffer(_projectionBuffer, 0, new UniformProjection
			{
				Selective = context.SelectiveTransform,
				Model = Matrix4x4.Identity,
				View = lightView,
				Projection = lightProjection
			});
		}
	}
}
