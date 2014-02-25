namespace ProcessHacker
{
    partial class HiddenProcessesWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HiddenProcessesWindow));
            this.listProcesses = new System.Windows.Forms.ListView();
            this.columnProcess = new System.Windows.Forms.ColumnHeader();
            this.columnPID = new System.Windows.Forms.ColumnHeader();
            this.buttonClose = new System.Windows.Forms.Button();
            this.buttonScan = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonTerminate = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
            this.labelCount = new System.Windows.Forms.Label();
            this.comboMethod = new System.Windows.Forms.ComboBox();
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
            this.listProcesses.Location = new System.Drawing.Point(12, 44);
            this.listProcesses.Name = "listProcesses";
            this.listProcesses.ShowItemToolTips = true;
            this.listProcesses.Size = new System.Drawing.Size(487, 296);
            this.listProcesses.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listProcesses.TabIndex = 1;
            this.listProcesses.UseCompatibleStateImageBehavior = false;
            this.listProcesses.View = System.Windows.Forms.View.Details;
            this.listProcesses.SelectedIndexChanged += new System.EventHandler(this.listProcesses_SelectedIndexChanged);
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
            this.buttonClose.Location = new System.Drawing.Point(424, 361);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 7;
            this.buttonClose.Text = "&Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // buttonScan
            // 
            this.buttonScan.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonScan.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonScan.Location = new System.Drawing.Point(343, 361);
            this.buttonScan.Name = "buttonScan";
            this.buttonScan.Size = new System.Drawing.Size(75, 23);
            this.buttonScan.TabIndex = 6;
            this.buttonScan.Text = "&Scan";
            this.buttonScan.UseVisualStyleBackColor = true;
            this.buttonScan.Click += new System.EventHandler(this.buttonScan_Click);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoEllipsis = true;
            this.label2.Location = new System.Drawing.Point(12, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(487, 32);
            this.label2.TabIndex = 0;
            this.label2.Text = "Processes highlighted red are hidden while those highlighted gray have terminated" +
                " but are still being referenced by other processes.";
            // 
            // buttonTerminate
            // 
            this.buttonTerminate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonTerminate.Enabled = false;
            this.buttonTerminate.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonTerminate.Location = new System.Drawing.Point(181, 361);
            this.buttonTerminate.Name = "buttonTerminate";
            this.buttonTerminate.Size = new System.Drawing.Size(75, 23);
            this.buttonTerminate.TabIndex = 4;
            this.buttonTerminate.Text = "T&erminate";
            this.buttonTerminate.UseVisualStyleBackColor = true;
            this.buttonTerminate.Click += new System.EventHandler(this.buttonTerminate_Click);
            // 
            // buttonSave
            // 
            this.buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSave.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonSave.Location = new System.Drawing.Point(262, 361);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(75, 23);
            this.buttonSave.TabIndex = 5;
            this.buttonSave.Text = "Save...";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // labelCount
            // 
            this.labelCount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelCount.Location = new System.Drawing.Point(12, 343);
            this.labelCount.Name = "labelCount";
            this.labelCount.Size = new System.Drawing.Size(487, 15);
            this.labelCount.TabIndex = 2;
            this.labelCount.Text = "Count";
            this.labelCount.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // comboMethod
            // 
            this.comboMethod.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.comboMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboMethod.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.comboMethod.FormattingEnabled = true;
            this.comboMethod.Items.AddRange(new object[] {
            "Brute Force",
            "CSR Handles"});
            this.comboMethod.Location = new System.Drawing.Point(12, 363);
            this.comboMethod.Name = "comboMethod";
            this.comboMethod.Size = new System.Drawing.Size(121, 21);
            this.comboMethod.TabIndex = 3;
            // 
            // HiddenProcessesWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(511, 396);
            this.Controls.Add(this.comboMethod);
            this.Controls.Add(this.labelCount);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.buttonTerminate);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonScan);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.listProcesses);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "HiddenProcessesWindow";
            this.Text = "Hidden Processes";
            this.Load += new System.EventHandler(this.HiddenProcessesWindow_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.HiddenProcessesWindow_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listProcesses;
        private System.Windows.Forms.ColumnHeader columnProcess;
        private System.Windows.Forms.ColumnHeader columnPID;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Button buttonScan;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonTerminate;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Label labelCount;
        private System.Windows.Forms.ComboBox comboMethod;
    }
}