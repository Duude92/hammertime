namespace Sledge.BspEditor.Tools.PathTool.Forms
{
    partial class NodeProperties
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
			OKButton = new System.Windows.Forms.Button();
			label1 = new System.Windows.Forms.Label();
			nameBox = new System.Windows.Forms.TextBox();
			CancelButton = new System.Windows.Forms.Button();
			label2 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			speedBox = new System.Windows.Forms.TextBox();
			yawBox = new System.Windows.Forms.TextBox();
			waitBox = new System.Windows.Forms.TextBox();
			label4 = new System.Windows.Forms.Label();
			retriggerCheck = new System.Windows.Forms.CheckBox();
			SuspendLayout();
			// 
			// OKButton
			// 
			OKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			OKButton.Location = new System.Drawing.Point(34, 194);
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
			label1.Location = new System.Drawing.Point(12, 21);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(88, 15);
			label1.TabIndex = 15;
			label1.Text = "Name override:";
			// 
			// nameBox
			// 
			nameBox.Location = new System.Drawing.Point(12, 39);
			nameBox.Name = "nameBox";
			nameBox.Size = new System.Drawing.Size(166, 23);
			nameBox.TabIndex = 17;
			// 
			// CancelButton
			// 
			CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			CancelButton.Location = new System.Drawing.Point(130, 194);
			CancelButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			CancelButton.Name = "CancelButton";
			CancelButton.Size = new System.Drawing.Size(88, 27);
			CancelButton.TabIndex = 14;
			CancelButton.Text = "Cancel";
			CancelButton.UseVisualStyleBackColor = true;
			CancelButton.Click += CancelClicked;
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(12, 74);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(39, 15);
			label2.TabIndex = 18;
			label2.Text = "Speed";
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(106, 74);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(62, 15);
			label3.TabIndex = 19;
			label3.Text = "Yaw speed";
			// 
			// speedBox
			// 
			speedBox.Location = new System.Drawing.Point(12, 92);
			speedBox.Name = "speedBox";
			speedBox.Size = new System.Drawing.Size(76, 23);
			speedBox.TabIndex = 20;
			// 
			// yawBox
			// 
			yawBox.Location = new System.Drawing.Point(106, 92);
			yawBox.Name = "yawBox";
			yawBox.Size = new System.Drawing.Size(76, 23);
			yawBox.TabIndex = 21;
			// 
			// waitBox
			// 
			waitBox.Location = new System.Drawing.Point(12, 144);
			waitBox.Name = "waitBox";
			waitBox.Size = new System.Drawing.Size(76, 23);
			waitBox.TabIndex = 23;
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.Location = new System.Drawing.Point(12, 126);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(31, 15);
			label4.TabIndex = 22;
			label4.Text = "Wait";
			// 
			// retriggerCheck
			// 
			retriggerCheck.AutoSize = true;
			retriggerCheck.Location = new System.Drawing.Point(102, 151);
			retriggerCheck.Name = "retriggerCheck";
			retriggerCheck.Size = new System.Drawing.Size(116, 19);
			retriggerCheck.TabIndex = 24;
			retriggerCheck.Text = "Wait for retrigger";
			retriggerCheck.UseVisualStyleBackColor = true;
			// 
			// NodeProperties
			// 
			AcceptButton = OKButton;
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			ClientSize = new System.Drawing.Size(224, 235);
			Controls.Add(retriggerCheck);
			Controls.Add(waitBox);
			Controls.Add(label4);
			Controls.Add(yawBox);
			Controls.Add(speedBox);
			Controls.Add(label3);
			Controls.Add(label2);
			Controls.Add(nameBox);
			Controls.Add(label1);
			Controls.Add(OKButton);
			Controls.Add(CancelButton);
			FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "NodeProperties";
			ShowInTaskbar = false;
			Text = "Node properties";
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
        private System.Windows.Forms.CheckBox RescaleTextures;
        private System.Windows.Forms.Button OKButton;
        private System.Windows.Forms.TextBox FindTextbox;
        private System.Windows.Forms.TextBox ReplaceTextbox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox nameBox;
		private System.Windows.Forms.Button CancelButton;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox speedBox;
		private System.Windows.Forms.TextBox yawBox;
		private System.Windows.Forms.TextBox waitBox;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.CheckBox retriggerCheck;
	}
}