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
            this.linkFamFamFam = new System.Windows.Forms.LinkLabel();
            this.linkVistaMenu = new System.Windows.Forms.LinkLabel();
            this.linkHexBox = new System.Windows.Forms.LinkLabel();
            this.labelBy = new System.Windows.Forms.Label();
            this.linkSourceforge = new System.Windows.Forms.LinkLabel();
            this.linkEmail = new System.Windows.Forms.LinkLabel();
            this.linkAsm = new System.Windows.Forms.LinkLabel();
            this.linkTreeViewAdv = new System.Windows.Forms.LinkLabel();
            this.flowCredits = new System.Windows.Forms.FlowLayoutPanel();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.linkTaskDialog = new System.Windows.Forms.LinkLabel();
            this.linkICSharpCode = new System.Windows.Forms.LinkLabel();
            this.label8 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.labelFiller = new System.Windows.Forms.Label();
            this.buttonChangelog = new System.Windows.Forms.Button();
            this.buttonDiagnostics = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.flowCredits.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonClose.Location = new System.Drawing.Point(333, 251);
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
            // linkFamFamFam
            // 
            this.linkFamFamFam.AutoSize = true;
            this.linkFamFamFam.Location = new System.Drawing.Point(6, 81);
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
            this.linkVistaMenu.Location = new System.Drawing.Point(6, 94);
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
            this.linkHexBox.Location = new System.Drawing.Point(6, 68);
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
            this.labelBy.Size = new System.Drawing.Size(234, 17);
            this.labelBy.TabIndex = 8;
            this.labelBy.Text = "Licensed under the GNU GPL, v3.";
            // 
            // linkSourceforge
            // 
            this.linkSourceforge.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.linkSourceforge.AutoSize = true;
            this.linkSourceforge.Location = new System.Drawing.Point(12, 256);
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
            this.linkEmail.Location = new System.Drawing.Point(12, 238);
            this.linkEmail.Name = "linkEmail";
            this.linkEmail.Size = new System.Drawing.Size(145, 13);
            this.linkEmail.TabIndex = 10;
            this.linkEmail.TabStop = true;
            this.linkEmail.Text = "Post feedback on our tracker";
            this.linkEmail.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkEmail_LinkClicked);
            // 
            // linkAsm
            // 
            this.linkAsm.AutoSize = true;
            this.linkAsm.Location = new System.Drawing.Point(6, 107);
            this.linkAsm.Name = "linkAsm";
            this.linkAsm.Size = new System.Drawing.Size(198, 13);
            this.linkAsm.TabIndex = 12;
            this.linkAsm.TabStop = true;
            this.linkAsm.Text = "Oleh Yuschuk - Disassembler/Assembler";
            this.linkAsm.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkAsm_LinkClicked);
            // 
            // linkTreeViewAdv
            // 
            this.linkTreeViewAdv.AutoSize = true;
            this.linkTreeViewAdv.Location = new System.Drawing.Point(6, 120);
            this.linkTreeViewAdv.Name = "linkTreeViewAdv";
            this.linkTreeViewAdv.Size = new System.Drawing.Size(165, 13);
            this.linkTreeViewAdv.TabIndex = 12;
            this.linkTreeViewAdv.TabStop = true;
            this.linkTreeViewAdv.Text = "Andrey Gliznetsov - TreeViewAdv";
            this.linkTreeViewAdv.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkTreeViewAdv_LinkClicked);
            // 
            // flowCredits
            // 
            this.flowCredits.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.flowCredits.AutoScroll = true;
            this.flowCredits.Controls.Add(this.label5);
            this.flowCredits.Controls.Add(this.label6);
            this.flowCredits.Controls.Add(this.label1);
            this.flowCredits.Controls.Add(this.label9);
            this.flowCredits.Controls.Add(this.label7);
            this.flowCredits.Controls.Add(this.linkHexBox);
            this.flowCredits.Controls.Add(this.linkFamFamFam);
            this.flowCredits.Controls.Add(this.linkVistaMenu);
            this.flowCredits.Controls.Add(this.linkAsm);
            this.flowCredits.Controls.Add(this.linkTreeViewAdv);
            this.flowCredits.Controls.Add(this.linkTaskDialog);
            this.flowCredits.Controls.Add(this.linkICSharpCode);
            this.flowCredits.Controls.Add(this.label8);
            this.flowCredits.Controls.Add(this.label4);
            this.flowCredits.Controls.Add(this.label3);
            this.flowCredits.Controls.Add(this.label2);
            this.flowCredits.Controls.Add(this.labelFiller);
            this.flowCredits.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowCredits.Location = new System.Drawing.Point(177, 70);
            this.flowCredits.Name = "flowCredits";
            this.flowCredits.Padding = new System.Windows.Forms.Padding(3);
            this.flowCredits.Size = new System.Drawing.Size(231, 147);
            this.flowCredits.TabIndex = 14;
            this.flowCredits.WrapContents = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 3);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(116, 13);
            this.label5.TabIndex = 18;
            this.label5.Text = "wj32 - Project Manager";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 16);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(91, 13);
            this.label6.TabIndex = 18;
            this.label6.Text = "Dean - Developer";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(188, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "XhmikosR - Installer Developer, Tester";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 42);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(127, 13);
            this.label9.TabIndex = 22;
            this.label9.Text = "Mikalai Chaly - Developer";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 55);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(142, 13);
            this.label7.TabIndex = 18;
            this.label7.Text = "Uday Shanbhag - Developer";
            // 
            // linkTaskDialog
            // 
            this.linkTaskDialog.AutoSize = true;
            this.linkTaskDialog.Location = new System.Drawing.Point(6, 133);
            this.linkTaskDialog.Name = "linkTaskDialog";
            this.linkTaskDialog.Size = new System.Drawing.Size(114, 13);
            this.linkTaskDialog.TabIndex = 21;
            this.linkTaskDialog.TabStop = true;
            this.linkTaskDialog.Text = "KevinGre - TaskDialog";
            this.linkTaskDialog.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkTaskDialog_LinkClicked);
            // 
            // linkICSharpCode
            // 
            this.linkICSharpCode.AutoSize = true;
            this.linkICSharpCode.Location = new System.Drawing.Point(6, 146);
            this.linkICSharpCode.Name = "linkICSharpCode";
            this.linkICSharpCode.Size = new System.Drawing.Size(144, 13);
            this.linkICSharpCode.TabIndex = 19;
            this.linkICSharpCode.TabStop = true;
            this.linkICSharpCode.Text = "ic#code - .NET runtime code";
            this.linkICSharpCode.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkICSharpCode_LinkClicked);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 159);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(100, 13);
            this.label8.TabIndex = 20;
            this.label8.Text = "Sysinternals Forums";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 172);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(199, 26);
            this.label4.TabIndex = 16;
            this.label4.Text = "NTinternals - a great guide to the Native API";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 198);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(202, 26);
            this.label3.TabIndex = 15;
            this.label3.Text = "ReactOS - the ultimate guide to Windows internals";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 224);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(193, 26);
            this.label2.TabIndex = 14;
            this.label2.Text = "Greg Hoglund, James Butler - Rootkits: Subverting the Windows Kernel";
            // 
            // labelFiller
            // 
            this.labelFiller.AutoSize = true;
            this.labelFiller.Location = new System.Drawing.Point(6, 250);
            this.labelFiller.Name = "labelFiller";
            this.labelFiller.Size = new System.Drawing.Size(31, 13);
            this.labelFiller.TabIndex = 17;
            this.labelFiller.Text = "        ";
            // 
            // buttonChangelog
            // 
            this.buttonChangelog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonChangelog.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonChangelog.Location = new System.Drawing.Point(238, 223);
            this.buttonChangelog.Name = "buttonChangelog";
            this.buttonChangelog.Size = new System.Drawing.Size(75, 23);
            this.buttonChangelog.TabIndex = 15;
            this.buttonChangelog.Text = "Changelog...";
            this.buttonChangelog.UseVisualStyleBackColor = true;
            this.buttonChangelog.Click += new System.EventHandler(this.buttonChangelog_Click);
            // 
            // buttonDiagnostics
            // 
            this.buttonDiagnostics.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonDiagnostics.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonDiagnostics.Location = new System.Drawing.Point(319, 223);
            this.buttonDiagnostics.Name = "buttonDiagnostics";
            this.buttonDiagnostics.Size = new System.Drawing.Size(89, 23);
            this.buttonDiagnostics.TabIndex = 16;
            this.buttonDiagnostics.Text = "Diagnostics...";
            this.buttonDiagnostics.UseVisualStyleBackColor = true;
            this.buttonDiagnostics.Click += new System.EventHandler(this.buttonDiagnostics_Click);
            // 
            // AboutWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(420, 286);
            this.Controls.Add(this.buttonDiagnostics);
            this.Controls.Add(this.buttonChangelog);
            this.Controls.Add(this.flowCredits);
            this.Controls.Add(this.linkEmail);
            this.Controls.Add(this.linkSourceforge);
            this.Controls.Add(this.labelBy);
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
            this.flowCredits.ResumeLayout(false);
            this.flowCredits.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Label labelAppName;
        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.LinkLabel linkFamFamFam;
        private System.Windows.Forms.LinkLabel linkVistaMenu;
        private System.Windows.Forms.LinkLabel linkHexBox;
        private System.Windows.Forms.Label labelBy;
        private System.Windows.Forms.LinkLabel linkSourceforge;
        private System.Windows.Forms.LinkLabel linkEmail;
        private System.Windows.Forms.LinkLabel linkAsm;
        private System.Windows.Forms.LinkLabel linkTreeViewAdv;
        private System.Windows.Forms.FlowLayoutPanel flowCredits;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label labelFiller;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.LinkLabel linkICSharpCode;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button buttonChangelog;
        private System.Windows.Forms.LinkLabel linkTaskDialog;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button buttonDiagnostics;

    }
}
