using System;
using Veldrid;

namespace Sledge.Rendering.Viewports
{
    public interface IRenderTarget : IDisposable
    {
        bool ShouldRender(long frame);
        Veldrid.Viewport GetViewport();
	}
}