namespace Sledge.BspEditor.Tools.Vertex.Controls
{
    partial class VertexEditFaceControl
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
			BevelButton = new System.Windows.Forms.Button();
			WithSelectedFacesLabel = new System.Windows.Forms.Label();
			BevelValue = new System.Windows.Forms.NumericUpDown();
			UnitsLabel2 = new System.Windows.Forms.Label();
			BevelByLabel = new System.Windows.Forms.Label();
			PokeFaceButton = new System.Windows.Forms.Button();
			PokeByLabel = new System.Windows.Forms.Label();
			UnitsLabel1 = new System.Windows.Forms.Label();
			PokeFaceCount = new System.Windows.Forms.NumericUpDown();
			ExtrudeButton = new System.Windows.Forms.Button();
			ExtrudeValue = new System.Windows.Forms.NumericUpDown();
			label1 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)BevelValue).BeginInit();
			((System.ComponentModel.ISupportInitialize)PokeFaceCount).BeginInit();
			((System.ComponentModel.ISupportInitialize)ExtrudeValue).BeginInit();
			SuspendLayout();
			// 
			// BevelButton
			// 
			BevelButton.Location = new System.Drawing.Point(177, 54);
			BevelButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			BevelButton.Name = "BevelButton";
			BevelButton.Size = new System.Drawing.Size(58, 27);
			BevelButton.TabIndex = 9;
			BevelButton.Text = "Bevel";
			BevelButton.UseVisualStyleBackColor = true;
			BevelButton.Click += BevelButtonClicked;
			// 
			// WithSelectedFacesLabel
			// 
			WithSelectedFacesLabel.AutoSize = true;
			WithSelectedFacesLabel.Location = new System.Drawing.Point(2, 5);
			WithSelectedFacesLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			WithSelectedFacesLabel.Name = "WithSelectedFacesLabel";
			WithSelectedFacesLabel.Size = new System.Drawing.Size(111, 15);
			WithSelectedFacesLabel.TabIndex = 3;
			WithSelectedFacesLabel.Text = "With selected faces:";
			// 
			// BevelValue
			// 
			BevelValue.Location = new System.Drawing.Point(70, 58);
			BevelValue.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			BevelValue.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
			BevelValue.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
			BevelValue.Name = "BevelValue";
			BevelValue.Size = new System.Drawing.Size(68, 23);
			BevelValue.TabIndex = 11;
			BevelValue.Value = new decimal(new int[] { 16, 0, 0, 0 });
			// 
			// UnitsLabel2
			// 
			UnitsLabel2.AutoSize = true;
			UnitsLabel2.Location = new System.Drawing.Point(140, 60);
			UnitsLabel2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			UnitsLabel2.Name = "UnitsLabel2";
			UnitsLabel2.Size = new System.Drawing.Size(33, 15);
			UnitsLabel2.TabIndex = 6;
			UnitsLabel2.Text = "units";
			// 
			// BevelByLabel
			// 
			BevelByLabel.AutoSize = true;
			BevelByLabel.Location = new System.Drawing.Point(4, 60);
			BevelByLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			BevelByLabel.Name = "BevelByLabel";
			BevelByLabel.Size = new System.Drawing.Size(51, 15);
			BevelByLabel.TabIndex = 7;
			BevelByLabel.Text = "Bevel by";
			// 
			// PokeFaceButton
			// 
			PokeFaceButton.Location = new System.Drawing.Point(177, 24);
			PokeFaceButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			PokeFaceButton.Name = "PokeFaceButton";
			PokeFaceButton.Size = new System.Drawing.Size(58, 27);
			PokeFaceButton.TabIndex = 9;
			PokeFaceButton.Text = "Poke";
			PokeFaceButton.UseVisualStyleBackColor = true;
			PokeFaceButton.Click += PokeFaceButtonClicked;
			// 
			// PokeByLabel
			// 
			PokeByLabel.AutoSize = true;
			PokeByLabel.Location = new System.Drawing.Point(4, 30);
			PokeByLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			PokeByLabel.Name = "PokeByLabel";
			PokeByLabel.Size = new System.Drawing.Size(49, 15);
			PokeByLabel.TabIndex = 7;
			PokeByLabel.Text = "Poke by";
			// 
			// UnitsLabel1
			// 
			UnitsLabel1.AutoSize = true;
			UnitsLabel1.Location = new System.Drawing.Point(140, 30);
			UnitsLabel1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			UnitsLabel1.Name = "UnitsLabel1";
			UnitsLabel1.Size = new System.Drawing.Size(33, 15);
			UnitsLabel1.TabIndex = 6;
			UnitsLabel1.Text = "units";
			// 
			// PokeFaceCount
			// 
			PokeFaceCount.Location = new System.Drawing.Point(70, 28);
			PokeFaceCount.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			PokeFaceCount.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
			PokeFaceCount.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
			PokeFaceCount.Name = "PokeFaceCount";
			PokeFaceCount.Size = new System.Drawing.Size(68, 23);
			PokeFaceCount.TabIndex = 11;
			PokeFaceCount.Value = new decimal(new int[] { 16, 0, 0, 0 });
			// 
			// ExtrudeButton
			// 
			ExtrudeButton.Location = new System.Drawing.Point(177, 83);
			ExtrudeButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			ExtrudeButton.Name = "ExtrudeButton";
			ExtrudeButton.Size = new System.Drawing.Size(58, 27);
			ExtrudeButton.TabIndex = 14;
			ExtrudeButton.Text = "Extrude";
			ExtrudeButton.UseVisualStyleBackColor = true;
			ExtrudeButton.Click += ExtrudeButtonClicked;
			// 
			// ExtrudeValue
			// 
			ExtrudeValue.Location = new System.Drawing.Point(70, 87);
			ExtrudeValue.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			ExtrudeValue.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
			ExtrudeValue.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
			ExtrudeValue.Name = "ExtrudeValue";
			ExtrudeValue.Size = new System.Drawing.Size(68, 23);
			ExtrudeValue.TabIndex = 15;
			ExtrudeValue.Value = new decimal(new int[] { 16, 0, 0, 0 });
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(140, 89);
			label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(33, 15);
			label1.TabIndex = 12;
			label1.Text = "units";
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(4, 89);
			label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(63, 15);
			label2.TabIndex = 13;
			label2.Text = "Extrude by";
			// 
			// VertexEditFaceControl
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			Controls.Add(ExtrudeButton);
			Controls.Add(ExtrudeValue);
			Controls.Add(label1);
			Controls.Add(label2);
			Controls.Add(PokeFaceButton);
			Controls.Add(BevelButton);
			Controls.Add(WithSelectedFacesLabel);
			Controls.Add(PokeFaceCount);
			Controls.Add(BevelValue);
			Controls.Add(UnitsLabel1);
			Controls.Add(PokeByLabel);
			Controls.Add(UnitsLabel2);
			Controls.Add(BevelByLabel);
			Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			Name = "VertexEditFaceControl";
			Size = new System.Drawing.Size(246, 113);
			((System.ComponentModel.ISupportInitialize)BevelValue).EndInit();
			((System.ComponentModel.ISupportInitialize)PokeFaceCount).EndInit();
			((System.ComponentModel.ISupportInitialize)ExtrudeValue).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private System.Windows.Forms.Button BevelButton;
        private System.Windows.Forms.Label WithSelectedFacesLabel;
        private System.Windows.Forms.NumericUpDown BevelValue;
        private System.Windows.Forms.Label UnitsLabel2;
        private System.Windows.Forms.Label BevelByLabel;
        private System.Windows.Forms.Button PokeFaceButton;
        private System.Windows.Forms.Label PokeByLabel;
        private System.Windows.Forms.Label UnitsLabel1;
        private System.Windows.Forms.NumericUpDown PokeFaceCount;
		private System.Windows.Forms.Button ExtrudeButton;
		private System.Windows.Forms.NumericUpDown ExtrudeValue;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
	}
}
