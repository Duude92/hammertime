using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Context;
using Sledge.Common.Translations;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace Sledge.BspEditor.Tools.PathTool.Commands
{
	[AutoTranslate]
	[Export(typeof(ICommand))]
	[CommandID("BspEditor:PathInsertNode")]
	public class InsertNode : ICommand
	{
		public string Name => "Insert Node";

		public string Details => "Insert Node before selected";

		public Task Invoke(IContext context, CommandParameters parameters)
		{
			Oy.Publish("PathTool:InsertNode");

			return Task.CompletedTask;
		}

		public bool IsInContext(IContext context)
		{
			return context.TryGet("ActiveDocument", out MapDocument _);
		}
	}
}
