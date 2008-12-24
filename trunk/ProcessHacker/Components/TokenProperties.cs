/*
 * Process Hacker
 * 
 * Copyright (C) 2008 Dean
 * Copyright (C) 2008 wj32
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace ProcessHacker
{
    public partial class TokenProperties : UserControl
    {
        private Win32.IWithToken _object;

        public TokenProperties(Win32.IWithToken obj)
        {
            InitializeComponent();

            _object = obj;

            Misc.SetDoubleBuffered(listGroups, typeof(ListView), true);
            Misc.SetDoubleBuffered(listPrivileges, typeof(ListView), true);

            listGroups.ContextMenu = GenericViewMenu.GetMenu(listGroups);
            GenericViewMenu.AddMenuItems(copyMenuItem.MenuItems, listPrivileges, null);
            listPrivileges.ContextMenu = menuPrivileges;

            typeof(ListView).GetProperty("DoubleBuffered",
                BindingFlags.NonPublic | BindingFlags.Instance).SetValue(listPrivileges, true, null);

            _object = obj;

            try
            {
                using (Win32.TokenHandle token = _object.GetToken(Win32.TOKEN_RIGHTS.TOKEN_QUERY))
                {
                    textUser.Text = token.GetUsername(true);
                    textUserSID.Text = token.GetUserStringSID();
                    textSessionID.Text = token.GetSessionId().ToString();

                    Win32.TOKEN_ELEVATION_TYPE type = token.GetElevationType();

                    if (type == Win32.TOKEN_ELEVATION_TYPE.TokenElevationTypeDefault)
                        textElevated.Text = "N/A";
                    else if (type == Win32.TOKEN_ELEVATION_TYPE.TokenElevationTypeFull)
                        textElevated.Text = "True";
                    else if (type == Win32.TOKEN_ELEVATION_TYPE.TokenElevationTypeLimited)
                        textElevated.Text = "False";

                    if (type == Win32.TOKEN_ELEVATION_TYPE.TokenElevationTypeDefault)
                        buttonLinkedToken.Visible = false;

                    bool virtAllowed = token.IsVirtualizationAllowed();
                    bool virtEnabled = token.IsVirtualizationEnabled();
                    string virtText;

                    if (virtAllowed)
                        virtText = "Virtualization is allowed ";
                    else
                        virtText = "Virtualization is not allowed.";

                    if (virtEnabled)
                        virtText += "and enabled.";
                    else if (virtAllowed)
                        virtText += "but disabled.";

                    textVirtualized.Text = virtText;
                }
            }
            catch (Exception ex)
            {
                textUser.Text = "(" + ex.Message + ")";
            }

            try
            {
                Win32.TOKEN_GROUPS groups = Win32.ReadTokenGroups(_object.GetToken(Win32.TOKEN_RIGHTS.TOKEN_QUERY),
                    Properties.Settings.Default.ShowAccountDomains);

                for (int i = 0; i < groups.GroupCount; i++)
                {
                    string name = groups.Names[i];

                    if (name == "" || name == null)
                        continue;

                    ListViewItem item = listGroups.Items.Add(name.ToLower(), name, 0);

                    item.BackColor = GetAttributeColor(groups.Groups[i].Attributes);
                    item.SubItems.Add(new ListViewItem.ListViewSubItem(item,
                        GetAttributeString(groups.Groups[i].Attributes)));
                }
            }
            catch (Exception ex)
            {
                tabGroups.Text = "(" + ex.Message + ")";
            }

            try
            {
                Win32.TOKEN_PRIVILEGES privileges = Win32.ReadTokenPrivileges(_object.GetToken(Win32.TOKEN_RIGHTS.TOKEN_QUERY));

                for (int i = 0; i < privileges.PrivilegeCount; i++)
                {
                    string name = Win32.GetPrivilegeName(privileges.Privileges[i].Luid);
                    ListViewItem item = listPrivileges.Items.Add(name.ToLower(), name, 0);

                    item.BackColor = GetAttributeColor(privileges.Privileges[i].Attributes);
                    item.SubItems.Add(new ListViewItem.ListViewSubItem(item,
                        GetAttributeString(privileges.Privileges[i].Attributes)));
                    item.SubItems.Add(new ListViewItem.ListViewSubItem(item, Win32.GetPrivilegeDisplayName(name)));
                }
            }
            catch (Exception ex)
            {
                tabPrivileges.Text = "(" + ex.Message + ")";
            }

            if (tabControl.TabPages[Properties.Settings.Default.TokenWindowTab] != null)
                tabControl.SelectedTab = tabControl.TabPages[Properties.Settings.Default.TokenWindowTab];

            ColumnSettings.LoadSettings(Properties.Settings.Default.GroupListColumns, listGroups);
            ColumnSettings.LoadSettings(Properties.Settings.Default.PrivilegeListColumns, listPrivileges);
        }

        public void SaveSettings()
        {
            Properties.Settings.Default.TokenWindowTab = tabControl.SelectedTab.Name;

            Properties.Settings.Default.GroupListColumns = ColumnSettings.SaveSettings(listGroups);
            Properties.Settings.Default.PrivilegeListColumns = ColumnSettings.SaveSettings(listPrivileges);
        }

        private string GetAttributeString(Win32.SID_ATTRIBUTES Attributes)
        {
            string text = "";

            if ((Attributes & Win32.SID_ATTRIBUTES.SE_GROUP_INTEGRITY) != 0)
            {
                if ((Attributes & Win32.SID_ATTRIBUTES.SE_GROUP_INTEGRITY_ENABLED) != 0)
                    return "Integrity";
                else
                    return "Integrity (Disabled)";
            }
            else if ((Attributes & Win32.SID_ATTRIBUTES.SE_GROUP_LOGON_ID) != 0)
                text = "Logon ID";
            else if ((Attributes & Win32.SID_ATTRIBUTES.SE_GROUP_MANDATORY) != 0)
                text = "Mandatory";
            else if ((Attributes & Win32.SID_ATTRIBUTES.SE_GROUP_OWNER) != 0)
                text = "Owner";
            else if ((Attributes & Win32.SID_ATTRIBUTES.SE_GROUP_RESOURCE) != 0)
                text = "Resource";
            else if ((Attributes & Win32.SID_ATTRIBUTES.SE_GROUP_USE_FOR_DENY_ONLY) != 0)
                text = "Use for Deny Only";

            if ((Attributes & Win32.SID_ATTRIBUTES.SE_GROUP_ENABLED_BY_DEFAULT) != 0)
                return text + " (Default Enabled)";
            else if ((Attributes & Win32.SID_ATTRIBUTES.SE_GROUP_ENABLED) != 0)
                return text;
            else
                return text + " (Disabled)";
        }

        private Color GetAttributeColor(Win32.SID_ATTRIBUTES Attributes)
        {
            if ((Attributes & Win32.SID_ATTRIBUTES.SE_GROUP_INTEGRITY) != 0)
            {
                if ((Attributes & Win32.SID_ATTRIBUTES.SE_GROUP_INTEGRITY_ENABLED) == 0)
                    return Color.FromArgb(0xe0e0e0);
                else
                    return Color.White;
            }

            if ((Attributes & Win32.SID_ATTRIBUTES.SE_GROUP_ENABLED_BY_DEFAULT) != 0)
                return Color.FromArgb(0xe0f0e0);
            else if ((Attributes & Win32.SID_ATTRIBUTES.SE_GROUP_ENABLED) != 0)
                return Color.White;
            else
                return Color.FromArgb(0xf0e0e0);
        }

        private string GetAttributeString(Win32.SE_PRIVILEGE_ATTRIBUTES Attributes)
        {
            if ((Attributes & Win32.SE_PRIVILEGE_ATTRIBUTES.SE_PRIVILEGE_ENABLED_BY_DEFAULT) != 0)
                return "Default Enabled";
            else if ((Attributes & Win32.SE_PRIVILEGE_ATTRIBUTES.SE_PRIVILEGE_ENABLED) != 0)
                return "Enabled";
            else if (Attributes == Win32.SE_PRIVILEGE_ATTRIBUTES.SE_PRIVILEGE_DISABLED)
                return "Disabled";
            else
                return "Unknown";
        }

        private Color GetAttributeColor(Win32.SE_PRIVILEGE_ATTRIBUTES Attributes)
        {
            if ((Attributes & Win32.SE_PRIVILEGE_ATTRIBUTES.SE_PRIVILEGE_ENABLED_BY_DEFAULT) != 0)
                return Color.FromArgb(0xc0f0c0);
            else if ((Attributes & Win32.SE_PRIVILEGE_ATTRIBUTES.SE_PRIVILEGE_ENABLED) != 0)
                return Color.FromArgb(0xe0f0e0);
            else if (Attributes == Win32.SE_PRIVILEGE_ATTRIBUTES.SE_PRIVILEGE_DISABLED)
                return Color.FromArgb(0xf0e0e0);
            else
                return Color.White;
        }

        private void menuPrivileges_Popup(object sender, EventArgs e)
        {
            if (listPrivileges.SelectedItems.Count == 0)
            {
                Misc.DisableAllMenuItems(menuPrivileges);
            }
            else
            {
                Misc.EnableAllMenuItems(menuPrivileges);
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
                    Win32.WriteTokenPrivilege(
                        _object.GetToken(Win32.TOKEN_RIGHTS.TOKEN_ADJUST_PRIVILEGES),
                        item.Text, Win32.SE_PRIVILEGE_ATTRIBUTES.SE_PRIVILEGE_ENABLED);

                    if (item.SubItems[1].Text != "Default Enabled")
                    {
                        item.BackColor = GetAttributeColor(Win32.SE_PRIVILEGE_ATTRIBUTES.SE_PRIVILEGE_ENABLED);
                        item.SubItems[1].Text = GetAttributeString(Win32.SE_PRIVILEGE_ATTRIBUTES.SE_PRIVILEGE_ENABLED);
                    }
                }
                catch (Exception ex)
                {
                    if (MessageBox.Show("Could not enable " + item.Text + ":\n\n" + ex.Message, "Process Hacker",
                        MessageBoxButtons.OKCancel, MessageBoxIcon.Error) == DialogResult.Cancel)
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
                    if (MessageBox.Show("Could not disable " + item.Text + ":\n\nInvalid operation.", "Process Hacker",
                        MessageBoxButtons.OKCancel, MessageBoxIcon.Error) == DialogResult.Cancel)
                        return;

                    continue;
                }

                try
                {
                    Win32.WriteTokenPrivilege(
                        _object.GetToken(Win32.TOKEN_RIGHTS.TOKEN_ADJUST_PRIVILEGES),
                        item.Text, Win32.SE_PRIVILEGE_ATTRIBUTES.SE_PRIVILEGE_DISABLED);

                    item.BackColor = GetAttributeColor(Win32.SE_PRIVILEGE_ATTRIBUTES.SE_PRIVILEGE_DISABLED);
                    item.SubItems[1].Text = GetAttributeString(Win32.SE_PRIVILEGE_ATTRIBUTES.SE_PRIVILEGE_DISABLED);
                }
                catch (Exception ex)
                {
                    if (MessageBox.Show("Could not disable " + item.Text + ":\n\n" + ex.Message, "Process Hacker",
                        MessageBoxButtons.OKCancel, MessageBoxIcon.Error) == DialogResult.Cancel)
                        return;
                }
            }
        }

        private void removeMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to remove the selected privilege(s)? This action is permanent.",
                "Process Hacker", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
            {
                foreach (ListViewItem item in listPrivileges.SelectedItems)
                {
                    try
                    {
                        Win32.WriteTokenPrivilege(
                            _object.GetToken(Win32.TOKEN_RIGHTS.TOKEN_ADJUST_PRIVILEGES),
                            item.Text, Win32.SE_PRIVILEGE_ATTRIBUTES.SE_PRIVILEGE_REMOVED);

                        item.Remove();
                    }
                    catch (Exception ex)
                    {
                        if (MessageBox.Show("Could not remove " + item.Text + ":\n\n" + ex.Message, "Process Hacker",
                             MessageBoxButtons.OKCancel, MessageBoxIcon.Error) == DialogResult.Cancel)
                            return;
                    }
                }
            }
        }

        private void selectAllMenuItem_Click(object sender, EventArgs e)
        {
            Misc.SelectAll(listPrivileges.Items);
        }

        private void buttonLinkedToken_Click(object sender, EventArgs e)
        {
            Win32.TokenWithLinkedToken token = new Win32.TokenWithLinkedToken(_object.GetToken(Win32.TOKEN_RIGHTS.TOKEN_QUERY));
            TokenWindow window = new TokenWindow(token);

            window.ShowDialog();
        }
    }
}
