/*
 * Process Hacker - 
 *   thread handle
 * 
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
using System.Diagnostics;
using System.Runtime.InteropServices;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Security;

namespace ProcessHacker.Native.Objects
{
    /// <summary>
    /// Represents a handle to a Windows thread.
    /// </summary>
    public class ThreadHandle : Win32Handle<ThreadAccess>, IWithToken
    {
        public delegate bool WalkStackDelegate(ThreadStackFrame stackFrame);

        public static ThreadHandle Create(
            ThreadAccess access,
            string name,
            ObjectFlags objectFlags,
            DirectoryHandle rootDirectory,
            ProcessHandle processHandle,
            out ClientId clientId,
            ref Context threadContext,
            ref InitialTeb initialTeb,
            bool createSuspended
            )
        {
            NtStatus status;
            ObjectAttributes oa = new ObjectAttributes(name, objectFlags, rootDirectory);
            IntPtr handle;

            try
            {
                if ((status = Win32.NtCreateThread(
                    out handle,
                    access,
                    ref oa,
                    processHandle,
                    out clientId,
                    ref threadContext,
                    ref initialTeb,
                    createSuspended
                    )) >= NtStatus.Error)
                    Win32.ThrowLastError(status);
            }
            finally
            {
                oa.Dispose();
            }

            return new ThreadHandle(handle, true);
        }

        /// <summary>
        /// Creates a thread handle using an existing handle. 
        /// The handle will not be closed automatically.
        /// </summary>
        /// <param name="Handle">The handle value.</param>
        /// <returns>The thread handle.</returns>
        public static ThreadHandle FromHandle(IntPtr handle)
        {
            return new ThreadHandle(handle, false);
        }

        /// <summary>
        /// Gets a handle to the current thread.
        /// </summary>
        /// <returns>A thread handle.</returns>
        public static ThreadHandle GetCurrent()
        {
            return new ThreadHandle(new IntPtr(-2), false);
        }

        public static void RegisterTerminationPort(PortHandle portHandle)
        {
            NtStatus status;

            if ((status = Win32.NtRegisterThreadTerminatePort(portHandle)) >= NtStatus.Error)
                Win32.ThrowLastError(status);
        }

        public static void TestAlert()
        {
            NtStatus status;

            if ((status = Win32.NtTestAlert()) >= NtStatus.Error)
                Win32.ThrowLastError(status);
        }

        internal ThreadHandle(IntPtr handle, bool owned)
            : base(handle, owned)
        { }

        /// <summary>
        /// Creates a new thread handle.
        /// </summary>
        /// <param name="TID">The ID of the thread to open.</param>
        public ThreadHandle(int tid)
            : this(tid, ThreadAccess.All)
        { }

        /// <summary>
        /// Creates a new thread handle.
        /// </summary>
        /// <param name="TID">The ID of the thread to open.</param>
        /// <param name="access">The desired access to the thread.</param>
        public ThreadHandle(int tid, ThreadAccess access)
        {
            if (KProcessHacker.Instance != null)
            {
                try
                {
                    this.Handle = new IntPtr(KProcessHacker.Instance.KphOpenThread(tid, access));
                }
                catch (WindowsException)
                {
                    // Open the thread with minimum access (SYNCHRONIZE) and set the granted access.
                    this.Handle = new IntPtr(KProcessHacker.Instance.KphOpenThread(tid, 
                        (ThreadAccess)StandardRights.Synchronize));
                    KProcessHacker.Instance.KphSetHandleGrantedAccess(this.Handle, (int)access);
                }
            }
            else
            {
                this.Handle = Win32.OpenThread(access, false, tid);
            }

            if (this.Handle == IntPtr.Zero)
                Win32.ThrowLastError();
        }

        public ThreadHandle(
            string name,
            ObjectFlags objectFlags,
            DirectoryHandle rootDirectory, 
            ClientId clientId, 
            ThreadAccess access
            )
        {
            NtStatus status;
            ObjectAttributes oa = new ObjectAttributes(name, objectFlags, rootDirectory);
            IntPtr handle;

            try
            {
                if (clientId.ProcessId == 0 && clientId.ThreadId == 0)
                {
                    if ((status = Win32.NtOpenThread(
                        out handle,
                        access,
                        ref oa,
                        IntPtr.Zero
                        )) >= NtStatus.Error)
                        Win32.ThrowLastError(status);
                }
                else
                {
                    if ((status = Win32.NtOpenThread(
                        out handle,
                        access,
                        ref oa,
                        ref clientId
                        )) >= NtStatus.Error)
                        Win32.ThrowLastError(status);
                }
            }
            finally
            {
                oa.Dispose();
            }

            this.Handle = handle;
        }

        public ThreadHandle(string name, ThreadAccess access)
            : this(name, 0, null, new ClientId(), access)
        { }

        /// <summary>
        /// Puts the thread in an alerted state.
        /// </summary>
        public void Alert()
        {
            NtStatus status;

            if ((status = Win32.NtAlertThread(this)) >= NtStatus.Error)
                Win32.ThrowLastError(status);
        }

        /// <summary>
        /// Resumes the thread in an alerted state.
        /// </summary>
        public int AlertResume()
        {
            NtStatus status;
            int suspendCount;

            if ((status = Win32.NtAlertResumeThread(this, out suspendCount)) >= NtStatus.Error)
                Win32.ThrowLastError(status);

            return suspendCount;
        }

        /// <summary>
        /// Gets the thread's base priority.
        /// </summary>
        public int GetBasePriority()
        {
            return this.GetInformationInt32(ThreadInformationClass.ThreadBasePriority);
        }

        /// <summary>
        /// Gets the thread's basic information.
        /// </summary>
        /// <returns>A THREAD_BASIC_INFORMATION structure.</returns>
        public ThreadBasicInformation GetBasicInformation()
        {
            NtStatus status;
            ThreadBasicInformation basicInfo = new ThreadBasicInformation();
            int retLen;

            if ((status = Win32.NtQueryInformationThread(this, ThreadInformationClass.ThreadBasicInformation,
                ref basicInfo, Marshal.SizeOf(basicInfo), out retLen)) >= NtStatus.Error)
                Win32.ThrowLastError(status);

            return basicInfo;
        }

        /// <summary>
        /// Gets the thread's context.
        /// </summary>
        /// <returns>A CONTEXT struct.</returns>
        public Context GetContext(ContextFlags flags)
        {
            Context context = new Context();

            context.ContextFlags = flags;
            this.GetContext(ref context);

            return context;
        }

        /// <summary>
        /// Gets the thread's context.
        /// </summary>
        /// <param name="context">A Context structure. The ContextFlags must be set appropriately.</param>
        public unsafe void GetContext(ref Context context)
        {
            if (KProcessHacker.Instance != null)
            {
                fixed (Context* contextPtr = &context)
                    KProcessHacker.Instance.KphGetContextThread(this, contextPtr);
            }
            else
            {
                if (!Win32.GetThreadContext(this, ref context))
                    Win32.ThrowLastError();
            }
        }

        /// <summary>
        /// Gets the number of processor cycles consumed by the thread.
        /// </summary>
        public ulong GetCycleTime()
        {
            ulong cycles;

            if (!Win32.QueryThreadCycleTime(this, out cycles))
                Win32.ThrowLastError();

            return cycles;
        }

        /// <summary>
        /// Gets the thread's exit code.
        /// </summary>
        /// <returns>A number.</returns>
        public int GetExitCode()
        {
            int exitCode;

            if (!Win32.GetExitCodeThread(this, out exitCode))
                Win32.ThrowLastError();

            return exitCode;
        }

        private int GetInformationInt32(ThreadInformationClass infoClass)
        {
            NtStatus status;
            int value;
            int retLength;

            if ((status = Win32.NtQueryInformationThread(
                this, infoClass, out value, sizeof(int), out retLength)) >= NtStatus.Error)
                Win32.ThrowLastError(status);

            return value;
        }

        /// <summary>
        /// Gets the thread's I/O priority.
        /// </summary>
        public int GetIoPriority()
        {
            return this.GetInformationInt32(ThreadInformationClass.ThreadIoPriority);
        }

        /// <summary>
        /// Gets the last system call the thread made.
        /// </summary>
        /// <returns>A system call number.</returns>
        public int GetLastSystemCall()
        {
            int firstArgument;

            return this.GetLastSystemCall(out firstArgument);
        }

        /// <summary>
        /// Gets the last system call the thread made.
        /// </summary>
        /// <param name="firstArgument">The first argument to the last system call.</param>
        /// <returns>A system call number.</returns>
        public unsafe int GetLastSystemCall(out int firstArgument)
        {
            NtStatus status;
            int* data = stackalloc int[2];
            int retLength;

            if ((status = Win32.NtQueryInformationThread(
                this, ThreadInformationClass.ThreadLastSystemCall, data, sizeof(int) * 2, out retLength)) >= NtStatus.Error)
                Win32.ThrowLastError(status);

            firstArgument = data[0];

            return data[1];
        }

        /// <summary>
        /// Gets the thread's page priority.
        /// </summary>
        public int GetPagePriority()
        {
            return this.GetInformationInt32(ThreadInformationClass.ThreadPagePriority);
        }

        /// <summary>
        /// Gets the thread's priority.
        /// </summary>
        public int GetPriority()
        {
            return this.GetInformationInt32(ThreadInformationClass.ThreadPriority);
        }

        /// <summary>
        /// Gets the thread's priority level.
        /// </summary>
        /// <returns>A ThreadPriorityLevel enum.</returns>
        public ThreadPriorityLevel GetPriorityLevel()
        {
            int priority = Win32.GetThreadPriority(this);

            if (priority == 0x7fffffff)
                Win32.ThrowLastError();

            return (ThreadPriorityLevel)priority;
        }

        /// <summary>
        /// Gets the thread's Win32 start address.
        /// </summary>
        public int GetWin32StartAddress()
        {
            return this.GetInformationInt32(ThreadInformationClass.ThreadQuerySetWin32StartAddress);
        }

        public void Impersonate(ThreadHandle clientThreadHandle, SecurityImpersonationLevel impersonationLevel)
        {
            NtStatus status;
            SecurityQualityOfService securityQos =
                new SecurityQualityOfService(impersonationLevel, false, false);

            if ((status = Win32.NtImpersonateThread(this, clientThreadHandle, ref securityQos)) >= NtStatus.Error)
                Win32.ThrowLastError(status);
        }

        public void ImpersonateAnonymous()
        {
            NtStatus status;

            if ((status = Win32.NtImpersonateAnonymousToken(this)) >= NtStatus.Error)
                Win32.ThrowLastError(status);
        }

        /// <summary>
        /// Gets whether the system will break (crash) upon the thread terminating.
        /// </summary>
        public bool IsCritical()
        {
            return this.GetInformationInt32(ThreadInformationClass.ThreadBreakOnTermination) != 0;
        }

        /// <summary>
        /// Gets whether any I/O request packets (IRPs) are still pending for the thread.
        /// </summary>
        public bool IsIoPending()
        {
            return this.GetInformationInt32(ThreadInformationClass.ThreadIsIoPending) != 0;
        }

        /// <summary>
        /// Gets whether the thread is the last in its process.
        /// </summary>
        public bool IsLastThread()
        {
            return this.GetInformationInt32(ThreadInformationClass.ThreadAmILastThread) != 0;
        }

        /// <summary>
        /// Gets whether priority boost is enabled for the thread.
        /// </summary>
        public bool IsPriorityBoostEnabled()
        {
            return this.GetInformationInt32(ThreadInformationClass.ThreadPriorityBoost) == 0;
        }

        /// <summary>
        /// Gets whether the thread has terminated.
        /// </summary>
        public bool IsTerminated()
        {
            return this.GetInformationInt32(ThreadInformationClass.ThreadIsTerminated) != 0;
        }

        /// <summary>
        /// Adds an user-mode asynchronous procedure call (APC) to the thread's APC queue.
        /// This requires the THREAD_SET_CONTEXT permission.
        /// </summary>
        /// <param name="address">The address of the APC procedure.</param>
        /// <param name="parameter">The parameter to pass to the procedure.</param>
        public void QueueApc(IntPtr address, IntPtr parameter)
        {
            if (!Win32.QueueUserAPC(address, this, parameter))
                Win32.ThrowLastError();
        }

        public void QueueApc(IntPtr address, IntPtr param1, IntPtr param2, IntPtr param3)
        {
            NtStatus status;

            if ((status = Win32.NtQueueApcThread(
                this,
                address,
                param1,
                param2,
                param3
                )) >= NtStatus.Error)
                Win32.ThrowLastError(status);
        }

        /// <summary>
        /// Sets the thread's context.
        /// </summary>
        /// <param name="context">A CONTEXT struct.</param>
        public unsafe void SetContext(Context context)
        {
            if (KProcessHacker.Instance != null)
            {
                KProcessHacker.Instance.KphSetContextThread(this, &context);
            }
            else
            {
                if (!Win32.SetThreadContext(this, ref context))
                    Win32.ThrowLastError();
            }
        }   

        public void SetCritical(bool critical)
        {
            this.SetInformationInt32(ThreadInformationClass.ThreadBreakOnTermination, critical ? 1 : 0);
        }

        private void SetInformationInt32(ThreadInformationClass infoClass, int value)
        {
            NtStatus status;

            if ((status = Win32.NtSetInformationThread(
                this, infoClass, ref value, sizeof(int))) >= NtStatus.Error)
                Win32.ThrowLastError(status);
        }

        /// <summary>
        /// Sets the thread's priority level.
        /// </summary>
        /// <param name="priority">The priority of the thread.</param>
        public void SetPriorityLevel(ThreadPriorityLevel priority)
        {
            if (!Win32.SetThreadPriority(this, (int)priority))
                Win32.ThrowLastError();
        }

        /// <summary>
        /// Suspends the thread.
        /// </summary>
        public int Suspend()
        {
            NtStatus status;
            int suspendCount;

            if ((status = Win32.NtSuspendThread(this, out suspendCount)) >= NtStatus.Error)
                Win32.ThrowLastError(status);

            return suspendCount;
        }

        /// <summary>
        /// Resumes the thread.
        /// </summary>
        public int Resume()
        {
            NtStatus status;
            int suspendCount;

            if ((status = Win32.NtResumeThread(this, out suspendCount)) >= NtStatus.Error)
                Win32.ThrowLastError(status);

            return suspendCount;
        }

        /// <summary>
        /// Terminates the thread.
        /// </summary>
        public void Terminate()
        {
            this.Terminate(0);
        }

        /// <summary>
        /// Terminates the thread, specifying an exit code.
        /// </summary>
        /// <param name="ExitCode">The exit code.</param>
        public void Terminate(int ExitCode)
        {
            if (KProcessHacker.Instance != null)
            {
                try
                {
                    KProcessHacker.Instance.KphTerminateThread(this, ExitCode);
                    return;
                }
                catch (WindowsException ex)
                {
                    if (ex.ErrorCode != 0x32) // ERROR_NOT_SUPPORTED
                        throw ex;
                }
            }

            if (!Win32.TerminateThread(this, ExitCode))
                Win32.ThrowLastError();
        }

        /// <summary>
        /// Walks the call stack for the thread.
        /// </summary>
        /// <param name="walkStackCallback">A callback to execute.</param>
        public void WalkStack(WalkStackDelegate walkStackCallback)
        {
            // We need to duplicate the handle to get QueryInformation access.
            using (var dupThreadHandle = this.Duplicate(OSVersion.MinThreadQueryInfoAccess))
            {
                using (var phandle = new ProcessHandle(
                    ThreadHandle.FromHandle(dupThreadHandle).GetBasicInformation().ClientId.ProcessId,
                    ProcessAccess.QueryInformation | ProcessAccess.VmRead
                    ))
                    this.WalkStack(phandle, walkStackCallback);
            }
        }

        /// <summary>
        /// Walks the call stack for the thread.
        /// </summary>
        /// <param name="parentProcess">A handle to the thread's parent process.</param>
        /// <param name="walkStackCallback">A callback to execute.</param>
        public unsafe void WalkStack(ProcessHandle parentProcess, WalkStackDelegate walkStackCallback)
        {
            var context = new Context();
            bool suspended = false;

            context.ContextFlags = ContextFlags.All;

            // Suspend the thread to avoid inaccurate thread stacks.
            try
            {
                this.Suspend();
                suspended = true;
            }
            catch
            {
                suspended = false;
            }

            // Use KPH for reading memory if we can.
            Win32.ReadProcessMemoryProc64 readMemoryProc = null;

            if (KProcessHacker.Instance != null)
            {
                readMemoryProc = new Win32.ReadProcessMemoryProc64(
                    delegate(IntPtr processHandle, ulong baseAddress, byte* buffer, int size, out int bytesRead)
                    {
                        return KProcessHacker.Instance.KphReadVirtualMemorySafe(
                            ProcessHandle.FromHandle(processHandle), (int)baseAddress, buffer, size, out bytesRead);
                    });
            }

            try
            {
                // Get the context.
                this.GetContext(ref context);

                // Set up the initial stack frame structure.
                var stackFrame = new StackFrame64();

                stackFrame.AddrPC.Mode = AddressMode.AddrModeFlat;
                stackFrame.AddrPC.Offset = (ulong)context.Eip;
                stackFrame.AddrStack.Mode = AddressMode.AddrModeFlat;
                stackFrame.AddrStack.Offset = (ulong)context.Esp;
                stackFrame.AddrFrame.Mode = AddressMode.AddrModeFlat;
                stackFrame.AddrFrame.Offset = (ulong)context.Ebp;

                while (true)
                {
                    if (!Win32.StackWalk64(
                        MachineType.I386,
                        parentProcess,
                        this,
                        ref stackFrame,
                        ref context,
                        readMemoryProc,
                        Win32.SymFunctionTableAccess64,
                        Win32.SymGetModuleBase64,
                        IntPtr.Zero
                        ))
                        break;

                    // If we got an invalid eip, break.
                    if (stackFrame.AddrPC.Offset == 0)
                        break;

                    // Execute the callback.
                    if (!walkStackCallback(new ThreadStackFrame(ref stackFrame)))
                        break;
                }
            }
            finally
            {
                // If we suspended the thread before, resume it.
                if (suspended)
                {
                    try
                    {
                        this.Resume();
                    }
                    catch
                    { }
                }
            }
        }

        /// <summary>
        /// Opens and returns a handle to the thread's token.
        /// </summary>
        /// <returns>A handle to the thread's token.</returns>
        public TokenHandle GetToken()
        {
            return GetToken(TokenAccess.All);
        }

        /// <summary>
        /// Opens and returns a handle to the thread's token.
        /// </summary>
        /// <param name="access">The desired access to the token.</param>
        /// <returns>A handle to the thread's token.</returns>
        public TokenHandle GetToken(TokenAccess access)
        {
            return new TokenHandle(this, access);
        }
    }

    public class ThreadStackFrame
    {
        private IntPtr _pcAddress;
        private IntPtr _returnAddress;
        private IntPtr _frameAddress;
        private IntPtr _stackAddress;
        private IntPtr _bStoreAddress;
        private IntPtr[] _params;

        internal ThreadStackFrame(ref StackFrame64 stackFrame)
        {
            _pcAddress = new IntPtr((long)stackFrame.AddrPC.Offset);
            _returnAddress = new IntPtr((long)stackFrame.AddrReturn.Offset);
            _frameAddress = new IntPtr((long)stackFrame.AddrFrame.Offset);
            _stackAddress = new IntPtr((long)stackFrame.AddrStack.Offset);
            _bStoreAddress = new IntPtr((long)stackFrame.AddrBStore.Offset);
            _params = new IntPtr[4];

            for (int i = 0; i < 4; i++)
                _params[i] = new IntPtr(stackFrame.Params[i]);
        }

        public IntPtr PcAddress { get { return _pcAddress; } }
        public IntPtr ReturnAddress { get { return _returnAddress; } }
        public IntPtr FrameAddress { get { return _frameAddress; } }
        public IntPtr StackAddress { get { return _stackAddress; } }
        public IntPtr BStoreAddress { get { return _bStoreAddress; } }
        public IntPtr[] Params { get { return _params; } }
    }
}
