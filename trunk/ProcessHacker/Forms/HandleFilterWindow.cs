/*
 * Process Hacker - 
 *   handle filter user interface
 * 
 * Copyright (C) 2008 Dean
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ProcessHacker.FormHelper;
using System.Collections;

namespace ProcessHacker
{
    public partial class HandleFilterWindow : Form
    {
        private HandleFilter currWorker;

        public HandleFilterWindow()
        {
            InitializeComponent();

            GenericViewMenu.AddMenuItems(copyMenuItem.MenuItems, listHandles, null);
            listHandles.ContextMenu = menuHandle;

            Misc.SetDoubleBuffered(listHandles, typeof(ListView), true);
        }

        private void HandleFilterWindow_Load(object sender, EventArgs e)
        {
            ColumnSettings.LoadSettings(Properties.Settings.Default.FilterHandleListViewColumns, listHandles);
            this.Size = Properties.Settings.Default.FilterHandleWindowSize;
        }

        private void HandleFilterWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.FilterHandleListViewColumns = ColumnSettings.SaveSettings(listHandles);
            Properties.Settings.Default.FilterHandleWindowSize = this.Size;

            e.Cancel = true;
            this.Visible = false;
        }

        private void menuHandle_Popup(object sender, EventArgs e)
        {
            if (listHandles.SelectedItems.Count == 0)
            {
                Misc.DisableAllMenuItems(menuHandle);
            }
            else if (listHandles.SelectedItems.Count == 1)
            {
                Misc.EnableAllMenuItems(menuHandle);
            }
            else
            {
                Misc.DisableAllMenuItems(menuHandle);

                copyMenuItem.Enabled = true;
            }
        }

        private void closeMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                int handle = (int)BaseConverter.ToNumberParse(listHandles.SelectedItems[0].SubItems[3].Text);

                using (Win32.ProcessHandle process =
                       new Win32.ProcessHandle((int)listHandles.SelectedItems[0].Tag, Win32.PROCESS_RIGHTS.PROCESS_DUP_HANDLE))
                {
                    if (Win32.ZwDuplicateObject(process.Handle, handle, 0, 0, 0, 0,
                        0x1 // DUPLICATE_CLOSE_SOURCE
                        ) != 0)
                        Win32.ThrowLastWin32Error();

                    listHandles.SelectedItems[0].Remove();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not close handle:\n\n" + ex.Message,
                     "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }
        }

        #region  buttonFind_ClickOld
        //private void buttonFind_Click(object sender, EventArgs e)
        //{
        //    buttonFind.Enabled = false;
        //    this.UseWaitCursor = true;
        //    progress.Visible = true;
        //    Application.DoEvents();
        //    listHandles.BeginUpdate();
        //    listHandles.Items.Clear();
                                       
        //    Win32.SYSTEM_HANDLE_INFORMATION[] handles = null;

        //    try
        //    {
        //        handles = Win32.EnumHandles();
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }

        //    Dictionary<int, Win32.ProcessHandle> processHandles = new Dictionary<int, Win32.ProcessHandle>();

            

        //    for (int i = 0; i < handles.Length; i++)
        //    {
        //        Win32.SYSTEM_HANDLE_INFORMATION handle = handles[i];

        //        progress.Value = i;

        //        try
        //        {
        //            try
        //            {
        //                if (Win32.GetProcessSessionId(handle.ProcessId) != Program.CurrentSessionId)
        //                    continue;
        //            }
        //            catch
        //            {
        //                continue;
        //            }

        //            if (!processHandles.ContainsKey(handle.ProcessId))
        //                processHandles.Add(handle.ProcessId, 
        //                    new Win32.ProcessHandle(handle.ProcessId, Win32.PROCESS_RIGHTS.PROCESS_DUP_HANDLE));

        //            Win32.ObjectInformation info = Win32.GetHandleInfo(processHandles[handle.ProcessId], handle);

        //            if (!info.BestName.ToLower().Contains(textFilter.Text.ToLower()))
        //                continue;

        //            ListViewItem item = new ListViewItem();

        //            item.Name = handle.Handle.ToString();
        //            item.Text = Program.HackerWindow.ProcessList.Items[handle.ProcessId.ToString()].Text +
        //                " (" + handle.ProcessId.ToString() + ")";
        //            item.Tag = handle.ProcessId;
        //            item.SubItems.Add(new ListViewItem.ListViewSubItem(item, info.TypeName));
        //            item.SubItems.Add(new ListViewItem.ListViewSubItem(item, info.BestName));
        //            item.SubItems.Add(new ListViewItem.ListViewSubItem(item, "0x" + handle.Handle.ToString("x")));

        //            listHandles.Items.Add(item);
        //            Application.DoEvents();
        //        }
        //        catch
        //        {
        //            continue;
        //        }
        //    }

        //    foreach (Win32.ProcessHandle phandle in processHandles.Values)
        //        phandle.Dispose();

        //    listHandles.EndUpdate();
        //    progress.Visible = false;
        //    this.UseWaitCursor = false;
        //    buttonFind.Enabled = true;
        //}
        #endregion 
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
        }
       
    }
}
