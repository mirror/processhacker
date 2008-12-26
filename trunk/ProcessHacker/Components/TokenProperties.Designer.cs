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
            this.groupSource = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.textSourceName = new System.Windows.Forms.TextBox();
            this.textSourceLUID = new System.Windows.Forms.TextBox();
            this.groupToken = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonLinkedToken = new System.Windows.Forms.Button();
            this.textPrimaryGroup = new System.Windows.Forms.TextBox();
            this.textUser = new System.Windows.Forms.TextBox();
            this.textElevated = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textOwner = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textVirtualized = new System.Windows.Forms.TextBox();
            this.textSessionID = new System.Windows.Forms.TextBox();
            this.labelVirtualization = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.labelElevated = new System.Windows.Forms.Label();
            this.textUserSID = new System.Windows.Forms.TextBox();
            this.tabGroups = new System.Windows.Forms.TabPage();
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
            this.vistaMenu = new wyDay.Controls.VistaMenu(this.components);
            this.tabControl.SuspendLayout();
            this.tabToken.SuspendLayout();
            this.groupSource.SuspendLayout();
            this.groupToken.SuspendLayout();
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
            this.tabToken.AutoScroll = true;
            this.tabToken.Controls.Add(this.groupSource);
            this.tabToken.Controls.Add(this.groupToken);
            this.tabToken.Location = new System.Drawing.Point(4, 22);
            this.tabToken.Name = "tabToken";
            this.tabToken.Padding = new System.Windows.Forms.Padding(3, 5, 3, 3);
            this.tabToken.Size = new System.Drawing.Size(567, 407);
            this.tabToken.TabIndex = 2;
            this.tabToken.Text = "Token";
            this.tabToken.UseVisualStyleBackColor = true;
            // 
            // groupSource
            // 
            this.groupSource.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupSource.Controls.Add(this.label7);
            this.groupSource.Controls.Add(this.label6);
            this.groupSource.Controls.Add(this.textSourceName);
            this.groupSource.Controls.Add(this.textSourceLUID);
            this.groupSource.Location = new System.Drawing.Point(6, 221);
            this.groupSource.Name = "groupSource";
            this.groupSource.Size = new System.Drawing.Size(555, 75);
            this.groupSource.TabIndex = 15;
            this.groupSource.TabStop = false;
            this.groupSource.Text = "Source";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 48);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(35, 13);
            this.label7.TabIndex = 3;
            this.label7.Text = "LUID:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 22);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(38, 13);
            this.label6.TabIndex = 2;
            this.label6.Text = "Name:";
            // 
            // textSourceName
            // 
            this.textSourceName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textSourceName.Location = new System.Drawing.Point(73, 19);
            this.textSourceName.Name = "textSourceName";
            this.textSourceName.ReadOnly = true;
            this.textSourceName.Size = new System.Drawing.Size(476, 20);
            this.textSourceName.TabIndex = 1;
            // 
            // textSourceLUID
            // 
            this.textSourceLUID.Location = new System.Drawing.Point(73, 45);
            this.textSourceLUID.Name = "textSourceLUID";
            this.textSourceLUID.ReadOnly = true;
            this.textSourceLUID.Size = new System.Drawing.Size(109, 20);
            this.textSourceLUID.TabIndex = 4;
            // 
            // groupToken
            // 
            this.groupToken.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupToken.Controls.Add(this.label9);
            this.groupToken.Controls.Add(this.label1);
            this.groupToken.Controls.Add(this.buttonLinkedToken);
            this.groupToken.Controls.Add(this.textPrimaryGroup);
            this.groupToken.Controls.Add(this.textUser);
            this.groupToken.Controls.Add(this.textElevated);
            this.groupToken.Controls.Add(this.label8);
            this.groupToken.Controls.Add(this.textOwner);
            this.groupToken.Controls.Add(this.label2);
            this.groupToken.Controls.Add(this.textVirtualized);
            this.groupToken.Controls.Add(this.textSessionID);
            this.groupToken.Controls.Add(this.labelVirtualization);
            this.groupToken.Controls.Add(this.label3);
            this.groupToken.Controls.Add(this.labelElevated);
            this.groupToken.Controls.Add(this.textUserSID);
            this.groupToken.Location = new System.Drawing.Point(6, 8);
            this.groupToken.Name = "groupToken";
            this.groupToken.Size = new System.Drawing.Size(555, 207);
            this.groupToken.TabIndex = 14;
            this.groupToken.TabStop = false;
            this.groupToken.Text = "Token";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 100);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(76, 13);
            this.label9.TabIndex = 17;
            this.label9.Text = "Primary Group:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "User:";
            // 
            // buttonLinkedToken
            // 
            this.buttonLinkedToken.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonLinkedToken.Location = new System.Drawing.Point(203, 147);
            this.buttonLinkedToken.Name = "buttonLinkedToken";
            this.buttonLinkedToken.Size = new System.Drawing.Size(105, 23);
            this.buttonLinkedToken.TabIndex = 13;
            this.buttonLinkedToken.Text = "Linked Token...";
            this.buttonLinkedToken.UseVisualStyleBackColor = true;
            this.buttonLinkedToken.Click += new System.EventHandler(this.buttonLinkedToken_Click);
            // 
            // textPrimaryGroup
            // 
            this.textPrimaryGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textPrimaryGroup.Location = new System.Drawing.Point(88, 97);
            this.textPrimaryGroup.Name = "textPrimaryGroup";
            this.textPrimaryGroup.ReadOnly = true;
            this.textPrimaryGroup.Size = new System.Drawing.Size(461, 20);
            this.textPrimaryGroup.TabIndex = 16;
            // 
            // textUser
            // 
            this.textUser.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textUser.Location = new System.Drawing.Point(88, 19);
            this.textUser.Name = "textUser";
            this.textUser.ReadOnly = true;
            this.textUser.Size = new System.Drawing.Size(461, 20);
            this.textUser.TabIndex = 1;
            // 
            // textElevated
            // 
            this.textElevated.Location = new System.Drawing.Point(88, 149);
            this.textElevated.Name = "textElevated";
            this.textElevated.ReadOnly = true;
            this.textElevated.Size = new System.Drawing.Size(109, 20);
            this.textElevated.TabIndex = 12;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 74);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(41, 13);
            this.label8.TabIndex = 15;
            this.label8.Text = "Owner:";
            // 
            // textOwner
            // 
            this.textOwner.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textOwner.Location = new System.Drawing.Point(88, 71);
            this.textOwner.Name = "textOwner";
            this.textOwner.ReadOnly = true;
            this.textOwner.Size = new System.Drawing.Size(461, 20);
            this.textOwner.TabIndex = 14;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 126);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Session ID:";
            // 
            // textVirtualized
            // 
            this.textVirtualized.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textVirtualized.Location = new System.Drawing.Point(88, 175);
            this.textVirtualized.Name = "textVirtualized";
            this.textVirtualized.ReadOnly = true;
            this.textVirtualized.Size = new System.Drawing.Size(461, 20);
            this.textVirtualized.TabIndex = 11;
            // 
            // textSessionID
            // 
            this.textSessionID.Location = new System.Drawing.Point(88, 123);
            this.textSessionID.Name = "textSessionID";
            this.textSessionID.ReadOnly = true;
            this.textSessionID.Size = new System.Drawing.Size(109, 20);
            this.textSessionID.TabIndex = 4;
            // 
            // labelVirtualization
            // 
            this.labelVirtualization.AutoSize = true;
            this.labelVirtualization.Location = new System.Drawing.Point(6, 178);
            this.labelVirtualization.Name = "labelVirtualization";
            this.labelVirtualization.Size = new System.Drawing.Size(69, 13);
            this.labelVirtualization.TabIndex = 8;
            this.labelVirtualization.Text = "Virtualization:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 48);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "User SID:";
            // 
            // labelElevated
            // 
            this.labelElevated.AutoSize = true;
            this.labelElevated.Location = new System.Drawing.Point(6, 152);
            this.labelElevated.Name = "labelElevated";
            this.labelElevated.Size = new System.Drawing.Size(52, 13);
            this.labelElevated.TabIndex = 7;
            this.labelElevated.Text = "Elevated:";
            // 
            // textUserSID
            // 
            this.textUserSID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textUserSID.Location = new System.Drawing.Point(88, 45);
            this.textUserSID.Name = "textUserSID";
            this.textUserSID.ReadOnly = true;
            this.textUserSID.Size = new System.Drawing.Size(461, 20);
            this.textUserSID.TabIndex = 6;
            // 
            // tabGroups
            // 
            this.tabGroups.Location = new System.Drawing.Point(4, 22);
            this.tabGroups.Name = "tabGroups";
            this.tabGroups.Padding = new System.Windows.Forms.Padding(3);
            this.tabGroups.Size = new System.Drawing.Size(567, 407);
            this.tabGroups.TabIndex = 1;
            this.tabGroups.Text = "Groups";
            this.tabGroups.UseVisualStyleBackColor = true;
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
            // vistaMenu
            // 
            this.vistaMenu.ContainerControl = this;
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
            this.groupSource.ResumeLayout(false);
            this.groupSource.PerformLayout();
            this.groupToken.ResumeLayout(false);
            this.groupToken.PerformLayout();
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
        private System.Windows.Forms.Label labelVirtualization;
        private System.Windows.Forms.Label labelElevated;
        private System.Windows.Forms.Button buttonLinkedToken;
        private System.Windows.Forms.GroupBox groupSource;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textSourceName;
        private System.Windows.Forms.TextBox textSourceLUID;
        private System.Windows.Forms.GroupBox groupToken;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textPrimaryGroup;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textOwner;
    }
}
