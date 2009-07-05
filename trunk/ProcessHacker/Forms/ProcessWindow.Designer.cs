using ProcessHacker.Common;
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

            if (_process != null)
                _process.Close();

            if (_threadP != null)
            {
                Program.SecondarySharedThreadProvider.Remove(_threadP);
                // May take a very, very long time
                WorkQueue.GlobalQueueWorkItemTag(
                    new System.Windows.Forms.MethodInvoker(_threadP.Dispose),
                    "threadprovider-dispose"
                    );
                _threadP = null;
            }

            if (_moduleP != null)
            {
                Program.SecondarySharedThreadProvider.Remove(_moduleP);
                _moduleP.Dispose();
                _moduleP = null;
            }

            if (_memoryP != null)
            {
                Program.SecondarySharedThreadProvider.Remove(_memoryP);
                _memoryP.Dispose();
                _memoryP = null;
            }

            if (_handleP != null)
            {
                Program.SecondarySharedThreadProvider.Remove(_handleP);
                _handleP.Dispose();
                _handleP = null;
            }

            if (_tokenProps != null)
                _tokenProps.Dispose();

            if (_serviceProps != null)
                _serviceProps.Dispose();

            // A temporary fix for any handle/memory leaks
            Program.CollectGarbage();

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
            this.mainMenu = new System.Windows.Forms.MainMenu(this.components);
            this.processMenuItem = new System.Windows.Forms.MenuItem();
            this.inspectImageFileMenuItem = new System.Windows.Forms.MenuItem();
            this.windowMenuItem = new System.Windows.Forms.MenuItem();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabGeneral = new System.Windows.Forms.TabPage();
            this.groupProcess = new System.Windows.Forms.GroupBox();
            this.fileCurrentDirectory = new ProcessHacker.Components.FileNameBox();
            this.label26 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.textProtected = new System.Windows.Forms.TextBox();
            this.labelProtected = new System.Windows.Forms.Label();
            this.textDEP = new System.Windows.Forms.TextBox();
            this.labelDEP = new System.Windows.Forms.Label();
            this.buttonTerminate = new System.Windows.Forms.Button();
            this.buttonInspectPEB = new System.Windows.Forms.Button();
            this.buttonEditProtected = new System.Windows.Forms.Button();
            this.buttonInspectParent = new System.Windows.Forms.Button();
            this.buttonEditDEP = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.textParent = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textPEBAddress = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textStartTime = new System.Windows.Forms.TextBox();
            this.textCmdLine = new System.Windows.Forms.TextBox();
            this.groupFile = new System.Windows.Forms.GroupBox();
            this.fileImage = new ProcessHacker.Components.FileNameBox();
            this.pictureIcon = new System.Windows.Forms.PictureBox();
            this.textFileDescription = new System.Windows.Forms.TextBox();
            this.textFileCompany = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textFileVersion = new System.Windows.Forms.TextBox();
            this.tabStatistics = new System.Windows.Forms.TabPage();
            this.tabPerformance = new System.Windows.Forms.TabPage();
            this.tablePerformance = new System.Windows.Forms.TableLayoutPanel();
            this.groupBoxIO = new System.Windows.Forms.GroupBox();
            this.indicatorIO = new ProcessHacker.Components.Indicator();
            this.groupBoxPvt = new System.Windows.Forms.GroupBox();
            this.indicatorPvt = new ProcessHacker.Components.Indicator();
            this.groupCPUUsage = new System.Windows.Forms.GroupBox();
            this.plotterCPUUsage = new ProcessHacker.Components.Plotter();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.plotterMemory = new ProcessHacker.Components.Plotter();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.plotterIO = new ProcessHacker.Components.Plotter();
            this.groupBoxCpu = new System.Windows.Forms.GroupBox();
            this.indicatorCpu = new ProcessHacker.Components.Indicator();
            this.tabThreads = new System.Windows.Forms.TabPage();
            this.listThreads = new ProcessHacker.Components.ThreadList();
            this.tabToken = new System.Windows.Forms.TabPage();
            this.tabModules = new System.Windows.Forms.TabPage();
            this.listModules = new ProcessHacker.Components.ModuleList();
            this.tabMemory = new System.Windows.Forms.TabPage();
            this.label15 = new System.Windows.Forms.Label();
            this.checkHideFreeRegions = new System.Windows.Forms.CheckBox();
            this.buttonSearch = new wyDay.Controls.SplitButton();
            this.menuSearch = new System.Windows.Forms.ContextMenu();
            this.newWindowSearchMenuItem = new System.Windows.Forms.MenuItem();
            this.literalSearchMenuItem = new System.Windows.Forms.MenuItem();
            this.regexSearchMenuItem = new System.Windows.Forms.MenuItem();
            this.stringScanMenuItem = new System.Windows.Forms.MenuItem();
            this.heapScanMenuItem = new System.Windows.Forms.MenuItem();
            this.structSearchMenuItem = new System.Windows.Forms.MenuItem();
            this.listMemory = new ProcessHacker.Components.MemoryList();
            this.tabEnvironment = new System.Windows.Forms.TabPage();
            this.listEnvironment = new System.Windows.Forms.ListView();
            this.columnVarName = new System.Windows.Forms.ColumnHeader();
            this.columnVarValue = new System.Windows.Forms.ColumnHeader();
            this.tabHandles = new System.Windows.Forms.TabPage();
            this.checkHideHandlesNoName = new System.Windows.Forms.CheckBox();
            this.listHandles = new ProcessHacker.Components.HandleList();
            this.tabJob = new System.Windows.Forms.TabPage();
            this.tabServices = new System.Windows.Forms.TabPage();
            this.timerUpdate = new System.Windows.Forms.Timer(this.components);
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.vistaMenu = new wyDay.Controls.VistaMenu(this.components);
            this.tabControl.SuspendLayout();
            this.tabGeneral.SuspendLayout();
            this.groupProcess.SuspendLayout();
            this.groupFile.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureIcon)).BeginInit();
            this.tabPerformance.SuspendLayout();
            this.tablePerformance.SuspendLayout();
            this.groupBoxIO.SuspendLayout();
            this.groupBoxPvt.SuspendLayout();
            this.groupCPUUsage.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBoxCpu.SuspendLayout();
            this.tabThreads.SuspendLayout();
            this.tabModules.SuspendLayout();
            this.tabMemory.SuspendLayout();
            this.tabEnvironment.SuspendLayout();
            this.tabHandles.SuspendLayout();
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
            this.tabControl.Controls.Add(this.tabStatistics);
            this.tabControl.Controls.Add(this.tabPerformance);
            this.tabControl.Controls.Add(this.tabThreads);
            this.tabControl.Controls.Add(this.tabToken);
            this.tabControl.Controls.Add(this.tabModules);
            this.tabControl.Controls.Add(this.tabMemory);
            this.tabControl.Controls.Add(this.tabEnvironment);
            this.tabControl.Controls.Add(this.tabHandles);
            this.tabControl.Controls.Add(this.tabJob);
            this.tabControl.Controls.Add(this.tabServices);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.ItemSize = new System.Drawing.Size(80, 18);
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Multiline = true;
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(489, 434);
            this.tabControl.SizeMode = System.Windows.Forms.TabSizeMode.FillToRight;
            this.tabControl.TabIndex = 0;
            this.tabControl.SelectedIndexChanged += new System.EventHandler(this.tabControl_SelectedIndexChanged);
            // 
            // tabGeneral
            // 
            this.tabGeneral.AutoScroll = true;
            this.tabGeneral.Controls.Add(this.groupProcess);
            this.tabGeneral.Controls.Add(this.groupFile);
            this.tabGeneral.ImageKey = "(none)";
            this.tabGeneral.Location = new System.Drawing.Point(4, 40);
            this.tabGeneral.Name = "tabGeneral";
            this.tabGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tabGeneral.Size = new System.Drawing.Size(481, 390);
            this.tabGeneral.TabIndex = 2;
            this.tabGeneral.Text = "General";
            this.tabGeneral.UseVisualStyleBackColor = true;
            // 
            // groupProcess
            // 
            this.groupProcess.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupProcess.Controls.Add(this.fileCurrentDirectory);
            this.groupProcess.Controls.Add(this.label26);
            this.groupProcess.Controls.Add(this.label7);
            this.groupProcess.Controls.Add(this.textProtected);
            this.groupProcess.Controls.Add(this.labelProtected);
            this.groupProcess.Controls.Add(this.textDEP);
            this.groupProcess.Controls.Add(this.labelDEP);
            this.groupProcess.Controls.Add(this.buttonTerminate);
            this.groupProcess.Controls.Add(this.buttonInspectPEB);
            this.groupProcess.Controls.Add(this.buttonEditProtected);
            this.groupProcess.Controls.Add(this.buttonInspectParent);
            this.groupProcess.Controls.Add(this.buttonEditDEP);
            this.groupProcess.Controls.Add(this.label5);
            this.groupProcess.Controls.Add(this.textParent);
            this.groupProcess.Controls.Add(this.label4);
            this.groupProcess.Controls.Add(this.textPEBAddress);
            this.groupProcess.Controls.Add(this.label2);
            this.groupProcess.Controls.Add(this.textStartTime);
            this.groupProcess.Controls.Add(this.textCmdLine);
            this.groupProcess.Location = new System.Drawing.Point(8, 126);
            this.groupProcess.Name = "groupProcess";
            this.groupProcess.Size = new System.Drawing.Size(467, 258);
            this.groupProcess.TabIndex = 5;
            this.groupProcess.TabStop = false;
            this.groupProcess.Text = "Process";
            // 
            // fileCurrentDirectory
            // 
            this.fileCurrentDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.fileCurrentDirectory.Location = new System.Drawing.Point(101, 73);
            this.fileCurrentDirectory.Name = "fileCurrentDirectory";
            this.fileCurrentDirectory.ReadOnly = true;
            this.fileCurrentDirectory.Size = new System.Drawing.Size(360, 24);
            this.fileCurrentDirectory.TabIndex = 10;
            this.fileCurrentDirectory.Leave += new System.EventHandler(this.fileCurrentDirectory_TextBoxLeave);
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(6, 24);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(44, 13);
            this.label26.TabIndex = 9;
            this.label26.Text = "Started:";
            this.toolTip.SetToolTip(this.label26, "The time at which the program was started.");
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 106);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(72, 13);
            this.label7.TabIndex = 8;
            this.label7.Text = "PEB Address:";
            this.toolTip.SetToolTip(this.label7, "The address of the Process Environment Block (PEB).");
            // 
            // textProtected
            // 
            this.textProtected.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textProtected.BackColor = System.Drawing.SystemColors.Control;
            this.textProtected.Location = new System.Drawing.Point(101, 181);
            this.textProtected.Name = "textProtected";
            this.textProtected.ReadOnly = true;
            this.textProtected.Size = new System.Drawing.Size(330, 20);
            this.textProtected.TabIndex = 7;
            // 
            // labelProtected
            // 
            this.labelProtected.AutoSize = true;
            this.labelProtected.Location = new System.Drawing.Point(6, 184);
            this.labelProtected.Name = "labelProtected";
            this.labelProtected.Size = new System.Drawing.Size(56, 13);
            this.labelProtected.TabIndex = 6;
            this.labelProtected.Text = "Protected:";
            this.toolTip.SetToolTip(this.labelProtected, "Whether the process is DRM-protected.");
            // 
            // textDEP
            // 
            this.textDEP.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textDEP.BackColor = System.Drawing.SystemColors.Control;
            this.textDEP.Location = new System.Drawing.Point(101, 155);
            this.textDEP.Name = "textDEP";
            this.textDEP.ReadOnly = true;
            this.textDEP.Size = new System.Drawing.Size(330, 20);
            this.textDEP.TabIndex = 7;
            // 
            // labelDEP
            // 
            this.labelDEP.AutoSize = true;
            this.labelDEP.Location = new System.Drawing.Point(6, 158);
            this.labelDEP.Name = "labelDEP";
            this.labelDEP.Size = new System.Drawing.Size(32, 13);
            this.labelDEP.TabIndex = 6;
            this.labelDEP.Text = "DEP:";
            this.toolTip.SetToolTip(this.labelDEP, "The status of Data Execution Prevention (DEP) for this process.");
            // 
            // buttonTerminate
            // 
            this.buttonTerminate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonTerminate.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonTerminate.Location = new System.Drawing.Point(386, 20);
            this.buttonTerminate.Name = "buttonTerminate";
            this.buttonTerminate.Size = new System.Drawing.Size(75, 23);
            this.buttonTerminate.TabIndex = 5;
            this.buttonTerminate.Text = "Terminate";
            this.buttonTerminate.UseVisualStyleBackColor = true;
            this.buttonTerminate.Click += new System.EventHandler(this.buttonTerminate_Click);
            // 
            // buttonInspectPEB
            // 
            this.buttonInspectPEB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonInspectPEB.Image = global::ProcessHacker.Properties.Resources.application_form_magnify;
            this.buttonInspectPEB.Location = new System.Drawing.Point(437, 100);
            this.buttonInspectPEB.Name = "buttonInspectPEB";
            this.buttonInspectPEB.Size = new System.Drawing.Size(24, 24);
            this.buttonInspectPEB.TabIndex = 4;
            this.toolTip.SetToolTip(this.buttonInspectPEB, "Inspects the PEB.");
            this.buttonInspectPEB.UseVisualStyleBackColor = true;
            this.buttonInspectPEB.Click += new System.EventHandler(this.buttonInspectPEB_Click);
            // 
            // buttonEditProtected
            // 
            this.buttonEditProtected.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonEditProtected.Image = global::ProcessHacker.Properties.Resources.cog_edit;
            this.buttonEditProtected.Location = new System.Drawing.Point(437, 178);
            this.buttonEditProtected.Name = "buttonEditProtected";
            this.buttonEditProtected.Size = new System.Drawing.Size(24, 24);
            this.buttonEditProtected.TabIndex = 4;
            this.toolTip.SetToolTip(this.buttonEditProtected, "Allows you to protect or unprotect the process.");
            this.buttonEditProtected.UseVisualStyleBackColor = true;
            this.buttonEditProtected.Click += new System.EventHandler(this.buttonEditProtected_Click);
            // 
            // buttonInspectParent
            // 
            this.buttonInspectParent.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonInspectParent.Image = global::ProcessHacker.Properties.Resources.application_form_magnify;
            this.buttonInspectParent.Location = new System.Drawing.Point(437, 126);
            this.buttonInspectParent.Name = "buttonInspectParent";
            this.buttonInspectParent.Size = new System.Drawing.Size(24, 24);
            this.buttonInspectParent.TabIndex = 4;
            this.toolTip.SetToolTip(this.buttonInspectParent, "Inspects the parent process.");
            this.buttonInspectParent.UseVisualStyleBackColor = true;
            this.buttonInspectParent.Click += new System.EventHandler(this.buttonInspectParent_Click);
            // 
            // buttonEditDEP
            // 
            this.buttonEditDEP.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonEditDEP.Image = global::ProcessHacker.Properties.Resources.cog_edit;
            this.buttonEditDEP.Location = new System.Drawing.Point(437, 152);
            this.buttonEditDEP.Name = "buttonEditDEP";
            this.buttonEditDEP.Size = new System.Drawing.Size(24, 24);
            this.buttonEditDEP.TabIndex = 4;
            this.toolTip.SetToolTip(this.buttonEditDEP, "Allows you to change the process\' DEP policy.");
            this.buttonEditDEP.UseVisualStyleBackColor = true;
            this.buttonEditDEP.Click += new System.EventHandler(this.buttonEditDEP_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 132);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Parent:";
            this.toolTip.SetToolTip(this.label5, "The name and ID of the process which started this process.");
            // 
            // textParent
            // 
            this.textParent.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textParent.BackColor = System.Drawing.SystemColors.Control;
            this.textParent.Location = new System.Drawing.Point(101, 129);
            this.textParent.Name = "textParent";
            this.textParent.ReadOnly = true;
            this.textParent.Size = new System.Drawing.Size(330, 20);
            this.textParent.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 78);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Current Directory:";
            this.toolTip.SetToolTip(this.label4, "The program\'s current directory.");
            // 
            // textPEBAddress
            // 
            this.textPEBAddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textPEBAddress.Location = new System.Drawing.Point(101, 103);
            this.textPEBAddress.Name = "textPEBAddress";
            this.textPEBAddress.ReadOnly = true;
            this.textPEBAddress.Size = new System.Drawing.Size(330, 20);
            this.textPEBAddress.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Command Line:";
            this.toolTip.SetToolTip(this.label2, "The command used to start the program.");
            // 
            // textStartTime
            // 
            this.textStartTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textStartTime.Location = new System.Drawing.Point(101, 21);
            this.textStartTime.Name = "textStartTime";
            this.textStartTime.ReadOnly = true;
            this.textStartTime.Size = new System.Drawing.Size(279, 20);
            this.textStartTime.TabIndex = 3;
            // 
            // textCmdLine
            // 
            this.textCmdLine.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textCmdLine.Location = new System.Drawing.Point(101, 47);
            this.textCmdLine.Name = "textCmdLine";
            this.textCmdLine.ReadOnly = true;
            this.textCmdLine.Size = new System.Drawing.Size(360, 20);
            this.textCmdLine.TabIndex = 3;
            // 
            // groupFile
            // 
            this.groupFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupFile.Controls.Add(this.fileImage);
            this.groupFile.Controls.Add(this.pictureIcon);
            this.groupFile.Controls.Add(this.textFileDescription);
            this.groupFile.Controls.Add(this.textFileCompany);
            this.groupFile.Controls.Add(this.label1);
            this.groupFile.Controls.Add(this.label3);
            this.groupFile.Controls.Add(this.textFileVersion);
            this.groupFile.Location = new System.Drawing.Point(6, 7);
            this.groupFile.Name = "groupFile";
            this.groupFile.Size = new System.Drawing.Size(469, 114);
            this.groupFile.TabIndex = 4;
            this.groupFile.TabStop = false;
            this.groupFile.Text = "File";
            // 
            // fileImage
            // 
            this.fileImage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.fileImage.Location = new System.Drawing.Point(103, 83);
            this.fileImage.Name = "fileImage";
            this.fileImage.ReadOnly = true;
            this.fileImage.Size = new System.Drawing.Size(360, 24);
            this.fileImage.TabIndex = 10;
            // 
            // pictureIcon
            // 
            this.pictureIcon.Location = new System.Drawing.Point(6, 20);
            this.pictureIcon.Name = "pictureIcon";
            this.pictureIcon.Size = new System.Drawing.Size(32, 33);
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
            this.textFileDescription.Location = new System.Drawing.Point(44, 20);
            this.textFileDescription.Name = "textFileDescription";
            this.textFileDescription.ReadOnly = true;
            this.textFileDescription.Size = new System.Drawing.Size(419, 13);
            this.textFileDescription.TabIndex = 2;
            this.textFileDescription.Text = "File Description";
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
            this.textFileCompany.Size = new System.Drawing.Size(419, 13);
            this.textFileCompany.TabIndex = 2;
            this.textFileCompany.Text = "File Company";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 60);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Image Version:";
            this.toolTip.SetToolTip(this.label1, "The version of the program.");
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 88);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Image File Name:";
            this.toolTip.SetToolTip(this.label3, "The file name of the program.");
            // 
            // textFileVersion
            // 
            this.textFileVersion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textFileVersion.Location = new System.Drawing.Point(101, 57);
            this.textFileVersion.Name = "textFileVersion";
            this.textFileVersion.ReadOnly = true;
            this.textFileVersion.Size = new System.Drawing.Size(362, 20);
            this.textFileVersion.TabIndex = 2;
            // 
            // tabStatistics
            // 
            this.tabStatistics.ImageKey = "(none)";
            this.tabStatistics.Location = new System.Drawing.Point(4, 40);
            this.tabStatistics.Name = "tabStatistics";
            this.tabStatistics.Padding = new System.Windows.Forms.Padding(3);
            this.tabStatistics.Size = new System.Drawing.Size(481, 390);
            this.tabStatistics.TabIndex = 9;
            this.tabStatistics.Text = "Statistics";
            this.tabStatistics.UseVisualStyleBackColor = true;
            // 
            // tabPerformance
            // 
            this.tabPerformance.Controls.Add(this.tablePerformance);
            this.tabPerformance.ImageKey = "(none)";
            this.tabPerformance.Location = new System.Drawing.Point(4, 40);
            this.tabPerformance.Name = "tabPerformance";
            this.tabPerformance.Padding = new System.Windows.Forms.Padding(3);
            this.tabPerformance.Size = new System.Drawing.Size(481, 390);
            this.tabPerformance.TabIndex = 8;
            this.tabPerformance.Text = "Performance";
            this.tabPerformance.UseVisualStyleBackColor = true;
            // 
            // tablePerformance
            // 
            this.tablePerformance.ColumnCount = 2;
            this.tablePerformance.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 86F));
            this.tablePerformance.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tablePerformance.Controls.Add(this.groupBoxIO, 0, 2);
            this.tablePerformance.Controls.Add(this.groupBoxPvt, 0, 1);
            this.tablePerformance.Controls.Add(this.groupCPUUsage, 1, 0);
            this.tablePerformance.Controls.Add(this.groupBox2, 1, 1);
            this.tablePerformance.Controls.Add(this.groupBox3, 1, 2);
            this.tablePerformance.Controls.Add(this.groupBoxCpu, 0, 0);
            this.tablePerformance.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tablePerformance.Location = new System.Drawing.Point(3, 3);
            this.tablePerformance.Name = "tablePerformance";
            this.tablePerformance.RowCount = 3;
            this.tablePerformance.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tablePerformance.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tablePerformance.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tablePerformance.Size = new System.Drawing.Size(475, 384);
            this.tablePerformance.TabIndex = 1;
            // 
            // groupBoxIO
            // 
            this.groupBoxIO.Controls.Add(this.indicatorIO);
            this.groupBoxIO.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxIO.Location = new System.Drawing.Point(3, 259);
            this.groupBoxIO.Name = "groupBoxIO";
            this.groupBoxIO.Size = new System.Drawing.Size(80, 122);
            this.groupBoxIO.TabIndex = 3;
            this.groupBoxIO.TabStop = false;
            this.groupBoxIO.Text = "I/O Bytes";
            // 
            // indicatorIO
            // 
            this.indicatorIO.BackColor = System.Drawing.Color.Black;
            this.indicatorIO.Color1 = System.Drawing.Color.Cyan;
            this.indicatorIO.Color2 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))));
            this.indicatorIO.Data1 = 0;
            this.indicatorIO.Data2 = 0;
            this.indicatorIO.Dock = System.Windows.Forms.DockStyle.Fill;
            this.indicatorIO.ForeColor = System.Drawing.Color.Lime;
            this.indicatorIO.GraphWidth = 33;
            this.indicatorIO.Location = new System.Drawing.Point(3, 16);
            this.indicatorIO.Maximum = 2147483647;
            this.indicatorIO.Minimum = 0;
            this.indicatorIO.Name = "indicatorIO";
            this.indicatorIO.Size = new System.Drawing.Size(74, 103);
            this.indicatorIO.TabIndex = 1;
            this.indicatorIO.TextValue = "";
            // 
            // groupBoxPvt
            // 
            this.groupBoxPvt.Controls.Add(this.indicatorPvt);
            this.groupBoxPvt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxPvt.Location = new System.Drawing.Point(3, 131);
            this.groupBoxPvt.Name = "groupBoxPvt";
            this.groupBoxPvt.Size = new System.Drawing.Size(80, 122);
            this.groupBoxPvt.TabIndex = 2;
            this.groupBoxPvt.TabStop = false;
            this.groupBoxPvt.Text = "Pvt Bytes";
            // 
            // indicatorPvt
            // 
            this.indicatorPvt.BackColor = System.Drawing.Color.Black;
            this.indicatorPvt.Color1 = System.Drawing.Color.Orange;
            this.indicatorPvt.Color2 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))));
            this.indicatorPvt.Data1 = 0;
            this.indicatorPvt.Data2 = 0;
            this.indicatorPvt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.indicatorPvt.ForeColor = System.Drawing.Color.Lime;
            this.indicatorPvt.GraphWidth = 33;
            this.indicatorPvt.Location = new System.Drawing.Point(3, 16);
            this.indicatorPvt.Maximum = 2147483647;
            this.indicatorPvt.Minimum = 0;
            this.indicatorPvt.Name = "indicatorPvt";
            this.indicatorPvt.Size = new System.Drawing.Size(74, 103);
            this.indicatorPvt.TabIndex = 1;
            this.indicatorPvt.TextValue = "";
            // 
            // groupCPUUsage
            // 
            this.groupCPUUsage.Controls.Add(this.plotterCPUUsage);
            this.groupCPUUsage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupCPUUsage.Location = new System.Drawing.Point(89, 3);
            this.groupCPUUsage.Name = "groupCPUUsage";
            this.groupCPUUsage.Size = new System.Drawing.Size(383, 122);
            this.groupCPUUsage.TabIndex = 0;
            this.groupCPUUsage.TabStop = false;
            this.groupCPUUsage.Text = "CPU Usage (Kernel, User)";
            // 
            // plotterCPUUsage
            // 
            this.plotterCPUUsage.BackColor = System.Drawing.Color.Black;
            this.plotterCPUUsage.Data1 = null;
            this.plotterCPUUsage.Data2 = null;
            this.plotterCPUUsage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plotterCPUUsage.GridColor = System.Drawing.Color.Green;
            this.plotterCPUUsage.GridSize = new System.Drawing.Size(12, 12);
            this.plotterCPUUsage.LineColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))));
            this.plotterCPUUsage.LineColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.plotterCPUUsage.Location = new System.Drawing.Point(3, 16);
            this.plotterCPUUsage.LongData1 = null;
            this.plotterCPUUsage.LongData2 = null;
            this.plotterCPUUsage.MinMaxValue = ((long)(0));
            this.plotterCPUUsage.MoveStep = -1;
            this.plotterCPUUsage.Name = "plotterCPUUsage";
            this.plotterCPUUsage.OverlaySecondLine = false;
            this.plotterCPUUsage.ShowGrid = true;
            this.plotterCPUUsage.Size = new System.Drawing.Size(377, 103);
            this.plotterCPUUsage.TabIndex = 0;
            this.plotterCPUUsage.TextBoxColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.plotterCPUUsage.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))));
            this.plotterCPUUsage.TextMargin = new System.Windows.Forms.Padding(3);
            this.plotterCPUUsage.TextPadding = new System.Windows.Forms.Padding(3);
            this.plotterCPUUsage.TextPosition = System.Drawing.ContentAlignment.TopLeft;
            this.plotterCPUUsage.UseLongData = false;
            this.plotterCPUUsage.UseSecondLine = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.plotterMemory);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(89, 131);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(383, 122);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Memory (Private Pages, Working Set)";
            // 
            // plotterMemory
            // 
            this.plotterMemory.BackColor = System.Drawing.Color.Black;
            this.plotterMemory.Data1 = null;
            this.plotterMemory.Data2 = null;
            this.plotterMemory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plotterMemory.GridColor = System.Drawing.Color.Green;
            this.plotterMemory.GridSize = new System.Drawing.Size(12, 12);
            this.plotterMemory.LineColor1 = System.Drawing.Color.Orange;
            this.plotterMemory.LineColor2 = System.Drawing.Color.Cyan;
            this.plotterMemory.Location = new System.Drawing.Point(3, 16);
            this.plotterMemory.LongData1 = null;
            this.plotterMemory.LongData2 = null;
            this.plotterMemory.MinMaxValue = ((long)(0));
            this.plotterMemory.MoveStep = -1;
            this.plotterMemory.Name = "plotterMemory";
            this.plotterMemory.OverlaySecondLine = true;
            this.plotterMemory.ShowGrid = true;
            this.plotterMemory.Size = new System.Drawing.Size(377, 103);
            this.plotterMemory.TabIndex = 0;
            this.plotterMemory.TextBoxColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.plotterMemory.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))));
            this.plotterMemory.TextMargin = new System.Windows.Forms.Padding(3);
            this.plotterMemory.TextPadding = new System.Windows.Forms.Padding(3);
            this.plotterMemory.TextPosition = System.Drawing.ContentAlignment.TopLeft;
            this.plotterMemory.UseLongData = true;
            this.plotterMemory.UseSecondLine = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.plotterIO);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(89, 259);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(383, 122);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "I/O (R+O, W)";
            // 
            // plotterIO
            // 
            this.plotterIO.BackColor = System.Drawing.Color.Black;
            this.plotterIO.Data1 = null;
            this.plotterIO.Data2 = null;
            this.plotterIO.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plotterIO.GridColor = System.Drawing.Color.Green;
            this.plotterIO.GridSize = new System.Drawing.Size(12, 12);
            this.plotterIO.LineColor1 = System.Drawing.Color.Yellow;
            this.plotterIO.LineColor2 = System.Drawing.Color.Purple;
            this.plotterIO.Location = new System.Drawing.Point(3, 16);
            this.plotterIO.LongData1 = null;
            this.plotterIO.LongData2 = null;
            this.plotterIO.MinMaxValue = ((long)(0));
            this.plotterIO.MoveStep = -1;
            this.plotterIO.Name = "plotterIO";
            this.plotterIO.OverlaySecondLine = true;
            this.plotterIO.ShowGrid = true;
            this.plotterIO.Size = new System.Drawing.Size(377, 103);
            this.plotterIO.TabIndex = 0;
            this.plotterIO.TextBoxColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.plotterIO.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))));
            this.plotterIO.TextMargin = new System.Windows.Forms.Padding(3);
            this.plotterIO.TextPadding = new System.Windows.Forms.Padding(3);
            this.plotterIO.TextPosition = System.Drawing.ContentAlignment.TopLeft;
            this.plotterIO.UseLongData = true;
            this.plotterIO.UseSecondLine = true;
            // 
            // groupBoxCpu
            // 
            this.groupBoxCpu.Controls.Add(this.indicatorCpu);
            this.groupBoxCpu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxCpu.Location = new System.Drawing.Point(3, 3);
            this.groupBoxCpu.Name = "groupBoxCpu";
            this.groupBoxCpu.Size = new System.Drawing.Size(80, 122);
            this.groupBoxCpu.TabIndex = 1;
            this.groupBoxCpu.TabStop = false;
            this.groupBoxCpu.Text = "CPU Usage";
            // 
            // indicatorCpu
            // 
            this.indicatorCpu.BackColor = System.Drawing.Color.Black;
            this.indicatorCpu.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.indicatorCpu.Color2 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))));
            this.indicatorCpu.Data1 = 0;
            this.indicatorCpu.Data2 = 0;
            this.indicatorCpu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.indicatorCpu.ForeColor = System.Drawing.Color.Lime;
            this.indicatorCpu.GraphWidth = 33;
            this.indicatorCpu.Location = new System.Drawing.Point(3, 16);
            this.indicatorCpu.Maximum = 2147483647;
            this.indicatorCpu.Minimum = 0;
            this.indicatorCpu.Name = "indicatorCpu";
            this.indicatorCpu.Size = new System.Drawing.Size(74, 103);
            this.indicatorCpu.TabIndex = 0;
            this.indicatorCpu.TextValue = "";
            // 
            // tabThreads
            // 
            this.tabThreads.Controls.Add(this.listThreads);
            this.tabThreads.ImageKey = "(none)";
            this.tabThreads.Location = new System.Drawing.Point(4, 40);
            this.tabThreads.Name = "tabThreads";
            this.tabThreads.Size = new System.Drawing.Size(481, 390);
            this.tabThreads.TabIndex = 3;
            this.tabThreads.Text = "Threads";
            this.tabThreads.UseVisualStyleBackColor = true;
            // 
            // listThreads
            // 
            this.listThreads.Cursor = System.Windows.Forms.Cursors.Default;
            this.listThreads.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listThreads.DoubleBuffered = true;
            this.listThreads.Location = new System.Drawing.Point(0, 0);
            this.listThreads.Name = "listThreads";
            this.listThreads.Provider = null;
            this.listThreads.Size = new System.Drawing.Size(481, 390);
            this.listThreads.TabIndex = 0;
            // 
            // tabToken
            // 
            this.tabToken.ImageKey = "(none)";
            this.tabToken.Location = new System.Drawing.Point(4, 40);
            this.tabToken.Name = "tabToken";
            this.tabToken.Padding = new System.Windows.Forms.Padding(3);
            this.tabToken.Size = new System.Drawing.Size(481, 390);
            this.tabToken.TabIndex = 1;
            this.tabToken.Text = "Token";
            this.tabToken.UseVisualStyleBackColor = true;
            // 
            // tabModules
            // 
            this.tabModules.Controls.Add(this.listModules);
            this.tabModules.ImageKey = "(none)";
            this.tabModules.Location = new System.Drawing.Point(4, 40);
            this.tabModules.Name = "tabModules";
            this.tabModules.Size = new System.Drawing.Size(481, 390);
            this.tabModules.TabIndex = 6;
            this.tabModules.Text = "Modules";
            this.tabModules.UseVisualStyleBackColor = true;
            // 
            // listModules
            // 
            this.listModules.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listModules.DoubleBuffered = true;
            this.listModules.Location = new System.Drawing.Point(0, 0);
            this.listModules.Name = "listModules";
            this.listModules.Provider = null;
            this.listModules.Size = new System.Drawing.Size(481, 390);
            this.listModules.TabIndex = 0;
            // 
            // tabMemory
            // 
            this.tabMemory.Controls.Add(this.label15);
            this.tabMemory.Controls.Add(this.checkHideFreeRegions);
            this.tabMemory.Controls.Add(this.buttonSearch);
            this.tabMemory.Controls.Add(this.listMemory);
            this.tabMemory.ImageKey = "(none)";
            this.tabMemory.Location = new System.Drawing.Point(4, 40);
            this.tabMemory.Name = "tabMemory";
            this.tabMemory.Padding = new System.Windows.Forms.Padding(3);
            this.tabMemory.Size = new System.Drawing.Size(481, 390);
            this.tabMemory.TabIndex = 4;
            this.tabMemory.Text = "Memory";
            this.tabMemory.UseVisualStyleBackColor = true;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(8, 11);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(44, 13);
            this.label15.TabIndex = 10;
            this.label15.Text = "Search:";
            // 
            // checkHideFreeRegions
            // 
            this.checkHideFreeRegions.AutoSize = true;
            this.checkHideFreeRegions.Checked = true;
            this.checkHideFreeRegions.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkHideFreeRegions.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkHideFreeRegions.Location = new System.Drawing.Point(6, 35);
            this.checkHideFreeRegions.Name = "checkHideFreeRegions";
            this.checkHideFreeRegions.Size = new System.Drawing.Size(120, 18);
            this.checkHideFreeRegions.TabIndex = 1;
            this.checkHideFreeRegions.Text = "Hide Free Regions";
            this.checkHideFreeRegions.UseVisualStyleBackColor = true;
            this.checkHideFreeRegions.CheckedChanged += new System.EventHandler(this.checkHideFreeRegions_CheckedChanged);
            // 
            // buttonSearch
            // 
            this.buttonSearch.AutoSize = true;
            this.buttonSearch.Location = new System.Drawing.Point(58, 7);
            this.buttonSearch.Name = "buttonSearch";
            this.buttonSearch.Size = new System.Drawing.Size(117, 25);
            this.buttonSearch.SplitMenu = this.menuSearch;
            this.buttonSearch.TabIndex = 9;
            this.buttonSearch.Text = "&String Scan...";
            this.buttonSearch.UseVisualStyleBackColor = true;
            this.buttonSearch.Click += new System.EventHandler(this.buttonSearch_Click);
            // 
            // menuSearch
            // 
            this.menuSearch.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.newWindowSearchMenuItem,
            this.literalSearchMenuItem,
            this.regexSearchMenuItem,
            this.stringScanMenuItem,
            this.heapScanMenuItem,
            this.structSearchMenuItem});
            // 
            // newWindowSearchMenuItem
            // 
            this.newWindowSearchMenuItem.Index = 0;
            this.newWindowSearchMenuItem.Text = "&New Window...";
            this.newWindowSearchMenuItem.Click += new System.EventHandler(this.newWindowSearchMenuItem_Click);
            // 
            // literalSearchMenuItem
            // 
            this.literalSearchMenuItem.Index = 1;
            this.literalSearchMenuItem.Text = "&Literal...";
            this.literalSearchMenuItem.Click += new System.EventHandler(this.literalSearchMenuItem_Click);
            // 
            // regexSearchMenuItem
            // 
            this.regexSearchMenuItem.Index = 2;
            this.regexSearchMenuItem.Text = "&Regex...";
            this.regexSearchMenuItem.Click += new System.EventHandler(this.regexSearchMenuItem_Click);
            // 
            // stringScanMenuItem
            // 
            this.stringScanMenuItem.Index = 3;
            this.stringScanMenuItem.Text = "&String Scan...";
            this.stringScanMenuItem.Click += new System.EventHandler(this.stringScanMenuItem_Click);
            // 
            // heapScanMenuItem
            // 
            this.heapScanMenuItem.Index = 4;
            this.heapScanMenuItem.Text = "&Heap Scan...";
            this.heapScanMenuItem.Click += new System.EventHandler(this.heapScanMenuItem_Click);
            // 
            // structSearchMenuItem
            // 
            this.structSearchMenuItem.Index = 5;
            this.structSearchMenuItem.Text = "S&truct...";
            this.structSearchMenuItem.Click += new System.EventHandler(this.structSearchMenuItem_Click);
            // 
            // listMemory
            // 
            this.listMemory.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listMemory.DoubleBuffered = true;
            this.listMemory.Location = new System.Drawing.Point(6, 59);
            this.listMemory.Name = "listMemory";
            this.listMemory.Provider = null;
            this.listMemory.Size = new System.Drawing.Size(469, 325);
            this.listMemory.TabIndex = 0;
            // 
            // tabEnvironment
            // 
            this.tabEnvironment.Controls.Add(this.listEnvironment);
            this.tabEnvironment.ImageKey = "(none)";
            this.tabEnvironment.Location = new System.Drawing.Point(4, 40);
            this.tabEnvironment.Name = "tabEnvironment";
            this.tabEnvironment.Padding = new System.Windows.Forms.Padding(3);
            this.tabEnvironment.Size = new System.Drawing.Size(481, 390);
            this.tabEnvironment.TabIndex = 10;
            this.tabEnvironment.Text = "Environment";
            this.tabEnvironment.UseVisualStyleBackColor = true;
            // 
            // listEnvironment
            // 
            this.listEnvironment.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnVarName,
            this.columnVarValue});
            this.listEnvironment.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listEnvironment.FullRowSelect = true;
            this.listEnvironment.HideSelection = false;
            this.listEnvironment.Location = new System.Drawing.Point(3, 3);
            this.listEnvironment.Name = "listEnvironment";
            this.listEnvironment.ShowItemToolTips = true;
            this.listEnvironment.Size = new System.Drawing.Size(475, 384);
            this.listEnvironment.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listEnvironment.TabIndex = 0;
            this.listEnvironment.UseCompatibleStateImageBehavior = false;
            this.listEnvironment.View = System.Windows.Forms.View.Details;
            // 
            // columnVarName
            // 
            this.columnVarName.Text = "Name";
            this.columnVarName.Width = 150;
            // 
            // columnVarValue
            // 
            this.columnVarValue.Text = "Value";
            this.columnVarValue.Width = 250;
            // 
            // tabHandles
            // 
            this.tabHandles.Controls.Add(this.checkHideHandlesNoName);
            this.tabHandles.Controls.Add(this.listHandles);
            this.tabHandles.ImageKey = "(none)";
            this.tabHandles.Location = new System.Drawing.Point(4, 40);
            this.tabHandles.Name = "tabHandles";
            this.tabHandles.Padding = new System.Windows.Forms.Padding(3);
            this.tabHandles.Size = new System.Drawing.Size(481, 390);
            this.tabHandles.TabIndex = 5;
            this.tabHandles.Text = "Handles";
            this.tabHandles.UseVisualStyleBackColor = true;
            // 
            // checkHideHandlesNoName
            // 
            this.checkHideHandlesNoName.AutoSize = true;
            this.checkHideHandlesNoName.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkHideHandlesNoName.Location = new System.Drawing.Point(6, 7);
            this.checkHideHandlesNoName.Name = "checkHideHandlesNoName";
            this.checkHideHandlesNoName.Size = new System.Drawing.Size(160, 18);
            this.checkHideHandlesNoName.TabIndex = 9;
            this.checkHideHandlesNoName.Text = "Hide handles with no name";
            this.checkHideHandlesNoName.UseVisualStyleBackColor = true;
            this.checkHideHandlesNoName.CheckedChanged += new System.EventHandler(this.checkHideHandlesNoName_CheckedChanged);
            // 
            // listHandles
            // 
            this.listHandles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listHandles.DoubleBuffered = true;
            this.listHandles.Location = new System.Drawing.Point(6, 30);
            this.listHandles.Name = "listHandles";
            this.listHandles.Provider = null;
            this.listHandles.Size = new System.Drawing.Size(469, 354);
            this.listHandles.TabIndex = 0;
            // 
            // tabJob
            // 
            this.tabJob.ImageKey = "(none)";
            this.tabJob.Location = new System.Drawing.Point(4, 40);
            this.tabJob.Name = "tabJob";
            this.tabJob.Size = new System.Drawing.Size(481, 390);
            this.tabJob.TabIndex = 11;
            this.tabJob.Text = "Job";
            this.tabJob.UseVisualStyleBackColor = true;
            // 
            // tabServices
            // 
            this.tabServices.ImageKey = "(none)";
            this.tabServices.Location = new System.Drawing.Point(4, 40);
            this.tabServices.Name = "tabServices";
            this.tabServices.Size = new System.Drawing.Size(481, 390);
            this.tabServices.TabIndex = 7;
            this.tabServices.Text = "Services";
            this.tabServices.UseVisualStyleBackColor = true;
            // 
            // timerUpdate
            // 
            this.timerUpdate.Enabled = true;
            this.timerUpdate.Interval = 2000;
            this.timerUpdate.Tick += new System.EventHandler(this.timerUpdate_Tick);
            // 
            // vistaMenu
            // 
            this.vistaMenu.ContainerControl = this;
            this.vistaMenu.DelaySetImageCalls = false;
            // 
            // ProcessWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(489, 434);
            this.Controls.Add(this.tabControl);
            this.KeyPreview = true;
            this.Menu = this.mainMenu;
            this.MinimumSize = new System.Drawing.Size(454, 433);
            this.Name = "ProcessWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Process";
            this.Load += new System.EventHandler(this.ProcessWindow_Load);
            this.SizeChanged += new System.EventHandler(this.ProcessWindow_SizeChanged);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ProcessWindow_FormClosing);
            this.tabControl.ResumeLayout(false);
            this.tabGeneral.ResumeLayout(false);
            this.groupProcess.ResumeLayout(false);
            this.groupProcess.PerformLayout();
            this.groupFile.ResumeLayout(false);
            this.groupFile.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureIcon)).EndInit();
            this.tabPerformance.ResumeLayout(false);
            this.tablePerformance.ResumeLayout(false);
            this.groupBoxIO.ResumeLayout(false);
            this.groupBoxPvt.ResumeLayout(false);
            this.groupCPUUsage.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBoxCpu.ResumeLayout(false);
            this.tabThreads.ResumeLayout(false);
            this.tabModules.ResumeLayout(false);
            this.tabMemory.ResumeLayout(false);
            this.tabMemory.PerformLayout();
            this.tabEnvironment.ResumeLayout(false);
            this.tabHandles.ResumeLayout(false);
            this.tabHandles.PerformLayout();
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
        private System.Windows.Forms.TabPage tabGeneral;
        private System.Windows.Forms.TabPage tabThreads;
        private ProcessHacker.Components.ThreadList listThreads;
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
        private System.Windows.Forms.GroupBox groupFile;
        private System.Windows.Forms.GroupBox groupProcess;
        private System.Windows.Forms.TabPage tabServices;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private ProcessHacker.Components.ModuleList listModules;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textParent;
        private ProcessHacker.Components.HandleList listHandles;
        private ProcessHacker.Components.MemoryList listMemory;
        private System.Windows.Forms.Button buttonTerminate;
        private System.Windows.Forms.TextBox textDEP;
        private System.Windows.Forms.Label labelDEP;
        private System.Windows.Forms.Button buttonEditDEP;
        private System.Windows.Forms.Button buttonInspectParent;
        private System.Windows.Forms.ContextMenu menuSearch;
        private System.Windows.Forms.MenuItem newWindowSearchMenuItem;
        private System.Windows.Forms.MenuItem literalSearchMenuItem;
        private System.Windows.Forms.MenuItem regexSearchMenuItem;
        private System.Windows.Forms.MenuItem stringScanMenuItem;
        private System.Windows.Forms.MenuItem heapScanMenuItem;
        private System.Windows.Forms.CheckBox checkHideFreeRegions;
        private System.Windows.Forms.CheckBox checkHideHandlesNoName;
        private wyDay.Controls.SplitButton buttonSearch;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textPEBAddress;
        private System.Windows.Forms.Button buttonInspectPEB;
        private System.Windows.Forms.TabPage tabPerformance;
        private System.Windows.Forms.GroupBox groupCPUUsage;
        private ProcessHacker.Components.Plotter plotterCPUUsage;
        private System.Windows.Forms.TabPage tabStatistics;
        private System.Windows.Forms.GroupBox groupBox2;
        private ProcessHacker.Components.Plotter plotterMemory;
        private System.Windows.Forms.TableLayoutPanel tablePerformance;
        private System.Windows.Forms.GroupBox groupBox3;
        private ProcessHacker.Components.Plotter plotterIO;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Timer timerUpdate;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.TextBox textStartTime;
        private ProcessHacker.Components.FileNameBox fileCurrentDirectory;
        private ProcessHacker.Components.FileNameBox fileImage;
        private System.Windows.Forms.MenuItem structSearchMenuItem;
        private System.Windows.Forms.TextBox textProtected;
        private System.Windows.Forms.Label labelProtected;
        private System.Windows.Forms.Button buttonEditProtected;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.TabPage tabEnvironment;
        private System.Windows.Forms.ListView listEnvironment;
        private System.Windows.Forms.ColumnHeader columnVarName;
        private System.Windows.Forms.ColumnHeader columnVarValue;
        private System.Windows.Forms.TabPage tabJob;
        private System.Windows.Forms.GroupBox groupBoxIO;
        private ProcessHacker.Components.Indicator indicatorIO;
        private System.Windows.Forms.GroupBox groupBoxPvt;
        private ProcessHacker.Components.Indicator indicatorPvt;
        private System.Windows.Forms.GroupBox groupBoxCpu;
        private ProcessHacker.Components.Indicator indicatorCpu;
    }
}