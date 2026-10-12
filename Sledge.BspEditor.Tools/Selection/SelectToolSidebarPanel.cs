using System;
using System.Drawing;
using System.ComponentModel.Composition;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Context;
using Sledge.Common.Translations;
using Sledge.Shell;
using Sledge.BspEditor.Documents;
using Sledge.Common.Shell.Documents;

namespace Sledge.BspEditor.Tools.Selection
{
	[Export(typeof(ISidebarComponent))]
	[OrderHint("F")]
	[AutoTranslate]
	public partial class SelectToolSidebarPanel : UserControl, ISidebarComponent, IManualTranslate
	{
		public string Title { get; set; } = "Selection Tool";
		public object Control => this;

		private WeakReference<MapDocument> _activeDocument;
		private bool _updatingWidgetToggles;

		public SelectToolSidebarPanel()
		{
			InitializeComponent();
			LayoutControls();

			Oy.Subscribe<String>("SelectTool:TransformationModeChanged", x =>
			{
				if (Enum.TryParse(x, out SelectionBoxDraggableState.TransformationMode mode))
				{
					TransformationToolChanged(mode);
				}
			});

			Oy.Subscribe<String>("SelectTool:SetShow3DWidgets", x =>
			{
				Show3DWidgetsCheckbox.Checked = x == "1";
			});

			Oy.Subscribe<String>("SelectTool:SetShowBoundingBox", x =>
			{
				ShowBoundingBoxCheckbox.Checked = x == "1";
			});

			Oy.Subscribe<String>("SelectTool:SetAutoSelectBox", x =>
			{
				AutoSelectBoxCheckbox.Checked = x == "1";
			});

			Oy.Subscribe<String>("SelectTool:SetWidgetToggles", x =>
			{
				if (string.IsNullOrEmpty(x) || x.Length < 4) return;
				_updatingWidgetToggles = true;
				try
				{
					MoveWidgetCheckbox.Checked = x[0] == '1';
					RotateWidgetCheckbox.Checked = x[1] == '1';
					MoveArrowWidgetCheckbox.Checked = x[2] == '1';
					RotationArcWidgetCheckbox.Checked = x[3] == '1';
					if (x.Length > 4) CameraWidgetCheckbox.Checked = x[4] == '1';
				}
				finally
				{
					_updatingWidgetToggles = false;
				}
			});
			Oy.Subscribe<IDocument>("Document:Activated", DocumentActivated);
		}

		private bool _layouting;

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			LayoutControls();
		}

		protected override void OnLayout(LayoutEventArgs e)
		{
			base.OnLayout(e);
			LayoutControls();
		}

		/// <summary>
		/// The three mode buttons split the width into thirds and the action buttons fill it,
		/// so nothing is cut off at the edge. The height follows the font/DPI.
		/// </summary>
		private void LayoutControls()
		{
			if (_layouting || lblMode == null) return;
			_layouting = true;
			try
			{
				SuspendLayout();

				var p = LogicalToDeviceUnits(6);
				var m = LogicalToDeviceUnits(3);
				var indent = LogicalToDeviceUnits(18);
				var buttonH = Math.Max(LogicalToDeviceUnits(25), Font.Height + LogicalToDeviceUnits(10));
				var inner = Math.Max(1, ClientSize.Width - 2 * p);
				var y = p;

				void Row(Control c, int x)
				{
					c.Location = new Point(x, y);
					y += c.Height + m;
				}

				Row(lblMode, p);

				var w = Math.Max(1, (inner - 2 * m) / 3);
				TranslateModeCheckbox.SetBounds(p, y, w, buttonH);
				RotateModeCheckbox.SetBounds(p + w + m, y, w, buttonH);
				SkewModeCheckbox.SetBounds(p + 2 * (w + m), y, Math.Max(1, inner - 2 * (w + m)), buttonH);
				y += buttonH + 2 * m;

				var x1 = p + LogicalToDeviceUnits(2);
				var x2 = x1 + indent;
				Row(AutoSelectBoxCheckbox, x1);
				Row(ShowBoundingBoxCheckbox, x1);
				Row(Show3DWidgetsCheckbox, x1);
				Row(MoveWidgetCheckbox, x2);
				Row(RotateWidgetCheckbox, x2);
				Row(MoveArrowWidgetCheckbox, x2);
				Row(RotationArcWidgetCheckbox, x2);
				Row(CameraWidgetCheckbox, x2);
				Row(keepEntityAngle, x1);
				y += m;

				Row(lblActions, p);
				MoveToWorldButton.SetBounds(p, y, inner, buttonH);
				y += buttonH + m;
				MoveToEntityButton.SetBounds(p, y, inner, buttonH);
				y += buttonH + p;

				if (Height != y) Height = y;

				ResumeLayout(false);
			}
			finally
			{
				_layouting = false;
			}
		}

		public void Translate(ITranslationStringProvider strings)
		{
			CreateHandle();
			var prefix = GetType().FullName;
			this.InvokeLater(() =>
			{
				Title = strings.GetString(prefix, "Title");
				lblMode.Text = strings.GetString(prefix, "Mode");
				TranslateModeCheckbox.Text = strings.GetString(prefix, "Translate");
				RotateModeCheckbox.Text = strings.GetString(prefix, "Rotate");
				SkewModeCheckbox.Text = strings.GetString(prefix, "Skew");
				Show3DWidgetsCheckbox.Text = strings.GetString(prefix, "Show3DWidgets");
				AutoSelectBoxCheckbox.Text = strings.GetString(prefix, "AutoSelectBox") ?? AutoSelectBoxCheckbox.Text;
				ShowBoundingBoxCheckbox.Text = strings.GetString(prefix, "ShowBoundingBox") ?? ShowBoundingBoxCheckbox.Text;
				MoveWidgetCheckbox.Text = strings.GetString(prefix, "MoveWidget") ?? MoveWidgetCheckbox.Text;
				RotateWidgetCheckbox.Text = strings.GetString(prefix, "RotateWidget") ?? RotateWidgetCheckbox.Text;
				MoveArrowWidgetCheckbox.Text = strings.GetString(prefix, "MoveArrowWidget") ?? MoveArrowWidgetCheckbox.Text;
				RotationArcWidgetCheckbox.Text = strings.GetString(prefix, "RotationArcWidget") ?? RotationArcWidgetCheckbox.Text;
				CameraWidgetCheckbox.Text = strings.GetString(prefix, "CameraWidget") ?? CameraWidgetCheckbox.Text;
				lblActions.Text = strings.GetString(prefix, "Actions");
				MoveToWorldButton.Text = strings.GetString(prefix, "MoveToWorld");
				MoveToEntityButton.Text = strings.GetString(prefix, "TieToEntity");
			});
		}

		public bool IsInContext(IContext context)
		{
			return context.TryGet("ActiveTool", out SelectTool _);
		}

		private SelectionBoxDraggableState.TransformationMode _selectedType;

		public void TransformationToolChanged(SelectionBoxDraggableState.TransformationMode tt)
		{
			_selectedType = tt;
			SetCheckState();
		}
		private delegate void SetCheckStateCallback(CheckBox checkBox, bool state);
		private void SetCheckState(CheckBox checkBox, bool state)
		{
			if (checkBox.InvokeRequired)
			{
				SetCheckStateCallback callback = new SetCheckStateCallback(SetCheckState);
				this.Invoke(callback, new object[] { checkBox, state });
			}
			else
			{
				checkBox.Checked = state;
			}
		}

		private void SetCheckState()
		{
			if (_selectedType == SelectionBoxDraggableState.TransformationMode.Resize)
			{
				SetCheckState(RotateModeCheckbox, false);
				SetCheckState(SkewModeCheckbox, false);
				SetCheckState(TranslateModeCheckbox, true);
			}
			else if (_selectedType == SelectionBoxDraggableState.TransformationMode.Rotate)
			{
				SetCheckState(TranslateModeCheckbox, false);
				SetCheckState(SkewModeCheckbox, false);
				SetCheckState(RotateModeCheckbox, true);
			}
			else if (_selectedType == SelectionBoxDraggableState.TransformationMode.Skew)
			{
				SetCheckState(RotateModeCheckbox, false);
				SetCheckState(TranslateModeCheckbox, false);
				SetCheckState(SkewModeCheckbox, true);
			}
		}

		private void TranslateModeChecked(object sender, EventArgs e)
		{
			if (TranslateModeCheckbox.Checked && _selectedType != SelectionBoxDraggableState.TransformationMode.Resize)
				Oy.Publish("SelectTool:TransformationModeChanged", "Resize");
			else
				SetCheckState();
		}

		private void RotateModeChecked(object sender, EventArgs e)
		{
			if (RotateModeCheckbox.Checked && _selectedType != SelectionBoxDraggableState.TransformationMode.Rotate)
				Oy.Publish("SelectTool:TransformationModeChanged", "Rotate");
			else
				SetCheckState();
		}

		private void SkewModeChecked(object sender, EventArgs e)
		{
			if (SkewModeCheckbox.Checked && _selectedType != SelectionBoxDraggableState.TransformationMode.Skew)
				Oy.Publish("SelectTool:TransformationModeChanged", "Skew");
			else
				SetCheckState();
		}

		private void Show3DWidgetsChecked(object sender, EventArgs e)
		{
			Oy.Publish("SelectTool:Show3DWidgetsChanged", Show3DWidgetsCheckbox.Checked ? "1" : "0");

			// The individual widget toggles only make sense while 3D widgets are on
			// (the camera widget is drawn in the 2D views, so it is independent of this checkbox)
			MoveWidgetCheckbox.Enabled = RotateWidgetCheckbox.Enabled =
				MoveArrowWidgetCheckbox.Enabled = RotationArcWidgetCheckbox.Enabled = Show3DWidgetsCheckbox.Checked;
		}

		private void ShowBoundingBoxChecked(object sender, EventArgs e)
		{
			Oy.Publish("SelectTool:ShowBoundingBoxChanged", ShowBoundingBoxCheckbox.Checked ? "1" : "0");
		}

		private void AutoSelectBoxChecked(object sender, EventArgs e)
		{
			Oy.Publish("SelectTool:AutoSelectBoxChanged", AutoSelectBoxCheckbox.Checked ? "1" : "0");
		}

		private void WidgetToggleChecked(object sender, EventArgs e)
		{
			if (_updatingWidgetToggles) return;
			Oy.Publish("SelectTool:WidgetTogglesChanged",
				(MoveWidgetCheckbox.Checked ? "1" : "0") +
				(RotateWidgetCheckbox.Checked ? "1" : "0") +
				(MoveArrowWidgetCheckbox.Checked ? "1" : "0") +
				(RotationArcWidgetCheckbox.Checked ? "1" : "0") +
				(CameraWidgetCheckbox.Checked ? "1" : "0"));
		}

		private void MoveToWorldButtonClicked(object sender, EventArgs e)
		{
			Oy.Publish("Command:Run", new CommandMessage("BspEditor:Tools:MoveToWorld"));
		}

		private void TieToEntityButtonClicked(object sender, EventArgs e)
		{
			Oy.Publish("Command:Run", new CommandMessage("BspEditor:Tools:TieToEntity"));
		}
		private void KeepEntityAngleChecked(object sender, EventArgs e)
		{
			_activeDocument.TryGetTarget(out var doc);
			var flags = doc.Map.Data.GetOne<TransformationFlags>() ?? new TransformationFlags();
			flags.KeepRotationAngle = keepEntityAngle.Checked;
			doc.Map.Data.Replace(flags);
		}
		private void DocumentActivated(IDocument document)
		{
			var md = document as MapDocument;

			_activeDocument = new WeakReference<MapDocument>(md);
		}
	}
}
