namespace Sledge.Shell.Forms
{
    partial class SaveChangesForm1
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
			CancelButton = new System.Windows.Forms.Button();
			DiscardButton = new System.Windows.Forms.Button();
			SaveAllButton = new System.Windows.Forms.Button();
			DocumentList = new System.Windows.Forms.ListBox();
			UnsavedChangesLabel = new System.Windows.Forms.Label();
			SuspendLayout();
			// 
			// CancelButton
			// 
			CancelButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			CancelButton.Location = new System.Drawing.Point(314, 213);
			CancelButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			CancelButton.Name = "CancelButton";
			CancelButton.Size = new System.Drawing.Size(88, 27);
			CancelButton.TabIndex = 0;
			CancelButton.Text = "Cancel";
			CancelButton.UseVisualStyleBackColor = true;
			CancelButton.Click += CancelClicked;
			// 
			// DiscardButton
			// 
			DiscardButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			DiscardButton.Location = new System.Drawing.Point(219, 213);
			DiscardButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			DiscardButton.Name = "DiscardButton";
			DiscardButton.Size = new System.Drawing.Size(88, 27);
			DiscardButton.TabIndex = 0;
			DiscardButton.Text = "Discard all";
			DiscardButton.UseVisualStyleBackColor = true;
			DiscardButton.Click += DiscardAllClicked;
			// 
			// SaveAllButton
			// 
			SaveAllButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			SaveAllButton.Location = new System.Drawing.Point(125, 213);
			SaveAllButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			SaveAllButton.Name = "SaveAllButton";
			SaveAllButton.Size = new System.Drawing.Size(88, 27);
			SaveAllButton.TabIndex = 0;
			SaveAllButton.Text = "Save all";
			SaveAllButton.UseVisualStyleBackColor = true;
			SaveAllButton.Click += SaveAllClicked;
			// 
			// DocumentList
			// 
			DocumentList.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			DocumentList.FormattingEnabled = true;
			DocumentList.IntegralHeight = false;
			DocumentList.ItemHeight = 15;
			DocumentList.Location = new System.Drawing.Point(14, 54);
			DocumentList.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			DocumentList.Name = "DocumentList";
			DocumentList.Size = new System.Drawing.Size(387, 152);
			DocumentList.TabIndex = 1;
			// 
			// UnsavedChangesLabel
			// 
			UnsavedChangesLabel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			UnsavedChangesLabel.Location = new System.Drawing.Point(14, 14);
			UnsavedChangesLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			UnsavedChangesLabel.Name = "UnsavedChangesLabel";
			UnsavedChangesLabel.Size = new System.Drawing.Size(387, 37);
			UnsavedChangesLabel.TabIndex = 2;
			UnsavedChangesLabel.Text = "Some documents have unsaved changes. Would you like to save or discard these changes?";
			// 
			// SaveChangesForm1
			// 
			AcceptButton = SaveAllButton;
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			ClientSize = new System.Drawing.Size(415, 254);
			Controls.Add(UnsavedChangesLabel);
			Controls.Add(DocumentList);
			Controls.Add(SaveAllButton);
			Controls.Add(DiscardButton);
			Controls.Add(CancelButton);
			FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "SaveChangesForm1";
			ShowIcon = false;
			ShowInTaskbar = false;
			StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			Text = "Unsaved changes";
			ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.Button DiscardButton;
        private System.Windows.Forms.Button SaveAllButton;
        private System.Windows.Forms.ListBox DocumentList;
        private System.Windows.Forms.Label UnsavedChangesLabel;
    }
}