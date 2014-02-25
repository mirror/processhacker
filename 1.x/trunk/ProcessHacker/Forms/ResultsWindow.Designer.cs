using System;

namespace ProcessHacker
{
    partial class ResultsWindow
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
            
            if (_searchThread != null)
                _searchThread.Abort();

            _so = null;

            Program.ResultsWindows.Remove(Id);
            Program.ResultsIds.Push(_id);

            Program.CollectGarbage();

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ResultsWindow));
            this.listResults = new System.Windows.Forms.ListView();
            this.columnAddress = new System.Windows.Forms.ColumnHeader();
            this.columnOffset = new System.Windows.Forms.ColumnHeader();
            this.columnLength = new System.Windows.Forms.ColumnHeader();
            this.columnString = new System.Windows.Forms.ColumnHeader();
            this.labelText = new System.Windows.Forms.Label();
            this.mainMenu = new System.Windows.Forms.MainMenu(this.components);
            this.windowMenuItem = new System.Windows.Forms.MenuItem();
            this.buttonFilter = new System.Windows.Forms.Button();
            this.buttonIntersect = new System.Windows.Forms.Button();
            this.buttonEdit = new System.Windows.Forms.Button();
            this.buttonFind = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
            this.vistaMenu = new wyDay.Controls.VistaMenu(this.components);
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.vistaMenu)).BeginInit();
            this.SuspendLayout();
            // 
            // listResults
            // 
            this.listResults.AllowColumnReorder = true;
            this.listResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listResults.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnAddress,
            this.columnOffset,
            this.columnLength,
            this.columnString});
            this.listResults.FullRowSelect = true;
            this.listResults.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listResults.HideSelection = false;
            this.listResults.Location = new System.Drawing.Point(12, 35);
            this.listResults.Name = "listResults";
            this.listResults.ShowItemToolTips = true;
            this.listResults.Size = new System.Drawing.Size(464, 296);
            this.listResults.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listResults.TabIndex = 6;
            this.listResults.UseCompatibleStateImageBehavior = false;
            this.listResults.View = System.Windows.Forms.View.Details;
            this.listResults.VirtualMode = true;
            this.listResults.DoubleClick += new System.EventHandler(this.listResults_DoubleClick);
            this.listResults.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.listResults_RetrieveVirtualItem);
            // 
            // columnAddress
            // 
            this.columnAddress.Text = "Address";
            this.columnAddress.Width = 100;
            // 
            // columnOffset
            // 
            this.columnOffset.Text = "+ Offset";
            this.columnOffset.Width = 100;
            // 
            // columnLength
            // 
            this.columnLength.Text = "Length";
            this.columnLength.Width = 100;
            // 
            // columnString
            // 
            this.columnString.Text = "String";
            this.columnString.Width = 160;
            // 
            // labelText
            // 
            this.labelText.AutoSize = true;
            this.labelText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelText.Location = new System.Drawing.Point(72, 11);
            this.labelText.Name = "labelText";
            this.labelText.Size = new System.Drawing.Size(32, 13);
            this.labelText.TabIndex = 2;
            this.labelText.Text = "Text";
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
            // buttonFilter
            // 
            this.buttonFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonFilter.Image = global::ProcessHacker.Properties.Resources.table_sort;
            this.buttonFilter.Location = new System.Drawing.Point(392, 5);
            this.buttonFilter.Name = "buttonFilter";
            this.buttonFilter.Size = new System.Drawing.Size(24, 24);
            this.buttonFilter.TabIndex = 3;
            this.toolTip.SetToolTip(this.buttonFilter, "Filter");
            this.buttonFilter.UseVisualStyleBackColor = true;
            this.buttonFilter.Click += new System.EventHandler(this.buttonFilter_Click);
            // 
            // buttonIntersect
            // 
            this.buttonIntersect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonIntersect.Image = global::ProcessHacker.Properties.Resources.table_relationship;
            this.buttonIntersect.Location = new System.Drawing.Point(422, 5);
            this.buttonIntersect.Name = "buttonIntersect";
            this.buttonIntersect.Size = new System.Drawing.Size(24, 24);
            this.buttonIntersect.TabIndex = 4;
            this.toolTip.SetToolTip(this.buttonIntersect, "Intersect");
            this.buttonIntersect.UseVisualStyleBackColor = true;
            this.buttonIntersect.Click += new System.EventHandler(this.buttonIntersect_Click);
            // 
            // buttonEdit
            // 
            this.buttonEdit.Image = global::ProcessHacker.Properties.Resources.pencil;
            this.buttonEdit.Location = new System.Drawing.Point(42, 5);
            this.buttonEdit.Name = "buttonEdit";
            this.buttonEdit.Size = new System.Drawing.Size(24, 24);
            this.buttonEdit.TabIndex = 1;
            this.toolTip.SetToolTip(this.buttonEdit, "Edit Search...");
            this.buttonEdit.UseVisualStyleBackColor = true;
            this.buttonEdit.Click += new System.EventHandler(this.buttonEdit_Click);
            // 
            // buttonFind
            // 
            this.buttonFind.Image = global::ProcessHacker.Properties.Resources.arrow_refresh;
            this.buttonFind.Location = new System.Drawing.Point(12, 5);
            this.buttonFind.Name = "buttonFind";
            this.buttonFind.Size = new System.Drawing.Size(24, 24);
            this.buttonFind.TabIndex = 0;
            this.toolTip.SetToolTip(this.buttonFind, "Search");
            this.buttonFind.UseVisualStyleBackColor = true;
            this.buttonFind.Click += new System.EventHandler(this.buttonFind_Click);
            // 
            // buttonSave
            // 
            this.buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSave.Image = global::ProcessHacker.Properties.Resources.disk;
            this.buttonSave.Location = new System.Drawing.Point(452, 5);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(24, 24);
            this.buttonSave.TabIndex = 5;
            this.toolTip.SetToolTip(this.buttonSave, "Save...");
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // vistaMenu
            // 
            this.vistaMenu.ContainerControl = this;
            this.vistaMenu.DelaySetImageCalls = false;
            // 
            // ResultsWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(488, 343);
            this.Controls.Add(this.buttonFilter);
            this.Controls.Add(this.buttonIntersect);
            this.Controls.Add(this.buttonEdit);
            this.Controls.Add(this.buttonFind);
            this.Controls.Add(this.labelText);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.listResults);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Menu = this.mainMenu;
            this.Name = "ResultsWindow";
            this.Text = "Results";
            this.Load += new System.EventHandler(this.ResultsWindow_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ResultsWindow_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.vistaMenu)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listResults;
        private System.Windows.Forms.ColumnHeader columnAddress;
        private System.Windows.Forms.ColumnHeader columnOffset;
        private System.Windows.Forms.ColumnHeader columnLength;
        private System.Windows.Forms.ColumnHeader columnString;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Label labelText;
        private System.Windows.Forms.Button buttonFind;
        private System.Windows.Forms.Button buttonEdit;
        private System.Windows.Forms.Button buttonIntersect;
        private wyDay.Controls.VistaMenu vistaMenu;
        private System.Windows.Forms.MainMenu mainMenu;
        private System.Windows.Forms.MenuItem windowMenuItem;
        private System.Windows.Forms.Button buttonFilter;
        private System.Windows.Forms.ToolTip toolTip;
    }
}