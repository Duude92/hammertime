using System.Windows.Forms;
using Sledge.Shell.Controls;

namespace Sledge.Shell.Forms
{
    partial class Shell
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
			MenuStrip = new MenuStrip();
			StatusStrip = new StatusStrip();
			ToolStripContainer = new ToolStripContainer();
			DocumentContainer = new Panel();
			DocumentTabs = new ClosableTabControl();
			RightSidebar = new DockedPanel();
			RightSidebarContainer = new SidebarContainer();
			LeftSidebar = new DockedPanel();
			LeftSidebarContainer = new SidebarContainer();
			BottomSidebar = new DockedPanel();
			BottomTabs = new TabControl();
			tabPage1 = new TabPage();
			tabPage2 = new TabPage();
			ToolsContainer = new ToolStrip();
			ToolStripContainer.ContentPanel.SuspendLayout();
			ToolStripContainer.LeftToolStripPanel.SuspendLayout();
			ToolStripContainer.SuspendLayout();
			RightSidebar.SuspendLayout();
			LeftSidebar.SuspendLayout();
			BottomSidebar.SuspendLayout();
			BottomTabs.SuspendLayout();
			SuspendLayout();
			// 
			// MenuStrip
			// 
			MenuStrip.Location = new System.Drawing.Point(0, 0);
			MenuStrip.Name = "MenuStrip";
			MenuStrip.Padding = new Padding(7, 2, 0, 2);
			MenuStrip.Size = new System.Drawing.Size(808, 24);
			MenuStrip.TabIndex = 0;
			MenuStrip.Text = "menuStrip1";
			// 
			// StatusStrip
			// 
			StatusStrip.Location = new System.Drawing.Point(0, 501);
			StatusStrip.Name = "StatusStrip";
			StatusStrip.Padding = new Padding(1, 0, 16, 0);
			StatusStrip.Size = new System.Drawing.Size(808, 22);
			StatusStrip.TabIndex = 1;
			StatusStrip.Text = "statusStrip1";
			// 
			// ToolStripContainer
			// 
			// 
			// ToolStripContainer.ContentPanel
			// 
			ToolStripContainer.ContentPanel.Controls.Add(DocumentContainer);
			ToolStripContainer.ContentPanel.Controls.Add(DocumentTabs);
			ToolStripContainer.ContentPanel.Controls.Add(RightSidebar);
			ToolStripContainer.ContentPanel.Controls.Add(LeftSidebar);
			ToolStripContainer.ContentPanel.Controls.Add(BottomSidebar);
			ToolStripContainer.ContentPanel.Margin = new Padding(4, 3, 4, 3);
			ToolStripContainer.ContentPanel.Size = new System.Drawing.Size(782, 448);
			ToolStripContainer.Dock = DockStyle.Fill;
			// 
			// ToolStripContainer.LeftToolStripPanel
			// 
			ToolStripContainer.LeftToolStripPanel.Controls.Add(ToolsContainer);
			ToolStripContainer.Location = new System.Drawing.Point(0, 24);
			ToolStripContainer.Margin = new Padding(4, 3, 4, 3);
			ToolStripContainer.Name = "ToolStripContainer";
			ToolStripContainer.Size = new System.Drawing.Size(808, 477);
			ToolStripContainer.TabIndex = 2;
			ToolStripContainer.Text = "ToolStripContainer";
			// 
			// DocumentContainer
			// 
			DocumentContainer.Dock = DockStyle.Fill;
			DocumentContainer.Location = new System.Drawing.Point(9, 28);
			DocumentContainer.Margin = new Padding(4, 3, 4, 3);
			DocumentContainer.Name = "DocumentContainer";
			DocumentContainer.Size = new System.Drawing.Size(540, 305);
			DocumentContainer.TabIndex = 3;
			// 
			// DocumentTabs
			// 
			DocumentTabs.Dock = DockStyle.Top;
			DocumentTabs.Location = new System.Drawing.Point(9, 0);
			DocumentTabs.Margin = new Padding(4, 3, 4, 3);
			DocumentTabs.Name = "DocumentTabs";
			DocumentTabs.SelectedIndex = 0;
			DocumentTabs.Size = new System.Drawing.Size(540, 28);
			DocumentTabs.TabIndex = 4;
			DocumentTabs.RequestClose += RequestClose;
			DocumentTabs.SelectedIndexChanged += TabChanged;
			// 
			// RightSidebar
			// 
			RightSidebar.Controls.Add(RightSidebarContainer);
			RightSidebar.Dock = DockStyle.Right;
			RightSidebar.DockDimension = 233;
			RightSidebar.Hidden = false;
			RightSidebar.Location = new System.Drawing.Point(549, 0);
			RightSidebar.Margin = new Padding(4, 3, 4, 3);
			RightSidebar.Name = "RightSidebar";
			RightSidebar.Padding = new Padding(9, 0, 0, 0);
			RightSidebar.Size = new System.Drawing.Size(233, 333);
			RightSidebar.TabIndex = 2;
			// 
			// RightSidebarContainer
			// 
			RightSidebarContainer.Dock = DockStyle.Fill;
			RightSidebarContainer.Location = new System.Drawing.Point(9, 0);
			RightSidebarContainer.Margin = new Padding(5, 3, 5, 3);
			RightSidebarContainer.Name = "RightSidebarContainer";
			RightSidebarContainer.Size = new System.Drawing.Size(224, 333);
			RightSidebarContainer.TabIndex = 0;
			// 
			// LeftSidebar
			// 
			LeftSidebar.Controls.Add(LeftSidebarContainer);
			LeftSidebar.Dock = DockStyle.Left;
			LeftSidebar.DockDimension = 9;
			LeftSidebar.Hidden = true;
			LeftSidebar.Location = new System.Drawing.Point(0, 0);
			LeftSidebar.Margin = new Padding(4, 3, 4, 3);
			LeftSidebar.Name = "LeftSidebar";
			LeftSidebar.Padding = new Padding(0, 0, 9, 0);
			LeftSidebar.Size = new System.Drawing.Size(9, 333);
			LeftSidebar.TabIndex = 1;
			// 
			// LeftSidebarContainer
			// 
			LeftSidebarContainer.Dock = DockStyle.Fill;
			LeftSidebarContainer.Location = new System.Drawing.Point(0, 0);
			LeftSidebarContainer.Margin = new Padding(5, 3, 5, 3);
			LeftSidebarContainer.Name = "LeftSidebarContainer";
			LeftSidebarContainer.Size = new System.Drawing.Size(0, 333);
			LeftSidebarContainer.TabIndex = 0;
			// 
			// BottomSidebar
			// 
			BottomSidebar.Controls.Add(BottomTabs);
			BottomSidebar.Dock = DockStyle.Bottom;
			BottomSidebar.DockDimension = 115;
			BottomSidebar.Hidden = false;
			BottomSidebar.Location = new System.Drawing.Point(0, 333);
			BottomSidebar.Margin = new Padding(4, 3, 4, 3);
			BottomSidebar.Name = "BottomSidebar";
			BottomSidebar.Padding = new Padding(0, 9, 0, 0);
			BottomSidebar.Size = new System.Drawing.Size(782, 115);
			BottomSidebar.TabIndex = 0;
			// 
			// BottomTabs
			// 
			BottomTabs.Alignment = TabAlignment.Bottom;
			BottomTabs.Controls.Add(tabPage1);
			BottomTabs.Controls.Add(tabPage2);
			BottomTabs.Dock = DockStyle.Fill;
			BottomTabs.Location = new System.Drawing.Point(0, 9);
			BottomTabs.Margin = new Padding(4, 3, 4, 3);
			BottomTabs.Name = "BottomTabs";
			BottomTabs.SelectedIndex = 0;
			BottomTabs.Size = new System.Drawing.Size(782, 106);
			BottomTabs.TabIndex = 0;
			// 
			// tabPage1
			// 
			tabPage1.Location = new System.Drawing.Point(4, 4);
			tabPage1.Margin = new Padding(4, 3, 4, 3);
			tabPage1.Name = "tabPage1";
			tabPage1.Padding = new Padding(4, 3, 4, 3);
			tabPage1.Size = new System.Drawing.Size(774, 78);
			tabPage1.TabIndex = 0;
			tabPage1.Text = "tabPage1";
			tabPage1.UseVisualStyleBackColor = true;
			// 
			// tabPage2
			// 
			tabPage2.Location = new System.Drawing.Point(4, 4);
			tabPage2.Margin = new Padding(4, 3, 4, 3);
			tabPage2.Name = "tabPage2";
			tabPage2.Padding = new Padding(4, 3, 4, 3);
			tabPage2.Size = new System.Drawing.Size(770, 78);
			tabPage2.TabIndex = 1;
			tabPage2.Text = "tabPage2";
			tabPage2.UseVisualStyleBackColor = true;
			// 
			// ToolsContainer
			// 
			ToolsContainer.Dock = DockStyle.None;
			ToolsContainer.LayoutStyle = ToolStripLayoutStyle.VerticalStackWithOverflow;
			ToolsContainer.Location = new System.Drawing.Point(0, 3);
			ToolsContainer.Name = "ToolsContainer";
			ToolsContainer.Size = new System.Drawing.Size(26, 111);
			ToolsContainer.TabIndex = 0;
			// 
			// Shell
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new System.Drawing.Size(808, 523);
			Controls.Add(ToolStripContainer);
			Controls.Add(StatusStrip);
			Controls.Add(MenuStrip);
			MainMenuStrip = MenuStrip;
			Margin = new Padding(4, 3, 4, 3);
			Name = "Shell";
			Text = "Hammertime Shell";
			WindowState = FormWindowState.Maximized;
			ToolStripContainer.ContentPanel.ResumeLayout(false);
			ToolStripContainer.LeftToolStripPanel.ResumeLayout(false);
			ToolStripContainer.LeftToolStripPanel.PerformLayout();
			ToolStripContainer.ResumeLayout(false);
			ToolStripContainer.PerformLayout();
			RightSidebar.ResumeLayout(false);
			LeftSidebar.ResumeLayout(false);
			BottomSidebar.ResumeLayout(false);
			BottomTabs.ResumeLayout(false);
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion
		private System.Windows.Forms.ToolStripContainer ToolStripContainer;
        private Sledge.Shell.Controls.DockedPanel BottomSidebar;
        private Sledge.Shell.Controls.DockedPanel RightSidebar;
        private Sledge.Shell.Controls.DockedPanel LeftSidebar;
        private System.Windows.Forms.Panel DocumentContainer;
        private Sledge.Shell.Controls.ClosableTabControl DocumentTabs;
        internal SidebarContainer RightSidebarContainer;
        internal SidebarContainer LeftSidebarContainer;
        private TabPage tabPage2;
        internal TabPage tabPage1;
        internal TabControl BottomTabs;
        internal MenuStrip MenuStrip;
        internal ToolStrip ToolsContainer;
        internal StatusStrip StatusStrip;
    }
}