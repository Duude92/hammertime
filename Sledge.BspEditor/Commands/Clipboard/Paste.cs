using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Components;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations.Selection;
using Sledge.BspEditor.Modification.Operations.Tree;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Properties;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Hotkeys;
using Sledge.Common.Shell.Menu;
using Sledge.Common.Translations;

namespace Sledge.BspEditor.Commands.Clipboard
{
	[AutoTranslate]
	[Export(typeof(ICommand))]
	[CommandID("BspEditor:Edit:Paste")]
	[DefaultHotkey("Ctrl+V")]
	[MenuItem("Edit", "", "Clipboard", "F")]
	[MenuImage(typeof(Resources), nameof(Resources.Menu_Paste))]
	public class Paste : BaseCommand
	{
		private readonly Lazy<ClipboardManager> _clipboard;
		private readonly Random _random;

		public override string Name { get; set; } = "Paste";
		public override string Details { get; set; } = "Paste the current clipboard contents";
		private MapDocument _document;

		[ImportingConstructor]
		public Paste([Import] Lazy<ClipboardManager> clipboard)
		{
			_clipboard = clipboard;
			_random = new Random();
			Oy.Subscribe<string>("BspEditor:Edit:PasteFromView",async (arg) => await PasteClipboard(arg));
		}

		protected override async Task Invoke(MapDocument document, CommandParameters parameters)
		{
			_document = document;
			await Oy.Publish("BspEditor:Viewport:Paste");
		}
		private async Task PasteClipboard(string arg)
		{
			if (_clipboard.Value.CanPaste())
			{
				// Work out a random offset to offset duplicate ids
				var step = Vector3.One * 16;

				// If there's a grid, use the grid spacing instead of the box dimensions
				var grid = _document.Map.Data.GetOne<GridData>();
				if (grid?.Grid != null && grid.Grid.Spacing > 1)
				{
					step = grid.Grid.AddStep(Vector3.Zero, Vector3.One);
				}
				//(((document.Control as MapDocumentControlHost).ActiveControl as MapDocumentContainer).ActiveControl as  Sledge.Rendering.Viewports.Viewport)
				// Get the pasted values, moving objects that have an id already in the map
				//var content = _clipboard.Value.GetPastedContent(document, (d, o) => CopyAndMove(d, o, step)).ToList();
				//var moveLock = parameters.Get<string>("AxisLock", null);
				var moveLock = arg;

				var translation = Matrix4x4.CreateTranslation(_random.Next(-4, 5) * step.X * (moveLock == "X" ? 0 : 1), _random.Next(-4, 5) * step.Y * (moveLock == "Y" ? 0 : 1), 0);
				//without random moving

				var content = _clipboard.Value.GetPastedContent(_document, (d, o) => CopyAndMove(d, o, translation)).ToList();

				var transaction = new Transaction(
					new Deselect(_document.Selection),
					new Attach(_document.Map.Root.ID, content),
					new Select(content)
				);

				await MapDocumentOperation.Perform(_document, transaction);
			}
		}
		private IMapObject Copy(MapDocument document, IMapObject o)
		{
			return (IMapObject)o.Copy(document.Map.NumberGenerator);
		}


		private IMapObject CopyAndMove(MapDocument document, IMapObject o, Vector3 step)
		{
			var copy = Copy(document, o);
			copy.Transform(Matrix4x4.CreateTranslation(_random.Next(-4, 5) * step.X, _random.Next(-4, 5) * step.Y, _random.Next(-4, 5) * step.Z));
			return copy;
		}
		private IMapObject CopyAndMove(MapDocument document, IMapObject o, Matrix4x4 translation)
		{
			var copy = Copy(document, o);
			copy.Transform(translation);
			return copy;
		}
	}
}