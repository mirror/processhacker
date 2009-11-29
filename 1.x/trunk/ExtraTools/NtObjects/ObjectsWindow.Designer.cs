namespace NtObjects
{
    partial class ObjectsWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ObjectsWindow));
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.treeDirectories = new System.Windows.Forms.TreeView();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.listObjects = new System.Windows.Forms.ListView();
            this.columnName = new System.Windows.Forms.ColumnHeader();
            this.columnType = new System.Windows.Forms.ColumnHeader();
            this.columnData = new System.Windows.Forms.ColumnHeader();
            this.menuObject = new System.Windows.Forms.ContextMenu();
            this.permissionsMenuItem = new System.Windows.Forms.MenuItem();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.treeDirectories);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.listObjects);
            this.splitContainer.Size = new System.Drawing.Size(794, 441);
            this.splitContainer.SplitterDistance = 264;
            this.splitContainer.TabIndex = 0;
            // 
            // treeDirectories
            // 
            this.treeDirectories.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeDirectories.HideSelection = false;
            this.treeDirectories.ImageKey = "directory";
            this.treeDirectories.ImageList = this.imageList;
            this.treeDirectories.Location = new System.Drawing.Point(0, 0);
            this.treeDirectories.Name = "treeDirectories";
            this.treeDirectories.SelectedImageKey = "directory";
            this.treeDirectories.Size = new System.Drawing.Size(264, 441);
            this.treeDirectories.TabIndex = 0;
            this.treeDirectories.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeDirectories_MouseDown);
            this.treeDirectories.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeDirectories_NodeMouseClick);
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "object");
            this.imageList.Images.SetKeyName(1, "directory");
            this.imageList.Images.SetKeyName(2, "symboliclink");
            this.imageList.Images.SetKeyName(3, "event");
            this.imageList.Images.SetKeyName(4, "mutant");
            this.imageList.Images.SetKeyName(5, "device");
            this.imageList.Images.SetKeyName(6, "key");
            this.imageList.Images.SetKeyName(7, "alpc port");
            this.imageList.Images.SetKeyName(8, "port");
            this.imageList.Images.SetKeyName(9, "section");
            this.imageList.Images.SetKeyName(10, "job");
            this.imageList.Images.SetKeyName(11, "callback");
            this.imageList.Images.SetKeyName(12, "type");
            this.imageList.Images.SetKeyName(13, "windowstation");
            this.imageList.Images.SetKeyName(14, "desktop");
            this.imageList.Images.SetKeyName(15, "filterconnectionport");
            this.imageList.Images.SetKeyName(16, "semaphore");
            this.imageList.Images.SetKeyName(17, "session");
            this.imageList.Images.SetKeyName(18, "keyedevent");
            this.imageList.Images.SetKeyName(19, "driver");
            // 
            // listObjects
            // 
            this.listObjects.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnName,
            this.columnType,
            this.columnData});
            this.listObjects.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listObjects.FullRowSelect = true;
            this.listObjects.HideSelection = false;
            this.listObjects.Location = new System.Drawing.Point(0, 0);
            this.listObjects.Name = "listObjects";
            this.listObjects.Size = new System.Drawing.Size(526, 441);
            this.listObjects.SmallImageList = this.imageList;
            this.listObjects.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listObjects.TabIndex = 0;
            this.listObjects.UseCompatibleStateImageBehavior = false;
            this.listObjects.View = System.Windows.Forms.View.Details;
            this.listObjects.DoubleClick += new System.EventHandler(this.listObjects_DoubleClick);
            // 
            // columnName
            // 
            this.columnName.Text = "Name";
            this.columnName.Width = 200;
            // 
            // columnType
            // 
            this.columnType.Text = "Type";
            this.columnType.Width = 100;
            // 
            // columnData
            // 
            this.columnData.Text = "Data";
            this.columnData.Width = 200;
            // 
            // menuObject
            // 
            this.menuObject.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.permissionsMenuItem});
            // 
            // permissionsMenuItem
            // 
            this.permissionsMenuItem.Index = 0;
            this.permissionsMenuItem.Text = "Permissions";
            this.permissionsMenuItem.Click += new System.EventHandler(this.permissionsMenuItem_Click);
            // 
            // ObjectsWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(794, 441);
            this.Controls.Add(this.splitContainer);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ObjectsWindow";
            this.Text = "NtObjects";
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.TreeView treeDirectories;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.ListView listObjects;
        private System.Windows.Forms.ColumnHeader columnName;
        private System.Windows.Forms.ColumnHeader columnType;
        private System.Windows.Forms.ColumnHeader columnData;
        private System.Windows.Forms.ContextMenu menuObject;
        private System.Windows.Forms.MenuItem permissionsMenuItem;

    }
}

