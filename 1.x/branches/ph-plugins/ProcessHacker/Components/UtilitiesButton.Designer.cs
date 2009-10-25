namespace ProcessHacker.Components
{
    partial class UtilitiesButton
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
            this.buttonUtilities = new System.Windows.Forms.Button();
            this.menuUtilities = new System.Windows.Forms.ContextMenu();
            this.insertNumberMenuItem = new System.Windows.Forms.MenuItem();
            this.bitMenuItem = new System.Windows.Forms.MenuItem();
            this.bitLittleEndianMenuItem = new System.Windows.Forms.MenuItem();
            this.bitBigEndianMenuItem = new System.Windows.Forms.MenuItem();
            this.bitLittleEndianMenuItem1 = new System.Windows.Forms.MenuItem();
            this.bitBigEndianMenuItem1 = new System.Windows.Forms.MenuItem();
            this.bitLittleEndianMenuItem2 = new System.Windows.Forms.MenuItem();
            this.bitBigEndianMenuItem2 = new System.Windows.Forms.MenuItem();
            this.insertStringMenuItem = new System.Windows.Forms.MenuItem();
            this.aSCIIMenuItem = new System.Windows.Forms.MenuItem();
            this.uTF8MenuItem = new System.Windows.Forms.MenuItem();
            this.uTF16MenuItem = new System.Windows.Forms.MenuItem();
            this.uTF16BigEndianMenuItem = new System.Windows.Forms.MenuItem();
            this.uTF32MenuItem = new System.Windows.Forms.MenuItem();
            this.aSCIIMultilineMenuItem = new System.Windows.Forms.MenuItem();
            this.uTF8MultilineMenuItem = new System.Windows.Forms.MenuItem();
            this.uTF16MultilineMenuItem = new System.Windows.Forms.MenuItem();
            this.uTF16BigEndianMultilineMenuItem = new System.Windows.Forms.MenuItem();
            this.uTF32MultilineMenuItem = new System.Windows.Forms.MenuItem();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // buttonUtilities
            // 
            this.buttonUtilities.Image = global::ProcessHacker.Properties.Resources.page_gear;
            this.buttonUtilities.Location = new System.Drawing.Point(0, 0);
            this.buttonUtilities.Name = "buttonUtilities";
            this.buttonUtilities.Size = new System.Drawing.Size(24, 24);
            this.buttonUtilities.TabIndex = 0;
            this.toolTip.SetToolTip(this.buttonUtilities, "Insert Data");
            this.buttonUtilities.UseVisualStyleBackColor = true;
            this.buttonUtilities.Click += new System.EventHandler(this.buttonUtilities_Click);
            // 
            // menuUtilities
            // 
            this.menuUtilities.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.insertNumberMenuItem,
            this.insertStringMenuItem});
            // 
            // insertNumberMenuItem
            // 
            this.insertNumberMenuItem.Index = 0;
            this.insertNumberMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.bitMenuItem,
            this.bitLittleEndianMenuItem,
            this.bitBigEndianMenuItem,
            this.bitLittleEndianMenuItem1,
            this.bitBigEndianMenuItem1,
            this.bitLittleEndianMenuItem2,
            this.bitBigEndianMenuItem2});
            this.insertNumberMenuItem.Text = "Insert &Number";
            // 
            // bitMenuItem
            // 
            this.bitMenuItem.Index = 0;
            this.bitMenuItem.Text = "8-bit";
            this.bitMenuItem.Click += new System.EventHandler(this.bitMenuItem_Click);
            // 
            // bitLittleEndianMenuItem
            // 
            this.bitLittleEndianMenuItem.Index = 1;
            this.bitLittleEndianMenuItem.Text = "16-bit (little-endian)";
            this.bitLittleEndianMenuItem.Click += new System.EventHandler(this.bitLittleEndianMenuItem_Click);
            // 
            // bitBigEndianMenuItem
            // 
            this.bitBigEndianMenuItem.Index = 2;
            this.bitBigEndianMenuItem.Text = "16-bit (big-endian)";
            this.bitBigEndianMenuItem.Click += new System.EventHandler(this.bitBigEndianMenuItem_Click);
            // 
            // bitLittleEndianMenuItem1
            // 
            this.bitLittleEndianMenuItem1.Index = 3;
            this.bitLittleEndianMenuItem1.Text = "32-bit (little-endian)";
            this.bitLittleEndianMenuItem1.Click += new System.EventHandler(this.bitLittleEndianMenuItem1_Click);
            // 
            // bitBigEndianMenuItem1
            // 
            this.bitBigEndianMenuItem1.Index = 4;
            this.bitBigEndianMenuItem1.Text = "32-bit (big-endian)";
            this.bitBigEndianMenuItem1.Click += new System.EventHandler(this.bitBigEndianMenuItem1_Click);
            // 
            // bitLittleEndianMenuItem2
            // 
            this.bitLittleEndianMenuItem2.Index = 5;
            this.bitLittleEndianMenuItem2.Text = "64-bit (little-endian)";
            this.bitLittleEndianMenuItem2.Click += new System.EventHandler(this.bitLittleEndianMenuItem2_Click);
            // 
            // bitBigEndianMenuItem2
            // 
            this.bitBigEndianMenuItem2.Index = 6;
            this.bitBigEndianMenuItem2.Text = "64-bit (big-endian)";
            this.bitBigEndianMenuItem2.Click += new System.EventHandler(this.bitBigEndianMenuItem2_Click);
            // 
            // insertStringMenuItem
            // 
            this.insertStringMenuItem.Index = 1;
            this.insertStringMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.aSCIIMenuItem,
            this.uTF8MenuItem,
            this.uTF16MenuItem,
            this.uTF16BigEndianMenuItem,
            this.uTF32MenuItem,
            this.aSCIIMultilineMenuItem,
            this.uTF8MultilineMenuItem,
            this.uTF16MultilineMenuItem,
            this.uTF16BigEndianMultilineMenuItem,
            this.uTF32MultilineMenuItem});
            this.insertStringMenuItem.Text = "Insert &String";
            // 
            // aSCIIMenuItem
            // 
            this.aSCIIMenuItem.Index = 0;
            this.aSCIIMenuItem.Text = "ASCII";
            this.aSCIIMenuItem.Click += new System.EventHandler(this.aSCIIMenuItem_Click);
            // 
            // uTF8MenuItem
            // 
            this.uTF8MenuItem.Index = 1;
            this.uTF8MenuItem.Text = "UTF-8";
            this.uTF8MenuItem.Click += new System.EventHandler(this.uTF8MenuItem_Click);
            // 
            // uTF16MenuItem
            // 
            this.uTF16MenuItem.Index = 2;
            this.uTF16MenuItem.Text = "UTF-16";
            this.uTF16MenuItem.Click += new System.EventHandler(this.uTF16MenuItem_Click);
            // 
            // uTF16BigEndianMenuItem
            // 
            this.uTF16BigEndianMenuItem.Index = 3;
            this.uTF16BigEndianMenuItem.Text = "UTF-16 (big-endian)";
            this.uTF16BigEndianMenuItem.Click += new System.EventHandler(this.uTF16BigEndianMenuItem_Click);
            // 
            // uTF32MenuItem
            // 
            this.uTF32MenuItem.Index = 4;
            this.uTF32MenuItem.Text = "UTF-32";
            this.uTF32MenuItem.Click += new System.EventHandler(this.uTF32MenuItem_Click);
            // 
            // aSCIIMultilineMenuItem
            // 
            this.aSCIIMultilineMenuItem.Index = 5;
            this.aSCIIMultilineMenuItem.Text = "ASCII (multiline)";
            this.aSCIIMultilineMenuItem.Click += new System.EventHandler(this.aSCIIMultilineMenuItem_Click);
            // 
            // uTF8MultilineMenuItem
            // 
            this.uTF8MultilineMenuItem.Index = 6;
            this.uTF8MultilineMenuItem.Text = "UTF-8 (multiline)";
            this.uTF8MultilineMenuItem.Click += new System.EventHandler(this.uTF8MultilineMenuItem_Click);
            // 
            // uTF16MultilineMenuItem
            // 
            this.uTF16MultilineMenuItem.Index = 7;
            this.uTF16MultilineMenuItem.Text = "UTF-16 (multiline)";
            this.uTF16MultilineMenuItem.Click += new System.EventHandler(this.uTF16MultilineMenuItem_Click);
            // 
            // uTF16BigEndianMultilineMenuItem
            // 
            this.uTF16BigEndianMultilineMenuItem.Index = 8;
            this.uTF16BigEndianMultilineMenuItem.Text = "UTF-16 (big-endian) (multiline)";
            this.uTF16BigEndianMultilineMenuItem.Click += new System.EventHandler(this.uTF16BigEndianMultilineMenuItem_Click);
            // 
            // uTF32MultilineMenuItem
            // 
            this.uTF32MultilineMenuItem.Index = 9;
            this.uTF32MultilineMenuItem.Text = "UTF-32 (multiline)";
            this.uTF32MultilineMenuItem.Click += new System.EventHandler(this.uTF32MultilineMenuItem_Click);
            // 
            // UtilitiesButton
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.buttonUtilities);
            this.Name = "UtilitiesButton";
            this.Size = new System.Drawing.Size(24, 24);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonUtilities;
        private System.Windows.Forms.ContextMenu menuUtilities;
        private System.Windows.Forms.MenuItem insertNumberMenuItem;
        private System.Windows.Forms.MenuItem bitMenuItem;
        private System.Windows.Forms.MenuItem bitLittleEndianMenuItem;
        private System.Windows.Forms.MenuItem bitBigEndianMenuItem;
        private System.Windows.Forms.MenuItem bitLittleEndianMenuItem1;
        private System.Windows.Forms.MenuItem bitBigEndianMenuItem1;
        private System.Windows.Forms.MenuItem bitLittleEndianMenuItem2;
        private System.Windows.Forms.MenuItem bitBigEndianMenuItem2;
        private System.Windows.Forms.MenuItem insertStringMenuItem;
        private System.Windows.Forms.MenuItem aSCIIMenuItem;
        private System.Windows.Forms.MenuItem uTF8MenuItem;
        private System.Windows.Forms.MenuItem uTF16MenuItem;
        private System.Windows.Forms.MenuItem uTF16BigEndianMenuItem;
        private System.Windows.Forms.MenuItem uTF32MenuItem;
        private System.Windows.Forms.MenuItem aSCIIMultilineMenuItem;
        private System.Windows.Forms.MenuItem uTF8MultilineMenuItem;
        private System.Windows.Forms.MenuItem uTF16MultilineMenuItem;
        private System.Windows.Forms.MenuItem uTF16BigEndianMultilineMenuItem;
        private System.Windows.Forms.MenuItem uTF32MultilineMenuItem;
        private System.Windows.Forms.ToolTip toolTip;
    }
}
