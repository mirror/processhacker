using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SysCallHacker
{
    public partial class SelectSystemCallWindow : Form
    {
        public SelectSystemCallWindow(IEnumerable<string> names)
        {
            InitializeComponent();

            foreach (var name in names)
                listSystemCalls.Items.Add(name);

            listSystemCalls.Sorted = true;
        }

        public string SelectedItem
        {
            get { return (string)listSystemCalls.SelectedItem; }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void listSystemCalls_SelectedIndexChanged(object sender, EventArgs e)
        {
            buttonOK.Enabled = listSystemCalls.SelectedIndices.Count == 1;
        }

        private void SelectSystemCallWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
                e.Handled = true;
            }
        }

        private void listSystemCalls_DoubleClick(object sender, EventArgs e)
        {
            if (buttonOK.Enabled)
                buttonOK.PerformClick();
        }
    }
}
