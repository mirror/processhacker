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
            this.columnMemory = new Aga.Controls.Tree.TreeColumn();
            this.columnCPU = new Aga.Controls.Tree.TreeColumn();
            this.columnUsername = new Aga.Controls.Tree.TreeColumn();
            this.nodeIcon = new Aga.Controls.Tree.NodeControls.NodeIcon();
            this.nodeName = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodePID = new Aga.Controls.Tree.NodeControls.NodeIntegerTextBox();
            this.nodeMemory = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeCPU = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeUsername = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.SuspendLayout();
            // 
            // treeProcesses
            // 
            this.treeProcesses.AllowColumnReorder = true;
            this.treeProcesses.BackColor = System.Drawing.SystemColors.Window;
            this.treeProcesses.Columns.Add(this.columnName);
            this.treeProcesses.Columns.Add(this.columnPID);
            this.treeProcesses.Columns.Add(this.columnMemory);
            this.treeProcesses.Columns.Add(this.columnCPU);
            this.treeProcesses.Columns.Add(this.columnUsername);
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
            this.treeProcesses.NodeControls.Add(this.nodeMemory);
            this.treeProcesses.NodeControls.Add(this.nodeCPU);
            this.treeProcesses.NodeControls.Add(this.nodeUsername);
            this.treeProcesses.SelectedNode = null;
            this.treeProcesses.SelectionMode = Aga.Controls.Tree.TreeSelectionMode.Multi;
            this.treeProcesses.ShowNodeToolTips = true;
            this.treeProcesses.Size = new System.Drawing.Size(450, 472);
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
            this.columnName.Width = 160;
            // 
            // columnPID
            // 
            this.columnPID.Header = "PID";
            this.columnPID.Sortable = true;
            this.columnPID.SortOrder = System.Windows.Forms.SortOrder.None;
            this.columnPID.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnPID.TooltipText = null;
            // 
            // columnMemory
            // 
            this.columnMemory.Header = "Pvt. Memory";
            this.columnMemory.Sortable = true;
            this.columnMemory.SortOrder = System.Windows.Forms.SortOrder.None;
            this.columnMemory.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnMemory.TooltipText = null;
            this.columnMemory.Width = 70;
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
            this.columnUsername.Width = 100;
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
            this.nodePID.DataPropertyName = "PID";
            this.nodePID.EditEnabled = false;
            this.nodePID.IncrementalSearchEnabled = true;
            this.nodePID.LeftMargin = 3;
            this.nodePID.ParentColumn = this.columnPID;
            this.nodePID.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nodePID.Trimming = System.Drawing.StringTrimming.EllipsisCharacter;
            // 
            // nodeMemory
            // 
            this.nodeMemory.DataPropertyName = "Memory";
            this.nodeMemory.EditEnabled = false;
            this.nodeMemory.IncrementalSearchEnabled = true;
            this.nodeMemory.LeftMargin = 3;
            this.nodeMemory.ParentColumn = this.columnMemory;
            this.nodeMemory.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nodeMemory.Trimming = System.Drawing.StringTrimming.EllipsisCharacter;
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
            // ProcessTree
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.treeProcesses);
            this.DoubleBuffered = true;
            this.Name = "ProcessTree";
            this.Size = new System.Drawing.Size(450, 472);
            this.ResumeLayout(false);

        }

        #endregion

        private Aga.Controls.Tree.TreeViewAdv treeProcesses;
        private Aga.Controls.Tree.TreeColumn columnName;
        private Aga.Controls.Tree.TreeColumn columnPID;
        private Aga.Controls.Tree.TreeColumn columnMemory;
        private Aga.Controls.Tree.TreeColumn columnUsername;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeName;
        private Aga.Controls.Tree.NodeControls.NodeIntegerTextBox nodePID;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeMemory;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeCPU;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeUsername;
        private Aga.Controls.Tree.NodeControls.NodeIcon nodeIcon;
        private Aga.Controls.Tree.TreeColumn columnCPU;
    }
}
