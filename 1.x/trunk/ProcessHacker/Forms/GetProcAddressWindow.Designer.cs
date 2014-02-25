namespace ProcessHacker
{
    partial class GetProcAddressWindow
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textProcName = new System.Windows.Forms.TextBox();
            this.textProcAddress = new System.Windows.Forms.TextBox();
            this.buttonLookup = new System.Windows.Forms.Button();
            this.buttonClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(109, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Export Name/Ordinal:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Address:";
            // 
            // textProcName
            // 
            this.textProcName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textProcName.Location = new System.Drawing.Point(127, 12);
            this.textProcName.Name = "textProcName";
            this.textProcName.Size = new System.Drawing.Size(255, 20);
            this.textProcName.TabIndex = 0;
            this.textProcName.Leave += new System.EventHandler(this.textProcName_Leave);
            this.textProcName.Enter += new System.EventHandler(this.textProcName_Enter);
            // 
            // textProcAddress
            // 
            this.textProcAddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textProcAddress.Location = new System.Drawing.Point(127, 38);
            this.textProcAddress.Name = "textProcAddress";
            this.textProcAddress.ReadOnly = true;
            this.textProcAddress.Size = new System.Drawing.Size(255, 20);
            this.textProcAddress.TabIndex = 1;
            // 
            // buttonLookup
            // 
            this.buttonLookup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonLookup.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonLookup.Location = new System.Drawing.Point(226, 64);
            this.buttonLookup.Name = "buttonLookup";
            this.buttonLookup.Size = new System.Drawing.Size(75, 23);
            this.buttonLookup.TabIndex = 2;
            this.buttonLookup.Text = "&Lookup";
            this.buttonLookup.UseVisualStyleBackColor = true;
            this.buttonLookup.Click += new System.EventHandler(this.buttonLookup_Click);
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonClose.Location = new System.Drawing.Point(307, 64);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 3;
            this.buttonClose.Text = "&Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // GetProcAddressWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(394, 99);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.buttonLookup);
            this.Controls.Add(this.textProcAddress);
            this.Controls.Add(this.textProcName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GetProcAddressWindow";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Get Function Address";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textProcName;
        private System.Windows.Forms.TextBox textProcAddress;
        private System.Windows.Forms.Button buttonLookup;
        private System.Windows.Forms.Button buttonClose;
    }
}