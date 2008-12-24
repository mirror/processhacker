/*
 * Process Hacker
 * 
 * Copyright (C) 2008 wj32
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Reflection;
using System.Windows.Forms;
using System.Diagnostics;

namespace ProcessHacker
{
    public partial class ThreadList : UserControl
    {
        ThreadProvider _provider;
        public new event KeyEventHandler KeyDown;
        public new event MouseEventHandler MouseDown;
        public new event MouseEventHandler MouseUp;
        public event EventHandler SelectedIndexChanged;

        public ThreadList()
        {
            InitializeComponent();

            listThreads.KeyDown += new KeyEventHandler(ThreadList_KeyDown);
            listThreads.MouseDown += new MouseEventHandler(listThreads_MouseDown);
            listThreads.MouseUp += new MouseEventHandler(listThreads_MouseUp);
            listThreads.SelectedIndexChanged += new System.EventHandler(listThreads_SelectedIndexChanged);

            listThreads.ContextMenu = menuThread;
        }

        private void listThreads_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.MouseUp != null)
                this.MouseUp(sender, e);
        }

        private void listThreads_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.MouseDown != null)
                this.MouseDown(sender, e);
        }

        private void listThreads_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (this.SelectedIndexChanged != null)
                this.SelectedIndexChanged(sender, e);
        }

        private void ThreadList_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.KeyDown != null)
                this.KeyDown(sender, e);
        }

        #region Properties

        public new bool DoubleBuffered
        {
            get
            {
                return (bool)typeof(ListView).GetProperty("DoubleBuffered",
                    BindingFlags.NonPublic | BindingFlags.Instance).GetValue(listThreads, null);
            }
            set
            {
                typeof(ListView).GetProperty("DoubleBuffered",
                    BindingFlags.NonPublic | BindingFlags.Instance).SetValue(listThreads, value, null);
            }
        }

        public override bool Focused
        {
            get
            {
                return listThreads.Focused;
            }
        }

        public override ContextMenu ContextMenu
        {
            get { return listThreads.ContextMenu; }
            set { listThreads.ContextMenu = value; }
        }

        public override ContextMenuStrip ContextMenuStrip
        {
            get { return listThreads.ContextMenuStrip; }
            set { listThreads.ContextMenuStrip = value; }
        }

        public ListView List
        {
            get { return listThreads; }
        }

        public ThreadProvider Provider
        {
            get { return _provider; }
            set
            {
                _pid = -1;
                _process = null;

                if (_provider != null)
                {
                    _provider.DictionaryAdded -= new Provider<int, ThreadItem>.ProviderDictionaryAdded(provider_DictionaryAdded);
                    _provider.DictionaryModified -= new Provider<int, ThreadItem>.ProviderDictionaryModified(provider_DictionaryModified);
                    _provider.DictionaryRemoved -= new Provider<int, ThreadItem>.ProviderDictionaryRemoved(provider_DictionaryRemoved);
                }

                _provider = value;

                listThreads.Items.Clear();

                if (_provider != null)
                {
                    foreach (ThreadItem item in _provider.Dictionary.Values)
                    {
                        provider_DictionaryAdded(item);
                    }

                    _provider.UseInvoke = true;
                    _provider.Invoke = new Provider<int, ThreadItem>.ProviderInvokeMethod(this.BeginInvoke);
                    _provider.DictionaryAdded += new Provider<int, ThreadItem>.ProviderDictionaryAdded(provider_DictionaryAdded);
                    _provider.DictionaryModified += new Provider<int, ThreadItem>.ProviderDictionaryModified(provider_DictionaryModified);
                    _provider.DictionaryRemoved += new Provider<int, ThreadItem>.ProviderDictionaryRemoved(provider_DictionaryRemoved);

                    _pid = _provider.PID;
                    _process = Process.GetProcessById(_pid);
                }
            }
        }

        #endregion

        #region Core Thread List

        private System.Drawing.Color GetThreadColor(ThreadItem titem)
        {
            if (titem.WaitReason == "Suspended")
                return System.Drawing.Color.LightGray;

            return System.Drawing.SystemColors.Window;
        }

        private void provider_DictionaryAdded(ThreadItem item)
        {
            HighlightedListViewItem litem = new HighlightedListViewItem();

            litem.Name = item.TID.ToString();
            litem.Text = item.TID.ToString();
            litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem, item.StartAddress));
            litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem, item.CPUTime));
            litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem, item.Priority));

            litem.NormalColor = GetThreadColor(item);

            listThreads.Items.Add(litem);
        }

        private void provider_DictionaryModified(ThreadItem oldItem, ThreadItem newItem)
        {
            lock (listThreads)
            {
                ListViewItem litem = listThreads.Items[newItem.TID.ToString()];

                if (litem == null)
                    return;

                litem.SubItems[1].Text = newItem.StartAddress;
                litem.SubItems[2].Text = newItem.CPUTime;
                litem.SubItems[3].Text = newItem.Priority;

                (litem as HighlightedListViewItem).NormalColor = GetThreadColor(newItem);
            }
        }

        private void provider_DictionaryRemoved(ThreadItem item)
        {
            int index = listThreads.Items[item.TID.ToString()].Index;
            bool selected = listThreads.Items[item.TID.ToString()].Selected;
            int selectedCount = listThreads.SelectedItems.Count;

            listThreads.Items[item.TID.ToString()].Remove();

            if (selected && selectedCount == 1)
            {
                if (listThreads.Items.Count == 0)
                { }
                else if (index > (listThreads.Items.Count - 1))
                {
                    listThreads.Items[listThreads.Items.Count - 1].Selected = true;
                }
                else
                {
                    listThreads.Items[index].Selected = true;
                }
            }
        }

        #endregion

        #region Interfacing

        public void BeginUpdate()
        {
            listThreads.BeginUpdate();
        }

        public void EndUpdate()
        {
            listThreads.EndUpdate();
        }

        public ListView.ListViewItemCollection Items
        {
            get { return listThreads.Items; }
        }

        public ListView.SelectedListViewItemCollection SelectedItems
        {
            get { return listThreads.SelectedItems; }
        }

        #endregion

        int _pid;
        Process _process;

        public void SaveSettings()
        {
            Properties.Settings.Default.ThreadListViewColumns = ColumnSettings.SaveSettings(listThreads);
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
                    catch
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
