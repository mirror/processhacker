namespace ProcessHacker.Components
{
    partial class ModuleList
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

            _highlightingContext.Dispose();
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
            this.listModules = new System.Windows.Forms.ListView();
            this.columnName = new System.Windows.Forms.ColumnHeader();
            this.columnBaseAddress = new System.Windows.Forms.ColumnHeader();
            this.columnSize = new System.Windows.Forms.ColumnHeader();
            this.columnDesc = new System.Windows.Forms.ColumnHeader();
            this.changeMemoryProtectionModuleMenuItem = new System.Windows.Forms.MenuItem();
            this.readMemoryModuleMenuItem = new System.Windows.Forms.MenuItem();
            this.inspectModuleMenuItem = new System.Windows.Forms.MenuItem();
            this.copyModuleMenuItem = new System.Windows.Forms.MenuItem();
            this.openContainingFolderMenuItem = new System.Windows.Forms.MenuItem();
            this.propertiesMenuItem = new System.Windows.Forms.MenuItem();
            this.unloadMenuItem = new System.Windows.Forms.MenuItem();
            this.menuModule = new System.Windows.Forms.ContextMenu();
            this.getFuncAddressMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.searchModuleMenuItem = new System.Windows.Forms.MenuItem();
            this.copyFileNameMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem6 = new System.Windows.Forms.MenuItem();
            this.selectAllModuleMenuItem = new System.Windows.Forms.MenuItem();
            this.vistaMenu = new wyDay.Controls.VistaMenu(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.vistaMenu)).BeginInit();
            this.SuspendLayout();
            // 
            // listModules
            // 
            this.listModules.AllowColumnReorder = true;
            this.listModules.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnName,
            this.columnBaseAddress,
            this.columnSize,
            this.columnDesc});
            this.listModules.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listModules.FullRowSelect = true;
            this.listModules.HideSelection = false;
            this.listModules.Location = new System.Drawing.Point(0, 0);
            this.listModules.Name = "listModules";
            this.listModules.ShowItemToolTips = true;
            this.listModules.Size = new System.Drawing.Size(450, 472);
            this.listModules.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listModules.TabIndex = 3;
            this.listModules.UseCompatibleStateImageBehavior = false;
            this.listModules.View = System.Windows.Forms.View.Details;
            // 
            // columnName
            // 
            this.columnName.Text = "Name";
            this.columnName.Width = 99;
            // 
            // columnBaseAddress
            // 
            this.columnBaseAddress.Text = "Base Address";
            this.columnBaseAddress.Width = 80;
            // 
            // columnSize
            // 
            this.columnSize.Text = "Size";
            this.columnSize.Width = 70;
            // 
            // columnDesc
            // 
            this.columnDesc.Text = "Description";
            this.columnDesc.Width = 188;
            // 
            // changeMemoryProtectionModuleMenuItem
            // 
            this.vistaMenu.SetImage(this.changeMemoryProtectionModuleMenuItem, global::ProcessHacker.Properties.Resources.lock_edit);
            this.changeMemoryProtectionModuleMenuItem.Index = 0;
            this.changeMemoryProtectionModuleMenuItem.Text = "Change &Memory Protection...";
            this.changeMemoryProtectionModuleMenuItem.Click += new System.EventHandler(this.changeMemoryProtectionModuleMenuItem_Click);
            // 
            // readMemoryModuleMenuItem
            // 
            this.vistaMenu.SetImage(this.readMemoryModuleMenuItem, global::ProcessHacker.Properties.Resources.page);
            this.readMemoryModuleMenuItem.Index = 2;
            this.readMemoryModuleMenuItem.Text = "Read Memory";
            this.readMemoryModuleMenuItem.Click += new System.EventHandler(this.readMemoryModuleMenuItem_Click);
            // 
            // inspectModuleMenuItem
            // 
            this.vistaMenu.SetImage(this.inspectModuleMenuItem, global::ProcessHacker.Properties.Resources.application_form_magnify);
            this.inspectModuleMenuItem.Index = 5;
            this.inspectModuleMenuItem.Text = "&Inspect";
            this.inspectModuleMenuItem.Click += new System.EventHandler(this.inspectModuleMenuItem_Click);
            // 
            // copyModuleMenuItem
            // 
            this.vistaMenu.SetImage(this.copyModuleMenuItem, global::ProcessHacker.Properties.Resources.page_copy);
            this.copyModuleMenuItem.Index = 8;
            this.copyModuleMenuItem.Text = "Copy";
            // 
            // openContainingFolderMenuItem
            // 
            this.vistaMenu.SetImage(this.openContainingFolderMenuItem, global::ProcessHacker.Properties.Resources.folder_explore);
            this.openContainingFolderMenuItem.Index = 9;
            this.openContainingFolderMenuItem.Text = "&Open Containing Folder";
            this.openContainingFolderMenuItem.Click += new System.EventHandler(this.openContainingFolderMenuItem_Click);
            // 
            // propertiesMenuItem
            // 
            this.vistaMenu.SetImage(this.propertiesMenuItem, global::ProcessHacker.Properties.Resources.application_view_detail);
            this.propertiesMenuItem.Index = 10;
            this.propertiesMenuItem.Text = "Prope&rties";
            this.propertiesMenuItem.Click += new System.EventHandler(this.propertiesMenuItem_Click);
            // 
            // unloadMenuItem
            // 
            this.vistaMenu.SetImage(this.unloadMenuItem, global::ProcessHacker.Properties.Resources.cross);
            this.unloadMenuItem.Index = 3;
            this.unloadMenuItem.Text = "&Unload";
            this.unloadMenuItem.Click += new System.EventHandler(this.unloadMenuItem_Click);
            // 
            // menuModule
            // 
            this.menuModule.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.changeMemoryProtectionModuleMenuItem,
            this.getFuncAddressMenuItem,
            this.readMemoryModuleMenuItem,
            this.unloadMenuItem,
            this.menuItem3,
            this.inspectModuleMenuItem,
            this.searchModuleMenuItem,
            this.copyFileNameMenuItem,
            this.copyModuleMenuItem,
            this.openContainingFolderMenuItem,
            this.propertiesMenuItem,
            this.menuItem6,
            this.selectAllModuleMenuItem});
            this.menuModule.Popup += new System.EventHandler(this.menuModule_Popup);
            // 
            // getFuncAddressMenuItem
            // 
            this.getFuncAddressMenuItem.Index = 1;
            this.getFuncAddressMenuItem.Text = "Get &Function Address...";
            this.getFuncAddressMenuItem.Click += new System.EventHandler(this.getFuncAddressMenuItem_Click);
            // 
            // menuItem3
            // 
            this.menuItem3.Index = 4;
            this.menuItem3.Text = "-";
            // 
            // searchModuleMenuItem
            // 
            this.searchModuleMenuItem.Index = 6;
            this.searchModuleMenuItem.Text = "&Search Online";
            this.searchModuleMenuItem.Click += new System.EventHandler(this.searchModuleMenuItem_Click);
            // 
            // copyFileNameMenuItem
            // 
            this.copyFileNameMenuItem.Index = 7;
            this.copyFileNameMenuItem.Text = "&Copy File Name(s)";
            this.copyFileNameMenuItem.Click += new System.EventHandler(this.copyFileNameMenuItem_Click);
            // 
            // menuItem6
            // 
            this.menuItem6.Index = 11;
            this.menuItem6.Text = "-";
            // 
            // selectAllModuleMenuItem
            // 
            this.selectAllModuleMenuItem.Index = 12;
            this.selectAllModuleMenuItem.Text = "Select &All";
            this.selectAllModuleMenuItem.Click += new System.EventHandler(this.selectAllModuleMenuItem_Click);
            // 
            // vistaMenu
            // 
            this.vistaMenu.ContainerControl = this;
            this.vistaMenu.DelaySetImageCalls = false;
            // 
            // ModuleList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.listModules);
            this.DoubleBuffered = true;
            this.Name = "ModuleList";
            this.Size = new System.Drawing.Size(450, 472);
            ((System.ComponentModel.ISupportInitialize)(this.vistaMenu)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listModules;
        private System.Windows.Forms.ColumnHeader columnBaseAddress;
        private System.Windows.Forms.ColumnHeader columnName;
        private System.Windows.Forms.ColumnHeader columnSize;
        private System.Windows.Forms.ColumnHeader columnDesc;
        private wyDay.Controls.VistaMenu vistaMenu;
        private System.Windows.Forms.ContextMenu menuModule;
        private System.Windows.Forms.MenuItem getFuncAddressMenuItem;
        private System.Windows.Forms.MenuItem changeMemoryProtectionModuleMenuItem;
        private System.Windows.Forms.MenuItem readMemoryModuleMenuItem;
        private System.Windows.Forms.MenuItem inspectModuleMenuItem;
        private System.Windows.Forms.MenuItem menuItem3;
        private System.Windows.Forms.MenuItem searchModuleMenuItem;
        private System.Windows.Forms.MenuItem copyFileNameMenuItem;
        private System.Windows.Forms.MenuItem copyModuleMenuItem;
        private System.Windows.Forms.MenuItem openContainingFolderMenuItem;
        private System.Windows.Forms.MenuItem propertiesMenuItem;
        private System.Windows.Forms.MenuItem menuItem6;
        private System.Windows.Forms.MenuItem selectAllModuleMenuItem;
        private System.Windows.Forms.MenuItem unloadMenuItem;
    }
}
