namespace Sledge.BspEditor.Editing.Components
{
    partial class AboutDialog
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
			button1 = new System.Windows.Forms.Button();
			pictureBox1 = new System.Windows.Forms.PictureBox();
			label1 = new System.Windows.Forms.Label();
			VersionLabel = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			LTLink = new System.Windows.Forms.LinkLabel();
			label3 = new System.Windows.Forms.Label();
			GithubLink = new System.Windows.Forms.LinkLabel();
			GPLLink = new System.Windows.Forms.LinkLabel();
			label4 = new System.Windows.Forms.Label();
			label5 = new System.Windows.Forms.Label();
			label6 = new System.Windows.Forms.Label();
			label7 = new System.Windows.Forms.Label();
			label8 = new System.Windows.Forms.Label();
			label9 = new System.Windows.Forms.Label();
			TWHLLink = new System.Windows.Forms.LinkLabel();
			label10 = new System.Windows.Forms.Label();
			AJLink = new System.Windows.Forms.LinkLabel();
			label11 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
			SuspendLayout();
			// 
			// button1
			// 
			button1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			button1.DialogResult = System.Windows.Forms.DialogResult.OK;
			button1.Location = new System.Drawing.Point(248, 321);
			button1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			button1.Name = "button1";
			button1.Size = new System.Drawing.Size(153, 27);
			button1.TabIndex = 1;
			button1.Text = "Close";
			button1.UseVisualStyleBackColor = true;
			// 
			// pictureBox1
			// 
			pictureBox1.Image = Editing.Properties.Resources.sledge_large;
			pictureBox1.Location = new System.Drawing.Point(14, 14);
			pictureBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			pictureBox1.Name = "pictureBox1";
			pictureBox1.Size = new System.Drawing.Size(256, 256);
			pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			pictureBox1.TabIndex = 2;
			pictureBox1.TabStop = false;
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Font = new System.Drawing.Font("Georgia", 30F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			label1.Location = new System.Drawing.Point(288, 14);
			label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(258, 46);
			label1.TabIndex = 3;
			label1.Text = "Hammertime";
			// 
			// VersionLabel
			// 
			VersionLabel.AutoSize = true;
			VersionLabel.Location = new System.Drawing.Point(544, 44);
			VersionLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			VersionLabel.Name = "VersionLabel";
			VersionLabel.Size = new System.Drawing.Size(53, 15);
			VersionLabel.TabIndex = 4;
			VersionLabel.Text = "(version)";
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(334, 91);
			label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(151, 15);
			label2.TabIndex = 5;
			label2.Text = "Created by Daniel Walder - ";
			// 
			// LTLink
			// 
			LTLink.AutoSize = true;
			LTLink.Location = new System.Drawing.Point(490, 91);
			LTLink.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			LTLink.Name = "LTLink";
			LTLink.Size = new System.Drawing.Size(113, 15);
			LTLink.TabIndex = 6;
			LTLink.TabStop = true;
			LTLink.Text = "logic-and-trick.com";
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(334, 119);
			label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(245, 15);
			label3.TabIndex = 5;
			label3.Text = "Open source software - licensed under BSD-3";
			// 
			// GithubLink
			// 
			GithubLink.AutoSize = true;
			GithubLink.Location = new System.Drawing.Point(406, 143);
			GithubLink.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			GithubLink.Name = "GithubLink";
			GithubLink.Size = new System.Drawing.Size(195, 15);
			GithubLink.TabIndex = 6;
			GithubLink.TabStop = true;
			GithubLink.Text = "github.com/Duude92/hammertime";
			// 
			// GPLLink
			// 
			GPLLink.AutoSize = true;
			GPLLink.Location = new System.Drawing.Point(406, 168);
			GPLLink.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			GPLLink.Name = "GPLLink";
			GPLLink.Size = new System.Drawing.Size(213, 15);
			GPLLink.TabIndex = 6;
			GPLLink.TabStop = true;
			GPLLink.Text = "opensource.org/licenses/BSD-3-Clause";
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.Location = new System.Drawing.Point(344, 143);
			label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(46, 15);
			label4.TabIndex = 5;
			label4.Text = "Source:";
			// 
			// label5
			// 
			label5.AutoSize = true;
			label5.Location = new System.Drawing.Point(344, 168);
			label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label5.Name = "label5";
			label5.Size = new System.Drawing.Size(49, 15);
			label5.TabIndex = 5;
			label5.Text = "License:";
			// 
			// label6
			// 
			label6.AutoSize = true;
			label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
			label6.Location = new System.Drawing.Point(332, 204);
			label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label6.Name = "label6";
			label6.Size = new System.Drawing.Size(95, 13);
			label6.TabIndex = 7;
			label6.Text = "Special Thanks";
			// 
			// label7
			// 
			label7.AutoSize = true;
			label7.Location = new System.Drawing.Point(344, 227);
			label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label7.Name = "label7";
			label7.Size = new System.Drawing.Size(245, 15);
			label7.TabIndex = 8;
			label7.Text = "Strider && Instant Mix (from TWHL) - Graphics";
			// 
			// label8
			// 
			label8.AutoSize = true;
			label8.Location = new System.Drawing.Point(344, 248);
			label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label8.Name = "label8";
			label8.Size = new System.Drawing.Size(133, 15);
			label8.TabIndex = 8;
			label8.Text = "AJ Scarcella - Website - ";
			// 
			// label9
			// 
			label9.AutoSize = true;
			label9.Location = new System.Drawing.Point(344, 269);
			label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label9.Name = "label9";
			label9.Size = new System.Drawing.Size(187, 15);
			label9.TabIndex = 8;
			label9.Text = "The members of TWHL - Testing - ";
			// 
			// TWHLLink
			// 
			TWHLLink.AutoSize = true;
			TWHLLink.Location = new System.Drawing.Point(541, 269);
			TWHLLink.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			TWHLLink.Name = "TWHLLink";
			TWHLLink.Size = new System.Drawing.Size(54, 15);
			TWHLLink.TabIndex = 6;
			TWHLLink.TabStop = true;
			TWHLLink.Text = "twhl.info";
			// 
			// label10
			// 
			label10.AutoSize = true;
			label10.Location = new System.Drawing.Point(344, 290);
			label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label10.Name = "label10";
			label10.Size = new System.Drawing.Size(263, 15);
			label10.TabIndex = 8;
			label10.Text = "...and everyone else in the modding community!";
			// 
			// AJLink
			// 
			AJLink.AutoSize = true;
			AJLink.Location = new System.Drawing.Point(485, 248);
			AJLink.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			AJLink.Name = "AJLink";
			AJLink.Size = new System.Drawing.Size(106, 15);
			AJLink.TabIndex = 6;
			AJLink.TabStop = true;
			AJLink.Text = "scrub-studios.com";
			// 
			// label11
			// 
			label11.AutoSize = true;
			label11.Location = new System.Drawing.Point(334, 66);
			label11.Name = "label11";
			label11.Size = new System.Drawing.Size(313, 15);
			label11.TabIndex = 9;
			label11.Text = "Hammertime fixes and new features by Yurii Hladyshenko";
			// 
			// AboutDialog
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			ClientSize = new System.Drawing.Size(653, 352);
			Controls.Add(label11);
			Controls.Add(label10);
			Controls.Add(label9);
			Controls.Add(label8);
			Controls.Add(label7);
			Controls.Add(label6);
			Controls.Add(GPLLink);
			Controls.Add(GithubLink);
			Controls.Add(label3);
			Controls.Add(AJLink);
			Controls.Add(TWHLLink);
			Controls.Add(LTLink);
			Controls.Add(label5);
			Controls.Add(label4);
			Controls.Add(label2);
			Controls.Add(VersionLabel);
			Controls.Add(label1);
			Controls.Add(pictureBox1);
			Controls.Add(button1);
			FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "AboutDialog";
			ShowIcon = false;
			ShowInTaskbar = false;
			StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			Text = "About Hammertime";
			((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private System.Windows.Forms.Button button1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label VersionLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.LinkLabel LTLink;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.LinkLabel GithubLink;
        private System.Windows.Forms.LinkLabel GPLLink;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.LinkLabel TWHLLink;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.LinkLabel AJLink;
		private System.Windows.Forms.Label label11;
	}
}