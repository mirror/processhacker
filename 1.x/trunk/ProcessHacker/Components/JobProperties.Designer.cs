namespace ProcessHacker.Components
{
    partial class JobProperties
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

            _jobObject.Dereference(disposing);

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
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabGeneral = new System.Windows.Forms.TabPage();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textJobName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.listLimits = new System.Windows.Forms.ListView();
            this.columnLimit = new System.Windows.Forms.ColumnHeader();
            this.columnValue = new System.Windows.Forms.ColumnHeader();
            this.listProcesses = new System.Windows.Forms.ListView();
            this.columnName = new System.Windows.Forms.ColumnHeader();
            this.columnPid = new System.Windows.Forms.ColumnHeader();
            this.tabStatistics = new System.Windows.Forms.TabPage();
            this.flowStatistics = new System.Windows.Forms.FlowLayoutPanel();
            this.groupGeneral = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label6 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.labelGeneralActiveProcesses = new System.Windows.Forms.Label();
            this.labelGeneralTotalProcesses = new System.Windows.Forms.Label();
            this.labelGeneralTerminatedProcesses = new System.Windows.Forms.Label();
            this.groupTime = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.labelTimeUserTime = new System.Windows.Forms.Label();
            this.labelTimeKernelTime = new System.Windows.Forms.Label();
            this.labelTimeUserTimePeriod = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.labelTimeKernelTimePeriod = new System.Windows.Forms.Label();
            this.groupMemory = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.labelMemoryPageFaults = new System.Windows.Forms.Label();
            this.labelMemoryPeakProcessUsage = new System.Windows.Forms.Label();
            this.labelMemoryPeakJobUsage = new System.Windows.Forms.Label();
            this.groupIO = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
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
            this.timerUpdate = new System.Windows.Forms.Timer(this.components);
            this.buttonTerminate = new System.Windows.Forms.Button();
            this.tabControl.SuspendLayout();
            this.tabGeneral.SuspendLayout();
            this.tabStatistics.SuspendLayout();
            this.flowStatistics.SuspendLayout();
            this.groupGeneral.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupTime.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.groupMemory.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.groupIO.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabGeneral);
            this.tabControl.Controls.Add(this.tabStatistics);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(646, 434);
            this.tabControl.TabIndex = 0;
            // 
            // tabGeneral
            // 
            this.tabGeneral.Controls.Add(this.buttonTerminate);
            this.tabGeneral.Controls.Add(this.label3);
            this.tabGeneral.Controls.Add(this.label2);
            this.tabGeneral.Controls.Add(this.textJobName);
            this.tabGeneral.Controls.Add(this.label1);
            this.tabGeneral.Controls.Add(this.listLimits);
            this.tabGeneral.Controls.Add(this.listProcesses);
            this.tabGeneral.Location = new System.Drawing.Point(4, 22);
            this.tabGeneral.Name = "tabGeneral";
            this.tabGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tabGeneral.Size = new System.Drawing.Size(638, 408);
            this.tabGeneral.TabIndex = 0;
            this.tabGeneral.Text = "General";
            this.tabGeneral.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 149);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(36, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Limits:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Process in job:";
            // 
            // textJobName
            // 
            this.textJobName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textJobName.Location = new System.Drawing.Point(50, 6);
            this.textJobName.Name = "textJobName";
            this.textJobName.ReadOnly = true;
            this.textJobName.Size = new System.Drawing.Size(501, 20);
            this.textJobName.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Name:";
            // 
            // listLimits
            // 
            this.listLimits.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listLimits.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnLimit,
            this.columnValue});
            this.listLimits.FullRowSelect = true;
            this.listLimits.HideSelection = false;
            this.listLimits.Location = new System.Drawing.Point(6, 165);
            this.listLimits.MultiSelect = false;
            this.listLimits.Name = "listLimits";
            this.listLimits.ShowItemToolTips = true;
            this.listLimits.Size = new System.Drawing.Size(626, 237);
            this.listLimits.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listLimits.TabIndex = 0;
            this.listLimits.UseCompatibleStateImageBehavior = false;
            this.listLimits.View = System.Windows.Forms.View.Details;
            // 
            // columnLimit
            // 
            this.columnLimit.Text = "Limit";
            this.columnLimit.Width = 250;
            // 
            // columnValue
            // 
            this.columnValue.Text = "Value";
            this.columnValue.Width = 150;
            // 
            // listProcesses
            // 
            this.listProcesses.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listProcesses.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnName,
            this.columnPid});
            this.listProcesses.FullRowSelect = true;
            this.listProcesses.HideSelection = false;
            this.listProcesses.Location = new System.Drawing.Point(6, 50);
            this.listProcesses.MultiSelect = false;
            this.listProcesses.Name = "listProcesses";
            this.listProcesses.ShowItemToolTips = true;
            this.listProcesses.Size = new System.Drawing.Size(626, 96);
            this.listProcesses.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listProcesses.TabIndex = 0;
            this.listProcesses.UseCompatibleStateImageBehavior = false;
            this.listProcesses.View = System.Windows.Forms.View.Details;
            // 
            // columnName
            // 
            this.columnName.Text = "Name";
            this.columnName.Width = 200;
            // 
            // columnPid
            // 
            this.columnPid.Text = "PID";
            // 
            // tabStatistics
            // 
            this.tabStatistics.Controls.Add(this.flowStatistics);
            this.tabStatistics.Location = new System.Drawing.Point(4, 22);
            this.tabStatistics.Name = "tabStatistics";
            this.tabStatistics.Padding = new System.Windows.Forms.Padding(3);
            this.tabStatistics.Size = new System.Drawing.Size(638, 408);
            this.tabStatistics.TabIndex = 2;
            this.tabStatistics.Text = "Statistics";
            this.tabStatistics.UseVisualStyleBackColor = true;
            // 
            // flowStatistics
            // 
            this.flowStatistics.Controls.Add(this.groupGeneral);
            this.flowStatistics.Controls.Add(this.groupTime);
            this.flowStatistics.Controls.Add(this.groupMemory);
            this.flowStatistics.Controls.Add(this.groupIO);
            this.flowStatistics.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowStatistics.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowStatistics.Location = new System.Drawing.Point(3, 3);
            this.flowStatistics.Name = "flowStatistics";
            this.flowStatistics.Size = new System.Drawing.Size(632, 402);
            this.flowStatistics.TabIndex = 2;
            // 
            // groupGeneral
            // 
            this.groupGeneral.Controls.Add(this.tableLayoutPanel1);
            this.groupGeneral.Location = new System.Drawing.Point(3, 3);
            this.groupGeneral.Name = "groupGeneral";
            this.groupGeneral.Size = new System.Drawing.Size(195, 81);
            this.groupGeneral.TabIndex = 1;
            this.groupGeneral.TabStop = false;
            this.groupGeneral.Text = "General";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.label6, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label8, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label9, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.labelGeneralActiveProcesses, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelGeneralTotalProcesses, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelGeneralTerminatedProcesses, 1, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(189, 62);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 3);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(89, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Active Processes";
            // 
            // label8
            // 
            this.label8.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 23);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(83, 13);
            this.label8.TabIndex = 1;
            this.label8.Text = "Total Processes";
            // 
            // label9
            // 
            this.label9.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(3, 44);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(112, 13);
            this.label9.TabIndex = 1;
            this.label9.Text = "Terminated Processes";
            // 
            // labelGeneralActiveProcesses
            // 
            this.labelGeneralActiveProcesses.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelGeneralActiveProcesses.AutoSize = true;
            this.labelGeneralActiveProcesses.Location = new System.Drawing.Point(153, 3);
            this.labelGeneralActiveProcesses.Name = "labelGeneralActiveProcesses";
            this.labelGeneralActiveProcesses.Size = new System.Drawing.Size(33, 13);
            this.labelGeneralActiveProcesses.TabIndex = 1;
            this.labelGeneralActiveProcesses.Text = "value";
            // 
            // labelGeneralTotalProcesses
            // 
            this.labelGeneralTotalProcesses.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelGeneralTotalProcesses.AutoSize = true;
            this.labelGeneralTotalProcesses.Location = new System.Drawing.Point(153, 23);
            this.labelGeneralTotalProcesses.Name = "labelGeneralTotalProcesses";
            this.labelGeneralTotalProcesses.Size = new System.Drawing.Size(33, 13);
            this.labelGeneralTotalProcesses.TabIndex = 1;
            this.labelGeneralTotalProcesses.Text = "value";
            // 
            // labelGeneralTerminatedProcesses
            // 
            this.labelGeneralTerminatedProcesses.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelGeneralTerminatedProcesses.AutoSize = true;
            this.labelGeneralTerminatedProcesses.Location = new System.Drawing.Point(153, 44);
            this.labelGeneralTerminatedProcesses.Name = "labelGeneralTerminatedProcesses";
            this.labelGeneralTerminatedProcesses.Size = new System.Drawing.Size(33, 13);
            this.labelGeneralTerminatedProcesses.TabIndex = 1;
            this.labelGeneralTerminatedProcesses.Text = "value";
            // 
            // groupTime
            // 
            this.groupTime.Controls.Add(this.tableLayoutPanel2);
            this.groupTime.Location = new System.Drawing.Point(3, 90);
            this.groupTime.Name = "groupTime";
            this.groupTime.Size = new System.Drawing.Size(195, 100);
            this.groupTime.TabIndex = 2;
            this.groupTime.TabStop = false;
            this.groupTime.Text = "Time";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.label4, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.label5, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.label7, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.labelTimeUserTime, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.labelTimeKernelTime, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.labelTimeUserTimePeriod, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.label13, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.labelTimeKernelTimePeriod, 1, 3);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 4;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(189, 81);
            this.tableLayoutPanel2.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 3);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "User Time";
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 23);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(63, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "Kernel Time";
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 43);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(112, 13);
            this.label7.TabIndex = 1;
            this.label7.Text = "User Time (this period)";
            // 
            // labelTimeUserTime
            // 
            this.labelTimeUserTime.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelTimeUserTime.AutoSize = true;
            this.labelTimeUserTime.Location = new System.Drawing.Point(153, 3);
            this.labelTimeUserTime.Name = "labelTimeUserTime";
            this.labelTimeUserTime.Size = new System.Drawing.Size(33, 13);
            this.labelTimeUserTime.TabIndex = 1;
            this.labelTimeUserTime.Text = "value";
            // 
            // labelTimeKernelTime
            // 
            this.labelTimeKernelTime.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelTimeKernelTime.AutoSize = true;
            this.labelTimeKernelTime.Location = new System.Drawing.Point(153, 23);
            this.labelTimeKernelTime.Name = "labelTimeKernelTime";
            this.labelTimeKernelTime.Size = new System.Drawing.Size(33, 13);
            this.labelTimeKernelTime.TabIndex = 1;
            this.labelTimeKernelTime.Text = "value";
            // 
            // labelTimeUserTimePeriod
            // 
            this.labelTimeUserTimePeriod.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelTimeUserTimePeriod.AutoSize = true;
            this.labelTimeUserTimePeriod.Location = new System.Drawing.Point(153, 43);
            this.labelTimeUserTimePeriod.Name = "labelTimeUserTimePeriod";
            this.labelTimeUserTimePeriod.Size = new System.Drawing.Size(33, 13);
            this.labelTimeUserTimePeriod.TabIndex = 1;
            this.labelTimeUserTimePeriod.Text = "value";
            // 
            // label13
            // 
            this.label13.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(3, 64);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(120, 13);
            this.label13.TabIndex = 1;
            this.label13.Text = "Kernel Time (this period)";
            // 
            // labelTimeKernelTimePeriod
            // 
            this.labelTimeKernelTimePeriod.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelTimeKernelTimePeriod.AutoSize = true;
            this.labelTimeKernelTimePeriod.Location = new System.Drawing.Point(153, 64);
            this.labelTimeKernelTimePeriod.Name = "labelTimeKernelTimePeriod";
            this.labelTimeKernelTimePeriod.Size = new System.Drawing.Size(33, 13);
            this.labelTimeKernelTimePeriod.TabIndex = 1;
            this.labelTimeKernelTimePeriod.Text = "value";
            // 
            // groupMemory
            // 
            this.groupMemory.Controls.Add(this.tableLayoutPanel3);
            this.groupMemory.Location = new System.Drawing.Point(3, 196);
            this.groupMemory.Name = "groupMemory";
            this.groupMemory.Size = new System.Drawing.Size(195, 78);
            this.groupMemory.TabIndex = 2;
            this.groupMemory.TabStop = false;
            this.groupMemory.Text = "Memory";
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.Controls.Add(this.label10, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.label11, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.label12, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.labelMemoryPageFaults, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.labelMemoryPeakProcessUsage, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.labelMemoryPeakJobUsage, 1, 2);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 3;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(189, 59);
            this.tableLayoutPanel3.TabIndex = 1;
            // 
            // label10
            // 
            this.label10.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(3, 3);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(63, 13);
            this.label10.TabIndex = 1;
            this.label10.Text = "Page Faults";
            // 
            // label11
            // 
            this.label11.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(3, 22);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(107, 13);
            this.label11.TabIndex = 1;
            this.label11.Text = "Peak Process Usage";
            // 
            // label12
            // 
            this.label12.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(3, 42);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(86, 13);
            this.label12.TabIndex = 1;
            this.label12.Text = "Peak Job Usage";
            // 
            // labelMemoryPageFaults
            // 
            this.labelMemoryPageFaults.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelMemoryPageFaults.AutoSize = true;
            this.labelMemoryPageFaults.Location = new System.Drawing.Point(153, 3);
            this.labelMemoryPageFaults.Name = "labelMemoryPageFaults";
            this.labelMemoryPageFaults.Size = new System.Drawing.Size(33, 13);
            this.labelMemoryPageFaults.TabIndex = 1;
            this.labelMemoryPageFaults.Text = "value";
            // 
            // labelMemoryPeakProcessUsage
            // 
            this.labelMemoryPeakProcessUsage.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelMemoryPeakProcessUsage.AutoSize = true;
            this.labelMemoryPeakProcessUsage.Location = new System.Drawing.Point(153, 22);
            this.labelMemoryPeakProcessUsage.Name = "labelMemoryPeakProcessUsage";
            this.labelMemoryPeakProcessUsage.Size = new System.Drawing.Size(33, 13);
            this.labelMemoryPeakProcessUsage.TabIndex = 1;
            this.labelMemoryPeakProcessUsage.Text = "value";
            // 
            // labelMemoryPeakJobUsage
            // 
            this.labelMemoryPeakJobUsage.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelMemoryPeakJobUsage.AutoSize = true;
            this.labelMemoryPeakJobUsage.Location = new System.Drawing.Point(153, 42);
            this.labelMemoryPeakJobUsage.Name = "labelMemoryPeakJobUsage";
            this.labelMemoryPeakJobUsage.Size = new System.Drawing.Size(33, 13);
            this.labelMemoryPeakJobUsage.TabIndex = 1;
            this.labelMemoryPeakJobUsage.Text = "value";
            // 
            // groupIO
            // 
            this.groupIO.Controls.Add(this.tableLayoutPanel4);
            this.groupIO.Location = new System.Drawing.Point(204, 3);
            this.groupIO.Name = "groupIO";
            this.groupIO.Size = new System.Drawing.Size(195, 136);
            this.groupIO.TabIndex = 3;
            this.groupIO.TabStop = false;
            this.groupIO.Text = "I/O";
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 2;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel4.Controls.Add(this.label16, 0, 5);
            this.tableLayoutPanel4.Controls.Add(this.label17, 0, 4);
            this.tableLayoutPanel4.Controls.Add(this.label18, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.label19, 0, 1);
            this.tableLayoutPanel4.Controls.Add(this.label21, 0, 2);
            this.tableLayoutPanel4.Controls.Add(this.label23, 0, 3);
            this.tableLayoutPanel4.Controls.Add(this.labelIOReads, 1, 0);
            this.tableLayoutPanel4.Controls.Add(this.labelIOReadBytes, 1, 1);
            this.tableLayoutPanel4.Controls.Add(this.labelIOWrites, 1, 2);
            this.tableLayoutPanel4.Controls.Add(this.labelIOWriteBytes, 1, 3);
            this.tableLayoutPanel4.Controls.Add(this.labelIOOther, 1, 4);
            this.tableLayoutPanel4.Controls.Add(this.labelIOOtherBytes, 1, 5);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 6;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(189, 117);
            this.tableLayoutPanel4.TabIndex = 1;
            // 
            // label16
            // 
            this.label16.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(3, 99);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(62, 13);
            this.label16.TabIndex = 5;
            this.label16.Text = "Other Bytes";
            // 
            // label17
            // 
            this.label17.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(3, 79);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(33, 13);
            this.label17.TabIndex = 3;
            this.label17.Text = "Other";
            // 
            // label18
            // 
            this.label18.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(3, 3);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(38, 13);
            this.label18.TabIndex = 1;
            this.label18.Text = "Reads";
            // 
            // label19
            // 
            this.label19.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(3, 22);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(62, 13);
            this.label19.TabIndex = 1;
            this.label19.Text = "Read Bytes";
            // 
            // label21
            // 
            this.label21.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(3, 41);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(37, 13);
            this.label21.TabIndex = 1;
            this.label21.Text = "Writes";
            // 
            // label23
            // 
            this.label23.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(3, 60);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(61, 13);
            this.label23.TabIndex = 1;
            this.label23.Text = "Write Bytes";
            // 
            // labelIOReads
            // 
            this.labelIOReads.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelIOReads.AutoSize = true;
            this.labelIOReads.Location = new System.Drawing.Point(153, 3);
            this.labelIOReads.Name = "labelIOReads";
            this.labelIOReads.Size = new System.Drawing.Size(33, 13);
            this.labelIOReads.TabIndex = 1;
            this.labelIOReads.Text = "value";
            // 
            // labelIOReadBytes
            // 
            this.labelIOReadBytes.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelIOReadBytes.AutoSize = true;
            this.labelIOReadBytes.Location = new System.Drawing.Point(153, 22);
            this.labelIOReadBytes.Name = "labelIOReadBytes";
            this.labelIOReadBytes.Size = new System.Drawing.Size(33, 13);
            this.labelIOReadBytes.TabIndex = 1;
            this.labelIOReadBytes.Text = "value";
            // 
            // labelIOWrites
            // 
            this.labelIOWrites.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelIOWrites.AutoSize = true;
            this.labelIOWrites.Location = new System.Drawing.Point(153, 41);
            this.labelIOWrites.Name = "labelIOWrites";
            this.labelIOWrites.Size = new System.Drawing.Size(33, 13);
            this.labelIOWrites.TabIndex = 1;
            this.labelIOWrites.Text = "value";
            // 
            // labelIOWriteBytes
            // 
            this.labelIOWriteBytes.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelIOWriteBytes.AutoSize = true;
            this.labelIOWriteBytes.Location = new System.Drawing.Point(153, 60);
            this.labelIOWriteBytes.Name = "labelIOWriteBytes";
            this.labelIOWriteBytes.Size = new System.Drawing.Size(33, 13);
            this.labelIOWriteBytes.TabIndex = 1;
            this.labelIOWriteBytes.Text = "value";
            // 
            // labelIOOther
            // 
            this.labelIOOther.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelIOOther.AutoSize = true;
            this.labelIOOther.Location = new System.Drawing.Point(153, 79);
            this.labelIOOther.Name = "labelIOOther";
            this.labelIOOther.Size = new System.Drawing.Size(33, 13);
            this.labelIOOther.TabIndex = 1;
            this.labelIOOther.Text = "value";
            // 
            // labelIOOtherBytes
            // 
            this.labelIOOtherBytes.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelIOOtherBytes.AutoSize = true;
            this.labelIOOtherBytes.Location = new System.Drawing.Point(153, 99);
            this.labelIOOtherBytes.Name = "labelIOOtherBytes";
            this.labelIOOtherBytes.Size = new System.Drawing.Size(33, 13);
            this.labelIOOtherBytes.TabIndex = 1;
            this.labelIOOtherBytes.Text = "value";
            // 
            // timerUpdate
            // 
            this.timerUpdate.Enabled = true;
            this.timerUpdate.Interval = 1000;
            this.timerUpdate.Tick += new System.EventHandler(this.timerUpdate_Tick);
            // 
            // buttonTerminate
            // 
            this.buttonTerminate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonTerminate.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonTerminate.Location = new System.Drawing.Point(557, 6);
            this.buttonTerminate.Name = "buttonTerminate";
            this.buttonTerminate.Size = new System.Drawing.Size(75, 23);
            this.buttonTerminate.TabIndex = 5;
            this.buttonTerminate.Text = "Terminate";
            this.buttonTerminate.UseVisualStyleBackColor = true;
            this.buttonTerminate.Click += new System.EventHandler(this.buttonTerminate_Click);
            // 
            // JobProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl);
            this.DoubleBuffered = true;
            this.Name = "JobProperties";
            this.Size = new System.Drawing.Size(646, 434);
            this.tabControl.ResumeLayout(false);
            this.tabGeneral.ResumeLayout(false);
            this.tabGeneral.PerformLayout();
            this.tabStatistics.ResumeLayout(false);
            this.flowStatistics.ResumeLayout(false);
            this.groupGeneral.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.groupTime.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.groupMemory.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.groupIO.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabGeneral;
        private System.Windows.Forms.TabPage tabStatistics;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textJobName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView listProcesses;
        private System.Windows.Forms.ColumnHeader columnName;
        private System.Windows.Forms.ColumnHeader columnPid;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListView listLimits;
        private System.Windows.Forms.ColumnHeader columnLimit;
        private System.Windows.Forms.ColumnHeader columnValue;
        private System.Windows.Forms.GroupBox groupGeneral;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label labelGeneralActiveProcesses;
        private System.Windows.Forms.Label labelGeneralTotalProcesses;
        private System.Windows.Forms.Label labelGeneralTerminatedProcesses;
        private System.Windows.Forms.FlowLayoutPanel flowStatistics;
        private System.Windows.Forms.Timer timerUpdate;
        private System.Windows.Forms.GroupBox groupTime;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label labelTimeUserTime;
        private System.Windows.Forms.Label labelTimeKernelTime;
        private System.Windows.Forms.Label labelTimeUserTimePeriod;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label labelTimeKernelTimePeriod;
        private System.Windows.Forms.GroupBox groupMemory;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label labelMemoryPageFaults;
        private System.Windows.Forms.Label labelMemoryPeakProcessUsage;
        private System.Windows.Forms.Label labelMemoryPeakJobUsage;
        private System.Windows.Forms.GroupBox groupIO;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
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
        private System.Windows.Forms.Button buttonTerminate;
    }
}
