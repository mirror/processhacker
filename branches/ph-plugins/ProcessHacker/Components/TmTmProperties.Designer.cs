namespace ProcessHacker.Components
{
    partial class TmTmProperties
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

            _tmHandle.Dereference(disposing);

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
            this.textGuid = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textLogFileName = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "GUID:";
            // 
            // textGuid
            // 
            this.textGuid.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textGuid.Location = new System.Drawing.Point(49, 6);
            this.textGuid.Name = "textGuid";
            this.textGuid.ReadOnly = true;
            this.textGuid.Size = new System.Drawing.Size(244, 20);
            this.textGuid.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Log File Name:";
            // 
            // textLogFileName
            // 
            this.textLogFileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textLogFileName.Location = new System.Drawing.Point(90, 32);
            this.textLogFileName.Name = "textLogFileName";
            this.textLogFileName.ReadOnly = true;
            this.textLogFileName.Size = new System.Drawing.Size(203, 20);
            this.textLogFileName.TabIndex = 1;
            // 
            // TmTmProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.textLogFileName);
            this.Controls.Add(this.textGuid);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "TmTmProperties";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Size = new System.Drawing.Size(299, 62);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textGuid;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textLogFileName;
    }
}
