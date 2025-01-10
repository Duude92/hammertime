using System;
using System.Linq;
using System.Numerics;
using System.Windows.Forms;
using Sledge.BspEditor.Tools.Draggable;

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
		private void OkClicked(object sender, EventArgs e)
		{
			_handle.Name = nameBox.Text;
			ReadProperty(_handle, "speed", speedBox);
			ReadProperty(_handle, "yaw_speed", yawBox);
			ReadProperty(_handle, "wait", waitBox);
			var properties = _handle.Properties;
			if (retriggerCheck.Checked)
			{
				if (properties.TryGetValue("spawnflags", out var spawnstring) && int.TryParse(spawnstring, out int spawnFlags))
				{
					properties["spawnflags"] = (spawnFlags | 0x1).ToString();
				}
				else
				{
					properties.Add("spawnflags", "1");
				}
			}
			else
			{
				if (properties.Keys.Contains("spawnflags"))
				{
					properties.Remove("spawnflag");
				}
			}
			Close();
		}
		private void CancelClicked(object sender, EventArgs e)
		{
			this.Close();
		}
		private void FillProperty(PathNodeHandle handle, string property, TextBox box)
		{
			box.Text = handle.Properties.TryGetValue(property, out var wait) ? wait : "0";
		}
		private void ReadProperty(PathNodeHandle handle, string property, TextBox box)
		{
			if (!float.TryParse(box.Text, out var prop)) return;
			if (prop == 0)
			{
				if (!handle.Properties.ContainsKey(property)) return;
				handle.Properties.Remove(property);
				return;
			}
			if (handle.Properties.ContainsKey(property))
			{
				handle.Properties[property] = box.Text;
				return;
			}
			handle.Properties.TryAdd(property, box.Text);
		}
	}
}
