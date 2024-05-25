using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Hotkeys;
using Sledge.Common.Translations;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace Sledge.BspEditor.Tools.PathTool.Commands
{
	[AutoTranslate]
	[Export(typeof(ICommand))]
	[CommandID("BspEditor:PathToolDeleteNodes")]
	[DefaultHotkey("Del")] //TODO: add support of using HotKeys on ActiveTool
	internal class DeleteNodes : ICommand
	{
		public string Name => "Delete Nodes";

		public string Details => "Removes selected nodes from its Path";

		public Task Invoke(IContext context, CommandParameters parameters)
		{
			Oy.Publish("PathTool:Delete");

			return Task.CompletedTask;
		}

		public bool IsInContext(IContext context)
		{
			return context.TryGet("ActiveDocument", out MapDocument _);
		}
	}
}
