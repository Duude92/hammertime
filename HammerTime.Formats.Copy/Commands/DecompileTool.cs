using LogicAndTrick.Oy;
using Sledge.BspEditor.Commands;
using Sledge.BspEditor.Documents;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Menu;
using Sledge.Common.Translations;
using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace HammerTime.Formats.Commands
{
	[AutoTranslate]
	[Export(typeof(ICommand))]
	//[CommandID("Tool:DecompileToolCommand")]
	[MenuItem("Tools", "", "Decompile BSP", "D")]
	internal class DecompileTool : BaseCommand
	{
		public override string Name { get; set; } = "Decompile Tool";
		public override string Details { get; set; } = "Tool to decompile BSP maps";

		protected override async Task Invoke(MapDocument document, CommandParameters parameters)
		{
			await Oy.Publish("Context:Add", new ContextInfo("BspEditor:DecompileOptions"));
		}
	}
}
