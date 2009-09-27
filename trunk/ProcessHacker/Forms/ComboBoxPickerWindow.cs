/*
 * Process Hacker - 
 *   easy-to-use combobox window
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ProcessHacker
{
    public partial class ComboBoxPickerWindow : Form
    {
        public ComboBoxPickerWindow(string[] items)
        {
            InitializeComponent();
            this.AddEscapeToClose();

            if (Program.HackerWindow.TopMost)
                this.TopMost = true;

            comboBox.Items.AddRange(items);

            if (comboBox.Items.Count > 0)
                comboBox.SelectedItem = comboBox.Items[0];
        }

        public string SelectedItem
        {
            get { return comboBox.SelectedItem as string; }
            set { comboBox.SelectedItem = value; }
        }

        public string Message
        {
            get
            {
                return labelText.Text;
            }
            set
            {
                labelText.Text = value;
            }
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
    }
}
