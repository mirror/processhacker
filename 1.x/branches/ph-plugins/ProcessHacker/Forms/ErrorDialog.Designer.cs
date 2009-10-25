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
            this.labelIntro = new System.Windows.Forms.Label();
            this.buttonContinue = new System.Windows.Forms.Button();
            this.buttonQuit = new System.Windows.Forms.Button();
            this.textException = new System.Windows.Forms.TextBox();
            this.buttonSubmitReport = new System.Windows.Forms.Button();
            this.statusLinkLabel = new System.Windows.Forms.LinkLabel();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelIntro
            // 
            this.labelIntro.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelIntro.Location = new System.Drawing.Point(12, 26);
            this.labelIntro.Name = "labelIntro";
            this.labelIntro.Size = new System.Drawing.Size(484, 32);
            this.labelIntro.TabIndex = 0;
            this.labelIntro.Text = "Please report this error to the Process Hacker team via our bug tracker hosted at" +
                " SourceForge by clicking Send Report. You will recieve a tracker item for keeing" +
                " track of its resolution status.";
            // 
            // buttonContinue
            // 
            this.buttonContinue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonContinue.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonContinue.Location = new System.Drawing.Point(421, 331);
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
            this.buttonQuit.Location = new System.Drawing.Point(259, 331);
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
            this.textException.BackColor = System.Drawing.SystemColors.Control;
            this.textException.Location = new System.Drawing.Point(12, 61);
            this.textException.Multiline = true;
            this.textException.Name = "textException";
            this.textException.ReadOnly = true;
            this.textException.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textException.Size = new System.Drawing.Size(484, 256);
            this.textException.TabIndex = 10;
            // 
            // buttonSubmitReport
            // 
            this.buttonSubmitReport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSubmitReport.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonSubmitReport.Location = new System.Drawing.Point(340, 331);
            this.buttonSubmitReport.Name = "buttonSubmitReport";
            this.buttonSubmitReport.Size = new System.Drawing.Size(75, 23);
            this.buttonSubmitReport.TabIndex = 5;
            this.buttonSubmitReport.Text = "&Send Report";
            this.buttonSubmitReport.UseVisualStyleBackColor = true;
            this.buttonSubmitReport.Click += new System.EventHandler(this.submitReportButton_Click);
            // 
            // statusLinkLabel
            // 
            this.statusLinkLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.statusLinkLabel.AutoSize = true;
            this.statusLinkLabel.Enabled = false;
            this.statusLinkLabel.Location = new System.Drawing.Point(12, 336);
            this.statusLinkLabel.Name = "statusLinkLabel";
            this.statusLinkLabel.Size = new System.Drawing.Size(199, 13);
            this.statusLinkLabel.TabIndex = 6;
            this.statusLinkLabel.TabStop = true;
            this.statusLinkLabel.Text = "Please Wait, Reporting to Bug Tracker...";
            this.statusLinkLabel.Visible = false;
            this.statusLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.statusLinkLabel_LinkClicked);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(334, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "An unhandled exception has occured in Process Hacker. ";
            // 
            // ErrorDialog
            // 
            this.AcceptButton = this.buttonQuit;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(508, 366);
            this.Controls.Add(this.labelIntro);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.statusLinkLabel);
            this.Controls.Add(this.buttonSubmitReport);
            this.Controls.Add(this.textException);
            this.Controls.Add(this.buttonQuit);
            this.Controls.Add(this.buttonContinue);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(524, 200);
            this.Name = "ErrorDialog";
            this.ShowIcon = false;
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
        private System.Windows.Forms.Button buttonSubmitReport;
        private System.Windows.Forms.LinkLabel statusLinkLabel;
        private System.Windows.Forms.Label label1;
    }
}