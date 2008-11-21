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
            this.labelLink = new System.Windows.Forms.Label();
            this.buttonContinue = new System.Windows.Forms.Button();
            this.buttonQuit = new System.Windows.Forms.Button();
            this.textException = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // labelIntro
            // 
            this.labelIntro.AutoSize = true;
            this.labelIntro.Location = new System.Drawing.Point(12, 9);
            this.labelIntro.Name = "labelIntro";
            this.labelIntro.Size = new System.Drawing.Size(277, 13);
            this.labelIntro.TabIndex = 0;
            this.labelIntro.Text = "An unhandled exception has occured in Process Hacker:";
            // 
            // labelLink
            // 
            this.labelLink.AutoSize = true;
            this.labelLink.Cursor = System.Windows.Forms.Cursors.Hand;
            this.labelLink.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelLink.ForeColor = System.Drawing.Color.Blue;
            this.labelLink.Location = new System.Drawing.Point(9, 217);
            this.labelLink.Name = "labelLink";
            this.labelLink.Size = new System.Drawing.Size(349, 13);
            this.labelLink.TabIndex = 2;
            this.labelLink.Text = "Please report this bug to http://sourceforge.net/projects/processhacker.";
            this.labelLink.Click += new System.EventHandler(this.labelLink_Click);
            // 
            // buttonContinue
            // 
            this.buttonContinue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonContinue.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonContinue.Location = new System.Drawing.Point(352, 244);
            this.buttonContinue.Name = "buttonContinue";
            this.buttonContinue.Size = new System.Drawing.Size(75, 23);
            this.buttonContinue.TabIndex = 3;
            this.buttonContinue.Text = "&Continue";
            this.buttonContinue.UseVisualStyleBackColor = true;
            this.buttonContinue.Click += new System.EventHandler(this.buttonContinue_Click);
            // 
            // buttonQuit
            // 
            this.buttonQuit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonQuit.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonQuit.Location = new System.Drawing.Point(271, 244);
            this.buttonQuit.Name = "buttonQuit";
            this.buttonQuit.Size = new System.Drawing.Size(75, 23);
            this.buttonQuit.TabIndex = 4;
            this.buttonQuit.Text = "&Quit";
            this.buttonQuit.UseVisualStyleBackColor = true;
            this.buttonQuit.Click += new System.EventHandler(this.buttonQuit_Click);
            // 
            // textException
            // 
            this.textException.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textException.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textException.Location = new System.Drawing.Point(12, 25);
            this.textException.Multiline = true;
            this.textException.Name = "textException";
            this.textException.ReadOnly = true;
            this.textException.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textException.Size = new System.Drawing.Size(415, 189);
            this.textException.TabIndex = 5;
            // 
            // ErrorDialog
            // 
            this.AcceptButton = this.buttonQuit;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(439, 279);
            this.Controls.Add(this.textException);
            this.Controls.Add(this.buttonQuit);
            this.Controls.Add(this.buttonContinue);
            this.Controls.Add(this.labelLink);
            this.Controls.Add(this.labelIntro);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ErrorDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ErrorDialog";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelIntro;
        private System.Windows.Forms.Label labelLink;
        private System.Windows.Forms.Button buttonContinue;
        private System.Windows.Forms.Button buttonQuit;
        private System.Windows.Forms.TextBox textException;
    }
}