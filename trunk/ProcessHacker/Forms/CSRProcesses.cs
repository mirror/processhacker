/*
 * Process Hacker - 
 *   CSR processes window
 * 
 * Copyright (C) 2008 Dean
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
    public partial class CSRProcessesWindow : Form
    {
        public CSRProcessesWindow()
        {
            InitializeComponent();
        }

        private void CSRProcessesWindow_Load(object sender, EventArgs e)
        {
            buttonScan.Select();
        }

        private void CSRProcessesWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void buttonScan_Click(object sender, EventArgs e)
        {
            listProcesses.Items.Clear();

            var processes = Win32.EnumProcesses();
            var handles = Win32.EnumHandles();

            // Step 1: Get the PIDs of csrss.exe processes and open them. There is one server for each session.   
            var csrPIDs = new List<int>();
            var csrProcesses = new List<Win32.ProcessHandle>();

            foreach (var process in processes.Values)
            {
                if (process.Name.ToLower() == "csrss.exe")
                {
                    try
                    {
                        csrProcesses.Add(new Win32.ProcessHandle(process.Process.ProcessId,
                            Win32.PROCESS_RIGHTS.PROCESS_DUP_HANDLE));
                        csrPIDs.Add(process.Process.ProcessId);
                    }
                    catch
                    { }
                }
            }

            // Step 2: Get the handles which belong to the two csrss.exe processes and create a list of PIDs.
            var processIds = new List<int>();

            foreach (var handle in handles)
            {
                if (csrPIDs.Contains(handle.ProcessId))
                {
                    int dupHandle;

                    if (Win32.ZwDuplicateObject(csrProcesses[csrPIDs.IndexOf(handle.ProcessId)], handle.Handle,
                        -1, out dupHandle, (Win32.STANDARD_RIGHTS)Program.MinProcessQueryRights, 0, 0) != 0)
                        continue;


                }
            }
        }
    }
}
