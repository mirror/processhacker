/*
 * Process Hacker - 
 *   token properties viewer
 * 
 * Copyright (C) 2008-2009 wj32
 * 
 * This file is part of Process Hacker.
 * 
 * Process Hacker is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * Process Hacker is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Process Hacker.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Drawing;
using System.Windows.Forms;
using ProcessHacker.Common;
using ProcessHacker.Common.Ui;
using ProcessHacker.Native;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;
using ProcessHacker.Native.Security.AccessControl;
using ProcessHacker.UI;

namespace ProcessHacker.Components
{
    public partial class TokenProperties : UserControl
    {
        private IWithToken _object;
        private TokenGroupsList _groups;

        public TokenProperties(IWithToken obj)
        {
            InitializeComponent();

            listPrivileges.SetDoubleBuffered(true);
            listPrivileges.ListViewItemSorter = new SortedListViewComparer(listPrivileges);
            GenericViewMenu.AddMenuItems(copyMenuItem.MenuItems, listPrivileges, null);
            listPrivileges.ContextMenu = menuPrivileges;

            if (obj == null)
                return;

            _object = obj;

            try
            {
                using (TokenHandle thandle = _object.GetToken(TokenAccess.Query))
                {
                    // "General"
                    try
                    {
                        textUser.Text = thandle.GetUser().GetFullName(true);
                        textUserSID.Text = thandle.GetUser().StringSid;
                        textOwner.Text = thandle.GetOwner().GetFullName(true);
                        textPrimaryGroup.Text = thandle.GetPrimaryGroup().GetFullName(true);
                    }
                    catch (Exception ex)
                    {
                        textUser.Text = "(" + ex.Message + ")";
                    }

                    try
                    {
                        textSessionID.Text = thandle.GetSessionId().ToString();
                    }
                    catch (Exception ex)
                    {
                        textSessionID.Text = "(" + ex.Message + ")";
                    }

                    try
                    {
                        var type = thandle.GetElevationType();

                        if (type == TokenElevationType.Default)
                            textElevated.Text = "N/A";
                        else if (type == TokenElevationType.Full)
                            textElevated.Text = "True";
                        else if (type == TokenElevationType.Limited)
                            textElevated.Text = "False";
                    }
                    catch (Exception ex)
                    {
                        textElevated.Text = "(" + ex.Message + ")";
                    }

                    // Determine if the token has a linked token.
                    if (OSVersion.HasUac)
                    {
                        try
                        {
                            TokenHandle linkedToken = thandle.GetLinkedToken();

                            if (linkedToken != null)
                                linkedToken.Dispose();
                            else
                                buttonLinkedToken.Visible = false;
                        }
                        catch
                        {
                            buttonLinkedToken.Visible = false;
                        }
                    }
                    else
                    {
                        buttonLinkedToken.Visible = false;
                    }

                    try
                    {
                        bool virtAllowed = thandle.IsVirtualizationAllowed();
                        bool virtEnabled = thandle.IsVirtualizationEnabled();

                        if (virtEnabled)
                            textVirtualized.Text = "Enabled";
                        else if (virtAllowed)
                            textVirtualized.Text = "Disabled";
                        else
                            textVirtualized.Text = "Not Allowed";
                    }
                    catch (Exception ex)
                    {
                        textVirtualized.Text = "(" + ex.Message + ")";
                    }

                    try
                    {
                        using (TokenHandle tokenSource = _object.GetToken(TokenAccess.QuerySource))
                        {
                            var source = tokenSource.GetSource();

                            textSourceName.Text = source.SourceName.TrimEnd('\0', '\r', '\n', ' ');

                            long luid = source.SourceIdentifier.QuadPart;

                            textSourceLUID.Text = "0x" + luid.ToString("x");
                        }
                    }
                    catch (Exception ex)
                    {
                        textSourceName.Text = "(" + ex.Message + ")";
                    }

                    // "Advanced"
                    try
                    {
                        var statistics = thandle.GetStatistics();

                        textTokenType.Text = statistics.TokenType.ToString();
                        textImpersonationLevel.Text = statistics.ImpersonationLevel.ToString();
                        textTokenId.Text = "0x" + statistics.TokenId.ToString();
                        textAuthenticationId.Text = "0x" + statistics.AuthenticationId.ToString();
                        textMemoryUsed.Text = Utils.FormatSize(statistics.DynamicCharged);
                        textMemoryAvailable.Text = Utils.FormatSize(statistics.DynamicAvailable);
                    }
                    catch (Exception ex)
                    {
                        textTokenType.Text = "(" + ex.Message + ")";
                    }

                    try
                    {
                        var groups = thandle.GetGroups();

                        _groups = new TokenGroupsList(groups);

                        foreach (var group in groups)
                            group.Dispose();

                        _groups.Dock = DockStyle.Fill;
                        tabGroups.Controls.Add(_groups);
                    }
                    catch (Exception ex)
                    {
                        tabGroups.Text = "(" + ex.Message + ")";
                    }

                    try
                    {
                        var privileges = thandle.GetPrivileges();

                        for (int i = 0; i < privileges.Length; i++)
                        {
                            this.AddPrivilege(privileges[i]);
                        }
                    }
                    catch (Exception ex)
                    {
                        tabPrivileges.Text = "(" + ex.Message + ")";
                    }
                }
            }
            catch (Exception ex)
            {
                tabControl.Visible = false;

                Label errorMessage = new Label();

                errorMessage.Text = ex.Message;

                this.Padding = new Padding(15, 10, 0, 0);
                this.Controls.Add(errorMessage);
            }

            if (!OSVersion.HasUac)
            {
                labelElevated.Enabled = false;
                textElevated.Enabled = false;
                textElevated.Text = "";
                labelVirtualization.Enabled = false;
                textVirtualized.Enabled = false;
                textVirtualized.Text = "";
            }

            if (tabControl.TabPages[Settings.Instance.TokenWindowTab] != null)
                tabControl.SelectedTab = tabControl.TabPages[Settings.Instance.TokenWindowTab];

            ColumnSettings.LoadSettings(Settings.Instance.PrivilegeListColumns, listPrivileges);
            listPrivileges.AddShortcuts();
        }

        public IWithToken Object
        {
            get { return _object; }
        }

        private void AddPrivilege(Privilege privilege)
        {
            ListViewItem item = listPrivileges.Items.Add(privilege.Name.ToLowerInvariant(), privilege.Name, 0);

            item.BackColor = GetAttributeColor(privilege.Attributes);
            item.SubItems.Add(new ListViewItem.ListViewSubItem(item, GetAttributeString(privilege.Attributes)));
            item.SubItems.Add(new ListViewItem.ListViewSubItem(item, privilege.DisplayName));
        }

        public void DumpInitialize()
        {
            buttonLinkedToken.Visible = false;
            buttonPermissions.Visible = false;
            listPrivileges.ContextMenu = listPrivileges.GetCopyMenu();

            _groups = new TokenGroupsList(null);
            _groups.Dock = DockStyle.Fill;
            tabGroups.Controls.Add(_groups);
        }

        public void DumpSetTextToken(
            string userName,
            string userStringSid,
            string ownerName,
            string primaryGroupName,
            string sessionId,
            string elevated,
            string virtualization
            )
        {
            textUser.Text = userName;
            textUserSID.Text = userStringSid;
            textOwner.Text = ownerName;
            textPrimaryGroup.Text = primaryGroupName;
            textSessionID.Text = sessionId;
            textElevated.Text = elevated;
            textVirtualized.Text = virtualization;
        }

        public void DumpSetTextSource(
            string name,
            string luid
            )
        {
            textSourceName.Text = name;
            textSourceLUID.Text = luid;
        }

        public void DumpSetTextAdvanced(
            string type,
            string impersonationLevel,
            string tokenLuid,
            string authenticationLuid,
            string memoryUsed,
            string memoryAvailable
            )
        {
            textTokenType.Text = type;
            textImpersonationLevel.Text = impersonationLevel;
            textTokenId.Text = tokenLuid;
            textAuthenticationId.Text = authenticationLuid;
            textMemoryUsed.Text = memoryUsed;
            textMemoryAvailable.Text = memoryAvailable;
        }

        public void DumpAddGroup(
            string name,
            SidAttributes attributes
            )
        {
            _groups.DumpAddGroup(name, attributes);
        }

        public void DumpAddPrivilege(
            string name,
            string displayName,
            SePrivilegeAttributes attributes
            )
        {
            ListViewItem item = listPrivileges.Items.Add(name.ToLowerInvariant(), name, 0);

            item.BackColor = GetAttributeColor(attributes);
            item.SubItems.Add(new ListViewItem.ListViewSubItem(item, GetAttributeString(attributes)));
            item.SubItems.Add(new ListViewItem.ListViewSubItem(item, displayName));
        }

        public void SaveSettings()
        {
            if (_groups != null)
                _groups.SaveSettings();

            Settings.Instance.TokenWindowTab = tabControl.SelectedTab.Name;
            Settings.Instance.PrivilegeListColumns = ColumnSettings.SaveSettings(listPrivileges);
        }

        private string GetAttributeString(SePrivilegeAttributes Attributes)
        {
            if ((Attributes & SePrivilegeAttributes.EnabledByDefault) != 0)
                return "Default Enabled";
            else if ((Attributes & SePrivilegeAttributes.Enabled) != 0)
                return "Enabled";
            else if (Attributes == SePrivilegeAttributes.Disabled)
                return "Disabled";
            else
                return "Unknown";
        }

        private Color GetAttributeColor(SePrivilegeAttributes Attributes)
        {
            if ((Attributes & SePrivilegeAttributes.EnabledByDefault) != 0)
                return Color.FromArgb(0xc0f0c0);
            else if ((Attributes & SePrivilegeAttributes.Enabled) != 0)
                return Color.FromArgb(0xe0f0e0);
            else if (Attributes == SePrivilegeAttributes.Disabled)
                return Color.FromArgb(0xf0e0e0);
            else
                return Color.White;
        }

        private void menuPrivileges_Popup(object sender, EventArgs e)
        {
            if (listPrivileges.SelectedItems.Count == 0)
            {
                menuPrivileges.DisableAll();
            }
            else
            {
                menuPrivileges.EnableAll();
            }

            if (listPrivileges.Items.Count > 0)
            {
                selectAllMenuItem.Enabled = true;
            }
            else
            {
                selectAllMenuItem.Enabled = false;
            }
        }

        private void enableMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listPrivileges.SelectedItems)
            {
                try
                {
                    using (var thandle = _object.GetToken(TokenAccess.AdjustPrivileges))
                        thandle.SetPrivilege(item.Text, SePrivilegeAttributes.Enabled);

                    if (item.SubItems[1].Text != "Default Enabled")
                    {
                        item.BackColor = GetAttributeColor(SePrivilegeAttributes.Enabled);
                        item.SubItems[1].Text = GetAttributeString(SePrivilegeAttributes.Enabled);
                    }
                }
                catch (Exception ex)
                {
                    if (!PhUtils.ShowContinueMessage(
                        "Unable to enable " + item.Text,
                        ex
                        ))
                        return;
                }
            }
        }

        private void disableMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listPrivileges.SelectedItems)
            {
                if (item.SubItems[1].Text == "Default Enabled")
                {
                    if (!PhUtils.ShowContinueMessage(
                        "Unable to disable " + item.Text,
                        new Exception("Invalid operation.")
                        ))
                        return;

                    continue;
                }

                try
                {
                    using (var thandle = _object.GetToken(TokenAccess.AdjustPrivileges))
                        thandle.SetPrivilege(item.Text, SePrivilegeAttributes.Disabled);

                    item.BackColor = GetAttributeColor(SePrivilegeAttributes.Disabled);
                    item.SubItems[1].Text = GetAttributeString(SePrivilegeAttributes.Disabled);
                }
                catch (Exception ex)
                {
                    if (!PhUtils.ShowContinueMessage(
                        "Unable to disable " + item.Text,
                        ex
                        ))
                        return;
                }
            }
        }

        private void removeMenuItem_Click(object sender, EventArgs e)
        {
            if (PhUtils.ShowConfirmMessage(
                "remove",
                "the selected privilege(s)",
                "Removing privileges may reduce the functionality of the process, " + 
                "and is permanent for the lifetime of the process.",
                false
                ))
            {
                foreach (ListViewItem item in listPrivileges.SelectedItems)
                {
                    try
                    {
                        using (var thandle = _object.GetToken(TokenAccess.AdjustPrivileges))
                            thandle.SetPrivilege(item.Text, SePrivilegeAttributes.Removed);

                        item.Remove();
                    }
                    catch (Exception ex)
                    {
                        if (!PhUtils.ShowContinueMessage(
                            "Unable to remove " + item.Text,
                            ex
                            ))
                            return;
                    }
                }
            }
        }

        private void selectAllMenuItem_Click(object sender, EventArgs e)
        {
            Utils.SelectAll(listPrivileges.Items);
        }

        private void buttonLinkedToken_Click(object sender, EventArgs e)
        {
            using (var thandle = _object.GetToken(TokenAccess.Query))
            {
                var token = new TokenWithLinkedToken(thandle);
                TokenWindow window = new TokenWindow(token);

                window.ShowDialog();
            }
        }

        private void buttonPermissions_Click(object sender, EventArgs e)
        {
            try
            {
                SecurityEditor.EditSecurity(
                    this,
                    SecurityEditor.GetSecurableWrapper((access) => _object.GetToken((TokenAccess)access)),
                    "Token",
                    NativeTypeFactory.GetAccessEntries(NativeTypeFactory.ObjectType.Token)
                    );
            }
            catch (Exception ex)
            {
                PhUtils.ShowException("Unable to edit security", ex);
            }
        }
    }
}
