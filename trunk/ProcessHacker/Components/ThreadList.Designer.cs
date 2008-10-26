namespace ProcessHacker
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
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.listThreads = new System.Windows.Forms.ListView();
            this.columnThreadID = new System.Windows.Forms.ColumnHeader();
            this.columnThreadState = new System.Windows.Forms.ColumnHeader();
            this.columnCPUTime = new System.Windows.Forms.ColumnHeader();
            this.columnPriority = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // listThreads
            // 
            this.listThreads.AllowColumnReorder = true;
            this.listThreads.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnThreadID,
            this.columnThreadState,
            this.columnCPUTime,
            this.columnPriority});
            this.listThreads.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listThreads.FullRowSelect = true;
            this.listThreads.HideSelection = false;
            this.listThreads.Location = new System.Drawing.Point(0, 0);
            this.listThreads.Name = "listThreads";
            this.listThreads.Size = new System.Drawing.Size(450, 472);
            this.listThreads.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listThreads.TabIndex = 3;
            this.listThreads.UseCompatibleStateImageBehavior = false;
            this.listThreads.View = System.Windows.Forms.View.Details;
            // 
            // columnThreadID
            // 
            this.columnThreadID.Text = "TID";
            // 
            // columnThreadState
            // 
            this.columnThreadState.Text = "State";
            this.columnThreadState.Width = 100;
            // 
            // columnCPUTime
            // 
            this.columnCPUTime.Text = "CPU Time";
            this.columnCPUTime.Width = 70;
            // 
            // columnPriority
            // 
            this.columnPriority.Text = "Priority";
            this.columnPriority.Width = 80;
            // 
            // ThreadList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.listThreads);
            this.DoubleBuffered = true;
            this.Name = "ThreadList";
            this.Size = new System.Drawing.Size(450, 472);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listThreads;
        private System.Windows.Forms.ColumnHeader columnThreadID;
        private System.Windows.Forms.ColumnHeader columnThreadState;
        private System.Windows.Forms.ColumnHeader columnCPUTime;
        private System.Windows.Forms.ColumnHeader columnPriority;
    }
}
