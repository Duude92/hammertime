using System;
using System.Numerics;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Tools.Draggable;
using Sledge.BspEditor.Primitives.MapObjectData;
using static Sledge.BspEditor.Tools.Draggable.PathState;

namespace Sledge.BspEditor.Tools.PathTool.Forms
{
	public partial class NodeProperties : Form
	{
		private Vector3 _position;
		private PathNodeHandle _handle;
		public NodeProperties(PathNodeHandle handle)
		{
			_handle = handle;
			InitializeComponent();
			nameBox.Text = handle.Name;

			FillProperty(handle, "speed", speedBox);
			FillProperty(handle, "yaw_speed", yawBox);
			FillProperty(handle, "wait", waitBox);
			if (handle.Properties.TryGetValue("spawnflags", out var spawnstring) && int.TryParse(spawnstring, out int spawnFlags))
			{
				retriggerCheck.Checked = (spawnFlags & 0x1) != 0;
			}
		}

		private void FillProperty(PathNodeHandle handle, string property, TextBox box)
		{
			box.Text = handle.Properties.TryGetValue(property, out var wait) ? wait : "0";
		}

		private void OkClicked(object sender, EventArgs e)
		{
			Close();
		}

		private void CancelClicked(object sender, EventArgs e)
		{
			this.Close();
		}

	}
}
