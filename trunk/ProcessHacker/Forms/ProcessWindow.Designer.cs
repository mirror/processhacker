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

            if (_process != null)
                _process.Close();

            if (_threadP != null)
            {
                Program.SecondarySharedThreadProvider.Remove(_threadP);
                _threadP.Dispose();
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

            Program.PWindows.Remove(_pid);
            Program.UpdateWindows();

            // A temporary fix for any handle/memory leaks
            System.GC.Collect();

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
            this.flowStats = new System.Windows.Forms.FlowLayoutPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label6 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.labelCPUPriority = new System.Windows.Forms.Label();
            this.labelCPUKernelTime = new System.Windows.Forms.Label();
            this.labelCPUUserTime = new System.Windows.Forms.Label();
            this.labelCPUTotalTime = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label24 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.labelMemoryPB = new System.Windows.Forms.Label();
            this.labelMemoryWS = new System.Windows.Forms.Label();
            this.labelMemoryPWS = new System.Windows.Forms.Label();
            this.labelMemoryVS = new System.Windows.Forms.Label();
            this.labelMemoryPVS = new System.Windows.Forms.Label();
            this.labelMemoryPU = new System.Windows.Forms.Label();
            this.labelMemoryPPU = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.labelMemoryPF = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.labelIOReads = new System.Windows.Forms.Label();
            this.labelIOReadBytes = new System.Windows.Forms.Label();
            this.labelIOWrites = new System.Windows.Forms.Label();
            this.labelIOWriteBytes = new System.Windows.Forms.Label();
            this.labelIOOther = new System.Windows.Forms.Label();
            this.labelIOOtherBytes = new System.Windows.Forms.Label();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.label27 = new System.Windows.Forms.Label();
            this.labelOtherHandles = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.labelOtherGDIHandles = new System.Windows.Forms.Label();
            this.labelOtherUSERHandles = new System.Windows.Forms.Label();
            this.tabPerformance = new System.Windows.Forms.TabPage();
            this.tablePerformance = new System.Windows.Forms.TableLayoutPanel();
            this.groupCPUUsage = new System.Windows.Forms.GroupBox();
            this.plotterCPUUsage = new ProcessHacker.Components.Plotter();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.plotterIO = new ProcessHacker.Components.Plotter();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.plotterMemory = new ProcessHacker.Components.Plotter();
            this.tabThreads = new System.Windows.Forms.TabPage();
            this.listThreads = new ProcessHacker.ThreadList();
            this.tabToken = new System.Windows.Forms.TabPage();
            this.tabModules = new System.Windows.Forms.TabPage();
            this.listModules = new ProcessHacker.ModuleList();
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
            this.listMemory = new ProcessHacker.MemoryList();
            this.tabEnvironment = new System.Windows.Forms.TabPage();
            this.listEnvironment = new System.Windows.Forms.ListView();
            this.columnVarName = new System.Windows.Forms.ColumnHeader();
            this.columnVarValue = new System.Windows.Forms.ColumnHeader();
            this.tabHandles = new System.Windows.Forms.TabPage();
            this.checkHideHandlesNoName = new System.Windows.Forms.CheckBox();
            this.listHandles = new ProcessHacker.HandleList();
            this.tabServices = new System.Windows.Forms.TabPage();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.timerUpdate = new System.Windows.Forms.Timer(this.components);
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.vistaMenu = new wyDay.Controls.VistaMenu(this.components);
            this.tabControl.SuspendLayout();
            this.tabGeneral.SuspendLayout();
            this.groupProcess.SuspendLayout();
            this.groupFile.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureIcon)).BeginInit();
            this.tabStatistics.SuspendLayout();
            this.flowStats.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.tabPerformance.SuspendLayout();
            this.tablePerformance.SuspendLayout();
            this.groupCPUUsage.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
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
            this.tabControl.Controls.Add(this.tabServices);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.ImageList = this.imageList;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Multiline = true;
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(438, 397);
            this.tabControl.TabIndex = 0;
            this.tabControl.SelectedIndexChanged += new System.EventHandler(this.tabControl_SelectedIndexChanged);
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
            this.tabGeneral.Size = new System.Drawing.Size(430, 351);
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
            this.groupProcess.Size = new System.Drawing.Size(416, 219);
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
            this.fileCurrentDirectory.Size = new System.Drawing.Size(309, 24);
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
            this.textProtected.Size = new System.Drawing.Size(279, 20);
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
            this.textDEP.Size = new System.Drawing.Size(279, 20);
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
            this.buttonTerminate.Location = new System.Drawing.Point(335, 19);
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
            this.buttonInspectPEB.Location = new System.Drawing.Point(386, 100);
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
            this.buttonEditProtected.Location = new System.Drawing.Point(386, 178);
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
            this.buttonInspectParent.Location = new System.Drawing.Point(386, 126);
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
            this.buttonEditDEP.Location = new System.Drawing.Point(386, 152);
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
            this.textParent.Size = new System.Drawing.Size(279, 20);
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
            this.textPEBAddress.Size = new System.Drawing.Size(279, 20);
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
            this.textStartTime.Size = new System.Drawing.Size(228, 20);
            this.textStartTime.TabIndex = 3;
            // 
            // textCmdLine
            // 
            this.textCmdLine.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textCmdLine.Location = new System.Drawing.Point(101, 47);
            this.textCmdLine.Name = "textCmdLine";
            this.textCmdLine.ReadOnly = true;
            this.textCmdLine.Size = new System.Drawing.Size(309, 20);
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
            this.groupFile.Location = new System.Drawing.Point(6, 6);
            this.groupFile.Name = "groupFile";
            this.groupFile.Size = new System.Drawing.Size(418, 114);
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
            this.fileImage.Size = new System.Drawing.Size(309, 24);
            this.fileImage.TabIndex = 10;
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
            this.textFileDescription.Size = new System.Drawing.Size(368, 13);
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
            this.textFileCompany.Size = new System.Drawing.Size(368, 13);
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
            this.textFileVersion.Size = new System.Drawing.Size(311, 20);
            this.textFileVersion.TabIndex = 2;
            // 
            // tabStatistics
            // 
            this.tabStatistics.Controls.Add(this.flowStats);
            this.tabStatistics.ImageKey = "chart_bar";
            this.tabStatistics.Location = new System.Drawing.Point(4, 42);
            this.tabStatistics.Name = "tabStatistics";
            this.tabStatistics.Padding = new System.Windows.Forms.Padding(3);
            this.tabStatistics.Size = new System.Drawing.Size(430, 351);
            this.tabStatistics.TabIndex = 9;
            this.tabStatistics.Text = "Statistics";
            this.tabStatistics.UseVisualStyleBackColor = true;
            // 
            // flowStats
            // 
            this.flowStats.Controls.Add(this.groupBox1);
            this.flowStats.Controls.Add(this.groupBox4);
            this.flowStats.Controls.Add(this.groupBox5);
            this.flowStats.Controls.Add(this.groupBox6);
            this.flowStats.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowStats.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowStats.Location = new System.Drawing.Point(3, 3);
            this.flowStats.Name = "flowStats";
            this.flowStats.Size = new System.Drawing.Size(424, 345);
            this.flowStats.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tableLayoutPanel1);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(195, 108);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "CPU";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.label6, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label8, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label9, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label10, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.labelCPUPriority, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelCPUKernelTime, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelCPUUserTime, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.labelCPUTotalTime, 1, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(189, 89);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 4);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(38, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Priority";
            // 
            // label8
            // 
            this.label8.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 26);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(63, 13);
            this.label8.TabIndex = 1;
            this.label8.Text = "Kernel Time";
            // 
            // label9
            // 
            this.label9.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(3, 48);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(55, 13);
            this.label9.TabIndex = 1;
            this.label9.Text = "User Time";
            // 
            // label10
            // 
            this.label10.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(3, 71);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(57, 13);
            this.label10.TabIndex = 1;
            this.label10.Text = "Total Time";
            // 
            // labelCPUPriority
            // 
            this.labelCPUPriority.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelCPUPriority.AutoSize = true;
            this.labelCPUPriority.Location = new System.Drawing.Point(153, 4);
            this.labelCPUPriority.Name = "labelCPUPriority";
            this.labelCPUPriority.Size = new System.Drawing.Size(33, 13);
            this.labelCPUPriority.TabIndex = 1;
            this.labelCPUPriority.Text = "value";
            // 
            // labelCPUKernelTime
            // 
            this.labelCPUKernelTime.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelCPUKernelTime.AutoSize = true;
            this.labelCPUKernelTime.Location = new System.Drawing.Point(153, 26);
            this.labelCPUKernelTime.Name = "labelCPUKernelTime";
            this.labelCPUKernelTime.Size = new System.Drawing.Size(33, 13);
            this.labelCPUKernelTime.TabIndex = 1;
            this.labelCPUKernelTime.Text = "value";
            // 
            // labelCPUUserTime
            // 
            this.labelCPUUserTime.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelCPUUserTime.AutoSize = true;
            this.labelCPUUserTime.Location = new System.Drawing.Point(153, 48);
            this.labelCPUUserTime.Name = "labelCPUUserTime";
            this.labelCPUUserTime.Size = new System.Drawing.Size(33, 13);
            this.labelCPUUserTime.TabIndex = 1;
            this.labelCPUUserTime.Text = "value";
            // 
            // labelCPUTotalTime
            // 
            this.labelCPUTotalTime.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelCPUTotalTime.AutoSize = true;
            this.labelCPUTotalTime.Location = new System.Drawing.Point(153, 71);
            this.labelCPUTotalTime.Name = "labelCPUTotalTime";
            this.labelCPUTotalTime.Size = new System.Drawing.Size(33, 13);
            this.labelCPUTotalTime.TabIndex = 1;
            this.labelCPUTotalTime.Text = "value";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.tableLayoutPanel2);
            this.groupBox4.Location = new System.Drawing.Point(3, 117);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(195, 180);
            this.groupBox4.TabIndex = 0;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Memory";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.label24, 0, 6);
            this.tableLayoutPanel2.Controls.Add(this.label22, 0, 5);
            this.tableLayoutPanel2.Controls.Add(this.label20, 0, 4);
            this.tableLayoutPanel2.Controls.Add(this.label11, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.label12, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.label13, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.label14, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.labelMemoryPB, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.labelMemoryWS, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.labelMemoryPWS, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.labelMemoryVS, 1, 3);
            this.tableLayoutPanel2.Controls.Add(this.labelMemoryPVS, 1, 4);
            this.tableLayoutPanel2.Controls.Add(this.labelMemoryPU, 1, 5);
            this.tableLayoutPanel2.Controls.Add(this.labelMemoryPPU, 1, 6);
            this.tableLayoutPanel2.Controls.Add(this.label25, 0, 7);
            this.tableLayoutPanel2.Controls.Add(this.labelMemoryPF, 1, 7);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 8;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(189, 161);
            this.tableLayoutPanel2.TabIndex = 1;
            // 
            // label24
            // 
            this.label24.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(3, 123);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(107, 13);
            this.label24.TabIndex = 7;
            this.label24.Text = "Peak Pagefile Usage";
            // 
            // label22
            // 
            this.label22.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(3, 103);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(79, 13);
            this.label22.TabIndex = 5;
            this.label22.Text = "Pagefile Usage";
            // 
            // label20
            // 
            this.label20.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(3, 83);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(87, 13);
            this.label20.TabIndex = 3;
            this.label20.Text = "Peak Virtual Size";
            // 
            // label11
            // 
            this.label11.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(3, 3);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(69, 13);
            this.label11.TabIndex = 1;
            this.label11.Text = "Private Bytes";
            // 
            // label12
            // 
            this.label12.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(3, 23);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(66, 13);
            this.label12.TabIndex = 1;
            this.label12.Text = "Working Set";
            // 
            // label13
            // 
            this.label13.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(3, 43);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(94, 13);
            this.label13.TabIndex = 1;
            this.label13.Text = "Peak Working Set";
            // 
            // label14
            // 
            this.label14.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(3, 63);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(59, 13);
            this.label14.TabIndex = 1;
            this.label14.Text = "Virtual Size";
            // 
            // labelMemoryPB
            // 
            this.labelMemoryPB.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelMemoryPB.AutoSize = true;
            this.labelMemoryPB.Location = new System.Drawing.Point(153, 3);
            this.labelMemoryPB.Name = "labelMemoryPB";
            this.labelMemoryPB.Size = new System.Drawing.Size(33, 13);
            this.labelMemoryPB.TabIndex = 1;
            this.labelMemoryPB.Text = "value";
            // 
            // labelMemoryWS
            // 
            this.labelMemoryWS.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelMemoryWS.AutoSize = true;
            this.labelMemoryWS.Location = new System.Drawing.Point(153, 23);
            this.labelMemoryWS.Name = "labelMemoryWS";
            this.labelMemoryWS.Size = new System.Drawing.Size(33, 13);
            this.labelMemoryWS.TabIndex = 1;
            this.labelMemoryWS.Text = "value";
            // 
            // labelMemoryPWS
            // 
            this.labelMemoryPWS.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelMemoryPWS.AutoSize = true;
            this.labelMemoryPWS.Location = new System.Drawing.Point(153, 43);
            this.labelMemoryPWS.Name = "labelMemoryPWS";
            this.labelMemoryPWS.Size = new System.Drawing.Size(33, 13);
            this.labelMemoryPWS.TabIndex = 1;
            this.labelMemoryPWS.Text = "value";
            // 
            // labelMemoryVS
            // 
            this.labelMemoryVS.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelMemoryVS.AutoSize = true;
            this.labelMemoryVS.Location = new System.Drawing.Point(153, 63);
            this.labelMemoryVS.Name = "labelMemoryVS";
            this.labelMemoryVS.Size = new System.Drawing.Size(33, 13);
            this.labelMemoryVS.TabIndex = 1;
            this.labelMemoryVS.Text = "value";
            // 
            // labelMemoryPVS
            // 
            this.labelMemoryPVS.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelMemoryPVS.AutoSize = true;
            this.labelMemoryPVS.Location = new System.Drawing.Point(153, 83);
            this.labelMemoryPVS.Name = "labelMemoryPVS";
            this.labelMemoryPVS.Size = new System.Drawing.Size(33, 13);
            this.labelMemoryPVS.TabIndex = 1;
            this.labelMemoryPVS.Text = "value";
            // 
            // labelMemoryPU
            // 
            this.labelMemoryPU.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelMemoryPU.AutoSize = true;
            this.labelMemoryPU.Location = new System.Drawing.Point(153, 103);
            this.labelMemoryPU.Name = "labelMemoryPU";
            this.labelMemoryPU.Size = new System.Drawing.Size(33, 13);
            this.labelMemoryPU.TabIndex = 1;
            this.labelMemoryPU.Text = "value";
            // 
            // labelMemoryPPU
            // 
            this.labelMemoryPPU.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelMemoryPPU.AutoSize = true;
            this.labelMemoryPPU.Location = new System.Drawing.Point(153, 123);
            this.labelMemoryPPU.Name = "labelMemoryPPU";
            this.labelMemoryPPU.Size = new System.Drawing.Size(33, 13);
            this.labelMemoryPPU.TabIndex = 1;
            this.labelMemoryPPU.Text = "value";
            // 
            // label25
            // 
            this.label25.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(3, 144);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(63, 13);
            this.label25.TabIndex = 7;
            this.label25.Text = "Page Faults";
            // 
            // labelMemoryPF
            // 
            this.labelMemoryPF.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelMemoryPF.AutoSize = true;
            this.labelMemoryPF.Location = new System.Drawing.Point(153, 144);
            this.labelMemoryPF.Name = "labelMemoryPF";
            this.labelMemoryPF.Size = new System.Drawing.Size(33, 13);
            this.labelMemoryPF.TabIndex = 1;
            this.labelMemoryPF.Text = "value";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.tableLayoutPanel3);
            this.groupBox5.Location = new System.Drawing.Point(204, 3);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(195, 146);
            this.groupBox5.TabIndex = 0;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "I/O";
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.Controls.Add(this.label16, 0, 5);
            this.tableLayoutPanel3.Controls.Add(this.label17, 0, 4);
            this.tableLayoutPanel3.Controls.Add(this.label18, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.label19, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.label21, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.label23, 0, 3);
            this.tableLayoutPanel3.Controls.Add(this.labelIOReads, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.labelIOReadBytes, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.labelIOWrites, 1, 2);
            this.tableLayoutPanel3.Controls.Add(this.labelIOWriteBytes, 1, 3);
            this.tableLayoutPanel3.Controls.Add(this.labelIOOther, 1, 4);
            this.tableLayoutPanel3.Controls.Add(this.labelIOOtherBytes, 1, 5);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 6;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(189, 127);
            this.tableLayoutPanel3.TabIndex = 1;
            // 
            // label16
            // 
            this.label16.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(3, 109);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(62, 13);
            this.label16.TabIndex = 5;
            this.label16.Text = "Other Bytes";
            // 
            // label17
            // 
            this.label17.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(3, 88);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(33, 13);
            this.label17.TabIndex = 3;
            this.label17.Text = "Other";
            // 
            // label18
            // 
            this.label18.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(3, 4);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(38, 13);
            this.label18.TabIndex = 1;
            this.label18.Text = "Reads";
            // 
            // label19
            // 
            this.label19.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(3, 25);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(62, 13);
            this.label19.TabIndex = 1;
            this.label19.Text = "Read Bytes";
            // 
            // label21
            // 
            this.label21.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(3, 46);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(37, 13);
            this.label21.TabIndex = 1;
            this.label21.Text = "Writes";
            // 
            // label23
            // 
            this.label23.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(3, 67);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(61, 13);
            this.label23.TabIndex = 1;
            this.label23.Text = "Write Bytes";
            // 
            // labelIOReads
            // 
            this.labelIOReads.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelIOReads.AutoSize = true;
            this.labelIOReads.Location = new System.Drawing.Point(153, 4);
            this.labelIOReads.Name = "labelIOReads";
            this.labelIOReads.Size = new System.Drawing.Size(33, 13);
            this.labelIOReads.TabIndex = 1;
            this.labelIOReads.Text = "value";
            // 
            // labelIOReadBytes
            // 
            this.labelIOReadBytes.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelIOReadBytes.AutoSize = true;
            this.labelIOReadBytes.Location = new System.Drawing.Point(153, 25);
            this.labelIOReadBytes.Name = "labelIOReadBytes";
            this.labelIOReadBytes.Size = new System.Drawing.Size(33, 13);
            this.labelIOReadBytes.TabIndex = 1;
            this.labelIOReadBytes.Text = "value";
            // 
            // labelIOWrites
            // 
            this.labelIOWrites.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelIOWrites.AutoSize = true;
            this.labelIOWrites.Location = new System.Drawing.Point(153, 46);
            this.labelIOWrites.Name = "labelIOWrites";
            this.labelIOWrites.Size = new System.Drawing.Size(33, 13);
            this.labelIOWrites.TabIndex = 1;
            this.labelIOWrites.Text = "value";
            // 
            // labelIOWriteBytes
            // 
            this.labelIOWriteBytes.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelIOWriteBytes.AutoSize = true;
            this.labelIOWriteBytes.Location = new System.Drawing.Point(153, 67);
            this.labelIOWriteBytes.Name = "labelIOWriteBytes";
            this.labelIOWriteBytes.Size = new System.Drawing.Size(33, 13);
            this.labelIOWriteBytes.TabIndex = 1;
            this.labelIOWriteBytes.Text = "value";
            // 
            // labelIOOther
            // 
            this.labelIOOther.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelIOOther.AutoSize = true;
            this.labelIOOther.Location = new System.Drawing.Point(153, 88);
            this.labelIOOther.Name = "labelIOOther";
            this.labelIOOther.Size = new System.Drawing.Size(33, 13);
            this.labelIOOther.TabIndex = 1;
            this.labelIOOther.Text = "value";
            // 
            // labelIOOtherBytes
            // 
            this.labelIOOtherBytes.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelIOOtherBytes.AutoSize = true;
            this.labelIOOtherBytes.Location = new System.Drawing.Point(153, 109);
            this.labelIOOtherBytes.Name = "labelIOOtherBytes";
            this.labelIOOtherBytes.Size = new System.Drawing.Size(33, 13);
            this.labelIOOtherBytes.TabIndex = 1;
            this.labelIOOtherBytes.Text = "value";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.tableLayoutPanel4);
            this.groupBox6.Location = new System.Drawing.Point(204, 155);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(195, 80);
            this.groupBox6.TabIndex = 0;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Other";
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 2;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel4.Controls.Add(this.label27, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.labelOtherHandles, 1, 0);
            this.tableLayoutPanel4.Controls.Add(this.label28, 0, 1);
            this.tableLayoutPanel4.Controls.Add(this.label29, 0, 2);
            this.tableLayoutPanel4.Controls.Add(this.labelOtherGDIHandles, 1, 1);
            this.tableLayoutPanel4.Controls.Add(this.labelOtherUSERHandles, 1, 2);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 3;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(189, 61);
            this.tableLayoutPanel4.TabIndex = 1;
            // 
            // label27
            // 
            this.label27.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(3, 3);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(46, 13);
            this.label27.TabIndex = 1;
            this.label27.Text = "Handles";
            // 
            // labelOtherHandles
            // 
            this.labelOtherHandles.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelOtherHandles.AutoSize = true;
            this.labelOtherHandles.Location = new System.Drawing.Point(153, 3);
            this.labelOtherHandles.Name = "labelOtherHandles";
            this.labelOtherHandles.Size = new System.Drawing.Size(33, 13);
            this.labelOtherHandles.TabIndex = 1;
            this.labelOtherHandles.Text = "value";
            // 
            // label28
            // 
            this.label28.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(3, 23);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(68, 13);
            this.label28.TabIndex = 1;
            this.label28.Text = "GDI Handles";
            // 
            // label29
            // 
            this.label29.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(3, 44);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(79, 13);
            this.label29.TabIndex = 1;
            this.label29.Text = "USER Handles";
            // 
            // labelOtherGDIHandles
            // 
            this.labelOtherGDIHandles.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelOtherGDIHandles.AutoSize = true;
            this.labelOtherGDIHandles.Location = new System.Drawing.Point(153, 23);
            this.labelOtherGDIHandles.Name = "labelOtherGDIHandles";
            this.labelOtherGDIHandles.Size = new System.Drawing.Size(33, 13);
            this.labelOtherGDIHandles.TabIndex = 1;
            this.labelOtherGDIHandles.Text = "value";
            // 
            // labelOtherUSERHandles
            // 
            this.labelOtherUSERHandles.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelOtherUSERHandles.AutoSize = true;
            this.labelOtherUSERHandles.Location = new System.Drawing.Point(153, 44);
            this.labelOtherUSERHandles.Name = "labelOtherUSERHandles";
            this.labelOtherUSERHandles.Size = new System.Drawing.Size(33, 13);
            this.labelOtherUSERHandles.TabIndex = 1;
            this.labelOtherUSERHandles.Text = "value";
            // 
            // tabPerformance
            // 
            this.tabPerformance.Controls.Add(this.tablePerformance);
            this.tabPerformance.ImageKey = "chart_pie";
            this.tabPerformance.Location = new System.Drawing.Point(4, 42);
            this.tabPerformance.Name = "tabPerformance";
            this.tabPerformance.Padding = new System.Windows.Forms.Padding(3);
            this.tabPerformance.Size = new System.Drawing.Size(430, 351);
            this.tabPerformance.TabIndex = 8;
            this.tabPerformance.Text = "Performance";
            this.tabPerformance.UseVisualStyleBackColor = true;
            // 
            // tablePerformance
            // 
            this.tablePerformance.ColumnCount = 1;
            this.tablePerformance.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tablePerformance.Controls.Add(this.groupCPUUsage, 0, 0);
            this.tablePerformance.Controls.Add(this.groupBox3, 0, 2);
            this.tablePerformance.Controls.Add(this.groupBox2, 0, 1);
            this.tablePerformance.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tablePerformance.Location = new System.Drawing.Point(3, 3);
            this.tablePerformance.Name = "tablePerformance";
            this.tablePerformance.RowCount = 3;
            this.tablePerformance.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tablePerformance.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tablePerformance.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tablePerformance.Size = new System.Drawing.Size(424, 345);
            this.tablePerformance.TabIndex = 1;
            // 
            // groupCPUUsage
            // 
            this.groupCPUUsage.Controls.Add(this.plotterCPUUsage);
            this.groupCPUUsage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupCPUUsage.Location = new System.Drawing.Point(3, 3);
            this.groupCPUUsage.Name = "groupCPUUsage";
            this.groupCPUUsage.Size = new System.Drawing.Size(418, 109);
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
            this.plotterCPUUsage.Name = "plotterCPUUsage";
            this.plotterCPUUsage.OverlaySecondLine = false;
            this.plotterCPUUsage.ShowGrid = true;
            this.plotterCPUUsage.Size = new System.Drawing.Size(412, 90);
            this.plotterCPUUsage.TabIndex = 0;
            this.plotterCPUUsage.TextBoxColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.plotterCPUUsage.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))));
            this.plotterCPUUsage.TextMargin = new System.Windows.Forms.Padding(3);
            this.plotterCPUUsage.TextPadding = new System.Windows.Forms.Padding(3);
            this.plotterCPUUsage.TextPosition = System.Drawing.ContentAlignment.TopLeft;
            this.plotterCPUUsage.UseLongData = false;
            this.plotterCPUUsage.UseSecondLine = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.plotterIO);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(3, 233);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(418, 109);
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
            this.plotterIO.Name = "plotterIO";
            this.plotterIO.OverlaySecondLine = true;
            this.plotterIO.ShowGrid = true;
            this.plotterIO.Size = new System.Drawing.Size(412, 90);
            this.plotterIO.TabIndex = 0;
            this.plotterIO.TextBoxColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.plotterIO.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))));
            this.plotterIO.TextMargin = new System.Windows.Forms.Padding(3);
            this.plotterIO.TextPadding = new System.Windows.Forms.Padding(3);
            this.plotterIO.TextPosition = System.Drawing.ContentAlignment.TopLeft;
            this.plotterIO.UseLongData = true;
            this.plotterIO.UseSecondLine = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.plotterMemory);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(3, 118);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(418, 109);
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
            this.plotterMemory.Name = "plotterMemory";
            this.plotterMemory.OverlaySecondLine = true;
            this.plotterMemory.ShowGrid = true;
            this.plotterMemory.Size = new System.Drawing.Size(412, 90);
            this.plotterMemory.TabIndex = 0;
            this.plotterMemory.TextBoxColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.plotterMemory.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))));
            this.plotterMemory.TextMargin = new System.Windows.Forms.Padding(3);
            this.plotterMemory.TextPadding = new System.Windows.Forms.Padding(3);
            this.plotterMemory.TextPosition = System.Drawing.ContentAlignment.TopLeft;
            this.plotterMemory.UseLongData = true;
            this.plotterMemory.UseSecondLine = true;
            // 
            // tabThreads
            // 
            this.tabThreads.Controls.Add(this.listThreads);
            this.tabThreads.ImageKey = "hourglass";
            this.tabThreads.Location = new System.Drawing.Point(4, 42);
            this.tabThreads.Name = "tabThreads";
            this.tabThreads.Size = new System.Drawing.Size(430, 351);
            this.tabThreads.TabIndex = 3;
            this.tabThreads.Text = "Threads";
            this.tabThreads.UseVisualStyleBackColor = true;
            // 
            // listThreads
            // 
            this.listThreads.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listThreads.DoubleBuffered = true;
            this.listThreads.Highlight = false;
            this.listThreads.Location = new System.Drawing.Point(0, 0);
            this.listThreads.Name = "listThreads";
            this.listThreads.Provider = null;
            this.listThreads.Size = new System.Drawing.Size(430, 351);
            this.listThreads.TabIndex = 0;
            // 
            // tabToken
            // 
            this.tabToken.ImageKey = "token";
            this.tabToken.Location = new System.Drawing.Point(4, 42);
            this.tabToken.Name = "tabToken";
            this.tabToken.Padding = new System.Windows.Forms.Padding(3);
            this.tabToken.Size = new System.Drawing.Size(430, 351);
            this.tabToken.TabIndex = 1;
            this.tabToken.Text = "Token";
            this.tabToken.UseVisualStyleBackColor = true;
            // 
            // tabModules
            // 
            this.tabModules.Controls.Add(this.listModules);
            this.tabModules.ImageKey = "page_white_wrench";
            this.tabModules.Location = new System.Drawing.Point(4, 42);
            this.tabModules.Name = "tabModules";
            this.tabModules.Size = new System.Drawing.Size(430, 351);
            this.tabModules.TabIndex = 6;
            this.tabModules.Text = "Modules";
            this.tabModules.UseVisualStyleBackColor = true;
            // 
            // listModules
            // 
            this.listModules.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listModules.DoubleBuffered = true;
            this.listModules.Highlight = false;
            this.listModules.Location = new System.Drawing.Point(0, 0);
            this.listModules.Name = "listModules";
            this.listModules.Provider = null;
            this.listModules.Size = new System.Drawing.Size(430, 351);
            this.listModules.TabIndex = 0;
            // 
            // tabMemory
            // 
            this.tabMemory.Controls.Add(this.label15);
            this.tabMemory.Controls.Add(this.checkHideFreeRegions);
            this.tabMemory.Controls.Add(this.buttonSearch);
            this.tabMemory.Controls.Add(this.listMemory);
            this.tabMemory.ImageKey = "database";
            this.tabMemory.Location = new System.Drawing.Point(4, 42);
            this.tabMemory.Name = "tabMemory";
            this.tabMemory.Padding = new System.Windows.Forms.Padding(3);
            this.tabMemory.Size = new System.Drawing.Size(430, 351);
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
            this.buttonSearch.Location = new System.Drawing.Point(58, 6);
            this.buttonSearch.Name = "buttonSearch";
            this.buttonSearch.Size = new System.Drawing.Size(99, 23);
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
            this.listMemory.Highlight = false;
            this.listMemory.Location = new System.Drawing.Point(6, 59);
            this.listMemory.Name = "listMemory";
            this.listMemory.Provider = null;
            this.listMemory.Size = new System.Drawing.Size(418, 288);
            this.listMemory.TabIndex = 0;
            // 
            // tabEnvironment
            // 
            this.tabEnvironment.Controls.Add(this.listEnvironment);
            this.tabEnvironment.ImageKey = "environment";
            this.tabEnvironment.Location = new System.Drawing.Point(4, 42);
            this.tabEnvironment.Name = "tabEnvironment";
            this.tabEnvironment.Padding = new System.Windows.Forms.Padding(3);
            this.tabEnvironment.Size = new System.Drawing.Size(430, 351);
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
            this.listEnvironment.Size = new System.Drawing.Size(424, 345);
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
            this.tabHandles.ImageKey = "connect";
            this.tabHandles.Location = new System.Drawing.Point(4, 42);
            this.tabHandles.Name = "tabHandles";
            this.tabHandles.Padding = new System.Windows.Forms.Padding(3);
            this.tabHandles.Size = new System.Drawing.Size(430, 351);
            this.tabHandles.TabIndex = 5;
            this.tabHandles.Text = "Handles";
            this.tabHandles.UseVisualStyleBackColor = true;
            // 
            // checkHideHandlesNoName
            // 
            this.checkHideHandlesNoName.AutoSize = true;
            this.checkHideHandlesNoName.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkHideHandlesNoName.Location = new System.Drawing.Point(6, 6);
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
            this.listHandles.Highlight = false;
            this.listHandles.Location = new System.Drawing.Point(6, 30);
            this.listHandles.Name = "listHandles";
            this.listHandles.Provider = null;
            this.listHandles.Size = new System.Drawing.Size(418, 317);
            this.listHandles.TabIndex = 0;
            // 
            // tabServices
            // 
            this.tabServices.ImageKey = "cog";
            this.tabServices.Location = new System.Drawing.Point(4, 42);
            this.tabServices.Name = "tabServices";
            this.tabServices.Size = new System.Drawing.Size(430, 351);
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
            this.imageList.Images.SetKeyName(7, "chart_pie");
            this.imageList.Images.SetKeyName(8, "chart_bar");
            this.imageList.Images.SetKeyName(9, "environment");
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
            // 
            // ProcessWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(438, 397);
            this.Controls.Add(this.tabControl);
            this.Menu = this.mainMenu;
            this.MinimumSize = new System.Drawing.Size(454, 433);
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
            this.tabStatistics.ResumeLayout(false);
            this.flowStats.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.tabPerformance.ResumeLayout(false);
            this.tablePerformance.ResumeLayout(false);
            this.groupCPUUsage.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
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
        private System.Windows.Forms.GroupBox groupFile;
        private System.Windows.Forms.GroupBox groupProcess;
        private System.Windows.Forms.TabPage tabServices;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private ModuleList listModules;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textParent;
        private HandleList listHandles;
        private MemoryList listMemory;
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
        private System.Windows.Forms.FlowLayoutPanel flowStats;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label labelCPUPriority;
        private System.Windows.Forms.Label labelCPUKernelTime;
        private System.Windows.Forms.Label labelCPUUserTime;
        private System.Windows.Forms.Label labelCPUTotalTime;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label labelMemoryPB;
        private System.Windows.Forms.Label labelMemoryWS;
        private System.Windows.Forms.Label labelMemoryPWS;
        private System.Windows.Forms.Label labelMemoryVS;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label labelMemoryPVS;
        private System.Windows.Forms.Label labelMemoryPU;
        private System.Windows.Forms.Label labelMemoryPPU;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label labelMemoryPF;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label labelIOReads;
        private System.Windows.Forms.Label labelIOReadBytes;
        private System.Windows.Forms.Label labelIOWrites;
        private System.Windows.Forms.Label labelIOWriteBytes;
        private System.Windows.Forms.Label labelIOOther;
        private System.Windows.Forms.Label labelIOOtherBytes;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label labelOtherHandles;
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
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Label labelOtherGDIHandles;
        private System.Windows.Forms.Label labelOtherUSERHandles;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.TabPage tabEnvironment;
        private System.Windows.Forms.ListView listEnvironment;
        private System.Windows.Forms.ColumnHeader columnVarName;
        private System.Windows.Forms.ColumnHeader columnVarValue;
    }
}