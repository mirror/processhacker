namespace ProcessHacker
{
    partial class HandleFilterWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HandleFilterWindow));
            this.label1 = new System.Windows.Forms.Label();
            this.textFilter = new System.Windows.Forms.TextBox();
            this.buttonFind = new System.Windows.Forms.Button();
            this.listHandles = new System.Windows.Forms.ListView();
            this.columnProcess = new System.Windows.Forms.ColumnHeader();
            this.columnType = new System.Windows.Forms.ColumnHeader();
            this.columnName = new System.Windows.Forms.ColumnHeader();
            this.columnHandle = new System.Windows.Forms.ColumnHeader();
            this.menuHandle = new System.Windows.Forms.ContextMenu();
            this.closeMenuItem = new System.Windows.Forms.MenuItem();
            this.processPropertiesMenuItem = new System.Windows.Forms.MenuItem();
            this.propertiesMenuItem = new System.Windows.Forms.MenuItem();
            this.copyMenuItem = new System.Windows.Forms.MenuItem();
            this.progress = new System.Windows.Forms.ProgressBar();
            this.vistaMenu = new wyDay.Controls.VistaMenu(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.vistaMenu)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Filter:";
            // 
            // textFilter
            // 
            this.textFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textFilter.Location = new System.Drawing.Point(50, 14);
            this.textFilter.Name = "textFilter";
            this.textFilter.Size = new System.Drawing.Size(395, 20);
            this.textFilter.TabIndex = 1;
            this.textFilter.TextChanged += new System.EventHandler(this.textFilter_TextChanged);
            this.textFilter.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textFilter_KeyPress);
            this.textFilter.Enter += new System.EventHandler(this.textFilter_Enter);
            // 
            // buttonFind
            // 
            this.buttonFind.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonFind.Enabled = false;
            this.buttonFind.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonFind.Location = new System.Drawing.Point(451, 12);
            this.buttonFind.Name = "buttonFind";
            this.buttonFind.Size = new System.Drawing.Size(75, 23);
            this.buttonFind.TabIndex = 2;
            this.buttonFind.Text = "&Find";
            this.buttonFind.UseVisualStyleBackColor = true;
            this.buttonFind.Click += new System.EventHandler(this.buttonFind_Click);
            // 
            // listHandles
            // 
            this.listHandles.AllowColumnReorder = true;
            this.listHandles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listHandles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnProcess,
            this.columnType,
            this.columnName,
            this.columnHandle});
            this.listHandles.FullRowSelect = true;
            this.listHandles.HideSelection = false;
            this.listHandles.Location = new System.Drawing.Point(12, 41);
            this.listHandles.Name = "listHandles";
            this.listHandles.ShowItemToolTips = true;
            this.listHandles.Size = new System.Drawing.Size(514, 374);
            this.listHandles.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listHandles.TabIndex = 3;
            this.listHandles.UseCompatibleStateImageBehavior = false;
            this.listHandles.View = System.Windows.Forms.View.Details;
            this.listHandles.DoubleClick += new System.EventHandler(this.listHandles_DoubleClick);
            this.listHandles.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listHandles_KeyDown);
            // 
            // columnProcess
            // 
            this.columnProcess.Text = "Process";
            this.columnProcess.Width = 120;
            // 
            // columnType
            // 
            this.columnType.Text = "Type";
            this.columnType.Width = 80;
            // 
            // columnName
            // 
            this.columnName.Text = "Name";
            this.columnName.Width = 240;
            // 
            // columnHandle
            // 
            this.columnHandle.Text = "Handle";
            // 
            // menuHandle
            // 
            this.menuHandle.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.closeMenuItem,
            this.processPropertiesMenuItem,
            this.propertiesMenuItem,
            this.copyMenuItem});
            this.menuHandle.Popup += new System.EventHandler(this.menuHandle_Popup);
            // 
            // closeMenuItem
            // 
            this.vistaMenu.SetImage(this.closeMenuItem, global::ProcessHacker.Properties.Resources.cross);
            this.closeMenuItem.Index = 0;
            this.closeMenuItem.Text = "Close";
            this.closeMenuItem.Click += new System.EventHandler(this.closeMenuItem_Click);
            // 
            // processPropertiesMenuItem
            // 
            this.processPropertiesMenuItem.Index = 1;
            this.processPropertiesMenuItem.Text = "Process Properties...";
            this.processPropertiesMenuItem.Click += new System.EventHandler(this.processPropertiesMenuItem_Click);
            // 
            // propertiesMenuItem
            // 
            this.propertiesMenuItem.Index = 2;
            this.propertiesMenuItem.Text = "&Properties...";
            this.propertiesMenuItem.Click += new System.EventHandler(this.propertiesMenuItem_Click);
            // 
            // copyMenuItem
            // 
            this.vistaMenu.SetImage(this.copyMenuItem, global::ProcessHacker.Properties.Resources.page_copy);
            this.copyMenuItem.Index = 3;
            this.copyMenuItem.Text = "&Copy";
            // 
            // progress
            // 
            this.progress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.progress.Location = new System.Drawing.Point(50, 11);
            this.progress.Name = "progress";
            this.progress.Size = new System.Drawing.Size(395, 23);
            this.progress.TabIndex = 4;
            this.progress.Visible = false;
            // 
            // vistaMenu
            // 
            this.vistaMenu.ContainerControl = this;
            this.vistaMenu.DelaySetImageCalls = false;
            // 
            // HandleFilterWindow
            // 
            this.AcceptButton = this.buttonFind;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(538, 427);
            this.Controls.Add(this.progress);
            this.Controls.Add(this.listHandles);
            this.Controls.Add(this.buttonFind);
            this.Controls.Add(this.textFilter);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "HandleFilterWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Find Handles or DLLs";
            this.Load += new System.EventHandler(this.HandleFilterWindow_Load);
            this.VisibleChanged += new System.EventHandler(this.HandleFilterWindow_VisibleChanged);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.HandleFilterWindow_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.vistaMenu)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textFilter;
        private System.Windows.Forms.Button buttonFind;
        private System.Windows.Forms.ListView listHandles;
        private System.Windows.Forms.ColumnHeader columnProcess;
        private System.Windows.Forms.ColumnHeader columnType;
        private System.Windows.Forms.ColumnHeader columnName;
        private System.Windows.Forms.ColumnHeader columnHandle;
        private System.Windows.Forms.ContextMenu menuHandle;
        private System.Windows.Forms.MenuItem closeMenuItem;
        private wyDay.Controls.VistaMenu vistaMenu;
        private System.Windows.Forms.MenuItem copyMenuItem;
        private System.Windows.Forms.ProgressBar progress;
        private System.Windows.Forms.MenuItem propertiesMenuItem;
        private System.Windows.Forms.MenuItem processPropertiesMenuItem;
    }
}