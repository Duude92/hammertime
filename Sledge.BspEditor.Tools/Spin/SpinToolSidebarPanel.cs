using System;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Context;
using Sledge.Shell;

namespace Sledge.BspEditor.Tools.Spin
{
    /// <summary>
    /// Sidebar panel for the spin tool: configures the number of steps, the
    /// total sweep angle and whether pieces are bisected and UVs mirrored.
    /// Only shown while the spin tool is the active tool.
    /// </summary>
    [Export(typeof(ISidebarComponent))]
    [OrderHint("F")]
    public class SpinToolSidebarPanel : UserControl, ISidebarComponent
    {
        [Import] private SpinTool _tool;

        public string Title { get; set; } = "Spin";
        public object Control => this;

        private readonly NumericUpDown _steps;
        private readonly NumericUpDown _angle;
        private readonly CheckBox _bisect;
        private readonly CheckBox _mirror;
        private readonly Button _spin;

        public SpinToolSidebarPanel()
        {
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 2,
                Padding = new Padding(6)
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            _steps = new NumericUpDown
            {
                Minimum = 2,
                Maximum = 64,
                Value = 8,
                Width = 60
            };
            _angle = new NumericUpDown
            {
                Minimum = -359,
                Maximum = 359,
                DecimalPlaces = 1,
                Increment = 5,
                Value = 90,
                Width = 60
            };
            _bisect = new CheckBox { Text = "Bisect pieces", Checked = true, AutoSize = true };
            _mirror = new CheckBox { Text = "Mirror UVs (seamless)", Checked = true, AutoSize = true };
            _spin = new Button { Text = "Spin", AutoSize = true };

            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            layout.Controls.Add(new Label { Text = "Steps:", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 0);
            layout.Controls.Add(_steps, 1, 0);
            layout.Controls.Add(new Label { Text = "Angle:", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 1);
            layout.Controls.Add(_angle, 1, 1);
            layout.Controls.Add(_bisect, 0, 2);
            layout.SetColumnSpan(_bisect, 2);
            layout.Controls.Add(_mirror, 0, 3);
            layout.SetColumnSpan(_mirror, 2);
            layout.Controls.Add(_spin, 0, 4);
            layout.SetColumnSpan(_spin, 2);

            Controls.Add(layout);

            _steps.ValueChanged += (s, e) => { if (_tool != null) _tool.Steps = (int) _steps.Value; };
            _angle.ValueChanged += (s, e) => { if (_tool != null) _tool.AngleDegrees = (float) _angle.Value; };
            _bisect.CheckedChanged += (s, e) => { if (_tool != null) _tool.BisectPieces = _bisect.Checked; };
            _mirror.CheckedChanged += (s, e) => { if (_tool != null) _tool.MirrorUvs = _mirror.Checked; };
            _spin.Click += (s, e) => _tool?.ApplySpin();

            // When a spin is performed by dragging in a viewport, the tool pushes
            // the angle it used so the panel stays in sync
            Oy.Subscribe<string>("SpinTool:SetAngle", v =>
            {
                if (!float.TryParse(v, NumberStyles.Float, CultureInfo.InvariantCulture, out var f)) return;
                this.InvokeLater(() =>
                {
                    var value = (decimal) f;
                    value = Math.Min(_angle.Maximum, Math.Max(_angle.Minimum, value));
                    if (_angle.Value != value) _angle.Value = value;
                });
            });
        }

        public bool IsInContext(IContext context)
        {
            return context.TryGet("ActiveTool", out SpinTool _);
        }
    }
}
