using SharpDX.DXGI;
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

		public CubeMap(RenderContext context, string name, TextureSampleType sampleType)
		{

		}


        public CubeMap(RenderContext context,  TextureSampleType sampleType) : base()
		{
			//_right = SixLabors.ImageSharp.Image.Load<Rgba32>("d:/steam/steamapps/common/Half-Life/cstrike/gfx/env/de_stormrt.bmp");
			//_left = SixLabors.ImageSharp.Image.Load<Rgba32>("d:/steam/steamapps/common/Half-Life/cstrike/gfx/env/de_stormlf.bmp");
			//_top = SixLabors.ImageSharp.Image.Load<Rgba32>("d:/steam/steamapps/common/Half-Life/cstrike/gfx/env/de_stormup.bmp");
			//_bottom = SixLabors.ImageSharp.Image.Load<Rgba32>("d:/steam/steamapps/common/Half-Life/cstrike/gfx/env/de_stormdn.bmp");
			//_back = SixLabors.ImageSharp.Image.Load<Rgba32>("d:/steam/steamapps/common/Half-Life/cstrike/gfx/env/de_stormbk.bmp");
			//_front = SixLabors.ImageSharp.Image.Load<Rgba32>("d:/steam/steamapps/common/Half-Life/cstrike/gfx/env/de_stormft.bmp");


			var device = context.Device;
			var factory = context.Device.ResourceFactory;

			//ImageSharpCubemapTexture imageSharpCubemapTexture = new ImageSharpCubemapTexture(_right, _left, _top, _bottom, _back, _front, false);

			ImageSharpCubemapTexture imageSharpCubemapTexture = new ImageSharpCubemapTexture("d:/env/officert.tga", "d:/env/officelf.tga","d:/env/officeup.tga","d:/env/officedn.tga","d:/env/officebk.tga","d:/env/officeft.tga");



			_texture = imageSharpCubemapTexture.CreateDeviceTexture(context.Device, factory);
			_texture.Name = "SkyTexture";

			TextureView textureView = factory.CreateTextureView(new TextureViewDescription(_texture));

			_view = context.Device.ResourceFactory.CreateTextureView(_texture);


			var sampler = context.ResourceLoader.TextureSampler;
			_set = device.ResourceFactory.CreateResourceSet(new ResourceSetDescription(
				context.ResourceLoader.TextureLayout, textureView, sampler));

			//var tx = new Texture(context, _texture, TextureSampleType.Standard, _set);
			_mipsGenerated = true;



		}
	}
}
