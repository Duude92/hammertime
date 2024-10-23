using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Environment.Goldsource;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Context;
using Sledge.Rendering.Resources;

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
			if (context.TryGet("ActiveDocument", out MapDocument document) && document.Environment is GoldsourceEnvironment environment)
			{

				var tl = document.Map.Data.GetOne<DisplayFlags>() ?? new DisplayFlags();
				var dd = document.Map.Data.GetOne<DisplayData>() ?? new DisplayData();

				var data = document.Map.Root.Data.Get<EntityData>().First();
				var skyname = data?.Get<string>("skyname", null);


				var sky = environment.GetSkyboxes().FirstOrDefault(x => x.Name == skyname);
				if (sky == null) return Task.CompletedTask;

				tl.ToggleSkybox = !tl.ToggleSkybox;

				dd.SkyboxName = skyname;




				document.Map.Data.Replace(dd);
				document.Map.Data.Replace(tl);
			}

			return Task.CompletedTask;
		}
	}
}
