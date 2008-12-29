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
            this.tableGraphs = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkShowOneGraphPerCPU = new System.Windows.Forms.CheckBox();
            this.plotterMemory = new ProcessHacker.Components.Plotter();
            this.plotterIO = new ProcessHacker.Components.Plotter();
            this.plotterCPU = new ProcessHacker.Components.Plotter();
            this.gboxCPUPlotter.SuspendLayout();
            this.tableGraphs.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // gboxCPUPlotter
            // 
            this.gboxCPUPlotter.Controls.Add(this.tableCPUs);
            this.gboxCPUPlotter.Controls.Add(this.plotterCPU);
            this.gboxCPUPlotter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gboxCPUPlotter.Location = new System.Drawing.Point(3, 3);
            this.gboxCPUPlotter.Name = "gboxCPUPlotter";
            this.gboxCPUPlotter.Size = new System.Drawing.Size(706, 109);
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
            this.tableCPUs.Size = new System.Drawing.Size(46, 19);
            this.tableCPUs.TabIndex = 3;
            this.tableCPUs.Visible = false;
            // 
            // tableGraphs
            // 
            this.tableGraphs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableGraphs.ColumnCount = 1;
            this.tableGraphs.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableGraphs.Controls.Add(this.groupBox1, 0, 3);
            this.tableGraphs.Controls.Add(this.groupBox2, 0, 2);
            this.tableGraphs.Controls.Add(this.gboxCPUPlotter, 0, 0);
            this.tableGraphs.Controls.Add(this.checkShowOneGraphPerCPU, 0, 1);
            this.tableGraphs.Location = new System.Drawing.Point(12, 12);
            this.tableGraphs.Name = "tableGraphs";
            this.tableGraphs.RowCount = 4;
            this.tableGraphs.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableGraphs.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableGraphs.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableGraphs.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableGraphs.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableGraphs.Size = new System.Drawing.Size(712, 370);
            this.tableGraphs.TabIndex = 3;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.plotterMemory);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(3, 257);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(706, 110);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Commit, Physical Memory";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.plotterIO);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(3, 142);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(706, 109);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "I/O (R+O, W)";
            // 
            // checkShowOneGraphPerCPU
            // 
            this.checkShowOneGraphPerCPU.AutoSize = true;
            this.checkShowOneGraphPerCPU.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkShowOneGraphPerCPU.Location = new System.Drawing.Point(3, 118);
            this.checkShowOneGraphPerCPU.Name = "checkShowOneGraphPerCPU";
            this.checkShowOneGraphPerCPU.Size = new System.Drawing.Size(153, 18);
            this.checkShowOneGraphPerCPU.TabIndex = 3;
            this.checkShowOneGraphPerCPU.Text = "Show one graph per CPU";
            this.checkShowOneGraphPerCPU.UseVisualStyleBackColor = true;
            this.checkShowOneGraphPerCPU.CheckedChanged += new System.EventHandler(this.checkShowOneGraphPerCPU_CheckedChanged);
            // 
            // plotterMemory
            // 
            this.plotterMemory.BackColor = System.Drawing.Color.Black;
            this.plotterMemory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plotterMemory.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(128)))), ((int)(((byte)(64)))));
            this.plotterMemory.GridSize = new System.Drawing.Size(12, 12);
            this.plotterMemory.IsMoved = true;
            this.plotterMemory.LineColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))));
            this.plotterMemory.LineColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.plotterMemory.Location = new System.Drawing.Point(3, 16);
            this.plotterMemory.MoveStep = 3;
            this.plotterMemory.Name = "plotterMemory";
            this.plotterMemory.Size = new System.Drawing.Size(700, 91);
            this.plotterMemory.TabIndex = 5;
            this.plotterMemory.TextBoxColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.plotterMemory.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))));
            this.plotterMemory.TextMargin = new System.Windows.Forms.Padding(3);
            this.plotterMemory.TextPadding = new System.Windows.Forms.Padding(3);
            this.plotterMemory.TextPosition = System.Drawing.ContentAlignment.TopLeft;
            this.plotterMemory.UseSecondLine = false;
            // 
            // plotterIO
            // 
            this.plotterIO.BackColor = System.Drawing.Color.Black;
            this.plotterIO.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plotterIO.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(128)))), ((int)(((byte)(64)))));
            this.plotterIO.GridSize = new System.Drawing.Size(12, 12);
            this.plotterIO.IsMoved = true;
            this.plotterIO.LineColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))));
            this.plotterIO.LineColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.plotterIO.Location = new System.Drawing.Point(3, 16);
            this.plotterIO.MoveStep = 3;
            this.plotterIO.Name = "plotterIO";
            this.plotterIO.Size = new System.Drawing.Size(700, 90);
            this.plotterIO.TabIndex = 5;
            this.plotterIO.TextBoxColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.plotterIO.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))));
            this.plotterIO.TextMargin = new System.Windows.Forms.Padding(3);
            this.plotterIO.TextPadding = new System.Windows.Forms.Padding(3);
            this.plotterIO.TextPosition = System.Drawing.ContentAlignment.TopLeft;
            this.plotterIO.UseSecondLine = false;
            // 
            // plotterCPU
            // 
            this.plotterCPU.BackColor = System.Drawing.Color.Black;
            this.plotterCPU.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plotterCPU.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(128)))), ((int)(((byte)(64)))));
            this.plotterCPU.GridSize = new System.Drawing.Size(12, 12);
            this.plotterCPU.IsMoved = true;
            this.plotterCPU.LineColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))));
            this.plotterCPU.LineColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.plotterCPU.Location = new System.Drawing.Point(3, 16);
            this.plotterCPU.MoveStep = 3;
            this.plotterCPU.Name = "plotterCPU";
            this.plotterCPU.Size = new System.Drawing.Size(700, 90);
            this.plotterCPU.TabIndex = 0;
            this.plotterCPU.TextBoxColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.plotterCPU.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))));
            this.plotterCPU.TextMargin = new System.Windows.Forms.Padding(3);
            this.plotterCPU.TextPadding = new System.Windows.Forms.Padding(3);
            this.plotterCPU.TextPosition = System.Drawing.ContentAlignment.TopLeft;
            this.plotterCPU.UseSecondLine = false;
            // 
            // SysInfoWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(736, 549);
            this.Controls.Add(this.tableGraphs);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SysInfoWindow";
            this.Text = "System Information";
            this.Load += new System.EventHandler(this.SysInfoWindow_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SysInfoWindow_FormClosing);
            this.gboxCPUPlotter.ResumeLayout(false);
            this.tableGraphs.ResumeLayout(false);
            this.tableGraphs.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

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
    }
}