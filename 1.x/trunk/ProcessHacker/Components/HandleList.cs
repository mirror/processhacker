/*
 * Process Hacker - 
 *   Handle list
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
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using ProcessHacker.Common;
using ProcessHacker.Common.Ui;
using ProcessHacker.Native;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;
using ProcessHacker.Native.Ui;
using ProcessHacker.UI;

namespace ProcessHacker.Components
{
    public partial class HandleList : UserControl
    {
        public static bool ConfirmHandleClose()
        {
            if (Settings.Instance.WarnDangerous)
            {
                return PhUtils.ShowConfirmMessage(
                    "close",
                    "the selected handle(s)",
                    "Closing handles may cause system instability and data corruption.",
                    false
                    );
            }
            else
            {
                return true;
            }
        }

        public static void ShowHandleProperties(SystemHandleEntry handleInfo)
        {
            try
            {
                HandlePropertiesWindow window = new HandlePropertiesWindow(handleInfo);
                IntPtr handle = new IntPtr(handleInfo.Handle);
                ProcessHandle phandle = new ProcessHandle(handleInfo.ProcessId, ProcessAccess.DupHandle);
                GenericHandle dupHandle = null; 

                window.HandlePropertiesCallback += (control, name, typeName) =>
                    {
                        switch (typeName.ToLowerInvariant())
                        {
                            // Objects with separate property windows:
                            case "file":
                            case "job":
                            case "key":
                            case "token":
                            case "process":
                                {
                                    Button b = new Button();

                                    b.FlatStyle = FlatStyle.System;
                                    b.Text = "Properties";
                                    b.Click += (sender, e) =>
                                        {
                                            try
                                            {
                                                switch (typeName.ToLowerInvariant())
                                                {
                                                    case "file":
                                                        {
                                                            FileUtils.ShowProperties(name);
                                                        }
                                                        break;
                                                    case "job":
                                                        {
                                                            dupHandle =
                                                                new GenericHandle(
                                                                    phandle, handle, 
                                                                    (int)JobObjectAccess.Query);
                                                            (new JobWindow(JobObjectHandle.FromHandle(dupHandle))).ShowDialog();
                                                        }
                                                        break;
                                                    case "key":
                                                        {
                                                            try
                                                            {
                                                                PhUtils.OpenKeyInRegedit(PhUtils.GetForegroundWindow(), name);
                                                            }
                                                            catch (Exception ex)
                                                            {
                                                                PhUtils.ShowException("Unable to open the Registry Editor", ex);
                                                            }
                                                        }
                                                        break;
                                                    case "token":
                                                        {
                                                            (new TokenWindow(new RemoteTokenHandle(phandle, 
                                                                handle))).ShowDialog();
                                                        }
                                                        break;
                                                    case "process":
                                                        {
                                                            int pid;

                                                            if (KProcessHacker.Instance != null)
                                                            {
                                                                pid = KProcessHacker.Instance.KphGetProcessId(phandle, handle);
                                                            }
                                                            else
                                                            {
                                                                dupHandle =
                                                                    new GenericHandle(
                                                                        phandle, handle,
                                                                        (int)OSVersion.MinProcessQueryInfoAccess);
                                                                pid = ProcessHandle.FromHandle(dupHandle).GetProcessId();
                                                            }

                                                            Program.GetProcessWindow(Program.ProcessProvider.Dictionary[pid],
                                                                (f) => Program.FocusWindow(f));
                                                        }
                                                        break;
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                PhUtils.ShowException("Unable to show object properties", ex);
                                            }
                                        };

                                    control.Controls.Add(b);
                                }
                                break;
                            case "event":
                                {
                                    dupHandle = new GenericHandle(phandle, handle, (int)EventAccess.QueryState);
                                    var eventProps = new EventProperties(EventHandle.FromHandle(dupHandle));
                                    control.Controls.Add(eventProps);
                                }
                                break;
                            case "eventpair":
                                {
                                    dupHandle = new GenericHandle(phandle, handle, (int)EventPairAccess.All);
                                    var eventPairProps = new EventPairProperties(EventPairHandle.FromHandle(dupHandle));
                                    control.Controls.Add(eventPairProps);
                                }
                                break;
                            case "mutant":
                                {
                                    dupHandle = new GenericHandle(phandle, handle, (int)MutantAccess.QueryState);
                                    var mutantProps = new MutantProperties(MutantHandle.FromHandle(dupHandle));
                                    control.Controls.Add(mutantProps);
                                }
                                break;
                            case "section":
                                {
                                    dupHandle = new GenericHandle(phandle, handle, (int)SectionAccess.Query);
                                    var sectionProps = new SectionProperties(SectionHandle.FromHandle(dupHandle));
                                    control.Controls.Add(sectionProps);
                                }
                                break;
                            case "semaphore":
                                {
                                    dupHandle = new GenericHandle(phandle, handle, (int)SemaphoreAccess.QueryState);
                                    var semaphoreProps = new SemaphoreProperties(SemaphoreHandle.FromHandle(dupHandle));
                                    control.Controls.Add(semaphoreProps);
                                }
                                break;
                            case "timer":
                                {
                                    dupHandle = new GenericHandle(phandle, handle, (int)TimerAccess.QueryState);
                                    var timerProps = new TimerProperties(TimerHandle.FromHandle(dupHandle));
                                    control.Controls.Add(timerProps);
                                }
                                break;
                            case "tmrm":
                                {
                                    dupHandle = new GenericHandle(phandle, handle, (int)ResourceManagerAccess.QueryInformation);
                                    var tmRmProps = new TmRmProperties(ResourceManagerHandle.FromHandle(dupHandle));
                                    control.Controls.Add(tmRmProps);
                                }
                                break;
                            case "tmtm":
                                {
                                    dupHandle = new GenericHandle(phandle, handle, (int)TmAccess.QueryInformation);
                                    var tmTmProps = new TmTmProperties(TmHandle.FromHandle(dupHandle));
                                    control.Controls.Add(tmTmProps);
                                }
                                break;
                        }
                    };

                if (dupHandle == null)
                {
                    // Try to get a handle, since we need one for security editing.
                    try { dupHandle = new GenericHandle(phandle, handle, 0); }
                    catch { }
                }

                window.ObjectHandle = dupHandle;

                window.ShowDialog();

                if (dupHandle != null)
                    dupHandle.Dispose();
            }
            catch (Exception ex)
            {
                PhUtils.ShowException("Unable to show handle properties", ex);
            }
        }

        private object _listLock = new object();
        private HandleProvider _provider;
        private int _runCount = 0;
        private List<ListViewItem> _needsAdd = new List<ListViewItem>();
        private HighlightingContext _highlightingContext;
        public new event KeyEventHandler KeyDown;
        public new event MouseEventHandler MouseDown;
        public new event MouseEventHandler MouseUp;
        public event EventHandler SelectedIndexChanged;

        public HandleList()
        {
            InitializeComponent();

            _highlightingContext = new HighlightingContext(listHandles);
            listHandles.KeyDown += new KeyEventHandler(listHandles_KeyDown);
            listHandles.MouseDown += new MouseEventHandler(listHandles_MouseDown);
            listHandles.MouseUp += new MouseEventHandler(listHandles_MouseUp);
            listHandles.DoubleClick += new EventHandler(listHandles_DoubleClick);
            listHandles.SelectedIndexChanged += new System.EventHandler(listHandles_SelectedIndexChanged);

            var comparer = (SortedListViewComparer)
                (listHandles.ListViewItemSorter = new SortedListViewComparer(listHandles));

            comparer.ColumnSortOrder.Add(0);
            comparer.ColumnSortOrder.Add(2);
            comparer.ColumnSortOrder.Add(1);

            listHandles.ContextMenu = menuHandle;
            GenericViewMenu.AddMenuItems(copyHandleMenuItem.MenuItems, listHandles, null);
            ColumnSettings.LoadSettings(Settings.Instance.HandleListViewColumns, listHandles);

            if (KProcessHacker.Instance == null)
            {
                protectedMenuItem.Visible = false;
                inheritMenuItem.Visible = false;
            }
        }

        private void listHandles_DoubleClick(object sender, EventArgs e)
        {
            propertiesHandleMenuItem_Click(sender, e);
        }

        private void listHandles_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.MouseUp != null)
                this.MouseUp(sender, e);
        }

        private void listHandles_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.MouseDown != null)
                this.MouseDown(sender, e);
        }

        private void listHandles_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (this.SelectedIndexChanged != null)
                this.SelectedIndexChanged(sender, e);
        }

        private void listHandles_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.KeyDown != null)
                this.KeyDown(sender, e);

            if (!e.Handled)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    propertiesHandleMenuItem_Click(null, null);
                }
                else if (e.KeyCode == Keys.Delete)
                {
                    if (ConfirmHandleClose())
                    {
                        closeHandleMenuItem_Click(null, null);
                    }
                }
            }
        }

        #region Properties

        public new bool DoubleBuffered
        {
            get
            {
                return (bool)typeof(ListView).GetProperty("DoubleBuffered",
                    BindingFlags.NonPublic | BindingFlags.Instance).GetValue(listHandles, null);
            }
            set
            {
                typeof(ListView).GetProperty("DoubleBuffered",
                    BindingFlags.NonPublic | BindingFlags.Instance).SetValue(listHandles, value, null);
            }
        }

        public override bool Focused
        {
            get
            {
                return listHandles.Focused;
            }
        }

        public override ContextMenu ContextMenu
        {
            get { return listHandles.ContextMenu; }
            set { listHandles.ContextMenu = value; }
        }

        public override ContextMenuStrip ContextMenuStrip
        {
            get { return listHandles.ContextMenuStrip; }
            set { listHandles.ContextMenuStrip = value; }
        }

        public ListView List
        {
            get { return listHandles; }
        }

        public HandleProvider Provider
        {
            get { return _provider; }
            set
            {
                if (_provider != null)
                {
                    _provider.DictionaryAdded -= provider_DictionaryAdded;
                    _provider.DictionaryModified -= provider_DictionaryModified;
                    _provider.DictionaryRemoved -= provider_DictionaryRemoved;
                    _provider.Updated -= provider_Updated;
                }

                _provider = value;

                listHandles.Items.Clear();
                _pid = -1;

                if (_provider != null)
                {
                    foreach (HandleItem item in _provider.Dictionary.Values)
                    {
                        provider_DictionaryAdded(item);
                    }

                    _provider.DictionaryAdded += provider_DictionaryAdded;
                    _provider.DictionaryModified += provider_DictionaryModified;
                    _provider.DictionaryRemoved += provider_DictionaryRemoved;
                    _provider.Updated += provider_Updated;
                    _pid = _provider.Pid;
                }
            }
        }

        #endregion

        #region Interfacing

        public void BeginUpdate()
        {
            listHandles.BeginUpdate();
        }

        public void EndUpdate()
        {
            listHandles.EndUpdate();
        }

        public ListView.ListViewItemCollection Items
        {
            get { return listHandles.Items; }
        }

        public ListView.SelectedListViewItemCollection SelectedItems
        {
            get { return listHandles.SelectedItems; }
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
                            listHandles.Items.AddRange(_needsAdd.ToArray());
                            _needsAdd.Clear();
                            _needsAdd.TrimExcess();
                        }
                    }));
                }
            }

            _highlightingContext.Tick();
            _runCount++;
        }

        private Color GetHandleColor(HandleItem item)
        {
            if (Settings.Instance.UseColorProtectedHandles &&
                (item.Handle.Flags & HandleFlags.ProtectFromClose) != 0
                )
                return Settings.Instance.ColorProtectedHandles;
            else if (Settings.Instance.UseColorInheritHandles &&
                (item.Handle.Flags & HandleFlags.Inherit) != 0
                )
                return Settings.Instance.ColorInheritHandles;
            else
                return SystemColors.Window;
        }

        public void AddItem(HandleItem item)
        {
            provider_DictionaryAdded(item);
        }

        public void UpdateItems()
        {
            provider_Updated();
        }

        public void DumpDisableEvents()
        {
            listHandles.DoubleClick -= listHandles_DoubleClick;
            listHandles.KeyDown -= listHandles_KeyDown;
        }

        private void provider_DictionaryAdded(HandleItem item)
        {
            HighlightedListViewItem litem = new HighlightedListViewItem(_highlightingContext,
                item.RunId > 0 && _runCount > 0);

            litem.Name = item.Handle.Handle.ToString();
            litem.Text = item.ObjectInfo.TypeName;
            litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem, item.ObjectInfo.BestName));
            litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem, "0x" + item.Handle.Handle.ToString("x")));
            litem.Tag = item;

            litem.NormalColor = this.GetHandleColor(item);

            lock (_needsAdd)
                _needsAdd.Add(litem);
        }

        private void provider_DictionaryModified(HandleItem oldItem, HandleItem newItem)
        {
            this.BeginInvoke(new MethodInvoker(() =>
                {
                    lock (_listLock)
                    {
                        (listHandles.Items[newItem.Handle.Handle.ToString()] as
                            HighlightedListViewItem).NormalColor = this.GetHandleColor(newItem);
                    }
                }));
        }

        private void provider_DictionaryRemoved(HandleItem item)
        {
            this.BeginInvoke(new MethodInvoker(() =>
                {
                    lock (_listLock)
                        listHandles.Items[item.Handle.Handle.ToString()].Remove();
                }));
        }

        private int _pid;

        public void SaveSettings()
        {
            Settings.Instance.HandleListViewColumns = ColumnSettings.SaveSettings(listHandles);
        }

        private void menuHandle_Popup(object sender, EventArgs e)
        {
            protectedMenuItem.Checked = false;
            inheritMenuItem.Checked = false;

            if (listHandles.SelectedItems.Count == 0)
            {
                menuHandle.DisableAll();
            }
            else if (listHandles.SelectedItems.Count == 1)
            {
                menuHandle.EnableAll();

                HandleItem item = (HandleItem)listHandles.SelectedItems[0].Tag;

                protectedMenuItem.Checked = (item.Handle.Flags & HandleFlags.ProtectFromClose) != 0;
                inheritMenuItem.Checked = (item.Handle.Flags & HandleFlags.Inherit) != 0;
            }
            else
            {
                menuHandle.EnableAll();
                propertiesHandleMenuItem.Enabled = false;
                protectedMenuItem.Enabled = false;
                inheritMenuItem.Enabled = false;
            }
        }

        private void closeHandleMenuItem_Click(object sender, EventArgs e)
        {
            lock (_listLock)
            {
                bool allGood = true;

                foreach (ListViewItem item in listHandles.SelectedItems)
                {
                    try
                    {
                        IntPtr handle = new IntPtr((int)BaseConverter.ToNumberParse(item.SubItems[2].Text));

                        using (ProcessHandle process =
                               new ProcessHandle(_pid, Program.MinProcessGetHandleInformationRights))
                        {
                            Win32.DuplicateObject(process.Handle, handle, 0, 0, DuplicateOptions.CloseSource);
                        }
                    }
                    catch (Exception ex)
                    {
                        allGood = false;

                        if (!PhUtils.ShowContinueMessage(
                            "Unable to close the handle \"" + item.SubItems[1].Text + "\"",
                            ex
                            ))
                            return;
                    }
                }

                if (allGood)
                {
                    foreach (ListViewItem item in listHandles.SelectedItems)
                        item.Selected = false;
                }
            }
        }

        private void protectedMenuItem_Click(object sender, EventArgs e)
        {
            HandleItem item = (HandleItem)listHandles.SelectedItems[0].Tag;
            HandleFlags flags = item.Handle.Flags;

            if ((flags & HandleFlags.ProtectFromClose) != 0)
                flags &= ~HandleFlags.ProtectFromClose;
            else
                flags |= HandleFlags.ProtectFromClose;

            try
            {
                using (var phandle = new ProcessHandle(_pid, Program.MinProcessQueryRights))
                    KProcessHacker.Instance.SetHandleAttributes(phandle, new IntPtr(item.Handle.Handle), flags);
            }
            catch (Exception ex)
            {
                PhUtils.ShowException("Unable to set handle attributes", ex);
            }
        }

        private void inheritMenuItem_Click(object sender, EventArgs e)
        {
            HandleItem item = (HandleItem)listHandles.SelectedItems[0].Tag;
            HandleFlags flags = item.Handle.Flags;

            if ((flags & HandleFlags.Inherit) != 0)
                flags &= ~HandleFlags.Inherit;
            else
                flags |= HandleFlags.Inherit;

            try
            {
                using (var phandle = new ProcessHandle(_pid, Program.MinProcessQueryRights))
                    KProcessHacker.Instance.SetHandleAttributes(phandle, new IntPtr(item.Handle.Handle), flags);
            }
            catch (Exception ex)
            {
                PhUtils.ShowException("Unable to set handle attributes", ex);
            }
        }

        private void propertiesHandleMenuItem_Click(object sender, EventArgs e)
        {
            if (listHandles.SelectedItems.Count != 1)
                return;

            var handleInfo = ((HandleItem)listHandles.SelectedItems[0].Tag).Handle;

            try
            {
                ShowHandleProperties(handleInfo);
            }
            catch (Exception ex)
            {
                PhUtils.ShowException("Unable to show handle properties", ex);
            }
        }
    }
}
