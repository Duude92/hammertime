using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Tools.Draggable;
using Sledge.BspEditor.Tools.PathTool.Forms;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Context;
using Sledge.Common.Translations;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sledge.BspEditor.Tools.PathTool.Commands
{
	[AutoTranslate]
	[Export(typeof(ICommand))]
	[CommandID("BspEditor:NodeProperties")]
	internal class OpenNodeProperties : ICommand
	{
		public string Name => "Node properties";

		public string Details => "Open chosen Node properties";

		public Task Invoke(IContext context, CommandParameters parameters)
		{
			var nodeHandle = parameters.Get<IEnumerable<PathNodeHandle>>("SyncRoot").FirstOrDefault();
			if (nodeHandle == null) return Task.CompletedTask;
			NodeProperties dialog = new NodeProperties(nodeHandle);
			var result = dialog.ShowDialog();
			if (result == DialogResult.Cancel) return Task.CompletedTask;
			return Task.CompletedTask;
		}

		public bool IsInContext(IContext context)
		{
			return context.TryGet("ActiveDocument", out MapDocument _);
		}
	}
}
