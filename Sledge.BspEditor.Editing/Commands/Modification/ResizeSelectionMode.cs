using LogicAndTrick.Oy;
using Sledge.BspEditor.Commands;
using Sledge.BspEditor.Documents;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Hotkeys;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace Sledge.BspEditor.Editing.Commands.Modification
{
	[Export(typeof(ICommand))]
	[CommandID("BspEditor:Tools:Resize")]
	[DefaultHotkey("T")]
	public class ResizeSelectionMode : BaseCommand
	{
		public override string Name { get; set; } = "Resize";
		public override string Details { get; set; } = "Switch to Resize mode";

		protected override bool IsInContext(IContext context, MapDocument document)
		{
			return base.IsInContext(context, document) && !document.Selection.IsEmpty;
		}

		protected override async Task Invoke(MapDocument document, CommandParameters parameters)
		{
			if (parameters.Count == 0)
			{
				await Oy.Publish("SelectTool:TransformationModeChanged", "Resize");
				return;
			}
		}
	}
}
