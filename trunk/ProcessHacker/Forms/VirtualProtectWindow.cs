/*
 * Process Hacker - 
 *   memory protection modifier tool
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
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;

namespace ProcessHacker
{
    public partial class VirtualProtectWindow : Form
    {
        private int _pid, _address, _size;

        public VirtualProtectWindow(int pid, int address, int size)
        {
            InitializeComponent();

            _pid = pid;
            _address = address;
            _size = size;
        }

        private void textNewProtection_Enter(object sender, EventArgs e)
        {
            this.AcceptButton = buttonVirtualProtect;
        }

        private void buttonCloseVirtualProtect_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonVirtualProtect_Click(object sender, EventArgs e)
        {
            try
            {
                int old = 0;
                int newprotect;

                try
                {
                    newprotect = (int)BaseConverter.ToNumberParse(textNewProtection.Text);
                }
                catch
                {
                    return;
                }

                using (ProcessHandle phandle =
                    new ProcessHandle(_pid, ProcessAccess.VmOperation))
                {
                    if (!Win32.VirtualProtectEx(phandle, _address,
                        _size, newprotect, out old))
                    {
                        MessageBox.Show("There was an error setting memory protection:\n\n" +
                            Win32.GetLastErrorMessage(), "Process Hacker",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error setting memory protection:\n\n" + ex.Message, "Process Hacker",
                 MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void textNewProtection_Leave(object sender, EventArgs e)
        {
            this.AcceptButton = null;
        }
    }
}
