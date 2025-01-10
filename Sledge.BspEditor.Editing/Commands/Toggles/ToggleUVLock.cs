using Sledge.BspEditor.Commands;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Editing.Properties;
using Sledge.BspEditor.Modification.Operations;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Menu;
using Sledge.Common.Translations;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace Sledge.BspEditor.Editing.Commands.Toggles
{
	[AutoTranslate]
	[Export(typeof(ICommand))]
	[CommandID("BspEditor:Map:ToggleUVLock")]
	[MenuItem("Map", "", "Texture", "U")]
	[MenuImage(typeof(Resources), nameof(Resources.Menu_UVLock))]
	public class ToggleUVLock : BaseCommand, IMenuItemExtendedProperties
	{
		public bool IsToggle => true;

		public override string Name { get; set; } = "UV Lock";
		public override string Details { get; set; } = "Toggle UV locking.";

		protected override async Task Invoke(MapDocument document, CommandParameters parameters)
		{
			var tl = document.Map.Data.GetOne<TransformationFlags>() ?? new TransformationFlags();
			tl.UVLock = !tl.UVLock;

			await MapDocumentOperation.Perform(document, new TrivialOperation(x => x.Map.Data.Replace(tl), x => x.Update(tl)));
		}

		public bool GetToggleState(IContext context)
		{
			if (!context.TryGet("ActiveDocument", out MapDocument doc)) return false;
			var tf = doc.Map.Data.GetOne<TransformationFlags>() ?? new TransformationFlags();
			return tf.UVLock;
		}

	}
}
