/*
 * Process Hacker - 
 *   simple-to-use list display box
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
using System.Windows.Forms;
using ProcessHacker.Common;
using ProcessHacker.UI;

namespace ProcessHacker
{
    public partial class ListWindow : Form
    {
        public ListWindow(List<KeyValuePair<string, string>> list)
        {
            InitializeComponent();
            this.AddEscapeToClose();
            this.SetTopMost();

            foreach (KeyValuePair<string, string> kvp in list)
            {
                ListViewItem item = new ListViewItem();

                item.Text = kvp.Key;
                item.SubItems.Add(new ListViewItem.ListViewSubItem(item, kvp.Value));

                listView.Items.Add(item);
            }

            listView.ContextMenu = listView.GetCopyMenu();
        }

        private void ListWindow_Load(object sender, EventArgs e)
        {
            listView.SetTheme("explorer");
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
