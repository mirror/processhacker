/*
 * Process Hacker - 
 *   simple-to-use list picker box
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
using System.Windows.Forms;

namespace ProcessHacker
{
    public partial class ListPickerWindow : Form
    {
        public ListPickerWindow(string[] items)
        {
            InitializeComponent();

            listItems.Items.AddRange(items);

            if (listItems.Items.Count > 0)
                listItems.SelectedItem = listItems.Items[0];
        }

        public string SelectedItem
        {
            get { return listItems.SelectedItem as string; }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void listItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listItems.SelectedItem == null)
                buttonOK.Enabled = false;
            else
                buttonOK.Enabled = true;
        }
    }
}
