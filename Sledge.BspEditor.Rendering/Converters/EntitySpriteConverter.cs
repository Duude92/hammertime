using System.ComponentModel.Composition;
using System.Numerics;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Rendering.ChangeHandlers;
using Sledge.BspEditor.Rendering.Resources;
using Sledge.DataStructures.Geometric;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Engine;
using Sledge.Rendering.Pipelines;
using Sledge.Rendering.Primitives;
using Sledge.Rendering.Resources;

namespace Sledge.BspEditor.Rendering.Converters
{
    [Export(typeof(IMapObjectSceneConverter))]
    public class EntitySpriteConverter : IMapObjectSceneConverter
    {
        [Import] private EngineInterface _engine;

        public MapObjectSceneConverterPriority Priority => MapObjectSceneConverterPriority.DefaultLow;

        public bool ShouldStopProcessing(MapDocument document, IMapObject obj)
        {
            return false;
        }

        public bool Supports(IMapObject obj)
        {
            return obj is Entity e && GetSpriteData(e) != null;
        }

        private EntitySprite GetSpriteData(Entity e)
        {
            var es = e.Data.GetOne<EntitySprite>();
            return es != null && es.ContentsReplaced ? es : null;
        }

        public async Task Convert(BufferBuilder builder, MapDocument document, IMapObject obj, ResourceCollector resourceCollector)
        {
            var entity = (Entity) obj;
			var sd = GetSpriteData(entity);
            if (sd == null || !sd.ContentsReplaced) return;

            if (sd.ContentsReplaced && sd.Renderable != null)
            {
                resourceCollector.AddRenderables(new[] { sd.Renderable });
            }
        }
    }
}