namespace ProcessHacker
{
    partial class TokenProperties
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
            this.components = new System.ComponentModel.Container();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabToken = new System.Windows.Forms.TabPage();
            this.textSessionID = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textUser = new System.Windows.Forms.TextBox();
            this.tabGroups = new System.Windows.Forms.TabPage();
            this.listGroups = new System.Windows.Forms.ListView();
            this.columnGroupName = new System.Windows.Forms.ColumnHeader();
            this.columnFlags = new System.Windows.Forms.ColumnHeader();
            this.tabPrivileges = new System.Windows.Forms.TabPage();
            this.listPrivileges = new System.Windows.Forms.ListView();
            this.columnName = new System.Windows.Forms.ColumnHeader();
            this.columnStatus = new System.Windows.Forms.ColumnHeader();
            this.columnDesc = new System.Windows.Forms.ColumnHeader();
            this.enableMenuItem = new System.Windows.Forms.MenuItem();
            this.disableMenuItem = new System.Windows.Forms.MenuItem();
            this.removeMenuItem = new System.Windows.Forms.MenuItem();
            this.copyMenuItem = new System.Windows.Forms.MenuItem();
            this.menuPrivileges = new System.Windows.Forms.ContextMenu();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.selectAllMenuItem = new System.Windows.Forms.MenuItem();
            this.label3 = new System.Windows.Forms.Label();
            this.textUserSID = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.textVirtualized = new System.Windows.Forms.TextBox();
            this.textElevated = new System.Windows.Forms.TextBox();
            this.vistaMenu = new wyDay.Controls.VistaMenu(this.components);
            this.buttonLinkedToken = new System.Windows.Forms.Button();
            this.tabControl.SuspendLayout();
            this.tabToken.SuspendLayout();
            this.tabGroups.SuspendLayout();
            this.tabPrivileges.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.vistaMenu)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabToken);
            this.tabControl.Controls.Add(this.tabGroups);
            this.tabControl.Controls.Add(this.tabPrivileges);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(575, 433);
            this.tabControl.TabIndex = 3;
            // 
            // tabToken
            // 
            this.tabToken.Controls.Add(this.buttonLinkedToken);
            this.tabToken.Controls.Add(this.textElevated);
            this.tabToken.Controls.Add(this.textVirtualized);
            this.tabToken.Controls.Add(this.label5);
            this.tabToken.Controls.Add(this.label4);
            this.tabToken.Controls.Add(this.textUserSID);
            this.tabToken.Controls.Add(this.label3);
            this.tabToken.Controls.Add(this.textSessionID);
            this.tabToken.Controls.Add(this.label2);
            this.tabToken.Controls.Add(this.label1);
            this.tabToken.Controls.Add(this.textUser);
            this.tabToken.Location = new System.Drawing.Point(4, 22);
            this.tabToken.Name = "tabToken";
            this.tabToken.Padding = new System.Windows.Forms.Padding(3, 5, 3, 3);
            this.tabToken.Size = new System.Drawing.Size(567, 407);
            this.tabToken.TabIndex = 2;
            this.tabToken.Text = "Token";
            this.tabToken.UseVisualStyleBackColor = true;
            // 
            // textSessionID
            // 
            this.textSessionID.Location = new System.Drawing.Point(73, 60);
            this.textSessionID.Name = "textSessionID";
            this.textSessionID.ReadOnly = true;
            this.textSessionID.Size = new System.Drawing.Size(109, 20);
            this.textSessionID.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Session ID:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "User:";
            // 
            // textUser
            // 
            this.textUser.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textUser.Location = new System.Drawing.Point(73, 8);
            this.textUser.Name = "textUser";
            this.textUser.ReadOnly = true;
            this.textUser.Size = new System.Drawing.Size(488, 20);
            this.textUser.TabIndex = 1;
            // 
            // tabGroups
            // 
            this.tabGroups.Controls.Add(this.listGroups);
            this.tabGroups.Location = new System.Drawing.Point(4, 22);
            this.tabGroups.Name = "tabGroups";
            this.tabGroups.Padding = new System.Windows.Forms.Padding(3);
            this.tabGroups.Size = new System.Drawing.Size(567, 407);
            this.tabGroups.TabIndex = 1;
            this.tabGroups.Text = "Groups";
            this.tabGroups.UseVisualStyleBackColor = true;
            // 
            // listGroups
            // 
            this.listGroups.AllowColumnReorder = true;
            this.listGroups.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnGroupName,
            this.columnFlags});
            this.listGroups.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listGroups.FullRowSelect = true;
            this.listGroups.Location = new System.Drawing.Point(3, 3);
            this.listGroups.Name = "listGroups";
            this.listGroups.ShowItemToolTips = true;
            this.listGroups.Size = new System.Drawing.Size(561, 401);
            this.listGroups.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listGroups.TabIndex = 3;
            this.listGroups.UseCompatibleStateImageBehavior = false;
            this.listGroups.View = System.Windows.Forms.View.Details;
            // 
            // columnGroupName
            // 
            this.columnGroupName.Text = "Name";
            this.columnGroupName.Width = 200;
            // 
            // columnFlags
            // 
            this.columnFlags.Text = "Flags";
            this.columnFlags.Width = 180;
            // 
            // tabPrivileges
            // 
            this.tabPrivileges.Controls.Add(this.listPrivileges);
            this.tabPrivileges.Location = new System.Drawing.Point(4, 22);
            this.tabPrivileges.Name = "tabPrivileges";
            this.tabPrivileges.Padding = new System.Windows.Forms.Padding(3);
            this.tabPrivileges.Size = new System.Drawing.Size(567, 407);
            this.tabPrivileges.TabIndex = 0;
            this.tabPrivileges.Text = "Privileges";
            this.tabPrivileges.UseVisualStyleBackColor = true;
            // 
            // listPrivileges
            // 
            this.listPrivileges.AllowColumnReorder = true;
            this.listPrivileges.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnName,
            this.columnStatus,
            this.columnDesc});
            this.listPrivileges.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listPrivileges.FullRowSelect = true;
            this.listPrivileges.Location = new System.Drawing.Point(3, 3);
            this.listPrivileges.Name = "listPrivileges";
            this.listPrivileges.ShowItemToolTips = true;
            this.listPrivileges.Size = new System.Drawing.Size(561, 401);
            this.listPrivileges.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listPrivileges.TabIndex = 0;
            this.listPrivileges.UseCompatibleStateImageBehavior = false;
            this.listPrivileges.View = System.Windows.Forms.View.Details;
            // 
            // columnName
            // 
            this.columnName.Text = "Name";
            this.columnName.Width = 100;
            // 
            // columnStatus
            // 
            this.columnStatus.Text = "Status";
            this.columnStatus.Width = 120;
            // 
            // columnDesc
            // 
            this.columnDesc.Text = "Description";
            this.columnDesc.Width = 190;
            // 
            // enableMenuItem
            // 
            this.vistaMenu.SetImage(this.enableMenuItem, global::ProcessHacker.Properties.Resources.tick);
            this.enableMenuItem.Index = 0;
            this.enableMenuItem.Text = "&Enable";
            this.enableMenuItem.Click += new System.EventHandler(this.enableMenuItem_Click);
            // 
            // disableMenuItem
            // 
            this.vistaMenu.SetImage(this.disableMenuItem, global::ProcessHacker.Properties.Resources.cross);
            this.disableMenuItem.Index = 1;
            this.disableMenuItem.Text = "&Disable";
            this.disableMenuItem.Click += new System.EventHandler(this.disableMenuItem_Click);
            // 
            // removeMenuItem
            // 
            this.vistaMenu.SetImage(this.removeMenuItem, global::ProcessHacker.Properties.Resources.delete);
            this.removeMenuItem.Index = 2;
            this.removeMenuItem.Text = "&Remove";
            this.removeMenuItem.Click += new System.EventHandler(this.removeMenuItem_Click);
            // 
            // copyMenuItem
            // 
            this.vistaMenu.SetImage(this.copyMenuItem, global::ProcessHacker.Properties.Resources.page_copy);
            this.copyMenuItem.Index = 4;
            this.copyMenuItem.Text = "&Copy";
            // 
            // menuPrivileges
            // 
            this.menuPrivileges.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.enableMenuItem,
            this.disableMenuItem,
            this.removeMenuItem,
            this.menuItem2,
            this.copyMenuItem,
            this.selectAllMenuItem});
            this.menuPrivileges.Popup += new System.EventHandler(this.menuPrivileges_Popup);
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 3;
            this.menuItem2.Text = "-";
            // 
            // selectAllMenuItem
            // 
            this.selectAllMenuItem.Index = 5;
            this.selectAllMenuItem.Text = "Select &All";
            this.selectAllMenuItem.Click += new System.EventHandler(this.selectAllMenuItem_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 37);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "User SID:";
            // 
            // textUserSID
            // 
            this.textUserSID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textUserSID.Location = new System.Drawing.Point(73, 34);
            this.textUserSID.Name = "textUserSID";
            this.textUserSID.ReadOnly = true;
            this.textUserSID.Size = new System.Drawing.Size(488, 20);
            this.textUserSID.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 89);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(52, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Elevated:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 115);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(58, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Virtualized:";
            // 
            // textVirtualized
            // 
            this.textVirtualized.Location = new System.Drawing.Point(73, 112);
            this.textVirtualized.Name = "textVirtualized";
            this.textVirtualized.ReadOnly = true;
            this.textVirtualized.Size = new System.Drawing.Size(109, 20);
            this.textVirtualized.TabIndex = 11;
            // 
            // textElevated
            // 
            this.textElevated.Location = new System.Drawing.Point(73, 86);
            this.textElevated.Name = "textElevated";
            this.textElevated.ReadOnly = true;
            this.textElevated.Size = new System.Drawing.Size(109, 20);
            this.textElevated.TabIndex = 12;
            // 
            // vistaMenu
            // 
            this.vistaMenu.ContainerControl = this;
            // 
            // buttonLinkedToken
            // 
            this.buttonLinkedToken.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonLinkedToken.Location = new System.Drawing.Point(188, 84);
            this.buttonLinkedToken.Name = "buttonLinkedToken";
            this.buttonLinkedToken.Size = new System.Drawing.Size(105, 23);
            this.buttonLinkedToken.TabIndex = 13;
            this.buttonLinkedToken.Text = "Linked Token...";
            this.buttonLinkedToken.UseVisualStyleBackColor = true;
            this.buttonLinkedToken.Click += new System.EventHandler(this.buttonLinkedToken_Click);
            // 
            // TokenProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl);
            this.Name = "TokenProperties";
            this.Size = new System.Drawing.Size(575, 433);
            this.tabControl.ResumeLayout(false);
            this.tabToken.ResumeLayout(false);
            this.tabToken.PerformLayout();
            this.tabGroups.ResumeLayout(false);
            this.tabPrivileges.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.vistaMenu)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabToken;
        private System.Windows.Forms.TextBox textSessionID;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textUser;
        private System.Windows.Forms.TabPage tabGroups;
        private System.Windows.Forms.ListView listGroups;
        private System.Windows.Forms.ColumnHeader columnGroupName;
        private System.Windows.Forms.ColumnHeader columnFlags;
        private System.Windows.Forms.TabPage tabPrivileges;
        private System.Windows.Forms.ListView listPrivileges;
        private System.Windows.Forms.ColumnHeader columnName;
        private System.Windows.Forms.ColumnHeader columnStatus;
        private System.Windows.Forms.ColumnHeader columnDesc;
        private wyDay.Controls.VistaMenu vistaMenu;
        private System.Windows.Forms.MenuItem enableMenuItem;
        private System.Windows.Forms.MenuItem disableMenuItem;
        private System.Windows.Forms.MenuItem removeMenuItem;
        private System.Windows.Forms.MenuItem copyMenuItem;
        private System.Windows.Forms.ContextMenu menuPrivileges;
        private System.Windows.Forms.MenuItem menuItem2;
        private System.Windows.Forms.MenuItem selectAllMenuItem;
        private System.Windows.Forms.TextBox textUserSID;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textElevated;
        private System.Windows.Forms.TextBox textVirtualized;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button buttonLinkedToken;
    }
}
