namespace ProcessHacker
{
    partial class RunWindow
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
            this.label1 = new System.Windows.Forms.Label();
            this.textCmdLine = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.comboUsername = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textSessionID = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textPassword = new System.Windows.Forms.TextBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.buttonSessions = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.comboType = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 49);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Program:";
            // 
            // textCmdLine
            // 
            this.textCmdLine.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textCmdLine.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.textCmdLine.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.AllSystemSources;
            this.textCmdLine.Location = new System.Drawing.Point(79, 46);
            this.textCmdLine.Name = "textCmdLine";
            this.textCmdLine.Size = new System.Drawing.Size(230, 20);
            this.textCmdLine.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 75);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Username:";
            // 
            // comboUsername
            // 
            this.comboUsername.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.comboUsername.FormattingEnabled = true;
            this.comboUsername.Location = new System.Drawing.Point(79, 72);
            this.comboUsername.Name = "comboUsername";
            this.comboUsername.Size = new System.Drawing.Size(154, 21);
            this.comboUsername.TabIndex = 2;
            this.comboUsername.TextChanged += new System.EventHandler(this.comboUsername_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 128);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(61, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "Session ID:";
            // 
            // textSessionID
            // 
            this.textSessionID.Location = new System.Drawing.Point(79, 125);
            this.textSessionID.Name = "textSessionID";
            this.textSessionID.Size = new System.Drawing.Size(100, 20);
            this.textSessionID.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 102);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "Password:";
            // 
            // textPassword
            // 
            this.textPassword.Location = new System.Drawing.Point(79, 99);
            this.textPassword.Name = "textPassword";
            this.textPassword.Size = new System.Drawing.Size(154, 20);
            this.textPassword.TabIndex = 4;
            this.textPassword.UseSystemPasswordChar = true;
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonOK.Location = new System.Drawing.Point(234, 155);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 7;
            this.buttonOK.Text = "&OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonCancel.Location = new System.Drawing.Point(315, 155);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 8;
            this.buttonCancel.Text = "&Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBrowse.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonBrowse.Location = new System.Drawing.Point(315, 44);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(75, 23);
            this.buttonBrowse.TabIndex = 1;
            this.buttonBrowse.Text = "&Browse...";
            this.buttonBrowse.UseVisualStyleBackColor = true;
            this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.Location = new System.Drawing.Point(12, 9);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(378, 30);
            this.label5.TabIndex = 9;
            this.label5.Text = "Enter the command to start as the specified user. Note that the program may take " +
                "a while to start as Windows loads the user\'s profile.";
            // 
            // buttonSessions
            // 
            this.buttonSessions.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonSessions.Location = new System.Drawing.Point(185, 122);
            this.buttonSessions.Name = "buttonSessions";
            this.buttonSessions.Size = new System.Drawing.Size(24, 24);
            this.buttonSessions.TabIndex = 6;
            this.buttonSessions.Text = "...";
            this.buttonSessions.UseVisualStyleBackColor = true;
            this.buttonSessions.Click += new System.EventHandler(this.buttonSessions_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(239, 75);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(34, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "Type:";
            // 
            // comboType
            // 
            this.comboType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboType.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.comboType.FormattingEnabled = true;
            this.comboType.Items.AddRange(new object[] {
            "Batch",
            "Interactive",
            "Network",
            "NetworkCleartext",
            "NewCredentials",
            "Service",
            "Unlock"});
            this.comboType.Location = new System.Drawing.Point(279, 72);
            this.comboType.Name = "comboType";
            this.comboType.Size = new System.Drawing.Size(111, 21);
            this.comboType.TabIndex = 3;
            // 
            // RunWindow
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(402, 190);
            this.Controls.Add(this.comboType);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.buttonSessions);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.buttonBrowse);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.textPassword);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textSessionID);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.comboUsername);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textCmdLine);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RunWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Run As...";
            this.Load += new System.EventHandler(this.RunWindow_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RunWindow_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textCmdLine;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboUsername;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textSessionID;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textPassword;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonBrowse;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button buttonSessions;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox comboType;
    }
}