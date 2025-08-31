using Sledge.FileSystem;
using Sledge.Formats.Texture.Vtf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Sledge.Providers.Texture.Vmt
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
				if (matRef.File == null) throw new NullReferenceException();
				var vtfFile = new VtfFile(matRef?.File?.Open());
				return new List<Bitmap>(){
				vtfFile.Images.Select(x =>
				{
					var data = x.GetBgra32Data();
					Bitmap bmp = new Bitmap(x.Width, x.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
					BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);
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
