﻿using System.Numerics;
using System.Runtime.InteropServices;

namespace Sledge.Rendering.Primitives
{
    [StructLayout(LayoutKind.Sequential)]
    public struct VertexModel3
    {
        /// <summary>The position of the vertex</summary>
        public Vector3 Position;

        /// <summary>The normal of the vertex</summary>
        public Vector3 Normal;

        /// <summary>The texture coordinates of the vertex</summary>
        public Vector3 Texture;

        /// <summary>The bone weightings of the vertex</summary>
        public uint Bone;

		/// <summary>The flags of the vertex</summary>
		public VertexFlags Flags;

        /// <summary>The size of this structure in bytes</summary>
        public const int SizeInBytes = (3 + 3 + 3 + 1 + 1) * 4;
    }
}