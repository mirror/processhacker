namespace ProcessHacker
{
    partial class CSRProcessesWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CSRProcessesWindow));
            this.listProcesses = new System.Windows.Forms.ListView();
            this.columnProcess = new System.Windows.Forms.ColumnHeader();
            this.columnPID = new System.Windows.Forms.ColumnHeader();
            this.buttonClose = new System.Windows.Forms.Button();
            this.buttonScan = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // listProcesses
            // 
            this.listProcesses.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listProcesses.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnProcess,
            this.columnPID});
            this.listProcesses.FullRowSelect = true;
            this.listProcesses.HideSelection = false;
            this.listProcesses.Location = new System.Drawing.Point(12, 42);
            this.listProcesses.Name = "listProcesses";
            this.listProcesses.ShowItemToolTips = true;
            this.listProcesses.Size = new System.Drawing.Size(487, 297);
            this.listProcesses.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listProcesses.TabIndex = 0;
            this.listProcesses.UseCompatibleStateImageBehavior = false;
            this.listProcesses.View = System.Windows.Forms.View.Details;
            // 
            // columnProcess
            // 
            this.columnProcess.Text = "Process";
            this.columnProcess.Width = 340;
            // 
            // columnPID
            // 
            this.columnPID.Text = "PID";
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonClose.Location = new System.Drawing.Point(424, 358);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 1;
            this.buttonClose.Text = "&Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // buttonScan
            // 
            this.buttonScan.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonScan.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonScan.Location = new System.Drawing.Point(343, 358);
            this.buttonScan.Name = "buttonScan";
            this.buttonScan.Size = new System.Drawing.Size(75, 23);
            this.buttonScan.TabIndex = 1;
            this.buttonScan.Text = "&Scan";
            this.buttonScan.UseVisualStyleBackColor = true;
            this.buttonScan.Click += new System.EventHandler(this.buttonScan_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoEllipsis = true;
            this.label1.Location = new System.Drawing.Point(12, 342);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(487, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "IMPORTANT: If you start any processes during the scan, Process Hacker may report " +
                "them as hidden!";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoEllipsis = true;
            this.label2.Location = new System.Drawing.Point(12, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(487, 30);
            this.label2.TabIndex = 3;
            this.label2.Text = "This tool displays the processes running on your computer by scanning the handles" +
                " of the Client Server Runtime processes. This can be used to identify hidden pro" +
                "cesses.";
            // 
            // CSRProcessesWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(511, 393);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonScan);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.listProcesses);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CSRProcessesWindow";
            this.Text = "CSR Processes";
            this.Load += new System.EventHandler(this.CSRProcessesWindow_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CSRProcessesWindow_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listProcesses;
        private System.Windows.Forms.ColumnHeader columnProcess;
        private System.Windows.Forms.ColumnHeader columnPID;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Button buttonScan;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}