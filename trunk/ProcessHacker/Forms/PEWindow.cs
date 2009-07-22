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
using ProcessHacker.PE;
using ProcessHacker.UI;

namespace ProcessHacker
{
    public partial class PEWindow : Form
    {
        private string _path;
        private PEFile _peFile;
        private List<long> _exportVAs;

        public PEWindow(string path)
        {
            InitializeComponent();
            this.AddEscapeToClose();

            _path = path;
            this.Text = "PE File - " + path;
            Program.PEWindows.Add(Id, this);

            this.InitializeLists();

            try
            {
                this.Read(_path);
            }
            catch (Exception ex)
            {
                PhUtils.ShowException("Unable to load the specified file", ex);

                this.Close();
            }
        }

        private void PEWindow_Load(object sender, EventArgs e)
        {
            this.Size = Properties.Settings.Default.PEWindowSize;

            this.SetPhParent();
        }

        private void PEWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Visible = false;

            Properties.Settings.Default.PECOFFHColumns = ColumnSettings.SaveSettings(listCOFFHeader);
            Properties.Settings.Default.PECOFFOHColumns = ColumnSettings.SaveSettings(listCOFFOptionalHeader);
            Properties.Settings.Default.PEImageDataColumns = ColumnSettings.SaveSettings(listImageData);
            Properties.Settings.Default.PESectionsColumns = ColumnSettings.SaveSettings(listSections);
            Properties.Settings.Default.PEExportsColumns = ColumnSettings.SaveSettings(listExports);
            Properties.Settings.Default.PEImportsColumns = ColumnSettings.SaveSettings(listImports);
            Properties.Settings.Default.PEWindowSize = this.Size;
        }

        private void InitializeLists()
        {
            listCOFFHeader.SetDoubleBuffered(true);
            listCOFFHeader.SetTheme("explorer");
            listCOFFHeader.ContextMenu = listCOFFHeader.GetCopyMenu();
            listCOFFHeader.AddShortcuts();
            ColumnSettings.LoadSettings(Properties.Settings.Default.PECOFFHColumns, listCOFFHeader);

            listCOFFOptionalHeader.SetDoubleBuffered(true);
            listCOFFOptionalHeader.SetTheme("explorer");
            listCOFFOptionalHeader.ContextMenu = listCOFFOptionalHeader.GetCopyMenu();
            listCOFFOptionalHeader.AddShortcuts();
            ColumnSettings.LoadSettings(Properties.Settings.Default.PECOFFOHColumns, listCOFFOptionalHeader);

            listImageData.SetDoubleBuffered(true);
            listImageData.SetTheme("explorer");
            listImageData.ContextMenu = listImageData.GetCopyMenu();
            listImageData.AddShortcuts();
            ColumnSettings.LoadSettings(Properties.Settings.Default.PEImageDataColumns, listImageData);

            listSections.SetDoubleBuffered(true);
            listSections.SetTheme("explorer");
            listSections.ContextMenu = listSections.GetCopyMenu();
            listSections.AddShortcuts();
            ColumnSettings.LoadSettings(Properties.Settings.Default.PESectionsColumns, listSections);

            listExports.SetDoubleBuffered(true);
            listExports.SetTheme("explorer");
            listExports.ContextMenu = listExports.GetCopyMenu(listExports_RetrieveVirtualItem);
            listExports.AddShortcuts(this.listExports_RetrieveVirtualItem);
            ColumnSettings.LoadSettings(Properties.Settings.Default.PEExportsColumns, listExports);

            listImports.SetDoubleBuffered(true);
            listImports.SetTheme("explorer");
            listImports.ContextMenu = listImports.GetCopyMenu();
            listImports.AddShortcuts();
            ColumnSettings.LoadSettings(Properties.Settings.Default.PEImportsColumns, listImports);
        }

        public string Id
        {
            get { return _path; } 
        }

        private void Read(string path)
        {
            PEFile peFile;
     
            peFile = new PEFile(path);

            _peFile = peFile;

            // preprare lists

            #region COFF Header

            // COFF header
            listCOFFHeader.Items.Clear();
            listCOFFHeader.Items.Add(new ListViewItem(new string[] { "Target Machine", 
                _peFile.COFFHeader.Machine.ToString() }));
            listCOFFHeader.Items.Add(new ListViewItem(new string[] { "Number of Sections", 
                _peFile.COFFHeader.NumberOfSections.ToString() }));
            listCOFFHeader.Items.Add(new ListViewItem(new string[] { "Time/Date Stamp", 
                Utils.GetDateTimeFromUnixTime(_peFile.COFFHeader.TimeDateStamp).ToString() }));
            listCOFFHeader.Items.Add(new ListViewItem(new string[] { "Pointer to Symbol Table", 
                "0x" + _peFile.COFFHeader.PointerToSymbolTable.ToString("x8") }));
            listCOFFHeader.Items.Add(new ListViewItem(new string[] { "Number of Symbols", 
                _peFile.COFFHeader.NumberOfSymbols.ToString() }));
            listCOFFHeader.Items.Add(new ListViewItem(new string[] { "Size of Optional Header", 
                _peFile.COFFHeader.SizeOfOptionalHeader.ToString() }));
            listCOFFHeader.Items.Add(new ListViewItem(new string[] { "Characteristics", 
                Utils.FlagsToString(typeof(ImageCharacteristics), (long)_peFile.COFFHeader.Characteristics) }));

            #endregion

            #region COFF Optional Header

            // COFF optional header
            listCOFFOptionalHeader.Items.Clear();
            listCOFFOptionalHeader.Items.Add(new ListViewItem(new string[] { "Magic", 
                _peFile.COFFOptionalHeader.Magic == COFFOptionalHeader.PE32Magic ? "PE32 (0x10b)" : 
                (_peFile.COFFOptionalHeader.Magic == COFFOptionalHeader.PE32PlusMagic ? "PE32+ (0x20b)" : 
                "Unknown (0x" + _peFile.COFFOptionalHeader.Magic.ToString("x") + ")") }));
            listCOFFOptionalHeader.Items.Add(new ListViewItem(new string[] { "Linker Version",
                _peFile.COFFOptionalHeader.MajorLinkerVersion.ToString() + "." +
                _peFile.COFFOptionalHeader.MinorLinkerVersion.ToString() }));
            listCOFFOptionalHeader.Items.Add(new ListViewItem(new string[] { "Size of Code",
                "0x" + _peFile.COFFOptionalHeader.SizeOfCode.ToString("x") }));
            listCOFFOptionalHeader.Items.Add(new ListViewItem(new string[] { "Size of Initialized Data",
                "0x" + _peFile.COFFOptionalHeader.SizeOfInitializedData.ToString("x") }));
            listCOFFOptionalHeader.Items.Add(new ListViewItem(new string[] { "Size of Uninitialized Data",
                "0x" + _peFile.COFFOptionalHeader.SizeOfUninitializedData.ToString("x") }));
            listCOFFOptionalHeader.Items.Add(new ListViewItem(new string[] { "Entry Point RVA",
                "0x" + _peFile.COFFOptionalHeader.AddressOfEntryPoint.ToString("x8") }));
            listCOFFOptionalHeader.Items.Add(new ListViewItem(new string[] { "Base of Code",
                "0x" + _peFile.COFFOptionalHeader.BaseOfCode.ToString("x8") }));
            if (_peFile.COFFOptionalHeader.Magic == COFFOptionalHeader.PE32PlusMagic)
                listCOFFOptionalHeader.Items.Add(new ListViewItem(new string[] { "Base of Data",
                    "0x" + _peFile.COFFOptionalHeader.BaseOfData.ToString("x8") }));
            listCOFFOptionalHeader.Items.Add(new ListViewItem(new string[] { "Preferred Image Base",
                "0x" + _peFile.COFFOptionalHeader.ImageBase.ToString("x8") }));
            listCOFFOptionalHeader.Items.Add(new ListViewItem(new string[] { "Section Alignment",
                _peFile.COFFOptionalHeader.SectionAlignment.ToString() }));
            listCOFFOptionalHeader.Items.Add(new ListViewItem(new string[] { "File Alignment",
                _peFile.COFFOptionalHeader.FileAlignment.ToString() }));
            listCOFFOptionalHeader.Items.Add(new ListViewItem(new string[] { "Operating System Version",
                _peFile.COFFOptionalHeader.MajorOperatingSystemVersion.ToString() + "." +
                _peFile.COFFOptionalHeader.MinorOperatingSystemVersion.ToString() }));
            listCOFFOptionalHeader.Items.Add(new ListViewItem(new string[] { "Image Version",
                _peFile.COFFOptionalHeader.MajorImageVersion.ToString() + "." +
                _peFile.COFFOptionalHeader.MinorImageVersion.ToString() }));
            listCOFFOptionalHeader.Items.Add(new ListViewItem(new string[] { "Subsystem Version",
                _peFile.COFFOptionalHeader.MajorSubsystemVersion.ToString() + "." +
                _peFile.COFFOptionalHeader.MinorSubsystemVersion.ToString() }));
            listCOFFOptionalHeader.Items.Add(new ListViewItem(new string[] { "Size of Image",
                "0x" + _peFile.COFFOptionalHeader.SizeOfImage.ToString("x") }));
            listCOFFOptionalHeader.Items.Add(new ListViewItem(new string[] { "Size of Headers",
                "0x" + _peFile.COFFOptionalHeader.SizeOfHeaders.ToString("x") }));
            listCOFFOptionalHeader.Items.Add(new ListViewItem(new string[] { "Checksum",
                "0x" + _peFile.COFFOptionalHeader.CheckSum.ToString("x8") }));
            listCOFFOptionalHeader.Items.Add(new ListViewItem(new string[] { "Subsystem",
                _peFile.COFFOptionalHeader.Subsystem.ToString() }));
            listCOFFOptionalHeader.Items.Add(new ListViewItem(new string[] { "DLL Characteristics",
                Utils.FlagsToString(typeof(DllCharacteristics), (long)_peFile.COFFOptionalHeader.DllCharacteristics) }));
            listCOFFOptionalHeader.Items.Add(new ListViewItem(new string[] { "Size of Stack Reserve",
                "0x" + _peFile.COFFOptionalHeader.SizeOfStackReserve.ToString("x") }));
            listCOFFOptionalHeader.Items.Add(new ListViewItem(new string[] { "Size of Stack Commit",
                "0x" + _peFile.COFFOptionalHeader.SizeOfStackCommit.ToString("x") }));
            listCOFFOptionalHeader.Items.Add(new ListViewItem(new string[] { "Size of Heap Reserve",
                "0x" + _peFile.COFFOptionalHeader.SizeOfHeapReserve.ToString("x") }));
            listCOFFOptionalHeader.Items.Add(new ListViewItem(new string[] { "Size of Heap Commit",
                "0x" + _peFile.COFFOptionalHeader.SizeOfHeapCommit.ToString("x") }));
            listCOFFOptionalHeader.Items.Add(new ListViewItem(new string[] { "Number of Data Directory Entries",
                _peFile.COFFOptionalHeader.NumberOfRvaAndSizes.ToString() }));

            #endregion

            #region Image Data

            listImageData.Items.Clear();

            for (int i = 0; i < _peFile.ImageData.Count; i++)
            {
                ImageDataType type = (ImageDataType)i;
                ImageData data = _peFile.ImageData[type];

                if (data.VirtualAddress != 0)
                {
                    ListViewItem item = new ListViewItem();

                    item.Text = type.ToString();
                    item.SubItems.Add(new ListViewItem.ListViewSubItem(item, "0x" + data.VirtualAddress.ToString("x8")));
                    item.SubItems.Add(new ListViewItem.ListViewSubItem(item, "0x" + data.Size.ToString("x")));

                    listImageData.Items.Add(item);
                }
            }

            #endregion

            #region Sections

            listSections.Items.Clear();

            foreach (SectionHeader sh in _peFile.Sections)
            {
                ListViewItem item = new ListViewItem();

                item.Text = sh.Name;
                item.SubItems.Add(new ListViewItem.ListViewSubItem(item, "0x" + sh.VirtualAddress.ToString("x8")));
                item.SubItems.Add(new ListViewItem.ListViewSubItem(item, "0x" + sh.VirtualSize.ToString("x")));
                item.SubItems.Add(new ListViewItem.ListViewSubItem(item, "0x" + sh.PointerToRawData.ToString("x8")));
                item.SubItems.Add(new ListViewItem.ListViewSubItem(item, 
                    Utils.FlagsToString(typeof(SectionFlags), (long)sh.Characteristics)));

                listSections.Items.Add(item);
            }

            #endregion

            #region Exports

            if (_peFile.ExportData != null)
            {
                listExports.VirtualListSize = _peFile.ExportData.ExportOrdinalTable.Count;

                _exportVAs = new List<long>();

                for (int i = 0; i < _peFile.ExportData.ExportAddressTable.Count; i++)
                {
                    ExportEntry entry = _peFile.ExportData.ExportAddressTable[i];

                    if (entry.ExportRVA != 0)
                        _exportVAs.Add(_peFile.RvaToVa(entry.ExportRVA));
                    else
                        _exportVAs.Add(0);
                }
            }
            else  
            {
                listExports.VirtualListSize = 0;
            }

            #endregion

            #region Imports

            listImports.Items.Clear();
            listImports.Groups.Clear();

            if (_peFile.ImportData != null)
            {      
                var list = new List<KeyValuePair<string,int>>();

                for (int i = 0; i < _peFile.ImportData.ImportDirectoryTable.Count; i++)
                    list.Add(new KeyValuePair<string,int>(_peFile.ImportData.ImportDirectoryTable[i].Name, i));

                list.Sort((kvp1, kvp2) => StringComparer.CurrentCultureIgnoreCase.Compare(kvp1.Key, kvp2.Key));

                for (int i = 0; i < list.Count; i++)
                {
                    int index = list[i].Value;

                    listImports.Groups.Add(new ListViewGroup(list[i].Key));

                    for (int j = 0; j < _peFile.ImportData.ImportLookupTable[index].Count; j++)
                    {
                        ImportLookupEntry entry = _peFile.ImportData.ImportLookupTable[index][j];
                        ListViewItem item = new ListViewItem(listImports.Groups[listImports.Groups.Count - 1]);

                        if (entry.UseOrdinal)
                        {
                            item.Text = "(Ordinal " + entry.Ordinal.ToString() + ")";
                            item.SubItems.Add(new ListViewItem.ListViewSubItem());
                        }
                        else
                        {
                            item.Text = entry.NameEntry.Name;
                            item.SubItems.Add(new ListViewItem.ListViewSubItem(item, entry.NameEntry.Hint.ToString()));
                        }

                        listImports.Items.Add(item);
                    }
                }
            }

            #endregion
        }

        private void listExports_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            if (_peFile != null)
            {
                ushort ordinal = _peFile.ExportData.ExportOrdinalTable[e.ItemIndex];

                if (ordinal >= _peFile.ExportData.ExportAddressTable.Count)
                {
                    e.Item = new ListViewItem(new string[] { ordinal.ToString(), ordinal.ToString(), "", "" });
                    return;
                } 

                ExportEntry entry = _peFile.ExportData.ExportAddressTable[ordinal];
                
                e.Item = new ListViewItem();

                if (e.ItemIndex < _peFile.ExportData.ExportNameTable.Count)
                    e.Item.Text = _peFile.ExportData.ExportNameTable[e.ItemIndex];

                e.Item.SubItems.Add(new ListViewItem.ListViewSubItem(e.Item, (e.ItemIndex + _peFile.ExportData.OrdinalBase).ToString()));
                e.Item.SubItems.Add(new ListViewItem.ListViewSubItem());
                e.Item.SubItems.Add(new ListViewItem.ListViewSubItem());  

                if (entry.Type == ExportEntry.ExportType.Export)
                {
                    e.Item.SubItems[2].Text = "0x" + entry.ExportRVA.ToString("x8");

                    if (entry.ExportRVA != 0)
                        e.Item.SubItems[3].Text = "0x" + _exportVAs[ordinal].ToString("x8");
                }
                else if (entry.Type == ExportEntry.ExportType.Forwarder)
                {
                    e.Item.ImageIndex = 0;
                    e.Item.Text += " > " + entry.ForwardedString;
                }
            }
        }

        private void listExports_DoubleClick(object sender, EventArgs e)
        {
            //DisassemblyWindow dw = new DisassemblyWindow(new FileStream(_path, FileMode.Open, FileAccess.Read), 
            //    _exportVAs[_peFile.ExportData.ExportOrdinalTable[listExports.SelectedIndices[0]]], -1);

            //dw.Show();
        }
    }
}
