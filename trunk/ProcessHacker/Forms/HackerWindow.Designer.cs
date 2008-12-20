namespace ProcessHacker
{
    partial class HackerWindow
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HackerWindow));
            this.panelProc = new System.Windows.Forms.Panel();
            this.buttonCloseProc = new System.Windows.Forms.Button();
            this.textProcAddress = new System.Windows.Forms.TextBox();
            this.buttonGetProcAddress = new System.Windows.Forms.Button();
            this.textProcName = new System.Windows.Forms.TextBox();
            this.labelProcedureName = new System.Windows.Forms.Label();
            this.timerFire = new System.Windows.Forms.Timer(this.components);
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.panelVirtualProtect = new System.Windows.Forms.Panel();
            this.buttonCloseVirtualProtect = new System.Windows.Forms.Button();
            this.buttonVirtualProtect = new System.Windows.Forms.Button();
            this.textNewProtection = new System.Windows.Forms.TextBox();
            this.labelNewValue = new System.Windows.Forms.Label();
            this.labelVirtualProtectInfo = new System.Windows.Forms.Label();
            this.menuProcess = new System.Windows.Forms.ContextMenu();
            this.terminateMenuItem = new System.Windows.Forms.MenuItem();
            this.suspendMenuItem = new System.Windows.Forms.MenuItem();
            this.resumeMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem5 = new System.Windows.Forms.MenuItem();
            this.inspectProcessMenuItem = new System.Windows.Forms.MenuItem();
            this.affinityProcessMenuItem = new System.Windows.Forms.MenuItem();
            this.servicesProcessMenuItem = new System.Windows.Forms.MenuItem();
            this.tokenMenuItem = new System.Windows.Forms.MenuItem();
            this.priorityMenuItem = new System.Windows.Forms.MenuItem();
            this.realTimeMenuItem = new System.Windows.Forms.MenuItem();
            this.highMenuItem = new System.Windows.Forms.MenuItem();
            this.aboveNormalMenuItem = new System.Windows.Forms.MenuItem();
            this.normalMenuItem = new System.Windows.Forms.MenuItem();
            this.belowNormalMenuItem = new System.Windows.Forms.MenuItem();
            this.idleMenuItem = new System.Windows.Forms.MenuItem();
            this.runAsProcessMenuItem = new System.Windows.Forms.MenuItem();
            this.launchAsUserProcessMenuItem = new System.Windows.Forms.MenuItem();
            this.launchAsThisUserProcessMenuItem = new System.Windows.Forms.MenuItem();
            this.injectorMenuItem = new System.Windows.Forms.MenuItem();
            this.startProcessProcessMenuItem = new System.Windows.Forms.MenuItem();
            this.getCommandLineProcessMenuItem = new System.Windows.Forms.MenuItem();
            this.exitProcessProcessMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem7 = new System.Windows.Forms.MenuItem();
            this.searchProcessMenuItem = new System.Windows.Forms.MenuItem();
            this.menuThread = new System.Windows.Forms.ContextMenu();
            this.inspectThreadMenuItem = new System.Windows.Forms.MenuItem();
            this.terminateThreadMenuItem = new System.Windows.Forms.MenuItem();
            this.suspendThreadMenuItem = new System.Windows.Forms.MenuItem();
            this.resumeThreadMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.priorityThreadMenuItem = new System.Windows.Forms.MenuItem();
            this.timeCriticalThreadMenuItem = new System.Windows.Forms.MenuItem();
            this.highestThreadMenuItem = new System.Windows.Forms.MenuItem();
            this.aboveNormalThreadMenuItem = new System.Windows.Forms.MenuItem();
            this.normalThreadMenuItem = new System.Windows.Forms.MenuItem();
            this.belowNormalThreadMenuItem = new System.Windows.Forms.MenuItem();
            this.lowestThreadMenuItem = new System.Windows.Forms.MenuItem();
            this.idleThreadMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem9 = new System.Windows.Forms.MenuItem();
            this.copyThreadMenuItem = new System.Windows.Forms.MenuItem();
            this.selectAllThreadMenuItem = new System.Windows.Forms.MenuItem();
            this.toolStripMenuItem9 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem10 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem11 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem12 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem13 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem14 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem15 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuModule = new System.Windows.Forms.ContextMenu();
            this.goToInMemoryViewModuleMenuItem = new System.Windows.Forms.MenuItem();
            this.getFuncAddressMenuItem = new System.Windows.Forms.MenuItem();
            this.changeMemoryProtectionModuleMenuItem = new System.Windows.Forms.MenuItem();
            this.readMemoryModuleMenuItem = new System.Windows.Forms.MenuItem();
            this.inspectModuleMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.searchModuleMenuItem = new System.Windows.Forms.MenuItem();
            this.copyFileNameMenuItem = new System.Windows.Forms.MenuItem();
            this.copyModuleMenuItem = new System.Windows.Forms.MenuItem();
            this.openContainingFolderMenuItem = new System.Windows.Forms.MenuItem();
            this.propertiesMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem6 = new System.Windows.Forms.MenuItem();
            this.selectAllModuleMenuItem = new System.Windows.Forms.MenuItem();
            this.menuMemory = new System.Windows.Forms.ContextMenu();
            this.changeMemoryProtectionMemoryMenuItem = new System.Windows.Forms.MenuItem();
            this.readWriteMemoryMemoryMenuItem = new System.Windows.Forms.MenuItem();
            this.readWriteAddressMemoryMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.copyMemoryMenuItem = new System.Windows.Forms.MenuItem();
            this.selectAllMemoryMenuItem = new System.Windows.Forms.MenuItem();
            this.menuSearch = new System.Windows.Forms.ContextMenu();
            this.newResultsWindowMenuItem = new System.Windows.Forms.MenuItem();
            this.literalSearchMenuItem = new System.Windows.Forms.MenuItem();
            this.regexSearchMenuItem = new System.Windows.Forms.MenuItem();
            this.stringScanMenuItem = new System.Windows.Forms.MenuItem();
            this.heapScanMenuItem = new System.Windows.Forms.MenuItem();
            this.mainMenu = new System.Windows.Forms.MainMenu(this.components);
            this.hackerMenuItem = new System.Windows.Forms.MenuItem();
            this.selectAllHackerMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.runAsMenuItem = new System.Windows.Forms.MenuItem();
            this.findHandlesMenuItem = new System.Windows.Forms.MenuItem();
            this.inspectPEFileMenuItem = new System.Windows.Forms.MenuItem();
            this.logMenuItem = new System.Windows.Forms.MenuItem();
            this.aboutMenuItem = new System.Windows.Forms.MenuItem();
            this.optionsMenuItem = new System.Windows.Forms.MenuItem();
            this.helpMenuItem = new System.Windows.Forms.MenuItem();
            this.exitMenuItem = new System.Windows.Forms.MenuItem();
            this.toolsMenuItem = new System.Windows.Forms.MenuItem();
            this.getSNFAMenuItem = new System.Windows.Forms.MenuItem();
            this.FSPWSSIDMenuItem = new System.Windows.Forms.MenuItem();
            this.windowMenuItem = new System.Windows.Forms.MenuItem();
            this.statusBar = new System.Windows.Forms.StatusBar();
            this.statusGeneral = new System.Windows.Forms.StatusBarPanel();
            this.statusIcon = new System.Windows.Forms.StatusBarPanel();
            this.statusText = new System.Windows.Forms.StatusBarPanel();
            this.timerMessages = new System.Windows.Forms.Timer(this.components);
            this.tabControlBig = new System.Windows.Forms.TabControl();
            this.tabProcesses = new System.Windows.Forms.TabPage();
            this.splitMain = new System.Windows.Forms.SplitContainer();
            this.treeProcesses = new ProcessHacker.ProcessTree();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabProcess = new System.Windows.Forms.TabPage();
            this.groupSearch = new System.Windows.Forms.GroupBox();
            this.buttonSearch = new wyDay.Controls.SplitButton();
            this.treeMisc = new System.Windows.Forms.TreeView();
            this.tabThreads = new System.Windows.Forms.TabPage();
            this.listThreads = new ProcessHacker.ThreadList();
            this.tabModules = new System.Windows.Forms.TabPage();
            this.listModules = new System.Windows.Forms.ListView();
            this.columnModuleName = new System.Windows.Forms.ColumnHeader();
            this.columnBaseAddress = new System.Windows.Forms.ColumnHeader();
            this.columnModuleSize = new System.Windows.Forms.ColumnHeader();
            this.columnDescription = new System.Windows.Forms.ColumnHeader();
            this.tabMemory = new System.Windows.Forms.TabPage();
            this.listMemory = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
            this.tabHandles = new System.Windows.Forms.TabPage();
            this.listHandles = new ProcessHacker.HandleList();
            this.tabServices = new System.Windows.Forms.TabPage();
            this.listServices = new ProcessHacker.ServiceList();
            this.menuService = new System.Windows.Forms.ContextMenu();
            this.goToProcessServiceMenuItem = new System.Windows.Forms.MenuItem();
            this.startServiceMenuItem = new System.Windows.Forms.MenuItem();
            this.continueServiceMenuItem = new System.Windows.Forms.MenuItem();
            this.pauseServiceMenuItem = new System.Windows.Forms.MenuItem();
            this.stopServiceMenuItem = new System.Windows.Forms.MenuItem();
            this.deleteServiceMenuItem = new System.Windows.Forms.MenuItem();
            this.propertiesServiceMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem8 = new System.Windows.Forms.MenuItem();
            this.copyServiceMenuItem = new System.Windows.Forms.MenuItem();
            this.selectAllServiceMenuItem = new System.Windows.Forms.MenuItem();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.menuIcon = new System.Windows.Forms.ContextMenu();
            this.showHideMenuItem = new System.Windows.Forms.MenuItem();
            this.notificationsMenuItem = new System.Windows.Forms.MenuItem();
            this.NPMenuItem = new System.Windows.Forms.MenuItem();
            this.TPMenuItem = new System.Windows.Forms.MenuItem();
            this.NSMenuItem = new System.Windows.Forms.MenuItem();
            this.startedSMenuItem = new System.Windows.Forms.MenuItem();
            this.stoppedSMenuItem = new System.Windows.Forms.MenuItem();
            this.DSMenuItem = new System.Windows.Forms.MenuItem();
            this.exitTrayMenuItem = new System.Windows.Forms.MenuItem();
            this.menuHandle = new System.Windows.Forms.ContextMenu();
            this.closeHandleMenuItem = new System.Windows.Forms.MenuItem();
            this.copyHandleMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem11 = new System.Windows.Forms.MenuItem();
            this.propertiesHandleMenuItem = new System.Windows.Forms.MenuItem();
            this.menuMisc = new System.Windows.Forms.ContextMenu();
            this.copyMiscMenuItem = new System.Windows.Forms.MenuItem();
            this.vistaMenu = new wyDay.Controls.VistaMenu(this.components);
            this.panelProc.SuspendLayout();
            this.panelVirtualProtect.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.statusGeneral)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusText)).BeginInit();
            this.tabControlBig.SuspendLayout();
            this.tabProcesses.SuspendLayout();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabProcess.SuspendLayout();
            this.groupSearch.SuspendLayout();
            this.tabThreads.SuspendLayout();
            this.tabModules.SuspendLayout();
            this.tabMemory.SuspendLayout();
            this.tabHandles.SuspendLayout();
            this.tabServices.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.vistaMenu)).BeginInit();
            this.SuspendLayout();
            // 
            // panelProc
            // 
            this.panelProc.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panelProc.BackColor = System.Drawing.SystemColors.Control;
            this.panelProc.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelProc.Controls.Add(this.buttonCloseProc);
            this.panelProc.Controls.Add(this.textProcAddress);
            this.panelProc.Controls.Add(this.buttonGetProcAddress);
            this.panelProc.Controls.Add(this.textProcName);
            this.panelProc.Controls.Add(this.labelProcedureName);
            this.panelProc.Location = new System.Drawing.Point(592, 30);
            this.panelProc.Name = "panelProc";
            this.panelProc.Padding = new System.Windows.Forms.Padding(3);
            this.panelProc.Size = new System.Drawing.Size(200, 100);
            this.panelProc.TabIndex = 3;
            this.panelProc.Visible = false;
            // 
            // buttonCloseProc
            // 
            this.buttonCloseProc.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonCloseProc.Location = new System.Drawing.Point(119, 45);
            this.buttonCloseProc.Name = "buttonCloseProc";
            this.buttonCloseProc.Size = new System.Drawing.Size(75, 23);
            this.buttonCloseProc.TabIndex = 4;
            this.buttonCloseProc.Text = "&Close";
            this.buttonCloseProc.UseVisualStyleBackColor = true;
            this.buttonCloseProc.Click += new System.EventHandler(this.buttonCloseProc_Click);
            // 
            // textProcAddress
            // 
            this.textProcAddress.Location = new System.Drawing.Point(6, 74);
            this.textProcAddress.Name = "textProcAddress";
            this.textProcAddress.ReadOnly = true;
            this.textProcAddress.Size = new System.Drawing.Size(188, 20);
            this.textProcAddress.TabIndex = 3;
            // 
            // buttonGetProcAddress
            // 
            this.buttonGetProcAddress.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonGetProcAddress.Location = new System.Drawing.Point(6, 45);
            this.buttonGetProcAddress.Name = "buttonGetProcAddress";
            this.buttonGetProcAddress.Size = new System.Drawing.Size(75, 23);
            this.buttonGetProcAddress.TabIndex = 2;
            this.buttonGetProcAddress.Text = "Get Address";
            this.buttonGetProcAddress.UseVisualStyleBackColor = true;
            this.buttonGetProcAddress.Click += new System.EventHandler(this.buttonGetProcAddress_Click);
            // 
            // textProcName
            // 
            this.textProcName.Location = new System.Drawing.Point(6, 19);
            this.textProcName.Name = "textProcName";
            this.textProcName.Size = new System.Drawing.Size(188, 20);
            this.textProcName.TabIndex = 1;
            // 
            // labelProcedureName
            // 
            this.labelProcedureName.AutoSize = true;
            this.labelProcedureName.Location = new System.Drawing.Point(6, 3);
            this.labelProcedureName.Name = "labelProcedureName";
            this.labelProcedureName.Size = new System.Drawing.Size(120, 13);
            this.labelProcedureName.TabIndex = 0;
            this.labelProcedureName.Text = "Function Name/Ordinal:";
            // 
            // timerFire
            // 
            this.timerFire.Interval = 250;
            this.timerFire.Tick += new System.EventHandler(this.timerFire_Tick);
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "process_small");
            // 
            // panelVirtualProtect
            // 
            this.panelVirtualProtect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panelVirtualProtect.BackColor = System.Drawing.SystemColors.Control;
            this.panelVirtualProtect.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelVirtualProtect.Controls.Add(this.buttonCloseVirtualProtect);
            this.panelVirtualProtect.Controls.Add(this.buttonVirtualProtect);
            this.panelVirtualProtect.Controls.Add(this.textNewProtection);
            this.panelVirtualProtect.Controls.Add(this.labelNewValue);
            this.panelVirtualProtect.Controls.Add(this.labelVirtualProtectInfo);
            this.panelVirtualProtect.Location = new System.Drawing.Point(467, 30);
            this.panelVirtualProtect.Name = "panelVirtualProtect";
            this.panelVirtualProtect.Padding = new System.Windows.Forms.Padding(3);
            this.panelVirtualProtect.Size = new System.Drawing.Size(325, 208);
            this.panelVirtualProtect.TabIndex = 4;
            this.panelVirtualProtect.Visible = false;
            // 
            // buttonCloseVirtualProtect
            // 
            this.buttonCloseVirtualProtect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCloseVirtualProtect.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonCloseVirtualProtect.Location = new System.Drawing.Point(242, 177);
            this.buttonCloseVirtualProtect.Name = "buttonCloseVirtualProtect";
            this.buttonCloseVirtualProtect.Size = new System.Drawing.Size(75, 23);
            this.buttonCloseVirtualProtect.TabIndex = 5;
            this.buttonCloseVirtualProtect.Text = "Close";
            this.buttonCloseVirtualProtect.UseVisualStyleBackColor = true;
            this.buttonCloseVirtualProtect.Click += new System.EventHandler(this.buttonCloseVirtualProtect_Click);
            // 
            // buttonVirtualProtect
            // 
            this.buttonVirtualProtect.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonVirtualProtect.Location = new System.Drawing.Point(161, 177);
            this.buttonVirtualProtect.Name = "buttonVirtualProtect";
            this.buttonVirtualProtect.Size = new System.Drawing.Size(75, 23);
            this.buttonVirtualProtect.TabIndex = 4;
            this.buttonVirtualProtect.Text = "&Change";
            this.buttonVirtualProtect.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonVirtualProtect.UseVisualStyleBackColor = true;
            this.buttonVirtualProtect.Click += new System.EventHandler(this.buttonVirtualProtect_Click);
            // 
            // textNewProtection
            // 
            this.textNewProtection.Location = new System.Drawing.Point(73, 179);
            this.textNewProtection.Name = "textNewProtection";
            this.textNewProtection.Size = new System.Drawing.Size(82, 20);
            this.textNewProtection.TabIndex = 3;
            // 
            // labelNewValue
            // 
            this.labelNewValue.AutoSize = true;
            this.labelNewValue.Location = new System.Drawing.Point(6, 182);
            this.labelNewValue.Name = "labelNewValue";
            this.labelNewValue.Size = new System.Drawing.Size(61, 13);
            this.labelNewValue.TabIndex = 2;
            this.labelNewValue.Text = "New value:";
            // 
            // labelVirtualProtectInfo
            // 
            this.labelVirtualProtectInfo.Location = new System.Drawing.Point(6, 3);
            this.labelVirtualProtectInfo.Name = "labelVirtualProtectInfo";
            this.labelVirtualProtectInfo.Size = new System.Drawing.Size(313, 165);
            this.labelVirtualProtectInfo.TabIndex = 0;
            this.labelVirtualProtectInfo.Text = resources.GetString("labelVirtualProtectInfo.Text");
            // 
            // menuProcess
            // 
            this.menuProcess.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.terminateMenuItem,
            this.suspendMenuItem,
            this.resumeMenuItem,
            this.menuItem5,
            this.inspectProcessMenuItem,
            this.affinityProcessMenuItem,
            this.servicesProcessMenuItem,
            this.tokenMenuItem,
            this.priorityMenuItem,
            this.runAsProcessMenuItem,
            this.injectorMenuItem,
            this.menuItem7,
            this.searchProcessMenuItem});
            this.menuProcess.Popup += new System.EventHandler(this.menuProcess_Popup);
            // 
            // terminateMenuItem
            // 
            this.vistaMenu.SetImage(this.terminateMenuItem, global::ProcessHacker.Properties.Resources.cross);
            this.terminateMenuItem.Index = 0;
            this.terminateMenuItem.Shortcut = System.Windows.Forms.Shortcut.Del;
            this.terminateMenuItem.Text = "&Terminate";
            this.terminateMenuItem.Click += new System.EventHandler(this.terminateMenuItem_Click);
            // 
            // suspendMenuItem
            // 
            this.vistaMenu.SetImage(this.suspendMenuItem, global::ProcessHacker.Properties.Resources.control_pause_blue);
            this.suspendMenuItem.Index = 1;
            this.suspendMenuItem.Text = "&Suspend";
            this.suspendMenuItem.Click += new System.EventHandler(this.suspendMenuItem_Click);
            // 
            // resumeMenuItem
            // 
            this.vistaMenu.SetImage(this.resumeMenuItem, global::ProcessHacker.Properties.Resources.control_play_blue);
            this.resumeMenuItem.Index = 2;
            this.resumeMenuItem.Text = "&Resume";
            this.resumeMenuItem.Click += new System.EventHandler(this.resumeMenuItem_Click);
            // 
            // menuItem5
            // 
            this.menuItem5.Index = 3;
            this.menuItem5.Text = "-";
            // 
            // inspectProcessMenuItem
            // 
            this.vistaMenu.SetImage(this.inspectProcessMenuItem, global::ProcessHacker.Properties.Resources.application_form_magnify);
            this.inspectProcessMenuItem.Index = 4;
            this.inspectProcessMenuItem.Text = "&Inspect...";
            this.inspectProcessMenuItem.Click += new System.EventHandler(this.inspectProcessMenuItem_Click);
            // 
            // affinityProcessMenuItem
            // 
            this.affinityProcessMenuItem.Index = 5;
            this.affinityProcessMenuItem.Text = "Affinity...";
            this.affinityProcessMenuItem.Click += new System.EventHandler(this.affinityProcessMenuItem_Click);
            // 
            // servicesProcessMenuItem
            // 
            this.vistaMenu.SetImage(this.servicesProcessMenuItem, global::ProcessHacker.Properties.Resources.cog);
            this.servicesProcessMenuItem.Index = 6;
            this.servicesProcessMenuItem.Text = "Services...";
            this.servicesProcessMenuItem.Click += new System.EventHandler(this.servicesProcessMenuItem_Click);
            // 
            // tokenMenuItem
            // 
            this.vistaMenu.SetImage(this.tokenMenuItem, global::ProcessHacker.Properties.Resources.lock_edit);
            this.tokenMenuItem.Index = 7;
            this.tokenMenuItem.Text = "To&ken...";
            this.tokenMenuItem.Click += new System.EventHandler(this.tokenMenuItem_Click);
            // 
            // priorityMenuItem
            // 
            this.vistaMenu.SetImage(this.priorityMenuItem, global::ProcessHacker.Properties.Resources.control_equalizer_blue);
            this.priorityMenuItem.Index = 8;
            this.priorityMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.realTimeMenuItem,
            this.highMenuItem,
            this.aboveNormalMenuItem,
            this.normalMenuItem,
            this.belowNormalMenuItem,
            this.idleMenuItem});
            this.priorityMenuItem.Text = "&Priority";
            // 
            // realTimeMenuItem
            // 
            this.realTimeMenuItem.Index = 0;
            this.realTimeMenuItem.RadioCheck = true;
            this.realTimeMenuItem.Text = "Real Time";
            this.realTimeMenuItem.Click += new System.EventHandler(this.realTimeMenuItem_Click);
            // 
            // highMenuItem
            // 
            this.highMenuItem.Index = 1;
            this.highMenuItem.RadioCheck = true;
            this.highMenuItem.Text = "High";
            this.highMenuItem.Click += new System.EventHandler(this.highMenuItem_Click);
            // 
            // aboveNormalMenuItem
            // 
            this.aboveNormalMenuItem.Index = 2;
            this.aboveNormalMenuItem.RadioCheck = true;
            this.aboveNormalMenuItem.Text = "Above Normal";
            this.aboveNormalMenuItem.Click += new System.EventHandler(this.aboveNormalMenuItem_Click);
            // 
            // normalMenuItem
            // 
            this.normalMenuItem.Index = 3;
            this.normalMenuItem.RadioCheck = true;
            this.normalMenuItem.Text = "Normal";
            this.normalMenuItem.Click += new System.EventHandler(this.normalMenuItem_Click);
            // 
            // belowNormalMenuItem
            // 
            this.belowNormalMenuItem.Index = 4;
            this.belowNormalMenuItem.RadioCheck = true;
            this.belowNormalMenuItem.Text = "Below Normal";
            this.belowNormalMenuItem.Click += new System.EventHandler(this.belowNormalMenuItem_Click);
            // 
            // idleMenuItem
            // 
            this.idleMenuItem.Index = 5;
            this.idleMenuItem.RadioCheck = true;
            this.idleMenuItem.Text = "Idle";
            this.idleMenuItem.Click += new System.EventHandler(this.idleMenuItem_Click);
            // 
            // runAsProcessMenuItem
            // 
            this.runAsProcessMenuItem.Index = 9;
            this.runAsProcessMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.launchAsUserProcessMenuItem,
            this.launchAsThisUserProcessMenuItem});
            this.runAsProcessMenuItem.Text = "Run As";
            // 
            // launchAsUserProcessMenuItem
            // 
            this.launchAsUserProcessMenuItem.Index = 0;
            this.launchAsUserProcessMenuItem.Text = "Launch As User...";
            this.launchAsUserProcessMenuItem.Click += new System.EventHandler(this.launchAsUserProcessMenuItem_Click);
            // 
            // launchAsThisUserProcessMenuItem
            // 
            this.launchAsThisUserProcessMenuItem.Index = 1;
            this.launchAsThisUserProcessMenuItem.Text = "Launch As This User...";
            this.launchAsThisUserProcessMenuItem.Click += new System.EventHandler(this.launchAsThisUserProcessMenuItem_Click);
            // 
            // injectorMenuItem
            // 
            this.vistaMenu.SetImage(this.injectorMenuItem, global::ProcessHacker.Properties.Resources.asterisk_orange);
            this.injectorMenuItem.Index = 10;
            this.injectorMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.startProcessProcessMenuItem,
            this.getCommandLineProcessMenuItem,
            this.exitProcessProcessMenuItem});
            this.injectorMenuItem.Text = "Injector";
            // 
            // startProcessProcessMenuItem
            // 
            this.startProcessProcessMenuItem.Index = 0;
            this.startProcessProcessMenuItem.Text = "Start Process...";
            this.startProcessProcessMenuItem.Click += new System.EventHandler(this.startProcessProcessMenuItem_Click);
            // 
            // getCommandLineProcessMenuItem
            // 
            this.getCommandLineProcessMenuItem.Index = 1;
            this.getCommandLineProcessMenuItem.Text = "Get Command Line";
            this.getCommandLineProcessMenuItem.Click += new System.EventHandler(this.getCommandLineProcessMenuItem_Click);
            // 
            // exitProcessProcessMenuItem
            // 
            this.exitProcessProcessMenuItem.Index = 2;
            this.exitProcessProcessMenuItem.Text = "Exit Process";
            this.exitProcessProcessMenuItem.Click += new System.EventHandler(this.exitProcessProcessMenuItem_Click);
            // 
            // menuItem7
            // 
            this.menuItem7.Index = 11;
            this.menuItem7.Text = "-";
            // 
            // searchProcessMenuItem
            // 
            this.searchProcessMenuItem.Index = 12;
            this.searchProcessMenuItem.Text = "&Search Online...";
            this.searchProcessMenuItem.Click += new System.EventHandler(this.searchProcessMenuItem_Click);
            // 
            // menuThread
            // 
            this.menuThread.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.inspectThreadMenuItem,
            this.terminateThreadMenuItem,
            this.suspendThreadMenuItem,
            this.resumeThreadMenuItem,
            this.menuItem4,
            this.priorityThreadMenuItem,
            this.menuItem9,
            this.copyThreadMenuItem,
            this.selectAllThreadMenuItem});
            this.menuThread.Popup += new System.EventHandler(this.menuThread_Popup);
            // 
            // inspectThreadMenuItem
            // 
            this.inspectThreadMenuItem.DefaultItem = true;
            this.vistaMenu.SetImage(this.inspectThreadMenuItem, global::ProcessHacker.Properties.Resources.application_form_magnify);
            this.inspectThreadMenuItem.Index = 0;
            this.inspectThreadMenuItem.Text = "&Inspect";
            this.inspectThreadMenuItem.Click += new System.EventHandler(this.inspectThreadMenuItem_Click);
            // 
            // terminateThreadMenuItem
            // 
            this.vistaMenu.SetImage(this.terminateThreadMenuItem, global::ProcessHacker.Properties.Resources.cross);
            this.terminateThreadMenuItem.Index = 1;
            this.terminateThreadMenuItem.Text = "&Terminate";
            this.terminateThreadMenuItem.Click += new System.EventHandler(this.terminateThreadMenuItem_Click);
            // 
            // suspendThreadMenuItem
            // 
            this.vistaMenu.SetImage(this.suspendThreadMenuItem, global::ProcessHacker.Properties.Resources.control_pause_blue);
            this.suspendThreadMenuItem.Index = 2;
            this.suspendThreadMenuItem.Text = "&Suspend";
            this.suspendThreadMenuItem.Click += new System.EventHandler(this.suspendThreadMenuItem_Click);
            // 
            // resumeThreadMenuItem
            // 
            this.vistaMenu.SetImage(this.resumeThreadMenuItem, global::ProcessHacker.Properties.Resources.control_play_blue);
            this.resumeThreadMenuItem.Index = 3;
            this.resumeThreadMenuItem.Text = "&Resume";
            this.resumeThreadMenuItem.Click += new System.EventHandler(this.resumeThreadMenuItem_Click);
            // 
            // menuItem4
            // 
            this.menuItem4.Index = 4;
            this.menuItem4.Text = "-";
            // 
            // priorityThreadMenuItem
            // 
            this.vistaMenu.SetImage(this.priorityThreadMenuItem, global::ProcessHacker.Properties.Resources.control_equalizer_blue);
            this.priorityThreadMenuItem.Index = 5;
            this.priorityThreadMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.timeCriticalThreadMenuItem,
            this.highestThreadMenuItem,
            this.aboveNormalThreadMenuItem,
            this.normalThreadMenuItem,
            this.belowNormalThreadMenuItem,
            this.lowestThreadMenuItem,
            this.idleThreadMenuItem});
            this.priorityThreadMenuItem.Text = "&Priority";
            // 
            // timeCriticalThreadMenuItem
            // 
            this.timeCriticalThreadMenuItem.Index = 0;
            this.timeCriticalThreadMenuItem.RadioCheck = true;
            this.timeCriticalThreadMenuItem.Text = "Time Critical";
            this.timeCriticalThreadMenuItem.Click += new System.EventHandler(this.timeCriticalThreadMenuItem_Click);
            // 
            // highestThreadMenuItem
            // 
            this.highestThreadMenuItem.Index = 1;
            this.highestThreadMenuItem.RadioCheck = true;
            this.highestThreadMenuItem.Text = "Highest";
            this.highestThreadMenuItem.Click += new System.EventHandler(this.highestThreadMenuItem_Click);
            // 
            // aboveNormalThreadMenuItem
            // 
            this.aboveNormalThreadMenuItem.Index = 2;
            this.aboveNormalThreadMenuItem.RadioCheck = true;
            this.aboveNormalThreadMenuItem.Text = "Above Normal";
            this.aboveNormalThreadMenuItem.Click += new System.EventHandler(this.aboveNormalThreadMenuItem_Click);
            // 
            // normalThreadMenuItem
            // 
            this.normalThreadMenuItem.Index = 3;
            this.normalThreadMenuItem.RadioCheck = true;
            this.normalThreadMenuItem.Text = "Normal";
            this.normalThreadMenuItem.Click += new System.EventHandler(this.normalThreadMenuItem_Click);
            // 
            // belowNormalThreadMenuItem
            // 
            this.belowNormalThreadMenuItem.Index = 4;
            this.belowNormalThreadMenuItem.RadioCheck = true;
            this.belowNormalThreadMenuItem.Text = "Below Normal";
            this.belowNormalThreadMenuItem.Click += new System.EventHandler(this.belowNormalThreadMenuItem_Click);
            // 
            // lowestThreadMenuItem
            // 
            this.lowestThreadMenuItem.Index = 5;
            this.lowestThreadMenuItem.RadioCheck = true;
            this.lowestThreadMenuItem.Text = "Lowest";
            this.lowestThreadMenuItem.Click += new System.EventHandler(this.lowestThreadMenuItem_Click);
            // 
            // idleThreadMenuItem
            // 
            this.idleThreadMenuItem.Index = 6;
            this.idleThreadMenuItem.RadioCheck = true;
            this.idleThreadMenuItem.Text = "Idle";
            this.idleThreadMenuItem.Click += new System.EventHandler(this.idleThreadMenuItem_Click);
            // 
            // menuItem9
            // 
            this.menuItem9.Index = 6;
            this.menuItem9.Text = "-";
            // 
            // copyThreadMenuItem
            // 
            this.vistaMenu.SetImage(this.copyThreadMenuItem, global::ProcessHacker.Properties.Resources.page_copy);
            this.copyThreadMenuItem.Index = 7;
            this.copyThreadMenuItem.Text = "C&opy";
            // 
            // selectAllThreadMenuItem
            // 
            this.selectAllThreadMenuItem.Index = 8;
            this.selectAllThreadMenuItem.Text = "Select &All";
            this.selectAllThreadMenuItem.Click += new System.EventHandler(this.selectAllThreadMenuItem_Click);
            // 
            // toolStripMenuItem9
            // 
            this.toolStripMenuItem9.Name = "toolStripMenuItem9";
            this.toolStripMenuItem9.Size = new System.Drawing.Size(151, 22);
            this.toolStripMenuItem9.Text = "Time Critical";
            // 
            // toolStripMenuItem10
            // 
            this.toolStripMenuItem10.Name = "toolStripMenuItem10";
            this.toolStripMenuItem10.Size = new System.Drawing.Size(151, 22);
            this.toolStripMenuItem10.Text = "Highest";
            // 
            // toolStripMenuItem11
            // 
            this.toolStripMenuItem11.Name = "toolStripMenuItem11";
            this.toolStripMenuItem11.Size = new System.Drawing.Size(151, 22);
            this.toolStripMenuItem11.Text = "Above Normal";
            // 
            // toolStripMenuItem12
            // 
            this.toolStripMenuItem12.Name = "toolStripMenuItem12";
            this.toolStripMenuItem12.Size = new System.Drawing.Size(151, 22);
            this.toolStripMenuItem12.Text = "Normal";
            // 
            // toolStripMenuItem13
            // 
            this.toolStripMenuItem13.Name = "toolStripMenuItem13";
            this.toolStripMenuItem13.Size = new System.Drawing.Size(151, 22);
            this.toolStripMenuItem13.Text = "Below Normal";
            // 
            // toolStripMenuItem14
            // 
            this.toolStripMenuItem14.Name = "toolStripMenuItem14";
            this.toolStripMenuItem14.Size = new System.Drawing.Size(151, 22);
            this.toolStripMenuItem14.Text = "Lowest";
            // 
            // toolStripMenuItem15
            // 
            this.toolStripMenuItem15.Name = "toolStripMenuItem15";
            this.toolStripMenuItem15.Size = new System.Drawing.Size(151, 22);
            this.toolStripMenuItem15.Text = "Idle";
            // 
            // menuModule
            // 
            this.menuModule.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.goToInMemoryViewModuleMenuItem,
            this.getFuncAddressMenuItem,
            this.changeMemoryProtectionModuleMenuItem,
            this.readMemoryModuleMenuItem,
            this.inspectModuleMenuItem,
            this.menuItem3,
            this.searchModuleMenuItem,
            this.copyFileNameMenuItem,
            this.copyModuleMenuItem,
            this.openContainingFolderMenuItem,
            this.propertiesMenuItem,
            this.menuItem6,
            this.selectAllModuleMenuItem});
            this.menuModule.Popup += new System.EventHandler(this.menuModule_Popup);
            // 
            // goToInMemoryViewModuleMenuItem
            // 
            this.goToInMemoryViewModuleMenuItem.DefaultItem = true;
            this.vistaMenu.SetImage(this.goToInMemoryViewModuleMenuItem, global::ProcessHacker.Properties.Resources.arrow_right);
            this.goToInMemoryViewModuleMenuItem.Index = 0;
            this.goToInMemoryViewModuleMenuItem.Text = "&Go To In Memory View";
            this.goToInMemoryViewModuleMenuItem.Click += new System.EventHandler(this.goToInMemoryViewModuleMenuItem_Click);
            // 
            // getFuncAddressMenuItem
            // 
            this.getFuncAddressMenuItem.Index = 1;
            this.getFuncAddressMenuItem.Text = "Get &Function Address...";
            this.getFuncAddressMenuItem.Click += new System.EventHandler(this.getFuncAddressMenuItem_Click);
            // 
            // changeMemoryProtectionModuleMenuItem
            // 
            this.vistaMenu.SetImage(this.changeMemoryProtectionModuleMenuItem, global::ProcessHacker.Properties.Resources.lock_edit);
            this.changeMemoryProtectionModuleMenuItem.Index = 2;
            this.changeMemoryProtectionModuleMenuItem.Text = "Change &Memory Protection...";
            this.changeMemoryProtectionModuleMenuItem.Click += new System.EventHandler(this.changeMemoryProtectionModuleMenuItem_Click);
            // 
            // readMemoryModuleMenuItem
            // 
            this.vistaMenu.SetImage(this.readMemoryModuleMenuItem, global::ProcessHacker.Properties.Resources.page);
            this.readMemoryModuleMenuItem.Index = 3;
            this.readMemoryModuleMenuItem.Text = "Read Memory...";
            this.readMemoryModuleMenuItem.Click += new System.EventHandler(this.readMemoryModuleMenuItem_Click);
            // 
            // inspectModuleMenuItem
            // 
            this.vistaMenu.SetImage(this.inspectModuleMenuItem, global::ProcessHacker.Properties.Resources.application_form_magnify);
            this.inspectModuleMenuItem.Index = 4;
            this.inspectModuleMenuItem.Text = "&Inspect...";
            this.inspectModuleMenuItem.Click += new System.EventHandler(this.inspectModuleMenuItem_Click);
            // 
            // menuItem3
            // 
            this.menuItem3.Index = 5;
            this.menuItem3.Text = "-";
            // 
            // searchModuleMenuItem
            // 
            this.searchModuleMenuItem.Index = 6;
            this.searchModuleMenuItem.Text = "&Search Online...";
            this.searchModuleMenuItem.Click += new System.EventHandler(this.searchModuleMenuItem_Click);
            // 
            // copyFileNameMenuItem
            // 
            this.copyFileNameMenuItem.Index = 7;
            this.copyFileNameMenuItem.Text = "&Copy File Name(s)";
            this.copyFileNameMenuItem.Click += new System.EventHandler(this.copyFileNameMenuItem_Click);
            // 
            // copyModuleMenuItem
            // 
            this.vistaMenu.SetImage(this.copyModuleMenuItem, global::ProcessHacker.Properties.Resources.page_copy);
            this.copyModuleMenuItem.Index = 8;
            this.copyModuleMenuItem.Text = "Copy";
            // 
            // openContainingFolderMenuItem
            // 
            this.vistaMenu.SetImage(this.openContainingFolderMenuItem, global::ProcessHacker.Properties.Resources.folder_explore);
            this.openContainingFolderMenuItem.Index = 9;
            this.openContainingFolderMenuItem.Text = "&Open Containing Folder";
            this.openContainingFolderMenuItem.Click += new System.EventHandler(this.openContainingFolderMenuItem_Click);
            // 
            // propertiesMenuItem
            // 
            this.vistaMenu.SetImage(this.propertiesMenuItem, global::ProcessHacker.Properties.Resources.application_view_detail);
            this.propertiesMenuItem.Index = 10;
            this.propertiesMenuItem.Text = "Prope&rties...";
            this.propertiesMenuItem.Click += new System.EventHandler(this.propertiesMenuItem_Click);
            // 
            // menuItem6
            // 
            this.menuItem6.Index = 11;
            this.menuItem6.Text = "-";
            // 
            // selectAllModuleMenuItem
            // 
            this.selectAllModuleMenuItem.Index = 12;
            this.selectAllModuleMenuItem.Text = "Select &All";
            this.selectAllModuleMenuItem.Click += new System.EventHandler(this.selectAllModuleMenuItem_Click);
            // 
            // menuMemory
            // 
            this.menuMemory.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.changeMemoryProtectionMemoryMenuItem,
            this.readWriteMemoryMemoryMenuItem,
            this.readWriteAddressMemoryMenuItem,
            this.menuItem2,
            this.copyMemoryMenuItem,
            this.selectAllMemoryMenuItem});
            this.menuMemory.Popup += new System.EventHandler(this.menuMemory_Popup);
            // 
            // changeMemoryProtectionMemoryMenuItem
            // 
            this.vistaMenu.SetImage(this.changeMemoryProtectionMemoryMenuItem, global::ProcessHacker.Properties.Resources.lock_edit);
            this.changeMemoryProtectionMemoryMenuItem.Index = 0;
            this.changeMemoryProtectionMemoryMenuItem.Text = "Change &Memory Protection...";
            this.changeMemoryProtectionMemoryMenuItem.Click += new System.EventHandler(this.changeMemoryProtectionMemoryMenuItem_Click);
            // 
            // readWriteMemoryMemoryMenuItem
            // 
            this.readWriteMemoryMemoryMenuItem.DefaultItem = true;
            this.vistaMenu.SetImage(this.readWriteMemoryMemoryMenuItem, global::ProcessHacker.Properties.Resources.page_edit);
            this.readWriteMemoryMemoryMenuItem.Index = 1;
            this.readWriteMemoryMemoryMenuItem.Text = "Read/Write Memory...";
            this.readWriteMemoryMemoryMenuItem.Click += new System.EventHandler(this.readWriteMemoryMemoryMenuItem_Click);
            // 
            // readWriteAddressMemoryMenuItem
            // 
            this.vistaMenu.SetImage(this.readWriteAddressMemoryMenuItem, global::ProcessHacker.Properties.Resources.pencil_go);
            this.readWriteAddressMemoryMenuItem.Index = 2;
            this.readWriteAddressMemoryMenuItem.Text = "Read/Write Address...";
            this.readWriteAddressMemoryMenuItem.Click += new System.EventHandler(this.readWriteAddressMemoryMenuItem_Click);
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 3;
            this.menuItem2.Text = "-";
            // 
            // copyMemoryMenuItem
            // 
            this.vistaMenu.SetImage(this.copyMemoryMenuItem, global::ProcessHacker.Properties.Resources.page_copy);
            this.copyMemoryMenuItem.Index = 4;
            this.copyMemoryMenuItem.Text = "C&opy";
            // 
            // selectAllMemoryMenuItem
            // 
            this.selectAllMemoryMenuItem.Index = 5;
            this.selectAllMemoryMenuItem.Text = "Select &All";
            this.selectAllMemoryMenuItem.Click += new System.EventHandler(this.selectAllMemoryMenuItem_Click);
            // 
            // menuSearch
            // 
            this.menuSearch.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.newResultsWindowMenuItem,
            this.literalSearchMenuItem,
            this.regexSearchMenuItem,
            this.stringScanMenuItem,
            this.heapScanMenuItem});
            // 
            // newResultsWindowMenuItem
            // 
            this.vistaMenu.SetImage(this.newResultsWindowMenuItem, global::ProcessHacker.Properties.Resources.table);
            this.newResultsWindowMenuItem.Index = 0;
            this.newResultsWindowMenuItem.Text = "&New Results Window...";
            // 
            // literalSearchMenuItem
            // 
            this.literalSearchMenuItem.Index = 1;
            this.literalSearchMenuItem.Text = "&Literal Search...";
            // 
            // regexSearchMenuItem
            // 
            this.regexSearchMenuItem.Index = 2;
            this.regexSearchMenuItem.Text = "&Regex Search...";
            // 
            // stringScanMenuItem
            // 
            this.stringScanMenuItem.Index = 3;
            this.stringScanMenuItem.Text = "&String Scan...";
            // 
            // heapScanMenuItem
            // 
            this.heapScanMenuItem.Index = 4;
            this.heapScanMenuItem.Text = "&Heap Scan...";
            // 
            // mainMenu
            // 
            this.mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.hackerMenuItem,
            this.toolsMenuItem,
            this.windowMenuItem});
            // 
            // hackerMenuItem
            // 
            this.hackerMenuItem.Index = 0;
            this.hackerMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.selectAllHackerMenuItem,
            this.menuItem1,
            this.runAsMenuItem,
            this.findHandlesMenuItem,
            this.inspectPEFileMenuItem,
            this.logMenuItem,
            this.aboutMenuItem,
            this.optionsMenuItem,
            this.helpMenuItem,
            this.exitMenuItem});
            this.hackerMenuItem.Text = "&Hacker";
            // 
            // selectAllHackerMenuItem
            // 
            this.selectAllHackerMenuItem.Index = 0;
            this.selectAllHackerMenuItem.Shortcut = System.Windows.Forms.Shortcut.CtrlA;
            this.selectAllHackerMenuItem.Text = "&Select All";
            this.selectAllHackerMenuItem.Click += new System.EventHandler(this.selectAllHackerMenuItem_Click);
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 1;
            this.menuItem1.Text = "-";
            // 
            // runAsMenuItem
            // 
            this.runAsMenuItem.Index = 2;
            this.runAsMenuItem.Text = "&Run As...";
            this.runAsMenuItem.Click += new System.EventHandler(this.runAsMenuItem_Click);
            // 
            // findHandlesMenuItem
            // 
            this.vistaMenu.SetImage(this.findHandlesMenuItem, global::ProcessHacker.Properties.Resources.find);
            this.findHandlesMenuItem.Index = 3;
            this.findHandlesMenuItem.Text = "&Find Handles...";
            this.findHandlesMenuItem.Click += new System.EventHandler(this.findHandlesMenuItem_Click);
            // 
            // inspectPEFileMenuItem
            // 
            this.vistaMenu.SetImage(this.inspectPEFileMenuItem, global::ProcessHacker.Properties.Resources.application_form_magnify);
            this.inspectPEFileMenuItem.Index = 4;
            this.inspectPEFileMenuItem.Text = "Inspect &PE File...";
            this.inspectPEFileMenuItem.Click += new System.EventHandler(this.inspectPEFileMenuItem_Click);
            // 
            // logMenuItem
            // 
            this.vistaMenu.SetImage(this.logMenuItem, global::ProcessHacker.Properties.Resources.page_white_text);
            this.logMenuItem.Index = 5;
            this.logMenuItem.Text = "&Log...";
            this.logMenuItem.Click += new System.EventHandler(this.logMenuItem_Click);
            // 
            // aboutMenuItem
            // 
            this.vistaMenu.SetImage(this.aboutMenuItem, global::ProcessHacker.Properties.Resources.information);
            this.aboutMenuItem.Index = 6;
            this.aboutMenuItem.Text = "&About...";
            this.aboutMenuItem.Click += new System.EventHandler(this.aboutMenuItem_Click);
            // 
            // optionsMenuItem
            // 
            this.vistaMenu.SetImage(this.optionsMenuItem, global::ProcessHacker.Properties.Resources.page_gear);
            this.optionsMenuItem.Index = 7;
            this.optionsMenuItem.Text = "&Options...";
            this.optionsMenuItem.Click += new System.EventHandler(this.optionsMenuItem_Click);
            // 
            // helpMenuItem
            // 
            this.vistaMenu.SetImage(this.helpMenuItem, global::ProcessHacker.Properties.Resources.help);
            this.helpMenuItem.Index = 8;
            this.helpMenuItem.Text = "&Help...";
            this.helpMenuItem.Click += new System.EventHandler(this.helpMenuItem_Click);
            // 
            // exitMenuItem
            // 
            this.vistaMenu.SetImage(this.exitMenuItem, global::ProcessHacker.Properties.Resources.door_out);
            this.exitMenuItem.Index = 9;
            this.exitMenuItem.Text = "E&xit";
            this.exitMenuItem.Click += new System.EventHandler(this.exitMenuItem_Click);
            // 
            // toolsMenuItem
            // 
            this.toolsMenuItem.Index = 1;
            this.toolsMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.getSNFAMenuItem,
            this.FSPWSSIDMenuItem});
            this.toolsMenuItem.Text = "&Tools";
            // 
            // getSNFAMenuItem
            // 
            this.getSNFAMenuItem.Index = 0;
            this.getSNFAMenuItem.Text = "Get Symbol Name From Address...";
            this.getSNFAMenuItem.Click += new System.EventHandler(this.getSNFAMenuItem_Click);
            // 
            // FSPWSSIDMenuItem
            // 
            this.FSPWSSIDMenuItem.Index = 1;
            this.FSPWSSIDMenuItem.Text = "Find SYSTEM processes with same Session ID";
            this.FSPWSSIDMenuItem.Visible = false;
            this.FSPWSSIDMenuItem.Click += new System.EventHandler(this.FSPWSSIDMenuItem_Click);
            // 
            // windowMenuItem
            // 
            this.windowMenuItem.Index = 2;
            this.windowMenuItem.Text = "&Window";
            // 
            // statusBar
            // 
            this.statusBar.Location = new System.Drawing.Point(0, 409);
            this.statusBar.Name = "statusBar";
            this.statusBar.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
            this.statusGeneral,
            this.statusIcon,
            this.statusText});
            this.statusBar.ShowPanels = true;
            this.statusBar.Size = new System.Drawing.Size(804, 22);
            this.statusBar.TabIndex = 5;
            // 
            // statusGeneral
            // 
            this.statusGeneral.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
            this.statusGeneral.Name = "statusGeneral";
            this.statusGeneral.Width = 10;
            // 
            // statusIcon
            // 
            this.statusIcon.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
            this.statusIcon.BorderStyle = System.Windows.Forms.StatusBarPanelBorderStyle.None;
            this.statusIcon.MinWidth = 0;
            this.statusIcon.Name = "statusIcon";
            this.statusIcon.Width = 10;
            // 
            // statusText
            // 
            this.statusText.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
            this.statusText.BorderStyle = System.Windows.Forms.StatusBarPanelBorderStyle.None;
            this.statusText.Name = "statusText";
            this.statusText.Width = 10;
            // 
            // timerMessages
            // 
            this.timerMessages.Interval = 2000;
            this.timerMessages.Tick += new System.EventHandler(this.timerMessages_Tick);
            // 
            // tabControlBig
            // 
            this.tabControlBig.Controls.Add(this.tabProcesses);
            this.tabControlBig.Controls.Add(this.tabServices);
            this.tabControlBig.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlBig.Location = new System.Drawing.Point(0, 0);
            this.tabControlBig.Name = "tabControlBig";
            this.tabControlBig.SelectedIndex = 0;
            this.tabControlBig.Size = new System.Drawing.Size(804, 409);
            this.tabControlBig.TabIndex = 6;
            // 
            // tabProcesses
            // 
            this.tabProcesses.Controls.Add(this.splitMain);
            this.tabProcesses.Location = new System.Drawing.Point(4, 22);
            this.tabProcesses.Name = "tabProcesses";
            this.tabProcesses.Padding = new System.Windows.Forms.Padding(3);
            this.tabProcesses.Size = new System.Drawing.Size(796, 383);
            this.tabProcesses.TabIndex = 0;
            this.tabProcesses.Text = "Processes";
            this.tabProcesses.UseVisualStyleBackColor = true;
            // 
            // splitMain
            // 
            this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitMain.Location = new System.Drawing.Point(3, 3);
            this.splitMain.Name = "splitMain";
            // 
            // splitMain.Panel1
            // 
            this.splitMain.Panel1.Controls.Add(this.treeProcesses);
            this.splitMain.Panel1.Padding = new System.Windows.Forms.Padding(0, 0, 0, 1);
            // 
            // splitMain.Panel2
            // 
            this.splitMain.Panel2.Controls.Add(this.tabControl);
            this.splitMain.Size = new System.Drawing.Size(790, 377);
            this.splitMain.SplitterDistance = 348;
            this.splitMain.TabIndex = 3;
            // 
            // treeProcesses
            // 
            this.treeProcesses.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeProcesses.Location = new System.Drawing.Point(0, 0);
            this.treeProcesses.Name = "treeProcesses";
            this.treeProcesses.Provider = null;
            this.treeProcesses.Size = new System.Drawing.Size(348, 376);
            this.treeProcesses.TabIndex = 4;
            this.treeProcesses.SelectionChanged += new System.EventHandler(this.listProcesses_SelectionChanged);
            this.treeProcesses.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listProcesses_KeyDown);
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabProcess);
            this.tabControl.Controls.Add(this.tabThreads);
            this.tabControl.Controls.Add(this.tabModules);
            this.tabControl.Controls.Add(this.tabMemory);
            this.tabControl.Controls.Add(this.tabHandles);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(438, 377);
            this.tabControl.TabIndex = 5;
            this.tabControl.SelectedIndexChanged += new System.EventHandler(this.tabControl_SelectedIndexChanged);
            // 
            // tabProcess
            // 
            this.tabProcess.Controls.Add(this.groupSearch);
            this.tabProcess.Controls.Add(this.treeMisc);
            this.tabProcess.Location = new System.Drawing.Point(4, 22);
            this.tabProcess.Name = "tabProcess";
            this.tabProcess.Padding = new System.Windows.Forms.Padding(3);
            this.tabProcess.Size = new System.Drawing.Size(430, 351);
            this.tabProcess.TabIndex = 4;
            this.tabProcess.Text = "Process";
            this.tabProcess.UseVisualStyleBackColor = true;
            // 
            // groupSearch
            // 
            this.groupSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupSearch.Controls.Add(this.buttonSearch);
            this.groupSearch.Location = new System.Drawing.Point(6, 6);
            this.groupSearch.Name = "groupSearch";
            this.groupSearch.Size = new System.Drawing.Size(418, 47);
            this.groupSearch.TabIndex = 4;
            this.groupSearch.TabStop = false;
            this.groupSearch.Text = "Search";
            // 
            // buttonSearch
            // 
            this.buttonSearch.AutoSize = true;
            this.buttonSearch.Location = new System.Drawing.Point(6, 19);
            this.buttonSearch.Name = "buttonSearch";
            this.buttonSearch.Size = new System.Drawing.Size(102, 23);
            this.buttonSearch.SplitMenu = this.menuSearch;
            this.buttonSearch.TabIndex = 3;
            this.buttonSearch.Text = "Search button";
            this.buttonSearch.UseVisualStyleBackColor = true;
            this.buttonSearch.Click += new System.EventHandler(this.buttonSearch_Click);
            // 
            // treeMisc
            // 
            this.treeMisc.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.treeMisc.FullRowSelect = true;
            this.treeMisc.HideSelection = false;
            this.treeMisc.Location = new System.Drawing.Point(6, 59);
            this.treeMisc.Name = "treeMisc";
            this.treeMisc.ShowNodeToolTips = true;
            this.treeMisc.Size = new System.Drawing.Size(418, 286);
            this.treeMisc.TabIndex = 1;
            // 
            // tabThreads
            // 
            this.tabThreads.Controls.Add(this.listThreads);
            this.tabThreads.Location = new System.Drawing.Point(4, 22);
            this.tabThreads.Name = "tabThreads";
            this.tabThreads.Padding = new System.Windows.Forms.Padding(3);
            this.tabThreads.Size = new System.Drawing.Size(430, 351);
            this.tabThreads.TabIndex = 6;
            this.tabThreads.Text = "Threads";
            this.tabThreads.UseVisualStyleBackColor = true;
            // 
            // listThreads
            // 
            this.listThreads.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listThreads.DoubleBuffered = true;
            this.listThreads.Location = new System.Drawing.Point(3, 3);
            this.listThreads.Name = "listThreads";
            this.listThreads.Provider = null;
            this.listThreads.Size = new System.Drawing.Size(424, 345);
            this.listThreads.TabIndex = 0;
            this.listThreads.DoubleClick += new System.EventHandler(this.listThreads_DoubleClick);
            // 
            // tabModules
            // 
            this.tabModules.Controls.Add(this.listModules);
            this.tabModules.Location = new System.Drawing.Point(4, 22);
            this.tabModules.Name = "tabModules";
            this.tabModules.Padding = new System.Windows.Forms.Padding(3);
            this.tabModules.Size = new System.Drawing.Size(430, 351);
            this.tabModules.TabIndex = 0;
            this.tabModules.Text = "Modules";
            this.tabModules.UseVisualStyleBackColor = true;
            // 
            // listModules
            // 
            this.listModules.AllowColumnReorder = true;
            this.listModules.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnModuleName,
            this.columnBaseAddress,
            this.columnModuleSize,
            this.columnDescription});
            this.listModules.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listModules.FullRowSelect = true;
            this.listModules.HideSelection = false;
            this.listModules.Location = new System.Drawing.Point(3, 3);
            this.listModules.Name = "listModules";
            this.listModules.ShowItemToolTips = true;
            this.listModules.Size = new System.Drawing.Size(424, 345);
            this.listModules.TabIndex = 1;
            this.listModules.UseCompatibleStateImageBehavior = false;
            this.listModules.View = System.Windows.Forms.View.Details;
            this.listModules.DoubleClick += new System.EventHandler(this.listModules_DoubleClick);
            // 
            // columnModuleName
            // 
            this.columnModuleName.Text = "Name";
            this.columnModuleName.Width = 110;
            // 
            // columnBaseAddress
            // 
            this.columnBaseAddress.Text = "Base Address";
            this.columnBaseAddress.Width = 90;
            // 
            // columnModuleSize
            // 
            this.columnModuleSize.Text = "Size";
            this.columnModuleSize.Width = 70;
            // 
            // columnDescription
            // 
            this.columnDescription.Text = "Description";
            this.columnDescription.Width = 140;
            // 
            // tabMemory
            // 
            this.tabMemory.Controls.Add(this.listMemory);
            this.tabMemory.Location = new System.Drawing.Point(4, 22);
            this.tabMemory.Name = "tabMemory";
            this.tabMemory.Padding = new System.Windows.Forms.Padding(3);
            this.tabMemory.Size = new System.Drawing.Size(430, 351);
            this.tabMemory.TabIndex = 1;
            this.tabMemory.Text = "Memory";
            this.tabMemory.UseVisualStyleBackColor = true;
            // 
            // listMemory
            // 
            this.listMemory.AllowColumnReorder = true;
            this.listMemory.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5});
            this.listMemory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listMemory.FullRowSelect = true;
            this.listMemory.HideSelection = false;
            this.listMemory.Location = new System.Drawing.Point(3, 3);
            this.listMemory.Name = "listMemory";
            this.listMemory.ShowItemToolTips = true;
            this.listMemory.Size = new System.Drawing.Size(424, 345);
            this.listMemory.TabIndex = 2;
            this.listMemory.UseCompatibleStateImageBehavior = false;
            this.listMemory.View = System.Windows.Forms.View.Details;
            this.listMemory.DoubleClick += new System.EventHandler(this.listMemory_DoubleClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Address";
            this.columnHeader1.Width = 80;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Size";
            this.columnHeader2.Width = 80;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "State";
            this.columnHeader3.Width = 70;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Type";
            this.columnHeader4.Width = 70;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Protection";
            this.columnHeader5.Width = 80;
            // 
            // tabHandles
            // 
            this.tabHandles.Controls.Add(this.listHandles);
            this.tabHandles.Location = new System.Drawing.Point(4, 22);
            this.tabHandles.Name = "tabHandles";
            this.tabHandles.Padding = new System.Windows.Forms.Padding(3);
            this.tabHandles.Size = new System.Drawing.Size(430, 351);
            this.tabHandles.TabIndex = 7;
            this.tabHandles.Text = "Handles";
            this.tabHandles.UseVisualStyleBackColor = true;
            // 
            // listHandles
            // 
            this.listHandles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listHandles.DoubleBuffered = true;
            this.listHandles.Location = new System.Drawing.Point(3, 3);
            this.listHandles.Name = "listHandles";
            this.listHandles.Provider = null;
            this.listHandles.Size = new System.Drawing.Size(424, 345);
            this.listHandles.TabIndex = 0;
            // 
            // tabServices
            // 
            this.tabServices.Controls.Add(this.listServices);
            this.tabServices.Location = new System.Drawing.Point(4, 22);
            this.tabServices.Name = "tabServices";
            this.tabServices.Padding = new System.Windows.Forms.Padding(3);
            this.tabServices.Size = new System.Drawing.Size(796, 383);
            this.tabServices.TabIndex = 1;
            this.tabServices.Text = "Services";
            this.tabServices.UseVisualStyleBackColor = true;
            // 
            // listServices
            // 
            this.listServices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listServices.DoubleBuffered = true;
            this.listServices.Location = new System.Drawing.Point(3, 3);
            this.listServices.Name = "listServices";
            this.listServices.Provider = null;
            this.listServices.Size = new System.Drawing.Size(790, 377);
            this.listServices.TabIndex = 0;
            this.listServices.DoubleClick += new System.EventHandler(this.listServices_DoubleClick);
            // 
            // menuService
            // 
            this.menuService.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.goToProcessServiceMenuItem,
            this.startServiceMenuItem,
            this.continueServiceMenuItem,
            this.pauseServiceMenuItem,
            this.stopServiceMenuItem,
            this.deleteServiceMenuItem,
            this.propertiesServiceMenuItem,
            this.menuItem8,
            this.copyServiceMenuItem,
            this.selectAllServiceMenuItem});
            this.menuService.Popup += new System.EventHandler(this.menuService_Popup);
            // 
            // goToProcessServiceMenuItem
            // 
            this.vistaMenu.SetImage(this.goToProcessServiceMenuItem, global::ProcessHacker.Properties.Resources.arrow_right);
            this.goToProcessServiceMenuItem.Index = 0;
            this.goToProcessServiceMenuItem.Text = "&Go to Process";
            this.goToProcessServiceMenuItem.Click += new System.EventHandler(this.goToProcessServiceMenuItem_Click);
            // 
            // startServiceMenuItem
            // 
            this.vistaMenu.SetImage(this.startServiceMenuItem, global::ProcessHacker.Properties.Resources.control_play_blue);
            this.startServiceMenuItem.Index = 1;
            this.startServiceMenuItem.Text = "&Start";
            this.startServiceMenuItem.Click += new System.EventHandler(this.startServiceMenuItem_Click);
            // 
            // continueServiceMenuItem
            // 
            this.continueServiceMenuItem.Index = 2;
            this.continueServiceMenuItem.Text = "&Continue";
            this.continueServiceMenuItem.Click += new System.EventHandler(this.continueServiceMenuItem_Click);
            // 
            // pauseServiceMenuItem
            // 
            this.vistaMenu.SetImage(this.pauseServiceMenuItem, global::ProcessHacker.Properties.Resources.control_pause_blue);
            this.pauseServiceMenuItem.Index = 3;
            this.pauseServiceMenuItem.Text = "&Pause";
            this.pauseServiceMenuItem.Click += new System.EventHandler(this.pauseServiceMenuItem_Click);
            // 
            // stopServiceMenuItem
            // 
            this.vistaMenu.SetImage(this.stopServiceMenuItem, global::ProcessHacker.Properties.Resources.control_stop_blue);
            this.stopServiceMenuItem.Index = 4;
            this.stopServiceMenuItem.Text = "S&top";
            this.stopServiceMenuItem.Click += new System.EventHandler(this.stopServiceMenuItem_Click);
            // 
            // deleteServiceMenuItem
            // 
            this.vistaMenu.SetImage(this.deleteServiceMenuItem, global::ProcessHacker.Properties.Resources.cross);
            this.deleteServiceMenuItem.Index = 5;
            this.deleteServiceMenuItem.Text = "Delete";
            this.deleteServiceMenuItem.Click += new System.EventHandler(this.deleteServiceMenuItem_Click);
            // 
            // propertiesServiceMenuItem
            // 
            this.propertiesServiceMenuItem.DefaultItem = true;
            this.vistaMenu.SetImage(this.propertiesServiceMenuItem, global::ProcessHacker.Properties.Resources.application_form_magnify);
            this.propertiesServiceMenuItem.Index = 6;
            this.propertiesServiceMenuItem.Text = "&Properties...";
            this.propertiesServiceMenuItem.Click += new System.EventHandler(this.propertiesServiceMenuItem_Click);
            // 
            // menuItem8
            // 
            this.menuItem8.Index = 7;
            this.menuItem8.Text = "-";
            // 
            // copyServiceMenuItem
            // 
            this.vistaMenu.SetImage(this.copyServiceMenuItem, global::ProcessHacker.Properties.Resources.page_copy);
            this.copyServiceMenuItem.Index = 8;
            this.copyServiceMenuItem.Text = "Copy";
            // 
            // selectAllServiceMenuItem
            // 
            this.selectAllServiceMenuItem.Index = 9;
            this.selectAllServiceMenuItem.Text = "Select &All";
            this.selectAllServiceMenuItem.Click += new System.EventHandler(this.selectAllServiceMenuItem_Click);
            // 
            // notifyIcon
            // 
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "Process Hacker";
            this.notifyIcon.Visible = true;
            this.notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseDoubleClick);
            // 
            // menuIcon
            // 
            this.menuIcon.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.showHideMenuItem,
            this.notificationsMenuItem,
            this.exitTrayMenuItem});
            // 
            // showHideMenuItem
            // 
            this.showHideMenuItem.Index = 0;
            this.showHideMenuItem.Text = "&Show/Hide Process Hacker";
            this.showHideMenuItem.Click += new System.EventHandler(this.showHideMenuItem_Click);
            // 
            // notificationsMenuItem
            // 
            this.notificationsMenuItem.Index = 1;
            this.notificationsMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.NPMenuItem,
            this.TPMenuItem,
            this.NSMenuItem,
            this.startedSMenuItem,
            this.stoppedSMenuItem,
            this.DSMenuItem});
            this.notificationsMenuItem.Text = "&Notifications";
            // 
            // NPMenuItem
            // 
            this.NPMenuItem.Index = 0;
            this.NPMenuItem.Text = "New Processes";
            // 
            // TPMenuItem
            // 
            this.TPMenuItem.Index = 1;
            this.TPMenuItem.Text = "Terminated Processes";
            // 
            // NSMenuItem
            // 
            this.NSMenuItem.Index = 2;
            this.NSMenuItem.Text = "New Services";
            // 
            // startedSMenuItem
            // 
            this.startedSMenuItem.Index = 3;
            this.startedSMenuItem.Text = "Started Services";
            // 
            // stoppedSMenuItem
            // 
            this.stoppedSMenuItem.Index = 4;
            this.stoppedSMenuItem.Text = "Stopped Services";
            // 
            // DSMenuItem
            // 
            this.DSMenuItem.Index = 5;
            this.DSMenuItem.Text = "Deleted Services";
            // 
            // exitTrayMenuItem
            // 
            this.vistaMenu.SetImage(this.exitTrayMenuItem, global::ProcessHacker.Properties.Resources.door_out);
            this.exitTrayMenuItem.Index = 2;
            this.exitTrayMenuItem.Text = "E&xit";
            this.exitTrayMenuItem.Click += new System.EventHandler(this.exitTrayMenuItem_Click);
            // 
            // menuHandle
            // 
            this.menuHandle.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.closeHandleMenuItem,
            this.copyHandleMenuItem,
            this.menuItem11,
            this.propertiesHandleMenuItem});
            this.menuHandle.Popup += new System.EventHandler(this.menuHandle_Popup);
            // 
            // closeHandleMenuItem
            // 
            this.vistaMenu.SetImage(this.closeHandleMenuItem, global::ProcessHacker.Properties.Resources.cross);
            this.closeHandleMenuItem.Index = 0;
            this.closeHandleMenuItem.Text = "Close";
            this.closeHandleMenuItem.Click += new System.EventHandler(this.closeHandleMenuItem_Click);
            // 
            // copyHandleMenuItem
            // 
            this.vistaMenu.SetImage(this.copyHandleMenuItem, global::ProcessHacker.Properties.Resources.page_copy);
            this.copyHandleMenuItem.Index = 1;
            this.copyHandleMenuItem.Text = "&Copy";
            // 
            // menuItem11
            // 
            this.menuItem11.Index = 2;
            this.menuItem11.Text = "-";
            // 
            // propertiesHandleMenuItem
            // 
            this.propertiesHandleMenuItem.Index = 3;
            this.propertiesHandleMenuItem.Text = "&Properties...";
            this.propertiesHandleMenuItem.Click += new System.EventHandler(this.propertiesHandleMenuItem_Click);
            // 
            // menuMisc
            // 
            this.menuMisc.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.copyMiscMenuItem});
            this.menuMisc.Popup += new System.EventHandler(this.menuMisc_Popup);
            // 
            // copyMiscMenuItem
            // 
            this.vistaMenu.SetImage(this.copyMiscMenuItem, global::ProcessHacker.Properties.Resources.page_copy);
            this.copyMiscMenuItem.Index = 0;
            this.copyMiscMenuItem.Text = "&Copy";
            this.copyMiscMenuItem.Click += new System.EventHandler(this.copyMiscMenuItem_Click);
            // 
            // vistaMenu
            // 
            this.vistaMenu.ContainerControl = this;
            // 
            // HackerWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(804, 431);
            this.Controls.Add(this.tabControlBig);
            this.Controls.Add(this.statusBar);
            this.Controls.Add(this.panelVirtualProtect);
            this.Controls.Add(this.panelProc);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Menu = this.mainMenu;
            this.Name = "HackerWindow";
            this.Text = "Process Hacker";
            this.Load += new System.EventHandler(this.HackerWindow_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.formViewer_FormClosing);
            this.panelProc.ResumeLayout(false);
            this.panelProc.PerformLayout();
            this.panelVirtualProtect.ResumeLayout(false);
            this.panelVirtualProtect.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.statusGeneral)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusText)).EndInit();
            this.tabControlBig.ResumeLayout(false);
            this.tabProcesses.ResumeLayout(false);
            this.splitMain.Panel1.ResumeLayout(false);
            this.splitMain.Panel2.ResumeLayout(false);
            this.splitMain.ResumeLayout(false);
            this.tabControl.ResumeLayout(false);
            this.tabProcess.ResumeLayout(false);
            this.groupSearch.ResumeLayout(false);
            this.groupSearch.PerformLayout();
            this.tabThreads.ResumeLayout(false);
            this.tabModules.ResumeLayout(false);
            this.tabMemory.ResumeLayout(false);
            this.tabHandles.ResumeLayout(false);
            this.tabServices.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.vistaMenu)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listModules;
        private System.Windows.Forms.ColumnHeader columnModuleName;
        private System.Windows.Forms.ColumnHeader columnBaseAddress;
        private System.Windows.Forms.ColumnHeader columnModuleSize;
        private System.Windows.Forms.Timer timerFire;
        private System.Windows.Forms.Panel panelProc;
        private System.Windows.Forms.Label labelProcedureName;
        private System.Windows.Forms.TextBox textProcName;
        private System.Windows.Forms.Button buttonGetProcAddress;
        private System.Windows.Forms.TextBox textProcAddress;
        private System.Windows.Forms.SplitContainer splitMain;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabModules;
        private System.Windows.Forms.TabPage tabMemory;
        private System.Windows.Forms.ListView listMemory;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.Panel panelVirtualProtect;
        private System.Windows.Forms.Button buttonCloseVirtualProtect;
        private System.Windows.Forms.Button buttonVirtualProtect;
        private System.Windows.Forms.TextBox textNewProtection;
        private System.Windows.Forms.Label labelNewValue;
        private System.Windows.Forms.Label labelVirtualProtectInfo;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.ContextMenu menuProcess;
        private System.Windows.Forms.MenuItem terminateMenuItem;
        private System.Windows.Forms.MenuItem suspendMenuItem;
        private System.Windows.Forms.MenuItem resumeMenuItem;
        private System.Windows.Forms.MenuItem menuItem5;
        private System.Windows.Forms.MenuItem priorityMenuItem;
        private System.Windows.Forms.MenuItem menuItem7;
        private System.Windows.Forms.MenuItem realTimeMenuItem;
        private System.Windows.Forms.MenuItem highMenuItem;
        private System.Windows.Forms.MenuItem aboveNormalMenuItem;
        private System.Windows.Forms.MenuItem normalMenuItem;
        private System.Windows.Forms.MenuItem belowNormalMenuItem;
        private System.Windows.Forms.MenuItem idleMenuItem;
        private System.Windows.Forms.ContextMenu menuThread;
        private System.Windows.Forms.MenuItem terminateThreadMenuItem;
        private System.Windows.Forms.MenuItem suspendThreadMenuItem;
        private System.Windows.Forms.MenuItem resumeThreadMenuItem;
        private System.Windows.Forms.MenuItem menuItem4;
        private System.Windows.Forms.MenuItem priorityThreadMenuItem;
        private System.Windows.Forms.MenuItem menuItem9;
        private System.Windows.Forms.MenuItem selectAllThreadMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem9;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem10;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem11;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem12;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem13;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem14;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem15;
        private System.Windows.Forms.MenuItem timeCriticalThreadMenuItem;
        private System.Windows.Forms.MenuItem highestThreadMenuItem;
        private System.Windows.Forms.MenuItem aboveNormalThreadMenuItem;
        private System.Windows.Forms.MenuItem normalThreadMenuItem;
        private System.Windows.Forms.MenuItem belowNormalThreadMenuItem;
        private System.Windows.Forms.MenuItem lowestThreadMenuItem;
        private System.Windows.Forms.MenuItem idleThreadMenuItem;
        private System.Windows.Forms.ContextMenu menuModule;
        private System.Windows.Forms.MenuItem copyFileNameMenuItem;
        private System.Windows.Forms.MenuItem propertiesMenuItem;
        private System.Windows.Forms.MenuItem menuItem3;
        private System.Windows.Forms.MenuItem getFuncAddressMenuItem;
        private System.Windows.Forms.MenuItem changeMemoryProtectionModuleMenuItem;
        private System.Windows.Forms.MenuItem readMemoryModuleMenuItem;
        private System.Windows.Forms.MenuItem goToInMemoryViewModuleMenuItem;
        private System.Windows.Forms.Button buttonCloseProc;
        private System.Windows.Forms.ContextMenu menuMemory;
        private System.Windows.Forms.MenuItem changeMemoryProtectionMemoryMenuItem;
        private System.Windows.Forms.MenuItem readWriteMemoryMemoryMenuItem;
        private System.Windows.Forms.TabPage tabProcess;
        private wyDay.Controls.VistaMenu vistaMenu;
        private System.Windows.Forms.MenuItem readWriteAddressMemoryMenuItem;
        private System.Windows.Forms.TabPage tabThreads;
        private System.Windows.Forms.TreeView treeMisc;
        private System.Windows.Forms.ColumnHeader columnDescription;
        private System.Windows.Forms.MenuItem openContainingFolderMenuItem;
        private System.Windows.Forms.MainMenu mainMenu;
        private System.Windows.Forms.MenuItem hackerMenuItem;
        private System.Windows.Forms.MenuItem aboutMenuItem;
        private System.Windows.Forms.MenuItem optionsMenuItem;
        private System.Windows.Forms.MenuItem helpMenuItem;
        private System.Windows.Forms.MenuItem exitMenuItem;
        private System.Windows.Forms.GroupBox groupSearch;
        private System.Windows.Forms.MenuItem windowMenuItem;
        private System.Windows.Forms.MenuItem inspectThreadMenuItem;
        private wyDay.Controls.SplitButton buttonSearch;
        private System.Windows.Forms.ContextMenu menuSearch;
        private System.Windows.Forms.MenuItem literalSearchMenuItem;
        private System.Windows.Forms.MenuItem regexSearchMenuItem;
        private System.Windows.Forms.MenuItem stringScanMenuItem;
        private System.Windows.Forms.MenuItem heapScanMenuItem;
        private System.Windows.Forms.MenuItem newResultsWindowMenuItem;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem selectAllHackerMenuItem;
        private ProcessHacker.ProcessTree treeProcesses;
        private ThreadList listThreads;
        private System.Windows.Forms.MenuItem copyThreadMenuItem;
        private System.Windows.Forms.MenuItem copyModuleMenuItem;
        private System.Windows.Forms.MenuItem menuItem2;
        private System.Windows.Forms.MenuItem copyMemoryMenuItem;
        private System.Windows.Forms.MenuItem menuItem6;
        private System.Windows.Forms.MenuItem selectAllModuleMenuItem;
        private System.Windows.Forms.MenuItem selectAllMemoryMenuItem;
        private System.Windows.Forms.MenuItem tokenMenuItem;
        private System.Windows.Forms.MenuItem inspectPEFileMenuItem;
        private System.Windows.Forms.MenuItem inspectProcessMenuItem;
        private System.Windows.Forms.MenuItem inspectModuleMenuItem;
        private System.Windows.Forms.MenuItem searchProcessMenuItem;
        private System.Windows.Forms.MenuItem searchModuleMenuItem;
        private System.Windows.Forms.StatusBar statusBar;
        private System.Windows.Forms.Timer timerMessages;
        private System.Windows.Forms.StatusBarPanel statusIcon;
        private System.Windows.Forms.StatusBarPanel statusText;
        private System.Windows.Forms.MenuItem logMenuItem;
        private System.Windows.Forms.StatusBarPanel statusGeneral;
        private System.Windows.Forms.TabControl tabControlBig;
        private System.Windows.Forms.TabPage tabProcesses;
        private System.Windows.Forms.TabPage tabServices;
        private ServiceList listServices;
        private System.Windows.Forms.ContextMenu menuService;
        private System.Windows.Forms.MenuItem propertiesServiceMenuItem;
        private System.Windows.Forms.MenuItem startServiceMenuItem;
        private System.Windows.Forms.MenuItem pauseServiceMenuItem;
        private System.Windows.Forms.MenuItem stopServiceMenuItem;
        private System.Windows.Forms.MenuItem deleteServiceMenuItem;
        private System.Windows.Forms.MenuItem continueServiceMenuItem;
        private System.Windows.Forms.MenuItem goToProcessServiceMenuItem;
        private System.Windows.Forms.MenuItem servicesProcessMenuItem;
        private System.Windows.Forms.MenuItem menuItem8;
        private System.Windows.Forms.MenuItem copyServiceMenuItem;
        private System.Windows.Forms.MenuItem selectAllServiceMenuItem;
        private System.Windows.Forms.MenuItem injectorMenuItem;
        private System.Windows.Forms.MenuItem startProcessProcessMenuItem;
        private System.Windows.Forms.MenuItem getCommandLineProcessMenuItem;
        private System.Windows.Forms.MenuItem getSNFAMenuItem;
        private System.Windows.Forms.MenuItem exitProcessProcessMenuItem;
        private System.Windows.Forms.MenuItem toolsMenuItem;
        private System.Windows.Forms.MenuItem FSPWSSIDMenuItem;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ContextMenu menuIcon;
        private System.Windows.Forms.MenuItem showHideMenuItem;
        private System.Windows.Forms.MenuItem exitTrayMenuItem;
        private System.Windows.Forms.MenuItem notificationsMenuItem;
        private System.Windows.Forms.MenuItem NPMenuItem;
        private System.Windows.Forms.MenuItem TPMenuItem;
        private System.Windows.Forms.MenuItem NSMenuItem;
        private System.Windows.Forms.MenuItem startedSMenuItem;
        private System.Windows.Forms.MenuItem stoppedSMenuItem;
        private System.Windows.Forms.MenuItem DSMenuItem;
        private System.Windows.Forms.TabPage tabHandles;
        private HandleList listHandles;
        private System.Windows.Forms.ContextMenu menuHandle;
        private System.Windows.Forms.MenuItem closeHandleMenuItem;
        private System.Windows.Forms.MenuItem copyHandleMenuItem;
        private System.Windows.Forms.MenuItem findHandlesMenuItem;
        private System.Windows.Forms.ContextMenu menuMisc;
        private System.Windows.Forms.MenuItem copyMiscMenuItem;
        private System.Windows.Forms.MenuItem affinityProcessMenuItem;
        private System.Windows.Forms.MenuItem runAsMenuItem;
        private System.Windows.Forms.MenuItem runAsProcessMenuItem;
        private System.Windows.Forms.MenuItem launchAsUserProcessMenuItem;
        private System.Windows.Forms.MenuItem launchAsThisUserProcessMenuItem;
        private System.Windows.Forms.MenuItem menuItem11;
        private System.Windows.Forms.MenuItem propertiesHandleMenuItem;
    }
}

