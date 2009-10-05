namespace ProcessHacker
{
    partial class WaitChainWindow
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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.moreInfoLink = new System.Windows.Forms.LinkLabel();
            this.waitChainListView = new ProcessHacker.ExtendedListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.BackColor = System.Drawing.SystemColors.Control;
            this.textBox1.Location = new System.Drawing.Point(12, 12);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(390, 20);
            this.textBox1.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(9, 195);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(393, 43);
            this.label1.TabIndex = 2;
            this.label1.Text = "The Analyze Wait Chain tree shows which Processes are using, or waiting to use, a" +
                " resource that is being used by another process and is required by the current p" +
                "rocess to continue.";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(327, 241);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "&Cancel";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(246, 241);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "End Thread";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // moreInfoLink
            // 
            this.moreInfoLink.AutoSize = true;
            this.moreInfoLink.Location = new System.Drawing.Point(9, 246);
            this.moreInfoLink.Name = "moreInfoLink";
            this.moreInfoLink.Size = new System.Drawing.Size(121, 13);
            this.moreInfoLink.TabIndex = 5;
            this.moreInfoLink.TabStop = true;
            this.moreInfoLink.Text = "More about Wait Chains";
            this.moreInfoLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.moreInfoLink_LinkClicked);
            // 
            // waitChainListView
            // 
            this.waitChainListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.waitChainListView.CheckBoxes = true;
            this.waitChainListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.waitChainListView.FullRowSelect = true;
            this.waitChainListView.Location = new System.Drawing.Point(12, 38);
            this.waitChainListView.Name = "waitChainListView";
            this.waitChainListView.Size = new System.Drawing.Size(390, 145);
            this.waitChainListView.TabIndex = 0;
            this.waitChainListView.UseCompatibleStateImageBehavior = false;
            this.waitChainListView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Threads";
            this.columnHeader1.Width = 353;
            // 
            // WaitChainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(414, 273);
            this.Controls.Add(this.moreInfoLink);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.waitChainListView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WaitChainWindow";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "WaitChainWindow";
            this.Load += new System.EventHandler(this.WaitChainWindow_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ExtendedListView waitChainListView;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.LinkLabel moreInfoLink;
        private System.Windows.Forms.ColumnHeader columnHeader1;
    }
}