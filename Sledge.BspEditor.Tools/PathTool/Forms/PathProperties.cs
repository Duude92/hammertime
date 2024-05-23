using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations.Data;
using Sledge.BspEditor.Modification.Operations.Selection;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Hooks;
using Sledge.Common.Translations;
using Sledge.Shell;

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
			PathTool.PathProperty property = new PathTool.PathProperty
			{
				Name = nameBox.Text.Trim(),
				ClassName = classBox.Text.Trim(),
				Direction = OneWay.Checked ? PathTool.PathDirection.ONE_WAY : Circular.Checked ? PathTool.PathDirection.CIRCULAR : PathTool.PathDirection.PING_PONG,
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
