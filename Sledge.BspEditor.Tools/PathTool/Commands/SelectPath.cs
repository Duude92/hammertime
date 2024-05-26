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
	[CommandID("BspEditor:PathToolSelectPath")]
	internal class SelectPath : ICommand
	{
		public string Name => "Select Path";

		public string Details => "Select Path of chosen Node";

		public Task Invoke(IContext context, CommandParameters parameters)
		{
			var path = parameters.Get<IEnumerable<PathState>>("SyncRoot").FirstOrDefault();
            foreach (var handle in path.Handles)
            {
				handle.IsSelected = true;
            }
			return Task.CompletedTask;
        }

		public bool IsInContext(IContext context)
		{
			return context.TryGet("ActiveDocument", out MapDocument _);
		}
	}
}
