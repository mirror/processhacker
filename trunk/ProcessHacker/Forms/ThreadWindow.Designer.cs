namespace ProcessHacker
{
    partial class ThreadWindow
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

            Program.ThreadWindows.Remove(Id);
            Program.UpdateWindows();

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ThreadWindow));
            this.mainMenu = new System.Windows.Forms.MainMenu(this.components);
            this.threadMenuItem = new System.Windows.Forms.MenuItem();
            this.tokenMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.suspendMenuItem = new System.Windows.Forms.MenuItem();
            this.resumeMenuItem = new System.Windows.Forms.MenuItem();
            this.terminateMenuItem = new System.Windows.Forms.MenuItem();
            this.windowMenuItem = new System.Windows.Forms.MenuItem();
            this.timerUpdate = new System.Windows.Forms.Timer(this.components);
            this.listViewCallStack = new System.Windows.Forms.ListView();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.groupBoxCallStack = new System.Windows.Forms.GroupBox();
            this.buttonWalk = new System.Windows.Forms.Button();
            this.groupRegisters = new System.Windows.Forms.GroupBox();
            this.listViewRegisters = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.labelThreadUser = new System.Windows.Forms.Label();
            this.vistaMenu = new wyDay.Controls.VistaMenu(this.components);
            this.groupBoxCallStack.SuspendLayout();
            this.groupRegisters.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.vistaMenu)).BeginInit();
            this.SuspendLayout();
            // 
            // mainMenu
            // 
            this.mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.threadMenuItem,
            this.windowMenuItem});
            // 
            // threadMenuItem
            // 
            this.threadMenuItem.Index = 0;
            this.threadMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.tokenMenuItem,
            this.menuItem2,
            this.suspendMenuItem,
            this.resumeMenuItem,
            this.terminateMenuItem});
            this.threadMenuItem.Text = "&Thread";
            // 
            // tokenMenuItem
            // 
            this.vistaMenu.SetImage(this.tokenMenuItem, global::ProcessHacker.Properties.Resources.lock_edit);
            this.tokenMenuItem.Index = 0;
            this.tokenMenuItem.Text = "To&ken...";
            this.tokenMenuItem.Click += new System.EventHandler(this.tokenMenuItem_Click);
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 1;
            this.menuItem2.Text = "-";
            // 
            // suspendMenuItem
            // 
            this.vistaMenu.SetImage(this.suspendMenuItem, global::ProcessHacker.Properties.Resources.control_pause_blue);
            this.suspendMenuItem.Index = 2;
            this.suspendMenuItem.Shortcut = System.Windows.Forms.Shortcut.CtrlS;
            this.suspendMenuItem.Text = "&Suspend";
            this.suspendMenuItem.Click += new System.EventHandler(this.suspendMenuItem_Click);
            // 
            // resumeMenuItem
            // 
            this.vistaMenu.SetImage(this.resumeMenuItem, global::ProcessHacker.Properties.Resources.control_play_blue);
            this.resumeMenuItem.Index = 3;
            this.resumeMenuItem.Shortcut = System.Windows.Forms.Shortcut.CtrlR;
            this.resumeMenuItem.Text = "&Resume";
            this.resumeMenuItem.Click += new System.EventHandler(this.resumeMenuItem_Click);
            // 
            // terminateMenuItem
            // 
            this.vistaMenu.SetImage(this.terminateMenuItem, global::ProcessHacker.Properties.Resources.cross);
            this.terminateMenuItem.Index = 4;
            this.terminateMenuItem.Shortcut = System.Windows.Forms.Shortcut.CtrlDel;
            this.terminateMenuItem.Text = "&Terminate";
            this.terminateMenuItem.Click += new System.EventHandler(this.terminateMenuItem_Click);
            // 
            // windowMenuItem
            // 
            this.windowMenuItem.Index = 1;
            this.windowMenuItem.Text = "&Window";
            // 
            // timerUpdate
            // 
            this.timerUpdate.Enabled = true;
            this.timerUpdate.Interval = 1000;
            this.timerUpdate.Tick += new System.EventHandler(this.timerUpdate_Tick);
            // 
            // listViewCallStack
            // 
            this.listViewCallStack.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewCallStack.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader4});
            this.listViewCallStack.FullRowSelect = true;
            this.listViewCallStack.HideSelection = false;
            this.listViewCallStack.Location = new System.Drawing.Point(6, 19);
            this.listViewCallStack.Name = "listViewCallStack";
            this.listViewCallStack.ShowItemToolTips = true;
            this.listViewCallStack.Size = new System.Drawing.Size(363, 158);
            this.listViewCallStack.TabIndex = 0;
            this.listViewCallStack.UseCompatibleStateImageBehavior = false;
            this.listViewCallStack.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Address";
            this.columnHeader3.Width = 100;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Symbol Name";
            this.columnHeader4.Width = 220;
            // 
            // groupBoxCallStack
            // 
            this.groupBoxCallStack.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxCallStack.Controls.Add(this.buttonWalk);
            this.groupBoxCallStack.Controls.Add(this.listViewCallStack);
            this.groupBoxCallStack.Location = new System.Drawing.Point(12, 34);
            this.groupBoxCallStack.Name = "groupBoxCallStack";
            this.groupBoxCallStack.Size = new System.Drawing.Size(375, 212);
            this.groupBoxCallStack.TabIndex = 1;
            this.groupBoxCallStack.TabStop = false;
            this.groupBoxCallStack.Text = "Call Stack";
            // 
            // buttonWalk
            // 
            this.buttonWalk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonWalk.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonWalk.Location = new System.Drawing.Point(294, 183);
            this.buttonWalk.Name = "buttonWalk";
            this.buttonWalk.Size = new System.Drawing.Size(75, 23);
            this.buttonWalk.TabIndex = 3;
            this.buttonWalk.Text = "&Refresh";
            this.buttonWalk.UseVisualStyleBackColor = true;
            this.buttonWalk.Click += new System.EventHandler(this.buttonWalk_Click);
            // 
            // groupRegisters
            // 
            this.groupRegisters.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupRegisters.Controls.Add(this.listViewRegisters);
            this.groupRegisters.Location = new System.Drawing.Point(12, 252);
            this.groupRegisters.Name = "groupRegisters";
            this.groupRegisters.Size = new System.Drawing.Size(375, 132);
            this.groupRegisters.TabIndex = 2;
            this.groupRegisters.TabStop = false;
            this.groupRegisters.Text = "Registers";
            // 
            // listViewRegisters
            // 
            this.listViewRegisters.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewRegisters.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listViewRegisters.FullRowSelect = true;
            this.listViewRegisters.Location = new System.Drawing.Point(6, 19);
            this.listViewRegisters.Name = "listViewRegisters";
            this.listViewRegisters.Size = new System.Drawing.Size(363, 107);
            this.listViewRegisters.TabIndex = 0;
            this.listViewRegisters.UseCompatibleStateImageBehavior = false;
            this.listViewRegisters.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Value";
            this.columnHeader2.Width = 100;
            // 
            // labelThreadUser
            // 
            this.labelThreadUser.AutoSize = true;
            this.labelThreadUser.Location = new System.Drawing.Point(12, 9);
            this.labelThreadUser.Name = "labelThreadUser";
            this.labelThreadUser.Size = new System.Drawing.Size(107, 13);
            this.labelThreadUser.TabIndex = 3;
            this.labelThreadUser.Text = "Username: Unknown";
            // 
            // vistaMenu
            // 
            this.vistaMenu.ContainerControl = this;
            // 
            // ThreadWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(399, 396);
            this.Controls.Add(this.labelThreadUser);
            this.Controls.Add(this.groupRegisters);
            this.Controls.Add(this.groupBoxCallStack);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Menu = this.mainMenu;
            this.Name = "ThreadWindow";
            this.Text = "Thread";
            this.Load += new System.EventHandler(this.ThreadWindow_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ThreadWindow_FormClosing);
            this.groupBoxCallStack.ResumeLayout(false);
            this.groupRegisters.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.vistaMenu)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private wyDay.Controls.VistaMenu vistaMenu;
        private System.Windows.Forms.MainMenu mainMenu;
        private System.Windows.Forms.MenuItem windowMenuItem;
        private System.Windows.Forms.Timer timerUpdate;
        private System.Windows.Forms.GroupBox groupBoxCallStack;
        private System.Windows.Forms.ListView listViewCallStack;
        private System.Windows.Forms.GroupBox groupRegisters;
        private System.Windows.Forms.ListView listViewRegisters;
        private System.Windows.Forms.MenuItem threadMenuItem;
        private System.Windows.Forms.MenuItem suspendMenuItem;
        private System.Windows.Forms.MenuItem resumeMenuItem;
        private System.Windows.Forms.MenuItem terminateMenuItem;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.Button buttonWalk;
        private System.Windows.Forms.MenuItem tokenMenuItem;
        private System.Windows.Forms.Label labelThreadUser;
        private System.Windows.Forms.MenuItem menuItem2;
    }
}