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
            flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            keepEntityAngle = new System.Windows.Forms.CheckBox();
            flowLayoutPanel1.SuspendLayout();
            flowLayoutPanel2.SuspendLayout();
            SuspendLayout();
            // 
            // Show3DWidgetsCheckbox
            // 
            Show3DWidgetsCheckbox.AutoSize = true;
            Show3DWidgetsCheckbox.Location = new System.Drawing.Point(8, 62);
            Show3DWidgetsCheckbox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Show3DWidgetsCheckbox.Name = "Show3DWidgetsCheckbox";
            Show3DWidgetsCheckbox.Size = new System.Drawing.Size(118, 19);
            Show3DWidgetsCheckbox.TabIndex = 6;
            Show3DWidgetsCheckbox.Text = "Show 3D Widgets";
            Show3DWidgetsCheckbox.UseVisualStyleBackColor = true;
            Show3DWidgetsCheckbox.CheckedChanged += Show3DWidgetsChecked;
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
            TranslateModeCheckbox.AutoSize = true;
            TranslateModeCheckbox.Location = new System.Drawing.Point(4, 3);
            TranslateModeCheckbox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TranslateModeCheckbox.Name = "TranslateModeCheckbox";
            TranslateModeCheckbox.Size = new System.Drawing.Size(63, 25);
            TranslateModeCheckbox.TabIndex = 7;
            TranslateModeCheckbox.Text = "Translate";
            TranslateModeCheckbox.UseVisualStyleBackColor = true;
            TranslateModeCheckbox.CheckedChanged += TranslateModeChecked;
            // 
            // RotateModeCheckbox
            // 
            RotateModeCheckbox.Appearance = System.Windows.Forms.Appearance.Button;
            RotateModeCheckbox.AutoSize = true;
            RotateModeCheckbox.Location = new System.Drawing.Point(75, 3);
            RotateModeCheckbox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            RotateModeCheckbox.Name = "RotateModeCheckbox";
            RotateModeCheckbox.Size = new System.Drawing.Size(51, 25);
            RotateModeCheckbox.TabIndex = 7;
            RotateModeCheckbox.Text = "Rotate";
            RotateModeCheckbox.UseVisualStyleBackColor = true;
            RotateModeCheckbox.CheckedChanged += RotateModeChecked;
            // 
            // SkewModeCheckbox
            // 
            SkewModeCheckbox.Appearance = System.Windows.Forms.Appearance.Button;
            SkewModeCheckbox.AutoSize = true;
            SkewModeCheckbox.Location = new System.Drawing.Point(134, 3);
            SkewModeCheckbox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            SkewModeCheckbox.Name = "SkewModeCheckbox";
            SkewModeCheckbox.Size = new System.Drawing.Size(44, 25);
            SkewModeCheckbox.TabIndex = 7;
            SkewModeCheckbox.Text = "Skew";
            SkewModeCheckbox.UseVisualStyleBackColor = true;
            SkewModeCheckbox.CheckedChanged += SkewModeChecked;
            // 
            // MoveToWorldButton
            // 
            MoveToWorldButton.AutoSize = true;
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
            MoveToEntityButton.AutoSize = true;
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
            lblActions.Location = new System.Drawing.Point(4, 113);
            lblActions.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblActions.Name = "lblActions";
            lblActions.Size = new System.Drawing.Size(50, 15);
            lblActions.TabIndex = 5;
            lblActions.Text = "Actions:";
            lblActions.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
            flowLayoutPanel1.Controls.Add(TranslateModeCheckbox);
            flowLayoutPanel1.Controls.Add(RotateModeCheckbox);
            flowLayoutPanel1.Controls.Add(SkewModeCheckbox);
            flowLayoutPanel1.Location = new System.Drawing.Point(4, 22);
            flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new System.Drawing.Size(257, 35);
            flowLayoutPanel1.TabIndex = 10;
            flowLayoutPanel1.WrapContents = false;
            // 
            // flowLayoutPanel2
            // 
            flowLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
            flowLayoutPanel2.Controls.Add(MoveToWorldButton);
            flowLayoutPanel2.Controls.Add(MoveToEntityButton);
            flowLayoutPanel2.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            flowLayoutPanel2.Location = new System.Drawing.Point(4, 131);
            flowLayoutPanel2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            flowLayoutPanel2.Name = "flowLayoutPanel2";
            flowLayoutPanel2.Size = new System.Drawing.Size(257, 61);
            flowLayoutPanel2.TabIndex = 11;
            flowLayoutPanel2.WrapContents = false;
            // 
            // keepEntityAngle
            // 
            keepEntityAngle.Location = new System.Drawing.Point(8, 87);
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
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            AutoSize = true;
            Controls.Add(keepEntityAngle);
            Controls.Add(flowLayoutPanel2);
            Controls.Add(flowLayoutPanel1);
            Controls.Add(lblActions);
            Controls.Add(lblMode);
            Controls.Add(Show3DWidgetsCheckbox);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            Size = new System.Drawing.Size(271, 201);
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.PerformLayout();
            flowLayoutPanel2.ResumeLayout(false);
            flowLayoutPanel2.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

		private System.Windows.Forms.CheckBox keepEntityAngle;

        #endregion

        private System.Windows.Forms.CheckBox Show3DWidgetsCheckbox;
        private System.Windows.Forms.Label lblMode;
        private System.Windows.Forms.CheckBox TranslateModeCheckbox;
        private System.Windows.Forms.CheckBox RotateModeCheckbox;
        private System.Windows.Forms.CheckBox SkewModeCheckbox;
        private System.Windows.Forms.Button MoveToWorldButton;
        private System.Windows.Forms.Button MoveToEntityButton;
        private System.Windows.Forms.Label lblActions;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
    }
}
