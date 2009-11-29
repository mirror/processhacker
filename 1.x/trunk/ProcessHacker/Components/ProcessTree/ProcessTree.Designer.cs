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
            this.columnPrivateWorkingSet = new Aga.Controls.Tree.TreeColumn();
            this.columnSharedWorkingSet = new Aga.Controls.Tree.TreeColumn();
            this.columnShareableWorkingSet = new Aga.Controls.Tree.TreeColumn();
            this.columnVirtualSize = new Aga.Controls.Tree.TreeColumn();
            this.columnPeakVirtualSize = new Aga.Controls.Tree.TreeColumn();
            this.columnPagefileUsage = new Aga.Controls.Tree.TreeColumn();
            this.columnPeakPagefileUsage = new Aga.Controls.Tree.TreeColumn();
            this.columnPageFaults = new Aga.Controls.Tree.TreeColumn();
            this.columnCPU = new Aga.Controls.Tree.TreeColumn();
            this.columnIoTotal = new Aga.Controls.Tree.TreeColumn();
            this.columnUsername = new Aga.Controls.Tree.TreeColumn();
            this.columnSessionId = new Aga.Controls.Tree.TreeColumn();
            this.columnPriorityClass = new Aga.Controls.Tree.TreeColumn();
            this.columnBasePriority = new Aga.Controls.Tree.TreeColumn();
            this.columnDescription = new Aga.Controls.Tree.TreeColumn();
            this.columnCompany = new Aga.Controls.Tree.TreeColumn();
            this.columnFileName = new Aga.Controls.Tree.TreeColumn();
            this.columnCommandLine = new Aga.Controls.Tree.TreeColumn();
            this.columnThreads = new Aga.Controls.Tree.TreeColumn();
            this.columnHandles = new Aga.Controls.Tree.TreeColumn();
            this.columnGdiHandles = new Aga.Controls.Tree.TreeColumn();
            this.columnUserHandles = new Aga.Controls.Tree.TreeColumn();
            this.columnIoReadOther = new Aga.Controls.Tree.TreeColumn();
            this.columnIoWrite = new Aga.Controls.Tree.TreeColumn();
            this.columnIntegrity = new Aga.Controls.Tree.TreeColumn();
            this.columnIoPriority = new Aga.Controls.Tree.TreeColumn();
            this.columnPagePriority = new Aga.Controls.Tree.TreeColumn();
            this.columnStartTime = new Aga.Controls.Tree.TreeColumn();
            this.columnRelativeStartTime = new Aga.Controls.Tree.TreeColumn();
            this.columnTotalCpuTime = new Aga.Controls.Tree.TreeColumn();
            this.columnUserCpuTime = new Aga.Controls.Tree.TreeColumn();
            this.columnKernelCpuTime = new Aga.Controls.Tree.TreeColumn();
            this.columnVerificationStatus = new Aga.Controls.Tree.TreeColumn();
            this.nodeIcon = new Aga.Controls.Tree.NodeControls.NodeIcon();
            this.nodeName = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodePid = new Aga.Controls.Tree.NodeControls.NodeIntegerTextBox();
            this.nodePvtMemory = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeWorkingSet = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodePeakWorkingSet = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeVirtualSize = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodePeakVirtualSize = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodePrivateWorkingSet = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeSharedWorkingSet = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeShareableWorkingSet = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodePagefileUsage = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodePeakPagefileUsage = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodePageFaults = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeCpu = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeUsername = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeSessionId = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodePriorityClass = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeBasePriority = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeDescription = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeCompany = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeFileName = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeCommandLine = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeThreads = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeHandles = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeGdiHandles = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeUserHandles = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeIoTotal = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeIoReadOther = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeIoWrite = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeIntegrity = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeIoPriority = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodePagePriority = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeStartTime = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeRelativeStartTime = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeTotalCpuTime = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeKernelCpuTime = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeUserCpuTime = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeVerificationStatus = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeVerifiedSigner = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.columnVerifiedSigner = new Aga.Controls.Tree.TreeColumn();
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
            this.treeProcesses.Columns.Add(this.columnPrivateWorkingSet);
            this.treeProcesses.Columns.Add(this.columnSharedWorkingSet);
            this.treeProcesses.Columns.Add(this.columnShareableWorkingSet);
            this.treeProcesses.Columns.Add(this.columnVirtualSize);
            this.treeProcesses.Columns.Add(this.columnPeakVirtualSize);
            this.treeProcesses.Columns.Add(this.columnPagefileUsage);
            this.treeProcesses.Columns.Add(this.columnPeakPagefileUsage);
            this.treeProcesses.Columns.Add(this.columnPageFaults);
            this.treeProcesses.Columns.Add(this.columnCPU);
            this.treeProcesses.Columns.Add(this.columnIoTotal);
            this.treeProcesses.Columns.Add(this.columnUsername);
            this.treeProcesses.Columns.Add(this.columnSessionId);
            this.treeProcesses.Columns.Add(this.columnPriorityClass);
            this.treeProcesses.Columns.Add(this.columnBasePriority);
            this.treeProcesses.Columns.Add(this.columnDescription);
            this.treeProcesses.Columns.Add(this.columnCompany);
            this.treeProcesses.Columns.Add(this.columnFileName);
            this.treeProcesses.Columns.Add(this.columnCommandLine);
            this.treeProcesses.Columns.Add(this.columnThreads);
            this.treeProcesses.Columns.Add(this.columnHandles);
            this.treeProcesses.Columns.Add(this.columnGdiHandles);
            this.treeProcesses.Columns.Add(this.columnUserHandles);
            this.treeProcesses.Columns.Add(this.columnIoReadOther);
            this.treeProcesses.Columns.Add(this.columnIoWrite);
            this.treeProcesses.Columns.Add(this.columnIntegrity);
            this.treeProcesses.Columns.Add(this.columnIoPriority);
            this.treeProcesses.Columns.Add(this.columnPagePriority);
            this.treeProcesses.Columns.Add(this.columnStartTime);
            this.treeProcesses.Columns.Add(this.columnRelativeStartTime);
            this.treeProcesses.Columns.Add(this.columnTotalCpuTime);
            this.treeProcesses.Columns.Add(this.columnUserCpuTime);
            this.treeProcesses.Columns.Add(this.columnKernelCpuTime);
            this.treeProcesses.Columns.Add(this.columnVerificationStatus);
            this.treeProcesses.Columns.Add(this.columnVerifiedSigner);
            this.treeProcesses.DefaultToolTipProvider = null;
            this.treeProcesses.DisplayDraggingNodes = true;
            this.treeProcesses.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeProcesses.DragDropMarkColor = System.Drawing.Color.Black;
            this.treeProcesses.FullRowSelect = true;
            this.treeProcesses.LineColor = System.Drawing.SystemColors.ControlDark;
            this.treeProcesses.Location = new System.Drawing.Point(0, 0);
            this.treeProcesses.Model = null;
            this.treeProcesses.Name = "treeProcesses";
            this.treeProcesses.NodeControls.Add(this.nodeIcon);
            this.treeProcesses.NodeControls.Add(this.nodeName);
            this.treeProcesses.NodeControls.Add(this.nodePid);
            this.treeProcesses.NodeControls.Add(this.nodePvtMemory);
            this.treeProcesses.NodeControls.Add(this.nodeWorkingSet);
            this.treeProcesses.NodeControls.Add(this.nodePeakWorkingSet);
            this.treeProcesses.NodeControls.Add(this.nodeVirtualSize);
            this.treeProcesses.NodeControls.Add(this.nodePeakVirtualSize);
            this.treeProcesses.NodeControls.Add(this.nodePrivateWorkingSet);
            this.treeProcesses.NodeControls.Add(this.nodeSharedWorkingSet);
            this.treeProcesses.NodeControls.Add(this.nodeShareableWorkingSet);
            this.treeProcesses.NodeControls.Add(this.nodePagefileUsage);
            this.treeProcesses.NodeControls.Add(this.nodePeakPagefileUsage);
            this.treeProcesses.NodeControls.Add(this.nodePageFaults);
            this.treeProcesses.NodeControls.Add(this.nodeCpu);
            this.treeProcesses.NodeControls.Add(this.nodeUsername);
            this.treeProcesses.NodeControls.Add(this.nodeSessionId);
            this.treeProcesses.NodeControls.Add(this.nodePriorityClass);
            this.treeProcesses.NodeControls.Add(this.nodeBasePriority);
            this.treeProcesses.NodeControls.Add(this.nodeDescription);
            this.treeProcesses.NodeControls.Add(this.nodeCompany);
            this.treeProcesses.NodeControls.Add(this.nodeFileName);
            this.treeProcesses.NodeControls.Add(this.nodeCommandLine);
            this.treeProcesses.NodeControls.Add(this.nodeThreads);
            this.treeProcesses.NodeControls.Add(this.nodeHandles);
            this.treeProcesses.NodeControls.Add(this.nodeGdiHandles);
            this.treeProcesses.NodeControls.Add(this.nodeUserHandles);
            this.treeProcesses.NodeControls.Add(this.nodeIoTotal);
            this.treeProcesses.NodeControls.Add(this.nodeIoReadOther);
            this.treeProcesses.NodeControls.Add(this.nodeIoWrite);
            this.treeProcesses.NodeControls.Add(this.nodeIntegrity);
            this.treeProcesses.NodeControls.Add(this.nodeIoPriority);
            this.treeProcesses.NodeControls.Add(this.nodePagePriority);
            this.treeProcesses.NodeControls.Add(this.nodeStartTime);
            this.treeProcesses.NodeControls.Add(this.nodeRelativeStartTime);
            this.treeProcesses.NodeControls.Add(this.nodeTotalCpuTime);
            this.treeProcesses.NodeControls.Add(this.nodeKernelCpuTime);
            this.treeProcesses.NodeControls.Add(this.nodeUserCpuTime);
            this.treeProcesses.NodeControls.Add(this.nodeVerificationStatus);
            this.treeProcesses.NodeControls.Add(this.nodeVerifiedSigner);
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
            this.columnName.Width = 245;
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
            // columnPrivateWorkingSet
            // 
            this.columnPrivateWorkingSet.Header = "Private WS";
            this.columnPrivateWorkingSet.IsVisible = false;
            this.columnPrivateWorkingSet.Sortable = true;
            this.columnPrivateWorkingSet.SortOrder = System.Windows.Forms.SortOrder.None;
            this.columnPrivateWorkingSet.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnPrivateWorkingSet.TooltipText = null;
            this.columnPrivateWorkingSet.Width = 70;
            // 
            // columnSharedWorkingSet
            // 
            this.columnSharedWorkingSet.Header = "Shared WS";
            this.columnSharedWorkingSet.IsVisible = false;
            this.columnSharedWorkingSet.Sortable = true;
            this.columnSharedWorkingSet.SortOrder = System.Windows.Forms.SortOrder.None;
            this.columnSharedWorkingSet.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnSharedWorkingSet.TooltipText = null;
            this.columnSharedWorkingSet.Width = 70;
            // 
            // columnShareableWorkingSet
            // 
            this.columnShareableWorkingSet.Header = "Shareable WS";
            this.columnShareableWorkingSet.IsVisible = false;
            this.columnShareableWorkingSet.Sortable = true;
            this.columnShareableWorkingSet.SortOrder = System.Windows.Forms.SortOrder.None;
            this.columnShareableWorkingSet.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnShareableWorkingSet.TooltipText = null;
            this.columnShareableWorkingSet.Width = 70;
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
            // columnIoTotal
            // 
            this.columnIoTotal.Header = "I/O Total";
            this.columnIoTotal.Sortable = true;
            this.columnIoTotal.SortOrder = System.Windows.Forms.SortOrder.None;
            this.columnIoTotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnIoTotal.TooltipText = null;
            this.columnIoTotal.Width = 65;
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
            // columnPriorityClass
            // 
            this.columnPriorityClass.Header = "Priority Class";
            this.columnPriorityClass.IsVisible = false;
            this.columnPriorityClass.Sortable = true;
            this.columnPriorityClass.SortOrder = System.Windows.Forms.SortOrder.None;
            this.columnPriorityClass.TooltipText = null;
            this.columnPriorityClass.Width = 70;
            // 
            // columnBasePriority
            // 
            this.columnBasePriority.Header = "Base Priority";
            this.columnBasePriority.IsVisible = false;
            this.columnBasePriority.Sortable = true;
            this.columnBasePriority.SortOrder = System.Windows.Forms.SortOrder.None;
            this.columnBasePriority.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnBasePriority.TooltipText = null;
            // 
            // columnDescription
            // 
            this.columnDescription.Header = "Description";
            this.columnDescription.Sortable = true;
            this.columnDescription.SortOrder = System.Windows.Forms.SortOrder.None;
            this.columnDescription.TooltipText = null;
            this.columnDescription.Width = 170;
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
            // columnIoReadOther
            // 
            this.columnIoReadOther.Header = "I/O R+O";
            this.columnIoReadOther.IsVisible = false;
            this.columnIoReadOther.Sortable = true;
            this.columnIoReadOther.SortOrder = System.Windows.Forms.SortOrder.None;
            this.columnIoReadOther.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnIoReadOther.TooltipText = null;
            this.columnIoReadOther.Width = 65;
            // 
            // columnIoWrite
            // 
            this.columnIoWrite.Header = "I/O W";
            this.columnIoWrite.IsVisible = false;
            this.columnIoWrite.Sortable = true;
            this.columnIoWrite.SortOrder = System.Windows.Forms.SortOrder.None;
            this.columnIoWrite.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnIoWrite.TooltipText = null;
            this.columnIoWrite.Width = 65;
            // 
            // columnIntegrity
            // 
            this.columnIntegrity.Header = "Integrity";
            this.columnIntegrity.IsVisible = false;
            this.columnIntegrity.Sortable = true;
            this.columnIntegrity.SortOrder = System.Windows.Forms.SortOrder.None;
            this.columnIntegrity.TooltipText = null;
            this.columnIntegrity.Width = 100;
            // 
            // columnIoPriority
            // 
            this.columnIoPriority.Header = "I/O Priority";
            this.columnIoPriority.IsVisible = false;
            this.columnIoPriority.Sortable = true;
            this.columnIoPriority.SortOrder = System.Windows.Forms.SortOrder.None;
            this.columnIoPriority.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnIoPriority.TooltipText = null;
            // 
            // columnPagePriority
            // 
            this.columnPagePriority.Header = "Page Priority";
            this.columnPagePriority.IsVisible = false;
            this.columnPagePriority.Sortable = true;
            this.columnPagePriority.SortOrder = System.Windows.Forms.SortOrder.None;
            this.columnPagePriority.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnPagePriority.TooltipText = null;
            // 
            // columnStartTime
            // 
            this.columnStartTime.Header = "Start Time";
            this.columnStartTime.IsVisible = false;
            this.columnStartTime.Sortable = true;
            this.columnStartTime.SortOrder = System.Windows.Forms.SortOrder.None;
            this.columnStartTime.TooltipText = null;
            this.columnStartTime.Width = 100;
            // 
            // columnRelativeStartTime
            // 
            this.columnRelativeStartTime.Header = "Start Time (Relative)";
            this.columnRelativeStartTime.IsVisible = false;
            this.columnRelativeStartTime.Sortable = true;
            this.columnRelativeStartTime.SortOrder = System.Windows.Forms.SortOrder.None;
            this.columnRelativeStartTime.TooltipText = null;
            this.columnRelativeStartTime.Width = 100;
            // 
            // columnTotalCpuTime
            // 
            this.columnTotalCpuTime.Header = "Total CPU Time";
            this.columnTotalCpuTime.IsVisible = false;
            this.columnTotalCpuTime.Sortable = true;
            this.columnTotalCpuTime.SortOrder = System.Windows.Forms.SortOrder.None;
            this.columnTotalCpuTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnTotalCpuTime.TooltipText = null;
            this.columnTotalCpuTime.Width = 100;
            // 
            // columnUserCpuTime
            // 
            this.columnUserCpuTime.Header = "User CPU Time";
            this.columnUserCpuTime.IsVisible = false;
            this.columnUserCpuTime.Sortable = true;
            this.columnUserCpuTime.SortOrder = System.Windows.Forms.SortOrder.None;
            this.columnUserCpuTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnUserCpuTime.TooltipText = null;
            this.columnUserCpuTime.Width = 100;
            // 
            // columnKernelCpuTime
            // 
            this.columnKernelCpuTime.Header = "Kernel CPU Time";
            this.columnKernelCpuTime.IsVisible = false;
            this.columnKernelCpuTime.Sortable = true;
            this.columnKernelCpuTime.SortOrder = System.Windows.Forms.SortOrder.None;
            this.columnKernelCpuTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnKernelCpuTime.TooltipText = null;
            this.columnKernelCpuTime.Width = 100;
            // 
            // columnVerificationStatus
            // 
            this.columnVerificationStatus.Header = "Verification Status";
            this.columnVerificationStatus.IsVisible = false;
            this.columnVerificationStatus.Sortable = true;
            this.columnVerificationStatus.SortOrder = System.Windows.Forms.SortOrder.None;
            this.columnVerificationStatus.TooltipText = null;
            this.columnVerificationStatus.Width = 60;
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
            // nodePid
            // 
            this.nodePid.DataPropertyName = "DisplayPid";
            this.nodePid.EditEnabled = false;
            this.nodePid.IncrementalSearchEnabled = true;
            this.nodePid.LeftMargin = 3;
            this.nodePid.ParentColumn = this.columnPID;
            this.nodePid.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nodePid.Trimming = System.Drawing.StringTrimming.EllipsisCharacter;
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
            // nodePrivateWorkingSet
            // 
            this.nodePrivateWorkingSet.DataPropertyName = "PrivateWorkingSet";
            this.nodePrivateWorkingSet.EditEnabled = false;
            this.nodePrivateWorkingSet.IncrementalSearchEnabled = true;
            this.nodePrivateWorkingSet.LeftMargin = 3;
            this.nodePrivateWorkingSet.ParentColumn = this.columnPrivateWorkingSet;
            this.nodePrivateWorkingSet.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nodePrivateWorkingSet.Trimming = System.Drawing.StringTrimming.EllipsisCharacter;
            // 
            // nodeSharedWorkingSet
            // 
            this.nodeSharedWorkingSet.DataPropertyName = "SharedWorkingSet";
            this.nodeSharedWorkingSet.EditEnabled = false;
            this.nodeSharedWorkingSet.IncrementalSearchEnabled = true;
            this.nodeSharedWorkingSet.LeftMargin = 3;
            this.nodeSharedWorkingSet.ParentColumn = this.columnSharedWorkingSet;
            this.nodeSharedWorkingSet.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nodeSharedWorkingSet.Trimming = System.Drawing.StringTrimming.EllipsisCharacter;
            // 
            // nodeShareableWorkingSet
            // 
            this.nodeShareableWorkingSet.DataPropertyName = "ShareableWorkingSet";
            this.nodeShareableWorkingSet.EditEnabled = false;
            this.nodeShareableWorkingSet.IncrementalSearchEnabled = true;
            this.nodeShareableWorkingSet.LeftMargin = 3;
            this.nodeShareableWorkingSet.ParentColumn = this.columnShareableWorkingSet;
            this.nodeShareableWorkingSet.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nodeShareableWorkingSet.Trimming = System.Drawing.StringTrimming.EllipsisCharacter;
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
            // nodeCpu
            // 
            this.nodeCpu.DataPropertyName = "Cpu";
            this.nodeCpu.EditEnabled = false;
            this.nodeCpu.IncrementalSearchEnabled = true;
            this.nodeCpu.LeftMargin = 3;
            this.nodeCpu.ParentColumn = this.columnCPU;
            this.nodeCpu.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nodeCpu.Trimming = System.Drawing.StringTrimming.EllipsisCharacter;
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
            // nodePriorityClass
            // 
            this.nodePriorityClass.DataPropertyName = "PriorityClass";
            this.nodePriorityClass.EditEnabled = false;
            this.nodePriorityClass.IncrementalSearchEnabled = true;
            this.nodePriorityClass.LeftMargin = 3;
            this.nodePriorityClass.ParentColumn = this.columnPriorityClass;
            this.nodePriorityClass.Trimming = System.Drawing.StringTrimming.EllipsisCharacter;
            // 
            // nodeBasePriority
            // 
            this.nodeBasePriority.DataPropertyName = "BasePriority";
            this.nodeBasePriority.EditEnabled = false;
            this.nodeBasePriority.IncrementalSearchEnabled = true;
            this.nodeBasePriority.LeftMargin = 3;
            this.nodeBasePriority.ParentColumn = this.columnBasePriority;
            this.nodeBasePriority.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
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
            this.nodeFileName.Trimming = System.Drawing.StringTrimming.EllipsisPath;
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
            // nodeIoTotal
            // 
            this.nodeIoTotal.DataPropertyName = "IoTotal";
            this.nodeIoTotal.EditEnabled = false;
            this.nodeIoTotal.IncrementalSearchEnabled = true;
            this.nodeIoTotal.LeftMargin = 3;
            this.nodeIoTotal.ParentColumn = this.columnIoTotal;
            this.nodeIoTotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nodeIoTotal.Trimming = System.Drawing.StringTrimming.EllipsisCharacter;
            // 
            // nodeIoReadOther
            // 
            this.nodeIoReadOther.DataPropertyName = "IoReadOther";
            this.nodeIoReadOther.EditEnabled = false;
            this.nodeIoReadOther.IncrementalSearchEnabled = true;
            this.nodeIoReadOther.LeftMargin = 3;
            this.nodeIoReadOther.ParentColumn = this.columnIoReadOther;
            this.nodeIoReadOther.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nodeIoReadOther.Trimming = System.Drawing.StringTrimming.EllipsisCharacter;
            // 
            // nodeIoWrite
            // 
            this.nodeIoWrite.DataPropertyName = "IoWrite";
            this.nodeIoWrite.EditEnabled = false;
            this.nodeIoWrite.IncrementalSearchEnabled = true;
            this.nodeIoWrite.LeftMargin = 3;
            this.nodeIoWrite.ParentColumn = this.columnIoWrite;
            this.nodeIoWrite.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nodeIoWrite.Trimming = System.Drawing.StringTrimming.EllipsisCharacter;
            // 
            // nodeIntegrity
            // 
            this.nodeIntegrity.DataPropertyName = "Integrity";
            this.nodeIntegrity.EditEnabled = false;
            this.nodeIntegrity.IncrementalSearchEnabled = true;
            this.nodeIntegrity.LeftMargin = 3;
            this.nodeIntegrity.ParentColumn = this.columnIntegrity;
            this.nodeIntegrity.Trimming = System.Drawing.StringTrimming.EllipsisCharacter;
            // 
            // nodeIoPriority
            // 
            this.nodeIoPriority.DataPropertyName = "IoPriority";
            this.nodeIoPriority.EditEnabled = false;
            this.nodeIoPriority.IncrementalSearchEnabled = true;
            this.nodeIoPriority.LeftMargin = 3;
            this.nodeIoPriority.ParentColumn = this.columnIoPriority;
            this.nodeIoPriority.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nodeIoPriority.Trimming = System.Drawing.StringTrimming.EllipsisCharacter;
            // 
            // nodePagePriority
            // 
            this.nodePagePriority.DataPropertyName = "PagePriority";
            this.nodePagePriority.EditEnabled = false;
            this.nodePagePriority.IncrementalSearchEnabled = true;
            this.nodePagePriority.LeftMargin = 3;
            this.nodePagePriority.ParentColumn = this.columnPagePriority;
            this.nodePagePriority.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nodePagePriority.Trimming = System.Drawing.StringTrimming.EllipsisCharacter;
            // 
            // nodeStartTime
            // 
            this.nodeStartTime.DataPropertyName = "StartTime";
            this.nodeStartTime.EditEnabled = false;
            this.nodeStartTime.IncrementalSearchEnabled = true;
            this.nodeStartTime.LeftMargin = 3;
            this.nodeStartTime.ParentColumn = this.columnStartTime;
            this.nodeStartTime.Trimming = System.Drawing.StringTrimming.EllipsisCharacter;
            // 
            // nodeRelativeStartTime
            // 
            this.nodeRelativeStartTime.DataPropertyName = "RelativeStartTime";
            this.nodeRelativeStartTime.EditEnabled = false;
            this.nodeRelativeStartTime.IncrementalSearchEnabled = true;
            this.nodeRelativeStartTime.LeftMargin = 3;
            this.nodeRelativeStartTime.ParentColumn = this.columnRelativeStartTime;
            this.nodeRelativeStartTime.Trimming = System.Drawing.StringTrimming.EllipsisCharacter;
            // 
            // nodeTotalCpuTime
            // 
            this.nodeTotalCpuTime.DataPropertyName = "TotalCpuTime";
            this.nodeTotalCpuTime.EditEnabled = false;
            this.nodeTotalCpuTime.IncrementalSearchEnabled = true;
            this.nodeTotalCpuTime.LeftMargin = 3;
            this.nodeTotalCpuTime.ParentColumn = this.columnTotalCpuTime;
            this.nodeTotalCpuTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nodeTotalCpuTime.Trimming = System.Drawing.StringTrimming.EllipsisCharacter;
            // 
            // nodeKernelCpuTime
            // 
            this.nodeKernelCpuTime.DataPropertyName = "KernelCpuTime";
            this.nodeKernelCpuTime.EditEnabled = false;
            this.nodeKernelCpuTime.IncrementalSearchEnabled = true;
            this.nodeKernelCpuTime.LeftMargin = 3;
            this.nodeKernelCpuTime.ParentColumn = this.columnKernelCpuTime;
            this.nodeKernelCpuTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nodeKernelCpuTime.Trimming = System.Drawing.StringTrimming.EllipsisCharacter;
            // 
            // nodeUserCpuTime
            // 
            this.nodeUserCpuTime.DataPropertyName = "UserCpuTime";
            this.nodeUserCpuTime.EditEnabled = false;
            this.nodeUserCpuTime.IncrementalSearchEnabled = true;
            this.nodeUserCpuTime.LeftMargin = 3;
            this.nodeUserCpuTime.ParentColumn = this.columnUserCpuTime;
            this.nodeUserCpuTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nodeUserCpuTime.Trimming = System.Drawing.StringTrimming.EllipsisCharacter;
            // 
            // nodeVerificationStatus
            // 
            this.nodeVerificationStatus.DataPropertyName = "VerificationStatus";
            this.nodeVerificationStatus.EditEnabled = false;
            this.nodeVerificationStatus.IncrementalSearchEnabled = true;
            this.nodeVerificationStatus.LeftMargin = 3;
            this.nodeVerificationStatus.ParentColumn = this.columnVerificationStatus;
            this.nodeVerificationStatus.Trimming = System.Drawing.StringTrimming.EllipsisCharacter;
            // 
            // nodeVerifiedSigner
            // 
            this.nodeVerifiedSigner.DataPropertyName = "VerifiedSigner";
            this.nodeVerifiedSigner.EditEnabled = false;
            this.nodeVerifiedSigner.IncrementalSearchEnabled = true;
            this.nodeVerifiedSigner.LeftMargin = 3;
            this.nodeVerifiedSigner.ParentColumn = this.columnVerifiedSigner;
            this.nodeVerifiedSigner.Trimming = System.Drawing.StringTrimming.EllipsisCharacter;
            // 
            // columnVerifiedSigner
            // 
            this.columnVerifiedSigner.Header = "Verified Signer";
            this.columnVerifiedSigner.IsVisible = false;
            this.columnVerifiedSigner.Sortable = true;
            this.columnVerifiedSigner.SortOrder = System.Windows.Forms.SortOrder.None;
            this.columnVerifiedSigner.TooltipText = null;
            this.columnVerifiedSigner.Width = 140;
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
        private Aga.Controls.Tree.NodeControls.NodeIntegerTextBox nodePid;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodePvtMemory;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeCpu;
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
        private Aga.Controls.Tree.TreeColumn columnPriorityClass;
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
        private Aga.Controls.Tree.TreeColumn columnIoTotal;
        private Aga.Controls.Tree.TreeColumn columnIoReadOther;
        private Aga.Controls.Tree.TreeColumn columnIoWrite;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeIoTotal;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeIoReadOther;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeIoWrite;
        private Aga.Controls.Tree.TreeColumn columnPrivateWorkingSet;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodePrivateWorkingSet;
        private Aga.Controls.Tree.TreeColumn columnSharedWorkingSet;
        private Aga.Controls.Tree.TreeColumn columnShareableWorkingSet;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeSharedWorkingSet;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeShareableWorkingSet;
        private Aga.Controls.Tree.TreeColumn columnIntegrity;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeIntegrity;
        private Aga.Controls.Tree.TreeColumn columnIoPriority;
        private Aga.Controls.Tree.TreeColumn columnPagePriority;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeIoPriority;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodePagePriority;
        private Aga.Controls.Tree.TreeColumn columnBasePriority;
        private Aga.Controls.Tree.TreeColumn columnStartTime;
        private Aga.Controls.Tree.TreeColumn columnTotalCpuTime;
        private Aga.Controls.Tree.TreeColumn columnUserCpuTime;
        private Aga.Controls.Tree.TreeColumn columnKernelCpuTime;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodePriorityClass;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeStartTime;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeTotalCpuTime;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeKernelCpuTime;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeUserCpuTime;
        private Aga.Controls.Tree.TreeColumn columnRelativeStartTime;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeRelativeStartTime;
        private Aga.Controls.Tree.TreeColumn columnVerificationStatus;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeVerificationStatus;
        private Aga.Controls.Tree.TreeColumn columnVerifiedSigner;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeVerifiedSigner;
    }
}
