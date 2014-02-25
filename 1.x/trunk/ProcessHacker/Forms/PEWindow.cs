/*
 * Process Hacker - 
 *   PE window
 * 
 * Copyright (C) 2008 wj32
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
using System.Windows.Forms;
using ProcessHacker.Common;
using ProcessHacker.Components;
using ProcessHacker.Native;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Image;
using ProcessHacker.UI;

namespace ProcessHacker
{
    public partial class PEWindow : Form
    {
        private string _path;
        private MappedImage _mappedImage;

        public PEWindow(string path)
        {
            InitializeComponent();
            this.AddEscapeToClose();
            this.SetTopMost();

            _path = path;
            this.Text = "PE File - " + path;
            Program.PEWindows.Add(Id, this);

            this.InitializeLists();

            try
            {
                _mappedImage = new MappedImage(path);
                this.Read();
            }
            catch (Exception ex)
            {
                PhUtils.ShowException("Unable to load the specified file", ex);

                this.Close();
            }
        }

        private void PEWindow_Load(object sender, EventArgs e)
        {
            this.Size = Settings.Instance.PEWindowSize;

            this.SetPhParent();
        }

        private void PEWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Visible = false;

            Settings.Instance.PECOFFHColumns = ColumnSettings.SaveSettings(listCOFFHeader);
            Settings.Instance.PECOFFOHColumns = ColumnSettings.SaveSettings(listCOFFOptionalHeader);
            Settings.Instance.PEImageDataColumns = ColumnSettings.SaveSettings(listImageData);
            Settings.Instance.PESectionsColumns = ColumnSettings.SaveSettings(listSections);
            Settings.Instance.PEExportsColumns = ColumnSettings.SaveSettings(listExports);
            Settings.Instance.PEImportsColumns = ColumnSettings.SaveSettings(listImports);
            Settings.Instance.PEWindowSize = this.Size;

            if (_mappedImage != null)
                _mappedImage.Dispose();
        }

        private void InitializeLists()
        {
            listCOFFHeader.SetDoubleBuffered(true);
            listCOFFHeader.SetTheme("explorer");
            listCOFFHeader.ContextMenu = listCOFFHeader.GetCopyMenu();
            listCOFFHeader.AddShortcuts();
            ColumnSettings.LoadSettings(Settings.Instance.PECOFFHColumns, listCOFFHeader);

            listCOFFOptionalHeader.SetDoubleBuffered(true);
            listCOFFOptionalHeader.SetTheme("explorer");
            listCOFFOptionalHeader.ContextMenu = listCOFFOptionalHeader.GetCopyMenu();
            listCOFFOptionalHeader.AddShortcuts();
            ColumnSettings.LoadSettings(Settings.Instance.PECOFFOHColumns, listCOFFOptionalHeader);

            listImageData.SetDoubleBuffered(true);
            listImageData.SetTheme("explorer");
            listImageData.ContextMenu = listImageData.GetCopyMenu();
            listImageData.AddShortcuts();
            ColumnSettings.LoadSettings(Settings.Instance.PEImageDataColumns, listImageData);

            listSections.SetDoubleBuffered(true);
            listSections.SetTheme("explorer");
            listSections.ContextMenu = listSections.GetCopyMenu();
            listSections.AddShortcuts();
            ColumnSettings.LoadSettings(Settings.Instance.PESectionsColumns, listSections);

            listExports.SetDoubleBuffered(true);
            listExports.SetTheme("explorer");
            listExports.ContextMenu = listExports.GetCopyMenu(listExports_RetrieveVirtualItem);
            listExports.AddShortcuts(this.listExports_RetrieveVirtualItem);
            ColumnSettings.LoadSettings(Settings.Instance.PEExportsColumns, listExports);

            listImports.SetDoubleBuffered(true);
            listImports.SetTheme("explorer");
            listImports.ContextMenu = listImports.GetCopyMenu();
            listImports.AddShortcuts();
            ColumnSettings.LoadSettings(Settings.Instance.PEImportsColumns, listImports);
        }

        public string Id
        {
            get { return _path; } 
        }

        private unsafe void Read()
        {
            // Preprare lists

            #region COFF Header

            // COFF header
            listCOFFHeader.Items.Clear();
            listCOFFHeader.Items.Add(new ListViewItem(new string[] { "Target Machine", 
                _mappedImage.NtHeaders->FileHeader.Machine.ToString() }));
            listCOFFHeader.Items.Add(new ListViewItem(new string[] { "Number of Sections", 
                _mappedImage.NtHeaders->FileHeader.NumberOfSections.ToString() }));
            listCOFFHeader.Items.Add(new ListViewItem(new string[] { "Time/Date Stamp", 
                Utils.GetDateTimeFromUnixTime((uint)_mappedImage.NtHeaders->FileHeader.TimeDateStamp).ToString() }));
            listCOFFHeader.Items.Add(new ListViewItem(new string[] { "Pointer to Symbol Table", 
                Utils.FormatAddress(_mappedImage.NtHeaders->FileHeader.PointerToSymbolTable) }));
            listCOFFHeader.Items.Add(new ListViewItem(new string[] { "Number of Symbols", 
                _mappedImage.NtHeaders->FileHeader.NumberOfSymbols.ToString() }));
            listCOFFHeader.Items.Add(new ListViewItem(new string[] { "Size of Optional Header", 
                _mappedImage.NtHeaders->FileHeader.SizeOfOptionalHeader.ToString() }));
            listCOFFHeader.Items.Add(new ListViewItem(new string[] { "Characteristics", 
                _mappedImage.NtHeaders->FileHeader.Characteristics.ToString() }));

            #endregion

            #region COFF Optional Header

            // COFF optional header
            listCOFFOptionalHeader.Items.Clear();
            listCOFFOptionalHeader.Items.Add(new ListViewItem(new string[] { "Magic", 
                _mappedImage.NtHeaders->OptionalHeader.Magic == Win32.Pe32Magic ? "PE32 (0x10b)" : 
                (_mappedImage.NtHeaders->OptionalHeader.Magic == Win32.Pe32PlusMagic ? "PE32+ (0x20b)" : 
                "Unknown (0x" + _mappedImage.NtHeaders->OptionalHeader.Magic.ToString("x") + ")") }));
            listCOFFOptionalHeader.Items.Add(new ListViewItem(new string[] { "Linker Version",
                _mappedImage.NtHeaders->OptionalHeader.MajorLinkerVersion.ToString() + "." +
                _mappedImage.NtHeaders->OptionalHeader.MinorLinkerVersion.ToString() }));
            listCOFFOptionalHeader.Items.Add(new ListViewItem(new string[] { "Size of Code",
                "0x" + _mappedImage.NtHeaders->OptionalHeader.SizeOfCode.ToString("x") }));
            listCOFFOptionalHeader.Items.Add(new ListViewItem(new string[] { "Size of Initialized Data",
                "0x" + _mappedImage.NtHeaders->OptionalHeader.SizeOfInitializedData.ToString("x") }));
            listCOFFOptionalHeader.Items.Add(new ListViewItem(new string[] { "Size of Uninitialized Data",
                "0x" + _mappedImage.NtHeaders->OptionalHeader.SizeOfUninitializedData.ToString("x") }));
            listCOFFOptionalHeader.Items.Add(new ListViewItem(new string[] { "Entry Point RVA",
                "0x" + _mappedImage.NtHeaders->OptionalHeader.AddressOfEntryPoint.ToString("x") }));
            listCOFFOptionalHeader.Items.Add(new ListViewItem(new string[] { "Base of Code",
                "0x" + _mappedImage.NtHeaders->OptionalHeader.BaseOfCode.ToString("x") }));
            listCOFFOptionalHeader.Items.Add(new ListViewItem(new string[] { "Preferred Image Base",
                "0x" + _mappedImage.NtHeaders->OptionalHeader.ImageBase.ToString("x") }));
            listCOFFOptionalHeader.Items.Add(new ListViewItem(new string[] { "Section Alignment",
                _mappedImage.NtHeaders->OptionalHeader.SectionAlignment.ToString() }));
            listCOFFOptionalHeader.Items.Add(new ListViewItem(new string[] { "File Alignment",
                _mappedImage.NtHeaders->OptionalHeader.FileAlignment.ToString() }));
            listCOFFOptionalHeader.Items.Add(new ListViewItem(new string[] { "Operating System Version",
                _mappedImage.NtHeaders->OptionalHeader.MajorOperatingSystemVersion.ToString() + "." +
                _mappedImage.NtHeaders->OptionalHeader.MinorOperatingSystemVersion.ToString() }));
            listCOFFOptionalHeader.Items.Add(new ListViewItem(new string[] { "Image Version",
                _mappedImage.NtHeaders->OptionalHeader.MajorImageVersion.ToString() + "." +
                _mappedImage.NtHeaders->OptionalHeader.MinorImageVersion.ToString() }));
            listCOFFOptionalHeader.Items.Add(new ListViewItem(new string[] { "Subsystem Version",
                _mappedImage.NtHeaders->OptionalHeader.MajorSubsystemVersion.ToString() + "." +
                _mappedImage.NtHeaders->OptionalHeader.MinorSubsystemVersion.ToString() }));
            listCOFFOptionalHeader.Items.Add(new ListViewItem(new string[] { "Size of Image",
                "0x" + _mappedImage.NtHeaders->OptionalHeader.SizeOfImage.ToString("x") }));
            listCOFFOptionalHeader.Items.Add(new ListViewItem(new string[] { "Size of Headers",
                "0x" + _mappedImage.NtHeaders->OptionalHeader.SizeOfHeaders.ToString("x") }));
            listCOFFOptionalHeader.Items.Add(new ListViewItem(new string[] { "Checksum",
                "0x" + _mappedImage.NtHeaders->OptionalHeader.CheckSum.ToString("x") }));
            listCOFFOptionalHeader.Items.Add(new ListViewItem(new string[] { "Subsystem",
                _mappedImage.NtHeaders->OptionalHeader.Subsystem.ToString() }));
            listCOFFOptionalHeader.Items.Add(new ListViewItem(new string[] { "DLL Characteristics",
                _mappedImage.NtHeaders->OptionalHeader.DllCharacteristics.ToString() }));
            listCOFFOptionalHeader.Items.Add(new ListViewItem(new string[] { "Size of Stack Reserve",
                "0x" + _mappedImage.NtHeaders->OptionalHeader.SizeOfStackReserve.ToString("x") }));
            listCOFFOptionalHeader.Items.Add(new ListViewItem(new string[] { "Size of Stack Commit",
                "0x" + _mappedImage.NtHeaders->OptionalHeader.SizeOfStackCommit.ToString("x") }));
            listCOFFOptionalHeader.Items.Add(new ListViewItem(new string[] { "Size of Heap Reserve",
                "0x" + _mappedImage.NtHeaders->OptionalHeader.SizeOfHeapReserve.ToString("x") }));
            listCOFFOptionalHeader.Items.Add(new ListViewItem(new string[] { "Size of Heap Commit",
                "0x" + _mappedImage.NtHeaders->OptionalHeader.SizeOfHeapCommit.ToString("x") }));
            listCOFFOptionalHeader.Items.Add(new ListViewItem(new string[] { "Number of Data Directory Entries",
                _mappedImage.NtHeaders->OptionalHeader.NumberOfRvaAndSizes.ToString() }));

            #endregion

            #region Image Data

            listImageData.Items.Clear();

            for (int i = 0; i < _mappedImage.NumberOfDataEntries; i++)
            {
                ImageDataDirectory* dataEntry;

                dataEntry = _mappedImage.GetDataEntry((ImageDataEntry)i);

                if (dataEntry != null && dataEntry->VirtualAddress != 0)
                {
                    ListViewItem item = new ListViewItem();

                    item.Text = ((ImageDataEntry)i).ToString();
                    item.SubItems.Add(new ListViewItem.ListViewSubItem(item, "0x" + dataEntry->VirtualAddress.ToString("x")));
                    item.SubItems.Add(new ListViewItem.ListViewSubItem(item, "0x" + dataEntry->Size.ToString("x")));

                    listImageData.Items.Add(item);
                }
            }

            #endregion

            #region Sections

            listSections.Items.Clear();

            for (int i = 0; i < _mappedImage.NumberOfSections; i++)
            {
                ImageSectionHeader* section = &_mappedImage.Sections[i];
                ListViewItem item = new ListViewItem();

                item.Text = _mappedImage.GetSectionName(section);
                item.SubItems.Add(new ListViewItem.ListViewSubItem(item, "0x" + section->VirtualAddress.ToString("x")));
                item.SubItems.Add(new ListViewItem.ListViewSubItem(item, "0x" + section->SizeOfRawData.ToString("x")));
                item.SubItems.Add(new ListViewItem.ListViewSubItem(item, "0x" + section->PointerToRawData.ToString("x")));
                item.SubItems.Add(new ListViewItem.ListViewSubItem(item, section->Characteristics.ToString()));

                listSections.Items.Add(item);
            }

            #endregion

            #region Exports

            listExports.VirtualListSize = _mappedImage.Exports.Count;

            #endregion

            #region Imports

            listImports.Items.Clear();
            listImports.Groups.Clear();

            var list = new List<KeyValuePair<string,int>>();

            for (int i = 0; i < _mappedImage.Imports.Count; i++)
                list.Add(new KeyValuePair<string, int>(_mappedImage.Imports[i].Name, i));

            list.Sort((kvp1, kvp2) => StringComparer.CurrentCultureIgnoreCase.Compare(kvp1.Key, kvp2.Key));

            for (int i = 0; i < list.Count; i++)
            {
                var dll = _mappedImage.Imports[list[i].Value];
                int index = list[i].Value;

                listImports.Groups.Add(new ListViewGroup(list[i].Key));

                for (int j = 0; j < dll.Count; j++)
                {
                    var entry = dll[j];
                    ListViewItem item = new ListViewItem(listImports.Groups[listImports.Groups.Count - 1]);

                    if (entry.Name == null)
                    {
                        item.Text = "(Ordinal " + entry.Ordinal.ToString() + ")";
                        item.SubItems.Add(new ListViewItem.ListViewSubItem());
                    }
                    else
                    {
                        item.Text = entry.Name;
                        item.SubItems.Add(new ListViewItem.ListViewSubItem(item, entry.NameHint.ToString()));
                    }

                    listImports.Items.Add(item);
                }
            }

            // We set GroupState here else there are no groups to set state.
            listImports.SetGroupState(ListViewGroupState.Collapsible, "Properties");

            #endregion
        }

        private void listExports_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            unsafe
            {
                var entry = _mappedImage.Exports.GetEntry(e.ItemIndex);
                var function = _mappedImage.Exports.GetFunction(entry.Ordinal);

                e.Item = new ListViewItem(new string[]
                    {
                        entry.Ordinal.ToString(),
                        function.ForwardedName != null ? entry.Name + " > " + function.ForwardedName : entry.Name,
                        function.ForwardedName == null ? 
                        "0x" + function.Function.Decrement(new IntPtr(_mappedImage.Memory)).ToString("x") :
                        ""
                    });
            }
        }

        private void listExports_DoubleClick(object sender, EventArgs e)
        {
            //DisassemblyWindow dw = new DisassemblyWindow(new FileStream(_path, FileMode.Open, FileAccess.Read), 
            //    _exportVAs[_peFile.ExportData.ExportOrdinalTable[listExports.SelectedIndices[0]]], -1);

            //dw.Show();
        }

        private void listImports_GroupLinkClicked(object sender, ProcessHacker.Components.LinkClickedEventArgs e)
        {
            string fileName;

            this.Cursor = Cursors.WaitCursor;

            try
            {
                fileName = FileUtils.FindFile(System.IO.Path.GetDirectoryName(_path), e.Group.Header);

                if (fileName != null)
                {
                    Program.GetPEWindow(fileName, (f) => Program.FocusWindow(f));
                }
                else
                {
                    PhUtils.ShowError("Unable to find the DLL.");
                }
            }
            catch (Exception ex)
            {
                PhUtils.ShowException("Unable to inspect the DLL", ex);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }
    }
}
