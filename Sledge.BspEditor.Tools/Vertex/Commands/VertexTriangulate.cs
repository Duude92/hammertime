using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Hotkeys;
using Sledge.Common.Translations;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sledge.BspEditor.Tools.Vertex.Commands
{
	[AutoTranslate]
	[Export(typeof(ICommand))]
	[CommandID("BspEditor:VertexTriangulate")]
	[DefaultHotkey("Alt+J")]
	internal class VertexTriangulate : ICommand
	{
		public string Name => "Triangulate mesh";

		public string Details => "Triangulate selected mesh";

		public async Task Invoke(IContext context, CommandParameters parameters)
		{
			await Oy.Publish("VertexTool:Triangulate");
		}

		public bool IsInContext(IContext context)
		{
			return context.TryGet("ActiveDocument", out MapDocument _);
		}
	}
}
