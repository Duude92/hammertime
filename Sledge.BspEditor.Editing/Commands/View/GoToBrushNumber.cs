using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Commands;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Editing.Properties;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations.Selection;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Menu;
using Sledge.Common.Translations;
using Sledge.QuickForms;

namespace Sledge.BspEditor.Editing.Commands.View
{
	[AutoTranslate]
	[Export(typeof(ICommand))]
	[CommandID("BspEditor:View:GoToBrushNumber")]
	[MenuItem("View", "", "GoTo", "B")]
	[MenuImage(typeof(Resources), nameof(Resources.Menu_GoToBrushEntityID))]
	public class GoToBrushNumber : BaseCommand
	{
		public override string Name { get; set; } = "Go to brush ID";
		public override string Details { get; set; } = "Select and center views on a specific object ID.";

		public string Title { get; set; }
		public string EntityID { get; set; }
		public string BrushID { get; set; }
		public string OK { get; set; }
		public string Cancel { get; set; }

		protected override async Task Invoke(MapDocument document, CommandParameters parameters)
		{
			using (var qf = new QuickForm(Title) { UseShortcutKeys = true }.TextBox("EntityID", EntityID).TextBox("BrushID", BrushID).OkCancel(OK, Cancel))
			{
				qf.ClientSize = new Size(230, qf.ClientSize.Height);

				if (await qf.ShowDialogAsync() != DialogResult.OK) return;

				IMapObject obja = null;

				if (int.TryParse(qf.String("EntityID"), out var entityId))
				{
					var entityObjs = new List<Entity>();
					CollectEntities(entityObjs, document.Map.Root);
					void CollectEntities(List<Entity> entities, IMapObject parent)
					{
						foreach (var obj1 in parent.Hierarchy)
						{
							if (obj1 is Entity e) entities.Add(e);
							else if (obj1 is Group) CollectEntities(entities, obj1);
						}
					}
					if (entityId < entityObjs.Count) obja = entityObjs[entityId];
				}
				if (int.TryParse(qf.String("BrushID"), out var brushId))
				{
					var solidObjs = new List<Solid>();

					CollectSolids(solidObjs, document.Map.Root);

					void CollectSolids(List<Solid> solids, IMapObject parent)
					{
						foreach (var obj1 in parent.Hierarchy)
						{
							if (obj1 is Solid s) solids.Add(s);
							else if (obj1 is Group) CollectSolids(solids, obj1);
						}
					}
					if (brushId < solidObjs.Count) obja = solidObjs[brushId];
				}
				if (obja == null) return;
				var tran = new Transaction(
					new Deselect(document.Selection),
					new Select(obja)
				);

				await MapDocumentOperation.Perform(document, tran);

				var box = obja.BoundingBox;

				await Task.WhenAll(
					Oy.Publish("MapDocument:Viewport:Focus3D", box),
					Oy.Publish("MapDocument:Viewport:Focus2D", box)
				);
			}
		}
	}
}