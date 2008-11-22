namespace ProcessHacker
{
    partial class DisassemblyWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DisassemblyWindow));
            this.listDisasm = new System.Windows.Forms.ListView();
            this.columnAddress = new System.Windows.Forms.ColumnHeader();
            this.columnRaw = new System.Windows.Forms.ColumnHeader();
            this.columnCode = new System.Windows.Forms.ColumnHeader();
            this.columnComment = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // listDisasm
            // 
            this.listDisasm.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listDisasm.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnAddress,
            this.columnRaw,
            this.columnCode,
            this.columnComment});
            this.listDisasm.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listDisasm.FullRowSelect = true;
            this.listDisasm.GridLines = true;
            this.listDisasm.HideSelection = false;
            this.listDisasm.Location = new System.Drawing.Point(12, 12);
            this.listDisasm.Name = "listDisasm";
            this.listDisasm.ShowItemToolTips = true;
            this.listDisasm.Size = new System.Drawing.Size(498, 509);
            this.listDisasm.TabIndex = 0;
            this.listDisasm.UseCompatibleStateImageBehavior = false;
            this.listDisasm.View = System.Windows.Forms.View.Details;
            this.listDisasm.VirtualMode = true;
            this.listDisasm.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.listDisasm_RetrieveVirtualItem);
            // 
            // columnAddress
            // 
            this.columnAddress.Text = "Address";
            this.columnAddress.Width = 80;
            // 
            // columnRaw
            // 
            this.columnRaw.Text = "Raw";
            this.columnRaw.Width = 100;
            // 
            // columnCode
            // 
            this.columnCode.Text = "Code";
            this.columnCode.Width = 140;
            // 
            // columnComment
            // 
            this.columnComment.Text = "Comment";
            this.columnComment.Width = 160;
            // 
            // DisassemblyWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(522, 533);
            this.Controls.Add(this.listDisasm);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DisassemblyWindow";
            this.Text = "Disassembly";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listDisasm;
        private System.Windows.Forms.ColumnHeader columnAddress;
        private System.Windows.Forms.ColumnHeader columnRaw;
        private System.Windows.Forms.ColumnHeader columnCode;
        private System.Windows.Forms.ColumnHeader columnComment;

    }
}