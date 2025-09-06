using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Avalonia.Controls;
using Sledge.BspEditor.Tools.Draggable;
using Sledge.BspEditor.Tools.Vertex.Selection;

namespace Sledge.BspEditor.Tools.Vertex.Tools
{
    public abstract class VertexSubtool : BaseDraggableTool
    {
        protected VertexSubtool()
        {
            Active = false;
            Title = GetName() ?? GetType().Name;
        }
		public override string ImageName => null;

		public abstract string OrderHint { get; }
        public VertexSelection Selection { get; set; }
        [Import] public VertexTool Parent { get; set; }
        
        public override System.Drawing.Image GetIcon() => null;
        public abstract Task SelectionChanged();
        public abstract Control Control { get; }
        public string Title { get; set; }

        protected void Invalidate()
        {
            Parent.Invalidate();
        }
        
        public abstract void Update();
    }
}
