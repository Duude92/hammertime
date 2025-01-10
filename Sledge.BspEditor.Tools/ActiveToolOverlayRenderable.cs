using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Rendering.Dynamic;
using Sledge.BspEditor.Rendering.Resources;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Hooks;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Overlay;
using Sledge.Rendering.Resources;
using Sledge.Rendering.Viewports;
using Sledge.Shell.Registers;

namespace Sledge.BspEditor.Tools
{
    [Export(typeof(IOverlayRenderable))]
    [Export(typeof(IDynamicRenderable))]
    [Export(typeof(IStartupHook))]
    public class ActiveToolRenderable : IOverlayRenderable, IDynamicRenderable, IStartupHook
    {
        private readonly WeakReference<BaseTool> _activeTool = new WeakReference<BaseTool>(null);
        private BaseTool ActiveTool => _activeTool.TryGetTarget(out var t) ? t : null;
		[ImportMany] private IEnumerable<Lazy<ITool>> _tools;
        private IEnumerable<BaseTool> _components;


		public Task OnStartup()
        {
            Oy.Subscribe<ITool>("Tool:Activated", ToolActivated);
            _components = _tools.Where(t => t.Value is BaseTool).Select(t => t.Value as BaseTool);
			return Task.CompletedTask;
        }

        private Task ToolActivated(ITool tool)
        {
            _activeTool.SetTarget(tool as BaseTool);
            return Task.CompletedTask;
        }

        public void Render(BufferBuilder builder, ResourceCollector resourceCollector)
        {
            foreach(var tool in _components)
            {
                if(tool.RenderedByDefault || tool==ActiveTool)
                {
					tool?.Render(builder, resourceCollector);

				}
			}
            //ActiveTool?.Render(builder, resourceCollector);
        }

        public void Render(IViewport viewport, OrthographicCamera camera, Vector3 worldMin, Vector3 worldMax, I2DRenderer im)
        {
			foreach (var tool in _components)
			{
				if (tool.RenderedByDefault || tool == ActiveTool)
				{

			tool?.Render(viewport, camera, worldMin, worldMax, im);
				}
			}
        }

        public void Render(IViewport viewport, PerspectiveCamera camera, I2DRenderer im)
        {
			foreach (var tool in _components)
			{
				if (tool.RenderedByDefault || tool == ActiveTool)
				{

					tool?.Render(viewport, camera, im);
				}
			}
        }
    }
}
