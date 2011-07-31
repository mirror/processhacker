/*
 * Process Hacker - 
 *   module list
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
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.Win32;
using ProcessHacker.Common;
using ProcessHacker.Common.Ui;
using ProcessHacker.Native;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;
using ProcessHacker.UI;

namespace ProcessHacker.Components
{
    public partial class ModuleList : UserControl
    {
        private ModuleProvider _provider;
        private int _runCount = 0;
        private List<ListViewItem> _needsAdd = new List<ListViewItem>();
        private HighlightingContext _highlightingContext;
        public new event KeyEventHandler KeyDown;
        public new event MouseEventHandler MouseDown;
        public new event MouseEventHandler MouseUp;
        public new event EventHandler DoubleClick;
        private int _pid;
        private string _mainModule;

        public ModuleList()
        {
            InitializeComponent();

            _highlightingContext = new HighlightingContext(listModules);
            listModules.KeyDown += new KeyEventHandler(ModuleList_KeyDown);
            listModules.MouseDown += new MouseEventHandler(listModules_MouseDown);
            listModules.MouseUp += new MouseEventHandler(listModules_MouseUp);
            listModules.DoubleClick += new EventHandler(listModules_DoubleClick);

            ColumnSettings.LoadSettings(Settings.Instance.ModuleListViewColumns, listModules);
            listModules.ContextMenu = menuModule;
            GenericViewMenu.AddMenuItems(copyModuleMenuItem.MenuItems, listModules, null);
        }

        private void listModules_DoubleClick(object sender, EventArgs e)
        {
            if (this.DoubleClick != null)
                this.DoubleClick(sender, e);
        }

        private void listModules_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.MouseUp != null)
                this.MouseUp(sender, e);
        }

        private void listModules_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.MouseDown != null)
                this.MouseDown(sender, e);
        }

        private void ModuleList_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.KeyDown != null)
                this.KeyDown(sender, e);
        }

        #region Properties

        public new bool DoubleBuffered
        {
            get
            {
                return (bool)typeof(ListView).GetProperty("DoubleBuffered",
                    BindingFlags.NonPublic | BindingFlags.Instance).GetValue(listModules, null);
            }
            set
            {
                typeof(ListView).GetProperty("DoubleBuffered",
                    BindingFlags.NonPublic | BindingFlags.Instance).SetValue(listModules, value, null);
            }
        }

        public override bool Focused
        {
            get
            {
                return listModules.Focused;
            }
        }

        public override ContextMenu ContextMenu
        {
            get { return listModules.ContextMenu; }
            set { listModules.ContextMenu = value; }
        }

        public override ContextMenuStrip ContextMenuStrip
        {
            get { return listModules.ContextMenuStrip; }
            set { listModules.ContextMenuStrip = value; }
        }

        public ListView List
        {
            get { return listModules; }
        }

        public ModuleProvider Provider
        {
            get { return _provider; }
            set
            {
                if (_provider != null)
                {
                    _provider.DictionaryAdded -= new ModuleProvider.ProviderDictionaryAdded(provider_DictionaryAdded);
                    _provider.DictionaryRemoved -= new ModuleProvider.ProviderDictionaryRemoved(provider_DictionaryRemoved);
                    _provider.Updated -= new ModuleProvider.ProviderUpdateOnce(provider_Updated);
                }

                _provider = value;

                listModules.Items.Clear();
                listModules.ListViewItemSorter = null;
                _pid = -1;
                _mainModule = null;

                if (_provider != null)
                {
                    foreach (ModuleItem item in _provider.Dictionary.Values)
                    {
                        provider_DictionaryAdded(item);
                    }

                    _provider.DictionaryAdded += new ModuleProvider.ProviderDictionaryAdded(provider_DictionaryAdded);
                    _provider.DictionaryRemoved += new ModuleProvider.ProviderDictionaryRemoved(provider_DictionaryRemoved);
                    _provider.Updated += new ModuleProvider.ProviderUpdateOnce(provider_Updated);
                    _pid = _provider.Pid;

                    try
                    {
                        if (_pid == 4)
                        {
                            _mainModule = FileUtils.GetFileName(Windows.KernelFileName);
                        }
                        else
                        {
                            using (var phandle =
                                new ProcessHandle(_pid,
                                    Program.MinProcessQueryRights | Program.MinProcessReadMemoryRights))
                                _mainModule = FileUtils.GetFileName(phandle.GetMainModule().FileName);
                        }

                        this.SetMainModule(_mainModule);
                    }
                    catch
                    { }
                }
            }
        }

        #endregion

        #region Interfacing

        public void BeginUpdate()
        {
            listModules.BeginUpdate();
        }

        public void EndUpdate()
        {
            listModules.EndUpdate();
        }

        public ListView.ListViewItemCollection Items
        {
            get { return listModules.Items; }
        }

        public ListView.SelectedListViewItemCollection SelectedItems
        {
            get { return listModules.SelectedItems; }
        }

        #endregion

        private void SetMainModule(string mainModule)
        {
            _mainModule = mainModule;

            SortedListViewComparer comparer = (SortedListViewComparer)
                (listModules.ListViewItemSorter = new SortedListViewComparer(listModules)
                {
                    TriState = true,
                    TriStateComparer = new ModuleListComparer(_mainModule),
                    SortColumn = 0,
                    SortOrder = SortOrder.None
                });

            comparer.ColumnSortOrder.Add(0);
            comparer.ColumnSortOrder.Add(1);
            comparer.ColumnSortOrder.Add(2);

            (listModules.ListViewItemSorter as SortedListViewComparer).CustomSorters.Add(2,
                (x, y) =>
                {
                    ModuleItem ix = (ModuleItem)x.Tag;
                    ModuleItem iy = (ModuleItem)y.Tag;

                    return ix.Size.CompareTo(iy.Size);
                });
        }

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
                            listModules.Items.AddRange(_needsAdd.ToArray());
                            _needsAdd.Clear();
                        }
                    }));
                }
            }

            _highlightingContext.Tick();
            _runCount++;
        }

        private Color GetModuleColor(ModuleItem item)
        {
            if (Settings.Instance.UseColorDotNetProcesses &&
                (item.Flags & LdrpDataTableEntryFlags.CorImage) != 0
                )
                return Settings.Instance.ColorDotNetProcesses;
            else if (Settings.Instance.UseColorRelocatedDlls &&
                (item.Flags & LdrpDataTableEntryFlags.ImageNotAtBase) != 0
                )
                return Settings.Instance.ColorRelocatedDlls;
            else
                return SystemColors.Window;
        }

        public void AddItem(ModuleItem item)
        {
            provider_DictionaryAdded(item);
        }

        public void UpdateItems()
        {
            provider_Updated();
        }

        public void DumpSetMainModule(string mainModule)
        {
            this.SetMainModule(mainModule);
        }

        private void provider_DictionaryAdded(ModuleItem item)
        {          
            HighlightedListViewItem litem = new HighlightedListViewItem(_highlightingContext,
                item.RunId > 0 && _runCount > 0);

            litem.Name = item.BaseAddress.ToString();
            litem.Text = item.Name;
            litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem, Utils.FormatAddress(item.BaseAddress)));
            litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem, Utils.FormatSize(item.Size)));
            litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem, item.FileDescription));
            litem.ToolTipText = PhUtils.FormatFileInfo(
                item.FileName, item.FileDescription, item.FileCompanyName, item.FileVersion, 0);
            litem.Tag = item;
            litem.NormalColor = this.GetModuleColor(item);

            if (item.FileName.Equals(_mainModule, StringComparison.OrdinalIgnoreCase))
                litem.Font = new System.Drawing.Font(litem.Font, System.Drawing.FontStyle.Bold);

            lock (_needsAdd)
                _needsAdd.Add(litem);
        }

        private void provider_DictionaryRemoved(ModuleItem item)
        {
            this.BeginInvoke(new MethodInvoker(() =>
            {
                listModules.Items[item.BaseAddress.ToString()].Remove();
            }));
        }

        public void SaveSettings()
        {
            Settings.Instance.ModuleListViewColumns = ColumnSettings.SaveSettings(listModules);
        }

        private string GetItemFileName(ListViewItem litem)
        {
            return ((ModuleItem)litem.Tag).FileName;
        }

        private void menuModule_Popup(object sender, EventArgs e)
        {
            if (listModules.SelectedItems.Count == 1)
            {
                if (_pid == 4)
                {
                    menuModule.DisableAll();

                    if (KProcessHacker.Instance != null)
                        unloadMenuItem.Enabled = true;

                    inspectModuleMenuItem.Enabled = true;
                    searchModuleMenuItem.Enabled = true;
                    copyFileNameMenuItem.Enabled = true;
                    copyModuleMenuItem.Enabled = true;
                    openContainingFolderMenuItem.Enabled = true;
                    propertiesMenuItem.Enabled = true;
                }
                else
                {
                    menuModule.EnableAll();
                }
            }
            else
            {
                menuModule.DisableAll();

                if (listModules.SelectedItems.Count > 1)
                {
                    copyFileNameMenuItem.Enabled = true;
                    copyModuleMenuItem.Enabled = true;
                }
            }

            if (listModules.Items.Count > 0)
            {
                selectAllModuleMenuItem.Enabled = true;
            }
            else
            {
                selectAllModuleMenuItem.Enabled = false;
            }
        }

        private void searchModuleMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(Settings.Instance.SearchEngine.Replace("%s",
                    listModules.SelectedItems[0].Text));
            }
            catch (Exception ex)
            {
                PhUtils.ShowException("Unable to search for the module", ex);
            }
        }

        private void copyFileNameMenuItem_Click(object sender, EventArgs e)
        {
            string text = "";

            for (int i = 0; i < listModules.SelectedItems.Count; i++)
            {
                text += this.GetItemFileName(listModules.SelectedItems[i]);

                if (i != listModules.SelectedItems.Count - 1)
                    text += "\r\n";
            }

            Clipboard.SetText(text);
        }

        private void openContainingFolderMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Utils.ShowFileInExplorer(this.GetItemFileName(listModules.SelectedItems[0]));
            }
            catch (Exception ex)
            {
                PhUtils.ShowException("Unable to show the file", ex);
            }
        }

        private void propertiesMenuItem_Click(object sender, EventArgs e)
        {
            FileUtils.ShowProperties(this.GetItemFileName(listModules.SelectedItems[0]));
        }

        private void inspectModuleMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                PEWindow pw = Program.GetPEWindow(this.GetItemFileName(listModules.SelectedItems[0]),
                    new Program.PEWindowInvokeAction(delegate(PEWindow f)
                    {
                        if (!f.IsDisposed)
                        {
                            try
                            {
                                f.Show();
                                f.Activate();
                            }
                            catch (Exception ex)
                            {
                                Logging.Log(ex);
                            }
                        }
                    }));
            }
            catch (Exception ex)
            {
                PhUtils.ShowException("Unable to inspect the module", ex);
            }
        }

        private void getFuncAddressMenuItem_Click(object sender, EventArgs e)
        {
            GetProcAddressWindow gpaWindow = new GetProcAddressWindow(
                this.GetItemFileName(listModules.SelectedItems[0]));

            gpaWindow.ShowDialog();
        }

        private void changeMemoryProtectionModuleMenuItem_Click(object sender, EventArgs e)
        {
            ModuleItem item = (ModuleItem)listModules.SelectedItems[0].Tag;
            VirtualProtectWindow w = new VirtualProtectWindow(_pid, item.BaseAddress.ToIntPtr(), item.Size);

            w.ShowDialog();
        }

        private void readMemoryModuleMenuItem_Click(object sender, EventArgs e)
        {
            ModuleItem item = (ModuleItem)listModules.SelectedItems[0].Tag;

            MemoryEditor.ReadWriteMemory(_pid, item.BaseAddress.ToIntPtr(), item.Size, true);
        }

        private void selectAllModuleMenuItem_Click(object sender, EventArgs e)
        {
            Utils.SelectAll(listModules.Items);
        }

        private void unloadMenuItem_Click(object sender, EventArgs e)
        {
            if (!PhUtils.ShowConfirmMessage(
                "Unload",
                _pid != 4 ? "the selected module" : "the selected driver",
                _pid != 4 ?
                "Unloading a module may cause the process to crash." :
                "Unloading a driver may cause system instability.",
                true
                ))
                return;

            if (_pid == 4)
            {
                try
                {
                    var moduleItem = (ModuleItem)listModules.SelectedItems[0].Tag;
                    string serviceName = null;

                    // Try to find the name of the service key for the driver by 
                    // looping through the objects in the Driver directory and 
                    // opening each one.
                    using (var dhandle = new DirectoryHandle("\\Driver", DirectoryAccess.Query))
                    {
                        foreach (var obj in dhandle.GetObjects())
                        {
                            try
                            {
                                using (var driverHandle = new DriverHandle("\\Driver\\" + obj.Name))
                                {
                                    if (driverHandle.GetBasicInformation().DriverStart == moduleItem.BaseAddress.ToIntPtr())
                                    {
                                        serviceName = driverHandle.GetServiceKeyName();
                                        break;
                                    }
                                }
                            }
                            catch
                            { }
                        }
                    }

                    // If we didn't find the service name, use the driver base name.
                    if (serviceName == null)
                    {
                        if (moduleItem.Name.EndsWith(".sys", StringComparison.OrdinalIgnoreCase))
                            serviceName = moduleItem.Name.Remove(moduleItem.Name.Length - 4, 4);
                        else
                            serviceName = moduleItem.Name;
                    }

                    RegistryKey servicesKey =
                        Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services", true);
                    bool serviceKeyCreated;
                    RegistryKey serviceKey;

                    // Check if the service key exists so that we don't delete it 
                    // later if it does.
                    if (Array.Exists<string>(servicesKey.GetSubKeyNames(),
                        (keyName) => (string.Compare(keyName, serviceName, true) == 0)))
                    {
                        serviceKeyCreated = false;
                    }
                    else
                    {
                        serviceKeyCreated = true;
                        // Create the service key.
                        serviceKey = servicesKey.CreateSubKey(serviceName);

                        serviceKey.SetValue("ErrorControl", 1, RegistryValueKind.DWord);
                        serviceKey.SetValue("ImagePath", "\\??\\" + moduleItem.FileName, RegistryValueKind.ExpandString);
                        serviceKey.SetValue("Start", 1, RegistryValueKind.DWord);
                        serviceKey.SetValue("Type", 1, RegistryValueKind.DWord);
                        serviceKey.Close();
                        servicesKey.Flush();
                    }

                    try
                    {
                        Windows.UnloadDriver(serviceName);
                    }
                    finally
                    {
                        if (serviceKeyCreated)
                            servicesKey.DeleteSubKeyTree(serviceName);

                        servicesKey.Close();
                    }

                    listModules.SelectedItems.Clear();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to unload the driver. Make sure Process Hacker " +
                        "is running with administrative privileges. Error:\n\n" +
                        ex.Message, "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                try
                {
                    using (ProcessHandle phandle = new ProcessHandle(_pid,
                        Program.MinProcessQueryRights | ProcessAccess.VmOperation |
                        ProcessAccess.VmRead | ProcessAccess.VmWrite | ProcessAccess.CreateThread))
                    {
                        IntPtr baseAddress = ((ModuleItem)listModules.SelectedItems[0].Tag).BaseAddress.ToIntPtr();

                        phandle.SetModuleReferenceCount(baseAddress, 1);

                        ThreadHandle thread;

                        if (OSVersion.IsAboveOrEqual(WindowsVersion.Vista))
                        {
                            // Use RtlCreateUserThread to bypass session boundaries. Since 
                            // LdrUnloadDll is a native function we don't need to notify CSR.
                            thread = phandle.CreateThread(
                                Loader.GetProcedure("ntdll.dll", "LdrUnloadDll"),
                                baseAddress
                                );
                        }
                        else
                        {
                            // On XP it seems we need to notify CSR...
                            thread = phandle.CreateThreadWin32(
                                Loader.GetProcedure("kernel32.dll", "FreeLibrary"),
                                baseAddress
                                );
                        }

                        thread.Wait(1000 * Win32.TimeMsTo100Ns);

                        NtStatus exitStatus = thread.GetExitStatus();

                        if (exitStatus == NtStatus.DllNotFound)
                        {
                            if (OSVersion.Architecture == OSArch.Amd64)
                            {
                                PhUtils.ShowError("Unable to find the module to unload. This may be caused " +
                                    "by an attempt to unload a mapped file or a 32-bit module.");
                            }
                            else
                            {
                                PhUtils.ShowError("Unable to find the module to unload. This may be caused " +
                                    "by an attempt to unload a mapped file.");
                            }
                        }
                        else
                        {
                            exitStatus.ThrowIf();
                        }

                        thread.Dispose();
                    }

                    listModules.SelectedItems.Clear();
                }
                catch (Exception ex)
                {
                    PhUtils.ShowException("Unable to unload the module", ex);
                }
            }
        }
    }

    public class ModuleListComparer : ISortedListViewComparer
    {
        private string _mainModule;

        public ModuleListComparer(string mainModule)
        {
            _mainModule = mainModule;
        }

        public int Compare(ListViewItem x, ListViewItem y, int column)
        {
            ModuleItem mx = (ModuleItem)x.Tag;
            ModuleItem my = (ModuleItem)y.Tag;

            if (mx.FileName.Equals(_mainModule, StringComparison.OrdinalIgnoreCase))
                return -1;
            if (my.FileName.Equals(_mainModule, StringComparison.OrdinalIgnoreCase))
                return 1;

            return 0;
        }
    }
}
