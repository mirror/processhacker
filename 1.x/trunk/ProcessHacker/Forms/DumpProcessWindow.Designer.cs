namespace ProcessHacker
{
    partial class DumpProcessWindow
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

            if (_tokenProps != null)
                _tokenProps.Dispose();

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
            this.tabGeneral = new System.Windows.Forms.TabPage();
            this.tabToken = new System.Windows.Forms.TabPage();
            this.tabModules = new System.Windows.Forms.TabPage();
            this.tabEnvironment = new System.Windows.Forms.TabPage();
            this.listEnvironment = new System.Windows.Forms.ListView();
            this.columnVarName = new System.Windows.Forms.ColumnHeader();
            this.columnVarValue = new System.Windows.Forms.ColumnHeader();
            this.tabHandles = new System.Windows.Forms.TabPage();
            this.groupProcess = new System.Windows.Forms.GroupBox();
            this.labelProcessTypeValue = new System.Windows.Forms.Label();
            this.labelProcessType = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.textDEP = new System.Windows.Forms.TextBox();
            this.labelDEP = new System.Windows.Forms.Label();
            this.buttonInspectParent = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.textParent = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textStartTime = new System.Windows.Forms.TextBox();
            this.textCmdLine = new System.Windows.Forms.TextBox();
            this.groupFile = new System.Windows.Forms.GroupBox();
            this.pictureIcon = new System.Windows.Forms.PictureBox();
            this.textFileDescription = new System.Windows.Forms.TextBox();
            this.textFileCompany = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textFileVersion = new System.Windows.Forms.TextBox();
            this.textFileName = new System.Windows.Forms.TextBox();
            this.textCurrentDirectory = new System.Windows.Forms.TextBox();
            this.listModules = new ProcessHacker.Components.ModuleList();
            this.listHandles = new ProcessHacker.Components.HandleList();
            this.tabControl.SuspendLayout();
            this.tabGeneral.SuspendLayout();
            this.tabModules.SuspendLayout();
            this.tabEnvironment.SuspendLayout();
            this.tabHandles.SuspendLayout();
            this.groupProcess.SuspendLayout();
            this.groupFile.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabGeneral);
            this.tabControl.Controls.Add(this.tabToken);
            this.tabControl.Controls.Add(this.tabModules);
            this.tabControl.Controls.Add(this.tabEnvironment);
            this.tabControl.Controls.Add(this.tabHandles);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(3, 3);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(477, 467);
            this.tabControl.TabIndex = 0;
            // 
            // tabGeneral
            // 
            this.tabGeneral.Controls.Add(this.groupProcess);
            this.tabGeneral.Controls.Add(this.groupFile);
            this.tabGeneral.Location = new System.Drawing.Point(4, 22);
            this.tabGeneral.Name = "tabGeneral";
            this.tabGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tabGeneral.Size = new System.Drawing.Size(469, 441);
            this.tabGeneral.TabIndex = 0;
            this.tabGeneral.Text = "General";
            this.tabGeneral.UseVisualStyleBackColor = true;
            // 
            // tabToken
            // 
            this.tabToken.Location = new System.Drawing.Point(4, 22);
            this.tabToken.Name = "tabToken";
            this.tabToken.Size = new System.Drawing.Size(469, 441);
            this.tabToken.TabIndex = 4;
            this.tabToken.Text = "Token";
            this.tabToken.UseVisualStyleBackColor = true;
            // 
            // tabModules
            // 
            this.tabModules.Controls.Add(this.listModules);
            this.tabModules.Location = new System.Drawing.Point(4, 22);
            this.tabModules.Name = "tabModules";
            this.tabModules.Padding = new System.Windows.Forms.Padding(3);
            this.tabModules.Size = new System.Drawing.Size(469, 441);
            this.tabModules.TabIndex = 1;
            this.tabModules.Text = "Modules";
            this.tabModules.UseVisualStyleBackColor = true;
            // 
            // tabEnvironment
            // 
            this.tabEnvironment.Controls.Add(this.listEnvironment);
            this.tabEnvironment.Location = new System.Drawing.Point(4, 22);
            this.tabEnvironment.Name = "tabEnvironment";
            this.tabEnvironment.Padding = new System.Windows.Forms.Padding(3);
            this.tabEnvironment.Size = new System.Drawing.Size(469, 441);
            this.tabEnvironment.TabIndex = 3;
            this.tabEnvironment.Text = "Environment";
            this.tabEnvironment.UseVisualStyleBackColor = true;
            // 
            // listEnvironment
            // 
            this.listEnvironment.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnVarName,
            this.columnVarValue});
            this.listEnvironment.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listEnvironment.FullRowSelect = true;
            this.listEnvironment.HideSelection = false;
            this.listEnvironment.Location = new System.Drawing.Point(3, 3);
            this.listEnvironment.Name = "listEnvironment";
            this.listEnvironment.ShowItemToolTips = true;
            this.listEnvironment.Size = new System.Drawing.Size(463, 435);
            this.listEnvironment.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listEnvironment.TabIndex = 1;
            this.listEnvironment.UseCompatibleStateImageBehavior = false;
            this.listEnvironment.View = System.Windows.Forms.View.Details;
            // 
            // columnVarName
            // 
            this.columnVarName.Text = "Name";
            this.columnVarName.Width = 150;
            // 
            // columnVarValue
            // 
            this.columnVarValue.Text = "Value";
            this.columnVarValue.Width = 250;
            // 
            // tabHandles
            // 
            this.tabHandles.Controls.Add(this.listHandles);
            this.tabHandles.Location = new System.Drawing.Point(4, 22);
            this.tabHandles.Name = "tabHandles";
            this.tabHandles.Padding = new System.Windows.Forms.Padding(3);
            this.tabHandles.Size = new System.Drawing.Size(469, 441);
            this.tabHandles.TabIndex = 2;
            this.tabHandles.Text = "Handles";
            this.tabHandles.UseVisualStyleBackColor = true;
            // 
            // groupProcess
            // 
            this.groupProcess.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupProcess.Controls.Add(this.labelProcessTypeValue);
            this.groupProcess.Controls.Add(this.labelProcessType);
            this.groupProcess.Controls.Add(this.label26);
            this.groupProcess.Controls.Add(this.textDEP);
            this.groupProcess.Controls.Add(this.labelDEP);
            this.groupProcess.Controls.Add(this.buttonInspectParent);
            this.groupProcess.Controls.Add(this.label5);
            this.groupProcess.Controls.Add(this.textParent);
            this.groupProcess.Controls.Add(this.label4);
            this.groupProcess.Controls.Add(this.label2);
            this.groupProcess.Controls.Add(this.textStartTime);
            this.groupProcess.Controls.Add(this.textCurrentDirectory);
            this.groupProcess.Controls.Add(this.textCmdLine);
            this.groupProcess.Location = new System.Drawing.Point(8, 125);
            this.groupProcess.Name = "groupProcess";
            this.groupProcess.Size = new System.Drawing.Size(455, 310);
            this.groupProcess.TabIndex = 3;
            this.groupProcess.TabStop = false;
            this.groupProcess.Text = "Process";
            // 
            // labelProcessTypeValue
            // 
            this.labelProcessTypeValue.AutoSize = true;
            this.labelProcessTypeValue.Location = new System.Drawing.Point(98, 157);
            this.labelProcessTypeValue.Name = "labelProcessTypeValue";
            this.labelProcessTypeValue.Size = new System.Drawing.Size(16, 13);
            this.labelProcessTypeValue.TabIndex = 20;
            this.labelProcessTypeValue.Text = "...";
            this.labelProcessTypeValue.Visible = false;
            // 
            // labelProcessType
            // 
            this.labelProcessType.AutoSize = true;
            this.labelProcessType.Location = new System.Drawing.Point(6, 157);
            this.labelProcessType.Name = "labelProcessType";
            this.labelProcessType.Size = new System.Drawing.Size(75, 13);
            this.labelProcessType.TabIndex = 19;
            this.labelProcessType.Text = "Process Type:";
            this.labelProcessType.Visible = false;
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(6, 22);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(44, 13);
            this.label26.TabIndex = 12;
            this.label26.Text = "Started:";
            // 
            // textDEP
            // 
            this.textDEP.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textDEP.BackColor = System.Drawing.SystemColors.Control;
            this.textDEP.Location = new System.Drawing.Point(101, 128);
            this.textDEP.Name = "textDEP";
            this.textDEP.ReadOnly = true;
            this.textDEP.Size = new System.Drawing.Size(348, 20);
            this.textDEP.TabIndex = 8;
            // 
            // labelDEP
            // 
            this.labelDEP.AutoSize = true;
            this.labelDEP.Location = new System.Drawing.Point(6, 131);
            this.labelDEP.Name = "labelDEP";
            this.labelDEP.Size = new System.Drawing.Size(32, 13);
            this.labelDEP.TabIndex = 17;
            this.labelDEP.Text = "DEP:";
            // 
            // buttonInspectParent
            // 
            this.buttonInspectParent.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonInspectParent.Image = global::ProcessHacker.Properties.Resources.application_form_magnify;
            this.buttonInspectParent.Location = new System.Drawing.Point(425, 99);
            this.buttonInspectParent.Name = "buttonInspectParent";
            this.buttonInspectParent.Size = new System.Drawing.Size(24, 24);
            this.buttonInspectParent.TabIndex = 7;
            this.buttonInspectParent.UseVisualStyleBackColor = true;
            this.buttonInspectParent.Click += new System.EventHandler(this.buttonInspectParent_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 105);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 13);
            this.label5.TabIndex = 16;
            this.label5.Text = "Parent:";
            // 
            // textParent
            // 
            this.textParent.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textParent.BackColor = System.Drawing.SystemColors.Control;
            this.textParent.Location = new System.Drawing.Point(101, 102);
            this.textParent.Name = "textParent";
            this.textParent.ReadOnly = true;
            this.textParent.Size = new System.Drawing.Size(318, 20);
            this.textParent.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 76);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 13);
            this.label4.TabIndex = 14;
            this.label4.Text = "Current Directory:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "Command Line:";
            // 
            // textStartTime
            // 
            this.textStartTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textStartTime.Location = new System.Drawing.Point(101, 19);
            this.textStartTime.Name = "textStartTime";
            this.textStartTime.ReadOnly = true;
            this.textStartTime.Size = new System.Drawing.Size(348, 20);
            this.textStartTime.TabIndex = 0;
            // 
            // textCmdLine
            // 
            this.textCmdLine.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textCmdLine.Location = new System.Drawing.Point(101, 45);
            this.textCmdLine.Name = "textCmdLine";
            this.textCmdLine.ReadOnly = true;
            this.textCmdLine.Size = new System.Drawing.Size(348, 20);
            this.textCmdLine.TabIndex = 2;
            // 
            // groupFile
            // 
            this.groupFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupFile.Controls.Add(this.pictureIcon);
            this.groupFile.Controls.Add(this.textFileDescription);
            this.groupFile.Controls.Add(this.textFileCompany);
            this.groupFile.Controls.Add(this.label1);
            this.groupFile.Controls.Add(this.label3);
            this.groupFile.Controls.Add(this.textFileName);
            this.groupFile.Controls.Add(this.textFileVersion);
            this.groupFile.Location = new System.Drawing.Point(6, 6);
            this.groupFile.Name = "groupFile";
            this.groupFile.Size = new System.Drawing.Size(457, 114);
            this.groupFile.TabIndex = 2;
            this.groupFile.TabStop = false;
            this.groupFile.Text = "File";
            // 
            // pictureIcon
            // 
            this.pictureIcon.Location = new System.Drawing.Point(6, 19);
            this.pictureIcon.Name = "pictureIcon";
            this.pictureIcon.Size = new System.Drawing.Size(32, 32);
            this.pictureIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureIcon.TabIndex = 1;
            this.pictureIcon.TabStop = false;
            // 
            // textFileDescription
            // 
            this.textFileDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textFileDescription.BackColor = System.Drawing.SystemColors.Window;
            this.textFileDescription.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textFileDescription.Location = new System.Drawing.Point(44, 20);
            this.textFileDescription.Name = "textFileDescription";
            this.textFileDescription.ReadOnly = true;
            this.textFileDescription.Size = new System.Drawing.Size(407, 13);
            this.textFileDescription.TabIndex = 2;
            this.textFileDescription.Text = "File Description";
            // 
            // textFileCompany
            // 
            this.textFileCompany.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textFileCompany.BackColor = System.Drawing.SystemColors.Window;
            this.textFileCompany.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textFileCompany.Location = new System.Drawing.Point(44, 38);
            this.textFileCompany.Name = "textFileCompany";
            this.textFileCompany.ReadOnly = true;
            this.textFileCompany.Size = new System.Drawing.Size(407, 13);
            this.textFileCompany.TabIndex = 3;
            this.textFileCompany.Text = "File Company";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 60);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Image Version:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 88);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Image File Name:";
            // 
            // textFileVersion
            // 
            this.textFileVersion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textFileVersion.Location = new System.Drawing.Point(103, 57);
            this.textFileVersion.Name = "textFileVersion";
            this.textFileVersion.ReadOnly = true;
            this.textFileVersion.Size = new System.Drawing.Size(348, 20);
            this.textFileVersion.TabIndex = 0;
            // 
            // textFileName
            // 
            this.textFileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textFileName.Location = new System.Drawing.Point(103, 85);
            this.textFileName.Name = "textFileName";
            this.textFileName.ReadOnly = true;
            this.textFileName.Size = new System.Drawing.Size(348, 20);
            this.textFileName.TabIndex = 0;
            // 
            // textCurrentDirectory
            // 
            this.textCurrentDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textCurrentDirectory.Location = new System.Drawing.Point(101, 73);
            this.textCurrentDirectory.Name = "textCurrentDirectory";
            this.textCurrentDirectory.ReadOnly = true;
            this.textCurrentDirectory.Size = new System.Drawing.Size(348, 20);
            this.textCurrentDirectory.TabIndex = 2;
            // 
            // listModules
            // 
            this.listModules.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listModules.DoubleBuffered = true;
            this.listModules.Location = new System.Drawing.Point(3, 3);
            this.listModules.Name = "listModules";
            this.listModules.Provider = null;
            this.listModules.Size = new System.Drawing.Size(463, 435);
            this.listModules.TabIndex = 0;
            // 
            // listHandles
            // 
            this.listHandles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listHandles.DoubleBuffered = true;
            this.listHandles.Location = new System.Drawing.Point(3, 3);
            this.listHandles.Name = "listHandles";
            this.listHandles.Provider = null;
            this.listHandles.Size = new System.Drawing.Size(463, 435);
            this.listHandles.TabIndex = 0;
            // 
            // DumpProcessWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(483, 473);
            this.Controls.Add(this.tabControl);
            this.Name = "DumpProcessWindow";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Process";
            this.Load += new System.EventHandler(this.DumpProcessWindow_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DumpProcessWindow_FormClosing);
            this.tabControl.ResumeLayout(false);
            this.tabGeneral.ResumeLayout(false);
            this.tabModules.ResumeLayout(false);
            this.tabEnvironment.ResumeLayout(false);
            this.tabHandles.ResumeLayout(false);
            this.groupProcess.ResumeLayout(false);
            this.groupProcess.PerformLayout();
            this.groupFile.ResumeLayout(false);
            this.groupFile.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureIcon)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabGeneral;
        private System.Windows.Forms.TabPage tabModules;
        private System.Windows.Forms.TabPage tabHandles;
        private System.Windows.Forms.TabPage tabToken;
        private System.Windows.Forms.TabPage tabEnvironment;
        private ProcessHacker.Components.ModuleList listModules;
        private ProcessHacker.Components.HandleList listHandles;
        private System.Windows.Forms.ListView listEnvironment;
        private System.Windows.Forms.ColumnHeader columnVarName;
        private System.Windows.Forms.ColumnHeader columnVarValue;
        private System.Windows.Forms.GroupBox groupProcess;
        private System.Windows.Forms.Label labelProcessTypeValue;
        private System.Windows.Forms.Label labelProcessType;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.TextBox textDEP;
        private System.Windows.Forms.Label labelDEP;
        private System.Windows.Forms.Button buttonInspectParent;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textParent;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textStartTime;
        private System.Windows.Forms.TextBox textCurrentDirectory;
        private System.Windows.Forms.TextBox textCmdLine;
        private System.Windows.Forms.GroupBox groupFile;
        private System.Windows.Forms.PictureBox pictureIcon;
        private System.Windows.Forms.TextBox textFileDescription;
        private System.Windows.Forms.TextBox textFileCompany;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textFileName;
        private System.Windows.Forms.TextBox textFileVersion;
    }
}