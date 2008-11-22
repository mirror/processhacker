using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace ProcessHacker
{
    public partial class DisassemblyWindow : Form
    {                     
        private List<string[]> _disasm = new List<string[]>();

        public DisassemblyWindow(Stream s, long position, int maxCount)
        {
            InitializeComponent();

            listDisasm.ContextMenu = ListViewMenu.GetMenu(listDisasm, 
                new RetrieveVirtualItemEventHandler(listDisasm_RetrieveVirtualItem));

            Asm.Disassembler disasm = new Asm.Disassembler();

            s.Seek(position, SeekOrigin.Begin);
                  
            while ((maxCount != -1 && _disasm.Count < maxCount) || (maxCount == -1))
            {
                byte[] data = new byte[16];
                long pos = s.Position;

                s.Read(data, 0, 16);
                s.Seek(disasm.Disassemble(data, 16, 0, Asm.DISASM_FILE) - 16, SeekOrigin.Current);

                _disasm.Add(new string[]
                {
                    "0x" + pos.ToString("x8"),
                    disasm.Result.Dump.ToString(),
                    disasm.Result.Result.ToString(),
                    disasm.Result.Comment
                });

                if (maxCount == -1)
                    if (disasm.Result.Result.ToString().StartsWith("ret"))
                        break;
            }

            listDisasm.VirtualListSize = _disasm.Count;
        }

        private void listDisasm_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            string[] item = _disasm[e.ItemIndex];

            e.Item = new ListViewItem(item); 
        }
    }
}
