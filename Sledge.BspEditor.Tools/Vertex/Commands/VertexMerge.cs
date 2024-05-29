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
	[CommandID("BspEditor:VertexMerge")]
	[DefaultHotkey("Ctrl+M")]
	public class VertexMerge : ICommand
	{
		public string Name => "Merge vertices";

		public string Details => "Merge selected vertices";

		public Task Invoke(IContext context, CommandParameters parameters)
		{
			Oy.Publish("VertexPointTool:Merge", string.Empty);

			return Task.CompletedTask;
		}

		public bool IsInContext(IContext context)
		{
			return context.TryGet("ActiveDocument", out MapDocument _);
		}
	}
}
