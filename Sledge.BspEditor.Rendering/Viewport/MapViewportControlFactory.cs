using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Sledge.BspEditor.Components;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.Common.Translations;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Engine;

namespace Sledge.BspEditor.Rendering.Viewport
{
	[Export(typeof(IMapDocumentControlFactory))]
	[AutoTranslate]
	public class MapViewportControlFactory : IMapDocumentControlFactory
	{
		private readonly IEnumerable<Lazy<IViewportEventListenerFactory>> _viewportEventListenerFactories;
		private readonly Lazy<EngineInterface> _engine;

		public string Perspective { get; set; }
		public string OrthographicTop { get; set; }
		public string OrthographicFront { get; set; }
		public string OrthographicSide { get; set; }
		public string PerspectiveWf { get; set; }
		public string PerspectiveSky { get; set; }

		public string Type => "MapViewport";

		[ImportingConstructor]
		public MapViewportControlFactory(
			[ImportMany] IEnumerable<Lazy<IViewportEventListenerFactory>> viewportEventListenerFactories,
			[Import] Lazy<EngineInterface> engine
		)
		{
			_viewportEventListenerFactories = viewportEventListenerFactories;
			_engine = engine;
		}

		public IMapDocumentControl Create()
		{
			return new ViewportMapDocumentControl(_engine.Value, _viewportEventListenerFactories.Select(x => x.Value));
		}

		public bool IsType(IMapDocumentControl control)
		{
			return control is ViewportMapDocumentControl;
		}

		public Dictionary<string, string> GetStyles()
		{
			return new Dictionary<string, string>
			{

				{"PerspectiveCamera/View", Perspective},
				{"PerspectiveCamera/Wireframe", PerspectiveWf},
				{"PerspectiveCamera/Skybox", PerspectiveSky },
				{"OrthographicCamera/Top", OrthographicTop},
				{"OrthographicCamera/Front", OrthographicFront},
				{"OrthographicCamera/Side", OrthographicSide}
			};
		}

		public bool IsStyle(IMapDocumentControl control, string style, DisplayFlags displayFlags)
		{
			if (control is not ViewportMapDocumentControl vpControl) return false;
			if(vpControl.Camera is PerspectiveCamera camera)
			{
				if(style == "PerspectiveCamera/View") return true;
				var option = style.Split('/');
				if(option.Length < 2) return false;
				switch(option[1])
				{
					case "Wireframe":
						return displayFlags.Wireframe;
					case "Skybox":
						return displayFlags.ToggleSkybox;
				}
			}
			return control.GetSerialisedSettings().StartsWith(style);
		}
	}
}