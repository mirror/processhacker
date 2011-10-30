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
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ProcessHacker.Api;
using ProcessHacker.Common;
using ProcessHacker.Common.Ui;
using ProcessHacker.Native;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;
using ProcessHacker.Native.Security.AccessControl;
using ProcessHacker.Native.Ui;
using ProcessHacker.UI;

namespace ProcessHacker.Components
{
    public partial class HandleList : ProcessPropertySheetPage
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
            return true;
        }

        public static void ShowHandleProperties(SystemHandleEntry handleInfo)
        {
            try
            {
                IntPtr handle = new IntPtr(handleInfo.Handle);
                ProcessHandle phandle = new ProcessHandle(handleInfo.ProcessId, ProcessAccess.DupHandle);
                GenericHandle dupHandle = null;

                // Try to get a handle, since we need one for security editing.
                try
                {
                    dupHandle = new GenericHandle(phandle, handle, 0);
                }
                catch
                { }

                PropSheetHeader64 header = new PropSheetHeader64
                {
                    dwSize = (uint)PropSheetHeader64.SizeOf, 
                    nPages = 2, 
                    dwFlags = (uint)PropSheetFlags.PSH_DEFAULT, 
                    pszCaption = "Handle Properties"
                };

                using (HandleDetails hw = new HandleDetails())
                {
                    hw.ObjectHandle = handleInfo;
                    hw.HandlePropertiesCallback += (control, name, typeName) =>
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
                                    Button b = new Button
                                    {
                                        FlatStyle = FlatStyle.System,
                                        Text = "Properties"
                                    };

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
                                                        dupHandle = new GenericHandle(phandle, handle, (int)JobObjectAccess.Query);

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
                                                        using (TokenWindow twindow = new TokenWindow(new RemoteTokenHandle(phandle, handle)))
                                                        {
                                                            twindow.ShowDialog();
                                                        }
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
                                                            pid = ProcessHandle.FromHandle(dupHandle).ProcessId;
                                                        }

                                                        Program.GetProcessWindow(Program.ProcessProvider.Dictionary[pid], Program.FocusWindow);
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

                    hw.Init();

                    IntPtr[] pages = new IntPtr[2];
                    pages[0] = hw.CreatePageHandle();
                    pages[1] = CreateSecurityPage(SecurityEditor.EditSecurity2(
                        null,
                        SecurityEditor.GetSecurableWrapper(dupHandle),
                        hw._name,
                        NativeTypeFactory.GetAccessEntries(NativeTypeFactory.GetObjectType(hw._typeName))
                        ));

                    GCHandle gch = GCHandle.Alloc(pages, GCHandleType.Pinned);
                    header.phpage = gch.AddrOfPinnedObject();

                    

                    PropertySheetW(ref header);

                    if (dupHandle != null)
                        dupHandle.Dispose();
                }
            }
            catch (Exception ex)
            {
                PhUtils.ShowException("Unable to show handle properties", ex);
            }
        }

        [DllImport("Comctl32.dll")]
        public static extern IntPtr PropertySheetW([In, MarshalAs(UnmanagedType.Struct)]ref PropSheetHeader64 lppsph);

        [DllImport("Aclui.dll")]
        public static extern IntPtr CreateSecurityPage(ISecurityInformation lppsph);

        [StructLayout(LayoutKind.Explicit, Pack = 8, CharSet = CharSet.Unicode)]
        public struct PropSheetHeader64
        {
            public static readonly int SizeOf;

            static PropSheetHeader64()
            {
                SizeOf = Marshal.SizeOf(typeof(PropSheetHeader64));
            }

            [FieldOffset(0)]
            public UInt32 dwSize;
            [FieldOffset(4)]
            public UInt32 dwFlags;
            [FieldOffset(8)]
            public IntPtr hwndParent;
            [FieldOffset(16)]
            public IntPtr hInstance;
            [FieldOffset(24)]
            public IntPtr hIcon;
            [FieldOffset(32)]
            public String pszCaption;
            [FieldOffset(40)]
            public UInt32 nPages;
            [FieldOffset(48)]
            public string pStartPage;
            [FieldOffset(56)]
            public IntPtr phpage;

            // following fields all for PROPSHEETHEADER_V2
            [FieldOffset(64)]
            public IntPtr pfnCallback;
            [FieldOffset(72)]
            public IntPtr hbmWatermark;
            [FieldOffset(80)]
            public IntPtr hplWatermark;
            [FieldOffset(88)]
            public IntPtr hbmHeader;
        }

        [Flags]
        internal enum PropSheetFlags : uint
        {
            PSH_DEFAULT = 0x00000000,
            PSH_PROPTITLE = 0x00000001,
            PSH_USEHICON = 0x00000002,
            PSH_USEICONID = 0x00000004,
            PSH_PROPSHEETPAGE = 0x00000008,
            PSH_WIZARDHASFINISH = 0x00000010,
            PSH_WIZARD = 0x00000020,
            PSH_USEPSTARTPAGE = 0x00000040,
            PSH_NOAPPLYNOW = 0x00000080,
            PSH_USECALLBACK = 0x00000100,
            PSH_HASHELP = 0x00000200,
            PSH_MODELESS = 0x00000400,
            PSH_RTLREADING = 0x00000800,
            PSH_WIZARDCONTEXTHELP = 0x00001000,
            PSH_WIZARD97 = 0x01000000,
            PSH_WATERMARK = 0x00008000,
            PSH_USEHBMWATERMARK = 0x00010000,  // user pass in a hbmWatermark instead of pszbmWatermark
            PSH_USEHPLWATERMARK = 0x00020000,  //
            PSH_STRETCHWATERMARK = 0x00040000,  // stretchwatermark also applies for the header
            PSH_HEADER = 0x00080000,
            PSH_USEHBMHEADER = 0x00100000,
            PSH_USEPAGELANG = 0x00200000  // use frame dialog template matched to page
        }

        private readonly object _listLock = new object();
        private HandleProvider _provider;
        private int _runCount;
        private readonly List<ListViewItem> _needsAdd = new List<ListViewItem>();
        private readonly HighlightingContext _highlightingContext;
        public new event KeyEventHandler KeyDown;
        public new event MouseEventHandler MouseDown;
        public new event MouseEventHandler MouseUp;
        public event EventHandler SelectedIndexChanged;

        public HandleList()
        {
            InitializeComponent();

            _highlightingContext = new HighlightingContext(listHandles);
            listHandles.KeyDown += this.listHandles_KeyDown;
            listHandles.MouseDown += this.listHandles_MouseDown;
            listHandles.MouseUp += this.listHandles_MouseUp;
            listHandles.DoubleClick += this.listHandles_DoubleClick;
            listHandles.SelectedIndexChanged += this.listHandles_SelectedIndexChanged;

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

        private void listHandles_SelectedIndexChanged(object sender, EventArgs e)
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
                switch (e.KeyCode)
                {
                    case Keys.Enter:
                        this.propertiesHandleMenuItem_Click(null, null);
                        break;
                    case Keys.Delete:
                        if (ConfirmHandleClose())
                        {
                            this.closeHandleMenuItem_Click(null, null);
                        }
                        break;
                }
            }
        }

        #region Properties

        public override bool Focused
        {
            get
            {
                return listHandles.Focused;
            }
        }

        public override ContextMenuStrip ContextMenuStrip
        {
            get { return listHandles.ContextMenuStrip; }
            set { listHandles.ContextMenuStrip = value; }
        }

        public ExtendedListView List
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
            
            if (Settings.Instance.UseColorInheritHandles &&
                (item.Handle.Flags & HandleFlags.Inherit) != 0
                )
                return Settings.Instance.ColorInheritHandles;
            
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
            HighlightedListViewItem litem = new HighlightedListViewItem(_highlightingContext, item.RunId > 0 && _runCount > 0)
            {
                Name = item.Handle.Handle.ToString(), 
                Text = item.ObjectInfo.TypeName
            };

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
                    ((HighlightedListViewItem)this.listHandles.Items[newItem.Handle.Handle.ToString()]).NormalColor = this.GetHandleColor(newItem);
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
                //menuHandle.DisableAll();
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
