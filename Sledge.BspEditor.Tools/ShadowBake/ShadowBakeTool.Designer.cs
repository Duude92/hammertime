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
        label1 = new System.Windows.Forms.Label();
        bakeButton = new System.Windows.Forms.Button();
        SuspendLayout();
        // 
        // label1
        // 
        label1.Location = new System.Drawing.Point(19, 8);
        label1.Name = "label1";
        label1.Size = new System.Drawing.Size(151, 23);
        label1.TabIndex = 0;
        label1.Text = "label1";
        // 
        // button1
        // 
        bakeButton.Location = new System.Drawing.Point(19, 34);
        bakeButton.Name = "button1";
        bakeButton.Size = new System.Drawing.Size(75, 23);
        bakeButton.TabIndex = 1;
        bakeButton.Text = "button1";
        bakeButton.UseVisualStyleBackColor = true;
		bakeButton.Click += BakeButton_Click;
        // 
        // ShadowBakeTool
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(800, 450);
        Controls.Add(bakeButton);
        Controls.Add(label1);
        Text = "ShadowBakeTool";
        ResumeLayout(false);
    }

	private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Button bakeButton;

    #endregion
}