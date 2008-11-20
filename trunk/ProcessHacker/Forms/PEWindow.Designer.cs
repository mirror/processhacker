namespace ProcessHacker
{
    partial class PEWindow
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

            Program.PEWindows.Remove(Id);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PEWindow));
            this.mainMenu = new System.Windows.Forms.MainMenu(this.components);
            this.windowMenuItem = new System.Windows.Forms.MenuItem();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabCOFFHeader = new System.Windows.Forms.TabPage();
            this.tabCOFFOptionalHeader = new System.Windows.Forms.TabPage();
            this.tabSections = new System.Windows.Forms.TabPage();
            this.tabExports = new System.Windows.Forms.TabPage();
            this.tabImports = new System.Windows.Forms.TabPage();
            this.listExports = new System.Windows.Forms.ListView();
            this.columnExportName = new System.Windows.Forms.ColumnHeader();
            this.columnExportOrdinal = new System.Windows.Forms.ColumnHeader();
            this.columnExportRVA = new System.Windows.Forms.ColumnHeader();
            this.columnExportFileAddress = new System.Windows.Forms.ColumnHeader();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.listCOFFHeader = new System.Windows.Forms.ListView();
            this.columnCHName = new System.Windows.Forms.ColumnHeader();
            this.columnCHValue = new System.Windows.Forms.ColumnHeader();
            this.listCOFFOptionalHeader = new System.Windows.Forms.ListView();
            this.columnCOHName = new System.Windows.Forms.ColumnHeader();
            this.columnCOHValue = new System.Windows.Forms.ColumnHeader();
            this.listSections = new System.Windows.Forms.ListView();
            this.columnSectionName = new System.Windows.Forms.ColumnHeader();
            this.columnSectionVA = new System.Windows.Forms.ColumnHeader();
            this.columnSectionFileAddress = new System.Windows.Forms.ColumnHeader();
            this.columnSectionCharacteristics = new System.Windows.Forms.ColumnHeader();
            this.columnSectionVS = new System.Windows.Forms.ColumnHeader();
            this.vistaMenu = new wyDay.Controls.VistaMenu(this.components);
            this.tabImageData = new System.Windows.Forms.TabPage();
            this.listImageData = new System.Windows.Forms.ListView();
            this.columnIDName = new System.Windows.Forms.ColumnHeader();
            this.columnIDRVA = new System.Windows.Forms.ColumnHeader();
            this.columnIDSize = new System.Windows.Forms.ColumnHeader();
            this.tabControl.SuspendLayout();
            this.tabCOFFHeader.SuspendLayout();
            this.tabCOFFOptionalHeader.SuspendLayout();
            this.tabSections.SuspendLayout();
            this.tabExports.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.vistaMenu)).BeginInit();
            this.tabImageData.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenu
            // 
            this.mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.windowMenuItem});
            // 
            // windowMenuItem
            // 
            this.windowMenuItem.Index = 0;
            this.windowMenuItem.Text = "&Window";
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabCOFFHeader);
            this.tabControl.Controls.Add(this.tabCOFFOptionalHeader);
            this.tabControl.Controls.Add(this.tabImageData);
            this.tabControl.Controls.Add(this.tabSections);
            this.tabControl.Controls.Add(this.tabExports);
            this.tabControl.Controls.Add(this.tabImports);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(423, 429);
            this.tabControl.TabIndex = 0;
            // 
            // tabCOFFHeader
            // 
            this.tabCOFFHeader.Controls.Add(this.listCOFFHeader);
            this.tabCOFFHeader.Location = new System.Drawing.Point(4, 22);
            this.tabCOFFHeader.Name = "tabCOFFHeader";
            this.tabCOFFHeader.Padding = new System.Windows.Forms.Padding(3);
            this.tabCOFFHeader.Size = new System.Drawing.Size(415, 403);
            this.tabCOFFHeader.TabIndex = 0;
            this.tabCOFFHeader.Text = "COFF Header";
            this.tabCOFFHeader.UseVisualStyleBackColor = true;
            // 
            // tabCOFFOptionalHeader
            // 
            this.tabCOFFOptionalHeader.Controls.Add(this.listCOFFOptionalHeader);
            this.tabCOFFOptionalHeader.Location = new System.Drawing.Point(4, 22);
            this.tabCOFFOptionalHeader.Name = "tabCOFFOptionalHeader";
            this.tabCOFFOptionalHeader.Padding = new System.Windows.Forms.Padding(3);
            this.tabCOFFOptionalHeader.Size = new System.Drawing.Size(415, 403);
            this.tabCOFFOptionalHeader.TabIndex = 1;
            this.tabCOFFOptionalHeader.Text = "COFF Optional Header";
            this.tabCOFFOptionalHeader.UseVisualStyleBackColor = true;
            // 
            // tabSections
            // 
            this.tabSections.Controls.Add(this.listSections);
            this.tabSections.Location = new System.Drawing.Point(4, 22);
            this.tabSections.Name = "tabSections";
            this.tabSections.Size = new System.Drawing.Size(415, 403);
            this.tabSections.TabIndex = 2;
            this.tabSections.Text = "Sections";
            this.tabSections.UseVisualStyleBackColor = true;
            // 
            // tabExports
            // 
            this.tabExports.Controls.Add(this.listExports);
            this.tabExports.Location = new System.Drawing.Point(4, 22);
            this.tabExports.Name = "tabExports";
            this.tabExports.Padding = new System.Windows.Forms.Padding(3);
            this.tabExports.Size = new System.Drawing.Size(415, 403);
            this.tabExports.TabIndex = 3;
            this.tabExports.Text = "Exports";
            this.tabExports.UseVisualStyleBackColor = true;
            // 
            // tabImports
            // 
            this.tabImports.Location = new System.Drawing.Point(4, 22);
            this.tabImports.Name = "tabImports";
            this.tabImports.Size = new System.Drawing.Size(415, 403);
            this.tabImports.TabIndex = 4;
            this.tabImports.Text = "Imports";
            this.tabImports.UseVisualStyleBackColor = true;
            // 
            // listExports
            // 
            this.listExports.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnExportName,
            this.columnExportOrdinal,
            this.columnExportRVA,
            this.columnExportFileAddress});
            this.listExports.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listExports.FullRowSelect = true;
            this.listExports.HideSelection = false;
            this.listExports.Location = new System.Drawing.Point(3, 3);
            this.listExports.Name = "listExports";
            this.listExports.ShowItemToolTips = true;
            this.listExports.Size = new System.Drawing.Size(409, 397);
            this.listExports.SmallImageList = this.imageList;
            this.listExports.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listExports.TabIndex = 0;
            this.listExports.UseCompatibleStateImageBehavior = false;
            this.listExports.View = System.Windows.Forms.View.Details;
            this.listExports.VirtualMode = true;
            this.listExports.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.listExports_RetrieveVirtualItem);
            // 
            // columnExportName
            // 
            this.columnExportName.Text = "Name";
            this.columnExportName.Width = 120;
            // 
            // columnExportOrdinal
            // 
            this.columnExportOrdinal.Text = "Ordinal";
            // 
            // columnExportRVA
            // 
            this.columnExportRVA.Text = "RVA";
            this.columnExportRVA.Width = 80;
            // 
            // columnExportFileAddress
            // 
            this.columnExportFileAddress.Text = "File Address";
            this.columnExportFileAddress.Width = 80;
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "forwarder");
            // 
            // listCOFFHeader
            // 
            this.listCOFFHeader.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnCHName,
            this.columnCHValue});
            this.listCOFFHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listCOFFHeader.FullRowSelect = true;
            this.listCOFFHeader.HideSelection = false;
            this.listCOFFHeader.Location = new System.Drawing.Point(3, 3);
            this.listCOFFHeader.Name = "listCOFFHeader";
            this.listCOFFHeader.ShowItemToolTips = true;
            this.listCOFFHeader.Size = new System.Drawing.Size(409, 397);
            this.listCOFFHeader.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listCOFFHeader.TabIndex = 0;
            this.listCOFFHeader.UseCompatibleStateImageBehavior = false;
            this.listCOFFHeader.View = System.Windows.Forms.View.Details;
            // 
            // columnCHName
            // 
            this.columnCHName.Text = "Name";
            this.columnCHName.Width = 160;
            // 
            // columnCHValue
            // 
            this.columnCHValue.Text = "Value";
            this.columnCHValue.Width = 200;
            // 
            // listCOFFOptionalHeader
            // 
            this.listCOFFOptionalHeader.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnCOHName,
            this.columnCOHValue});
            this.listCOFFOptionalHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listCOFFOptionalHeader.FullRowSelect = true;
            this.listCOFFOptionalHeader.HideSelection = false;
            this.listCOFFOptionalHeader.Location = new System.Drawing.Point(3, 3);
            this.listCOFFOptionalHeader.Name = "listCOFFOptionalHeader";
            this.listCOFFOptionalHeader.ShowItemToolTips = true;
            this.listCOFFOptionalHeader.Size = new System.Drawing.Size(409, 397);
            this.listCOFFOptionalHeader.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listCOFFOptionalHeader.TabIndex = 1;
            this.listCOFFOptionalHeader.UseCompatibleStateImageBehavior = false;
            this.listCOFFOptionalHeader.View = System.Windows.Forms.View.Details;
            // 
            // columnCOHName
            // 
            this.columnCOHName.Text = "Name";
            this.columnCOHName.Width = 160;
            // 
            // columnCOHValue
            // 
            this.columnCOHValue.Text = "Value";
            this.columnCOHValue.Width = 200;
            // 
            // listSections
            // 
            this.listSections.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnSectionName,
            this.columnSectionVA,
            this.columnSectionVS,
            this.columnSectionFileAddress,
            this.columnSectionCharacteristics});
            this.listSections.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listSections.FullRowSelect = true;
            this.listSections.HideSelection = false;
            this.listSections.Location = new System.Drawing.Point(0, 0);
            this.listSections.Name = "listSections";
            this.listSections.ShowItemToolTips = true;
            this.listSections.Size = new System.Drawing.Size(415, 403);
            this.listSections.TabIndex = 1;
            this.listSections.UseCompatibleStateImageBehavior = false;
            this.listSections.View = System.Windows.Forms.View.Details;
            // 
            // columnSectionName
            // 
            this.columnSectionName.Text = "Name";
            this.columnSectionName.Width = 70;
            // 
            // columnSectionVA
            // 
            this.columnSectionVA.Text = "Virtual Address";
            this.columnSectionVA.Width = 80;
            // 
            // columnSectionFileAddress
            // 
            this.columnSectionFileAddress.Text = "File Address";
            this.columnSectionFileAddress.Width = 80;
            // 
            // columnSectionCharacteristics
            // 
            this.columnSectionCharacteristics.Text = "Characteristics";
            this.columnSectionCharacteristics.Width = 100;
            // 
            // columnSectionVS
            // 
            this.columnSectionVS.Text = "Virtual Size";
            // 
            // vistaMenu
            // 
            this.vistaMenu.ContainerControl = this;
            // 
            // tabImageData
            // 
            this.tabImageData.Controls.Add(this.listImageData);
            this.tabImageData.Location = new System.Drawing.Point(4, 22);
            this.tabImageData.Name = "tabImageData";
            this.tabImageData.Size = new System.Drawing.Size(415, 403);
            this.tabImageData.TabIndex = 5;
            this.tabImageData.Text = "Image Data";
            this.tabImageData.UseVisualStyleBackColor = true;
            // 
            // listImageData
            // 
            this.listImageData.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnIDName,
            this.columnIDRVA,
            this.columnIDSize});
            this.listImageData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listImageData.FullRowSelect = true;
            this.listImageData.HideSelection = false;
            this.listImageData.Location = new System.Drawing.Point(0, 0);
            this.listImageData.Name = "listImageData";
            this.listImageData.ShowItemToolTips = true;
            this.listImageData.Size = new System.Drawing.Size(415, 403);
            this.listImageData.TabIndex = 2;
            this.listImageData.UseCompatibleStateImageBehavior = false;
            this.listImageData.View = System.Windows.Forms.View.Details;
            // 
            // columnIDName
            // 
            this.columnIDName.Text = "Name";
            this.columnIDName.Width = 120;
            // 
            // columnIDRVA
            // 
            this.columnIDRVA.Text = "RVA";
            this.columnIDRVA.Width = 100;
            // 
            // columnIDSize
            // 
            this.columnIDSize.Text = "Size";
            this.columnIDSize.Width = 100;
            // 
            // PEWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(423, 429);
            this.Controls.Add(this.tabControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Menu = this.mainMenu;
            this.Name = "PEWindow";
            this.Text = "PE File";
            this.Load += new System.EventHandler(this.PEWindow_Load);
            this.tabControl.ResumeLayout(false);
            this.tabCOFFHeader.ResumeLayout(false);
            this.tabCOFFOptionalHeader.ResumeLayout(false);
            this.tabSections.ResumeLayout(false);
            this.tabExports.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.vistaMenu)).EndInit();
            this.tabImageData.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.MainMenu mainMenu;
        private System.Windows.Forms.MenuItem windowMenuItem;
        private wyDay.Controls.VistaMenu vistaMenu;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabCOFFHeader;
        private System.Windows.Forms.TabPage tabCOFFOptionalHeader;
        private System.Windows.Forms.TabPage tabSections;
        private System.Windows.Forms.TabPage tabExports;
        private System.Windows.Forms.TabPage tabImports;
        private System.Windows.Forms.ListView listExports;
        private System.Windows.Forms.ColumnHeader columnExportName;
        private System.Windows.Forms.ColumnHeader columnExportOrdinal;
        private System.Windows.Forms.ColumnHeader columnExportRVA;
        private System.Windows.Forms.ColumnHeader columnExportFileAddress;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.ListView listCOFFHeader;
        private System.Windows.Forms.ColumnHeader columnCHName;
        private System.Windows.Forms.ColumnHeader columnCHValue;
        private System.Windows.Forms.ListView listCOFFOptionalHeader;
        private System.Windows.Forms.ColumnHeader columnCOHName;
        private System.Windows.Forms.ColumnHeader columnCOHValue;
        private System.Windows.Forms.ListView listSections;
        private System.Windows.Forms.ColumnHeader columnSectionName;
        private System.Windows.Forms.ColumnHeader columnSectionVA;
        private System.Windows.Forms.ColumnHeader columnSectionFileAddress;
        private System.Windows.Forms.ColumnHeader columnSectionCharacteristics;
        private System.Windows.Forms.ColumnHeader columnSectionVS;
        private System.Windows.Forms.TabPage tabImageData;
        private System.Windows.Forms.ListView listImageData;
        private System.Windows.Forms.ColumnHeader columnIDName;
        private System.Windows.Forms.ColumnHeader columnIDRVA;
        private System.Windows.Forms.ColumnHeader columnIDSize;
    }
}