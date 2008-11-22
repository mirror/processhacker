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
        private int _bolded = -1;

        public DisassemblyWindow(Stream s, long position, int maxCount)
        {
            InitializeComponent();

            Misc.SetDoubleBuffered(listDisasm, typeof(ListView), true);
            ListViewMenu.AddMenuItems(copyMenuItem.MenuItems, listDisasm,
                new RetrieveVirtualItemEventHandler(listDisasm_RetrieveVirtualItem));
            listDisasm.ContextMenu = menuLine;

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

            if (item[2].StartsWith("jmp"))
                e.Item.ImageIndex = 1;
            else if (item[2].StartsWith("j"))
                e.Item.ImageIndex = 3;
            else if (item[2].StartsWith("call"))
                e.Item.ImageIndex = 2;
            else if (item[2].StartsWith("ret"))
                e.Item.ImageIndex = 4;

            if (e.ItemIndex == _bolded)
                e.Item.Font = new Font(listDisasm.Font, FontStyle.Bold);
        }

        private void followMenuItem_Click(object sender, EventArgs e)
        {
            int selected = listDisasm.SelectedIndices[0];

            try
            {
                listDisasm.SelectedIndices.Clear();
                listDisasm.SelectedIndices.Add(this.GetShortJumpItemIndex(selected));
            }
            catch
            { }
        }

        private int GetShortJumpItemIndex(int index)
        {
            string[] item = _disasm[index];
            int addr = (int)BaseConverter.ToNumberParse(item[0]);
            int arg = 0;
            string[] split = item[2].Split(' ');

            for (int i = 0; i < split.Length; i++)
            {
                if (split[i].ToLower() == "short")
                {
                    arg = (int)BaseConverter.ToNumber(split[i + 1], 16);
                    break;
                }
            }

            if (arg == 0)
                return -1;

            string str = "0x" + (addr + arg).ToString("x8");

            for (int i = 0; i < _disasm.Count; i++)
            {
                if (_disasm[i][0].ToLower() == str)
                {
                    return i;
                }
            }

            return -1;
        }

        private void listDisasm_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                _bolded = this.GetShortJumpItemIndex(listDisasm.SelectedIndices[0]);
                listDisasm.Invalidate();
            }
            catch
            { }
        }
    }
}
