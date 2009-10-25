namespace ProcessHacker.Components
{
    partial class ColorModifier
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
            this.panelColor = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // panelColor
            // 
            this.panelColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelColor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelColor.Location = new System.Drawing.Point(0, 0);
            this.panelColor.Name = "panelColor";
            this.panelColor.Size = new System.Drawing.Size(40, 20);
            this.panelColor.TabIndex = 0;
            this.panelColor.MouseLeave += new System.EventHandler(this.panelColor_MouseLeave);
            this.panelColor.Click += new System.EventHandler(this.panelColor_Click);
            this.panelColor.MouseEnter += new System.EventHandler(this.panelColor_MouseEnter);
            // 
            // ColorModifier
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelColor);
            this.Name = "ColorModifier";
            this.Size = new System.Drawing.Size(40, 20);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelColor;
    }
}
