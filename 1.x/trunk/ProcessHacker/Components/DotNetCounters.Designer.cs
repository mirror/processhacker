namespace ProcessHacker.Components
{
    partial class DotNetCounters
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
            this.label1 = new System.Windows.Forms.Label();
            this.listAppDomains = new System.Windows.Forms.ListView();
            this.label2 = new System.Windows.Forms.Label();
            this.comboCategories = new System.Windows.Forms.ComboBox();
            this.listValues = new System.Windows.Forms.ListView();
            this.columnName = new System.Windows.Forms.ColumnHeader();
            this.columnValue = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "AppDomains:";
            // 
            // listAppDomains
            // 
            this.listAppDomains.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listAppDomains.FullRowSelect = true;
            this.listAppDomains.HideSelection = false;
            this.listAppDomains.Location = new System.Drawing.Point(9, 19);
            this.listAppDomains.Name = "listAppDomains";
            this.listAppDomains.ShowItemToolTips = true;
            this.listAppDomains.Size = new System.Drawing.Size(365, 80);
            this.listAppDomains.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listAppDomains.TabIndex = 1;
            this.listAppDomains.UseCompatibleStateImageBehavior = false;
            this.listAppDomains.View = System.Windows.Forms.View.List;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 108);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Categories:";
            // 
            // comboCategories
            // 
            this.comboCategories.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboCategories.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboCategories.FormattingEnabled = true;
            this.comboCategories.Location = new System.Drawing.Point(72, 105);
            this.comboCategories.Name = "comboCategories";
            this.comboCategories.Size = new System.Drawing.Size(302, 21);
            this.comboCategories.TabIndex = 3;
            this.comboCategories.SelectedIndexChanged += new System.EventHandler(this.comboCategories_SelectedIndexChanged);
            // 
            // listValues
            // 
            this.listValues.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listValues.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnName,
            this.columnValue});
            this.listValues.FullRowSelect = true;
            this.listValues.HideSelection = false;
            this.listValues.Location = new System.Drawing.Point(9, 132);
            this.listValues.Name = "listValues";
            this.listValues.Size = new System.Drawing.Size(365, 264);
            this.listValues.TabIndex = 4;
            this.listValues.UseCompatibleStateImageBehavior = false;
            this.listValues.View = System.Windows.Forms.View.Details;
            // 
            // columnName
            // 
            this.columnName.Text = "Name";
            this.columnName.Width = 140;
            // 
            // columnValue
            // 
            this.columnValue.Text = "Value";
            this.columnValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnValue.Width = 200;
            // 
            // DotNetCounters
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.listValues);
            this.Controls.Add(this.comboCategories);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.listAppDomains);
            this.Controls.Add(this.label1);
            this.Name = "DotNetCounters";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Size = new System.Drawing.Size(380, 402);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView listAppDomains;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboCategories;
        private System.Windows.Forms.ListView listValues;
        private System.Windows.Forms.ColumnHeader columnName;
        private System.Windows.Forms.ColumnHeader columnValue;
    }
}
