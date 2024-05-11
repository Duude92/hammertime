using Sledge.Rendering.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veldrid;

namespace Sledge.Rendering.Resources
{
	public class ShadowMap : Texture
	{

		private ResourceSet _mainProjViewRS;
		private ResourceSet _mainSharedRS;
		private ResourceSet _mainPerObjectRS;
		private ResourceSet _reflectionRS;
		private ResourceSet _noReflectionRS;


		public ShadowMap(RenderContext context)
		{
			uint width = 256;
			uint height = 256;
			var device = context.Device;
			_texture = device.ResourceFactory.CreateTexture(TextureDescription.Texture2D(
				width, height, 1, 1,
				PixelFormat.B8_G8_R8_A8_UNorm,
				TextureUsage.Sampled
			));

			//try
			//{
			//	device.UpdateTexture(_texture, data, 0, 0, 0, w, h, _texture.Depth, 0, 0);
			//}
			//catch
			//{
			//	// Error updating texture, the texture may have been disposed
			//}

			var sampler = context.ResourceLoader.TextureSampler;
			//if (sampleType == TextureSampleType.Point) sampler = context.ResourceLoader.OverlaySampler;

			ResourceLayout projViewLayout = context.Device.ResourceFactory.CreateResourceLayout(
	new ResourceLayoutDescription(
		new ResourceLayoutElementDescription("Projection", ResourceKind.UniformBuffer, ShaderStages.Vertex)
	)
);

			ResourceLayout mainSharedLayout = context.Device.ResourceFactory.CreateResourceLayout(new ResourceLayoutDescription(
				new ResourceLayoutElementDescription("LightViewProjection1", ResourceKind.UniformBuffer, ShaderStages.Vertex | ShaderStages.Fragment),
				new ResourceLayoutElementDescription("LightViewProjection2", ResourceKind.UniformBuffer, ShaderStages.Vertex | ShaderStages.Fragment),
				new ResourceLayoutElementDescription("LightViewProjection3", ResourceKind.UniformBuffer, ShaderStages.Vertex | ShaderStages.Fragment),
				new ResourceLayoutElementDescription("DepthLimits", ResourceKind.UniformBuffer, ShaderStages.Vertex | ShaderStages.Fragment),
				new ResourceLayoutElementDescription("LightInfo", ResourceKind.UniformBuffer, ShaderStages.Vertex | ShaderStages.Fragment),
				new ResourceLayoutElementDescription("CameraInfo", ResourceKind.UniformBuffer, ShaderStages.Vertex | ShaderStages.Fragment),
				new ResourceLayoutElementDescription("PointLights", ResourceKind.UniformBuffer, ShaderStages.Vertex | ShaderStages.Fragment)));

			ResourceLayout mainPerObjectLayout = context.Device.ResourceFactory.CreateResourceLayout(new ResourceLayoutDescription(
				new ResourceLayoutElementDescription("WorldAndInverse", ResourceKind.UniformBuffer, ShaderStages.Vertex | ShaderStages.Fragment, ResourceLayoutElementOptions.DynamicBinding),
				new ResourceLayoutElementDescription("MaterialProperties", ResourceKind.UniformBuffer, ShaderStages.Vertex | ShaderStages.Fragment),
				new ResourceLayoutElementDescription("SurfaceTexture", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
				new ResourceLayoutElementDescription("RegularSampler", ResourceKind.Sampler, ShaderStages.Fragment),
				new ResourceLayoutElementDescription("AlphaMap", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
				new ResourceLayoutElementDescription("AlphaMapSampler", ResourceKind.Sampler, ShaderStages.Fragment),
				new ResourceLayoutElementDescription("ShadowMapNear", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
				new ResourceLayoutElementDescription("ShadowMapMid", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
				new ResourceLayoutElementDescription("ShadowMapFar", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
				new ResourceLayoutElementDescription("ShadowMapSampler", ResourceKind.Sampler, ShaderStages.Fragment)));

			ResourceLayout reflectionLayout = context.Device.ResourceFactory.CreateResourceLayout(new ResourceLayoutDescription(
				new ResourceLayoutElementDescription("ReflectionMap", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
				new ResourceLayoutElementDescription("ReflectionSampler", ResourceKind.Sampler, ShaderStages.Fragment),
				new ResourceLayoutElementDescription("ReflectionViewProj", ResourceKind.UniformBuffer, ShaderStages.Vertex),
				new ResourceLayoutElementDescription("ClipPlaneInfo", ResourceKind.UniformBuffer, ShaderStages.Fragment)));


			_view = device.ResourceFactory.CreateTextureView(_texture);
			_set = device.ResourceFactory.CreateResourceSet(new ResourceSetDescription(
				context.ResourceLoader.TextureLayout, _view, sampler
			));

	//		_mainProjViewRS = device.ResourceFactory.CreateResourceSet(new ResourceSetDescription(projViewLayout,
	//sc.ProjectionMatrixBuffer,
	//sc.ViewMatrixBuffer));

	//		_mainSharedRS = device.ResourceFactory.CreateResourceSet(new ResourceSetDescription(mainSharedLayout,
	//			sc.LightViewProjectionBuffer0,
	//			sc.LightViewProjectionBuffer1,
	//			sc.LightViewProjectionBuffer2,
	//			sc.DepthLimitsBuffer,
	//			sc.LightInfoBuffer,
	//			sc.CameraInfoBuffer,
	//			sc.PointLightsBuffer));

	//		_mainPerObjectRS = disposeFactory.CreateResourceSet(new ResourceSetDescription(mainPerObjectLayout,
	//			new DeviceBufferRange(_worldAndInverseBuffer, _uniformOffset, 128),
	//			_materialProps.UniformBuffer,
	//			_texture,
	//			gd.Aniso4xSampler,
	//			_alphaMapView,
	//			gd.LinearSampler,
	//			sc.NearShadowMapView,
	//			sc.MidShadowMapView,
	//			sc.FarShadowMapView,
	//			gd.PointSampler));

	//		_reflectionRS = StaticResourceCache.GetResourceSet(gd.ResourceFactory, new ResourceSetDescription(reflectionLayout,
	//			_alphaMapView, // Doesn't really matter -- just don't bind the actual reflection map since it's being rendered to.
	//			gd.PointSampler,
	//			sc.ReflectionViewProjBuffer,
	//			sc.MirrorClipPlaneBuffer));

	//		_noReflectionRS = StaticResourceCache.GetResourceSet(gd.ResourceFactory, new ResourceSetDescription(reflectionLayout,
	//			sc.ReflectionColorView,
	//			gd.PointSampler,
	//			sc.ReflectionViewProjBuffer,
	//			sc.NoClipPlaneBuffer));

		}
	}
}
