using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace ProcessHacker
{
    public partial class ProcessThreads : UserControl
    {
        private int _pid;
        private Process _process;
        private ThreadProvider threadP = null;

        public ProcessThreads(int PID)
        {
            InitializeComponent();

            _pid = PID;
            _process = Process.GetProcessById(PID);

            listThreads.ContextMenu = menuThread;
            ColumnSettings.LoadSettings(Properties.Settings.Default.ThreadListViewColumns, listThreads.List);
            GenericViewMenu.AddMenuItems(copyThreadMenuItem.MenuItems, listThreads.List, null);

            threadP = new ThreadProvider(_pid);
            threadP.Interval = Properties.Settings.Default.RefreshInterval;
            listThreads.Provider = threadP;
            threadP.RunOnceAsync();
            threadP.Enabled = true;
        }

        public void Kill()
        {
            threadP.Kill();
        }

        public void SaveSettings()
        {
            Properties.Settings.Default.ThreadListViewColumns = ColumnSettings.SaveSettings(listThreads.List);
        }

        private void SetThreadPriority(ThreadPriorityLevel priority)
        {
            try
            {
                ProcessThread thread = null;

                foreach (ProcessThread t in _process.Threads)
                {
                    if (t.Id.ToString() == listThreads.SelectedItems[0].SubItems[0].Text)
                    {
                        thread = t;
                        break;
                    }
                }

                if (thread == null)
                {
                    MessageBox.Show("Thread not found.", "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                thread.PriorityLevel = priority;
            }
            catch (Exception ex)
            {
                MessageBox.Show("The priority could not be set:\n\n" + ex.Message, "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void listThreads_DoubleClick(object sender, EventArgs e)
        {
            inspectThreadMenuItem_Click(null, null);
        }

        private bool IsDangerousPID(int pid)
        {
            if (pid == 4)
                return true;

            try
            {
                Process p = Process.GetProcessById(pid);

                foreach (string s in Misc.DangerousNames)
                {
                    if ((Environment.SystemDirectory + "\\" + s).ToLower() == Misc.GetRealPath(p.MainModule.FileName).ToLower())
                    {
                        return true;
                    }
                }
            }
            catch { }

            return false;
        }

        #region Thread Context Menu

        private void menuThread_Popup(object sender, EventArgs e)
        {
            if (listThreads.SelectedItems.Count == 0)
            {
                Misc.DisableAllMenuItems(menuThread);

                return;
            }
            else if (listThreads.SelectedItems.Count == 1)
            {
                Misc.EnableAllMenuItems(menuThread);

                timeCriticalThreadMenuItem.Checked = false;
                highestThreadMenuItem.Checked = false;
                aboveNormalThreadMenuItem.Checked = false;
                normalThreadMenuItem.Checked = false;
                belowNormalThreadMenuItem.Checked = false;
                lowestThreadMenuItem.Checked = false;
                idleThreadMenuItem.Checked = false;
                terminateThreadMenuItem.Text = "&Terminate Thread";
                suspendThreadMenuItem.Text = "&Suspend Thread";
                resumeThreadMenuItem.Text = "&Resume Thread";
                priorityThreadMenuItem.Text = "&Priority";

                try
                {
                    ProcessThread thread = null;

                    foreach (ProcessThread t in _process.Threads)
                    {
                        if (t.Id.ToString() == listThreads.SelectedItems[0].SubItems[0].Text)
                        {
                            thread = t;
                            break;
                        }
                    }

                    if (thread == null)
                        return;

                    switch (thread.PriorityLevel)
                    {
                        case ThreadPriorityLevel.TimeCritical:
                            timeCriticalThreadMenuItem.Checked = true;
                            break;

                        case ThreadPriorityLevel.Highest:
                            highestThreadMenuItem.Checked = true;
                            break;

                        case ThreadPriorityLevel.AboveNormal:
                            aboveNormalThreadMenuItem.Checked = true;
                            break;

                        case ThreadPriorityLevel.Normal:
                            normalThreadMenuItem.Checked = true;
                            break;

                        case ThreadPriorityLevel.BelowNormal:
                            belowNormalThreadMenuItem.Checked = true;
                            break;

                        case ThreadPriorityLevel.Lowest:
                            lowestThreadMenuItem.Checked = true;
                            break;

                        case ThreadPriorityLevel.Idle:
                            idleThreadMenuItem.Checked = true;
                            break;
                    }

                    priorityThreadMenuItem.Enabled = true;
                }
                catch (Exception ex)
                {
                    priorityThreadMenuItem.Text = "(" + ex.Message + ")";
                    priorityThreadMenuItem.Enabled = false;
                }
            }
            else
            {
                Misc.DisableAllMenuItems(menuThread);

                terminateThreadMenuItem.Enabled = true;
                suspendThreadMenuItem.Enabled = true;
                resumeThreadMenuItem.Enabled = true;
                terminateThreadMenuItem.Text = "&Terminate Threads";
                suspendThreadMenuItem.Text = "&Suspend Threads";
                resumeThreadMenuItem.Text = "&Resume Threads";
                copyThreadMenuItem.Enabled = true;
            }

            if (listThreads.Items.Count == 0)
            {
                selectAllThreadMenuItem.Enabled = false;
            }
            else
            {
                selectAllThreadMenuItem.Enabled = true;
            }
        }

        private void inspectThreadMenuItem_Click(object sender, EventArgs e)
        {
            if (_pid == Process.GetCurrentProcess().Id)
            {
                if (MessageBox.Show(
                    "Inspecting Process Hacker's threads will lead to instability. Are you sure you want to continue?",
                    "Process Hacker", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2)
                    == DialogResult.No)
                    return;
            }

            if (IsDangerousPID(_pid))
            {
                if (MessageBox.Show(
                  "Inspecting a system process' threads will lead to instability. Are you sure you want to continue?",
                  "Process Hacker", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2)
                  == DialogResult.No)
                    return;
            }

            ThreadWindow window;

            this.UseWaitCursor = true;

            foreach (string s in Symbols.Keys)
            {
                // unload EXE symbols - they usually conflict with the current process
                if (s.ToLower().EndsWith(".exe"))
                    Symbols.UnloadSymbols(s);
            }

            try
            {
                foreach (ProcessModule module in _process.Modules)
                {
                    try
                    {
                        Symbols.LoadSymbolsFromLibrary(module.FileName, module.BaseAddress.ToInt32());
                    }
                    catch (Exception ex)
                    { }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not load symbols for selected process:\n\n" + ex.Message,
                    "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            this.UseWaitCursor = false;

            try
            {
                window = Program.GetThreadWindow(_pid,
                    Int32.Parse(listThreads.SelectedItems[0].SubItems[0].Text),
                    new Program.ThreadWindowInvokeAction(delegate(ThreadWindow f)
                    {
                        try
                        {
                            f.Show();
                            f.Activate();
                        }
                        catch
                        { }
                    }));
            }
            catch
            { }
        }

        private void terminateThreadMenuItem_Click(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.WarnDangerous && IsDangerousPID(_pid))
            {
                DialogResult result = MessageBox.Show("The process with PID " + _pid + " is a system process. Are you" +
                    " sure you want to terminate the selected thread(s)?", "Process Hacker", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);

                if (result == DialogResult.No)
                    return;
            }

            foreach (ListViewItem item in listThreads.SelectedItems)
            {
                try
                {
                    using (Win32.ThreadHandle handle = new Win32.ThreadHandle(Int32.Parse(item.SubItems[0].Text),
                        Win32.THREAD_RIGHTS.THREAD_TERMINATE))
                        handle.Terminate();
                }
                catch (Exception ex)
                {
                    DialogResult result = MessageBox.Show("Could not terminate thread with ID " + item.SubItems[0].Text + ":\n\n" +
                            ex.Message, "Process Hacker", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);

                    if (result == DialogResult.Cancel)
                        return;
                }
            }
        }

        private void suspendThreadMenuItem_Click(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.WarnDangerous && IsDangerousPID(_pid))
            {
                DialogResult result = MessageBox.Show("The process with PID " + _pid + " is a system process. Are you" +
                    " sure you want to suspend the selected thread(s)?", "Process Hacker", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);

                if (result == DialogResult.No)
                    return;
            }

            foreach (ListViewItem item in listThreads.SelectedItems)
            {
                try
                {
                    using (Win32.ThreadHandle handle = new Win32.ThreadHandle(Int32.Parse(item.SubItems[0].Text),
                     Win32.THREAD_RIGHTS.THREAD_SUSPEND_RESUME))
                        handle.Suspend();
                }
                catch (Exception ex)
                {
                    DialogResult result = MessageBox.Show("Could not suspend thread with ID " + item.SubItems[0].Text + ":\n\n" +
                            ex.Message, "Process Hacker", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);

                    if (result == DialogResult.Cancel)
                        return;
                }
            }
        }

        private void resumeThreadMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listThreads.SelectedItems)
            {
                try
                {
                    using (Win32.ThreadHandle handle = new Win32.ThreadHandle(Int32.Parse(item.SubItems[0].Text),
                    Win32.THREAD_RIGHTS.THREAD_SUSPEND_RESUME))
                        handle.Resume();
                }
                catch (Exception ex)
                {
                    DialogResult result = MessageBox.Show("Could not resume thread with ID " + item.SubItems[0].Text + ":\n\n" +
                            ex.Message, "Process Hacker", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);

                    if (result == DialogResult.Cancel)
                        return;
                }
            }
        }

        #region Priority

        private void timeCriticalThreadMenuItem_Click(object sender, EventArgs e)
        {
            SetThreadPriority(ThreadPriorityLevel.TimeCritical);
        }

        private void highestThreadMenuItem_Click(object sender, EventArgs e)
        {
            SetThreadPriority(ThreadPriorityLevel.Highest);
        }

        private void aboveNormalThreadMenuItem_Click(object sender, EventArgs e)
        {
            SetThreadPriority(ThreadPriorityLevel.AboveNormal);
        }

        private void normalThreadMenuItem_Click(object sender, EventArgs e)
        {
            SetThreadPriority(ThreadPriorityLevel.Normal);
        }

        private void belowNormalThreadMenuItem_Click(object sender, EventArgs e)
        {
            SetThreadPriority(ThreadPriorityLevel.BelowNormal);
        }

        private void lowestThreadMenuItem_Click(object sender, EventArgs e)
        {
            SetThreadPriority(ThreadPriorityLevel.Lowest);
        }

        private void idleThreadMenuItem_Click(object sender, EventArgs e)
        {
            SetThreadPriority(ThreadPriorityLevel.Idle);
        }

        #endregion

        private void selectAllThreadMenuItem_Click(object sender, EventArgs e)
        {
            Misc.SelectAll(listThreads.Items);
        }

        #endregion
    }
}
