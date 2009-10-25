namespace ProcessHacker.Components
{
    partial class StructViewer
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
            this.treeStruct = new Aga.Controls.Tree.TreeViewAdv();
            this.columnName = new Aga.Controls.Tree.TreeColumn();
            this.columnValue = new Aga.Controls.Tree.TreeColumn();
            this.nodeName = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeValue = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.menuStruct = new System.Windows.Forms.ContextMenu();
            this.numbersMenuItem = new System.Windows.Forms.MenuItem();
            this.decMenuItem = new System.Windows.Forms.MenuItem();
            this.hexMenuItem = new System.Windows.Forms.MenuItem();
            this.copyMenuItem = new System.Windows.Forms.MenuItem();
            this.vistaMenu = new wyDay.Controls.VistaMenu(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.vistaMenu)).BeginInit();
            this.SuspendLayout();
            // 
            // treeStruct
            // 
            this.treeStruct.BackColor = System.Drawing.SystemColors.Window;
            this.treeStruct.Columns.Add(this.columnName);
            this.treeStruct.Columns.Add(this.columnValue);
            this.treeStruct.DefaultToolTipProvider = null;
            this.treeStruct.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeStruct.DragDropMarkColor = System.Drawing.Color.Black;
            this.treeStruct.FullRowSelect = true;
            this.treeStruct.GridLineStyle = Aga.Controls.Tree.GridLineStyle.Horizontal;
            this.treeStruct.LineColor = System.Drawing.SystemColors.ControlDark;
            this.treeStruct.Location = new System.Drawing.Point(0, 0);
            this.treeStruct.Model = null;
            this.treeStruct.Name = "treeStruct";
            this.treeStruct.NodeControls.Add(this.nodeName);
            this.treeStruct.NodeControls.Add(this.nodeValue);
            this.treeStruct.SelectedNode = null;
            this.treeStruct.SelectionMode = Aga.Controls.Tree.TreeSelectionMode.Multi;
            this.treeStruct.ShowNodeToolTips = true;
            this.treeStruct.Size = new System.Drawing.Size(362, 331);
            this.treeStruct.TabIndex = 0;
            this.treeStruct.UseColumns = true;
            // 
            // columnName
            // 
            this.columnName.Header = "Name";
            this.columnName.SortOrder = System.Windows.Forms.SortOrder.None;
            this.columnName.TooltipText = null;
            this.columnName.Width = 200;
            // 
            // columnValue
            // 
            this.columnValue.Header = "Value";
            this.columnValue.SortOrder = System.Windows.Forms.SortOrder.None;
            this.columnValue.TooltipText = null;
            this.columnValue.Width = 200;
            // 
            // nodeName
            // 
            this.nodeName.DataPropertyName = "Name";
            this.nodeName.EditEnabled = false;
            this.nodeName.IncrementalSearchEnabled = true;
            this.nodeName.LeftMargin = 3;
            this.nodeName.ParentColumn = this.columnName;
            this.nodeName.Trimming = System.Drawing.StringTrimming.EllipsisCharacter;
            // 
            // nodeValue
            // 
            this.nodeValue.DataPropertyName = "Value";
            this.nodeValue.EditEnabled = false;
            this.nodeValue.IncrementalSearchEnabled = true;
            this.nodeValue.LeftMargin = 3;
            this.nodeValue.ParentColumn = this.columnValue;
            this.nodeValue.Trimming = System.Drawing.StringTrimming.EllipsisCharacter;
            // 
            // menuStruct
            // 
            this.menuStruct.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.numbersMenuItem,
            this.copyMenuItem});
            this.menuStruct.Popup += new System.EventHandler(this.menuStruct_Popup);
            // 
            // numbersMenuItem
            // 
            this.numbersMenuItem.Index = 0;
            this.numbersMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.decMenuItem,
            this.hexMenuItem});
            this.numbersMenuItem.Text = "&Numbers";
            // 
            // decMenuItem
            // 
            this.decMenuItem.Index = 0;
            this.decMenuItem.Text = "&Decimal";
            this.decMenuItem.Click += new System.EventHandler(this.decMenuItem_Click);
            // 
            // hexMenuItem
            // 
            this.hexMenuItem.Index = 1;
            this.hexMenuItem.Text = "&Hexadecimal";
            this.hexMenuItem.Click += new System.EventHandler(this.hexMenuItem_Click);
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
            // 
            // StructViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.treeStruct);
            this.Name = "StructViewer";
            this.Size = new System.Drawing.Size(362, 331);
            ((System.ComponentModel.ISupportInitialize)(this.vistaMenu)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Aga.Controls.Tree.TreeViewAdv treeStruct;
        private Aga.Controls.Tree.TreeColumn columnName;
        private Aga.Controls.Tree.TreeColumn columnValue;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeName;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeValue;
        private System.Windows.Forms.ContextMenu menuStruct;
        private System.Windows.Forms.MenuItem numbersMenuItem;
        private System.Windows.Forms.MenuItem decMenuItem;
        private System.Windows.Forms.MenuItem hexMenuItem;
        private System.Windows.Forms.MenuItem copyMenuItem;
        private wyDay.Controls.VistaMenu vistaMenu;
    }
}
