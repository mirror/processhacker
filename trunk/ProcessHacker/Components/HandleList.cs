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

namespace ProcessHacker.Components
{
    public partial class HandleList : UserControl
    {
        private object _listLock = new object();
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
                propertiesHandleMenuItem_Click(null, null);
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
        }

        private void provider_DictionaryAdded(HandleItem item)
        {
            HighlightedListViewItem litem = new HighlightedListViewItem(_highlightingContext, this.Highlight);

            litem.Name = item.Handle.Handle.ToString();
            litem.Text = item.ObjectInfo.TypeName;
            litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem, item.ObjectInfo.BestName));
            litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem, "0x" + item.Handle.Handle.ToString("x")));

            if (Properties.Settings.Default.UseColorProtectedHandles && 
                (item.Handle.Flags & HandleFlags.ProtectFromClose) != 0
                )
                litem.NormalColor = Properties.Settings.Default.ColorProtectedHandles;
            if (Properties.Settings.Default.UseColorInheritHandles && 
                (item.Handle.Flags & HandleFlags.Inherit) != 0
                )
                litem.NormalColor = Properties.Settings.Default.ColorInheritHandles;

            listHandles.Items.Add(litem);
        }

        private void provider_DictionaryRemoved(HandleItem item)
        {
            lock (_listLock)
            {
                int index = listHandles.Items[item.Handle.Handle.ToString()].Index;
                bool selected = listHandles.Items[item.Handle.Handle.ToString()].Selected;
                int selectedCount = listHandles.SelectedItems.Count;

                listHandles.Items[item.Handle.Handle.ToString()].Remove();
            }
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

                if (HasHandleProperties(type))
                    propertiesHandleMenuItem.Enabled = true;
            }
            else
            {
                menuHandle.EnableAll();
                propertiesHandleMenuItem.Enabled = false;
            }
        }

        public static bool HasHandleProperties(string type)
        {
            if (type == "Token" || type == "Process" || type == "File" || 
                type == "Event" || type == "Mutant" || type == "Section")
                return true;
            else
                return false;
        }

        public static void ShowHandleProperties(int pid, string type, int handle, string name)
        {
            ProcessHandle phandle;

            try
            {
                phandle = new ProcessHandle(pid, ProcessHacker.Native.Security.ProcessAccess.DupHandle);
            }
            catch
            {
                phandle = new ProcessHandle(pid, Program.MinProcessGetHandleInformationRights);
            }

            using (phandle)
            {
                if (type == "Token")
                {
                    TokenWindow tokForm = new TokenWindow(new RemoteTokenHandle(phandle, handle));

                    tokForm.Text = String.Format("Token - Handle 0x{0:x} owned by {1} (PID {2})",
                        handle,
                        Program.ProcessProvider.Dictionary[pid].Name,
                        pid);
                    tokForm.ShowDialog();
                }
                else if (type == "Process")
                {
                    int processId;

                    if (KProcessHacker.Instance != null)
                    {
                        processId = KProcessHacker.Instance.KphGetProcessId(phandle, handle);
                    }
                    else
                    {
                        int newHandle = 0;

                        Win32.DuplicateObject(phandle, handle, -1, out newHandle, (int)Program.MinProcessQueryRights, 0, 0);
                        processId = Win32.GetProcessId(newHandle);
                        Win32.CloseHandle(newHandle);
                    }

                    if (!Program.ProcessProvider.Dictionary.ContainsKey(processId))
                    {
                        MessageBox.Show("The process does not exist.", "Process Hacker", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    ProcessWindow pForm = Program.GetProcessWindow(
                        Program.ProcessProvider.Dictionary[processId],
                        new Program.PWindowInvokeAction(delegate(ProcessWindow f)
                        {
                            Program.FocusWindow(f);
                        }));
                }
                else if (type == "File")
                {
                    FileUtils.ShowProperties(name);
                }
                else if (type == "Event")
                {
                    var eventHandle = new Win32Handle<EventAccess>(phandle, handle, EventAccess.All);
                    var ebi = new EventBasicInformation();
                    int retLen;

                    Win32.NtQueryEvent(eventHandle, EventInformationClass.EventBasicInformation,
                        ref ebi, Marshal.SizeOf(ebi), out retLen);

                    InformationBox info = new InformationBox(
                        "Type: " + ebi.EventType.ToString().Replace("Event", "") +
                        "\r\nState: " + (ebi.EventState != 0 ? "True" : "False"));

                    info.ShowDialog();
                    eventHandle.Dispose();
                }
                else if (type == "Mutant")
                {
                    var mutantHandle = new Win32Handle<MutexAccess>(phandle, handle, MutexAccess.All);
                    var mbi = new MutantBasicInformation();
                    int retLen;

                    Win32.NtQueryMutant(mutantHandle, MutantInformationClass.MutantBasicInformation,
                        ref mbi, Marshal.SizeOf(mbi), out retLen);

                    InformationBox info = new InformationBox(
                        "Count: " + mbi.CurrentCount +
                        "\r\nOwned by Caller: " + (mbi.OwnedByCaller != 0 ? "True" : "False") +
                        "\r\nAbandoned: " + (mbi.AbandonedState != 0 ? "True" : "False"));

                    info.ShowDialog();
                    mutantHandle.Dispose();
                }
                else if (type == "Section")
                {
                    var sectionHandle = new Win32Handle<SectionAccess>(phandle, handle, SectionAccess.Query);
                    var sbi = new SectionBasicInformation();
                    var sii = new SectionImageInformation();
                    int retLen;
                    int retVal;

                    Win32.NtQuerySection(sectionHandle, SectionInformationClass.SectionBasicInformation,
                        ref sbi, Marshal.SizeOf(sbi), out retLen);
                    retVal = Win32.NtQuerySection(sectionHandle, SectionInformationClass.SectionImageInformation,
                        ref sii, Marshal.SizeOf(sii), out retLen);

                    InformationBox info = new InformationBox(
                        "Attributes: " + Misc.FlagsToString(typeof(SectionAttributes), (long)sbi.SectionAttributes) +
                        "\r\nSize: " + Misc.GetNiceSizeName(sbi.SectionSize) + " (" + sbi.SectionSize.ToString() + " B)" +

                        (retVal == 0 ? ("\r\n\r\nImage Entry Point: 0x" + sii.EntryPoint.ToString("x8") +
                        "\r\nImage Machine Type: " + ((PE.MachineType)sii.ImageMachineType).ToString() +
                        "\r\nImage Characteristics: " + ((PE.ImageCharacteristics)sii.ImageCharacteristics).ToString() +
                        "\r\nImage Subsystem: " + ((PE.ImageSubsystem)sii.ImageSubsystem).ToString() +
                        "\r\nStack Reserve: 0x" + sii.StackReserved.ToString("x")) : ""));

                    info.ShowDialog();
                    sectionHandle.Dispose();
                }
            }
        }

        private void closeHandleMenuItem_Click(object sender, EventArgs e)
        {
            lock (_listLock)
            {
                foreach (ListViewItem item in listHandles.SelectedItems)
                {
                    try
                    {
                        int handle = (int)BaseConverter.ToNumberParse(item.SubItems[2].Text);

                        using (ProcessHandle process =
                               new ProcessHandle(_pid, Program.MinProcessGetHandleInformationRights))
                        {
                            Win32.DuplicateObject(process.Handle, handle, 0, 0, 0, 0,
                                0x1 // DUPLICATE_CLOSE_SOURCE
                                );
                        }
                    }
                    catch (Exception ex)
                    {
                        var result = MessageBox.Show(
                            "Could not close handle \"" + item.SubItems[1].Text + "\":\n\n" + ex.Message,
                             "Process Hacker", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);

                        if (result == DialogResult.Cancel)
                            return;
                    }
                }
            }
        }

        private void propertiesHandleMenuItem_Click(object sender, EventArgs e)
        {
            if (listHandles.SelectedItems.Count != 1)
                return;

            string type = listHandles.SelectedItems[0].Text;

            try
            {
                ShowHandleProperties(
                    _pid,
                    listHandles.SelectedItems[0].Text,
                    int.Parse(listHandles.SelectedItems[0].Name),
                    listHandles.SelectedItems[0].SubItems[1].Text
                    );
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
