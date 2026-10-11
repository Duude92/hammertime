using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Sledge.BspEditor.Primitives;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;

namespace Sledge.BspEditor.Modification.Operations.Mutation
{
    /// <summary>
    /// Shared by the texture transform operations. After the vertices of an object have been moved, faces get
    /// their texture re-fitted so every vertex keeps the (s, t) it had (exact texture lock). The texture each face
    /// had before is remembered so undo restores it exactly instead of re-deriving it.
    /// Anything textured that isn't a face (displacements, etc.) is handed to the old per-texture transform.
    /// </summary>
    internal sealed class ExactTextureLock
    {
        private readonly Dictionary<(long Id, int Index), Texture> _before = new Dictionary<(long Id, int Index), Texture>();

        /// <summary>
        /// Call after the object's vertices were transformed by <paramref name="applied"/>.
        /// Returns true if the object has anything textured.
        /// </summary>
        public bool Apply(IMapObject obj, Matrix4x4 applied, Action<Texture> fallback)
        {
            var any = false;
            var index = 0;
            foreach (var it in obj.Data.OfType<ITextured>())
            {
                any = true;
                if (it is Face face)
                {
                    _before[(obj.ID, index)] = face.Texture.Clone();
                    UvMath.TryLockAfterTransform(face.Texture, face.Vertices.ToList(), applied);
                }
                else
                {
                    fallback(it.Texture);
                }
                index++;
            }
            return any;
        }

        /// <summary>
        /// Puts the textures back the way <see cref="Apply"/> found them.
        /// Returns true if the object has anything textured.
        /// </summary>
        public bool Undo(IMapObject obj, Action<Texture> fallbackInverse)
        {
            var any = false;
            var index = 0;
            foreach (var it in obj.Data.OfType<ITextured>())
            {
                any = true;
                if (it is Face face)
                {
                    if (_before.TryGetValue((obj.ID, index), out var saved)) face.Texture.Unclone(saved);
                }
                else
                {
                    fallbackInverse(it.Texture);
                }
                index++;
            }
            return any;
        }
    }
}
