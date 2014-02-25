namespace ProcessHacker.Components
{
    partial class SemaphoreProperties
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

            _semaphoreHandle.Dereference(disposing);

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
            this.label2 = new System.Windows.Forms.Label();
            this.labelCurrentCount = new System.Windows.Forms.Label();
            this.labelMaximumCount = new System.Windows.Forms.Label();
            this.buttonRelease = new System.Windows.Forms.Button();
            this.buttonAcquire = new System.Windows.Forms.Button();
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
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Maximum Count:";
            // 
            // labelCurrentCount
            // 
            this.labelCurrentCount.AutoSize = true;
            this.labelCurrentCount.Location = new System.Drawing.Point(97, 3);
            this.labelCurrentCount.Name = "labelCurrentCount";
            this.labelCurrentCount.Size = new System.Drawing.Size(13, 13);
            this.labelCurrentCount.TabIndex = 0;
            this.labelCurrentCount.Text = "0";
            // 
            // labelMaximumCount
            // 
            this.labelMaximumCount.AutoSize = true;
            this.labelMaximumCount.Location = new System.Drawing.Point(97, 25);
            this.labelMaximumCount.Name = "labelMaximumCount";
            this.labelMaximumCount.Size = new System.Drawing.Size(13, 13);
            this.labelMaximumCount.TabIndex = 0;
            this.labelMaximumCount.Text = "0";
            // 
            // buttonRelease
            // 
            this.buttonRelease.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonRelease.Location = new System.Drawing.Point(87, 51);
            this.buttonRelease.Name = "buttonRelease";
            this.buttonRelease.Size = new System.Drawing.Size(75, 23);
            this.buttonRelease.TabIndex = 1;
            this.buttonRelease.Text = "Release";
            this.buttonRelease.UseVisualStyleBackColor = true;
            this.buttonRelease.Click += new System.EventHandler(this.buttonRelease_Click);
            // 
            // buttonAcquire
            // 
            this.buttonAcquire.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonAcquire.Location = new System.Drawing.Point(6, 51);
            this.buttonAcquire.Name = "buttonAcquire";
            this.buttonAcquire.Size = new System.Drawing.Size(75, 23);
            this.buttonAcquire.TabIndex = 2;
            this.buttonAcquire.Text = "Acquire";
            this.buttonAcquire.UseVisualStyleBackColor = true;
            this.buttonAcquire.Click += new System.EventHandler(this.buttonAcquire_Click);
            // 
            // SemaphoreProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.buttonAcquire);
            this.Controls.Add(this.buttonRelease);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.labelMaximumCount);
            this.Controls.Add(this.labelCurrentCount);
            this.Controls.Add(this.label1);
            this.Name = "SemaphoreProperties";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Size = new System.Drawing.Size(196, 80);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelCurrentCount;
        private System.Windows.Forms.Label labelMaximumCount;
        private System.Windows.Forms.Button buttonRelease;
        private System.Windows.Forms.Button buttonAcquire;
    }
}
