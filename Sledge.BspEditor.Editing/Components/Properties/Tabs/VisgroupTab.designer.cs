using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sledge.BspEditor.Editing.Components.Properties.Tabs
{
	public sealed partial class VisgroupTab
	{
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			btnEditVisgroups = new System.Windows.Forms.Button();
			lblMemberOfGroup = new System.Windows.Forms.Label();
			visgroupPanel = new Sledge.BspEditor.Editing.Components.Visgroup.VisgroupPanel();
			SuspendLayout();
			// 
			// btnEditVisgroups
			// 
			btnEditVisgroups.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right));
			btnEditVisgroups.Location = new System.Drawing.Point(486, 442);
			btnEditVisgroups.Name = "btnEditVisgroups";
			btnEditVisgroups.Size = new System.Drawing.Size(98, 23);
			btnEditVisgroups.TabIndex = 5;
			btnEditVisgroups.Text = "Edit Visgroups";
			btnEditVisgroups.UseVisualStyleBackColor = true;
			btnEditVisgroups.Click += EditVisgroupsClicked;
			// 
			// lblMemberOfGroup
			// 
			lblMemberOfGroup.Location = new System.Drawing.Point(3, 0);
			lblMemberOfGroup.Name = "lblMemberOfGroup";
			lblMemberOfGroup.Size = new System.Drawing.Size(103, 20);
			lblMemberOfGroup.TabIndex = 4;
			lblMemberOfGroup.Text = "Member of group:";
			lblMemberOfGroup.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// visgroupPanel
			// 
			visgroupPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
			visgroupPanel.Location = new System.Drawing.Point(3, 23);
			visgroupPanel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			visgroupPanel.Name = "visgroupPanel";
			visgroupPanel.SelectedVisgroup = null;
			visgroupPanel.ShowCheckboxes = true;
			visgroupPanel.Size = new System.Drawing.Size(581, 413);
			visgroupPanel.TabIndex = 6;
			visgroupPanel.VisgroupToggled += VisgroupToggled;
			// 
			// VisgroupTab
			// 
			BackColor = System.Drawing.SystemColors.ControlLightLight;
			Controls.Add(visgroupPanel);
			Controls.Add(btnEditVisgroups);
			Controls.Add(lblMemberOfGroup);
			Size = new System.Drawing.Size(587, 468);
			ResumeLayout(false);
		}

		private System.Windows.Forms.Button btnEditVisgroups;
		private System.Windows.Forms.Label lblMemberOfGroup;
		private Visgroup.VisgroupPanel visgroupPanel;
	}
}
