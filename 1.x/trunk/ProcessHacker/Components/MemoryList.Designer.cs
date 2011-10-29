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
            this.listMemory = new ProcessHacker.Components.ExtendedListView();
            this.columnName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnAddress = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnProtection = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
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
            this.listMemory.DoubleClickChecks = true;
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
            // changeMemoryProtectionMemoryMenuItem
            // 
            this.changeMemoryProtectionMemoryMenuItem.Index = 2;
            this.changeMemoryProtectionMemoryMenuItem.Text = "Change &Memory Protection...";
            this.changeMemoryProtectionMemoryMenuItem.Click += new System.EventHandler(this.changeMemoryProtectionMemoryMenuItem_Click);
            // 
            // readWriteMemoryMemoryMenuItem
            // 
            this.readWriteMemoryMemoryMenuItem.DefaultItem = true;
            this.readWriteMemoryMemoryMenuItem.Index = 0;
            this.readWriteMemoryMemoryMenuItem.Text = "Read/Write Memory";
            this.readWriteMemoryMemoryMenuItem.Click += new System.EventHandler(this.readWriteMemoryMemoryMenuItem_Click);
            // 
            // readWriteAddressMemoryMenuItem
            // 
            this.readWriteAddressMemoryMenuItem.Index = 6;
            this.readWriteAddressMemoryMenuItem.Text = "Read/Write Address...";
            this.readWriteAddressMemoryMenuItem.Click += new System.EventHandler(this.readWriteAddressMemoryMenuItem_Click);
            // 
            // copyMemoryMenuItem
            // 
            this.copyMemoryMenuItem.Index = 7;
            this.copyMemoryMenuItem.Text = "C&opy";
            // 
            // freeMenuItem
            // 
            this.freeMenuItem.Index = 3;
            this.freeMenuItem.Text = "&Free";
            this.freeMenuItem.Click += new System.EventHandler(this.freeMenuItem_Click);
            // 
            // decommitMenuItem
            // 
            this.decommitMenuItem.Index = 4;
            this.decommitMenuItem.Text = "&Decommit";
            this.decommitMenuItem.Click += new System.EventHandler(this.decommitMenuItem_Click);
            // 
            // dumpMemoryMenuItem
            // 
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
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Controls.Add(this.listMemory);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "MemoryList";
            this.Size = new System.Drawing.Size(450, 472);
            this.ResumeLayout(false);

        }

        #endregion

        private ExtendedListView listMemory;
        private System.Windows.Forms.ColumnHeader columnName;
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
