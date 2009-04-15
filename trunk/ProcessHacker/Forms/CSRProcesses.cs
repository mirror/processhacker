/*
 * Process Hacker - 
 *   CSR processes window
 * 
 * Copyright (C) 2009 wj32
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
using System.IO;
using ProcessHacker.UI;

namespace ProcessHacker
{
    public partial class CSRProcessesWindow : Form
    {
        public CSRProcessesWindow()
        {
            InitializeComponent();

            listProcesses.ListViewItemSorter = new SortedListComparer(listProcesses);
            listProcesses.ContextMenu = listProcesses.GetCopyMenu();
            listProcesses.SetDoubleBuffered(true);
            listProcesses.SetTheme("explorer");
        }

        private void CSRProcessesWindow_Load(object sender, EventArgs e)
        {
            buttonScan.Select();
            ColumnSettings.LoadSettings(Properties.Settings.Default.CSRProcessesColumns, listProcesses);
        }

        private void CSRProcessesWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.CSRProcessesColumns = ColumnSettings.SaveSettings(listProcesses);
        }

        private void buttonScan_Click(object sender, EventArgs e)
        {
            listProcesses.Items.Clear();

            var processes = Win32.EnumProcesses();
            var handles = Win32.EnumHandles();

            // Step 1: Get the PIDs of csrss.exe processes and open them. There is one server for each session.   
            var csrProcesses = new Dictionary<int, Win32.ProcessHandle>();

            foreach (var process in processes.Values)
            {
                if (process.Name != null && process.Name.Equals("csrss.exe", StringComparison.InvariantCultureIgnoreCase))
                {
                    try
                    {
                        csrProcesses.Add(process.Process.ProcessId, 
                            new Win32.ProcessHandle(process.Process.ProcessId, Win32.PROCESS_RIGHTS.PROCESS_DUP_HANDLE));
                    }
                    catch
                    { }
                }
            }

            // Step 2: Get the handles which belong to the two csrss.exe processes and create a list of PIDs.
            var processIds = new List<int>();

            foreach (var handle in handles)
            {
                if (csrProcesses.ContainsKey(handle.ProcessId))
                {
                    int dupHandle;

                    if (Win32.ZwDuplicateObject(csrProcesses[handle.ProcessId], handle.Handle,
                        -1, out dupHandle, (Win32.STANDARD_RIGHTS)Program.MinProcessQueryRights, 0, 0) != 0)
                        continue;

                    using (Win32.Win32Handle dupHandleAuto = new Win32.Win32Handle(dupHandle))
                    {
                        int processId = Win32.GetProcessId(dupHandle);

                        if (processId == 0)
                            continue;

                        processIds.Add(processId);
                    }
                }
            }

            // Step 3: Add the processes to the list while highlighting hidden processes.
            foreach (var pid in processIds)
            {
                try
                {
                    var phandle = new Win32.ProcessHandle(pid, Program.MinProcessQueryRights);
                    var item = listProcesses.Items.Add(new ListViewItem(new string[]
                    {
                        Win32.DeviceFileNameToDos(phandle.GetNativeImageFileName()),
                        pid.ToString()
                    }));

                    if (!processes.ContainsKey(pid))
                    {
                        item.BackColor = Color.Red;
                        item.ForeColor = Color.White;
                    }

                    phandle.Dispose();
                }
                catch (Exception ex)
                {
                    var item = listProcesses.Items.Add(new ListViewItem(new string[]
                    {
                        "(" + ex.Message + ")",
                        pid.ToString()
                    }));

                    item.BackColor = Color.Red;
                    item.ForeColor = Color.White;
                }
            }

            // Step 4: Clean up
            foreach (var phandle in csrProcesses.Values)
                phandle.Dispose();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonTerminate_Click(object sender, EventArgs e)
        {
            string promptMessage = "the selected processes";

            if (listProcesses.SelectedIndices.Count == 1)
                promptMessage = listProcesses.SelectedItems[0].SubItems[0].Text;

            if (MessageBox.Show("Are you sure you want to terminate " + promptMessage + "?",
                "Process Hacker", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                List<ListViewItem> remove = new List<ListViewItem>();

                foreach (ListViewItem item in listProcesses.SelectedItems)
                {
                    try
                    {
                        using (var phandle = new Win32.ProcessHandle(
                            int.Parse(item.SubItems[1].Text),
                            Win32.PROCESS_RIGHTS.PROCESS_TERMINATE))
                            phandle.Terminate();
                        remove.Add(item);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error terminating " + item.SubItems[0].Text + 
                            ": " + ex.Message, "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                foreach (var item in remove)
                    item.Remove();
            }
        }

        private void listProcesses_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listProcesses.SelectedItems.Count == 0)
                buttonTerminate.Enabled = false;
            else
                buttonTerminate.Enabled = true;
        }

        private void CSRProcessesWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
                Misc.SelectAll(listProcesses.Items);
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();

            sfd.FileName = "CSR Processes.txt";
            sfd.OverwritePrompt = true;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (StreamWriter sw = new StreamWriter(sfd.FileName))
                    {
                        foreach (ListViewItem item in listProcesses.Items)
                        {
                            sw.WriteLine((item.BackColor == Color.Red ? "[Hidden] " : "") +
                                item.SubItems[1].Text + ": " + item.SubItems[0].Text);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
