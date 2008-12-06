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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ProcessHacker
{
    public partial class HandleFilterWindow : Form
    {
        public HandleFilterWindow()
        {
            InitializeComponent();

            ListViewMenu.AddMenuItems(copyMenuItem.MenuItems, listHandles, null);
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
                        throw new Exception(Win32.GetLastErrorMessage());

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

        private void buttonFind_Click(object sender, EventArgs e)
        {
            buttonFind.Enabled = false;
            this.UseWaitCursor = true;
            progress.Visible = true;
            Application.DoEvents();
            listHandles.BeginUpdate();
                                       
            Win32.SYSTEM_HANDLE_INFORMATION[] handles = null;

            try
            {
                handles = Win32.EnumHandles();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            Dictionary<int, Win32.ProcessHandle> processHandles = new Dictionary<int, Win32.ProcessHandle>();

            progress.Minimum = 0;
            progress.Maximum = handles.Length;

            for (int i = 0; i < handles.Length; i++)
            {
                Win32.SYSTEM_HANDLE_INFORMATION handle = handles[i];

                progress.Value = i;

                try
                {
                    if (handle.ProcessId == 4)
                        continue;

                    if (!processHandles.ContainsKey(handle.ProcessId))
                        processHandles.Add(handle.ProcessId, 
                            new Win32.ProcessHandle(handle.ProcessId, Win32.PROCESS_RIGHTS.PROCESS_DUP_HANDLE));

                    Win32.ObjectInformation info = Win32.GetHandleInfo(processHandles[handle.ProcessId], handle);

                    if (!info.BestName.ToLower().Contains(textFilter.Text.ToLower()))
                        continue;

                    ListViewItem item = new ListViewItem();

                    item.Name = handle.Handle.ToString();
                    item.Text = Program.HackerWindow.ProcessList.Items[handle.ProcessId.ToString()].Text +
                        " (" + handle.ProcessId.ToString() + ")";
                    item.Tag = handle.ProcessId;
                    item.SubItems.Add(new ListViewItem.ListViewSubItem(item, info.TypeName));
                    item.SubItems.Add(new ListViewItem.ListViewSubItem(item, info.BestName));
                    item.SubItems.Add(new ListViewItem.ListViewSubItem(item, "0x" + handle.Handle.ToString("x")));

                    listHandles.Items.Add(item);
                    Application.DoEvents();
                }
                catch
                {
                    continue;
                }
            }

            foreach (Win32.ProcessHandle phandle in processHandles.Values)
                phandle.Dispose();

            listHandles.EndUpdate();
            progress.Visible = false;
            this.UseWaitCursor = false;
            buttonFind.Enabled = true;
        }
    }
}
