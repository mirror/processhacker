/*
 * Process Hacker - 
 *   list view/tree view context menu generator
 * 
 * Copyright (C) 2008-2009 wj32
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
using System.Windows.Forms;
using System.Collections.Generic;
using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;
using System.Text;

namespace ProcessHacker
{
    public static class GenericViewMenu
    {
        public static ContextMenu GetMenu(ListView lv)
        {
            return GetMenu(lv, null);
        }

        public static ContextMenu GetMenu(ListView lv, RetrieveVirtualItemEventHandler retrieveVirtualItem)
        {
            ContextMenu menu = new ContextMenu();

            menu.Tag = lv;
            menu.Popup += new EventHandler(ListViewMenu_Popup);
            AddMenuItems(menu.MenuItems, lv, retrieveVirtualItem);

            return menu;
        }

        public static void AddMenuItems(MenuItem.MenuItemCollection items, ListView lv, RetrieveVirtualItemEventHandler retrieveVirtualItem)
        {
            MenuItem copyItem = new MenuItem("Copy");

            copyItem.Tag = new object[] { -1, lv, retrieveVirtualItem };
            copyItem.Click += new EventHandler(ListViewMenuItem_Click);

            items.Add(copyItem);

            foreach (ColumnHeader ch in lv.Columns)
            {
                MenuItem item = new MenuItem("Copy \"" + ch.Text + "\"");

                item.Tag = new object[] { ch.Index, lv, retrieveVirtualItem };
                item.Click += new EventHandler(ListViewMenuItem_Click);

                items.Add(item);
            }
        }

        private static void ListViewMenu_Popup(object sender, EventArgs e)
        {
            ContextMenu citem = (ContextMenu)sender;
            ListView lv = (ListView)citem.Tag;

            if (lv.SelectedIndices.Count == 0)
            {
                Misc.DisableAllMenuItems(citem);
            }
            else
            {
                Misc.EnableAllMenuItems(citem);
            }
        }

        public static void ListViewCopy(ListView lv, int subItem)
        {
            ListViewCopy(lv, subItem, null);
        }

        public static void ListViewCopy(ListView lv, int subItem, RetrieveVirtualItemEventHandler retrieveVirtualItem)
        {
            List<ListViewItem> collection = new List<ListViewItem>();
            StringBuilder text = new StringBuilder();

            if (lv.SelectedIndices.Count == 0)
                return;

            if (retrieveVirtualItem != null)
            {
                foreach (int index in lv.SelectedIndices)
                {
                    RetrieveVirtualItemEventArgs args = new RetrieveVirtualItemEventArgs(index);

                    retrieveVirtualItem(lv, args);

                    collection.Add(args.Item);
                }
            }
            else
            {
                foreach (ListViewItem item in lv.SelectedItems)
                    collection.Add(item);
            }

            for (int i = 0; i < collection.Count; i++)
            {
                if (subItem == -1)
                {
                    for (int j = 0; j < lv.Columns.Count; j++)
                    {
                        text.Append(collection[i].SubItems[j].Text);

                        if (j != lv.Columns.Count - 1)
                            text.Append(", ");
                    }
                }
                else
                {
                    text.Append(collection[i].SubItems[subItem].Text);
                }

                if (i != collection.Count - 1)
                    text.AppendLine();
            }

            Clipboard.SetText(text.ToString());
        }

        private static void ListViewMenuItem_Click(object sender, EventArgs e)
        {
            MenuItem mitem = (MenuItem)sender;

            ListViewCopy((ListView)((object[])mitem.Tag)[1], (int)((object[])mitem.Tag)[0],
                (RetrieveVirtualItemEventHandler)((object[])mitem.Tag)[2]);
        }

        public static void AddMenuItems(MenuItem.MenuItemCollection items, TreeViewAdv tv)
        {
            MenuItem copyItem = new MenuItem("Copy");

            copyItem.Tag = new object[] { -1, tv };
            copyItem.Click += new EventHandler(TreeViewAdvMenuItem_Click);

            items.Add(copyItem);

            foreach (TreeColumn c in tv.Columns)
            {
                MenuItem item = new MenuItem("Copy \"" + c.Header + "\"");

                item.Tag = new object[] { c.Index, tv };
                item.Click += new EventHandler(TreeViewAdvMenuItem_Click);

                items.Add(item);
            }
        }

        public static void TreeViewAdvCopy(TreeViewAdv tv, int columnIndex)
        {
            List<string[]> collection = new List<string[]>();
            StringBuilder text = new StringBuilder();

            if (tv.SelectedNodes.Count == 0)
                return;

            foreach (TreeNodeAdv item in tv.SelectedNodes)
            {
                string[] array = new string[tv.Columns.Count];
                int i = 0;

                foreach (NodeControl control in tv.NodeControls)
                {
                    if (control.ParentColumn.Index == i && (control is BaseTextControl))
                    {
                        array[i] = (control as BaseTextControl).GetLabel(item);
                        i++;
                    }
                }

                collection.Add(array);
            }

            for (int i = 0; i < collection.Count; i++)
            {
                if (columnIndex == -1)
                {
                    for (int j = 0; j < tv.Columns.Count; j++)
                    {
                        text.Append(collection[i][j]);

                        if (j != tv.Columns.Count - 1)
                            text.Append(", ");
                    }
                }
                else
                {
                    text.AppendLine(collection[i][columnIndex]);
                }

                if (i != collection.Count - 1)
                    text.AppendLine();
            }

            Clipboard.SetText(text.ToString());
        }

        private static void TreeViewAdvMenuItem_Click(object sender, EventArgs e)
        {
            MenuItem mitem = (MenuItem)sender;
            
            TreeViewAdvCopy((TreeViewAdv)((object[])mitem.Tag)[1], (int)((object[])mitem.Tag)[0]);
        }
    }
}
