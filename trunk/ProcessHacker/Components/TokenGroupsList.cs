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
using ProcessHacker.Native;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.UI;

namespace ProcessHacker.Components
{
    public partial class TokenGroupsList : UserControl
    {
        public TokenGroupsList(TokenHandle.TokenGroupsData groups)
        {
            InitializeComponent();

            for (int i = 0; i < groups.Groups.GroupCount; i++)
            {
                ListViewItem item = listGroups.Items.Add(new ListViewItem());

                item.Text = Windows.GetAccountName(groups.Groups.Groups[i].SID, Properties.Settings.Default.ShowAccountDomains);
                item.BackColor = GetAttributeColor(groups.Groups.Groups[i].Attributes);
                item.SubItems.Add(new ListViewItem.ListViewSubItem(item,
                    GetAttributeString(groups.Groups.Groups[i].Attributes)));
            }

            listGroups.ListViewItemSorter = new SortedListViewComparer(listGroups);
            listGroups.SetDoubleBuffered(true);
            listGroups.ContextMenu = listGroups.GetCopyMenu();
            ColumnSettings.LoadSettings(Properties.Settings.Default.GroupListColumns, listGroups);
            listGroups.KeyDown +=
                (sender, e) =>
                {
                    if (e.Control && e.KeyCode == Keys.A) Misc.SelectAll(listGroups.Items);
                    if (e.Control && e.KeyCode == Keys.C) GenericViewMenu.ListViewCopy(listGroups, -1);
                };
        }

        public void SaveSettings()
        {
            Properties.Settings.Default.GroupListColumns = ColumnSettings.SaveSettings(listGroups);
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
