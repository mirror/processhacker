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
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using ProcessHacker.Common;
using ProcessHacker.Common.Ui;
using ProcessHacker.Native;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;
using ProcessHacker.Native.Security.AccessControl;
using ProcessHacker.UI;
using ProcessHacker.UI.Actions;

namespace ProcessHacker.Components
{
    public partial class ThreadList : UserControl
    {
        private ThreadProvider _provider;
        private bool _useCycleTime;
        private int _runCount;
        private readonly List<ListViewItem> _needsAdd = new List<ListViewItem>();
        private readonly HighlightingContext _highlightingContext;
        private bool _needsSort;
        public new event KeyEventHandler KeyDown;
        public new event MouseEventHandler MouseDown;
        public new event MouseEventHandler MouseUp;
        public event EventHandler SelectedIndexChanged;
        public event MethodInvoker ThreadItemsAdded;
        private int _pid;

        public ThreadList()
        {
            InitializeComponent();

            // Hide the I/O Priority menu item on XP and below.
            ioPriorityThreadMenuItem.Visible = OSVersion.HasIoPriority;

            // On x64, the first four arguments are passed in registers, 
            // which means Analyze won't work properly.
            if (OSVersion.Architecture != OSArch.I386)
            {
                analyzeWaitMenuItem.Visible = false;
                analyzeMenuItem.Visible = false;
            }

            _highlightingContext = new HighlightingContext(listThreads);
            var comparer = (SortedListViewComparer)
                (listThreads.ListViewItemSorter = new SortedListViewComparer(listThreads));

            comparer.CustomSorters.Add(1, (x, y) =>
            {
                if (OSVersion.HasCycleTime)
                {
                    return ((ThreadItem)x.Tag).CyclesDelta.CompareTo((y.Tag as ThreadItem).CyclesDelta);
                }

                return ((ThreadItem)x.Tag).ContextSwitchesDelta.CompareTo((y.Tag as ThreadItem).ContextSwitchesDelta);
            });
            comparer.CustomSorters.Add(3, (x, y) => ((ThreadItem)x.Tag).PriorityI.CompareTo((y.Tag as ThreadItem).PriorityI));
            comparer.ColumnSortOrder.Add(0);
            comparer.ColumnSortOrder.Add(2);
            comparer.ColumnSortOrder.Add(3);
            comparer.ColumnSortOrder.Add(1);
            comparer.SortColumn = 1;
            comparer.SortOrder = SortOrder.Descending;

            listThreads.KeyDown += this.ThreadList_KeyDown;
            listThreads.MouseDown += this.listThreads_MouseDown;
            listThreads.MouseUp += this.listThreads_MouseUp;
            listThreads.SelectedIndexChanged += this.listThreads_SelectedIndexChanged;

            ColumnSettings.LoadSettings(Settings.Instance.ThreadListViewColumns, listThreads);
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
                    var thread = Windows.GetProcessThreads(_pid)[tid];
                    ProcessThread processThread = null;

                    try
                    {
                        processThread = Utils.GetThreadFromId(Process.GetProcessById(_pid), tid);
                    }
                    catch
                    { }

                    fileModule.Text = _provider.Dictionary[tid].FileName;
                    fileModule.Enabled = !string.IsNullOrEmpty(fileModule.Text);

                    if (processThread != null)
                    {
                        try
                        {
                            if (processThread.ThreadState == ThreadState.Wait)
                            {
                                labelState.Text = "Wait: " + thread.WaitReason.ToString();
                            }
                            else
                            {
                                labelState.Text = processThread.ThreadState.ToString();
                            }

                            labelKernelTime.Text = Utils.FormatTimeSpan(processThread.PrivilegedProcessorTime);
                            labelUserTime.Text = Utils.FormatTimeSpan(processThread.UserProcessorTime);
                            labelTotalTime.Text = Utils.FormatTimeSpan(processThread.TotalProcessorTime);
                        }
                        catch
                        {
                            labelState.Text = thread.WaitReason.ToString();
                        }
                    }

                    labelPriority.Text = thread.Priority.ToString();
                    labelBasePriority.Text = thread.BasePriority.ToString();
                    labelContextSwitches.Text = thread.ContextSwitchCount.ToString("N0");

                    using (ThreadHandle thandle = new ThreadHandle(tid, ThreadAccess.QueryInformation))
                        labelTEBAddress.Text = Utils.FormatAddress(thandle.GetBasicInformation().TebBaseAddress);
                }
                catch
                { }
            }
            else
            {
                fileModule.Text = string.Empty;
                fileModule.Enabled = false;
                labelState.Text = string.Empty;
                labelKernelTime.Text = string.Empty;
                labelUserTime.Text = string.Empty;
                labelTotalTime.Text = string.Empty;
                labelTEBAddress.Text = string.Empty;
                labelPriority.Text = string.Empty;
                labelBasePriority.Text = string.Empty;
                labelContextSwitches.Text = string.Empty;
            }

            if (this.SelectedIndexChanged != null)
                this.SelectedIndexChanged(sender, e);
        }

        private void ThreadList_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.KeyDown != null)
                this.KeyDown(sender, e);

            if (!e.Handled)
            {
                switch (e.KeyCode)
                {
                    case Keys.Enter:
                        this.inspectThreadMenuItem_Click(null, null);
                        break;
                    case Keys.Delete:
                        this.terminateThreadMenuItem_Click(null, null);
                        break;
                }
            }
        }

        #region Properties

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

        public ExtendedListView List
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
                    _provider.DictionaryAdded -= this.provider_DictionaryAdded;
                    _provider.DictionaryModified -= this.provider_DictionaryModified;
                    _provider.DictionaryRemoved -= this.provider_DictionaryRemoved;
                    _provider.Updated -= this.provider_Updated;
                    _provider.LoadingStateChanged -= this.provider_LoadingStateChanged;
                }

                _provider = value;

                listThreads.Items.Clear();

                if (_provider != null)
                {
                    _pid = _provider.Pid;

                    foreach (ThreadItem item in _provider.Dictionary.Values)
                    {
                        provider_DictionaryAdded(item);
                    }

                    if (_pid != 0)
                    {
                        // Use Cycles instead of Context Switches on Vista.
                        if (OSVersion.HasCycleTime)
                            _useCycleTime = true;
                    }
                    else
                    {
                        _useCycleTime = false;
                    }

                    if (_useCycleTime)
                        listThreads.Columns[1].Text = "Cycles Delta";
                    else
                        listThreads.Columns[1].Text = "Context Switches Delta";

                    _provider.DictionaryAdded += this.provider_DictionaryAdded;
                    _provider.DictionaryModified += this.provider_DictionaryModified;
                    _provider.DictionaryRemoved += this.provider_DictionaryRemoved;
                    _provider.Updated += this.provider_Updated;
                    _provider.LoadingStateChanged += this.provider_LoadingStateChanged;

                    this.EnableDisableMenuItems();
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
            lock (_needsAdd)
            {
                if (_needsAdd.Count > 0)
                {
                    this.BeginInvoke(new MethodInvoker(() =>
                    {
                        lock (_needsAdd)
                        {
                            listThreads.Items.AddRange(_needsAdd.ToArray());
                            _needsAdd.Clear();
                        }

                        if (this.ThreadItemsAdded != null)
                            this.ThreadItemsAdded();
                    }));
                }
            }

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

        private void EnableDisableMenuItems()
        {
            if (
                // If KProcessHacker isn't available, hide Force Terminate.
                KProcessHacker.Instance != null &&
                // Terminating a system thread is the same as Force Terminate, 
                // so hide it if we're viewing PID 4.
                _pid != 4
                )
                forceTerminateThreadMenuItem.Visible = true;
            else
                forceTerminateThreadMenuItem.Visible = false;
        }

        private System.Drawing.Color GetThreadColor(ThreadItem titem)
        {
            if (Settings.Instance.UseColorSuspended && titem.WaitReason == KWaitReason.Suspended)
                return Settings.Instance.ColorSuspended;
            
            if (Settings.Instance.UseColorGuiThreads && titem.IsGuiThread)
                return Settings.Instance.ColorGuiThreads;

            return System.Drawing.SystemColors.Window;
        }

        private void provider_DictionaryAdded(ThreadItem item)
        {
            HighlightedListViewItem litem = new HighlightedListViewItem(_highlightingContext, item.RunId > 0 && _runCount > 0)
            {
                Name = item.Tid.ToString(), 
                Text = item.Tid.ToString()
            };

            litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem, string.Empty));
            litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem, item.StartAddress));
            litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem, item.Priority));
            litem.Tag = item;
            litem.NormalColor = GetThreadColor(item);

            lock (_needsAdd)
                _needsAdd.Add(litem);
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

                    if (!_useCycleTime)
                    {
                        if (newItem.ContextSwitchesDelta == 0)
                            litem.SubItems[1].Text = string.Empty;
                        else
                            litem.SubItems[1].Text = newItem.ContextSwitchesDelta.ToString("N0");
                    }
                    else
                    {
                        if (newItem.CyclesDelta == 0)
                            litem.SubItems[1].Text = string.Empty;
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
                    if (listThreads.Items.ContainsKey(item.Tid.ToString()))
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
            Settings.Instance.ThreadListViewColumns = ColumnSettings.SaveSettings(listThreads);
        }

        private void SetThreadPriority(ThreadPriorityLevel priority)
        {
            try
            {
                int tid = int.Parse(listThreads.SelectedItems[0].SubItems[0].Text);

                using (ThreadHandle thandle = new ThreadHandle(tid, OSVersion.MinThreadSetInfoAccess))
                    thandle.SetBasePriorityWin32(priority);
            }
            catch (Exception ex)
            {
                PhUtils.ShowException("Unable to set the priority of the thread", ex);
            }
        }

        private void SetThreadIoPriority(int ioPriority)
        {
            try
            {
                int tid = int.Parse(listThreads.SelectedItems[0].SubItems[0].Text);

                using (ThreadHandle thandle = new ThreadHandle(tid, OSVersion.MinThreadSetInfoAccess))
                    thandle.SetIoPriority(ioPriority);
            }
            catch (Exception ex)
            {
                PhUtils.ShowException("Unable to set the I/O priority of the thread", ex);
            }
        }

        private void listThreads_DoubleClick(object sender, EventArgs e)
        {
            inspectThreadMenuItem_Click(null, null);
        }

        private void menuThread_Popup(object sender, EventArgs e)
        {
            switch (this.listThreads.SelectedItems.Count)
            {
                case 0:
                    return;
                case 1:
                    this.menuThread.EnableAll();
                    this.terminateThreadMenuItem.Text = "&Terminate Thread";
                    this.forceTerminateThreadMenuItem.Text = "Force Terminate Thread";
                    this.suspendThreadMenuItem.Text = "&Suspend Thread";
                    this.resumeThreadMenuItem.Text = "&Resume Thread";
                    this.priorityThreadMenuItem.Text = "&Priority";
                    this.timeCriticalThreadMenuItem.Checked = false;
                    this.highestThreadMenuItem.Checked = false;
                    this.aboveNormalThreadMenuItem.Checked = false;
                    this.normalThreadMenuItem.Checked = false;
                    this.belowNormalThreadMenuItem.Checked = false;
                    this.lowestThreadMenuItem.Checked = false;
                    this.idleThreadMenuItem.Checked = false;
                    this.ioPriority0ThreadMenuItem.Checked = false;
                    this.ioPriority1ThreadMenuItem.Checked = false;
                    this.ioPriority2ThreadMenuItem.Checked = false;
                    this.ioPriority3ThreadMenuItem.Checked = false;
                    try
                    {
                        using (var thandle = new ThreadHandle(int.Parse(this.listThreads.SelectedItems[0].SubItems[0].Text), Program.MinThreadQueryRights))
                        {
                            try
                            {
                                switch (thandle.GetBasePriorityWin32())
                                {
                                    case ThreadPriorityLevel.TimeCritical:
                                        this.timeCriticalThreadMenuItem.Checked = true;
                                        break;
                                    case ThreadPriorityLevel.Highest:
                                        this.highestThreadMenuItem.Checked = true;
                                        break;
                                    case ThreadPriorityLevel.AboveNormal:
                                        this.aboveNormalThreadMenuItem.Checked = true;
                                        break;
                                    case ThreadPriorityLevel.Normal:
                                        this.normalThreadMenuItem.Checked = true;
                                        break;
                                    case ThreadPriorityLevel.BelowNormal:
                                        this.belowNormalThreadMenuItem.Checked = true;
                                        break;
                                    case ThreadPriorityLevel.Lowest:
                                        this.lowestThreadMenuItem.Checked = true;
                                        break;
                                    case ThreadPriorityLevel.Idle:
                                        this.idleThreadMenuItem.Checked = true;
                                        break;
                                }
                            }
                            catch
                            {
                                this.priorityThreadMenuItem.Enabled = false;
                            }

                            try
                            {
                                if (OSVersion.HasIoPriority)
                                {
                                    switch (thandle.GetIoPriority())
                                    {
                                        case 0:
                                            this.ioPriority0ThreadMenuItem.Checked = true;
                                            break;
                                        case 1:
                                            this.ioPriority1ThreadMenuItem.Checked = true;
                                            break;
                                        case 2:
                                            this.ioPriority2ThreadMenuItem.Checked = true;
                                            break;
                                        case 3:
                                            this.ioPriority3ThreadMenuItem.Checked = true;
                                            break;
                                    }
                                }
                            }
                            catch
                            {
                                this.ioPriorityThreadMenuItem.Enabled = false;
                            }
                        }
                    }
                    catch
                    {
                        this.priorityThreadMenuItem.Enabled = false;
                        this.ioPriorityThreadMenuItem.Enabled = false;
                    }
                    try
                    {
                        using (ThreadHandle thandle = new ThreadHandle(int.Parse(this.listThreads.SelectedItems[0].Text), Program.MinThreadQueryRights))
                        using (TokenHandle tokenHandle = thandle.GetToken(TokenAccess.Query))
                        {
                            this.tokenThreadMenuItem.Enabled = true;
                        }
                    }
                    catch (WindowsException)
                    {
                        this.tokenThreadMenuItem.Enabled = false;
                    }
                    break;
                default:
                    this.terminateThreadMenuItem.Enabled = true;
                    this.forceTerminateThreadMenuItem.Enabled = true;
                    this.suspendThreadMenuItem.Enabled = true;
                    this.resumeThreadMenuItem.Enabled = true;
                    this.terminateThreadMenuItem.Text = "&Terminate Threads";
                    this.forceTerminateThreadMenuItem.Text = "Force Terminate Threads";
                    this.suspendThreadMenuItem.Text = "&Suspend Threads";
                    this.resumeThreadMenuItem.Text = "&Resume Threads";
                    this.copyThreadMenuItem.Enabled = true;
                    break;
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

            // Can't view system thread stacks if KPH isn't present.
            if (
                _pid == 4 && 
                KProcessHacker.Instance == null
                )
            {
                PhUtils.ShowError(
                    "Process Hacker cannot view system thread stacks without KProcessHacker. " + 
                    "Make sure Process Hacker has administrative privileges and KProcessHacker " + 
                    "supports your operating system."
                    );

                return;
            }

            // Suspending PH threads is not a good idea :(
            if (_pid == ProcessHandle.CurrentId)
            {
                if (!PhUtils.ShowConfirmMessage(
                    "inspect",
                    "Process Hacker's threads",
                    "Inspecting Process Hacker's threads may lead to instability.",
                    true
                    ))
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

                // If we have KPH load kernel modules so we can get the kernel-mode stack.
                try
                {
                    _provider.LoadKernelSymbols();
                }
                catch
                { }

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

            // Special case for system threads.
            if (
                KProcessHacker.Instance != null &&
                _pid == 4
                )
            {
                if (!PhUtils.ShowConfirmMessage(
                    "terminate",
                    "the selected system thread(s)",
                    "Forcibly terminating system threads may cause the system to crash.",
                    true
                    ))
                    return;

                foreach (ListViewItem item in listThreads.SelectedItems)
                {
                    int tid = Int32.Parse(item.SubItems[0].Text);

                    try
                    {
                        using (var thandle = new ThreadHandle(tid, ThreadAccess.Terminate))
                            thandle.DangerousTerminate(NtStatus.Success);
                    }
                    catch (Exception ex)
                    {
                        PhUtils.ShowException("Unable to terminate the thread " + tid.ToString(), ex);
                    }
                }

                return;
            }

            if (!PhUtils.ShowConfirmMessage(
                "terminate",
                "the selected thread(s)",
                "Terminating a thread may cause the process to stop working.",
                false
                ))
                return;

            if (Program.ElevationType == TokenElevationType.Limited && 
                KProcessHacker.Instance == null &&
                Settings.Instance.ElevationLevel != (int)ElevationLevel.Never)
            {
                try
                {
                    foreach (ListViewItem item in listThreads.SelectedItems)
                    {
                        using (var thandle = new ThreadHandle(int.Parse(item.SubItems[0].Text), ThreadAccess.Terminate))
                        { }
                    }
                }
                catch
                {
                    string objects = string.Empty;

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
                    if (!PhUtils.ShowContinueMessage(
                        "Unable to terminate the thread with ID " + item.SubItems[0].Text,
                        ex
                        ))
                        return;
                }
            }
        }

        private void forceTerminateThreadMenuItem_Click(object sender, EventArgs e)
        {
            if (!PhUtils.ShowConfirmMessage(
                "force terminate",
                "the selected thread(s)",
                "Forcibly terminating threads may cause the system to crash.",
                true
                ))
                return;

            foreach (ListViewItem item in listThreads.SelectedItems)
            {
                int tid = Int32.Parse(item.SubItems[0].Text);

                try
                {
                    using (var thandle = new ThreadHandle(tid, ThreadAccess.Terminate))
                        thandle.DangerousTerminate(NtStatus.Success);
                }
                catch (Exception ex)
                {
                    if (!PhUtils.ShowContinueMessage(
                        "Unable to force terminate the thread with ID " + item.SubItems[0].Text,
                        ex
                        ))
                        return;
                }
            }
        }

        private void suspendThreadMenuItem_Click(object sender, EventArgs e)
        {
            //if (Properties.Settings.Default.WarnDangerous && PhUtils.IsDangerousPid(_pid))
            //{
            //    DialogResult result = MessageBox.Show("The process with PID " + _pid + " is a system process. Are you" +
            //        " sure you want to suspend the selected thread(s)?", "Process Hacker", MessageBoxButtons.YesNo,
            //        MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);

            //    if (result == DialogResult.No)
            //        return;
            //}

            if (Program.ElevationType == TokenElevationType.Limited &&
                KProcessHacker.Instance == null &&
                Settings.Instance.ElevationLevel != (int)ElevationLevel.Never)
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
                    string objects = string.Empty;

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
                    using (var thandle = new ThreadHandle(Int32.Parse(item.SubItems[0].Text), ThreadAccess.SuspendResume))
                        thandle.Suspend();
                }
                catch (Exception ex)
                {
                    if (!PhUtils.ShowContinueMessage(
                        "Unable to suspend the thread with ID " + item.SubItems[0].Text,
                        ex
                        ))
                        return;
                }
            }
        }

        private void resumeThreadMenuItem_Click(object sender, EventArgs e)
        {
            //if (Properties.Settings.Default.WarnDangerous && PhUtils.IsDangerousPid(_pid))
            //{
            //    DialogResult result = MessageBox.Show("The process with PID " + _pid + " is a system process. Are you" +
            //        " sure you want to resume the selected thread(s)?", "Process Hacker", MessageBoxButtons.YesNo,
            //        MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);

            //    if (result == DialogResult.No)
            //        return;
            //}

            if (Program.ElevationType == TokenElevationType.Limited &&
                KProcessHacker.Instance == null &&
                Settings.Instance.ElevationLevel != (int)ElevationLevel.Never)
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
                    string objects = string.Empty;

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
                    using (ThreadHandle thandle = new ThreadHandle(Int32.Parse(item.SubItems[0].Text), ThreadAccess.SuspendResume))
                        thandle.Resume();
                }
                catch (Exception ex)
                {
                    if (!PhUtils.ShowContinueMessage(
                        "Unable to resume the thread with ID " + item.SubItems[0].Text,
                        ex
                        ))
                        return;
                }
            }
        }

        private void inspectTEBMenuItem_Click(object sender, EventArgs e)
        {
            if (!Program.Structs.ContainsKey("TEB"))
            {
                PhUtils.ShowError("The struct 'TEB' has not been loaded. Make sure structs.txt was loaded successfully.");
                return;
            }

            try
            {
                using (ThreadHandle thandle = new ThreadHandle(int.Parse(listThreads.SelectedItems[0].Text)))
                {
                    IntPtr tebBaseAddress = thandle.GetBasicInformation().TebBaseAddress;

                    Program.HackerWindow.BeginInvoke(new MethodInvoker(() =>
                    {
                        StructWindow sw = new StructWindow(this._pid, tebBaseAddress, Program.Structs["TEB"]);
                        sw.Show();
                        sw.Activate();
                    }));
                }
            }
            catch (Exception ex)
            {
                PhUtils.ShowException("Unable to inspect the TEB of the thread", ex);
            }
        }

        private void permissionsThreadMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                SecurityEditor.EditSecurity(
                    this,
                    SecurityEditor.GetSecurableWrapper(
                        access => new ThreadHandle(int.Parse(listThreads.SelectedItems[0].Text), (ThreadAccess)access)
                        ),
                    "Thread " + listThreads.SelectedItems[0].Text,
                    NativeTypeFactory.GetAccessEntries(NativeTypeFactory.ObjectType.Thread)
                    );
            }
            catch (Exception ex)
            {
                PhUtils.ShowException("Unable to edit security", ex);
            }
        }

        private void tokenThreadMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                using (ThreadHandle thandle = new ThreadHandle(int.Parse(listThreads.SelectedItems[0].Text), Program.MinThreadQueryRights))
                using (TokenWindow tokForm = new TokenWindow(thandle))
                {
                    tokForm.Text = "Thread Token";
                    tokForm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                PhUtils.ShowException("Unable to view the thread token", ex);
            }
        }

        private SystemHandleEntry GetShiForHandle(int pid, IntPtr handle)
        {
            short handleValue = (short)handle.ToInt32();
            var handles = Windows.GetHandles();

            foreach (var handleInfo in handles)
            {
                if (handleInfo.ProcessId == pid && handleInfo.Handle == handleValue)
                    return handleInfo;
            }

            return new SystemHandleEntry();
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
            try
            {
                StringBuilder sb = new StringBuilder();
                int tid = int.Parse(listThreads.SelectedItems[0].SubItems[0].Text);
                ProcessHandle phandle;

                if ((_provider.ProcessAccess & (ProcessAccess.QueryInformation | ProcessAccess.VmRead)) != 0)
                    phandle = _provider.ProcessHandle;
                else
                    phandle = new ProcessHandle(_pid, ProcessAccess.QueryInformation | ProcessAccess.VmRead);

                //ProcessHandle processDupHandle = new ProcessHandle(_pid, ProcessAccess.DupHandle);

                bool found = false;

                using (ThreadHandle thandle = new ThreadHandle(tid, ThreadAccess.GetContext | ThreadAccess.SuspendResume))
                {
                    IntPtr[] lastParams = new IntPtr[4];

                    thandle.WalkStack(phandle, stackFrame =>
                    {
                        uint address = stackFrame.PcAddress.ToUInt32();
                        string name = _provider.Symbols.GetSymbolFromAddress(address).ToLowerInvariant();

                        if (string.IsNullOrEmpty(name))
                        {
                            // dummy
                        }
                        else if (
                            name.StartsWith("kernel32.dll!sleep", StringComparison.OrdinalIgnoreCase)
                            )
                        {
                            found = true;

                            sb.Append("Thread is sleeping. Timeout: " +
                                      stackFrame.Params[0].ToInt32().ToString() + " milliseconds");
                        }
                        else if (
                            name.StartsWith("ntdll.dll!zwdelayexecution", StringComparison.OrdinalIgnoreCase) ||
                            name.StartsWith("ntdll.dll!ntdelayexecution", StringComparison.OrdinalIgnoreCase)
                            )
                        {
                            found = true;

                            //bool alertable = stackFrame.Params[0].ToInt32() != 0;
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
                            name.StartsWith("ntdll.dll!zwdeviceiocontrolfile", StringComparison.OrdinalIgnoreCase) ||
                            name.StartsWith("ntdll.dll!ntdeviceiocontrolfile", StringComparison.OrdinalIgnoreCase)
                            )
                        {
                            found = true;

                            IntPtr handle = stackFrame.Params[0];

                            sb.AppendLine("Thread " + tid.ToString() + " is waiting for an I/O control request:");

                            sb.AppendLine(this.GetHandleString(_pid, handle));
                        }
                        else if (
                            name.StartsWith("ntdll.dll!ntfscontrolfile", StringComparison.OrdinalIgnoreCase) ||
                            name.StartsWith("ntdll.dll!zwfscontrolfile", StringComparison.OrdinalIgnoreCase)
                            )
                        {
                            found = true;

                            IntPtr handle = stackFrame.Params[0];

                            sb.AppendLine("Thread " + tid.ToString() + " is waiting for an FS control request:");

                            sb.AppendLine(this.GetHandleString(_pid, handle));
                        }
                        else if (
                            name.StartsWith("ntdll.dll!ntqueryobject", StringComparison.OrdinalIgnoreCase) ||
                            name.StartsWith("ntdll.dll!zwqueryobject", StringComparison.OrdinalIgnoreCase)
                            )
                        {
                            found = true;

                            IntPtr handle = stackFrame.Params[0];

                            // Use the KiFastSystemCallRet args if the handle we have is wrong.
                            if (handle.ToInt32() % 2 != 0 || handle == IntPtr.Zero)
                                handle = lastParams[1];

                            sb.AppendLine("Thread " + tid.ToString() + " is querying an object (most likely a named pipe):");

                            sb.AppendLine(this.GetHandleString(_pid, handle));
                        }
                        else if (
                            name.StartsWith("ntdll.dll!zwreadfile", StringComparison.OrdinalIgnoreCase) ||
                            name.StartsWith("ntdll.dll!ntreadfile", StringComparison.OrdinalIgnoreCase) ||
                            name.StartsWith("ntdll.dll!zwwritefile", StringComparison.OrdinalIgnoreCase) ||
                            name.StartsWith("ntdll.dll!ntwritefile", StringComparison.OrdinalIgnoreCase)
                            )
                        {
                            found = true;

                            IntPtr handle = stackFrame.Params[0];

                            sb.AppendLine("Thread " + tid.ToString() + " is waiting for a named pipe or a file:");

                            sb.AppendLine(this.GetHandleString(_pid, handle));
                        }
                        else if (
                            name.StartsWith("ntdll.dll!zwremoveiocompletion", StringComparison.OrdinalIgnoreCase) ||
                            name.StartsWith("ntdll.dll!ntremoveiocompletion", StringComparison.OrdinalIgnoreCase)
                            )
                        {
                            found = true;

                            IntPtr handle = stackFrame.Params[0];

                            sb.AppendLine("Thread " + tid.ToString() + " is waiting for an I/O completion object:");

                            sb.AppendLine(this.GetHandleString(_pid, handle));
                        }
                        else if (
                            name.StartsWith("ntdll.dll!zwreplywaitreceiveport", StringComparison.OrdinalIgnoreCase) ||
                            name.StartsWith("ntdll.dll!ntreplywaitreceiveport", StringComparison.OrdinalIgnoreCase) ||
                            name.StartsWith("ntdll.dll!zwrequestwaitreplyport", StringComparison.OrdinalIgnoreCase) ||
                            name.StartsWith("ntdll.dll!ntrequestwaitreplyport", StringComparison.OrdinalIgnoreCase) ||
                            name.StartsWith("ntdll.dll!zwalpcsendwaitreceiveport", StringComparison.OrdinalIgnoreCase) ||
                            name.StartsWith("ntdll.dll!ntalpcsendwaitreceiveport", StringComparison.OrdinalIgnoreCase)
                            )
                        {
                            found = true;

                            IntPtr handle = stackFrame.Params[0];

                            sb.AppendLine("Thread " + tid.ToString() + " is waiting for a LPC port:");

                            sb.AppendLine(this.GetHandleString(_pid, handle));
                        }
                        else if
                            (
                            name.StartsWith("ntdll.dll!zwsethighwaitloweventpair", StringComparison.OrdinalIgnoreCase) ||
                            name.StartsWith("ntdll.dll!ntsethighwaitloweventpair", StringComparison.OrdinalIgnoreCase) ||
                            name.StartsWith("ntdll.dll!zwsetlowwaithigheventpair", StringComparison.OrdinalIgnoreCase) ||
                            name.StartsWith("ntdll.dll!ntsetlowwaithigheventpair", StringComparison.OrdinalIgnoreCase) ||
                            name.StartsWith("ntdll.dll!zwwaithigheventpair", StringComparison.OrdinalIgnoreCase) ||
                            name.StartsWith("ntdll.dll!ntwaithigheventpair", StringComparison.OrdinalIgnoreCase) ||
                            name.StartsWith("ntdll.dll!zwwaitloweventpair", StringComparison.OrdinalIgnoreCase) ||
                            name.StartsWith("ntdll.dll!ntwaitloweventpair", StringComparison.OrdinalIgnoreCase)
                            )
                        {
                            found = true;

                            IntPtr handle = stackFrame.Params[0];

                            // Use the KiFastSystemCallRet args if the handle we have is wrong.
                            if (handle.ToInt32() % 2 != 0)
                                handle = lastParams[1];

                            sb.AppendLine("Thread " + tid.ToString() + " is waiting (" + name + ") for an event pair:");

                            sb.AppendLine(this.GetHandleString(_pid, handle));
                        }
                        else if (
                            name.StartsWith("user32.dll!ntusergetmessage", StringComparison.OrdinalIgnoreCase) ||
                            name.StartsWith("user32.dll!ntuserwaitmessage", StringComparison.OrdinalIgnoreCase)
                            )
                        {
                            found = true;

                            sb.AppendLine("Thread " + tid.ToString() + " is waiting for a USER message.");
                        }
                        else if (
                            name.StartsWith("ntdll.dll!zwwaitfordebugevent", StringComparison.OrdinalIgnoreCase) ||
                            name.StartsWith("ntdll.dll!ntwaitfordebugevent", StringComparison.OrdinalIgnoreCase)
                            )
                        {
                            found = true;

                            IntPtr handle = stackFrame.Params[0];

                            sb.AppendLine("Thread " + tid.ToString() + " is waiting for a debug event:");

                            sb.AppendLine(this.GetHandleString(_pid, handle));
                        }
                        else if (
                            name.StartsWith("ntdll.dll!zwwaitforkeyedevent", StringComparison.OrdinalIgnoreCase) ||
                            name.StartsWith("ntdll.dll!ntwaitforkeyedevent", StringComparison.OrdinalIgnoreCase) ||
                            name.StartsWith("ntdll.dll!zwreleasekeyedevent", StringComparison.OrdinalIgnoreCase) ||
                            name.StartsWith("ntdll.dll!ntreleasekeyedevent", StringComparison.OrdinalIgnoreCase)
                            )
                        {
                            found = true;

                            IntPtr handle = stackFrame.Params[0];
                            IntPtr key = stackFrame.Params[1];

                            sb.AppendLine("Thread " + tid.ToString() +
                                          " is waiting (" + name + ") for a keyed event (key 0x" +
                                          key.ToString("x") + "):");

                            sb.AppendLine(this.GetHandleString(_pid, handle));
                        }
                        else if (
                            name.StartsWith("ntdll.dll!zwwaitformultipleobjects", StringComparison.OrdinalIgnoreCase) ||
                            name.StartsWith("ntdll.dll!ntwaitformultipleobjects", StringComparison.OrdinalIgnoreCase) ||
                            name.StartsWith("kernel32.dll!waitformultipleobjects", StringComparison.OrdinalIgnoreCase)
                            )
                        {
                            found = true;

                            int handleCount = stackFrame.Params[0].ToInt32();
                            IntPtr handleAddress = stackFrame.Params[1];
                            WaitType waitType = (WaitType)stackFrame.Params[2].ToInt32();
                            bool alertable = stackFrame.Params[3].ToInt32() != 0;

                            // use the KiFastSystemCallRet args if we have the wrong args
                            if (handleCount > 64)
                            {
                                handleCount = lastParams[1].ToInt32();
                                handleAddress = lastParams[2];
                                waitType = (WaitType)lastParams[3].ToInt32();
                            }

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
                            name.StartsWith("ntdll.dll!zwwaitforsingleobject", StringComparison.OrdinalIgnoreCase) ||
                            name.StartsWith("ntdll.dll!ntwaitforsingleobject", StringComparison.OrdinalIgnoreCase) ||
                            name.StartsWith("kernel32.dll!waitforsingleobject", StringComparison.OrdinalIgnoreCase)
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
                            name.StartsWith("ntdll.dll!zwwaitforworkviaworkerfactory", StringComparison.OrdinalIgnoreCase) ||
                            name.StartsWith("ntdll.dll!ntwaitforworkviaworkerfactory", StringComparison.OrdinalIgnoreCase)
                            )
                        {
                            found = true;

                            IntPtr handle = stackFrame.Params[0];

                            sb.AppendLine("Thread " + tid.ToString() + " is waiting for work from a worker factory:");

                            sb.AppendLine(this.GetHandleString(_pid, handle));
                        }

                        lastParams = stackFrame.Params;

                        return !found;
                    });
                }

                if (found)
                {
                    new InformationBox(sb.ToString()).ShowDialog();
                }
                else
                {
                    PhUtils.ShowInformation("The thread does not appear to be waiting.");
                }
            }
            catch (Exception ex)
            {
                PhUtils.ShowException("Unable to analyze the thread", ex);
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

        #region I/O Priority

        private void ioPriority0ThreadMenuItem_Click(object sender, EventArgs e)
        {
            SetThreadIoPriority(0);
        }

        private void ioPriority1ThreadMenuItem_Click(object sender, EventArgs e)
        {
            SetThreadIoPriority(1);
        }

        private void ioPriority2ThreadMenuItem_Click(object sender, EventArgs e)
        {
            SetThreadIoPriority(2);
        }

        private void ioPriority3ThreadMenuItem_Click(object sender, EventArgs e)
        {
            SetThreadIoPriority(3);
        }

        #endregion

        private void selectAllThreadMenuItem_Click(object sender, EventArgs e)
        {
            this.listThreads.Items.SelectAll();
        }
    }
}
