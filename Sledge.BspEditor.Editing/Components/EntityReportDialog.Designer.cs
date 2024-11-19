namespace Sledge.BspEditor.Editing.Components
{
    partial class EntityReportDialog
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
			EntityList = new System.Windows.Forms.ListView();
			ClassNameHeader = new System.Windows.Forms.ColumnHeader();
			EntityNameHeader = new System.Windows.Forms.ColumnHeader();
			FilterGroup = new System.Windows.Forms.GroupBox();
			ResetFiltersButton = new System.Windows.Forms.Button();
			IncludeHidden = new System.Windows.Forms.CheckBox();
			FilterClassExact = new System.Windows.Forms.CheckBox();
			FilterKeyValueExact = new System.Windows.Forms.CheckBox();
			label2 = new System.Windows.Forms.Label();
			FilterValue = new System.Windows.Forms.TextBox();
			FilterClass = new System.Windows.Forms.TextBox();
			FilterByClassLabel = new System.Windows.Forms.Label();
			FilterKey = new System.Windows.Forms.TextBox();
			FilterByKeyValueLabel = new System.Windows.Forms.Label();
			TypeBrush = new System.Windows.Forms.RadioButton();
			TypePoint = new System.Windows.Forms.RadioButton();
			TypeAll = new System.Windows.Forms.RadioButton();
			GoToButton = new System.Windows.Forms.Button();
			DeleteButton = new System.Windows.Forms.Button();
			PropertiesButton = new System.Windows.Forms.Button();
			CloseButton = new System.Windows.Forms.Button();
			FollowSelection = new System.Windows.Forms.CheckBox();
			label1 = new System.Windows.Forms.Label();
			totalEntityCount = new System.Windows.Forms.Label();
			label4 = new System.Windows.Forms.Label();
			selectedEntityCount = new System.Windows.Forms.Label();
			FilterGroup.SuspendLayout();
			SuspendLayout();
			// 
			// EntityList
			// 
			EntityList.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			EntityList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { ClassNameHeader, EntityNameHeader });
			EntityList.FullRowSelect = true;
			EntityList.Location = new System.Drawing.Point(14, 14);
			EntityList.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			EntityList.Name = "EntityList";
			EntityList.Size = new System.Drawing.Size(359, 271);
			EntityList.TabIndex = 0;
			EntityList.UseCompatibleStateImageBehavior = false;
			EntityList.View = System.Windows.Forms.View.Details;
			EntityList.ColumnClick += SortByColumn;
			EntityList.SelectedIndexChanged += EntityList_SelectedIndexChanged;
			// 
			// ClassNameHeader
			// 
			ClassNameHeader.Text = "Class";
			ClassNameHeader.Width = 107;
			// 
			// EntityNameHeader
			// 
			EntityNameHeader.Text = "Name";
			EntityNameHeader.Width = 153;
			// 
			// FilterGroup
			// 
			FilterGroup.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			FilterGroup.Controls.Add(ResetFiltersButton);
			FilterGroup.Controls.Add(IncludeHidden);
			FilterGroup.Controls.Add(FilterClassExact);
			FilterGroup.Controls.Add(FilterKeyValueExact);
			FilterGroup.Controls.Add(label2);
			FilterGroup.Controls.Add(FilterValue);
			FilterGroup.Controls.Add(FilterClass);
			FilterGroup.Controls.Add(FilterByClassLabel);
			FilterGroup.Controls.Add(FilterKey);
			FilterGroup.Controls.Add(FilterByKeyValueLabel);
			FilterGroup.Controls.Add(TypeBrush);
			FilterGroup.Controls.Add(TypePoint);
			FilterGroup.Controls.Add(TypeAll);
			FilterGroup.Location = new System.Drawing.Point(380, 14);
			FilterGroup.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			FilterGroup.Name = "FilterGroup";
			FilterGroup.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
			FilterGroup.Size = new System.Drawing.Size(208, 271);
			FilterGroup.TabIndex = 1;
			FilterGroup.TabStop = false;
			FilterGroup.Text = "Filter";
			// 
			// ResetFiltersButton
			// 
			ResetFiltersButton.Location = new System.Drawing.Point(57, 235);
			ResetFiltersButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			ResetFiltersButton.Name = "ResetFiltersButton";
			ResetFiltersButton.Size = new System.Drawing.Size(88, 27);
			ResetFiltersButton.TabIndex = 6;
			ResetFiltersButton.Text = "Reset Filters";
			ResetFiltersButton.UseVisualStyleBackColor = true;
			ResetFiltersButton.Click += ResetFilters;
			// 
			// IncludeHidden
			// 
			IncludeHidden.AutoSize = true;
			IncludeHidden.Checked = true;
			IncludeHidden.CheckState = System.Windows.Forms.CheckState.Checked;
			IncludeHidden.Location = new System.Drawing.Point(13, 102);
			IncludeHidden.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			IncludeHidden.Name = "IncludeHidden";
			IncludeHidden.Size = new System.Drawing.Size(146, 19);
			IncludeHidden.TabIndex = 5;
			IncludeHidden.Text = "Include hidden objects";
			IncludeHidden.UseVisualStyleBackColor = true;
			IncludeHidden.CheckedChanged += FiltersChanged;
			// 
			// FilterClassExact
			// 
			FilterClassExact.AutoSize = true;
			FilterClassExact.Location = new System.Drawing.Point(139, 182);
			FilterClassExact.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			FilterClassExact.Name = "FilterClassExact";
			FilterClassExact.Size = new System.Drawing.Size(54, 19);
			FilterClassExact.TabIndex = 4;
			FilterClassExact.Text = "Exact";
			FilterClassExact.UseVisualStyleBackColor = true;
			FilterClassExact.CheckedChanged += FiltersChanged;
			// 
			// FilterKeyValueExact
			// 
			FilterKeyValueExact.AutoSize = true;
			FilterKeyValueExact.Location = new System.Drawing.Point(139, 129);
			FilterKeyValueExact.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			FilterKeyValueExact.Name = "FilterKeyValueExact";
			FilterKeyValueExact.Size = new System.Drawing.Size(54, 19);
			FilterKeyValueExact.TabIndex = 4;
			FilterKeyValueExact.Text = "Exact";
			FilterKeyValueExact.UseVisualStyleBackColor = true;
			FilterKeyValueExact.CheckedChanged += FiltersChanged;
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(97, 156);
			label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(15, 15);
			label2.TabIndex = 3;
			label2.Text = "=";
			// 
			// FilterValue
			// 
			FilterValue.Location = new System.Drawing.Point(119, 152);
			FilterValue.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			FilterValue.Name = "FilterValue";
			FilterValue.Size = new System.Drawing.Size(76, 23);
			FilterValue.TabIndex = 2;
			FilterValue.TextChanged += FiltersChanged;
			// 
			// FilterClass
			// 
			FilterClass.Location = new System.Drawing.Point(13, 205);
			FilterClass.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			FilterClass.Name = "FilterClass";
			FilterClass.Size = new System.Drawing.Size(182, 23);
			FilterClass.TabIndex = 2;
			FilterClass.TextChanged += FiltersChanged;
			// 
			// FilterByClassLabel
			// 
			FilterByClassLabel.AutoSize = true;
			FilterByClassLabel.Location = new System.Drawing.Point(9, 183);
			FilterByClassLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			FilterByClassLabel.Name = "FilterByClassLabel";
			FilterByClassLabel.Size = new System.Drawing.Size(80, 15);
			FilterByClassLabel.TabIndex = 1;
			FilterByClassLabel.Text = "Filter by class:";
			// 
			// FilterKey
			// 
			FilterKey.Location = new System.Drawing.Point(13, 152);
			FilterKey.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			FilterKey.Name = "FilterKey";
			FilterKey.Size = new System.Drawing.Size(76, 23);
			FilterKey.TabIndex = 2;
			FilterKey.TextChanged += FiltersChanged;
			// 
			// FilterByKeyValueLabel
			// 
			FilterByKeyValueLabel.AutoSize = true;
			FilterByKeyValueLabel.Location = new System.Drawing.Point(9, 130);
			FilterByKeyValueLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			FilterByKeyValueLabel.Name = "FilterByKeyValueLabel";
			FilterByKeyValueLabel.Size = new System.Drawing.Size(106, 15);
			FilterByKeyValueLabel.TabIndex = 1;
			FilterByKeyValueLabel.Text = "Filter by key/value:";
			// 
			// TypeBrush
			// 
			TypeBrush.AutoSize = true;
			TypeBrush.Location = new System.Drawing.Point(13, 75);
			TypeBrush.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			TypeBrush.Name = "TypeBrush";
			TypeBrush.Size = new System.Drawing.Size(124, 19);
			TypeBrush.TabIndex = 0;
			TypeBrush.Text = "Brush Entities Only";
			TypeBrush.UseVisualStyleBackColor = true;
			TypeBrush.Click += FiltersChanged;
			// 
			// TypePoint
			// 
			TypePoint.AutoSize = true;
			TypePoint.Location = new System.Drawing.Point(13, 48);
			TypePoint.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			TypePoint.Name = "TypePoint";
			TypePoint.Size = new System.Drawing.Size(122, 19);
			TypePoint.TabIndex = 0;
			TypePoint.Text = "Point Entities Only";
			TypePoint.UseVisualStyleBackColor = true;
			TypePoint.Click += FiltersChanged;
			// 
			// TypeAll
			// 
			TypeAll.AutoSize = true;
			TypeAll.Checked = true;
			TypeAll.Location = new System.Drawing.Point(13, 22);
			TypeAll.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			TypeAll.Name = "TypeAll";
			TypeAll.Size = new System.Drawing.Size(71, 19);
			TypeAll.TabIndex = 0;
			TypeAll.TabStop = true;
			TypeAll.Text = "Show All";
			TypeAll.UseVisualStyleBackColor = true;
			TypeAll.Click += FiltersChanged;
			// 
			// GoToButton
			// 
			GoToButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
			GoToButton.Location = new System.Drawing.Point(14, 294);
			GoToButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			GoToButton.Name = "GoToButton";
			GoToButton.Size = new System.Drawing.Size(88, 27);
			GoToButton.TabIndex = 2;
			GoToButton.Text = "Go to";
			GoToButton.UseVisualStyleBackColor = true;
			GoToButton.Click += GoToSelectedEntity;
			// 
			// DeleteButton
			// 
			DeleteButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
			DeleteButton.Location = new System.Drawing.Point(108, 294);
			DeleteButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			DeleteButton.Name = "DeleteButton";
			DeleteButton.Size = new System.Drawing.Size(88, 27);
			DeleteButton.TabIndex = 3;
			DeleteButton.Text = "Delete";
			DeleteButton.UseVisualStyleBackColor = true;
			DeleteButton.Click += DeleteSelectedEntity;
			// 
			// PropertiesButton
			// 
			PropertiesButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
			PropertiesButton.Location = new System.Drawing.Point(203, 294);
			PropertiesButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			PropertiesButton.Name = "PropertiesButton";
			PropertiesButton.Size = new System.Drawing.Size(88, 27);
			PropertiesButton.TabIndex = 4;
			PropertiesButton.Text = "Properties";
			PropertiesButton.UseVisualStyleBackColor = true;
			PropertiesButton.Click += OpenEntityProperties;
			// 
			// CloseButton
			// 
			CloseButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			CloseButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			CloseButton.Location = new System.Drawing.Point(500, 333);
			CloseButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			CloseButton.Name = "CloseButton";
			CloseButton.Size = new System.Drawing.Size(88, 27);
			CloseButton.TabIndex = 5;
			CloseButton.Text = "Close";
			CloseButton.UseVisualStyleBackColor = true;
			CloseButton.Click += CloseButtonClicked;
			// 
			// FollowSelection
			// 
			FollowSelection.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
			FollowSelection.AutoSize = true;
			FollowSelection.Checked = true;
			FollowSelection.CheckState = System.Windows.Forms.CheckState.Checked;
			FollowSelection.Location = new System.Drawing.Point(298, 300);
			FollowSelection.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			FollowSelection.Name = "FollowSelection";
			FollowSelection.Size = new System.Drawing.Size(111, 19);
			FollowSelection.TabIndex = 6;
			FollowSelection.Text = "Follow selection";
			FollowSelection.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(14, 339);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(76, 15);
			label1.TabIndex = 7;
			label1.Text = "Total entities:";
			// 
			// totalEntityCount
			// 
			totalEntityCount.AutoSize = true;
			totalEntityCount.Location = new System.Drawing.Point(96, 339);
			totalEntityCount.Name = "totalEntityCount";
			totalEntityCount.Size = new System.Drawing.Size(13, 15);
			totalEntityCount.TabIndex = 8;
			totalEntityCount.Text = "0";
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.Location = new System.Drawing.Point(158, 339);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(94, 15);
			label4.TabIndex = 9;
			label4.Text = "Entities selected:";
			// 
			// selectedEntityCount
			// 
			selectedEntityCount.AutoSize = true;
			selectedEntityCount.Location = new System.Drawing.Point(258, 339);
			selectedEntityCount.Name = "selectedEntityCount";
			selectedEntityCount.Size = new System.Drawing.Size(13, 15);
			selectedEntityCount.TabIndex = 10;
			selectedEntityCount.Text = "0";
			// 
			// EntityReportDialog
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			ClientSize = new System.Drawing.Size(600, 370);
			Controls.Add(selectedEntityCount);
			Controls.Add(label4);
			Controls.Add(totalEntityCount);
			Controls.Add(label1);
			Controls.Add(FollowSelection);
			Controls.Add(CloseButton);
			Controls.Add(PropertiesButton);
			Controls.Add(DeleteButton);
			Controls.Add(GoToButton);
			Controls.Add(FilterGroup);
			Controls.Add(EntityList);
			Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			MaximizeBox = false;
			MinimizeBox = false;
			MinimumSize = new System.Drawing.Size(616, 375);
			Name = "EntityReportDialog";
			ShowInTaskbar = false;
			StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			Text = "Entity Report";
			FilterGroup.ResumeLayout(false);
			FilterGroup.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private System.Windows.Forms.ListView EntityList;
        private System.Windows.Forms.ColumnHeader ClassNameHeader;
        private System.Windows.Forms.ColumnHeader EntityNameHeader;
        private System.Windows.Forms.GroupBox FilterGroup;
        private System.Windows.Forms.RadioButton TypeBrush;
        private System.Windows.Forms.RadioButton TypePoint;
        private System.Windows.Forms.RadioButton TypeAll;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox FilterValue;
        private System.Windows.Forms.TextBox FilterKey;
        private System.Windows.Forms.Label FilterByKeyValueLabel;
        private System.Windows.Forms.TextBox FilterClass;
        private System.Windows.Forms.Label FilterByClassLabel;
        private System.Windows.Forms.CheckBox FilterClassExact;
        private System.Windows.Forms.CheckBox FilterKeyValueExact;
        private System.Windows.Forms.CheckBox IncludeHidden;
        private System.Windows.Forms.Button ResetFiltersButton;
        private System.Windows.Forms.Button GoToButton;
        private System.Windows.Forms.Button DeleteButton;
        private System.Windows.Forms.Button PropertiesButton;
        private System.Windows.Forms.Button CloseButton;
        private System.Windows.Forms.CheckBox FollowSelection;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label totalEntityCount;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label selectedEntityCount;
	}
}