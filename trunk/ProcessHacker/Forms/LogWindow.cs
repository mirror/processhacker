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
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ProcessHacker.Common;
using ProcessHacker.UI;

namespace ProcessHacker
{
    public partial class LogWindow : Form
    {
        public LogWindow()
        {
            InitializeComponent();
            this.AddEscapeToClose();
            this.SetTopMost();

            listLog.SetDoubleBuffered(true);
            listLog.SetTheme("explorer");
            listLog.ContextMenu = listLog.GetCopyMenu(listLog_RetrieveVirtualItem);
            listLog.AddShortcuts(listLog_RetrieveVirtualItem);

            this.UpdateLog();

            if (listLog.SelectedIndices.Count == 0 && listLog.VirtualListSize > 0)
                listLog.EnsureVisible(listLog.VirtualListSize - 1);

            Program.HackerWindow.LogUpdated += new HackerWindow.LogUpdatedEventHandler(HackerWindow_LogUpdated);

            this.Size = Settings.Instance.LogWindowSize;
            this.Location = Utils.FitRectangle(new Rectangle(
                Settings.Instance.LogWindowLocation, this.Size), this).Location;
            checkAutoscroll.Checked = Settings.Instance.LogWindowAutoScroll;
        }

        private void HackerWindow_LogUpdated(KeyValuePair<DateTime, string>? value)
        {
            this.UpdateLog();
        }

        private void UpdateLog()
        {
            // HACK. Not my fault though, .NET wants to throw an exception when 
            // I set VirtualListSize and the window is minimized...
            try
            {
                listLog.VirtualListSize = Program.HackerWindow.Log.Count;
            }
            catch
            {
                // Do not put Logging.Log(ex) because this will cause a recursive call.
            }
        }

        private void LogWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                Settings.Instance.LogWindowLocation = this.Location;
                Settings.Instance.LogWindowSize = this.Size;
            }

            Settings.Instance.LogWindowAutoScroll = checkAutoscroll.Checked;
            
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
            if (checkAutoscroll.Checked)
            {
                if (!listLog.Focused)
                    listLog.SelectedIndices.Clear();

                if (listLog.SelectedIndices.Count == 0 && listLog.VirtualListSize > 0)
                    listLog.EnsureVisible(listLog.VirtualListSize - 1);
            }
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
                    PhUtils.ShowException("Unable to save the log", ex);
                }
            }
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            Program.HackerWindow.ClearLog();
        }

        private void listLog_DoubleClick(object sender, EventArgs e)
        {
            InformationBox info = new InformationBox(Program.HackerWindow.Log[listLog.SelectedIndices[0]].Value);

            info.ShowDialog();
        }
    }
}
