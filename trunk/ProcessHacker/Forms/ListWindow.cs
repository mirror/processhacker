using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ProcessHacker
{
    public partial class ListWindow : Form
    {
        public ListWindow(List<KeyValuePair<string, string>> list)
        {
            InitializeComponent();

            foreach (KeyValuePair<string, string> kvp in list)
            {
                ListViewItem item = new ListViewItem();

                item.Text = kvp.Key;
                item.SubItems.Add(new ListViewItem.ListViewSubItem(item, kvp.Value));

                listView.Items.Add(item);
            }

            listView.ContextMenu = GenericViewMenu.GetMenu(listView);
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
