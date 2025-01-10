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
		const int ENTITY_LIMIT = 16384;
		const int SOLID_BRUSH_LIMIT = 16384;
		public override string Name { get; set; } = "Go to brush ID";
		public override string Details { get; set; } = "Select and center views on a specific object ID.";

		public string Title { get; set; }
		public string EntityID { get; set; }
		public string BrushID { get; set; }
		public string OK { get; set; }
		public string Cancel { get; set; }

		protected override async Task Invoke(MapDocument document, CommandParameters parameters)
		{
			using (var qf = new QuickForm(Title) { UseShortcutKeys = true }.NumericUpDown("EntityID", "EntityID", 0, ENTITY_LIMIT, 0).NumericUpDown("BrushID", "BrushID", 0, SOLID_BRUSH_LIMIT, 0).OkCancel(OK, Cancel))
			{
				qf.ClientSize = new Size(230, qf.ClientSize.Height);

				if (await qf.ShowDialogAsync() != DialogResult.OK) return;

				IMapObject targetObject = null;
				IMapObject parentObject = document.Map.Root;

				int entityId = (int)qf.Decimal("EntityID") - 1;
				if (entityId > 0)
				{
					var entityObjs = new List<Entity>();
					CollectObjects<Entity>(entityObjs, document.Map.Root);
					if (entityId >= entityObjs.Count)
					{
						MessageBox.Show($"Entity ID {entityId + 1} not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						return;
					}
					parentObject = entityObjs[entityId];

				}
				var brushId = (int)qf.Decimal("BrushID");
				var solidObjs = new List<Solid>();

				CollectObjects<Solid>(solidObjs, parentObject);

				if (brushId >= solidObjs.Count)
				{
					MessageBox.Show($"Brush ID {brushId} not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
				targetObject = solidObjs[brushId];
				if (targetObject == null) return;
				var tran = new Transaction(
					new Deselect(document.Selection),
					new Select(targetObject)
				);

				await MapDocumentOperation.Perform(document, tran);

				var box = targetObject.BoundingBox;

				await Task.WhenAll(
					Oy.Publish("MapDocument:Viewport:Focus3D", box),
					Oy.Publish("MapDocument:Viewport:Focus2D", box)
				);
				void CollectObjects<T>(List<T> objects, IMapObject parent) where T: IMapObject
				{
					foreach (var obj1 in parent.Hierarchy)
					{
						if (obj1 is T e) objects.Add(e);
						else if (obj1 is Group) CollectObjects<T>(objects, obj1);
					}
				}
			}
		}
	}
}