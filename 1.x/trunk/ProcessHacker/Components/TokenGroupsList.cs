/*
 * Process Hacker - 
 *   token groups viewer
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

using System.Drawing;
using System.Windows.Forms;
using ProcessHacker.Common;
using ProcessHacker.Common.Ui;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Security;
using ProcessHacker.UI;

namespace ProcessHacker.Components
{
    public partial class TokenGroupsList : UserControl
    {
        public TokenGroupsList(Sid[] groups)
        {
            InitializeComponent();

            if (groups != null)
            {
                for (int i = 0; i < groups.Length; i++)
                {
                    ListViewItem item = listGroups.Items.Add(new ListViewItem());

                    item.Text = groups[i].GetFullName(Settings.Instance.ShowAccountDomains);
                    item.BackColor = GetAttributeColor(groups[i].Attributes);
                    item.SubItems.Add(new ListViewItem.ListViewSubItem(item, GetAttributeString(groups[i].Attributes)));
                }
            }

            listGroups.ListViewItemSorter = new SortedListViewComparer(listGroups);
            listGroups.SetDoubleBuffered(true);
            listGroups.ContextMenu = listGroups.GetCopyMenu();
            ColumnSettings.LoadSettings(Settings.Instance.GroupListColumns, listGroups);
            listGroups.AddShortcuts();
        }

        public void SaveSettings()
        {
            Settings.Instance.GroupListColumns = ColumnSettings.SaveSettings(listGroups);
        }

        public void DumpAddGroup(string name, SidAttributes attributes)
        {
            ListViewItem item = listGroups.Items.Add(new ListViewItem());

            item.Text = PhUtils.GetBestUserName(name, Settings.Instance.ShowAccountDomains);
            item.BackColor = GetAttributeColor(attributes);
            item.SubItems.Add(new ListViewItem.ListViewSubItem(item, GetAttributeString(attributes)));
        }

        private string GetAttributeString(SidAttributes Attributes)
        {
            string text = "";

            if ((Attributes & SidAttributes.Integrity) != 0)
            {
                if ((Attributes & SidAttributes.IntegrityEnabled) != 0)
                    return "Integrity";
                else
                    return "Integrity (Disabled)";
            }
            else if ((Attributes & SidAttributes.LogonId) != 0)
                text = "Logon ID";
            else if ((Attributes & SidAttributes.Mandatory) != 0)
                text = "Mandatory";
            else if ((Attributes & SidAttributes.Owner) != 0)
                text = "Owner";
            else if ((Attributes & SidAttributes.Resource) != 0)
                text = "Resource";
            else if ((Attributes & SidAttributes.UseForDenyOnly) != 0)
                text = "Use for Deny Only";

            if ((Attributes & SidAttributes.EnabledByDefault) != 0)
                return text + " (Default Enabled)";
            else if ((Attributes & SidAttributes.Enabled) != 0)
                return text;
            else
                return text + " (Disabled)";
        }

        private Color GetAttributeColor(SidAttributes Attributes)
        {
            if ((Attributes & SidAttributes.Integrity) != 0)
            {
                if ((Attributes & SidAttributes.IntegrityEnabled) == 0)
                    return Color.FromArgb(0xe0e0e0);
                else
                    return Color.White;
            }

            if ((Attributes & SidAttributes.EnabledByDefault) != 0)
                return Color.FromArgb(0xe0f0e0);
            else if ((Attributes & SidAttributes.Enabled) != 0)
                return Color.White;
            else
                return Color.FromArgb(0xf0e0e0);
        }
    }
}
