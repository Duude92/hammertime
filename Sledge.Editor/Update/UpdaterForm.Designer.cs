namespace Sledge.Editor.Update
{
    partial class UpdaterForm
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
			StatusLabel = new System.Windows.Forms.Label();
			ProgressBar = new System.Windows.Forms.ProgressBar();
			CancelButton = new System.Windows.Forms.Button();
			StartButton = new System.Windows.Forms.Button();
			ReleaseDetails = new System.Windows.Forms.TextBox();
			ReleaseNotesLink = new System.Windows.Forms.LinkLabel();
			label1 = new System.Windows.Forms.Label();
			archBox = new System.Windows.Forms.ComboBox();
			SuspendLayout();
			// 
			// StatusLabel
			// 
			StatusLabel.AutoSize = true;
			StatusLabel.Location = new System.Drawing.Point(14, 10);
			StatusLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			StatusLabel.Name = "StatusLabel";
			StatusLabel.Size = new System.Drawing.Size(70, 15);
			StatusLabel.TabIndex = 0;
			StatusLabel.Text = "Status Label";
			// 
			// ProgressBar
			// 
			ProgressBar.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			ProgressBar.Location = new System.Drawing.Point(14, 284);
			ProgressBar.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			ProgressBar.Name = "ProgressBar";
			ProgressBar.Size = new System.Drawing.Size(342, 12);
			ProgressBar.Step = 1;
			ProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			ProgressBar.TabIndex = 1;
			// 
			// CancelButton
			// 
			CancelButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			CancelButton.Location = new System.Drawing.Point(363, 269);
			CancelButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			CancelButton.Name = "CancelButton";
			CancelButton.Size = new System.Drawing.Size(88, 27);
			CancelButton.TabIndex = 2;
			CancelButton.Text = "Cancel";
			CancelButton.UseVisualStyleBackColor = true;
			CancelButton.Click += CancelButtonClicked;
			// 
			// StartButton
			// 
			StartButton.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
			StartButton.Location = new System.Drawing.Point(363, 14);
			StartButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			StartButton.Name = "StartButton";
			StartButton.Size = new System.Drawing.Size(88, 27);
			StartButton.TabIndex = 2;
			StartButton.Text = "Download";
			StartButton.UseVisualStyleBackColor = true;
			StartButton.Click += DownloadButtonClicked;
			// 
			// ReleaseDetails
			// 
			ReleaseDetails.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			ReleaseDetails.BackColor = System.Drawing.SystemColors.ControlLightLight;
			ReleaseDetails.Location = new System.Drawing.Point(18, 70);
			ReleaseDetails.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			ReleaseDetails.Multiline = true;
			ReleaseDetails.Name = "ReleaseDetails";
			ReleaseDetails.ReadOnly = true;
			ReleaseDetails.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			ReleaseDetails.Size = new System.Drawing.Size(432, 173);
			ReleaseDetails.TabIndex = 3;
			// 
			// ReleaseNotesLink
			// 
			ReleaseNotesLink.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
			ReleaseNotesLink.AutoSize = true;
			ReleaseNotesLink.Location = new System.Drawing.Point(14, 252);
			ReleaseNotesLink.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			ReleaseNotesLink.Name = "ReleaseNotesLink";
			ReleaseNotesLink.Size = new System.Drawing.Size(274, 15);
			ReleaseNotesLink.TabIndex = 7;
			ReleaseNotesLink.TabStop = true;
			ReleaseNotesLink.Text = "Click here to see release notes for previous releases";
			ReleaseNotesLink.LinkClicked += ReleaseNotesLinkClicked;
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(281, 47);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(75, 15);
			label1.TabIndex = 8;
			label1.Text = "Architecture:";
			// 
			// archBox
			// 
			archBox.FormattingEnabled = true;
			archBox.Items.AddRange(new object[] { "x86", "x64" });
			archBox.Location = new System.Drawing.Point(363, 44);
			archBox.Name = "archBox";
			archBox.Size = new System.Drawing.Size(87, 23);
			archBox.TabIndex = 9;
			archBox.Text = "x86";
			archBox.SelectedIndexChanged += archBox_SelectedIndexChanged;
			// 
			// UpdaterForm
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			ClientSize = new System.Drawing.Size(464, 307);
			Controls.Add(archBox);
			Controls.Add(label1);
			Controls.Add(ReleaseNotesLink);
			Controls.Add(ReleaseDetails);
			Controls.Add(StartButton);
			Controls.Add(CancelButton);
			Controls.Add(ProgressBar);
			Controls.Add(StatusLabel);
			FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "UpdaterForm";
			ShowInTaskbar = false;
			StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			Text = "Sledge Updater";
			FormClosing += UpdaterFormFormClosing;
			ResumeLayout(false);
			PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label StatusLabel;
        private System.Windows.Forms.ProgressBar ProgressBar;
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.Button StartButton;
        private System.Windows.Forms.TextBox ReleaseDetails;
        private System.Windows.Forms.LinkLabel ReleaseNotesLink;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox archBox;
	}
}

