namespace ProcessHacker
{
    partial class VirtualProtectWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VirtualProtectWindow));
            this.labelVirtualProtectInfo = new System.Windows.Forms.Label();
            this.buttonCloseVirtualProtect = new System.Windows.Forms.Button();
            this.buttonVirtualProtect = new System.Windows.Forms.Button();
            this.textNewProtection = new System.Windows.Forms.TextBox();
            this.labelNewValue = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelVirtualProtectInfo
            // 
            this.labelVirtualProtectInfo.Location = new System.Drawing.Point(12, 9);
            this.labelVirtualProtectInfo.Name = "labelVirtualProtectInfo";
            this.labelVirtualProtectInfo.Size = new System.Drawing.Size(399, 165);
            this.labelVirtualProtectInfo.TabIndex = 1;
            this.labelVirtualProtectInfo.Text = resources.GetString("labelVirtualProtectInfo.Text");
            // 
            // buttonCloseVirtualProtect
            // 
            this.buttonCloseVirtualProtect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCloseVirtualProtect.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonCloseVirtualProtect.Location = new System.Drawing.Point(336, 184);
            this.buttonCloseVirtualProtect.Name = "buttonCloseVirtualProtect";
            this.buttonCloseVirtualProtect.Size = new System.Drawing.Size(75, 23);
            this.buttonCloseVirtualProtect.TabIndex = 9;
            this.buttonCloseVirtualProtect.Text = "Close";
            this.buttonCloseVirtualProtect.UseVisualStyleBackColor = true;
            this.buttonCloseVirtualProtect.Click += new System.EventHandler(this.buttonCloseVirtualProtect_Click);
            // 
            // buttonVirtualProtect
            // 
            this.buttonVirtualProtect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonVirtualProtect.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonVirtualProtect.Location = new System.Drawing.Point(255, 184);
            this.buttonVirtualProtect.Name = "buttonVirtualProtect";
            this.buttonVirtualProtect.Size = new System.Drawing.Size(75, 23);
            this.buttonVirtualProtect.TabIndex = 8;
            this.buttonVirtualProtect.Text = "&Change";
            this.buttonVirtualProtect.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonVirtualProtect.UseVisualStyleBackColor = true;
            this.buttonVirtualProtect.Click += new System.EventHandler(this.buttonVirtualProtect_Click);
            // 
            // textNewProtection
            // 
            this.textNewProtection.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.textNewProtection.Location = new System.Drawing.Point(167, 186);
            this.textNewProtection.Name = "textNewProtection";
            this.textNewProtection.Size = new System.Drawing.Size(82, 20);
            this.textNewProtection.TabIndex = 7;
            this.textNewProtection.Leave += new System.EventHandler(this.textNewProtection_Leave);
            this.textNewProtection.Enter += new System.EventHandler(this.textNewProtection_Enter);
            // 
            // labelNewValue
            // 
            this.labelNewValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.labelNewValue.AutoSize = true;
            this.labelNewValue.Location = new System.Drawing.Point(100, 189);
            this.labelNewValue.Name = "labelNewValue";
            this.labelNewValue.Size = new System.Drawing.Size(61, 13);
            this.labelNewValue.TabIndex = 6;
            this.labelNewValue.Text = "New value:";
            // 
            // VirtualProtectWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(423, 219);
            this.Controls.Add(this.buttonCloseVirtualProtect);
            this.Controls.Add(this.buttonVirtualProtect);
            this.Controls.Add(this.textNewProtection);
            this.Controls.Add(this.labelNewValue);
            this.Controls.Add(this.labelVirtualProtectInfo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "VirtualProtectWindow";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Change Memory Protection";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelVirtualProtectInfo;
        private System.Windows.Forms.Button buttonCloseVirtualProtect;
        private System.Windows.Forms.Button buttonVirtualProtect;
        private System.Windows.Forms.TextBox textNewProtection;
        private System.Windows.Forms.Label labelNewValue;
    }
}