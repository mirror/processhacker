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
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using ProcessHacker.Common;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Security;

namespace ProcessHacker.Native.Objects
{
    /// <summary>
    /// Represents a handle to a Windows thread.
    /// </summary>
    public sealed class ThreadHandle : NativeHandle<ThreadAccess>, IWithToken
    {
        public delegate bool WalkStackDelegate(ThreadStackFrame stackFrame);

        private static readonly ThreadHandle _current = new ThreadHandle(new IntPtr(-2), false);

        /// <summary>
        /// Gets a handle to the current thread.
        /// </summary>
        public static ThreadHandle Current
        {
            get { return _current; }
        }

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
                    Win32.Throw(status);
            }
            finally
            {
                oa.Dispose();
            }

            return new ThreadHandle(handle, true);
        }

        public static ThreadHandle CreateUserThread(ProcessHandle processHandle, IntPtr startAddress, IntPtr parameter)
        {
            return CreateUserThread(processHandle, false, startAddress, parameter);
        }

        public static ThreadHandle CreateUserThread(
            ProcessHandle processHandle,
            bool createSuspended,
            IntPtr startAddress,
            IntPtr parameter
            )
        {
            ClientId clientId;

            return CreateUserThread(processHandle, createSuspended, 0, 0, startAddress, parameter, out clientId);
        }

        public static ThreadHandle CreateUserThread(
            ProcessHandle processHandle,
            bool createSuspended,
            int maximumStackSize,
            int initialStackSize,
            IntPtr startAddress,
            IntPtr parameter,
            out ClientId clientId
            )
        {
            NtStatus status;
            IntPtr threadHandle;

            if ((status = Win32.RtlCreateUserThread(
                processHandle,
                IntPtr.Zero,
                createSuspended,
                0,
                maximumStackSize.ToIntPtr(),
                initialStackSize.ToIntPtr(),
                startAddress,
                parameter,
                out threadHandle,
                out clientId
                )) >= NtStatus.Error)
                Win32.Throw(status);

            return new ThreadHandle(threadHandle, true);
        }

        /// <summary>
        /// Creates a thread handle using an existing handle. 
        /// The handle will not be closed automatically.
        /// </summary>
        /// <param name="handle">The handle value.</param>
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
            return Current;
        }

        /// <summary>
        /// Gets the client ID of the current thread.
        /// </summary>
        /// <returns>A client ID.</returns>
        public static ClientId GetCurrentCid()
        {
            return new ClientId(ProcessHandle.GetCurrentId(), ThreadHandle.GetCurrentId());
        }

        /// <summary>
        /// Gets the ID of the current thread.
        /// </summary>
        /// <returns>A thread ID.</returns>
        public static int GetCurrentId()
        {
            return Win32.GetCurrentThreadId();
        }

        /// <summary>
        /// Gets a pointer to the current thread's environment block.
        /// </summary>
        /// <returns>A pointer to the current TEB.</returns>
        public unsafe static Teb* GetCurrentTeb()
        {
            return (Teb*)Win32.NtCurrentTeb();
        }

        /// <summary>
        /// Opens the current thread.
        /// </summary>
        /// <param name="access">The desired access to the thread.</param>
        /// <returns>A handle to the current thread.</returns>
        public static ThreadHandle OpenCurrent(ThreadAccess access)
        {
            return new ThreadHandle(GetCurrentId(), access);
        }

        public static ThreadHandle OpenWithAnyAccess(int tid)
        {
            try
            {
                return new ThreadHandle(tid, OSVersion.MinThreadQueryInfoAccess);
            }
            catch
            {
                try
                {
                    return new ThreadHandle(tid, (ThreadAccess)StandardRights.Synchronize);
                }
                catch
                {
                    try
                    {
                        return new ThreadHandle(tid, (ThreadAccess)StandardRights.ReadControl);
                    }
                    catch
                    {
                        try
                        {
                            return new ThreadHandle(tid, (ThreadAccess)StandardRights.WriteDac);
                        }
                        catch
                        {
                            return new ThreadHandle(tid, (ThreadAccess)StandardRights.WriteOwner);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Registers a port which will be notified when the current thread terminates.
        /// </summary>
        /// <param name="portHandle">A handle to a port.</param>
        public static void RegisterTerminationPort(PortHandle portHandle)
        {
            NtStatus status;

            if ((status = Win32.NtRegisterThreadTerminatePort(portHandle)) >= NtStatus.Error)
                Win32.Throw(status);
        }

        /// <summary>
        /// Sleeps the current thread.
        /// </summary>
        /// <param name="timeout">The timeout, in 100ns units.</param>
        /// <param name="relative">Whether the timeout value is relative.</param>
        /// <returns>A NT status value.</returns>
        public static NtStatus Sleep(long timeout, bool relative)
        {
            return Sleep(false, timeout, relative);
        }

        /// <summary>
        /// Sleeps the current thread.
        /// </summary>
        /// <param name="alertable">
        /// Whether user-mode APCs can be delivered during the wait.
        /// </param>
        /// <param name="timeout">The timeout, in 100ns units.</param>
        /// <param name="relative">Whether the timeout value is relative.</param>
        /// <returns>A NT status value.</returns>
        public static NtStatus Sleep(bool alertable, long timeout, bool relative)
        {
            if (timeout == 0)
            {
                Yield();
                return NtStatus.Success;
            }

            long realTime = relative ? -timeout : timeout;

            return Win32.NtDelayExecution(alertable, ref realTime);
        }

        /// <summary>
        /// Checks whether the current thread is in an alerted state and 
        /// executes any pending user-mode APCs.
        /// </summary>
        /// <returns>
        /// NtStatus.Alerted if the current thread was in an alerted state, 
        /// otherwise NtStatus.Success.
        /// </returns>
        public static NtStatus TestAlert()
        {
            NtStatus status;

            if ((status = Win32.NtTestAlert()) >= NtStatus.Error)
                Win32.Throw(status);

            return status;
        }

        /// <summary>
        /// Switches to another thread.
        /// </summary>
        public static void Yield()
        {
            Win32.NtYieldExecution();
        }

        internal ThreadHandle(IntPtr handle, bool owned)
            : base(handle, owned)
        { }

        /// <summary>
        /// Opens a thread.
        /// </summary>
        /// <param name="tid">The ID of the thread to open.</param>
        public ThreadHandle(int tid)
            : this(tid, ThreadAccess.All)
        { }

        /// <summary>
        /// Opens a thread.
        /// </summary>
        /// <param name="tid">The ID of the thread to open.</param>
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
            {
                this.MarkAsInvalid();
                Win32.Throw();
            }
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
                        Win32.Throw(status);
                }
                else
                {
                    if ((status = Win32.NtOpenThread(
                        out handle,
                        access,
                        ref oa,
                        ref clientId
                        )) >= NtStatus.Error)
                        Win32.Throw(status);
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
                Win32.Throw(status);
        }

        /// <summary>
        /// Resumes the thread in an alerted state.
        /// </summary>
        public int AlertResume()
        {
            NtStatus status;
            int suspendCount;

            if ((status = Win32.NtAlertResumeThread(this, out suspendCount)) >= NtStatus.Error)
                Win32.Throw(status);

            return suspendCount;
        }

        /// <summary>
        /// Captures a kernel-mode stack trace for the thread.
        /// </summary>
        /// <returns>An array of function addresses.</returns>
        public IntPtr[] CaptureKernelStack()
        {
            return this.CaptureKernelStack(0);
        }

        /// <summary>
        /// Captures a kernel-mode stack trace for the thread.
        /// </summary>
        /// <param name="skipCount">The number of frames to skip.</param>
        /// <returns>An array of function addresses.</returns>
        public IntPtr[] CaptureKernelStack(int skipCount)
        {
            IntPtr[] stack = new IntPtr[62 - skipCount]; // 62 limit for XP and Server 2003
            int hash;

            // Capture a kernel-mode stack trace.
            int captured = KProcessHacker.Instance.KphCaptureStackBackTraceThread(
                this,
                skipCount,
                stack.Length,
                stack,
                out hash
                );

            // Create a new array with only the frames we captured.
            IntPtr[] newStack = new IntPtr[captured];

            Array.Copy(stack, 0, newStack, 0, captured);

            return newStack;
        }

        /// <summary>
        /// Captures a user-mode stack trace for the thread.
        /// </summary>
        /// <returns>An array of stack frames.</returns>
        public ThreadStackFrame[] CaptureUserStack()
        {
            return this.CaptureUserStack(0);
        }

        /// <summary>
        /// Captures a user-mode stack trace for the thread.
        /// </summary>
        /// <param name="skipCount">The number of frames to skip.</param>
        /// <returns>An array of stack frames.</returns>
        public ThreadStackFrame[] CaptureUserStack(int skipCount)
        {
            List<ThreadStackFrame> frames = new List<ThreadStackFrame>();

            // Walk the stack.
            this.WalkStack((frame) => { frames.Add(frame); return true; });

            // If we want to skip frames than we have, just return an empty array.
            if (frames.Count <= skipCount)
                return new ThreadStackFrame[0];

            // Otherwise, create a new array with the frames, minus what we skipped.
            ThreadStackFrame[] newFrames = new ThreadStackFrame[frames.Count - skipCount];

            Array.Copy(frames.ToArray(), skipCount, newFrames, 0, newFrames.Length);

            return newFrames;
        }

        /// <summary>
        /// Attempts to terminate the thread using a dangerous method. This 
        /// operation may cause the system to crash.
        /// </summary>
        /// <param name="exitStatus">The exit status.</param>
        public void DangerousTerminate(NtStatus exitStatus)
        {
            KProcessHacker.Instance.KphDangerousTerminateThread(this, exitStatus);
        }

        /// <summary>
        /// Gets the thread's base priority.
        /// </summary>
        public int GetBasePriority()
        {
            return this.GetInformationInt32(ThreadInformationClass.ThreadBasePriority);
        }

        /// <summary>
        /// Gets the thread's base priority.
        /// </summary>
        /// <returns>A ThreadPriorityLevel enum.</returns>
        public ThreadPriorityLevel GetBasePriorityWin32()
        {
            int priority = Win32.GetThreadPriority(this);

            if (priority == 0x7fffffff)
                Win32.Throw();

            return (ThreadPriorityLevel)priority;
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
                Win32.Throw(status);

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
                NtStatus status;

                if ((status = Win32.NtGetContextThread(this, ref context)) >= NtStatus.Error)
                    Win32.Throw(status);
            }
        }

        /// <summary>
        /// Gets the thread's context.
        /// </summary>
        /// <returns>A CONTEXT struct.</returns>
        public ContextAmd64 GetContext(ContextFlagsAmd64 flags)
        {
            ContextAmd64 context = new ContextAmd64();

            context.ContextFlags = flags;
            this.GetContext(ref context);

            return context;
        }

        /// <summary>
        /// Gets the thread's context.
        /// </summary>
        /// <param name="context">A Context structure. The ContextFlags must be set appropriately.</param>
        public void GetContext(ref ContextAmd64 context)
        {
            NtStatus status;

            // HACK: To avoid a datatype misalignment error, allocate some 
            // aligned memory.
            using (var data = new AlignedMemoryAlloc(Utils.SizeOf<ContextAmd64>(16), 16))
            {
                data.WriteStruct<ContextAmd64>(context);

                if ((status = Win32.NtGetContextThread(this, data)) >= NtStatus.Error)
                    Win32.Throw(status);

                context = data.ReadStruct<ContextAmd64>();
            }
        }

        /// <summary>
        /// Gets the thread's x86 context. The thread's process must be running 
        /// under WOW64.
        /// </summary>
        /// <param name="context">A Context structure. The ContextFlags must be set appropriately.</param>
        public void GetContextWow64(ref Context context)
        {
            NtStatus status;

            if ((status = Win32.RtlWow64GetThreadContext(this, ref context)) >= NtStatus.Error)
                Win32.Throw(status);
        }

        /// <summary>
        /// Gets the number of processor cycles consumed by the thread.
        /// </summary>
        public ulong GetCycleTime()
        {
            ulong cycles;

            if (!Win32.QueryThreadCycleTime(this, out cycles))
                Win32.Throw();

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
                Win32.Throw();

            return exitCode;
        }

        /// <summary>
        /// Gets the thread's exit status.
        /// </summary>
        /// <returns>A NT status value.</returns>
        public NtStatus GetExitStatus()
        {
            return this.GetBasicInformation().ExitStatus;
        }

        private int GetInformationInt32(ThreadInformationClass infoClass)
        {
            if (
                KProcessHacker.Instance != null &&
                infoClass == ThreadInformationClass.ThreadIoPriority
                )
            {
                unsafe
                {
                    int value;
                    int retLength;

                    KProcessHacker.Instance.KphQueryInformationThread(
                        this, infoClass, new IntPtr(&value), sizeof(int), out retLength
                        );

                    return value;
                }
            }
            else
            {
                NtStatus status;
                int value;
                int retLength;

                if ((status = Win32.NtQueryInformationThread(
                    this, infoClass, out value, sizeof(int), out retLength)) >= NtStatus.Error)
                    Win32.Throw(status);

                return value;
            }
        }

        private IntPtr GetInformationIntPtr(ThreadInformationClass infoClass)
        {
            NtStatus status;
            IntPtr value;
            int retLength;

            if ((status = Win32.NtQueryInformationThread(
                this, infoClass, out value, IntPtr.Size, out retLength)) >= NtStatus.Error)
                Win32.Throw(status);

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
                Win32.Throw(status);

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
        /// Opens the thread's process.
        /// </summary>
        /// <returns>A process handle.</returns>
        public ProcessHandle GetProcess(ProcessAccess access)
        {
            return new ProcessHandle(this, access);
        }

        /// <summary>
        /// Gets the thread's parent process' unique identifier.
        /// </summary>
        /// <returns>A process ID.</returns>
        public int GetProcessId()
        {
            return this.GetBasicInformation().ClientId.ProcessId;
        }

        /// <summary>
        /// Gets the thread's unique identifier.
        /// </summary>
        /// <returns>A thread ID.</returns>
        public int GetThreadId()
        {
            return this.GetBasicInformation().ClientId.ThreadId;
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

        /// <summary>
        /// Gets the thread's Win32 start address.
        /// </summary>
        public IntPtr GetWin32StartAddress()
        {
            return this.GetInformationIntPtr(ThreadInformationClass.ThreadQuerySetWin32StartAddress);
        }

        /// <summary>
        /// Causes the thread to impersonate a client thread.
        /// </summary>
        /// <param name="clientThreadHandle">A handle to a client thread.</param>
        /// <param name="impersonationLevel">The impersonation level to request.</param>
        public void Impersonate(ThreadHandle clientThreadHandle, SecurityImpersonationLevel impersonationLevel)
        {
            NtStatus status;
            SecurityQualityOfService securityQos =
                new SecurityQualityOfService(impersonationLevel, false, false);

            if ((status = Win32.NtImpersonateThread(this, clientThreadHandle, ref securityQos)) >= NtStatus.Error)
                Win32.Throw(status);
        }

        /// <summary>
        /// Causes the thread to impersonate the anonymous account.
        /// </summary>
        public void ImpersonateAnonymous()
        {
            NtStatus status;

            if ((status = Win32.NtImpersonateAnonymousToken(this)) >= NtStatus.Error)
                Win32.Throw(status);
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
        /// This requires THREAD_SET_CONTEXT access.
        /// </summary>
        /// <param name="address">The address of the APC procedure.</param>
        /// <param name="parameter">The parameter to pass to the procedure.</param>
        public void QueueApc(IntPtr address, IntPtr parameter)
        {
            if (!Win32.QueueUserAPC(address, this, parameter))
                Win32.Throw();
        }

        /// <summary>
        /// Adds an user-mode asynchronous procedure call (APC) to the thread's APC queue.
        /// This requires THREAD_SET_CONTEXT access.
        /// </summary>
        /// <param name="action">The delegate to execute..</param>
        /// <param name="parameter">The parameter to pass to the procedure.</param>
        public void QueueApc(ApcRoutine action, IntPtr parameter)
        {
            if (!Win32.QueueUserAPC(action, this, parameter))
                Win32.Throw();
        }

        /// <summary>
        /// Queues a user-mode asynchronous procedure call (APC) to the thread.
        /// </summary>
        /// <param name="address">The address of the function to execute.</param>
        /// <param name="param1">The first parameter to pass to the function.</param>
        /// <param name="param2">The second parameter to pass to the function.</param>
        /// <param name="param3">The third parameter to pass to the function.</param>
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
                Win32.Throw(status);
        }

        public void RemoteCall(IntPtr address, IntPtr[] arguments)
        {
            this.RemoteCall(address, arguments, false);
        }

        public void RemoteCall(IntPtr address, IntPtr[] arguments, bool alreadySuspended)
        {
            ProcessHandle processHandle;

            if (KProcessHacker.Instance != null)
                processHandle = this.GetProcess(ProcessAccess.VmWrite);
            else
                processHandle = new ProcessHandle(this.GetProcessId(), ProcessAccess.VmWrite);

            using (processHandle)
                this.RemoteCall(processHandle, address, arguments, alreadySuspended);
        }

        public void RemoteCall(ProcessHandle processHandle, IntPtr address, IntPtr[] arguments, bool alreadySuspended)
        {
            NtStatus status;

            if ((status = Win32.RtlRemoteCall(
                processHandle,
                this,
                address,
                arguments.Length,
                arguments,
                false,
                alreadySuspended
                )) >= NtStatus.Error)
                Win32.Throw(status);
        }

        /// <summary>
        /// Resumes the thread.
        /// </summary>
        public int Resume()
        {
            NtStatus status;
            int suspendCount;

            if ((status = Win32.NtResumeThread(this, out suspendCount)) >= NtStatus.Error)
                Win32.Throw(status);

            return suspendCount;
        }

        /// <summary>
        /// Sets the thread's base priority.
        /// </summary>
        /// <param name="basePriority">The thread's base priority.</param>
        public void SetBasePriority(int basePriority)
        {
            this.SetInformationInt32(ThreadInformationClass.ThreadBasePriority, basePriority);
        }

        /// <summary>
        /// Sets the thread's base priority.
        /// </summary>
        /// <param name="basePriority">The base priority of the thread.</param>
        public void SetBasePriorityWin32(ThreadPriorityLevel basePriority)
        {
            if (!Win32.SetThreadPriority(this, (int)basePriority))
                Win32.Throw();
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
                NtStatus status;

                if ((status = Win32.NtSetContextThread(this, ref context)) >= NtStatus.Error)
                    Win32.Throw(status);
            }
        }

        /// <summary>
        /// Sets the thread's context.
        /// </summary>
        /// <param name="context">A CONTEXT struct.</param>
        public void SetContext(ContextAmd64 context)
        {
            NtStatus status;

            // HACK: To avoid a datatype misalignment error, allocate 
            // some aligned memory.
            using (var data = new AlignedMemoryAlloc(Utils.SizeOf<ContextAmd64>(16), 16))
            {
                data.WriteStruct<ContextAmd64>(context);

                if ((status = Win32.NtSetContextThread(this, data)) >= NtStatus.Error)
                    Win32.Throw(status);
            }
        }

        /// <summary>
        /// Sets the thread's x86 context. The thread's process must 
        /// be running under WOW64.
        /// </summary>
        /// <param name="context">A CONTEXT struct.</param>
        public void SetContextWow64(Context context)
        {
            NtStatus status;

            if ((status = Win32.RtlWow64SetThreadContext(this, ref context)) >= NtStatus.Error)
                Win32.Throw(status);
        }   

        /// <summary>
        /// Sets whether the thread is critical.
        /// </summary>
        /// <param name="critical">Whether the thread should be critical.</param>
        public void SetCritical(bool critical)
        {
            this.SetInformationInt32(ThreadInformationClass.ThreadBreakOnTermination, critical ? 1 : 0);
        }

        private void SetInformationInt32(ThreadInformationClass infoClass, int value)
        {
            if (
                KProcessHacker.Instance != null &&
                infoClass == ThreadInformationClass.ThreadIoPriority
                )
            {
                unsafe
                {
                    KProcessHacker.Instance.KphSetInformationThread(
                        this, infoClass, new IntPtr(&value), sizeof(int)
                        );
                }
            }
            else
            {
                NtStatus status;

                if ((status = Win32.NtSetInformationThread(
                    this, infoClass, ref value, sizeof(int))) >= NtStatus.Error)
                    Win32.Throw(status);
            }
        }

        private void SetInformationIntPtr(ThreadInformationClass infoClass, IntPtr value)
        {
            NtStatus status;

            if ((status = Win32.NtSetInformationThread(
                this, infoClass, ref value, sizeof(int))) >= NtStatus.Error)
                Win32.Throw(status);
        }

        public void SetIoPriority(int ioPriority)
        {
            this.SetInformationInt32(ThreadInformationClass.ThreadIoPriority, ioPriority);
        }

        public void SetPagePriority(int pagePriority)
        {
            this.SetInformationInt32(ThreadInformationClass.ThreadPagePriority, pagePriority);
        }

        /// <summary>
        /// Sets the thread's priority.
        /// </summary>
        /// <param name="priority">The thread's priority.</param>
        public void SetPriority(int priority)
        {
            this.SetInformationInt32(ThreadInformationClass.ThreadPriority, priority);
        }

        /// <summary>
        /// Sets the thread's priority boost.
        /// </summary>
        /// <param name="enabled">Whether priority boost will be enabled.</param>
        public void SetPriorityBoost(bool enabled)
        {
            this.SetInformationInt32(ThreadInformationClass.ThreadPriorityBoost, enabled ? 0 : 1);
        }

        /// <summary>
        /// Sets the thread's impersonation token.
        /// </summary>
        /// <param name="tokenHandle">
        /// A handle to a token. Specify null to cause the thread to stop 
        /// impersonating.
        /// </param>
        public void SetToken(TokenHandle tokenHandle)
        {
            this.SetInformationIntPtr(ThreadInformationClass.ThreadImpersonationToken, tokenHandle ?? IntPtr.Zero);
        }

        /// <summary>
        /// Suspends the thread.
        /// </summary>
        public int Suspend()
        {
            NtStatus status;
            int suspendCount;

            if ((status = Win32.NtSuspendThread(this, out suspendCount)) >= NtStatus.Error)
                Win32.Throw(status);

            return suspendCount;
        }

        /// <summary>
        /// Terminates the thread.
        /// </summary>
        public void Terminate()
        {
            this.Terminate(NtStatus.Success);
        }

        /// <summary>
        /// Terminates the thread.
        /// </summary>
        /// <param name="exitStatus">The exit status.</param>
        public void Terminate(NtStatus exitStatus)
        {
            if (KProcessHacker.Instance != null)
            {
                try
                {
                    KProcessHacker.Instance.KphTerminateThread(this, exitStatus);
                    return;
                }
                catch (WindowsException ex)
                {
                    if (ex.ErrorCode != Win32Error.NotSupported)
                        throw ex;
                }
            }

            NtStatus status;

            if ((status = Win32.NtTerminateThread(this, exitStatus)) >= NtStatus.Error)
                Win32.Throw(status);
        }

        /// <summary>
        /// Walks the call stack for the thread.
        /// </summary>
        /// <param name="walkStackCallback">A callback to execute.</param>
        public void WalkStack(WalkStackDelegate walkStackCallback)
        {
            this.WalkStack(walkStackCallback, OSVersion.Architecture);
        }

        /// <summary>
        /// Walks the call stack for the thread.
        /// </summary>
        /// <param name="walkStackCallback">A callback to execute.</param>
        /// <param name="architecture">
        /// The type of stack walk. On 32-bit systems, this value is ignored. 
        /// On 64-bit systems, this value can be set to I386 to walk the 
        /// 32-bit stack.
        /// </param>
        public void WalkStack(WalkStackDelegate walkStackCallback, OSArch architecture)
        {
            if (KProcessHacker.Instance != null)
            {
                // Use KPH to open the parent process.
                using (var phandle = this.GetProcess(ProcessAccess.QueryInformation | ProcessAccess.VmRead))
                    this.WalkStack(phandle, walkStackCallback, architecture);
            }
            else
            {
                // We need to duplicate the handle to get QueryInformation access.
                using (var dupThreadHandle = this.Duplicate(OSVersion.MinThreadQueryInfoAccess))
                using (var phandle = new ProcessHandle(
                    ThreadHandle.FromHandle(dupThreadHandle).GetBasicInformation().ClientId.ProcessId,
                    ProcessAccess.QueryInformation | ProcessAccess.VmRead
                    ))
                {
                    this.WalkStack(phandle, walkStackCallback, architecture);
                }
            }
        }

        /// <summary>
        /// Walks the call stack for the thread.
        /// </summary>
        /// <param name="parentProcess">A handle to the thread's parent process.</param>
        /// <param name="walkStackCallback">A callback to execute.</param>
        public unsafe void WalkStack(ProcessHandle parentProcess, WalkStackDelegate walkStackCallback)
        {
            this.WalkStack(parentProcess, walkStackCallback, OSVersion.Architecture);
        }

        /// <summary>
        /// Walks the call stack for the thread.
        /// </summary>
        /// <param name="parentProcess">A handle to the thread's parent process.</param>
        /// <param name="walkStackCallback">A callback to execute.</param>
        /// <param name="architecture">
        /// The type of stack walk. On 32-bit systems, this value is ignored. 
        /// On 64-bit systems, this value can be set to I386 to walk the 
        /// 32-bit stack.
        /// </param>
        public unsafe void WalkStack(ProcessHandle parentProcess, WalkStackDelegate walkStackCallback, OSArch architecture)
        {
            bool suspended = false;

            // Suspend the thread to avoid inaccurate thread stacks.
            try
            {
                this.Suspend();
                suspended = true;
            }
            catch (WindowsException)
            {
                suspended = false;
            }

            // Use KPH for reading memory if we can.
            ReadProcessMemoryProc64 readMemoryProc = null;

            if (KProcessHacker.Instance != null)
            {
                readMemoryProc = new ReadProcessMemoryProc64(
                    delegate(IntPtr processHandle, ulong baseAddress, IntPtr buffer, int size, out int bytesRead)
                    {
                        return KProcessHacker.Instance.KphReadVirtualMemorySafe(
                            ProcessHandle.FromHandle(processHandle), (int)baseAddress, buffer, size, out bytesRead).IsSuccess();
                    });
            }

            try
            {
                // x86/WOW64 stack walk.
                if (OSVersion.Architecture == OSArch.I386 || (OSVersion.Architecture == OSArch.Amd64 && architecture == OSArch.I386))
                {
                    Context context = new Context();

                    context.ContextFlags = ContextFlags.All;

                    if (OSVersion.Architecture == OSArch.I386)
                    {
                        // Get the context.
                        this.GetContext(ref context);
                    }
                    else
                    {
                        // Get the WOW64 x86 context.
                        this.GetContextWow64(ref context);
                    }

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
                        using (Win32.DbgHelpLock.AcquireContext())
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
                        }

                        // If we got an invalid eip, break.
                        if (stackFrame.AddrPC.Offset == 0)
                            break;

                        // Execute the callback.
                        if (!walkStackCallback(new ThreadStackFrame(ref stackFrame)))
                            break;
                    }
                }
                // x64 stack walk.
                else if (OSVersion.Architecture == OSArch.Amd64)
                {
                    ContextAmd64 context = new ContextAmd64();

                    context.ContextFlags = ContextFlagsAmd64.All;
                    // Get the context.
                    this.GetContext(ref context);

                    // Set up the initial stack frame structure.
                    var stackFrame = new StackFrame64();

                    stackFrame.AddrPC.Mode = AddressMode.AddrModeFlat;
                    stackFrame.AddrPC.Offset = (ulong)context.Rip;
                    stackFrame.AddrStack.Mode = AddressMode.AddrModeFlat;
                    stackFrame.AddrStack.Offset = (ulong)context.Rsp;
                    stackFrame.AddrFrame.Mode = AddressMode.AddrModeFlat;
                    stackFrame.AddrFrame.Offset = (ulong)context.Rbp;

                    while (true)
                    {
                        using (Win32.DbgHelpLock.AcquireContext())
                        {
                            if (!Win32.StackWalk64(
                                MachineType.Amd64,
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
                        }

                        // If we got an invalid rip, break.
                        if (stackFrame.AddrPC.Offset == 0)
                            break;

                        // Execute the callback.
                        if (!walkStackCallback(new ThreadStackFrame(ref stackFrame)))
                            break;
                    }
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
                    catch (WindowsException)
                    { }
                }
            }
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
