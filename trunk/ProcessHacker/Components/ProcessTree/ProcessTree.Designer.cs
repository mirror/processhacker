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
            this.columnPeakWorkingSet = new Aga.Controls.Tree.TreeColumn();
            this.columnVirtualSize = new Aga.Controls.Tree.TreeColumn();
            this.columnPeakVirtualSize = new Aga.Controls.Tree.TreeColumn();
            this.columnPagefileUsage = new Aga.Controls.Tree.TreeColumn();
            this.columnPeakPagefileUsage = new Aga.Controls.Tree.TreeColumn();
            this.columnPageFaults = new Aga.Controls.Tree.TreeColumn();
            this.columnCPU = new Aga.Controls.Tree.TreeColumn();
            this.columnUsername = new Aga.Controls.Tree.TreeColumn();
            this.columnSessionId = new Aga.Controls.Tree.TreeColumn();
            this.columnBasePriority = new Aga.Controls.Tree.TreeColumn();
            this.columnDescription = new Aga.Controls.Tree.TreeColumn();
            this.columnCompany = new Aga.Controls.Tree.TreeColumn();
            this.columnFileName = new Aga.Controls.Tree.TreeColumn();
            this.columnCommandLine = new Aga.Controls.Tree.TreeColumn();
            this.columnThreads = new Aga.Controls.Tree.TreeColumn();
            this.columnHandles = new Aga.Controls.Tree.TreeColumn();
            this.columnGdiHandles = new Aga.Controls.Tree.TreeColumn();
            this.columnUserHandles = new Aga.Controls.Tree.TreeColumn();
            this.nodeIcon = new Aga.Controls.Tree.NodeControls.NodeIcon();
            this.nodeName = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodePID = new Aga.Controls.Tree.NodeControls.NodeIntegerTextBox();
            this.nodePvtMemory = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeWorkingSet = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodePeakWorkingSet = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeVirtualSize = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodePeakVirtualSize = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodePagefileUsage = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodePeakPagefileUsage = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodePageFaults = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeCPU = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeUsername = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeSessionId = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeBasePriority = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeDescription = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeCompany = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeFileName = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeCommandLine = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeThreads = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeHandles = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeGdiHandles = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeUserHandles = new Aga.Controls.Tree.NodeControls.NodeTextBox();
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
            this.treeProcesses.Columns.Add(this.columnPeakWorkingSet);
            this.treeProcesses.Columns.Add(this.columnVirtualSize);
            this.treeProcesses.Columns.Add(this.columnPeakVirtualSize);
            this.treeProcesses.Columns.Add(this.columnPagefileUsage);
            this.treeProcesses.Columns.Add(this.columnPeakPagefileUsage);
            this.treeProcesses.Columns.Add(this.columnPageFaults);
            this.treeProcesses.Columns.Add(this.columnCPU);
            this.treeProcesses.Columns.Add(this.columnUsername);
            this.treeProcesses.Columns.Add(this.columnSessionId);
            this.treeProcesses.Columns.Add(this.columnBasePriority);
            this.treeProcesses.Columns.Add(this.columnDescription);
            this.treeProcesses.Columns.Add(this.columnCompany);
            this.treeProcesses.Columns.Add(this.columnFileName);
            this.treeProcesses.Columns.Add(this.columnCommandLine);
            this.treeProcesses.Columns.Add(this.columnThreads);
            this.treeProcesses.Columns.Add(this.columnHandles);
            this.treeProcesses.Columns.Add(this.columnGdiHandles);
            this.treeProcesses.Columns.Add(this.columnUserHandles);
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
            this.treeProcesses.NodeControls.Add(this.nodePeakWorkingSet);
            this.treeProcesses.NodeControls.Add(this.nodeVirtualSize);
            this.treeProcesses.NodeControls.Add(this.nodePeakVirtualSize);
            this.treeProcesses.NodeControls.Add(this.nodePagefileUsage);
            this.treeProcesses.NodeControls.Add(this.nodePeakPagefileUsage);
            this.treeProcesses.NodeControls.Add(this.nodePageFaults);
            this.treeProcesses.NodeControls.Add(this.nodeCPU);
            this.treeProcesses.NodeControls.Add(this.nodeUsername);
            this.treeProcesses.NodeControls.Add(this.nodeSessionId);
            this.treeProcesses.NodeControls.Add(this.nodeBasePriority);
            this.treeProcesses.NodeControls.Add(this.nodeDescription);
            this.treeProcesses.NodeControls.Add(this.nodeCompany);
            this.treeProcesses.NodeControls.Add(this.nodeFileName);
            this.treeProcesses.NodeControls.Add(this.nodeCommandLine);
            this.treeProcesses.NodeControls.Add(this.nodeThreads);
            this.treeProcesses.NodeControls.Add(this.nodeHandles);
            this.treeProcesses.NodeControls.Add(this.nodeGdiHandles);
            this.treeProcesses.NodeControls.Add(this.nodeUserHandles);
            this.treeProcesses.SelectedNode = null;
            this.treeProcesses.SelectionMode = Aga.Controls.Tree.TreeSelectionMode.Multi;
            this.treeProcesses.ShowNodeToolTips = true;
            this.treeProcesses.Size = new System.Drawing.Size(808, 472);
            this.treeProcesses.TabIndex = 2;
            this.treeProcesses.UseColumns = true;
            this.treeProcesses.NodeMouseDoubleClick += new System.EventHandler<Aga.Controls.Tree.TreeNodeAdvMouseEventArgs>(this.treeProcesses_NodeMouseDoubleClick);
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
            // columnPeakWorkingSet
            // 
            this.columnPeakWorkingSet.Header = "Peak Working Set";
            this.columnPeakWorkingSet.IsVisible = false;
            this.columnPeakWorkingSet.Sortable = true;
            this.columnPeakWorkingSet.SortOrder = System.Windows.Forms.SortOrder.None;
            this.columnPeakWorkingSet.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnPeakWorkingSet.TooltipText = null;
            this.columnPeakWorkingSet.Width = 70;
            // 
            // columnVirtualSize
            // 
            this.columnVirtualSize.Header = "Virtual Size";
            this.columnVirtualSize.IsVisible = false;
            this.columnVirtualSize.Sortable = true;
            this.columnVirtualSize.SortOrder = System.Windows.Forms.SortOrder.None;
            this.columnVirtualSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnVirtualSize.TooltipText = null;
            this.columnVirtualSize.Width = 70;
            // 
            // columnPeakVirtualSize
            // 
            this.columnPeakVirtualSize.Header = "Peak Virtual Size";
            this.columnPeakVirtualSize.IsVisible = false;
            this.columnPeakVirtualSize.Sortable = true;
            this.columnPeakVirtualSize.SortOrder = System.Windows.Forms.SortOrder.None;
            this.columnPeakVirtualSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnPeakVirtualSize.TooltipText = null;
            this.columnPeakVirtualSize.Width = 70;
            // 
            // columnPagefileUsage
            // 
            this.columnPagefileUsage.Header = "Pagefile Usage";
            this.columnPagefileUsage.IsVisible = false;
            this.columnPagefileUsage.Sortable = true;
            this.columnPagefileUsage.SortOrder = System.Windows.Forms.SortOrder.None;
            this.columnPagefileUsage.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnPagefileUsage.TooltipText = null;
            this.columnPagefileUsage.Width = 70;
            // 
            // columnPeakPagefileUsage
            // 
            this.columnPeakPagefileUsage.Header = "Peak Pagefile Usage";
            this.columnPeakPagefileUsage.IsVisible = false;
            this.columnPeakPagefileUsage.Sortable = true;
            this.columnPeakPagefileUsage.SortOrder = System.Windows.Forms.SortOrder.None;
            this.columnPeakPagefileUsage.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnPeakPagefileUsage.TooltipText = null;
            this.columnPeakPagefileUsage.Width = 70;
            // 
            // columnPageFaults
            // 
            this.columnPageFaults.Header = "Page Faults";
            this.columnPageFaults.IsVisible = false;
            this.columnPageFaults.Sortable = true;
            this.columnPageFaults.SortOrder = System.Windows.Forms.SortOrder.None;
            this.columnPageFaults.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnPageFaults.TooltipText = null;
            this.columnPageFaults.Width = 60;
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
            // columnSessionId
            // 
            this.columnSessionId.Header = "Session ID";
            this.columnSessionId.IsVisible = false;
            this.columnSessionId.Sortable = true;
            this.columnSessionId.SortOrder = System.Windows.Forms.SortOrder.None;
            this.columnSessionId.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnSessionId.TooltipText = null;
            this.columnSessionId.Width = 20;
            // 
            // columnBasePriority
            // 
            this.columnBasePriority.Header = "Base Priority";
            this.columnBasePriority.IsVisible = false;
            this.columnBasePriority.Sortable = true;
            this.columnBasePriority.SortOrder = System.Windows.Forms.SortOrder.None;
            this.columnBasePriority.TooltipText = null;
            this.columnBasePriority.Width = 70;
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
            this.columnCommandLine.IsVisible = false;
            this.columnCommandLine.Sortable = true;
            this.columnCommandLine.SortOrder = System.Windows.Forms.SortOrder.None;
            this.columnCommandLine.TooltipText = null;
            this.columnCommandLine.Width = 200;
            // 
            // columnThreads
            // 
            this.columnThreads.Header = "Threads";
            this.columnThreads.IsVisible = false;
            this.columnThreads.Sortable = true;
            this.columnThreads.SortOrder = System.Windows.Forms.SortOrder.None;
            this.columnThreads.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnThreads.TooltipText = null;
            this.columnThreads.Width = 35;
            // 
            // columnHandles
            // 
            this.columnHandles.Header = "Handles";
            this.columnHandles.IsVisible = false;
            this.columnHandles.Sortable = true;
            this.columnHandles.SortOrder = System.Windows.Forms.SortOrder.None;
            this.columnHandles.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHandles.TooltipText = null;
            this.columnHandles.Width = 35;
            // 
            // columnGdiHandles
            // 
            this.columnGdiHandles.Header = "GDI Handles";
            this.columnGdiHandles.IsVisible = false;
            this.columnGdiHandles.SortOrder = System.Windows.Forms.SortOrder.None;
            this.columnGdiHandles.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnGdiHandles.TooltipText = null;
            this.columnGdiHandles.Width = 35;
            // 
            // columnUserHandles
            // 
            this.columnUserHandles.Header = "USER Handles";
            this.columnUserHandles.IsVisible = false;
            this.columnUserHandles.SortOrder = System.Windows.Forms.SortOrder.None;
            this.columnUserHandles.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnUserHandles.TooltipText = null;
            this.columnUserHandles.Width = 35;
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
            // nodePeakWorkingSet
            // 
            this.nodePeakWorkingSet.DataPropertyName = "PeakWorkingSet";
            this.nodePeakWorkingSet.EditEnabled = false;
            this.nodePeakWorkingSet.IncrementalSearchEnabled = true;
            this.nodePeakWorkingSet.LeftMargin = 3;
            this.nodePeakWorkingSet.ParentColumn = this.columnPeakWorkingSet;
            this.nodePeakWorkingSet.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nodePeakWorkingSet.Trimming = System.Drawing.StringTrimming.EllipsisCharacter;
            // 
            // nodeVirtualSize
            // 
            this.nodeVirtualSize.DataPropertyName = "VirtualSize";
            this.nodeVirtualSize.EditEnabled = false;
            this.nodeVirtualSize.IncrementalSearchEnabled = true;
            this.nodeVirtualSize.LeftMargin = 3;
            this.nodeVirtualSize.ParentColumn = this.columnVirtualSize;
            this.nodeVirtualSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nodeVirtualSize.Trimming = System.Drawing.StringTrimming.EllipsisCharacter;
            // 
            // nodePeakVirtualSize
            // 
            this.nodePeakVirtualSize.DataPropertyName = "PeakVirtualSize";
            this.nodePeakVirtualSize.EditEnabled = false;
            this.nodePeakVirtualSize.IncrementalSearchEnabled = true;
            this.nodePeakVirtualSize.LeftMargin = 3;
            this.nodePeakVirtualSize.ParentColumn = this.columnPeakVirtualSize;
            this.nodePeakVirtualSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nodePeakVirtualSize.Trimming = System.Drawing.StringTrimming.EllipsisCharacter;
            // 
            // nodePagefileUsage
            // 
            this.nodePagefileUsage.DataPropertyName = "PagefileUsage";
            this.nodePagefileUsage.EditEnabled = false;
            this.nodePagefileUsage.IncrementalSearchEnabled = true;
            this.nodePagefileUsage.LeftMargin = 3;
            this.nodePagefileUsage.ParentColumn = this.columnPagefileUsage;
            this.nodePagefileUsage.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nodePagefileUsage.Trimming = System.Drawing.StringTrimming.EllipsisCharacter;
            // 
            // nodePeakPagefileUsage
            // 
            this.nodePeakPagefileUsage.DataPropertyName = "PeakPagefileUsage";
            this.nodePeakPagefileUsage.EditEnabled = false;
            this.nodePeakPagefileUsage.IncrementalSearchEnabled = true;
            this.nodePeakPagefileUsage.LeftMargin = 3;
            this.nodePeakPagefileUsage.ParentColumn = this.columnPeakPagefileUsage;
            this.nodePeakPagefileUsage.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nodePeakPagefileUsage.Trimming = System.Drawing.StringTrimming.EllipsisCharacter;
            // 
            // nodePageFaults
            // 
            this.nodePageFaults.DataPropertyName = "PageFaults";
            this.nodePageFaults.EditEnabled = false;
            this.nodePageFaults.IncrementalSearchEnabled = true;
            this.nodePageFaults.LeftMargin = 3;
            this.nodePageFaults.ParentColumn = this.columnPageFaults;
            this.nodePageFaults.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nodePageFaults.Trimming = System.Drawing.StringTrimming.EllipsisCharacter;
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
            // nodeSessionId
            // 
            this.nodeSessionId.DataPropertyName = "SessionId";
            this.nodeSessionId.EditEnabled = false;
            this.nodeSessionId.IncrementalSearchEnabled = true;
            this.nodeSessionId.LeftMargin = 3;
            this.nodeSessionId.ParentColumn = this.columnSessionId;
            this.nodeSessionId.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nodeSessionId.Trimming = System.Drawing.StringTrimming.EllipsisCharacter;
            // 
            // nodeBasePriority
            // 
            this.nodeBasePriority.DataPropertyName = "BasePriority";
            this.nodeBasePriority.EditEnabled = false;
            this.nodeBasePriority.IncrementalSearchEnabled = true;
            this.nodeBasePriority.LeftMargin = 3;
            this.nodeBasePriority.ParentColumn = this.columnBasePriority;
            this.nodeBasePriority.Trimming = System.Drawing.StringTrimming.EllipsisCharacter;
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
            // nodeThreads
            // 
            this.nodeThreads.DataPropertyName = "Threads";
            this.nodeThreads.EditEnabled = false;
            this.nodeThreads.IncrementalSearchEnabled = true;
            this.nodeThreads.LeftMargin = 3;
            this.nodeThreads.ParentColumn = this.columnThreads;
            this.nodeThreads.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nodeThreads.Trimming = System.Drawing.StringTrimming.EllipsisCharacter;
            // 
            // nodeHandles
            // 
            this.nodeHandles.DataPropertyName = "Handles";
            this.nodeHandles.EditEnabled = false;
            this.nodeHandles.IncrementalSearchEnabled = true;
            this.nodeHandles.LeftMargin = 3;
            this.nodeHandles.ParentColumn = this.columnHandles;
            this.nodeHandles.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nodeHandles.Trimming = System.Drawing.StringTrimming.EllipsisCharacter;
            // 
            // nodeGdiHandles
            // 
            this.nodeGdiHandles.DataPropertyName = "GdiHandles";
            this.nodeGdiHandles.EditEnabled = false;
            this.nodeGdiHandles.IncrementalSearchEnabled = true;
            this.nodeGdiHandles.LeftMargin = 3;
            this.nodeGdiHandles.ParentColumn = this.columnGdiHandles;
            this.nodeGdiHandles.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nodeGdiHandles.Trimming = System.Drawing.StringTrimming.EllipsisCharacter;
            // 
            // nodeUserHandles
            // 
            this.nodeUserHandles.DataPropertyName = "UserHandles";
            this.nodeUserHandles.EditEnabled = false;
            this.nodeUserHandles.IncrementalSearchEnabled = true;
            this.nodeUserHandles.LeftMargin = 3;
            this.nodeUserHandles.ParentColumn = this.columnUserHandles;
            this.nodeUserHandles.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nodeUserHandles.Trimming = System.Drawing.StringTrimming.EllipsisCharacter;
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
        private Aga.Controls.Tree.TreeColumn columnSessionId;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeSessionId;
        private Aga.Controls.Tree.TreeColumn columnThreads;
        private Aga.Controls.Tree.TreeColumn columnHandles;
        private Aga.Controls.Tree.TreeColumn columnGdiHandles;
        private Aga.Controls.Tree.TreeColumn columnUserHandles;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeThreads;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeHandles;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeGdiHandles;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeUserHandles;
        private Aga.Controls.Tree.TreeColumn columnBasePriority;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeBasePriority;
        private Aga.Controls.Tree.TreeColumn columnVirtualSize;
        private Aga.Controls.Tree.TreeColumn columnPeakVirtualSize;
        private Aga.Controls.Tree.TreeColumn columnPeakWorkingSet;
        private Aga.Controls.Tree.TreeColumn columnPageFaults;
        private Aga.Controls.Tree.TreeColumn columnPagefileUsage;
        private Aga.Controls.Tree.TreeColumn columnPeakPagefileUsage;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodePeakWorkingSet;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeVirtualSize;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodePeakVirtualSize;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodePagefileUsage;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodePeakPagefileUsage;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodePageFaults;
    }
}
