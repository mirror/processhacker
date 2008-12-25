using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ProcessHacker
{
    public partial class ListPickerWindow : Form
    {
        public ListPickerWindow(string[] items)
        {
            InitializeComponent();

            listItems.Items.AddRange(items);

            if (listItems.Items.Count > 0)
                listItems.SelectedItem = listItems.Items[0];
        }

        public string SelectedItem
        {
            get { return listItems.SelectedItem as string; }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void listItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listItems.SelectedItem == null)
                buttonOK.Enabled = false;
            else
                buttonOK.Enabled = true;
        }
    }
}
