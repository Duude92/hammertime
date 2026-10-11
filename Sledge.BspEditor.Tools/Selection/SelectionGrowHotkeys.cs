using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Modification.Operations.Selection;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Documents;
using Sledge.Common.Shell.Hotkeys;

namespace Sledge.BspEditor.Tools.Selection
{
    [Export(typeof(IHotkeyProvider))]
    public sealed class SelectionGrowHotkeyProvider : IHotkeyProvider
    {
        public string OrderHint => "Z";

        public IEnumerable<IHotkey> GetHotkeys()
        {
            yield return new SelectMoreHotkey();
            yield return new SelectLessHotkey();
        }
    }

    internal sealed class SelectMoreHotkey : IHotkey
    {
        public string ID => "Selection:SelectMore";
        public string Name => "Select More";
        public string Description => "Grow the selection to objects that share a vertex position";
        public string DefaultHotkey => "Ctrl+Add";

        public bool IsInContext(IContext context)
        {
            var document = GetActiveDocument(context);
            return document != null && !document.Selection.IsEmpty;
        }

        public Task Invoke()
        {
            var document = GetActiveDocument(null);
            var tool = Sledge.Common.Container.Get<IContext>()?.Get<ITool>("ActiveTool");
            if (document != null && tool is SelectTool selectTool)
            {
                selectTool.SelectMore(document);
            }

            return Task.CompletedTask;
        }

        private static MapDocument GetActiveDocument(IContext context)
        {
            context = context ?? Sledge.Common.Container.Get<IContext>();
            return context?.Get<IDocument>("ActiveDocument") as MapDocument;
        }
    }

    internal sealed class SelectLessHotkey : IHotkey
    {
        public string ID => "Selection:SelectLess";
        public string Name => "Select Less";
        public string Description => "Shrink the selection by peeling off the outermost shared-vertex layer";
        public string DefaultHotkey => "Ctrl+Subtract";

        public bool IsInContext(IContext context)
        {
            var document = GetActiveDocument(context);
            return document != null && !document.Selection.IsEmpty;
        }

        public Task Invoke()
        {
            var document = GetActiveDocument(null);
            var tool = Sledge.Common.Container.Get<IContext>()?.Get<ITool>("ActiveTool");
            if (document != null && tool is SelectTool selectTool)
            {
                selectTool.SelectLess(document);
            }

            return Task.CompletedTask;
        }

        private static MapDocument GetActiveDocument(IContext context)
        {
            context = context ?? Sledge.Common.Container.Get<IContext>();
            return context?.Get<IDocument>("ActiveDocument") as MapDocument;
        }
    }
}
