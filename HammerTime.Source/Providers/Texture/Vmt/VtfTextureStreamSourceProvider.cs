using Sledge.Formats.Texture.Vtf;
using Sledge.Providers.Texture;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace HammerTime.Source.Providers.Texture.Vmt
{
	public class VtfTextureStreamSourceProvider : ITextureStreamSource
	{
		private VmtMaterialPackage _package;

		internal VtfTextureStreamSourceProvider(VmtMaterialPackage package)
		{
			_package = package;
		}
		public void Dispose()
		{
			_package = null;
		}

		public async Task<ICollection<Bitmap>> GetImage(string item, int maxWidth, int maxHeight)
		{
			return await Task.Factory.StartNew(() =>
			{
				var matRef = _package.GetTextureReference(item);

				var alphaVal = matRef.Material.GetFloat("$alpha");
				var alpha = false;
				if (alphaVal == 0)
				{
					alphaVal = matRef.Material.GetFloat("$alphatest");
				}

				alpha = alphaVal > 0;

				if (matRef.File == null) throw new NullReferenceException();
				var vtfFile = new VtfFile(matRef?.File?.Open());
				return new List<Bitmap>(){
				vtfFile.Images.Select(x =>
				{
					var data = x.GetBgra32Data();
					// Fixme: Should always be argb and controlled by shader?
					var pixelFormat = alpha? PixelFormat.Format32bppArgb: PixelFormat.Format32bppRgb;
					Bitmap bmp = new Bitmap(x.Width, x.Height, pixelFormat);
					BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, pixelFormat);
					Marshal.Copy(data, 0, bmpData.Scan0, data.Length);
					bmp.UnlockBits(bmpData);

					return  bmp ;
				}).Last() };
			});
		}

		public bool HasImage(string item)
		{
			return _package.HasTexture(item) && _package.GetTextureReference(item).File != null;
		}
	}

}
