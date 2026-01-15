namespace Sledge.Shell.Forms1
{
    partial class SettingsForm
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
			SettingsPanel = new System.Windows.Forms.TableLayoutPanel();
			CancelButton = new System.Windows.Forms.Button();
			OKButton = new System.Windows.Forms.Button();
			GroupList = new System.Windows.Forms.TreeView();
			SuspendLayout();
			// 
			// SettingsPanel
			// 
			SettingsPanel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			SettingsPanel.AutoScroll = true;
			SettingsPanel.ColumnCount = 1;
			SettingsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			SettingsPanel.Location = new System.Drawing.Point(197, 14);
			SettingsPanel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			SettingsPanel.Name = "SettingsPanel";
			SettingsPanel.Padding = new System.Windows.Forms.Padding(0, 0, 23, 0);
			SettingsPanel.Size = new System.Drawing.Size(840, 474);
			SettingsPanel.TabIndex = 7;
			// 
			// CancelButton
			// 
			CancelButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			CancelButton.Location = new System.Drawing.Point(950, 495);
			CancelButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			CancelButton.Name = "CancelButton";
			CancelButton.Size = new System.Drawing.Size(88, 27);
			CancelButton.TabIndex = 8;
			CancelButton.Text = "Cancel";
			CancelButton.UseVisualStyleBackColor = true;
			CancelButton.Click += CancelClicked;
			// 
			// OKButton
			// 
			OKButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			OKButton.Location = new System.Drawing.Point(855, 495);
			OKButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			OKButton.Name = "OKButton";
			OKButton.Size = new System.Drawing.Size(88, 27);
			OKButton.TabIndex = 8;
			OKButton.Text = "OK";
			OKButton.UseVisualStyleBackColor = true;
			OKButton.Click += OkClicked;
			// 
			// GroupList
			// 
			GroupList.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
			GroupList.Location = new System.Drawing.Point(14, 14);
			GroupList.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			GroupList.Name = "GroupList";
			GroupList.Size = new System.Drawing.Size(176, 474);
			GroupList.TabIndex = 9;
			GroupList.AfterSelect += GroupListSelectionChanged;
			// 
			// SettingsForm
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			ClientSize = new System.Drawing.Size(1051, 535);
			Controls.Add(GroupList);
			Controls.Add(OKButton);
			Controls.Add(CancelButton);
			Controls.Add(SettingsPanel);
			Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "SettingsForm";
			StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			Text = "SettingsForm";
			ResumeLayout(false);
		}

		#endregion
		private System.Windows.Forms.TableLayoutPanel SettingsPanel;
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.Button OKButton;
        private System.Windows.Forms.TreeView GroupList;
    }
}