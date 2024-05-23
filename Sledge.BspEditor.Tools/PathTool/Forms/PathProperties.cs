using System;
using System.Numerics;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using static Sledge.BspEditor.Tools.Draggable.PathState;

namespace Sledge.BspEditor.Tools.PathTool.Forms
{
	public partial class PathProperties : Form
	{
		//[Import("Shell", typeof(Form))] private Lazy<Form> _parent;
		//[Import] private IContext _context;
		private Vector3 _position;


		public PathProperties(Vector3 location)
		{
			_position = location;
			InitializeComponent();
		}

		private void OkClicked(object sender, EventArgs e)
		{
			PathProperty property = new PathProperty
			{
				Name = nameBox.Text.Trim(),
				ClassName = classBox.Text.Trim(),
				Direction = OneWay.Checked ? PathDirection.ONE_WAY : Circular.Checked ? PathDirection.CIRCULAR : PathDirection.PING_PONG,
				Position = _position
			};
			Oy.Publish("PathTool:NewPath", property);
			Close();
		}

		private void CancelClicked(object sender, EventArgs e)
		{
			this.Close();
		}

	}
}
