namespace ProcessAnalyzer
{
    partial class MainWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.analyzerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openProcessToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabHandleTracing = new System.Windows.Forms.TabPage();
            this.listHandleStack = new System.Windows.Forms.ListView();
            this.columnAddress = new System.Windows.Forms.ColumnHeader();
            this.columnSymbol = new System.Windows.Forms.ColumnHeader();
            this.listHandleTraces = new System.Windows.Forms.ListView();
            this.columnIndex = new System.Windows.Forms.ColumnHeader();
            this.columnHandle = new System.Windows.Forms.ColumnHeader();
            this.columnType = new System.Windows.Forms.ColumnHeader();
            this.columnTid = new System.Windows.Forms.ColumnHeader();
            this.columnHandleName = new System.Windows.Forms.ColumnHeader();
            this.buttonSnapshot = new System.Windows.Forms.Button();
            this.buttonDisableHandleTracing = new System.Windows.Forms.Button();
            this.buttonEnableHandleTracing = new System.Windows.Forms.Button();
            this.tabHiddenObjects = new System.Windows.Forms.TabPage();
            this.labelObjectsScanProgress = new System.Windows.Forms.Label();
            this.buttonScanHiddenObjects = new System.Windows.Forms.Button();
            this.listHiddenObjects = new System.Windows.Forms.ListView();
            this.columnObjectType = new System.Windows.Forms.ColumnHeader();
            this.columnObjectId = new System.Windows.Forms.ColumnHeader();
            this.columnObjectInfo = new System.Windows.Forms.ColumnHeader();
            this.menuStrip.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabHandleTracing.SuspendLayout();
            this.tabHiddenObjects.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.analyzerToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(695, 24);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip1";
            // 
            // analyzerToolStripMenuItem
            // 
            this.analyzerToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openProcessToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem});
            this.analyzerToolStripMenuItem.Name = "analyzerToolStripMenuItem";
            this.analyzerToolStripMenuItem.Size = new System.Drawing.Size(64, 20);
            this.analyzerToolStripMenuItem.Text = "Analyzer";
            // 
            // openProcessToolStripMenuItem
            // 
            this.openProcessToolStripMenuItem.Name = "openProcessToolStripMenuItem";
            this.openProcessToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openProcessToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.openProcessToolStripMenuItem.Text = "&Open Process...";
            this.openProcessToolStripMenuItem.Click += new System.EventHandler(this.openProcessToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(195, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabHandleTracing);
            this.tabControl.Controls.Add(this.tabHiddenObjects);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 24);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(695, 469);
            this.tabControl.TabIndex = 1;
            // 
            // tabHandleTracing
            // 
            this.tabHandleTracing.Controls.Add(this.listHandleStack);
            this.tabHandleTracing.Controls.Add(this.listHandleTraces);
            this.tabHandleTracing.Controls.Add(this.buttonSnapshot);
            this.tabHandleTracing.Controls.Add(this.buttonDisableHandleTracing);
            this.tabHandleTracing.Controls.Add(this.buttonEnableHandleTracing);
            this.tabHandleTracing.Location = new System.Drawing.Point(4, 22);
            this.tabHandleTracing.Name = "tabHandleTracing";
            this.tabHandleTracing.Padding = new System.Windows.Forms.Padding(3);
            this.tabHandleTracing.Size = new System.Drawing.Size(687, 443);
            this.tabHandleTracing.TabIndex = 0;
            this.tabHandleTracing.Text = "Handle Tracing";
            this.tabHandleTracing.UseVisualStyleBackColor = true;
            // 
            // listHandleStack
            // 
            this.listHandleStack.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listHandleStack.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnAddress,
            this.columnSymbol});
            this.listHandleStack.FullRowSelect = true;
            this.listHandleStack.HideSelection = false;
            this.listHandleStack.Location = new System.Drawing.Point(6, 226);
            this.listHandleStack.MultiSelect = false;
            this.listHandleStack.Name = "listHandleStack";
            this.listHandleStack.Size = new System.Drawing.Size(675, 211);
            this.listHandleStack.TabIndex = 1;
            this.listHandleStack.UseCompatibleStateImageBehavior = false;
            this.listHandleStack.View = System.Windows.Forms.View.Details;
            // 
            // columnAddress
            // 
            this.columnAddress.Text = "Address";
            this.columnAddress.Width = 100;
            // 
            // columnSymbol
            // 
            this.columnSymbol.Text = "Symbol";
            this.columnSymbol.Width = 300;
            // 
            // listHandleTraces
            // 
            this.listHandleTraces.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listHandleTraces.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnIndex,
            this.columnHandle,
            this.columnType,
            this.columnTid,
            this.columnHandleName});
            this.listHandleTraces.FullRowSelect = true;
            this.listHandleTraces.HideSelection = false;
            this.listHandleTraces.Location = new System.Drawing.Point(6, 35);
            this.listHandleTraces.MultiSelect = false;
            this.listHandleTraces.Name = "listHandleTraces";
            this.listHandleTraces.Size = new System.Drawing.Size(675, 185);
            this.listHandleTraces.TabIndex = 1;
            this.listHandleTraces.UseCompatibleStateImageBehavior = false;
            this.listHandleTraces.View = System.Windows.Forms.View.Details;
            this.listHandleTraces.SelectedIndexChanged += new System.EventHandler(this.listHandleTraces_SelectedIndexChanged);
            // 
            // columnIndex
            // 
            this.columnIndex.Text = "Index";
            // 
            // columnHandle
            // 
            this.columnHandle.Text = "Handle";
            // 
            // columnType
            // 
            this.columnType.Text = "Type";
            this.columnType.Width = 120;
            // 
            // columnTid
            // 
            this.columnTid.Text = "TID";
            // 
            // columnHandleName
            // 
            this.columnHandleName.Text = "Handle Name";
            this.columnHandleName.Width = 300;
            // 
            // buttonSnapshot
            // 
            this.buttonSnapshot.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonSnapshot.Location = new System.Drawing.Point(168, 6);
            this.buttonSnapshot.Name = "buttonSnapshot";
            this.buttonSnapshot.Size = new System.Drawing.Size(75, 23);
            this.buttonSnapshot.TabIndex = 0;
            this.buttonSnapshot.Text = "Snapshot";
            this.buttonSnapshot.UseVisualStyleBackColor = true;
            this.buttonSnapshot.Click += new System.EventHandler(this.buttonSnapshot_Click);
            // 
            // buttonDisableHandleTracing
            // 
            this.buttonDisableHandleTracing.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonDisableHandleTracing.Location = new System.Drawing.Point(87, 6);
            this.buttonDisableHandleTracing.Name = "buttonDisableHandleTracing";
            this.buttonDisableHandleTracing.Size = new System.Drawing.Size(75, 23);
            this.buttonDisableHandleTracing.TabIndex = 0;
            this.buttonDisableHandleTracing.Text = "Disable";
            this.buttonDisableHandleTracing.UseVisualStyleBackColor = true;
            this.buttonDisableHandleTracing.Click += new System.EventHandler(this.buttonDisableHandleTracing_Click);
            // 
            // buttonEnableHandleTracing
            // 
            this.buttonEnableHandleTracing.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonEnableHandleTracing.Location = new System.Drawing.Point(6, 6);
            this.buttonEnableHandleTracing.Name = "buttonEnableHandleTracing";
            this.buttonEnableHandleTracing.Size = new System.Drawing.Size(75, 23);
            this.buttonEnableHandleTracing.TabIndex = 0;
            this.buttonEnableHandleTracing.Text = "Enable";
            this.buttonEnableHandleTracing.UseVisualStyleBackColor = true;
            this.buttonEnableHandleTracing.Click += new System.EventHandler(this.buttonEnableHandleTracing_Click);
            // 
            // tabHiddenObjects
            // 
            this.tabHiddenObjects.Controls.Add(this.labelObjectsScanProgress);
            this.tabHiddenObjects.Controls.Add(this.buttonScanHiddenObjects);
            this.tabHiddenObjects.Controls.Add(this.listHiddenObjects);
            this.tabHiddenObjects.Location = new System.Drawing.Point(4, 22);
            this.tabHiddenObjects.Name = "tabHiddenObjects";
            this.tabHiddenObjects.Padding = new System.Windows.Forms.Padding(3);
            this.tabHiddenObjects.Size = new System.Drawing.Size(687, 443);
            this.tabHiddenObjects.TabIndex = 1;
            this.tabHiddenObjects.Text = "Hidden Objects";
            this.tabHiddenObjects.UseVisualStyleBackColor = true;
            // 
            // labelObjectsScanProgress
            // 
            this.labelObjectsScanProgress.AutoSize = true;
            this.labelObjectsScanProgress.Location = new System.Drawing.Point(87, 11);
            this.labelObjectsScanProgress.Name = "labelObjectsScanProgress";
            this.labelObjectsScanProgress.Size = new System.Drawing.Size(41, 13);
            this.labelObjectsScanProgress.TabIndex = 4;
            this.labelObjectsScanProgress.Text = "Ready.";
            // 
            // buttonScanHiddenObjects
            // 
            this.buttonScanHiddenObjects.Enabled = false;
            this.buttonScanHiddenObjects.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonScanHiddenObjects.Location = new System.Drawing.Point(6, 6);
            this.buttonScanHiddenObjects.Name = "buttonScanHiddenObjects";
            this.buttonScanHiddenObjects.Size = new System.Drawing.Size(75, 23);
            this.buttonScanHiddenObjects.TabIndex = 3;
            this.buttonScanHiddenObjects.Text = "Scan";
            this.buttonScanHiddenObjects.UseVisualStyleBackColor = true;
            this.buttonScanHiddenObjects.Click += new System.EventHandler(this.buttonScanHiddenObjects_Click);
            // 
            // listHiddenObjects
            // 
            this.listHiddenObjects.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listHiddenObjects.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnObjectType,
            this.columnObjectId,
            this.columnObjectInfo});
            this.listHiddenObjects.FullRowSelect = true;
            this.listHiddenObjects.HideSelection = false;
            this.listHiddenObjects.Location = new System.Drawing.Point(6, 35);
            this.listHiddenObjects.MultiSelect = false;
            this.listHiddenObjects.Name = "listHiddenObjects";
            this.listHiddenObjects.Size = new System.Drawing.Size(675, 402);
            this.listHiddenObjects.TabIndex = 2;
            this.listHiddenObjects.UseCompatibleStateImageBehavior = false;
            this.listHiddenObjects.View = System.Windows.Forms.View.Details;
            // 
            // columnObjectType
            // 
            this.columnObjectType.Text = "Type";
            this.columnObjectType.Width = 100;
            // 
            // columnObjectId
            // 
            this.columnObjectId.Text = "ID";
            // 
            // columnObjectInfo
            // 
            this.columnObjectInfo.Text = "Information";
            this.columnObjectInfo.Width = 300;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(695, 493);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.menuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip;
            this.Name = "MainWindow";
            this.Text = "Process Analyzer";
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.tabHandleTracing.ResumeLayout(false);
            this.tabHiddenObjects.ResumeLayout(false);
            this.tabHiddenObjects.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem analyzerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openProcessToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabHandleTracing;
        private System.Windows.Forms.Button buttonDisableHandleTracing;
        private System.Windows.Forms.Button buttonEnableHandleTracing;
        private System.Windows.Forms.Button buttonSnapshot;
        private System.Windows.Forms.ListView listHandleTraces;
        private System.Windows.Forms.ColumnHeader columnHandle;
        private System.Windows.Forms.ColumnHeader columnType;
        private System.Windows.Forms.ColumnHeader columnTid;
        private System.Windows.Forms.ListView listHandleStack;
        private System.Windows.Forms.ColumnHeader columnAddress;
        private System.Windows.Forms.ColumnHeader columnSymbol;
        private System.Windows.Forms.ColumnHeader columnHandleName;
        private System.Windows.Forms.ColumnHeader columnIndex;
        private System.Windows.Forms.TabPage tabHiddenObjects;
        private System.Windows.Forms.ListView listHiddenObjects;
        private System.Windows.Forms.ColumnHeader columnObjectType;
        private System.Windows.Forms.ColumnHeader columnObjectId;
        private System.Windows.Forms.ColumnHeader columnObjectInfo;
        private System.Windows.Forms.Button buttonScanHiddenObjects;
        private System.Windows.Forms.Label labelObjectsScanProgress;
    }
}

