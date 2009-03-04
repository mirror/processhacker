/*
 * Process Hacker - 
 *   log window
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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ProcessHacker
{
    public partial class LogWindow : Form
    {
        public LogWindow()
        {
            InitializeComponent();

            Misc.SetDoubleBuffered(listLog, typeof(ListView), true);
            Win32.SetWindowTheme(listLog.Handle, "explorer", null);
            listLog.ContextMenu = GenericViewMenu.GetMenu(listLog, listLog_RetrieveVirtualItem);
            listLog.KeyDown +=
                (sender, e) =>
                {
                    if (e.Control && e.KeyCode == Keys.A) Misc.SelectAll(listLog.Items);
                    if (e.Control && e.KeyCode == Keys.C) GenericViewMenu.ListViewCopy(listLog, -1, listLog_RetrieveVirtualItem);
                };

            this.UpdateLog();

            if (listLog.SelectedIndices.Count == 0 && listLog.VirtualListSize > 0)
                listLog.EnsureVisible(listLog.VirtualListSize - 1);

            Program.HackerWindow.LogUpdated += new HackerWindow.LogUpdatedEventHandler(HackerWindow_LogUpdated);

            this.Location = Properties.Settings.Default.LogWindowLocation;
            this.Size = Properties.Settings.Default.LogWindowSize;
        }

        private void HackerWindow_LogUpdated(KeyValuePair<DateTime, string>? value)
        {
            this.UpdateLog();
        }

        private void UpdateLog()
        {
            listLog.VirtualListSize = Program.HackerWindow.Log.Count;
        }

        private void LogWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                Properties.Settings.Default.LogWindowLocation = this.Location;
                Properties.Settings.Default.LogWindowSize = this.Size;
            }
            
            Program.HackerWindow.LogUpdated -= new HackerWindow.LogUpdatedEventHandler(HackerWindow_LogUpdated);
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void listLog_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            e.Item = new ListViewItem(new string[]
            {
                Program.HackerWindow.Log[e.ItemIndex].Key.ToString(),
                Program.HackerWindow.Log[e.ItemIndex].Value
            });
        }

        private void timerScroll_Tick(object sender, EventArgs e)
        {
            if (listLog.SelectedIndices.Count == 0 && listLog.VirtualListSize > 0)
                listLog.EnsureVisible(listLog.VirtualListSize - 1);
        }

        private void buttonCopy_Click(object sender, EventArgs e)
        {
            if (listLog.SelectedIndices.Count == 0)
                for (int i = 0; i < listLog.VirtualListSize; i++)
                    listLog.SelectedIndices.Add(i);

            GenericViewMenu.ListViewCopy(listLog, -1, listLog_RetrieveVirtualItem);
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();

            sfd.FileName = "Process Hacker Log.txt";
            sfd.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                StringBuilder sb = new StringBuilder();

                foreach (var value in Program.HackerWindow.Log)
                {
                    sb.AppendLine(value.Key.ToString() + ": " + value.Value);
                }

                try
                {
                    System.IO.File.WriteAllText(sfd.FileName, sb.ToString());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            Program.HackerWindow.ClearLog();
        }
    }
}
