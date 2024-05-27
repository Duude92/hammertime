namespace Sledge.BspEditor.Tools.PathTool.Forms
{
    partial class PathProperties
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
			DirectionGroup = new System.Windows.Forms.GroupBox();
			PP = new System.Windows.Forms.RadioButton();
			Circular = new System.Windows.Forms.RadioButton();
			OneWay = new System.Windows.Forms.RadioButton();
			OKButton = new System.Windows.Forms.Button();
			label1 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			nameBox = new System.Windows.Forms.TextBox();
			classBox = new System.Windows.Forms.ComboBox();
			CancelButton = new System.Windows.Forms.Button();
			DirectionGroup.SuspendLayout();
			SuspendLayout();
			// 
			// DirectionGroup
			// 
			DirectionGroup.Controls.Add(PP);
			DirectionGroup.Controls.Add(Circular);
			DirectionGroup.Controls.Add(OneWay);
			DirectionGroup.Location = new System.Drawing.Point(185, 12);
			DirectionGroup.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			DirectionGroup.Name = "DirectionGroup";
			DirectionGroup.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
			DirectionGroup.Size = new System.Drawing.Size(170, 99);
			DirectionGroup.TabIndex = 1;
			DirectionGroup.TabStop = false;
			DirectionGroup.Text = "Direction";
			// 
			// PP
			// 
			PP.AutoSize = true;
			PP.Location = new System.Drawing.Point(10, 75);
			PP.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			PP.Name = "PP";
			PP.Size = new System.Drawing.Size(82, 19);
			PP.TabIndex = 11;
			PP.Text = "Ping-pong";
			PP.UseVisualStyleBackColor = true;
			// 
			// Circular
			// 
			Circular.AutoSize = true;
			Circular.Location = new System.Drawing.Point(10, 48);
			Circular.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			Circular.Name = "Circular";
			Circular.Size = new System.Drawing.Size(66, 19);
			Circular.TabIndex = 10;
			Circular.Text = "Circular";
			Circular.UseVisualStyleBackColor = true;
			// 
			// OneWay
			// 
			OneWay.AutoSize = true;
			OneWay.Checked = true;
			OneWay.Location = new System.Drawing.Point(10, 22);
			OneWay.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			OneWay.Name = "OneWay";
			OneWay.Size = new System.Drawing.Size(71, 19);
			OneWay.TabIndex = 9;
			OneWay.TabStop = true;
			OneWay.Text = "One way";
			OneWay.UseVisualStyleBackColor = true;
			// 
			// OKButton
			// 
			OKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			OKButton.Location = new System.Drawing.Point(171, 122);
			OKButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			OKButton.Name = "OKButton";
			OKButton.Size = new System.Drawing.Size(88, 27);
			OKButton.TabIndex = 13;
			OKButton.Text = "OK";
			OKButton.UseVisualStyleBackColor = true;
			OKButton.Click += OkClicked;
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(8, 21);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(42, 15);
			label1.TabIndex = 15;
			label1.Text = "Name:";
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(8, 75);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(37, 15);
			label2.TabIndex = 16;
			label2.Text = "Class:";
			// 
			// nameBox
			// 
			nameBox.Location = new System.Drawing.Point(12, 39);
			nameBox.Name = "nameBox";
			nameBox.Size = new System.Drawing.Size(166, 23);
			nameBox.TabIndex = 17;
			// 
			// classBox
			// 
			classBox.FormattingEnabled = true;
			classBox.Items.AddRange(new object[] { "path_corner", "path_track" });
			classBox.Location = new System.Drawing.Point(12, 93);
			classBox.Name = "classBox";
			classBox.Size = new System.Drawing.Size(166, 23);
			classBox.Sorted = true;
			classBox.TabIndex = 18;
			// 
			// CancelButton
			// 
			CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			CancelButton.Location = new System.Drawing.Point(267, 122);
			CancelButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			CancelButton.Name = "CancelButton";
			CancelButton.Size = new System.Drawing.Size(88, 27);
			CancelButton.TabIndex = 14;
			CancelButton.Text = "Cancel";
			CancelButton.UseVisualStyleBackColor = true;
			CancelButton.Click += CancelClicked;
			// 
			// PathProperties
			// 
			AcceptButton = OKButton;
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			ClientSize = new System.Drawing.Size(361, 157);
			Controls.Add(classBox);
			Controls.Add(nameBox);
			Controls.Add(label2);
			Controls.Add(label1);
			Controls.Add(OKButton);
			Controls.Add(CancelButton);
			Controls.Add(DirectionGroup);
			FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "PathProperties";
			ShowInTaskbar = false;
			Text = "New Path";
			DirectionGroup.ResumeLayout(false);
			DirectionGroup.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private System.Windows.Forms.GroupBox FindGroup;
        private System.Windows.Forms.Button FindBrowse;
        private System.Windows.Forms.PictureBox FindImage;
        private System.Windows.Forms.Label FindInfo;
        private System.Windows.Forms.Label ReplaceInfo;
        private System.Windows.Forms.PictureBox ReplaceImage;
        private System.Windows.Forms.Button ReplaceBrowse;
        private System.Windows.Forms.GroupBox ReplaceInGroup;
        private System.Windows.Forms.RadioButton ReplaceEverything;
        private System.Windows.Forms.RadioButton ReplaceVisible;
        private System.Windows.Forms.RadioButton ReplaceSelection;
        private System.Windows.Forms.GroupBox DirectionGroup;
        private System.Windows.Forms.RadioButton PP;
        private System.Windows.Forms.RadioButton Circular;
        private System.Windows.Forms.RadioButton OneWay;
        private System.Windows.Forms.CheckBox RescaleTextures;
        private System.Windows.Forms.Button OKButton;
        private System.Windows.Forms.TextBox FindTextbox;
        private System.Windows.Forms.TextBox ReplaceTextbox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox nameBox;
		private System.Windows.Forms.ComboBox classBox;
		private System.Windows.Forms.Button CancelButton;
	}
}