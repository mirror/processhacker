namespace ProcessHacker.Components
{
    partial class ProcessList
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProcessList));
            this.listProcesses = new System.Windows.Forms.ListView();
            this.columnName = new System.Windows.Forms.ColumnHeader();
            this.columnPID = new System.Windows.Forms.ColumnHeader();
            this.columnPvtMemory = new System.Windows.Forms.ColumnHeader();
            this.columnUsername = new System.Windows.Forms.ColumnHeader();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // listProcesses
            // 
            this.listProcesses.AllowColumnReorder = true;
            this.listProcesses.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnName,
            this.columnPID,
            this.columnPvtMemory,
            this.columnUsername});
            this.listProcesses.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listProcesses.FullRowSelect = true;
            this.listProcesses.HideSelection = false;
            this.listProcesses.Location = new System.Drawing.Point(0, 0);
            this.listProcesses.Name = "listProcesses";
            this.listProcesses.ShowItemToolTips = true;
            this.listProcesses.Size = new System.Drawing.Size(450, 472);
            this.listProcesses.SmallImageList = this.imageList;
            this.listProcesses.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listProcesses.TabIndex = 1;
            this.listProcesses.UseCompatibleStateImageBehavior = false;
            this.listProcesses.View = System.Windows.Forms.View.Details;
            // 
            // columnName
            // 
            this.columnName.Text = "Name";
            this.columnName.Width = 120;
            // 
            // columnPID
            // 
            this.columnPID.Text = "PID";
            // 
            // columnPvtMemory
            // 
            this.columnPvtMemory.Text = "Pvt. Memory";
            this.columnPvtMemory.Width = 80;
            // 
            // columnUsername
            // 
            this.columnUsername.Text = "User";
            this.columnUsername.Width = 80;
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "Generic");
            // 
            // ProcessList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.listProcesses);
            this.DoubleBuffered = true;
            this.Name = "ProcessList";
            this.Size = new System.Drawing.Size(450, 472);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listProcesses;
        private System.Windows.Forms.ColumnHeader columnName;
        private System.Windows.Forms.ColumnHeader columnPID;
        private System.Windows.Forms.ColumnHeader columnPvtMemory;
        private System.Windows.Forms.ColumnHeader columnUsername;
        private System.Windows.Forms.ImageList imageList;
    }
}
