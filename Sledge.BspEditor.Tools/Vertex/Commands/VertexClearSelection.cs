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
	[CommandID("BspEditor:VertexDeselectAll")]
	[DefaultHotkey("Esc")]
	public class VertexClearSelection : ICommand
	{
		public string Name => "Clear selection";

		public string Details => "Clear current selection";

		public Task Invoke(IContext context, CommandParameters parameters)
		{
			Oy.Publish("VertexTool:DeselectAll", string.Empty);

			return Task.CompletedTask;
		}

		public bool IsInContext(IContext context)
		{
			return context.TryGet("ActiveDocument", out MapDocument _);
		}
	}
}
