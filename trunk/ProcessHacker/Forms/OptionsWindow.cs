/*
 * Process Hacker
 * 
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
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ProcessHacker
{
    public partial class OptionsWindow : Form
    {
        bool _needsReload = false;

        public OptionsWindow()
        {
            InitializeComponent();

            textUpdateInterval.Value = Properties.Settings.Default.RefreshInterval;
            checkWarnDangerous.Checked = Properties.Settings.Default.WarnDangerous;
            checkShowProcessDomains.Checked = Properties.Settings.Default.ShowProcessDomains;

            _needsReload = false;
        }

        private void textUpdateInterval_Leave(object sender, EventArgs e)
        {
            try
            {
                Properties.Settings.Default.RefreshInterval = Int32.Parse(textUpdateInterval.Value.ToString());
            }
            catch
            {
                MessageBox.Show("The entered value is not valid.", "Process Hacker", MessageBoxButtons.OK,
                 MessageBoxIcon.Error);

                textUpdateInterval.Focus();
            }
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            if (_needsReload)
                Program.HackerWindow.ReloadProcessList();

            this.Close();
        }

        private void checkWarnDangerous_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.WarnDangerous = checkWarnDangerous.Checked;

            _needsReload = true;
        }

        private void checkShowProcessDomains_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.ShowProcessDomains = checkShowProcessDomains.Checked;
        }
    }
}
