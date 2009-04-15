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
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using ProcessHacker.UI;

namespace ProcessHacker.Components
{
    public partial class ModuleList : UserControl
    {
        private ModuleProvider _provider;
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

            ColumnSettings.LoadSettings(Properties.Settings.Default.ModuleListViewColumns, listModules);
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

        public bool Highlight { get; set; }

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

                    _provider.UseInvoke = true;
                    _provider.Invoke = new ModuleProvider.ProviderInvokeMethod(this.BeginInvoke);
                    _provider.DictionaryAdded += new ModuleProvider.ProviderDictionaryAdded(provider_DictionaryAdded);
                    _provider.DictionaryRemoved += new ModuleProvider.ProviderDictionaryRemoved(provider_DictionaryRemoved);
                    _provider.Updated += new ModuleProvider.ProviderUpdateOnce(provider_Updated);
                    _pid = _provider.PID;

                    try
                    {
                        if (_pid == 4)
                        {
                            _mainModule = Misc.GetRealPath(Misc.GetKernelFileName());
                        }
                        else
                        {
                            using (var phandle =
                                new Win32.ProcessHandle(_pid, 
                                    Program.MinProcessQueryRights | Program.MinProcessReadMemoryRights))
                                _mainModule = Misc.GetRealPath(phandle.GetMainModule().FileName);
                        }

                        _mainModule = _mainModule.ToLower();
                        listModules.ListViewItemSorter = new SortedListComparer(listModules)
                            {
                                TriState = true,
                                TriStateComparer = new ModuleListComparer(_mainModule),
                                SortColumn = 0,
                                SortOrder = SortOrder.None
                            };

                        (listModules.ListViewItemSorter as SortedListComparer).CustomSorters.Add(2,
                            (x, y) =>
                            {
                                ModuleItem ix = (ModuleItem)x.Tag;
                                ModuleItem iy = (ModuleItem)y.Tag;

                                return ix.Size.CompareTo(iy.Size);
                            });
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

        private void provider_Updated()
        {
            _highlightingContext.Tick();
        }

        private void provider_DictionaryAdded(ModuleItem item)
        {
            HighlightedListViewItem litem = new HighlightedListViewItem(_highlightingContext, this.Highlight);

            litem.Name = item.BaseAddress.ToString();
            litem.Text = item.Name;
            litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem, "0x" + item.BaseAddress.ToString("x8")));
            litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem, _pid != 4 ? Misc.GetNiceSizeName(item.Size) : ""));
            litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem, item.FileDescription));
            litem.ToolTipText = item.FileName;
            litem.Tag = item;

            if (item.FileName.Equals(_mainModule, StringComparison.InvariantCultureIgnoreCase))
                litem.Font = new System.Drawing.Font(litem.Font, System.Drawing.FontStyle.Bold);

            listModules.Items.Add(litem);
        }

        private void provider_DictionaryRemoved(ModuleItem item)
        {
            listModules.Items[item.BaseAddress.ToString()].Remove();
        }

        public void SaveSettings()
        {
            Properties.Settings.Default.ModuleListViewColumns = ColumnSettings.SaveSettings(listModules);
        }

        private void menuModule_Popup(object sender, EventArgs e)
        {
            if (listModules.SelectedItems.Count == 1)
            {
                if (_pid == 4)
                {
                    menuModule.DisableAll();

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
                Process.Start(Properties.Settings.Default.SearchEngine.Replace("%s",
                    listModules.SelectedItems[0].Text));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void copyFileNameMenuItem_Click(object sender, EventArgs e)
        {
            string text = "";

            for (int i = 0; i < listModules.SelectedItems.Count; i++)
            {
                text += listModules.SelectedItems[i].ToolTipText;

                if (i != listModules.SelectedItems.Count - 1)
                    text += "\r\n";
            }

            Clipboard.SetText(text);
        }

        private void openContainingFolderMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("explorer.exe", "/select," + listModules.SelectedItems[0].ToolTipText);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not start process:\n\n" + ex.Message, "Process Hacker",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void propertiesMenuItem_Click(object sender, EventArgs e)
        {
            Win32.ShowProperties(listModules.SelectedItems[0].ToolTipText);
        }

        private void inspectModuleMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                PEWindow pw = Program.GetPEWindow(listModules.SelectedItems[0].ToolTipText,
                    new Program.PEWindowInvokeAction(delegate(PEWindow f)
                    {
                        try
                        {
                            f.Show();
                            f.Activate();
                        }
                        catch
                        { }
                    }));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inspecting:\n\n" + ex.Message, "Process Hacker", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void getFuncAddressMenuItem_Click(object sender, EventArgs e)
        {
            GetProcAddressWindow gpaWindow = new GetProcAddressWindow(listModules.SelectedItems[0].ToolTipText);

            gpaWindow.ShowDialog();
        }

        private void changeMemoryProtectionModuleMenuItem_Click(object sender, EventArgs e)
        {
            ModuleItem item = (ModuleItem)listModules.SelectedItems[0].Tag;
            VirtualProtectWindow w = new VirtualProtectWindow(_pid, item.BaseAddress, item.Size);

            w.ShowDialog();
        }

        private void readMemoryModuleMenuItem_Click(object sender, EventArgs e)
        {
            ModuleItem item = (ModuleItem)listModules.SelectedItems[0].Tag;

            MemoryEditor.ReadWriteMemory(_pid, item.BaseAddress, item.Size, true);
        }

        private void selectAllModuleMenuItem_Click(object sender, EventArgs e)
        {
            Misc.SelectAll(listModules.Items);
        }

        private void unloadMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to unload this library?", "Process Hacker",
                MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.No)
                return;

            try
            {
                int kernel32 = Win32.GetModuleHandle("kernel32.dll");
                int freeLibrary = Win32.GetProcAddress(kernel32, "FreeLibrary");

                if (freeLibrary == 0)
                    throw new Exception("Could not find the entry point of FreeLibrary in kernel32.dll!");

                using (Win32.ProcessHandle phandle = new Win32.ProcessHandle(_pid, 
                    Program.MinProcessQueryRights | Program.MinProcessReadMemoryRights | 
                    Program.MinProcessWriteMemoryRights | Win32.PROCESS_RIGHTS.PROCESS_CREATE_THREAD))
                {
                    int baseAddress = ((ModuleItem)listModules.SelectedItems[0].Tag).BaseAddress;

                    phandle.SetModuleReferenceCount(baseAddress, 1);

                    var thread = phandle.CreateThread(freeLibrary, baseAddress, Win32.THREAD_RIGHTS.THREAD_ALL_ACCESS);

                    thread.Wait(1000);

                    int exitCode = thread.GetExitCode();

                    if (exitCode == 0)
                    {
                        throw new Exception("Error unloading the library.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    public class ModuleListComparer : System.Collections.IComparer
    {
        private string _mainModule;

        public ModuleListComparer(string mainModule)
        {
            _mainModule = mainModule.ToLower();
        }

        public int Compare(object x, object y)
        {
            ListViewItem lx = x as ListViewItem;
            ListViewItem ly = y as ListViewItem;
            ModuleItem mx = (ModuleItem)lx.Tag;
            ModuleItem my = (ModuleItem)ly.Tag;

            if (mx.FileName.Equals(_mainModule, StringComparison.InvariantCultureIgnoreCase))
                return -1;
            if (my.FileName.Equals(_mainModule, StringComparison.InvariantCultureIgnoreCase))
                return 1;

            return mx.Name.CompareTo(my.Name);
        }
    }
}
