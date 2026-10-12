using Sledge.BspEditor.Primitives.MapData;
using Sledge.BspEditor.Primitives;
using System;

namespace Sledge.BspEditor.Tools.Selection
{
    partial class SelectToolSidebarPanel
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Show3DWidgetsCheckbox = new System.Windows.Forms.CheckBox();
            lblMode = new System.Windows.Forms.Label();
            TranslateModeCheckbox = new System.Windows.Forms.CheckBox();
            RotateModeCheckbox = new System.Windows.Forms.CheckBox();
            SkewModeCheckbox = new System.Windows.Forms.CheckBox();
            MoveToWorldButton = new System.Windows.Forms.Button();
            MoveToEntityButton = new System.Windows.Forms.Button();
            lblActions = new System.Windows.Forms.Label();
            keepEntityAngle = new System.Windows.Forms.CheckBox();
            MoveWidgetCheckbox = new System.Windows.Forms.CheckBox();
            RotateWidgetCheckbox = new System.Windows.Forms.CheckBox();
            MoveArrowWidgetCheckbox = new System.Windows.Forms.CheckBox();
            RotationArcWidgetCheckbox = new System.Windows.Forms.CheckBox();
            CameraWidgetCheckbox = new System.Windows.Forms.CheckBox();
            AutoSelectBoxCheckbox = new System.Windows.Forms.CheckBox();
            ShowBoundingBoxCheckbox = new System.Windows.Forms.CheckBox();
            SuspendLayout();
            // 
            // Show3DWidgetsCheckbox
            // 
            Show3DWidgetsCheckbox.AutoSize = true;
            Show3DWidgetsCheckbox.Location = new System.Drawing.Point(8, 86);
            Show3DWidgetsCheckbox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Show3DWidgetsCheckbox.Name = "Show3DWidgetsCheckbox";
            Show3DWidgetsCheckbox.Size = new System.Drawing.Size(118, 19);
            Show3DWidgetsCheckbox.TabIndex = 6;
            Show3DWidgetsCheckbox.Text = "Show 3D Widgets";
            Show3DWidgetsCheckbox.UseVisualStyleBackColor = true;
            Show3DWidgetsCheckbox.CheckedChanged += Show3DWidgetsChecked;
            // 
            // MoveWidgetCheckbox
            // 
            MoveWidgetCheckbox.AutoSize = true;
            MoveWidgetCheckbox.Checked = true;
            MoveWidgetCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            MoveWidgetCheckbox.Enabled = false;
            MoveWidgetCheckbox.Location = new System.Drawing.Point(26, 108);
            MoveWidgetCheckbox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MoveWidgetCheckbox.Name = "MoveWidgetCheckbox";
            MoveWidgetCheckbox.Size = new System.Drawing.Size(100, 19);
            MoveWidgetCheckbox.TabIndex = 13;
            MoveWidgetCheckbox.Text = "Move widget";
            MoveWidgetCheckbox.UseVisualStyleBackColor = true;
            MoveWidgetCheckbox.CheckedChanged += WidgetToggleChecked;
            // 
            // RotateWidgetCheckbox
            // 
            RotateWidgetCheckbox.AutoSize = true;
            RotateWidgetCheckbox.Checked = true;
            RotateWidgetCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            RotateWidgetCheckbox.Enabled = false;
            RotateWidgetCheckbox.Location = new System.Drawing.Point(26, 129);
            RotateWidgetCheckbox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            RotateWidgetCheckbox.Name = "RotateWidgetCheckbox";
            RotateWidgetCheckbox.Size = new System.Drawing.Size(100, 19);
            RotateWidgetCheckbox.TabIndex = 14;
            RotateWidgetCheckbox.Text = "Rotate widget";
            RotateWidgetCheckbox.UseVisualStyleBackColor = true;
            RotateWidgetCheckbox.CheckedChanged += WidgetToggleChecked;
            // 
            // MoveArrowWidgetCheckbox
            // 
            MoveArrowWidgetCheckbox.AutoSize = true;
            MoveArrowWidgetCheckbox.Checked = true;
            MoveArrowWidgetCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            MoveArrowWidgetCheckbox.Enabled = false;
            MoveArrowWidgetCheckbox.Location = new System.Drawing.Point(26, 150);
            MoveArrowWidgetCheckbox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MoveArrowWidgetCheckbox.Name = "MoveArrowWidgetCheckbox";
            MoveArrowWidgetCheckbox.Size = new System.Drawing.Size(140, 19);
            MoveArrowWidgetCheckbox.TabIndex = 15;
            MoveArrowWidgetCheckbox.Text = "Move arrow";
            MoveArrowWidgetCheckbox.UseVisualStyleBackColor = true;
            MoveArrowWidgetCheckbox.CheckedChanged += WidgetToggleChecked;
            // 
            // RotationArcWidgetCheckbox
            // 
            RotationArcWidgetCheckbox.AutoSize = true;
            RotationArcWidgetCheckbox.Checked = true;
            RotationArcWidgetCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            RotationArcWidgetCheckbox.Enabled = false;
            RotationArcWidgetCheckbox.Location = new System.Drawing.Point(26, 171);
            RotationArcWidgetCheckbox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            RotationArcWidgetCheckbox.Name = "RotationArcWidgetCheckbox";
            RotationArcWidgetCheckbox.Size = new System.Drawing.Size(140, 19);
            RotationArcWidgetCheckbox.TabIndex = 16;
            RotationArcWidgetCheckbox.Text = "Rotation arc";
            RotationArcWidgetCheckbox.UseVisualStyleBackColor = true;
            RotationArcWidgetCheckbox.CheckedChanged += WidgetToggleChecked;
            // 
            // CameraWidgetCheckbox
            // 
            CameraWidgetCheckbox.AutoSize = true;
            CameraWidgetCheckbox.Checked = true;
            CameraWidgetCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            CameraWidgetCheckbox.Location = new System.Drawing.Point(26, 192);
            CameraWidgetCheckbox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CameraWidgetCheckbox.Name = "CameraWidgetCheckbox";
            CameraWidgetCheckbox.Size = new System.Drawing.Size(140, 19);
            CameraWidgetCheckbox.TabIndex = 17;
            CameraWidgetCheckbox.Text = "Camera widget (2D)";
            CameraWidgetCheckbox.UseVisualStyleBackColor = true;
            CameraWidgetCheckbox.CheckedChanged += WidgetToggleChecked;
            // 
            // AutoSelectBoxCheckbox
            // 
            AutoSelectBoxCheckbox.AutoSize = true;
            AutoSelectBoxCheckbox.Location = new System.Drawing.Point(8, 62);
            AutoSelectBoxCheckbox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            AutoSelectBoxCheckbox.Name = "AutoSelectBoxCheckbox";
            AutoSelectBoxCheckbox.Size = new System.Drawing.Size(140, 19);
            AutoSelectBoxCheckbox.TabIndex = 18;
            AutoSelectBoxCheckbox.Text = "Auto-select box";
            AutoSelectBoxCheckbox.UseVisualStyleBackColor = true;
            AutoSelectBoxCheckbox.CheckedChanged += AutoSelectBoxChecked;
            // 
            // ShowBoundingBoxCheckbox
            // 
            ShowBoundingBoxCheckbox.AutoSize = true;
            ShowBoundingBoxCheckbox.Checked = true;
            ShowBoundingBoxCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            ShowBoundingBoxCheckbox.Location = new System.Drawing.Point(8, 74);
            ShowBoundingBoxCheckbox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            ShowBoundingBoxCheckbox.Name = "ShowBoundingBoxCheckbox";
            ShowBoundingBoxCheckbox.Size = new System.Drawing.Size(140, 19);
            ShowBoundingBoxCheckbox.TabIndex = 19;
            ShowBoundingBoxCheckbox.Text = "Show bounding box";
            ShowBoundingBoxCheckbox.UseVisualStyleBackColor = true;
            ShowBoundingBoxCheckbox.CheckedChanged += ShowBoundingBoxChecked;
            // 
            // lblMode
            // 
            lblMode.AutoSize = true;
            lblMode.Location = new System.Drawing.Point(4, 6);
            lblMode.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblMode.Name = "lblMode";
            lblMode.Size = new System.Drawing.Size(115, 15);
            lblMode.TabIndex = 5;
            lblMode.Text = "Manipulation Mode:";
            lblMode.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // TranslateModeCheckbox
            // 
            TranslateModeCheckbox.Appearance = System.Windows.Forms.Appearance.Button;
            TranslateModeCheckbox.Location = new System.Drawing.Point(4, 3);
            TranslateModeCheckbox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TranslateModeCheckbox.Name = "TranslateModeCheckbox";
            TranslateModeCheckbox.Size = new System.Drawing.Size(63, 25);
            TranslateModeCheckbox.TabIndex = 7;
            TranslateModeCheckbox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            TranslateModeCheckbox.Text = "Move";
            TranslateModeCheckbox.UseVisualStyleBackColor = true;
            TranslateModeCheckbox.CheckedChanged += TranslateModeChecked;
            // 
            // RotateModeCheckbox
            // 
            RotateModeCheckbox.Appearance = System.Windows.Forms.Appearance.Button;
            RotateModeCheckbox.Location = new System.Drawing.Point(75, 3);
            RotateModeCheckbox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            RotateModeCheckbox.Name = "RotateModeCheckbox";
            RotateModeCheckbox.Size = new System.Drawing.Size(51, 25);
            RotateModeCheckbox.TabIndex = 7;
            RotateModeCheckbox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            RotateModeCheckbox.Text = "Rotate";
            RotateModeCheckbox.UseVisualStyleBackColor = true;
            RotateModeCheckbox.CheckedChanged += RotateModeChecked;
            // 
            // SkewModeCheckbox
            // 
            SkewModeCheckbox.Appearance = System.Windows.Forms.Appearance.Button;
            SkewModeCheckbox.Location = new System.Drawing.Point(134, 3);
            SkewModeCheckbox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            SkewModeCheckbox.Name = "SkewModeCheckbox";
            SkewModeCheckbox.Size = new System.Drawing.Size(44, 25);
            SkewModeCheckbox.TabIndex = 7;
            SkewModeCheckbox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            SkewModeCheckbox.Text = "Skew";
            SkewModeCheckbox.UseVisualStyleBackColor = true;
            SkewModeCheckbox.CheckedChanged += SkewModeChecked;
            // 
            // MoveToWorldButton
            // 
            MoveToWorldButton.Location = new System.Drawing.Point(1, 1);
            MoveToWorldButton.Margin = new System.Windows.Forms.Padding(1);
            MoveToWorldButton.Name = "MoveToWorldButton";
            MoveToWorldButton.Size = new System.Drawing.Size(117, 29);
            MoveToWorldButton.TabIndex = 8;
            MoveToWorldButton.Text = "Move to World";
            MoveToWorldButton.UseVisualStyleBackColor = true;
            MoveToWorldButton.Click += MoveToWorldButtonClicked;
            // 
            // MoveToEntityButton
            // 
            MoveToEntityButton.Location = new System.Drawing.Point(1, 32);
            MoveToEntityButton.Margin = new System.Windows.Forms.Padding(1);
            MoveToEntityButton.Name = "MoveToEntityButton";
            MoveToEntityButton.Size = new System.Drawing.Size(117, 29);
            MoveToEntityButton.TabIndex = 9;
            MoveToEntityButton.Text = "Tie to Entity";
            MoveToEntityButton.UseVisualStyleBackColor = true;
            MoveToEntityButton.Click += TieToEntityButtonClicked;
            // 
            // lblActions
            // 
            lblActions.AutoSize = true;
            lblActions.Location = new System.Drawing.Point(4, 242);
            lblActions.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblActions.Name = "lblActions";
            lblActions.Size = new System.Drawing.Size(50, 15);
            lblActions.TabIndex = 5;
            lblActions.Text = "Actions:";
            lblActions.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // keepEntityAngle
            // 
            keepEntityAngle.AutoSize = true;
            keepEntityAngle.Location = new System.Drawing.Point(8, 216);
            keepEntityAngle.Name = "keepEntityAngle";
            keepEntityAngle.Size = new System.Drawing.Size(122, 26);
            keepEntityAngle.TabIndex = 12;
            keepEntityAngle.Text = "Keep entity angles";
            keepEntityAngle.UseVisualStyleBackColor = true;
            keepEntityAngle.Checked = true;
            keepEntityAngle.CheckedChanged += KeepEntityAngleChecked;
			// 
			// SelectToolSidebarPanel
			// 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            Controls.Add(keepEntityAngle);
            Controls.Add(MoveToWorldButton);
            Controls.Add(MoveToEntityButton);
            Controls.Add(TranslateModeCheckbox);
            Controls.Add(RotateModeCheckbox);
            Controls.Add(SkewModeCheckbox);
            Controls.Add(lblActions);
            Controls.Add(lblMode);
            Controls.Add(Show3DWidgetsCheckbox);
            Controls.Add(MoveWidgetCheckbox);
            Controls.Add(RotateWidgetCheckbox);
            Controls.Add(MoveArrowWidgetCheckbox);
            Controls.Add(RotationArcWidgetCheckbox);
            Controls.Add(CameraWidgetCheckbox);
            Controls.Add(AutoSelectBoxCheckbox);
            Controls.Add(ShowBoundingBoxCheckbox);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            Size = new System.Drawing.Size(271, 330);
            ResumeLayout(false);
            PerformLayout();
        }

		private System.Windows.Forms.CheckBox keepEntityAngle;

        #endregion

        private System.Windows.Forms.CheckBox Show3DWidgetsCheckbox;
        private System.Windows.Forms.CheckBox MoveWidgetCheckbox;
        private System.Windows.Forms.CheckBox RotateWidgetCheckbox;
        private System.Windows.Forms.CheckBox MoveArrowWidgetCheckbox;
        private System.Windows.Forms.CheckBox RotationArcWidgetCheckbox;
        private System.Windows.Forms.CheckBox CameraWidgetCheckbox;
        private System.Windows.Forms.CheckBox AutoSelectBoxCheckbox;
        private System.Windows.Forms.CheckBox ShowBoundingBoxCheckbox;
        private System.Windows.Forms.Label lblMode;
        private System.Windows.Forms.CheckBox TranslateModeCheckbox;
        private System.Windows.Forms.CheckBox RotateModeCheckbox;
        private System.Windows.Forms.CheckBox SkewModeCheckbox;
        private System.Windows.Forms.Button MoveToWorldButton;
        private System.Windows.Forms.Button MoveToEntityButton;
        private System.Windows.Forms.Label lblActions;
    }
}
