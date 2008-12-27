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
            this.components = new System.ComponentModel.Container();
            this.gboxCPUPlotter = new System.Windows.Forms.GroupBox();
            this.plotterCPU = new ProcessHacker.Components.Plotter();
            this.tmrSysInfo = new System.Windows.Forms.Timer(this.components);
            this.gboxCPUPlotter.SuspendLayout();
            this.SuspendLayout();
            // 
            // gboxCPUPlotter
            // 
            this.gboxCPUPlotter.Controls.Add(this.plotterCPU);
            this.gboxCPUPlotter.Location = new System.Drawing.Point(170, 12);
            this.gboxCPUPlotter.Name = "gboxCPUPlotter";
            this.gboxCPUPlotter.Size = new System.Drawing.Size(395, 181);
            this.gboxCPUPlotter.TabIndex = 2;
            this.gboxCPUPlotter.TabStop = false;
            this.gboxCPUPlotter.Text = "CPU Usage History";
            // 
            // plotterCPU
            // 
            this.plotterCPU.BackColor = System.Drawing.Color.Black;
            this.plotterCPU.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(128)))), ((int)(((byte)(64)))));
            this.plotterCPU.GridSize = new System.Drawing.Size(12, 12);
            this.plotterCPU.IsMoved = true;
            this.plotterCPU.LineColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))));
            this.plotterCPU.Location = new System.Drawing.Point(6, 21);
            this.plotterCPU.MoveStep = 3;
            this.plotterCPU.Name = "plotterCPU";
            this.plotterCPU.Size = new System.Drawing.Size(382, 154);
            this.plotterCPU.TabIndex = 0;
            // 
            // tmrSysInfo
            // 
            this.tmrSysInfo.Enabled = true;
            this.tmrSysInfo.Interval = 1000;
            this.tmrSysInfo.Tick += new System.EventHandler(this.tmrSysInfo_Tick);
            // 
            // SysInfoWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(590, 528);
            this.Controls.Add(this.gboxCPUPlotter);
            this.Name = "SysInfoWindow";
            this.Text = "SysInfoWindow";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SysInfoWindow_FormClosing);
            this.gboxCPUPlotter.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ProcessHacker.Components.Plotter plotterCPU;
        private System.Windows.Forms.GroupBox gboxCPUPlotter;
        private System.Windows.Forms.Timer tmrSysInfo;
    }
}