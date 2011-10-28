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
            this.menuProcess = new System.Windows.Forms.ContextMenu();
            this.terminateMenuItem = new System.Windows.Forms.MenuItem();
            this.terminateProcessTreeMenuItem = new System.Windows.Forms.MenuItem();
            this.suspendMenuItem = new System.Windows.Forms.MenuItem();
            this.resumeMenuItem = new System.Windows.Forms.MenuItem();
            this.restartProcessMenuItem = new System.Windows.Forms.MenuItem();
            this.reduceWorkingSetProcessMenuItem = new System.Windows.Forms.MenuItem();
            this.virtualizationProcessMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem5 = new System.Windows.Forms.MenuItem();
            this.affinityProcessMenuItem = new System.Windows.Forms.MenuItem();
            this.createDumpFileProcessMenuItem = new System.Windows.Forms.MenuItem();
            this.terminatorProcessMenuItem = new System.Windows.Forms.MenuItem();
            this.miscellaneousProcessMenuItem = new System.Windows.Forms.MenuItem();
            this.analyzeWaitChainProcessMenuItem = new System.Windows.Forms.MenuItem();
            this.detachFromDebuggerProcessMenuItem = new System.Windows.Forms.MenuItem();
            this.heapsProcessMenuItem = new System.Windows.Forms.MenuItem();
            this.injectDllProcessMenuItem = new System.Windows.Forms.MenuItem();
            this.ioPriorityThreadMenuItem = new System.Windows.Forms.MenuItem();
            this.ioPriority0ThreadMenuItem = new System.Windows.Forms.MenuItem();
            this.ioPriority1ThreadMenuItem = new System.Windows.Forms.MenuItem();
            this.ioPriority2ThreadMenuItem = new System.Windows.Forms.MenuItem();
            this.ioPriority3ThreadMenuItem = new System.Windows.Forms.MenuItem();
            this.protectionProcessMenuItem = new System.Windows.Forms.MenuItem();
            this.setTokenProcessMenuItem = new System.Windows.Forms.MenuItem();
            this.VirusTotalMenuItem = new System.Windows.Forms.MenuItem();
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
            this.windowProcessMenuItem = new System.Windows.Forms.MenuItem();
            this.bringToFrontProcessMenuItem = new System.Windows.Forms.MenuItem();
            this.restoreProcessMenuItem = new System.Windows.Forms.MenuItem();
            this.minimizeProcessMenuItem = new System.Windows.Forms.MenuItem();
            this.maximizeProcessMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem15 = new System.Windows.Forms.MenuItem();
            this.closeProcessMenuItem = new System.Windows.Forms.MenuItem();
            this.propertiesProcessMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem7 = new System.Windows.Forms.MenuItem();
            this.searchProcessMenuItem = new System.Windows.Forms.MenuItem();
            this.reanalyzeProcessMenuItem = new System.Windows.Forms.MenuItem();
            this.copyProcessMenuItem = new System.Windows.Forms.MenuItem();
            this.selectAllProcessMenuItem = new System.Windows.Forms.MenuItem();
            this.toolStripMenuItem9 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem10 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem11 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem12 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem13 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem14 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem15 = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabProcesses = new System.Windows.Forms.TabPage();
            this.treeProcesses = new ProcessHacker.ProcessTree();
            this.tabServices = new System.Windows.Forms.TabPage();
            this.listServices = new ProcessHacker.Components.ServiceList();
            this.tabNetwork = new System.Windows.Forms.TabPage();
            this.listNetwork = new ProcessHacker.Components.NetworkList();
            this.contextMenuStripNetwork = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.goToProcessNetworkMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.whoisNetworkMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tracertNetworkMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pingNetworkMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeNetworkMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectAllNetworkMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.refreshToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.optionsToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.shutDownToolStripMenuItem = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.findHandlesToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.sysInfoToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripTextBox2 = new System.Windows.Forms.ToolStripTextBox();
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
            this.menuIcon = new System.Windows.Forms.ContextMenu();
            this.showHideMenuItem = new System.Windows.Forms.MenuItem();
            this.sysInformationIconMenuItem = new System.Windows.Forms.MenuItem();
            this.networkInfomationMenuItem = new System.Windows.Forms.MenuItem();
            this.notificationsMenuItem = new System.Windows.Forms.MenuItem();
            this.enableAllNotificationsMenuItem = new System.Windows.Forms.MenuItem();
            this.disableAllNotificationsMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.NPMenuItem = new System.Windows.Forms.MenuItem();
            this.TPMenuItem = new System.Windows.Forms.MenuItem();
            this.NSMenuItem = new System.Windows.Forms.MenuItem();
            this.startedSMenuItem = new System.Windows.Forms.MenuItem();
            this.stoppedSMenuItem = new System.Windows.Forms.MenuItem();
            this.DSMenuItem = new System.Windows.Forms.MenuItem();
            this.processesMenuItem = new System.Windows.Forms.MenuItem();
            this.shutdownTrayMenuItem = new System.Windows.Forms.MenuItem();
            this.exitTrayMenuItem = new System.Windows.Forms.MenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusMemory = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusCPU = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusGeneral = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStripEx1 = new System.MenuStripEx();
            this.hackerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runAsAdministratorMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showDetailsForAllProcessesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.findHandlesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.inspectPEFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.shutdownMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolbarMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sysInfoMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.trayIconsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cpuHistoryMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cpuUsageMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ioHistoryMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.commitHistoryMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.physMemHistoryMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateProcessesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateServicesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createServiceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hiddenProcessesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.verifyFileSignatureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.usersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.windowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkForUpdatesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.logToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl.SuspendLayout();
            this.tabProcesses.SuspendLayout();
            this.tabServices.SuspendLayout();
            this.tabNetwork.SuspendLayout();
            this.contextMenuStripNetwork.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.menuStripEx1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuProcess
            // 
            this.menuProcess.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.terminateMenuItem,
            this.terminateProcessTreeMenuItem,
            this.suspendMenuItem,
            this.resumeMenuItem,
            this.restartProcessMenuItem,
            this.reduceWorkingSetProcessMenuItem,
            this.virtualizationProcessMenuItem,
            this.menuItem5,
            this.affinityProcessMenuItem,
            this.createDumpFileProcessMenuItem,
            this.terminatorProcessMenuItem,
            this.miscellaneousProcessMenuItem,
            this.priorityMenuItem,
            this.runAsProcessMenuItem,
            this.windowProcessMenuItem,
            this.propertiesProcessMenuItem,
            this.menuItem7,
            this.searchProcessMenuItem,
            this.reanalyzeProcessMenuItem,
            this.copyProcessMenuItem,
            this.selectAllProcessMenuItem});
            this.menuProcess.Popup += new System.EventHandler(this.menuProcess_Popup);
            // 
            // terminateMenuItem
            // 
            this.terminateMenuItem.Index = 0;
            this.terminateMenuItem.Shortcut = System.Windows.Forms.Shortcut.Del;
            this.terminateMenuItem.Text = "&Terminate";
            this.terminateMenuItem.Click += new System.EventHandler(this.terminateMenuItem_Click);
            // 
            // terminateProcessTreeMenuItem
            // 
            this.terminateProcessTreeMenuItem.Index = 1;
            this.terminateProcessTreeMenuItem.Text = "Terminate Process Tree";
            this.terminateProcessTreeMenuItem.Click += new System.EventHandler(this.terminateProcessTreeMenuItem_Click);
            // 
            // suspendMenuItem
            // 
            this.suspendMenuItem.Index = 2;
            this.suspendMenuItem.Text = "&Suspend";
            this.suspendMenuItem.Click += new System.EventHandler(this.suspendMenuItem_Click);
            // 
            // resumeMenuItem
            // 
            this.resumeMenuItem.Index = 3;
            this.resumeMenuItem.Text = "&Resume";
            this.resumeMenuItem.Click += new System.EventHandler(this.resumeMenuItem_Click);
            // 
            // restartProcessMenuItem
            // 
            this.restartProcessMenuItem.Index = 4;
            this.restartProcessMenuItem.Text = "Restart";
            this.restartProcessMenuItem.Click += new System.EventHandler(this.restartProcessMenuItem_Click);
            // 
            // reduceWorkingSetProcessMenuItem
            // 
            this.reduceWorkingSetProcessMenuItem.Index = 5;
            this.reduceWorkingSetProcessMenuItem.Text = "Reduce Working Set";
            this.reduceWorkingSetProcessMenuItem.Click += new System.EventHandler(this.reduceWorkingSetProcessMenuItem_Click);
            // 
            // virtualizationProcessMenuItem
            // 
            this.virtualizationProcessMenuItem.Index = 6;
            this.virtualizationProcessMenuItem.Text = "Virtualization";
            this.virtualizationProcessMenuItem.Click += new System.EventHandler(this.virtualizationProcessMenuItem_Click);
            // 
            // menuItem5
            // 
            this.menuItem5.Index = 7;
            this.menuItem5.Text = "-";
            // 
            // affinityProcessMenuItem
            // 
            this.affinityProcessMenuItem.Index = 8;
            this.affinityProcessMenuItem.Text = "Affinity...";
            this.affinityProcessMenuItem.Click += new System.EventHandler(this.affinityProcessMenuItem_Click);
            // 
            // createDumpFileProcessMenuItem
            // 
            this.createDumpFileProcessMenuItem.Index = 9;
            this.createDumpFileProcessMenuItem.Text = "Create Dump File...";
            this.createDumpFileProcessMenuItem.Click += new System.EventHandler(this.createDumpFileProcessMenuItem_Click);
            // 
            // terminatorProcessMenuItem
            // 
            this.terminatorProcessMenuItem.Index = 10;
            this.terminatorProcessMenuItem.Text = "Terminator";
            this.terminatorProcessMenuItem.Click += new System.EventHandler(this.terminatorProcessMenuItem_Click);
            // 
            // miscellaneousProcessMenuItem
            // 
            this.miscellaneousProcessMenuItem.Index = 11;
            this.miscellaneousProcessMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.analyzeWaitChainProcessMenuItem,
            this.detachFromDebuggerProcessMenuItem,
            this.heapsProcessMenuItem,
            this.injectDllProcessMenuItem,
            this.ioPriorityThreadMenuItem,
            this.protectionProcessMenuItem,
            this.setTokenProcessMenuItem,
            this.VirusTotalMenuItem});
            this.miscellaneousProcessMenuItem.Text = "Miscellaneous";
            // 
            // analyzeWaitChainProcessMenuItem
            // 
            this.analyzeWaitChainProcessMenuItem.Index = 0;
            this.analyzeWaitChainProcessMenuItem.Text = "Analyze Wait Chain";
            this.analyzeWaitChainProcessMenuItem.Click += new System.EventHandler(this.analyzeWaitChainProcessMenuItem_Click);
            // 
            // detachFromDebuggerProcessMenuItem
            // 
            this.detachFromDebuggerProcessMenuItem.Index = 1;
            this.detachFromDebuggerProcessMenuItem.Text = "Detach from Debugger";
            this.detachFromDebuggerProcessMenuItem.Click += new System.EventHandler(this.detachFromDebuggerProcessMenuItem_Click);
            // 
            // heapsProcessMenuItem
            // 
            this.heapsProcessMenuItem.Index = 2;
            this.heapsProcessMenuItem.Text = "Heaps";
            this.heapsProcessMenuItem.Click += new System.EventHandler(this.heapsProcessMenuItem_Click);
            // 
            // injectDllProcessMenuItem
            // 
            this.injectDllProcessMenuItem.Index = 3;
            this.injectDllProcessMenuItem.Text = "Inject DLL...";
            this.injectDllProcessMenuItem.Click += new System.EventHandler(this.injectDllProcessMenuItem_Click);
            // 
            // ioPriorityThreadMenuItem
            // 
            this.ioPriorityThreadMenuItem.Index = 4;
            this.ioPriorityThreadMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.ioPriority0ThreadMenuItem,
            this.ioPriority1ThreadMenuItem,
            this.ioPriority2ThreadMenuItem,
            this.ioPriority3ThreadMenuItem});
            this.ioPriorityThreadMenuItem.Text = "I/O Priority";
            // 
            // ioPriority0ThreadMenuItem
            // 
            this.ioPriority0ThreadMenuItem.Index = 0;
            this.ioPriority0ThreadMenuItem.Text = "0";
            this.ioPriority0ThreadMenuItem.Click += new System.EventHandler(this.ioPriority0ThreadMenuItem_Click);
            // 
            // ioPriority1ThreadMenuItem
            // 
            this.ioPriority1ThreadMenuItem.Index = 1;
            this.ioPriority1ThreadMenuItem.Text = "1";
            this.ioPriority1ThreadMenuItem.Click += new System.EventHandler(this.ioPriority1ThreadMenuItem_Click);
            // 
            // ioPriority2ThreadMenuItem
            // 
            this.ioPriority2ThreadMenuItem.Index = 2;
            this.ioPriority2ThreadMenuItem.Text = "2";
            this.ioPriority2ThreadMenuItem.Click += new System.EventHandler(this.ioPriority2ThreadMenuItem_Click);
            // 
            // ioPriority3ThreadMenuItem
            // 
            this.ioPriority3ThreadMenuItem.Index = 3;
            this.ioPriority3ThreadMenuItem.Text = "3";
            this.ioPriority3ThreadMenuItem.Click += new System.EventHandler(this.ioPriority3ThreadMenuItem_Click);
            // 
            // protectionProcessMenuItem
            // 
            this.protectionProcessMenuItem.Index = 5;
            this.protectionProcessMenuItem.Text = "Protection";
            this.protectionProcessMenuItem.Click += new System.EventHandler(this.protectionProcessMenuItem_Click);
            // 
            // setTokenProcessMenuItem
            // 
            this.setTokenProcessMenuItem.Index = 6;
            this.setTokenProcessMenuItem.Text = "Set Token...";
            this.setTokenProcessMenuItem.Click += new System.EventHandler(this.setTokenProcessMenuItem_Click);
            // 
            // VirusTotalMenuItem
            // 
            this.VirusTotalMenuItem.Index = 7;
            this.VirusTotalMenuItem.Text = "Upload to VirusTotal";
            this.VirusTotalMenuItem.Click += new System.EventHandler(this.virusTotalMenuItem_Click);
            // 
            // priorityMenuItem
            // 
            this.priorityMenuItem.Index = 12;
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
            this.runAsProcessMenuItem.Index = 13;
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
            // windowProcessMenuItem
            // 
            this.windowProcessMenuItem.Index = 14;
            this.windowProcessMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.bringToFrontProcessMenuItem,
            this.restoreProcessMenuItem,
            this.minimizeProcessMenuItem,
            this.maximizeProcessMenuItem,
            this.menuItem15,
            this.closeProcessMenuItem});
            this.windowProcessMenuItem.Text = "&Window";
            // 
            // bringToFrontProcessMenuItem
            // 
            this.bringToFrontProcessMenuItem.Index = 0;
            this.bringToFrontProcessMenuItem.Text = "&Bring to Front";
            this.bringToFrontProcessMenuItem.Click += new System.EventHandler(this.bringToFrontProcessMenuItem_Click);
            // 
            // restoreProcessMenuItem
            // 
            this.restoreProcessMenuItem.Index = 1;
            this.restoreProcessMenuItem.Text = "&Restore";
            this.restoreProcessMenuItem.Click += new System.EventHandler(this.restoreProcessMenuItem_Click);
            // 
            // minimizeProcessMenuItem
            // 
            this.minimizeProcessMenuItem.Index = 2;
            this.minimizeProcessMenuItem.Text = "&Minimize";
            this.minimizeProcessMenuItem.Click += new System.EventHandler(this.minimizeProcessMenuItem_Click);
            // 
            // maximizeProcessMenuItem
            // 
            this.maximizeProcessMenuItem.Index = 3;
            this.maximizeProcessMenuItem.Text = "Ma&ximize";
            this.maximizeProcessMenuItem.Click += new System.EventHandler(this.maximizeProcessMenuItem_Click);
            // 
            // menuItem15
            // 
            this.menuItem15.Index = 4;
            this.menuItem15.Text = "-";
            // 
            // closeProcessMenuItem
            // 
            this.closeProcessMenuItem.Index = 5;
            this.closeProcessMenuItem.Text = "&Close";
            this.closeProcessMenuItem.Click += new System.EventHandler(this.closeProcessMenuItem_Click);
            // 
            // propertiesProcessMenuItem
            // 
            this.propertiesProcessMenuItem.DefaultItem = true;
            this.propertiesProcessMenuItem.Index = 15;
            this.propertiesProcessMenuItem.Text = "&Properties";
            this.propertiesProcessMenuItem.Click += new System.EventHandler(this.propertiesProcessMenuItem_Click);
            // 
            // menuItem7
            // 
            this.menuItem7.Index = 16;
            this.menuItem7.Text = "-";
            // 
            // searchProcessMenuItem
            // 
            this.searchProcessMenuItem.Index = 17;
            this.searchProcessMenuItem.Shortcut = System.Windows.Forms.Shortcut.CtrlM;
            this.searchProcessMenuItem.Text = "&Search Online";
            this.searchProcessMenuItem.Click += new System.EventHandler(this.searchProcessMenuItem_Click);
            // 
            // reanalyzeProcessMenuItem
            // 
            this.reanalyzeProcessMenuItem.Index = 18;
            this.reanalyzeProcessMenuItem.Text = "Re-analyze";
            this.reanalyzeProcessMenuItem.Click += new System.EventHandler(this.reanalyzeProcessMenuItem_Click);
            // 
            // copyProcessMenuItem
            // 
            this.copyProcessMenuItem.Index = 19;
            this.copyProcessMenuItem.Text = "&Copy";
            // 
            // selectAllProcessMenuItem
            // 
            this.selectAllProcessMenuItem.Index = 20;
            this.selectAllProcessMenuItem.Shortcut = System.Windows.Forms.Shortcut.CtrlA;
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
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabProcesses);
            this.tabControl.Controls.Add(this.tabServices);
            this.tabControl.Controls.Add(this.tabNetwork);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 49);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(804, 497);
            this.tabControl.TabIndex = 6;
            this.tabControl.SelectedIndexChanged += new System.EventHandler(this.tabControlBig_SelectedIndexChanged);
            // 
            // tabProcesses
            // 
            this.tabProcesses.Controls.Add(this.treeProcesses);
            this.tabProcesses.Location = new System.Drawing.Point(4, 22);
            this.tabProcesses.Name = "tabProcesses";
            this.tabProcesses.Padding = new System.Windows.Forms.Padding(3);
            this.tabProcesses.Size = new System.Drawing.Size(796, 471);
            this.tabProcesses.TabIndex = 0;
            this.tabProcesses.Text = "Processes";
            this.tabProcesses.UseVisualStyleBackColor = true;
            // 
            // treeProcesses
            // 
            this.treeProcesses.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeProcesses.Draw = true;
            this.treeProcesses.DumpMode = false;
            this.treeProcesses.DumpProcesses = null;
            this.treeProcesses.DumpProcessServices = null;
            this.treeProcesses.DumpServices = null;
            this.treeProcesses.DumpUserName = null;
            this.treeProcesses.Location = new System.Drawing.Point(3, 3);
            this.treeProcesses.Name = "treeProcesses";
            this.treeProcesses.Provider = null;
            this.treeProcesses.Size = new System.Drawing.Size(790, 465);
            this.treeProcesses.TabIndex = 4;
            this.treeProcesses.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeProcesses_KeyDown);
            this.treeProcesses.SelectionChanged += new System.EventHandler(this.treeProcesses_SelectionChanged);
            this.treeProcesses.NodeMouseDoubleClick += new System.EventHandler<Aga.Controls.Tree.TreeNodeAdvMouseEventArgs>(this.treeProcesses_NodeMouseDoubleClick);
            // 
            // tabServices
            // 
            this.tabServices.Controls.Add(this.listServices);
            this.tabServices.Location = new System.Drawing.Point(4, 22);
            this.tabServices.Name = "tabServices";
            this.tabServices.Padding = new System.Windows.Forms.Padding(3);
            this.tabServices.Size = new System.Drawing.Size(796, 471);
            this.tabServices.TabIndex = 1;
            this.tabServices.Text = "Services";
            this.tabServices.UseVisualStyleBackColor = true;
            // 
            // listServices
            // 
            this.listServices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listServices.Location = new System.Drawing.Point(3, 3);
            this.listServices.Name = "listServices";
            this.listServices.Provider = null;
            this.listServices.Size = new System.Drawing.Size(790, 489);
            this.listServices.TabIndex = 0;
            this.listServices.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listServices_KeyDown);
            this.listServices.DoubleClick += new System.EventHandler(this.listServices_DoubleClick);
            // 
            // tabNetwork
            // 
            this.tabNetwork.Controls.Add(this.listNetwork);
            this.tabNetwork.Location = new System.Drawing.Point(4, 22);
            this.tabNetwork.Name = "tabNetwork";
            this.tabNetwork.Padding = new System.Windows.Forms.Padding(3);
            this.tabNetwork.Size = new System.Drawing.Size(796, 471);
            this.tabNetwork.TabIndex = 2;
            this.tabNetwork.Text = "Network";
            this.tabNetwork.UseVisualStyleBackColor = true;
            // 
            // listNetwork
            // 
            this.listNetwork.ContextMenuStrip = this.contextMenuStripNetwork;
            this.listNetwork.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listNetwork.DoubleBuffered = true;
            this.listNetwork.Location = new System.Drawing.Point(3, 3);
            this.listNetwork.Name = "listNetwork";
            this.listNetwork.Provider = null;
            this.listNetwork.Size = new System.Drawing.Size(790, 465);
            this.listNetwork.TabIndex = 0;
            this.listNetwork.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listNetwork_KeyDown);
            this.listNetwork.DoubleClick += new System.EventHandler(this.listNetwork_DoubleClick);
            // 
            // contextMenuStripNetwork
            // 
            this.contextMenuStripNetwork.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.goToProcessNetworkMenuItem,
            this.toolsToolStripMenuItem1,
            this.closeNetworkMenuItem,
            this.toolStripSeparator7,
            this.copyToolStripMenuItem,
            this.selectAllNetworkMenuItem});
            this.contextMenuStripNetwork.Name = "contextMenuStripNetwork";
            this.contextMenuStripNetwork.Size = new System.Drawing.Size(153, 142);
            // 
            // goToProcessNetworkMenuItem
            // 
            this.goToProcessNetworkMenuItem.Name = "goToProcessNetworkMenuItem";
            this.goToProcessNetworkMenuItem.Size = new System.Drawing.Size(152, 22);
            this.goToProcessNetworkMenuItem.Text = "&Go to Process";
            this.goToProcessNetworkMenuItem.Click += new System.EventHandler(this.goToProcessNetworkMenuItem_Click);
            // 
            // toolsToolStripMenuItem1
            // 
            this.toolsToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.whoisNetworkMenuItem,
            this.tracertNetworkMenuItem,
            this.pingNetworkMenuItem});
            this.toolsToolStripMenuItem1.Name = "toolsToolStripMenuItem1";
            this.toolsToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.toolsToolStripMenuItem1.Text = "Tools";
            // 
            // whoisNetworkMenuItem
            // 
            this.whoisNetworkMenuItem.Name = "whoisNetworkMenuItem";
            this.whoisNetworkMenuItem.Size = new System.Drawing.Size(111, 22);
            this.whoisNetworkMenuItem.Text = "Whois";
            this.whoisNetworkMenuItem.Click += new System.EventHandler(this.whoisNetworkMenuItem_Click);
            // 
            // tracertNetworkMenuItem
            // 
            this.tracertNetworkMenuItem.Name = "tracertNetworkMenuItem";
            this.tracertNetworkMenuItem.Size = new System.Drawing.Size(111, 22);
            this.tracertNetworkMenuItem.Text = "Tracert";
            this.tracertNetworkMenuItem.Click += new System.EventHandler(this.tracertNetworkMenuItem_Click);
            // 
            // pingNetworkMenuItem
            // 
            this.pingNetworkMenuItem.Name = "pingNetworkMenuItem";
            this.pingNetworkMenuItem.Size = new System.Drawing.Size(111, 22);
            this.pingNetworkMenuItem.Text = "Ping";
            this.pingNetworkMenuItem.Click += new System.EventHandler(this.pingNetworkMenuItem_Click);
            // 
            // closeNetworkMenuItem
            // 
            this.closeNetworkMenuItem.Name = "closeNetworkMenuItem";
            this.closeNetworkMenuItem.Size = new System.Drawing.Size(152, 22);
            this.closeNetworkMenuItem.Text = "Close";
            this.closeNetworkMenuItem.Click += new System.EventHandler(this.closeNetworkMenuItem_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(149, 6);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            // 
            // selectAllNetworkMenuItem
            // 
            this.selectAllNetworkMenuItem.Name = "selectAllNetworkMenuItem";
            this.selectAllNetworkMenuItem.Size = new System.Drawing.Size(152, 22);
            this.selectAllNetworkMenuItem.Text = "Select All";
            this.selectAllNetworkMenuItem.Click += new System.EventHandler(this.selectAllNetworkMenuItem_Click);
            // 
            // toolStrip
            // 
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.refreshToolStripButton,
            this.optionsToolStripButton,
            this.shutDownToolStripMenuItem,
            this.toolStripSeparator1,
            this.findHandlesToolStripButton,
            this.sysInfoToolStripButton,
            this.toolStripTextBox2});
            this.toolStrip.Location = new System.Drawing.Point(0, 24);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(804, 25);
            this.toolStrip.TabIndex = 5;
            this.toolStrip.Text = "toolStrip1";
            // 
            // refreshToolStripButton
            // 
            this.refreshToolStripButton.Image = global::ProcessHacker.Properties.Resources.arrow_refresh;
            this.refreshToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.refreshToolStripButton.Name = "refreshToolStripButton";
            this.refreshToolStripButton.Size = new System.Drawing.Size(66, 22);
            this.refreshToolStripButton.Text = "Refresh";
            this.refreshToolStripButton.ToolTipText = "Refresh (F5)";
            this.refreshToolStripButton.Click += new System.EventHandler(this.refreshToolStripButton_Click);
            // 
            // optionsToolStripButton
            // 
            this.optionsToolStripButton.Image = global::ProcessHacker.Properties.Resources.cog_edit;
            this.optionsToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.optionsToolStripButton.Name = "optionsToolStripButton";
            this.optionsToolStripButton.Size = new System.Drawing.Size(69, 22);
            this.optionsToolStripButton.Text = "Options";
            this.optionsToolStripButton.ToolTipText = "Options... (Ctrl+O)";
            this.optionsToolStripButton.Click += new System.EventHandler(this.optionsToolStripButton_Click);
            // 
            // shutDownToolStripMenuItem
            // 
            this.shutDownToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.shutDownToolStripMenuItem.Image = global::ProcessHacker.Properties.Resources.lightbulb_off;
            this.shutDownToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.shutDownToolStripMenuItem.Name = "shutDownToolStripMenuItem";
            this.shutDownToolStripMenuItem.Size = new System.Drawing.Size(29, 22);
            this.shutDownToolStripMenuItem.Text = "Shutdown";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // findHandlesToolStripButton
            // 
            this.findHandlesToolStripButton.Image = global::ProcessHacker.Properties.Resources.find;
            this.findHandlesToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.findHandlesToolStripButton.Name = "findHandlesToolStripButton";
            this.findHandlesToolStripButton.Size = new System.Drawing.Size(147, 22);
            this.findHandlesToolStripButton.Text = "Find Handles or DLLs...";
            this.findHandlesToolStripButton.ToolTipText = "Find Handles or DLLs... (Ctrl+F)";
            this.findHandlesToolStripButton.Click += new System.EventHandler(this.findHandlesToolStripButton_Click);
            // 
            // sysInfoToolStripButton
            // 
            this.sysInfoToolStripButton.Image = global::ProcessHacker.Properties.Resources.chart_line;
            this.sysInfoToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.sysInfoToolStripButton.Name = "sysInfoToolStripButton";
            this.sysInfoToolStripButton.Size = new System.Drawing.Size(140, 22);
            this.sysInfoToolStripButton.Text = "System Information...";
            this.sysInfoToolStripButton.ToolTipText = "System Information... (Ctrl+I)";
            this.sysInfoToolStripButton.Click += new System.EventHandler(this.sysInfoToolStripButton_Click);
            // 
            // toolStripTextBox2
            // 
            this.toolStripTextBox2.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripTextBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.toolStripTextBox2.Name = "toolStripTextBox2";
            this.toolStripTextBox2.Size = new System.Drawing.Size(180, 25);
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
            this.goToProcessServiceMenuItem.Index = 0;
            this.goToProcessServiceMenuItem.Text = "&Go to Process";
            this.goToProcessServiceMenuItem.Click += new System.EventHandler(this.goToProcessServiceMenuItem_Click);
            // 
            // startServiceMenuItem
            // 
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
            this.pauseServiceMenuItem.Index = 3;
            this.pauseServiceMenuItem.Text = "&Pause";
            this.pauseServiceMenuItem.Click += new System.EventHandler(this.pauseServiceMenuItem_Click);
            // 
            // stopServiceMenuItem
            // 
            this.stopServiceMenuItem.Index = 4;
            this.stopServiceMenuItem.Text = "S&top";
            this.stopServiceMenuItem.Click += new System.EventHandler(this.stopServiceMenuItem_Click);
            // 
            // deleteServiceMenuItem
            // 
            this.deleteServiceMenuItem.Index = 5;
            this.deleteServiceMenuItem.Shortcut = System.Windows.Forms.Shortcut.Del;
            this.deleteServiceMenuItem.Text = "Delete";
            this.deleteServiceMenuItem.Click += new System.EventHandler(this.deleteServiceMenuItem_Click);
            // 
            // propertiesServiceMenuItem
            // 
            this.propertiesServiceMenuItem.DefaultItem = true;
            this.propertiesServiceMenuItem.Index = 6;
            this.propertiesServiceMenuItem.Text = "&Properties";
            this.propertiesServiceMenuItem.Click += new System.EventHandler(this.propertiesServiceMenuItem_Click);
            // 
            // menuItem8
            // 
            this.menuItem8.Index = 7;
            this.menuItem8.Text = "-";
            // 
            // copyServiceMenuItem
            // 
            this.copyServiceMenuItem.Index = 8;
            this.copyServiceMenuItem.Text = "Copy";
            // 
            // selectAllServiceMenuItem
            // 
            this.selectAllServiceMenuItem.Index = 9;
            this.selectAllServiceMenuItem.Shortcut = System.Windows.Forms.Shortcut.CtrlA;
            this.selectAllServiceMenuItem.Text = "Select &All";
            this.selectAllServiceMenuItem.Click += new System.EventHandler(this.selectAllServiceMenuItem_Click);
            // 
            // menuIcon
            // 
            this.menuIcon.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.showHideMenuItem,
            this.sysInformationIconMenuItem,
            this.networkInfomationMenuItem,
            this.notificationsMenuItem,
            this.processesMenuItem,
            this.shutdownTrayMenuItem,
            this.exitTrayMenuItem});
            this.menuIcon.Popup += new System.EventHandler(this.menuIcon_Popup);
            // 
            // showHideMenuItem
            // 
            this.showHideMenuItem.Index = 0;
            this.showHideMenuItem.Text = "&Show/Hide Process Hacker";
            this.showHideMenuItem.Click += new System.EventHandler(this.showHideMenuItem_Click);
            // 
            // sysInformationIconMenuItem
            // 
            this.sysInformationIconMenuItem.Index = 1;
            this.sysInformationIconMenuItem.Text = "System &Information";
            this.sysInformationIconMenuItem.Click += new System.EventHandler(this.sysInformationIconMenuItem_Click);
            // 
            // networkInfomationMenuItem
            // 
            this.networkInfomationMenuItem.Index = 2;
            this.networkInfomationMenuItem.Text = "Network Infomation";
            this.networkInfomationMenuItem.Click += new System.EventHandler(this.networkInfomationMenuItem_Click);
            // 
            // notificationsMenuItem
            // 
            this.notificationsMenuItem.Index = 3;
            this.notificationsMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.enableAllNotificationsMenuItem,
            this.disableAllNotificationsMenuItem,
            this.menuItem4,
            this.NPMenuItem,
            this.TPMenuItem,
            this.NSMenuItem,
            this.startedSMenuItem,
            this.stoppedSMenuItem,
            this.DSMenuItem});
            this.notificationsMenuItem.Text = "&Notifications";
            // 
            // enableAllNotificationsMenuItem
            // 
            this.enableAllNotificationsMenuItem.Index = 0;
            this.enableAllNotificationsMenuItem.Text = "&Enable All";
            this.enableAllNotificationsMenuItem.Click += new System.EventHandler(this.enableAllNotificationsMenuItem_Click);
            // 
            // disableAllNotificationsMenuItem
            // 
            this.disableAllNotificationsMenuItem.Index = 1;
            this.disableAllNotificationsMenuItem.Text = "&Disable All";
            this.disableAllNotificationsMenuItem.Click += new System.EventHandler(this.disableAllNotificationsMenuItem_Click);
            // 
            // menuItem4
            // 
            this.menuItem4.Index = 2;
            this.menuItem4.Text = "-";
            // 
            // NPMenuItem
            // 
            this.NPMenuItem.Index = 3;
            this.NPMenuItem.Text = "New Processes";
            // 
            // TPMenuItem
            // 
            this.TPMenuItem.Index = 4;
            this.TPMenuItem.Text = "Terminated Processes";
            // 
            // NSMenuItem
            // 
            this.NSMenuItem.Index = 5;
            this.NSMenuItem.Text = "New Services";
            // 
            // startedSMenuItem
            // 
            this.startedSMenuItem.Index = 6;
            this.startedSMenuItem.Text = "Started Services";
            // 
            // stoppedSMenuItem
            // 
            this.stoppedSMenuItem.Index = 7;
            this.stoppedSMenuItem.Text = "Stopped Services";
            // 
            // DSMenuItem
            // 
            this.DSMenuItem.Index = 8;
            this.DSMenuItem.Text = "Deleted Services";
            // 
            // processesMenuItem
            // 
            this.processesMenuItem.Index = 4;
            this.processesMenuItem.Text = "&Processes";
            // 
            // shutdownTrayMenuItem
            // 
            this.shutdownTrayMenuItem.Index = 5;
            this.shutdownTrayMenuItem.Text = "Shutdown";
            // 
            // exitTrayMenuItem
            // 
            this.exitTrayMenuItem.Index = 6;
            this.exitTrayMenuItem.Text = "E&xit";
            this.exitTrayMenuItem.Click += new System.EventHandler(this.exitTrayMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.AllowItemReorder = true;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusMemory,
            this.statusCPU,
            this.statusGeneral});
            this.statusStrip1.Location = new System.Drawing.Point(0, 546);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.ManagerRenderMode;
            this.statusStrip1.Size = new System.Drawing.Size(804, 22);
            this.statusStrip1.TabIndex = 7;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // statusMemory
            // 
            this.statusMemory.Name = "statusMemory";
            this.statusMemory.Size = new System.Drawing.Size(52, 17);
            this.statusMemory.Text = "Memory";
            // 
            // statusCPU
            // 
            this.statusCPU.Name = "statusCPU";
            this.statusCPU.Size = new System.Drawing.Size(30, 17);
            this.statusCPU.Text = "CPU";
            // 
            // statusGeneral
            // 
            this.statusGeneral.Name = "statusGeneral";
            this.statusGeneral.Size = new System.Drawing.Size(47, 17);
            this.statusGeneral.Text = "General";
            // 
            // menuStripEx1
            // 
            this.menuStripEx1.ClickThrough = true;
            this.menuStripEx1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.hackerToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.usersToolStripMenuItem,
            this.windowToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStripEx1.Location = new System.Drawing.Point(0, 0);
            this.menuStripEx1.Name = "menuStripEx1";
            this.menuStripEx1.Size = new System.Drawing.Size(804, 24);
            this.menuStripEx1.TabIndex = 8;
            this.menuStripEx1.Text = "menuStripEx1";
            // 
            // hackerToolStripMenuItem
            // 
            this.hackerToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.runToolStripMenuItem,
            this.runAsToolStripMenuItem,
            this.runAsAdministratorMenuItem,
            this.showDetailsForAllProcessesMenuItem,
            this.toolStripSeparator4,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.toolStripSeparator6,
            this.findHandlesMenuItem,
            this.inspectPEFileToolStripMenuItem,
            this.optionsMenuItem,
            this.toolStripSeparator5,
            this.shutdownMenuItem,
            this.exitToolStripMenuItem});
            this.hackerToolStripMenuItem.Name = "hackerToolStripMenuItem";
            this.hackerToolStripMenuItem.Size = new System.Drawing.Size(56, 20);
            this.hackerToolStripMenuItem.Text = "Hacker";
            // 
            // runToolStripMenuItem
            // 
            this.runToolStripMenuItem.Name = "runToolStripMenuItem";
            this.runToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.runToolStripMenuItem.Text = "Run...";
            this.runToolStripMenuItem.Click += new System.EventHandler(this.runMenuItem_Click);
            // 
            // runAsToolStripMenuItem
            // 
            this.runAsToolStripMenuItem.Name = "runAsToolStripMenuItem";
            this.runAsToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.runAsToolStripMenuItem.Text = "Run As...";
            this.runAsToolStripMenuItem.Click += new System.EventHandler(this.runAsServiceMenuItem_Click);
            // 
            // runAsAdministratorMenuItem
            // 
            this.runAsAdministratorMenuItem.Name = "runAsAdministratorMenuItem";
            this.runAsAdministratorMenuItem.Size = new System.Drawing.Size(230, 22);
            this.runAsAdministratorMenuItem.Text = "Run As Administrator...";
            this.runAsAdministratorMenuItem.Click += new System.EventHandler(this.runAsAdministratorMenuItem_Click);
            // 
            // showDetailsForAllProcessesMenuItem
            // 
            this.showDetailsForAllProcessesMenuItem.Name = "showDetailsForAllProcessesMenuItem";
            this.showDetailsForAllProcessesMenuItem.Size = new System.Drawing.Size(230, 22);
            this.showDetailsForAllProcessesMenuItem.Text = "Show Details for All Processes";
            this.showDetailsForAllProcessesMenuItem.Click += new System.EventHandler(this.showDetailsForAllProcessesMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(227, 6);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveMenuItem_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(227, 6);
            // 
            // findHandlesMenuItem
            // 
            this.findHandlesMenuItem.Name = "findHandlesMenuItem";
            this.findHandlesMenuItem.Size = new System.Drawing.Size(230, 22);
            this.findHandlesMenuItem.Text = "&Find Handles or DLLs...";
            this.findHandlesMenuItem.Click += new System.EventHandler(this.findHandlesMenuItem_Click);
            // 
            // inspectPEFileToolStripMenuItem
            // 
            this.inspectPEFileToolStripMenuItem.Name = "inspectPEFileToolStripMenuItem";
            this.inspectPEFileToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.inspectPEFileToolStripMenuItem.Text = "Inspect &PE File...";
            this.inspectPEFileToolStripMenuItem.Click += new System.EventHandler(this.inspectPEFileMenuItem_Click);
            // 
            // optionsMenuItem
            // 
            this.optionsMenuItem.Name = "optionsMenuItem";
            this.optionsMenuItem.Size = new System.Drawing.Size(230, 22);
            this.optionsMenuItem.Text = "&Options...";
            this.optionsMenuItem.Click += new System.EventHandler(this.optionsMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(227, 6);
            // 
            // shutdownMenuItem
            // 
            this.shutdownMenuItem.Name = "shutdownMenuItem";
            this.shutdownMenuItem.Size = new System.Drawing.Size(230, 22);
            this.shutdownMenuItem.Text = "Shutdown";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolbarMenuItem,
            this.sysInfoMenuItem,
            this.trayIconsToolStripMenuItem,
            this.toolStripSeparator3,
            this.refreshToolStripMenuItem,
            this.updateProcessesMenuItem,
            this.updateServicesMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // toolbarMenuItem
            // 
            this.toolbarMenuItem.Name = "toolbarMenuItem";
            this.toolbarMenuItem.Size = new System.Drawing.Size(178, 22);
            this.toolbarMenuItem.Text = "Toolbar";
            this.toolbarMenuItem.Click += new System.EventHandler(this.toolbarMenuItem_Click);
            // 
            // sysInfoMenuItem
            // 
            this.sysInfoMenuItem.Name = "sysInfoMenuItem";
            this.sysInfoMenuItem.Size = new System.Drawing.Size(178, 22);
            this.sysInfoMenuItem.Text = "System &Information";
            this.sysInfoMenuItem.Click += new System.EventHandler(this.sysInfoMenuItem_Click);
            // 
            // trayIconsToolStripMenuItem
            // 
            this.trayIconsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cpuHistoryMenuItem,
            this.cpuUsageMenuItem,
            this.ioHistoryMenuItem,
            this.commitHistoryMenuItem,
            this.physMemHistoryMenuItem});
            this.trayIconsToolStripMenuItem.Name = "trayIconsToolStripMenuItem";
            this.trayIconsToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.trayIconsToolStripMenuItem.Text = "Tray Icons";
            // 
            // cpuHistoryMenuItem
            // 
            this.cpuHistoryMenuItem.Name = "cpuHistoryMenuItem";
            this.cpuHistoryMenuItem.Size = new System.Drawing.Size(206, 22);
            this.cpuHistoryMenuItem.Text = "CPU History";
            this.cpuHistoryMenuItem.Click += new System.EventHandler(this.cpuHistoryMenuItem_Click);
            // 
            // cpuUsageMenuItem
            // 
            this.cpuUsageMenuItem.Name = "cpuUsageMenuItem";
            this.cpuUsageMenuItem.Size = new System.Drawing.Size(206, 22);
            this.cpuUsageMenuItem.Text = "CPU Usage";
            this.cpuUsageMenuItem.Click += new System.EventHandler(this.cpuUsageMenuItem_Click);
            // 
            // ioHistoryMenuItem
            // 
            this.ioHistoryMenuItem.Name = "ioHistoryMenuItem";
            this.ioHistoryMenuItem.Size = new System.Drawing.Size(206, 22);
            this.ioHistoryMenuItem.Text = "I/O History";
            this.ioHistoryMenuItem.Click += new System.EventHandler(this.ioHistoryMenuItem_Click);
            // 
            // commitHistoryMenuItem
            // 
            this.commitHistoryMenuItem.Name = "commitHistoryMenuItem";
            this.commitHistoryMenuItem.Size = new System.Drawing.Size(206, 22);
            this.commitHistoryMenuItem.Text = "Commit History";
            this.commitHistoryMenuItem.Click += new System.EventHandler(this.commitHistoryMenuItem_Click);
            // 
            // physMemHistoryMenuItem
            // 
            this.physMemHistoryMenuItem.Name = "physMemHistoryMenuItem";
            this.physMemHistoryMenuItem.Size = new System.Drawing.Size(206, 22);
            this.physMemHistoryMenuItem.Text = "Physical Memory History";
            this.physMemHistoryMenuItem.Click += new System.EventHandler(this.physMemHistoryMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(175, 6);
            // 
            // refreshToolStripMenuItem
            // 
            this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            this.refreshToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.refreshToolStripMenuItem.Text = "Refresh";
            this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripButton_Click);
            // 
            // updateProcessesMenuItem
            // 
            this.updateProcessesMenuItem.Name = "updateProcessesMenuItem";
            this.updateProcessesMenuItem.Size = new System.Drawing.Size(178, 22);
            this.updateProcessesMenuItem.Text = "Update &Processes";
            this.updateProcessesMenuItem.Click += new System.EventHandler(this.updateProcessesMenuItem_Click);
            // 
            // updateServicesMenuItem
            // 
            this.updateServicesMenuItem.Name = "updateServicesMenuItem";
            this.updateServicesMenuItem.Size = new System.Drawing.Size(178, 22);
            this.updateServicesMenuItem.Text = "Update &Services";
            this.updateServicesMenuItem.Click += new System.EventHandler(this.updateServicesMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createServiceToolStripMenuItem,
            this.hiddenProcessesToolStripMenuItem,
            this.verifyFileSignatureToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // createServiceToolStripMenuItem
            // 
            this.createServiceToolStripMenuItem.Name = "createServiceToolStripMenuItem";
            this.createServiceToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.createServiceToolStripMenuItem.Text = "Create &Service";
            this.createServiceToolStripMenuItem.Click += new System.EventHandler(this.createServiceMenuItem_Click);
            // 
            // hiddenProcessesToolStripMenuItem
            // 
            this.hiddenProcessesToolStripMenuItem.Name = "hiddenProcessesToolStripMenuItem";
            this.hiddenProcessesToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.hiddenProcessesToolStripMenuItem.Text = "&Hidden Processes";
            this.hiddenProcessesToolStripMenuItem.Click += new System.EventHandler(this.hiddenProcessesMenuItem_Click);
            // 
            // verifyFileSignatureToolStripMenuItem
            // 
            this.verifyFileSignatureToolStripMenuItem.Name = "verifyFileSignatureToolStripMenuItem";
            this.verifyFileSignatureToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.verifyFileSignatureToolStripMenuItem.Text = "&Verify File Signature...";
            this.verifyFileSignatureToolStripMenuItem.Click += new System.EventHandler(this.verifyFileSignatureMenuItem_Click);
            // 
            // usersToolStripMenuItem
            // 
            this.usersToolStripMenuItem.Name = "usersToolStripMenuItem";
            this.usersToolStripMenuItem.Size = new System.Drawing.Size(47, 20);
            this.usersToolStripMenuItem.Text = "Users";
            // 
            // windowToolStripMenuItem
            // 
            this.windowToolStripMenuItem.Name = "windowToolStripMenuItem";
            this.windowToolStripMenuItem.Size = new System.Drawing.Size(63, 20);
            this.windowToolStripMenuItem.Text = "Window";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.checkForUpdatesMenuItem,
            this.toolStripSeparator2,
            this.logToolStripMenuItem,
            this.helpToolStripMenuItem1,
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // checkForUpdatesMenuItem
            // 
            this.checkForUpdatesMenuItem.Enabled = false;
            this.checkForUpdatesMenuItem.Name = "checkForUpdatesMenuItem";
            this.checkForUpdatesMenuItem.Size = new System.Drawing.Size(173, 22);
            this.checkForUpdatesMenuItem.Text = "Check For Updates";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(170, 6);
            // 
            // logToolStripMenuItem
            // 
            this.logToolStripMenuItem.Name = "logToolStripMenuItem";
            this.logToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.logToolStripMenuItem.Text = "Log";
            this.logToolStripMenuItem.Click += new System.EventHandler(this.logMenuItem_Click);
            // 
            // helpToolStripMenuItem1
            // 
            this.helpToolStripMenuItem1.Name = "helpToolStripMenuItem1";
            this.helpToolStripMenuItem1.Size = new System.Drawing.Size(173, 22);
            this.helpToolStripMenuItem1.Text = "Help";
            this.helpToolStripMenuItem1.Click += new System.EventHandler(this.helpMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutMenuItem_Click);
            // 
            // HackerWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(804, 568);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStripEx1);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStripEx1;
            this.Name = "HackerWindow";
            this.Text = "Process Hacker";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.HackerWindow_FormClosing);
            this.Load += new System.EventHandler(this.HackerWindow_Load);
            this.SizeChanged += new System.EventHandler(this.HackerWindow_SizeChanged);
            this.VisibleChanged += new System.EventHandler(this.HackerWindow_VisibleChanged);
            this.tabControl.ResumeLayout(false);
            this.tabProcesses.ResumeLayout(false);
            this.tabServices.ResumeLayout(false);
            this.tabNetwork.ResumeLayout(false);
            this.contextMenuStripNetwork.ResumeLayout(false);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStripEx1.ResumeLayout(false);
            this.menuStripEx1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

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
        private ProcessHacker.ProcessTree treeProcesses;
        private System.Windows.Forms.MenuItem propertiesProcessMenuItem;
        private System.Windows.Forms.MenuItem searchProcessMenuItem;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabProcesses;
        private System.Windows.Forms.TabPage tabServices;
        private ProcessHacker.Components.ServiceList listServices;
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
        private System.Windows.Forms.MenuItem affinityProcessMenuItem;
        private System.Windows.Forms.MenuItem runAsProcessMenuItem;
        private System.Windows.Forms.MenuItem launchAsUserProcessMenuItem;
        private System.Windows.Forms.MenuItem launchAsThisUserProcessMenuItem;
        private System.Windows.Forms.MenuItem copyProcessMenuItem;
        private System.Windows.Forms.MenuItem selectAllProcessMenuItem;
        private System.Windows.Forms.MenuItem terminatorProcessMenuItem;
        private System.Windows.Forms.TabPage tabNetwork;
        private ProcessHacker.Components.NetworkList listNetwork;
        private System.Windows.Forms.MenuItem sysInformationIconMenuItem;
        private System.Windows.Forms.MenuItem processesMenuItem;
        private System.Windows.Forms.MenuItem restartProcessMenuItem;
        private System.Windows.Forms.MenuItem setTokenProcessMenuItem;
        private System.Windows.Forms.MenuItem enableAllNotificationsMenuItem;
        private System.Windows.Forms.MenuItem disableAllNotificationsMenuItem;
        private System.Windows.Forms.MenuItem menuItem4;
        private System.Windows.Forms.MenuItem shutdownTrayMenuItem;
        private System.Windows.Forms.MenuItem reanalyzeProcessMenuItem;
        private System.Windows.Forms.MenuItem reduceWorkingSetProcessMenuItem;
        private System.Windows.Forms.MenuItem virtualizationProcessMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton refreshToolStripButton;
        private System.Windows.Forms.ToolStripButton findHandlesToolStripButton;
        private System.Windows.Forms.ToolStripButton sysInfoToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripDropDownButton shutDownToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton optionsToolStripButton;
        private System.Windows.Forms.MenuItem injectDllProcessMenuItem;
        private System.Windows.Forms.MenuItem terminateProcessTreeMenuItem;
        private System.Windows.Forms.MenuItem protectionProcessMenuItem;
        private System.Windows.Forms.MenuItem createDumpFileProcessMenuItem;
        private System.Windows.Forms.MenuItem miscellaneousProcessMenuItem;
        private System.Windows.Forms.MenuItem detachFromDebuggerProcessMenuItem;
        private System.Windows.Forms.MenuItem heapsProcessMenuItem;
        private System.Windows.Forms.MenuItem windowProcessMenuItem;
        private System.Windows.Forms.MenuItem bringToFrontProcessMenuItem;
        private System.Windows.Forms.MenuItem restoreProcessMenuItem;
        private System.Windows.Forms.MenuItem minimizeProcessMenuItem;
        private System.Windows.Forms.MenuItem maximizeProcessMenuItem;
        private System.Windows.Forms.MenuItem menuItem15;
        private System.Windows.Forms.MenuItem closeProcessMenuItem;
        private System.Windows.Forms.MenuItem VirusTotalMenuItem;
        private System.Windows.Forms.MenuItem networkInfomationMenuItem;
        private System.Windows.Forms.MenuItem analyzeWaitChainProcessMenuItem;
        private System.Windows.Forms.MenuItem ioPriorityThreadMenuItem;
        private System.Windows.Forms.MenuItem ioPriority0ThreadMenuItem;
        private System.Windows.Forms.MenuItem ioPriority1ThreadMenuItem;
        private System.Windows.Forms.MenuItem ioPriority2ThreadMenuItem;
        private System.Windows.Forms.MenuItem ioPriority3ThreadMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel statusMemory;
        private System.Windows.Forms.ToolStripStatusLabel statusCPU;
        private System.Windows.Forms.ToolStripStatusLabel statusGeneral;
        private System.MenuStripEx menuStripEx1;
        private System.Windows.Forms.ToolStripMenuItem hackerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem usersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem windowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkForUpdatesMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem logToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox2;
        private System.Windows.Forms.ToolStripMenuItem toolbarMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sysInfoMenuItem;
        private System.Windows.Forms.ToolStripMenuItem trayIconsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem updateProcessesMenuItem;
        private System.Windows.Forms.ToolStripMenuItem updateServicesMenuItem;
        private System.Windows.Forms.ToolStripMenuItem runToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem runAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem runAsAdministratorMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem shutdownMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showDetailsForAllProcessesMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem findHandlesMenuItem;
        private System.Windows.Forms.ToolStripMenuItem inspectPEFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createServiceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hiddenProcessesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem verifyFileSignatureToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cpuHistoryMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cpuUsageMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ioHistoryMenuItem;
        private System.Windows.Forms.ToolStripMenuItem commitHistoryMenuItem;
        private System.Windows.Forms.ToolStripMenuItem physMemHistoryMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripNetwork;
        private System.Windows.Forms.ToolStripMenuItem goToProcessNetworkMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem closeNetworkMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectAllNetworkMenuItem;
        private System.Windows.Forms.ToolStripMenuItem whoisNetworkMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tracertNetworkMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pingNetworkMenuItem;
    }
}

