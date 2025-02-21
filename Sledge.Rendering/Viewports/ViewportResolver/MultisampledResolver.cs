using Veldrid;

namespace Sledge.Rendering.Viewports.ViewportResolver
{
	internal class MultisampledResolver : IViewportResolver
	{
		public void ResolveTexture(CommandList commandList, Texture src, Texture dst)
		{
			commandList.ResolveTexture(src, dst);
		}
	}
}
