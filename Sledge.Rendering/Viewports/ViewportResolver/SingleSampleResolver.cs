using Veldrid;

namespace Sledge.Rendering.Viewports.ViewportResolver
{
	internal class SingleSampleResolver : IViewportResolver
	{
		public void ResolveTexture(CommandList commandList, Texture src, Texture dst)
		{
			commandList.CopyTexture(src, dst);
		}
	}
}
