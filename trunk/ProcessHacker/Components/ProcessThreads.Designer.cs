namespace ProcessHacker
{
    partial class ProcessThreads
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
            this.menuThread = new System.Windows.Forms.ContextMenu();
            this.inspectThreadMenuItem = new System.Windows.Forms.MenuItem();
            this.terminateThreadMenuItem = new System.Windows.Forms.MenuItem();
            this.suspendThreadMenuItem = new System.Windows.Forms.MenuItem();
            this.resumeThreadMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.priorityThreadMenuItem = new System.Windows.Forms.MenuItem();
            this.timeCriticalThreadMenuItem = new System.Windows.Forms.MenuItem();
            this.highestThreadMenuItem = new System.Windows.Forms.MenuItem();
            this.aboveNormalThreadMenuItem = new System.Windows.Forms.MenuItem();
            this.normalThreadMenuItem = new System.Windows.Forms.MenuItem();
            this.belowNormalThreadMenuItem = new System.Windows.Forms.MenuItem();
            this.lowestThreadMenuItem = new System.Windows.Forms.MenuItem();
            this.idleThreadMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem9 = new System.Windows.Forms.MenuItem();
            this.copyThreadMenuItem = new System.Windows.Forms.MenuItem();
            this.selectAllThreadMenuItem = new System.Windows.Forms.MenuItem();
            this.listThreads = new ProcessHacker.ThreadList();
            this.vistaMenu = new wyDay.Controls.VistaMenu(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.vistaMenu)).BeginInit();
            this.SuspendLayout();
            // 
            // menuThread
            // 
            this.menuThread.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.inspectThreadMenuItem,
            this.terminateThreadMenuItem,
            this.suspendThreadMenuItem,
            this.resumeThreadMenuItem,
            this.menuItem4,
            this.priorityThreadMenuItem,
            this.menuItem9,
            this.copyThreadMenuItem,
            this.selectAllThreadMenuItem});
            this.menuThread.Popup += new System.EventHandler(this.menuThread_Popup);
            // 
            // inspectThreadMenuItem
            // 
            this.inspectThreadMenuItem.DefaultItem = true;
            this.vistaMenu.SetImage(this.inspectThreadMenuItem, global::ProcessHacker.Properties.Resources.application_form_magnify);
            this.inspectThreadMenuItem.Index = 0;
            this.inspectThreadMenuItem.Text = "&Inspect";
            this.inspectThreadMenuItem.Click += new System.EventHandler(this.inspectThreadMenuItem_Click);
            // 
            // terminateThreadMenuItem
            // 
            this.vistaMenu.SetImage(this.terminateThreadMenuItem, global::ProcessHacker.Properties.Resources.cross);
            this.terminateThreadMenuItem.Index = 1;
            this.terminateThreadMenuItem.Text = "&Terminate";
            this.terminateThreadMenuItem.Click += new System.EventHandler(this.terminateThreadMenuItem_Click);
            // 
            // suspendThreadMenuItem
            // 
            this.vistaMenu.SetImage(this.suspendThreadMenuItem, global::ProcessHacker.Properties.Resources.control_pause_blue);
            this.suspendThreadMenuItem.Index = 2;
            this.suspendThreadMenuItem.Text = "&Suspend";
            this.suspendThreadMenuItem.Click += new System.EventHandler(this.suspendThreadMenuItem_Click);
            // 
            // resumeThreadMenuItem
            // 
            this.vistaMenu.SetImage(this.resumeThreadMenuItem, global::ProcessHacker.Properties.Resources.control_play_blue);
            this.resumeThreadMenuItem.Index = 3;
            this.resumeThreadMenuItem.Text = "&Resume";
            this.resumeThreadMenuItem.Click += new System.EventHandler(this.resumeThreadMenuItem_Click);
            // 
            // menuItem4
            // 
            this.menuItem4.Index = 4;
            this.menuItem4.Text = "-";
            // 
            // priorityThreadMenuItem
            // 
            this.priorityThreadMenuItem.Index = 5;
            this.priorityThreadMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.timeCriticalThreadMenuItem,
            this.highestThreadMenuItem,
            this.aboveNormalThreadMenuItem,
            this.normalThreadMenuItem,
            this.belowNormalThreadMenuItem,
            this.lowestThreadMenuItem,
            this.idleThreadMenuItem});
            this.priorityThreadMenuItem.Text = "&Priority";
            // 
            // timeCriticalThreadMenuItem
            // 
            this.timeCriticalThreadMenuItem.Index = 0;
            this.timeCriticalThreadMenuItem.RadioCheck = true;
            this.timeCriticalThreadMenuItem.Text = "Time Critical";
            this.timeCriticalThreadMenuItem.Click += new System.EventHandler(this.timeCriticalThreadMenuItem_Click);
            // 
            // highestThreadMenuItem
            // 
            this.highestThreadMenuItem.Index = 1;
            this.highestThreadMenuItem.RadioCheck = true;
            this.highestThreadMenuItem.Text = "Highest";
            this.highestThreadMenuItem.Click += new System.EventHandler(this.highestThreadMenuItem_Click);
            // 
            // aboveNormalThreadMenuItem
            // 
            this.aboveNormalThreadMenuItem.Index = 2;
            this.aboveNormalThreadMenuItem.RadioCheck = true;
            this.aboveNormalThreadMenuItem.Text = "Above Normal";
            this.aboveNormalThreadMenuItem.Click += new System.EventHandler(this.aboveNormalThreadMenuItem_Click);
            // 
            // normalThreadMenuItem
            // 
            this.normalThreadMenuItem.Index = 3;
            this.normalThreadMenuItem.RadioCheck = true;
            this.normalThreadMenuItem.Text = "Normal";
            this.normalThreadMenuItem.Click += new System.EventHandler(this.normalThreadMenuItem_Click);
            // 
            // belowNormalThreadMenuItem
            // 
            this.belowNormalThreadMenuItem.Index = 4;
            this.belowNormalThreadMenuItem.RadioCheck = true;
            this.belowNormalThreadMenuItem.Text = "Below Normal";
            this.belowNormalThreadMenuItem.Click += new System.EventHandler(this.belowNormalThreadMenuItem_Click);
            // 
            // lowestThreadMenuItem
            // 
            this.lowestThreadMenuItem.Index = 5;
            this.lowestThreadMenuItem.RadioCheck = true;
            this.lowestThreadMenuItem.Text = "Lowest";
            this.lowestThreadMenuItem.Click += new System.EventHandler(this.lowestThreadMenuItem_Click);
            // 
            // idleThreadMenuItem
            // 
            this.idleThreadMenuItem.Index = 6;
            this.idleThreadMenuItem.RadioCheck = true;
            this.idleThreadMenuItem.Text = "Idle";
            this.idleThreadMenuItem.Click += new System.EventHandler(this.idleThreadMenuItem_Click);
            // 
            // menuItem9
            // 
            this.menuItem9.Index = 6;
            this.menuItem9.Text = "-";
            // 
            // copyThreadMenuItem
            // 
            this.vistaMenu.SetImage(this.copyThreadMenuItem, global::ProcessHacker.Properties.Resources.page_copy);
            this.copyThreadMenuItem.Index = 7;
            this.copyThreadMenuItem.Text = "C&opy";
            // 
            // selectAllThreadMenuItem
            // 
            this.selectAllThreadMenuItem.Index = 8;
            this.selectAllThreadMenuItem.Text = "Select &All";
            this.selectAllThreadMenuItem.Click += new System.EventHandler(this.selectAllThreadMenuItem_Click);
            // 
            // listThreads
            // 
            this.listThreads.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listThreads.DoubleBuffered = true;
            this.listThreads.Location = new System.Drawing.Point(0, 0);
            this.listThreads.Name = "listThreads";
            this.listThreads.Provider = null;
            this.listThreads.Size = new System.Drawing.Size(503, 427);
            this.listThreads.TabIndex = 2;
            this.listThreads.DoubleClick += new System.EventHandler(this.listThreads_DoubleClick);
            // 
            // vistaMenu
            // 
            this.vistaMenu.ContainerControl = this;
            // 
            // ProcessThreads
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.listThreads);
            this.Name = "ProcessThreads";
            this.Size = new System.Drawing.Size(503, 427);
            ((System.ComponentModel.ISupportInitialize)(this.vistaMenu)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenu menuThread;
        private System.Windows.Forms.MenuItem inspectThreadMenuItem;
        private System.Windows.Forms.MenuItem terminateThreadMenuItem;
        private System.Windows.Forms.MenuItem suspendThreadMenuItem;
        private System.Windows.Forms.MenuItem resumeThreadMenuItem;
        private System.Windows.Forms.MenuItem menuItem4;
        private System.Windows.Forms.MenuItem priorityThreadMenuItem;
        private System.Windows.Forms.MenuItem timeCriticalThreadMenuItem;
        private System.Windows.Forms.MenuItem highestThreadMenuItem;
        private System.Windows.Forms.MenuItem aboveNormalThreadMenuItem;
        private System.Windows.Forms.MenuItem normalThreadMenuItem;
        private System.Windows.Forms.MenuItem belowNormalThreadMenuItem;
        private System.Windows.Forms.MenuItem lowestThreadMenuItem;
        private System.Windows.Forms.MenuItem idleThreadMenuItem;
        private System.Windows.Forms.MenuItem menuItem9;
        private System.Windows.Forms.MenuItem copyThreadMenuItem;
        private System.Windows.Forms.MenuItem selectAllThreadMenuItem;
        private ThreadList listThreads;
        private wyDay.Controls.VistaMenu vistaMenu;
    }
}
