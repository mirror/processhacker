namespace ProcessHacker.Native.Ui
{
    partial class HandlePropertiesWindow
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
            this.tabDetails = new System.Windows.Forms.TabPage();
            this.groupObjectInfo = new System.Windows.Forms.GroupBox();
            this.groupQuotaCharges = new System.Windows.Forms.GroupBox();
            this.labelNonPaged = new System.Windows.Forms.Label();
            this.labelPaged = new System.Windows.Forms.Label();
            this.groupReferences = new System.Windows.Forms.GroupBox();
            this.labelHandles = new System.Windows.Forms.Label();
            this.labelReferences = new System.Windows.Forms.Label();
            this.groupBasicInfo = new System.Windows.Forms.GroupBox();
            this.textGrantedAccess = new System.Windows.Forms.TextBox();
            this.textAddress = new System.Windows.Forms.TextBox();
            this.textType = new System.Windows.Forms.TextBox();
            this.textName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonClose = new System.Windows.Forms.Button();
            this.buttonPermissions = new System.Windows.Forms.Button();
            this.tabControl.SuspendLayout();
            this.tabDetails.SuspendLayout();
            this.groupQuotaCharges.SuspendLayout();
            this.groupReferences.SuspendLayout();
            this.groupBasicInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.tabDetails);
            this.tabControl.Location = new System.Drawing.Point(12, 12);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(370, 382);
            this.tabControl.TabIndex = 0;
            // 
            // tabDetails
            // 
            this.tabDetails.Controls.Add(this.groupObjectInfo);
            this.tabDetails.Controls.Add(this.groupQuotaCharges);
            this.tabDetails.Controls.Add(this.groupReferences);
            this.tabDetails.Controls.Add(this.groupBasicInfo);
            this.tabDetails.Location = new System.Drawing.Point(4, 22);
            this.tabDetails.Name = "tabDetails";
            this.tabDetails.Padding = new System.Windows.Forms.Padding(3);
            this.tabDetails.Size = new System.Drawing.Size(362, 356);
            this.tabDetails.TabIndex = 0;
            this.tabDetails.Text = "Details";
            this.tabDetails.UseVisualStyleBackColor = true;
            // 
            // groupObjectInfo
            // 
            this.groupObjectInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupObjectInfo.Location = new System.Drawing.Point(6, 215);
            this.groupObjectInfo.Name = "groupObjectInfo";
            this.groupObjectInfo.Size = new System.Drawing.Size(350, 135);
            this.groupObjectInfo.TabIndex = 3;
            this.groupObjectInfo.TabStop = false;
            this.groupObjectInfo.Text = "Object Information";
            // 
            // groupQuotaCharges
            // 
            this.groupQuotaCharges.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupQuotaCharges.Controls.Add(this.labelNonPaged);
            this.groupQuotaCharges.Controls.Add(this.labelPaged);
            this.groupQuotaCharges.Location = new System.Drawing.Point(179, 138);
            this.groupQuotaCharges.Name = "groupQuotaCharges";
            this.groupQuotaCharges.Size = new System.Drawing.Size(177, 71);
            this.groupQuotaCharges.TabIndex = 2;
            this.groupQuotaCharges.TabStop = false;
            this.groupQuotaCharges.Text = "Quota Charges";
            // 
            // labelNonPaged
            // 
            this.labelNonPaged.AutoSize = true;
            this.labelNonPaged.Location = new System.Drawing.Point(6, 42);
            this.labelNonPaged.Name = "labelNonPaged";
            this.labelNonPaged.Size = new System.Drawing.Size(64, 13);
            this.labelNonPaged.TabIndex = 2;
            this.labelNonPaged.Text = "Non-Paged:";
            // 
            // labelPaged
            // 
            this.labelPaged.AutoSize = true;
            this.labelPaged.Location = new System.Drawing.Point(6, 21);
            this.labelPaged.Name = "labelPaged";
            this.labelPaged.Size = new System.Drawing.Size(41, 13);
            this.labelPaged.TabIndex = 2;
            this.labelPaged.Text = "Paged:";
            // 
            // groupReferences
            // 
            this.groupReferences.Controls.Add(this.labelHandles);
            this.groupReferences.Controls.Add(this.labelReferences);
            this.groupReferences.Location = new System.Drawing.Point(6, 138);
            this.groupReferences.Name = "groupReferences";
            this.groupReferences.Size = new System.Drawing.Size(167, 71);
            this.groupReferences.TabIndex = 1;
            this.groupReferences.TabStop = false;
            this.groupReferences.Text = "References";
            // 
            // labelHandles
            // 
            this.labelHandles.AutoSize = true;
            this.labelHandles.Location = new System.Drawing.Point(6, 42);
            this.labelHandles.Name = "labelHandles";
            this.labelHandles.Size = new System.Drawing.Size(49, 13);
            this.labelHandles.TabIndex = 2;
            this.labelHandles.Text = "Handles:";
            // 
            // labelReferences
            // 
            this.labelReferences.AutoSize = true;
            this.labelReferences.Location = new System.Drawing.Point(6, 21);
            this.labelReferences.Name = "labelReferences";
            this.labelReferences.Size = new System.Drawing.Size(65, 13);
            this.labelReferences.TabIndex = 2;
            this.labelReferences.Text = "References:";
            // 
            // groupBasicInfo
            // 
            this.groupBasicInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBasicInfo.Controls.Add(this.buttonPermissions);
            this.groupBasicInfo.Controls.Add(this.textGrantedAccess);
            this.groupBasicInfo.Controls.Add(this.textAddress);
            this.groupBasicInfo.Controls.Add(this.textType);
            this.groupBasicInfo.Controls.Add(this.textName);
            this.groupBasicInfo.Controls.Add(this.label4);
            this.groupBasicInfo.Controls.Add(this.label3);
            this.groupBasicInfo.Controls.Add(this.label2);
            this.groupBasicInfo.Controls.Add(this.label1);
            this.groupBasicInfo.Location = new System.Drawing.Point(6, 6);
            this.groupBasicInfo.Name = "groupBasicInfo";
            this.groupBasicInfo.Size = new System.Drawing.Size(350, 126);
            this.groupBasicInfo.TabIndex = 0;
            this.groupBasicInfo.TabStop = false;
            this.groupBasicInfo.Text = "Basic Information";
            // 
            // textGrantedAccess
            // 
            this.textGrantedAccess.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textGrantedAccess.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textGrantedAccess.Location = new System.Drawing.Point(98, 76);
            this.textGrantedAccess.Name = "textGrantedAccess";
            this.textGrantedAccess.Size = new System.Drawing.Size(246, 13);
            this.textGrantedAccess.TabIndex = 1;
            // 
            // textAddress
            // 
            this.textAddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textAddress.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textAddress.Location = new System.Drawing.Point(98, 57);
            this.textAddress.Name = "textAddress";
            this.textAddress.Size = new System.Drawing.Size(246, 13);
            this.textAddress.TabIndex = 1;
            // 
            // textType
            // 
            this.textType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textType.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textType.Location = new System.Drawing.Point(60, 38);
            this.textType.Name = "textType";
            this.textType.Size = new System.Drawing.Size(284, 13);
            this.textType.TabIndex = 1;
            // 
            // textName
            // 
            this.textName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textName.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textName.Location = new System.Drawing.Point(60, 19);
            this.textName.Name = "textName";
            this.textName.Size = new System.Drawing.Size(284, 13);
            this.textName.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 76);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(86, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Granted Access:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 57);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Object Address:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Type:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name:";
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonClose.Location = new System.Drawing.Point(307, 400);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 1;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // buttonPermissions
            // 
            this.buttonPermissions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonPermissions.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonPermissions.Location = new System.Drawing.Point(269, 97);
            this.buttonPermissions.Name = "buttonPermissions";
            this.buttonPermissions.Size = new System.Drawing.Size(75, 23);
            this.buttonPermissions.TabIndex = 2;
            this.buttonPermissions.Text = "Permissions";
            this.buttonPermissions.UseVisualStyleBackColor = true;
            this.buttonPermissions.Click += new System.EventHandler(this.buttonPermissions_Click);
            // 
            // HandlePropertiesWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(394, 435);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.tabControl);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "HandlePropertiesWindow";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Handle Properties";
            this.Load += new System.EventHandler(this.HandlePropertiesWindow_Load);
            this.tabControl.ResumeLayout(false);
            this.tabDetails.ResumeLayout(false);
            this.groupQuotaCharges.ResumeLayout(false);
            this.groupQuotaCharges.PerformLayout();
            this.groupReferences.ResumeLayout(false);
            this.groupReferences.PerformLayout();
            this.groupBasicInfo.ResumeLayout(false);
            this.groupBasicInfo.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabDetails;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.GroupBox groupBasicInfo;
        private System.Windows.Forms.TextBox textName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textGrantedAccess;
        private System.Windows.Forms.TextBox textAddress;
        private System.Windows.Forms.TextBox textType;
        private System.Windows.Forms.GroupBox groupReferences;
        private System.Windows.Forms.Label labelHandles;
        private System.Windows.Forms.Label labelReferences;
        private System.Windows.Forms.GroupBox groupQuotaCharges;
        private System.Windows.Forms.Label labelNonPaged;
        private System.Windows.Forms.Label labelPaged;
        private System.Windows.Forms.GroupBox groupObjectInfo;
        private System.Windows.Forms.Button buttonPermissions;
    }
}