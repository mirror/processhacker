namespace BugCheck
{
    partial class BugCheckWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BugCheckWindow));
            this.buttonBugCheck = new System.Windows.Forms.Button();
            this.comboBugCheckCode = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textCustomCode = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textDescription = new System.Windows.Forms.TextBox();
            this.textParam1 = new System.Windows.Forms.TextBox();
            this.textParam2 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textParam3 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textParam4 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonBugCheck
            // 
            this.buttonBugCheck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBugCheck.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonBugCheck.Location = new System.Drawing.Point(335, 285);
            this.buttonBugCheck.Name = "buttonBugCheck";
            this.buttonBugCheck.Size = new System.Drawing.Size(75, 23);
            this.buttonBugCheck.TabIndex = 0;
            this.buttonBugCheck.Text = "Bug Check";
            this.buttonBugCheck.UseVisualStyleBackColor = true;
            this.buttonBugCheck.Click += new System.EventHandler(this.buttonBugCheck_Click);
            // 
            // comboBugCheckCode
            // 
            this.comboBugCheckCode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBugCheckCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBugCheckCode.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.comboBugCheckCode.FormattingEnabled = true;
            this.comboBugCheckCode.Location = new System.Drawing.Point(53, 12);
            this.comboBugCheckCode.Name = "comboBugCheckCode";
            this.comboBugCheckCode.Size = new System.Drawing.Size(357, 21);
            this.comboBugCheckCode.TabIndex = 1;
            this.comboBugCheckCode.SelectedIndexChanged += new System.EventHandler(this.comboBugCheckCode_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Code:";
            // 
            // textCustomCode
            // 
            this.textCustomCode.Enabled = false;
            this.textCustomCode.Location = new System.Drawing.Point(53, 39);
            this.textCustomCode.Name = "textCustomCode";
            this.textCustomCode.Size = new System.Drawing.Size(89, 20);
            this.textCustomCode.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 71);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Parameter 1:";
            // 
            // textDescription
            // 
            this.textDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textDescription.Location = new System.Drawing.Point(12, 172);
            this.textDescription.Multiline = true;
            this.textDescription.Name = "textDescription";
            this.textDescription.ReadOnly = true;
            this.textDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textDescription.Size = new System.Drawing.Size(398, 107);
            this.textDescription.TabIndex = 5;
            // 
            // textParam1
            // 
            this.textParam1.Location = new System.Drawing.Point(85, 68);
            this.textParam1.Name = "textParam1";
            this.textParam1.Size = new System.Drawing.Size(100, 20);
            this.textParam1.TabIndex = 6;
            this.textParam1.Text = "0";
            // 
            // textParam2
            // 
            this.textParam2.Location = new System.Drawing.Point(85, 94);
            this.textParam2.Name = "textParam2";
            this.textParam2.Size = new System.Drawing.Size(100, 20);
            this.textParam2.TabIndex = 8;
            this.textParam2.Text = "0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 98);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Parameter 2:";
            // 
            // textParam3
            // 
            this.textParam3.Location = new System.Drawing.Point(85, 120);
            this.textParam3.Name = "textParam3";
            this.textParam3.Size = new System.Drawing.Size(100, 20);
            this.textParam3.TabIndex = 10;
            this.textParam3.Text = "0";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 123);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(67, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Parameter 3:";
            // 
            // textParam4
            // 
            this.textParam4.Location = new System.Drawing.Point(85, 146);
            this.textParam4.Name = "textParam4";
            this.textParam4.Size = new System.Drawing.Size(100, 20);
            this.textParam4.TabIndex = 12;
            this.textParam4.Text = "0";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 149);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(67, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Parameter 4:";
            // 
            // BugCheckWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(422, 320);
            this.Controls.Add(this.textParam4);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textParam3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textParam2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textParam1);
            this.Controls.Add(this.textDescription);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textCustomCode);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBugCheckCode);
            this.Controls.Add(this.buttonBugCheck);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "BugCheckWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Bug Check";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonBugCheck;
        private System.Windows.Forms.ComboBox comboBugCheckCode;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textCustomCode;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textDescription;
        private System.Windows.Forms.TextBox textParam1;
        private System.Windows.Forms.TextBox textParam2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textParam3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textParam4;
        private System.Windows.Forms.Label label5;
    }
}

