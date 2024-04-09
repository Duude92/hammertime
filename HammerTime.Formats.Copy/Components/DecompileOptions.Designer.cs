namespace HammerTime.Formats.Components
{
	partial class DecompileOptions
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
			brushOptimizationBox = new System.Windows.Forms.ComboBox();
			originBoxCheckBox = new System.Windows.Forms.CheckBox();
			mergeBrushesCheckBox = new System.Windows.Forms.CheckBox();
			applyNullCheckBox = new System.Windows.Forms.CheckBox();
			includeLiquidsCheckBox = new System.Windows.Forms.CheckBox();
			strategyComboBox = new System.Windows.Forms.ComboBox();
			textBox1 = new System.Windows.Forms.TextBox();
			label1 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			cancelButton = new System.Windows.Forms.Button();
			openBsp = new System.Windows.Forms.Button();
			SuspendLayout();
			// 
			// brushOptimizationBox
			// 
			brushOptimizationBox.FormattingEnabled = true;
			brushOptimizationBox.Items.AddRange(new object[] { "Best texture match", "Fewest brushes" });
			brushOptimizationBox.Location = new System.Drawing.Point(12, 43);
			brushOptimizationBox.Name = "brushOptimizationBox";
			brushOptimizationBox.Size = new System.Drawing.Size(179, 23);
			brushOptimizationBox.TabIndex = 0;
			// 
			// originBoxCheckBox
			// 
			originBoxCheckBox.AutoSize = true;
			originBoxCheckBox.Location = new System.Drawing.Point(12, 72);
			originBoxCheckBox.Name = "originBoxCheckBox";
			originBoxCheckBox.Size = new System.Drawing.Size(179, 19);
			originBoxCheckBox.TabIndex = 1;
			originBoxCheckBox.Text = "Always generate origin brush";
			originBoxCheckBox.UseVisualStyleBackColor = true;
			// 
			// mergeBrushesCheckBox
			// 
			mergeBrushesCheckBox.AutoSize = true;
			mergeBrushesCheckBox.Location = new System.Drawing.Point(12, 97);
			mergeBrushesCheckBox.Name = "mergeBrushesCheckBox";
			mergeBrushesCheckBox.Size = new System.Drawing.Size(104, 19);
			mergeBrushesCheckBox.TabIndex = 2;
			mergeBrushesCheckBox.Text = "Merge brushes";
			mergeBrushesCheckBox.UseVisualStyleBackColor = true;
			// 
			// applyNullCheckBox
			// 
			applyNullCheckBox.AutoSize = true;
			applyNullCheckBox.Location = new System.Drawing.Point(12, 122);
			applyNullCheckBox.Name = "applyNullCheckBox";
			applyNullCheckBox.Size = new System.Drawing.Size(182, 19);
			applyNullCheckBox.TabIndex = 3;
			applyNullCheckBox.Text = "Apply Null to generated faces";
			applyNullCheckBox.UseVisualStyleBackColor = true;
			// 
			// includeLiquidsCheckBox
			// 
			includeLiquidsCheckBox.AutoSize = true;
			includeLiquidsCheckBox.Location = new System.Drawing.Point(12, 147);
			includeLiquidsCheckBox.Name = "includeLiquidsCheckBox";
			includeLiquidsCheckBox.Size = new System.Drawing.Size(106, 19);
			includeLiquidsCheckBox.TabIndex = 4;
			includeLiquidsCheckBox.Text = "Include Liquids";
			includeLiquidsCheckBox.UseVisualStyleBackColor = true;
			// 
			// strategyComboBox
			// 
			strategyComboBox.FormattingEnabled = true;
			strategyComboBox.Items.AddRange(new object[] { "Tree decompiler strategy", "Face-To-Brush decompiler strategy" });
			strategyComboBox.Location = new System.Drawing.Point(240, 43);
			strategyComboBox.Name = "strategyComboBox";
			strategyComboBox.Size = new System.Drawing.Size(184, 23);
			strategyComboBox.TabIndex = 5;
			// 
			// textBox1
			// 
			textBox1.Location = new System.Drawing.Point(240, 70);
			textBox1.Multiline = true;
			textBox1.Name = "textBox1";
			textBox1.Size = new System.Drawing.Size(184, 95);
			textBox1.TabIndex = 6;
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			label1.Location = new System.Drawing.Point(12, 9);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(144, 21);
			label1.TabIndex = 7;
			label1.Text = "Brush optimization:";
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			label2.Location = new System.Drawing.Point(240, 14);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(174, 21);
			label2.TabIndex = 8;
			label2.Text = "Decompilation strategy:";
			// 
			// cancelButton
			// 
			cancelButton.Location = new System.Drawing.Point(349, 180);
			cancelButton.Name = "cancelButton";
			cancelButton.Size = new System.Drawing.Size(75, 23);
			cancelButton.TabIndex = 9;
			cancelButton.Text = "Cancel";
			cancelButton.UseVisualStyleBackColor = true;
			cancelButton.Click += cancelButton_Click;
			// 
			// openBsp
			// 
			openBsp.Location = new System.Drawing.Point(240, 180);
			openBsp.Name = "openBsp";
			openBsp.Size = new System.Drawing.Size(75, 23);
			openBsp.TabIndex = 10;
			openBsp.Text = "Open BSP";
			openBsp.UseVisualStyleBackColor = true;
			openBsp.Click += openBsp_Click;
			// 
			// DecompileOptions
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			ClientSize = new System.Drawing.Size(450, 215);
			Controls.Add(openBsp);
			Controls.Add(cancelButton);
			Controls.Add(label2);
			Controls.Add(label1);
			Controls.Add(textBox1);
			Controls.Add(strategyComboBox);
			Controls.Add(includeLiquidsCheckBox);
			Controls.Add(applyNullCheckBox);
			Controls.Add(mergeBrushesCheckBox);
			Controls.Add(originBoxCheckBox);
			Controls.Add(brushOptimizationBox);
			FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "DecompileOptions";
			ShowIcon = false;
			ShowInTaskbar = false;
			Text = "Decompile options";
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private System.Windows.Forms.ComboBox brushOptimizationBox;
		private System.Windows.Forms.CheckBox originBoxCheckBox;
		private System.Windows.Forms.CheckBox mergeBrushesCheckBox;
		private System.Windows.Forms.CheckBox applyNullCheckBox;
		private System.Windows.Forms.CheckBox includeLiquidsCheckBox;
		private System.Windows.Forms.ComboBox strategyComboBox;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button openBsp;
	}
}