using Sledge.Rendering.Cameras;
using Sledge.Rendering.Engine;
using Sledge.Rendering.Primitives;
using Sledge.Rendering.Renderables;
using Sledge.Rendering.Viewports;
using System;
using System.Collections.Generic;
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
		private Engine.Engine.ViewProjectionBuffer _lightData;
		public Resources.Texture NearShadowResourceTexture { get; private set; }
		public Veldrid.Texture NearShadowMap { get; private set; }
		public TextureView NearShadowMapView { get; private set; }
		public Framebuffer NearShadowMapFramebuffer { get; private set; }
		public ShadowDepthPipeline(Engine.Engine.ViewProjectionBuffer lightData)
		{
			_lightData = lightData;
		}

		public void Bind(RenderContext context, CommandList cl, string binding)
		{
			cl.SetGraphicsResourceSet(0, _projectionResourceSet);
		}

		public void Create(RenderContext context, TextureSampleCount sampleCount)
		{
			var gd = context.Device;
			var factory = gd.ResourceFactory;

			TextureDescription desc = TextureDescription.Texture2D(2048, 2048, 1, 1, PixelFormat.D32_Float_S8_UInt, TextureUsage.DepthStencil | TextureUsage.Sampled, TextureSampleCount.Count1);
			NearShadowMap = factory.CreateTexture(ref desc);

			NearShadowMap.Name = "Near Shadow Map";
			NearShadowMapView = factory.CreateTextureView(NearShadowMap);
			NearShadowMapFramebuffer = factory.CreateFramebuffer(new FramebufferDescription(
				new FramebufferAttachmentDescription(NearShadowMap, 0), Array.Empty<FramebufferAttachmentDescription>() /*new FramebufferAttachmentDescription[] { new FramebufferAttachmentDescription(ColorShadowMap,1) }*/   ));
			NearShadowMapFramebuffer.Name = "near shadowmap";

			(_vertex, _fragment) = context.ResourceLoader.LoadShaders(Type.ToString());

			ResourceLayout projViewCombinedLayout = context.ResourceLoader.ProjectionLayout;

			var pd = new GraphicsPipelineDescription
			{
				BlendState = BlendStateDescription.Empty,
				DepthStencilState = DepthStencilStateDescription.DepthOnlyLessEqual,
				RasterizerState = new RasterizerStateDescription { CullMode = FaceCullMode.Back, DepthClipEnabled = true, FrontFace = FrontFace.CounterClockwise, FillMode = PolygonFillMode.Solid },

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
#if USE_COMPARISON_SAMPLER
			var shadowSampler = factory.CreateSampler(new SamplerDescription(
				
	addressModeU: SamplerAddressMode.Border,  // Prevents artifacts outside shadow map
	addressModeV: SamplerAddressMode.Border,  // Prevents artifacts outside shadow map
	addressModeW: SamplerAddressMode.Border,  // Prevents artifacts outside shadow map
	filter: SamplerFilter.MinLinear_MagLinear_MipPoint, // Enables hardware PCF
	comparisonKind: ComparisonKind.LessEqual, // Depth test (currentDepth ≤ storedDepth)
	maximumAnisotropy: 1, // No anisotropic filtering
	minimumLod: 0, // No LOD biasing
	maximumLod: 0, // No LOD biasing
	lodBias: 0, // No LOD biasing
	borderColor: SamplerBorderColor.OpaqueWhite
));
#endif
			SamplerDescription samplerDescription = new SamplerDescription(
	addressModeU: SamplerAddressMode.Border,  // Repeat in U direction
	addressModeV: SamplerAddressMode.Border,  // Repeat in V direction
	addressModeW: SamplerAddressMode.Border,  // Repeat in W direction (for 3D textures)
	SamplerFilter.MinLinear_MagLinear_MipLinear,
	ComparisonKind.LessEqual,
	0,0,0,0, SamplerBorderColor.OpaqueBlack
);
			Sampler sampler = factory.CreateSampler(ref samplerDescription);

			_textureSet = gd.ResourceFactory.CreateResourceSet(new ResourceSetDescription(
				context.ResourceLoader.TextureLayout, NearShadowMapView, sampler
			));
			NearShadowResourceTexture = new Resources.Texture(context, NearShadowMap, Resources.TextureSampleType.Standard, _textureSet);
			NearShadowResourceTexture.GenerateMips = false;
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

			//SetupFrame(context, target);
			cl.PushDebugGroup("Shadow Map - Near Cascade");
			cl.SetFramebuffer(NearShadowMapFramebuffer);

			cl.SetFullViewports();
			cl.ClearDepthStencil(1);

			cl.SetPipeline(_pipeline);
			cl.SetGraphicsResourceSet(0, _projectionResourceSet);
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
		public void SetupFrame(RenderContext context, Engine.Engine.ViewProjectionBuffer viewProjectionBuffer)
		{
			if (viewProjectionBuffer.RenderTarget.Camera is not PerspectiveCamera perspectiveCamera) return;

			Vector3 lightDirection = (GetLightDirection(Engine.Engine.Instance.LightAngle) );
			var newLightDirection = new Vector3(lightDirection.Z, lightDirection.X, lightDirection.Y);
			Vector3 lightPosition = perspectiveCamera.Position + newLightDirection * 2000;

			float dotProduct = (GetZAxisDotProduct(perspectiveCamera.Direction, lightDirection) );

			float r = 1000;  // Radius
			lightPosition = GetCircularPosition(lightPosition ,  r, dotProduct);

			Vector3 GetCircularPosition(Vector3 center, float radius, float value)
			{
				float angle = 2 * MathF.PI * value; // Convert value [0,1] into full circle angle
				angle = -value;
				angle += MathF.PI;
				if (angle >= MathF.Tau) angle -= MathF.Tau;

				float x = center.X + radius * MathF.Cos(angle);
				float y = center.Y + radius * MathF.Sin(angle);
				float z = center.Z; // Remains unchanged (optional)

				return new Vector3(x, y, z);
			}

			float GetZAxisDotProduct(Vector3 cameraForward, Vector3 lightDirection)
			{
				// Project vectors onto the XY plane (ignore Z)
				Vector2 cameraXY = new Vector2(cameraForward.X, cameraForward.Y);
				Vector2 lightXY = new Vector2(lightDirection.X, lightDirection.Y);

				// Normalize both vectors
				cameraXY = Vector2.Normalize(cameraXY);
				lightXY = Vector2.Normalize(lightXY);

				// Compute the dot and cross product
				float dotProduct = Vector2.Dot(cameraXY, lightXY);
				float crossProduct = cameraXY.X * lightXY.Y - cameraXY.Y * lightXY.X;

				// Compute angle using Atan2
				float angle = MathF.Atan2(crossProduct, dotProduct); // Range: [-π, π]

				// Convert to [0, 2π] range
				if (angle < 0) angle += MathF.Tau; // MathF.Tau = 2π

				// 🔹 Shift angle by -π/2 to align with the expected result
				angle -= MathF.PI / 2;
				if (angle < 0) angle += MathF.Tau; // Ensure it's still in [0, 2π]

				return angle;
			}

			Engine.Engine.Instance.LightSourcePosition = lightPosition;


			context.Device.UpdateBuffer(_projectionBuffer, 0, new UniformProjection
			{
				Selective = context.SelectiveTransform,
				Model = Matrix4x4.Identity,
				View = _lightData.View,
				Projection = _lightData.Projection
			});
			Vector3 GetLightDirection(Vector3 angles)
			{
				var rot = Matrix4x4.CreateRotationX(angles.Y) * Matrix4x4.CreateRotationZ(angles.X);
				return Vector3.Transform(-Vector3.UnitZ, rot);
			}
		}
	}
}
