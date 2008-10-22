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
            this.menuModule2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuThread2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.timerFire = new System.Windows.Forms.Timer(this.components);
            this.menuProcess2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.menuMemory2 = new System.Windows.Forms.ContextMenuStrip(this.components);
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
            this.closeActiveWindowMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem5 = new System.Windows.Forms.MenuItem();
            this.priorityMenuItem = new System.Windows.Forms.MenuItem();
            this.realTimeMenuItem = new System.Windows.Forms.MenuItem();
            this.highMenuItem = new System.Windows.Forms.MenuItem();
            this.aboveNormalMenuItem = new System.Windows.Forms.MenuItem();
            this.normalMenuItem = new System.Windows.Forms.MenuItem();
            this.belowNormalMenuItem = new System.Windows.Forms.MenuItem();
            this.idleMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem7 = new System.Windows.Forms.MenuItem();
            this.selectAllMenuItem = new System.Windows.Forms.MenuItem();
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
            this.selectAllThreadMenuItem = new System.Windows.Forms.MenuItem();
            this.toolStripMenuItem9 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem10 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem11 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem12 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem13 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem14 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem15 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuModule = new System.Windows.Forms.ContextMenu();
            this.copyFileNameMenuItem = new System.Windows.Forms.MenuItem();
            this.openContainingFolderMenuItem = new System.Windows.Forms.MenuItem();
            this.propertiesMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.goToInMemoryViewModuleMenuItem = new System.Windows.Forms.MenuItem();
            this.getFuncAddressMenuItem = new System.Windows.Forms.MenuItem();
            this.changeMemoryProtectionModuleMenuItem = new System.Windows.Forms.MenuItem();
            this.readMemoryModuleMenuItem = new System.Windows.Forms.MenuItem();
            this.menuMemory = new System.Windows.Forms.ContextMenu();
            this.changeMemoryProtectionMemoryMenuItem = new System.Windows.Forms.MenuItem();
            this.readWriteMemoryMemoryMenuItem = new System.Windows.Forms.MenuItem();
            this.readWriteAddressMemoryMenuItem = new System.Windows.Forms.MenuItem();
            this.splitMain = new System.Windows.Forms.SplitContainer();
            this.listProcesses = new System.Windows.Forms.ListView();
            this.columnName = new System.Windows.Forms.ColumnHeader();
            this.columnPID = new System.Windows.Forms.ColumnHeader();
            this.columnPvtMemory = new System.Windows.Forms.ColumnHeader();
            this.columnUsername = new System.Windows.Forms.ColumnHeader();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabProcess = new System.Windows.Forms.TabPage();
            this.groupSearch = new System.Windows.Forms.GroupBox();
            this.buttonSearch = new wyDay.Controls.SplitButton();
            this.menuSearch = new System.Windows.Forms.ContextMenu();
            this.newResultsWindowMenuItem = new System.Windows.Forms.MenuItem();
            this.literalSearchMenuItem = new System.Windows.Forms.MenuItem();
            this.regexSearchMenuItem = new System.Windows.Forms.MenuItem();
            this.stringScanMenuItem = new System.Windows.Forms.MenuItem();
            this.heapScanMenuItem = new System.Windows.Forms.MenuItem();
            this.treeMisc = new System.Windows.Forms.TreeView();
            this.tabThreads = new System.Windows.Forms.TabPage();
            this.listThreads = new System.Windows.Forms.ListView();
            this.columnThreadID = new System.Windows.Forms.ColumnHeader();
            this.columnThreadState = new System.Windows.Forms.ColumnHeader();
            this.columnCPUTime = new System.Windows.Forms.ColumnHeader();
            this.columnPriority = new System.Windows.Forms.ColumnHeader();
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
            this.mainMenu = new System.Windows.Forms.MainMenu(this.components);
            this.hackerMenuItem = new System.Windows.Forms.MenuItem();
            this.aboutMenuItem = new System.Windows.Forms.MenuItem();
            this.optionsMenuItem = new System.Windows.Forms.MenuItem();
            this.helpMenuItem = new System.Windows.Forms.MenuItem();
            this.exitMenuItem = new System.Windows.Forms.MenuItem();
            this.processesMenuItem = new System.Windows.Forms.MenuItem();
            this.refreshMenuItem = new System.Windows.Forms.MenuItem();
            this.windowMenuItem = new System.Windows.Forms.MenuItem();
            this.vistaMenu = new wyDay.Controls.VistaMenu(this.components);
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.selectAllHackerMenuItem = new System.Windows.Forms.MenuItem();
            this.panelProc.SuspendLayout();
            this.panelVirtualProtect.SuspendLayout();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabProcess.SuspendLayout();
            this.groupSearch.SuspendLayout();
            this.tabThreads.SuspendLayout();
            this.tabModules.SuspendLayout();
            this.tabMemory.SuspendLayout();
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
            // menuModule2
            // 
            this.menuModule2.Name = "menuModule";
            this.menuModule2.Size = new System.Drawing.Size(61, 4);
            this.menuModule2.Opening += new System.ComponentModel.CancelEventHandler(this.menuModule2_Opening);
            // 
            // menuThread2
            // 
            this.menuThread2.Name = "menuProcess";
            this.menuThread2.Size = new System.Drawing.Size(61, 4);
            this.menuThread2.Opening += new System.ComponentModel.CancelEventHandler(this.menuThread2_Opening);
            // 
            // timerFire
            // 
            this.timerFire.Interval = 250;
            this.timerFire.Tick += new System.EventHandler(this.timerFire_Tick);
            // 
            // menuProcess2
            // 
            this.menuProcess2.Name = "menuProcess";
            this.menuProcess2.Size = new System.Drawing.Size(61, 4);
            this.menuProcess2.Opening += new System.ComponentModel.CancelEventHandler(this.menuProcess2_Opening);
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "process_small");
            // 
            // menuMemory2
            // 
            this.menuMemory2.Name = "menuModule";
            this.menuMemory2.Size = new System.Drawing.Size(61, 4);
            this.menuMemory2.Opening += new System.ComponentModel.CancelEventHandler(this.menuMemory2_Opening);
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
            this.closeActiveWindowMenuItem,
            this.menuItem5,
            this.priorityMenuItem,
            this.menuItem7,
            this.selectAllMenuItem});
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
            // closeActiveWindowMenuItem
            // 
            this.vistaMenu.SetImage(this.closeActiveWindowMenuItem, global::ProcessHacker.Properties.Resources.application_delete);
            this.closeActiveWindowMenuItem.Index = 3;
            this.closeActiveWindowMenuItem.Text = "&Close Active Window";
            this.closeActiveWindowMenuItem.Click += new System.EventHandler(this.closeActiveWindowMenuItem_Click);
            // 
            // menuItem5
            // 
            this.menuItem5.Index = 4;
            this.menuItem5.Text = "-";
            // 
            // priorityMenuItem
            // 
            this.vistaMenu.SetImage(this.priorityMenuItem, global::ProcessHacker.Properties.Resources.control_equalizer_blue);
            this.priorityMenuItem.Index = 5;
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
            // menuItem7
            // 
            this.menuItem7.Index = 6;
            this.menuItem7.Text = "-";
            // 
            // selectAllMenuItem
            // 
            this.selectAllMenuItem.Index = 7;
            this.selectAllMenuItem.Text = "Select &All";
            this.selectAllMenuItem.Click += new System.EventHandler(this.selectAllMenuItem_Click);
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
            this.selectAllThreadMenuItem});
            this.menuThread.Popup += new System.EventHandler(this.menuThread_Popup);
            // 
            // inspectThreadMenuItem
            // 
            this.inspectThreadMenuItem.DefaultItem = true;
            this.vistaMenu.SetImage(this.inspectThreadMenuItem, global::ProcessHacker.Properties.Resources.application_form_magnify);
            this.inspectThreadMenuItem.Index = 0;
            this.inspectThreadMenuItem.Text = "&Inspect";
            this.inspectThreadMenuItem.Visible = false;
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
            // selectAllThreadMenuItem
            // 
            this.selectAllThreadMenuItem.Index = 7;
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
            this.copyFileNameMenuItem,
            this.openContainingFolderMenuItem,
            this.propertiesMenuItem,
            this.menuItem3,
            this.goToInMemoryViewModuleMenuItem,
            this.getFuncAddressMenuItem,
            this.changeMemoryProtectionModuleMenuItem,
            this.readMemoryModuleMenuItem});
            this.menuModule.Popup += new System.EventHandler(this.menuModule_Popup);
            // 
            // copyFileNameMenuItem
            // 
            this.vistaMenu.SetImage(this.copyFileNameMenuItem, global::ProcessHacker.Properties.Resources.page_copy);
            this.copyFileNameMenuItem.Index = 0;
            this.copyFileNameMenuItem.Text = "&Copy File Name";
            this.copyFileNameMenuItem.Click += new System.EventHandler(this.copyFileNameMenuItem_Click);
            // 
            // openContainingFolderMenuItem
            // 
            this.vistaMenu.SetImage(this.openContainingFolderMenuItem, global::ProcessHacker.Properties.Resources.folder_explore);
            this.openContainingFolderMenuItem.Index = 1;
            this.openContainingFolderMenuItem.Text = "&Open Containing Folder";
            this.openContainingFolderMenuItem.Click += new System.EventHandler(this.openContainingFolderMenuItem_Click);
            // 
            // propertiesMenuItem
            // 
            this.vistaMenu.SetImage(this.propertiesMenuItem, global::ProcessHacker.Properties.Resources.application_view_detail);
            this.propertiesMenuItem.Index = 2;
            this.propertiesMenuItem.Text = "Prope&rties...";
            this.propertiesMenuItem.Click += new System.EventHandler(this.propertiesMenuItem_Click);
            // 
            // menuItem3
            // 
            this.menuItem3.Index = 3;
            this.menuItem3.Text = "-";
            // 
            // goToInMemoryViewModuleMenuItem
            // 
            this.goToInMemoryViewModuleMenuItem.DefaultItem = true;
            this.vistaMenu.SetImage(this.goToInMemoryViewModuleMenuItem, global::ProcessHacker.Properties.Resources.arrow_right);
            this.goToInMemoryViewModuleMenuItem.Index = 4;
            this.goToInMemoryViewModuleMenuItem.Text = "&Go To In Memory View";
            this.goToInMemoryViewModuleMenuItem.Click += new System.EventHandler(this.goToInMemoryViewModuleMenuItem_Click);
            // 
            // getFuncAddressMenuItem
            // 
            this.getFuncAddressMenuItem.Index = 5;
            this.getFuncAddressMenuItem.Text = "Get &Function Address...";
            this.getFuncAddressMenuItem.Click += new System.EventHandler(this.getFuncAddressMenuItem_Click);
            // 
            // changeMemoryProtectionModuleMenuItem
            // 
            this.vistaMenu.SetImage(this.changeMemoryProtectionModuleMenuItem, global::ProcessHacker.Properties.Resources.lock_edit);
            this.changeMemoryProtectionModuleMenuItem.Index = 6;
            this.changeMemoryProtectionModuleMenuItem.Text = "Change &Memory Protection...";
            this.changeMemoryProtectionModuleMenuItem.Click += new System.EventHandler(this.changeMemoryProtectionModuleMenuItem_Click);
            // 
            // readMemoryModuleMenuItem
            // 
            this.vistaMenu.SetImage(this.readMemoryModuleMenuItem, global::ProcessHacker.Properties.Resources.page);
            this.readMemoryModuleMenuItem.Index = 7;
            this.readMemoryModuleMenuItem.Text = "Read Memory...";
            this.readMemoryModuleMenuItem.Click += new System.EventHandler(this.readMemoryModuleMenuItem_Click);
            // 
            // menuMemory
            // 
            this.menuMemory.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.changeMemoryProtectionMemoryMenuItem,
            this.readWriteMemoryMemoryMenuItem,
            this.readWriteAddressMemoryMenuItem});
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
            // splitMain
            // 
            this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitMain.Location = new System.Drawing.Point(0, 0);
            this.splitMain.Name = "splitMain";
            // 
            // splitMain.Panel1
            // 
            this.splitMain.Panel1.Controls.Add(this.listProcesses);
            this.splitMain.Panel1.Padding = new System.Windows.Forms.Padding(0, 0, 0, 1);
            // 
            // splitMain.Panel2
            // 
            this.splitMain.Panel2.Controls.Add(this.tabControl);
            this.splitMain.Size = new System.Drawing.Size(804, 464);
            this.splitMain.SplitterDistance = 355;
            this.splitMain.TabIndex = 3;
            // 
            // listProcesses
            // 
            this.listProcesses.AllowColumnReorder = true;
            this.listProcesses.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnName,
            this.columnPID,
            this.columnPvtMemory,
            this.columnUsername});
            this.listProcesses.ContextMenuStrip = this.menuProcess2;
            this.listProcesses.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listProcesses.FullRowSelect = true;
            this.listProcesses.HideSelection = false;
            this.listProcesses.Location = new System.Drawing.Point(0, 0);
            this.listProcesses.Name = "listProcesses";
            this.listProcesses.ShowItemToolTips = true;
            this.listProcesses.Size = new System.Drawing.Size(355, 463);
            this.listProcesses.SmallImageList = this.imageList;
            this.listProcesses.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listProcesses.TabIndex = 0;
            this.listProcesses.UseCompatibleStateImageBehavior = false;
            this.listProcesses.View = System.Windows.Forms.View.Details;
            this.listProcesses.SelectedIndexChanged += new System.EventHandler(this.listProcesses_SelectedIndexChanged);
            this.listProcesses.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listProcesses_MouseDown);
            this.listProcesses.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listProcesses_KeyDown);
            // 
            // columnName
            // 
            this.columnName.Text = "Name";
            this.columnName.Width = 120;
            // 
            // columnPID
            // 
            this.columnPID.Text = "PID";
            // 
            // columnPvtMemory
            // 
            this.columnPvtMemory.Text = "Pvt. Memory";
            this.columnPvtMemory.Width = 80;
            // 
            // columnUsername
            // 
            this.columnUsername.Text = "User";
            this.columnUsername.Width = 80;
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabProcess);
            this.tabControl.Controls.Add(this.tabThreads);
            this.tabControl.Controls.Add(this.tabModules);
            this.tabControl.Controls.Add(this.tabMemory);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(445, 464);
            this.tabControl.TabIndex = 5;
            // 
            // tabProcess
            // 
            this.tabProcess.Controls.Add(this.groupSearch);
            this.tabProcess.Controls.Add(this.treeMisc);
            this.tabProcess.Location = new System.Drawing.Point(4, 22);
            this.tabProcess.Name = "tabProcess";
            this.tabProcess.Padding = new System.Windows.Forms.Padding(3);
            this.tabProcess.Size = new System.Drawing.Size(437, 438);
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
            this.groupSearch.Size = new System.Drawing.Size(425, 47);
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
            this.treeMisc.Size = new System.Drawing.Size(425, 373);
            this.treeMisc.TabIndex = 1;
            // 
            // tabThreads
            // 
            this.tabThreads.Controls.Add(this.listThreads);
            this.tabThreads.Location = new System.Drawing.Point(4, 22);
            this.tabThreads.Name = "tabThreads";
            this.tabThreads.Padding = new System.Windows.Forms.Padding(3);
            this.tabThreads.Size = new System.Drawing.Size(437, 438);
            this.tabThreads.TabIndex = 6;
            this.tabThreads.Text = "Threads";
            this.tabThreads.UseVisualStyleBackColor = true;
            // 
            // listThreads
            // 
            this.listThreads.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listThreads.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnThreadID,
            this.columnThreadState,
            this.columnCPUTime,
            this.columnPriority});
            this.listThreads.ContextMenuStrip = this.menuThread2;
            this.listThreads.FullRowSelect = true;
            this.listThreads.HideSelection = false;
            this.listThreads.Location = new System.Drawing.Point(6, 6);
            this.listThreads.Name = "listThreads";
            this.listThreads.Size = new System.Drawing.Size(425, 426);
            this.listThreads.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listThreads.TabIndex = 2;
            this.listThreads.UseCompatibleStateImageBehavior = false;
            this.listThreads.View = System.Windows.Forms.View.Details;
            this.listThreads.DoubleClick += new System.EventHandler(this.listThreads_DoubleClick);
            this.listThreads.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listThreads_MouseDown);
            // 
            // columnThreadID
            // 
            this.columnThreadID.Text = "TID";
            // 
            // columnThreadState
            // 
            this.columnThreadState.Text = "State";
            this.columnThreadState.Width = 100;
            // 
            // columnCPUTime
            // 
            this.columnCPUTime.Text = "CPU Time";
            this.columnCPUTime.Width = 70;
            // 
            // columnPriority
            // 
            this.columnPriority.Text = "Priority";
            this.columnPriority.Width = 80;
            // 
            // tabModules
            // 
            this.tabModules.Controls.Add(this.listModules);
            this.tabModules.Location = new System.Drawing.Point(4, 22);
            this.tabModules.Name = "tabModules";
            this.tabModules.Padding = new System.Windows.Forms.Padding(3);
            this.tabModules.Size = new System.Drawing.Size(437, 438);
            this.tabModules.TabIndex = 0;
            this.tabModules.Text = "Modules";
            this.tabModules.UseVisualStyleBackColor = true;
            // 
            // listModules
            // 
            this.listModules.AllowColumnReorder = true;
            this.listModules.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listModules.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnModuleName,
            this.columnBaseAddress,
            this.columnModuleSize,
            this.columnDescription});
            this.listModules.ContextMenuStrip = this.menuModule2;
            this.listModules.FullRowSelect = true;
            this.listModules.HideSelection = false;
            this.listModules.Location = new System.Drawing.Point(6, 6);
            this.listModules.MultiSelect = false;
            this.listModules.Name = "listModules";
            this.listModules.ShowItemToolTips = true;
            this.listModules.Size = new System.Drawing.Size(425, 426);
            this.listModules.TabIndex = 1;
            this.listModules.UseCompatibleStateImageBehavior = false;
            this.listModules.View = System.Windows.Forms.View.Details;
            this.listModules.DoubleClick += new System.EventHandler(this.listModules_DoubleClick);
            this.listModules.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listModules_MouseDown);
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
            this.tabMemory.Size = new System.Drawing.Size(437, 438);
            this.tabMemory.TabIndex = 1;
            this.tabMemory.Text = "Memory";
            this.tabMemory.UseVisualStyleBackColor = true;
            // 
            // listMemory
            // 
            this.listMemory.AllowColumnReorder = true;
            this.listMemory.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listMemory.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5});
            this.listMemory.ContextMenuStrip = this.menuMemory2;
            this.listMemory.FullRowSelect = true;
            this.listMemory.HideSelection = false;
            this.listMemory.Location = new System.Drawing.Point(6, 6);
            this.listMemory.MultiSelect = false;
            this.listMemory.Name = "listMemory";
            this.listMemory.ShowItemToolTips = true;
            this.listMemory.Size = new System.Drawing.Size(425, 426);
            this.listMemory.TabIndex = 2;
            this.listMemory.UseCompatibleStateImageBehavior = false;
            this.listMemory.View = System.Windows.Forms.View.Details;
            this.listMemory.DoubleClick += new System.EventHandler(this.listMemory_DoubleClick);
            this.listMemory.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listMemory_MouseDown);
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
            // mainMenu
            // 
            this.mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.hackerMenuItem,
            this.processesMenuItem,
            this.windowMenuItem});
            // 
            // hackerMenuItem
            // 
            this.hackerMenuItem.Index = 0;
            this.hackerMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.aboutMenuItem,
            this.optionsMenuItem,
            this.helpMenuItem,
            this.exitMenuItem,
            this.menuItem1,
            this.selectAllHackerMenuItem});
            this.hackerMenuItem.Text = "&Hacker";
            // 
            // aboutMenuItem
            // 
            this.vistaMenu.SetImage(this.aboutMenuItem, global::ProcessHacker.Properties.Resources.information);
            this.aboutMenuItem.Index = 0;
            this.aboutMenuItem.Text = "&About...";
            this.aboutMenuItem.Click += new System.EventHandler(this.aboutMenuItem_Click);
            // 
            // optionsMenuItem
            // 
            this.vistaMenu.SetImage(this.optionsMenuItem, global::ProcessHacker.Properties.Resources.page_gear);
            this.optionsMenuItem.Index = 1;
            this.optionsMenuItem.Text = "&Options...";
            this.optionsMenuItem.Click += new System.EventHandler(this.optionsMenuItem_Click);
            // 
            // helpMenuItem
            // 
            this.vistaMenu.SetImage(this.helpMenuItem, global::ProcessHacker.Properties.Resources.help);
            this.helpMenuItem.Index = 2;
            this.helpMenuItem.Text = "&Help...";
            this.helpMenuItem.Click += new System.EventHandler(this.helpMenuItem_Click);
            // 
            // exitMenuItem
            // 
            this.vistaMenu.SetImage(this.exitMenuItem, global::ProcessHacker.Properties.Resources.door_out);
            this.exitMenuItem.Index = 3;
            this.exitMenuItem.Text = "E&xit";
            this.exitMenuItem.Click += new System.EventHandler(this.exitMenuItem_Click);
            // 
            // processesMenuItem
            // 
            this.processesMenuItem.Index = 1;
            this.processesMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.refreshMenuItem});
            this.processesMenuItem.Text = "&Processes";
            // 
            // refreshMenuItem
            // 
            this.vistaMenu.SetImage(this.refreshMenuItem, global::ProcessHacker.Properties.Resources.arrow_refresh);
            this.refreshMenuItem.Index = 0;
            this.refreshMenuItem.Shortcut = System.Windows.Forms.Shortcut.F5;
            this.refreshMenuItem.Text = "&Refresh";
            this.refreshMenuItem.Click += new System.EventHandler(this.refreshMenuItem_Click);
            // 
            // windowMenuItem
            // 
            this.windowMenuItem.Index = 2;
            this.windowMenuItem.Text = "&Window";
            // 
            // vistaMenu
            // 
            this.vistaMenu.ContainerControl = this;
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 4;
            this.menuItem1.Text = "-";
            // 
            // selectAllHackerMenuItem
            // 
            this.selectAllHackerMenuItem.Index = 5;
            this.selectAllHackerMenuItem.Shortcut = System.Windows.Forms.Shortcut.CtrlA;
            this.selectAllHackerMenuItem.Text = "&Select All";
            this.selectAllHackerMenuItem.Click += new System.EventHandler(this.selectAllHackerMenuItem_Click);
            // 
            // HackerWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(804, 464);
            this.Controls.Add(this.splitMain);
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
            ((System.ComponentModel.ISupportInitialize)(this.vistaMenu)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listModules;
        private System.Windows.Forms.ColumnHeader columnModuleName;
        private System.Windows.Forms.ListView listThreads;
        private System.Windows.Forms.ColumnHeader columnThreadID;
        private System.Windows.Forms.ColumnHeader columnBaseAddress;
        private System.Windows.Forms.ColumnHeader columnThreadState;
        private System.Windows.Forms.ColumnHeader columnCPUTime;
        private System.Windows.Forms.ColumnHeader columnModuleSize;
        private System.Windows.Forms.ContextMenuStrip menuModule2;
        private System.Windows.Forms.ContextMenuStrip menuThread2;
        private System.Windows.Forms.Timer timerFire;
        private System.Windows.Forms.ColumnHeader columnPriority;
        private System.Windows.Forms.Panel panelProc;
        private System.Windows.Forms.Label labelProcedureName;
        private System.Windows.Forms.TextBox textProcName;
        private System.Windows.Forms.Button buttonGetProcAddress;
        private System.Windows.Forms.TextBox textProcAddress;
        private System.Windows.Forms.SplitContainer splitMain;
        private System.Windows.Forms.ListView listProcesses;
        private System.Windows.Forms.ColumnHeader columnName;
        private System.Windows.Forms.ColumnHeader columnPID;
        private System.Windows.Forms.ColumnHeader columnPvtMemory;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabModules;
        private System.Windows.Forms.TabPage tabMemory;
        private System.Windows.Forms.ListView listMemory;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ContextMenuStrip menuMemory2;
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
        private System.Windows.Forms.MenuItem closeActiveWindowMenuItem;
        private System.Windows.Forms.MenuItem menuItem5;
        private System.Windows.Forms.MenuItem priorityMenuItem;
        private System.Windows.Forms.MenuItem menuItem7;
        private System.Windows.Forms.MenuItem selectAllMenuItem;
        private System.Windows.Forms.MenuItem realTimeMenuItem;
        private System.Windows.Forms.MenuItem highMenuItem;
        private System.Windows.Forms.MenuItem aboveNormalMenuItem;
        private System.Windows.Forms.MenuItem normalMenuItem;
        private System.Windows.Forms.MenuItem belowNormalMenuItem;
        private System.Windows.Forms.MenuItem idleMenuItem;
        private System.Windows.Forms.ContextMenuStrip menuProcess2;
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
        private System.Windows.Forms.ColumnHeader columnUsername;
        private System.Windows.Forms.MenuItem processesMenuItem;
        private System.Windows.Forms.MenuItem refreshMenuItem;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem selectAllHackerMenuItem;
    }
}

