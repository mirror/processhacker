namespace ProcessHacker
{
    partial class MiniSysInfo
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
            this.plotterCPU = new ProcessHacker.Components.Plotter();
            this.plotterIO = new ProcessHacker.Components.Plotter();
            this.SuspendLayout();
            // 
            // plotterCPU
            // 
            this.plotterCPU.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.plotterCPU.Data1 = null;
            this.plotterCPU.Data2 = null;
            this.plotterCPU.GridColor = System.Drawing.Color.Green;
            this.plotterCPU.GridSize = new System.Drawing.Size(12, 12);
            this.plotterCPU.LineColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))));
            this.plotterCPU.LineColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.plotterCPU.Location = new System.Drawing.Point(0, 0);
            this.plotterCPU.LongData1 = null;
            this.plotterCPU.LongData2 = null;
            this.plotterCPU.MinMaxValue = ((long)(0));
            this.plotterCPU.MoveStep = 3;
            this.plotterCPU.Name = "plotterCPU";
            this.plotterCPU.OverlaySecondLine = false;
            this.plotterCPU.ShowGrid = true;
            this.plotterCPU.Size = new System.Drawing.Size(238, 50);
            this.plotterCPU.TabIndex = 0;
            this.plotterCPU.TextBoxColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.plotterCPU.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))));
            this.plotterCPU.TextMargin = new System.Windows.Forms.Padding(3);
            this.plotterCPU.TextPadding = new System.Windows.Forms.Padding(3);
            this.plotterCPU.TextPosition = System.Drawing.ContentAlignment.TopLeft;
            this.plotterCPU.UseLongData = false;
            this.plotterCPU.UseSecondLine = true;
            // 
            // plotterIO
            // 
            this.plotterIO.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.plotterIO.Data1 = null;
            this.plotterIO.Data2 = null;
            this.plotterIO.GridColor = System.Drawing.Color.Green;
            this.plotterIO.GridSize = new System.Drawing.Size(12, 12);
            this.plotterIO.LineColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))));
            this.plotterIO.LineColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.plotterIO.Location = new System.Drawing.Point(0, 56);
            this.plotterIO.LongData1 = null;
            this.plotterIO.LongData2 = null;
            this.plotterIO.MinMaxValue = ((long)(0));
            this.plotterIO.MoveStep = 3;
            this.plotterIO.Name = "plotterIO";
            this.plotterIO.OverlaySecondLine = false;
            this.plotterIO.ShowGrid = true;
            this.plotterIO.Size = new System.Drawing.Size(238, 50);
            this.plotterIO.TabIndex = 1;
            this.plotterIO.TextBoxColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.plotterIO.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))));
            this.plotterIO.TextMargin = new System.Windows.Forms.Padding(3);
            this.plotterIO.TextPadding = new System.Windows.Forms.Padding(3);
            this.plotterIO.TextPosition = System.Drawing.ContentAlignment.TopLeft;
            this.plotterIO.UseLongData = true;
            this.plotterIO.UseSecondLine = true;
            // 
            // MiniSysInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(238, 192);
            this.Controls.Add(this.plotterIO);
            this.Controls.Add(this.plotterCPU);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MiniSysInfo";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Deactivate += new System.EventHandler(this.MiniSysInfo_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MiniSysInfo_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private ProcessHacker.Components.Plotter plotterCPU;
        private ProcessHacker.Components.Plotter plotterIO;
    }
}