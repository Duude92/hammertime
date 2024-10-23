using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Sledge.BspEditor.Commands;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.Common.Shell.Commands;

namespace Sledge.BspEditor.Editing.Commands.Toggles
{
	[Export(typeof(ICommand))]
	[CommandID("BspEditor:Map:ToggleWireframe")]
	public class ToggleWireframe : BaseCommand
	{
		public override string Name { get; set; } = "Toggle wireframe";
		public override string Details { get; set; } = "Toggle wireframe for every surface";

		protected override Task Invoke(MapDocument document, CommandParameters parameters)
		{
			var tl = document.Map.Data.GetOne<DisplayFlags>() ?? new DisplayFlags();
			tl.Wireframe = !tl.Wireframe;

			document.Map.Data.Replace(tl);

			return Task.CompletedTask;
		}
	}
}
