using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Tools.Draggable;
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
	[CommandID("BspEditor:PathProperties")]
	[DefaultHotkey("Alt+Enter")]

	internal class OpenPathProperties : ICommand
	{
		public string Name => "Open Path properties";

		public string Details => "Open properties of Path of selected Node";

		public async Task Invoke(IContext context, CommandParameters parameters)
		{
			var path = parameters.Get<PathState>("state");
			if (path == null) return;
			await Oy.Publish("Context:Add", new ContextInfo("PathState", path));
			await Oy.Publish("Context:Add", new ContextInfo("BspEditor:PathPropertiesShow"));

			return;
		}

		public bool IsInContext(IContext context)
		{
			return context.TryGet("ActiveDocument", out MapDocument _);
		}
	}
}
