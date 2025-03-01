using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Rendering.ChangeHandlers;
using Sledge.BspEditor.Rendering.Resources;
using Sledge.Rendering.Primitives;
using Sledge.Rendering.Resources;

namespace Sledge.BspEditor.Rendering.Converters
{
    [Export(typeof(IMapObjectSceneConverter))]
    public class EntityModelConverter : IMapObjectSceneConverter
    {
        public MapObjectSceneConverterPriority Priority => MapObjectSceneConverterPriority.DefaultLow;

        public bool ShouldStopProcessing(MapDocument document, IMapObject obj)
        {
            return false;
        }

        public bool Supports(IMapObject obj)
        {
            return obj is Entity e && GetModelData(e) != null;
        }

        private EntityModel GetModelData(Entity e)
        {
            var em = e.Data.GetOne<EntityModel>();
            return em != null && em.ContentsReplaced ? em : null;
        }

        public Task Convert(BufferBuilder builder, MapDocument document, IMapObject obj, ResourceCollector resourceCollector)
        {
            var em = obj.Data.GetOne<EntityModel>();
			var flags = obj.IsSelected ? VertexFlags.SelectiveTransformed : VertexFlags.None;

			if (em.ContentsReplaced && em.Renderable != null)
            {
                em.Renderable.Flags = flags;
                resourceCollector.AddRenderables(new []{ em.Renderable });
            }

            return Task.CompletedTask;
        }
    }
}