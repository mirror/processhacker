namespace ProcessHacker.Components
{
    partial class HandleDetails
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
            this.groupQuotaCharges.SuspendLayout();
            this.groupReferences.SuspendLayout();
            this.groupBasicInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupObjectInfo
            // 
            this.groupObjectInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupObjectInfo.Location = new System.Drawing.Point(6, 189);
            this.groupObjectInfo.Name = "groupObjectInfo";
            this.groupObjectInfo.Size = new System.Drawing.Size(430, 234);
            this.groupObjectInfo.TabIndex = 7;
            this.groupObjectInfo.TabStop = false;
            this.groupObjectInfo.Text = "Object Information";
            // 
            // groupQuotaCharges
            // 
            this.groupQuotaCharges.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupQuotaCharges.Controls.Add(this.labelNonPaged);
            this.groupQuotaCharges.Controls.Add(this.labelPaged);
            this.groupQuotaCharges.Location = new System.Drawing.Point(188, 112);
            this.groupQuotaCharges.Name = "groupQuotaCharges";
            this.groupQuotaCharges.Size = new System.Drawing.Size(248, 71);
            this.groupQuotaCharges.TabIndex = 6;
            this.groupQuotaCharges.TabStop = false;
            this.groupQuotaCharges.Text = "Quota Charges";
            // 
            // labelNonPaged
            // 
            this.labelNonPaged.AutoSize = true;
            this.labelNonPaged.Location = new System.Drawing.Point(6, 42);
            this.labelNonPaged.Name = "labelNonPaged";
            this.labelNonPaged.Size = new System.Drawing.Size(68, 13);
            this.labelNonPaged.TabIndex = 2;
            this.labelNonPaged.Text = "Non-Paged:";
            // 
            // labelPaged
            // 
            this.labelPaged.AutoSize = true;
            this.labelPaged.Location = new System.Drawing.Point(6, 21);
            this.labelPaged.Name = "labelPaged";
            this.labelPaged.Size = new System.Drawing.Size(42, 13);
            this.labelPaged.TabIndex = 2;
            this.labelPaged.Text = "Paged:";
            // 
            // groupReferences
            // 
            this.groupReferences.Controls.Add(this.labelHandles);
            this.groupReferences.Controls.Add(this.labelReferences);
            this.groupReferences.Location = new System.Drawing.Point(6, 112);
            this.groupReferences.Name = "groupReferences";
            this.groupReferences.Size = new System.Drawing.Size(176, 71);
            this.groupReferences.TabIndex = 5;
            this.groupReferences.TabStop = false;
            this.groupReferences.Text = "References";
            // 
            // labelHandles
            // 
            this.labelHandles.AutoSize = true;
            this.labelHandles.Location = new System.Drawing.Point(6, 42);
            this.labelHandles.Name = "labelHandles";
            this.labelHandles.Size = new System.Drawing.Size(52, 13);
            this.labelHandles.TabIndex = 2;
            this.labelHandles.Text = "Handles:";
            // 
            // labelReferences
            // 
            this.labelReferences.AutoSize = true;
            this.labelReferences.Location = new System.Drawing.Point(6, 21);
            this.labelReferences.Name = "labelReferences";
            this.labelReferences.Size = new System.Drawing.Size(66, 13);
            this.labelReferences.TabIndex = 2;
            this.labelReferences.Text = "References:";
            // 
            // groupBasicInfo
            // 
            this.groupBasicInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
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
            this.groupBasicInfo.Size = new System.Drawing.Size(430, 102);
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
            this.textGrantedAccess.Size = new System.Drawing.Size(326, 15);
            this.textGrantedAccess.TabIndex = 20;
            // 
            // textAddress
            // 
            this.textAddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textAddress.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textAddress.Location = new System.Drawing.Point(98, 57);
            this.textAddress.Name = "textAddress";
            this.textAddress.Size = new System.Drawing.Size(326, 15);
            this.textAddress.TabIndex = 1;
            // 
            // textType
            // 
            this.textType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textType.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textType.Location = new System.Drawing.Point(60, 38);
            this.textType.Name = "textType";
            this.textType.Size = new System.Drawing.Size(364, 15);
            this.textType.TabIndex = 1;
            // 
            // textName
            // 
            this.textName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textName.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textName.Location = new System.Drawing.Point(60, 19);
            this.textName.Name = "textName";
            this.textName.Size = new System.Drawing.Size(364, 15);
            this.textName.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 76);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(88, 13);
            this.label4.TabIndex = 21;
            this.label4.Text = "Granted Access:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 57);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Object Address:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Type:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name:";
            // 
            // HandleDetails
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.groupObjectInfo);
            this.Controls.Add(this.groupQuotaCharges);
            this.Controls.Add(this.groupReferences);
            this.Controls.Add(this.groupBasicInfo);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "HandleDetails";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Size = new System.Drawing.Size(442, 429);
            this.groupQuotaCharges.ResumeLayout(false);
            this.groupQuotaCharges.PerformLayout();
            this.groupReferences.ResumeLayout(false);
            this.groupReferences.PerformLayout();
            this.groupBasicInfo.ResumeLayout(false);
            this.groupBasicInfo.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupObjectInfo;
        private System.Windows.Forms.GroupBox groupQuotaCharges;
        private System.Windows.Forms.Label labelNonPaged;
        private System.Windows.Forms.Label labelPaged;
        private System.Windows.Forms.GroupBox groupReferences;
        private System.Windows.Forms.Label labelHandles;
        private System.Windows.Forms.Label labelReferences;
        private System.Windows.Forms.GroupBox groupBasicInfo;
        private System.Windows.Forms.TextBox textGrantedAccess;
        private System.Windows.Forms.TextBox textAddress;
        private System.Windows.Forms.TextBox textType;
        private System.Windows.Forms.TextBox textName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;

    }
}
