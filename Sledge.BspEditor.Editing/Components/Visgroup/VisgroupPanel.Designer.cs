namespace Sledge.BspEditor.Editing.Components.Visgroup
{
    partial class VisgroupPanel
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
			VisgroupTree = new System.Windows.Forms.TreeView();
			CheckboxImages = new System.Windows.Forms.ImageList(components);
			SuspendLayout();
			// 
			// VisgroupTree
			// 
			VisgroupTree.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			VisgroupTree.Dock = System.Windows.Forms.DockStyle.Fill;
			VisgroupTree.Indent = 16;
			VisgroupTree.Location = new System.Drawing.Point(0, 0);
			VisgroupTree.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			VisgroupTree.Name = "VisgroupTree";
			VisgroupTree.Size = new System.Drawing.Size(285, 263);
			VisgroupTree.StateImageList = CheckboxImages;
			VisgroupTree.TabIndex = 0;
			VisgroupTree.ItemDrag += OnItemDrag;
			VisgroupTree.AfterSelect += NodeSelected;
			VisgroupTree.NodeMouseClick += NodeMouseClick;
			VisgroupTree.NodeMouseDoubleClick += NodeMouseClick;
			VisgroupTree.DragDrop += OnDragDrop;
			VisgroupTree.DragEnter += OnDragEnter;
			// 
			// CheckboxImages
			// 
			CheckboxImages.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			CheckboxImages.ImageSize = new System.Drawing.Size(16, 16);
			CheckboxImages.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// VisgroupPanel
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			Controls.Add(VisgroupTree);
			Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			Name = "VisgroupPanel";
			Size = new System.Drawing.Size(285, 263);
			ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TreeView VisgroupTree;
        private System.Windows.Forms.ImageList CheckboxImages;
    }
}
