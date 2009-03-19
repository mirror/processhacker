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
using System.Reflection;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections.Generic;

namespace ProcessHacker
{
    public partial class MemoryList : UserControl
    {
        private MemoryProvider _provider;
        private HighlightingContext _highlightingContext;
        public new event KeyEventHandler KeyDown;
        public new event MouseEventHandler MouseDown;
        public new event MouseEventHandler MouseUp;
        private int _pid;

        public MemoryList()
        {
            InitializeComponent();

            _highlightingContext = new HighlightingContext(listMemory);
            listMemory.KeyDown += new KeyEventHandler(MemoryList_KeyDown);
            listMemory.MouseDown += new MouseEventHandler(listMemory_MouseDown);
            listMemory.MouseUp += new MouseEventHandler(listMemory_MouseUp);

            ColumnSettings.LoadSettings(Properties.Settings.Default.MemoryListViewColumns, listMemory);
            listMemory.ContextMenu = menuMemory;
            GenericViewMenu.AddMenuItems(copyMemoryMenuItem.MenuItems, listMemory, null);

            listMemory.ListViewItemSorter = new SortedListComparer(listMemory)
                {
                    SortColumn = 1,
                    SortOrder = SortOrder.Ascending
                };

            (listMemory.ListViewItemSorter as SortedListComparer).CustomSorters.Add(2,
                (x, y) =>
                {
                    MemoryItem ix = (MemoryItem)x.Tag;
                    MemoryItem iy = (MemoryItem)y.Tag;

                    return ix.Size.CompareTo(iy.Size);
                });
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

        private void MemoryList_KeyDown(object sender, KeyEventArgs e)
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
                }

                _provider = value;

                listMemory.Items.Clear();
                _pid = -1;

                if (_provider != null)
                {
                    foreach (MemoryItem item in _provider.Dictionary.Values)
                    {
                        provider_DictionaryAdded(item);
                    }

                    _provider.UseInvoke = true;
                    _provider.Invoke = new MemoryProvider.ProviderInvokeMethod(this.BeginInvoke);
                    _provider.DictionaryAdded += new MemoryProvider.ProviderDictionaryAdded(provider_DictionaryAdded);
                    _provider.DictionaryModified += new MemoryProvider.ProviderDictionaryModified(provider_DictionaryModified);
                    _provider.DictionaryRemoved += new MemoryProvider.ProviderDictionaryRemoved(provider_DictionaryRemoved);
                    _provider.Updated += new MemoryProvider.ProviderUpdateOnce(provider_Updated);
                    _pid = _provider.PID;
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

        private string GetProtectStr(Win32.MEMORY_PROTECTION protect)
        {
            string protectStr;

            if (protect == Win32.MEMORY_PROTECTION.PAGE_ACCESS_DENIED)
                protectStr = "";
            else if ((protect & Win32.MEMORY_PROTECTION.PAGE_EXECUTE) != 0)
                protectStr = "X";
            else if ((protect & Win32.MEMORY_PROTECTION.PAGE_EXECUTE_READ) != 0)
                protectStr = "RX";
            else if ((protect & Win32.MEMORY_PROTECTION.PAGE_EXECUTE_READWRITE) != 0)
                protectStr = "RWX";
            else if ((protect & Win32.MEMORY_PROTECTION.PAGE_EXECUTE_WRITECOPY) != 0)
                protectStr = "WCX";
            else if ((protect & Win32.MEMORY_PROTECTION.PAGE_NOACCESS) != 0)
                protectStr = "NA";
            else if ((protect & Win32.MEMORY_PROTECTION.PAGE_READONLY) != 0)
                protectStr = "R";
            else if ((protect & Win32.MEMORY_PROTECTION.PAGE_READWRITE) != 0)
                protectStr = "RW";
            else if ((protect & Win32.MEMORY_PROTECTION.PAGE_WRITECOPY) != 0)
                protectStr = "WC";
            else
                protectStr = "?";

            if ((protect & Win32.MEMORY_PROTECTION.PAGE_GUARD) != 0)
                protectStr += "+G";
            if ((protect & Win32.MEMORY_PROTECTION.PAGE_NOCACHE) != 0)
                protectStr += "+NC";
            if ((protect & Win32.MEMORY_PROTECTION.PAGE_WRITECOMBINE) != 0)
                protectStr = "+WCM";

            return protectStr;
        }

        private string GetStateStr(Win32.MEMORY_STATE state)
        {
            if (state == Win32.MEMORY_STATE.MEM_COMMIT)
                return "Commit";
            else if (state == Win32.MEMORY_STATE.MEM_FREE)
                return "Free";
            else if (state == Win32.MEMORY_STATE.MEM_RESERVE)
                return "Reserve";
            else if (state == Win32.MEMORY_STATE.MEM_RESET)
                return "Reset";
            else
                return "Unknown";
        }

        private string GetTypeStr(Win32.MEMORY_TYPE type)
        {
            if (type == Win32.MEMORY_TYPE.MEM_IMAGE)
                return "Image";
            else if (type == Win32.MEMORY_TYPE.MEM_MAPPED)
                return "Mapped";
            else if (type == Win32.MEMORY_TYPE.MEM_PRIVATE)
                return "Private";
            else
                return "Unknown";
        }       

        private void provider_Updated()
        {
            _highlightingContext.Tick();
        }

        private void provider_DictionaryAdded(MemoryItem item)
        {
            HighlightedListViewItem litem = new HighlightedListViewItem(_highlightingContext, this.Highlight);

            litem.Name = item.Address.ToString();

            if (item.State == Win32.MEMORY_STATE.MEM_FREE)
            {
                litem.Text = "Free";
            }
            else if (item.Type == Win32.MEMORY_TYPE.MEM_IMAGE)
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

            listMemory.Items.Add(litem);
        }

        private void provider_DictionaryModified(MemoryItem oldItem, MemoryItem newItem)
        {
            lock (this)
            {
                ListViewItem litem = listMemory.Items[newItem.Address.ToString()];

                if (newItem.State == Win32.MEMORY_STATE.MEM_FREE) 
                {
                    litem.Text = "Free";
                }
                else if (newItem.Type == Win32.MEMORY_TYPE.MEM_IMAGE)
                {
                    if (newItem.ModuleName != null)
                        litem.Text = newItem.ModuleName;
                    else
                        litem.Text = "Image";

                    litem.Text += " (" + GetStateStr(newItem.State) + ")";
                }
                else
                {
                    litem.Text = GetTypeStr(newItem.Type);
                    litem.Text += " (" + GetStateStr(newItem.State) + ")";
                }

                litem.SubItems[1].Text = "0x" + newItem.Address.ToString("x8");
                litem.SubItems[2].Text = Misc.GetNiceSizeName(newItem.Size);
                litem.SubItems[3].Text = GetProtectStr(newItem.Protection);
                litem.Tag = newItem;

                listMemory.Sort();
            }
        }

        private void provider_DictionaryRemoved(MemoryItem item)
        {
            lock (this)
            {
                listMemory.Items[item.Address.ToString()].Remove();
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

        private void menuMemory_Popup(object sender, EventArgs e)
        {
            if (listMemory.SelectedItems.Count == 1)
            {
                menuMemory.EnableAll();

                MemoryItem item = (MemoryItem)listMemory.SelectedItems[0].Tag;

                if (item.State != Win32.MEMORY_STATE.MEM_COMMIT ||
                    item.Type != Win32.MEMORY_TYPE.MEM_PRIVATE)
                {                                         
                    freeMenuItem.Enabled = false;
                    decommitMenuItem.Enabled = false;
                }
            }
            else
            {
                menuMemory.DisableAll();

                readWriteAddressMemoryMenuItem.Enabled = true;

                if (listMemory.SelectedItems.Count > 1)
                {
                    copyMemoryMenuItem.Enabled = true;
                }

                if (listMemory.Items.Count > 0)
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
            MemoryItem item = (MemoryItem)listMemory.SelectedItems[0].Tag;
            VirtualProtectWindow w = new VirtualProtectWindow(_pid, item.Address, item.Size);

            w.ShowDialog();
        }

        private void readWriteMemoryMemoryMenuItem_Click(object sender, EventArgs e)
        {
            MemoryItem item = (MemoryItem)listMemory.SelectedItems[0].Tag;

            MemoryEditor.ReadWriteMemory(_pid, item.Address, item.Size, false);
        }

        private void readWriteAddressMemoryMenuItem_Click(object sender, EventArgs e)
        {
            PromptBox prompt = new PromptBox();

            if (prompt.ShowDialog() == DialogResult.OK)
            {
                int address = -1;
                int regionAddress = 0;
                int regionSize = 0;
                bool found = false;

                try
                {
                    address = (int)BaseConverter.ToNumberParse(prompt.Value);

                    if (address < 0)
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

                items.Sort(new Comparison<MemoryItem>(delegate(MemoryItem i1, MemoryItem i2)
                    {
                        return i1.Address.CompareTo(i2.Address);
                    }));

                int i = 0;

                foreach (MemoryItem item in items)
                {
                    if (item.Address > address)
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
                   new Program.MemoryEditorInvokeAction(delegate(MemoryEditor f) { f.Select(address - regionAddress, 1); }));
            }
        }

        private void selectAllMemoryMenuItem_Click(object sender, EventArgs e)
        {
            Misc.SelectAll(listMemory.Items);
        }

        private void freeMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to free this memory region?",
                "Process Hacker", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2)
                == DialogResult.Yes)
            {
                try
                {
                    using (Win32.ProcessHandle phandle =
                        new Win32.ProcessHandle(_pid, Win32.PROCESS_RIGHTS.PROCESS_VM_OPERATION))
                    {
                        MemoryItem item = (MemoryItem)listMemory.SelectedItems[0].Tag;

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
                    using (Win32.ProcessHandle phandle =
                        new Win32.ProcessHandle(_pid, Win32.PROCESS_RIGHTS.PROCESS_VM_OPERATION))
                    {
                        MemoryItem item = (MemoryItem)listMemory.SelectedItems[0].Tag;

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
