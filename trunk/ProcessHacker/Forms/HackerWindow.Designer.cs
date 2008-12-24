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
            this.timerFire = new System.Windows.Forms.Timer(this.components);
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.menuProcess = new System.Windows.Forms.ContextMenu();
            this.terminateMenuItem = new System.Windows.Forms.MenuItem();
            this.suspendMenuItem = new System.Windows.Forms.MenuItem();
            this.resumeMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem5 = new System.Windows.Forms.MenuItem();
            this.affinityProcessMenuItem = new System.Windows.Forms.MenuItem();
            this.terminatorProcessMenuItem = new System.Windows.Forms.MenuItem();
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
            this.propertiesProcessMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem7 = new System.Windows.Forms.MenuItem();
            this.searchProcessMenuItem = new System.Windows.Forms.MenuItem();
            this.copyProcessMenuItem = new System.Windows.Forms.MenuItem();
            this.selectAllProcessMenuItem = new System.Windows.Forms.MenuItem();
            this.toolStripMenuItem9 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem10 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem11 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem12 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem13 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem14 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem15 = new System.Windows.Forms.ToolStripMenuItem();
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
            this.sysInfoMenuItem = new System.Windows.Forms.MenuItem();
            this.toolsMenuItem = new System.Windows.Forms.MenuItem();
            this.getSNFAMenuItem = new System.Windows.Forms.MenuItem();
            this.windowMenuItem = new System.Windows.Forms.MenuItem();
            this.statusBar = new System.Windows.Forms.StatusBar();
            this.statusGeneral = new System.Windows.Forms.StatusBarPanel();
            this.statusIcon = new System.Windows.Forms.StatusBarPanel();
            this.statusText = new System.Windows.Forms.StatusBarPanel();
            this.timerMessages = new System.Windows.Forms.Timer(this.components);
            this.tabControlBig = new System.Windows.Forms.TabControl();
            this.tabProcesses = new System.Windows.Forms.TabPage();
            this.treeProcesses = new ProcessHacker.ProcessTree();
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
            this.menuMisc = new System.Windows.Forms.ContextMenu();
            this.copyMiscMenuItem = new System.Windows.Forms.MenuItem();
            this.vistaMenu = new wyDay.Controls.VistaMenu(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.statusGeneral)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusText)).BeginInit();
            this.tabControlBig.SuspendLayout();
            this.tabProcesses.SuspendLayout();
            this.tabServices.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.vistaMenu)).BeginInit();
            this.SuspendLayout();
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
            // menuProcess
            // 
            this.menuProcess.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.terminateMenuItem,
            this.suspendMenuItem,
            this.resumeMenuItem,
            this.menuItem5,
            this.affinityProcessMenuItem,
            this.terminatorProcessMenuItem,
            this.priorityMenuItem,
            this.runAsProcessMenuItem,
            this.propertiesProcessMenuItem,
            this.menuItem7,
            this.searchProcessMenuItem,
            this.copyProcessMenuItem,
            this.selectAllProcessMenuItem});
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
            // affinityProcessMenuItem
            // 
            this.affinityProcessMenuItem.Index = 4;
            this.affinityProcessMenuItem.Text = "Affinity...";
            this.affinityProcessMenuItem.Click += new System.EventHandler(this.affinityProcessMenuItem_Click);
            // 
            // terminatorProcessMenuItem
            // 
            this.terminatorProcessMenuItem.Index = 5;
            this.terminatorProcessMenuItem.Text = "Terminator...";
            this.terminatorProcessMenuItem.Click += new System.EventHandler(this.terminatorProcessMenuItem_Click);
            // 
            // priorityMenuItem
            // 
            this.vistaMenu.SetImage(this.priorityMenuItem, global::ProcessHacker.Properties.Resources.control_equalizer_blue);
            this.priorityMenuItem.Index = 6;
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
            this.runAsProcessMenuItem.Index = 7;
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
            // propertiesProcessMenuItem
            // 
            this.vistaMenu.SetImage(this.propertiesProcessMenuItem, global::ProcessHacker.Properties.Resources.application_form_magnify);
            this.propertiesProcessMenuItem.Index = 8;
            this.propertiesProcessMenuItem.Text = "&Properties...";
            this.propertiesProcessMenuItem.Click += new System.EventHandler(this.inspectProcessMenuItem_Click);
            // 
            // menuItem7
            // 
            this.menuItem7.Index = 9;
            this.menuItem7.Text = "-";
            // 
            // searchProcessMenuItem
            // 
            this.searchProcessMenuItem.Index = 10;
            this.searchProcessMenuItem.Text = "&Search Online...";
            this.searchProcessMenuItem.Click += new System.EventHandler(this.searchProcessMenuItem_Click);
            // 
            // copyProcessMenuItem
            // 
            this.vistaMenu.SetImage(this.copyProcessMenuItem, global::ProcessHacker.Properties.Resources.page_copy);
            this.copyProcessMenuItem.Index = 11;
            this.copyProcessMenuItem.Text = "&Copy";
            // 
            // selectAllProcessMenuItem
            // 
            this.selectAllProcessMenuItem.Index = 12;
            this.selectAllProcessMenuItem.Text = "Select &All";
            this.selectAllProcessMenuItem.Click += new System.EventHandler(this.selectAllProcessMenuItem_Click);
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
            this.exitMenuItem,
            this.sysInfoMenuItem});
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
            // sysInfoMenuItem
            // 
            this.sysInfoMenuItem.Index = 10;
            this.sysInfoMenuItem.Text = "System &Information";
            this.sysInfoMenuItem.Visible = false;
            this.sysInfoMenuItem.Click += new System.EventHandler(this.sysInfoMenuItem_Click);
            // 
            // toolsMenuItem
            // 
            this.toolsMenuItem.Index = 1;
            this.toolsMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.getSNFAMenuItem});
            this.toolsMenuItem.Text = "&Tools";
            // 
            // getSNFAMenuItem
            // 
            this.getSNFAMenuItem.Index = 0;
            this.getSNFAMenuItem.Text = "Get Symbol Name From Address...";
            this.getSNFAMenuItem.Click += new System.EventHandler(this.getSNFAMenuItem_Click);
            // 
            // windowMenuItem
            // 
            this.windowMenuItem.Index = 2;
            this.windowMenuItem.Text = "&Window";
            // 
            // statusBar
            // 
            this.statusBar.Location = new System.Drawing.Point(0, 426);
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
            this.tabControlBig.Size = new System.Drawing.Size(804, 426);
            this.tabControlBig.TabIndex = 6;
            // 
            // tabProcesses
            // 
            this.tabProcesses.Controls.Add(this.treeProcesses);
            this.tabProcesses.Location = new System.Drawing.Point(4, 22);
            this.tabProcesses.Name = "tabProcesses";
            this.tabProcesses.Padding = new System.Windows.Forms.Padding(3);
            this.tabProcesses.Size = new System.Drawing.Size(796, 400);
            this.tabProcesses.TabIndex = 0;
            this.tabProcesses.Text = "Processes";
            this.tabProcesses.UseVisualStyleBackColor = true;
            // 
            // treeProcesses
            // 
            this.treeProcesses.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeProcesses.Location = new System.Drawing.Point(3, 3);
            this.treeProcesses.Name = "treeProcesses";
            this.treeProcesses.Provider = null;
            this.treeProcesses.Size = new System.Drawing.Size(790, 394);
            this.treeProcesses.TabIndex = 4;
            this.treeProcesses.SelectionChanged += new System.EventHandler(this.listProcesses_SelectionChanged);
            this.treeProcesses.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listProcesses_KeyDown);
            // 
            // tabServices
            // 
            this.tabServices.Controls.Add(this.listServices);
            this.tabServices.Location = new System.Drawing.Point(4, 22);
            this.tabServices.Name = "tabServices";
            this.tabServices.Padding = new System.Windows.Forms.Padding(3);
            this.tabServices.Size = new System.Drawing.Size(796, 400);
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
            this.listServices.Size = new System.Drawing.Size(790, 394);
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
            // menuMisc
            // 
            this.menuMisc.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.copyMiscMenuItem});
            // 
            // copyMiscMenuItem
            // 
            this.vistaMenu.SetImage(this.copyMiscMenuItem, global::ProcessHacker.Properties.Resources.page_copy);
            this.copyMiscMenuItem.Index = 0;
            this.copyMiscMenuItem.Text = "&Copy";
            // 
            // vistaMenu
            // 
            this.vistaMenu.ContainerControl = this;
            // 
            // HackerWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(804, 448);
            this.Controls.Add(this.tabControlBig);
            this.Controls.Add(this.statusBar);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Menu = this.mainMenu;
            this.Name = "HackerWindow";
            this.Text = "Process Hacker";
            this.Load += new System.EventHandler(this.HackerWindow_Load);
            this.SizeChanged += new System.EventHandler(this.HackerWindow_SizeChanged);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.formViewer_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.statusGeneral)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusText)).EndInit();
            this.tabControlBig.ResumeLayout(false);
            this.tabProcesses.ResumeLayout(false);
            this.tabServices.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.vistaMenu)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timerFire;
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
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem9;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem10;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem11;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem12;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem13;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem14;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem15;
        private wyDay.Controls.VistaMenu vistaMenu;
        private System.Windows.Forms.MainMenu mainMenu;
        private System.Windows.Forms.MenuItem hackerMenuItem;
        private System.Windows.Forms.MenuItem aboutMenuItem;
        private System.Windows.Forms.MenuItem optionsMenuItem;
        private System.Windows.Forms.MenuItem helpMenuItem;
        private System.Windows.Forms.MenuItem exitMenuItem;
        private System.Windows.Forms.MenuItem windowMenuItem;
        private System.Windows.Forms.ContextMenu menuSearch;
        private System.Windows.Forms.MenuItem literalSearchMenuItem;
        private System.Windows.Forms.MenuItem regexSearchMenuItem;
        private System.Windows.Forms.MenuItem stringScanMenuItem;
        private System.Windows.Forms.MenuItem heapScanMenuItem;
        private System.Windows.Forms.MenuItem newResultsWindowMenuItem;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem selectAllHackerMenuItem;
        private ProcessHacker.ProcessTree treeProcesses;
        private System.Windows.Forms.MenuItem inspectPEFileMenuItem;
        private System.Windows.Forms.MenuItem propertiesProcessMenuItem;
        private System.Windows.Forms.MenuItem searchProcessMenuItem;
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
        private System.Windows.Forms.MenuItem menuItem8;
        private System.Windows.Forms.MenuItem copyServiceMenuItem;
        private System.Windows.Forms.MenuItem selectAllServiceMenuItem;
        private System.Windows.Forms.MenuItem getSNFAMenuItem;
        private System.Windows.Forms.MenuItem toolsMenuItem;
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
        private System.Windows.Forms.MenuItem findHandlesMenuItem;
        private System.Windows.Forms.ContextMenu menuMisc;
        private System.Windows.Forms.MenuItem copyMiscMenuItem;
        private System.Windows.Forms.MenuItem affinityProcessMenuItem;
        private System.Windows.Forms.MenuItem runAsMenuItem;
        private System.Windows.Forms.MenuItem runAsProcessMenuItem;
        private System.Windows.Forms.MenuItem launchAsUserProcessMenuItem;
        private System.Windows.Forms.MenuItem launchAsThisUserProcessMenuItem;
        private System.Windows.Forms.MenuItem sysInfoMenuItem;
        private System.Windows.Forms.MenuItem copyProcessMenuItem;
        private System.Windows.Forms.MenuItem selectAllProcessMenuItem;
        private System.Windows.Forms.MenuItem terminatorProcessMenuItem;
    }
}

