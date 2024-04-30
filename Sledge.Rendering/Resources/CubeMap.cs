using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Sledge.Rendering.Engine;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veldrid;
using Veldrid.ImageSharp;

namespace Sledge.Rendering.Resources
{
	public class CubeMap : Texture
	{
		private readonly SixLabors.ImageSharp.Image<Rgba32>[] _images = new SixLabors.ImageSharp.Image<Rgba32>[6];

		public CubeMap(RenderContext context, IEnumerable<SixLabors.ImageSharp.Image<Rgba32>> images , TextureSampleType sampleType)
		{
			var device = context.Device;
			var factory = context.Device.ResourceFactory;

			_images = images.ToArray();
			ImageSharpCubemapTexture imageSharpCubemapTexture = new ImageSharpCubemapTexture(_images[2], _images[0], _images[5], _images[1], _images[4], _images[3], false);

			_texture = imageSharpCubemapTexture.CreateDeviceTexture(context.Device, factory);
			_texture.Name = "SkyTexture";

			TextureView textureView = factory.CreateTextureView(new TextureViewDescription(_texture));

			_view = context.Device.ResourceFactory.CreateTextureView(_texture);


			var sampler = context.ResourceLoader.TextureSampler;
			_set = device.ResourceFactory.CreateResourceSet(new ResourceSetDescription(
				context.ResourceLoader.TextureLayout, textureView, sampler));

			_mipsGenerated = true;
		}
	}
}
