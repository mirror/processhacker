/*
 * Process Hacker - 
 *   token properties viewer
 * 
 * Copyright (C) 2008 wj32
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
        private TokenGroups _groups;

        public TokenProperties(Win32.IWithToken obj)
        {
            InitializeComponent();

            _object = obj;

            Misc.SetDoubleBuffered(listPrivileges, typeof(ListView), true);

            GenericViewMenu.AddMenuItems(copyMenuItem.MenuItems, listPrivileges, null);
            listPrivileges.ContextMenu = menuPrivileges;

            typeof(ListView).GetProperty("DoubleBuffered",
                BindingFlags.NonPublic | BindingFlags.Instance).SetValue(listPrivileges, true, null);

            _object = obj;

            try
            {
                using (Win32.TokenHandle token = _object.GetToken(Win32.TOKEN_RIGHTS.TOKEN_QUERY))
                {
                    try
                    {
                        textUser.Text = token.GetUser().GetName(true);
                        textUserSID.Text = token.GetUser().GetStringSID();
                        textOwner.Text = token.GetOwner().GetName(true);
                        textPrimaryGroup.Text = token.GetPrimaryGroup().GetName(true);
                    }
                    catch (Exception ex)
                    {
                        textUser.Text = "(" + ex.Message + ")";
                    }

                    try
                    {
                        textSessionID.Text = token.GetSessionId().ToString();
                    }
                    catch (Exception ex)
                    {
                        textSessionID.Text = "(" + ex.Message + ")";
                    }

                    try
                    {
                        Win32.TOKEN_ELEVATION_TYPE type = token.GetElevationType();

                        if (type == Win32.TOKEN_ELEVATION_TYPE.TokenElevationTypeDefault)
                            textElevated.Text = "N/A";
                        else if (type == Win32.TOKEN_ELEVATION_TYPE.TokenElevationTypeFull)
                            textElevated.Text = "True";
                        else if (type == Win32.TOKEN_ELEVATION_TYPE.TokenElevationTypeLimited)
                            textElevated.Text = "False";
                    }
                    catch (Exception ex)
                    {
                        textElevated.Text = "(" + ex.Message + ")";
                    }

                    try
                    {
                        Win32.TokenWithLinkedToken tokWLT = new Win32.TokenWithLinkedToken(token);

                        tokWLT.GetToken().Dispose();
                    }
                    catch
                    {
                       buttonLinkedToken.Visible = false;
                    }

                    try
                    {
                        bool virtAllowed = token.IsVirtualizationAllowed();
                        bool virtEnabled = token.IsVirtualizationEnabled();

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
                        using (Win32.TokenHandle tokenSource = _object.GetToken(Win32.TOKEN_RIGHTS.TOKEN_QUERY_SOURCE))
                        {
                            Win32.TOKEN_SOURCE source = tokenSource.GetSource();

                            textSourceName.Text = source.SourceName.TrimEnd('\0', '\r', '\n', ' ');

                            long luid = (source.SourceIdentifier.HighPart << 32) | source.SourceIdentifier.LowPart;

                            textSourceLUID.Text = "0x" + luid.ToString("x");
                        }
                    }
                    catch (Exception ex)
                    {
                        textSourceName.Text = "(" + ex.Message + ")";
                    }

                    try
                    {
                        Win32.TokenHandle.TokenGroupsData groups = token.GetGroups();
                        _groups = new TokenGroups(groups);

                        _groups.Dock = DockStyle.Fill;
                        tabGroups.Controls.Add(_groups);
                    }
                    catch (Exception ex)
                    {
                        tabGroups.Text = "(" + ex.Message + ")";
                    }

                    try
                    {
                        Win32.TOKEN_PRIVILEGES privileges = token.GetPrivileges();

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

            if (Program.WindowsVersion == "XP")
            {
                // XP obviously doesn't have UAC
                labelElevated.Enabled = false;
                textElevated.Enabled = false;
                textElevated.Text = "";
                labelVirtualization.Enabled = false;
                textVirtualized.Enabled = false;
                textVirtualized.Text = "";
            }

            if (tabControl.TabPages[Properties.Settings.Default.TokenWindowTab] != null)
                tabControl.SelectedTab = tabControl.TabPages[Properties.Settings.Default.TokenWindowTab];

            ColumnSettings.LoadSettings(Properties.Settings.Default.PrivilegeListColumns, listPrivileges);
        }

        public void SaveSettings()
        {
            if (_groups != null)
                _groups.SaveSettings();

            Properties.Settings.Default.TokenWindowTab = tabControl.SelectedTab.Name;
            Properties.Settings.Default.PrivilegeListColumns = ColumnSettings.SaveSettings(listPrivileges);
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
                    _object.GetToken(Win32.TOKEN_RIGHTS.TOKEN_ADJUST_PRIVILEGES).SetPrivilege(
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
                    _object.GetToken(Win32.TOKEN_RIGHTS.TOKEN_ADJUST_PRIVILEGES).SetPrivilege(
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
                        _object.GetToken(Win32.TOKEN_RIGHTS.TOKEN_ADJUST_PRIVILEGES).SetPrivilege(
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
