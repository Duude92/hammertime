using LogicAndTrick.Oy;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Tools.Draggable;
using Sledge.BspEditor.Tools.Properties;
using Sledge.BspEditor.Tools.Selection;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Hotkeys;
using Sledge.Common.Shell.Settings;
using Sledge.Common.Translations;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sledge.BspEditor.Tools.WrapTexture
{
	[Export(typeof(ITool))]
	[Export]
	[OrderHint("JJ")]
	[AutoTranslate]
	public class WrapTextureTool : BaseDraggableTool
	{

		public override Image GetIcon()
		{
			return Resources.Tool_Wrap;
		}

		public IEnumerable<SettingKey> GetKeys()
		{
			return new SettingKey[0];
		}

		public override string GetName()
		{
			return "Apply current texture to selected brushes";
		}

		public void LoadValues(ISettingsStore store)
		{
			return;
		}

		public void StoreValues(ISettingsStore store)
		{
			return;
		}

		public override async Task ToolSelected()
		{
			await base.ToolSelected();

			await Oy.Publish("Command:Run", new CommandMessage("BspEditor:ApplyActiveTexture"));
			await Oy.Publish("ActivateTool", "SelectTool");

		}
		public override async Task ToolDeselected()
		{
			await base.ToolDeselected();
		}
	}
}
