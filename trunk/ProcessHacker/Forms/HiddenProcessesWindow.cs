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
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ProcessHacker.Native;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.UI;

namespace ProcessHacker
{
    public partial class HiddenProcessesWindow : Form
    {
        public HiddenProcessesWindow()
        {
            InitializeComponent();
            this.AddEscapeToClose();

            listProcesses.ListViewItemSorter = new SortedListViewComparer(listProcesses);
            listProcesses.ContextMenu = listProcesses.GetCopyMenu();
            listProcesses.SetDoubleBuffered(true);
            listProcesses.SetTheme("explorer");

            labelCount.Text = "";
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
            this.Cursor = Cursors.WaitCursor;
            listProcesses.BeginUpdate();
            listProcesses.Items.Clear();

            var processes = Windows.GetProcesses();
            int totalCount = 0;
            int hiddenCount = 0;
            int terminatedCount = 0;

            for (int pid = 8; pid <= 65536; pid += 4)
            {
                try
                {
                    var phandle = new ProcessHandle(pid, Program.MinProcessQueryRights);
                    string fileName = phandle.GetNativeImageFileName();

                    if (fileName != null)
                        fileName = FileUtils.DeviceFileNameToDos(fileName);

                    var item = listProcesses.Items.Add(new ListViewItem(new string[]
                    {
                        fileName,
                        pid.ToString()
                    }));

                    // Check if the process has terminated. This is possible because 
                    // a process can be terminated while its object is still being 
                    // referenced.
                    ulong[] times = new ulong[4];

                    Win32.GetProcessTimes(phandle, out times[0], out times[1], out times[2], out times[3]);

                    if (times[1] != 0)
                    {
                        item.BackColor = Color.DarkGray;
                        item.ForeColor = Color.White;
                        terminatedCount++;
                    }
                    else
                    {
                        totalCount++;

                        if (!processes.ContainsKey(pid))
                        {
                            item.BackColor = Color.Red;
                            item.ForeColor = Color.White;
                            hiddenCount++;
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
                    totalCount++;
                }
            }

            labelCount.Text = totalCount.ToString() + " running processes (excl. kernel and idle), " +
                hiddenCount.ToString() + " hidden, " + terminatedCount.ToString() + " terminated.";

            if (hiddenCount > 0)
                labelCount.ForeColor = Color.Red;
            else
                labelCount.ForeColor = SystemColors.WindowText;

            listProcesses.EndUpdate();
            this.Cursor = Cursors.Default;
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

            if (MessageBox.Show("Are you sure you want to terminate " + promptMessage + "?\n" + 
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
                        using (var phandle = 
                            new ProcessHandle(pid, ProcessHacker.Native.Security.ProcessAccess.Terminate))
                            phandle.Terminate();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error terminating " + item.SubItems[0].Text + 
                            ": " + ex.Message, "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                // Wait a bit to avoid BSODs
                System.Threading.Thread.Sleep(200);
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
                    using (var sw = new StreamWriter(sfd.FileName))
                    {
                        foreach (ListViewItem item in listProcesses.Items)
                        {
                            sw.WriteLine(
                                (item.BackColor == Color.Red ? "[HIDDEN] " : "") +
                                (item.BackColor == Color.DarkGray ? "[Terminated] " : "") +
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
