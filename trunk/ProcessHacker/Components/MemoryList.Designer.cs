namespace ProcessHacker.Components
{
    partial class MemoryList
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

            this.Provider = null;

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
            this.listMemory = new System.Windows.Forms.ListView();
            this.columnName = new System.Windows.Forms.ColumnHeader();
            this.columnAddress = new System.Windows.Forms.ColumnHeader();
            this.columnSize = new System.Windows.Forms.ColumnHeader();
            this.columnProtection = new System.Windows.Forms.ColumnHeader();
            this.vistaMenu = new wyDay.Controls.VistaMenu(this.components);
            this.changeMemoryProtectionMemoryMenuItem = new System.Windows.Forms.MenuItem();
            this.readWriteMemoryMemoryMenuItem = new System.Windows.Forms.MenuItem();
            this.readWriteAddressMemoryMenuItem = new System.Windows.Forms.MenuItem();
            this.copyMemoryMenuItem = new System.Windows.Forms.MenuItem();
            this.freeMenuItem = new System.Windows.Forms.MenuItem();
            this.decommitMenuItem = new System.Windows.Forms.MenuItem();
            this.dumpMemoryMenuItem = new System.Windows.Forms.MenuItem();
            this.menuMemory = new System.Windows.Forms.ContextMenu();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.selectAllMemoryMenuItem = new System.Windows.Forms.MenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.vistaMenu)).BeginInit();
            this.SuspendLayout();
            // 
            // listMemory
            // 
            this.listMemory.AllowColumnReorder = true;
            this.listMemory.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnName,
            this.columnAddress,
            this.columnSize,
            this.columnProtection});
            this.listMemory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listMemory.FullRowSelect = true;
            this.listMemory.HideSelection = false;
            this.listMemory.Location = new System.Drawing.Point(0, 0);
            this.listMemory.Name = "listMemory";
            this.listMemory.ShowItemToolTips = true;
            this.listMemory.Size = new System.Drawing.Size(450, 472);
            this.listMemory.TabIndex = 3;
            this.listMemory.UseCompatibleStateImageBehavior = false;
            this.listMemory.View = System.Windows.Forms.View.Details;
            this.listMemory.DoubleClick += new System.EventHandler(this.listMemory_DoubleClick);
            // 
            // columnName
            // 
            this.columnName.Text = "Name";
            this.columnName.Width = 160;
            // 
            // columnAddress
            // 
            this.columnAddress.Text = "Address";
            this.columnAddress.Width = 80;
            // 
            // columnSize
            // 
            this.columnSize.Text = "Size";
            // 
            // columnProtection
            // 
            this.columnProtection.Text = "Protection";
            // 
            // vistaMenu
            // 
            this.vistaMenu.ContainerControl = this;
            this.vistaMenu.DelaySetImageCalls = false;
            // 
            // changeMemoryProtectionMemoryMenuItem
            // 
            this.vistaMenu.SetImage(this.changeMemoryProtectionMemoryMenuItem, global::ProcessHacker.Properties.Resources.lock_edit);
            this.changeMemoryProtectionMemoryMenuItem.Index = 2;
            this.changeMemoryProtectionMemoryMenuItem.Text = "Change &Memory Protection...";
            this.changeMemoryProtectionMemoryMenuItem.Click += new System.EventHandler(this.changeMemoryProtectionMemoryMenuItem_Click);
            // 
            // readWriteMemoryMemoryMenuItem
            // 
            this.readWriteMemoryMemoryMenuItem.DefaultItem = true;
            this.vistaMenu.SetImage(this.readWriteMemoryMemoryMenuItem, global::ProcessHacker.Properties.Resources.page_edit);
            this.readWriteMemoryMemoryMenuItem.Index = 0;
            this.readWriteMemoryMemoryMenuItem.Text = "Read/Write Memory";
            this.readWriteMemoryMemoryMenuItem.Click += new System.EventHandler(this.readWriteMemoryMemoryMenuItem_Click);
            // 
            // readWriteAddressMemoryMenuItem
            // 
            this.vistaMenu.SetImage(this.readWriteAddressMemoryMenuItem, global::ProcessHacker.Properties.Resources.pencil_go);
            this.readWriteAddressMemoryMenuItem.Index = 6;
            this.readWriteAddressMemoryMenuItem.Text = "Read/Write Address...";
            this.readWriteAddressMemoryMenuItem.Click += new System.EventHandler(this.readWriteAddressMemoryMenuItem_Click);
            // 
            // copyMemoryMenuItem
            // 
            this.vistaMenu.SetImage(this.copyMemoryMenuItem, global::ProcessHacker.Properties.Resources.page_copy);
            this.copyMemoryMenuItem.Index = 7;
            this.copyMemoryMenuItem.Text = "C&opy";
            // 
            // freeMenuItem
            // 
            this.vistaMenu.SetImage(this.freeMenuItem, global::ProcessHacker.Properties.Resources.cross);
            this.freeMenuItem.Index = 3;
            this.freeMenuItem.Text = "&Free";
            this.freeMenuItem.Click += new System.EventHandler(this.freeMenuItem_Click);
            // 
            // decommitMenuItem
            // 
            this.vistaMenu.SetImage(this.decommitMenuItem, global::ProcessHacker.Properties.Resources.delete);
            this.decommitMenuItem.Index = 4;
            this.decommitMenuItem.Text = "&Decommit";
            this.decommitMenuItem.Click += new System.EventHandler(this.decommitMenuItem_Click);
            // 
            // dumpMemoryMenuItem
            // 
            this.vistaMenu.SetImage(this.dumpMemoryMenuItem, global::ProcessHacker.Properties.Resources.disk);
            this.dumpMemoryMenuItem.Index = 1;
            this.dumpMemoryMenuItem.Text = "Dump...";
            this.dumpMemoryMenuItem.Click += new System.EventHandler(this.dumpMemoryMenuItem_Click);
            // 
            // menuMemory
            // 
            this.menuMemory.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.readWriteMemoryMemoryMenuItem,
            this.dumpMemoryMenuItem,
            this.changeMemoryProtectionMemoryMenuItem,
            this.freeMenuItem,
            this.decommitMenuItem,
            this.menuItem2,
            this.readWriteAddressMemoryMenuItem,
            this.copyMemoryMenuItem,
            this.selectAllMemoryMenuItem});
            this.menuMemory.Popup += new System.EventHandler(this.menuMemory_Popup);
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 5;
            this.menuItem2.Text = "-";
            // 
            // selectAllMemoryMenuItem
            // 
            this.selectAllMemoryMenuItem.Index = 8;
            this.selectAllMemoryMenuItem.Text = "Select &All";
            this.selectAllMemoryMenuItem.Click += new System.EventHandler(this.selectAllMemoryMenuItem_Click);
            // 
            // MemoryList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.listMemory);
            this.DoubleBuffered = true;
            this.Name = "MemoryList";
            this.Size = new System.Drawing.Size(450, 472);
            ((System.ComponentModel.ISupportInitialize)(this.vistaMenu)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listMemory;
        private System.Windows.Forms.ColumnHeader columnName;
        private wyDay.Controls.VistaMenu vistaMenu;
        private System.Windows.Forms.ColumnHeader columnSize;
        private System.Windows.Forms.ColumnHeader columnAddress;
        private System.Windows.Forms.ColumnHeader columnProtection;
        private System.Windows.Forms.ContextMenu menuMemory;
        private System.Windows.Forms.MenuItem changeMemoryProtectionMemoryMenuItem;
        private System.Windows.Forms.MenuItem readWriteMemoryMemoryMenuItem;
        private System.Windows.Forms.MenuItem readWriteAddressMemoryMenuItem;
        private System.Windows.Forms.MenuItem menuItem2;
        private System.Windows.Forms.MenuItem copyMemoryMenuItem;
        private System.Windows.Forms.MenuItem selectAllMemoryMenuItem;
        private System.Windows.Forms.MenuItem freeMenuItem;
        private System.Windows.Forms.MenuItem decommitMenuItem;
        private System.Windows.Forms.MenuItem dumpMemoryMenuItem;
    }
}
