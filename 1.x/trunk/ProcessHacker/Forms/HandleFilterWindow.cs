/*
 * Process Hacker - 
 *   handle filter user interface
 * 
 * Copyright (C) 2008 Dean
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
using System.Collections.Generic;
using System.Windows.Forms;
using ProcessHacker.Common;
using ProcessHacker.Common.Ui;
using ProcessHacker.Components;
using ProcessHacker.FormHelper;
using ProcessHacker.Native;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;
using ProcessHacker.UI;
using ProcessHacker.UI.Actions;

namespace ProcessHacker
{
    public partial class HandleFilterWindow : Form
    {
        private HandleFilter currWorker;

        public HandleFilterWindow()
        {
            InitializeComponent();
            this.AddEscapeToClose();
            this.SetTopMost();

            listHandles.SetDoubleBuffered(true);
            listHandles.SetTheme("explorer");
            GenericViewMenu.AddMenuItems(copyMenuItem.MenuItems, listHandles, null);
            listHandles.ContextMenu = menuHandle;

            var comparer = (SortedListViewComparer)(listHandles.ListViewItemSorter = new SortedListViewComparer(listHandles));

            comparer.ColumnSortOrder.Add(0);
            comparer.ColumnSortOrder.Add(1);
            comparer.ColumnSortOrder.Add(2);
            comparer.ColumnSortOrder.Add(3);
        }

        private void HandleFilterWindow_Load(object sender, EventArgs e)
        {
            ColumnSettings.LoadSettings(Settings.Instance.HandleFilterWindowListViewColumns, listHandles);
            this.Size = Settings.Instance.HandleFilterWindowSize;
            this.Location = Utils.FitRectangle(new System.Drawing.Rectangle(
                Settings.Instance.HandleFilterWindowLocation, this.Size), this).Location;
            listHandles.AddShortcuts();
        }

        private void HandleFilterWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (OSVersion.HasExtendedTaskbar)
            {
                TaskbarLib.Windows7Taskbar.SetTaskbarProgressState(
                    Program.HackerWindowHandle,
                    TaskbarLib.Windows7Taskbar.ThumbnailProgressState.NoProgress
                    );
            }

            Settings.Instance.HandleFilterWindowListViewColumns = ColumnSettings.SaveSettings(listHandles);

            if (this.WindowState == FormWindowState.Normal)
            {
                Settings.Instance.HandleFilterWindowSize = this.Size;
                Settings.Instance.HandleFilterWindowLocation = this.Location;
            }

            e.Cancel = true;
            this.Visible = false;
        }

        private void HandleFilterWindow_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                this.SetPhParent();
                textFilter.SelectAll();
            }
        }

        private void menuHandle_Popup(object sender, EventArgs e)
        {
            if (listHandles.SelectedItems.Count == 0)
            {
                menuHandle.DisableAll();
            }
            else if (listHandles.SelectedItems.Count == 1)
            {
                menuHandle.EnableAll();

                string type = listHandles.SelectedItems[0].SubItems[1].Text;

                if (type == "DLL" || type == "Mapped File")
                    closeMenuItem.Enabled = false;
            }
            else
            {
                menuHandle.EnableAll();
                processPropertiesMenuItem.Enabled = false;
                propertiesMenuItem.Enabled = false;

                string firstProcess = listHandles.SelectedItems[0].SubItems[0].Text;
                bool good = true;

                foreach (ListViewItem item in listHandles.SelectedItems)
                {
                    if (item.SubItems[0].Text != firstProcess)
                    {
                        good = false;
                        break;
                    }
                }

                if (good)
                    processPropertiesMenuItem.Enabled = true;
            }
        }

        private void closeMenuItem_Click(object sender, EventArgs e)
        {
            List<ListViewItem> remove = new List<ListViewItem>();

            foreach (int index in listHandles.SelectedIndices)
            {
                if (listHandles.Items[index].SubItems[1].Text == "DLL" ||
                    listHandles.Items[index].SubItems[1].Text == "Mapped File")
                    continue;

                try
                {
                    IntPtr handle = new IntPtr((int)BaseConverter.ToNumberParse(listHandles.Items[index].SubItems[3].Text));

                    using (ProcessHandle process =
                           new ProcessHandle(((SystemHandleEntry)listHandles.SelectedItems[0].Tag).ProcessId, 
                               ProcessAccess.DupHandle))
                    {
                        Win32.DuplicateObject(process.Handle, handle, 0, 0, DuplicateOptions.CloseSource);
                        remove.Add(listHandles.Items[index]);
                    }
                }
                catch (Exception ex)
                {
                    if (!PhUtils.ShowContinueMessage(
                        "Unable to close the handle \"" + listHandles.Items[index].SubItems[2].Text + "\"",
                        ex
                        ))
                        return;
                }
            }

            foreach (ListViewItem item in remove)
                item.Remove();
        }

        private void buttonFind_Click(object sender, EventArgs e)
        {
            if (currWorker == null)
            {
                progress.Visible = true;
                progress.Minimum = 0;
                listHandles.Items.Clear();
                currWorker = new HandleFilter(this, textFilter.Text);
                currWorker.Completed += new EventHandler(Filter_Finished);
                currWorker.Cancelled += new EventHandler(Filter_Cancelled);
                currWorker.MatchListView += new HandleFilter.MatchListViewEvent(ListView_Result);
                currWorker.MatchProgress += new HandleFilter.MatchProgressEvent(Progress_Result);                
                currWorker.Failed += new System.Threading.ThreadExceptionEventHandler(Filter_Failed);
                buttonFind.Text = "&Cancel";
                Cursor = Cursors.AppStarting;
                currWorker.Start();
            }
            else
            {    
                if (OSVersion.HasExtendedTaskbar)
                {
                    TaskbarLib.Windows7Taskbar.SetTaskbarProgressState(
                        Program.HackerWindowHandle,
                        TaskbarLib.Windows7Taskbar.ThumbnailProgressState.NoProgress
                        );
                }

                progress.Visible = false;
                Cursor = Cursors.WaitCursor;
                currWorker.CancelAndWait();
                Cursor = Cursors.Default;
            }
        }

        private void Filter_Finished(object sender, EventArgs e)
        {
            progress.Visible = false;
            ResetCtls();            
        }

        private void Filter_Cancelled(object sender, EventArgs e)
        {
            ResetCtls();
        }

        private void Filter_Failed(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            progress.Visible = false;
            ResetCtls();
            //log
        }

        private void ResetCtls()
        {
            if (OSVersion.HasExtendedTaskbar)
            {
                TaskbarLib.Windows7Taskbar.SetTaskbarProgressState(
                    Program.HackerWindowHandle,
                    TaskbarLib.Windows7Taskbar.ThumbnailProgressState.NoProgress
                    );
            }

            buttonFind.Text = "&Find";
            currWorker = null;
            Cursor = Cursors.Default;
        }

        private void ListView_Result(List<ListViewItem> items)
        {
            listHandles.Items.AddRange(items.ToArray());
        }

        private void Progress_Result(int currentValue, int count)
        {
            progress.Value = currentValue;
            progress.Maximum = count;

            if (OSVersion.HasExtendedTaskbar)
            {
                TaskbarLib.Windows7Taskbar.SetTaskbarProgress(
                    Program.HackerWindowHandle,
                    (ulong)currentValue,
                    (ulong)count
                    );
            }
        }

        private void processPropertiesMenuItem_Click(object sender, EventArgs e)
        {
            string type = listHandles.SelectedItems[0].SubItems[1].Text;
            int pid;

            if (type == "DLL" || type == "Mapped File")
                pid = (int)listHandles.SelectedItems[0].Tag;
            else
                pid = ((SystemHandleEntry)listHandles.SelectedItems[0].Tag).ProcessId;

            if (Program.ProcessProvider.Dictionary.ContainsKey(pid))
            {
                ProcessActions.ShowProperties(this, pid, Program.ProcessProvider.Dictionary[pid].Name);
            }
            else
            {
                PhUtils.ShowError("The process does not exist.");
            }
        }

        private void propertiesMenuItem_Click(object sender, EventArgs e)
        {
            string type = listHandles.SelectedItems[0].SubItems[1].Text;

            if (type == "DLL" || type == "Mapped File")
            {
                FileUtils.ShowProperties(listHandles.SelectedItems[0].SubItems[2].Text);
                return;
            }

            try
            {
                HandleList.ShowHandleProperties(
                    (SystemHandleEntry)listHandles.SelectedItems[0].Tag
                    );
            }
            catch (Exception ex)
            {
                PhUtils.ShowException("Unable to show handle properties", ex);
            }
        }

        private void listHandles_DoubleClick(object sender, EventArgs e)
        {
            propertiesMenuItem_Click(sender, e);
        }

        private void listHandles_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (listHandles.SelectedIndices.Count == 0)
                    return;

                if (HandleList.ConfirmHandleClose())
                {
                    closeMenuItem_Click(sender, null);
                }
            }
        }

        private void textFilter_TextChanged(object sender, EventArgs e)
        {
            if (textFilter.Text == "")
                buttonFind.Enabled = false;
            else
                buttonFind.Enabled = true;
        }

        private void textFilter_Enter(object sender, EventArgs e)
        {
            // Select all *after* this event, since the selection due to 
            // the user's mouse click will be made after this code 
            // executes.
            this.BeginInvoke(new MethodInvoker(textFilter.SelectAll));
        }

        private void textFilter_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Prevent the beep when the user presses Escape.
            if (e.KeyChar == (char)Keys.Escape)
                e.Handled = true;
        }
    }
}
