using System.IO;
using Sledge.BspEditor.Tools.Properties;
using Avalonia.Input;
using Avalonia.Media.Imaging;

namespace Sledge.BspEditor.Tools
{
	public static class ToolCursors
	{
		public static Cursor RotateCursor { get; }

		static ToolCursors()
		{
			RotateCursor = new Cursor(new Bitmap(new MemoryStream(Resources.Cursor_Rotate)), Avalonia.PixelPoint.Origin);
		}
	}
}
