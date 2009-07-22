/*
 * Process Hacker - 
 *   save processes
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

using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;
using ProcessHacker.Common;

namespace ProcessHacker
{
    internal static class Save
    {
        private const int TabSize = 8;

        public static void SaveToFile()
        {
            SaveFileDialog sfd = new SaveFileDialog();

            //sfd.Filter = "Text Files (*.txt;*.log)|*.txt;*.log|Comma-separated values (*.csv)|*.csv|HTML Files (*.htm;*.html)|*.htm;*.html|All Files (*.*)|*.*";
            sfd.Filter = "Text Files (*.txt;*.log)|*.txt;*.log|Comma-separated values (*.csv)|*.csv|All Files (*.*)|*.*";
            sfd.FileName = "Process Hacker.txt";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                FileInfo fi = new FileInfo(sfd.FileName);
                string ext = fi.Extension.ToLower();

                try
                {
                    using (StreamWriter sw = new StreamWriter(fi.FullName))
                    {
                        Program.HackerWindow.ProcessTree.Tree.ExpandAll();

                        if (ext == ".htm" || ext == ".html")
                        {

                        }
                        else if (ext == ".csv")
                        {
                            sw.Write(GetText(false));
                        }
                        else
                        {
                            sw.Write(GetText(true));
                        }
                    }
                }
                catch (IOException ex)
                {
                    PhUtils.ShowException("Unable to save the process list", ex);
                }
            }
        }

        private static string GetText(bool tabs)
        {
            StringBuilder sb = new StringBuilder();
            int items = Program.HackerWindow.ProcessTree.Tree.ItemCount + 1;
            int columns = 0;
            Dictionary<TreeColumn, int> columnIndexMap = new Dictionary<TreeColumn, int>();
            string[][] str = new string[items][];

            foreach (TreeColumn column in Program.HackerWindow.ProcessTree.Tree.Columns)
            {
                if (column.IsVisible && column.Header != "CPU History" && column.Header != "I/O History")
                {
                    columnIndexMap[column] = columns;
                    columns++;
                }
            }

            for (int i = 0; i < items; i++)
                str[i] = new string[columns];

            foreach (var column in Program.HackerWindow.ProcessTree.Tree.Columns)
            {
                if (columnIndexMap.ContainsKey(column))
                    str[0][columnIndexMap[column]] = column.Header;
            }

            {
                int i = 0;

                foreach (var node in Program.HackerWindow.ProcessTree.Tree.AllNodes)
                {
                    foreach (var control in Program.HackerWindow.ProcessTree.Tree.NodeControls)
                    {
                        if (!control.ParentColumn.IsVisible || !(control is BaseTextControl))
                            continue;

                        string text = (control as BaseTextControl).GetLabel(node);
                        int columnIndex = columnIndexMap[control.ParentColumn];

                        str[i + 1][columnIndex] = (columnIndex == 0 ? (new string(' ', (node.Level - 1) * 2)) : "") + 
                            (text == null ? "" : text);
                    }

                    i++;
                }
            }

            int[] tabCount = new int[columns];

            for (int i = 0; i < items; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    int newCount = str[i][j].Length / TabSize;

                    if (newCount > tabCount[j])
                        tabCount[j] = newCount;
                }
            }

            for (int i = 0; i < items; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (tabs)
                    {
                        sb.Append(str[i][j]);
                        sb.Append('\t', tabCount[j] - str[i][j].Length / TabSize + 1);
                    }
                    else
                    {
                        sb.Append("\"");
                        sb.Append(str[i][j].Replace("\"", "\\\""));
                        sb.Append("\"");

                        if (j != columns - 1)
                            sb.Append(",");
                    }
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
