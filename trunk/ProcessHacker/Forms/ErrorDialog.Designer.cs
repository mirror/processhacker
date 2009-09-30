namespace ProcessHacker
{
    partial class ErrorDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ErrorDialog));
            this.labelIntro = new System.Windows.Forms.Label();
            this.buttonContinue = new System.Windows.Forms.Button();
            this.buttonQuit = new System.Windows.Forms.Button();
            this.textException = new System.Windows.Forms.TextBox();
            this.submitReportButton = new System.Windows.Forms.Button();
            this.statusLinkLabel = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // labelIntro
            // 
            this.labelIntro.Location = new System.Drawing.Point(12, 9);
            this.labelIntro.Name = "labelIntro";
            this.labelIntro.Size = new System.Drawing.Size(461, 45);
            this.labelIntro.TabIndex = 0;
            this.labelIntro.Text = resources.GetString("labelIntro.Text");
            // 
            // buttonContinue
            // 
            this.buttonContinue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonContinue.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonContinue.Location = new System.Drawing.Point(398, 275);
            this.buttonContinue.Name = "buttonContinue";
            this.buttonContinue.Size = new System.Drawing.Size(75, 23);
            this.buttonContinue.TabIndex = 4;
            this.buttonContinue.Text = "&Continue";
            this.buttonContinue.UseVisualStyleBackColor = true;
            this.buttonContinue.Click += new System.EventHandler(this.buttonContinue_Click);
            // 
            // buttonQuit
            // 
            this.buttonQuit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonQuit.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonQuit.Location = new System.Drawing.Point(236, 275);
            this.buttonQuit.Name = "buttonQuit";
            this.buttonQuit.Size = new System.Drawing.Size(75, 23);
            this.buttonQuit.TabIndex = 0;
            this.buttonQuit.Text = "&Quit";
            this.buttonQuit.UseVisualStyleBackColor = true;
            this.buttonQuit.Click += new System.EventHandler(this.buttonQuit_Click);
            // 
            // textException
            // 
            this.textException.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textException.Location = new System.Drawing.Point(12, 57);
            this.textException.Multiline = true;
            this.textException.Name = "textException";
            this.textException.ReadOnly = true;
            this.textException.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textException.Size = new System.Drawing.Size(461, 212);
            this.textException.TabIndex = 10;
            // 
            // submitReportButton
            // 
            this.submitReportButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.submitReportButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.submitReportButton.Location = new System.Drawing.Point(317, 275);
            this.submitReportButton.Name = "submitReportButton";
            this.submitReportButton.Size = new System.Drawing.Size(75, 23);
            this.submitReportButton.TabIndex = 5;
            this.submitReportButton.Text = "&Send Report";
            this.submitReportButton.UseVisualStyleBackColor = true;
            this.submitReportButton.Click += new System.EventHandler(this.submitReportButton_Click);
            // 
            // statusLinkLabel
            // 
            this.statusLinkLabel.AutoSize = true;
            this.statusLinkLabel.Enabled = false;
            this.statusLinkLabel.Location = new System.Drawing.Point(9, 280);
            this.statusLinkLabel.Name = "statusLinkLabel";
            this.statusLinkLabel.Size = new System.Drawing.Size(199, 13);
            this.statusLinkLabel.TabIndex = 6;
            this.statusLinkLabel.TabStop = true;
            this.statusLinkLabel.Text = "Please Wait, Reporting to Bug Tracker...";
            this.statusLinkLabel.Visible = false;
            this.statusLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.statusLinkLabel_LinkClicked);
            // 
            // ErrorDialog
            // 
            this.AcceptButton = this.buttonQuit;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(485, 310);
            this.Controls.Add(this.statusLinkLabel);
            this.Controls.Add(this.submitReportButton);
            this.Controls.Add(this.textException);
            this.Controls.Add(this.buttonQuit);
            this.Controls.Add(this.buttonContinue);
            this.Controls.Add(this.labelIntro);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ErrorDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Error";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelIntro;
        private System.Windows.Forms.Button buttonContinue;
        private System.Windows.Forms.Button buttonQuit;
        private System.Windows.Forms.TextBox textException;
        private System.Windows.Forms.Button submitReportButton;
        private System.Windows.Forms.LinkLabel statusLinkLabel;
    }
}