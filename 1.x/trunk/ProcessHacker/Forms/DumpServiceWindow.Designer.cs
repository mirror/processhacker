namespace ProcessHacker
{
    partial class DumpServiceWindow
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
            this.textServiceDll = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textDescription = new System.Windows.Forms.TextBox();
            this.textLoadOrderGroup = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textUserAccount = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textServiceBinaryPath = new System.Windows.Forms.TextBox();
            this.labelServiceDisplayName = new System.Windows.Forms.Label();
            this.labelServiceName = new System.Windows.Forms.Label();
            this.buttonClose = new System.Windows.Forms.Button();
            this.textServiceType = new System.Windows.Forms.TextBox();
            this.textStartType = new System.Windows.Forms.TextBox();
            this.textErrorControl = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // textServiceDll
            // 
            this.textServiceDll.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textServiceDll.Location = new System.Drawing.Point(93, 200);
            this.textServiceDll.Name = "textServiceDll";
            this.textServiceDll.ReadOnly = true;
            this.textServiceDll.Size = new System.Drawing.Size(303, 20);
            this.textServiceDll.TabIndex = 43;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 203);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(69, 13);
            this.label8.TabIndex = 42;
            this.label8.Text = "Service DLL:";
            // 
            // textDescription
            // 
            this.textDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textDescription.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textDescription.Location = new System.Drawing.Point(12, 51);
            this.textDescription.Multiline = true;
            this.textDescription.Name = "textDescription";
            this.textDescription.ReadOnly = true;
            this.textDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textDescription.Size = new System.Drawing.Size(384, 38);
            this.textDescription.TabIndex = 38;
            // 
            // textLoadOrderGroup
            // 
            this.textLoadOrderGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textLoadOrderGroup.Location = new System.Drawing.Point(246, 122);
            this.textLoadOrderGroup.Name = "textLoadOrderGroup";
            this.textLoadOrderGroup.ReadOnly = true;
            this.textLoadOrderGroup.Size = new System.Drawing.Size(150, 20);
            this.textLoadOrderGroup.TabIndex = 37;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(201, 98);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(59, 13);
            this.label6.TabIndex = 33;
            this.label6.Text = "Start Type:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 125);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(68, 13);
            this.label5.TabIndex = 32;
            this.label5.Text = "Error Control:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 98);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(34, 13);
            this.label4.TabIndex = 31;
            this.label4.Text = "Type:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(201, 125);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 13);
            this.label3.TabIndex = 30;
            this.label3.Text = "Group:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 151);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 13);
            this.label2.TabIndex = 29;
            this.label2.Text = "Binary Path:";
            // 
            // textUserAccount
            // 
            this.textUserAccount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textUserAccount.Location = new System.Drawing.Point(93, 174);
            this.textUserAccount.Name = "textUserAccount";
            this.textUserAccount.ReadOnly = true;
            this.textUserAccount.Size = new System.Drawing.Size(303, 20);
            this.textUserAccount.TabIndex = 28;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 177);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 27;
            this.label1.Text = "User Account:";
            // 
            // textServiceBinaryPath
            // 
            this.textServiceBinaryPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textServiceBinaryPath.Location = new System.Drawing.Point(93, 148);
            this.textServiceBinaryPath.Name = "textServiceBinaryPath";
            this.textServiceBinaryPath.ReadOnly = true;
            this.textServiceBinaryPath.Size = new System.Drawing.Size(303, 20);
            this.textServiceBinaryPath.TabIndex = 26;
            // 
            // labelServiceDisplayName
            // 
            this.labelServiceDisplayName.AutoSize = true;
            this.labelServiceDisplayName.Location = new System.Drawing.Point(12, 30);
            this.labelServiceDisplayName.Name = "labelServiceDisplayName";
            this.labelServiceDisplayName.Size = new System.Drawing.Size(111, 13);
            this.labelServiceDisplayName.TabIndex = 25;
            this.labelServiceDisplayName.Text = "Service Display Name";
            // 
            // labelServiceName
            // 
            this.labelServiceName.AutoSize = true;
            this.labelServiceName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelServiceName.Location = new System.Drawing.Point(12, 9);
            this.labelServiceName.Name = "labelServiceName";
            this.labelServiceName.Size = new System.Drawing.Size(86, 13);
            this.labelServiceName.TabIndex = 24;
            this.labelServiceName.Text = "Service Name";
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonClose.Location = new System.Drawing.Point(321, 232);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 44;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // textServiceType
            // 
            this.textServiceType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textServiceType.Location = new System.Drawing.Point(52, 95);
            this.textServiceType.Name = "textServiceType";
            this.textServiceType.ReadOnly = true;
            this.textServiceType.Size = new System.Drawing.Size(143, 20);
            this.textServiceType.TabIndex = 37;
            // 
            // textStartType
            // 
            this.textStartType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textStartType.Location = new System.Drawing.Point(266, 95);
            this.textStartType.Name = "textStartType";
            this.textStartType.ReadOnly = true;
            this.textStartType.Size = new System.Drawing.Size(130, 20);
            this.textStartType.TabIndex = 37;
            // 
            // textErrorControl
            // 
            this.textErrorControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textErrorControl.Location = new System.Drawing.Point(86, 122);
            this.textErrorControl.Name = "textErrorControl";
            this.textErrorControl.ReadOnly = true;
            this.textErrorControl.Size = new System.Drawing.Size(109, 20);
            this.textErrorControl.TabIndex = 37;
            // 
            // DumpServiceWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(408, 267);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.textServiceDll);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.textDescription);
            this.Controls.Add(this.textErrorControl);
            this.Controls.Add(this.textStartType);
            this.Controls.Add(this.textServiceType);
            this.Controls.Add(this.textLoadOrderGroup);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textUserAccount);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textServiceBinaryPath);
            this.Controls.Add(this.labelServiceDisplayName);
            this.Controls.Add(this.labelServiceName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DumpServiceWindow";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Service";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DumpServiceWindow_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textServiceDll;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textDescription;
        private System.Windows.Forms.TextBox textLoadOrderGroup;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textUserAccount;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textServiceBinaryPath;
        private System.Windows.Forms.Label labelServiceDisplayName;
        private System.Windows.Forms.Label labelServiceName;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.TextBox textServiceType;
        private System.Windows.Forms.TextBox textStartType;
        private System.Windows.Forms.TextBox textErrorControl;
    }
}