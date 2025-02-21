﻿using System;
using System.Windows.Forms;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Overlay;
using Veldrid;

namespace Sledge.Rendering.Viewports
{
    public interface IViewport : IRenderTarget
    {
        int ID { get; }

        int Width { get; }
        int Height { get; }
        Control Control { get; }
        bool IsFocused { get; }

        ICamera Camera { get; set; }
        ViewportOverlay Overlay { get; }
        Framebuffer ViewportFramebuffer { get; }
        Texture ViewportResolvedTexture { get; }
		public Resources.Texture ViewportRenderTexture { get; }
		void InitFramebuffer(TextureSampleCount sampleCount);
        void ResolveRenderTexture(CommandList commandList);

		void Update(long frame);
        event EventHandler<long> OnUpdate;
    }
}