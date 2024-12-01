namespace Sledge.Shell.Settings.Editors
{
    partial class HotkeysEditor
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
			HotkeyResetButton = new System.Windows.Forms.Button();
			groupBox22 = new System.Windows.Forms.GroupBox();
			HotkeyActionList = new System.Windows.Forms.ComboBox();
			HotkeyAddButton = new System.Windows.Forms.Button();
			label25 = new System.Windows.Forms.Label();
			label23 = new System.Windows.Forms.Label();
			HotkeyRemoveButton = new System.Windows.Forms.Button();
			HotkeyCombination = new System.Windows.Forms.TextBox();
			HotkeyList = new System.Windows.Forms.ListView();
			chAction = new System.Windows.Forms.ColumnHeader();
			chDescription = new System.Windows.Forms.ColumnHeader();
			ckKeyCombo = new System.Windows.Forms.ColumnHeader();
			FilterLabel = new System.Windows.Forms.Label();
			FilterBox = new System.Windows.Forms.TextBox();
			groupBox22.SuspendLayout();
			SuspendLayout();
			// 
			// HotkeyResetButton
			// 
			HotkeyResetButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			HotkeyResetButton.Location = new System.Drawing.Point(610, 250);
			HotkeyResetButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			HotkeyResetButton.Name = "HotkeyResetButton";
			HotkeyResetButton.Size = new System.Drawing.Size(138, 27);
			HotkeyResetButton.TabIndex = 10;
			HotkeyResetButton.Text = "Reset to Defaults";
			HotkeyResetButton.UseVisualStyleBackColor = true;
			HotkeyResetButton.Click += HotkeyResetButtonClicked;
			// 
			// groupBox22
			// 
			groupBox22.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
			groupBox22.Controls.Add(HotkeyActionList);
			groupBox22.Controls.Add(HotkeyAddButton);
			groupBox22.Controls.Add(label25);
			groupBox22.Controls.Add(label23);
			groupBox22.Controls.Add(HotkeyRemoveButton);
			groupBox22.Controls.Add(HotkeyCombination);
			groupBox22.Location = new System.Drawing.Point(4, 232);
			groupBox22.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			groupBox22.Name = "groupBox22";
			groupBox22.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
			groupBox22.Size = new System.Drawing.Size(590, 58);
			groupBox22.TabIndex = 9;
			groupBox22.TabStop = false;
			groupBox22.Text = "Assign Hotkey";
			// 
			// HotkeyActionList
			// 
			HotkeyActionList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			HotkeyActionList.FormattingEnabled = true;
			HotkeyActionList.Location = new System.Drawing.Point(57, 21);
			HotkeyActionList.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			HotkeyActionList.Name = "HotkeyActionList";
			HotkeyActionList.Size = new System.Drawing.Size(174, 23);
			HotkeyActionList.TabIndex = 3;
			// 
			// HotkeyAddButton
			// 
			HotkeyAddButton.Location = new System.Drawing.Point(418, 18);
			HotkeyAddButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			HotkeyAddButton.Name = "HotkeyAddButton";
			HotkeyAddButton.Size = new System.Drawing.Size(80, 27);
			HotkeyAddButton.TabIndex = 3;
			HotkeyAddButton.Text = "Set";
			HotkeyAddButton.UseVisualStyleBackColor = true;
			HotkeyAddButton.Click += HotkeySetButtonClicked;
			// 
			// label25
			// 
			label25.AutoSize = true;
			label25.Location = new System.Drawing.Point(7, 25);
			label25.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label25.Name = "label25";
			label25.Size = new System.Drawing.Size(42, 15);
			label25.TabIndex = 2;
			label25.Text = "Action";
			// 
			// label23
			// 
			label23.AutoSize = true;
			label23.Location = new System.Drawing.Point(239, 25);
			label23.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label23.Name = "label23";
			label23.Size = new System.Drawing.Size(45, 15);
			label23.TabIndex = 2;
			label23.Text = "Hotkey";
			// 
			// HotkeyRemoveButton
			// 
			HotkeyRemoveButton.Location = new System.Drawing.Point(503, 18);
			HotkeyRemoveButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			HotkeyRemoveButton.Name = "HotkeyRemoveButton";
			HotkeyRemoveButton.Size = new System.Drawing.Size(80, 27);
			HotkeyRemoveButton.TabIndex = 8;
			HotkeyRemoveButton.Text = "Unset";
			HotkeyRemoveButton.UseVisualStyleBackColor = true;
			HotkeyRemoveButton.Click += HotkeyUnsetButtonClicked;
			// 
			// HotkeyCombination
			// 
			HotkeyCombination.Location = new System.Drawing.Point(294, 22);
			HotkeyCombination.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			HotkeyCombination.Name = "HotkeyCombination";
			HotkeyCombination.Size = new System.Drawing.Size(116, 23);
			HotkeyCombination.TabIndex = 1;
			HotkeyCombination.KeyDown += HotkeyCombinationKeyDown;
			// 
			// HotkeyList
			// 
			HotkeyList.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			HotkeyList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { chAction, chDescription, ckKeyCombo });
			HotkeyList.FullRowSelect = true;
			HotkeyList.Location = new System.Drawing.Point(4, 33);
			HotkeyList.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			HotkeyList.MultiSelect = false;
			HotkeyList.Name = "HotkeyList";
			HotkeyList.OwnerDraw = true;
			HotkeyList.Size = new System.Drawing.Size(744, 191);
			HotkeyList.TabIndex = 6;
			HotkeyList.UseCompatibleStateImageBehavior = false;
			HotkeyList.View = System.Windows.Forms.View.Details;
			HotkeyList.DrawColumnHeader += HotkeyList_DrawColumnHeader;
			HotkeyList.DrawItem += HotkeyList_DrawItem;
			HotkeyList.DrawSubItem += HotkeyList_DrawSubItem;
			HotkeyList.SelectedIndexChanged += HotkeyListSelectionChanged;
			HotkeyList.KeyDown += HotkeyListKeyDown;
			// 
			// chAction
			// 
			chAction.Text = "Action";
			// 
			// chDescription
			// 
			chDescription.Text = "Description";
			// 
			// ckKeyCombo
			// 
			ckKeyCombo.Text = "Hotkey";
			// 
			// FilterLabel
			// 
			FilterLabel.AutoSize = true;
			FilterLabel.Location = new System.Drawing.Point(6, 7);
			FilterLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			FilterLabel.Name = "FilterLabel";
			FilterLabel.Size = new System.Drawing.Size(33, 15);
			FilterLabel.TabIndex = 12;
			FilterLabel.Text = "Filter";
			// 
			// FilterBox
			// 
			FilterBox.Location = new System.Drawing.Point(61, 3);
			FilterBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			FilterBox.Name = "FilterBox";
			FilterBox.Size = new System.Drawing.Size(174, 23);
			FilterBox.TabIndex = 11;
			FilterBox.TextChanged += UpdateFilter;
			// 
			// HotkeysEditor
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			AutoSize = true;
			Controls.Add(FilterLabel);
			Controls.Add(FilterBox);
			Controls.Add(HotkeyResetButton);
			Controls.Add(groupBox22);
			Controls.Add(HotkeyList);
			Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			Name = "HotkeysEditor";
			Size = new System.Drawing.Size(751, 292);
			groupBox22.ResumeLayout(false);
			groupBox22.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private System.Windows.Forms.Button HotkeyResetButton;
        private System.Windows.Forms.GroupBox groupBox22;
        private System.Windows.Forms.ComboBox HotkeyActionList;
        private System.Windows.Forms.Button HotkeyAddButton;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.TextBox HotkeyCombination;
        private System.Windows.Forms.Button HotkeyRemoveButton;
        private System.Windows.Forms.ListView HotkeyList;
        private System.Windows.Forms.ColumnHeader chAction;
        private System.Windows.Forms.ColumnHeader chDescription;
        private System.Windows.Forms.ColumnHeader ckKeyCombo;
        private System.Windows.Forms.Label FilterLabel;
        private System.Windows.Forms.TextBox FilterBox;
    }
}
