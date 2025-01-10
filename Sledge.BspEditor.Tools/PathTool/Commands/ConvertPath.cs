using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations.Tree;
using Sledge.BspEditor.Tools.Draggable;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Context;
using Sledge.Common.Translations;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;

namespace Sledge.BspEditor.Tools.PathTool.Commands
{
	[AutoTranslate]
	[Export(typeof(ICommand))]
	[CommandID("BspEditor:PathToolConvertPath")]
	internal class ConvertPath : ICommand
	{
		public string Name => "Convert Path";

		public string Details => "Convert Path into Entity-based path";

		public async Task Invoke(IContext context, CommandParameters parameters)
		{
			context.TryGet<MapDocument>("ActiveDocument", out var document);
			var path = parameters.Get<IEnumerable<PathState>>("SyncRoot").FirstOrDefault();
			var entities = path.ToMapObject(document);
			var transaction = new Transaction(new Attach(document.Map.Root.ID, entities));

			await MapDocumentOperation.Perform(document, transaction);
		}

		public bool IsInContext(IContext context)
		{
			return context.TryGet("ActiveDocument", out MapDocument _);
		}
	}
}
