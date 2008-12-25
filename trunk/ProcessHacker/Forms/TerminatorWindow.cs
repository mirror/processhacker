/*
 * Process Hacker
 * 
 * Copyright (C) 2008 Dean
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
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ProcessHacker
{
    public partial class TerminatorWindow : Form
    {
        private int _pid;
        private List<string> _tests = new List<string>();

        public TerminatorWindow(int PID)
        {
            InitializeComponent();

            _pid = PID;

            labelProgress.Text = "";

            this.AddTest("TP1", "Terminates the process using TerminateProcess");
            this.AddTest("TP2", "Creates a remote thread in the process which terminates the process");
            this.AddTest("TT1", "Terminates the process' threads");
            this.AddTest("TT2", "Modifies the process' threads with invalid contexts");
            this.AddTest("M1", "Writes garbage to the process' memory regions"); 
            this.AddTest("M2", "Sets the page protection of the process' memory regions to PAGE_NOACCESS"); 
            this.AddTest("CH1", "Closes the process' handles");
        }

        private void AddTest(string id, string description)
        {
            ListViewItem item = new ListViewItem();

            item.Name = id; 
            item.Text = id;
            item.Tag = Delegate.CreateDelegate(typeof(MethodInvoker), this, id);
            item.ImageKey = "";

            item.SubItems.Add(new ListViewItem.ListViewSubItem(item, description));

            listTests.Items.Add(item);
            _tests.Add(id);
        }

        private bool RunTest(string id)
        {
            this.Cursor = Cursors.WaitCursor;

            ListViewItem item = listTests.Items[id];

            bool hadException = false;

            try
            {
                (item.Tag as Delegate).DynamicInvoke(null);
            }
            catch (Exception ex)
            {
                item.ToolTipText = ex.InnerException.Message;
                item.ImageKey = "cross";
                hadException = true;
            }

            this.Cursor = Cursors.Default;
            
            System.Threading.Thread.Sleep(1000);

            try
            {
                System.Diagnostics.Process.GetProcessById(_pid);

                if (!hadException)
                {
                    item.ToolTipText = "Process was not terminated.";
                    item.ImageKey = "cross";
                }
            }
            catch
            {
                // the process doesn't exist (or at least we think it doesn't)
                labelProgress.Text = "Process was terminated.";

                item.ToolTipText = "This test succeeded";
                item.ImageKey = "tick";

                return true;
            }

            // bad
            Application.DoEvents();

            return false;
        }

        private void TP1()
        {
            using (Win32.ProcessHandle phandle = new Win32.ProcessHandle(_pid, Win32.PROCESS_RIGHTS.PROCESS_TERMINATE))
                phandle.Terminate();
        }

        private void TP2()
        {
            int kernel32 = Win32.LoadLibrary("kernel32.dll");
            int exitProcess = Win32.GetProcAddress(kernel32, "ExitProcess");
            int threadId;

            using (Win32.ProcessHandle phandle = new Win32.ProcessHandle(_pid, Win32.PROCESS_RIGHTS.PROCESS_CREATE_THREAD))
                if (!Win32.CreateRemoteThread(phandle, 0, 0, exitProcess, 0, 0, out threadId))
                    throw new Exception(Win32.GetLastErrorMessage());
        }

        private void TT1()
        {
            System.Diagnostics.Process p = System.Diagnostics.Process.GetProcessById(_pid);

            foreach (System.Diagnostics.ProcessThread t in p.Threads)
            {
                using (Win32.ThreadHandle thandle = new Win32.ThreadHandle(t.Id, Win32.THREAD_RIGHTS.THREAD_TERMINATE))
                {
                    try
                    {
                        thandle.Terminate();
                    }
                    catch
                    { }
                }
            }
        }

        private void TT2()
        {
            Win32.CONTEXT context = new Win32.CONTEXT();

            context.ContextFlags = Win32.CONTEXT_FLAGS.CONTEXT_ALL;

            System.Diagnostics.Process p = System.Diagnostics.Process.GetProcessById(_pid);

            foreach (System.Diagnostics.ProcessThread t in p.Threads)
            {
                using (Win32.ThreadHandle thandle = new Win32.ThreadHandle(t.Id, Win32.THREAD_RIGHTS.THREAD_SET_CONTEXT))
                {
                    try
                    {
                        thandle.SetContext(context);
                    }
                    catch
                    { }
                }
            }
        }

        private void M1()
        {
            using (MemoryAlloc alloc = new MemoryAlloc(0x1000))
            {
                using (Win32.ProcessHandle phandle = new Win32.ProcessHandle(_pid, 
                    Win32.PROCESS_RIGHTS.PROCESS_QUERY_INFORMATION | 
                    Win32.PROCESS_RIGHTS.PROCESS_VM_OPERATION |
                    Win32.PROCESS_RIGHTS.PROCESS_VM_WRITE))
                {
                    Win32.MEMORY_BASIC_INFORMATION info = new Win32.MEMORY_BASIC_INFORMATION();
                    int address = 0;

                    while (true)
                    {
                        if (!Win32.VirtualQueryEx(phandle, address, ref info,
                            Marshal.SizeOf(typeof(Win32.MEMORY_BASIC_INFORMATION))))
                        {
                            break;
                        }
                        else
                        {
                            int written;
                            int old;

                            Win32.VirtualProtectEx(phandle, info.BaseAddress, info.RegionSize,
                                (int)Win32.MEMORY_PROTECTION.PAGE_READWRITE, out old);

                            for (int i = 0; i < info.RegionSize; i += 0x1000)
                                Win32.WriteProcessMemory(phandle, info.BaseAddress + i, alloc, 0x1000, out written);

                            address += info.RegionSize;
                        }
                    }
                }
            }
        }

        private void M2()
        {
            using (Win32.ProcessHandle phandle = new Win32.ProcessHandle(_pid, 
                Win32.PROCESS_RIGHTS.PROCESS_QUERY_INFORMATION | Win32.PROCESS_RIGHTS.PROCESS_VM_OPERATION))
            {
                Win32.MEMORY_BASIC_INFORMATION info = new Win32.MEMORY_BASIC_INFORMATION();
                int address = 0;

                while (true)
                {
                    if (!Win32.VirtualQueryEx(phandle, address, ref info,
                        Marshal.SizeOf(typeof(Win32.MEMORY_BASIC_INFORMATION))))
                    {
                        break;
                    }
                    else
                    {
                        int old;

                        Win32.VirtualProtectEx(phandle, info.BaseAddress, info.RegionSize,
                            (int)Win32.MEMORY_PROTECTION.PAGE_NOACCESS, out old);

                        address += info.RegionSize;
                    }
                }
            }
        }

        private void CH1()
        {
            using (Win32.ProcessHandle phandle = new Win32.ProcessHandle(_pid, Win32.PROCESS_RIGHTS.PROCESS_DUP_HANDLE))
            {
                int i = 0;

                while (true)
                {
                    if (i >= 0x1000)
                        break;

                    int handle;

                    Win32.DuplicateHandle(phandle, i, Program.CurrentProcess, out handle, 0, 0, 0x1 // close source
                        );

                    i++;
                }
            }
        }

        private void buttonRun_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to run the tests?", "Process Hacker",
                MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
                return;

            foreach (string test in _tests)
            {
                if (this.RunTest(test))
                    return;
            }
        }

        private void listTests_DoubleClick(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to run the selected test?", "Process Hacker",
                MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
                return;

            this.RunTest(listTests.SelectedItems[0].Name);
        }
    }
}
