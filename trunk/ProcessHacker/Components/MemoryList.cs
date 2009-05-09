/*
 * Process Hacker - 
 *   memory region list
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
using System.Reflection;
using System.Windows.Forms;
using ProcessHacker.Native;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;
using ProcessHacker.UI;

namespace ProcessHacker.Components
{
    public partial class MemoryList : UserControl
    {
        private List<MemoryItem> _memoryItems;
        private int _runCount = 0;
        private MemoryProvider _provider;
        private bool _needsSort = false;
        public new event KeyEventHandler KeyDown;
        public new event MouseEventHandler MouseDown;
        public new event MouseEventHandler MouseUp;
        private object _listLock = new object();
        private int _pid;

        public MemoryList()
        {
            InitializeComponent();

            listMemory.KeyDown += new KeyEventHandler(listMemory_KeyDown);
            listMemory.MouseDown += new MouseEventHandler(listMemory_MouseDown);
            listMemory.MouseUp += new MouseEventHandler(listMemory_MouseUp);

            ColumnSettings.LoadSettings(Properties.Settings.Default.MemoryListViewColumns, listMemory);
            listMemory.ContextMenu = menuMemory;
            GenericViewMenu.AddMenuItems(copyMemoryMenuItem.MenuItems, listMemory, null);

            listMemory.ListViewItemSorter = new SortedListViewComparer(listMemory)
                {
                    SortColumn = 1,
                    SortOrder = SortOrder.Ascending,
                    VirtualMode = true,
                    RetrieveVirtualItem = this.listMemory_RetrieveVirtualItem
                };

            (listMemory.ListViewItemSorter as SortedListViewComparer).CustomSorters.Add(2,
                (x, y) =>
                {
                    MemoryItem ix = (MemoryItem)x.Tag;
                    MemoryItem iy = (MemoryItem)y.Tag;

                    return ix.Size.CompareTo(iy.Size);
                });
            listMemory.ColumnClick += (sender, e) => this.Sort();
        }

        private void listMemory_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.MouseUp != null)
                this.MouseUp(sender, e);
        }

        private void listMemory_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.MouseDown != null)
                this.MouseDown(sender, e);
        }

        private void listMemory_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.KeyDown != null)
                this.KeyDown(sender, e);

            if (!e.Handled)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    readWriteMemoryMemoryMenuItem_Click(null, null);
                }
            }
        }

        #region Properties

        public bool AutomaticSort { get; set; }

        public new bool DoubleBuffered
        {
            get
            {
                return (bool)typeof(ListView).GetProperty("DoubleBuffered",
                    BindingFlags.NonPublic | BindingFlags.Instance).GetValue(listMemory, null);
            }
            set
            {
                typeof(ListView).GetProperty("DoubleBuffered",
                    BindingFlags.NonPublic | BindingFlags.Instance).SetValue(listMemory, value, null);
            }
        }

        public override bool Focused
        {
            get
            {
                return listMemory.Focused;
            }
        }

        public override ContextMenu ContextMenu
        {
            get { return listMemory.ContextMenu; }
            set { listMemory.ContextMenu = value; }
        }

        public override ContextMenuStrip ContextMenuStrip
        {
            get { return listMemory.ContextMenuStrip; }
            set { listMemory.ContextMenuStrip = value; }
        }

        public ListView List
        {
            get { return listMemory; }
        }

        public MemoryProvider Provider
        {
            get { return _provider; }
            set
            {
                if (_provider != null)
                {
                    _provider.DictionaryAdded -= new MemoryProvider.ProviderDictionaryAdded(provider_DictionaryAdded);
                    _provider.DictionaryModified -= new MemoryProvider.ProviderDictionaryModified(provider_DictionaryModified);
                    _provider.DictionaryRemoved -= new MemoryProvider.ProviderDictionaryRemoved(provider_DictionaryRemoved);
                    _provider.Updated -= new MemoryProvider.ProviderUpdateOnce(provider_Updated);
                    _memoryItems = null;
                }

                _provider = value;

                listMemory.Items.Clear();
                _pid = -1;

                if (_provider != null)
                {
                    _provider.DictionaryAdded += new MemoryProvider.ProviderDictionaryAdded(provider_DictionaryAdded);
                    _provider.DictionaryModified += new MemoryProvider.ProviderDictionaryModified(provider_DictionaryModified);
                    _provider.DictionaryRemoved += new MemoryProvider.ProviderDictionaryRemoved(provider_DictionaryRemoved);
                    _provider.Updated += new MemoryProvider.ProviderUpdateOnce(provider_Updated);
                    _pid = _provider.Pid;
                    _memoryItems = new List<MemoryItem>();

                    foreach (MemoryItem item in _provider.Dictionary.Values)
                    {
                        provider_DictionaryAdded(item);
                    }
                }
            }
        }

        #endregion

        #region Interfacing

        public void BeginUpdate()
        {
            listMemory.BeginUpdate();
        }

        public void EndUpdate()
        {
            listMemory.EndUpdate();
        }

        public ListView.ListViewItemCollection Items
        {
            get { return listMemory.Items; }
        }

        public ListView.SelectedListViewItemCollection SelectedItems
        {
            get { return listMemory.SelectedItems; }
        }

        #endregion

        private string GetProtectStr(MemoryProtection protect)
        {
            string protectStr;

            if (protect == MemoryProtection.AccessDenied)
                protectStr = "";
            else if ((protect & MemoryProtection.Execute) != 0)
                protectStr = "X";
            else if ((protect & MemoryProtection.ExecuteRead) != 0)
                protectStr = "RX";
            else if ((protect & MemoryProtection.ExecuteReadWrite) != 0)
                protectStr = "RWX";
            else if ((protect & MemoryProtection.ExecuteWriteCopy) != 0)
                protectStr = "WCX";
            else if ((protect & MemoryProtection.NoAccess) != 0)
                protectStr = "NA";
            else if ((protect & MemoryProtection.ReadOnly) != 0)
                protectStr = "R";
            else if ((protect & MemoryProtection.ReadWrite) != 0)
                protectStr = "RW";
            else if ((protect & MemoryProtection.WriteCopy) != 0)
                protectStr = "WC";
            else
                protectStr = "?";

            if ((protect & MemoryProtection.Guard) != 0)
                protectStr += "+G";
            if ((protect & MemoryProtection.NoCache) != 0)
                protectStr += "+NC";
            if ((protect & MemoryProtection.WriteCombine) != 0)
                protectStr = "+WCM";

            return protectStr;
        }

        private string GetStateStr(MemoryState state)
        {
            if (state == MemoryState.Commit)
                return "Commit";
            else if (state == MemoryState.Free)
                return "Free";
            else if (state == MemoryState.Reserve)
                return "Reserve";
            else if (state == MemoryState.Reset)
                return "Reset";
            else
                return "Unknown";
        }

        private string GetTypeStr(MemoryType type)
        {
            if (type == MemoryType.Image)
                return "Image";
            else if (type == MemoryType.Mapped)
                return "Mapped";
            else if (type == MemoryType.Private)
                return "Private";
            else
                return "Unknown";
        }

        public void Sort()
        {
            _memoryItems.Sort((m1, m2) =>
                (listMemory.ListViewItemSorter as SortedListViewComparer).Compare(
                this.MakeListViewItem(m1),
                this.MakeListViewItem(m2)
                ));
            listMemory.Invalidate();
        }

        private void provider_Updated()
        {
            if (_needsSort)
            {
                this.BeginInvoke(new MethodInvoker(() =>
                    {
                        if (_needsSort)
                        {
                            this.Sort();
                            _needsSort = false;
                        }
                    }));
            }

            _runCount++;
        }

        private void provider_DictionaryAdded(MemoryItem item)
        {
            this.BeginInvoke(new MethodInvoker(() =>
                {
                    _memoryItems.Add(item);
                    listMemory.VirtualListSize = _memoryItems.Count;
                    _needsSort = true;
                }));
        }

        private void provider_DictionaryModified(MemoryItem oldItem, MemoryItem newItem)
        {
            this.BeginInvoke(new MethodInvoker(() =>
                {
                    lock (_memoryItems)
                    {
                        _memoryItems[_memoryItems.IndexOf(oldItem)] = newItem;
                        _needsSort = true;
                    }
                }));
        }

        private void provider_DictionaryRemoved(MemoryItem item)
        {
            this.BeginInvoke(new MethodInvoker(() =>
                {
                    lock (_memoryItems)
                    {
                        _memoryItems.Remove(item);
                        listMemory.VirtualListSize = _provider.Dictionary.Count;
                    }
                }));
        }

        public ListViewItem MakeListViewItem(MemoryItem item)
        {
            ListViewItem litem = new ListViewItem();

            litem.Name = item.Address.ToString();

            if (item.State == MemoryState.Free)
            {
                litem.Text = "Free";
            }
            else if (item.Type == MemoryType.Image)
            {
                if (item.ModuleName != null)
                    litem.Text = item.ModuleName;
                else
                    litem.Text = "Image";

                litem.Text += " (" + GetStateStr(item.State) + ")";
            }
            else
            {
                litem.Text = GetTypeStr(item.Type);
                litem.Text += " (" + GetStateStr(item.State) + ")";
            }

            litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem, "0x" + item.Address.ToString("x8")));
            litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem, Misc.GetNiceSizeName(item.Size)));
            litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem, GetProtectStr(item.Protection)));
            litem.Tag = item;

            return litem;
        }

        public void listMemory_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            lock (listMemory)
            {
                e.Item = this.MakeListViewItem(this.GetMemoryItem(e.ItemIndex));
            }
        }

        public void SaveSettings()
        {
            Properties.Settings.Default.MemoryListViewColumns = ColumnSettings.SaveSettings(listMemory);
        }

        private void listMemory_DoubleClick(object sender, EventArgs e)
        {
            readWriteMemoryMemoryMenuItem_Click(sender, e);
        }

        private MemoryItem GetMemoryItem(int index)
        {
            return _memoryItems[index];
        }

        private void menuMemory_Popup(object sender, EventArgs e)
        {
            if (listMemory.SelectedIndices.Count == 1)
            {
                menuMemory.EnableAll();

                MemoryItem item = this.GetMemoryItem(listMemory.SelectedIndices[0]);

                if (item.State != MemoryState.Commit ||
                    item.Type != MemoryType.Private)
                {                                         
                    freeMenuItem.Enabled = false;
                    decommitMenuItem.Enabled = false;
                }
            }
            else
            {
                menuMemory.DisableAll();

                readWriteAddressMemoryMenuItem.Enabled = true;

                if (listMemory.SelectedIndices.Count > 1)
                {
                    copyMemoryMenuItem.Enabled = true;
                }

                if (listMemory.VirtualListSize > 0)
                {
                    selectAllMemoryMenuItem.Enabled = true;
                }
                else
                {
                    selectAllMemoryMenuItem.Enabled = false;
                }
            }
        }

        private void changeMemoryProtectionMemoryMenuItem_Click(object sender, EventArgs e)
        {
            MemoryItem item = this.GetMemoryItem(listMemory.SelectedIndices[0]);
            VirtualProtectWindow w = new VirtualProtectWindow(_pid, item.Address, item.Size);

            w.ShowDialog();
        }

        private void readWriteMemoryMemoryMenuItem_Click(object sender, EventArgs e)
        {
            if (listMemory.SelectedIndices.Count != 1)
                return;

            MemoryItem item = this.GetMemoryItem(listMemory.SelectedIndices[0]);

            MemoryEditor.ReadWriteMemory(_pid, item.Address, item.Size, false);
        }

        private void readWriteAddressMemoryMenuItem_Click(object sender, EventArgs e)
        {
            PromptBox prompt = new PromptBox();

            if (prompt.ShowDialog() == DialogResult.OK)
            {
                IntPtr address = new IntPtr(-1);
                IntPtr regionAddress = IntPtr.Zero;
                int regionSize = 0;
                bool found = false;

                try
                {
                    address = new IntPtr((int)BaseConverter.ToNumberParse(prompt.Value));

                    if (address.CompareTo(0) < 0)
                        throw new Exception();
                }
                catch
                {
                    MessageBox.Show("Invalid address!", "Process Hacker", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    return;
                }

                List<MemoryItem> items = new List<MemoryItem>();

                foreach (MemoryItem item in _provider.Dictionary.Values)
                    items.Add(item);

                items.Sort((i1, i2) => i1.Address.CompareTo(i2.Address));

                int i = 0;

                foreach (MemoryItem item in items)
                {
                    if (item.Address.CompareTo(address) > 0)
                    {
                        MemoryItem regionItem = items[i - 1];

                        listMemory.Items[regionItem.Address.ToString()].Selected = true;
                        listMemory.Items[regionItem.Address.ToString()].EnsureVisible();
                        regionAddress = regionItem.Address;
                        regionSize = regionItem.Size;
                        found = true;

                        break;
                    }

                    i++;
                }

                if (!found)
                {
                    MessageBox.Show("Memory address not found!", "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                MemoryEditor m_e = MemoryEditor.ReadWriteMemory(_pid, regionAddress, regionSize, false,
                   new Program.MemoryEditorInvokeAction(delegate(MemoryEditor f) { f.Select(address.Decrement(regionAddress).ToInt64(), 1); }));
            }
        }

        private void selectAllMemoryMenuItem_Click(object sender, EventArgs e)
        {
            Misc.SelectAll(listMemory);
        }

        private void freeMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to free this memory region?",
                "Process Hacker", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2)
                == DialogResult.Yes)
            {
                try
                {
                    using (var phandle =
                        new ProcessHandle(_pid, ProcessAccess.VmOperation))
                    {
                        MemoryItem item = this.GetMemoryItem(listMemory.SelectedIndices[0]);

                        phandle.FreeMemory(item.Address, item.Size, false);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Message Box", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void decommitMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to decommit this memory region?",
               "Process Hacker", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2)
               == DialogResult.Yes)
            {
                try
                {
                    using (ProcessHandle phandle =
                        new ProcessHandle(_pid, ProcessAccess.VmOperation))
                    {
                        MemoryItem item = this.GetMemoryItem(listMemory.SelectedIndices[0]);

                        phandle.FreeMemory(item.Address, item.Size, true);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Message Box", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
