/*
 * Process Hacker - 
 *   TcpUdp list
 * 
 * Copyright (C) 2009 Dean
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
    public partial class TcpUdpList : UserControl
    {
        TcpUdpProvider _provider;
        public new event KeyEventHandler KeyDown;
        public new event MouseEventHandler MouseDown;
        public new event MouseEventHandler MouseUp;
        public new event EventHandler DoubleClick;
        public event EventHandler SelectedIndexChanged;

        public TcpUdpList()
        {
            InitializeComponent();

            listTcpUdp.KeyDown += new KeyEventHandler(list_KeyDown);
            listTcpUdp.MouseDown += new MouseEventHandler(list_MouseDown);
            listTcpUdp.MouseUp += new MouseEventHandler(list_MouseUp);
            listTcpUdp.DoubleClick += new EventHandler(list_DoubleClick);
            listTcpUdp.SelectedIndexChanged += new System.EventHandler(list_SelectedIndexChanged);
            //GenericViewMenu.AddMenuItems(copyHandleMenuItem.MenuItems, listTcpUdp, null);
            ColumnSettings.LoadSettings(Properties.Settings.Default.TcpUdpListViewColumns, listTcpUdp);
        }

        private void list_DoubleClick(object sender, EventArgs e)
        {
            if (this.DoubleClick != null)
                this.DoubleClick(sender, e);
        }
        private void list_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.MouseUp != null)
                this.MouseUp(sender, e);
        }
        private void list_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.MouseDown != null)
                this.MouseDown(sender, e);
        }
        private void list_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (this.SelectedIndexChanged != null)
                this.SelectedIndexChanged(sender, e);
        }
        private void list_KeyDown(object sender, KeyEventArgs e)
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
                    BindingFlags.NonPublic | BindingFlags.Instance).GetValue(listTcpUdp, null);
            }
            set
            {
                typeof(ListView).GetProperty("DoubleBuffered",
                    BindingFlags.NonPublic | BindingFlags.Instance).SetValue(listTcpUdp, value, null);
            }
        }

        public override bool Focused
        {
            get
            {
                return listTcpUdp.Focused;
            }
        }

        public override ContextMenu ContextMenu
        {
            get { return listTcpUdp.ContextMenu; }
            set { listTcpUdp.ContextMenu = value; }
        }

        public override ContextMenuStrip ContextMenuStrip
        {
            get { return listTcpUdp.ContextMenuStrip; }
            set { listTcpUdp.ContextMenuStrip = value; }
        }

        public ListView List
        {
            get { return listTcpUdp; }
        }

        public TcpUdpProvider Provider
        {
            get { return _provider; }
            set
            {
                if (_provider != null)
                {
                    _provider.DictionaryAdded -= new TcpUdpProvider.ProviderDictionaryAdded(provider_DictionaryAdded);
                    _provider.DictionaryRemoved -= new TcpUdpProvider.ProviderDictionaryRemoved(provider_DictionaryRemoved);
                }

                _provider = value;

                listTcpUdp.Items.Clear();
                _pid = -1;

                if (_provider != null)
                {
                    foreach (TcpUdpItem item in _provider.Dictionary.Values)
                    {
                        provider_DictionaryAdded(item);
                    }
                    _provider.UseInvoke = true;
                    _provider.Invoke = new TcpUdpProvider.ProviderInvokeMethod(this.BeginInvoke);
                    _provider.DictionaryAdded += new TcpUdpProvider.ProviderDictionaryAdded(provider_DictionaryAdded);
                    _provider.DictionaryRemoved += new TcpUdpProvider.ProviderDictionaryRemoved(provider_DictionaryRemoved);
                    _pid = _provider.PID;
                }
            }
        }

        #endregion

        #region Core Handle List

        private void provider_DictionaryAdded(TcpUdpItem item)
        {
            HighlightedListViewItem litem = new HighlightedListViewItem(this.Highlight);

            litem.Name = item.LocalAddress+item.RemoteAddress;
            litem.Text = item.Protocol.ToString();
            litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem, item.LocalAddress));
            litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem, item.RemoteAddress));
            litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem, item.State));
            listTcpUdp.Items.Add(litem);
        }

        private void provider_DictionaryRemoved(TcpUdpItem item)
        {
            int index = listTcpUdp.Items[item.LocalAddress + item.RemoteAddress].Index;
            bool selected = listTcpUdp.Items[item.LocalAddress + item.RemoteAddress].Selected;
            int selectedCount = listTcpUdp.SelectedItems.Count;

            listTcpUdp.Items[item.LocalAddress + item.RemoteAddress].Remove();

            if (selected && selectedCount == 1)
            {
                if (listTcpUdp.Items.Count == 0)
                { }
                else if (index > (listTcpUdp.Items.Count - 1))
                {
                    listTcpUdp.Items[listTcpUdp.Items.Count - 1].Selected = true;
                }
                else
                {
                    listTcpUdp.Items[index].Selected = true;
                }
            }
        }

        #endregion

        #region Interfacing

        public void BeginUpdate()
        {
            listTcpUdp.BeginUpdate();
        }

        public void EndUpdate()
        {
            listTcpUdp.EndUpdate();
        }

        public ListView.ListViewItemCollection Items
        {
            get { return listTcpUdp.Items; }
        }

        public ListView.SelectedListViewItemCollection SelectedItems
        {
            get { return listTcpUdp.SelectedItems; }
        }

        #endregion

        private int _pid;

        public void SaveSettings()
        {
            Properties.Settings.Default.TcpUdpListViewColumns = ColumnSettings.SaveSettings(listTcpUdp);
        }

        //private void menuHandle_Popup(object sender, EventArgs e)
        //{
        //    if (listTcpUdp.SelectedItems.Count == 0)
        //    {
        //        Misc.DisableAllMenuItems(menuHandle);
        //    }
        //    else if (listTcpUdp.SelectedItems.Count == 1)
        //    {
        //        Misc.EnableAllMenuItems(menuHandle);

        //        propertiesHandleMenuItem.Enabled = false;

        //        string type = listTcpUdp.SelectedItems[0].SubItems[0].Text;

        //        if (type == "Token" || type == "Event" || type == "Mutant" || type == "Section")
        //            propertiesHandleMenuItem.Enabled = true;
        //    }
        //    else
        //    {
        //        Misc.EnableAllMenuItems(menuHandle);
        //        closeHandleMenuItem.Enabled = false;
        //    }
        //}

        //private void closeHandleMenuItem_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        int handle = (int)BaseConverter.ToNumberParse(listTcpUdp.SelectedItems[0].SubItems[2].Text);

        //        using (Win32.ProcessHandle process =
        //               new Win32.ProcessHandle(_pid, Win32.PROCESS_RIGHTS.PROCESS_DUP_HANDLE))
        //        {
        //            if (Win32.ZwDuplicateObject(process.Handle, handle, 0, 0, 0, 0,
        //                0x1 // DUPLICATE_CLOSE_SOURCE
        //                ) != 0)
        //                Win32.ThrowLastWin32Error();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Could not close handle:\n\n" + ex.Message,
        //             "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);

        //        return;
        //    }
        //}

        //private void propertiesHandleMenuItem_Click(object sender, EventArgs e)
        //{
        //    string type = listTcpUdp.SelectedItems[0].Text;

        //    try
        //    {
        //        int handle = int.Parse(listTcpUdp.SelectedItems[0].Name);

        //        using (Win32.ProcessHandle phandle = new Win32.ProcessHandle(
        //            _pid, Win32.PROCESS_RIGHTS.PROCESS_DUP_HANDLE))
        //        {
        //            if (type == "Token")
        //            {
        //                TokenWindow tokForm = new TokenWindow(new Win32.RemoteTokenHandle(phandle, handle));

        //                tokForm.Text = String.Format("Token - Handle 0x{0:x} owned by {1} (PID {2})",
        //                    short.Parse(listTcpUdp.SelectedItems[0].Name),
        //                    Program.HackerWindow.ProcessProvider.Dictionary[_pid].Name,
        //                    _pid);
        //                tokForm.ShowDialog();
        //            }
        //            else if (type == "Event")
        //            {
        //                Win32.RemoteHandle rhandle = new Win32.RemoteHandle(phandle, handle);
        //                int event_handle = rhandle.GetHandle((int)Win32.SYNC_RIGHTS.EVENT_QUERY_STATE);
        //                Win32.EVENT_BASIC_INFORMATION ebi = new Win32.EVENT_BASIC_INFORMATION();
        //                int retLen;

        //                Win32.ZwQueryEvent(event_handle, Win32.EVENT_INFORMATION_CLASS.EventBasicInformation,
        //                    ref ebi, Marshal.SizeOf(ebi), out retLen);

        //                InformationBox info = new InformationBox(
        //                    "Type: " + ebi.EventType.ToString().Replace("Event", "") +
        //                    "\r\nState: " + (ebi.EventState != 0 ? "True" : "False"));

        //                info.ShowDialog();

        //                Win32.CloseHandle(event_handle);
        //            }
        //            else if (type == "Mutant")
        //            {
        //                Win32.RemoteHandle rhandle = new Win32.RemoteHandle(phandle, handle);
        //                int mutant_handle = rhandle.GetHandle((int)Win32.SYNC_RIGHTS.MUTEX_MODIFY_STATE);
        //                Win32.MUTANT_BASIC_INFORMATION mbi = new Win32.MUTANT_BASIC_INFORMATION();
        //                int retLen;

        //                Win32.ZwQueryMutant(mutant_handle, Win32.MUTANT_INFORMATION_CLASS.MutantBasicInformation,
        //                    ref mbi, Marshal.SizeOf(mbi), out retLen);

        //                InformationBox info = new InformationBox(
        //                    "Count: " + mbi.CurrentCount +
        //                    "\r\nOwned by Caller: " + (mbi.OwnedByCaller != 0 ? "True" : "False") +
        //                    "\r\nAbandoned: " + (mbi.AbandonedState != 0 ? "True" : "False"));

        //                info.ShowDialog();

        //                Win32.CloseHandle(mutant_handle);
        //            }
        //            else if (type == "Section")
        //            {
        //                Win32.RemoteHandle rhandle = new Win32.RemoteHandle(phandle, handle);
        //                int section_handle = rhandle.GetHandle((int)Win32.SECTION_RIGHTS.SECTION_QUERY);
        //                Win32.SECTION_BASIC_INFORMATION sbi = new Win32.SECTION_BASIC_INFORMATION();
        //                Win32.SECTION_IMAGE_INFORMATION sii = new Win32.SECTION_IMAGE_INFORMATION();
        //                int retLen;
        //                int retVal;

        //                Win32.ZwQuerySection(section_handle, Win32.SECTION_INFORMATION_CLASS.SectionBasicInformation,
        //                    ref sbi, Marshal.SizeOf(sbi), out retLen);
        //                retVal = Win32.ZwQuerySection(section_handle, Win32.SECTION_INFORMATION_CLASS.SectionImageInformation,
        //                    ref sii, Marshal.SizeOf(sii), out retLen);

        //                InformationBox info = new InformationBox(
        //                    "Attributes: " + Misc.FlagsToString(typeof(Win32.SECTION_ATTRIBUTES), (long)sbi.SectionAttributes) +
        //                    "\r\nSize: " + Misc.GetNiceSizeName(sbi.SectionSize) + " (" + sbi.SectionSize.ToString() + " B)" +

        //                    (retVal == 0 ? ("\r\n\r\nImage Entry Point: 0x" + sii.EntryPoint.ToString("x8") +
        //                    "\r\nImage Machine Type: " + ((PE.MachineType)sii.ImageMachineType).ToString() +
        //                    "\r\nImage Characteristics: " + ((PE.ImageCharacteristics)sii.ImageCharacteristics).ToString() +
        //                    "\r\nImage Subsystem: " + ((PE.ImageSubsystem)sii.ImageSubsystem).ToString() +
        //                    "\r\nStack Reserve: 0x" + sii.StackReserved.ToString("x")) : ""));

        //                info.ShowDialog();

        //                Win32.CloseHandle(section_handle);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}
    }
}
