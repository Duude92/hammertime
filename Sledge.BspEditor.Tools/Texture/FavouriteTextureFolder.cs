using System.Collections.Generic;
namespace Sledge.BspEditor.Tools.Texture
{
	public class FavouriteTextureFolder
	{
		public List<string> Items { get; set; } = new();
		public List<FavouriteTextureFolder> Children { get; set; } = new();
		public string Name { get; set; }
	}
}
