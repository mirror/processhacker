/*
 * Process Hacker - 
 *   terminator tool
 * 
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
using System.Windows.Forms;
using ProcessHacker.Common;
using ProcessHacker.Native;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;

namespace ProcessHacker
{
    public partial class TerminatorWindow : Form
    {
        private int _pid;
        private List<string> _tests = new List<string>();

        public TerminatorWindow(int PID)
        {
            InitializeComponent();
            this.AddEscapeToClose();
            this.SetTopMost();

            _pid = PID;

            labelProgress.Text = "Double-click a termination method or click Run.";

            this.AddTest("TP1", "Terminates the process using NtTerminateProcess");
            this.AddTest("TP2", "Creates a remote thread in the process which terminates the process");
            this.AddTest("TT1", "Terminates the process' threads");
            this.AddTest("TT2", "Modifies the process' threads with contexts which terminate the process");
            if (OSVersion.IsAboveOrEqual(WindowsVersion.Server2003))
            {
                this.AddTest("TP1a", "Terminates the process using NtTerminateProcess (alternative method)");
                this.AddTest("TT1a", "Terminates the process' threads (alternative method)");
            }
            this.AddTest("CH1", "Closes the process' handles");
            this.AddTest("W1", "Sends the WM_DESTROY message to the process' windows");
            this.AddTest("W2", "Sends the WM_QUIT message to the process' windows");
            this.AddTest("TJ1", "Assigns the process to a job object and terminates the job");
            this.AddTest("TD1", "Debugs the process and closes the debug object");
            this.AddTest("TP3", "Terminates the process in kernel-mode (if possible)");
            this.AddTest("TT3", "Terminates the process' threads in kernel-mode (if possible)");
            if (KProcessHacker.Instance != null)
                this.AddTest("TT4", "Terminates the process' threads using a dangerous kernel-mode method");
            this.AddTest("M1", "Writes garbage to the process' memory regions");
            this.AddTest("M2", "Sets the page protection of the process' memory regions to PAGE_NOACCESS"); 
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

            // HACK
            Application.DoEvents();

            return false;
        }

        private void CH1()
        {
            using (ProcessHandle phandle = new ProcessHandle(_pid, ProcessAccess.DupHandle))
            {
                int i = 0;

                while (true)
                {
                    if (i >= 0x1000)
                        break;

                    try
                    {
                        Win32.DuplicateObject(phandle, new IntPtr(i), 0, 0, DuplicateOptions.CloseSource);
                    }
                    catch
                    { }

                    i++;
                }
            }
        }

        private void M1()
        {
            this.M1Internal();
        }

        private unsafe void M1Internal()
        {
            using (MemoryAlloc alloc = new MemoryAlloc(0x1000))
            {
                using (ProcessHandle phandle = new ProcessHandle(_pid,
                    ProcessAccess.QueryInformation |
                    Program.MinProcessWriteMemoryRights))
                {
                    phandle.EnumMemory((info) =>
                    {
                        for (int i = 0; i < info.RegionSize.ToInt32(); i += 0x1000)
                        {
                            try
                            {
                                phandle.WriteMemory(info.BaseAddress.Increment(i), (IntPtr)alloc, 0x1000);
                            }
                            catch
                            { }
                        }

                        return true;
                    });
                }
            }
        }

        private void M2()
        {
            using (ProcessHandle phandle = new ProcessHandle(_pid,
                ProcessAccess.QueryInformation | ProcessAccess.VmOperation))
            {
                phandle.EnumMemory((info) =>
                {
                    phandle.ProtectMemory(info.BaseAddress, info.RegionSize.ToInt32(), MemoryProtection.NoAccess);
                    return true;
                });
            }
        }

        private void TD1()
        {
            using (var dhandle =
                DebugObjectHandle.Create(DebugObjectAccess.ProcessAssign, DebugObjectFlags.KillOnClose))
            {
                using (var phandle = new ProcessHandle(_pid, ProcessAccess.SuspendResume))
                    phandle.Debug(dhandle);
            }
        }

        private void TJ1()
        {
            if (KProcessHacker.Instance != null)
            {
                try
                {
                    using (var phandle = new ProcessHandle(_pid, Program.MinProcessQueryRights))
                    {
                        var jhandle = phandle.GetJobObject(JobObjectAccess.Query | JobObjectAccess.Terminate);

                        if (jhandle != null)
                        {
                            // Make sure we're not terminating more than one process
                            if (jhandle.GetProcessIdList().Length == 1)
                            {
                                jhandle.Terminate();
                                return;
                            }
                        }
                    }
                }
                catch
                { }
            }

            using (var jhandle = JobObjectHandle.Create(JobObjectAccess.AssignProcess | JobObjectAccess.Terminate))
            {
                using (ProcessHandle phandle =
                    new ProcessHandle(_pid, ProcessAccess.SetQuota | ProcessAccess.Terminate))
                {
                    phandle.AssignToJobObject(jhandle);
                }

                jhandle.Terminate();
            }
        }

        private void TP1()
        {
            using (ProcessHandle phandle = new ProcessHandle(_pid, ProcessAccess.Terminate))
            {
                // Don't use KPH.
                Win32.NtTerminateProcess(phandle, NtStatus.Success).ThrowIf();
            }
        }

        private void TP1a()
        {
            ProcessHandle phandle = ProcessHandle.Current;
            bool found = false;

            // Loop through the processes until we find our target process.
            for (int count = 0; count < 1000; count++)
            {
                try
                {
                    phandle = phandle.GetNextProcess(Program.MinProcessQueryRights | ProcessAccess.Terminate);

                    if (phandle.GetProcessId() == _pid)
                    {
                        found = true;
                        break;
                    }
                }
                catch
                { }
            }

            if (found)
                phandle.Terminate();
        }

        private void TP2()
        {
            using (ProcessHandle phandle = new ProcessHandle(_pid,
                ProcessAccess.CreateThread | ProcessAccess.VmOperation | ProcessAccess.VmWrite))
            {
                if (OSVersion.IsAboveOrEqual(WindowsVersion.Vista))
                {
                    // Vista and above export.
                    phandle.CreateThread(Loader.GetProcedure("ntdll.dll", "RtlExitUserProcess"), IntPtr.Zero);
                }
                else
                {
                    phandle.CreateThread(Loader.GetProcedure("kernel32.dll", "ExitProcess"), IntPtr.Zero);
                }
            }
        }

        private void TP3()
        {
            using (ProcessHandle phandle = new ProcessHandle(_pid, Program.MinProcessQueryRights))
            {
                phandle.Terminate();
            }
        }

        private void TT1()
        {
            foreach (var thread in Windows.GetProcessThreads(_pid).Values)
            {
                using (ThreadHandle thandle = new ThreadHandle(thread.ClientId.ThreadId, ThreadAccess.Terminate))
                {
                    // Don't use KPH.
                    Win32.NtTerminateThread(thandle, NtStatus.Success).ThrowIf();
                }
            }
        }

        private void TT1a()
        {
            using (var phandle = new ProcessHandle(_pid, ProcessAccess.QueryInformation))
            {
                ThreadHandle thandle = null;

                // Loop through the process' threads and terminate each one.
                for (int count = 0; count < 1000; count++)
                {
                    try
                    {
                        thandle = phandle.GetNextThread(thandle, ThreadAccess.Terminate);
                        thandle.Terminate();
                    }
                    catch
                    { }
                }
            }
        }

        private void TT2()
        {
            Context context;
            IntPtr exitProcess = Loader.GetProcedure("kernel32.dll", "ExitProcess");

            foreach (var thread in Windows.GetProcessThreads(_pid).Values)
            {
                using (ThreadHandle thandle = new ThreadHandle(thread.ClientId.ThreadId,
                    ThreadAccess.GetContext | ThreadAccess.SetContext))
                {
                    try
                    {
                        context = thandle.GetContext(ContextFlags.Control);
                        context.ContextFlags = ContextFlags.Control;
                        context.Eip = exitProcess.ToInt32();
                        thandle.SetContext(context);
                    }
                    catch
                    { }
                }
            }
        }

        private void TT3()
        {
            foreach (var thread in Windows.GetProcessThreads(_pid).Values)
            {
                using (ThreadHandle thandle = new ThreadHandle(thread.ClientId.ThreadId, ThreadAccess.Terminate))
                {
                    thandle.Terminate();
                }
            }
        }

        private void TT4()
        {
            foreach (var thread in Windows.GetProcessThreads(_pid).Values)
            {
                using (ThreadHandle thandle = new ThreadHandle(thread.ClientId.ThreadId, ThreadAccess.Terminate))
                {
                    thandle.DangerousTerminate(NtStatus.Success);
                }
            }
        }

        private void W1()
        {
            WindowHandle.Enumerate((window) =>
                {
                    if (window.GetClientId().ProcessId == _pid)
                        window.PostMessage(WindowMessage.Destroy, 0, 0);

                    return true;
                });
        }

        private void W2()
        {
            WindowHandle.Enumerate((window) =>
            {
                if (window.GetClientId().ProcessId == _pid)
                    window.PostMessage(WindowMessage.Quit, 0, 0);

                return true;
            });
        }

        private void buttonRun_Click(object sender, EventArgs e)
        {
            if (!PhUtils.ShowConfirmMessage("run", "the tests", null, false))
                return;

            foreach (string test in _tests)
            {
                if (test == "TT4")
                {
                    if (!PhUtils.ShowConfirmMessage(
                        "run",
                        "the TT4 test",
                        "This test may cause the system to crash.",
                        true
                        ))
                        continue;
                }

                if (this.RunTest(test))
                    return;
            }
        }

        private void listTests_DoubleClick(object sender, EventArgs e)
        {
            if (!PhUtils.ShowConfirmMessage("run", "the selected test", null, false))
                return;

            this.RunTest(listTests.SelectedItems[0].Name);
        }
    }
}
