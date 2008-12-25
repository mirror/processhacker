using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ProcessHacker.Structs;

namespace ProcessHacker
{
    public partial class StructWindow : Form
    {
        public StructWindow(int pid, int address, StructDef struc)
        {
            InitializeComponent();

            StructViewer sv = new StructViewer(pid, address, struc);

            sv.Dock = DockStyle.Fill;
            panel.Controls.Add(sv);
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
