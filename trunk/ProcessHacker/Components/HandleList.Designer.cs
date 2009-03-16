namespace ProcessHacker
{
    partial class HandleList
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
            this.listHandles = new System.Windows.Forms.ListView();
            this.columnType = new System.Windows.Forms.ColumnHeader();
            this.columnName = new System.Windows.Forms.ColumnHeader();
            this.columnHandle = new System.Windows.Forms.ColumnHeader();
            this.vistaMenu = new wyDay.Controls.VistaMenu(this.components);
            this.closeHandleMenuItem = new System.Windows.Forms.MenuItem();
            this.copyHandleMenuItem = new System.Windows.Forms.MenuItem();
            this.menuHandle = new System.Windows.Forms.ContextMenu();
            this.menuItem11 = new System.Windows.Forms.MenuItem();
            this.propertiesHandleMenuItem = new System.Windows.Forms.MenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.vistaMenu)).BeginInit();
            this.SuspendLayout();
            // 
            // listHandles
            // 
            this.listHandles.AllowColumnReorder = true;
            this.listHandles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnType,
            this.columnName,
            this.columnHandle});
            this.listHandles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listHandles.FullRowSelect = true;
            this.listHandles.HideSelection = false;
            this.listHandles.Location = new System.Drawing.Point(0, 0);
            this.listHandles.Name = "listHandles";
            this.listHandles.ShowItemToolTips = true;
            this.listHandles.Size = new System.Drawing.Size(450, 472);
            this.listHandles.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listHandles.TabIndex = 3;
            this.listHandles.UseCompatibleStateImageBehavior = false;
            this.listHandles.View = System.Windows.Forms.View.Details;
            // 
            // columnType
            // 
            this.columnType.Text = "Type";
            this.columnType.Width = 100;
            // 
            // columnName
            // 
            this.columnName.Text = "Name";
            this.columnName.Width = 250;
            // 
            // columnHandle
            // 
            this.columnHandle.Text = "Handle";
            // 
            // vistaMenu
            // 
            this.vistaMenu.ContainerControl = this;
            // 
            // closeHandleMenuItem
            // 
            this.vistaMenu.SetImage(this.closeHandleMenuItem, global::ProcessHacker.Properties.Resources.cross);
            this.closeHandleMenuItem.Index = 0;
            this.closeHandleMenuItem.Text = "Close";
            this.closeHandleMenuItem.Click += new System.EventHandler(this.closeHandleMenuItem_Click);
            // 
            // copyHandleMenuItem
            // 
            this.vistaMenu.SetImage(this.copyHandleMenuItem, global::ProcessHacker.Properties.Resources.page_copy);
            this.copyHandleMenuItem.Index = 1;
            this.copyHandleMenuItem.Text = "&Copy";
            // 
            // menuHandle
            // 
            this.menuHandle.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.closeHandleMenuItem,
            this.copyHandleMenuItem,
            this.menuItem11,
            this.propertiesHandleMenuItem});
            this.menuHandle.Popup += new System.EventHandler(this.menuHandle_Popup);
            // 
            // menuItem11
            // 
            this.menuItem11.Index = 2;
            this.menuItem11.Text = "-";
            // 
            // propertiesHandleMenuItem
            // 
            this.propertiesHandleMenuItem.Index = 3;
            this.propertiesHandleMenuItem.Text = "&Properties...";
            this.propertiesHandleMenuItem.Click += new System.EventHandler(this.propertiesHandleMenuItem_Click);
            // 
            // HandleList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.listHandles);
            this.DoubleBuffered = true;
            this.Name = "HandleList";
            this.Size = new System.Drawing.Size(450, 472);
            ((System.ComponentModel.ISupportInitialize)(this.vistaMenu)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listHandles;
        private System.Windows.Forms.ColumnHeader columnType;
        private System.Windows.Forms.ColumnHeader columnName;
        private System.Windows.Forms.ColumnHeader columnHandle;
        private wyDay.Controls.VistaMenu vistaMenu;
        private System.Windows.Forms.ContextMenu menuHandle;
        private System.Windows.Forms.MenuItem closeHandleMenuItem;
        private System.Windows.Forms.MenuItem copyHandleMenuItem;
        private System.Windows.Forms.MenuItem menuItem11;
        private System.Windows.Forms.MenuItem propertiesHandleMenuItem;
    }
}
