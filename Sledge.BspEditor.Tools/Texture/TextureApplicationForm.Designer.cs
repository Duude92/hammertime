namespace Sledge.BspEditor.Tools.Texture
{
    partial class TextureApplicationForm
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
			HideMaskCheckbox = new System.Windows.Forms.CheckBox();
			RecentFilterTextbox = new System.Windows.Forms.TextBox();
			FilterRecentLabel = new System.Windows.Forms.Label();
			SmoothingGroupsButton = new System.Windows.Forms.Button();
			AlignGroup = new System.Windows.Forms.GroupBox();
			AlignToFaceCheckbox = new System.Windows.Forms.CheckBox();
			AlignToWorldCheckbox = new System.Windows.Forms.CheckBox();
			JustifyGroup = new System.Windows.Forms.GroupBox();
			RLeftButton = new System.Windows.Forms.Button();
			RRightButton = new System.Windows.Forms.Button();
			JustifyTopButton = new System.Windows.Forms.Button();
			JustifyFitButton = new System.Windows.Forms.Button();
			TreatAsOneCheckbox = new System.Windows.Forms.CheckBox();
			JustifyRightButton = new System.Windows.Forms.Button();
			JustifyBottomButton = new System.Windows.Forms.Button();
			JustifyCenterButton = new System.Windows.Forms.Button();
			JustifyLeftButton = new System.Windows.Forms.Button();
			ApplyButton = new System.Windows.Forms.Button();
			RotationValue = new Sledge.Shell.Controls.NumericUpDownEx();
			ReplaceButton = new System.Windows.Forms.Button();
			BrowseButton = new System.Windows.Forms.Button();
			RotationLabel = new System.Windows.Forms.Label();
			TextureDetailsLabel = new System.Windows.Forms.Label();
			tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			ScaleXValue = new Sledge.Shell.Controls.NumericUpDownEx();
			ScaleLabel = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			ShiftLabel = new System.Windows.Forms.Label();
			ScaleYValue = new Sledge.Shell.Controls.NumericUpDownEx();
			ShiftXValue = new Sledge.Shell.Controls.NumericUpDownEx();
			ShiftYValue = new Sledge.Shell.Controls.NumericUpDownEx();
			LightmapLabel = new System.Windows.Forms.Label();
			LightmapValue = new Sledge.Shell.Controls.NumericUpDownEx();
			HoverTip = new System.Windows.Forms.ToolTip(components);
			SelectedTextureListPanel = new System.Windows.Forms.Panel();
			RecentTextureListPanel = new System.Windows.Forms.Panel();
			LeftClickActionButton = new Sledge.Shell.Controls.DropdownButton();
			LeftClickActionMenu = new System.Windows.Forms.ContextMenuStrip(components);
			RightClickActionButton = new Sledge.Shell.Controls.DropdownButton();
			RightClickActionMenu = new System.Windows.Forms.ContextMenuStrip(components);
			ResetButton = new System.Windows.Forms.Button();
			apply_null = new System.Windows.Forms.Button();
			AlignGroup.SuspendLayout();
			JustifyGroup.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)RotationValue).BeginInit();
			tableLayoutPanel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)ScaleXValue).BeginInit();
			((System.ComponentModel.ISupportInitialize)ScaleYValue).BeginInit();
			((System.ComponentModel.ISupportInitialize)ShiftXValue).BeginInit();
			((System.ComponentModel.ISupportInitialize)ShiftYValue).BeginInit();
			((System.ComponentModel.ISupportInitialize)LightmapValue).BeginInit();
			SuspendLayout();
			// 
			// HideMaskCheckbox
			// 
			HideMaskCheckbox.Appearance = System.Windows.Forms.Appearance.Button;
			HideMaskCheckbox.Location = new System.Drawing.Point(354, 175);
			HideMaskCheckbox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			HideMaskCheckbox.Name = "HideMaskCheckbox";
			HideMaskCheckbox.Size = new System.Drawing.Size(119, 27);
			HideMaskCheckbox.TabIndex = 34;
			HideMaskCheckbox.Text = "Hide Mask";
			HideMaskCheckbox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			HideMaskCheckbox.UseVisualStyleBackColor = true;
			HideMaskCheckbox.CheckedChanged += HideMaskCheckboxToggled;
			// 
			// RecentFilterTextbox
			// 
			RecentFilterTextbox.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			RecentFilterTextbox.Location = new System.Drawing.Point(371, 450);
			RecentFilterTextbox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			RecentFilterTextbox.Name = "RecentFilterTextbox";
			RecentFilterTextbox.Size = new System.Drawing.Size(101, 23);
			RecentFilterTextbox.TabIndex = 33;
			RecentFilterTextbox.TextChanged += RecentFilterTextChanged;
			// 
			// FilterRecentLabel
			// 
			FilterRecentLabel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			FilterRecentLabel.Location = new System.Drawing.Point(371, 415);
			FilterRecentLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			FilterRecentLabel.Name = "FilterRecentLabel";
			FilterRecentLabel.Size = new System.Drawing.Size(102, 28);
			FilterRecentLabel.TabIndex = 32;
			FilterRecentLabel.Text = "Filter Recent:";
			FilterRecentLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// SmoothingGroupsButton
			// 
			SmoothingGroupsButton.Enabled = false;
			SmoothingGroupsButton.Location = new System.Drawing.Point(210, 80);
			SmoothingGroupsButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			SmoothingGroupsButton.Name = "SmoothingGroupsButton";
			SmoothingGroupsButton.Size = new System.Drawing.Size(136, 27);
			SmoothingGroupsButton.TabIndex = 31;
			SmoothingGroupsButton.Text = "Smoothing Groups";
			SmoothingGroupsButton.UseVisualStyleBackColor = true;
			SmoothingGroupsButton.Click += SmoothingGroupsButtonClicked;
			// 
			// AlignGroup
			// 
			AlignGroup.Controls.Add(AlignToFaceCheckbox);
			AlignGroup.Controls.Add(AlignToWorldCheckbox);
			AlignGroup.Location = new System.Drawing.Point(203, 112);
			AlignGroup.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			AlignGroup.Name = "AlignGroup";
			AlignGroup.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
			AlignGroup.Size = new System.Drawing.Size(142, 60);
			AlignGroup.TabIndex = 30;
			AlignGroup.TabStop = false;
			AlignGroup.Text = "Align";
			// 
			// AlignToFaceCheckbox
			// 
			AlignToFaceCheckbox.AutoSize = true;
			AlignToFaceCheckbox.Location = new System.Drawing.Point(74, 25);
			AlignToFaceCheckbox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			AlignToFaceCheckbox.Name = "AlignToFaceCheckbox";
			AlignToFaceCheckbox.Size = new System.Drawing.Size(50, 19);
			AlignToFaceCheckbox.TabIndex = 0;
			AlignToFaceCheckbox.Text = "Face";
			AlignToFaceCheckbox.UseVisualStyleBackColor = true;
			AlignToFaceCheckbox.Click += AlignToFaceClicked;
			// 
			// AlignToWorldCheckbox
			// 
			AlignToWorldCheckbox.AutoSize = true;
			AlignToWorldCheckbox.Location = new System.Drawing.Point(8, 25);
			AlignToWorldCheckbox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			AlignToWorldCheckbox.Name = "AlignToWorldCheckbox";
			AlignToWorldCheckbox.Size = new System.Drawing.Size(58, 19);
			AlignToWorldCheckbox.TabIndex = 0;
			AlignToWorldCheckbox.Text = "World";
			AlignToWorldCheckbox.UseVisualStyleBackColor = true;
			AlignToWorldCheckbox.Click += AlignToWorldClicked;
			// 
			// JustifyGroup
			// 
			JustifyGroup.Controls.Add(RLeftButton);
			JustifyGroup.Controls.Add(RRightButton);
			JustifyGroup.Controls.Add(JustifyTopButton);
			JustifyGroup.Controls.Add(JustifyFitButton);
			JustifyGroup.Controls.Add(TreatAsOneCheckbox);
			JustifyGroup.Controls.Add(JustifyRightButton);
			JustifyGroup.Controls.Add(JustifyBottomButton);
			JustifyGroup.Controls.Add(JustifyCenterButton);
			JustifyGroup.Controls.Add(JustifyLeftButton);
			JustifyGroup.Location = new System.Drawing.Point(354, 14);
			JustifyGroup.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			JustifyGroup.Name = "JustifyGroup";
			JustifyGroup.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
			JustifyGroup.Size = new System.Drawing.Size(119, 158);
			JustifyGroup.TabIndex = 29;
			JustifyGroup.TabStop = false;
			JustifyGroup.Text = "Justify";
			// 
			// RLeftButton
			// 
			RLeftButton.Location = new System.Drawing.Point(75, 17);
			RLeftButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			RLeftButton.Name = "RLeftButton";
			RLeftButton.Size = new System.Drawing.Size(23, 23);
			RLeftButton.TabIndex = 7;
			RLeftButton.Text = "↰";
			RLeftButton.UseVisualStyleBackColor = true;
			RLeftButton.Click += RLeftButton_Click;
			// 
			// RRightButton
			// 
			RRightButton.Location = new System.Drawing.Point(19, 17);
			RRightButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			RRightButton.Name = "RRightButton";
			RRightButton.Size = new System.Drawing.Size(23, 23);
			RRightButton.TabIndex = 6;
			RRightButton.Text = "↱";
			RRightButton.UseVisualStyleBackColor = true;
			RRightButton.Click += RRightButton_Click;
			// 
			// JustifyTopButton
			// 
			JustifyTopButton.Location = new System.Drawing.Point(47, 17);
			JustifyTopButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			JustifyTopButton.Name = "JustifyTopButton";
			JustifyTopButton.Size = new System.Drawing.Size(23, 23);
			JustifyTopButton.TabIndex = 3;
			JustifyTopButton.Text = "T";
			JustifyTopButton.UseVisualStyleBackColor = true;
			JustifyTopButton.Click += JustifyTopClicked;
			// 
			// JustifyFitButton
			// 
			JustifyFitButton.Location = new System.Drawing.Point(19, 100);
			JustifyFitButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			JustifyFitButton.Name = "JustifyFitButton";
			JustifyFitButton.Size = new System.Drawing.Size(79, 23);
			JustifyFitButton.TabIndex = 4;
			JustifyFitButton.Text = "Fit";
			JustifyFitButton.UseVisualStyleBackColor = true;
			JustifyFitButton.Click += JustifyFitClicked;
			// 
			// TreatAsOneCheckbox
			// 
			TreatAsOneCheckbox.Location = new System.Drawing.Point(7, 130);
			TreatAsOneCheckbox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			TreatAsOneCheckbox.Name = "TreatAsOneCheckbox";
			TreatAsOneCheckbox.Size = new System.Drawing.Size(105, 24);
			TreatAsOneCheckbox.TabIndex = 5;
			TreatAsOneCheckbox.Text = "Treat as One";
			TreatAsOneCheckbox.UseVisualStyleBackColor = true;
			TreatAsOneCheckbox.CheckedChanged += TreatAsOneCheckboxToggled;
			// 
			// JustifyRightButton
			// 
			JustifyRightButton.Location = new System.Drawing.Point(75, 45);
			JustifyRightButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			JustifyRightButton.Name = "JustifyRightButton";
			JustifyRightButton.Size = new System.Drawing.Size(23, 23);
			JustifyRightButton.TabIndex = 3;
			JustifyRightButton.Text = "R";
			JustifyRightButton.UseVisualStyleBackColor = true;
			JustifyRightButton.Click += JustifyRightClicked;
			// 
			// JustifyBottomButton
			// 
			JustifyBottomButton.Location = new System.Drawing.Point(47, 73);
			JustifyBottomButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			JustifyBottomButton.Name = "JustifyBottomButton";
			JustifyBottomButton.Size = new System.Drawing.Size(23, 23);
			JustifyBottomButton.TabIndex = 3;
			JustifyBottomButton.Text = "B";
			JustifyBottomButton.UseVisualStyleBackColor = true;
			JustifyBottomButton.Click += JustifyBottomClicked;
			// 
			// JustifyCenterButton
			// 
			JustifyCenterButton.Location = new System.Drawing.Point(47, 45);
			JustifyCenterButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			JustifyCenterButton.Name = "JustifyCenterButton";
			JustifyCenterButton.Size = new System.Drawing.Size(23, 23);
			JustifyCenterButton.TabIndex = 3;
			JustifyCenterButton.Text = "C";
			JustifyCenterButton.UseVisualStyleBackColor = true;
			JustifyCenterButton.Click += JustifyCenterClicked;
			// 
			// JustifyLeftButton
			// 
			JustifyLeftButton.Location = new System.Drawing.Point(19, 45);
			JustifyLeftButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			JustifyLeftButton.Name = "JustifyLeftButton";
			JustifyLeftButton.Size = new System.Drawing.Size(23, 23);
			JustifyLeftButton.TabIndex = 3;
			JustifyLeftButton.Text = "L";
			JustifyLeftButton.UseVisualStyleBackColor = true;
			JustifyLeftButton.Click += JustifyLeftClicked;
			// 
			// ApplyButton
			// 
			ApplyButton.Location = new System.Drawing.Point(108, 145);
			ApplyButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			ApplyButton.Name = "ApplyButton";
			ApplyButton.Size = new System.Drawing.Size(88, 27);
			ApplyButton.TabIndex = 22;
			ApplyButton.Text = "Apply";
			ApplyButton.UseVisualStyleBackColor = true;
			ApplyButton.Click += ApplyButtonClicked;
			// 
			// RotationValue
			// 
			RotationValue.BackColor = System.Drawing.SystemColors.Window;
			RotationValue.DecimalPlaces = 2;
			RotationValue.Location = new System.Drawing.Point(279, 20);
			RotationValue.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			RotationValue.Maximum = new decimal(new int[] { 360, 0, 0, 0 });
			RotationValue.Minimum = new decimal(new int[] { 360, 0, 0, int.MinValue });
			RotationValue.Name = "RotationValue";
			RotationValue.Size = new System.Drawing.Size(66, 23);
			RotationValue.TabIndex = 18;
			RotationValue.ValueChanged += RotationValueChanged;
			RotationValue.Enter += FocusTextInControl;
			// 
			// ReplaceButton
			// 
			ReplaceButton.Location = new System.Drawing.Point(14, 145);
			ReplaceButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			ReplaceButton.Name = "ReplaceButton";
			ReplaceButton.Size = new System.Drawing.Size(88, 27);
			ReplaceButton.TabIndex = 24;
			ReplaceButton.Text = "Replace...";
			ReplaceButton.UseVisualStyleBackColor = true;
			ReplaceButton.Click += ReplaceButtonClicked;
			// 
			// BrowseButton
			// 
			BrowseButton.Location = new System.Drawing.Point(14, 112);
			BrowseButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			BrowseButton.Name = "BrowseButton";
			BrowseButton.Size = new System.Drawing.Size(88, 27);
			BrowseButton.TabIndex = 23;
			BrowseButton.Text = "Browse...";
			BrowseButton.UseVisualStyleBackColor = true;
			BrowseButton.Click += BrowseButtonClicked;
			// 
			// RotationLabel
			// 
			RotationLabel.Location = new System.Drawing.Point(203, 15);
			RotationLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			RotationLabel.Name = "RotationLabel";
			RotationLabel.Size = new System.Drawing.Size(69, 29);
			RotationLabel.TabIndex = 17;
			RotationLabel.Text = "Rotation";
			RotationLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// TextureDetailsLabel
			// 
			TextureDetailsLabel.Location = new System.Drawing.Point(14, 175);
			TextureDetailsLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			TextureDetailsLabel.Name = "TextureDetailsLabel";
			TextureDetailsLabel.Size = new System.Drawing.Size(458, 27);
			TextureDetailsLabel.TabIndex = 21;
			TextureDetailsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tableLayoutPanel1
			// 
			tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
			tableLayoutPanel1.ColumnCount = 3;
			tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 23F));
			tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 76F));
			tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 267F));
			tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 23F));
			tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 23F));
			tableLayoutPanel1.Controls.Add(ScaleXValue, 1, 1);
			tableLayoutPanel1.Controls.Add(ScaleLabel, 1, 0);
			tableLayoutPanel1.Controls.Add(label2, 0, 1);
			tableLayoutPanel1.Controls.Add(label3, 0, 2);
			tableLayoutPanel1.Controls.Add(ShiftLabel, 2, 0);
			tableLayoutPanel1.Controls.Add(ScaleYValue, 1, 2);
			tableLayoutPanel1.Controls.Add(ShiftXValue, 2, 1);
			tableLayoutPanel1.Controls.Add(ShiftYValue, 2, 2);
			tableLayoutPanel1.Location = new System.Drawing.Point(14, 14);
			tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			tableLayoutPanel1.Name = "tableLayoutPanel1";
			tableLayoutPanel1.RowCount = 3;
			tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
			tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
			tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
			tableLayoutPanel1.Size = new System.Drawing.Size(182, 92);
			tableLayoutPanel1.TabIndex = 20;
			// 
			// ScaleXValue
			// 
			ScaleXValue.DecimalPlaces = 4;
			ScaleXValue.Increment = new decimal(new int[] { 1, 0, 0, 131072 });
			ScaleXValue.Location = new System.Drawing.Point(29, 34);
			ScaleXValue.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			ScaleXValue.Maximum = new decimal(new int[] { 4096, 0, 0, 0 });
			ScaleXValue.Minimum = new decimal(new int[] { 4096, 0, 0, int.MinValue });
			ScaleXValue.Name = "ScaleXValue";
			ScaleXValue.Size = new System.Drawing.Size(66, 23);
			ScaleXValue.TabIndex = 1;
			ScaleXValue.Value = new decimal(new int[] { 100, 0, 0, 131072 });
			ScaleXValue.WheelIncrement = new decimal(new int[] { 1, 0, 0, 65536 });
			ScaleXValue.ValueChanged += ScaleXValueChanged;
			ScaleXValue.Enter += FocusTextInControl;
			// 
			// ScaleLabel
			// 
			ScaleLabel.Location = new System.Drawing.Point(29, 1);
			ScaleLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			ScaleLabel.Name = "ScaleLabel";
			ScaleLabel.Size = new System.Drawing.Size(68, 29);
			ScaleLabel.TabIndex = 0;
			ScaleLabel.Text = "Scale";
			ScaleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label2
			// 
			label2.Location = new System.Drawing.Point(5, 31);
			label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(15, 29);
			label2.TabIndex = 2;
			label2.Text = "X";
			label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label3
			// 
			label3.Location = new System.Drawing.Point(5, 61);
			label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(15, 30);
			label3.TabIndex = 2;
			label3.Text = "Y";
			label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// ShiftLabel
			// 
			ShiftLabel.Location = new System.Drawing.Point(106, 1);
			ShiftLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			ShiftLabel.Name = "ShiftLabel";
			ShiftLabel.Size = new System.Drawing.Size(69, 29);
			ShiftLabel.TabIndex = 0;
			ShiftLabel.Text = "Shift";
			ShiftLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// ScaleYValue
			// 
			ScaleYValue.DecimalPlaces = 4;
			ScaleYValue.Increment = new decimal(new int[] { 1, 0, 0, 131072 });
			ScaleYValue.Location = new System.Drawing.Point(29, 64);
			ScaleYValue.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			ScaleYValue.Maximum = new decimal(new int[] { 4096, 0, 0, 0 });
			ScaleYValue.Minimum = new decimal(new int[] { 4096, 0, 0, int.MinValue });
			ScaleYValue.Name = "ScaleYValue";
			ScaleYValue.Size = new System.Drawing.Size(66, 23);
			ScaleYValue.TabIndex = 1;
			ScaleYValue.Value = new decimal(new int[] { 100, 0, 0, 131072 });
			ScaleYValue.WheelIncrement = new decimal(new int[] { 1, 0, 0, 65536 });
			ScaleYValue.ValueChanged += ScaleYValueChanged;
			ScaleYValue.Enter += FocusTextInControl;
			// 
			// ShiftXValue
			// 
			ShiftXValue.CtrlWheelMultiplier = new decimal(new int[] { 0, 0, 0, 0 });
			ShiftXValue.Location = new System.Drawing.Point(106, 34);
			ShiftXValue.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			ShiftXValue.Maximum = new decimal(new int[] { 4096, 0, 0, 0 });
			ShiftXValue.Minimum = new decimal(new int[] { 4096, 0, 0, int.MinValue });
			ShiftXValue.Name = "ShiftXValue";
			ShiftXValue.Size = new System.Drawing.Size(68, 23);
			ShiftXValue.TabIndex = 1;
			ShiftXValue.Value = new decimal(new int[] { 1, 0, 0, 65536 });
			ShiftXValue.ValueChanged += ShiftXValueChanged;
			ShiftXValue.Enter += FocusTextInControl;
			// 
			// ShiftYValue
			// 
			ShiftYValue.CtrlWheelMultiplier = new decimal(new int[] { 0, 0, 0, 0 });
			ShiftYValue.Location = new System.Drawing.Point(106, 64);
			ShiftYValue.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			ShiftYValue.Maximum = new decimal(new int[] { 4096, 0, 0, 0 });
			ShiftYValue.Minimum = new decimal(new int[] { 4096, 0, 0, int.MinValue });
			ShiftYValue.Name = "ShiftYValue";
			ShiftYValue.Size = new System.Drawing.Size(68, 23);
			ShiftYValue.TabIndex = 1;
			ShiftYValue.Value = new decimal(new int[] { 1, 0, 0, 65536 });
			ShiftYValue.ValueChanged += ShiftYValueChanged;
			ShiftYValue.Enter += FocusTextInControl;
			// 
			// LightmapLabel
			// 
			LightmapLabel.Location = new System.Drawing.Point(203, 44);
			LightmapLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			LightmapLabel.Name = "LightmapLabel";
			LightmapLabel.Size = new System.Drawing.Size(69, 29);
			LightmapLabel.TabIndex = 16;
			LightmapLabel.Text = "Lightmap";
			LightmapLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// LightmapValue
			// 
			LightmapValue.CtrlWheelMultiplier = new decimal(new int[] { 0, 0, 0, 0 });
			LightmapValue.Enabled = false;
			LightmapValue.Location = new System.Drawing.Point(279, 50);
			LightmapValue.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			LightmapValue.Maximum = new decimal(new int[] { 512, 0, 0, 0 });
			LightmapValue.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
			LightmapValue.Name = "LightmapValue";
			LightmapValue.ShiftWheelMultiplier = new decimal(new int[] { 0, 0, 0, 0 });
			LightmapValue.Size = new System.Drawing.Size(68, 23);
			LightmapValue.TabIndex = 19;
			LightmapValue.Value = new decimal(new int[] { 16, 0, 0, 0 });
			LightmapValue.ValueChanged += LightmapValueChanged;
			LightmapValue.Enter += FocusTextInControl;
			// 
			// HoverTip
			// 
			HoverTip.AutoPopDelay = 5000;
			HoverTip.InitialDelay = 200;
			HoverTip.IsBalloon = true;
			HoverTip.ReshowDelay = 100;
			// 
			// SelectedTextureListPanel
			// 
			SelectedTextureListPanel.Location = new System.Drawing.Point(14, 205);
			SelectedTextureListPanel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			SelectedTextureListPanel.Name = "SelectedTextureListPanel";
			SelectedTextureListPanel.Size = new System.Drawing.Size(350, 238);
			SelectedTextureListPanel.TabIndex = 35;
			// 
			// RecentTextureListPanel
			// 
			RecentTextureListPanel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			RecentTextureListPanel.Location = new System.Drawing.Point(371, 205);
			RecentTextureListPanel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			RecentTextureListPanel.Name = "RecentTextureListPanel";
			RecentTextureListPanel.Size = new System.Drawing.Size(102, 207);
			RecentTextureListPanel.TabIndex = 36;
			// 
			// LeftClickActionButton
			// 
			LeftClickActionButton.Location = new System.Drawing.Point(14, 450);
			LeftClickActionButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			LeftClickActionButton.Menu = LeftClickActionMenu;
			LeftClickActionButton.Name = "LeftClickActionButton";
			LeftClickActionButton.Size = new System.Drawing.Size(173, 27);
			LeftClickActionButton.TabIndex = 37;
			LeftClickActionButton.Text = "Left click: Lift";
			LeftClickActionButton.UseVisualStyleBackColor = true;
			// 
			// LeftClickActionMenu
			// 
			LeftClickActionMenu.Name = "LeftClickActionMenu";
			LeftClickActionMenu.Size = new System.Drawing.Size(61, 4);
			LeftClickActionMenu.ItemClicked += SetLeftClickAction;
			// 
			// RightClickActionButton
			// 
			RightClickActionButton.Location = new System.Drawing.Point(191, 450);
			RightClickActionButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			RightClickActionButton.Menu = RightClickActionMenu;
			RightClickActionButton.Name = "RightClickActionButton";
			RightClickActionButton.Size = new System.Drawing.Size(173, 27);
			RightClickActionButton.TabIndex = 37;
			RightClickActionButton.Text = "Right click: Apply";
			RightClickActionButton.UseVisualStyleBackColor = true;
			// 
			// RightClickActionMenu
			// 
			RightClickActionMenu.Name = "RightClickActionMenu";
			RightClickActionMenu.Size = new System.Drawing.Size(61, 4);
			RightClickActionMenu.ItemClicked += SetRightClickAction;
			// 
			// ResetButton
			// 
			ResetButton.Location = new System.Drawing.Point(110, 112);
			ResetButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			ResetButton.Name = "ResetButton";
			ResetButton.Size = new System.Drawing.Size(88, 27);
			ResetButton.TabIndex = 38;
			ResetButton.Text = "Reset";
			ResetButton.UseVisualStyleBackColor = true;
			ResetButton.Click += ResetButton_Click;
			// 
			// apply_null
			// 
			apply_null.Location = new System.Drawing.Point(272, 175);
			apply_null.Name = "apply_null";
			apply_null.Size = new System.Drawing.Size(75, 27);
			apply_null.TabIndex = 39;
			apply_null.Text = "NULL";
			apply_null.UseVisualStyleBackColor = true;
			apply_null.Click += apply_null_Click;
			// 
			// TextureApplicationForm
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			ClientSize = new System.Drawing.Size(486, 485);
			Controls.Add(apply_null);
			Controls.Add(ResetButton);
			Controls.Add(RightClickActionButton);
			Controls.Add(LeftClickActionButton);
			Controls.Add(RecentTextureListPanel);
			Controls.Add(SelectedTextureListPanel);
			Controls.Add(HideMaskCheckbox);
			Controls.Add(RecentFilterTextbox);
			Controls.Add(FilterRecentLabel);
			Controls.Add(SmoothingGroupsButton);
			Controls.Add(AlignGroup);
			Controls.Add(JustifyGroup);
			Controls.Add(ApplyButton);
			Controls.Add(RotationValue);
			Controls.Add(ReplaceButton);
			Controls.Add(BrowseButton);
			Controls.Add(RotationLabel);
			Controls.Add(TextureDetailsLabel);
			Controls.Add(tableLayoutPanel1);
			Controls.Add(LightmapLabel);
			Controls.Add(LightmapValue);
			FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			Name = "TextureApplicationForm";
			ShowIcon = false;
			ShowInTaskbar = false;
			StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			Text = "Texture Application";
			FormClosing += OnClosing;
			AlignGroup.ResumeLayout(false);
			AlignGroup.PerformLayout();
			JustifyGroup.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)RotationValue).EndInit();
			tableLayoutPanel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)ScaleXValue).EndInit();
			((System.ComponentModel.ISupportInitialize)ScaleYValue).EndInit();
			((System.ComponentModel.ISupportInitialize)ShiftXValue).EndInit();
			((System.ComponentModel.ISupportInitialize)ShiftYValue).EndInit();
			((System.ComponentModel.ISupportInitialize)LightmapValue).EndInit();
			ResumeLayout(false);
			PerformLayout();

		}

		#endregion
		private System.Windows.Forms.CheckBox HideMaskCheckbox;
        private System.Windows.Forms.TextBox RecentFilterTextbox;
        private System.Windows.Forms.Label FilterRecentLabel;
        private System.Windows.Forms.Button SmoothingGroupsButton;
        private System.Windows.Forms.GroupBox AlignGroup;
        private System.Windows.Forms.GroupBox JustifyGroup;
        private System.Windows.Forms.Button JustifyTopButton;
        private System.Windows.Forms.Button JustifyFitButton;
        private System.Windows.Forms.CheckBox TreatAsOneCheckbox;
        private System.Windows.Forms.Button JustifyRightButton;
        private System.Windows.Forms.Button JustifyBottomButton;
        private System.Windows.Forms.Button JustifyCenterButton;
        private System.Windows.Forms.Button JustifyLeftButton;
        private System.Windows.Forms.Button ApplyButton;
        private Sledge.Shell.Controls.NumericUpDownEx RotationValue;
        private System.Windows.Forms.Button ReplaceButton;
        private System.Windows.Forms.Button BrowseButton;
        private System.Windows.Forms.Label RotationLabel;
        private System.Windows.Forms.Label TextureDetailsLabel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private Sledge.Shell.Controls.NumericUpDownEx ScaleXValue;
        private System.Windows.Forms.Label ScaleLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label ShiftLabel;
        private Sledge.Shell.Controls.NumericUpDownEx ScaleYValue;
        private Sledge.Shell.Controls.NumericUpDownEx ShiftXValue;
        private Sledge.Shell.Controls.NumericUpDownEx ShiftYValue;
        private System.Windows.Forms.Label LightmapLabel;
        private Sledge.Shell.Controls.NumericUpDownEx LightmapValue;
        private System.Windows.Forms.ToolTip HoverTip;
        private System.Windows.Forms.CheckBox AlignToFaceCheckbox;
        private System.Windows.Forms.CheckBox AlignToWorldCheckbox;
        private System.Windows.Forms.Panel SelectedTextureListPanel;
        private System.Windows.Forms.Panel RecentTextureListPanel;
        private Shell.Controls.DropdownButton LeftClickActionButton;
        private Shell.Controls.DropdownButton RightClickActionButton;
        private System.Windows.Forms.ContextMenuStrip LeftClickActionMenu;
        private System.Windows.Forms.ContextMenuStrip RightClickActionMenu;
		private System.Windows.Forms.Button ResetButton;
		private System.Windows.Forms.Button RRightButton;
		private System.Windows.Forms.Button RLeftButton;
		private System.Windows.Forms.Button apply_null;
	}
}