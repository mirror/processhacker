/*
 * Process Hacker - 
 *   unhandled exception dialog
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
using System.Diagnostics;
using System.Windows.Forms;

namespace ProcessHacker
{
    public partial class ErrorDialog : Form
    {
        public ErrorDialog(Exception ex)
        {
            InitializeComponent();

            textException.Text = ex.ToString();
        }

        private void TryStart(string command)
        {
            try
            {
                Process.Start(command);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not start process:\n\n" + ex.Message, "Process Hacker", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void labelLink_Click(object sender, EventArgs e)
        {
            TryStart("http://sourceforge.net/tracker2/?group_id=242527");
        }

        private void buttonContinue_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonQuit_Click(object sender, EventArgs e)
        {
            try
            {
                Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
            }

            Win32.ExitProcess(1);
        }
    }
}
