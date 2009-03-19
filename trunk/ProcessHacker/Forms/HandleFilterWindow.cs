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
using System.Runtime.InteropServices;

namespace ProcessHacker
{
    public partial class HandleFilterWindow : Form
    {
        private HandleFilter currWorker;

        public HandleFilterWindow()
        {
            InitializeComponent();

            listHandles.ListViewItemSorter = new SortedListComparer(listHandles);
            GenericViewMenu.AddMenuItems(copyMenuItem.MenuItems, listHandles, null);
            listHandles.ContextMenu = menuHandle;

            Misc.SetDoubleBuffered(listHandles, typeof(ListView), true);
        }

        private void HandleFilterWindow_Load(object sender, EventArgs e)
        {
            ColumnSettings.LoadSettings(Properties.Settings.Default.FilterHandleListViewColumns, listHandles);
            this.Size = Properties.Settings.Default.FilterHandleWindowSize;
            Win32.SetWindowTheme(listHandles.Handle, "explorer", null);
            listHandles.KeyDown +=
                (sender_, e_) =>
                {
                    if (e_.Control && e_.KeyCode == Keys.A) Misc.SelectAll(listHandles.Items);
                    if (e_.Control && e_.KeyCode == Keys.C) GenericViewMenu.ListViewCopy(listHandles, -1);
                };
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
                menuHandle.DisableAll();
            }
            else if (listHandles.SelectedItems.Count == 1)
            {
                menuHandle.EnableAll();

                propertiesMenuItem.Enabled = false;

                string type = listHandles.SelectedItems[0].SubItems[1].Text;

                if (type == "Token" || type == "File" || type == "Event" || type == "Mutant" || type == "Section")
                    propertiesMenuItem.Enabled = true;
            }
            else
            {
                menuHandle.EnableAll();
                closeMenuItem.Enabled = false;
                propertiesMenuItem.Enabled = false;
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

        private void propertiesMenuItem_Click(object sender, EventArgs e)
        {
            string type = listHandles.SelectedItems[0].SubItems[1].Text;

            try
            {
                int handle = (int)BaseConverter.ToNumberParse(listHandles.SelectedItems[0].SubItems[3].Text);
                int pid = (int)listHandles.SelectedItems[0].Tag;

                using (Win32.ProcessHandle phandle = new Win32.ProcessHandle(
                    pid, Win32.PROCESS_RIGHTS.PROCESS_DUP_HANDLE))
                {
                    if (type == "Token")
                    {
                        TokenWindow tokForm = new TokenWindow(new Win32.RemoteTokenHandle(phandle, handle));

                        tokForm.Text = String.Format("Token - Handle 0x{0:x} owned by {1} (PID {2})",
                            short.Parse(listHandles.SelectedItems[0].Tag.ToString()),
                            Program.HackerWindow.ProcessProvider.Dictionary[pid].Name,
                            pid);
                        tokForm.ShowDialog();
                    }
                    else if (type == "File")
                    {
                        Win32.ShowProperties(listHandles.SelectedItems[0].SubItems[2].Text);
                    }
                    else if (type == "Event")
                    {
                        Win32.RemoteHandle rhandle = new Win32.RemoteHandle(phandle, handle);
                        int event_handle = rhandle.GetHandle((int)Win32.SYNC_RIGHTS.EVENT_QUERY_STATE);
                        Win32.EVENT_BASIC_INFORMATION ebi = new Win32.EVENT_BASIC_INFORMATION();
                        int retLen;

                        Win32.ZwQueryEvent(event_handle, Win32.EVENT_INFORMATION_CLASS.EventBasicInformation,
                            ref ebi, Marshal.SizeOf(ebi), out retLen);

                        InformationBox info = new InformationBox(
                            "Type: " + ebi.EventType.ToString().Replace("Event", "") +
                            "\r\nState: " + (ebi.EventState != 0 ? "True" : "False"));

                        info.ShowDialog();

                        Win32.CloseHandle(event_handle);
                    }
                    else if (type == "Mutant")
                    {
                        Win32.RemoteHandle rhandle = new Win32.RemoteHandle(phandle, handle);
                        int mutant_handle = rhandle.GetHandle((int)Win32.SYNC_RIGHTS.MUTEX_MODIFY_STATE);
                        Win32.MUTANT_BASIC_INFORMATION mbi = new Win32.MUTANT_BASIC_INFORMATION();
                        int retLen;

                        Win32.ZwQueryMutant(mutant_handle, Win32.MUTANT_INFORMATION_CLASS.MutantBasicInformation,
                            ref mbi, Marshal.SizeOf(mbi), out retLen);

                        InformationBox info = new InformationBox(
                            "Count: " + mbi.CurrentCount +
                            "\r\nOwned by Caller: " + (mbi.OwnedByCaller != 0 ? "True" : "False") +
                            "\r\nAbandoned: " + (mbi.AbandonedState != 0 ? "True" : "False"));

                        info.ShowDialog();

                        Win32.CloseHandle(mutant_handle);
                    }
                    else if (type == "Section")
                    {
                        Win32.RemoteHandle rhandle = new Win32.RemoteHandle(phandle, handle);
                        int section_handle = rhandle.GetHandle((int)Win32.SECTION_RIGHTS.SECTION_QUERY);
                        Win32.SECTION_BASIC_INFORMATION sbi = new Win32.SECTION_BASIC_INFORMATION();
                        Win32.SECTION_IMAGE_INFORMATION sii = new Win32.SECTION_IMAGE_INFORMATION();
                        int retLen;
                        int retVal;

                        Win32.ZwQuerySection(section_handle, Win32.SECTION_INFORMATION_CLASS.SectionBasicInformation,
                            ref sbi, Marshal.SizeOf(sbi), out retLen);
                        retVal = Win32.ZwQuerySection(section_handle, Win32.SECTION_INFORMATION_CLASS.SectionImageInformation,
                            ref sii, Marshal.SizeOf(sii), out retLen);

                        InformationBox info = new InformationBox(
                            "Attributes: " + Misc.FlagsToString(typeof(Win32.SECTION_ATTRIBUTES), (long)sbi.SectionAttributes) +
                            "\r\nSize: " + Misc.GetNiceSizeName(sbi.SectionSize) + " (" + sbi.SectionSize.ToString() + " B)" +

                            (retVal == 0 ? ("\r\n\r\nImage Entry Point: 0x" + sii.EntryPoint.ToString("x8") +
                            "\r\nImage Machine Type: " + ((PE.MachineType)sii.ImageMachineType).ToString() +
                            "\r\nImage Characteristics: " + ((PE.ImageCharacteristics)sii.ImageCharacteristics).ToString() +
                            "\r\nImage Subsystem: " + ((PE.ImageSubsystem)sii.ImageSubsystem).ToString() +
                            "\r\nStack Reserve: 0x" + sii.StackReserved.ToString("x")) : ""));

                        info.ShowDialog();

                        Win32.CloseHandle(section_handle);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void listHandles_DoubleClick(object sender, EventArgs e)
        {
            propertiesMenuItem_Click(sender, e);
        } 

        private void textFilter_TextChanged(object sender, EventArgs e)
        {
            if (textFilter.Text == "")
                buttonFind.Enabled = false;
            else
                buttonFind.Enabled = true;
        }
    }
}
