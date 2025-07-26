using System.ComponentModel;

namespace Sledge.BspEditor.Tools.ShadowBake;

partial class ShadowBakeTool
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private IContainer components = null;

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
		bakeButton = new System.Windows.Forms.Button();
		progressBar1 = new System.Windows.Forms.ProgressBar();
		label1 = new System.Windows.Forms.Label();
		SuspendLayout();
		// 
		// bakeButton
		// 
		bakeButton.Location = new System.Drawing.Point(3, 44);
		bakeButton.Name = "bakeButton";
		bakeButton.Size = new System.Drawing.Size(75, 23);
		bakeButton.TabIndex = 1;
		bakeButton.Text = "Bake";
		bakeButton.UseVisualStyleBackColor = true;
		bakeButton.Click += BakeButton_Click;
		// 
		// progressBar1
		// 
		progressBar1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		progressBar1.Location = new System.Drawing.Point(3, 105);
		progressBar1.Name = "progressBar1";
		progressBar1.Size = new System.Drawing.Size(133, 23);
		progressBar1.TabIndex = 2;
		// 
		// label1
		// 
		label1.Location = new System.Drawing.Point(3, 18);
		label1.Name = "label1";
		label1.Size = new System.Drawing.Size(133, 23);
		label1.TabIndex = 0;
		label1.Text = "Shadows settings";
		// 
		// ShadowBakeTool
		// 
		AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
		AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		Controls.Add(progressBar1);
		Controls.Add(bakeButton);
		Controls.Add(label1);
		Name = "ShadowBakeTool";
		Size = new System.Drawing.Size(139, 131);
		ResumeLayout(false);
	}
	private System.Windows.Forms.Button bakeButton;

	#endregion

	private System.Windows.Forms.ProgressBar progressBar1;
	private System.Windows.Forms.Label label1;
}