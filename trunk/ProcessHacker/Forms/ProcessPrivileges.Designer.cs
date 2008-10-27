namespace ProcessHacker
{
    partial class ProcessPrivileges
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
            this.listPrivileges = new System.Windows.Forms.ListView();
            this.columnName = new System.Windows.Forms.ColumnHeader();
            this.columnStatus = new System.Windows.Forms.ColumnHeader();
            this.columnDesc = new System.Windows.Forms.ColumnHeader();
            this.buttonClose = new System.Windows.Forms.Button();
            this.menuPrivileges = new System.Windows.Forms.ContextMenu();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.copyMenuItem = new System.Windows.Forms.MenuItem();
            this.enableMenuItem = new System.Windows.Forms.MenuItem();
            this.disableMenuItem = new System.Windows.Forms.MenuItem();
            this.removeMenuItem = new System.Windows.Forms.MenuItem();
            this.vistaMenu = new wyDay.Controls.VistaMenu(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.vistaMenu)).BeginInit();
            this.SuspendLayout();
            // 
            // listPrivileges
            // 
            this.listPrivileges.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listPrivileges.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnName,
            this.columnStatus,
            this.columnDesc});
            this.listPrivileges.FullRowSelect = true;
            this.listPrivileges.Location = new System.Drawing.Point(12, 12);
            this.listPrivileges.Name = "listPrivileges";
            this.listPrivileges.ShowItemToolTips = true;
            this.listPrivileges.Size = new System.Drawing.Size(424, 379);
            this.listPrivileges.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listPrivileges.TabIndex = 0;
            this.listPrivileges.UseCompatibleStateImageBehavior = false;
            this.listPrivileges.View = System.Windows.Forms.View.Details;
            // 
            // columnName
            // 
            this.columnName.Text = "Name";
            this.columnName.Width = 100;
            // 
            // columnStatus
            // 
            this.columnStatus.Text = "Status";
            this.columnStatus.Width = 120;
            // 
            // columnDesc
            // 
            this.columnDesc.Text = "Description";
            this.columnDesc.Width = 190;
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.Location = new System.Drawing.Point(361, 397);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 1;
            this.buttonClose.Text = "&Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // menuPrivileges
            // 
            this.menuPrivileges.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.copyMenuItem,
            this.menuItem1,
            this.enableMenuItem,
            this.disableMenuItem,
            this.removeMenuItem});
            this.menuPrivileges.Popup += new System.EventHandler(this.menuPrivileges_Popup);
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 1;
            this.menuItem1.Text = "-";
            // 
            // copyMenuItem
            // 
            this.vistaMenu.SetImage(this.copyMenuItem, global::ProcessHacker.Properties.Resources.page_copy);
            this.copyMenuItem.Index = 0;
            this.copyMenuItem.Text = "&Copy";
            // 
            // enableMenuItem
            // 
            this.vistaMenu.SetImage(this.enableMenuItem, global::ProcessHacker.Properties.Resources.tick);
            this.enableMenuItem.Index = 2;
            this.enableMenuItem.Text = "&Enable";
            this.enableMenuItem.Click += new System.EventHandler(this.enableMenuItem_Click);
            // 
            // disableMenuItem
            // 
            this.vistaMenu.SetImage(this.disableMenuItem, global::ProcessHacker.Properties.Resources.cross);
            this.disableMenuItem.Index = 3;
            this.disableMenuItem.Text = "&Disable";
            this.disableMenuItem.Click += new System.EventHandler(this.disableMenuItem_Click);
            // 
            // removeMenuItem
            // 
            this.vistaMenu.SetImage(this.removeMenuItem, global::ProcessHacker.Properties.Resources.delete);
            this.removeMenuItem.Index = 4;
            this.removeMenuItem.Text = "&Remove";
            this.removeMenuItem.Click += new System.EventHandler(this.removeMenuItem_Click);
            // 
            // vistaMenu
            // 
            this.vistaMenu.ContainerControl = this;
            // 
            // ProcessPrivileges
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(448, 432);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.listPrivileges);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProcessPrivileges";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Privileges";
            this.Load += new System.EventHandler(this.ProcessPrivileges_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ProcessPrivileges_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.vistaMenu)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listPrivileges;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.ColumnHeader columnName;
        private System.Windows.Forms.ColumnHeader columnStatus;
        private System.Windows.Forms.ColumnHeader columnDesc;
        private System.Windows.Forms.ContextMenu menuPrivileges;
        private System.Windows.Forms.MenuItem copyMenuItem;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem enableMenuItem;
        private System.Windows.Forms.MenuItem disableMenuItem;
        private System.Windows.Forms.MenuItem removeMenuItem;
        private wyDay.Controls.VistaMenu vistaMenu;
    }
}