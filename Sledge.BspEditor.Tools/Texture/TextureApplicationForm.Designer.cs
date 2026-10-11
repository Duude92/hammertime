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
            SmoothingGroupsButton = new System.Windows.Forms.Button();
            AlignGroup = new System.Windows.Forms.GroupBox();
            AlignToFaceCheckbox = new System.Windows.Forms.CheckBox();
            AlignToWorldCheckbox = new System.Windows.Forms.CheckBox();
            JustifyGroup = new System.Windows.Forms.GroupBox();
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
            ScaleXNegateButton = new System.Windows.Forms.Button();
            ScaleYNegateButton = new System.Windows.Forms.Button();
            ShiftLabel = new System.Windows.Forms.Label();
            ScaleYValue = new Sledge.Shell.Controls.NumericUpDownEx();
            ShiftXValue = new Sledge.Shell.Controls.NumericUpDownEx();
            ShiftYValue = new Sledge.Shell.Controls.NumericUpDownEx();
            LightmapLabel = new System.Windows.Forms.Label();
            LightmapValue = new Sledge.Shell.Controls.NumericUpDownEx();
            HoverTip = new System.Windows.Forms.ToolTip(components);
            TextureViewerPanel = new System.Windows.Forms.Panel();
            MarkButton = new System.Windows.Forms.Button();
            LeftClickActionButton = new Sledge.Shell.Controls.DropdownButton();
            LeftClickActionMenu = new System.Windows.Forms.ContextMenuStrip(components);
            RightClickActionButton = new Sledge.Shell.Controls.DropdownButton();
            RightClickActionMenu = new System.Windows.Forms.ContextMenuStrip(components);
            ResetButton = new System.Windows.Forms.Button();
            RotateLabel = new System.Windows.Forms.Label();
            RotPlus45Button = new System.Windows.Forms.Button();
            RotPlus90Button = new System.Windows.Forms.Button();
            RotPlus180Button = new System.Windows.Forms.Button();
            RotMinus45Button = new System.Windows.Forms.Button();
            RotMinus90Button = new System.Windows.Forms.Button();
            RotMinus180Button = new System.Windows.Forms.Button();
            apply_null = new System.Windows.Forms.Button();
            lightmapGrp = new System.Windows.Forms.Panel();
            UvVectorsLabel = new System.Windows.Forms.Label();
            UvUX = new System.Windows.Forms.TextBox();
            UvUY = new System.Windows.Forms.TextBox();
            UvUZ = new System.Windows.Forms.TextBox();
            UvVX = new System.Windows.Forms.TextBox();
            UvVY = new System.Windows.Forms.TextBox();
            UvVZ = new System.Windows.Forms.TextBox();
            UvMappingButton = new System.Windows.Forms.Button();
            AlignGroup.SuspendLayout();
            JustifyGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)RotationValue).BeginInit();
            tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)ScaleXValue).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ScaleYValue).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ShiftXValue).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ShiftYValue).BeginInit();
            ((System.ComponentModel.ISupportInitialize)LightmapValue).BeginInit();
            lightmapGrp.SuspendLayout();
            SuspendLayout();
            // 
            // HideMaskCheckbox
            // 
            HideMaskCheckbox.Appearance = System.Windows.Forms.Appearance.Button;
            HideMaskCheckbox.Location = new System.Drawing.Point(371, 563);
            HideMaskCheckbox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            HideMaskCheckbox.Name = "HideMaskCheckbox";
            HideMaskCheckbox.Size = new System.Drawing.Size(102, 27);
            HideMaskCheckbox.TabIndex = 34;
            HideMaskCheckbox.Text = "Hide Mask";
            HideMaskCheckbox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            HideMaskCheckbox.UseVisualStyleBackColor = true;
            HideMaskCheckbox.CheckedChanged += HideMaskCheckboxToggled;
            // 
            // SmoothingGroupsButton
            // 
            SmoothingGroupsButton.Enabled = false;
            SmoothingGroupsButton.Location = new System.Drawing.Point(4, 34);
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
            AlignGroup.Location = new System.Drawing.Point(211, 10);
            AlignGroup.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            AlignGroup.Name = "AlignGroup";
            AlignGroup.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            AlignGroup.Size = new System.Drawing.Size(100, 96);
            AlignGroup.TabIndex = 30;
            AlignGroup.TabStop = false;
            AlignGroup.Text = "Align";
            // 
            // AlignToFaceCheckbox
            // 
            AlignToFaceCheckbox.AutoSize = true;
            AlignToFaceCheckbox.Location = new System.Drawing.Point(12, 58);
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
            AlignToWorldCheckbox.Location = new System.Drawing.Point(12, 26);
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
            JustifyGroup.Controls.Add(JustifyTopButton);
            JustifyGroup.Controls.Add(JustifyFitButton);
            JustifyGroup.Controls.Add(JustifyRightButton);
            JustifyGroup.Controls.Add(JustifyBottomButton);
            JustifyGroup.Controls.Add(JustifyCenterButton);
            JustifyGroup.Controls.Add(JustifyLeftButton);
            JustifyGroup.Location = new System.Drawing.Point(14, 116);
            JustifyGroup.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            JustifyGroup.Name = "JustifyGroup";
            JustifyGroup.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            JustifyGroup.Size = new System.Drawing.Size(119, 106);
            JustifyGroup.TabIndex = 29;
            JustifyGroup.TabStop = false;
            JustifyGroup.Text = "Justify";
            // 
            // JustifyTopButton
            // 
            JustifyTopButton.Location = new System.Drawing.Point(44, 18);
            JustifyTopButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            JustifyTopButton.Name = "JustifyTopButton";
            JustifyTopButton.Size = new System.Drawing.Size(32, 23);
            JustifyTopButton.TabIndex = 3;
            JustifyTopButton.Text = "T";
            JustifyTopButton.UseVisualStyleBackColor = true;
            JustifyTopButton.Click += JustifyTopClicked;
            // 
            // JustifyFitButton
            // 
            JustifyFitButton.Location = new System.Drawing.Point(8, 18);
            JustifyFitButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            JustifyFitButton.Name = "JustifyFitButton";
            JustifyFitButton.Size = new System.Drawing.Size(32, 23);
            JustifyFitButton.TabIndex = 4;
            JustifyFitButton.Text = "Fit";
            JustifyFitButton.UseVisualStyleBackColor = true;
            JustifyFitButton.Click += JustifyFitClicked;
            // 
            // TreatAsOneCheckbox
            // 
            TreatAsOneCheckbox.Location = new System.Drawing.Point(14, 228);
            TreatAsOneCheckbox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TreatAsOneCheckbox.Name = "TreatAsOneCheckbox";
            TreatAsOneCheckbox.Size = new System.Drawing.Size(134, 24);
            TreatAsOneCheckbox.TabIndex = 5;
            TreatAsOneCheckbox.Text = "Treat as One";
            TreatAsOneCheckbox.UseVisualStyleBackColor = true;
            TreatAsOneCheckbox.CheckedChanged += TreatAsOneCheckboxToggled;
            // 
            // JustifyRightButton
            // 
            JustifyRightButton.Location = new System.Drawing.Point(80, 46);
            JustifyRightButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            JustifyRightButton.Name = "JustifyRightButton";
            JustifyRightButton.Size = new System.Drawing.Size(32, 23);
            JustifyRightButton.TabIndex = 3;
            JustifyRightButton.Text = "R";
            JustifyRightButton.UseVisualStyleBackColor = true;
            JustifyRightButton.Click += JustifyRightClicked;
            // 
            // JustifyBottomButton
            // 
            JustifyBottomButton.Location = new System.Drawing.Point(44, 74);
            JustifyBottomButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            JustifyBottomButton.Name = "JustifyBottomButton";
            JustifyBottomButton.Size = new System.Drawing.Size(32, 23);
            JustifyBottomButton.TabIndex = 3;
            JustifyBottomButton.Text = "B";
            JustifyBottomButton.UseVisualStyleBackColor = true;
            JustifyBottomButton.Click += JustifyBottomClicked;
            // 
            // JustifyCenterButton
            // 
            JustifyCenterButton.Location = new System.Drawing.Point(44, 46);
            JustifyCenterButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            JustifyCenterButton.Name = "JustifyCenterButton";
            JustifyCenterButton.Size = new System.Drawing.Size(32, 23);
            JustifyCenterButton.TabIndex = 3;
            JustifyCenterButton.Text = "C";
            JustifyCenterButton.UseVisualStyleBackColor = true;
            JustifyCenterButton.Click += JustifyCenterClicked;
            // 
            // JustifyLeftButton
            // 
            JustifyLeftButton.Location = new System.Drawing.Point(8, 46);
            JustifyLeftButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            JustifyLeftButton.Name = "JustifyLeftButton";
            JustifyLeftButton.Size = new System.Drawing.Size(32, 23);
            JustifyLeftButton.TabIndex = 3;
            JustifyLeftButton.Text = "L";
            JustifyLeftButton.UseVisualStyleBackColor = true;
            JustifyLeftButton.Click += JustifyLeftClicked;
            // 
            // ApplyButton
            // 
            ApplyButton.Location = new System.Drawing.Point(371, 384);
            ApplyButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            ApplyButton.Name = "ApplyButton";
            ApplyButton.Size = new System.Drawing.Size(102, 27);
            ApplyButton.TabIndex = 22;
            ApplyButton.Text = "Apply";
            ApplyButton.UseVisualStyleBackColor = true;
            ApplyButton.Click += ApplyButtonClicked;
            // 
            // RotationValue
            // 
            RotationValue.BackColor = System.Drawing.SystemColors.Window;
            RotationValue.DecimalPlaces = 2;
            RotationValue.Location = new System.Drawing.Point(402, 30);
            RotationValue.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            RotationValue.Maximum = new decimal(new int[] { 360, 0, 0, 0 });
            RotationValue.Minimum = new decimal(new int[] { 360, 0, 0, int.MinValue });
            RotationValue.Name = "RotationValue";
            RotationValue.Size = new System.Drawing.Size(70, 23);
            RotationValue.TabIndex = 18;
            RotationValue.ValueChanged += RotationValueChanged;
            RotationValue.Enter += FocusTextInControl;
            // 
            // ReplaceButton
            // 
            ReplaceButton.Location = new System.Drawing.Point(371, 351);
            ReplaceButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            ReplaceButton.Name = "ReplaceButton";
            ReplaceButton.Size = new System.Drawing.Size(102, 27);
            ReplaceButton.TabIndex = 24;
            ReplaceButton.Text = "Replace...";
            ReplaceButton.UseVisualStyleBackColor = true;
            ReplaceButton.Click += ReplaceButtonClicked;
            // 
            // BrowseButton
            // 
            BrowseButton.Location = new System.Drawing.Point(371, 318);
            BrowseButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            BrowseButton.Name = "BrowseButton";
            BrowseButton.Size = new System.Drawing.Size(102, 27);
            BrowseButton.TabIndex = 23;
            BrowseButton.Text = "Browse...";
            BrowseButton.UseVisualStyleBackColor = true;
            BrowseButton.Click += BrowseButtonClicked;
            // 
            // RotationLabel
            // 
            RotationLabel.Location = new System.Drawing.Point(402, 10);
            RotationLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            RotationLabel.Name = "RotationLabel";
            RotationLabel.Size = new System.Drawing.Size(70, 18);
            RotationLabel.TabIndex = 17;
            RotationLabel.Text = "Rotation";
            RotationLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TextureDetailsLabel
            // 
            TextureDetailsLabel.Location = new System.Drawing.Point(14, 298);
            TextureDetailsLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            TextureDetailsLabel.Name = "TextureDetailsLabel";
            TextureDetailsLabel.Size = new System.Drawing.Size(458, 18);
            TextureDetailsLabel.TabIndex = 21;
            TextureDetailsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            tableLayoutPanel1.ColumnCount = 3;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 27F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 78F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 78F));
            tableLayoutPanel1.Controls.Add(ScaleXValue, 1, 1);
            tableLayoutPanel1.Controls.Add(ScaleLabel, 1, 0);
            tableLayoutPanel1.Controls.Add(ScaleXNegateButton, 0, 1);
            tableLayoutPanel1.Controls.Add(ScaleYNegateButton, 0, 2);
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
            tableLayoutPanel1.Size = new System.Drawing.Size(187, 92);
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
            // ScaleXNegateButton
            // 
            ScaleXNegateButton.Dock = System.Windows.Forms.DockStyle.Fill;
            ScaleXNegateButton.Margin = new System.Windows.Forms.Padding(1);
            ScaleXNegateButton.Name = "ScaleXNegateButton";
            ScaleXNegateButton.TabIndex = 2;
            ScaleXNegateButton.Text = "X";
            ScaleXNegateButton.UseVisualStyleBackColor = true;
            ScaleXNegateButton.Click += ScaleXNegateClicked;
            // 
            // ScaleYNegateButton
            // 
            ScaleYNegateButton.Dock = System.Windows.Forms.DockStyle.Fill;
            ScaleYNegateButton.Margin = new System.Windows.Forms.Padding(1);
            ScaleYNegateButton.Name = "ScaleYNegateButton";
            ScaleYNegateButton.TabIndex = 3;
            ScaleYNegateButton.Text = "Y";
            ScaleYNegateButton.UseVisualStyleBackColor = true;
            ScaleYNegateButton.Click += ScaleYNegateClicked;
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
            LightmapLabel.Location = new System.Drawing.Point(0, 0);
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
            LightmapValue.Location = new System.Drawing.Point(72, 5);
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
            // MarkButton
            // 
            MarkButton.Location = new System.Drawing.Point(371, 417);
            MarkButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MarkButton.Name = "MarkButton";
            MarkButton.Size = new System.Drawing.Size(102, 27);
            MarkButton.TabIndex = 47;
            MarkButton.Text = "Mark";
            MarkButton.UseVisualStyleBackColor = true;
            MarkButton.Click += MarkButtonClicked;
            // 
            // TextureViewerPanel
            // 
            TextureViewerPanel.Location = new System.Drawing.Point(14, 318);
            TextureViewerPanel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TextureViewerPanel.Name = "TextureViewerPanel";
            TextureViewerPanel.Size = new System.Drawing.Size(350, 272);
            TextureViewerPanel.TabIndex = 35;
            // 
            // LeftClickActionButton
            // 
            LeftClickActionButton.Location = new System.Drawing.Point(14, 598);
            LeftClickActionButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            LeftClickActionButton.Menu = LeftClickActionMenu;
            LeftClickActionButton.Name = "LeftClickActionButton";
            LeftClickActionButton.Size = new System.Drawing.Size(171, 23);
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
            RightClickActionButton.Location = new System.Drawing.Point(193, 598);
            RightClickActionButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            RightClickActionButton.Menu = RightClickActionMenu;
            RightClickActionButton.Name = "RightClickActionButton";
            RightClickActionButton.Size = new System.Drawing.Size(171, 23);
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
            ResetButton.Location = new System.Drawing.Point(402, 57);
            ResetButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            ResetButton.Name = "ResetButton";
            ResetButton.Size = new System.Drawing.Size(70, 23);
            ResetButton.TabIndex = 38;
            ResetButton.Text = "Reset";
            ResetButton.UseVisualStyleBackColor = true;
            ResetButton.Click += ResetButton_Click;
            // 
            // apply_null
            // 
            apply_null.Location = new System.Drawing.Point(402, 83);
            apply_null.Name = "apply_null";
            apply_null.Size = new System.Drawing.Size(70, 23);
            apply_null.TabIndex = 39;
            apply_null.Text = "NULL";
            apply_null.UseVisualStyleBackColor = true;
            apply_null.Click += apply_null_Click;
            // 
            // RotateLabel
            // 
            RotateLabel.Location = new System.Drawing.Point(143, 118);
            RotateLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            RotateLabel.Name = "RotateLabel";
            RotateLabel.Size = new System.Drawing.Size(80, 18);
            RotateLabel.TabIndex = 46;
            RotateLabel.Text = "Rotate:";
            RotateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // RotPlus45Button
            // 
            RotPlus45Button.Location = new System.Drawing.Point(143, 140);
            RotPlus45Button.Margin = new System.Windows.Forms.Padding(0);
            RotPlus45Button.Name = "RotPlus45Button";
            RotPlus45Button.Size = new System.Drawing.Size(44, 23);
            RotPlus45Button.TabIndex = 40;
            RotPlus45Button.Tag = 45F;
            RotPlus45Button.Text = "+45";
            RotPlus45Button.UseVisualStyleBackColor = true;
            RotPlus45Button.Click += RotateButton_Click;
            // 
            // RotPlus90Button
            // 
            RotPlus90Button.Location = new System.Drawing.Point(192, 140);
            RotPlus90Button.Margin = new System.Windows.Forms.Padding(0);
            RotPlus90Button.Name = "RotPlus90Button";
            RotPlus90Button.Size = new System.Drawing.Size(44, 23);
            RotPlus90Button.TabIndex = 41;
            RotPlus90Button.Tag = 90F;
            RotPlus90Button.Text = "+90";
            RotPlus90Button.UseVisualStyleBackColor = true;
            RotPlus90Button.Click += RotateButton_Click;
            // 
            // RotPlus180Button
            // 
            RotPlus180Button.Location = new System.Drawing.Point(241, 140);
            RotPlus180Button.Margin = new System.Windows.Forms.Padding(0);
            RotPlus180Button.Name = "RotPlus180Button";
            RotPlus180Button.Size = new System.Drawing.Size(44, 23);
            RotPlus180Button.TabIndex = 42;
            RotPlus180Button.Tag = 180F;
            RotPlus180Button.Text = "+180";
            RotPlus180Button.UseVisualStyleBackColor = true;
            RotPlus180Button.Click += RotateButton_Click;
            // 
            // RotMinus45Button
            // 
            RotMinus45Button.Location = new System.Drawing.Point(143, 166);
            RotMinus45Button.Margin = new System.Windows.Forms.Padding(0);
            RotMinus45Button.Name = "RotMinus45Button";
            RotMinus45Button.Size = new System.Drawing.Size(44, 23);
            RotMinus45Button.TabIndex = 43;
            RotMinus45Button.Tag = -45F;
            RotMinus45Button.Text = "-45";
            RotMinus45Button.UseVisualStyleBackColor = true;
            RotMinus45Button.Click += RotateButton_Click;
            // 
            // RotMinus90Button
            // 
            RotMinus90Button.Location = new System.Drawing.Point(192, 166);
            RotMinus90Button.Margin = new System.Windows.Forms.Padding(0);
            RotMinus90Button.Name = "RotMinus90Button";
            RotMinus90Button.Size = new System.Drawing.Size(44, 23);
            RotMinus90Button.TabIndex = 44;
            RotMinus90Button.Tag = -90F;
            RotMinus90Button.Text = "-90";
            RotMinus90Button.UseVisualStyleBackColor = true;
            RotMinus90Button.Click += RotateButton_Click;
            // 
            // RotMinus180Button
            // 
            RotMinus180Button.Location = new System.Drawing.Point(241, 166);
            RotMinus180Button.Margin = new System.Windows.Forms.Padding(0);
            RotMinus180Button.Name = "RotMinus180Button";
            RotMinus180Button.Size = new System.Drawing.Size(44, 23);
            RotMinus180Button.TabIndex = 45;
            RotMinus180Button.Tag = -180F;
            RotMinus180Button.Text = "-180";
            RotMinus180Button.UseVisualStyleBackColor = true;
            RotMinus180Button.Click += RotateButton_Click;
            // 
            // lightmapGrp
            // 
            lightmapGrp.Controls.Add(SmoothingGroupsButton);
            lightmapGrp.Controls.Add(LightmapValue);
            lightmapGrp.Controls.Add(LightmapLabel);
            lightmapGrp.Location = new System.Drawing.Point(299, 196);
            lightmapGrp.Name = "lightmapGrp";
            lightmapGrp.Size = new System.Drawing.Size(140, 61);
            lightmapGrp.TabIndex = 39;
            lightmapGrp.Text = "groupBox1";
            // 
            // UvVectorsLabel
            // 
            UvVectorsLabel.Location = new System.Drawing.Point(299, 118);
            UvVectorsLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            UvVectorsLabel.Name = "UvVectorsLabel";
            UvVectorsLabel.Size = new System.Drawing.Size(173, 18);
            UvVectorsLabel.TabIndex = 50;
            UvVectorsLabel.Text = "UV vectors ( X Y Z ):";
            // 
            // UvUX
            // 
            UvUX.BackColor = System.Drawing.SystemColors.Window;
            UvUX.Location = new System.Drawing.Point(299, 140);
            UvUX.Name = "UvUX";
            UvUX.Size = new System.Drawing.Size(54, 23);
            UvUX.TabIndex = 51;
            UvUX.KeyDown += UvKeyDown;
            UvUX.Leave += UvLeave;
            UvUX.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            HoverTip.SetToolTip(UvUX, "U axis, X");
            // 
            // UvUY
            // 
            UvUY.BackColor = System.Drawing.SystemColors.Window;
            UvUY.Location = new System.Drawing.Point(358, 140);
            UvUY.Name = "UvUY";
            UvUY.Size = new System.Drawing.Size(54, 23);
            UvUY.TabIndex = 52;
            UvUY.KeyDown += UvKeyDown;
            UvUY.Leave += UvLeave;
            UvUY.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            HoverTip.SetToolTip(UvUY, "U axis, Y");
            // 
            // UvUZ
            // 
            UvUZ.BackColor = System.Drawing.SystemColors.Window;
            UvUZ.Location = new System.Drawing.Point(417, 140);
            UvUZ.Name = "UvUZ";
            UvUZ.Size = new System.Drawing.Size(54, 23);
            UvUZ.TabIndex = 53;
            UvUZ.KeyDown += UvKeyDown;
            UvUZ.Leave += UvLeave;
            UvUZ.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            HoverTip.SetToolTip(UvUZ, "U axis, Z");
            // 
            // UvVX
            // 
            UvVX.BackColor = System.Drawing.SystemColors.Window;
            UvVX.Location = new System.Drawing.Point(299, 166);
            UvVX.Name = "UvVX";
            UvVX.Size = new System.Drawing.Size(54, 23);
            UvVX.TabIndex = 54;
            UvVX.KeyDown += UvKeyDown;
            UvVX.Leave += UvLeave;
            UvVX.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            HoverTip.SetToolTip(UvVX, "V axis, X");
            // 
            // UvVY
            // 
            UvVY.BackColor = System.Drawing.SystemColors.Window;
            UvVY.Location = new System.Drawing.Point(358, 166);
            UvVY.Name = "UvVY";
            UvVY.Size = new System.Drawing.Size(54, 23);
            UvVY.TabIndex = 55;
            UvVY.KeyDown += UvKeyDown;
            UvVY.Leave += UvLeave;
            UvVY.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            HoverTip.SetToolTip(UvVY, "V axis, Y");
            // 
            // UvVZ
            // 
            UvVZ.BackColor = System.Drawing.SystemColors.Window;
            UvVZ.Location = new System.Drawing.Point(417, 166);
            UvVZ.Name = "UvVZ";
            UvVZ.Size = new System.Drawing.Size(54, 23);
            UvVZ.TabIndex = 56;
            UvVZ.KeyDown += UvKeyDown;
            UvVZ.Leave += UvLeave;
            UvVZ.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            HoverTip.SetToolTip(UvVZ, "V axis, Z");
            // 
            // UvMappingButton
            // 
            UvMappingButton.Location = new System.Drawing.Point(299, 264);
            UvMappingButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            UvMappingButton.Name = "UvMappingButton";
            UvMappingButton.Size = new System.Drawing.Size(173, 27);
            UvMappingButton.TabIndex = 57;
            UvMappingButton.Text = "UV Mapping...";
            UvMappingButton.UseVisualStyleBackColor = true;
            UvMappingButton.Click += UvMappingButtonClicked;
            // 
            // TextureApplicationForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(486, 632);
            Controls.Add(UvVectorsLabel);
            Controls.Add(UvMappingButton);
            Controls.Add(UvUX);
            Controls.Add(UvUY);
            Controls.Add(UvUZ);
            Controls.Add(UvVX);
            Controls.Add(UvVY);
            Controls.Add(UvVZ);
            Controls.Add(RotateLabel);
            Controls.Add(TreatAsOneCheckbox);
            Controls.Add(RotMinus180Button);
            Controls.Add(RotMinus90Button);
            Controls.Add(RotMinus45Button);
            Controls.Add(RotPlus180Button);
            Controls.Add(RotPlus90Button);
            Controls.Add(RotPlus45Button);
            Controls.Add(lightmapGrp);
            Controls.Add(apply_null);
            Controls.Add(ResetButton);
            Controls.Add(RightClickActionButton);
            Controls.Add(LeftClickActionButton);
            Controls.Add(MarkButton);
            Controls.Add(TextureViewerPanel);
            Controls.Add(HideMaskCheckbox);
            Controls.Add(AlignGroup);
            Controls.Add(JustifyGroup);
            Controls.Add(ApplyButton);
            Controls.Add(RotationValue);
            Controls.Add(ReplaceButton);
            Controls.Add(BrowseButton);
            Controls.Add(RotationLabel);
            Controls.Add(TextureDetailsLabel);
            Controls.Add(tableLayoutPanel1);
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
            lightmapGrp.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion
        private System.Windows.Forms.CheckBox HideMaskCheckbox;
        private System.Windows.Forms.Label UvVectorsLabel;
        private System.Windows.Forms.TextBox UvUX;
        private System.Windows.Forms.TextBox UvUY;
        private System.Windows.Forms.TextBox UvUZ;
        private System.Windows.Forms.TextBox UvVX;
        private System.Windows.Forms.TextBox UvVY;
        private System.Windows.Forms.TextBox UvVZ;
        private System.Windows.Forms.Button UvMappingButton;
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
        private System.Windows.Forms.Button ScaleXNegateButton;
        private System.Windows.Forms.Button ScaleYNegateButton;
        private System.Windows.Forms.Label ShiftLabel;
        private Sledge.Shell.Controls.NumericUpDownEx ScaleYValue;
        private Sledge.Shell.Controls.NumericUpDownEx ShiftXValue;
        private Sledge.Shell.Controls.NumericUpDownEx ShiftYValue;
        private System.Windows.Forms.Label LightmapLabel;
        private Sledge.Shell.Controls.NumericUpDownEx LightmapValue;
        private System.Windows.Forms.ToolTip HoverTip;
        private System.Windows.Forms.CheckBox AlignToFaceCheckbox;
        private System.Windows.Forms.CheckBox AlignToWorldCheckbox;
        private System.Windows.Forms.Panel TextureViewerPanel;
        private System.Windows.Forms.Button MarkButton;
        private Shell.Controls.DropdownButton LeftClickActionButton;
        private Shell.Controls.DropdownButton RightClickActionButton;
        private System.Windows.Forms.ContextMenuStrip LeftClickActionMenu;
        private System.Windows.Forms.ContextMenuStrip RightClickActionMenu;
		private System.Windows.Forms.Button ResetButton;
		private System.Windows.Forms.Label RotateLabel;
		private System.Windows.Forms.Button RotPlus45Button;
		private System.Windows.Forms.Button RotPlus90Button;
		private System.Windows.Forms.Button RotPlus180Button;
		private System.Windows.Forms.Button RotMinus45Button;
		private System.Windows.Forms.Button RotMinus90Button;
		private System.Windows.Forms.Button RotMinus180Button;
		private System.Windows.Forms.Panel lightmapGrp;
		private System.Windows.Forms.Button apply_null;
	}
}