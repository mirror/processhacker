namespace ProcessHacker.Components
{
    partial class MutantProperties
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
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.labelCurrentCount = new System.Windows.Forms.Label();
            this.labelAbandoned = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Current Count:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Abandoned:";
            // 
            // labelCurrentCount
            // 
            this.labelCurrentCount.AutoSize = true;
            this.labelCurrentCount.Location = new System.Drawing.Point(102, 3);
            this.labelCurrentCount.Name = "labelCurrentCount";
            this.labelCurrentCount.Size = new System.Drawing.Size(13, 13);
            this.labelCurrentCount.TabIndex = 0;
            this.labelCurrentCount.Text = "0";
            // 
            // labelAbandoned
            // 
            this.labelAbandoned.AutoSize = true;
            this.labelAbandoned.Location = new System.Drawing.Point(102, 26);
            this.labelAbandoned.Name = "labelAbandoned";
            this.labelAbandoned.Size = new System.Drawing.Size(32, 13);
            this.labelAbandoned.TabIndex = 0;
            this.labelAbandoned.Text = "False";
            // 
            // MutantProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label3);
            this.Controls.Add(this.labelAbandoned);
            this.Controls.Add(this.labelCurrentCount);
            this.Controls.Add(this.label1);
            this.Name = "MutantProperties";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Size = new System.Drawing.Size(183, 50);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labelCurrentCount;
        private System.Windows.Forms.Label labelAbandoned;
    }
}
