using System;
using System.Numerics;
using System.Windows.Forms;
using Sledge.BspEditor.Tools.Draggable;
using Sledge.BspEditor.Primitives.MapObjectData;
using static Sledge.BspEditor.Tools.Draggable.PathState;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Context;
using System.ComponentModel.Composition;
using static Sledge.Shell.ControlExtensions;
using LogicAndTrick.Oy;
using System.ComponentModel;

namespace Sledge.BspEditor.Tools.PathTool.Forms
{
	[Export(typeof(IDialog))]
	public partial class PathProperties : Form, IDialog
	{
		private Vector3 _position;
		private PathState _state;
		[Import("Shell", typeof(Form))] private Lazy<Form> _parent;
        public PathProperties()
        {
            InitializeComponent();
			CreateHandle();
        }

        public PathProperties(PathState path)
		{
			InitializeComponent();
			InitializeForm(path);
		}
		private void InitializeForm(PathState path)
		{
			_state = path;
			nameBox.Text = path.Property.Name;
			if (string.IsNullOrEmpty(path.Property.ClassName)) classBox.SelectedIndex = 0;
			else classBox.Text = path.Property.ClassName;
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
			_state.Property = property;
			Close();
		}

		private void CancelClicked(object sender, EventArgs e)
		{
			this.Close();
		}

		public void SetVisible(IContext context, bool visible)
		{
			this.InvokeLater(() =>
			{
				if (visible)
				{
					var state = context.Get<PathState>("PathState");

					InitializeForm(state);
					if (!Visible) Show(_parent.Value);
				}
				else
				{
					Hide();
				}
			});
		}
		protected override void OnClosing(CancelEventArgs e)
		{
			e.Cancel = true;
			Oy.Publish("Context:Remove", new ContextInfo("BspEditor:PathPropertiesShow"));
		}
		public bool IsInContext(IContext context)
		{
			return context.HasAny("BspEditor:PathPropertiesShow");
		}
	}
}
