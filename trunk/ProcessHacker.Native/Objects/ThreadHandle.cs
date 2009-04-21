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
        /// <summary>
        /// Creates a thread handle using an existing handle. 
        /// The handle will not be closed automatically.
        /// </summary>
        /// <param name="Handle">The handle value.</param>
        /// <returns>The thread handle.</returns>
        public static ThreadHandle FromHandle(int handle)
        {
            return new ThreadHandle(handle, false);
        }

        /// <summary>
        /// Gets a handle to the current thread.
        /// </summary>
        /// <returns>A thread handle.</returns>
        public static ThreadHandle GetCurrent()
        {
            return new ThreadHandle(-2, false);
        }

        internal ThreadHandle(int handle, bool owned)
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
                this.Handle = KProcessHacker.Instance.KphOpenThread(tid, access);
            else
                this.Handle = Win32.OpenThread(access, 0, tid);

            if (this.Handle == 0)
                Win32.ThrowLastWin32Error();
        }

        /// <summary>
        /// Puts the thread in an alerted state.
        /// </summary>
        public void Alert()
        {
            if (Win32.NtAlertThread(this) < 0)
                Win32.ThrowLastWin32Error();
        }

        /// <summary>
        /// Gets the thread's basic information.
        /// </summary>
        /// <returns>A THREAD_BASIC_INFORMATION structure.</returns>
        public ThreadBasicInformation GetBasicInformation()
        {
            ThreadBasicInformation basicInfo = new ThreadBasicInformation();
            int retLen;

            if (Win32.NtQueryInformationThread(this, ThreadInformationClass.ThreadBasicInformation,
                ref basicInfo, Marshal.SizeOf(basicInfo), out retLen) < 0)
                Win32.ThrowLastWin32Error();

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
                    Win32.ThrowLastWin32Error();
            }
        }

        /// <summary>
        /// Gets the number of processor cycles consumed by the thread.
        /// </summary>
        public ulong GetCycleTime()
        {
            ulong cycles;

            if (!Win32.QueryThreadCycleTime(this, out cycles))
                Win32.ThrowLastWin32Error();

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
                Win32.ThrowLastWin32Error();

            return exitCode;
        }

        /// <summary>
        /// Gets the thread's priority level.
        /// </summary>
        /// <returns>A ThreadPriorityLevel enum.</returns>
        public ThreadPriorityLevel GetPriorityLevel()
        {
            int priority = Win32.GetThreadPriority(this);

            if (priority == 0x7fffffff)
                Win32.ThrowLastWin32Error();

            return (ThreadPriorityLevel)priority;
        }

        /// <summary>
        /// Adds an user-mode asynchronous procedure call (APC) to the thread's APC queue.
        /// This requires the THREAD_SET_CONTEXT permission.
        /// </summary>
        /// <param name="address">The address of the APC procedure.</param>
        /// <param name="parameter">The parameter to pass to the procedure.</param>
        public void QueueApc(int address, int parameter)
        {
            if (!Win32.QueueUserAPC(address, this, parameter))
                Win32.ThrowLastWin32Error();
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
                    Win32.ThrowLastWin32Error();
            }
        }

        /// <summary>
        /// Sets the thread's priority level.
        /// </summary>
        /// <param name="priority">The priority of the thread.</param>
        public void SetPriorityLevel(ThreadPriorityLevel priority)
        {
            if (!Win32.SetThreadPriority(this, (int)priority))
                Win32.ThrowLastWin32Error();
        }

        /// <summary>
        /// Suspends the thread.
        /// </summary>
        public void Suspend()
        {
            if (Win32.SuspendThread(this) == -1)
                Win32.ThrowLastWin32Error();
        }

        /// <summary>
        /// Resumes the thread.
        /// </summary>
        public void Resume()
        {
            if (Win32.ResumeThread(this) == -1)
                Win32.ThrowLastWin32Error();
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
                Win32.ThrowLastWin32Error();
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
}
