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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DisassemblyWindow));
            this.listDisasm = new System.Windows.Forms.ListView();
            this.columnAddress = new System.Windows.Forms.ColumnHeader();
            this.columnRaw = new System.Windows.Forms.ColumnHeader();
            this.columnCode = new System.Windows.Forms.ColumnHeader();
            this.columnComment = new System.Windows.Forms.ColumnHeader();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.menuLine = new System.Windows.Forms.ContextMenu();
            this.followMenuItem = new System.Windows.Forms.MenuItem();
            this.copyMenuItem = new System.Windows.Forms.MenuItem();
            this.vistaMenu = new wyDay.Controls.VistaMenu(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.vistaMenu)).BeginInit();
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
            this.listDisasm.Size = new System.Drawing.Size(498, 488);
            this.listDisasm.SmallImageList = this.imageList;
            this.listDisasm.TabIndex = 0;
            this.listDisasm.UseCompatibleStateImageBehavior = false;
            this.listDisasm.View = System.Windows.Forms.View.Details;
            this.listDisasm.VirtualMode = true;
            this.listDisasm.SelectedIndexChanged += new System.EventHandler(this.listDisasm_SelectedIndexChanged);
            this.listDisasm.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.listDisasm_RetrieveVirtualItem);
            this.listDisasm.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listDisasm_KeyDown);
            // 
            // columnAddress
            // 
            this.columnAddress.Text = "Address";
            this.columnAddress.Width = 100;
            // 
            // columnRaw
            // 
            this.columnRaw.Text = "Raw";
            this.columnRaw.Width = 100;
            // 
            // columnCode
            // 
            this.columnCode.Text = "Code";
            this.columnCode.Width = 180;
            // 
            // columnComment
            // 
            this.columnComment.Text = "Comment";
            this.columnComment.Width = 100;
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "arrow_left.png");
            this.imageList.Images.SetKeyName(1, "arrow_right.png");
            this.imageList.Images.SetKeyName(2, "arrow_rotate_anticlockwise.png");
            this.imageList.Images.SetKeyName(3, "arrow_branch.png");
            this.imageList.Images.SetKeyName(4, "arrow_rotate_clockwise.png");
            // 
            // menuLine
            // 
            this.menuLine.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.followMenuItem,
            this.copyMenuItem});
            this.menuLine.Popup += new System.EventHandler(this.menuLine_Popup);
            // 
            // followMenuItem
            // 
            this.vistaMenu.SetImage(this.followMenuItem, global::ProcessHacker.Properties.Resources.arrow_right);
            this.followMenuItem.Index = 0;
            this.followMenuItem.Text = "&Follow";
            this.followMenuItem.Click += new System.EventHandler(this.followMenuItem_Click);
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
            // DisassemblyWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(522, 512);
            this.Controls.Add(this.listDisasm);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DisassemblyWindow";
            this.Text = "Disassembly";
            ((System.ComponentModel.ISupportInitialize)(this.vistaMenu)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listDisasm;
        private System.Windows.Forms.ColumnHeader columnAddress;
        private System.Windows.Forms.ColumnHeader columnRaw;
        private System.Windows.Forms.ColumnHeader columnCode;
        private System.Windows.Forms.ColumnHeader columnComment;
        private wyDay.Controls.VistaMenu vistaMenu;
        private System.Windows.Forms.ContextMenu menuLine;
        private System.Windows.Forms.MenuItem followMenuItem;
        private System.Windows.Forms.MenuItem copyMenuItem;
        private System.Windows.Forms.ImageList imageList;

    }
}