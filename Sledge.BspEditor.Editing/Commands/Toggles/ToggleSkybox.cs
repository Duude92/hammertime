﻿using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Context;

namespace Sledge.BspEditor.Editing.Commands.Toggles
{
	[Export(typeof(ICommand))]
	[CommandID("BspEditor:Map:ToggleSkybox")]
	public class ToggleSkybox : ICommand
	{
		public string Name { get; set; } = "Toggle skybox";
		public string Details { get; set; } = "Toggle skybox";

		public bool IsInContext(IContext context)
		{
			return true;
		}

		public Task Invoke(IContext context, CommandParameters parameters)
		{
			if (context.TryGet("ActiveDocument", out MapDocument document))
			{

				var tl = document.Map.Data.GetOne<DisplayFlags>() ?? new DisplayFlags();
				tl.ToggleSkybox = !tl.ToggleSkybox;

				document.Map.Data.Replace<DisplayFlags>(tl);
				var data = document.Map.Root.Data.Get<EntityData>().First();
				var skyname = data?.Get<string>("skyname", null);

			}

			return Task.CompletedTask;
		}
	}
}
