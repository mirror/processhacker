namespace ProcessHacker.Components
{
    partial class ProcessStatistics
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
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
            this.labelCPUCyclesText = new System.Windows.Forms.Label();
            this.labelCPUCycles = new System.Windows.Forms.Label();
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
            this.label30 = new System.Windows.Forms.Label();
            this.labelMemoryPP = new System.Windows.Forms.Label();
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
            this.label31 = new System.Windows.Forms.Label();
            this.labelIOPriority = new System.Windows.Forms.Label();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.label27 = new System.Windows.Forms.Label();
            this.labelOtherHandles = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.labelOtherGDIHandles = new System.Windows.Forms.Label();
            this.labelOtherUSERHandles = new System.Windows.Forms.Label();
            this.buttonHandleDetails = new System.Windows.Forms.Button();
            this.flowStats.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowStats
            // 
            this.flowStats.Controls.Add(this.groupBox1);
            this.flowStats.Controls.Add(this.groupBox4);
            this.flowStats.Controls.Add(this.groupBox5);
            this.flowStats.Controls.Add(this.groupBox6);
            this.flowStats.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowStats.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowStats.Location = new System.Drawing.Point(0, 0);
            this.flowStats.Name = "flowStats";
            this.flowStats.Size = new System.Drawing.Size(433, 374);
            this.flowStats.TabIndex = 1;
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
            this.tableLayoutPanel1.Controls.Add(this.label8, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label9, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.label10, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.labelCPUPriority, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelCPUKernelTime, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.labelCPUUserTime, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.labelCPUTotalTime, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.labelCPUCyclesText, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelCPUCycles, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(189, 89);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 2);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(38, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Priority";
            // 
            // label8
            // 
            this.label8.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 36);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(63, 13);
            this.label8.TabIndex = 1;
            this.label8.Text = "Kernel Time";
            // 
            // label9
            // 
            this.label9.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(3, 53);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(55, 13);
            this.label9.TabIndex = 1;
            this.label9.Text = "User Time";
            // 
            // label10
            // 
            this.label10.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(3, 72);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(57, 13);
            this.label10.TabIndex = 1;
            this.label10.Text = "Total Time";
            // 
            // labelCPUPriority
            // 
            this.labelCPUPriority.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelCPUPriority.AutoSize = true;
            this.labelCPUPriority.Location = new System.Drawing.Point(153, 2);
            this.labelCPUPriority.Name = "labelCPUPriority";
            this.labelCPUPriority.Size = new System.Drawing.Size(33, 13);
            this.labelCPUPriority.TabIndex = 1;
            this.labelCPUPriority.Text = "value";
            // 
            // labelCPUKernelTime
            // 
            this.labelCPUKernelTime.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelCPUKernelTime.AutoSize = true;
            this.labelCPUKernelTime.Location = new System.Drawing.Point(153, 36);
            this.labelCPUKernelTime.Name = "labelCPUKernelTime";
            this.labelCPUKernelTime.Size = new System.Drawing.Size(33, 13);
            this.labelCPUKernelTime.TabIndex = 1;
            this.labelCPUKernelTime.Text = "value";
            // 
            // labelCPUUserTime
            // 
            this.labelCPUUserTime.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelCPUUserTime.AutoSize = true;
            this.labelCPUUserTime.Location = new System.Drawing.Point(153, 53);
            this.labelCPUUserTime.Name = "labelCPUUserTime";
            this.labelCPUUserTime.Size = new System.Drawing.Size(33, 13);
            this.labelCPUUserTime.TabIndex = 1;
            this.labelCPUUserTime.Text = "value";
            // 
            // labelCPUTotalTime
            // 
            this.labelCPUTotalTime.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelCPUTotalTime.AutoSize = true;
            this.labelCPUTotalTime.Location = new System.Drawing.Point(153, 72);
            this.labelCPUTotalTime.Name = "labelCPUTotalTime";
            this.labelCPUTotalTime.Size = new System.Drawing.Size(33, 13);
            this.labelCPUTotalTime.TabIndex = 1;
            this.labelCPUTotalTime.Text = "value";
            // 
            // labelCPUCyclesText
            // 
            this.labelCPUCyclesText.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelCPUCyclesText.AutoSize = true;
            this.labelCPUCyclesText.Location = new System.Drawing.Point(3, 19);
            this.labelCPUCyclesText.Name = "labelCPUCyclesText";
            this.labelCPUCyclesText.Size = new System.Drawing.Size(38, 13);
            this.labelCPUCyclesText.TabIndex = 1;
            this.labelCPUCyclesText.Text = "Cycles";
            // 
            // labelCPUCycles
            // 
            this.labelCPUCycles.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelCPUCycles.AutoSize = true;
            this.labelCPUCycles.Location = new System.Drawing.Point(153, 19);
            this.labelCPUCycles.Name = "labelCPUCycles";
            this.labelCPUCycles.Size = new System.Drawing.Size(33, 13);
            this.labelCPUCycles.TabIndex = 1;
            this.labelCPUCycles.Text = "value";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.tableLayoutPanel2);
            this.groupBox4.Location = new System.Drawing.Point(3, 117);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(195, 194);
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
            this.tableLayoutPanel2.Controls.Add(this.label30, 0, 8);
            this.tableLayoutPanel2.Controls.Add(this.labelMemoryPP, 1, 8);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 9;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(189, 175);
            this.tableLayoutPanel2.TabIndex = 1;
            // 
            // label24
            // 
            this.label24.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(3, 117);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(107, 13);
            this.label24.TabIndex = 7;
            this.label24.Text = "Peak Pagefile Usage";
            // 
            // label22
            // 
            this.label22.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(3, 98);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(79, 13);
            this.label22.TabIndex = 5;
            this.label22.Text = "Pagefile Usage";
            // 
            // label20
            // 
            this.label20.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(3, 79);
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
            this.label12.Location = new System.Drawing.Point(3, 22);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(66, 13);
            this.label12.TabIndex = 1;
            this.label12.Text = "Working Set";
            // 
            // label13
            // 
            this.label13.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(3, 41);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(94, 13);
            this.label13.TabIndex = 1;
            this.label13.Text = "Peak Working Set";
            // 
            // label14
            // 
            this.label14.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(3, 60);
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
            this.labelMemoryWS.Location = new System.Drawing.Point(153, 22);
            this.labelMemoryWS.Name = "labelMemoryWS";
            this.labelMemoryWS.Size = new System.Drawing.Size(33, 13);
            this.labelMemoryWS.TabIndex = 1;
            this.labelMemoryWS.Text = "value";
            // 
            // labelMemoryPWS
            // 
            this.labelMemoryPWS.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelMemoryPWS.AutoSize = true;
            this.labelMemoryPWS.Location = new System.Drawing.Point(153, 41);
            this.labelMemoryPWS.Name = "labelMemoryPWS";
            this.labelMemoryPWS.Size = new System.Drawing.Size(33, 13);
            this.labelMemoryPWS.TabIndex = 1;
            this.labelMemoryPWS.Text = "value";
            // 
            // labelMemoryVS
            // 
            this.labelMemoryVS.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelMemoryVS.AutoSize = true;
            this.labelMemoryVS.Location = new System.Drawing.Point(153, 60);
            this.labelMemoryVS.Name = "labelMemoryVS";
            this.labelMemoryVS.Size = new System.Drawing.Size(33, 13);
            this.labelMemoryVS.TabIndex = 1;
            this.labelMemoryVS.Text = "value";
            // 
            // labelMemoryPVS
            // 
            this.labelMemoryPVS.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelMemoryPVS.AutoSize = true;
            this.labelMemoryPVS.Location = new System.Drawing.Point(153, 79);
            this.labelMemoryPVS.Name = "labelMemoryPVS";
            this.labelMemoryPVS.Size = new System.Drawing.Size(33, 13);
            this.labelMemoryPVS.TabIndex = 1;
            this.labelMemoryPVS.Text = "value";
            // 
            // labelMemoryPU
            // 
            this.labelMemoryPU.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelMemoryPU.AutoSize = true;
            this.labelMemoryPU.Location = new System.Drawing.Point(153, 98);
            this.labelMemoryPU.Name = "labelMemoryPU";
            this.labelMemoryPU.Size = new System.Drawing.Size(33, 13);
            this.labelMemoryPU.TabIndex = 1;
            this.labelMemoryPU.Text = "value";
            // 
            // labelMemoryPPU
            // 
            this.labelMemoryPPU.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelMemoryPPU.AutoSize = true;
            this.labelMemoryPPU.Location = new System.Drawing.Point(153, 117);
            this.labelMemoryPPU.Name = "labelMemoryPPU";
            this.labelMemoryPPU.Size = new System.Drawing.Size(33, 13);
            this.labelMemoryPPU.TabIndex = 1;
            this.labelMemoryPPU.Text = "value";
            // 
            // label25
            // 
            this.label25.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(3, 136);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(63, 13);
            this.label25.TabIndex = 7;
            this.label25.Text = "Page Faults";
            // 
            // labelMemoryPF
            // 
            this.labelMemoryPF.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelMemoryPF.AutoSize = true;
            this.labelMemoryPF.Location = new System.Drawing.Point(153, 136);
            this.labelMemoryPF.Name = "labelMemoryPF";
            this.labelMemoryPF.Size = new System.Drawing.Size(33, 13);
            this.labelMemoryPF.TabIndex = 1;
            this.labelMemoryPF.Text = "value";
            // 
            // label30
            // 
            this.label30.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(3, 157);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(66, 13);
            this.label30.TabIndex = 7;
            this.label30.Text = "Page Priority";
            // 
            // labelMemoryPP
            // 
            this.labelMemoryPP.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelMemoryPP.AutoSize = true;
            this.labelMemoryPP.Location = new System.Drawing.Point(153, 157);
            this.labelMemoryPP.Name = "labelMemoryPP";
            this.labelMemoryPP.Size = new System.Drawing.Size(33, 13);
            this.labelMemoryPP.TabIndex = 1;
            this.labelMemoryPP.Text = "value";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.tableLayoutPanel3);
            this.groupBox5.Location = new System.Drawing.Point(204, 3);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(195, 148);
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
            this.tableLayoutPanel3.Controls.Add(this.label31, 0, 6);
            this.tableLayoutPanel3.Controls.Add(this.labelIOPriority, 1, 6);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 7;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(189, 129);
            this.tableLayoutPanel3.TabIndex = 1;
            // 
            // label16
            // 
            this.label16.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(3, 92);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(62, 13);
            this.label16.TabIndex = 5;
            this.label16.Text = "Other Bytes";
            // 
            // label17
            // 
            this.label17.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(3, 74);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(33, 13);
            this.label17.TabIndex = 3;
            this.label17.Text = "Other";
            // 
            // label18
            // 
            this.label18.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(3, 2);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(38, 13);
            this.label18.TabIndex = 1;
            this.label18.Text = "Reads";
            // 
            // label19
            // 
            this.label19.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(3, 20);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(62, 13);
            this.label19.TabIndex = 1;
            this.label19.Text = "Read Bytes";
            // 
            // label21
            // 
            this.label21.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(3, 38);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(37, 13);
            this.label21.TabIndex = 1;
            this.label21.Text = "Writes";
            // 
            // label23
            // 
            this.label23.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(3, 56);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(61, 13);
            this.label23.TabIndex = 1;
            this.label23.Text = "Write Bytes";
            // 
            // labelIOReads
            // 
            this.labelIOReads.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelIOReads.AutoSize = true;
            this.labelIOReads.Location = new System.Drawing.Point(153, 2);
            this.labelIOReads.Name = "labelIOReads";
            this.labelIOReads.Size = new System.Drawing.Size(33, 13);
            this.labelIOReads.TabIndex = 1;
            this.labelIOReads.Text = "value";
            // 
            // labelIOReadBytes
            // 
            this.labelIOReadBytes.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelIOReadBytes.AutoSize = true;
            this.labelIOReadBytes.Location = new System.Drawing.Point(153, 20);
            this.labelIOReadBytes.Name = "labelIOReadBytes";
            this.labelIOReadBytes.Size = new System.Drawing.Size(33, 13);
            this.labelIOReadBytes.TabIndex = 1;
            this.labelIOReadBytes.Text = "value";
            // 
            // labelIOWrites
            // 
            this.labelIOWrites.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelIOWrites.AutoSize = true;
            this.labelIOWrites.Location = new System.Drawing.Point(153, 38);
            this.labelIOWrites.Name = "labelIOWrites";
            this.labelIOWrites.Size = new System.Drawing.Size(33, 13);
            this.labelIOWrites.TabIndex = 1;
            this.labelIOWrites.Text = "value";
            // 
            // labelIOWriteBytes
            // 
            this.labelIOWriteBytes.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelIOWriteBytes.AutoSize = true;
            this.labelIOWriteBytes.Location = new System.Drawing.Point(153, 56);
            this.labelIOWriteBytes.Name = "labelIOWriteBytes";
            this.labelIOWriteBytes.Size = new System.Drawing.Size(33, 13);
            this.labelIOWriteBytes.TabIndex = 1;
            this.labelIOWriteBytes.Text = "value";
            // 
            // labelIOOther
            // 
            this.labelIOOther.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelIOOther.AutoSize = true;
            this.labelIOOther.Location = new System.Drawing.Point(153, 74);
            this.labelIOOther.Name = "labelIOOther";
            this.labelIOOther.Size = new System.Drawing.Size(33, 13);
            this.labelIOOther.TabIndex = 1;
            this.labelIOOther.Text = "value";
            // 
            // labelIOOtherBytes
            // 
            this.labelIOOtherBytes.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelIOOtherBytes.AutoSize = true;
            this.labelIOOtherBytes.Location = new System.Drawing.Point(153, 92);
            this.labelIOOtherBytes.Name = "labelIOOtherBytes";
            this.labelIOOtherBytes.Size = new System.Drawing.Size(33, 13);
            this.labelIOOtherBytes.TabIndex = 1;
            this.labelIOOtherBytes.Text = "value";
            // 
            // label31
            // 
            this.label31.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(3, 112);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(57, 13);
            this.label31.TabIndex = 5;
            this.label31.Text = "I/O Priority";
            // 
            // labelIOPriority
            // 
            this.labelIOPriority.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelIOPriority.AutoSize = true;
            this.labelIOPriority.Location = new System.Drawing.Point(153, 112);
            this.labelIOPriority.Name = "labelIOPriority";
            this.labelIOPriority.Size = new System.Drawing.Size(33, 13);
            this.labelIOPriority.TabIndex = 1;
            this.labelIOPriority.Text = "value";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.tableLayoutPanel4);
            this.groupBox6.Location = new System.Drawing.Point(204, 157);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(195, 99);
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
            this.tableLayoutPanel4.Controls.Add(this.buttonHandleDetails, 1, 3);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 4;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(189, 80);
            this.tableLayoutPanel4.TabIndex = 1;
            // 
            // label27
            // 
            this.label27.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(3, 2);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(46, 13);
            this.label27.TabIndex = 1;
            this.label27.Text = "Handles";
            // 
            // labelOtherHandles
            // 
            this.labelOtherHandles.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelOtherHandles.AutoSize = true;
            this.labelOtherHandles.Location = new System.Drawing.Point(153, 2);
            this.labelOtherHandles.Name = "labelOtherHandles";
            this.labelOtherHandles.Size = new System.Drawing.Size(33, 13);
            this.labelOtherHandles.TabIndex = 1;
            this.labelOtherHandles.Text = "value";
            // 
            // label28
            // 
            this.label28.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(3, 19);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(68, 13);
            this.label28.TabIndex = 1;
            this.label28.Text = "GDI Handles";
            // 
            // label29
            // 
            this.label29.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(3, 36);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(79, 13);
            this.label29.TabIndex = 1;
            this.label29.Text = "USER Handles";
            // 
            // labelOtherGDIHandles
            // 
            this.labelOtherGDIHandles.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelOtherGDIHandles.AutoSize = true;
            this.labelOtherGDIHandles.Location = new System.Drawing.Point(153, 19);
            this.labelOtherGDIHandles.Name = "labelOtherGDIHandles";
            this.labelOtherGDIHandles.Size = new System.Drawing.Size(33, 13);
            this.labelOtherGDIHandles.TabIndex = 1;
            this.labelOtherGDIHandles.Text = "value";
            // 
            // labelOtherUSERHandles
            // 
            this.labelOtherUSERHandles.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelOtherUSERHandles.AutoSize = true;
            this.labelOtherUSERHandles.Location = new System.Drawing.Point(153, 36);
            this.labelOtherUSERHandles.Name = "labelOtherUSERHandles";
            this.labelOtherUSERHandles.Size = new System.Drawing.Size(33, 13);
            this.labelOtherUSERHandles.TabIndex = 1;
            this.labelOtherUSERHandles.Text = "value";
            // 
            // buttonHandleDetails
            // 
            this.buttonHandleDetails.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.buttonHandleDetails.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonHandleDetails.Location = new System.Drawing.Point(112, 54);
            this.buttonHandleDetails.Name = "buttonHandleDetails";
            this.buttonHandleDetails.Size = new System.Drawing.Size(74, 23);
            this.buttonHandleDetails.TabIndex = 2;
            this.buttonHandleDetails.Text = "Details...";
            this.buttonHandleDetails.UseVisualStyleBackColor = true;
            this.buttonHandleDetails.Click += new System.EventHandler(this.buttonHandleDetails_Click);
            // 
            // ProcessStatistics
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.flowStats);
            this.Name = "ProcessStatistics";
            this.Size = new System.Drawing.Size(433, 374);
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
            this.ResumeLayout(false);

        }

        #endregion

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
        private System.Windows.Forms.Label labelCPUCyclesText;
        private System.Windows.Forms.Label labelCPUCycles;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label labelMemoryPB;
        private System.Windows.Forms.Label labelMemoryWS;
        private System.Windows.Forms.Label labelMemoryPWS;
        private System.Windows.Forms.Label labelMemoryVS;
        private System.Windows.Forms.Label labelMemoryPVS;
        private System.Windows.Forms.Label labelMemoryPU;
        private System.Windows.Forms.Label labelMemoryPPU;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label labelMemoryPF;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.Label labelMemoryPP;
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
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.Label labelIOPriority;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label labelOtherHandles;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Label labelOtherGDIHandles;
        private System.Windows.Forms.Label labelOtherUSERHandles;
        private System.Windows.Forms.Button buttonHandleDetails;
    }
}
