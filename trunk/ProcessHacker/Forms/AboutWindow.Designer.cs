namespace ProcessHacker
{
    partial class AboutWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
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
            this.buttonClose = new System.Windows.Forms.Button();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.labelAppName = new System.Windows.Forms.Label();
            this.labelVersion = new System.Windows.Forms.Label();
            this.labelCredits = new System.Windows.Forms.Label();
            this.linkFamFamFam = new System.Windows.Forms.LinkLabel();
            this.linkVistaMenu = new System.Windows.Forms.LinkLabel();
            this.linkHexBox = new System.Windows.Forms.LinkLabel();
            this.labelBy = new System.Windows.Forms.Label();
            this.linkSourceforge = new System.Windows.Forms.LinkLabel();
            this.linkEmail = new System.Windows.Forms.LinkLabel();
            this.linkAsm = new System.Windows.Forms.LinkLabel();
            this.buttonLicenses = new System.Windows.Forms.Button();
            this.linkTreeViewAdv = new System.Windows.Forms.LinkLabel();
            this.flowThanks = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.flowThanks.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonClose.Location = new System.Drawing.Point(333, 250);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 0;
            this.buttonClose.Text = "&Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // pictureBox
            // 
            this.pictureBox.Image = global::ProcessHacker.Properties.Resources.ProcessHacker;
            this.pictureBox.Location = new System.Drawing.Point(12, 12);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(156, 150);
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox.TabIndex = 1;
            this.pictureBox.TabStop = false;
            // 
            // labelAppName
            // 
            this.labelAppName.AutoSize = true;
            this.labelAppName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelAppName.Location = new System.Drawing.Point(174, 12);
            this.labelAppName.Name = "labelAppName";
            this.labelAppName.Size = new System.Drawing.Size(119, 16);
            this.labelAppName.TabIndex = 2;
            this.labelAppName.Text = "Process Hacker";
            // 
            // labelVersion
            // 
            this.labelVersion.AutoSize = true;
            this.labelVersion.Location = new System.Drawing.Point(174, 33);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(42, 13);
            this.labelVersion.TabIndex = 3;
            this.labelVersion.Text = "Version";
            // 
            // labelCredits
            // 
            this.labelCredits.AutoSize = true;
            this.labelCredits.Location = new System.Drawing.Point(174, 85);
            this.labelCredits.Name = "labelCredits";
            this.labelCredits.Size = new System.Drawing.Size(58, 26);
            this.labelCredits.TabIndex = 4;
            this.labelCredits.Text = "Thanks to:\r\n\r\n";
            // 
            // linkFamFamFam
            // 
            this.linkFamFamFam.AutoSize = true;
            this.linkFamFamFam.Location = new System.Drawing.Point(6, 16);
            this.linkFamFamFam.Name = "linkFamFamFam";
            this.linkFamFamFam.Size = new System.Drawing.Size(136, 13);
            this.linkFamFamFam.TabIndex = 5;
            this.linkFamFamFam.TabStop = true;
            this.linkFamFamFam.Text = "famfamfam.com - Silk Icons";
            this.linkFamFamFam.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkFamFamFam_LinkClicked);
            // 
            // linkVistaMenu
            // 
            this.linkVistaMenu.AutoSize = true;
            this.linkVistaMenu.Location = new System.Drawing.Point(6, 29);
            this.linkVistaMenu.Name = "linkVistaMenu";
            this.linkVistaMenu.Size = new System.Drawing.Size(183, 13);
            this.linkVistaMenu.TabIndex = 6;
            this.linkVistaMenu.TabStop = true;
            this.linkVistaMenu.Text = "Wyatt O\'Day - VistaMenu, SplitButton";
            this.linkVistaMenu.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkVistaMenu_LinkClicked);
            // 
            // linkHexBox
            // 
            this.linkHexBox.AutoSize = true;
            this.linkHexBox.Location = new System.Drawing.Point(6, 3);
            this.linkHexBox.Name = "linkHexBox";
            this.linkHexBox.Size = new System.Drawing.Size(151, 13);
            this.linkHexBox.TabIndex = 7;
            this.linkHexBox.TabStop = true;
            this.linkHexBox.Text = "Bernhard Elbl - HexBox control";
            this.linkHexBox.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkHexBox_LinkClicked);
            // 
            // labelBy
            // 
            this.labelBy.Location = new System.Drawing.Point(174, 50);
            this.labelBy.Name = "labelBy";
            this.labelBy.Size = new System.Drawing.Size(234, 35);
            this.labelBy.TabIndex = 8;
            this.labelBy.Text = "by wj32 and Dean. Licensed under the GNU GPL, v3.";
            // 
            // linkSourceforge
            // 
            this.linkSourceforge.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.linkSourceforge.AutoSize = true;
            this.linkSourceforge.Location = new System.Drawing.Point(12, 255);
            this.linkSourceforge.Name = "linkSourceforge";
            this.linkSourceforge.Size = new System.Drawing.Size(229, 13);
            this.linkSourceforge.TabIndex = 9;
            this.linkSourceforge.TabStop = true;
            this.linkSourceforge.Text = "http://sourceforge.net/projects/processhacker";
            this.linkSourceforge.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkSourceforge_LinkClicked);
            // 
            // linkEmail
            // 
            this.linkEmail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.linkEmail.AutoSize = true;
            this.linkEmail.Location = new System.Drawing.Point(12, 237);
            this.linkEmail.Name = "linkEmail";
            this.linkEmail.Size = new System.Drawing.Size(190, 13);
            this.linkEmail.TabIndex = 10;
            this.linkEmail.TabStop = true;
            this.linkEmail.Text = "Send feedback to wj32.64@gmail.com";
            this.linkEmail.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkEmail_LinkClicked);
            // 
            // linkAsm
            // 
            this.linkAsm.AutoSize = true;
            this.linkAsm.Location = new System.Drawing.Point(6, 42);
            this.linkAsm.Name = "linkAsm";
            this.linkAsm.Size = new System.Drawing.Size(198, 13);
            this.linkAsm.TabIndex = 12;
            this.linkAsm.TabStop = true;
            this.linkAsm.Text = "Oleh Yuschuk - Disassembler/Assembler";
            this.linkAsm.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkAsm_LinkClicked);
            // 
            // buttonLicenses
            // 
            this.buttonLicenses.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonLicenses.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonLicenses.Location = new System.Drawing.Point(252, 250);
            this.buttonLicenses.Name = "buttonLicenses";
            this.buttonLicenses.Size = new System.Drawing.Size(75, 23);
            this.buttonLicenses.TabIndex = 13;
            this.buttonLicenses.Text = "Licenses...";
            this.buttonLicenses.UseVisualStyleBackColor = true;
            this.buttonLicenses.Click += new System.EventHandler(this.buttonLicenses_Click);
            // 
            // linkTreeViewAdv
            // 
            this.linkTreeViewAdv.AutoSize = true;
            this.linkTreeViewAdv.Location = new System.Drawing.Point(6, 55);
            this.linkTreeViewAdv.Name = "linkTreeViewAdv";
            this.linkTreeViewAdv.Size = new System.Drawing.Size(165, 13);
            this.linkTreeViewAdv.TabIndex = 12;
            this.linkTreeViewAdv.TabStop = true;
            this.linkTreeViewAdv.Text = "Andrey Gliznetsov - TreeViewAdv";
            this.linkTreeViewAdv.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkTreeViewAdv_LinkClicked);
            // 
            // flowThanks
            // 
            this.flowThanks.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.flowThanks.AutoScroll = true;
            this.flowThanks.Controls.Add(this.linkHexBox);
            this.flowThanks.Controls.Add(this.linkFamFamFam);
            this.flowThanks.Controls.Add(this.linkVistaMenu);
            this.flowThanks.Controls.Add(this.linkAsm);
            this.flowThanks.Controls.Add(this.linkTreeViewAdv);
            this.flowThanks.Controls.Add(this.label1);
            this.flowThanks.Controls.Add(this.label4);
            this.flowThanks.Controls.Add(this.label3);
            this.flowThanks.Controls.Add(this.label2);
            this.flowThanks.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowThanks.Location = new System.Drawing.Point(177, 103);
            this.flowThanks.Name = "flowThanks";
            this.flowThanks.Padding = new System.Windows.Forms.Padding(3);
            this.flowThanks.Size = new System.Drawing.Size(231, 113);
            this.flowThanks.TabIndex = 14;
            this.flowThanks.WrapContents = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 68);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(162, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "XhmikosR - Installer, suggestions";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 107);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(202, 26);
            this.label3.TabIndex = 15;
            this.label3.Text = "ReactOS - the ultimate guide to Windows internals";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 133);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(193, 26);
            this.label2.TabIndex = 14;
            this.label2.Text = "Greg Hoglund, James Butler - Rootkits: Subverting the Windows Kernel";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 81);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(199, 26);
            this.label4.TabIndex = 16;
            this.label4.Text = "NTinternals - a great guide to the Native API";
            // 
            // AboutWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(420, 285);
            this.Controls.Add(this.flowThanks);
            this.Controls.Add(this.buttonLicenses);
            this.Controls.Add(this.linkEmail);
            this.Controls.Add(this.linkSourceforge);
            this.Controls.Add(this.labelBy);
            this.Controls.Add(this.labelCredits);
            this.Controls.Add(this.labelVersion);
            this.Controls.Add(this.labelAppName);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.buttonClose);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutWindow";
            this.Padding = new System.Windows.Forms.Padding(9);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.flowThanks.ResumeLayout(false);
            this.flowThanks.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Label labelAppName;
        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.Label labelCredits;
        private System.Windows.Forms.LinkLabel linkFamFamFam;
        private System.Windows.Forms.LinkLabel linkVistaMenu;
        private System.Windows.Forms.LinkLabel linkHexBox;
        private System.Windows.Forms.Label labelBy;
        private System.Windows.Forms.LinkLabel linkSourceforge;
        private System.Windows.Forms.LinkLabel linkEmail;
        private System.Windows.Forms.LinkLabel linkAsm;
        private System.Windows.Forms.Button buttonLicenses;
        private System.Windows.Forms.LinkLabel linkTreeViewAdv;
        private System.Windows.Forms.FlowLayoutPanel flowThanks;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;

    }
}
