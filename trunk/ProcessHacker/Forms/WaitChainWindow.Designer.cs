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
            this.textDescription = new System.Windows.Forms.TextBox();
            this.labelIntro = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonEndThread = new System.Windows.Forms.Button();
            this.moreInfoLink = new System.Windows.Forms.LinkLabel();
            this.listWaitChain = new ProcessHacker.ExtendedListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // textDescription
            // 
            this.textDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textDescription.BackColor = System.Drawing.SystemColors.Control;
            this.textDescription.Location = new System.Drawing.Point(12, 12);
            this.textDescription.Name = "textDescription";
            this.textDescription.Size = new System.Drawing.Size(390, 20);
            this.textDescription.TabIndex = 1;
            // 
            // labelIntro
            // 
            this.labelIntro.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelIntro.Location = new System.Drawing.Point(9, 233);
            this.labelIntro.Name = "labelIntro";
            this.labelIntro.Size = new System.Drawing.Size(393, 43);
            this.labelIntro.TabIndex = 2;
            this.labelIntro.Text = "The Analyze Wait Chain tree shows which processes are using or waiting to use a r" +
                "esource that is being used by another process and is required by the current pro" +
                "cess to continue.";
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonCancel.Location = new System.Drawing.Point(327, 276);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 3;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonEndThread
            // 
            this.buttonEndThread.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonEndThread.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonEndThread.Location = new System.Drawing.Point(246, 276);
            this.buttonEndThread.Name = "buttonEndThread";
            this.buttonEndThread.Size = new System.Drawing.Size(75, 23);
            this.buttonEndThread.TabIndex = 4;
            this.buttonEndThread.Text = "End Thread";
            this.buttonEndThread.UseVisualStyleBackColor = true;
            // 
            // moreInfoLink
            // 
            this.moreInfoLink.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.moreInfoLink.AutoSize = true;
            this.moreInfoLink.Location = new System.Drawing.Point(12, 281);
            this.moreInfoLink.Name = "moreInfoLink";
            this.moreInfoLink.Size = new System.Drawing.Size(121, 13);
            this.moreInfoLink.TabIndex = 5;
            this.moreInfoLink.TabStop = true;
            this.moreInfoLink.Text = "More about Wait Chains";
            this.moreInfoLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.moreInfoLink_LinkClicked);
            // 
            // listWaitChain
            // 
            this.listWaitChain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listWaitChain.CheckBoxes = true;
            this.listWaitChain.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.listWaitChain.FullRowSelect = true;
            this.listWaitChain.Location = new System.Drawing.Point(12, 38);
            this.listWaitChain.Name = "listWaitChain";
            this.listWaitChain.Size = new System.Drawing.Size(390, 183);
            this.listWaitChain.TabIndex = 0;
            this.listWaitChain.UseCompatibleStateImageBehavior = false;
            this.listWaitChain.View = System.Windows.Forms.View.Details;
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
            this.ClientSize = new System.Drawing.Size(414, 311);
            this.Controls.Add(this.moreInfoLink);
            this.Controls.Add(this.buttonEndThread);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.labelIntro);
            this.Controls.Add(this.textDescription);
            this.Controls.Add(this.listWaitChain);
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

        private ExtendedListView listWaitChain;
        private System.Windows.Forms.TextBox textDescription;
        private System.Windows.Forms.Label labelIntro;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonEndThread;
        private System.Windows.Forms.LinkLabel moreInfoLink;
        private System.Windows.Forms.ColumnHeader columnHeader1;
    }
}