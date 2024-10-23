using Sledge.BspEditor.Environment.Controls;

namespace Sledge.BspEditor.Environment.Goldsource
{
    partial class GoldsourceEnvironmentEditor
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
			lblBaseGame = new System.Windows.Forms.Label();
			cmbBaseGame = new System.Windows.Forms.ComboBox();
			txtGameDir = new System.Windows.Forms.TextBox();
			lblGameDir = new System.Windows.Forms.Label();
			btnGameDirBrowse = new System.Windows.Forms.Button();
			lblGameExe = new System.Windows.Forms.Label();
			lblGameMod = new System.Windows.Forms.Label();
			cmbGameExe = new System.Windows.Forms.ComboBox();
			cmbGameMod = new System.Windows.Forms.ComboBox();
			lstFgds = new System.Windows.Forms.ListView();
			colFgdName = new System.Windows.Forms.ColumnHeader();
			colFgdPath = new System.Windows.Forms.ColumnHeader();
			btnAddFgd = new System.Windows.Forms.Button();
			lblDefaultBrushEntity = new System.Windows.Forms.Label();
			btnRemoveFgd = new System.Windows.Forms.Button();
			lblDefaultPointEntity = new System.Windows.Forms.Label();
			cmbDefaultBrushEntity = new System.Windows.Forms.ComboBox();
			cmbDefaultPointEntity = new System.Windows.Forms.ComboBox();
			nudDefaultTextureScale = new System.Windows.Forms.NumericUpDown();
			lblDefaultTextureScale = new System.Windows.Forms.Label();
			chkIncludeFgdDirectories = new System.Windows.Forms.CheckBox();
			cmbMapSizeOverrideHigh = new System.Windows.Forms.ComboBox();
			lblMapSizeOverrideHigh = new System.Windows.Forms.Label();
			cmbMapSizeOverrideLow = new System.Windows.Forms.ComboBox();
			chkOverrideMapSize = new System.Windows.Forms.CheckBox();
			lblMapSizeOverrideLow = new System.Windows.Forms.Label();
			chkLoadHdModels = new System.Windows.Forms.CheckBox();
			grpDirectories = new System.Windows.Forms.GroupBox();
			grpFgds = new System.Windows.Forms.GroupBox();
			gridUpDown = new GridUpDown();
			DefaultGridSizeLabel = new System.Windows.Forms.Label();
			grpBuildTools = new System.Windows.Forms.GroupBox();
			lblCopyToMapFolder = new System.Windows.Forms.Label();
			chkCopyBsp = new System.Windows.Forms.CheckBox();
			chkCopyRes = new System.Windows.Forms.CheckBox();
			chkCopyErr = new System.Windows.Forms.CheckBox();
			chkCopyLog = new System.Windows.Forms.CheckBox();
			chkCopyMap = new System.Windows.Forms.CheckBox();
			chkMapCopyBsp = new System.Windows.Forms.CheckBox();
			chkAskRunGame = new System.Windows.Forms.CheckBox();
			chkRunGame = new System.Windows.Forms.CheckBox();
			chkIncludeToolsDirectory = new System.Windows.Forms.CheckBox();
			lblBuildExeFolder = new System.Windows.Forms.Label();
			lblBuildBSP = new System.Windows.Forms.Label();
			txtBuildToolsDirectory = new System.Windows.Forms.TextBox();
			lblBuildCSG = new System.Windows.Forms.Label();
			cmbRadExe = new System.Windows.Forms.ComboBox();
			cmbBspExe = new System.Windows.Forms.ComboBox();
			lblBuildVIS = new System.Windows.Forms.Label();
			cmbVisExe = new System.Windows.Forms.ComboBox();
			cmbCsgExe = new System.Windows.Forms.ComboBox();
			lblBuildRAD = new System.Windows.Forms.Label();
			btnBuildToolsBrowse = new System.Windows.Forms.Button();
			grpTextures = new System.Windows.Forms.GroupBox();
			FilterBox = new System.Windows.Forms.TextBox();
			lstAdditionalTextures = new System.Windows.Forms.ListView();
			colWadName = new System.Windows.Forms.ColumnHeader();
			colWadPath = new System.Windows.Forms.ColumnHeader();
			btnRemoveTextures = new System.Windows.Forms.Button();
			btnAddTextures = new System.Windows.Forms.Button();
			lblAdditionalTexturePackages = new System.Windows.Forms.Label();
			lblTexturePackageExclusions = new System.Windows.Forms.Label();
			cklTexturePackages = new System.Windows.Forms.CheckedListBox();
			chkToggleAllTextures = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)nudDefaultTextureScale).BeginInit();
			grpDirectories.SuspendLayout();
			grpFgds.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)gridUpDown).BeginInit();
			grpBuildTools.SuspendLayout();
			grpTextures.SuspendLayout();
			SuspendLayout();
			// 
			// lblBaseGame
			// 
			lblBaseGame.Location = new System.Drawing.Point(2, 47);
			lblBaseGame.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			lblBaseGame.Name = "lblBaseGame";
			lblBaseGame.Size = new System.Drawing.Size(231, 23);
			lblBaseGame.TabIndex = 20;
			lblBaseGame.Text = "Base Game Directory (e.g. 'valve')";
			lblBaseGame.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// cmbBaseGame
			// 
			cmbBaseGame.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			cmbBaseGame.FormattingEnabled = true;
			cmbBaseGame.Items.AddRange(new object[] { "(Steam only) Half-Life", "Counter-Strike" });
			cmbBaseGame.Location = new System.Drawing.Point(245, 48);
			cmbBaseGame.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			cmbBaseGame.Name = "cmbBaseGame";
			cmbBaseGame.Size = new System.Drawing.Size(178, 23);
			cmbBaseGame.TabIndex = 21;
			cmbBaseGame.SelectedIndexChanged += BaseGameDirectoryChanged;
			// 
			// txtGameDir
			// 
			txtGameDir.Location = new System.Drawing.Point(125, 18);
			txtGameDir.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			txtGameDir.Name = "txtGameDir";
			txtGameDir.Size = new System.Drawing.Size(298, 23);
			txtGameDir.TabIndex = 13;
			txtGameDir.Text = "example: C:\\Sierra\\Half-Life";
			txtGameDir.TextChanged += GameDirectoryTextChanged;
			// 
			// lblGameDir
			// 
			lblGameDir.Location = new System.Drawing.Point(7, 18);
			lblGameDir.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			lblGameDir.Name = "lblGameDir";
			lblGameDir.Size = new System.Drawing.Size(111, 23);
			lblGameDir.TabIndex = 14;
			lblGameDir.Text = "Game Dir";
			lblGameDir.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// btnGameDirBrowse
			// 
			btnGameDirBrowse.Location = new System.Drawing.Point(430, 18);
			btnGameDirBrowse.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			btnGameDirBrowse.Name = "btnGameDirBrowse";
			btnGameDirBrowse.Size = new System.Drawing.Size(86, 23);
			btnGameDirBrowse.TabIndex = 15;
			btnGameDirBrowse.Text = "Browse...";
			btnGameDirBrowse.UseVisualStyleBackColor = true;
			btnGameDirBrowse.Click += BrowseGameDirectory;
			// 
			// lblGameExe
			// 
			lblGameExe.Location = new System.Drawing.Point(4, 110);
			lblGameExe.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			lblGameExe.Name = "lblGameExe";
			lblGameExe.Size = new System.Drawing.Size(231, 23);
			lblGameExe.TabIndex = 16;
			lblGameExe.Text = "Game Executable (e.g. 'hl.exe')";
			lblGameExe.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblGameMod
			// 
			lblGameMod.Location = new System.Drawing.Point(2, 78);
			lblGameMod.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			lblGameMod.Name = "lblGameMod";
			lblGameMod.Size = new System.Drawing.Size(231, 23);
			lblGameMod.TabIndex = 17;
			lblGameMod.Text = "Mod Directory (e.g. 'cstrike')";
			lblGameMod.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// cmbGameExe
			// 
			cmbGameExe.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			cmbGameExe.FormattingEnabled = true;
			cmbGameExe.Items.AddRange(new object[] { "Valve" });
			cmbGameExe.Location = new System.Drawing.Point(245, 111);
			cmbGameExe.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			cmbGameExe.Name = "cmbGameExe";
			cmbGameExe.Size = new System.Drawing.Size(178, 23);
			cmbGameExe.TabIndex = 18;
			// 
			// cmbGameMod
			// 
			cmbGameMod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			cmbGameMod.FormattingEnabled = true;
			cmbGameMod.Items.AddRange(new object[] { "Valve" });
			cmbGameMod.Location = new System.Drawing.Point(245, 80);
			cmbGameMod.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			cmbGameMod.Name = "cmbGameMod";
			cmbGameMod.Size = new System.Drawing.Size(178, 23);
			cmbGameMod.TabIndex = 19;
			cmbGameMod.SelectedIndexChanged += ModDirectoryChanged;
			// 
			// lstFgds
			// 
			lstFgds.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { colFgdName, colFgdPath });
			lstFgds.FullRowSelect = true;
			lstFgds.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			lstFgds.Location = new System.Drawing.Point(7, 22);
			lstFgds.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			lstFgds.Name = "lstFgds";
			lstFgds.ShowItemToolTips = true;
			lstFgds.Size = new System.Drawing.Size(416, 132);
			lstFgds.TabIndex = 34;
			lstFgds.UseCompatibleStateImageBehavior = false;
			lstFgds.View = System.Windows.Forms.View.Details;
			// 
			// colFgdName
			// 
			colFgdName.Text = "Name";
			// 
			// colFgdPath
			// 
			colFgdPath.Text = "Path";
			// 
			// btnAddFgd
			// 
			btnAddFgd.Location = new System.Drawing.Point(430, 22);
			btnAddFgd.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			btnAddFgd.Name = "btnAddFgd";
			btnAddFgd.Size = new System.Drawing.Size(86, 27);
			btnAddFgd.TabIndex = 27;
			btnAddFgd.Text = "Add...";
			btnAddFgd.UseVisualStyleBackColor = true;
			btnAddFgd.Click += BrowseFgd;
			// 
			// lblDefaultBrushEntity
			// 
			lblDefaultBrushEntity.Location = new System.Drawing.Point(8, 194);
			lblDefaultBrushEntity.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			lblDefaultBrushEntity.Name = "lblDefaultBrushEntity";
			lblDefaultBrushEntity.Size = new System.Drawing.Size(176, 23);
			lblDefaultBrushEntity.TabIndex = 30;
			lblDefaultBrushEntity.Text = "Default Brush Entity";
			lblDefaultBrushEntity.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// btnRemoveFgd
			// 
			btnRemoveFgd.Location = new System.Drawing.Point(430, 55);
			btnRemoveFgd.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			btnRemoveFgd.Name = "btnRemoveFgd";
			btnRemoveFgd.Size = new System.Drawing.Size(86, 27);
			btnRemoveFgd.TabIndex = 28;
			btnRemoveFgd.Text = "Remove";
			btnRemoveFgd.UseVisualStyleBackColor = true;
			btnRemoveFgd.Click += RemoveFgd;
			// 
			// lblDefaultPointEntity
			// 
			lblDefaultPointEntity.Location = new System.Drawing.Point(8, 163);
			lblDefaultPointEntity.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			lblDefaultPointEntity.Name = "lblDefaultPointEntity";
			lblDefaultPointEntity.Size = new System.Drawing.Size(176, 23);
			lblDefaultPointEntity.TabIndex = 31;
			lblDefaultPointEntity.Text = "Default Point Entity";
			lblDefaultPointEntity.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// cmbDefaultBrushEntity
			// 
			cmbDefaultBrushEntity.DropDownHeight = 300;
			cmbDefaultBrushEntity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			cmbDefaultBrushEntity.FormattingEnabled = true;
			cmbDefaultBrushEntity.IntegralHeight = false;
			cmbDefaultBrushEntity.Items.AddRange(new object[] { "Valve" });
			cmbDefaultBrushEntity.Location = new System.Drawing.Point(191, 193);
			cmbDefaultBrushEntity.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			cmbDefaultBrushEntity.Name = "cmbDefaultBrushEntity";
			cmbDefaultBrushEntity.Size = new System.Drawing.Size(231, 23);
			cmbDefaultBrushEntity.TabIndex = 32;
			// 
			// cmbDefaultPointEntity
			// 
			cmbDefaultPointEntity.DropDownHeight = 300;
			cmbDefaultPointEntity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			cmbDefaultPointEntity.FormattingEnabled = true;
			cmbDefaultPointEntity.IntegralHeight = false;
			cmbDefaultPointEntity.Items.AddRange(new object[] { "Valve" });
			cmbDefaultPointEntity.Location = new System.Drawing.Point(191, 162);
			cmbDefaultPointEntity.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			cmbDefaultPointEntity.Name = "cmbDefaultPointEntity";
			cmbDefaultPointEntity.Size = new System.Drawing.Size(231, 23);
			cmbDefaultPointEntity.TabIndex = 33;
			// 
			// nudDefaultTextureScale
			// 
			nudDefaultTextureScale.DecimalPlaces = 2;
			nudDefaultTextureScale.Increment = new decimal(new int[] { 5, 0, 0, 131072 });
			nudDefaultTextureScale.Location = new System.Drawing.Point(231, 18);
			nudDefaultTextureScale.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			nudDefaultTextureScale.Name = "nudDefaultTextureScale";
			nudDefaultTextureScale.Size = new System.Drawing.Size(59, 23);
			nudDefaultTextureScale.TabIndex = 38;
			nudDefaultTextureScale.Value = new decimal(new int[] { 25, 0, 0, 131072 });
			// 
			// lblDefaultTextureScale
			// 
			lblDefaultTextureScale.Location = new System.Drawing.Point(7, 18);
			lblDefaultTextureScale.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			lblDefaultTextureScale.Name = "lblDefaultTextureScale";
			lblDefaultTextureScale.Size = new System.Drawing.Size(217, 23);
			lblDefaultTextureScale.TabIndex = 36;
			lblDefaultTextureScale.Text = "Default Texture Scale";
			lblDefaultTextureScale.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// chkIncludeFgdDirectories
			// 
			chkIncludeFgdDirectories.Checked = true;
			chkIncludeFgdDirectories.CheckState = System.Windows.Forms.CheckState.Checked;
			chkIncludeFgdDirectories.Location = new System.Drawing.Point(7, 224);
			chkIncludeFgdDirectories.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			chkIncludeFgdDirectories.Name = "chkIncludeFgdDirectories";
			chkIncludeFgdDirectories.Size = new System.Drawing.Size(416, 28);
			chkIncludeFgdDirectories.TabIndex = 39;
			chkIncludeFgdDirectories.Text = "Load sprites and models from FGD directories";
			chkIncludeFgdDirectories.UseVisualStyleBackColor = true;
			// 
			// cmbMapSizeOverrideHigh
			// 
			cmbMapSizeOverrideHigh.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			cmbMapSizeOverrideHigh.FormattingEnabled = true;
			cmbMapSizeOverrideHigh.Items.AddRange(new object[] { "4096", "8192", "16384", "32768", "65536", "131072" });
			cmbMapSizeOverrideHigh.Location = new System.Drawing.Point(88, 317);
			cmbMapSizeOverrideHigh.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			cmbMapSizeOverrideHigh.Name = "cmbMapSizeOverrideHigh";
			cmbMapSizeOverrideHigh.Size = new System.Drawing.Size(66, 23);
			cmbMapSizeOverrideHigh.TabIndex = 44;
			// 
			// lblMapSizeOverrideHigh
			// 
			lblMapSizeOverrideHigh.Location = new System.Drawing.Point(6, 316);
			lblMapSizeOverrideHigh.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			lblMapSizeOverrideHigh.Name = "lblMapSizeOverrideHigh";
			lblMapSizeOverrideHigh.Size = new System.Drawing.Size(75, 23);
			lblMapSizeOverrideHigh.TabIndex = 43;
			lblMapSizeOverrideHigh.Text = "High";
			lblMapSizeOverrideHigh.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// cmbMapSizeOverrideLow
			// 
			cmbMapSizeOverrideLow.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			cmbMapSizeOverrideLow.FormattingEnabled = true;
			cmbMapSizeOverrideLow.Items.AddRange(new object[] { "-4096", "-8192", "-16384", "-32768", "-65536", "-131072" });
			cmbMapSizeOverrideLow.Location = new System.Drawing.Point(88, 286);
			cmbMapSizeOverrideLow.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			cmbMapSizeOverrideLow.Name = "cmbMapSizeOverrideLow";
			cmbMapSizeOverrideLow.Size = new System.Drawing.Size(66, 23);
			cmbMapSizeOverrideLow.TabIndex = 42;
			// 
			// chkOverrideMapSize
			// 
			chkOverrideMapSize.Checked = true;
			chkOverrideMapSize.CheckState = System.Windows.Forms.CheckState.Checked;
			chkOverrideMapSize.Location = new System.Drawing.Point(7, 255);
			chkOverrideMapSize.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			chkOverrideMapSize.Name = "chkOverrideMapSize";
			chkOverrideMapSize.Size = new System.Drawing.Size(229, 28);
			chkOverrideMapSize.TabIndex = 41;
			chkOverrideMapSize.Text = "Override FGD map size";
			chkOverrideMapSize.UseVisualStyleBackColor = true;
			// 
			// lblMapSizeOverrideLow
			// 
			lblMapSizeOverrideLow.Location = new System.Drawing.Point(7, 287);
			lblMapSizeOverrideLow.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			lblMapSizeOverrideLow.Name = "lblMapSizeOverrideLow";
			lblMapSizeOverrideLow.Size = new System.Drawing.Size(74, 23);
			lblMapSizeOverrideLow.TabIndex = 40;
			lblMapSizeOverrideLow.Text = "Low";
			lblMapSizeOverrideLow.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// chkLoadHdModels
			// 
			chkLoadHdModels.Checked = true;
			chkLoadHdModels.CheckState = System.Windows.Forms.CheckState.Checked;
			chkLoadHdModels.Location = new System.Drawing.Point(245, 142);
			chkLoadHdModels.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			chkLoadHdModels.Name = "chkLoadHdModels";
			chkLoadHdModels.Size = new System.Drawing.Size(284, 28);
			chkLoadHdModels.TabIndex = 45;
			chkLoadHdModels.Text = "Load HD Models (if available)";
			chkLoadHdModels.UseVisualStyleBackColor = true;
			// 
			// grpDirectories
			// 
			grpDirectories.Controls.Add(lblGameDir);
			grpDirectories.Controls.Add(cmbGameMod);
			grpDirectories.Controls.Add(chkLoadHdModels);
			grpDirectories.Controls.Add(cmbGameExe);
			grpDirectories.Controls.Add(lblGameMod);
			grpDirectories.Controls.Add(lblGameExe);
			grpDirectories.Controls.Add(btnGameDirBrowse);
			grpDirectories.Controls.Add(txtGameDir);
			grpDirectories.Controls.Add(cmbBaseGame);
			grpDirectories.Controls.Add(lblBaseGame);
			grpDirectories.Location = new System.Drawing.Point(7, 3);
			grpDirectories.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			grpDirectories.Name = "grpDirectories";
			grpDirectories.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
			grpDirectories.Size = new System.Drawing.Size(536, 185);
			grpDirectories.TabIndex = 46;
			grpDirectories.TabStop = false;
			grpDirectories.Text = "Directories";
			// 
			// grpFgds
			// 
			grpFgds.Controls.Add(gridUpDown);
			grpFgds.Controls.Add(DefaultGridSizeLabel);
			grpFgds.Controls.Add(lstFgds);
			grpFgds.Controls.Add(cmbDefaultPointEntity);
			grpFgds.Controls.Add(cmbDefaultBrushEntity);
			grpFgds.Controls.Add(chkIncludeFgdDirectories);
			grpFgds.Controls.Add(cmbMapSizeOverrideHigh);
			grpFgds.Controls.Add(lblDefaultPointEntity);
			grpFgds.Controls.Add(lblMapSizeOverrideHigh);
			grpFgds.Controls.Add(btnRemoveFgd);
			grpFgds.Controls.Add(cmbMapSizeOverrideLow);
			grpFgds.Controls.Add(lblDefaultBrushEntity);
			grpFgds.Controls.Add(chkOverrideMapSize);
			grpFgds.Controls.Add(btnAddFgd);
			grpFgds.Controls.Add(lblMapSizeOverrideLow);
			grpFgds.Location = new System.Drawing.Point(7, 195);
			grpFgds.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			grpFgds.Name = "grpFgds";
			grpFgds.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
			grpFgds.Size = new System.Drawing.Size(536, 351);
			grpFgds.TabIndex = 47;
			grpFgds.TabStop = false;
			grpFgds.Text = "Game Data Files";
			// 
			// gridUpDown
			// 
			gridUpDown.DecimalPlaces = 2;
			gridUpDown.Location = new System.Drawing.Point(311, 259);
			gridUpDown.Maximum = new decimal(new int[] { 1024, 0, 0, 0 });
			gridUpDown.Name = "gridUpDown";
			gridUpDown.Size = new System.Drawing.Size(120, 23);
			gridUpDown.TabIndex = 47;
			// 
			// DefaultGridSizeLabel
			// 
			DefaultGridSizeLabel.AutoSize = true;
			DefaultGridSizeLabel.Location = new System.Drawing.Point(211, 261);
			DefaultGridSizeLabel.Name = "DefaultGridSizeLabel";
			DefaultGridSizeLabel.Size = new System.Drawing.Size(94, 15);
			DefaultGridSizeLabel.TabIndex = 46;
			DefaultGridSizeLabel.Text = "Default grid size:";
			// 
			// grpBuildTools
			// 
			grpBuildTools.Controls.Add(lblCopyToMapFolder);
			grpBuildTools.Controls.Add(chkCopyBsp);
			grpBuildTools.Controls.Add(chkCopyRes);
			grpBuildTools.Controls.Add(chkCopyErr);
			grpBuildTools.Controls.Add(chkCopyLog);
			grpBuildTools.Controls.Add(chkCopyMap);
			grpBuildTools.Controls.Add(chkMapCopyBsp);
			grpBuildTools.Controls.Add(chkAskRunGame);
			grpBuildTools.Controls.Add(chkRunGame);
			grpBuildTools.Controls.Add(chkIncludeToolsDirectory);
			grpBuildTools.Controls.Add(lblBuildExeFolder);
			grpBuildTools.Controls.Add(lblBuildBSP);
			grpBuildTools.Controls.Add(txtBuildToolsDirectory);
			grpBuildTools.Controls.Add(lblBuildCSG);
			grpBuildTools.Controls.Add(cmbRadExe);
			grpBuildTools.Controls.Add(cmbBspExe);
			grpBuildTools.Controls.Add(lblBuildVIS);
			grpBuildTools.Controls.Add(cmbVisExe);
			grpBuildTools.Controls.Add(cmbCsgExe);
			grpBuildTools.Controls.Add(lblBuildRAD);
			grpBuildTools.Controls.Add(btnBuildToolsBrowse);
			grpBuildTools.Location = new System.Drawing.Point(7, 553);
			grpBuildTools.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			grpBuildTools.Name = "grpBuildTools";
			grpBuildTools.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
			grpBuildTools.Size = new System.Drawing.Size(536, 354);
			grpBuildTools.TabIndex = 48;
			grpBuildTools.TabStop = false;
			grpBuildTools.Text = "Build Tools";
			// 
			// lblCopyToMapFolder
			// 
			lblCopyToMapFolder.Location = new System.Drawing.Point(7, 320);
			lblCopyToMapFolder.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			lblCopyToMapFolder.Name = "lblCopyToMapFolder";
			lblCopyToMapFolder.Size = new System.Drawing.Size(212, 23);
			lblCopyToMapFolder.TabIndex = 43;
			lblCopyToMapFolder.Text = "Copy to map folder:";
			lblCopyToMapFolder.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// chkCopyBsp
			// 
			chkCopyBsp.Checked = true;
			chkCopyBsp.CheckState = System.Windows.Forms.CheckState.Checked;
			chkCopyBsp.Location = new System.Drawing.Point(88, 233);
			chkCopyBsp.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			chkCopyBsp.Name = "chkCopyBsp";
			chkCopyBsp.Size = new System.Drawing.Size(429, 23);
			chkCopyBsp.TabIndex = 42;
			chkCopyBsp.Text = "Copy BSP/RES to game folder";
			chkCopyBsp.UseVisualStyleBackColor = true;
			// 
			// chkCopyRes
			// 
			chkCopyRes.Checked = true;
			chkCopyRes.CheckState = System.Windows.Forms.CheckState.Checked;
			chkCopyRes.Location = new System.Drawing.Point(442, 318);
			chkCopyRes.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			chkCopyRes.Name = "chkCopyRes";
			chkCopyRes.Size = new System.Drawing.Size(57, 28);
			chkCopyRes.TabIndex = 42;
			chkCopyRes.Text = "RES";
			chkCopyRes.UseVisualStyleBackColor = true;
			// 
			// chkCopyErr
			// 
			chkCopyErr.Checked = true;
			chkCopyErr.CheckState = System.Windows.Forms.CheckState.Checked;
			chkCopyErr.Location = new System.Drawing.Point(387, 318);
			chkCopyErr.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			chkCopyErr.Name = "chkCopyErr";
			chkCopyErr.Size = new System.Drawing.Size(57, 28);
			chkCopyErr.TabIndex = 42;
			chkCopyErr.Text = "ERR";
			chkCopyErr.UseVisualStyleBackColor = true;
			// 
			// chkCopyLog
			// 
			chkCopyLog.Checked = true;
			chkCopyLog.CheckState = System.Windows.Forms.CheckState.Checked;
			chkCopyLog.Location = new System.Drawing.Point(331, 318);
			chkCopyLog.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			chkCopyLog.Name = "chkCopyLog";
			chkCopyLog.Size = new System.Drawing.Size(57, 28);
			chkCopyLog.TabIndex = 42;
			chkCopyLog.Text = "LOG";
			chkCopyLog.UseVisualStyleBackColor = true;
			// 
			// chkCopyMap
			// 
			chkCopyMap.Checked = true;
			chkCopyMap.CheckState = System.Windows.Forms.CheckState.Checked;
			chkCopyMap.Location = new System.Drawing.Point(278, 318);
			chkCopyMap.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			chkCopyMap.Name = "chkCopyMap";
			chkCopyMap.Size = new System.Drawing.Size(57, 28);
			chkCopyMap.TabIndex = 42;
			chkCopyMap.Text = "MAP";
			chkCopyMap.UseVisualStyleBackColor = true;
			// 
			// chkMapCopyBsp
			// 
			chkMapCopyBsp.Checked = true;
			chkMapCopyBsp.CheckState = System.Windows.Forms.CheckState.Checked;
			chkMapCopyBsp.Location = new System.Drawing.Point(226, 318);
			chkMapCopyBsp.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			chkMapCopyBsp.Name = "chkMapCopyBsp";
			chkMapCopyBsp.Size = new System.Drawing.Size(57, 28);
			chkMapCopyBsp.TabIndex = 42;
			chkMapCopyBsp.Text = "BSP";
			chkMapCopyBsp.UseVisualStyleBackColor = true;
			// 
			// chkAskRunGame
			// 
			chkAskRunGame.Checked = true;
			chkAskRunGame.CheckState = System.Windows.Forms.CheckState.Checked;
			chkAskRunGame.Location = new System.Drawing.Point(88, 293);
			chkAskRunGame.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			chkAskRunGame.Name = "chkAskRunGame";
			chkAskRunGame.Size = new System.Drawing.Size(429, 23);
			chkAskRunGame.TabIndex = 42;
			chkAskRunGame.Text = "Ask before running the game";
			chkAskRunGame.UseVisualStyleBackColor = true;
			// 
			// chkRunGame
			// 
			chkRunGame.Checked = true;
			chkRunGame.CheckState = System.Windows.Forms.CheckState.Checked;
			chkRunGame.Location = new System.Drawing.Point(88, 263);
			chkRunGame.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			chkRunGame.Name = "chkRunGame";
			chkRunGame.Size = new System.Drawing.Size(429, 23);
			chkRunGame.TabIndex = 42;
			chkRunGame.Text = "Run the game after successful compile";
			chkRunGame.UseVisualStyleBackColor = true;
			// 
			// chkIncludeToolsDirectory
			// 
			chkIncludeToolsDirectory.Checked = true;
			chkIncludeToolsDirectory.CheckState = System.Windows.Forms.CheckState.Checked;
			chkIncludeToolsDirectory.Location = new System.Drawing.Point(10, 74);
			chkIncludeToolsDirectory.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			chkIncludeToolsDirectory.Name = "chkIncludeToolsDirectory";
			chkIncludeToolsDirectory.Size = new System.Drawing.Size(506, 23);
			chkIncludeToolsDirectory.TabIndex = 33;
			chkIncludeToolsDirectory.Text = "Automatically include textures found in this directory";
			chkIncludeToolsDirectory.UseVisualStyleBackColor = true;
			chkIncludeToolsDirectory.CheckedChanged += IncludeBuildToolsChanged;
			// 
			// lblBuildExeFolder
			// 
			lblBuildExeFolder.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			lblBuildExeFolder.Location = new System.Drawing.Point(7, 18);
			lblBuildExeFolder.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			lblBuildExeFolder.Name = "lblBuildExeFolder";
			lblBuildExeFolder.Size = new System.Drawing.Size(416, 23);
			lblBuildExeFolder.TabIndex = 27;
			lblBuildExeFolder.Text = "Folder containing build executables:";
			lblBuildExeFolder.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblBuildBSP
			// 
			lblBuildBSP.Location = new System.Drawing.Point(41, 105);
			lblBuildBSP.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			lblBuildBSP.Name = "lblBuildBSP";
			lblBuildBSP.Size = new System.Drawing.Size(40, 23);
			lblBuildBSP.TabIndex = 23;
			lblBuildBSP.Text = "BSP";
			lblBuildBSP.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// txtBuildToolsDirectory
			// 
			txtBuildToolsDirectory.Location = new System.Drawing.Point(10, 44);
			txtBuildToolsDirectory.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			txtBuildToolsDirectory.Name = "txtBuildToolsDirectory";
			txtBuildToolsDirectory.Size = new System.Drawing.Size(412, 23);
			txtBuildToolsDirectory.TabIndex = 22;
			txtBuildToolsDirectory.Text = "example: C:\\hammer_alt";
			txtBuildToolsDirectory.TextChanged += BuildToolsDirectoryTextChanged;
			// 
			// lblBuildCSG
			// 
			lblBuildCSG.Location = new System.Drawing.Point(41, 136);
			lblBuildCSG.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			lblBuildCSG.Name = "lblBuildCSG";
			lblBuildCSG.Size = new System.Drawing.Size(40, 23);
			lblBuildCSG.TabIndex = 24;
			lblBuildCSG.Text = "CSG";
			lblBuildCSG.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// cmbRadExe
			// 
			cmbRadExe.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			cmbRadExe.FormattingEnabled = true;
			cmbRadExe.Location = new System.Drawing.Point(88, 197);
			cmbRadExe.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			cmbRadExe.Name = "cmbRadExe";
			cmbRadExe.Size = new System.Drawing.Size(178, 23);
			cmbRadExe.TabIndex = 28;
			// 
			// cmbBspExe
			// 
			cmbBspExe.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			cmbBspExe.FormattingEnabled = true;
			cmbBspExe.Location = new System.Drawing.Point(88, 104);
			cmbBspExe.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			cmbBspExe.Name = "cmbBspExe";
			cmbBspExe.Size = new System.Drawing.Size(178, 23);
			cmbBspExe.TabIndex = 29;
			// 
			// lblBuildVIS
			// 
			lblBuildVIS.Location = new System.Drawing.Point(41, 167);
			lblBuildVIS.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			lblBuildVIS.Name = "lblBuildVIS";
			lblBuildVIS.Size = new System.Drawing.Size(40, 23);
			lblBuildVIS.TabIndex = 25;
			lblBuildVIS.Text = "VIS";
			lblBuildVIS.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// cmbVisExe
			// 
			cmbVisExe.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			cmbVisExe.FormattingEnabled = true;
			cmbVisExe.Location = new System.Drawing.Point(88, 166);
			cmbVisExe.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			cmbVisExe.Name = "cmbVisExe";
			cmbVisExe.Size = new System.Drawing.Size(178, 23);
			cmbVisExe.TabIndex = 30;
			// 
			// cmbCsgExe
			// 
			cmbCsgExe.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			cmbCsgExe.FormattingEnabled = true;
			cmbCsgExe.Location = new System.Drawing.Point(88, 135);
			cmbCsgExe.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			cmbCsgExe.Name = "cmbCsgExe";
			cmbCsgExe.Size = new System.Drawing.Size(178, 23);
			cmbCsgExe.TabIndex = 31;
			// 
			// lblBuildRAD
			// 
			lblBuildRAD.Location = new System.Drawing.Point(41, 198);
			lblBuildRAD.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			lblBuildRAD.Name = "lblBuildRAD";
			lblBuildRAD.Size = new System.Drawing.Size(40, 23);
			lblBuildRAD.TabIndex = 26;
			lblBuildRAD.Text = "RAD";
			lblBuildRAD.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// btnBuildToolsBrowse
			// 
			btnBuildToolsBrowse.Location = new System.Drawing.Point(430, 44);
			btnBuildToolsBrowse.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			btnBuildToolsBrowse.Name = "btnBuildToolsBrowse";
			btnBuildToolsBrowse.Size = new System.Drawing.Size(86, 23);
			btnBuildToolsBrowse.TabIndex = 32;
			btnBuildToolsBrowse.Text = "Browse...";
			btnBuildToolsBrowse.UseVisualStyleBackColor = true;
			btnBuildToolsBrowse.Click += BrowseBuildToolsDirectory;
			// 
			// grpTextures
			// 
			grpTextures.Controls.Add(FilterBox);
			grpTextures.Controls.Add(lstAdditionalTextures);
			grpTextures.Controls.Add(btnRemoveTextures);
			grpTextures.Controls.Add(btnAddTextures);
			grpTextures.Controls.Add(lblAdditionalTexturePackages);
			grpTextures.Controls.Add(lblTexturePackageExclusions);
			grpTextures.Controls.Add(cklTexturePackages);
			grpTextures.Controls.Add(chkToggleAllTextures);
			grpTextures.Controls.Add(lblDefaultTextureScale);
			grpTextures.Controls.Add(nudDefaultTextureScale);
			grpTextures.Location = new System.Drawing.Point(7, 914);
			grpTextures.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			grpTextures.Name = "grpTextures";
			grpTextures.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
			grpTextures.Size = new System.Drawing.Size(536, 470);
			grpTextures.TabIndex = 49;
			grpTextures.TabStop = false;
			grpTextures.Text = "Textures";
			// 
			// FilterBox
			// 
			FilterBox.Location = new System.Drawing.Point(191, 43);
			FilterBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			FilterBox.Name = "FilterBox";
			FilterBox.Size = new System.Drawing.Size(116, 23);
			FilterBox.TabIndex = 46;
			FilterBox.TextChanged += FilterBox_TextChanged;
			// 
			// lstAdditionalTextures
			// 
			lstAdditionalTextures.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { colWadName, colWadPath });
			lstAdditionalTextures.FullRowSelect = true;
			lstAdditionalTextures.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			lstAdditionalTextures.Location = new System.Drawing.Point(10, 293);
			lstAdditionalTextures.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			lstAdditionalTextures.Name = "lstAdditionalTextures";
			lstAdditionalTextures.ShowItemToolTips = true;
			lstAdditionalTextures.Size = new System.Drawing.Size(424, 169);
			lstAdditionalTextures.TabIndex = 45;
			lstAdditionalTextures.UseCompatibleStateImageBehavior = false;
			lstAdditionalTextures.View = System.Windows.Forms.View.Details;
			// 
			// colWadName
			// 
			colWadName.Text = "Name";
			// 
			// colWadPath
			// 
			colWadPath.Text = "Path";
			// 
			// btnRemoveTextures
			// 
			btnRemoveTextures.Location = new System.Drawing.Point(442, 327);
			btnRemoveTextures.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			btnRemoveTextures.Name = "btnRemoveTextures";
			btnRemoveTextures.Size = new System.Drawing.Size(86, 27);
			btnRemoveTextures.TabIndex = 44;
			btnRemoveTextures.Text = "Remove";
			btnRemoveTextures.UseVisualStyleBackColor = true;
			btnRemoveTextures.Click += RemoveWad;
			// 
			// btnAddTextures
			// 
			btnAddTextures.Location = new System.Drawing.Point(442, 293);
			btnAddTextures.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			btnAddTextures.Name = "btnAddTextures";
			btnAddTextures.Size = new System.Drawing.Size(86, 27);
			btnAddTextures.TabIndex = 43;
			btnAddTextures.Text = "Add...";
			btnAddTextures.UseVisualStyleBackColor = true;
			btnAddTextures.Click += BrowseWad;
			// 
			// lblAdditionalTexturePackages
			// 
			lblAdditionalTexturePackages.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			lblAdditionalTexturePackages.Location = new System.Drawing.Point(7, 267);
			lblAdditionalTexturePackages.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			lblAdditionalTexturePackages.Name = "lblAdditionalTexturePackages";
			lblAdditionalTexturePackages.Size = new System.Drawing.Size(416, 23);
			lblAdditionalTexturePackages.TabIndex = 34;
			lblAdditionalTexturePackages.Text = "Additional texture packages:";
			lblAdditionalTexturePackages.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblTexturePackageExclusions
			// 
			lblTexturePackageExclusions.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			lblTexturePackageExclusions.Location = new System.Drawing.Point(14, 42);
			lblTexturePackageExclusions.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			lblTexturePackageExclusions.Name = "lblTexturePackageExclusions";
			lblTexturePackageExclusions.Size = new System.Drawing.Size(366, 23);
			lblTexturePackageExclusions.TabIndex = 34;
			lblTexturePackageExclusions.Text = "Texture packages to include:";
			lblTexturePackageExclusions.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// cklTexturePackages
			// 
			cklTexturePackages.CheckOnClick = true;
			cklTexturePackages.FormattingEnabled = true;
			cklTexturePackages.Location = new System.Drawing.Point(10, 68);
			cklTexturePackages.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			cklTexturePackages.Name = "cklTexturePackages";
			cklTexturePackages.Size = new System.Drawing.Size(517, 184);
			cklTexturePackages.TabIndex = 39;
			cklTexturePackages.KeyPress += cklTexturePackages_KeyPress;
			// 
			// chkToggleAllTextures
			// 
			chkToggleAllTextures.Checked = true;
			chkToggleAllTextures.CheckState = System.Windows.Forms.CheckState.Checked;
			chkToggleAllTextures.Location = new System.Drawing.Point(387, 47);
			chkToggleAllTextures.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			chkToggleAllTextures.Name = "chkToggleAllTextures";
			chkToggleAllTextures.Size = new System.Drawing.Size(141, 21);
			chkToggleAllTextures.TabIndex = 42;
			chkToggleAllTextures.Text = "Toggle all";
			chkToggleAllTextures.UseVisualStyleBackColor = true;
			chkToggleAllTextures.CheckedChanged += ToggleAllTextures;
			// 
			// GoldsourceEnvironmentEditor
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			Controls.Add(grpTextures);
			Controls.Add(grpBuildTools);
			Controls.Add(grpFgds);
			Controls.Add(grpDirectories);
			Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			Name = "GoldsourceEnvironmentEditor";
			Size = new System.Drawing.Size(551, 1387);
			((System.ComponentModel.ISupportInitialize)nudDefaultTextureScale).EndInit();
			grpDirectories.ResumeLayout(false);
			grpDirectories.PerformLayout();
			grpFgds.ResumeLayout(false);
			grpFgds.PerformLayout();
			((System.ComponentModel.ISupportInitialize)gridUpDown).EndInit();
			grpBuildTools.ResumeLayout(false);
			grpBuildTools.PerformLayout();
			grpTextures.ResumeLayout(false);
			grpTextures.PerformLayout();
			ResumeLayout(false);
		}

		#endregion

		private System.Windows.Forms.Label lblBaseGame;
        private System.Windows.Forms.ComboBox cmbBaseGame;
        private System.Windows.Forms.TextBox txtGameDir;
        private System.Windows.Forms.Label lblGameDir;
        private System.Windows.Forms.Button btnGameDirBrowse;
        private System.Windows.Forms.Label lblGameExe;
        private System.Windows.Forms.Label lblGameMod;
        private System.Windows.Forms.ComboBox cmbGameExe;
        private System.Windows.Forms.ComboBox cmbGameMod;
        private System.Windows.Forms.ListView lstFgds;
        private System.Windows.Forms.ColumnHeader colFgdName;
        private System.Windows.Forms.ColumnHeader colFgdPath;
        private System.Windows.Forms.Button btnAddFgd;
        private System.Windows.Forms.Label lblDefaultBrushEntity;
        private System.Windows.Forms.Button btnRemoveFgd;
        private System.Windows.Forms.Label lblDefaultPointEntity;
        private System.Windows.Forms.ComboBox cmbDefaultBrushEntity;
        private System.Windows.Forms.ComboBox cmbDefaultPointEntity;
        private System.Windows.Forms.NumericUpDown nudDefaultTextureScale;
        private System.Windows.Forms.Label lblDefaultTextureScale;
        private System.Windows.Forms.CheckBox chkIncludeFgdDirectories;
        private System.Windows.Forms.ComboBox cmbMapSizeOverrideHigh;
        private System.Windows.Forms.Label lblMapSizeOverrideHigh;
        private System.Windows.Forms.ComboBox cmbMapSizeOverrideLow;
        private System.Windows.Forms.CheckBox chkOverrideMapSize;
        private System.Windows.Forms.Label lblMapSizeOverrideLow;
        private System.Windows.Forms.CheckBox chkLoadHdModels;
        private System.Windows.Forms.GroupBox grpDirectories;
        private System.Windows.Forms.GroupBox grpFgds;
        private System.Windows.Forms.GroupBox grpBuildTools;
        private System.Windows.Forms.CheckBox chkIncludeToolsDirectory;
        private System.Windows.Forms.Label lblBuildExeFolder;
        private System.Windows.Forms.Label lblBuildBSP;
        private System.Windows.Forms.TextBox txtBuildToolsDirectory;
        private System.Windows.Forms.Label lblBuildCSG;
        private System.Windows.Forms.ComboBox cmbRadExe;
        private System.Windows.Forms.ComboBox cmbBspExe;
        private System.Windows.Forms.Label lblBuildVIS;
        private System.Windows.Forms.ComboBox cmbVisExe;
        private System.Windows.Forms.ComboBox cmbCsgExe;
        private System.Windows.Forms.Label lblBuildRAD;
        private System.Windows.Forms.Button btnBuildToolsBrowse;
        private System.Windows.Forms.GroupBox grpTextures;
        private System.Windows.Forms.CheckedListBox cklTexturePackages;
        private System.Windows.Forms.Label lblTexturePackageExclusions;
        private System.Windows.Forms.Label lblCopyToMapFolder;
        private System.Windows.Forms.CheckBox chkCopyBsp;
        private System.Windows.Forms.CheckBox chkCopyRes;
        private System.Windows.Forms.CheckBox chkCopyErr;
        private System.Windows.Forms.CheckBox chkCopyLog;
        private System.Windows.Forms.CheckBox chkCopyMap;
        private System.Windows.Forms.CheckBox chkMapCopyBsp;
        private System.Windows.Forms.CheckBox chkAskRunGame;
        private System.Windows.Forms.CheckBox chkRunGame;
        private System.Windows.Forms.CheckBox chkToggleAllTextures;
        private System.Windows.Forms.ListView lstAdditionalTextures;
        private System.Windows.Forms.ColumnHeader colWadName;
        private System.Windows.Forms.ColumnHeader colWadPath;
        private System.Windows.Forms.Button btnRemoveTextures;
        private System.Windows.Forms.Button btnAddTextures;
        private System.Windows.Forms.Label lblAdditionalTexturePackages;
		private System.Windows.Forms.TextBox FilterBox;
		private System.Windows.Forms.Label DefaultGridSizeLabel;
		private GridUpDown gridUpDown;
	}
}
