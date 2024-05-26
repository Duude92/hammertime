using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Tools.Draggable;
using Sledge.BspEditor.Tools.PathTool.Forms;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Hotkeys;
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
	[CommandID("BspEditor:PathProperties")]
	[DefaultHotkey("Alt+Enter")]

	internal class OpenPathProperties : ICommand
	{
		public string Name => "Open Path properties";

		public string Details => "Open properties of Path of selected Node";

		public Task Invoke(IContext context, CommandParameters parameters)
		{
			var path = parameters.Get<IEnumerable<PathState>>("SyncRoot").FirstOrDefault();
			if(path == null) return Task.CompletedTask;
			PathProperties dialog = new PathProperties(path);
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
