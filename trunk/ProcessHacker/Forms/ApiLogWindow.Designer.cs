namespace ProcessHacker.Forms
{
    partial class ApiLogWindow
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
            this.listLog = new System.Windows.Forms.ListView();
            this.columnTime = new System.Windows.Forms.ColumnHeader();
            this.columnProcess = new System.Windows.Forms.ColumnHeader();
            this.columnFunction = new System.Windows.Forms.ColumnHeader();
            this.columnInformation = new System.Windows.Forms.ColumnHeader();
            this.timerUpdate = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // listLog
            // 
            this.listLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listLog.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnTime,
            this.columnProcess,
            this.columnFunction,
            this.columnInformation});
            this.listLog.FullRowSelect = true;
            this.listLog.HideSelection = false;
            this.listLog.Location = new System.Drawing.Point(12, 12);
            this.listLog.Name = "listLog";
            this.listLog.Size = new System.Drawing.Size(870, 578);
            this.listLog.TabIndex = 0;
            this.listLog.UseCompatibleStateImageBehavior = false;
            this.listLog.View = System.Windows.Forms.View.Details;
            this.listLog.VirtualMode = true;
            this.listLog.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.listLog_RetrieveVirtualItem);
            // 
            // columnTime
            // 
            this.columnTime.Text = "Time";
            this.columnTime.Width = 120;
            // 
            // columnProcess
            // 
            this.columnProcess.Text = "Process";
            this.columnProcess.Width = 100;
            // 
            // columnFunction
            // 
            this.columnFunction.Text = "Function";
            this.columnFunction.Width = 140;
            // 
            // columnInformation
            // 
            this.columnInformation.Text = "Information";
            this.columnInformation.Width = 500;
            // 
            // timerUpdate
            // 
            this.timerUpdate.Enabled = true;
            this.timerUpdate.Interval = 1000;
            this.timerUpdate.Tick += new System.EventHandler(this.timerUpdate_Tick);
            // 
            // ApiLogWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(894, 602);
            this.Controls.Add(this.listLog);
            this.Name = "ApiLogWindow";
            this.Text = "API Log";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ApiLogWindow_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listLog;
        private System.Windows.Forms.ColumnHeader columnTime;
        private System.Windows.Forms.ColumnHeader columnFunction;
        private System.Windows.Forms.ColumnHeader columnInformation;
        private System.Windows.Forms.ColumnHeader columnProcess;
        private System.Windows.Forms.Timer timerUpdate;
    }
}