/*
 * Process Hacker - 
 *   hidden processes scanner
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
    public partial class HiddenProcessesWindow : Form
    {
        public HiddenProcessesWindow()
        {
            InitializeComponent();

            listProcesses.ListViewItemSorter = new SortedListComparer(listProcesses);
            listProcesses.ContextMenu = listProcesses.GetCopyMenu();
            listProcesses.SetDoubleBuffered(true);
            listProcesses.SetTheme("explorer");
        }

        private void HiddenProcessesWindow_Load(object sender, EventArgs e)
        {
            buttonScan.Select();
            ColumnSettings.LoadSettings(Properties.Settings.Default.HiddenProcessesColumns, listProcesses);
        }

        private void HiddenProcessesWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.HiddenProcessesColumns = ColumnSettings.SaveSettings(listProcesses);
        }

        private void Scan()
        {
            listProcesses.Items.Clear();

            var processes = Win32.EnumProcesses();

            for (int pid = 8; pid <= 8096; pid += 4)
            {
                try
                {
                    var phandle = new Win32.ProcessHandle(pid, Program.MinProcessQueryRights);
                    string fileName = phandle.GetNativeImageFileName();

                    if (fileName != null)
                        fileName = Win32.DeviceFileNameToDos(fileName);

                    var item = listProcesses.Items.Add(new ListViewItem(new string[]
                    {
                        fileName,
                        pid.ToString()
                    }));

                    ulong[] times = new ulong[4];

                    Win32.GetProcessTimes(phandle, out times[0], out times[1], out times[2], out times[3]);

                    if (times[1] != 0)
                    {
                        item.BackColor = Color.DarkGray;
                        item.ForeColor = Color.White;
                    }
                    else
                    {
                        if (!processes.ContainsKey(pid))
                        {
                            item.BackColor = Color.Red;
                            item.ForeColor = Color.White;
                        }
                    }

                    phandle.Dispose();
                }
                catch (WindowsException ex)
                {
                    if (ex.ErrorCode == 87) // ERROR_INVALID_PARAMETER
                        continue;

                    var item = listProcesses.Items.Add(new ListViewItem(new string[]
                    {
                        "(" + ex.Message + ")",
                        pid.ToString()
                    }));

                    item.BackColor = Color.Red;
                    item.ForeColor = Color.White;
                }
            }
        }

        private void buttonScan_Click(object sender, EventArgs e)
        {
            this.Scan();
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

            if (MessageBox.Show("Are you sure you want to terminate " + promptMessage + "? " + 
                "WARNING: Terminating a hidden process may cause the system to crash or become " +
                "unstable because of modifications made by rootkit activity.",
                "Process Hacker", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                foreach (ListViewItem item in listProcesses.SelectedItems)
                {
                    int pid = int.Parse(item.SubItems[1].Text);

                    try
                    {
                        using (var phandle = new Win32.ProcessHandle(
                            pid,
                            Win32.PROCESS_RIGHTS.PROCESS_TERMINATE))
                            phandle.Terminate();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error terminating " + item.SubItems[0].Text + 
                            ": " + ex.Message, "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                this.Scan();
            }
        }

        private void listProcesses_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listProcesses.SelectedItems.Count == 0)
                buttonTerminate.Enabled = false;
            else
                buttonTerminate.Enabled = true;
        }

        private void HiddenProcessesWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
                Misc.SelectAll(listProcesses.Items);
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();

            sfd.FileName = "Process Scan.txt";
            sfd.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
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
