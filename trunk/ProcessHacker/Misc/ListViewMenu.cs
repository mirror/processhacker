/*
 * Process Hacker
 * 
 * Copyright (C) 2008 wj32
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ProcessHacker
{
    public static class ListViewMenu
    {
        public static ContextMenu GetMenu(ListView lv)
        {
            return GetMenu(lv, null);
        }

        public static ContextMenu GetMenu(ListView lv, RetrieveVirtualItemEventHandler retrieveVirtualItem)
        {
            ContextMenu menu = new ContextMenu();

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

        private static void ListViewMenuItem_Click(object sender, EventArgs e)
        {
            MenuItem mitem = (MenuItem)sender;
            int subitem = (int)((object[])mitem.Tag)[0];
            ListView lv = (ListView)((object[])mitem.Tag)[1];
            RetrieveVirtualItemEventHandler retrieveVirtualItem = (RetrieveVirtualItemEventHandler)((object[])mitem.Tag)[2];
            List<ListViewItem> collection = new List<ListViewItem>();
            string text = "";

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
                if (subitem == -1)
                {
                    for (int j = 0; j < lv.Columns.Count; j++)
                    {
                        text += collection[i].SubItems[j].Text;

                        if (j != lv.Columns.Count - 1)
                            text += ", ";
                    }
                }
                else
                {
                    text += collection[i].SubItems[subitem].Text;
                }

                if (i != collection.Count - 1)
                    text += "\r\n";
            }

            Clipboard.SetText(text);
        }
    }
}
