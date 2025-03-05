using LogicAndTrick.Oy;
using Sledge.BspEditor.Commands;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Editing.Properties;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Menu;
using Sledge.Common.Translations;
using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace Sledge.BspEditor.Editing.Commands.View
{
	[AutoTranslate]
	[Export(typeof(ICommand))]
	[CommandID("BspEditor:View:Shadows")]
	[MenuItem("View", "", "Rendering", "Y")]
	[MenuImage(typeof(Resources), nameof(Resources.Menu_Shadows))]

	public class ShadowsEnableCommand : BaseCommand, IMenuItemExtendedProperties
	{
		public override string Name { get; set; } = "Enable shadows";
		public override string Details { get; set; } = "Enable shadows rendering";

		public bool IsToggle => true;

		protected override async Task Invoke(MapDocument document, CommandParameters parameters)
		{
			throw new NotImplementedException();
		}

		public bool GetToggleState(IContext context)
		{
			return IsToggle;
		}
	}
}
