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
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using ProcessHacker.Native;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;
using ProcessHacker.UI;
using System.Text;

namespace ProcessHacker.Components
{
    public partial class ThreadList : UserControl
    {
        private ThreadProvider _provider;
        private int _runCount = 0;
        private HighlightingContext _highlightingContext;
        private bool _needsSort = false;
        public new event KeyEventHandler KeyDown;
        public new event MouseEventHandler MouseDown;
        public new event MouseEventHandler MouseUp;
        public event EventHandler SelectedIndexChanged;
        private int _pid;

        public ThreadList()
        {
            InitializeComponent();

            // Use Cycles instead of Context Switches on Vista
            if (OSVersion.HasCycleTime)
                listThreads.Columns[1].Text = "Cycles Delta";

            _highlightingContext = new HighlightingContext(listThreads);
            var comparer = (SortedListViewComparer)
                (listThreads.ListViewItemSorter = new SortedListViewComparer(listThreads));

            comparer.CustomSorters.Add(1,
                (x, y) =>
                {
                    if (OSVersion.HasCycleTime)
                    {
                        return (x.Tag as ThreadItem).CyclesDelta.CompareTo((y.Tag as ThreadItem).CyclesDelta);
                    }
                    else
                    {
                        return (x.Tag as ThreadItem).ContextSwitchesDelta.CompareTo((y.Tag as ThreadItem).ContextSwitchesDelta);
                    }
                });
            comparer.CustomSorters.Add(3,
                (x, y) =>
                    {
                        return (x.Tag as ThreadItem).PriorityI.CompareTo((y.Tag as ThreadItem).PriorityI);
                    });
            comparer.ColumnSortOrder.Add(0);
            comparer.ColumnSortOrder.Add(2);
            comparer.ColumnSortOrder.Add(3);
            comparer.ColumnSortOrder.Add(1);
            comparer.SortColumn = 1;
            comparer.SortOrder = SortOrder.Descending;

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
            this.Cursor = Cursors.WaitCursor;

            if (listThreads.SelectedItems.Count == 1)
            {
                try
                {
                    int tid = int.Parse(listThreads.SelectedItems[0].Name);
                    var thread = Windows.GetProcessThreads(_pid)[tid];
                    ProcessThread processThread = null;
                    string fileName;

                    try
                    {
                        processThread = Misc.GetThreadById(Process.GetProcessById(_pid), tid);
                    }
                    catch
                    { }

                    if (!_provider.Symbols.Busy)
                    {
                        try
                        {
                            _provider.Symbols.GetSymbolFromAddress(_provider.Dictionary[tid].StartAddressI, out fileName);
                            fileModule.Text = fileName;
                            fileModule.Enabled = true;
                        }
                        catch
                        {
                            fileModule.Enabled = false;
                        }
                    }
                    else
                    {
                        fileModule.Enabled = false;
                    }

                    if (processThread != null)
                    {
                        if (processThread.ThreadState == ThreadState.Wait)
                        {
                            labelState.Text = "Wait: " + thread.WaitReason.ToString();
                        }
                        else
                        {
                            labelState.Text = processThread.ThreadState.ToString();
                        }

                        labelKernelTime.Text = Misc.GetNiceTimeSpan(processThread.PrivilegedProcessorTime);
                        labelUserTime.Text = Misc.GetNiceTimeSpan(processThread.UserProcessorTime);
                        labelTotalTime.Text = Misc.GetNiceTimeSpan(processThread.TotalProcessorTime);
                    }

                    labelPriority.Text = thread.Priority.ToString();
                    labelBasePriority.Text = thread.BasePriority.ToString();
                    labelContextSwitches.Text = thread.ContextSwitchCount.ToString("N0");

                    using (ThreadHandle thandle = new ThreadHandle(tid))
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

            this.Cursor = Cursors.Default;
        }

        private void ThreadList_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.KeyDown != null)
                this.KeyDown(sender, e);

            if (!e.Handled)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    inspectThreadMenuItem_Click(null, null);
                }
                else if (e.KeyCode == Keys.Delete)
                {
                    terminateThreadMenuItem_Click(null, null);
                }
            }
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

                if (_provider != null)
                {
                    _provider.DictionaryAdded -= new ThreadProvider.ProviderDictionaryAdded(provider_DictionaryAdded);
                    _provider.DictionaryModified -= new ThreadProvider.ProviderDictionaryModified(provider_DictionaryModified);
                    _provider.DictionaryRemoved -= new ThreadProvider.ProviderDictionaryRemoved(provider_DictionaryRemoved);
                    _provider.Updated -= new ThreadProvider.ProviderUpdateOnce(provider_Updated);
                    _provider.LoadingStateChanged -= new ThreadProvider.LoadingStateChangedDelegate(provider_LoadingStateChanged);
                }

                _provider = value;

                listThreads.Items.Clear();

                if (_provider != null)
                {
                    foreach (ThreadItem item in _provider.Dictionary.Values)
                    {
                        provider_DictionaryAdded(item);
                    }

                    _provider.DictionaryAdded += new ThreadProvider.ProviderDictionaryAdded(provider_DictionaryAdded);
                    _provider.DictionaryModified += new ThreadProvider.ProviderDictionaryModified(provider_DictionaryModified);
                    _provider.DictionaryRemoved += new ThreadProvider.ProviderDictionaryRemoved(provider_DictionaryRemoved);
                    _provider.Updated += new ThreadProvider.ProviderUpdateOnce(provider_Updated);
                    _provider.LoadingStateChanged += new ThreadProvider.LoadingStateChangedDelegate(provider_LoadingStateChanged);

                    _pid = _provider.Pid;
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

            if (_needsSort)
            {
                this.BeginInvoke(new MethodInvoker(() =>
                    {
                        if (_needsSort)
                        {
                            listThreads.Sort();
                            _needsSort = false;
                        }
                    }));
            }

            _runCount++;
        }   

        private System.Drawing.Color GetThreadColor(ThreadItem titem)
        {
            if (Properties.Settings.Default.UseColorSuspended && titem.WaitReason == KWaitReason.Suspended)
                return Properties.Settings.Default.ColorSuspended;
            else if (Properties.Settings.Default.UseColorGuiThreads && titem.IsGuiThread)
                return Properties.Settings.Default.ColorGuiThreads;

            return System.Drawing.SystemColors.Window;
        }

        private void provider_DictionaryAdded(ThreadItem item)
        {
            this.BeginInvoke(new MethodInvoker(() =>
                {
                    HighlightedListViewItem litem = new HighlightedListViewItem(_highlightingContext,
                        item.RunId > 0 && _runCount > 0);

                    litem.Name = item.Tid.ToString();
                    litem.Text = item.Tid.ToString();
                    litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem, ""));
                    litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem, item.StartAddress));
                    litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem, item.Priority));
                    litem.Tag = item;
                    litem.NormalColor = GetThreadColor(item);

                    listThreads.Items.Add(litem);
                }));
        }

        private void provider_DictionaryModified(ThreadItem oldItem, ThreadItem newItem)
        {
            this.BeginInvoke(new MethodInvoker(() =>
                {
                    lock (listThreads)
                    {
                        ListViewItem litem = listThreads.Items[newItem.Tid.ToString()];

                        if (litem == null)
                            return;

                        if (!OSVersion.HasCycleTime)
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
                        _needsSort = true;
                    }
                }));
        }

        private void provider_DictionaryRemoved(ThreadItem item)
        {
            this.BeginInvoke(new MethodInvoker(() =>
                {
                    lock (listThreads)
                    {
                        int index = listThreads.Items[item.Tid.ToString()].Index;
                        bool selected = listThreads.Items[item.Tid.ToString()].Selected;
                        int selectedCount = listThreads.SelectedItems.Count;

                        listThreads.Items[item.Tid.ToString()].Remove();
                    }
                }));
        }

        private void provider_LoadingStateChanged(bool loading)
        {
            this.BeginInvoke(new MethodInvoker(() =>
                {
                    if (loading)
                        listThreads.Cursor = Cursors.AppStarting;
                    else
                        listThreads.Cursor = Cursors.Default;
                }));
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

                using (var thandle = new ThreadHandle(tid, OSVersion.MinThreadSetInfoAccess))
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
                    using (var thandle = new ThreadHandle(
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
            if (listThreads.SelectedItems.Count != 1)
                return;

            if (_pid == 4)
            {
                MessageBox.Show(
                    "Process Hacker does not currently support viewing kernel call stacks.",
                    "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            if (_pid == Win32.GetCurrentProcessId())
            {
                if (MessageBox.Show(
                    "Inspecting Process Hacker's threads may lead to instability. Are you sure you want to continue?",
                    "Process Hacker", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2)
                    == DialogResult.No)
                    return;
            }

            if (Misc.IsDangerousPid(_pid))
            {
                if (MessageBox.Show(
                  "Inspecting a system process' threads may lead to instability. Are you sure you want to continue?",
                  "Process Hacker", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2)
                  == DialogResult.No)
                    return;
            }

            try
            {
                ProcessHandle phandle = null;

                // If we have KPH, we don't need much access.
                if (KProcessHacker.Instance != null)
                {
                    if ((_provider.ProcessAccess & ProcessAccess.QueryLimitedInformation) != 0 || 
                        (_provider.ProcessAccess & ProcessAccess.QueryInformation) != 0)
                        phandle = _provider.ProcessHandle;
                }
                else
                {
                    if ((_provider.ProcessAccess & (ProcessAccess.QueryInformation | ProcessAccess.VmRead)) != 0)
                        phandle = _provider.ProcessHandle;
                }

                (new ThreadWindow(
                    _pid,
                    Int32.Parse(listThreads.SelectedItems[0].SubItems[0].Text),
                    _provider.Symbols,
                    phandle
                    )
                    ).ShowDialog(this);
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
            }
        }

        private void terminateThreadMenuItem_Click(object sender, EventArgs e)
        {              
            if (listThreads.SelectedItems.Count == 0)
                return;

            if (MessageBox.Show("Are you sure you want to terminate the selected thread(s)?",
                "Process Hacker", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, 
                MessageBoxDefaultButton.Button2) == DialogResult.No)
                return;

            if (Program.ElevationType == TokenElevationType.Limited && 
                KProcessHacker.Instance == null)
            {
                try
                {
                    foreach (ListViewItem item in listThreads.SelectedItems)
                    {
                        using (var thandle = new ThreadHandle(int.Parse(item.SubItems[0].Text),
                            ThreadAccess.Terminate))
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
                    using (var thandle = new ThreadHandle(Int32.Parse(item.SubItems[0].Text),
                        ThreadAccess.Terminate))
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

            if (Program.ElevationType == TokenElevationType.Limited &&
                KProcessHacker.Instance == null)
            {
                try
                {
                    foreach (ListViewItem item in listThreads.SelectedItems)
                    {
                        using (var thandle = new ThreadHandle(int.Parse(item.SubItems[0].Text),
                            ThreadAccess.SuspendResume))
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
                    using (var thandle = new ThreadHandle(Int32.Parse(item.SubItems[0].Text),
                        ThreadAccess.SuspendResume))
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

            if (Program.ElevationType == TokenElevationType.Limited &&
                KProcessHacker.Instance == null)
            {
                try
                {
                    foreach (ListViewItem item in listThreads.SelectedItems)
                    {
                        using (var thandle = new ThreadHandle(int.Parse(item.SubItems[0].Text),
                            ThreadAccess.SuspendResume))
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
                    using (var thandle = new ThreadHandle(Int32.Parse(item.SubItems[0].Text),
                        ThreadAccess.SuspendResume))
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
                using (ThreadHandle thandle = new ThreadHandle(int.Parse(listThreads.SelectedItems[0].Text)))
                {
                    IntPtr tebBaseAddress = thandle.GetBasicInformation().TebBaseAddress;

                    Program.HackerWindow.BeginInvoke(new MethodInvoker(delegate
                        {
                            StructWindow sw = new StructWindow(_pid, tebBaseAddress, Program.Structs["TEB"]);

                            try
                            {
                                sw.Show();
                                sw.Activate();
                            }
                            catch (Exception ex)
                            {
                                Logging.Log(ex);
                            }
                        }));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private SystemHandleInformation GetShiForHandle(int pid, IntPtr handle)
        {
            short handleValue = (short)handle.ToInt32();
            var handles = Windows.GetHandles();

            foreach (var handleInfo in handles)
            {
                if (handleInfo.ProcessId == pid && handleInfo.Handle == handleValue)
                    return handleInfo;
            }

            return new SystemHandleInformation();
        }

        #region Analyze

        private string GetHandleString(int pid, IntPtr handle)
        {
            var shi = this.GetShiForHandle(pid, handle);

            try
            {
                var handleInfo = shi.GetHandleInfo();

                return "Handle 0x" + handle.ToString("x") + " (" + handleInfo.TypeName + "): " +
                    (string.IsNullOrEmpty(handleInfo.BestName) ? "(unnamed object)" : handleInfo.BestName);
            }
            catch
            {
                return "Handle 0x" + handle.ToString("x") + ": (error querying name)";
            }
        }

        private unsafe void analyzeWaitMenuItem_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            int tid = int.Parse(listThreads.SelectedItems[0].SubItems[0].Text);

            try
            {
                ProcessHandle phandle = null;

                if ((_provider.ProcessAccess & (ProcessAccess.QueryInformation | ProcessAccess.VmRead)) != 0)
                    phandle = _provider.ProcessHandle;
                else
                    phandle = new ProcessHandle(_pid, ProcessAccess.QueryInformation | ProcessAccess.VmRead);

                ProcessHandle processDupHandle = new ProcessHandle(_pid, ProcessAccess.DupHandle);

                bool found = false;

                using (var thandle = new ThreadHandle(tid, ThreadAccess.GetContext | ThreadAccess.SuspendResume))
                {
                    thandle.WalkStack(phandle, (stackFrame) =>
                        {
                            uint address = stackFrame.PcAddress.ToUInt32();
                            string name = _provider.Symbols.GetSymbolFromAddress(address).ToLower();

                            if (
                                name.StartsWith("ntdll.dll!zwwaitforsingleobject") ||
                                name.StartsWith("ntdll.dll!ntwaitforsingleobject") || 
                                name.StartsWith("kernel32.dll!waitforsingleobject")
                                )
                            {
                                found = true;
                                
                                IntPtr handle = stackFrame.Params[0];
                                bool alertable = stackFrame.Params[1].ToInt32() != 0;

                                sb.AppendLine("Thread " + tid.ToString() + 
                                    " is waiting (alertable: " + alertable.ToString() + ") for:");

                                sb.AppendLine(this.GetHandleString(_pid, handle));
                            }
                            else if (
                                name.StartsWith("ntdll.dll!zwwaitformultipleobjects") ||
                                name.StartsWith("ntdll.dll!ntwaitformultipleobjects") || 
                                name.StartsWith("kernel32.dll!waitformultipleobjects")
                                )
                            {
                                found = true;

                                int handleCount = stackFrame.Params[0].ToInt32();
                                IntPtr handleAddress = stackFrame.Params[1];
                                WaitType waitType = (WaitType)stackFrame.Params[2].ToInt32();
                                bool alertable = stackFrame.Params[3].ToInt32() != 0;
                                IntPtr* handles = stackalloc IntPtr[handleCount];

                                phandle.ReadMemory(handleAddress, handles, handleCount * IntPtr.Size);

                                sb.AppendLine("Thread " + tid.ToString() +
                                    " is waiting (alertable: " + alertable.ToString() + ", wait type: " + 
                                    waitType.ToString() + ") for:");

                                for (int i = 0; i < handleCount; i++)
                                {
                                    sb.AppendLine(this.GetHandleString(_pid, handles[i]));
                                }
                            }
                            else if (
                                name.StartsWith("ntdll.dll!zwremoveiocompletion") ||
                                name.StartsWith("ntdll.dll!ntremoveiocompletion")
                                )
                            {
                                found = true;

                                IntPtr handle = stackFrame.Params[0];

                                sb.AppendLine("Thread " + tid.ToString() + " is waiting for an I/O completion object:");

                                sb.AppendLine(this.GetHandleString(_pid, handle));
                            }
                            else if (
                                name.StartsWith("ntdll.dll!zwwaitforworkviaworkerfactory") ||
                                name.StartsWith("ntdll.dll!ntwaitforworkviaworkerfactory")
                                )
                            {
                                found = true;

                                IntPtr handle = stackFrame.Params[0];

                                sb.AppendLine("Thread " + tid.ToString() + " is waiting for work from a worker factory:");

                                sb.AppendLine(this.GetHandleString(_pid, handle));
                            }
                            else if (
                                name.StartsWith("ntdll.dll!zwreadfile") ||
                                name.StartsWith("ntdll.dll!ntreadfile")
                                )
                            {
                                found = true;

                                IntPtr handle = stackFrame.Params[0];

                                sb.AppendLine("Thread " + tid.ToString() + " is waiting for a named pipe or a file:");

                                sb.AppendLine(this.GetHandleString(_pid, handle));
                            }
                            else if (
                                name.StartsWith("ntdll.dll!zwdeviceiocontrolfile") ||
                                name.StartsWith("ntdll.dll!ntdeviceiocontrolfile")
                                )
                            {
                                found = true;

                                IntPtr handle = stackFrame.Params[0];

                                sb.AppendLine("Thread " + tid.ToString() + " is waiting for an I/O control request:");

                                sb.AppendLine(this.GetHandleString(_pid, handle));
                            }
                            else if (
                                name.StartsWith("ntdll.dll!zwreplywaitreceiveport") ||
                                name.StartsWith("ntdll.dll!ntreplywaitreceiveport") ||
                                name.StartsWith("ntdll.dll!zwrequestwaitreplyport") ||
                                name.StartsWith("ntdll.dll!ntrequestwaitreplyport") ||
                                name.StartsWith("ntdll.dll!zwalpcsendwaitreceiveport") ||
                                name.StartsWith("ntdll.dll!ntalpcsendwaitreceiveport")
                                )
                            {
                                found = true;

                                IntPtr handle = stackFrame.Params[0];

                                sb.AppendLine("Thread " + tid.ToString() + " is waiting for a LPC port:");

                                sb.AppendLine(this.GetHandleString(_pid, handle));
                            }
                            else if (
                                name.StartsWith("user32.dll!ntusergetmessage") ||
                                name.StartsWith("user32.dll!ntuserwaitmessage")
                                )
                            {
                                found = true;

                                sb.AppendLine("Thread " + tid.ToString() + " is waiting for a USER message.");
                            }
                            else if (
                                name.StartsWith("ntdll.dll!zwdelayexecution") ||
                                name.StartsWith("ntdll.dll!ntdelayexecution")
                                )
                            {
                                found = true;

                                bool alertable = stackFrame.Params[0].ToInt32() != 0;
                                IntPtr timeoutAddress = stackFrame.Params[1];
                                long timeout;

                                phandle.ReadMemory(timeoutAddress, &timeout, sizeof(long));

                                if (timeout < 0)
                                {
                                    sb.Append("Thread is sleeping. Timeout: " +
                                        (new TimeSpan(-timeout)).TotalMilliseconds.ToString() + " milliseconds");
                                }
                                else
                                {
                                    sb.AppendLine("Thread is sleeping. Timeout: " + (new DateTime(timeout)).ToString());
                                }
                            }
                            else if (
                                name.StartsWith("kernel32.dll!sleep")
                                )
                            {
                                found = true;

                                sb.Append("Thread is sleeping. Timeout: " +
                                    stackFrame.Params[0].ToInt32().ToString() + " milliseconds");
                            }

                            return !found;
                        });
                }

                if (found)
                {
                    ScratchpadWindow.Create(sb.ToString());
                }
                else
                {
                    MessageBox.Show("The thread does not appear to be waiting.", "Process Hacker",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

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
