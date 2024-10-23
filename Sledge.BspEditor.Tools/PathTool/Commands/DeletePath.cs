using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Tools.Draggable;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Context;
using Sledge.Common.Translations;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;

namespace Sledge.BspEditor.Tools.PathTool.Commands
{
	[AutoTranslate]
	[Export(typeof(ICommand))]
	[CommandID("BspEditor:PathToolDeletePath")]
	internal class DeletePath : ICommand
	{
		public string Name => "Delete Path";

		public string Details => "Delete chosen Path";

		public Task Invoke(IContext context, CommandParameters parameters)
		{
			var path = parameters.Get<IEnumerable<PathState>>("SyncRoot").FirstOrDefault();
			path.Clear();
			return Task.CompletedTask;
		}

		public bool IsInContext(IContext context)
		{
			return context.TryGet("ActiveDocument", out MapDocument _);
		}
	}
}
