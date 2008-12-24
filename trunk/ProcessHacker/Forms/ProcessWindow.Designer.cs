namespace ProcessHacker
{
    partial class ProcessWindow
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

            Program.PWindows.Remove(_pid);
            Program.UpdateWindows();

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProcessWindow));
            this.mainMenu = new System.Windows.Forms.MainMenu(this.components);
            this.processMenuItem = new System.Windows.Forms.MenuItem();
            this.inspectImageFileMenuItem = new System.Windows.Forms.MenuItem();
            this.windowMenuItem = new System.Windows.Forms.MenuItem();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabGeneral = new System.Windows.Forms.TabPage();
            this.groupProcess = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textCmdLine = new System.Windows.Forms.TextBox();
            this.groupFile = new System.Windows.Forms.GroupBox();
            this.pictureIcon = new System.Windows.Forms.PictureBox();
            this.textFileDescription = new System.Windows.Forms.TextBox();
            this.textFileName = new System.Windows.Forms.TextBox();
            this.textFileCompany = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textFileVersion = new System.Windows.Forms.TextBox();
            this.tabThreads = new System.Windows.Forms.TabPage();
            this.listThreads = new ProcessHacker.ThreadList();
            this.tabToken = new System.Windows.Forms.TabPage();
            this.tabModules = new System.Windows.Forms.TabPage();
            this.tabMemory = new System.Windows.Forms.TabPage();
            this.tabHandles = new System.Windows.Forms.TabPage();
            this.tabServices = new System.Windows.Forms.TabPage();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.vistaMenu = new wyDay.Controls.VistaMenu(this.components);
            this.tabControl.SuspendLayout();
            this.tabGeneral.SuspendLayout();
            this.groupProcess.SuspendLayout();
            this.groupFile.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureIcon)).BeginInit();
            this.tabThreads.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.vistaMenu)).BeginInit();
            this.SuspendLayout();
            // 
            // mainMenu
            // 
            this.mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.processMenuItem,
            this.windowMenuItem});
            // 
            // processMenuItem
            // 
            this.processMenuItem.Index = 0;
            this.processMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.inspectImageFileMenuItem});
            this.processMenuItem.Text = "&Process";
            // 
            // inspectImageFileMenuItem
            // 
            this.vistaMenu.SetImage(this.inspectImageFileMenuItem, global::ProcessHacker.Properties.Resources.application_form_magnify);
            this.inspectImageFileMenuItem.Index = 0;
            this.inspectImageFileMenuItem.Text = "&Inspect Image File...";
            this.inspectImageFileMenuItem.Click += new System.EventHandler(this.inspectImageFileMenuItem_Click);
            // 
            // windowMenuItem
            // 
            this.windowMenuItem.Index = 1;
            this.windowMenuItem.Text = "&Window";
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabGeneral);
            this.tabControl.Controls.Add(this.tabThreads);
            this.tabControl.Controls.Add(this.tabToken);
            this.tabControl.Controls.Add(this.tabModules);
            this.tabControl.Controls.Add(this.tabMemory);
            this.tabControl.Controls.Add(this.tabHandles);
            this.tabControl.Controls.Add(this.tabServices);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.ImageList = this.imageList;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Multiline = true;
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(477, 291);
            this.tabControl.TabIndex = 0;
            this.tabControl.TabIndexChanged += new System.EventHandler(this.tabControl_TabIndexChanged);
            // 
            // tabGeneral
            // 
            this.tabGeneral.AutoScroll = true;
            this.tabGeneral.Controls.Add(this.groupProcess);
            this.tabGeneral.Controls.Add(this.groupFile);
            this.tabGeneral.ImageKey = "application";
            this.tabGeneral.Location = new System.Drawing.Point(4, 42);
            this.tabGeneral.Name = "tabGeneral";
            this.tabGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tabGeneral.Size = new System.Drawing.Size(469, 245);
            this.tabGeneral.TabIndex = 2;
            this.tabGeneral.Text = "General";
            this.tabGeneral.UseVisualStyleBackColor = true;
            // 
            // groupProcess
            // 
            this.groupProcess.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupProcess.Controls.Add(this.label2);
            this.groupProcess.Controls.Add(this.textCmdLine);
            this.groupProcess.Location = new System.Drawing.Point(8, 117);
            this.groupProcess.Name = "groupProcess";
            this.groupProcess.Size = new System.Drawing.Size(455, 122);
            this.groupProcess.TabIndex = 5;
            this.groupProcess.TabStop = false;
            this.groupProcess.Text = "Process";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Command Line:";
            // 
            // textCmdLine
            // 
            this.textCmdLine.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textCmdLine.Location = new System.Drawing.Point(92, 19);
            this.textCmdLine.Name = "textCmdLine";
            this.textCmdLine.ReadOnly = true;
            this.textCmdLine.Size = new System.Drawing.Size(357, 20);
            this.textCmdLine.TabIndex = 3;
            // 
            // groupFile
            // 
            this.groupFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupFile.Controls.Add(this.pictureIcon);
            this.groupFile.Controls.Add(this.textFileDescription);
            this.groupFile.Controls.Add(this.textFileName);
            this.groupFile.Controls.Add(this.textFileCompany);
            this.groupFile.Controls.Add(this.label3);
            this.groupFile.Controls.Add(this.textFileVersion);
            this.groupFile.Location = new System.Drawing.Point(6, 6);
            this.groupFile.Name = "groupFile";
            this.groupFile.Size = new System.Drawing.Size(457, 105);
            this.groupFile.TabIndex = 4;
            this.groupFile.TabStop = false;
            this.groupFile.Text = "File";
            // 
            // pictureIcon
            // 
            this.pictureIcon.Location = new System.Drawing.Point(6, 19);
            this.pictureIcon.Name = "pictureIcon";
            this.pictureIcon.Size = new System.Drawing.Size(32, 32);
            this.pictureIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureIcon.TabIndex = 1;
            this.pictureIcon.TabStop = false;
            // 
            // textFileDescription
            // 
            this.textFileDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textFileDescription.BackColor = System.Drawing.SystemColors.Window;
            this.textFileDescription.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textFileDescription.Location = new System.Drawing.Point(44, 19);
            this.textFileDescription.Name = "textFileDescription";
            this.textFileDescription.ReadOnly = true;
            this.textFileDescription.Size = new System.Drawing.Size(407, 13);
            this.textFileDescription.TabIndex = 2;
            this.textFileDescription.Text = "File Description";
            // 
            // textFileName
            // 
            this.textFileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textFileName.Location = new System.Drawing.Point(101, 76);
            this.textFileName.Name = "textFileName";
            this.textFileName.ReadOnly = true;
            this.textFileName.Size = new System.Drawing.Size(350, 20);
            this.textFileName.TabIndex = 3;
            // 
            // textFileCompany
            // 
            this.textFileCompany.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textFileCompany.BackColor = System.Drawing.SystemColors.Window;
            this.textFileCompany.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textFileCompany.Location = new System.Drawing.Point(44, 38);
            this.textFileCompany.Name = "textFileCompany";
            this.textFileCompany.ReadOnly = true;
            this.textFileCompany.Size = new System.Drawing.Size(407, 13);
            this.textFileCompany.TabIndex = 2;
            this.textFileCompany.Text = "File Company";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 79);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Image File Name:";
            // 
            // textFileVersion
            // 
            this.textFileVersion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textFileVersion.BackColor = System.Drawing.SystemColors.Window;
            this.textFileVersion.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textFileVersion.Location = new System.Drawing.Point(6, 57);
            this.textFileVersion.Name = "textFileVersion";
            this.textFileVersion.ReadOnly = true;
            this.textFileVersion.Size = new System.Drawing.Size(445, 13);
            this.textFileVersion.TabIndex = 2;
            this.textFileVersion.Text = "File Version";
            // 
            // tabThreads
            // 
            this.tabThreads.Controls.Add(this.listThreads);
            this.tabThreads.ImageKey = "hourglass";
            this.tabThreads.Location = new System.Drawing.Point(4, 42);
            this.tabThreads.Name = "tabThreads";
            this.tabThreads.Size = new System.Drawing.Size(469, 245);
            this.tabThreads.TabIndex = 3;
            this.tabThreads.Text = "Threads";
            this.tabThreads.UseVisualStyleBackColor = true;
            // 
            // listThreads
            // 
            this.listThreads.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listThreads.DoubleBuffered = true;
            this.listThreads.Location = new System.Drawing.Point(0, 0);
            this.listThreads.Name = "listThreads";
            this.listThreads.Provider = null;
            this.listThreads.Size = new System.Drawing.Size(469, 245);
            this.listThreads.TabIndex = 0;
            // 
            // tabToken
            // 
            this.tabToken.ImageKey = "token";
            this.tabToken.Location = new System.Drawing.Point(4, 42);
            this.tabToken.Name = "tabToken";
            this.tabToken.Padding = new System.Windows.Forms.Padding(3);
            this.tabToken.Size = new System.Drawing.Size(469, 245);
            this.tabToken.TabIndex = 1;
            this.tabToken.Text = "Token";
            this.tabToken.UseVisualStyleBackColor = true;
            // 
            // tabModules
            // 
            this.tabModules.ImageKey = "page_white_wrench";
            this.tabModules.Location = new System.Drawing.Point(4, 42);
            this.tabModules.Name = "tabModules";
            this.tabModules.Size = new System.Drawing.Size(469, 245);
            this.tabModules.TabIndex = 6;
            this.tabModules.Text = "Modules";
            this.tabModules.UseVisualStyleBackColor = true;
            // 
            // tabMemory
            // 
            this.tabMemory.ImageKey = "database";
            this.tabMemory.Location = new System.Drawing.Point(4, 42);
            this.tabMemory.Name = "tabMemory";
            this.tabMemory.Size = new System.Drawing.Size(469, 245);
            this.tabMemory.TabIndex = 4;
            this.tabMemory.Text = "Memory";
            this.tabMemory.UseVisualStyleBackColor = true;
            // 
            // tabHandles
            // 
            this.tabHandles.ImageKey = "connect";
            this.tabHandles.Location = new System.Drawing.Point(4, 42);
            this.tabHandles.Name = "tabHandles";
            this.tabHandles.Size = new System.Drawing.Size(469, 245);
            this.tabHandles.TabIndex = 5;
            this.tabHandles.Text = "Handles";
            this.tabHandles.UseVisualStyleBackColor = true;
            // 
            // tabServices
            // 
            this.tabServices.ImageKey = "cog";
            this.tabServices.Location = new System.Drawing.Point(4, 42);
            this.tabServices.Name = "tabServices";
            this.tabServices.Size = new System.Drawing.Size(469, 245);
            this.tabServices.TabIndex = 7;
            this.tabServices.Text = "Services";
            this.tabServices.UseVisualStyleBackColor = true;
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "token");
            this.imageList.Images.SetKeyName(1, "application");
            this.imageList.Images.SetKeyName(2, "cog");
            this.imageList.Images.SetKeyName(3, "page_white_wrench");
            this.imageList.Images.SetKeyName(4, "database");
            this.imageList.Images.SetKeyName(5, "connect");
            this.imageList.Images.SetKeyName(6, "hourglass");
            // 
            // vistaMenu
            // 
            this.vistaMenu.ContainerControl = this;
            // 
            // ProcessWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(477, 291);
            this.Controls.Add(this.tabControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Menu = this.mainMenu;
            this.Name = "ProcessWindow";
            this.Text = "Process";
            this.Load += new System.EventHandler(this.ProcessWindow_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ProcessWindow_FormClosing);
            this.tabControl.ResumeLayout(false);
            this.tabGeneral.ResumeLayout(false);
            this.groupProcess.ResumeLayout(false);
            this.groupProcess.PerformLayout();
            this.groupFile.ResumeLayout(false);
            this.groupFile.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureIcon)).EndInit();
            this.tabThreads.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.vistaMenu)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.MainMenu mainMenu;
        private wyDay.Controls.VistaMenu vistaMenu;
        private System.Windows.Forms.MenuItem processMenuItem;
        private System.Windows.Forms.MenuItem windowMenuItem;
        private System.Windows.Forms.MenuItem inspectImageFileMenuItem;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabToken;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.TabPage tabGeneral;
        private System.Windows.Forms.TabPage tabThreads;
        private ThreadList listThreads;
        private System.Windows.Forms.TabPage tabModules;
        private System.Windows.Forms.TabPage tabMemory;
        private System.Windows.Forms.TabPage tabHandles;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pictureIcon;
        private System.Windows.Forms.TextBox textFileDescription;
        private System.Windows.Forms.TextBox textFileVersion;
        private System.Windows.Forms.TextBox textFileCompany;
        private System.Windows.Forms.TextBox textCmdLine;
        private System.Windows.Forms.TextBox textFileName;
        private System.Windows.Forms.GroupBox groupFile;
        private System.Windows.Forms.GroupBox groupProcess;
        private System.Windows.Forms.TabPage tabServices;
    }
}