namespace ProcessHacker
{
    partial class SearchWindow
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
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabLiteral = new System.Windows.Forms.TabPage();
            this.checkNoOverlap = new System.Windows.Forms.CheckBox();
            this.utilitiesButtonLiteral = new ProcessHacker.Components.UtilitiesButton();
            this.hexBoxSearch = new Be.Windows.Forms.HexBox();
            this.tabRegex = new System.Windows.Forms.TabPage();
            this.checkIgnoreCase = new System.Windows.Forms.CheckBox();
            this.textRegex = new System.Windows.Forms.TextBox();
            this.tabString = new System.Windows.Forms.TabPage();
            this.checkUnicode = new System.Windows.Forms.CheckBox();
            this.textStringMS = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tabHeap = new System.Windows.Forms.TabPage();
            this.textHeapMS = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tabStruct = new System.Windows.Forms.TabPage();
            this.textStructAlign = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.listStructName = new System.Windows.Forms.ListBox();
            this.checkPrivate = new System.Windows.Forms.CheckBox();
            this.checkImage = new System.Windows.Forms.CheckBox();
            this.checkMapped = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.tabControl.SuspendLayout();
            this.tabLiteral.SuspendLayout();
            this.tabRegex.SuspendLayout();
            this.tabString.SuspendLayout();
            this.tabHeap.SuspendLayout();
            this.tabStruct.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.tabLiteral);
            this.tabControl.Controls.Add(this.tabRegex);
            this.tabControl.Controls.Add(this.tabString);
            this.tabControl.Controls.Add(this.tabHeap);
            this.tabControl.Controls.Add(this.tabStruct);
            this.tabControl.Location = new System.Drawing.Point(12, 12);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(493, 311);
            this.tabControl.TabIndex = 0;
            this.tabControl.SelectedIndexChanged += new System.EventHandler(this.tabControl_SelectedIndexChanged);
            // 
            // tabLiteral
            // 
            this.tabLiteral.Controls.Add(this.checkNoOverlap);
            this.tabLiteral.Controls.Add(this.utilitiesButtonLiteral);
            this.tabLiteral.Controls.Add(this.hexBoxSearch);
            this.tabLiteral.Location = new System.Drawing.Point(4, 22);
            this.tabLiteral.Name = "tabLiteral";
            this.tabLiteral.Padding = new System.Windows.Forms.Padding(3);
            this.tabLiteral.Size = new System.Drawing.Size(485, 285);
            this.tabLiteral.TabIndex = 0;
            this.tabLiteral.Text = "Literal Search";
            this.tabLiteral.UseVisualStyleBackColor = true;
            // 
            // checkNoOverlap
            // 
            this.checkNoOverlap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkNoOverlap.AutoSize = true;
            this.checkNoOverlap.Location = new System.Drawing.Point(6, 262);
            this.checkNoOverlap.Name = "checkNoOverlap";
            this.checkNoOverlap.Size = new System.Drawing.Size(154, 17);
            this.checkNoOverlap.TabIndex = 1;
            this.checkNoOverlap.Text = "Prevent overlapping results";
            this.checkNoOverlap.UseVisualStyleBackColor = true;
            // 
            // utilitiesButtonLiteral
            // 
            this.utilitiesButtonLiteral.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.utilitiesButtonLiteral.HexBox = this.hexBoxSearch;
            this.utilitiesButtonLiteral.Location = new System.Drawing.Point(455, 255);
            this.utilitiesButtonLiteral.Name = "utilitiesButtonLiteral";
            this.utilitiesButtonLiteral.Size = new System.Drawing.Size(24, 24);
            this.utilitiesButtonLiteral.TabIndex = 2;
            // 
            // hexBoxSearch
            // 
            this.hexBoxSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.hexBoxSearch.BytesPerLine = 8;
            this.hexBoxSearch.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.hexBoxSearch.HexCasing = Be.Windows.Forms.HexCasing.Lower;
            this.hexBoxSearch.LineInfoForeColor = System.Drawing.Color.Empty;
            this.hexBoxSearch.LineInfoVisible = true;
            this.hexBoxSearch.Location = new System.Drawing.Point(6, 6);
            this.hexBoxSearch.Name = "hexBoxSearch";
            this.hexBoxSearch.ShadowSelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(60)))), ((int)(((byte)(188)))), ((int)(((byte)(255)))));
            this.hexBoxSearch.Size = new System.Drawing.Size(473, 243);
            this.hexBoxSearch.StringViewVisible = true;
            this.hexBoxSearch.TabIndex = 0;
            this.hexBoxSearch.UseFixedBytesPerLine = true;
            this.hexBoxSearch.VScrollBarVisible = true;
            // 
            // tabRegex
            // 
            this.tabRegex.Controls.Add(this.checkIgnoreCase);
            this.tabRegex.Controls.Add(this.textRegex);
            this.tabRegex.Location = new System.Drawing.Point(4, 22);
            this.tabRegex.Name = "tabRegex";
            this.tabRegex.Padding = new System.Windows.Forms.Padding(3);
            this.tabRegex.Size = new System.Drawing.Size(485, 285);
            this.tabRegex.TabIndex = 1;
            this.tabRegex.Text = "Regex Search";
            this.tabRegex.UseVisualStyleBackColor = true;
            // 
            // checkIgnoreCase
            // 
            this.checkIgnoreCase.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkIgnoreCase.AutoSize = true;
            this.checkIgnoreCase.Location = new System.Drawing.Point(6, 262);
            this.checkIgnoreCase.Name = "checkIgnoreCase";
            this.checkIgnoreCase.Size = new System.Drawing.Size(83, 17);
            this.checkIgnoreCase.TabIndex = 1;
            this.checkIgnoreCase.Text = "Ignore Case";
            this.checkIgnoreCase.UseVisualStyleBackColor = true;
            // 
            // textRegex
            // 
            this.textRegex.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textRegex.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textRegex.Location = new System.Drawing.Point(6, 6);
            this.textRegex.Multiline = true;
            this.textRegex.Name = "textRegex";
            this.textRegex.Size = new System.Drawing.Size(473, 250);
            this.textRegex.TabIndex = 0;
            // 
            // tabString
            // 
            this.tabString.Controls.Add(this.checkUnicode);
            this.tabString.Controls.Add(this.textStringMS);
            this.tabString.Controls.Add(this.label2);
            this.tabString.Location = new System.Drawing.Point(4, 22);
            this.tabString.Name = "tabString";
            this.tabString.Padding = new System.Windows.Forms.Padding(5);
            this.tabString.Size = new System.Drawing.Size(485, 285);
            this.tabString.TabIndex = 2;
            this.tabString.Text = "String Scan";
            this.tabString.UseVisualStyleBackColor = true;
            // 
            // checkUnicode
            // 
            this.checkUnicode.AutoSize = true;
            this.checkUnicode.Location = new System.Drawing.Point(8, 34);
            this.checkUnicode.Name = "checkUnicode";
            this.checkUnicode.Size = new System.Drawing.Size(122, 17);
            this.checkUnicode.TabIndex = 1;
            this.checkUnicode.Text = "Find Unicode strings";
            this.checkUnicode.UseVisualStyleBackColor = true;
            // 
            // textStringMS
            // 
            this.textStringMS.Location = new System.Drawing.Point(88, 8);
            this.textStringMS.Name = "textStringMS";
            this.textStringMS.Size = new System.Drawing.Size(100, 20);
            this.textStringMS.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Minimum Size:";
            // 
            // tabHeap
            // 
            this.tabHeap.Controls.Add(this.textHeapMS);
            this.tabHeap.Controls.Add(this.label4);
            this.tabHeap.Location = new System.Drawing.Point(4, 22);
            this.tabHeap.Name = "tabHeap";
            this.tabHeap.Padding = new System.Windows.Forms.Padding(5);
            this.tabHeap.Size = new System.Drawing.Size(485, 285);
            this.tabHeap.TabIndex = 3;
            this.tabHeap.Text = "Heap Scan";
            this.tabHeap.UseVisualStyleBackColor = true;
            // 
            // textHeapMS
            // 
            this.textHeapMS.Location = new System.Drawing.Point(88, 8);
            this.textHeapMS.Name = "textHeapMS";
            this.textHeapMS.Size = new System.Drawing.Size(100, 20);
            this.textHeapMS.TabIndex = 0;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 11);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(74, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Minimum Size:";
            // 
            // tabStruct
            // 
            this.tabStruct.Controls.Add(this.textStructAlign);
            this.tabStruct.Controls.Add(this.label5);
            this.tabStruct.Controls.Add(this.label3);
            this.tabStruct.Controls.Add(this.listStructName);
            this.tabStruct.Location = new System.Drawing.Point(4, 22);
            this.tabStruct.Name = "tabStruct";
            this.tabStruct.Padding = new System.Windows.Forms.Padding(3);
            this.tabStruct.Size = new System.Drawing.Size(485, 285);
            this.tabStruct.TabIndex = 4;
            this.tabStruct.Text = "Struct Search";
            this.tabStruct.UseVisualStyleBackColor = true;
            // 
            // textStructAlign
            // 
            this.textStructAlign.Location = new System.Drawing.Point(68, 259);
            this.textStructAlign.Name = "textStructAlign";
            this.textStructAlign.Size = new System.Drawing.Size(100, 20);
            this.textStructAlign.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 262);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "Alignment:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 6);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Struct:";
            // 
            // listStructName
            // 
            this.listStructName.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listStructName.FormattingEnabled = true;
            this.listStructName.IntegralHeight = false;
            this.listStructName.Location = new System.Drawing.Point(50, 6);
            this.listStructName.Name = "listStructName";
            this.listStructName.Size = new System.Drawing.Size(429, 247);
            this.listStructName.TabIndex = 0;
            // 
            // checkPrivate
            // 
            this.checkPrivate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkPrivate.AutoSize = true;
            this.checkPrivate.Checked = true;
            this.checkPrivate.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkPrivate.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkPrivate.Location = new System.Drawing.Point(73, 328);
            this.checkPrivate.Name = "checkPrivate";
            this.checkPrivate.Size = new System.Drawing.Size(65, 18);
            this.checkPrivate.TabIndex = 2;
            this.checkPrivate.Text = "Private";
            this.checkPrivate.UseVisualStyleBackColor = true;
            // 
            // checkImage
            // 
            this.checkImage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkImage.AutoSize = true;
            this.checkImage.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkImage.Location = new System.Drawing.Point(138, 328);
            this.checkImage.Name = "checkImage";
            this.checkImage.Size = new System.Drawing.Size(61, 18);
            this.checkImage.TabIndex = 3;
            this.checkImage.Text = "Image";
            this.checkImage.UseVisualStyleBackColor = true;
            // 
            // checkMapped
            // 
            this.checkMapped.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkMapped.AutoSize = true;
            this.checkMapped.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkMapped.Location = new System.Drawing.Point(199, 328);
            this.checkMapped.Name = "checkMapped";
            this.checkMapped.Size = new System.Drawing.Size(71, 18);
            this.checkMapped.TabIndex = 4;
            this.checkMapped.Text = "Mapped";
            this.checkMapped.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 330);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Search in:";
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonOK.Location = new System.Drawing.Point(349, 342);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 5;
            this.buttonOK.Text = "&OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonCancel.Location = new System.Drawing.Point(430, 342);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 6;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // SearchWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(517, 377);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.checkMapped);
            this.Controls.Add(this.checkImage);
            this.Controls.Add(this.checkPrivate);
            this.Controls.Add(this.tabControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SearchWindow";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Edit Search";
            this.tabControl.ResumeLayout(false);
            this.tabLiteral.ResumeLayout(false);
            this.tabLiteral.PerformLayout();
            this.tabRegex.ResumeLayout(false);
            this.tabRegex.PerformLayout();
            this.tabString.ResumeLayout(false);
            this.tabString.PerformLayout();
            this.tabHeap.ResumeLayout(false);
            this.tabHeap.PerformLayout();
            this.tabStruct.ResumeLayout(false);
            this.tabStruct.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabLiteral;
        private System.Windows.Forms.TabPage tabRegex;
        private System.Windows.Forms.TabPage tabString;
        private System.Windows.Forms.TabPage tabHeap;
        private Be.Windows.Forms.HexBox hexBoxSearch;
        private ProcessHacker.Components.UtilitiesButton utilitiesButtonLiteral;
        private System.Windows.Forms.CheckBox checkPrivate;
        private System.Windows.Forms.CheckBox checkImage;
        private System.Windows.Forms.CheckBox checkMapped;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.CheckBox checkIgnoreCase;
        private System.Windows.Forms.TextBox textRegex;
        private System.Windows.Forms.TextBox textStringMS;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textHeapMS;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox checkNoOverlap;
        private System.Windows.Forms.TabPage tabStruct;
        private System.Windows.Forms.TextBox textStructAlign;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListBox listStructName;
        private System.Windows.Forms.CheckBox checkUnicode;
    }
}