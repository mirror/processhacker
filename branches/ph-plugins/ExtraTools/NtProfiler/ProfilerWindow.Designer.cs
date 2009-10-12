namespace NtProfiler
{
    partial class ProfilerWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProfilerWindow));
            this.menuStripMain = new System.Windows.Forms.MenuStrip();
            this.profilerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.profileProcessToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.profileKernelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabModules = new System.Windows.Forms.TabPage();
            this.listModules = new System.Windows.Forms.ListView();
            this.columnName = new System.Windows.Forms.ColumnHeader();
            this.columnHits = new System.Windows.Forms.ColumnHeader();
            this.columnFileName = new System.Windows.Forms.ColumnHeader();
            this.tabFunctions = new System.Windows.Forms.TabPage();
            this.toolStripContainer = new System.Windows.Forms.ToolStripContainer();
            this.toolStripProfileControl = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonStart = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonStop = new System.Windows.Forms.ToolStripButton();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listFunctions = new System.Windows.Forms.ListView();
            this.columnFunction = new System.Windows.Forms.ColumnHeader();
            this.columnFunctionHits = new System.Windows.Forms.ColumnHeader();
            this.menuStripMain.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabModules.SuspendLayout();
            this.tabFunctions.SuspendLayout();
            this.toolStripContainer.ContentPanel.SuspendLayout();
            this.toolStripContainer.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer.SuspendLayout();
            this.toolStripProfileControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStripMain
            // 
            this.menuStripMain.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.profilerToolStripMenuItem});
            this.menuStripMain.Location = new System.Drawing.Point(0, 0);
            this.menuStripMain.Name = "menuStripMain";
            this.menuStripMain.Size = new System.Drawing.Size(661, 24);
            this.menuStripMain.TabIndex = 0;
            this.menuStripMain.Text = "menuStrip1";
            // 
            // profilerToolStripMenuItem
            // 
            this.profilerToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.profileProcessToolStripMenuItem,
            this.profileKernelToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem});
            this.profilerToolStripMenuItem.Name = "profilerToolStripMenuItem";
            this.profilerToolStripMenuItem.ShowShortcutKeys = false;
            this.profilerToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.profilerToolStripMenuItem.Text = "&Profiler";
            // 
            // profileProcessToolStripMenuItem
            // 
            this.profileProcessToolStripMenuItem.Name = "profileProcessToolStripMenuItem";
            this.profileProcessToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.profileProcessToolStripMenuItem.Text = "Profile &Process...";
            this.profileProcessToolStripMenuItem.Click += new System.EventHandler(this.profileProcessToolStripMenuItem_Click);
            // 
            // profileKernelToolStripMenuItem
            // 
            this.profileKernelToolStripMenuItem.Name = "profileKernelToolStripMenuItem";
            this.profileKernelToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.profileKernelToolStripMenuItem.Text = "Profile &Kernel";
            this.profileKernelToolStripMenuItem.Click += new System.EventHandler(this.profileKernelToolStripMenuItem_Click);
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabModules);
            this.tabControl.Controls.Add(this.tabFunctions);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(661, 404);
            this.tabControl.TabIndex = 1;
            // 
            // tabModules
            // 
            this.tabModules.Controls.Add(this.listModules);
            this.tabModules.Location = new System.Drawing.Point(4, 22);
            this.tabModules.Name = "tabModules";
            this.tabModules.Padding = new System.Windows.Forms.Padding(3);
            this.tabModules.Size = new System.Drawing.Size(653, 378);
            this.tabModules.TabIndex = 0;
            this.tabModules.Text = "Modules";
            this.tabModules.UseVisualStyleBackColor = true;
            // 
            // listModules
            // 
            this.listModules.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnName,
            this.columnHits,
            this.columnFileName});
            this.listModules.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listModules.FullRowSelect = true;
            this.listModules.HideSelection = false;
            this.listModules.Location = new System.Drawing.Point(3, 3);
            this.listModules.Name = "listModules";
            this.listModules.ShowItemToolTips = true;
            this.listModules.Size = new System.Drawing.Size(647, 372);
            this.listModules.TabIndex = 0;
            this.listModules.UseCompatibleStateImageBehavior = false;
            this.listModules.View = System.Windows.Forms.View.Details;
            this.listModules.DoubleClick += new System.EventHandler(this.listModules_DoubleClick);
            // 
            // columnName
            // 
            this.columnName.Text = "Name";
            this.columnName.Width = 160;
            // 
            // columnHits
            // 
            this.columnHits.Text = "Hits";
            this.columnHits.Width = 100;
            // 
            // columnFileName
            // 
            this.columnFileName.Text = "File Name";
            this.columnFileName.Width = 300;
            // 
            // tabFunctions
            // 
            this.tabFunctions.Controls.Add(this.listFunctions);
            this.tabFunctions.Location = new System.Drawing.Point(4, 22);
            this.tabFunctions.Name = "tabFunctions";
            this.tabFunctions.Padding = new System.Windows.Forms.Padding(3);
            this.tabFunctions.Size = new System.Drawing.Size(653, 378);
            this.tabFunctions.TabIndex = 1;
            this.tabFunctions.Text = "Functions";
            this.tabFunctions.UseVisualStyleBackColor = true;
            // 
            // toolStripContainer
            // 
            // 
            // toolStripContainer.ContentPanel
            // 
            this.toolStripContainer.ContentPanel.Controls.Add(this.tabControl);
            this.toolStripContainer.ContentPanel.Size = new System.Drawing.Size(661, 404);
            this.toolStripContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer.Name = "toolStripContainer";
            this.toolStripContainer.Size = new System.Drawing.Size(661, 453);
            this.toolStripContainer.TabIndex = 1;
            this.toolStripContainer.Text = "toolStripContainer1";
            // 
            // toolStripContainer.TopToolStripPanel
            // 
            this.toolStripContainer.TopToolStripPanel.Controls.Add(this.menuStripMain);
            this.toolStripContainer.TopToolStripPanel.Controls.Add(this.toolStripProfileControl);
            // 
            // toolStripProfileControl
            // 
            this.toolStripProfileControl.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStripProfileControl.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonStart,
            this.toolStripButtonStop});
            this.toolStripProfileControl.Location = new System.Drawing.Point(3, 24);
            this.toolStripProfileControl.Name = "toolStripProfileControl";
            this.toolStripProfileControl.Size = new System.Drawing.Size(58, 25);
            this.toolStripProfileControl.TabIndex = 1;
            // 
            // toolStripButtonStart
            // 
            this.toolStripButtonStart.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonStart.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonStart.Image")));
            this.toolStripButtonStart.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonStart.Name = "toolStripButtonStart";
            this.toolStripButtonStart.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonStart.Text = "Start";
            this.toolStripButtonStart.Click += new System.EventHandler(this.toolStripButtonStart_Click);
            // 
            // toolStripButtonStop
            // 
            this.toolStripButtonStop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonStop.Enabled = false;
            this.toolStripButtonStop.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonStop.Image")));
            this.toolStripButtonStop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonStop.Name = "toolStripButtonStop";
            this.toolStripButtonStop.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonStop.Text = "Stop";
            this.toolStripButtonStop.Click += new System.EventHandler(this.toolStripButtonStop_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(157, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // listFunctions
            // 
            this.listFunctions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnFunction,
            this.columnFunctionHits});
            this.listFunctions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listFunctions.FullRowSelect = true;
            this.listFunctions.HideSelection = false;
            this.listFunctions.Location = new System.Drawing.Point(3, 3);
            this.listFunctions.Name = "listFunctions";
            this.listFunctions.ShowItemToolTips = true;
            this.listFunctions.Size = new System.Drawing.Size(647, 372);
            this.listFunctions.TabIndex = 1;
            this.listFunctions.UseCompatibleStateImageBehavior = false;
            this.listFunctions.View = System.Windows.Forms.View.Details;
            // 
            // columnFunction
            // 
            this.columnFunction.Text = "Function";
            this.columnFunction.Width = 300;
            // 
            // columnFunctionHits
            // 
            this.columnFunctionHits.Text = "Hits";
            this.columnFunctionHits.Width = 100;
            // 
            // ProfilerWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(661, 453);
            this.Controls.Add(this.toolStripContainer);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStripMain;
            this.Name = "ProfilerWindow";
            this.Text = "NtProfiler";
            this.menuStripMain.ResumeLayout(false);
            this.menuStripMain.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.tabModules.ResumeLayout(false);
            this.tabFunctions.ResumeLayout(false);
            this.toolStripContainer.ContentPanel.ResumeLayout(false);
            this.toolStripContainer.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer.TopToolStripPanel.PerformLayout();
            this.toolStripContainer.ResumeLayout(false);
            this.toolStripContainer.PerformLayout();
            this.toolStripProfileControl.ResumeLayout(false);
            this.toolStripProfileControl.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStripMain;
        private System.Windows.Forms.ToolStripMenuItem profilerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem profileProcessToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem profileKernelToolStripMenuItem;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabModules;
        private System.Windows.Forms.TabPage tabFunctions;
        private System.Windows.Forms.ListView listModules;
        private System.Windows.Forms.ColumnHeader columnName;
        private System.Windows.Forms.ColumnHeader columnHits;
        private System.Windows.Forms.ColumnHeader columnFileName;
        private System.Windows.Forms.ToolStripContainer toolStripContainer;
        private System.Windows.Forms.ToolStrip toolStripProfileControl;
        private System.Windows.Forms.ToolStripButton toolStripButtonStart;
        private System.Windows.Forms.ToolStripButton toolStripButtonStop;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ListView listFunctions;
        private System.Windows.Forms.ColumnHeader columnFunction;
        private System.Windows.Forms.ColumnHeader columnFunctionHits;
    }
}

