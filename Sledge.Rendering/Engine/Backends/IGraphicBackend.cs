using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
	}
}
