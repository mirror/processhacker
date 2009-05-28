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
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ProcessHacker.Native;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;
using ProcessHacker.UI;
using System.Drawing;
using ProcessHacker.Native.Ui;

namespace ProcessHacker.Components
{
    public partial class HandleList : UserControl
    {
        private object _listLock = new object();
        private HandleProvider _provider;
        private int _runCount = 0;
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
            ColumnSettings.LoadSettings(Properties.Settings.Default.HandleListViewColumns, listHandles);

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
            _highlightingContext.Tick();
            _runCount++;
        }

        private Color GetHandleColor(HandleItem item)
        {
            if (Properties.Settings.Default.UseColorProtectedHandles &&
                (item.Handle.Flags & HandleFlags.ProtectFromClose) != 0
                )
                return Properties.Settings.Default.ColorProtectedHandles;
            else if (Properties.Settings.Default.UseColorInheritHandles &&
                (item.Handle.Flags & HandleFlags.Inherit) != 0
                )
                return Properties.Settings.Default.ColorInheritHandles;
            else
                return SystemColors.Window;
        }

        private void provider_DictionaryAdded(HandleItem item)
        {
            this.BeginInvoke(new MethodInvoker(() =>
                {
                    HighlightedListViewItem litem = new HighlightedListViewItem(_highlightingContext,
                        item.RunId > 0 && _runCount > 0);

                    litem.Name = item.Handle.Handle.ToString();
                    litem.Text = item.ObjectInfo.TypeName;
                    litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem, item.ObjectInfo.BestName));
                    litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem, "0x" + item.Handle.Handle.ToString("x")));
                    litem.Tag = item;

                    litem.NormalColor = this.GetHandleColor(item);

                    listHandles.Items.Add(litem);
                }));
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
                    {
                        int index = listHandles.Items[item.Handle.Handle.ToString()].Index;
                        bool selected = listHandles.Items[item.Handle.Handle.ToString()].Selected;
                        int selectedCount = listHandles.SelectedItems.Count;

                        listHandles.Items[item.Handle.Handle.ToString()].Remove();
                    }
                }));
        }

        private int _pid;

        public void SaveSettings()
        {
            Properties.Settings.Default.HandleListViewColumns = ColumnSettings.SaveSettings(listHandles);
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

                propertiesHandleMenuItem.Enabled = false;

                string type = listHandles.SelectedItems[0].SubItems[0].Text;

                if (HasHandleProperties(type))
                    propertiesHandleMenuItem.Enabled = true;

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

        public static bool HasHandleProperties(string type)
        {
            if (type == "Token" || type == "Process" || type == "File" || 
                type == "Event" || type == "Mutant" || type == "Section" || 
                type == "Semaphore" || 
                type == "DLL" || type == "Mapped File")
                return true;
            else
                return false;
        }

        public static void ShowHandleProperties(SystemHandleInformation handleInfo)
        {
            try
            {
                HandlePropertiesWindow window = new HandlePropertiesWindow(handleInfo);
                IntPtr handle = new IntPtr(handleInfo.Handle);
                ProcessHandle phandle = new ProcessHandle(handleInfo.ProcessId, ProcessAccess.DupHandle);
                Win32Handle dupHandle; 

                window.HandlePropertiesCallback += (control, name, typeName) =>
                    {
                        switch (typeName.ToLower())
                        {
                            // Objects with separate property windows:
                            case "file":
                            case "job":
                            case "token":
                            case "process":
                                {
                                    Button b = new Button();

                                    b.FlatStyle = FlatStyle.System;
                                    b.Location = new Point(10, 20);
                                    b.Text = "Properties...";
                                    b.Click += (sender, e) =>
                                        {
                                            try
                                            {
                                                switch (typeName.ToLower())
                                                {
                                                    case "file":
                                                        {
                                                            FileUtils.ShowProperties(name);
                                                        }
                                                        break;
                                                    case "job":
                                                        {
                                                            dupHandle =
                                                                new Win32Handle(
                                                                    phandle, handle, 
                                                                    (int)JobObjectAccess.Query);
                                                            (new JobWindow(JobObjectHandle.FromHandle(dupHandle))).ShowDialog();
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
                                                                    new Win32Handle(
                                                                        phandle, handle,
                                                                        (int)OSVersion.MinProcessQueryInfoAccess);
                                                                pid = ProcessHandle.FromHandle(dupHandle).
                                                                    GetBasicInformation().UniqueProcessId;
                                                            }

                                                            Program.GetProcessWindow(Program.ProcessProvider.Dictionary[pid],
                                                                (f) => Program.FocusWindow(f));
                                                        }
                                                        break;
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                MessageBox.Show(ex.Message, "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            }
                                        };

                                    control.Controls.Add(b);
                                }
                                break;
                            case "event":
                                {
                                    dupHandle = new Win32Handle(phandle, handle, (int)EventAccess.QueryState);
                                    var eventProps = new EventProperties(EventHandle.FromHandle(dupHandle));
                                    eventProps.Location = new Point(10, 20);
                                    control.Controls.Add(eventProps);
                                }
                                break;
                            case "mutant":
                                {
                                    dupHandle = new Win32Handle(phandle, handle, (int)MutantAccess.QueryState);
                                    var mutantProps = new MutantProperties(MutantHandle.FromHandle(dupHandle));
                                    mutantProps.Location = new Point(10, 20);
                                    control.Controls.Add(mutantProps);
                                }
                                break;
                            case "section":
                                {
                                    dupHandle = new Win32Handle(phandle, handle, (int)SectionAccess.Query);
                                    var sectionProps = new SectionProperties(SectionHandle.FromHandle(dupHandle));
                                    sectionProps.Location = new Point(10, 20);
                                    control.Controls.Add(sectionProps);
                                }
                                break;
                            case "semaphore":
                                {
                                    dupHandle = new Win32Handle(phandle, handle, (int)SemaphoreAccess.QueryState);
                                    var semaphoreProps = new SemaphoreProperties(SemaphoreHandle.FromHandle(dupHandle));
                                    semaphoreProps.Location = new Point(10, 20);
                                    control.Controls.Add(semaphoreProps);
                                }
                                break;
                        }
                    };

                window.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                        var result = MessageBox.Show(
                            "Could not close handle \"" + item.SubItems[1].Text + "\":\n\n" + ex.Message,
                             "Process Hacker", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);

                        if (result == DialogResult.Cancel)
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
                MessageBox.Show(ex.Message, "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show(ex.Message, "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show(ex.Message, "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
