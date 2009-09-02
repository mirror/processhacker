namespace ProcessHacker
{
    partial class HackerWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HackerWindow));
            this.menuProcess = new System.Windows.Forms.ContextMenu();
            this.toolStripMenuItem9 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem10 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem11 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem12 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem13 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem14 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem15 = new System.Windows.Forms.ToolStripMenuItem();
            this.statusBar = new System.Windows.Forms.StatusBar();
            this.statusGeneral = new System.Windows.Forms.StatusBarPanel();
            this.statusCPU = new System.Windows.Forms.StatusBarPanel();
            this.statusMemory = new System.Windows.Forms.StatusBarPanel();
            this.statusIcon = new System.Windows.Forms.StatusBarPanel();
            this.statusText = new System.Windows.Forms.StatusBarPanel();
            this.timerMessages = new System.Windows.Forms.Timer(this.components);
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabProcesses = new System.Windows.Forms.TabPage();
            this.treeProcesses = new ProcessHacker.ProcessTree();
            this.tabServices = new System.Windows.Forms.TabPage();
            this.listServices = new ProcessHacker.Components.ServiceList();
            this.tabNetwork = new System.Windows.Forms.TabPage();
            this.listNetwork = new ProcessHacker.Components.NetworkList();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.refreshToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.optionsToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.shutDownToolStripMenuItem = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.findHandlesToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.sysInfoToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.menuService = new System.Windows.Forms.ContextMenu();
            this.menuIcon = new System.Windows.Forms.ContextMenu();
            this.menuNetwork = new System.Windows.Forms.ContextMenu();
            this.vistaMenu = new wyDay.Controls.VistaMenu(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.statusGeneral)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusCPU)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusMemory)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusText)).BeginInit();
            this.tabControl.SuspendLayout();
            this.tabProcesses.SuspendLayout();
            this.tabServices.SuspendLayout();
            this.tabNetwork.SuspendLayout();
            this.toolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.vistaMenu)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStripMenuItem9
            // 
            this.toolStripMenuItem9.Name = "toolStripMenuItem9";
            this.toolStripMenuItem9.Size = new System.Drawing.Size(151, 22);
            this.toolStripMenuItem9.Text = "Time Critical";
            // 
            // toolStripMenuItem10
            // 
            this.toolStripMenuItem10.Name = "toolStripMenuItem10";
            this.toolStripMenuItem10.Size = new System.Drawing.Size(151, 22);
            this.toolStripMenuItem10.Text = "Highest";
            // 
            // toolStripMenuItem11
            // 
            this.toolStripMenuItem11.Name = "toolStripMenuItem11";
            this.toolStripMenuItem11.Size = new System.Drawing.Size(151, 22);
            this.toolStripMenuItem11.Text = "Above Normal";
            // 
            // toolStripMenuItem12
            // 
            this.toolStripMenuItem12.Name = "toolStripMenuItem12";
            this.toolStripMenuItem12.Size = new System.Drawing.Size(151, 22);
            this.toolStripMenuItem12.Text = "Normal";
            // 
            // toolStripMenuItem13
            // 
            this.toolStripMenuItem13.Name = "toolStripMenuItem13";
            this.toolStripMenuItem13.Size = new System.Drawing.Size(151, 22);
            this.toolStripMenuItem13.Text = "Below Normal";
            // 
            // toolStripMenuItem14
            // 
            this.toolStripMenuItem14.Name = "toolStripMenuItem14";
            this.toolStripMenuItem14.Size = new System.Drawing.Size(151, 22);
            this.toolStripMenuItem14.Text = "Lowest";
            // 
            // toolStripMenuItem15
            // 
            this.toolStripMenuItem15.Name = "toolStripMenuItem15";
            this.toolStripMenuItem15.Size = new System.Drawing.Size(151, 22);
            this.toolStripMenuItem15.Text = "Idle";
            // 
            // statusBar
            // 
            this.statusBar.Location = new System.Drawing.Point(0, 236);
            this.statusBar.Name = "statusBar";
            this.statusBar.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
            this.statusGeneral,
            this.statusCPU,
            this.statusMemory,
            this.statusIcon,
            this.statusText});
            this.statusBar.ShowPanels = true;
            this.statusBar.Size = new System.Drawing.Size(804, 22);
            this.statusBar.TabIndex = 5;
            // 
            // statusGeneral
            // 
            this.statusGeneral.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
            this.statusGeneral.Name = "statusGeneral";
            this.statusGeneral.Width = 10;
            // 
            // statusCPU
            // 
            this.statusCPU.Name = "statusCPU";
            this.statusCPU.Text = "CPU: 99.99%";
            this.statusCPU.Width = 80;
            // 
            // statusMemory
            // 
            this.statusMemory.Name = "statusMemory";
            this.statusMemory.Text = "Phys. Memory: 50%";
            this.statusMemory.Width = 120;
            // 
            // statusIcon
            // 
            this.statusIcon.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
            this.statusIcon.BorderStyle = System.Windows.Forms.StatusBarPanelBorderStyle.None;
            this.statusIcon.MinWidth = 0;
            this.statusIcon.Name = "statusIcon";
            this.statusIcon.Width = 10;
            // 
            // statusText
            // 
            this.statusText.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
            this.statusText.BorderStyle = System.Windows.Forms.StatusBarPanelBorderStyle.None;
            this.statusText.Name = "statusText";
            this.statusText.Width = 10;
            // 
            // timerMessages
            // 
            this.timerMessages.Interval = 2000;
            this.timerMessages.Tick += new System.EventHandler(this.timerMessages_Tick);
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabProcesses);
            this.tabControl.Controls.Add(this.tabServices);
            this.tabControl.Controls.Add(this.tabNetwork);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 25);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(804, 211);
            this.tabControl.TabIndex = 6;
            this.tabControl.SelectedIndexChanged += new System.EventHandler(this.tabControlBig_SelectedIndexChanged);
            // 
            // tabProcesses
            // 
            this.tabProcesses.Controls.Add(this.treeProcesses);
            this.tabProcesses.Location = new System.Drawing.Point(4, 22);
            this.tabProcesses.Name = "tabProcesses";
            this.tabProcesses.Padding = new System.Windows.Forms.Padding(3);
            this.tabProcesses.Size = new System.Drawing.Size(796, 185);
            this.tabProcesses.TabIndex = 0;
            this.tabProcesses.Text = "Processes";
            this.tabProcesses.UseVisualStyleBackColor = true;
            // 
            // treeProcesses
            // 
            this.treeProcesses.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeProcesses.Draw = true;
            this.treeProcesses.Location = new System.Drawing.Point(3, 3);
            this.treeProcesses.Name = "treeProcesses";
            this.treeProcesses.Provider = null;
            this.treeProcesses.Size = new System.Drawing.Size(790, 179);
            this.treeProcesses.TabIndex = 4;
            this.treeProcesses.SelectionChanged += new System.EventHandler(this.treeProcesses_SelectionChanged);
            this.treeProcesses.NodeMouseDoubleClick += new System.EventHandler<Aga.Controls.Tree.TreeNodeAdvMouseEventArgs>(this.treeProcesses_NodeMouseDoubleClick);
            this.treeProcesses.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeProcesses_KeyDown);
            // 
            // tabServices
            // 
            this.tabServices.Controls.Add(this.listServices);
            this.tabServices.Location = new System.Drawing.Point(4, 22);
            this.tabServices.Name = "tabServices";
            this.tabServices.Padding = new System.Windows.Forms.Padding(3);
            this.tabServices.Size = new System.Drawing.Size(796, 185);
            this.tabServices.TabIndex = 1;
            this.tabServices.Text = "Services";
            this.tabServices.UseVisualStyleBackColor = true;
            // 
            // listServices
            // 
            this.listServices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listServices.DoubleBuffered = true;
            this.listServices.Location = new System.Drawing.Point(3, 3);
            this.listServices.Name = "listServices";
            this.listServices.Provider = null;
            this.listServices.Size = new System.Drawing.Size(790, 179);
            this.listServices.TabIndex = 0;
            this.listServices.DoubleClick += new System.EventHandler(this.listServices_DoubleClick);
            this.listServices.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listServices_KeyDown);
            // 
            // tabNetwork
            // 
            this.tabNetwork.Controls.Add(this.listNetwork);
            this.tabNetwork.Location = new System.Drawing.Point(4, 22);
            this.tabNetwork.Name = "tabNetwork";
            this.tabNetwork.Padding = new System.Windows.Forms.Padding(3);
            this.tabNetwork.Size = new System.Drawing.Size(796, 185);
            this.tabNetwork.TabIndex = 2;
            this.tabNetwork.Text = "Network";
            this.tabNetwork.UseVisualStyleBackColor = true;
            // 
            // listNetwork
            // 
            this.listNetwork.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listNetwork.DoubleBuffered = true;
            this.listNetwork.Location = new System.Drawing.Point(3, 3);
            this.listNetwork.Name = "listNetwork";
            this.listNetwork.Provider = null;
            this.listNetwork.Size = new System.Drawing.Size(790, 179);
            this.listNetwork.TabIndex = 0;
            this.listNetwork.DoubleClick += new System.EventHandler(this.listNetwork_DoubleClick);
            this.listNetwork.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listNetwork_KeyDown);
            // 
            // toolStrip
            // 
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.refreshToolStripButton,
            this.optionsToolStripButton,
            this.shutDownToolStripMenuItem,
            this.toolStripSeparator1,
            this.findHandlesToolStripButton,
            this.sysInfoToolStripButton});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(804, 25);
            this.toolStrip.TabIndex = 5;
            this.toolStrip.Text = "toolStrip1";
            // 
            // refreshToolStripButton
            // 
            this.refreshToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.refreshToolStripButton.Image = global::ProcessHacker.Properties.Resources.arrow_refresh;
            this.refreshToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.refreshToolStripButton.Name = "refreshToolStripButton";
            this.refreshToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.refreshToolStripButton.Text = "Refresh";
            this.refreshToolStripButton.ToolTipText = "Refresh (F5)";
            this.refreshToolStripButton.Click += new System.EventHandler(this.refreshToolStripButton_Click);
            // 
            // optionsToolStripButton
            // 
            this.optionsToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.optionsToolStripButton.Image = global::ProcessHacker.Properties.Resources.cog_edit;
            this.optionsToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.optionsToolStripButton.Name = "optionsToolStripButton";
            this.optionsToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.optionsToolStripButton.Text = "Options";
            this.optionsToolStripButton.ToolTipText = "Options... (Ctrl+O)";
            this.optionsToolStripButton.Click += new System.EventHandler(this.optionsToolStripButton_Click);
            // 
            // shutDownToolStripMenuItem
            // 
            this.shutDownToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.shutDownToolStripMenuItem.Image = global::ProcessHacker.Properties.Resources.lightbulb_off;
            this.shutDownToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.shutDownToolStripMenuItem.Name = "shutDownToolStripMenuItem";
            this.shutDownToolStripMenuItem.Size = new System.Drawing.Size(29, 22);
            this.shutDownToolStripMenuItem.Text = "Shutdown";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // findHandlesToolStripButton
            // 
            this.findHandlesToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.findHandlesToolStripButton.Image = global::ProcessHacker.Properties.Resources.find;
            this.findHandlesToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.findHandlesToolStripButton.Name = "findHandlesToolStripButton";
            this.findHandlesToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.findHandlesToolStripButton.Text = "Find Handles or DLLs...";
            this.findHandlesToolStripButton.ToolTipText = "Find Handles or DLLs... (Ctrl+F)";
            this.findHandlesToolStripButton.Click += new System.EventHandler(this.findHandlesToolStripButton_Click);
            // 
            // sysInfoToolStripButton
            // 
            this.sysInfoToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.sysInfoToolStripButton.Image = global::ProcessHacker.Properties.Resources.chart_line;
            this.sysInfoToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.sysInfoToolStripButton.Name = "sysInfoToolStripButton";
            this.sysInfoToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.sysInfoToolStripButton.Text = "System Information...";
            this.sysInfoToolStripButton.ToolTipText = "System Information... (Ctrl+I)";
            this.sysInfoToolStripButton.Click += new System.EventHandler(this.sysInfoToolStripButton_Click);
            // 
            // vistaMenu
            // 
            this.vistaMenu.ContainerControl = this;
            this.vistaMenu.DelaySetImageCalls = false;
            // 
            // HackerWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(804, 258);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.statusBar);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "HackerWindow";
            this.Text = "Process Hacker";
            this.Load += new System.EventHandler(this.HackerWindow_Load);
            this.SizeChanged += new System.EventHandler(this.HackerWindow_SizeChanged);
            this.VisibleChanged += new System.EventHandler(this.HackerWindow_VisibleChanged);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.HackerWindow_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.statusGeneral)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusCPU)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusMemory)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusText)).EndInit();
            this.tabControl.ResumeLayout(false);
            this.tabProcesses.ResumeLayout(false);
            this.tabServices.ResumeLayout(false);
            this.tabNetwork.ResumeLayout(false);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.vistaMenu)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ContextMenu menuProcess;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem9;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem10;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem11;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem12;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem13;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem14;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem15;
        private wyDay.Controls.VistaMenu vistaMenu;
        private ProcessHacker.ProcessTree treeProcesses;
        private System.Windows.Forms.StatusBar statusBar;
        private System.Windows.Forms.Timer timerMessages;
        private System.Windows.Forms.StatusBarPanel statusIcon;
        private System.Windows.Forms.StatusBarPanel statusText;       
        private System.Windows.Forms.StatusBarPanel statusGeneral;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabProcesses;
        private System.Windows.Forms.TabPage tabServices;
        private ProcessHacker.Components.ServiceList listServices;
        private System.Windows.Forms.ContextMenu menuService;
        private System.Windows.Forms.ContextMenu menuIcon;
        private System.Windows.Forms.StatusBarPanel statusCPU;
        private System.Windows.Forms.StatusBarPanel statusMemory;
        private System.Windows.Forms.TabPage tabNetwork;
        private ProcessHacker.Components.NetworkList listNetwork;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton refreshToolStripButton;
        private System.Windows.Forms.ToolStripButton findHandlesToolStripButton;
        private System.Windows.Forms.ToolStripButton sysInfoToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripDropDownButton shutDownToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton optionsToolStripButton;
        private System.Windows.Forms.ContextMenu menuNetwork;
    }
}

