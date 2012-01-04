using ProcessHacker.Components;

namespace ProcessHacker
{
    sealed partial class PEWindow
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

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PEWindow));
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabCOFFHeader = new System.Windows.Forms.TabPage();
            this.listCOFFHeader = new ProcessHacker.Components.ExtendedListView();
            this.columnCHName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnCHValue = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabCOFFOptionalHeader = new System.Windows.Forms.TabPage();
            this.listCOFFOptionalHeader = new ProcessHacker.Components.ExtendedListView();
            this.columnCOHName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnCOHValue = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabImageData = new System.Windows.Forms.TabPage();
            this.listImageData = new ProcessHacker.Components.ExtendedListView();
            this.columnIDName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnIDRVA = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnIDSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabSections = new System.Windows.Forms.TabPage();
            this.listSections = new ProcessHacker.Components.ExtendedListView();
            this.columnSectionName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnSectionVA = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnSectionVS = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnSectionFileAddress = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnSectionCharacteristics = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabExports = new System.Windows.Forms.TabPage();
            this.listExports = new ProcessHacker.Components.ExtendedListView();
            this.columnExportOrdinal = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnExportName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnExportFileAddress = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabImports = new System.Windows.Forms.TabPage();
            this.listImports = new ProcessHacker.Components.ExtendedListView();
            this.columnImportName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnImportHint = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabControl.SuspendLayout();
            this.tabCOFFHeader.SuspendLayout();
            this.tabCOFFOptionalHeader.SuspendLayout();
            this.tabImageData.SuspendLayout();
            this.tabSections.SuspendLayout();
            this.tabExports.SuspendLayout();
            this.tabImports.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.tabCOFFHeader);
            this.tabControl.Controls.Add(this.tabCOFFOptionalHeader);
            this.tabControl.Controls.Add(this.tabImageData);
            this.tabControl.Controls.Add(this.tabSections);
            this.tabControl.Controls.Add(this.tabExports);
            this.tabControl.Controls.Add(this.tabImports);
            this.tabControl.Location = new System.Drawing.Point(12, 12);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(439, 465);
            this.tabControl.TabIndex = 0;
            // 
            // tabCOFFHeader
            // 
            this.tabCOFFHeader.Controls.Add(this.listCOFFHeader);
            this.tabCOFFHeader.Location = new System.Drawing.Point(4, 22);
            this.tabCOFFHeader.Name = "tabCOFFHeader";
            this.tabCOFFHeader.Padding = new System.Windows.Forms.Padding(3);
            this.tabCOFFHeader.Size = new System.Drawing.Size(431, 439);
            this.tabCOFFHeader.TabIndex = 0;
            this.tabCOFFHeader.Text = "COFF Header";
            this.tabCOFFHeader.UseVisualStyleBackColor = true;
            // 
            // listCOFFHeader
            // 
            this.listCOFFHeader.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnCHName,
            this.columnCHValue});
            this.listCOFFHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listCOFFHeader.DoubleClickChecks = true;
            this.listCOFFHeader.FullRowSelect = true;
            this.listCOFFHeader.HideSelection = false;
            this.listCOFFHeader.Location = new System.Drawing.Point(3, 3);
            this.listCOFFHeader.Name = "listCOFFHeader";
            this.listCOFFHeader.ShowItemToolTips = true;
            this.listCOFFHeader.Size = new System.Drawing.Size(425, 433);
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
            // tabCOFFOptionalHeader
            // 
            this.tabCOFFOptionalHeader.Controls.Add(this.listCOFFOptionalHeader);
            this.tabCOFFOptionalHeader.Location = new System.Drawing.Point(4, 22);
            this.tabCOFFOptionalHeader.Name = "tabCOFFOptionalHeader";
            this.tabCOFFOptionalHeader.Padding = new System.Windows.Forms.Padding(3);
            this.tabCOFFOptionalHeader.Size = new System.Drawing.Size(415, 376);
            this.tabCOFFOptionalHeader.TabIndex = 1;
            this.tabCOFFOptionalHeader.Text = "COFF Optional Header";
            this.tabCOFFOptionalHeader.UseVisualStyleBackColor = true;
            // 
            // listCOFFOptionalHeader
            // 
            this.listCOFFOptionalHeader.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnCOHName,
            this.columnCOHValue});
            this.listCOFFOptionalHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listCOFFOptionalHeader.DoubleClickChecks = true;
            this.listCOFFOptionalHeader.FullRowSelect = true;
            this.listCOFFOptionalHeader.HideSelection = false;
            this.listCOFFOptionalHeader.Location = new System.Drawing.Point(3, 3);
            this.listCOFFOptionalHeader.Name = "listCOFFOptionalHeader";
            this.listCOFFOptionalHeader.ShowItemToolTips = true;
            this.listCOFFOptionalHeader.Size = new System.Drawing.Size(409, 370);
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
            // tabImageData
            // 
            this.tabImageData.Controls.Add(this.listImageData);
            this.tabImageData.Location = new System.Drawing.Point(4, 22);
            this.tabImageData.Name = "tabImageData";
            this.tabImageData.Padding = new System.Windows.Forms.Padding(3);
            this.tabImageData.Size = new System.Drawing.Size(415, 376);
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
            this.listImageData.DoubleClickChecks = true;
            this.listImageData.FullRowSelect = true;
            this.listImageData.HideSelection = false;
            this.listImageData.Location = new System.Drawing.Point(3, 3);
            this.listImageData.Name = "listImageData";
            this.listImageData.ShowItemToolTips = true;
            this.listImageData.Size = new System.Drawing.Size(409, 370);
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
            // tabSections
            // 
            this.tabSections.Controls.Add(this.listSections);
            this.tabSections.Location = new System.Drawing.Point(4, 22);
            this.tabSections.Name = "tabSections";
            this.tabSections.Padding = new System.Windows.Forms.Padding(3);
            this.tabSections.Size = new System.Drawing.Size(415, 376);
            this.tabSections.TabIndex = 2;
            this.tabSections.Text = "Sections";
            this.tabSections.UseVisualStyleBackColor = true;
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
            this.listSections.DoubleClickChecks = true;
            this.listSections.FullRowSelect = true;
            this.listSections.HideSelection = false;
            this.listSections.Location = new System.Drawing.Point(3, 3);
            this.listSections.Name = "listSections";
            this.listSections.ShowItemToolTips = true;
            this.listSections.Size = new System.Drawing.Size(409, 370);
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
            // columnSectionVS
            // 
            this.columnSectionVS.Text = "Virtual Size";
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
            // tabExports
            // 
            this.tabExports.Controls.Add(this.listExports);
            this.tabExports.Location = new System.Drawing.Point(4, 22);
            this.tabExports.Name = "tabExports";
            this.tabExports.Padding = new System.Windows.Forms.Padding(3);
            this.tabExports.Size = new System.Drawing.Size(415, 376);
            this.tabExports.TabIndex = 3;
            this.tabExports.Text = "Exports";
            this.tabExports.UseVisualStyleBackColor = true;
            // 
            // listExports
            // 
            this.listExports.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnExportOrdinal,
            this.columnExportName,
            this.columnExportFileAddress});
            this.listExports.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listExports.DoubleClickChecks = true;
            this.listExports.FullRowSelect = true;
            this.listExports.HideSelection = false;
            this.listExports.Location = new System.Drawing.Point(3, 3);
            this.listExports.Name = "listExports";
            this.listExports.ShowItemToolTips = true;
            this.listExports.Size = new System.Drawing.Size(409, 370);
            this.listExports.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listExports.TabIndex = 0;
            this.listExports.UseCompatibleStateImageBehavior = false;
            this.listExports.View = System.Windows.Forms.View.Details;
            this.listExports.VirtualMode = true;
            this.listExports.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.listExports_RetrieveVirtualItem);
            this.listExports.DoubleClick += new System.EventHandler(this.listExports_DoubleClick);
            // 
            // columnExportOrdinal
            // 
            this.columnExportOrdinal.Text = "Ordinal";
            // 
            // columnExportName
            // 
            this.columnExportName.Text = "Name";
            this.columnExportName.Width = 220;
            // 
            // columnExportFileAddress
            // 
            this.columnExportFileAddress.Text = "File Address";
            this.columnExportFileAddress.Width = 80;
            // 
            // tabImports
            // 
            this.tabImports.Controls.Add(this.listImports);
            this.tabImports.Location = new System.Drawing.Point(4, 22);
            this.tabImports.Name = "tabImports";
            this.tabImports.Padding = new System.Windows.Forms.Padding(3);
            this.tabImports.Size = new System.Drawing.Size(415, 376);
            this.tabImports.TabIndex = 4;
            this.tabImports.Text = "Imports";
            this.tabImports.UseVisualStyleBackColor = true;
            // 
            // listImports
            // 
            this.listImports.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnImportName,
            this.columnImportHint});
            this.listImports.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listImports.DoubleClickChecks = true;
            this.listImports.FullRowSelect = true;
            this.listImports.HideSelection = false;
            this.listImports.Location = new System.Drawing.Point(3, 3);
            this.listImports.Name = "listImports";
            this.listImports.ShowItemToolTips = true;
            this.listImports.Size = new System.Drawing.Size(409, 370);
            this.listImports.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listImports.TabIndex = 2;
            this.listImports.UseCompatibleStateImageBehavior = false;
            this.listImports.View = System.Windows.Forms.View.Details;
            this.listImports.GroupLinkClicked += new ProcessHacker.Components.LinkClickedEventHandler(this.listImports_GroupLinkClicked);
            // 
            // columnImportName
            // 
            this.columnImportName.Text = "Name";
            this.columnImportName.Width = 160;
            // 
            // columnImportHint
            // 
            this.columnImportHint.Text = "Hint";
            this.columnImportHint.Width = 80;
            // 
            // PEWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(463, 489);
            this.Controls.Add(this.tabControl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PEWindow";
            this.Text = "PE File";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PEWindow_FormClosing);
            this.Load += new System.EventHandler(this.PEWindow_Load);
            this.tabControl.ResumeLayout(false);
            this.tabCOFFHeader.ResumeLayout(false);
            this.tabCOFFOptionalHeader.ResumeLayout(false);
            this.tabImageData.ResumeLayout(false);
            this.tabSections.ResumeLayout(false);
            this.tabExports.ResumeLayout(false);
            this.tabImports.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabCOFFHeader;
        private System.Windows.Forms.TabPage tabCOFFOptionalHeader;
        private System.Windows.Forms.TabPage tabSections;
        private System.Windows.Forms.TabPage tabExports;
        private System.Windows.Forms.TabPage tabImports;
        private ExtendedListView listExports;
        private System.Windows.Forms.ColumnHeader columnExportName;
        private System.Windows.Forms.ColumnHeader columnExportOrdinal;
        private System.Windows.Forms.ColumnHeader columnExportFileAddress;
        private ExtendedListView listCOFFHeader;
        private System.Windows.Forms.ColumnHeader columnCHName;
        private System.Windows.Forms.ColumnHeader columnCHValue;
        private ExtendedListView listCOFFOptionalHeader;
        private System.Windows.Forms.ColumnHeader columnCOHName;
        private System.Windows.Forms.ColumnHeader columnCOHValue;
        private ExtendedListView listSections;
        private System.Windows.Forms.ColumnHeader columnSectionName;
        private System.Windows.Forms.ColumnHeader columnSectionVA;
        private System.Windows.Forms.ColumnHeader columnSectionFileAddress;
        private System.Windows.Forms.ColumnHeader columnSectionCharacteristics;
        private System.Windows.Forms.ColumnHeader columnSectionVS;
        private System.Windows.Forms.TabPage tabImageData;
        private ExtendedListView listImageData;
        private System.Windows.Forms.ColumnHeader columnIDName;
        private System.Windows.Forms.ColumnHeader columnIDRVA;
        private System.Windows.Forms.ColumnHeader columnIDSize;
        private ProcessHacker.Components.ExtendedListView listImports;
        private System.Windows.Forms.ColumnHeader columnImportName;
        private System.Windows.Forms.ColumnHeader columnImportHint;
    }
}