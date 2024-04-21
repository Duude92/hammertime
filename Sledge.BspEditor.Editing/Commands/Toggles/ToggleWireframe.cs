using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Sledge.BspEditor.Commands;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Editing.Properties;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations;
using Sledge.BspEditor.Modification.Operations.Data;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Menu;
using Sledge.Common.Translations;

namespace Sledge.BspEditor.Editing.Commands.Toggles
{
	[Export(typeof(ICommand))]
	[CommandID("BspEditor:Map:ToggleWireframe")]
	public class ToggleWireframe : BaseCommand
	{
		public override string Name { get; set; } = "Toggle wireframe";
		public override string Details { get; set; } = "Toggle wireframe for every surface";

		protected async override Task Invoke(MapDocument document, CommandParameters parameters)
		{
			var tl = document.Map.Data.GetOne<DisplayFlags>() ?? new DisplayFlags();
			tl.Wireframe = !tl.Wireframe;

			document.Map.Data.Replace<DisplayFlags>(tl);
		}
	}
}
