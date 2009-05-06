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
using System.Windows.Forms;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;
using System.Runtime.InteropServices;

namespace ProcessHacker
{
    public partial class EditDEPWindow : Form
    {
        private int _pid;

        public EditDEPWindow(int PID)
        {
            InitializeComponent();
            this.AddEscapeToClose();

            _pid = PID;

            try
            {
                using (ProcessHandle phandle
                  = new ProcessHandle(_pid, ProcessAccess.QueryInformation))
                {
                    var depStatus = phandle.GetDepStatus();
                    string str;

                    if ((depStatus & DepStatus.Enabled) != 0)
                    {
                        str = "Enabled";

                        if ((depStatus & DepStatus.AtlThunkEmulationDisabled) != 0)
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

            DepFlags flags = DepFlags.Enable;

            if (comboStatus.SelectedItem.ToString() == "Disabled")
                flags = DepFlags.Disable;
            else if (comboStatus.SelectedItem.ToString() == "Enabled")
                flags = DepFlags.Enable;
            else if (comboStatus.SelectedItem.ToString() == "Enabled, DEP-ATL thunk emulation disabled")
                flags = DepFlags.Enable | DepFlags.DisableAtlThunkEmulation;
            else
            {
                MessageBox.Show("Invalid value!", "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                IntPtr kernel32 = Win32.GetModuleHandle("kernel32.dll");
                IntPtr setProcessDepPolicy = Win32.GetProcAddress(kernel32, "SetProcessDEPPolicy");

                if (setProcessDepPolicy == IntPtr.Zero)
                    throw new Exception("This feature is not supported on your version of Windows.");

                using (ProcessHandle phandle = new ProcessHandle(_pid, 
                    Program.MinProcessQueryRights | ProcessAccess.CreateThread))
                {
                    var thread = phandle.CreateThread(setProcessDepPolicy, new IntPtr((int)flags),
                        ThreadAccess.All);

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
