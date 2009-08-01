namespace ProcessHacker
{
    partial class HeapsWindow
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
            this.buttonClose = new System.Windows.Forms.Button();
            this.listHeaps = new System.Windows.Forms.ListView();
            this.columnAddress = new System.Windows.Forms.ColumnHeader();
            this.columnUsed = new System.Windows.Forms.ColumnHeader();
            this.columnCommitted = new System.Windows.Forms.ColumnHeader();
            this.columnEntries = new System.Windows.Forms.ColumnHeader();
            this.menuHeap = new System.Windows.Forms.ContextMenu();
            this.destroyMenuItem = new System.Windows.Forms.MenuItem();
            this.copyMenuItem = new System.Windows.Forms.MenuItem();
            this.vistaMenu = new wyDay.Controls.VistaMenu(this.components);
            this.checkSizesInBytes = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.vistaMenu)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonClose.Location = new System.Drawing.Point(416, 419);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 2;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // listHeaps
            // 
            this.listHeaps.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listHeaps.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnAddress,
            this.columnUsed,
            this.columnCommitted,
            this.columnEntries});
            this.listHeaps.FullRowSelect = true;
            this.listHeaps.HideSelection = false;
            this.listHeaps.Location = new System.Drawing.Point(12, 12);
            this.listHeaps.Name = "listHeaps";
            this.listHeaps.Size = new System.Drawing.Size(479, 401);
            this.listHeaps.TabIndex = 0;
            this.listHeaps.UseCompatibleStateImageBehavior = false;
            this.listHeaps.View = System.Windows.Forms.View.Details;
            // 
            // columnAddress
            // 
            this.columnAddress.Text = "Address";
            this.columnAddress.Width = 80;
            // 
            // columnUsed
            // 
            this.columnUsed.Text = "Used";
            this.columnUsed.Width = 140;
            // 
            // columnCommitted
            // 
            this.columnCommitted.Text = "Committed";
            this.columnCommitted.Width = 140;
            // 
            // columnEntries
            // 
            this.columnEntries.Text = "Entries";
            // 
            // menuHeap
            // 
            this.menuHeap.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.destroyMenuItem,
            this.copyMenuItem});
            this.menuHeap.Popup += new System.EventHandler(this.menuHeap_Popup);
            // 
            // destroyMenuItem
            // 
            this.vistaMenu.SetImage(this.destroyMenuItem, global::ProcessHacker.Properties.Resources.cross);
            this.destroyMenuItem.Index = 0;
            this.destroyMenuItem.Text = "&Destroy";
            this.destroyMenuItem.Click += new System.EventHandler(this.destroyMenuItem_Click);
            // 
            // copyMenuItem
            // 
            this.vistaMenu.SetImage(this.copyMenuItem, global::ProcessHacker.Properties.Resources.page_copy);
            this.copyMenuItem.Index = 1;
            this.copyMenuItem.Text = "&Copy";
            // 
            // vistaMenu
            // 
            this.vistaMenu.ContainerControl = this;
            this.vistaMenu.DelaySetImageCalls = false;
            // 
            // checkSizesInBytes
            // 
            this.checkSizesInBytes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkSizesInBytes.AutoSize = true;
            this.checkSizesInBytes.Checked = true;
            this.checkSizesInBytes.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkSizesInBytes.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkSizesInBytes.Location = new System.Drawing.Point(12, 423);
            this.checkSizesInBytes.Name = "checkSizesInBytes";
            this.checkSizesInBytes.Size = new System.Drawing.Size(96, 18);
            this.checkSizesInBytes.TabIndex = 1;
            this.checkSizesInBytes.Text = "Sizes in bytes";
            this.checkSizesInBytes.UseVisualStyleBackColor = true;
            this.checkSizesInBytes.CheckedChanged += new System.EventHandler(this.checkSizesInBytes_CheckedChanged);
            // 
            // HeapsWindow
            // 
            this.AcceptButton = this.buttonClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(503, 454);
            this.Controls.Add(this.checkSizesInBytes);
            this.Controls.Add(this.listHeaps);
            this.Controls.Add(this.buttonClose);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "HeapsWindow";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Process Heaps";
            ((System.ComponentModel.ISupportInitialize)(this.vistaMenu)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.ListView listHeaps;
        private System.Windows.Forms.ColumnHeader columnAddress;
        private System.Windows.Forms.ColumnHeader columnUsed;
        private System.Windows.Forms.ColumnHeader columnCommitted;
        private System.Windows.Forms.ColumnHeader columnEntries;
        private System.Windows.Forms.ContextMenu menuHeap;
        private System.Windows.Forms.MenuItem destroyMenuItem;
        private wyDay.Controls.VistaMenu vistaMenu;
        private System.Windows.Forms.MenuItem copyMenuItem;
        private System.Windows.Forms.CheckBox checkSizesInBytes;
    }
}