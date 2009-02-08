/*
 * Process Hacker - 
 *   DEP status editor
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
    public partial class EditDEPWindow : Form
    {
        private int _pid;

        public EditDEPWindow(int PID)
        {
            InitializeComponent();

            _pid = PID;

            try
            {
                using (Win32.ProcessHandle phandle
                  = new Win32.ProcessHandle(_pid, Win32.PROCESS_RIGHTS.PROCESS_QUERY_INFORMATION))
                {
                    var depStatus = phandle.GetDepStatus();
                    string str;

                    if ((depStatus & Win32.ProcessHandle.DepStatus.Enabled) != 0)
                    {
                        str = "Enabled";

                        if ((depStatus & Win32.ProcessHandle.DepStatus.AtlThunkEmulationDisabled) != 0)
                            str += ", DEP-ATL thunk emulation disabled";
                    }
                    else
                    {
                        str = "Disabled";
                    }

                    comboStatus.SelectedItem = str;
                }
            }
            catch
            { }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (comboStatus.SelectedItem.ToString().StartsWith("Enabled"))
                if (
                    MessageBox.Show("Are you sure you want to set the DEP status of the process? This action is permanent.",
                    "Process Hacker", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
                    return;

            Win32.DEPFLAGS flags = Win32.DEPFLAGS.PROCESS_DEP_ENABLE;

            if (comboStatus.SelectedItem.ToString() == "Disabled")
                flags = Win32.DEPFLAGS.PROCESS_DEP_DISABLE;
            else if (comboStatus.SelectedItem.ToString() == "Enabled")
                flags = Win32.DEPFLAGS.PROCESS_DEP_ENABLE;
            else if (comboStatus.SelectedItem.ToString() == "Enabled, DEP-ATL thunk emulation disabled")
                flags = Win32.DEPFLAGS.PROCESS_DEP_ENABLE | Win32.DEPFLAGS.PROCESS_DEP_DISABLE_ATL_THUNK_EMULATION;
            else
            {
                MessageBox.Show("Invalid value!", "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                int kernel32 = Win32.GetModuleHandle("kernel32.dll");
                int setProcessDEPPolicy = Win32.GetProcAddress(kernel32, "SetProcessDEPPolicy");

                if (setProcessDEPPolicy == 0)
                    throw new Exception("This feature is not supported on your version of Windows.");

                using (Win32.ProcessHandle phandle = new Win32.ProcessHandle(_pid, Win32.PROCESS_RIGHTS.PROCESS_CREATE_THREAD))
                {
                    var thread = phandle.CreateThread(setProcessDEPPolicy, (int)flags,
                        Win32.PROCESS_RIGHTS.PROCESS_QUERY_INFORMATION);

                    thread.Wait(1000);

                    int exitCode = thread.GetExitCode();

                    if (exitCode == 0)
                    {
                        throw new Exception("Error setting the DEP policy.");
                    }
                }

                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
