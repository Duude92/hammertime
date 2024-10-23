namespace Sledge.Shell.Forms
{
    partial class ExceptionWindow
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExceptionWindow));
			label1 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			label4 = new System.Windows.Forms.Label();
			FrameworkVersion = new System.Windows.Forms.TextBox();
			label5 = new System.Windows.Forms.Label();
			OperatingSystem = new System.Windows.Forms.TextBox();
			label6 = new System.Windows.Forms.Label();
			SledgeVersion = new System.Windows.Forms.TextBox();
			label7 = new System.Windows.Forms.Label();
			FullError = new System.Windows.Forms.TextBox();
			label8 = new System.Windows.Forms.Label();
			InfoTextBox = new System.Windows.Forms.TextBox();
			SubmitButton = new System.Windows.Forms.Button();
			CancelButton = new System.Windows.Forms.Button();
			label3 = new System.Windows.Forms.Label();
			applicationBranch = new System.Windows.Forms.TextBox();
			SuspendLayout();
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
			label1.Location = new System.Drawing.Point(14, 10);
			label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(411, 25);
			label1.TabIndex = 0;
			label1.Text = "Oops! Something went horribly wrong.";
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(16, 52);
			label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(482, 135);
			label2.TabIndex = 1;
			label2.Text = resources.GetString("label2.Text");
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.Location = new System.Drawing.Point(16, 208);
			label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(72, 15);
			label4.TabIndex = 2;
			label4.Text = ".NET Version";
			// 
			// FrameworkVersion
			// 
			FrameworkVersion.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			FrameworkVersion.Location = new System.Drawing.Point(215, 204);
			FrameworkVersion.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			FrameworkVersion.Name = "FrameworkVersion";
			FrameworkVersion.ReadOnly = true;
			FrameworkVersion.Size = new System.Drawing.Size(335, 23);
			FrameworkVersion.TabIndex = 3;
			// 
			// label5
			// 
			label5.AutoSize = true;
			label5.Location = new System.Drawing.Point(16, 238);
			label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label5.Name = "label5";
			label5.Size = new System.Drawing.Size(101, 15);
			label5.TabIndex = 2;
			label5.Text = "Operating System";
			// 
			// OperatingSystem
			// 
			OperatingSystem.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			OperatingSystem.Location = new System.Drawing.Point(215, 234);
			OperatingSystem.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			OperatingSystem.Name = "OperatingSystem";
			OperatingSystem.ReadOnly = true;
			OperatingSystem.Size = new System.Drawing.Size(335, 23);
			OperatingSystem.TabIndex = 3;
			// 
			// label6
			// 
			label6.AutoSize = true;
			label6.Location = new System.Drawing.Point(16, 268);
			label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label6.Name = "label6";
			label6.Size = new System.Drawing.Size(83, 15);
			label6.TabIndex = 2;
			label6.Text = "Sledge Version";
			// 
			// SledgeVersion
			// 
			SledgeVersion.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			SledgeVersion.Location = new System.Drawing.Point(215, 264);
			SledgeVersion.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			SledgeVersion.Name = "SledgeVersion";
			SledgeVersion.ReadOnly = true;
			SledgeVersion.Size = new System.Drawing.Size(335, 23);
			SledgeVersion.TabIndex = 3;
			// 
			// label7
			// 
			label7.AutoSize = true;
			label7.Location = new System.Drawing.Point(16, 298);
			label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label7.Name = "label7";
			label7.Size = new System.Drawing.Size(103, 15);
			label7.TabIndex = 2;
			label7.Text = "Full Error Message";
			// 
			// FullError
			// 
			FullError.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			FullError.Location = new System.Drawing.Point(215, 294);
			FullError.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			FullError.Multiline = true;
			FullError.Name = "FullError";
			FullError.ReadOnly = true;
			FullError.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			FullError.Size = new System.Drawing.Size(335, 73);
			FullError.TabIndex = 3;
			// 
			// label8
			// 
			label8.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
			label8.Enabled = false;
			label8.Location = new System.Drawing.Point(16, 375);
			label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label8.Name = "label8";
			label8.Size = new System.Drawing.Size(191, 57);
			label8.TabIndex = 2;
			label8.Text = "Can you tell us any extra information about what you were doing when this error happened?";
			label8.Visible = false;
			// 
			// InfoTextBox
			// 
			InfoTextBox.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			InfoTextBox.Location = new System.Drawing.Point(215, 375);
			InfoTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			InfoTextBox.Multiline = true;
			InfoTextBox.Name = "InfoTextBox";
			InfoTextBox.Size = new System.Drawing.Size(335, 72);
			InfoTextBox.TabIndex = 3;
			InfoTextBox.Visible = false;
			// 
			// SubmitButton
			// 
			SubmitButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			SubmitButton.Enabled = false;
			SubmitButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
			SubmitButton.Location = new System.Drawing.Point(369, 455);
			SubmitButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			SubmitButton.Name = "SubmitButton";
			SubmitButton.Size = new System.Drawing.Size(182, 48);
			SubmitButton.TabIndex = 4;
			SubmitButton.Text = "Submit Error!";
			SubmitButton.UseVisualStyleBackColor = true;
			SubmitButton.Click += SubmitButtonClicked;
			// 
			// CancelButton
			// 
			CancelButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
			CancelButton.Location = new System.Drawing.Point(14, 477);
			CancelButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			CancelButton.Name = "CancelButton";
			CancelButton.Size = new System.Drawing.Size(141, 27);
			CancelButton.TabIndex = 4;
			CancelButton.Text = "Don't Submit Error :(";
			CancelButton.UseVisualStyleBackColor = true;
			CancelButton.Click += CancelButtonClicked;
			// 
			// label3
			// 
			label3.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(16, 375);
			label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(108, 15);
			label3.TabIndex = 5;
			label3.Text = "Application Branch";
			// 
			// applicationBranch
			// 
			applicationBranch.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			applicationBranch.Location = new System.Drawing.Point(214, 375);
			applicationBranch.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			applicationBranch.Name = "applicationBranch";
			applicationBranch.ReadOnly = true;
			applicationBranch.Size = new System.Drawing.Size(336, 23);
			applicationBranch.TabIndex = 6;
			// 
			// ExceptionWindow
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			ClientSize = new System.Drawing.Size(565, 517);
			Controls.Add(applicationBranch);
			Controls.Add(label3);
			Controls.Add(CancelButton);
			Controls.Add(SubmitButton);
			Controls.Add(InfoTextBox);
			Controls.Add(label8);
			Controls.Add(FullError);
			Controls.Add(label7);
			Controls.Add(SledgeVersion);
			Controls.Add(label6);
			Controls.Add(OperatingSystem);
			Controls.Add(label5);
			Controls.Add(FrameworkVersion);
			Controls.Add(label4);
			Controls.Add(label2);
			Controls.Add(label1);
			Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			MaximizeBox = false;
			MinimizeBox = false;
			MinimumSize = new System.Drawing.Size(581, 555);
			Name = "ExceptionWindow";
			StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			Text = "This isn't good!";
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox FrameworkVersion;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox OperatingSystem;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox SledgeVersion;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox FullError;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox InfoTextBox;
        private System.Windows.Forms.Button SubmitButton;
        private System.Windows.Forms.Button CancelButton;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox applicationBranch;
	}
}