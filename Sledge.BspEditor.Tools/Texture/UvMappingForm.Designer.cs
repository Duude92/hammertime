namespace Sledge.BspEditor.Tools.Texture
{
    partial class UvMappingForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            PreviewPanel = new UvPreviewPanel();
            StatusLabel = new System.Windows.Forms.Label();
            ValuesGroup = new System.Windows.Forms.GroupBox();
            ShiftLabel = new System.Windows.Forms.Label();
            ShiftXValue = new Sledge.Shell.Controls.NumericUpDownEx();
            ShiftYValue = new Sledge.Shell.Controls.NumericUpDownEx();
            ScaleLabel = new System.Windows.Forms.Label();
            ScaleXValue = new Sledge.Shell.Controls.NumericUpDownEx();
            ScaleYValue = new Sledge.Shell.Controls.NumericUpDownEx();
            RotationLabel = new System.Windows.Forms.Label();
            RotationValue = new Sledge.Shell.Controls.NumericUpDownEx();
            RotMinus90Button = new System.Windows.Forms.Button();
            RotPlus90Button = new System.Windows.Forms.Button();
            AxesGroup = new System.Windows.Forms.GroupBox();
            UAxisLabel = new System.Windows.Forms.Label();
            UvUX = new System.Windows.Forms.TextBox();
            UvUY = new System.Windows.Forms.TextBox();
            UvUZ = new System.Windows.Forms.TextBox();
            VAxisLabel = new System.Windows.Forms.Label();
            UvVX = new System.Windows.Forms.TextBox();
            UvVY = new System.Windows.Forms.TextBox();
            UvVZ = new System.Windows.Forms.TextBox();
            AlignFaceButton = new System.Windows.Forms.Button();
            AlignWorldButton = new System.Windows.Forms.Button();
            FitTextureButton = new System.Windows.Forms.Button();
            FitViewButton = new System.Windows.Forms.Button();
            HintLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)ShiftXValue).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ShiftYValue).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ScaleXValue).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ScaleYValue).BeginInit();
            ((System.ComponentModel.ISupportInitialize)RotationValue).BeginInit();
            ValuesGroup.SuspendLayout();
            AxesGroup.SuspendLayout();
            SuspendLayout();
            // 
            // PreviewPanel
            // 
            PreviewPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            PreviewPanel.Location = new System.Drawing.Point(12, 12);
            PreviewPanel.Name = "PreviewPanel";
            PreviewPanel.Size = new System.Drawing.Size(446, 240);
            PreviewPanel.TabIndex = 0;
            // 
            // StatusLabel
            // 
            StatusLabel.Location = new System.Drawing.Point(12, 258);
            StatusLabel.Name = "StatusLabel";
            StatusLabel.Size = new System.Drawing.Size(446, 18);
            StatusLabel.TabIndex = 1;
            StatusLabel.Text = "No faces selected";
            // 
            // ValuesGroup
            // 
            ValuesGroup.Controls.Add(ShiftLabel);
            ValuesGroup.Controls.Add(ShiftXValue);
            ValuesGroup.Controls.Add(ShiftYValue);
            ValuesGroup.Controls.Add(ScaleLabel);
            ValuesGroup.Controls.Add(ScaleXValue);
            ValuesGroup.Controls.Add(ScaleYValue);
            ValuesGroup.Controls.Add(RotationLabel);
            ValuesGroup.Controls.Add(RotationValue);
            ValuesGroup.Controls.Add(RotMinus90Button);
            ValuesGroup.Controls.Add(RotPlus90Button);
            ValuesGroup.Location = new System.Drawing.Point(12, 282);
            ValuesGroup.Name = "ValuesGroup";
            ValuesGroup.Size = new System.Drawing.Size(446, 120);
            ValuesGroup.TabIndex = 2;
            ValuesGroup.TabStop = false;
            ValuesGroup.Text = "UV values";
            // 
            // ShiftLabel
            // 
            ShiftLabel.Location = new System.Drawing.Point(14, 26);
            ShiftLabel.Name = "ShiftLabel";
            ShiftLabel.Size = new System.Drawing.Size(54, 18);
            ShiftLabel.TabIndex = 0;
            ShiftLabel.Text = "Shift:";
            // 
            // ShiftXValue
            // 
            ShiftXValue.DecimalPlaces = 2;
            ShiftXValue.Location = new System.Drawing.Point(72, 23);
            ShiftXValue.Maximum = new decimal(new int[] { 4096, 0, 0, 0 });
            ShiftXValue.Minimum = new decimal(new int[] { 4096, 0, 0, int.MinValue });
            ShiftXValue.Name = "ShiftXValue";
            ShiftXValue.Size = new System.Drawing.Size(90, 23);
            ShiftXValue.TabIndex = 1;
            // 
            // ShiftYValue
            // 
            ShiftYValue.DecimalPlaces = 2;
            ShiftYValue.Location = new System.Drawing.Point(168, 23);
            ShiftYValue.Maximum = new decimal(new int[] { 4096, 0, 0, 0 });
            ShiftYValue.Minimum = new decimal(new int[] { 4096, 0, 0, int.MinValue });
            ShiftYValue.Name = "ShiftYValue";
            ShiftYValue.Size = new System.Drawing.Size(90, 23);
            ShiftYValue.TabIndex = 2;
            // 
            // ScaleLabel
            // 
            ScaleLabel.Location = new System.Drawing.Point(14, 55);
            ScaleLabel.Name = "ScaleLabel";
            ScaleLabel.Size = new System.Drawing.Size(54, 18);
            ScaleLabel.TabIndex = 3;
            ScaleLabel.Text = "Scale:";
            // 
            // ScaleXValue
            // 
            ScaleXValue.DecimalPlaces = 4;
            ScaleXValue.Location = new System.Drawing.Point(72, 52);
            ScaleXValue.Maximum = new decimal(new int[] { 4096, 0, 0, 0 });
            ScaleXValue.Minimum = new decimal(new int[] { 4096, 0, 0, int.MinValue });
            ScaleXValue.Name = "ScaleXValue";
            ScaleXValue.Size = new System.Drawing.Size(90, 23);
            ScaleXValue.TabIndex = 4;
            // 
            // ScaleYValue
            // 
            ScaleYValue.DecimalPlaces = 4;
            ScaleYValue.Location = new System.Drawing.Point(168, 52);
            ScaleYValue.Maximum = new decimal(new int[] { 4096, 0, 0, 0 });
            ScaleYValue.Minimum = new decimal(new int[] { 4096, 0, 0, int.MinValue });
            ScaleYValue.Name = "ScaleYValue";
            ScaleYValue.Size = new System.Drawing.Size(90, 23);
            ScaleYValue.TabIndex = 5;
            // 
            // RotationLabel
            // 
            RotationLabel.Location = new System.Drawing.Point(14, 84);
            RotationLabel.Name = "RotationLabel";
            RotationLabel.Size = new System.Drawing.Size(64, 18);
            RotationLabel.TabIndex = 6;
            RotationLabel.Text = "Rotation:";
            // 
            // RotationValue
            // 
            RotationValue.DecimalPlaces = 2;
            RotationValue.Location = new System.Drawing.Point(84, 81);
            RotationValue.Maximum = new decimal(new int[] { 360, 0, 0, 0 });
            RotationValue.Minimum = new decimal(new int[] { 360, 0, 0, int.MinValue });
            RotationValue.Name = "RotationValue";
            RotationValue.Size = new System.Drawing.Size(90, 23);
            RotationValue.TabIndex = 7;
            // 
            // RotMinus90Button
            // 
            RotMinus90Button.Location = new System.Drawing.Point(300, 81);
            RotMinus90Button.Name = "RotMinus90Button";
            RotMinus90Button.Size = new System.Drawing.Size(60, 23);
            RotMinus90Button.TabIndex = 8;
            RotMinus90Button.Tag = -90F;
            RotMinus90Button.Text = "-90";
            RotMinus90Button.UseVisualStyleBackColor = true;
            // 
            // RotPlus90Button
            // 
            RotPlus90Button.Location = new System.Drawing.Point(366, 81);
            RotPlus90Button.Name = "RotPlus90Button";
            RotPlus90Button.Size = new System.Drawing.Size(60, 23);
            RotPlus90Button.TabIndex = 9;
            RotPlus90Button.Tag = 90F;
            RotPlus90Button.Text = "+90";
            RotPlus90Button.UseVisualStyleBackColor = true;
            // 
            // AxesGroup
            // 
            AxesGroup.Controls.Add(UAxisLabel);
            AxesGroup.Controls.Add(UvUX);
            AxesGroup.Controls.Add(UvUY);
            AxesGroup.Controls.Add(UvUZ);
            AxesGroup.Controls.Add(VAxisLabel);
            AxesGroup.Controls.Add(UvVX);
            AxesGroup.Controls.Add(UvVY);
            AxesGroup.Controls.Add(UvVZ);
            AxesGroup.Location = new System.Drawing.Point(12, 408);
            AxesGroup.Name = "AxesGroup";
            AxesGroup.Size = new System.Drawing.Size(446, 96);
            AxesGroup.TabIndex = 3;
            AxesGroup.TabStop = false;
            AxesGroup.Text = "UV vectors";
            // 
            // UAxisLabel
            // 
            UAxisLabel.Location = new System.Drawing.Point(14, 26);
            UAxisLabel.Name = "UAxisLabel";
            UAxisLabel.Size = new System.Drawing.Size(16, 18);
            UAxisLabel.TabIndex = 0;
            UAxisLabel.Text = "U";
            // 
            // UvUX
            // 
            UvUX.BackColor = System.Drawing.SystemColors.Window;
            UvUX.Location = new System.Drawing.Point(36, 23);
            UvUX.Name = "UvUX";
            UvUX.Size = new System.Drawing.Size(74, 23);
            UvUX.TabIndex = 1;
            UvUX.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // UvUY
            // 
            UvUY.BackColor = System.Drawing.SystemColors.Window;
            UvUY.Location = new System.Drawing.Point(116, 23);
            UvUY.Name = "UvUY";
            UvUY.Size = new System.Drawing.Size(74, 23);
            UvUY.TabIndex = 2;
            UvUY.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // UvUZ
            // 
            UvUZ.BackColor = System.Drawing.SystemColors.Window;
            UvUZ.Location = new System.Drawing.Point(196, 23);
            UvUZ.Name = "UvUZ";
            UvUZ.Size = new System.Drawing.Size(74, 23);
            UvUZ.TabIndex = 3;
            UvUZ.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // VAxisLabel
            // 
            VAxisLabel.Location = new System.Drawing.Point(14, 54);
            VAxisLabel.Name = "VAxisLabel";
            VAxisLabel.Size = new System.Drawing.Size(16, 18);
            VAxisLabel.TabIndex = 4;
            VAxisLabel.Text = "V";
            // 
            // UvVX
            // 
            UvVX.BackColor = System.Drawing.SystemColors.Window;
            UvVX.Location = new System.Drawing.Point(36, 51);
            UvVX.Name = "UvVX";
            UvVX.Size = new System.Drawing.Size(74, 23);
            UvVX.TabIndex = 5;
            UvVX.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // UvVY
            // 
            UvVY.BackColor = System.Drawing.SystemColors.Window;
            UvVY.Location = new System.Drawing.Point(116, 51);
            UvVY.Name = "UvVY";
            UvVY.Size = new System.Drawing.Size(74, 23);
            UvVY.TabIndex = 6;
            UvVY.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // UvVZ
            // 
            UvVZ.BackColor = System.Drawing.SystemColors.Window;
            UvVZ.Location = new System.Drawing.Point(196, 51);
            UvVZ.Name = "UvVZ";
            UvVZ.Size = new System.Drawing.Size(74, 23);
            UvVZ.TabIndex = 7;
            UvVZ.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // AlignFaceButton
            // 
            AlignFaceButton.Location = new System.Drawing.Point(12, 512);
            AlignFaceButton.Name = "AlignFaceButton";
            AlignFaceButton.Size = new System.Drawing.Size(104, 27);
            AlignFaceButton.TabIndex = 4;
            AlignFaceButton.Text = "Align to face";
            AlignFaceButton.UseVisualStyleBackColor = true;
            // 
            // AlignWorldButton
            // 
            AlignWorldButton.Location = new System.Drawing.Point(122, 512);
            AlignWorldButton.Name = "AlignWorldButton";
            AlignWorldButton.Size = new System.Drawing.Size(110, 27);
            AlignWorldButton.TabIndex = 5;
            AlignWorldButton.Text = "Align to world";
            AlignWorldButton.UseVisualStyleBackColor = true;
            // 
            // FitTextureButton
            // 
            FitTextureButton.Location = new System.Drawing.Point(238, 512);
            FitTextureButton.Name = "FitTextureButton";
            FitTextureButton.Size = new System.Drawing.Size(100, 27);
            FitTextureButton.TabIndex = 6;
            FitTextureButton.Text = "Fit";
            FitTextureButton.UseVisualStyleBackColor = true;
            // 
            // FitViewButton
            // 
            FitViewButton.Location = new System.Drawing.Point(344, 512);
            FitViewButton.Name = "FitViewButton";
            FitViewButton.Size = new System.Drawing.Size(114, 27);
            FitViewButton.TabIndex = 7;
            FitViewButton.Text = "Fit view";
            FitViewButton.UseVisualStyleBackColor = true;
            // 
            // HintLabel
            // 
            HintLabel.Location = new System.Drawing.Point(12, 546);
            HintLabel.Name = "HintLabel";
            HintLabel.Size = new System.Drawing.Size(446, 36);
            HintLabel.TabIndex = 8;
            HintLabel.Text = "Drag in the view to move the UV layout over the texture.\r\nUse the mouse wheel to zoom.";
            // 
            // UvMappingForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(470, 590);
            Controls.Add(PreviewPanel);
            Controls.Add(StatusLabel);
            Controls.Add(ValuesGroup);
            Controls.Add(AxesGroup);
            Controls.Add(AlignFaceButton);
            Controls.Add(AlignWorldButton);
            Controls.Add(FitTextureButton);
            Controls.Add(FitViewButton);
            Controls.Add(HintLabel);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            Name = "UvMappingForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "UV Mapping";
            ValuesGroup.ResumeLayout(false);
            AxesGroup.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)ShiftXValue).EndInit();
            ((System.ComponentModel.ISupportInitialize)ShiftYValue).EndInit();
            ((System.ComponentModel.ISupportInitialize)ScaleXValue).EndInit();
            ((System.ComponentModel.ISupportInitialize)ScaleYValue).EndInit();
            ((System.ComponentModel.ISupportInitialize)RotationValue).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private UvPreviewPanel PreviewPanel;
        private System.Windows.Forms.Label StatusLabel;
        private System.Windows.Forms.GroupBox ValuesGroup;
        private System.Windows.Forms.Label ShiftLabel;
        private Sledge.Shell.Controls.NumericUpDownEx ShiftXValue;
        private Sledge.Shell.Controls.NumericUpDownEx ShiftYValue;
        private System.Windows.Forms.Label ScaleLabel;
        private Sledge.Shell.Controls.NumericUpDownEx ScaleXValue;
        private Sledge.Shell.Controls.NumericUpDownEx ScaleYValue;
        private System.Windows.Forms.Label RotationLabel;
        private Sledge.Shell.Controls.NumericUpDownEx RotationValue;
        private System.Windows.Forms.Button RotMinus90Button;
        private System.Windows.Forms.Button RotPlus90Button;
        private System.Windows.Forms.GroupBox AxesGroup;
        private System.Windows.Forms.Label UAxisLabel;
        private System.Windows.Forms.TextBox UvUX;
        private System.Windows.Forms.TextBox UvUY;
        private System.Windows.Forms.TextBox UvUZ;
        private System.Windows.Forms.Label VAxisLabel;
        private System.Windows.Forms.TextBox UvVX;
        private System.Windows.Forms.TextBox UvVY;
        private System.Windows.Forms.TextBox UvVZ;
        private System.Windows.Forms.Button AlignFaceButton;
        private System.Windows.Forms.Button AlignWorldButton;
        private System.Windows.Forms.Button FitTextureButton;
        private System.Windows.Forms.Button FitViewButton;
        private System.Windows.Forms.Label HintLabel;
    }
}


