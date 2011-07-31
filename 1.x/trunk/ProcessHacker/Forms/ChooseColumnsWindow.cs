/*
 * Process Hacker - 
 *   column chooser
 * 
 * Copyright (C) 2009 wj32
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
using Aga.Controls.Tree;
using ProcessHacker.Common;

namespace ProcessHacker
{
    public partial class ChooseColumnsWindow : Form
    {
        private struct TvColumn
        {
            public string Header;
            public int Index;
            public bool Visible;
        }

        private object _list;

        public ChooseColumnsWindow(ListView list)
            : this()
        {
            _list = list;

            foreach (ColumnHeader column in list.Columns)
            {
                listColumns.Items.Add(new ListViewItem()
                {
                    Text = column.Text,
                    Name = column.Index.ToString()
                });
            }
        }

        public ChooseColumnsWindow(TreeViewAdv tree)
            : this()
        {
            _list = tree;

            List<TvColumn> columns = new List<TvColumn>();

            foreach (TreeColumn column in tree.Columns)
            {
                columns.Add(new TvColumn()
                {
                    Header = column.Header,
                    Index = column.Index,
                    Visible = column.IsVisible
                });
            }

            columns.Sort((tvc1, tvc2) =>
                {
                    // Sort by visibility first.
                    if (tvc1.Visible != tvc2.Visible)
                        return -tvc1.Visible.CompareTo(tvc2.Visible);

                    // Sort the visible columns by index.
                    if (tvc1.Visible == true)
                        return tvc1.Index.CompareTo(tvc2.Index);

                    // Sort the hidden columns by name.
                    if (tvc1.Visible == false)
                        return tvc1.Header.CompareTo(tvc2.Header);

                    return 0;
                });

            foreach (var column in columns)
            {
                listColumns.Items.Add(new ListViewItem()
                {
                    Text = column.Header,
                    Name = column.Header,
                    Checked = column.Visible
                });
            }
        }

        private ChooseColumnsWindow()
        {
            InitializeComponent();
            this.AddEscapeToClose();
            this.SetTopMost();

            listColumns.SetDoubleBuffered(true);
            listColumns.SetTheme("explorer");
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (_list is TreeViewAdv)
            {
                TreeViewAdv tree = _list as TreeViewAdv;

                foreach (TreeColumn column in tree.Columns)
                {
                    column.IsVisible = listColumns.Items[column.Header].Checked;
                }
            }
            else if (_list is ListView)
            {

            }

            this.Close();
        }
    }
}
