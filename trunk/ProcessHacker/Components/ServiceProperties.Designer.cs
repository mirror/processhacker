namespace ProcessHacker
{
    partial class ServiceProperties
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
            this.listServices = new System.Windows.Forms.ListView();
            this.columnName = new System.Windows.Forms.ColumnHeader();
            this.columnDescription = new System.Windows.Forms.ColumnHeader();
            this.columnStatus = new System.Windows.Forms.ColumnHeader();
            this.panelService = new System.Windows.Forms.Panel();
            this.buttonStart = new System.Windows.Forms.Button();
            this.buttonStop = new System.Windows.Forms.Button();
            this.textLoadOrderGroup = new System.Windows.Forms.TextBox();
            this.comboErrorControl = new System.Windows.Forms.ComboBox();
            this.comboStartType = new System.Windows.Forms.ComboBox();
            this.comboType = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonApply = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.textUserAccount = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textServiceBinaryPath = new System.Windows.Forms.TextBox();
            this.labelServiceDisplayName = new System.Windows.Forms.Label();
            this.labelServiceName = new System.Windows.Forms.Label();
            this.panelService.SuspendLayout();
            this.SuspendLayout();
            // 
            // listServices
            // 
            this.listServices.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listServices.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnName,
            this.columnDescription,
            this.columnStatus});
            this.listServices.FullRowSelect = true;
            this.listServices.HideSelection = false;
            this.listServices.Location = new System.Drawing.Point(3, 3);
            this.listServices.MultiSelect = false;
            this.listServices.Name = "listServices";
            this.listServices.ShowItemToolTips = true;
            this.listServices.Size = new System.Drawing.Size(387, 133);
            this.listServices.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listServices.TabIndex = 3;
            this.listServices.UseCompatibleStateImageBehavior = false;
            this.listServices.View = System.Windows.Forms.View.Details;
            this.listServices.SelectedIndexChanged += new System.EventHandler(this.listServices_SelectedIndexChanged);
            // 
            // columnName
            // 
            this.columnName.Text = "Name";
            this.columnName.Width = 100;
            // 
            // columnDescription
            // 
            this.columnDescription.Text = "Description";
            this.columnDescription.Width = 140;
            // 
            // columnStatus
            // 
            this.columnStatus.Text = "Status";
            this.columnStatus.Width = 100;
            // 
            // panelService
            // 
            this.panelService.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panelService.Controls.Add(this.buttonStart);
            this.panelService.Controls.Add(this.buttonStop);
            this.panelService.Controls.Add(this.textLoadOrderGroup);
            this.panelService.Controls.Add(this.comboErrorControl);
            this.panelService.Controls.Add(this.comboStartType);
            this.panelService.Controls.Add(this.comboType);
            this.panelService.Controls.Add(this.label6);
            this.panelService.Controls.Add(this.label5);
            this.panelService.Controls.Add(this.label4);
            this.panelService.Controls.Add(this.label3);
            this.panelService.Controls.Add(this.buttonApply);
            this.panelService.Controls.Add(this.buttonCancel);
            this.panelService.Controls.Add(this.label2);
            this.panelService.Controls.Add(this.textUserAccount);
            this.panelService.Controls.Add(this.label1);
            this.panelService.Controls.Add(this.textServiceBinaryPath);
            this.panelService.Controls.Add(this.labelServiceDisplayName);
            this.panelService.Controls.Add(this.labelServiceName);
            this.panelService.Location = new System.Drawing.Point(3, 142);
            this.panelService.Name = "panelService";
            this.panelService.Padding = new System.Windows.Forms.Padding(3, 5, 3, 3);
            this.panelService.Size = new System.Drawing.Size(387, 212);
            this.panelService.TabIndex = 2;
            // 
            // buttonStart
            // 
            this.buttonStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonStart.Image = global::ProcessHacker.Properties.Resources.control_play_blue;
            this.buttonStart.Location = new System.Drawing.Point(36, 182);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(24, 24);
            this.buttonStart.TabIndex = 16;
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // buttonStop
            // 
            this.buttonStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonStop.Image = global::ProcessHacker.Properties.Resources.control_stop_blue;
            this.buttonStop.Location = new System.Drawing.Point(6, 182);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(24, 24);
            this.buttonStop.TabIndex = 16;
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // textLoadOrderGroup
            // 
            this.textLoadOrderGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textLoadOrderGroup.Location = new System.Drawing.Point(107, 103);
            this.textLoadOrderGroup.Name = "textLoadOrderGroup";
            this.textLoadOrderGroup.Size = new System.Drawing.Size(274, 20);
            this.textLoadOrderGroup.TabIndex = 15;
            // 
            // comboErrorControl
            // 
            this.comboErrorControl.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboErrorControl.FormattingEnabled = true;
            this.comboErrorControl.Location = new System.Drawing.Point(80, 76);
            this.comboErrorControl.Name = "comboErrorControl";
            this.comboErrorControl.Size = new System.Drawing.Size(121, 21);
            this.comboErrorControl.TabIndex = 14;
            // 
            // comboStartType
            // 
            this.comboStartType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboStartType.FormattingEnabled = true;
            this.comboStartType.Location = new System.Drawing.Point(238, 49);
            this.comboStartType.Name = "comboStartType";
            this.comboStartType.Size = new System.Drawing.Size(121, 21);
            this.comboStartType.TabIndex = 13;
            // 
            // comboType
            // 
            this.comboType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboType.FormattingEnabled = true;
            this.comboType.Location = new System.Drawing.Point(46, 49);
            this.comboType.Name = "comboType";
            this.comboType.Size = new System.Drawing.Size(121, 21);
            this.comboType.TabIndex = 12;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(173, 52);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(59, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "Start Type:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 79);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(68, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Error Control:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 52);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(34, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Type:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 106);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(95, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Load Order Group:";
            // 
            // buttonApply
            // 
            this.buttonApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonApply.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonApply.Location = new System.Drawing.Point(306, 183);
            this.buttonApply.Name = "buttonApply";
            this.buttonApply.Size = new System.Drawing.Size(75, 23);
            this.buttonApply.TabIndex = 7;
            this.buttonApply.Text = "&Apply";
            this.buttonApply.UseVisualStyleBackColor = true;
            this.buttonApply.Click += new System.EventHandler(this.buttonApply_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonCancel.Location = new System.Drawing.Point(225, 183);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 6;
            this.buttonCancel.Text = "&Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 132);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Binary Path:";
            // 
            // textUserAccount
            // 
            this.textUserAccount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textUserAccount.Location = new System.Drawing.Point(87, 155);
            this.textUserAccount.Name = "textUserAccount";
            this.textUserAccount.Size = new System.Drawing.Size(294, 20);
            this.textUserAccount.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 158);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "User Account:";
            // 
            // textServiceBinaryPath
            // 
            this.textServiceBinaryPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textServiceBinaryPath.Location = new System.Drawing.Point(76, 129);
            this.textServiceBinaryPath.Name = "textServiceBinaryPath";
            this.textServiceBinaryPath.Size = new System.Drawing.Size(305, 20);
            this.textServiceBinaryPath.TabIndex = 2;
            // 
            // labelServiceDisplayName
            // 
            this.labelServiceDisplayName.AutoSize = true;
            this.labelServiceDisplayName.Location = new System.Drawing.Point(6, 26);
            this.labelServiceDisplayName.Name = "labelServiceDisplayName";
            this.labelServiceDisplayName.Size = new System.Drawing.Size(111, 13);
            this.labelServiceDisplayName.TabIndex = 1;
            this.labelServiceDisplayName.Text = "Service Display Name";
            // 
            // labelServiceName
            // 
            this.labelServiceName.AutoSize = true;
            this.labelServiceName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelServiceName.Location = new System.Drawing.Point(6, 5);
            this.labelServiceName.Name = "labelServiceName";
            this.labelServiceName.Size = new System.Drawing.Size(86, 13);
            this.labelServiceName.TabIndex = 0;
            this.labelServiceName.Text = "Service Name";
            // 
            // ServiceProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.listServices);
            this.Controls.Add(this.panelService);
            this.Name = "ServiceProperties";
            this.Size = new System.Drawing.Size(393, 357);
            this.panelService.ResumeLayout(false);
            this.panelService.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listServices;
        private System.Windows.Forms.ColumnHeader columnName;
        private System.Windows.Forms.ColumnHeader columnDescription;
        private System.Windows.Forms.ColumnHeader columnStatus;
        private System.Windows.Forms.Panel panelService;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.TextBox textLoadOrderGroup;
        private System.Windows.Forms.ComboBox comboErrorControl;
        private System.Windows.Forms.ComboBox comboStartType;
        private System.Windows.Forms.ComboBox comboType;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonApply;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textUserAccount;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textServiceBinaryPath;
        private System.Windows.Forms.Label labelServiceDisplayName;
        private System.Windows.Forms.Label labelServiceName;
    }
}
