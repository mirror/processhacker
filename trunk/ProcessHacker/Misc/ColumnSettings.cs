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
using System.Collections;
using Aga.Controls.Tree;

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
        /// Loads column settings from a string to a ListView.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="lv"></param>
        public static void LoadSettings(string settings, ListView lv)
        {
            string[] list = settings.Split('|');

            try
            {
                for (int i = 0; i < list.Length; i++)
                {
                    string[] s = list[i].Split(',');

                    lv.Columns[i].DisplayIndex = Int32.Parse(s[0]);
                    lv.Columns[i].Width = Int32.Parse(s[1]);
                }
            }
            catch
            { }
        }

        public static void LoadSettings(string settings, TreeViewAdv tv)
        {
            string[] list = settings.Split('|');

            try
            {
                TreeColumn[] columns = new TreeColumn[list.Length];

                for (int i = 0; i < list.Length; i++)
                {
                    string[] s = list[i].Split(',');

                    tv.Columns[i].Width = Int32.Parse(s[1]);
                    columns[Int32.Parse(s[0])] = tv.Columns[i];
                }

                tv.Columns.Clear();

                foreach (TreeColumn column in columns)
                    tv.Columns.Add(column);
            }
            catch
            { }
        }
    }
}
