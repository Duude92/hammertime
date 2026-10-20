using System.Windows.Forms;
using Veldrid;

namespace Sledge.Rendering.Engine.Backends
{
	internal interface IGraphicBackend
	{
		public VertexLayoutDescription VertexStandardLayoutDescription { get; }
		public VertexLayoutDescription VertexModel3LayoutDescription { get; }
		public VertexLayoutDescription ImGUILayoutDescription { get; }
		public (Shader, Shader) LoadShaders(string name);
		public (Shader, Shader, Shader) LoadShadersGeometry(string name);
		public RasterizerStateDescription RasterizerStateDescription { get; }
		public Swapchain CreateSwapchain(Control control, GraphicsDeviceOptions options);
		public void SetScissors(CommandList cl, Viewport viewport);
	}
}
