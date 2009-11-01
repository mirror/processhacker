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
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ProcessHacker.Common;
using ProcessHacker.Common.Ui;
using ProcessHacker.Native;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;
using ProcessHacker.UI;

namespace ProcessHacker
{
    public partial class HiddenProcessesWindow : Form
    {
        public HiddenProcessesWindow()
        {
            this.SetPhParent();
            InitializeComponent();
            this.AddEscapeToClose();
            this.SetTopMost();

            listProcesses.ListViewItemSorter = new SortedListViewComparer(listProcesses);
            listProcesses.ContextMenu = listProcesses.GetCopyMenu();
            listProcesses.AddShortcuts();
            listProcesses.SetDoubleBuffered(true);
            listProcesses.SetTheme("explorer");

            comboMethod.SelectedItem = "CSR Handles";
            labelCount.Text = "";
        }

        private void HiddenProcessesWindow_Load(object sender, EventArgs e)
        {
            buttonScan.Select();
            ColumnSettings.LoadSettings(Settings.Instance.HiddenProcessesColumns, listProcesses);

            this.Size = Settings.Instance.HiddenProcessesWindowSize;
            this.Location = Utils.FitRectangle(new Rectangle(
                Settings.Instance.HiddenProcessesWindowLocation, this.Size), this).Location;
        }

        private void HiddenProcessesWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Instance.HiddenProcessesColumns = ColumnSettings.SaveSettings(listProcesses);

            if (this.WindowState == FormWindowState.Normal)
            {
                Settings.Instance.HiddenProcessesWindowSize = this.Size;
                Settings.Instance.HiddenProcessesWindowLocation = this.Location;
            }
        }

        private void AddProcessItem(
            ProcessHandle phandle,
            int pid,
            ref int totalCount, ref int hiddenCount, ref int terminatedCount,
            Func<int, bool> exists
            )
        {
            string fileName = phandle.GetImageFileName();

            if (fileName != null)
                fileName = FileUtils.GetFileName(fileName);

            if (pid == 0)
                pid = phandle.GetBasicInformation().UniqueProcessId.ToInt32();

            var item = listProcesses.Items.Add(new ListViewItem(new string[]
                    {
                        fileName,
                        pid.ToString()
                    }));

            // Check if the process has terminated. This is possible because 
            // a process can be terminated while its object is still being 
            // referenced.
            DateTime exitTime = DateTime.FromFileTime(0);

            try { exitTime = phandle.GetExitTime(); }
            catch { }

            if (exitTime.ToFileTime() != 0)
            {
                item.BackColor = Color.DarkGray;
                item.ForeColor = Color.White;
                terminatedCount++;
            }
            else
            {
                totalCount++;

                if (!exists(pid))
                {
                    item.BackColor = Color.Red;
                    item.ForeColor = Color.White;
                    hiddenCount++;
                }
            }
        }

        private void AddErrorItem(
            WindowsException ex,
            int pid,
            ref int totalCount, ref int hiddenCount, ref int terminatedCount
            )
        {
            if (ex.ErrorCode == Win32Error.InvalidParameter)
                return;

            var item = listProcesses.Items.Add(new ListViewItem(new string[]
                    {
                        "(" + ex.Message + ")",
                        pid.ToString()
                    }));

            item.BackColor = Color.Red;
            item.ForeColor = Color.White;
            totalCount++;
        }

        private void ScanBruteForce()
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
                    using (var phandle = new ProcessHandle(pid, Program.MinProcessQueryRights))
                        AddProcessItem(
                            phandle,
                            pid,
                            ref totalCount, ref hiddenCount, ref terminatedCount,
                            (pid_) => processes.ContainsKey(pid_)
                            );
                }
                catch (WindowsException ex)
                {
                    AddErrorItem(ex, pid, ref totalCount, ref hiddenCount, ref terminatedCount);
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

        private void ScanCsrHandles()
        {
            this.Cursor = Cursors.WaitCursor;
            listProcesses.BeginUpdate();
            listProcesses.Items.Clear();

            try
            {
                var processes = Windows.GetProcesses();
                int totalCount = 0;
                int hiddenCount = 0;
                int terminatedCount = 0;

                processes.Remove(0);

                List<int> foundPids = new List<int>();

                var csrProcesses = this.GetCsrProcesses();

                // Duplicate each process handle and check if they exist in the normal list.
                foreach (var csrhandle in csrProcesses)
                {
                    try
                    {
                        var handles = csrhandle.GetHandles();

                        foreach (var handle in handles)
                        {
                            int pid = 0;
                            bool isThread = false;

                            try
                            {
                                pid = KProcessHacker.Instance.KphGetProcessId(csrhandle, handle.Handle);

                                // HACK: Using exception for program flow!
                                if (pid == 0)
                                    throw new Exception();
                            }
                            catch
                            {
                                // Probably not a process handle.
                                // Try opening it as a thread.
                                try
                                {
                                    int tid = KProcessHacker.Instance.KphGetThreadId(csrhandle, handle.Handle, out pid);
                                    isThread = true;

                                    if (tid == 0)
                                        throw new Exception();
                                }
                                catch
                                {
                                    continue;
                                }
                            }

                            // Avoid duplicate PIDs.
                            if (foundPids.Contains(pid))
                                continue;

                            foundPids.Add(pid);

                            try
                            {
                                ProcessHandle phandle;

                                if (!isThread)
                                {
                                    var dupHandle =
                                        new NativeHandle<ProcessAccess>(csrhandle,
                                            handle.Handle,
                                            Program.MinProcessQueryRights);
                                    phandle = ProcessHandle.FromHandle(dupHandle);
                                }
                                else
                                {
                                    using (var dupHandle =
                                        new NativeHandle<ThreadAccess>(csrhandle,
                                            handle.Handle,
                                            Program.MinThreadQueryRights))
                                        phandle = ThreadHandle.FromHandle(dupHandle).
                                            GetProcess(Program.MinProcessQueryRights);
                                }

                                AddProcessItem(
                                    phandle,
                                    pid,
                                    ref totalCount, ref hiddenCount, ref terminatedCount,
                                    (pid_) => processes.ContainsKey(pid_)
                                    );
                                phandle.Dispose();
                            }
                            catch (WindowsException ex2)
                            {
                                AddErrorItem(ex2, pid, ref totalCount, ref hiddenCount, ref terminatedCount);
                            }
                        }
                    }
                    catch (WindowsException ex)
                    {
                        PhUtils.ShowException("Unable to get the CSR handle list", ex);
                        return;
                    }

                    csrhandle.Dispose();
                }

                labelCount.Text = totalCount.ToString() + " running processes (excl. kernel, idle, non-Windows), " +
                    hiddenCount.ToString() + " hidden, " + terminatedCount.ToString() + " terminated.";

                if (hiddenCount > 0)
                    labelCount.ForeColor = Color.Red;
                else
                    labelCount.ForeColor = SystemColors.WindowText;
            }
            finally
            {
                listProcesses.EndUpdate();
                this.Cursor = Cursors.Default;
            }
        }

        private List<ProcessHandle> GetCsrProcesses()
        {
            List<ProcessHandle> csrProcesses = new List<ProcessHandle>();

            try
            {
                foreach (var process in Windows.GetProcesses())
                {
                    if (process.Key <= 4)
                        continue;

                    try
                    {
                        var phandle = new ProcessHandle(process.Key,
                            Program.MinProcessQueryRights | ProcessAccess.DupHandle
                            );

                        if (phandle.GetKnownProcessType() == KnownProcess.WindowsSubsystem)
                            csrProcesses.Add(phandle);
                        else
                            phandle.Dispose();
                    }
                    catch
                    { }
                }
            }
            catch (Exception ex)
            {
                PhUtils.ShowException("Unable to get the list of CSR processes", ex);
                return new List<ProcessHandle>();
            }

            return csrProcesses;
        }

        private ProcessHandle OpenProcessCsr(int pid, ProcessAccess access)
        {
            var csrProcesses = this.GetCsrProcesses();

            foreach (var csrProcess in csrProcesses)
            {
                foreach (var handle in csrProcess.GetHandles())
                {
                    try
                    {
                        // Assume that the handle is a process handle.
                        int handlePid = KProcessHacker.Instance.KphGetProcessId(csrProcess, handle.Handle);

                        if (handlePid == pid)
                            return ProcessHandle.FromHandle(
                                new NativeHandle<ProcessAccess>(csrProcess, handle.Handle, access)
                                );
                        else if (handlePid == 0)
                            throw new Exception(); // HACK
                    }
                    catch
                    {
                        try
                        {
                            // Assume that the handle is a thread handle.
                            int handlePid;

                            int tid = KProcessHacker.Instance.KphGetThreadId(csrProcess, handle.Handle, out handlePid);

                            if (tid == 0)
                                throw new Exception();

                            if (handlePid == pid)
                            {
                                using (var dupHandle =
                                    new NativeHandle<ThreadAccess>(csrProcess, handle.Handle, Program.MinThreadQueryRights))
                                    return ThreadHandle.FromHandle(dupHandle).GetProcess(access);
                            }
                        }
                        catch
                        { }
                    }
                }

                csrProcess.Dispose();
            }

            throw new Exception("Could not find process (hidden from handle table).");
        }

        private ProcessHandle OpenProcess(int pid, ProcessAccess access)
        {
            switch (comboMethod.SelectedItem.ToString())
            {
                case "Brute Force":
                    return new ProcessHandle(pid, access);
                case "CSR Handles":
                    return this.OpenProcessCsr(pid, access);
            }

            return null;
        }

        private void Scan()
        {
            switch (comboMethod.SelectedItem.ToString())
            {
                case "Brute Force":
                    this.ScanBruteForce();
                    break;
                case "CSR Handles":
                    this.ScanCsrHandles();
                    break;
            }
        }

        private void buttonScan_Click(object sender, EventArgs e)
        {
            buttonTerminate.Enabled = false;
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
                            this.OpenProcess(pid, ProcessAccess.Terminate))
                            phandle.Terminate();
                    }
                    catch (Exception ex)
                    {
                        PhUtils.ShowException("Unable to terminate " + item.SubItems[0].Text, ex);
                    }
                }

                // Wait a bit to avoid BSODs
                System.Threading.Thread.Sleep(200);
                buttonTerminate.Enabled = false;
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
                        sw.WriteLine("Process Hacker Hidden Processes Scan");
                        sw.WriteLine("Method: " + comboMethod.SelectedItem.ToString());
                        sw.WriteLine();

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
                    PhUtils.ShowException("Unable to save the scan results", ex);
                }
            }
        }
    }
}
