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
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ProcessHacker
{
    public partial class HandleList : UserControl
    {
        private HandleProvider _provider;
        private HighlightingContext _highlightingContext;
        public new event KeyEventHandler KeyDown;
        public new event MouseEventHandler MouseDown;
        public new event MouseEventHandler MouseUp;
        public event EventHandler SelectedIndexChanged;

        public HandleList()
        {
            InitializeComponent();

            _highlightingContext = new HighlightingContext(listHandles);
            listHandles.ListViewItemSorter = new SortedListComparer(listHandles);
            listHandles.KeyDown += new KeyEventHandler(HandleList_KeyDown);
            listHandles.MouseDown += new MouseEventHandler(listHandles_MouseDown);
            listHandles.MouseUp += new MouseEventHandler(listHandles_MouseUp);
            listHandles.DoubleClick += new EventHandler(listHandles_DoubleClick);
            listHandles.SelectedIndexChanged += new System.EventHandler(listHandles_SelectedIndexChanged);

            listHandles.ContextMenu = menuHandle;
            GenericViewMenu.AddMenuItems(copyHandleMenuItem.MenuItems, listHandles, null);
            ColumnSettings.LoadSettings(Properties.Settings.Default.HandleListViewColumns, listHandles);
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

        private void HandleList_KeyDown(object sender, KeyEventArgs e)
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
                    _provider.DictionaryAdded -= new HandleProvider.ProviderDictionaryAdded(provider_DictionaryAdded);
                    _provider.DictionaryRemoved -= new HandleProvider.ProviderDictionaryRemoved(provider_DictionaryRemoved);
                    _provider.Updated -= new HandleProvider.ProviderUpdateOnce(provider_Updated);
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

                    _provider.UseInvoke = true;
                    _provider.Invoke = new HandleProvider.ProviderInvokeMethod(this.BeginInvoke);
                    _provider.DictionaryAdded += new HandleProvider.ProviderDictionaryAdded(provider_DictionaryAdded);
                    _provider.DictionaryRemoved += new HandleProvider.ProviderDictionaryRemoved(provider_DictionaryRemoved);
                    _provider.Updated += new HandleProvider.ProviderUpdateOnce(provider_Updated);
                    _pid = _provider.PID;
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
        }

        private void provider_DictionaryAdded(HandleItem item)
        {
            HighlightedListViewItem litem = new HighlightedListViewItem(_highlightingContext, this.Highlight);

            litem.Name = item.Handle.Handle.ToString();
            litem.Text = item.ObjectInfo.TypeName;
            litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem, item.ObjectInfo.BestName));
            litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem, "0x" + item.Handle.Handle.ToString("x")));

            listHandles.Items.Add(litem);
        }

        private void provider_DictionaryRemoved(HandleItem item)
        {
            int index = listHandles.Items[item.Handle.Handle.ToString()].Index;
            bool selected = listHandles.Items[item.Handle.Handle.ToString()].Selected;
            int selectedCount = listHandles.SelectedItems.Count;

            listHandles.Items[item.Handle.Handle.ToString()].Remove();
        }

        private int _pid;

        public void SaveSettings()
        {
            Properties.Settings.Default.HandleListViewColumns = ColumnSettings.SaveSettings(listHandles);
        }

        private void menuHandle_Popup(object sender, EventArgs e)
        {
            if (listHandles.SelectedItems.Count == 0)
            {
                menuHandle.DisableAll();
            }
            else if (listHandles.SelectedItems.Count == 1)
            {
                menuHandle.EnableAll();

                propertiesHandleMenuItem.Enabled = false;

                string type = listHandles.SelectedItems[0].SubItems[0].Text;

                if (type == "Token" || type == "File" || type == "Event" || type == "Mutant" || type == "Section")
                    propertiesHandleMenuItem.Enabled = true;
            }
            else
            {
                menuHandle.EnableAll();
                propertiesHandleMenuItem.Enabled = false;
            }
        }

        private void closeHandleMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listHandles.SelectedItems)
            {
                try
                {
                    int handle = (int)BaseConverter.ToNumberParse(item.SubItems[2].Text);

                    using (Win32.ProcessHandle process =
                           new Win32.ProcessHandle(_pid, Win32.PROCESS_RIGHTS.PROCESS_DUP_HANDLE))
                    {
                        if (Win32.ZwDuplicateObject(process.Handle, handle, 0, 0, 0, 0,
                            0x1 // DUPLICATE_CLOSE_SOURCE
                            ) != 0)
                            Win32.ThrowLastWin32Error();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Could not close handle:\n\n" + ex.Message,
                         "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return;
                }
            }
        }

        private void propertiesHandleMenuItem_Click(object sender, EventArgs e)
        {
            string type = listHandles.SelectedItems[0].Text;

            try
            {
                int handle = int.Parse(listHandles.SelectedItems[0].Name);

                using (Win32.ProcessHandle phandle = new Win32.ProcessHandle(
                    _pid, Win32.PROCESS_RIGHTS.PROCESS_DUP_HANDLE))
                {
                    if (type == "Token")
                    {
                        TokenWindow tokForm = new TokenWindow(new Win32.RemoteTokenHandle(phandle, handle));

                        tokForm.Text = String.Format("Token - Handle 0x{0:x} owned by {1} (PID {2})",
                            short.Parse(listHandles.SelectedItems[0].Name),
                            Program.HackerWindow.ProcessProvider.Dictionary[_pid].Name,
                            _pid);
                        tokForm.ShowDialog();
                    }
                    else if (type == "File")
                    {
                        Win32.ShowProperties(listHandles.SelectedItems[0].SubItems[1].Text);
                    }
                    else if (type == "Event")
                    {
                        Win32.RemoteHandle rhandle = new Win32.RemoteHandle(phandle, handle);
                        int event_handle = rhandle.GetHandle((int)Win32.SYNC_RIGHTS.EVENT_QUERY_STATE);
                        Win32.EVENT_BASIC_INFORMATION ebi = new Win32.EVENT_BASIC_INFORMATION();
                        int retLen;

                        Win32.ZwQueryEvent(event_handle, Win32.EVENT_INFORMATION_CLASS.EventBasicInformation,
                            ref ebi, Marshal.SizeOf(ebi), out retLen);

                        InformationBox info = new InformationBox(
                            "Type: " + ebi.EventType.ToString().Replace("Event", "") +
                            "\r\nState: " + (ebi.EventState != 0 ? "True" : "False"));

                        info.ShowDialog();

                        Win32.CloseHandle(event_handle);
                    }
                    else if (type == "Mutant")
                    {
                        Win32.RemoteHandle rhandle = new Win32.RemoteHandle(phandle, handle);
                        int mutant_handle = rhandle.GetHandle((int)Win32.SYNC_RIGHTS.MUTEX_MODIFY_STATE);
                        Win32.MUTANT_BASIC_INFORMATION mbi = new Win32.MUTANT_BASIC_INFORMATION();
                        int retLen;

                        Win32.ZwQueryMutant(mutant_handle, Win32.MUTANT_INFORMATION_CLASS.MutantBasicInformation,
                            ref mbi, Marshal.SizeOf(mbi), out retLen);

                        InformationBox info = new InformationBox(
                            "Count: " + mbi.CurrentCount +
                            "\r\nOwned by Caller: " + (mbi.OwnedByCaller != 0 ? "True" : "False") +
                            "\r\nAbandoned: " + (mbi.AbandonedState != 0 ? "True" : "False"));

                        info.ShowDialog();

                        Win32.CloseHandle(mutant_handle);
                    }
                    else if (type == "Section")
                    {
                        Win32.RemoteHandle rhandle = new Win32.RemoteHandle(phandle, handle);
                        int section_handle = rhandle.GetHandle((int)Win32.SECTION_RIGHTS.SECTION_QUERY);
                        Win32.SECTION_BASIC_INFORMATION sbi = new Win32.SECTION_BASIC_INFORMATION();
                        Win32.SECTION_IMAGE_INFORMATION sii = new Win32.SECTION_IMAGE_INFORMATION();
                        int retLen;
                        int retVal;

                        Win32.ZwQuerySection(section_handle, Win32.SECTION_INFORMATION_CLASS.SectionBasicInformation,
                            ref sbi, Marshal.SizeOf(sbi), out retLen);
                        retVal = Win32.ZwQuerySection(section_handle, Win32.SECTION_INFORMATION_CLASS.SectionImageInformation,
                            ref sii, Marshal.SizeOf(sii), out retLen);

                        InformationBox info = new InformationBox(
                            "Attributes: " + Misc.FlagsToString(typeof(Win32.SECTION_ATTRIBUTES), (long)sbi.SectionAttributes) +
                            "\r\nSize: " + Misc.GetNiceSizeName(sbi.SectionSize) + " (" + sbi.SectionSize.ToString() + " B)" +

                            (retVal == 0 ? ("\r\n\r\nImage Entry Point: 0x" + sii.EntryPoint.ToString("x8") +
                            "\r\nImage Machine Type: " + ((PE.MachineType)sii.ImageMachineType).ToString() +
                            "\r\nImage Characteristics: " + ((PE.ImageCharacteristics)sii.ImageCharacteristics).ToString() +
                            "\r\nImage Subsystem: " + ((PE.ImageSubsystem)sii.ImageSubsystem).ToString() +
                            "\r\nStack Reserve: 0x" + sii.StackReserved.ToString("x")) : ""));

                        info.ShowDialog();

                        Win32.CloseHandle(section_handle);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
