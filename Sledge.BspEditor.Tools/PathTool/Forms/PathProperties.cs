using System;
using System.Numerics;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Tools.Draggable;
using Sledge.BspEditor.Primitives.MapObjectData;
using static Sledge.BspEditor.Tools.Draggable.PathState;

namespace Sledge.BspEditor.Tools.PathTool.Forms
{
	public partial class PathProperties : Form
	{
		private Vector3 _position;
		public PathProperties(Vector3 location)
		{
			_position = location;
			InitializeComponent();
		}
		public PathProperties(PathState path)
		{
			InitializeComponent();
			nameBox.Text = path.Property.Name;
			classBox.Text = path.Property.ClassName;
			OneWay.Checked = path.Property.Direction == Path.PathDirection.OneWay;
			Circular.Checked = path.Property.Direction == Path.PathDirection.Circular;
			PP.Checked = path.Property.Direction == Path.PathDirection.PingPong;
		}

		private void OkClicked(object sender, EventArgs e)
		{
			PathProperty property = new PathProperty
			{
				Name = nameBox.Text.Trim(),
				ClassName = classBox.Text.Trim(),
				Direction = OneWay.Checked ? Path.PathDirection.OneWay : Circular.Checked ? Path.PathDirection.Circular : Path.PathDirection.PingPong,				
				Position = _position
			};
			Oy.Publish("PathTool:SavePathProperties", property);
			Close();
		}

		private void CancelClicked(object sender, EventArgs e)
		{
			this.Close();
		}

	}
}
