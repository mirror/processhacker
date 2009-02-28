namespace ProcessHacker
{
    partial class ProcessTree
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
            this.treeProcesses = new Aga.Controls.Tree.TreeViewAdv();
            this.columnName = new Aga.Controls.Tree.TreeColumn();
            this.columnPID = new Aga.Controls.Tree.TreeColumn();
            this.columnPvtMemory = new Aga.Controls.Tree.TreeColumn();
            this.columnWorkingSet = new Aga.Controls.Tree.TreeColumn();
            this.columnCPU = new Aga.Controls.Tree.TreeColumn();
            this.columnUsername = new Aga.Controls.Tree.TreeColumn();
            this.columnDescription = new Aga.Controls.Tree.TreeColumn();
            this.columnCompany = new Aga.Controls.Tree.TreeColumn();
            this.nodeIcon = new Aga.Controls.Tree.NodeControls.NodeIcon();
            this.nodeName = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodePID = new Aga.Controls.Tree.NodeControls.NodeIntegerTextBox();
            this.nodePvtMemory = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeWorkingSet = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeCPU = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeUsername = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeDescription = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeCompany = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.columnFileName = new Aga.Controls.Tree.TreeColumn();
            this.columnCommandLine = new Aga.Controls.Tree.TreeColumn();
            this.nodeFileName = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeCommandLine = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.SuspendLayout();
            // 
            // treeProcesses
            // 
            this.treeProcesses.AllowColumnReorder = true;
            this.treeProcesses.BackColor = System.Drawing.SystemColors.Window;
            this.treeProcesses.Columns.Add(this.columnName);
            this.treeProcesses.Columns.Add(this.columnPID);
            this.treeProcesses.Columns.Add(this.columnPvtMemory);
            this.treeProcesses.Columns.Add(this.columnWorkingSet);
            this.treeProcesses.Columns.Add(this.columnCPU);
            this.treeProcesses.Columns.Add(this.columnUsername);
            this.treeProcesses.Columns.Add(this.columnDescription);
            this.treeProcesses.Columns.Add(this.columnCompany);
            this.treeProcesses.Columns.Add(this.columnFileName);
            this.treeProcesses.Columns.Add(this.columnCommandLine);
            this.treeProcesses.DefaultToolTipProvider = null;
            this.treeProcesses.DisplayDraggingNodes = true;
            this.treeProcesses.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeProcesses.DragDropMarkColor = System.Drawing.Color.Black;
            this.treeProcesses.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeProcesses.FullRowSelect = true;
            this.treeProcesses.LineColor = System.Drawing.SystemColors.ControlDark;
            this.treeProcesses.Location = new System.Drawing.Point(0, 0);
            this.treeProcesses.Model = null;
            this.treeProcesses.Name = "treeProcesses";
            this.treeProcesses.NodeControls.Add(this.nodeIcon);
            this.treeProcesses.NodeControls.Add(this.nodeName);
            this.treeProcesses.NodeControls.Add(this.nodePID);
            this.treeProcesses.NodeControls.Add(this.nodePvtMemory);
            this.treeProcesses.NodeControls.Add(this.nodeWorkingSet);
            this.treeProcesses.NodeControls.Add(this.nodeCPU);
            this.treeProcesses.NodeControls.Add(this.nodeUsername);
            this.treeProcesses.NodeControls.Add(this.nodeDescription);
            this.treeProcesses.NodeControls.Add(this.nodeCompany);
            this.treeProcesses.NodeControls.Add(this.nodeFileName);
            this.treeProcesses.NodeControls.Add(this.nodeCommandLine);
            this.treeProcesses.SelectedNode = null;
            this.treeProcesses.SelectionMode = Aga.Controls.Tree.TreeSelectionMode.Multi;
            this.treeProcesses.ShowNodeToolTips = true;
            this.treeProcesses.Size = new System.Drawing.Size(808, 472);
            this.treeProcesses.TabIndex = 2;
            this.treeProcesses.UseColumns = true;
            this.treeProcesses.SelectionChanged += new System.EventHandler(this.treeProcesses_SelectionChanged);
            this.treeProcesses.ColumnClicked += new System.EventHandler<Aga.Controls.Tree.TreeColumnEventArgs>(this.treeProcesses_ColumnClicked);
            // 
            // columnName
            // 
            this.columnName.Header = "Name";
            this.columnName.Sortable = true;
            this.columnName.SortOrder = System.Windows.Forms.SortOrder.None;
            this.columnName.TooltipText = null;
            this.columnName.Width = 270;
            // 
            // columnPID
            // 
            this.columnPID.Header = "PID";
            this.columnPID.Sortable = true;
            this.columnPID.SortOrder = System.Windows.Forms.SortOrder.None;
            this.columnPID.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnPID.TooltipText = null;
            // 
            // columnPvtMemory
            // 
            this.columnPvtMemory.Header = "Pvt. Memory";
            this.columnPvtMemory.Sortable = true;
            this.columnPvtMemory.SortOrder = System.Windows.Forms.SortOrder.None;
            this.columnPvtMemory.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnPvtMemory.TooltipText = null;
            this.columnPvtMemory.Width = 70;
            // 
            // columnWorkingSet
            // 
            this.columnWorkingSet.Header = "Working Set";
            this.columnWorkingSet.IsVisible = false;
            this.columnWorkingSet.Sortable = true;
            this.columnWorkingSet.SortOrder = System.Windows.Forms.SortOrder.None;
            this.columnWorkingSet.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnWorkingSet.TooltipText = null;
            this.columnWorkingSet.Width = 70;
            // 
            // columnCPU
            // 
            this.columnCPU.Header = "CPU";
            this.columnCPU.Sortable = true;
            this.columnCPU.SortOrder = System.Windows.Forms.SortOrder.None;
            this.columnCPU.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnCPU.TooltipText = null;
            this.columnCPU.Width = 40;
            // 
            // columnUsername
            // 
            this.columnUsername.Header = "Username";
            this.columnUsername.Sortable = true;
            this.columnUsername.SortOrder = System.Windows.Forms.SortOrder.None;
            this.columnUsername.TooltipText = null;
            this.columnUsername.Width = 130;
            // 
            // columnDescription
            // 
            this.columnDescription.Header = "Description";
            this.columnDescription.Sortable = true;
            this.columnDescription.SortOrder = System.Windows.Forms.SortOrder.None;
            this.columnDescription.TooltipText = null;
            this.columnDescription.Width = 200;
            // 
            // columnCompany
            // 
            this.columnCompany.Header = "Company";
            this.columnCompany.IsVisible = false;
            this.columnCompany.Sortable = true;
            this.columnCompany.SortOrder = System.Windows.Forms.SortOrder.None;
            this.columnCompany.TooltipText = null;
            this.columnCompany.Width = 140;
            // 
            // nodeIcon
            // 
            this.nodeIcon.DataPropertyName = "Icon";
            this.nodeIcon.LeftMargin = 1;
            this.nodeIcon.ParentColumn = this.columnName;
            // 
            // nodeName
            // 
            this.nodeName.DataPropertyName = "Name";
            this.nodeName.EditEnabled = false;
            this.nodeName.IncrementalSearchEnabled = true;
            this.nodeName.LeftMargin = 3;
            this.nodeName.ParentColumn = this.columnName;
            this.nodeName.Trimming = System.Drawing.StringTrimming.EllipsisCharacter;
            // 
            // nodePID
            // 
            this.nodePID.DataPropertyName = "DisplayPID";
            this.nodePID.EditEnabled = false;
            this.nodePID.IncrementalSearchEnabled = true;
            this.nodePID.LeftMargin = 3;
            this.nodePID.ParentColumn = this.columnPID;
            this.nodePID.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nodePID.Trimming = System.Drawing.StringTrimming.EllipsisCharacter;
            // 
            // nodePvtMemory
            // 
            this.nodePvtMemory.DataPropertyName = "PvtMemory";
            this.nodePvtMemory.EditEnabled = false;
            this.nodePvtMemory.IncrementalSearchEnabled = true;
            this.nodePvtMemory.LeftMargin = 3;
            this.nodePvtMemory.ParentColumn = this.columnPvtMemory;
            this.nodePvtMemory.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nodePvtMemory.Trimming = System.Drawing.StringTrimming.EllipsisCharacter;
            // 
            // nodeWorkingSet
            // 
            this.nodeWorkingSet.DataPropertyName = "WorkingSet";
            this.nodeWorkingSet.EditEnabled = false;
            this.nodeWorkingSet.IncrementalSearchEnabled = true;
            this.nodeWorkingSet.LeftMargin = 3;
            this.nodeWorkingSet.ParentColumn = this.columnWorkingSet;
            this.nodeWorkingSet.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nodeWorkingSet.Trimming = System.Drawing.StringTrimming.EllipsisCharacter;
            // 
            // nodeCPU
            // 
            this.nodeCPU.DataPropertyName = "CPU";
            this.nodeCPU.EditEnabled = false;
            this.nodeCPU.IncrementalSearchEnabled = true;
            this.nodeCPU.LeftMargin = 3;
            this.nodeCPU.ParentColumn = this.columnCPU;
            this.nodeCPU.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nodeCPU.Trimming = System.Drawing.StringTrimming.EllipsisCharacter;
            // 
            // nodeUsername
            // 
            this.nodeUsername.DataPropertyName = "Username";
            this.nodeUsername.EditEnabled = false;
            this.nodeUsername.IncrementalSearchEnabled = true;
            this.nodeUsername.LeftMargin = 3;
            this.nodeUsername.ParentColumn = this.columnUsername;
            this.nodeUsername.Trimming = System.Drawing.StringTrimming.EllipsisCharacter;
            // 
            // nodeDescription
            // 
            this.nodeDescription.DataPropertyName = "Description";
            this.nodeDescription.EditEnabled = false;
            this.nodeDescription.IncrementalSearchEnabled = true;
            this.nodeDescription.LeftMargin = 3;
            this.nodeDescription.ParentColumn = this.columnDescription;
            this.nodeDescription.Trimming = System.Drawing.StringTrimming.EllipsisCharacter;
            // 
            // nodeCompany
            // 
            this.nodeCompany.DataPropertyName = "Company";
            this.nodeCompany.EditEnabled = false;
            this.nodeCompany.IncrementalSearchEnabled = true;
            this.nodeCompany.LeftMargin = 3;
            this.nodeCompany.ParentColumn = this.columnCompany;
            this.nodeCompany.Trimming = System.Drawing.StringTrimming.EllipsisCharacter;
            // 
            // columnFileName
            // 
            this.columnFileName.Header = "File Name";
            this.columnFileName.IsVisible = false;
            this.columnFileName.Sortable = true;
            this.columnFileName.SortOrder = System.Windows.Forms.SortOrder.None;
            this.columnFileName.TooltipText = null;
            this.columnFileName.Width = 200;
            // 
            // columnCommandLine
            // 
            this.columnCommandLine.Header = "Command Line";
            this.columnCommandLine.Sortable = true;
            this.columnCommandLine.SortOrder = System.Windows.Forms.SortOrder.None;
            this.columnCommandLine.TooltipText = null;
            this.columnCommandLine.Width = 200;
            // 
            // nodeFileName
            // 
            this.nodeFileName.DataPropertyName = "FileName";
            this.nodeFileName.EditEnabled = false;
            this.nodeFileName.IncrementalSearchEnabled = true;
            this.nodeFileName.LeftMargin = 3;
            this.nodeFileName.ParentColumn = this.columnFileName;
            this.nodeFileName.Trimming = System.Drawing.StringTrimming.EllipsisCharacter;
            // 
            // nodeCommandLine
            // 
            this.nodeCommandLine.DataPropertyName = "CommandLine";
            this.nodeCommandLine.EditEnabled = false;
            this.nodeCommandLine.IncrementalSearchEnabled = true;
            this.nodeCommandLine.LeftMargin = 3;
            this.nodeCommandLine.ParentColumn = this.columnCommandLine;
            this.nodeCommandLine.Trimming = System.Drawing.StringTrimming.EllipsisCharacter;
            // 
            // ProcessTree
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.treeProcesses);
            this.DoubleBuffered = true;
            this.Name = "ProcessTree";
            this.Size = new System.Drawing.Size(808, 472);
            this.ResumeLayout(false);

        }

        #endregion

        private Aga.Controls.Tree.TreeViewAdv treeProcesses;
        private Aga.Controls.Tree.TreeColumn columnName;
        private Aga.Controls.Tree.TreeColumn columnPID;
        private Aga.Controls.Tree.TreeColumn columnPvtMemory;
        private Aga.Controls.Tree.TreeColumn columnUsername;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeName;
        private Aga.Controls.Tree.NodeControls.NodeIntegerTextBox nodePID;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodePvtMemory;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeCPU;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeUsername;
        private Aga.Controls.Tree.NodeControls.NodeIcon nodeIcon;
        private Aga.Controls.Tree.TreeColumn columnCPU;
        private Aga.Controls.Tree.TreeColumn columnDescription;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeDescription;
        private Aga.Controls.Tree.TreeColumn columnCompany;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeCompany;
        private Aga.Controls.Tree.TreeColumn columnWorkingSet;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeWorkingSet;
        private Aga.Controls.Tree.TreeColumn columnFileName;
        private Aga.Controls.Tree.TreeColumn columnCommandLine;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeFileName;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeCommandLine;
    }
}
