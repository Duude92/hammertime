using LogicAndTrick.Oy;
using Sledge.BspEditor.Commands;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Editing.Properties;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Hotkeys;
using Sledge.Common.Shell.Menu;
using Sledge.Common.Translations;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sledge.BspEditor.Editing.Commands
{
	[AutoTranslate]
	[Export(typeof(ICommand))]
	[MenuItem("File", "", "Build", "BA")]
	[MenuImage(typeof(Resources), nameof(Resources.Menu_Interrupt))]
	[CommandID("BspEditor:Compile:Interrupt")]
	[DefaultHotkey("Ctrl+Cancel")]
	public class InterruptCompiling : BaseCommand
	{
		public override string Name { get; set; } =  "Interrupt compilation";
		public override string Details { get; set; } = "Interrupt compilation";

		protected override async Task Invoke(MapDocument document, CommandParameters parameters)
		{
			await Oy.Publish("Compile:Interrupt");
		}
	}
}
