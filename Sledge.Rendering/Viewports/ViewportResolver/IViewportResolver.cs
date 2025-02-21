using Veldrid;

namespace Sledge.Rendering.Viewports.ViewportResolver
{
	internal interface IViewportResolver
	{
		void ResolveTexture(CommandList commandList, Texture src, Texture dst);
	}
}
