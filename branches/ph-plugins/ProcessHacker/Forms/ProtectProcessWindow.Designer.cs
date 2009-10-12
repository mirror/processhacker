namespace ProcessHacker
{
    partial class ProtectProcessWindow
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
            this.checkProtect = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.listProcessAccess = new System.Windows.Forms.CheckedListBox();
            this.listThreadAccess = new System.Windows.Forms.CheckedListBox();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.checkDontAllowKernelMode = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // checkProtect
            // 
            this.checkProtect.AutoSize = true;
            this.checkProtect.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkProtect.Location = new System.Drawing.Point(12, 12);
            this.checkProtect.Name = "checkProtect";
            this.checkProtect.Size = new System.Drawing.Size(125, 18);
            this.checkProtect.TabIndex = 0;
            this.checkProtect.Text = "Protect this process";
            this.checkProtect.UseVisualStyleBackColor = true;
            this.checkProtect.CheckedChanged += new System.EventHandler(this.checkProtect_CheckedChanged);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 62);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(124, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Allowed process access:";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 175);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(117, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Allowed thread access:";
            // 
            // listProcessAccess
            // 
            this.listProcessAccess.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listProcessAccess.FormattingEnabled = true;
            this.listProcessAccess.Location = new System.Drawing.Point(12, 78);
            this.listProcessAccess.Name = "listProcessAccess";
            this.listProcessAccess.Size = new System.Drawing.Size(474, 94);
            this.listProcessAccess.TabIndex = 3;
            // 
            // listThreadAccess
            // 
            this.listThreadAccess.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listThreadAccess.FormattingEnabled = true;
            this.listThreadAccess.Location = new System.Drawing.Point(12, 191);
            this.listThreadAccess.Name = "listThreadAccess";
            this.listThreadAccess.Size = new System.Drawing.Size(474, 94);
            this.listThreadAccess.TabIndex = 5;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonCancel.Location = new System.Drawing.Point(411, 291);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 7;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonOK.Location = new System.Drawing.Point(330, 291);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 6;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // checkDontAllowKernelMode
            // 
            this.checkDontAllowKernelMode.AutoSize = true;
            this.checkDontAllowKernelMode.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkDontAllowKernelMode.Location = new System.Drawing.Point(12, 36);
            this.checkDontAllowKernelMode.Name = "checkDontAllowKernelMode";
            this.checkDontAllowKernelMode.Size = new System.Drawing.Size(270, 18);
            this.checkDontAllowKernelMode.TabIndex = 1;
            this.checkDontAllowKernelMode.Text = "Don\'t allow kernel-mode code to bypass protection";
            this.checkDontAllowKernelMode.UseVisualStyleBackColor = true;
            // 
            // ProtectProcessWindow
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(498, 326);
            this.Controls.Add(this.checkDontAllowKernelMode);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.listThreadAccess);
            this.Controls.Add(this.listProcessAccess);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.checkProtect);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProtectProcessWindow";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Protect Process";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkProtect;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckedListBox listProcessAccess;
        private System.Windows.Forms.CheckedListBox listThreadAccess;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.CheckBox checkDontAllowKernelMode;
    }
}