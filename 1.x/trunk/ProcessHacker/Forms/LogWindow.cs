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

            ColumnSettings.LoadSettings(Settings.Instance.LogWindowListViewColumns, listLog);

            listLog.SetDoubleBuffered(true);
            listLog.SetTheme("explorer");
            listLog.ContextMenu = listLog.GetCopyMenu(listLog_RetrieveVirtualItem);
            listLog.AddShortcuts(listLog_RetrieveVirtualItem);

            ImageList imglist = new ImageList();
            imglist.ColorDepth = ColorDepth.Depth32Bit;
            imglist.Images.Add("Information", Properties.Resources.information);
            imglist.Images.Add("Warning", Properties.Resources.database_warn);
            imglist.Images.Add("Error", Properties.Resources.database_warn);
            imglist.Images.Add("Exception", Properties.Resources.database_ex);
            imglist.Images.Add("Debug", Properties.Resources.database_debug);
            listLog.SmallImageList = imglist;

            listLog.VirtualMode = true;
            listLog.VirtualListSize = HackerEvent.Log.Count;

            HackerEvent.HackerLogUpdated += new HackerLogUpdatedHandler(
                delegate
                {
                    listLog.VirtualListSize = HackerEvent.Log.Count;
                });

            if (listLog.SelectedIndices.Count == 0 && listLog.VirtualListSize > 0)
                listLog.EnsureVisible(listLog.VirtualListSize - 1);

            this.Size = Settings.Instance.LogWindowSize;
            this.Location = Utils.FitRectangle(new Rectangle(
                Settings.Instance.LogWindowLocation, this.Size), this).Location; 
            this.Icon = Icon.FromHandle(Properties.Resources.page_white_text.GetHicon());

            checkAutoscroll.Checked = Settings.Instance.LogWindowAutoScroll;
        }

        private void LogWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                Settings.Instance.LogWindowLocation = this.Location;
                Settings.Instance.LogWindowSize = this.Size;
            }

            Settings.Instance.LogWindowAutoScroll = checkAutoscroll.Checked;
                    }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void listLog_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            ListViewItem litem = new ListViewItem();
            EventType mType = HackerEvent.Log[e.ItemIndex].Key;

            litem.Text = mType.ToString();
            litem.SubItems.Add(HackerEvent.Log[e.ItemIndex].Value.Key.ToString());
            // Hack, Fix XP Listview displaying \r\n as chinese symbols by spliting the string 
            litem.SubItems.Add(HackerEvent.Log[e.ItemIndex].Value.Value.ToString().Split('\r')[0].ToString());

            switch (mType)
            {
                case EventType.Information:
                    litem.ImageIndex = 0;
                    break;
                case EventType.Warning:
                    litem.ImageIndex = 1;
                    break;
                case EventType.Error:
                    litem.ImageIndex = 2;
                    break;
                case EventType.Exception:
                    litem.ImageIndex = 3;
                    break;
                default:
                case EventType.Debug:
                    litem.ImageIndex = 4;
                    break;
            }

            e.Item = litem;
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

                foreach (var value in HackerEvent.Log)
                {
                    sb.AppendLine(value.ToString());
                }

                try
                {
                    System.IO.File.WriteAllText(sfd.FileName, sb.ToString());
                }
                catch (Exception ex)
                {
                    ex.LogEx(true, true, "Unable to save the log");
                }
            }
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            HackerEvent.Log.Clear();
        }

        private void listLog_DoubleClick(object sender, EventArgs e)
        {
            InformationBox info = new InformationBox(
                  HackerEvent.Log[listLog.SelectedIndices[0]].Value.Value.ToString()
                  );

            info.ShowDialog();
        }
    }
}
