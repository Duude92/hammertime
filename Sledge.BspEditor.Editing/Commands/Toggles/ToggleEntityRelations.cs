using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Sledge.BspEditor.Commands;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Editing.Properties;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Menu;
using Sledge.Common.Translations;

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

        public Task Invoke(IContext context, CommandParameters parameters)
        {
            if (context.TryGet("ActiveDocument", out MapDocument document))
            {
                var tl = document.Map.Data.GetOne<DisplayFlags>() ?? new DisplayFlags();
                tl.ToggleEntityRelations = !tl.ToggleEntityRelations;
                document.Map.Data.Replace(tl);
            }

            return Task.CompletedTask;
        }

        public bool IsInContext(IContext context)
        {
            return context.TryGet("ActiveDocument", out MapDocument doc);
        }
    }
}