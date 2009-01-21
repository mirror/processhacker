/*
 * Process Hacker - 
 *   column settings manager
 * 
 * Copyright (C) 2008 wj32
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
using System.Collections;
using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;

namespace ProcessHacker
{
    /// <summary>
    /// Provides methods for loading and saving ListView column settings.
    /// </summary>
    public static class ColumnSettings
    {
        /// <summary>
        /// Saves the column settings of the specified ListView to a string.
        /// </summary>
        /// <param name="lv"></param>
        /// <returns></returns>
        public static string SaveSettings(ListView lv)
        {     
            string result = "";

            try
            {
                foreach (ColumnHeader ch in lv.Columns)
                {
                    result += ch.DisplayIndex.ToString() + "," + ch.Width.ToString() + "|";
                }
            }
            catch
            { }

            return result;
        }

        /// <summary>
        /// Saves the column settings of the specified TreeViewAdv to a string.
        /// </summary>
        /// <param name="lv"></param>
        /// <returns></returns>
        public static string SaveSettings(TreeViewAdv tv)
        {
            string result = "";

            try
            {
                for (int i = 0; i < tv.Columns.Count; i++)
                {
                    TreeColumn c = tv.Columns[i];
                    result += c.Header + "," + c.Width.ToString() + "," + c.SortOrder.ToString() + "|";
                }
            }
            catch
            { }

            return result;
        }

        /// <summary>
        /// Loads column settings from a string to a ListView.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="lv"></param>
        public static void LoadSettings(string settings, ListView lv)
        {
            string[] list = settings.Split('|');

            if (settings == "")
                return;

            for (int i = 0; i < list.Length; i++)
            {
                string[] s = list[i].Split(',');

                if (s.Length == 1)
                    break;

                lv.Columns[i].DisplayIndex = Int32.Parse(s[0]);
                lv.Columns[i].Width = Int32.Parse(s[1]);
            }
        }

        /// <summary>
        /// Loads column settings from a string to a TreeViewAdv.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="tv"></param>
        public static void LoadSettings(string settings, TreeViewAdv tv)
        {
            string[] list = settings.Split('|');

            try
            {
                Dictionary<NodeControl, string> oldAssoc = new Dictionary<NodeControl, string>();

                foreach (NodeControl control in tv.NodeControls)
                {
                    oldAssoc.Add(control, control.ParentColumn.Header);
                }

                TreeColumn[] newColumns = new TreeColumn[tv.Columns.Count];
                Dictionary<string, TreeColumn> newColumnsD = new Dictionary<string,TreeColumn>();

                for (int i = 0; i < tv.Columns.Count; i++)
                {
                    string[] s = list[i].Split(',');

                    newColumns[i] = new TreeColumn(s[0], Int32.Parse(s[1]));
                    newColumns[i].SortOrder = (SortOrder)Enum.Parse(typeof(SortOrder), s[2]);
                    newColumnsD.Add(s[0], newColumns[i]);
                }

                tv.Columns.Clear();

                foreach (TreeColumn column in newColumns)
                    tv.Columns.Add(column);

                foreach (NodeControl c in oldAssoc.Keys)
                    c.ParentColumn = newColumnsD[oldAssoc[c]];
            }
            catch
            { }
        }
    }
}
