namespace ProcessHacker.Components
{
    partial class ThreadList
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

            _highlightingContext.Dispose();
            this.Provider = null;

            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.listThreads = new System.Windows.Forms.ListView();
            this.columnThreadID = new System.Windows.Forms.ColumnHeader();
            this.columnContextSwitchesDelta = new System.Windows.Forms.ColumnHeader();
            this.columnStartAddress = new System.Windows.Forms.ColumnHeader();
            this.columnPriority = new System.Windows.Forms.ColumnHeader();
            this.menuThread = new System.Windows.Forms.ContextMenu();
            this.inspectThreadMenuItem = new System.Windows.Forms.MenuItem();
            this.terminateThreadMenuItem = new System.Windows.Forms.MenuItem();
            this.forceTerminateThreadMenuItem = new System.Windows.Forms.MenuItem();
            this.suspendThreadMenuItem = new System.Windows.Forms.MenuItem();
            this.resumeThreadMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.inspectTEBMenuItem = new System.Windows.Forms.MenuItem();
            this.permissionsThreadMenuItem = new System.Windows.Forms.MenuItem();
            this.tokenThreadMenuItem = new System.Windows.Forms.MenuItem();
            this.analyzeMenuItem = new System.Windows.Forms.MenuItem();
            this.analyzeWaitMenuItem = new System.Windows.Forms.MenuItem();
            this.priorityThreadMenuItem = new System.Windows.Forms.MenuItem();
            this.timeCriticalThreadMenuItem = new System.Windows.Forms.MenuItem();
            this.highestThreadMenuItem = new System.Windows.Forms.MenuItem();
            this.aboveNormalThreadMenuItem = new System.Windows.Forms.MenuItem();
            this.normalThreadMenuItem = new System.Windows.Forms.MenuItem();
            this.belowNormalThreadMenuItem = new System.Windows.Forms.MenuItem();
            this.lowestThreadMenuItem = new System.Windows.Forms.MenuItem();
            this.idleThreadMenuItem = new System.Windows.Forms.MenuItem();
            this.ioPriorityThreadMenuItem = new System.Windows.Forms.MenuItem();
            this.ioPriority0ThreadMenuItem = new System.Windows.Forms.MenuItem();
            this.ioPriority1ThreadMenuItem = new System.Windows.Forms.MenuItem();
            this.ioPriority2ThreadMenuItem = new System.Windows.Forms.MenuItem();
            this.ioPriority3ThreadMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem9 = new System.Windows.Forms.MenuItem();
            this.copyThreadMenuItem = new System.Windows.Forms.MenuItem();
            this.selectAllThreadMenuItem = new System.Windows.Forms.MenuItem();
            this.tableInformation = new System.Windows.Forms.TableLayoutPanel();
            this.label6 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.labelState = new System.Windows.Forms.Label();
            this.labelKernelTime = new System.Windows.Forms.Label();
            this.labelUserTime = new System.Windows.Forms.Label();
            this.labelTotalTime = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.labelContextSwitches = new System.Windows.Forms.Label();
            this.labelBasePriority = new System.Windows.Forms.Label();
            this.labelPriority = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.labelTEBAddress = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.vistaMenu = new wyDay.Controls.VistaMenu(this.components);
            this.fileModule = new ProcessHacker.Components.FileNameBox();
            this.tableInformation.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.vistaMenu)).BeginInit();
            this.SuspendLayout();
            // 
            // listThreads
            // 
            this.listThreads.AllowColumnReorder = true;
            this.listThreads.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listThreads.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnThreadID,
            this.columnContextSwitchesDelta,
            this.columnStartAddress,
            this.columnPriority});
            this.listThreads.FullRowSelect = true;
            this.listThreads.HideSelection = false;
            this.listThreads.Location = new System.Drawing.Point(0, 0);
            this.listThreads.Name = "listThreads";
            this.listThreads.ShowItemToolTips = true;
            this.listThreads.Size = new System.Drawing.Size(450, 345);
            this.listThreads.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listThreads.TabIndex = 3;
            this.listThreads.UseCompatibleStateImageBehavior = false;
            this.listThreads.View = System.Windows.Forms.View.Details;
            this.listThreads.DoubleClick += new System.EventHandler(this.listThreads_DoubleClick);
            // 
            // columnThreadID
            // 
            this.columnThreadID.Text = "TID";
            this.columnThreadID.Width = 50;
            // 
            // columnContextSwitchesDelta
            // 
            this.columnContextSwitchesDelta.Text = "Context Switches Delta";
            this.columnContextSwitchesDelta.Width = 70;
            // 
            // columnStartAddress
            // 
            this.columnStartAddress.Text = "Start Address";
            this.columnStartAddress.Width = 220;
            // 
            // columnPriority
            // 
            this.columnPriority.Text = "Priority";
            this.columnPriority.Width = 100;
            // 
            // menuThread
            // 
            this.menuThread.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.inspectThreadMenuItem,
            this.terminateThreadMenuItem,
            this.forceTerminateThreadMenuItem,
            this.suspendThreadMenuItem,
            this.resumeThreadMenuItem,
            this.menuItem4,
            this.inspectTEBMenuItem,
            this.permissionsThreadMenuItem,
            this.tokenThreadMenuItem,
            this.analyzeMenuItem,
            this.priorityThreadMenuItem,
            this.ioPriorityThreadMenuItem,
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
            // forceTerminateThreadMenuItem
            // 
            this.forceTerminateThreadMenuItem.Index = 2;
            this.forceTerminateThreadMenuItem.Text = "Force Terminate";
            this.forceTerminateThreadMenuItem.Click += new System.EventHandler(this.forceTerminateThreadMenuItem_Click);
            // 
            // suspendThreadMenuItem
            // 
            this.vistaMenu.SetImage(this.suspendThreadMenuItem, global::ProcessHacker.Properties.Resources.control_pause_blue);
            this.suspendThreadMenuItem.Index = 3;
            this.suspendThreadMenuItem.Text = "&Suspend";
            this.suspendThreadMenuItem.Click += new System.EventHandler(this.suspendThreadMenuItem_Click);
            // 
            // resumeThreadMenuItem
            // 
            this.vistaMenu.SetImage(this.resumeThreadMenuItem, global::ProcessHacker.Properties.Resources.control_play_blue);
            this.resumeThreadMenuItem.Index = 4;
            this.resumeThreadMenuItem.Text = "&Resume";
            this.resumeThreadMenuItem.Click += new System.EventHandler(this.resumeThreadMenuItem_Click);
            // 
            // menuItem4
            // 
            this.menuItem4.Index = 5;
            this.menuItem4.Text = "-";
            // 
            // inspectTEBMenuItem
            // 
            this.inspectTEBMenuItem.Index = 6;
            this.inspectTEBMenuItem.Text = "Inspect TEB";
            this.inspectTEBMenuItem.Click += new System.EventHandler(this.inspectTEBMenuItem_Click);
            // 
            // permissionsThreadMenuItem
            // 
            this.permissionsThreadMenuItem.Index = 7;
            this.permissionsThreadMenuItem.Text = "Permissions";
            this.permissionsThreadMenuItem.Click += new System.EventHandler(this.permissionsThreadMenuItem_Click);
            // 
            // tokenThreadMenuItem
            // 
            this.vistaMenu.SetImage(this.tokenThreadMenuItem, global::ProcessHacker.Properties.Resources.locked);
            this.tokenThreadMenuItem.Index = 8;
            this.tokenThreadMenuItem.Text = "Token";
            this.tokenThreadMenuItem.Click += new System.EventHandler(this.tokenThreadMenuItem_Click);
            // 
            // analyzeMenuItem
            // 
            this.analyzeMenuItem.Index = 9;
            this.analyzeMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.analyzeWaitMenuItem});
            this.analyzeMenuItem.Text = "Analyze";
            // 
            // analyzeWaitMenuItem
            // 
            this.analyzeWaitMenuItem.Index = 0;
            this.analyzeWaitMenuItem.Text = "Wait";
            this.analyzeWaitMenuItem.Click += new System.EventHandler(this.analyzeWaitMenuItem_Click);
            // 
            // priorityThreadMenuItem
            // 
            this.priorityThreadMenuItem.Index = 10;
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
            // ioPriorityThreadMenuItem
            // 
            this.ioPriorityThreadMenuItem.Index = 11;
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
            // menuItem9
            // 
            this.menuItem9.Index = 12;
            this.menuItem9.Text = "-";
            // 
            // copyThreadMenuItem
            // 
            this.vistaMenu.SetImage(this.copyThreadMenuItem, global::ProcessHacker.Properties.Resources.page_copy);
            this.copyThreadMenuItem.Index = 13;
            this.copyThreadMenuItem.Text = "C&opy";
            // 
            // selectAllThreadMenuItem
            // 
            this.selectAllThreadMenuItem.Index = 14;
            this.selectAllThreadMenuItem.Text = "Select &All";
            this.selectAllThreadMenuItem.Click += new System.EventHandler(this.selectAllThreadMenuItem_Click);
            // 
            // tableInformation
            // 
            this.tableInformation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableInformation.ColumnCount = 4;
            this.tableInformation.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableInformation.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableInformation.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableInformation.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableInformation.Controls.Add(this.label6, 0, 0);
            this.tableInformation.Controls.Add(this.label8, 0, 1);
            this.tableInformation.Controls.Add(this.label9, 0, 2);
            this.tableInformation.Controls.Add(this.labelState, 1, 0);
            this.tableInformation.Controls.Add(this.labelKernelTime, 1, 1);
            this.tableInformation.Controls.Add(this.labelUserTime, 1, 2);
            this.tableInformation.Controls.Add(this.labelTotalTime, 1, 3);
            this.tableInformation.Controls.Add(this.label1, 2, 3);
            this.tableInformation.Controls.Add(this.label2, 2, 2);
            this.tableInformation.Controls.Add(this.label3, 2, 1);
            this.tableInformation.Controls.Add(this.labelContextSwitches, 3, 3);
            this.tableInformation.Controls.Add(this.labelBasePriority, 3, 2);
            this.tableInformation.Controls.Add(this.labelPriority, 3, 1);
            this.tableInformation.Controls.Add(this.label10, 0, 3);
            this.tableInformation.Controls.Add(this.label4, 2, 0);
            this.tableInformation.Controls.Add(this.labelTEBAddress, 3, 0);
            this.tableInformation.Location = new System.Drawing.Point(0, 381);
            this.tableInformation.Name = "tableInformation";
            this.tableInformation.RowCount = 4;
            this.tableInformation.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableInformation.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableInformation.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableInformation.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableInformation.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableInformation.Size = new System.Drawing.Size(450, 79);
            this.tableInformation.TabIndex = 4;
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 3);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(32, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "State";
            // 
            // label8
            // 
            this.label8.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 22);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(63, 13);
            this.label8.TabIndex = 1;
            this.label8.Text = "Kernel Time";
            // 
            // label9
            // 
            this.label9.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(3, 41);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(55, 13);
            this.label9.TabIndex = 1;
            this.label9.Text = "User Time";
            // 
            // labelState
            // 
            this.labelState.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelState.AutoSize = true;
            this.labelState.Location = new System.Drawing.Point(188, 3);
            this.labelState.Name = "labelState";
            this.labelState.Size = new System.Drawing.Size(33, 13);
            this.labelState.TabIndex = 1;
            this.labelState.Text = "value";
            // 
            // labelKernelTime
            // 
            this.labelKernelTime.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelKernelTime.AutoSize = true;
            this.labelKernelTime.Location = new System.Drawing.Point(188, 22);
            this.labelKernelTime.Name = "labelKernelTime";
            this.labelKernelTime.Size = new System.Drawing.Size(33, 13);
            this.labelKernelTime.TabIndex = 1;
            this.labelKernelTime.Text = "value";
            // 
            // labelUserTime
            // 
            this.labelUserTime.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelUserTime.AutoSize = true;
            this.labelUserTime.Location = new System.Drawing.Point(188, 41);
            this.labelUserTime.Name = "labelUserTime";
            this.labelUserTime.Size = new System.Drawing.Size(33, 13);
            this.labelUserTime.TabIndex = 1;
            this.labelUserTime.Text = "value";
            // 
            // labelTotalTime
            // 
            this.labelTotalTime.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelTotalTime.AutoSize = true;
            this.labelTotalTime.Location = new System.Drawing.Point(188, 61);
            this.labelTotalTime.Name = "labelTotalTime";
            this.labelTotalTime.Size = new System.Drawing.Size(33, 13);
            this.labelTotalTime.TabIndex = 1;
            this.labelTotalTime.Text = "value";
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(227, 61);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Context Switches";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(227, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Base Priority";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(227, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Priority";
            // 
            // labelContextSwitches
            // 
            this.labelContextSwitches.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelContextSwitches.AutoSize = true;
            this.labelContextSwitches.Location = new System.Drawing.Point(414, 61);
            this.labelContextSwitches.Name = "labelContextSwitches";
            this.labelContextSwitches.Size = new System.Drawing.Size(33, 13);
            this.labelContextSwitches.TabIndex = 7;
            this.labelContextSwitches.Text = "value";
            // 
            // labelBasePriority
            // 
            this.labelBasePriority.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelBasePriority.AutoSize = true;
            this.labelBasePriority.Location = new System.Drawing.Point(414, 41);
            this.labelBasePriority.Name = "labelBasePriority";
            this.labelBasePriority.Size = new System.Drawing.Size(33, 13);
            this.labelBasePriority.TabIndex = 4;
            this.labelBasePriority.Text = "value";
            // 
            // labelPriority
            // 
            this.labelPriority.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelPriority.AutoSize = true;
            this.labelPriority.Location = new System.Drawing.Point(414, 22);
            this.labelPriority.Name = "labelPriority";
            this.labelPriority.Size = new System.Drawing.Size(33, 13);
            this.labelPriority.TabIndex = 3;
            this.labelPriority.Text = "value";
            // 
            // label10
            // 
            this.label10.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(3, 61);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(57, 13);
            this.label10.TabIndex = 1;
            this.label10.Text = "Total Time";
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(227, 3);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(69, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "TEB Address";
            // 
            // labelTEBAddress
            // 
            this.labelTEBAddress.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelTEBAddress.AutoSize = true;
            this.labelTEBAddress.Location = new System.Drawing.Point(414, 3);
            this.labelTEBAddress.Name = "labelTEBAddress";
            this.labelTEBAddress.Size = new System.Drawing.Size(33, 13);
            this.labelTEBAddress.TabIndex = 3;
            this.labelTEBAddress.Text = "value";
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 356);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(70, 13);
            this.label7.TabIndex = 5;
            this.label7.Text = "Start Module:";
            // 
            // vistaMenu
            // 
            this.vistaMenu.ContainerControl = this;
            this.vistaMenu.DelaySetImageCalls = false;
            // 
            // fileModule
            // 
            this.fileModule.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.fileModule.Location = new System.Drawing.Point(79, 351);
            this.fileModule.Name = "fileModule";
            this.fileModule.ReadOnly = true;
            this.fileModule.Size = new System.Drawing.Size(368, 24);
            this.fileModule.TabIndex = 6;
            // 
            // ThreadList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.fileModule);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.tableInformation);
            this.Controls.Add(this.listThreads);
            this.DoubleBuffered = true;
            this.Name = "ThreadList";
            this.Size = new System.Drawing.Size(450, 460);
            this.tableInformation.ResumeLayout(false);
            this.tableInformation.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.vistaMenu)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listThreads;
        private System.Windows.Forms.ColumnHeader columnThreadID;
        private System.Windows.Forms.ColumnHeader columnContextSwitchesDelta;
        private System.Windows.Forms.ColumnHeader columnStartAddress;
        private wyDay.Controls.VistaMenu vistaMenu;
        private System.Windows.Forms.ContextMenu menuThread;
        private System.Windows.Forms.MenuItem inspectThreadMenuItem;
        private System.Windows.Forms.MenuItem terminateThreadMenuItem;
        private System.Windows.Forms.MenuItem suspendThreadMenuItem;
        private System.Windows.Forms.MenuItem resumeThreadMenuItem;
        private System.Windows.Forms.MenuItem menuItem4;
        private System.Windows.Forms.MenuItem priorityThreadMenuItem;
        private System.Windows.Forms.MenuItem timeCriticalThreadMenuItem;
        private System.Windows.Forms.MenuItem highestThreadMenuItem;
        private System.Windows.Forms.MenuItem aboveNormalThreadMenuItem;
        private System.Windows.Forms.MenuItem normalThreadMenuItem;
        private System.Windows.Forms.MenuItem belowNormalThreadMenuItem;
        private System.Windows.Forms.MenuItem lowestThreadMenuItem;
        private System.Windows.Forms.MenuItem idleThreadMenuItem;
        private System.Windows.Forms.MenuItem menuItem9;
        private System.Windows.Forms.MenuItem copyThreadMenuItem;
        private System.Windows.Forms.MenuItem selectAllThreadMenuItem;
        private System.Windows.Forms.MenuItem inspectTEBMenuItem;
        private System.Windows.Forms.ColumnHeader columnPriority;
        private System.Windows.Forms.TableLayoutPanel tableInformation;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label labelState;
        private System.Windows.Forms.Label labelKernelTime;
        private System.Windows.Forms.Label labelUserTime;
        private System.Windows.Forms.Label labelTotalTime;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labelBasePriority;
        private System.Windows.Forms.Label labelPriority;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelContextSwitches;
        private System.Windows.Forms.Label label7;
        private ProcessHacker.Components.FileNameBox fileModule;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label labelTEBAddress;
        private System.Windows.Forms.MenuItem analyzeMenuItem;
        private System.Windows.Forms.MenuItem analyzeWaitMenuItem;
        private System.Windows.Forms.MenuItem forceTerminateThreadMenuItem;
        private System.Windows.Forms.MenuItem tokenThreadMenuItem;
        private System.Windows.Forms.MenuItem permissionsThreadMenuItem;
        private System.Windows.Forms.MenuItem ioPriorityThreadMenuItem;
        private System.Windows.Forms.MenuItem ioPriority0ThreadMenuItem;
        private System.Windows.Forms.MenuItem ioPriority1ThreadMenuItem;
        private System.Windows.Forms.MenuItem ioPriority2ThreadMenuItem;
        private System.Windows.Forms.MenuItem ioPriority3ThreadMenuItem;
    }
}
