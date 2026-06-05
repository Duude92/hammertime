using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Editing.Properties;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Menu;
using Sledge.Common.Translations;
using Sledge.BspEditor.Primitives.MapObjects;
using System.Linq;

namespace Sledge.BspEditor.Editing.Commands.Toggles
{
    [AutoTranslate]
    [MenuItem("View", "", "Rendering", "X")]
    //TODO: add new icon
    [MenuImage(typeof(Resources), nameof(Resources.Menu_TextureLock))]

    [Export(typeof(ICommand))]
    [CommandID("BspEditor:View:ToggleEntityRelations")]
    public class ToggleEntityRelations : ICommand, IMenuItemExtendedProperties
    {
        public string Name => "Toggle entity relations";

        public string Details => "Enable or disable entity relations drawing";

        public bool IsToggle => true;

        public bool GetToggleState(IContext context)
        {
            if (context.TryGet("ActiveDocument", out MapDocument document))
            {
                var tl = document.Map.Data.GetOne<DisplayFlags>() ?? new DisplayFlags();
                return tl.ToggleEntityRelations;
            }
            return true;
        }

        public async Task Invoke(IContext context, CommandParameters parameters)
        {
            if (!context.TryGet("ActiveDocument", out MapDocument document))
            {
                return;
            }
            var tl = document.Map.Data.GetOne<DisplayFlags>() ?? new DisplayFlags();
            tl.ToggleEntityRelations = !tl.ToggleEntityRelations;
            document.Map.Data.Replace(tl);
            await MapDocumentOperation.Perform(document,
                new TrivialOperation(
                    x => x.Map.Data.Replace(tl),
                    x =>
                    {
                        x.Update(tl);
                        x.UpdateRange(x.Document.Map.Root.Find(s => s is Entity).Select(e => e as Entity).Where(e => e.Relations.Any()));
                    })
            );
        }

        public bool IsInContext(IContext context)
        {
            return context.TryGet("ActiveDocument", out MapDocument doc);
        }
    }
}