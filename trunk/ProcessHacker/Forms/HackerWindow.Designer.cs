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
            this.suspendMenuItem = new System.Windows.Forms.MenuItem();
            this.resumeMenuItem = new System.Windows.Forms.MenuItem();
            this.restartProcessMenuItem = new System.Windows.Forms.MenuItem();
            this.reduceWorkingSetProcessMenuItem = new System.Windows.Forms.MenuItem();
            this.virtualizationProcessMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem5 = new System.Windows.Forms.MenuItem();
            this.affinityProcessMenuItem = new System.Windows.Forms.MenuItem();
            this.injectDllProcessMenuItem = new System.Windows.Forms.MenuItem();
            this.setTokenProcessMenuItem = new System.Windows.Forms.MenuItem();
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
            this.mainMenu = new System.Windows.Forms.MainMenu(this.components);
            this.hackerMenuItem = new System.Windows.Forms.MenuItem();
            this.runMenuItem = new System.Windows.Forms.MenuItem();
            this.runAsAdministratorMenuItem = new System.Windows.Forms.MenuItem();
            this.runAsMenuItem = new System.Windows.Forms.MenuItem();
            this.runAsServiceMenuItem = new System.Windows.Forms.MenuItem();
            this.showDetailsForAllProcessesMenuItem = new System.Windows.Forms.MenuItem();
            this.uacSeparatorMenuItem = new System.Windows.Forms.MenuItem();
            this.saveMenuItem = new System.Windows.Forms.MenuItem();
            this.apiLoggerMenuItem = new System.Windows.Forms.MenuItem();
            this.findHandlesMenuItem = new System.Windows.Forms.MenuItem();
            this.inspectPEFileMenuItem = new System.Windows.Forms.MenuItem();
            this.reloadStructsMenuItem = new System.Windows.Forms.MenuItem();
            this.optionsMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.shutdownMenuItem = new System.Windows.Forms.MenuItem();
            this.exitMenuItem = new System.Windows.Forms.MenuItem();
            this.viewMenuItem = new System.Windows.Forms.MenuItem();
            this.toolbarMenuItem = new System.Windows.Forms.MenuItem();
            this.sysInfoMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.updateNowMenuItem = new System.Windows.Forms.MenuItem();
            this.updateProcessesMenuItem = new System.Windows.Forms.MenuItem();
            this.updateServicesMenuItem = new System.Windows.Forms.MenuItem();
            this.toolsMenuItem = new System.Windows.Forms.MenuItem();
            this.csrProcessesMenuItem = new System.Windows.Forms.MenuItem();
            this.verifyFileSignatureMenuItem = new System.Windows.Forms.MenuItem();
            this.windowMenuItem = new System.Windows.Forms.MenuItem();
            this.helpMenu = new System.Windows.Forms.MenuItem();
            this.freeMemoryMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.logMenuItem = new System.Windows.Forms.MenuItem();
            this.helpMenuItem = new System.Windows.Forms.MenuItem();
            this.aboutMenuItem = new System.Windows.Forms.MenuItem();
            this.statusBar = new System.Windows.Forms.StatusBar();
            this.statusGeneral = new System.Windows.Forms.StatusBarPanel();
            this.statusCPU = new System.Windows.Forms.StatusBarPanel();
            this.statusMemory = new System.Windows.Forms.StatusBarPanel();
            this.statusIcon = new System.Windows.Forms.StatusBarPanel();
            this.statusText = new System.Windows.Forms.StatusBarPanel();
            this.timerMessages = new System.Windows.Forms.Timer(this.components);
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabProcesses = new System.Windows.Forms.TabPage();
            this.treeProcesses = new ProcessHacker.ProcessTree();
            this.tabServices = new System.Windows.Forms.TabPage();
            this.listServices = new ProcessHacker.ServiceList();
            this.tabNetwork = new System.Windows.Forms.TabPage();
            this.listNetwork = new ProcessHacker.NetworkList();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.refreshToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.optionsToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.shutDownToolStripMenuItem = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.findHandlesToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.sysInfoToolStripButton = new System.Windows.Forms.ToolStripButton();
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
            this.sysInformationIconMenuItem = new System.Windows.Forms.MenuItem();
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
            this.vistaMenu = new wyDay.Controls.VistaMenu(this.components);
            this.goToProcessNetworkMenuItem = new System.Windows.Forms.MenuItem();
            this.copyNetworkMenuItem = new System.Windows.Forms.MenuItem();
            this.panelHack = new System.Windows.Forms.Panel();
            this.menuNetwork = new System.Windows.Forms.ContextMenu();
            this.menuItem6 = new System.Windows.Forms.MenuItem();
            this.selectAllNetworkMenuItem = new System.Windows.Forms.MenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.statusGeneral)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusCPU)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusMemory)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusText)).BeginInit();
            this.tabControl.SuspendLayout();
            this.tabProcesses.SuspendLayout();
            this.tabServices.SuspendLayout();
            this.tabNetwork.SuspendLayout();
            this.toolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.vistaMenu)).BeginInit();
            this.panelHack.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuProcess
            // 
            this.menuProcess.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.terminateMenuItem,
            this.suspendMenuItem,
            this.resumeMenuItem,
            this.restartProcessMenuItem,
            this.reduceWorkingSetProcessMenuItem,
            this.virtualizationProcessMenuItem,
            this.menuItem5,
            this.affinityProcessMenuItem,
            this.injectDllProcessMenuItem,
            this.setTokenProcessMenuItem,
            this.terminatorProcessMenuItem,
            this.priorityMenuItem,
            this.runAsProcessMenuItem,
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
            // restartProcessMenuItem
            // 
            this.restartProcessMenuItem.Index = 3;
            this.restartProcessMenuItem.Text = "Restart";
            this.restartProcessMenuItem.Click += new System.EventHandler(this.restartProcessMenuItem_Click);
            // 
            // reduceWorkingSetProcessMenuItem
            // 
            this.reduceWorkingSetProcessMenuItem.Index = 4;
            this.reduceWorkingSetProcessMenuItem.Text = "Reduce Working Set";
            this.reduceWorkingSetProcessMenuItem.Click += new System.EventHandler(this.reduceWorkingSetProcessMenuItem_Click);
            // 
            // virtualizationProcessMenuItem
            // 
            this.virtualizationProcessMenuItem.Index = 5;
            this.virtualizationProcessMenuItem.Text = "Virtualization";
            this.virtualizationProcessMenuItem.Click += new System.EventHandler(this.virtualizationProcessMenuItem_Click);
            // 
            // menuItem5
            // 
            this.menuItem5.Index = 6;
            this.menuItem5.Text = "-";
            // 
            // affinityProcessMenuItem
            // 
            this.affinityProcessMenuItem.Index = 7;
            this.affinityProcessMenuItem.Text = "Affinity...";
            this.affinityProcessMenuItem.Click += new System.EventHandler(this.affinityProcessMenuItem_Click);
            // 
            // injectDllProcessMenuItem
            // 
            this.injectDllProcessMenuItem.Index = 8;
            this.injectDllProcessMenuItem.Text = "Inject DLL...";
            this.injectDllProcessMenuItem.Click += new System.EventHandler(this.injectDllProcessMenuItem_Click);
            // 
            // setTokenProcessMenuItem
            // 
            this.setTokenProcessMenuItem.Index = 9;
            this.setTokenProcessMenuItem.Text = "Set Token...";
            this.setTokenProcessMenuItem.Click += new System.EventHandler(this.setTokenProcessMenuItem_Click);
            // 
            // terminatorProcessMenuItem
            // 
            this.terminatorProcessMenuItem.Index = 10;
            this.terminatorProcessMenuItem.Text = "Terminator...";
            this.terminatorProcessMenuItem.Click += new System.EventHandler(this.terminatorProcessMenuItem_Click);
            // 
            // priorityMenuItem
            // 
            this.vistaMenu.SetImage(this.priorityMenuItem, global::ProcessHacker.Properties.Resources.control_equalizer_blue);
            this.priorityMenuItem.Index = 11;
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
            this.runAsProcessMenuItem.Index = 12;
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
            this.propertiesProcessMenuItem.DefaultItem = true;
            this.vistaMenu.SetImage(this.propertiesProcessMenuItem, global::ProcessHacker.Properties.Resources.application_form_magnify);
            this.propertiesProcessMenuItem.Index = 13;
            this.propertiesProcessMenuItem.Text = "&Properties...";
            this.propertiesProcessMenuItem.Click += new System.EventHandler(this.inspectProcessMenuItem_Click);
            // 
            // menuItem7
            // 
            this.menuItem7.Index = 14;
            this.menuItem7.Text = "-";
            // 
            // searchProcessMenuItem
            // 
            this.searchProcessMenuItem.Index = 15;
            this.searchProcessMenuItem.Text = "&Search Online...";
            this.searchProcessMenuItem.Click += new System.EventHandler(this.searchProcessMenuItem_Click);
            // 
            // reanalyzeProcessMenuItem
            // 
            this.reanalyzeProcessMenuItem.Index = 16;
            this.reanalyzeProcessMenuItem.Text = "Re-analyze";
            this.reanalyzeProcessMenuItem.Click += new System.EventHandler(this.reanalyzeProcessMenuItem_Click);
            // 
            // copyProcessMenuItem
            // 
            this.vistaMenu.SetImage(this.copyProcessMenuItem, global::ProcessHacker.Properties.Resources.page_copy);
            this.copyProcessMenuItem.Index = 17;
            this.copyProcessMenuItem.Text = "&Copy";
            // 
            // selectAllProcessMenuItem
            // 
            this.selectAllProcessMenuItem.Index = 18;
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
            // mainMenu
            // 
            this.mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.hackerMenuItem,
            this.viewMenuItem,
            this.toolsMenuItem,
            this.windowMenuItem,
            this.helpMenu});
            // 
            // hackerMenuItem
            // 
            this.hackerMenuItem.Index = 0;
            this.hackerMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.runMenuItem,
            this.runAsAdministratorMenuItem,
            this.runAsMenuItem,
            this.runAsServiceMenuItem,
            this.showDetailsForAllProcessesMenuItem,
            this.uacSeparatorMenuItem,
            this.saveMenuItem,
            this.apiLoggerMenuItem,
            this.findHandlesMenuItem,
            this.inspectPEFileMenuItem,
            this.reloadStructsMenuItem,
            this.optionsMenuItem,
            this.menuItem2,
            this.shutdownMenuItem,
            this.exitMenuItem});
            this.hackerMenuItem.Text = "&Hacker";
            // 
            // runMenuItem
            // 
            this.runMenuItem.Index = 0;
            this.runMenuItem.Shortcut = System.Windows.Forms.Shortcut.CtrlR;
            this.runMenuItem.Text = "&Run...";
            this.runMenuItem.Click += new System.EventHandler(this.runMenuItem_Click);
            // 
            // runAsAdministratorMenuItem
            // 
            this.runAsAdministratorMenuItem.Index = 1;
            this.runAsAdministratorMenuItem.Text = "Run As Administrator...";
            this.runAsAdministratorMenuItem.Click += new System.EventHandler(this.runAsAdministratorMenuItem_Click);
            // 
            // runAsMenuItem
            // 
            this.runAsMenuItem.Index = 2;
            this.runAsMenuItem.Text = "Run As...";
            this.runAsMenuItem.Visible = false;
            this.runAsMenuItem.Click += new System.EventHandler(this.runAsMenuItem_Click);
            // 
            // runAsServiceMenuItem
            // 
            this.runAsServiceMenuItem.Index = 3;
            this.runAsServiceMenuItem.Text = "Run As...";
            this.runAsServiceMenuItem.Click += new System.EventHandler(this.runAsServiceMenuItem_Click);
            // 
            // showDetailsForAllProcessesMenuItem
            // 
            this.showDetailsForAllProcessesMenuItem.Index = 4;
            this.showDetailsForAllProcessesMenuItem.Text = "Show Details for All Processes";
            this.showDetailsForAllProcessesMenuItem.Click += new System.EventHandler(this.showDetailsForAllProcessesMenuItem_Click);
            // 
            // uacSeparatorMenuItem
            // 
            this.uacSeparatorMenuItem.Index = 5;
            this.uacSeparatorMenuItem.Text = "-";
            // 
            // saveMenuItem
            // 
            this.vistaMenu.SetImage(this.saveMenuItem, global::ProcessHacker.Properties.Resources.disk);
            this.saveMenuItem.Index = 6;
            this.saveMenuItem.Shortcut = System.Windows.Forms.Shortcut.CtrlS;
            this.saveMenuItem.Text = "Save...";
            this.saveMenuItem.Click += new System.EventHandler(this.saveMenuItem_Click);
            // 
            // apiLoggerMenuItem
            // 
            this.apiLoggerMenuItem.Index = 7;
            this.apiLoggerMenuItem.Text = "API Logger...";
            this.apiLoggerMenuItem.Visible = false;
            this.apiLoggerMenuItem.Click += new System.EventHandler(this.apiLoggerMenuItem_Click);
            // 
            // findHandlesMenuItem
            // 
            this.vistaMenu.SetImage(this.findHandlesMenuItem, global::ProcessHacker.Properties.Resources.find);
            this.findHandlesMenuItem.Index = 8;
            this.findHandlesMenuItem.Shortcut = System.Windows.Forms.Shortcut.CtrlF;
            this.findHandlesMenuItem.Text = "&Find Handles...";
            this.findHandlesMenuItem.Click += new System.EventHandler(this.findHandlesMenuItem_Click);
            // 
            // inspectPEFileMenuItem
            // 
            this.vistaMenu.SetImage(this.inspectPEFileMenuItem, global::ProcessHacker.Properties.Resources.application_form_magnify);
            this.inspectPEFileMenuItem.Index = 9;
            this.inspectPEFileMenuItem.Text = "Inspect &PE File...";
            this.inspectPEFileMenuItem.Click += new System.EventHandler(this.inspectPEFileMenuItem_Click);
            // 
            // reloadStructsMenuItem
            // 
            this.vistaMenu.SetImage(this.reloadStructsMenuItem, global::ProcessHacker.Properties.Resources.arrow_refresh);
            this.reloadStructsMenuItem.Index = 10;
            this.reloadStructsMenuItem.Text = "Reload Struct Definitions";
            this.reloadStructsMenuItem.Click += new System.EventHandler(this.reloadStructsMenuItem_Click);
            // 
            // optionsMenuItem
            // 
            this.vistaMenu.SetImage(this.optionsMenuItem, global::ProcessHacker.Properties.Resources.page_gear);
            this.optionsMenuItem.Index = 11;
            this.optionsMenuItem.Shortcut = System.Windows.Forms.Shortcut.CtrlO;
            this.optionsMenuItem.Text = "&Options...";
            this.optionsMenuItem.Click += new System.EventHandler(this.optionsMenuItem_Click);
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 12;
            this.menuItem2.Text = "-";
            // 
            // shutdownMenuItem
            // 
            this.shutdownMenuItem.Index = 13;
            this.shutdownMenuItem.Text = "Shutdown";
            // 
            // exitMenuItem
            // 
            this.vistaMenu.SetImage(this.exitMenuItem, global::ProcessHacker.Properties.Resources.door_out);
            this.exitMenuItem.Index = 14;
            this.exitMenuItem.Text = "E&xit";
            this.exitMenuItem.Click += new System.EventHandler(this.exitMenuItem_Click);
            // 
            // viewMenuItem
            // 
            this.viewMenuItem.Index = 1;
            this.viewMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.toolbarMenuItem,
            this.sysInfoMenuItem,
            this.menuItem3,
            this.updateNowMenuItem,
            this.updateProcessesMenuItem,
            this.updateServicesMenuItem});
            this.viewMenuItem.Text = "&View";
            // 
            // toolbarMenuItem
            // 
            this.toolbarMenuItem.Index = 0;
            this.toolbarMenuItem.Text = "Toolbar";
            this.toolbarMenuItem.Click += new System.EventHandler(this.toolbarMenuItem_Click);
            // 
            // sysInfoMenuItem
            // 
            this.sysInfoMenuItem.Index = 1;
            this.sysInfoMenuItem.Shortcut = System.Windows.Forms.Shortcut.CtrlI;
            this.sysInfoMenuItem.Text = "System &Information...";
            this.sysInfoMenuItem.Click += new System.EventHandler(this.sysInfoMenuItem_Click);
            // 
            // menuItem3
            // 
            this.menuItem3.Index = 2;
            this.menuItem3.Text = "-";
            // 
            // updateNowMenuItem
            // 
            this.vistaMenu.SetImage(this.updateNowMenuItem, global::ProcessHacker.Properties.Resources.arrow_refresh);
            this.updateNowMenuItem.Index = 3;
            this.updateNowMenuItem.Shortcut = System.Windows.Forms.Shortcut.F5;
            this.updateNowMenuItem.Text = "&Refresh";
            this.updateNowMenuItem.Click += new System.EventHandler(this.updateNowMenuItem_Click);
            // 
            // updateProcessesMenuItem
            // 
            this.updateProcessesMenuItem.Index = 4;
            this.updateProcessesMenuItem.Text = "Update &Processes";
            this.updateProcessesMenuItem.Click += new System.EventHandler(this.updateProcessesMenuItem_Click);
            // 
            // updateServicesMenuItem
            // 
            this.updateServicesMenuItem.Index = 5;
            this.updateServicesMenuItem.Text = "Update &Services";
            this.updateServicesMenuItem.Click += new System.EventHandler(this.updateServicesMenuItem_Click);
            // 
            // toolsMenuItem
            // 
            this.toolsMenuItem.Index = 2;
            this.toolsMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.csrProcessesMenuItem,
            this.verifyFileSignatureMenuItem});
            this.toolsMenuItem.Text = "&Tools";
            // 
            // csrProcessesMenuItem
            // 
            this.csrProcessesMenuItem.Index = 0;
            this.csrProcessesMenuItem.Text = "&CSR Processes...";
            this.csrProcessesMenuItem.Click += new System.EventHandler(this.csrProcessesMenuItem_Click);
            // 
            // verifyFileSignatureMenuItem
            // 
            this.verifyFileSignatureMenuItem.Index = 1;
            this.verifyFileSignatureMenuItem.Text = "&Verify File Signature...";
            this.verifyFileSignatureMenuItem.Click += new System.EventHandler(this.verifyFileSignatureMenuItem_Click);
            // 
            // windowMenuItem
            // 
            this.windowMenuItem.Index = 3;
            this.windowMenuItem.Text = "&Window";
            // 
            // helpMenu
            // 
            this.helpMenu.Index = 4;
            this.helpMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.freeMemoryMenuItem,
            this.menuItem1,
            this.logMenuItem,
            this.helpMenuItem,
            this.aboutMenuItem});
            this.helpMenu.Text = "H&elp";
            // 
            // freeMemoryMenuItem
            // 
            this.freeMemoryMenuItem.Index = 0;
            this.freeMemoryMenuItem.Text = "Free Memory";
            this.freeMemoryMenuItem.Click += new System.EventHandler(this.freeMemoryMenuItem_Click);
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 1;
            this.menuItem1.Text = "-";
            // 
            // logMenuItem
            // 
            this.vistaMenu.SetImage(this.logMenuItem, global::ProcessHacker.Properties.Resources.page_white_text);
            this.logMenuItem.Index = 2;
            this.logMenuItem.Shortcut = System.Windows.Forms.Shortcut.CtrlL;
            this.logMenuItem.Text = "&Log...";
            this.logMenuItem.Click += new System.EventHandler(this.logMenuItem_Click);
            // 
            // helpMenuItem
            // 
            this.vistaMenu.SetImage(this.helpMenuItem, global::ProcessHacker.Properties.Resources.help);
            this.helpMenuItem.Index = 3;
            this.helpMenuItem.Shortcut = System.Windows.Forms.Shortcut.F1;
            this.helpMenuItem.Text = "&Help...";
            this.helpMenuItem.Click += new System.EventHandler(this.helpMenuItem_Click);
            // 
            // aboutMenuItem
            // 
            this.vistaMenu.SetImage(this.aboutMenuItem, global::ProcessHacker.Properties.Resources.information);
            this.aboutMenuItem.Index = 4;
            this.aboutMenuItem.Text = "&About...";
            this.aboutMenuItem.Click += new System.EventHandler(this.aboutMenuItem_Click);
            // 
            // statusBar
            // 
            this.statusBar.Location = new System.Drawing.Point(0, 404);
            this.statusBar.Name = "statusBar";
            this.statusBar.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
            this.statusGeneral,
            this.statusCPU,
            this.statusMemory,
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
            // statusCPU
            // 
            this.statusCPU.Name = "statusCPU";
            this.statusCPU.Text = "CPU: 99.99%";
            this.statusCPU.Width = 80;
            // 
            // statusMemory
            // 
            this.statusMemory.Name = "statusMemory";
            this.statusMemory.Text = "Phys. Memory: 50%";
            this.statusMemory.Width = 120;
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
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabProcesses);
            this.tabControl.Controls.Add(this.tabServices);
            this.tabControl.Controls.Add(this.tabNetwork);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(3, 3);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(798, 376);
            this.tabControl.TabIndex = 6;
            this.tabControl.SelectedIndexChanged += new System.EventHandler(this.tabControlBig_SelectedIndexChanged);
            // 
            // tabProcesses
            // 
            this.tabProcesses.Controls.Add(this.treeProcesses);
            this.tabProcesses.Location = new System.Drawing.Point(4, 22);
            this.tabProcesses.Name = "tabProcesses";
            this.tabProcesses.Padding = new System.Windows.Forms.Padding(3);
            this.tabProcesses.Size = new System.Drawing.Size(790, 350);
            this.tabProcesses.TabIndex = 0;
            this.tabProcesses.Text = "Processes";
            this.tabProcesses.UseVisualStyleBackColor = true;
            // 
            // treeProcesses
            // 
            this.treeProcesses.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeProcesses.Draw = true;
            this.treeProcesses.Location = new System.Drawing.Point(3, 3);
            this.treeProcesses.Name = "treeProcesses";
            this.treeProcesses.Provider = null;
            this.treeProcesses.Size = new System.Drawing.Size(784, 344);
            this.treeProcesses.TabIndex = 4;
            this.treeProcesses.SelectionChanged += new System.EventHandler(this.listProcesses_SelectionChanged);
            this.treeProcesses.NodeMouseDoubleClick += new System.EventHandler<Aga.Controls.Tree.TreeNodeAdvMouseEventArgs>(this.treeProcesses_NodeMouseDoubleClick);
            this.treeProcesses.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listProcesses_KeyDown);
            // 
            // tabServices
            // 
            this.tabServices.Controls.Add(this.listServices);
            this.tabServices.Location = new System.Drawing.Point(4, 22);
            this.tabServices.Name = "tabServices";
            this.tabServices.Padding = new System.Windows.Forms.Padding(3);
            this.tabServices.Size = new System.Drawing.Size(790, 350);
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
            this.listServices.Size = new System.Drawing.Size(784, 344);
            this.listServices.TabIndex = 0;
            this.listServices.DoubleClick += new System.EventHandler(this.listServices_DoubleClick);
            // 
            // tabNetwork
            // 
            this.tabNetwork.Controls.Add(this.listNetwork);
            this.tabNetwork.Location = new System.Drawing.Point(4, 22);
            this.tabNetwork.Name = "tabNetwork";
            this.tabNetwork.Padding = new System.Windows.Forms.Padding(3);
            this.tabNetwork.Size = new System.Drawing.Size(790, 350);
            this.tabNetwork.TabIndex = 2;
            this.tabNetwork.Text = "Network";
            this.tabNetwork.UseVisualStyleBackColor = true;
            // 
            // listNetwork
            // 
            this.listNetwork.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listNetwork.DoubleBuffered = true;
            this.listNetwork.Location = new System.Drawing.Point(3, 3);
            this.listNetwork.Name = "listNetwork";
            this.listNetwork.Provider = null;
            this.listNetwork.Size = new System.Drawing.Size(784, 344);
            this.listNetwork.TabIndex = 0;
            this.listNetwork.DoubleClick += new System.EventHandler(this.listNetwork_DoubleClick);
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
            this.sysInfoToolStripButton});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(804, 25);
            this.toolStrip.TabIndex = 5;
            this.toolStrip.Text = "toolStrip1";
            // 
            // refreshToolStripButton
            // 
            this.refreshToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.refreshToolStripButton.Image = global::ProcessHacker.Properties.Resources.arrow_refresh;
            this.refreshToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.refreshToolStripButton.Name = "refreshToolStripButton";
            this.refreshToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.refreshToolStripButton.Text = "Refresh";
            this.refreshToolStripButton.ToolTipText = "Refresh (F5)";
            this.refreshToolStripButton.Click += new System.EventHandler(this.refreshToolStripButton_Click);
            // 
            // optionsToolStripButton
            // 
            this.optionsToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.optionsToolStripButton.Image = global::ProcessHacker.Properties.Resources.cog_edit;
            this.optionsToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.optionsToolStripButton.Name = "optionsToolStripButton";
            this.optionsToolStripButton.Size = new System.Drawing.Size(23, 22);
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
            this.findHandlesToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.findHandlesToolStripButton.Image = global::ProcessHacker.Properties.Resources.find;
            this.findHandlesToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.findHandlesToolStripButton.Name = "findHandlesToolStripButton";
            this.findHandlesToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.findHandlesToolStripButton.Text = "Find Handles...";
            this.findHandlesToolStripButton.ToolTipText = "Find Handles... (Ctrl+F)";
            this.findHandlesToolStripButton.Click += new System.EventHandler(this.findHandlesToolStripButton_Click);
            // 
            // sysInfoToolStripButton
            // 
            this.sysInfoToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.sysInfoToolStripButton.Image = global::ProcessHacker.Properties.Resources.chart_line;
            this.sysInfoToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.sysInfoToolStripButton.Name = "sysInfoToolStripButton";
            this.sysInfoToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.sysInfoToolStripButton.Text = "System Information...";
            this.sysInfoToolStripButton.ToolTipText = "System Information... (Ctrl+I)";
            this.sysInfoToolStripButton.Click += new System.EventHandler(this.sysInfoToolStripButton_Click);
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
            this.sysInformationIconMenuItem,
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
            this.sysInformationIconMenuItem.Text = "System &Information...";
            this.sysInformationIconMenuItem.Click += new System.EventHandler(this.sysInformationIconMenuItem_Click);
            // 
            // notificationsMenuItem
            // 
            this.notificationsMenuItem.Index = 2;
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
            this.processesMenuItem.Index = 3;
            this.processesMenuItem.Text = "&Processes";
            // 
            // shutdownTrayMenuItem
            // 
            this.shutdownTrayMenuItem.Index = 4;
            this.shutdownTrayMenuItem.Text = "Shutdown";
            // 
            // exitTrayMenuItem
            // 
            this.vistaMenu.SetImage(this.exitTrayMenuItem, global::ProcessHacker.Properties.Resources.door_out);
            this.exitTrayMenuItem.Index = 5;
            this.exitTrayMenuItem.Text = "E&xit";
            this.exitTrayMenuItem.Click += new System.EventHandler(this.exitTrayMenuItem_Click);
            // 
            // vistaMenu
            // 
            this.vistaMenu.ContainerControl = this;
            // 
            // goToProcessNetworkMenuItem
            // 
            this.goToProcessNetworkMenuItem.DefaultItem = true;
            this.vistaMenu.SetImage(this.goToProcessNetworkMenuItem, global::ProcessHacker.Properties.Resources.arrow_right);
            this.goToProcessNetworkMenuItem.Index = 0;
            this.goToProcessNetworkMenuItem.Text = "&Go to Process";
            this.goToProcessNetworkMenuItem.Click += new System.EventHandler(this.goToProcessNetworkMenuItem_Click);
            // 
            // copyNetworkMenuItem
            // 
            this.vistaMenu.SetImage(this.copyNetworkMenuItem, global::ProcessHacker.Properties.Resources.page_copy);
            this.copyNetworkMenuItem.Index = 2;
            this.copyNetworkMenuItem.Text = "&Copy";
            // 
            // panelHack
            // 
            this.panelHack.Controls.Add(this.tabControl);
            this.panelHack.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelHack.Location = new System.Drawing.Point(0, 25);
            this.panelHack.Name = "panelHack";
            this.panelHack.Padding = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.panelHack.Size = new System.Drawing.Size(804, 379);
            this.panelHack.TabIndex = 5;
            // 
            // menuNetwork
            // 
            this.menuNetwork.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.goToProcessNetworkMenuItem,
            this.menuItem6,
            this.copyNetworkMenuItem,
            this.selectAllNetworkMenuItem});
            this.menuNetwork.Popup += new System.EventHandler(this.menuNetwork_Popup);
            // 
            // menuItem6
            // 
            this.menuItem6.Index = 1;
            this.menuItem6.Text = "-";
            // 
            // selectAllNetworkMenuItem
            // 
            this.selectAllNetworkMenuItem.Index = 3;
            this.selectAllNetworkMenuItem.Text = "Select &All";
            this.selectAllNetworkMenuItem.Click += new System.EventHandler(this.selectAllNetworkMenuItem_Click);
            // 
            // HackerWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(804, 426);
            this.Controls.Add(this.panelHack);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.statusBar);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Menu = this.mainMenu;
            this.Name = "HackerWindow";
            this.Text = "Process Hacker";
            this.Load += new System.EventHandler(this.HackerWindow_Load);
            this.SizeChanged += new System.EventHandler(this.HackerWindow_SizeChanged);
            this.VisibleChanged += new System.EventHandler(this.HackerWindow_VisibleChanged);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.HackerWindow_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.statusGeneral)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusCPU)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusMemory)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusText)).EndInit();
            this.tabControl.ResumeLayout(false);
            this.tabProcesses.ResumeLayout(false);
            this.tabServices.ResumeLayout(false);
            this.tabNetwork.ResumeLayout(false);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.vistaMenu)).EndInit();
            this.panelHack.ResumeLayout(false);
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
        private wyDay.Controls.VistaMenu vistaMenu;
        private System.Windows.Forms.MainMenu mainMenu;
        private System.Windows.Forms.MenuItem hackerMenuItem;
        private System.Windows.Forms.MenuItem aboutMenuItem;
        private System.Windows.Forms.MenuItem optionsMenuItem;
        private System.Windows.Forms.MenuItem helpMenuItem;
        private System.Windows.Forms.MenuItem exitMenuItem;
        private System.Windows.Forms.MenuItem windowMenuItem;
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
        private System.Windows.Forms.TabControl tabControl;
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
        private System.Windows.Forms.MenuItem affinityProcessMenuItem;
        private System.Windows.Forms.MenuItem runAsServiceMenuItem;
        private System.Windows.Forms.MenuItem runAsProcessMenuItem;
        private System.Windows.Forms.MenuItem launchAsUserProcessMenuItem;
        private System.Windows.Forms.MenuItem launchAsThisUserProcessMenuItem;
        private System.Windows.Forms.MenuItem sysInfoMenuItem;
        private System.Windows.Forms.MenuItem copyProcessMenuItem;
        private System.Windows.Forms.MenuItem selectAllProcessMenuItem;
        private System.Windows.Forms.MenuItem terminatorProcessMenuItem;
        private System.Windows.Forms.MenuItem menuItem2;
        private System.Windows.Forms.StatusBarPanel statusCPU;
        private System.Windows.Forms.StatusBarPanel statusMemory;
        private System.Windows.Forms.MenuItem reloadStructsMenuItem;
        private System.Windows.Forms.TabPage tabNetwork;
        private NetworkList listNetwork;
        private System.Windows.Forms.MenuItem sysInformationIconMenuItem;
        private System.Windows.Forms.MenuItem csrProcessesMenuItem;
        private System.Windows.Forms.MenuItem viewMenuItem;
        private System.Windows.Forms.MenuItem updateNowMenuItem;
        private System.Windows.Forms.MenuItem updateProcessesMenuItem;
        private System.Windows.Forms.MenuItem updateServicesMenuItem;
        private System.Windows.Forms.MenuItem processesMenuItem;
        private System.Windows.Forms.MenuItem restartProcessMenuItem;
        private System.Windows.Forms.MenuItem setTokenProcessMenuItem;
        private System.Windows.Forms.MenuItem helpMenu;
        private System.Windows.Forms.MenuItem menuItem3;
        private System.Windows.Forms.MenuItem verifyFileSignatureMenuItem;
        private System.Windows.Forms.MenuItem enableAllNotificationsMenuItem;
        private System.Windows.Forms.MenuItem disableAllNotificationsMenuItem;
        private System.Windows.Forms.MenuItem menuItem4;
        private System.Windows.Forms.MenuItem shutdownTrayMenuItem;
        private System.Windows.Forms.MenuItem shutdownMenuItem;
        private System.Windows.Forms.MenuItem runAsAdministratorMenuItem;
        private System.Windows.Forms.MenuItem showDetailsForAllProcessesMenuItem;
        private System.Windows.Forms.MenuItem uacSeparatorMenuItem;
        private System.Windows.Forms.MenuItem runMenuItem;
        private System.Windows.Forms.MenuItem runAsMenuItem;
        private System.Windows.Forms.MenuItem freeMemoryMenuItem;
        private System.Windows.Forms.MenuItem menuItem1;
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
        private System.Windows.Forms.MenuItem toolbarMenuItem;
        private System.Windows.Forms.Panel panelHack;
        private System.Windows.Forms.MenuItem saveMenuItem;
        private System.Windows.Forms.ContextMenu menuNetwork;
        private System.Windows.Forms.MenuItem goToProcessNetworkMenuItem;
        private System.Windows.Forms.MenuItem copyNetworkMenuItem;
        private System.Windows.Forms.MenuItem menuItem6;
        private System.Windows.Forms.MenuItem selectAllNetworkMenuItem;
        private System.Windows.Forms.MenuItem injectDllProcessMenuItem;
        private System.Windows.Forms.MenuItem apiLoggerMenuItem;
    }
}

