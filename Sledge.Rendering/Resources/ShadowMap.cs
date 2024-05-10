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

			_view = device.ResourceFactory.CreateTextureView(_texture);
			_set = device.ResourceFactory.CreateResourceSet(new ResourceSetDescription(
				context.ResourceLoader.TextureLayout, _view, sampler
			));

		}
	}
}
