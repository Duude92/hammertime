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
		private VtfFile _vtfFile;
		private IFile _file;

		public VtfTextureStreamSourceProvider(IFile file)
		{
			_file = file;
			if (file != null)
				_vtfFile = new VtfFile(file.Open());
		}
		public void Dispose()
		{
			_vtfFile = null;
		}

		public async Task<ICollection<Bitmap>> GetImage(string item, int maxWidth, int maxHeight)
		{
			return await Task.Factory.StartNew(() => new List<Bitmap>(){ _vtfFile.Images.Select(x =>
				{
					var data = x.GetBgra32Data();
					Bitmap bmp = new Bitmap(x.Width, x.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
					BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
					Marshal.Copy(data, 0, bmpData.Scan0, data.Length);
					bmp.UnlockBits(bmpData);

					return  bmp ;
				}).Last() });
		}

		public bool HasImage(string item)
		{
			return _file.NameWithoutExtension.Equals(item, StringComparison.InvariantCultureIgnoreCase);
		}
	}

}
