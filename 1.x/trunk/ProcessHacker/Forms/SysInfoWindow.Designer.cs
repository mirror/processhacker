namespace ProcessHacker
{
    partial class SysInfoWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SysInfoWindow));
            this.gboxCPUPlotter = new System.Windows.Forms.GroupBox();
            this.tableCPUs = new System.Windows.Forms.TableLayoutPanel();
            this.plotterCPU = new ProcessHacker.Components.Plotter();
            this.tableGraphs = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.plotterIO = new ProcessHacker.Components.Plotter();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.plotterMemory = new ProcessHacker.Components.Plotter();
            this.groupBox11 = new System.Windows.Forms.GroupBox();
            this.indicatorPhysical = new ProcessHacker.Components.Indicator();
            this.groupBox12 = new System.Windows.Forms.GroupBox();
            this.indicatorIO = new ProcessHacker.Components.Indicator();
            this.groupBox13 = new System.Windows.Forms.GroupBox();
            this.indicatorCpu = new ProcessHacker.Components.Indicator();
            this.checkShowOneGraphPerCPU = new System.Windows.Forms.CheckBox();
            this.flowInfo = new System.Windows.Forms.FlowLayoutPanel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.labelTotalsUptime = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.labelTotalsProcesses = new System.Windows.Forms.Label();
            this.labelTotalsThreads = new System.Windows.Forms.Label();
            this.labelTotalsHandles = new System.Windows.Forms.Label();
            this.label34 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.labelCCC = new System.Windows.Forms.Label();
            this.labelCCP = new System.Windows.Forms.Label();
            this.labelCCL = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.label4 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.labelPMC = new System.Windows.Forms.Label();
            this.labelPMT = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.labelPSC = new System.Windows.Forms.Label();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.label15 = new System.Windows.Forms.Label();
            this.labelCacheMaximum = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.labelCacheMinimum = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.labelCacheCurrent = new System.Windows.Forms.Label();
            this.labelCachePeak = new System.Windows.Forms.Label();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.label14 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.labelKPPPU = new System.Windows.Forms.Label();
            this.labelKPPA = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.labelKPPVU = new System.Windows.Forms.Label();
            this.labelKPPF = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.labelKPPL = new System.Windows.Forms.Label();
            this.label33 = new System.Windows.Forms.Label();
            this.labelKPNPL = new System.Windows.Forms.Label();
            this.labelKPNPF = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.labelKPNPA = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.labelKPNPU = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.label20 = new System.Windows.Forms.Label();
            this.labelPFCache = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.labelPFDZ = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.labelPFTotal = new System.Windows.Forms.Label();
            this.labelPFTrans = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.labelPFCOW = new System.Windows.Forms.Label();
            this.labelPFCacheTrans = new System.Windows.Forms.Label();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel7 = new System.Windows.Forms.TableLayoutPanel();
            this.label16 = new System.Windows.Forms.Label();
            this.labelIOOB = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.labelIOO = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            this.label32 = new System.Windows.Forms.Label();
            this.labelIOR = new System.Windows.Forms.Label();
            this.labelIOW = new System.Windows.Forms.Label();
            this.label35 = new System.Windows.Forms.Label();
            this.labelIORB = new System.Windows.Forms.Label();
            this.labelIOWB = new System.Windows.Forms.Label();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel8 = new System.Windows.Forms.TableLayoutPanel();
            this.label37 = new System.Windows.Forms.Label();
            this.label38 = new System.Windows.Forms.Label();
            this.labelCPUContextSwitches = new System.Windows.Forms.Label();
            this.labelCPUSystemCalls = new System.Windows.Forms.Label();
            this.label41 = new System.Windows.Forms.Label();
            this.labelCPUInterrupts = new System.Windows.Forms.Label();
            this.checkAlwaysOnTop = new System.Windows.Forms.CheckBox();
            this.gboxCPUPlotter.SuspendLayout();
            this.tableGraphs.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox11.SuspendLayout();
            this.groupBox12.SuspendLayout();
            this.groupBox13.SuspendLayout();
            this.flowInfo.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.tableLayoutPanel6.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.tableLayoutPanel7.SuspendLayout();
            this.groupBox10.SuspendLayout();
            this.tableLayoutPanel8.SuspendLayout();
            this.SuspendLayout();
            // 
            // gboxCPUPlotter
            // 
            this.gboxCPUPlotter.Controls.Add(this.tableCPUs);
            this.gboxCPUPlotter.Controls.Add(this.plotterCPU);
            this.gboxCPUPlotter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gboxCPUPlotter.Location = new System.Drawing.Point(89, 3);
            this.gboxCPUPlotter.Name = "gboxCPUPlotter";
            this.gboxCPUPlotter.Size = new System.Drawing.Size(726, 62);
            this.gboxCPUPlotter.TabIndex = 2;
            this.gboxCPUPlotter.TabStop = false;
            this.gboxCPUPlotter.Text = "CPU Usage (Kernel, User)";
            // 
            // tableCPUs
            // 
            this.tableCPUs.ColumnCount = 1;
            this.tableCPUs.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableCPUs.Location = new System.Drawing.Point(436, 34);
            this.tableCPUs.Name = "tableCPUs";
            this.tableCPUs.RowCount = 1;
            this.tableCPUs.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableCPUs.Size = new System.Drawing.Size(46, 20);
            this.tableCPUs.TabIndex = 3;
            this.tableCPUs.Visible = false;
            // 
            // plotterCPU
            // 
            this.plotterCPU.BackColor = System.Drawing.Color.Black;
            this.plotterCPU.Data1 = null;
            this.plotterCPU.Data2 = null;
            this.plotterCPU.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plotterCPU.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(128)))), ((int)(((byte)(64)))));
            this.plotterCPU.GridSize = new System.Drawing.Size(12, 12);
            this.plotterCPU.LineColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))));
            this.plotterCPU.LineColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.plotterCPU.Location = new System.Drawing.Point(3, 16);
            this.plotterCPU.LongData1 = null;
            this.plotterCPU.LongData2 = null;
            this.plotterCPU.MinMaxValue = ((long)(0));
            this.plotterCPU.MoveStep = -1;
            this.plotterCPU.Name = "plotterCPU";
            this.plotterCPU.OverlaySecondLine = false;
            this.plotterCPU.ShowGrid = true;
            this.plotterCPU.Size = new System.Drawing.Size(720, 43);
            this.plotterCPU.TabIndex = 0;
            this.plotterCPU.TextBoxColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.plotterCPU.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))));
            this.plotterCPU.TextMargin = new System.Windows.Forms.Padding(3);
            this.plotterCPU.TextPadding = new System.Windows.Forms.Padding(3);
            this.plotterCPU.TextPosition = System.Drawing.ContentAlignment.TopLeft;
            this.plotterCPU.UseLongData = false;
            this.plotterCPU.UseSecondLine = true;
            // 
            // tableGraphs
            // 
            this.tableGraphs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableGraphs.ColumnCount = 2;
            this.tableGraphs.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 86F));
            this.tableGraphs.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableGraphs.Controls.Add(this.gboxCPUPlotter, 1, 0);
            this.tableGraphs.Controls.Add(this.groupBox2, 1, 2);
            this.tableGraphs.Controls.Add(this.groupBox1, 1, 3);
            this.tableGraphs.Controls.Add(this.groupBox11, 0, 3);
            this.tableGraphs.Controls.Add(this.groupBox12, 0, 2);
            this.tableGraphs.Controls.Add(this.groupBox13, 0, 0);
            this.tableGraphs.Controls.Add(this.checkShowOneGraphPerCPU, 1, 1);
            this.tableGraphs.Location = new System.Drawing.Point(12, 12);
            this.tableGraphs.Name = "tableGraphs";
            this.tableGraphs.RowCount = 4;
            this.tableGraphs.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableGraphs.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableGraphs.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableGraphs.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableGraphs.Size = new System.Drawing.Size(818, 228);
            this.tableGraphs.TabIndex = 3;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.plotterIO);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(89, 95);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(726, 62);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "I/O (R+O, W)";
            // 
            // plotterIO
            // 
            this.plotterIO.BackColor = System.Drawing.Color.Black;
            this.plotterIO.Data1 = null;
            this.plotterIO.Data2 = null;
            this.plotterIO.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plotterIO.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(128)))), ((int)(((byte)(64)))));
            this.plotterIO.GridSize = new System.Drawing.Size(12, 12);
            this.plotterIO.LineColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))));
            this.plotterIO.LineColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.plotterIO.Location = new System.Drawing.Point(3, 16);
            this.plotterIO.LongData1 = null;
            this.plotterIO.LongData2 = null;
            this.plotterIO.MinMaxValue = ((long)(0));
            this.plotterIO.MoveStep = -1;
            this.plotterIO.Name = "plotterIO";
            this.plotterIO.OverlaySecondLine = true;
            this.plotterIO.ShowGrid = true;
            this.plotterIO.Size = new System.Drawing.Size(720, 43);
            this.plotterIO.TabIndex = 5;
            this.plotterIO.TextBoxColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.plotterIO.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))));
            this.plotterIO.TextMargin = new System.Windows.Forms.Padding(3);
            this.plotterIO.TextPadding = new System.Windows.Forms.Padding(3);
            this.plotterIO.TextPosition = System.Drawing.ContentAlignment.TopLeft;
            this.plotterIO.UseLongData = true;
            this.plotterIO.UseSecondLine = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.plotterMemory);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(89, 163);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(726, 62);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Commit, Physical Memory";
            // 
            // plotterMemory
            // 
            this.plotterMemory.BackColor = System.Drawing.Color.Black;
            this.plotterMemory.Data1 = null;
            this.plotterMemory.Data2 = null;
            this.plotterMemory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plotterMemory.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(128)))), ((int)(((byte)(64)))));
            this.plotterMemory.GridSize = new System.Drawing.Size(12, 12);
            this.plotterMemory.LineColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))));
            this.plotterMemory.LineColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.plotterMemory.Location = new System.Drawing.Point(3, 16);
            this.plotterMemory.LongData1 = null;
            this.plotterMemory.LongData2 = null;
            this.plotterMemory.MinMaxValue = ((long)(0));
            this.plotterMemory.MoveStep = -1;
            this.plotterMemory.Name = "plotterMemory";
            this.plotterMemory.OverlaySecondLine = true;
            this.plotterMemory.ShowGrid = true;
            this.plotterMemory.Size = new System.Drawing.Size(720, 43);
            this.plotterMemory.TabIndex = 5;
            this.plotterMemory.TextBoxColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.plotterMemory.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))));
            this.plotterMemory.TextMargin = new System.Windows.Forms.Padding(3);
            this.plotterMemory.TextPadding = new System.Windows.Forms.Padding(3);
            this.plotterMemory.TextPosition = System.Drawing.ContentAlignment.TopLeft;
            this.plotterMemory.UseLongData = true;
            this.plotterMemory.UseSecondLine = true;
            // 
            // groupBox11
            // 
            this.groupBox11.Controls.Add(this.indicatorPhysical);
            this.groupBox11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox11.Location = new System.Drawing.Point(3, 163);
            this.groupBox11.Name = "groupBox11";
            this.groupBox11.Size = new System.Drawing.Size(80, 62);
            this.groupBox11.TabIndex = 9;
            this.groupBox11.TabStop = false;
            this.groupBox11.Text = "Physical";
            // 
            // indicatorPhysical
            // 
            this.indicatorPhysical.BackColor = System.Drawing.Color.Black;
            this.indicatorPhysical.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.indicatorPhysical.Color2 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))));
            this.indicatorPhysical.Data1 = ((long)(0));
            this.indicatorPhysical.Data2 = ((long)(0));
            this.indicatorPhysical.Dock = System.Windows.Forms.DockStyle.Fill;
            this.indicatorPhysical.ForeColor = System.Drawing.Color.Lime;
            this.indicatorPhysical.GraphWidth = 33;
            this.indicatorPhysical.Location = new System.Drawing.Point(3, 16);
            this.indicatorPhysical.Maximum = ((long)(2147483647));
            this.indicatorPhysical.Minimum = ((long)(0));
            this.indicatorPhysical.Name = "indicatorPhysical";
            this.indicatorPhysical.Size = new System.Drawing.Size(74, 43);
            this.indicatorPhysical.TabIndex = 8;
            this.indicatorPhysical.TextValue = "";
            // 
            // groupBox12
            // 
            this.groupBox12.Controls.Add(this.indicatorIO);
            this.groupBox12.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox12.Location = new System.Drawing.Point(3, 95);
            this.groupBox12.Name = "groupBox12";
            this.groupBox12.Size = new System.Drawing.Size(80, 62);
            this.groupBox12.TabIndex = 10;
            this.groupBox12.TabStop = false;
            this.groupBox12.Text = "I/O (R+O)";
            // 
            // indicatorIO
            // 
            this.indicatorIO.BackColor = System.Drawing.Color.Black;
            this.indicatorIO.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.indicatorIO.Color2 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))));
            this.indicatorIO.Data1 = ((long)(0));
            this.indicatorIO.Data2 = ((long)(0));
            this.indicatorIO.Dock = System.Windows.Forms.DockStyle.Fill;
            this.indicatorIO.ForeColor = System.Drawing.Color.Lime;
            this.indicatorIO.GraphWidth = 33;
            this.indicatorIO.Location = new System.Drawing.Point(3, 16);
            this.indicatorIO.Maximum = ((long)(2147483647));
            this.indicatorIO.Minimum = ((long)(0));
            this.indicatorIO.Name = "indicatorIO";
            this.indicatorIO.Size = new System.Drawing.Size(74, 43);
            this.indicatorIO.TabIndex = 8;
            this.indicatorIO.TextValue = "";
            // 
            // groupBox13
            // 
            this.groupBox13.Controls.Add(this.indicatorCpu);
            this.groupBox13.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox13.Location = new System.Drawing.Point(3, 3);
            this.groupBox13.Name = "groupBox13";
            this.groupBox13.Size = new System.Drawing.Size(80, 62);
            this.groupBox13.TabIndex = 11;
            this.groupBox13.TabStop = false;
            this.groupBox13.Text = "CPU Usage";
            // 
            // indicatorCpu
            // 
            this.indicatorCpu.BackColor = System.Drawing.Color.Black;
            this.indicatorCpu.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.indicatorCpu.Color2 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))));
            this.indicatorCpu.Data1 = ((long)(500000000));
            this.indicatorCpu.Data2 = ((long)(500000000));
            this.indicatorCpu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.indicatorCpu.ForeColor = System.Drawing.Color.Lime;
            this.indicatorCpu.GraphWidth = 33;
            this.indicatorCpu.Location = new System.Drawing.Point(3, 16);
            this.indicatorCpu.Maximum = ((long)(2147483647));
            this.indicatorCpu.Minimum = ((long)(0));
            this.indicatorCpu.Name = "indicatorCpu";
            this.indicatorCpu.Size = new System.Drawing.Size(74, 43);
            this.indicatorCpu.TabIndex = 8;
            this.indicatorCpu.TextValue = "";
            // 
            // checkShowOneGraphPerCPU
            // 
            this.checkShowOneGraphPerCPU.AutoSize = true;
            this.checkShowOneGraphPerCPU.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkShowOneGraphPerCPU.Location = new System.Drawing.Point(89, 71);
            this.checkShowOneGraphPerCPU.Name = "checkShowOneGraphPerCPU";
            this.checkShowOneGraphPerCPU.Size = new System.Drawing.Size(153, 18);
            this.checkShowOneGraphPerCPU.TabIndex = 3;
            this.checkShowOneGraphPerCPU.Text = "Show one graph per CPU";
            this.checkShowOneGraphPerCPU.UseVisualStyleBackColor = true;
            this.checkShowOneGraphPerCPU.CheckedChanged += new System.EventHandler(this.checkShowOneGraphPerCPU_CheckedChanged);
            // 
            // flowInfo
            // 
            this.flowInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.flowInfo.AutoScroll = true;
            this.flowInfo.Controls.Add(this.groupBox3);
            this.flowInfo.Controls.Add(this.groupBox4);
            this.flowInfo.Controls.Add(this.groupBox5);
            this.flowInfo.Controls.Add(this.groupBox6);
            this.flowInfo.Controls.Add(this.groupBox7);
            this.flowInfo.Controls.Add(this.groupBox8);
            this.flowInfo.Controls.Add(this.groupBox9);
            this.flowInfo.Controls.Add(this.groupBox10);
            this.flowInfo.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowInfo.Location = new System.Drawing.Point(12, 246);
            this.flowInfo.Name = "flowInfo";
            this.flowInfo.Size = new System.Drawing.Size(818, 256);
            this.flowInfo.TabIndex = 4;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.tableLayoutPanel1);
            this.groupBox3.Location = new System.Drawing.Point(3, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(195, 84);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "System";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.labelTotalsUptime, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.label6, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label8, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label9, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.labelTotalsProcesses, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelTotalsThreads, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelTotalsHandles, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.label34, 0, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(189, 65);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // labelTotalsUptime
            // 
            this.labelTotalsUptime.AutoEllipsis = true;
            this.labelTotalsUptime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelTotalsUptime.Location = new System.Drawing.Point(65, 48);
            this.labelTotalsUptime.Name = "labelTotalsUptime";
            this.labelTotalsUptime.Size = new System.Drawing.Size(121, 17);
            this.labelTotalsUptime.TabIndex = 2;
            this.labelTotalsUptime.Text = "value";
            this.labelTotalsUptime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 1);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(56, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Processes";
            // 
            // label8
            // 
            this.label8.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 17);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(46, 13);
            this.label8.TabIndex = 1;
            this.label8.Text = "Threads";
            // 
            // label9
            // 
            this.label9.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(3, 33);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(46, 13);
            this.label9.TabIndex = 1;
            this.label9.Text = "Handles";
            // 
            // labelTotalsProcesses
            // 
            this.labelTotalsProcesses.AutoEllipsis = true;
            this.labelTotalsProcesses.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelTotalsProcesses.Location = new System.Drawing.Point(65, 0);
            this.labelTotalsProcesses.Name = "labelTotalsProcesses";
            this.labelTotalsProcesses.Size = new System.Drawing.Size(121, 16);
            this.labelTotalsProcesses.TabIndex = 1;
            this.labelTotalsProcesses.Text = "value";
            this.labelTotalsProcesses.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelTotalsThreads
            // 
            this.labelTotalsThreads.AutoEllipsis = true;
            this.labelTotalsThreads.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelTotalsThreads.Location = new System.Drawing.Point(65, 16);
            this.labelTotalsThreads.Name = "labelTotalsThreads";
            this.labelTotalsThreads.Size = new System.Drawing.Size(121, 16);
            this.labelTotalsThreads.TabIndex = 1;
            this.labelTotalsThreads.Text = "value";
            this.labelTotalsThreads.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelTotalsHandles
            // 
            this.labelTotalsHandles.AutoEllipsis = true;
            this.labelTotalsHandles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelTotalsHandles.Location = new System.Drawing.Point(65, 32);
            this.labelTotalsHandles.Name = "labelTotalsHandles";
            this.labelTotalsHandles.Size = new System.Drawing.Size(121, 16);
            this.labelTotalsHandles.TabIndex = 1;
            this.labelTotalsHandles.Text = "value";
            this.labelTotalsHandles.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label34
            // 
            this.label34.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label34.AutoSize = true;
            this.label34.Location = new System.Drawing.Point(3, 50);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(40, 13);
            this.label34.TabIndex = 1;
            this.label34.Text = "Uptime";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.tableLayoutPanel2);
            this.groupBox4.Location = new System.Drawing.Point(3, 93);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(195, 78);
            this.groupBox4.TabIndex = 2;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Commit Charge";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.label3, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.labelCCC, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.labelCCP, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.labelCCL, 1, 2);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(189, 59);
            this.tableLayoutPanel2.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Current";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Peak";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 42);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(28, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Limit";
            // 
            // labelCCC
            // 
            this.labelCCC.AutoEllipsis = true;
            this.labelCCC.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelCCC.Location = new System.Drawing.Point(50, 0);
            this.labelCCC.Name = "labelCCC";
            this.labelCCC.Size = new System.Drawing.Size(136, 19);
            this.labelCCC.TabIndex = 1;
            this.labelCCC.Text = "value";
            this.labelCCC.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelCCP
            // 
            this.labelCCP.AutoEllipsis = true;
            this.labelCCP.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelCCP.Location = new System.Drawing.Point(50, 19);
            this.labelCCP.Name = "labelCCP";
            this.labelCCP.Size = new System.Drawing.Size(136, 19);
            this.labelCCP.TabIndex = 1;
            this.labelCCP.Text = "value";
            this.labelCCP.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelCCL
            // 
            this.labelCCL.AutoEllipsis = true;
            this.labelCCL.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelCCL.Location = new System.Drawing.Point(50, 38);
            this.labelCCL.Name = "labelCCL";
            this.labelCCL.Size = new System.Drawing.Size(136, 21);
            this.labelCCL.TabIndex = 1;
            this.labelCCL.Text = "value";
            this.labelCCL.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.tableLayoutPanel3);
            this.groupBox5.Location = new System.Drawing.Point(3, 177);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(195, 75);
            this.groupBox5.TabIndex = 3;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Physical Memory";
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.Controls.Add(this.label4, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.label7, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.labelPMC, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.labelPMT, 1, 2);
            this.tableLayoutPanel3.Controls.Add(this.label19, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.labelPSC, 1, 1);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 3;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(189, 56);
            this.tableLayoutPanel3.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 2);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Current";
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 39);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(31, 13);
            this.label7.TabIndex = 1;
            this.label7.Text = "Total";
            // 
            // labelPMC
            // 
            this.labelPMC.AutoEllipsis = true;
            this.labelPMC.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelPMC.Location = new System.Drawing.Point(84, 0);
            this.labelPMC.Name = "labelPMC";
            this.labelPMC.Size = new System.Drawing.Size(102, 18);
            this.labelPMC.TabIndex = 1;
            this.labelPMC.Text = "value";
            this.labelPMC.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelPMT
            // 
            this.labelPMT.AutoEllipsis = true;
            this.labelPMT.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelPMT.Location = new System.Drawing.Point(84, 36);
            this.labelPMT.Name = "labelPMT";
            this.labelPMT.Size = new System.Drawing.Size(102, 20);
            this.labelPMT.TabIndex = 1;
            this.labelPMT.Text = "value";
            this.labelPMT.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label19
            // 
            this.label19.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(3, 20);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(75, 13);
            this.label19.TabIndex = 1;
            this.label19.Text = "System Cache";
            // 
            // labelPSC
            // 
            this.labelPSC.AutoEllipsis = true;
            this.labelPSC.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelPSC.Location = new System.Drawing.Point(84, 18);
            this.labelPSC.Name = "labelPSC";
            this.labelPSC.Size = new System.Drawing.Size(102, 18);
            this.labelPSC.TabIndex = 1;
            this.labelPSC.Text = "value";
            this.labelPSC.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.tableLayoutPanel4);
            this.groupBox6.Location = new System.Drawing.Point(204, 3);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(195, 85);
            this.groupBox6.TabIndex = 4;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "File Cache";
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 2;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel4.Controls.Add(this.label15, 0, 3);
            this.tableLayoutPanel4.Controls.Add(this.labelCacheMaximum, 0, 3);
            this.tableLayoutPanel4.Controls.Add(this.label13, 0, 2);
            this.tableLayoutPanel4.Controls.Add(this.labelCacheMinimum, 0, 2);
            this.tableLayoutPanel4.Controls.Add(this.label5, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.label10, 0, 1);
            this.tableLayoutPanel4.Controls.Add(this.labelCacheCurrent, 1, 0);
            this.tableLayoutPanel4.Controls.Add(this.labelCachePeak, 1, 1);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 4;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(189, 66);
            this.tableLayoutPanel4.TabIndex = 1;
            // 
            // label15
            // 
            this.label15.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(3, 50);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(51, 13);
            this.label15.TabIndex = 5;
            this.label15.Text = "Maximum";
            // 
            // labelCacheMaximum
            // 
            this.labelCacheMaximum.AutoEllipsis = true;
            this.labelCacheMaximum.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelCacheMaximum.Location = new System.Drawing.Point(60, 48);
            this.labelCacheMaximum.Name = "labelCacheMaximum";
            this.labelCacheMaximum.Size = new System.Drawing.Size(130, 18);
            this.labelCacheMaximum.TabIndex = 4;
            this.labelCacheMaximum.Text = "value";
            this.labelCacheMaximum.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label13
            // 
            this.label13.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(3, 33);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(48, 13);
            this.label13.TabIndex = 3;
            this.label13.Text = "Minimum";
            // 
            // labelCacheMinimum
            // 
            this.labelCacheMinimum.AutoEllipsis = true;
            this.labelCacheMinimum.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelCacheMinimum.Location = new System.Drawing.Point(60, 32);
            this.labelCacheMinimum.Name = "labelCacheMinimum";
            this.labelCacheMinimum.Size = new System.Drawing.Size(130, 16);
            this.labelCacheMinimum.TabIndex = 2;
            this.labelCacheMinimum.Text = "value";
            this.labelCacheMinimum.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 1);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "Current";
            // 
            // label10
            // 
            this.label10.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(3, 17);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(32, 13);
            this.label10.TabIndex = 1;
            this.label10.Text = "Peak";
            // 
            // labelCacheCurrent
            // 
            this.labelCacheCurrent.AutoEllipsis = true;
            this.labelCacheCurrent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelCacheCurrent.Location = new System.Drawing.Point(60, 0);
            this.labelCacheCurrent.Name = "labelCacheCurrent";
            this.labelCacheCurrent.Size = new System.Drawing.Size(130, 16);
            this.labelCacheCurrent.TabIndex = 1;
            this.labelCacheCurrent.Text = "value";
            this.labelCacheCurrent.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelCachePeak
            // 
            this.labelCachePeak.AutoEllipsis = true;
            this.labelCachePeak.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelCachePeak.Location = new System.Drawing.Point(60, 16);
            this.labelCachePeak.Name = "labelCachePeak";
            this.labelCachePeak.Size = new System.Drawing.Size(130, 16);
            this.labelCachePeak.TabIndex = 1;
            this.labelCachePeak.Text = "value";
            this.labelCachePeak.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.tableLayoutPanel5);
            this.groupBox7.Location = new System.Drawing.Point(204, 94);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(195, 157);
            this.groupBox7.TabIndex = 5;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Kernel Pools";
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 2;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel5.Controls.Add(this.label14, 0, 3);
            this.tableLayoutPanel5.Controls.Add(this.label17, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.label18, 0, 2);
            this.tableLayoutPanel5.Controls.Add(this.labelKPPPU, 1, 0);
            this.tableLayoutPanel5.Controls.Add(this.labelKPPA, 1, 2);
            this.tableLayoutPanel5.Controls.Add(this.label12, 0, 1);
            this.tableLayoutPanel5.Controls.Add(this.labelKPPVU, 1, 1);
            this.tableLayoutPanel5.Controls.Add(this.labelKPPF, 1, 3);
            this.tableLayoutPanel5.Controls.Add(this.label29, 0, 4);
            this.tableLayoutPanel5.Controls.Add(this.labelKPPL, 1, 4);
            this.tableLayoutPanel5.Controls.Add(this.label33, 0, 8);
            this.tableLayoutPanel5.Controls.Add(this.labelKPNPL, 1, 8);
            this.tableLayoutPanel5.Controls.Add(this.labelKPNPF, 1, 7);
            this.tableLayoutPanel5.Controls.Add(this.label23, 0, 7);
            this.tableLayoutPanel5.Controls.Add(this.labelKPNPA, 1, 6);
            this.tableLayoutPanel5.Controls.Add(this.label21, 0, 6);
            this.tableLayoutPanel5.Controls.Add(this.labelKPNPU, 1, 5);
            this.tableLayoutPanel5.Controls.Add(this.label11, 0, 5);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 9;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(189, 138);
            this.tableLayoutPanel5.TabIndex = 1;
            // 
            // label14
            // 
            this.label14.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(3, 46);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(67, 13);
            this.label14.TabIndex = 3;
            this.label14.Text = "Paged Frees";
            // 
            // label17
            // 
            this.label17.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(3, 1);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(67, 13);
            this.label17.TabIndex = 1;
            this.label17.Text = "Paged Phys.";
            // 
            // label18
            // 
            this.label18.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(3, 31);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(69, 13);
            this.label18.TabIndex = 1;
            this.label18.Text = "Paged Allocs";
            // 
            // labelKPPPU
            // 
            this.labelKPPPU.AutoEllipsis = true;
            this.labelKPPPU.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelKPPPU.Location = new System.Drawing.Point(104, 0);
            this.labelKPPPU.Name = "labelKPPPU";
            this.labelKPPPU.Size = new System.Drawing.Size(82, 15);
            this.labelKPPPU.TabIndex = 1;
            this.labelKPPPU.Text = "value";
            this.labelKPPPU.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelKPPA
            // 
            this.labelKPPA.AutoEllipsis = true;
            this.labelKPPA.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelKPPA.Location = new System.Drawing.Point(104, 30);
            this.labelKPPA.Name = "labelKPPA";
            this.labelKPPA.Size = new System.Drawing.Size(82, 15);
            this.labelKPPA.TabIndex = 1;
            this.labelKPPA.Text = "value";
            this.labelKPPA.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label12
            // 
            this.label12.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(3, 16);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(59, 13);
            this.label12.TabIndex = 1;
            this.label12.Text = "Paged Virt.";
            // 
            // labelKPPVU
            // 
            this.labelKPPVU.AutoEllipsis = true;
            this.labelKPPVU.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelKPPVU.Location = new System.Drawing.Point(104, 15);
            this.labelKPPVU.Name = "labelKPPVU";
            this.labelKPPVU.Size = new System.Drawing.Size(82, 15);
            this.labelKPPVU.TabIndex = 1;
            this.labelKPPVU.Text = "value";
            this.labelKPPVU.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelKPPF
            // 
            this.labelKPPF.AutoEllipsis = true;
            this.labelKPPF.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelKPPF.Location = new System.Drawing.Point(104, 45);
            this.labelKPPF.Name = "labelKPPF";
            this.labelKPPF.Size = new System.Drawing.Size(82, 15);
            this.labelKPPF.TabIndex = 2;
            this.labelKPPF.Text = "value";
            this.labelKPPF.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label29
            // 
            this.label29.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(3, 61);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(62, 13);
            this.label29.TabIndex = 3;
            this.label29.Text = "Paged Limit";
            // 
            // labelKPPL
            // 
            this.labelKPPL.AutoEllipsis = true;
            this.labelKPPL.AutoSize = true;
            this.labelKPPL.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelKPPL.Location = new System.Drawing.Point(104, 60);
            this.labelKPPL.Name = "labelKPPL";
            this.labelKPPL.Size = new System.Drawing.Size(82, 15);
            this.labelKPPL.TabIndex = 10;
            this.labelKPPL.Text = "value";
            this.labelKPPL.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label33
            // 
            this.label33.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label33.AutoSize = true;
            this.label33.Location = new System.Drawing.Point(3, 122);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(85, 13);
            this.label33.TabIndex = 9;
            this.label33.Text = "Non-Paged Limit";
            // 
            // labelKPNPL
            // 
            this.labelKPNPL.AutoEllipsis = true;
            this.labelKPNPL.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelKPNPL.Location = new System.Drawing.Point(104, 120);
            this.labelKPNPL.Name = "labelKPNPL";
            this.labelKPNPL.Size = new System.Drawing.Size(82, 18);
            this.labelKPNPL.TabIndex = 8;
            this.labelKPNPL.Text = "value";
            this.labelKPNPL.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelKPNPF
            // 
            this.labelKPNPF.AutoEllipsis = true;
            this.labelKPNPF.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelKPNPF.Location = new System.Drawing.Point(104, 105);
            this.labelKPNPF.Name = "labelKPNPF";
            this.labelKPNPF.Size = new System.Drawing.Size(82, 15);
            this.labelKPNPF.TabIndex = 8;
            this.labelKPNPF.Text = "value";
            this.labelKPNPF.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label23
            // 
            this.label23.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(3, 106);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(90, 13);
            this.label23.TabIndex = 9;
            this.label23.Text = "Non-Paged Frees";
            // 
            // labelKPNPA
            // 
            this.labelKPNPA.AutoEllipsis = true;
            this.labelKPNPA.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelKPNPA.Location = new System.Drawing.Point(104, 90);
            this.labelKPNPA.Name = "labelKPNPA";
            this.labelKPNPA.Size = new System.Drawing.Size(82, 15);
            this.labelKPNPA.TabIndex = 6;
            this.labelKPNPA.Text = "value";
            this.labelKPNPA.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label21
            // 
            this.label21.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(3, 91);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(92, 13);
            this.label21.TabIndex = 7;
            this.label21.Text = "Non-Paged Allocs";
            // 
            // labelKPNPU
            // 
            this.labelKPNPU.AutoEllipsis = true;
            this.labelKPNPU.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelKPNPU.Location = new System.Drawing.Point(104, 75);
            this.labelKPNPU.Name = "labelKPNPU";
            this.labelKPNPU.Size = new System.Drawing.Size(82, 15);
            this.labelKPNPU.TabIndex = 4;
            this.labelKPNPU.Text = "value";
            this.labelKPNPU.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label11
            // 
            this.label11.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(3, 76);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(95, 13);
            this.label11.TabIndex = 5;
            this.label11.Text = "Non-Paged Usage";
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.tableLayoutPanel6);
            this.groupBox8.Location = new System.Drawing.Point(405, 3);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(195, 121);
            this.groupBox8.TabIndex = 6;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Page Faults";
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.ColumnCount = 2;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel6.Controls.Add(this.label20, 0, 5);
            this.tableLayoutPanel6.Controls.Add(this.labelPFCache, 0, 5);
            this.tableLayoutPanel6.Controls.Add(this.label24, 0, 4);
            this.tableLayoutPanel6.Controls.Add(this.label25, 0, 3);
            this.tableLayoutPanel6.Controls.Add(this.labelPFDZ, 0, 4);
            this.tableLayoutPanel6.Controls.Add(this.label27, 0, 0);
            this.tableLayoutPanel6.Controls.Add(this.label28, 0, 2);
            this.tableLayoutPanel6.Controls.Add(this.labelPFTotal, 1, 0);
            this.tableLayoutPanel6.Controls.Add(this.labelPFTrans, 1, 2);
            this.tableLayoutPanel6.Controls.Add(this.label31, 0, 1);
            this.tableLayoutPanel6.Controls.Add(this.labelPFCOW, 1, 1);
            this.tableLayoutPanel6.Controls.Add(this.labelPFCacheTrans, 1, 3);
            this.tableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel6.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 6;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel6.Size = new System.Drawing.Size(189, 102);
            this.tableLayoutPanel6.TabIndex = 1;
            // 
            // label20
            // 
            this.label20.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(3, 84);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(38, 13);
            this.label20.TabIndex = 7;
            this.label20.Text = "Cache";
            // 
            // labelPFCache
            // 
            this.labelPFCache.AutoEllipsis = true;
            this.labelPFCache.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelPFCache.Location = new System.Drawing.Point(96, 80);
            this.labelPFCache.Name = "labelPFCache";
            this.labelPFCache.Size = new System.Drawing.Size(90, 22);
            this.labelPFCache.TabIndex = 6;
            this.labelPFCache.Text = "value";
            this.labelPFCache.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label24
            // 
            this.label24.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(3, 65);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(72, 13);
            this.label24.TabIndex = 5;
            this.label24.Text = "Demand Zero";
            // 
            // label25
            // 
            this.label25.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(3, 49);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(87, 13);
            this.label25.TabIndex = 3;
            this.label25.Text = "Cache Transition";
            // 
            // labelPFDZ
            // 
            this.labelPFDZ.AutoEllipsis = true;
            this.labelPFDZ.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelPFDZ.Location = new System.Drawing.Point(96, 64);
            this.labelPFDZ.Name = "labelPFDZ";
            this.labelPFDZ.Size = new System.Drawing.Size(90, 16);
            this.labelPFDZ.TabIndex = 4;
            this.labelPFDZ.Text = "value";
            this.labelPFDZ.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label27
            // 
            this.label27.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(3, 1);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(31, 13);
            this.label27.TabIndex = 1;
            this.label27.Text = "Total";
            // 
            // label28
            // 
            this.label28.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(3, 33);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(53, 13);
            this.label28.TabIndex = 1;
            this.label28.Text = "Transition";
            // 
            // labelPFTotal
            // 
            this.labelPFTotal.AutoEllipsis = true;
            this.labelPFTotal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelPFTotal.Location = new System.Drawing.Point(96, 0);
            this.labelPFTotal.Name = "labelPFTotal";
            this.labelPFTotal.Size = new System.Drawing.Size(90, 16);
            this.labelPFTotal.TabIndex = 1;
            this.labelPFTotal.Text = "value";
            this.labelPFTotal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelPFTrans
            // 
            this.labelPFTrans.AutoEllipsis = true;
            this.labelPFTrans.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelPFTrans.Location = new System.Drawing.Point(96, 32);
            this.labelPFTrans.Name = "labelPFTrans";
            this.labelPFTrans.Size = new System.Drawing.Size(90, 16);
            this.labelPFTrans.TabIndex = 1;
            this.labelPFTrans.Text = "value";
            this.labelPFTrans.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label31
            // 
            this.label31.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(3, 17);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(76, 13);
            this.label31.TabIndex = 1;
            this.label31.Text = "Copy-On-Write";
            // 
            // labelPFCOW
            // 
            this.labelPFCOW.AutoEllipsis = true;
            this.labelPFCOW.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelPFCOW.Location = new System.Drawing.Point(96, 16);
            this.labelPFCOW.Name = "labelPFCOW";
            this.labelPFCOW.Size = new System.Drawing.Size(90, 16);
            this.labelPFCOW.TabIndex = 1;
            this.labelPFCOW.Text = "value";
            this.labelPFCOW.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelPFCacheTrans
            // 
            this.labelPFCacheTrans.AutoEllipsis = true;
            this.labelPFCacheTrans.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelPFCacheTrans.Location = new System.Drawing.Point(96, 48);
            this.labelPFCacheTrans.Name = "labelPFCacheTrans";
            this.labelPFCacheTrans.Size = new System.Drawing.Size(90, 16);
            this.labelPFCacheTrans.TabIndex = 2;
            this.labelPFCacheTrans.Text = "value";
            this.labelPFCacheTrans.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.tableLayoutPanel7);
            this.groupBox9.Location = new System.Drawing.Point(405, 130);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(195, 121);
            this.groupBox9.TabIndex = 7;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "I/O";
            // 
            // tableLayoutPanel7
            // 
            this.tableLayoutPanel7.ColumnCount = 2;
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel7.Controls.Add(this.label16, 0, 5);
            this.tableLayoutPanel7.Controls.Add(this.labelIOOB, 0, 5);
            this.tableLayoutPanel7.Controls.Add(this.label22, 0, 4);
            this.tableLayoutPanel7.Controls.Add(this.label26, 0, 3);
            this.tableLayoutPanel7.Controls.Add(this.labelIOO, 0, 4);
            this.tableLayoutPanel7.Controls.Add(this.label30, 0, 0);
            this.tableLayoutPanel7.Controls.Add(this.label32, 0, 2);
            this.tableLayoutPanel7.Controls.Add(this.labelIOR, 1, 0);
            this.tableLayoutPanel7.Controls.Add(this.labelIOW, 1, 2);
            this.tableLayoutPanel7.Controls.Add(this.label35, 0, 1);
            this.tableLayoutPanel7.Controls.Add(this.labelIORB, 1, 1);
            this.tableLayoutPanel7.Controls.Add(this.labelIOWB, 1, 3);
            this.tableLayoutPanel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel7.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel7.Name = "tableLayoutPanel7";
            this.tableLayoutPanel7.RowCount = 6;
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel7.Size = new System.Drawing.Size(189, 102);
            this.tableLayoutPanel7.TabIndex = 1;
            // 
            // label16
            // 
            this.label16.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(3, 84);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(62, 13);
            this.label16.TabIndex = 7;
            this.label16.Text = "Other Bytes";
            // 
            // labelIOOB
            // 
            this.labelIOOB.AutoEllipsis = true;
            this.labelIOOB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelIOOB.Location = new System.Drawing.Point(71, 80);
            this.labelIOOB.Name = "labelIOOB";
            this.labelIOOB.Size = new System.Drawing.Size(115, 22);
            this.labelIOOB.TabIndex = 6;
            this.labelIOOB.Text = "value";
            this.labelIOOB.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label22
            // 
            this.label22.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(3, 65);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(33, 13);
            this.label22.TabIndex = 5;
            this.label22.Text = "Other";
            // 
            // label26
            // 
            this.label26.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(3, 49);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(61, 13);
            this.label26.TabIndex = 3;
            this.label26.Text = "Write Bytes";
            // 
            // labelIOO
            // 
            this.labelIOO.AutoEllipsis = true;
            this.labelIOO.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelIOO.Location = new System.Drawing.Point(71, 64);
            this.labelIOO.Name = "labelIOO";
            this.labelIOO.Size = new System.Drawing.Size(115, 16);
            this.labelIOO.TabIndex = 4;
            this.labelIOO.Text = "value";
            this.labelIOO.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label30
            // 
            this.label30.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(3, 1);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(38, 13);
            this.label30.TabIndex = 1;
            this.label30.Text = "Reads";
            // 
            // label32
            // 
            this.label32.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label32.AutoSize = true;
            this.label32.Location = new System.Drawing.Point(3, 33);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(37, 13);
            this.label32.TabIndex = 1;
            this.label32.Text = "Writes";
            // 
            // labelIOR
            // 
            this.labelIOR.AutoEllipsis = true;
            this.labelIOR.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelIOR.Location = new System.Drawing.Point(71, 0);
            this.labelIOR.Name = "labelIOR";
            this.labelIOR.Size = new System.Drawing.Size(115, 16);
            this.labelIOR.TabIndex = 1;
            this.labelIOR.Text = "value";
            this.labelIOR.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelIOW
            // 
            this.labelIOW.AutoEllipsis = true;
            this.labelIOW.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelIOW.Location = new System.Drawing.Point(71, 32);
            this.labelIOW.Name = "labelIOW";
            this.labelIOW.Size = new System.Drawing.Size(115, 16);
            this.labelIOW.TabIndex = 1;
            this.labelIOW.Text = "value";
            this.labelIOW.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label35
            // 
            this.label35.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label35.AutoSize = true;
            this.label35.Location = new System.Drawing.Point(3, 17);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(62, 13);
            this.label35.TabIndex = 1;
            this.label35.Text = "Read Bytes";
            // 
            // labelIORB
            // 
            this.labelIORB.AutoEllipsis = true;
            this.labelIORB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelIORB.Location = new System.Drawing.Point(71, 16);
            this.labelIORB.Name = "labelIORB";
            this.labelIORB.Size = new System.Drawing.Size(115, 16);
            this.labelIORB.TabIndex = 1;
            this.labelIORB.Text = "value";
            this.labelIORB.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelIOWB
            // 
            this.labelIOWB.AutoEllipsis = true;
            this.labelIOWB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelIOWB.Location = new System.Drawing.Point(71, 48);
            this.labelIOWB.Name = "labelIOWB";
            this.labelIOWB.Size = new System.Drawing.Size(115, 16);
            this.labelIOWB.TabIndex = 2;
            this.labelIOWB.Text = "value";
            this.labelIOWB.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // groupBox10
            // 
            this.groupBox10.AutoSize = true;
            this.groupBox10.Controls.Add(this.tableLayoutPanel8);
            this.groupBox10.Location = new System.Drawing.Point(606, 3);
            this.groupBox10.MinimumSize = new System.Drawing.Size(195, 76);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.Size = new System.Drawing.Size(195, 76);
            this.groupBox10.TabIndex = 8;
            this.groupBox10.TabStop = false;
            this.groupBox10.Text = "CPU";
            // 
            // tableLayoutPanel8
            // 
            this.tableLayoutPanel8.AutoSize = true;
            this.tableLayoutPanel8.ColumnCount = 2;
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel8.Controls.Add(this.label37, 0, 0);
            this.tableLayoutPanel8.Controls.Add(this.label38, 0, 2);
            this.tableLayoutPanel8.Controls.Add(this.labelCPUContextSwitches, 1, 0);
            this.tableLayoutPanel8.Controls.Add(this.labelCPUSystemCalls, 1, 2);
            this.tableLayoutPanel8.Controls.Add(this.label41, 0, 1);
            this.tableLayoutPanel8.Controls.Add(this.labelCPUInterrupts, 1, 1);
            this.tableLayoutPanel8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel8.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel8.Name = "tableLayoutPanel8";
            this.tableLayoutPanel8.RowCount = 3;
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel8.Size = new System.Drawing.Size(189, 57);
            this.tableLayoutPanel8.TabIndex = 1;
            // 
            // label37
            // 
            this.label37.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label37.AutoSize = true;
            this.label37.Location = new System.Drawing.Point(3, 3);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(89, 13);
            this.label37.TabIndex = 1;
            this.label37.Text = "Context Switches";
            // 
            // label38
            // 
            this.label38.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label38.AutoSize = true;
            this.label38.Location = new System.Drawing.Point(3, 41);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(66, 13);
            this.label38.TabIndex = 1;
            this.label38.Text = "System Calls";
            // 
            // labelCPUContextSwitches
            // 
            this.labelCPUContextSwitches.AutoEllipsis = true;
            this.labelCPUContextSwitches.AutoSize = true;
            this.labelCPUContextSwitches.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelCPUContextSwitches.Location = new System.Drawing.Point(98, 0);
            this.labelCPUContextSwitches.Name = "labelCPUContextSwitches";
            this.labelCPUContextSwitches.Size = new System.Drawing.Size(88, 19);
            this.labelCPUContextSwitches.TabIndex = 1;
            this.labelCPUContextSwitches.Text = "value";
            this.labelCPUContextSwitches.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelCPUSystemCalls
            // 
            this.labelCPUSystemCalls.AutoEllipsis = true;
            this.labelCPUSystemCalls.AutoSize = true;
            this.labelCPUSystemCalls.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelCPUSystemCalls.Location = new System.Drawing.Point(98, 38);
            this.labelCPUSystemCalls.Name = "labelCPUSystemCalls";
            this.labelCPUSystemCalls.Size = new System.Drawing.Size(88, 19);
            this.labelCPUSystemCalls.TabIndex = 1;
            this.labelCPUSystemCalls.Text = "value";
            this.labelCPUSystemCalls.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label41
            // 
            this.label41.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label41.AutoSize = true;
            this.label41.Location = new System.Drawing.Point(3, 22);
            this.label41.Name = "label41";
            this.label41.Size = new System.Drawing.Size(51, 13);
            this.label41.TabIndex = 1;
            this.label41.Text = "Interrupts";
            // 
            // labelCPUInterrupts
            // 
            this.labelCPUInterrupts.AutoEllipsis = true;
            this.labelCPUInterrupts.AutoSize = true;
            this.labelCPUInterrupts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelCPUInterrupts.Location = new System.Drawing.Point(98, 19);
            this.labelCPUInterrupts.Name = "labelCPUInterrupts";
            this.labelCPUInterrupts.Size = new System.Drawing.Size(88, 19);
            this.labelCPUInterrupts.TabIndex = 1;
            this.labelCPUInterrupts.Text = "value";
            this.labelCPUInterrupts.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // checkAlwaysOnTop
            // 
            this.checkAlwaysOnTop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.checkAlwaysOnTop.AutoSize = true;
            this.checkAlwaysOnTop.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkAlwaysOnTop.Location = new System.Drawing.Point(728, 508);
            this.checkAlwaysOnTop.Name = "checkAlwaysOnTop";
            this.checkAlwaysOnTop.Size = new System.Drawing.Size(102, 18);
            this.checkAlwaysOnTop.TabIndex = 5;
            this.checkAlwaysOnTop.Text = "Always on Top";
            this.checkAlwaysOnTop.UseVisualStyleBackColor = true;
            this.checkAlwaysOnTop.CheckedChanged += new System.EventHandler(this.checkAlwaysOnTop_CheckedChanged);
            // 
            // SysInfoWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(842, 538);
            this.Controls.Add(this.checkAlwaysOnTop);
            this.Controls.Add(this.flowInfo);
            this.Controls.Add(this.tableGraphs);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(200, 500);
            this.Name = "SysInfoWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "System Information";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.SysInfoWindow_Paint);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SysInfoWindow_FormClosing);
            this.gboxCPUPlotter.ResumeLayout(false);
            this.tableGraphs.ResumeLayout(false);
            this.tableGraphs.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox11.ResumeLayout(false);
            this.groupBox12.ResumeLayout(false);
            this.groupBox13.ResumeLayout(false);
            this.flowInfo.ResumeLayout(false);
            this.flowInfo.PerformLayout();
            this.groupBox3.ResumeLayout(false);
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
            this.groupBox7.ResumeLayout(false);
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            this.groupBox8.ResumeLayout(false);
            this.tableLayoutPanel6.ResumeLayout(false);
            this.tableLayoutPanel6.PerformLayout();
            this.groupBox9.ResumeLayout(false);
            this.tableLayoutPanel7.ResumeLayout(false);
            this.tableLayoutPanel7.PerformLayout();
            this.groupBox10.ResumeLayout(false);
            this.groupBox10.PerformLayout();
            this.tableLayoutPanel8.ResumeLayout(false);
            this.tableLayoutPanel8.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ProcessHacker.Components.Plotter plotterCPU;
        private System.Windows.Forms.GroupBox gboxCPUPlotter;
        private System.Windows.Forms.TableLayoutPanel tableCPUs;
        private System.Windows.Forms.TableLayoutPanel tableGraphs;
        private System.Windows.Forms.CheckBox checkShowOneGraphPerCPU;
        private System.Windows.Forms.GroupBox groupBox2;
        private ProcessHacker.Components.Plotter plotterIO;
        private System.Windows.Forms.GroupBox groupBox1;
        private ProcessHacker.Components.Plotter plotterMemory;
        private System.Windows.Forms.FlowLayoutPanel flowInfo;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label labelTotalsProcesses;
        private System.Windows.Forms.Label labelTotalsThreads;
        private System.Windows.Forms.Label labelTotalsHandles;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labelCCC;
        private System.Windows.Forms.Label labelCCP;
        private System.Windows.Forms.Label labelCCL;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label labelPMC;
        private System.Windows.Forms.Label labelPMT;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label labelCacheCurrent;
        private System.Windows.Forms.Label labelCachePeak;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label labelCacheMaximum;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label labelCacheMinimum;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label labelKPNPU;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label labelKPPF;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label labelKPPPU;
        private System.Windows.Forms.Label labelKPPA;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label labelKPNPF;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label labelKPNPA;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label labelKPPVU;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label labelPFCache;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label labelPFDZ;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.Label labelPFTotal;
        private System.Windows.Forms.Label labelPFTrans;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.Label labelPFCOW;
        private System.Windows.Forms.Label labelPFCacheTrans;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel7;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label labelIOOB;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label labelIOO;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.Label labelIOR;
        private System.Windows.Forms.Label labelIOW;
        private System.Windows.Forms.Label label35;
        private System.Windows.Forms.Label labelIORB;
        private System.Windows.Forms.Label labelIOWB;
        private System.Windows.Forms.GroupBox groupBox10;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel8;
        private System.Windows.Forms.Label label37;
        private System.Windows.Forms.Label label38;
        private System.Windows.Forms.Label labelCPUContextSwitches;
        private System.Windows.Forms.Label labelCPUSystemCalls;
        private System.Windows.Forms.Label label41;
        private System.Windows.Forms.Label labelCPUInterrupts;
        private System.Windows.Forms.CheckBox checkAlwaysOnTop;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label labelPSC;
        private ProcessHacker.Components.Indicator indicatorCpu;
        private ProcessHacker.Components.Indicator indicatorIO;
        private ProcessHacker.Components.Indicator indicatorPhysical;
        private System.Windows.Forms.GroupBox groupBox11;
        private System.Windows.Forms.GroupBox groupBox12;
        private System.Windows.Forms.GroupBox groupBox13;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Label labelKPPL;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.Label labelKPNPL;
        private System.Windows.Forms.Label labelTotalsUptime;
        private System.Windows.Forms.Label label34;
    }
}