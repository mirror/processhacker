/*
 * Process Hacker - 
 *   thread list
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
using System.Reflection;
using System.Windows.Forms;
using System.Diagnostics;

namespace ProcessHacker
{
    public partial class ThreadList : UserControl
    {
        private ThreadProvider _provider;
        private HighlightingContext _highlightingContext;
        public new event KeyEventHandler KeyDown;
        public new event MouseEventHandler MouseDown;
        public new event MouseEventHandler MouseUp;
        public event EventHandler SelectedIndexChanged;
        private int _pid;
        private Process _process;

        public ThreadList()
        {
            InitializeComponent();

            // Use Cycles instead of Context Switches on Vista
            if (Program.WindowsVersion != WindowsVersion.XP)
                listThreads.Columns[1].Text = "Cycles Delta";

            _highlightingContext = new HighlightingContext(listThreads);
            listThreads.ListViewItemSorter = new SortedListComparer(listThreads);

            (listThreads.ListViewItemSorter as SortedListComparer).CustomSorters.Add(1,
                (x, y) =>
                {
                    if (Program.WindowsVersion == WindowsVersion.XP)
                    {
                        return (x.Tag as ThreadItem).ContextSwitchesDelta.CompareTo((y.Tag as ThreadItem).ContextSwitchesDelta);
                    }
                    else
                    {
                        return (x.Tag as ThreadItem).CyclesDelta.CompareTo((y.Tag as ThreadItem).CyclesDelta);
                    }
                });

            listThreads.KeyDown += new KeyEventHandler(ThreadList_KeyDown);
            listThreads.MouseDown += new MouseEventHandler(listThreads_MouseDown);
            listThreads.MouseUp += new MouseEventHandler(listThreads_MouseUp);
            listThreads.SelectedIndexChanged += new System.EventHandler(listThreads_SelectedIndexChanged);

            ColumnSettings.LoadSettings(Properties.Settings.Default.ThreadListViewColumns, listThreads);
            listThreads.ContextMenu = menuThread;
            GenericViewMenu.AddMenuItems(copyThreadMenuItem.MenuItems, listThreads, null);
            listThreads_SelectedIndexChanged(null, null);
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
            if (listThreads.SelectedItems.Count == 1)
            {
                try
                {
                    int tid = int.Parse(listThreads.SelectedItems[0].Name);
                    ProcessItem process = Program.HackerWindow.ProcessProvider.Dictionary[_pid];
                    ProcessThread thread = Misc.GetThreadById(Process.GetProcessById(_pid), tid);

                    fileModule.Text = _provider.Symbols.GetModuleFromAddress(_provider.Dictionary[tid].StartAddressI);
                    fileModule.Enabled = true;

                    if (thread.ThreadState == ThreadState.Wait)
                    {
                        labelState.Text = "Wait: " + process.Threads[tid].WaitReason.ToString();
                    }
                    else
                    {
                        labelState.Text = thread.ThreadState.ToString();
                    }

                    labelKernelTime.Text = Misc.GetNiceTimeSpan(thread.PrivilegedProcessorTime);
                    labelUserTime.Text = Misc.GetNiceTimeSpan(thread.UserProcessorTime);
                    labelTotalTime.Text = Misc.GetNiceTimeSpan(thread.TotalProcessorTime);
                    labelPriority.Text = process.Threads[tid].Priority.ToString();
                    labelBasePriority.Text = process.Threads[tid].BasePriority.ToString();
                    labelContextSwitches.Text = process.Threads[tid].ContextSwitchCount.ToString("N0");

                    using (Win32.ThreadHandle thandle = new Win32.ThreadHandle(tid))
                        labelTEBAddress.Text = "0x" + thandle.GetBasicInformation().TebBaseAddress.ToString("x8");
                }
                catch
                { }
            }
            else
            {
                fileModule.Text = "";
                fileModule.Enabled = false;
                labelState.Text = "";
                labelKernelTime.Text = "";
                labelUserTime.Text = "";
                labelTotalTime.Text = "";
                labelTEBAddress.Text = "";
                labelPriority.Text = "";
                labelBasePriority.Text = "";
                labelContextSwitches.Text = "";
            }

            if (this.SelectedIndexChanged != null)
                this.SelectedIndexChanged(sender, e);
        }

        private void ThreadList_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.KeyDown != null)
                this.KeyDown(sender, e);
        }

        #region Properties

        public bool Highlight { get; set; }

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
                    _provider.DictionaryAdded -= new ThreadProvider.ProviderDictionaryAdded(provider_DictionaryAdded);
                    _provider.DictionaryModified -= new ThreadProvider.ProviderDictionaryModified(provider_DictionaryModified);
                    _provider.DictionaryRemoved -= new ThreadProvider.ProviderDictionaryRemoved(provider_DictionaryRemoved);
                    _provider.Updated -= new ThreadProvider.ProviderUpdateOnce(provider_Updated);
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
                    _provider.Invoke = new ThreadProvider.ProviderInvokeMethod(this.BeginInvoke);
                    _provider.DictionaryAdded += new ThreadProvider.ProviderDictionaryAdded(provider_DictionaryAdded);
                    _provider.DictionaryModified += new ThreadProvider.ProviderDictionaryModified(provider_DictionaryModified);
                    _provider.DictionaryRemoved += new ThreadProvider.ProviderDictionaryRemoved(provider_DictionaryRemoved);
                    _provider.Updated += new ThreadProvider.ProviderUpdateOnce(provider_Updated);

                    _pid = _provider.PID;
                    _process = Process.GetProcessById(_pid);
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

        private void provider_Updated()
        {
            _highlightingContext.Tick();
        }   

        private System.Drawing.Color GetThreadColor(ThreadItem titem)
        {
            if (Properties.Settings.Default.UseColorSuspended && titem.WaitReason == Win32.KWAIT_REASON.Suspended)
                return Properties.Settings.Default.ColorSuspended;
            else if (Properties.Settings.Default.UseColorGuiThreads && titem.IsGuiThread)
                return Properties.Settings.Default.ColorGuiThreads;

            return System.Drawing.SystemColors.Window;
        }

        private void provider_DictionaryAdded(ThreadItem item)
        {
            HighlightedListViewItem litem = new HighlightedListViewItem(_highlightingContext, this.Highlight);

            litem.Name = item.TID.ToString();
            litem.Text = item.TID.ToString();
            litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem, ""));
            litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem, item.StartAddress));
            litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem, item.Priority));
            litem.Tag = item;

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

                if (Program.WindowsVersion == WindowsVersion.XP)
                {
                    if (newItem.ContextSwitchesDelta == 0)
                        litem.SubItems[1].Text = "";
                    else
                        litem.SubItems[1].Text = newItem.ContextSwitchesDelta.ToString("N0");
                }
                else
                {
                    if (newItem.CyclesDelta == 0)
                        litem.SubItems[1].Text = "";
                    else
                        litem.SubItems[1].Text = newItem.CyclesDelta.ToString("N0");
                }

                litem.SubItems[2].Text = newItem.StartAddress;
                litem.SubItems[3].Text = newItem.Priority;
                litem.Tag = newItem;

                (litem as HighlightedListViewItem).NormalColor = GetThreadColor(newItem);
                listThreads.Sort();
            }
        }

        private void provider_DictionaryRemoved(ThreadItem item)
        {
            int index = listThreads.Items[item.TID.ToString()].Index;
            bool selected = listThreads.Items[item.TID.ToString()].Selected;
            int selectedCount = listThreads.SelectedItems.Count;

            listThreads.Items[item.TID.ToString()].Remove();
        }

        public void SaveSettings()
        {
            Properties.Settings.Default.ThreadListViewColumns = ColumnSettings.SaveSettings(listThreads);
        }

        private void SetThreadPriority(ThreadPriorityLevel priority)
        {
            try
            {
                int tid = int.Parse(listThreads.SelectedItems[0].SubItems[0].Text);

                using (var thandle = new Win32.ThreadHandle(tid,
                    Program.WindowsVersion == WindowsVersion.Vista ? 
                    Win32.THREAD_RIGHTS.THREAD_SET_LIMITED_INFORMATION :
                    Win32.THREAD_RIGHTS.THREAD_SET_INFORMATION))
                    thandle.SetPriorityLevel(priority);
            }
            catch (Exception ex)
            {
                MessageBox.Show("The priority could not be set:\n\n" + ex.Message, 
                    "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void listThreads_DoubleClick(object sender, EventArgs e)
        {
            inspectThreadMenuItem_Click(null, null);
        }

        private void menuThread_Popup(object sender, EventArgs e)
        {
            if (listThreads.SelectedItems.Count == 0)
            {
                menuThread.DisableAll();

                return;
            }
            else if (listThreads.SelectedItems.Count == 1)
            {
                menuThread.EnableAll();

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
                    using (var thandle = new Win32.ThreadHandle(
                        int.Parse(listThreads.SelectedItems[0].SubItems[0].Text), 
                        Program.MinThreadQueryRights))
                    {
                        switch (thandle.GetPriorityLevel())
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
                menuThread.DisableAll();

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

            if (Misc.IsDangerousPid(_pid))
            {
                if (MessageBox.Show(
                  "Inspecting a system process' threads will lead to instability. Are you sure you want to continue?",
                  "Process Hacker", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2)
                  == DialogResult.No)
                    return;
            }

            ThreadWindow window;

            try
            {
                window = Program.GetThreadWindow(_pid,
                    Int32.Parse(listThreads.SelectedItems[0].SubItems[0].Text),
                    _provider.Symbols,
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
            if (Properties.Settings.Default.WarnDangerous && Misc.IsDangerousPid(_pid))
            {
                DialogResult result = MessageBox.Show("The process with PID " + _pid + " is a system process. Are you" +
                    " sure you want to terminate the selected thread(s)?", "Process Hacker", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);

                if (result == DialogResult.No)
                    return;
            }

            if (Program.ElevationType == Win32.TOKEN_ELEVATION_TYPE.TokenElevationTypeLimited && Program.KPH == null)
            {
                try
                {
                    foreach (ListViewItem item in listThreads.SelectedItems)
                    {
                        using (var thandle = new Win32.ThreadHandle(int.Parse(item.SubItems[0].Text),
                            Win32.THREAD_RIGHTS.THREAD_TERMINATE))
                        { }
                    }
                }
                catch
                {
                    string objects = "";

                    foreach (ListViewItem item in listThreads.SelectedItems)
                        objects += item.SubItems[0].Text + ",";

                    Program.StartProcessHackerAdmin("-e -type thread -action terminate -obj \"" +
                        objects + "\" -hwnd " + this.Handle.ToString(), null, this.Handle);

                    return;
                }
            }

            foreach (ListViewItem item in listThreads.SelectedItems)
            {
                try
                {
                    using (var thandle = new Win32.ThreadHandle(Int32.Parse(item.SubItems[0].Text),
                        Win32.THREAD_RIGHTS.THREAD_TERMINATE))
                        thandle.Terminate();
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
            if (Properties.Settings.Default.WarnDangerous && Misc.IsDangerousPid(_pid))
            {
                DialogResult result = MessageBox.Show("The process with PID " + _pid + " is a system process. Are you" +
                    " sure you want to suspend the selected thread(s)?", "Process Hacker", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);

                if (result == DialogResult.No)
                    return;
            }

            if (Program.ElevationType == Win32.TOKEN_ELEVATION_TYPE.TokenElevationTypeLimited && Program.KPH == null)
            {
                try
                {
                    foreach (ListViewItem item in listThreads.SelectedItems)
                    {
                        using (var thandle = new Win32.ThreadHandle(int.Parse(item.SubItems[0].Text),
                            Win32.THREAD_RIGHTS.THREAD_SUSPEND_RESUME))
                        { }
                    }
                }
                catch
                {
                    string objects = "";

                    foreach (ListViewItem item in listThreads.SelectedItems)
                        objects += item.SubItems[0].Text + ",";

                    Program.StartProcessHackerAdmin("-e -type thread -action suspend -obj \"" +
                        objects + "\" -hwnd " + this.Handle.ToString(), null, this.Handle);

                    return;
                }
            }

            foreach (ListViewItem item in listThreads.SelectedItems)
            {
                try
                {
                    using (var thandle = new Win32.ThreadHandle(Int32.Parse(item.SubItems[0].Text),
                     Win32.THREAD_RIGHTS.THREAD_SUSPEND_RESUME))
                        thandle.Suspend();
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
            if (Properties.Settings.Default.WarnDangerous && Misc.IsDangerousPid(_pid))
            {
                DialogResult result = MessageBox.Show("The process with PID " + _pid + " is a system process. Are you" +
                    " sure you want to resume the selected thread(s)?", "Process Hacker", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);

                if (result == DialogResult.No)
                    return;
            }

            if (Program.ElevationType == Win32.TOKEN_ELEVATION_TYPE.TokenElevationTypeLimited && Program.KPH == null)
            {
                try
                {
                    foreach (ListViewItem item in listThreads.SelectedItems)
                    {
                        using (var thandle = new Win32.ThreadHandle(int.Parse(item.SubItems[0].Text),
                            Win32.THREAD_RIGHTS.THREAD_SUSPEND_RESUME))
                        { }
                    }
                }
                catch
                {
                    string objects = "";

                    foreach (ListViewItem item in listThreads.SelectedItems)
                        objects += item.SubItems[0].Text + ",";

                    Program.StartProcessHackerAdmin("-e -type thread -action resume -obj \"" +
                        objects + "\" -hwnd " + this.Handle.ToString(), null, this.Handle);

                    return;
                }
            }
            foreach (ListViewItem item in listThreads.SelectedItems)
            {
                try
                {
                    using (var thandle = new Win32.ThreadHandle(Int32.Parse(item.SubItems[0].Text),
                    Win32.THREAD_RIGHTS.THREAD_SUSPEND_RESUME))
                        thandle.Resume();
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

        private void inspectTEBMenuItem_Click(object sender, EventArgs e)
        {
            if (!Program.Structs.ContainsKey("TEB"))
            {
                MessageBox.Show("The struct 'TEB' has not been loaded. Make sure structs.txt was loaded successfully.",
                    "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                using (Win32.ThreadHandle thandle = new Win32.ThreadHandle(int.Parse(listThreads.SelectedItems[0].Text)))
                {
                    int tebBaseAddress = thandle.GetBasicInformation().TebBaseAddress;

                    Program.HackerWindow.BeginInvoke(new MethodInvoker(delegate
                        {
                            StructWindow sw = new StructWindow(_pid, tebBaseAddress, Program.Structs["TEB"]);

                            try
                            {
                                sw.Show();
                                sw.Activate();
                            }
                            catch
                            { }
                        }));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
    }
}
