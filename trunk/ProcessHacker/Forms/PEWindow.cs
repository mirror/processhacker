using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ProcessHacker.PE;

namespace ProcessHacker
{
    public partial class PEWindow : Form
    {
        private string _path;
        private PEFile _peFile;

        public PEWindow(string path)
        {
            InitializeComponent();

            Misc.SetDoubleBuffered(listExports, typeof(ListView), true);
            listExports.ContextMenu = ListViewMenu.GetMenu(listExports);
            Misc.SetDoubleBuffered(listCOFFHeader, typeof(ListView), true);
            listCOFFHeader.ContextMenu = ListViewMenu.GetMenu(listCOFFHeader);
            Misc.SetDoubleBuffered(listCOFFOptionalHeader, typeof(ListView), true);
            listCOFFOptionalHeader.ContextMenu = ListViewMenu.GetMenu(listCOFFOptionalHeader);
            Misc.SetDoubleBuffered(listImageData, typeof(ListView), true);
            listImageData.ContextMenu = ListViewMenu.GetMenu(listImageData);
            Misc.SetDoubleBuffered(listSections, typeof(ListView), true);
            listSections.ContextMenu = ListViewMenu.GetMenu(listSections);

            _path = path;
            this.Text = "PE File - " + path;

            Program.PEWindows.Add(Id, this);

            if (!this.Read(path))
            {
                this.Close();
            }
        }

        private void PEWindow_Load(object sender, EventArgs e)
        {
            Program.UpdateWindows();
        }

        public string Id
        {
            get { return _path; } 
        }

        public MenuItem WindowMenuItem
        {
            get { return windowMenuItem; }
        }

        public wyDay.Controls.VistaMenu VistaMenu
        {
            get { return vistaMenu; }
        }

        private bool Read(string path)
        {
            PEFile peFile;
     
            try
            {
                peFile = new PEFile(path);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading the specified file:\n\n" + ex.Message, "Process Hacker",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                return false;
            }

            _peFile = peFile;

            // preprare lists
            listExports.VirtualListSize = _peFile.ExportData.ExportOrdinalTable.Count;

            #region COFF Header

            // COFF header
            listCOFFHeader.Items.Clear();
            listCOFFHeader.Items.Add(new ListViewItem(new string[] { "Target Machine", 
                _peFile.COFFHeader.Machine.ToString() }));
            listCOFFHeader.Items.Add(new ListViewItem(new string[] { "Number of Sections", 
                _peFile.COFFHeader.NumberOfSections.ToString() }));
            listCOFFHeader.Items.Add(new ListViewItem(new string[] { "Time/Date Stamp", 
                Misc.DateTimeFromUnixTime(_peFile.COFFHeader.TimeDateStamp).ToString() }));
            listCOFFHeader.Items.Add(new ListViewItem(new string[] { "Pointer to Symbol Table", 
                "0x" + _peFile.COFFHeader.PointerToSymbolTable.ToString("x8") }));
            listCOFFHeader.Items.Add(new ListViewItem(new string[] { "Number of Symbols", 
                _peFile.COFFHeader.NumberOfSymbols.ToString() }));
            listCOFFHeader.Items.Add(new ListViewItem(new string[] { "Size of Optional Header", 
                _peFile.COFFHeader.SizeOfOptionalHeader.ToString() }));
            listCOFFHeader.Items.Add(new ListViewItem(new string[] { "Characteristics", 
                Misc.FlagsToString(typeof(ImageCharacteristics), (long)_peFile.COFFHeader.Characteristics) }));

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
                Misc.FlagsToString(typeof(DllCharacteristics), (long)_peFile.COFFOptionalHeader.DllCharacteristics) }));
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
                item.SubItems.Add(new ListViewItem.ListViewSubItem(item, "0x" + sh.PointerToRawData.ToString("x8")));
                item.SubItems.Add(new ListViewItem.ListViewSubItem(item, "0x" + sh.VirtualSize.ToString("x")));
                item.SubItems.Add(new ListViewItem.ListViewSubItem(item, 
                    Misc.FlagsToString(typeof(SectionFlags), (long)sh.Characteristics)));

                listSections.Items.Add(item);
            }

            #endregion

            return true;
        }

        private void listExports_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            if (_peFile != null)
            {
                e.Item = new ListViewItem();

                e.Item.Text = _peFile.ExportData.ExportNameTable[e.ItemIndex];
                e.Item.SubItems.Add(new ListViewItem.ListViewSubItem(e.Item, (e.ItemIndex + 1).ToString()));
                e.Item.SubItems.Add(new ListViewItem.ListViewSubItem());
                e.Item.SubItems.Add(new ListViewItem.ListViewSubItem());  

                ExportEntry entry = _peFile.ExportData.ExportAddressTable[e.ItemIndex];

                if (entry.Type == ExportEntry.ExportType.Export)
                {
                    e.Item.SubItems[2].Text = "0x" + entry.ExportRVA.ToString("x8");
                    e.Item.SubItems[3].Text = "0x" + PEFile.RvaToVa(_peFile, entry.ExportRVA).ToString("x8");
                }
                else if (entry.Type == ExportEntry.ExportType.Forwarder)
                {                             
                    e.Item.ImageIndex = 0;
                    e.Item.Text += " > " + entry.ForwardedString;
                }
            }
        }
    }
}
