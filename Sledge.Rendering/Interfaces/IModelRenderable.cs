using System.Numerics;
using Sledge.Rendering.Primitives;
using Sledge.Rendering.Renderables;
using Sledge.Rendering.Resources;

namespace Sledge.Rendering.Interfaces
{
    public interface IModelRenderable : IRenderable, IUpdateable, IResource
    {
        IModel Model { get; }
        Vector3 Origin { get; set; }
        Vector3 Angles { get; set; }
        int Sequence { get; set; }
        int SkinId { get; set; }
        int BodyGroup { get; set; }


		Matrix4x4 GetModelTransformation();
        (Vector3, Vector3) GetBoundingBox();
        VertexFlags Flags { get; set; }
	}
}