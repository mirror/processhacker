namespace SysCallHacker
{
    partial class EventProperties
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
            this.tabEvent = new System.Windows.Forms.TabPage();
            this.groupArguments = new System.Windows.Forms.GroupBox();
            this.listArguments = new System.Windows.Forms.ListView();
            this.columnIndex = new System.Windows.Forms.ColumnHeader();
            this.columnValue = new System.Windows.Forms.ColumnHeader();
            this.columnExtendedValue = new System.Windows.Forms.ColumnHeader();
            this.columnType = new System.Windows.Forms.ColumnHeader();
            this.groupBasic = new System.Windows.Forms.GroupBox();
            this.textSystemCall = new System.Windows.Forms.TextBox();
            this.textMode = new System.Windows.Forms.TextBox();
            this.textTime = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.labelMode = new System.Windows.Forms.Label();
            this.tabProcess = new System.Windows.Forms.TabPage();
            this.tabStackTrace = new System.Windows.Forms.TabPage();
            this.listStackTrace = new System.Windows.Forms.ListView();
            this.columnAddress = new System.Windows.Forms.ColumnHeader();
            this.columnSymbol = new System.Windows.Forms.ColumnHeader();
            this.buttonClose = new System.Windows.Forms.Button();
            this.labelClientID = new System.Windows.Forms.Label();
            this.textClientID = new System.Windows.Forms.TextBox();
            this.tabControl.SuspendLayout();
            this.tabEvent.SuspendLayout();
            this.groupArguments.SuspendLayout();
            this.groupBasic.SuspendLayout();
            this.tabStackTrace.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.tabEvent);
            this.tabControl.Controls.Add(this.tabProcess);
            this.tabControl.Controls.Add(this.tabStackTrace);
            this.tabControl.Location = new System.Drawing.Point(12, 12);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(510, 409);
            this.tabControl.TabIndex = 0;
            // 
            // tabEvent
            // 
            this.tabEvent.Controls.Add(this.groupArguments);
            this.tabEvent.Controls.Add(this.groupBasic);
            this.tabEvent.Location = new System.Drawing.Point(4, 22);
            this.tabEvent.Name = "tabEvent";
            this.tabEvent.Padding = new System.Windows.Forms.Padding(3);
            this.tabEvent.Size = new System.Drawing.Size(502, 383);
            this.tabEvent.TabIndex = 0;
            this.tabEvent.Text = "Event";
            this.tabEvent.UseVisualStyleBackColor = true;
            // 
            // groupArguments
            // 
            this.groupArguments.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupArguments.Controls.Add(this.listArguments);
            this.groupArguments.Location = new System.Drawing.Point(6, 138);
            this.groupArguments.Name = "groupArguments";
            this.groupArguments.Size = new System.Drawing.Size(490, 239);
            this.groupArguments.TabIndex = 2;
            this.groupArguments.TabStop = false;
            this.groupArguments.Text = "Arguments";
            // 
            // listArguments
            // 
            this.listArguments.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnIndex,
            this.columnValue,
            this.columnExtendedValue,
            this.columnType});
            this.listArguments.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listArguments.FullRowSelect = true;
            this.listArguments.HideSelection = false;
            this.listArguments.Location = new System.Drawing.Point(3, 16);
            this.listArguments.Name = "listArguments";
            this.listArguments.ShowItemToolTips = true;
            this.listArguments.Size = new System.Drawing.Size(484, 220);
            this.listArguments.TabIndex = 1;
            this.listArguments.UseCompatibleStateImageBehavior = false;
            this.listArguments.View = System.Windows.Forms.View.Details;
            // 
            // columnIndex
            // 
            this.columnIndex.Text = "Index";
            this.columnIndex.Width = 40;
            // 
            // columnValue
            // 
            this.columnValue.Text = "Value";
            this.columnValue.Width = 100;
            // 
            // columnExtendedValue
            // 
            this.columnExtendedValue.Text = "Extended Value";
            this.columnExtendedValue.Width = 260;
            // 
            // columnType
            // 
            this.columnType.Text = "Type";
            this.columnType.Width = 80;
            // 
            // groupBasic
            // 
            this.groupBasic.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBasic.Controls.Add(this.textClientID);
            this.groupBasic.Controls.Add(this.labelClientID);
            this.groupBasic.Controls.Add(this.textSystemCall);
            this.groupBasic.Controls.Add(this.textMode);
            this.groupBasic.Controls.Add(this.textTime);
            this.groupBasic.Controls.Add(this.label1);
            this.groupBasic.Controls.Add(this.labelMode);
            this.groupBasic.Location = new System.Drawing.Point(6, 6);
            this.groupBasic.Name = "groupBasic";
            this.groupBasic.Size = new System.Drawing.Size(490, 126);
            this.groupBasic.TabIndex = 1;
            this.groupBasic.TabStop = false;
            this.groupBasic.Text = "Basic";
            // 
            // textSystemCall
            // 
            this.textSystemCall.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textSystemCall.BackColor = System.Drawing.SystemColors.Window;
            this.textSystemCall.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textSystemCall.Location = new System.Drawing.Point(6, 19);
            this.textSystemCall.Name = "textSystemCall";
            this.textSystemCall.ReadOnly = true;
            this.textSystemCall.Size = new System.Drawing.Size(478, 13);
            this.textSystemCall.TabIndex = 4;
            // 
            // textMode
            // 
            this.textMode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textMode.BackColor = System.Drawing.SystemColors.Window;
            this.textMode.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textMode.Location = new System.Drawing.Point(52, 57);
            this.textMode.Name = "textMode";
            this.textMode.ReadOnly = true;
            this.textMode.Size = new System.Drawing.Size(435, 13);
            this.textMode.TabIndex = 3;
            // 
            // textTime
            // 
            this.textTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textTime.BackColor = System.Drawing.SystemColors.Window;
            this.textTime.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textTime.Location = new System.Drawing.Point(45, 38);
            this.textTime.Name = "textTime";
            this.textTime.ReadOnly = true;
            this.textTime.Size = new System.Drawing.Size(439, 13);
            this.textTime.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Time:";
            // 
            // labelMode
            // 
            this.labelMode.AutoSize = true;
            this.labelMode.Location = new System.Drawing.Point(6, 57);
            this.labelMode.Name = "labelMode";
            this.labelMode.Size = new System.Drawing.Size(37, 13);
            this.labelMode.TabIndex = 1;
            this.labelMode.Text = "Mode:";
            // 
            // tabProcess
            // 
            this.tabProcess.Location = new System.Drawing.Point(4, 22);
            this.tabProcess.Name = "tabProcess";
            this.tabProcess.Padding = new System.Windows.Forms.Padding(3);
            this.tabProcess.Size = new System.Drawing.Size(502, 383);
            this.tabProcess.TabIndex = 1;
            this.tabProcess.Text = "Process";
            this.tabProcess.UseVisualStyleBackColor = true;
            // 
            // tabStackTrace
            // 
            this.tabStackTrace.Controls.Add(this.listStackTrace);
            this.tabStackTrace.Location = new System.Drawing.Point(4, 22);
            this.tabStackTrace.Name = "tabStackTrace";
            this.tabStackTrace.Padding = new System.Windows.Forms.Padding(3);
            this.tabStackTrace.Size = new System.Drawing.Size(502, 383);
            this.tabStackTrace.TabIndex = 2;
            this.tabStackTrace.Text = "Stack Trace";
            this.tabStackTrace.UseVisualStyleBackColor = true;
            // 
            // listStackTrace
            // 
            this.listStackTrace.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnAddress,
            this.columnSymbol});
            this.listStackTrace.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listStackTrace.FullRowSelect = true;
            this.listStackTrace.HideSelection = false;
            this.listStackTrace.Location = new System.Drawing.Point(3, 3);
            this.listStackTrace.Name = "listStackTrace";
            this.listStackTrace.ShowItemToolTips = true;
            this.listStackTrace.Size = new System.Drawing.Size(496, 377);
            this.listStackTrace.TabIndex = 0;
            this.listStackTrace.UseCompatibleStateImageBehavior = false;
            this.listStackTrace.View = System.Windows.Forms.View.Details;
            // 
            // columnAddress
            // 
            this.columnAddress.Text = "Address";
            this.columnAddress.Width = 100;
            // 
            // columnSymbol
            // 
            this.columnSymbol.Text = "Symbol";
            this.columnSymbol.Width = 360;
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonClose.Location = new System.Drawing.Point(447, 427);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 1;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // labelClientID
            // 
            this.labelClientID.AutoSize = true;
            this.labelClientID.Location = new System.Drawing.Point(6, 76);
            this.labelClientID.Name = "labelClientID";
            this.labelClientID.Size = new System.Drawing.Size(50, 13);
            this.labelClientID.TabIndex = 5;
            this.labelClientID.Text = "Client ID:";
            // 
            // textClientID
            // 
            this.textClientID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textClientID.BackColor = System.Drawing.SystemColors.Window;
            this.textClientID.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textClientID.Location = new System.Drawing.Point(62, 76);
            this.textClientID.Name = "textClientID";
            this.textClientID.ReadOnly = true;
            this.textClientID.Size = new System.Drawing.Size(428, 13);
            this.textClientID.TabIndex = 6;
            // 
            // EventProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(534, 462);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.tabControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EventProperties";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Event Properties";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.EventProperties_KeyDown);
            this.tabControl.ResumeLayout(false);
            this.tabEvent.ResumeLayout(false);
            this.groupArguments.ResumeLayout(false);
            this.groupBasic.ResumeLayout(false);
            this.groupBasic.PerformLayout();
            this.tabStackTrace.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabEvent;
        private System.Windows.Forms.TabPage tabProcess;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.TabPage tabStackTrace;
        private System.Windows.Forms.ListView listStackTrace;
        private System.Windows.Forms.ColumnHeader columnAddress;
        private System.Windows.Forms.ColumnHeader columnSymbol;
        private System.Windows.Forms.GroupBox groupArguments;
        private System.Windows.Forms.GroupBox groupBasic;
        private System.Windows.Forms.Label labelMode;
        private System.Windows.Forms.TextBox textMode;
        private System.Windows.Forms.TextBox textTime;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textSystemCall;
        private System.Windows.Forms.ListView listArguments;
        private System.Windows.Forms.ColumnHeader columnIndex;
        private System.Windows.Forms.ColumnHeader columnValue;
        private System.Windows.Forms.ColumnHeader columnExtendedValue;
        private System.Windows.Forms.ColumnHeader columnType;
        private System.Windows.Forms.TextBox textClientID;
        private System.Windows.Forms.Label labelClientID;
    }
}