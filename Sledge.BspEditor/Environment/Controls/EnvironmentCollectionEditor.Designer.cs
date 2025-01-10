namespace Sledge.BspEditor.Environment.Controls
{
    partial class EnvironmentCollectionEditor
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
			components = new System.ComponentModel.Container();
			treEnvironments = new System.Windows.Forms.TreeView();
			btnRemove = new System.Windows.Forms.Button();
			pnlSettings = new System.Windows.Forms.FlowLayoutPanel();
			btnAdd = new Shell.Controls.DropdownButton();
			ctxEnvironmentMenu = new System.Windows.Forms.ContextMenuStrip(components);
			noEnvironmentsFoundToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			importBtn = new Shell.Controls.DropdownButton();
			ctxImportMenu = new System.Windows.Forms.ContextMenuStrip(components);
			importProfileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			exportProfileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			ctxEnvironmentMenu.SuspendLayout();
			ctxImportMenu.SuspendLayout();
			SuspendLayout();
			// 
			// treEnvironments
			// 
			treEnvironments.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
			treEnvironments.HideSelection = false;
			treEnvironments.Location = new System.Drawing.Point(4, 3);
			treEnvironments.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			treEnvironments.Name = "treEnvironments";
			treEnvironments.Size = new System.Drawing.Size(182, 335);
			treEnvironments.TabIndex = 0;
			treEnvironments.AfterSelect += EnvironmentSelected;
			// 
			// btnRemove
			// 
			btnRemove.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
			btnRemove.Location = new System.Drawing.Point(66, 346);
			btnRemove.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			btnRemove.Name = "btnRemove";
			btnRemove.Size = new System.Drawing.Size(58, 27);
			btnRemove.TabIndex = 1;
			btnRemove.Text = "Remove";
			btnRemove.UseVisualStyleBackColor = true;
			btnRemove.Click += RemoveEnvironment;
			// 
			// pnlSettings
			// 
			pnlSettings.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			pnlSettings.AutoScroll = true;
			pnlSettings.Location = new System.Drawing.Point(194, 3);
			pnlSettings.Margin = new System.Windows.Forms.Padding(4, 3, 29, 3);
			pnlSettings.Name = "pnlSettings";
			pnlSettings.Size = new System.Drawing.Size(590, 369);
			pnlSettings.TabIndex = 2;
			// 
			// btnAdd
			// 
			btnAdd.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
			btnAdd.Location = new System.Drawing.Point(5, 346);
			btnAdd.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			btnAdd.Menu = ctxEnvironmentMenu;
			btnAdd.Name = "btnAdd";
			btnAdd.Size = new System.Drawing.Size(58, 27);
			btnAdd.TabIndex = 3;
			btnAdd.Text = "Add";
			btnAdd.UseVisualStyleBackColor = true;
			// 
			// ctxEnvironmentMenu
			// 
			ctxEnvironmentMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { noEnvironmentsFoundToolStripMenuItem });
			ctxEnvironmentMenu.Name = "ctxEnvironmentMenu";
			ctxEnvironmentMenu.Size = new System.Drawing.Size(205, 26);
			// 
			// noEnvironmentsFoundToolStripMenuItem
			// 
			noEnvironmentsFoundToolStripMenuItem.Enabled = false;
			noEnvironmentsFoundToolStripMenuItem.Name = "noEnvironmentsFoundToolStripMenuItem";
			noEnvironmentsFoundToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
			noEnvironmentsFoundToolStripMenuItem.Text = "No environments found!";
			// 
			// importBtn
			// 
			importBtn.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
			importBtn.Location = new System.Drawing.Point(126, 346);
			importBtn.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			importBtn.Menu = ctxImportMenu;
			importBtn.Name = "importBtn";
			importBtn.Size = new System.Drawing.Size(60, 27);
			importBtn.TabIndex = 4;
			importBtn.Text = "Import";
			importBtn.UseVisualStyleBackColor = true;
			// 
			// ctxImportMenu
			// 
			ctxImportMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { importProfileToolStripMenuItem, exportProfileToolStripMenuItem });
			ctxImportMenu.Name = "ctxImportMenu";
			ctxImportMenu.Size = new System.Drawing.Size(181, 70);
			// 
			// importProfileToolStripMenuItem
			// 
			importProfileToolStripMenuItem.Name = "importProfileToolStripMenuItem";
			importProfileToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			importProfileToolStripMenuItem.Text = "Import profile";
			importProfileToolStripMenuItem.Click += importProfileToolStripMenuItem_Click;
			// 
			// exportProfileToolStripMenuItem
			// 
			exportProfileToolStripMenuItem.Name = "exportProfileToolStripMenuItem";
			exportProfileToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			exportProfileToolStripMenuItem.Text = "Export profile";
			exportProfileToolStripMenuItem.Click += exportProfileToolStripMenuItem_Click;
			// 
			// EnvironmentCollectionEditor
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			Controls.Add(importBtn);
			Controls.Add(btnAdd);
			Controls.Add(pnlSettings);
			Controls.Add(btnRemove);
			Controls.Add(treEnvironments);
			Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			Name = "EnvironmentCollectionEditor";
			Size = new System.Drawing.Size(788, 376);
			ctxEnvironmentMenu.ResumeLayout(false);
			ctxImportMenu.ResumeLayout(false);
			ResumeLayout(false);
		}

		#endregion

		private System.Windows.Forms.TreeView treEnvironments;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.FlowLayoutPanel pnlSettings;
        private Shell.Controls.DropdownButton btnAdd;
        private System.Windows.Forms.ContextMenuStrip ctxEnvironmentMenu;
        private System.Windows.Forms.ToolStripMenuItem noEnvironmentsFoundToolStripMenuItem;
		private Shell.Controls.DropdownButton importBtn;
		private System.Windows.Forms.ContextMenuStrip ctxImportMenu;
		private System.Windows.Forms.ToolStripMenuItem importProfileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exportProfileToolStripMenuItem;
	}
}
