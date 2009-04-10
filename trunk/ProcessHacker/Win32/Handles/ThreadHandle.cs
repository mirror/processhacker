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
using System.Runtime.InteropServices;

namespace ProcessHacker
{
    public partial class Win32
    {
        /// <summary>
        /// Represents a handle to a Windows thread.
        /// </summary>
        public class ThreadHandle : Win32Handle, ISynchronizable, IWithToken
        {
            /// <summary>
            /// Creates a thread handle using an existing handle. 
            /// The handle will not be closed automatically.
            /// </summary>
            /// <param name="Handle">The handle value.</param>
            /// <returns>The thread handle.</returns>
            public static ThreadHandle FromHandle(int Handle)
            {
                return new ThreadHandle(Handle, false);
            }

            internal ThreadHandle(int Handle, bool Owned)
                : base(Handle, Owned)
            { }

            /// <summary>
            /// Creates a new thread handle.
            /// </summary>
            /// <param name="TID">The ID of the thread to open.</param>
            public ThreadHandle(int tid)
                : this(tid, THREAD_RIGHTS.THREAD_ALL_ACCESS)
            { }

            /// <summary>
            /// Creates a new thread handle.
            /// </summary>
            /// <param name="TID">The ID of the thread to open.</param>
            /// <param name="access">The desired access to the thread.</param>
            public ThreadHandle(int tid, THREAD_RIGHTS access)
            {
                if (Program.Aggressive)
                {
                    if (Program.KPH != null)
                        this.Handle = Program.KPH.KphOpenThread(tid, Program.MinThreadQueryRights);
                    else
                        this.Handle = OpenThread(Program.MinThreadQueryRights, 0, tid);

                    if (this.Handle == 0)
                        ThrowLastWin32Error();

                    int newHandle;

                    try
                    {
                        if (ZwDuplicateObject(-1, this.Handle, -1, out newHandle, (STANDARD_RIGHTS)access, 0, 0) < 0)
                            ThrowLastWin32Error();
                    }
                    finally
                    {
                        CloseHandle(this.Handle);
                    }

                    this.Handle = newHandle;
                }
                else
                {
                    if (Program.KPH != null)
                        this.Handle = Program.KPH.KphOpenThread(tid, access);
                    else
                        this.Handle = OpenThread(access, 0, tid);

                    if (this.Handle == 0)
                        ThrowLastWin32Error();
                }
            }

            /// <summary>
            /// Puts the thread in an alerted state.
            /// </summary>
            public void Alert()
            {
                if (ZwAlertThread(this) < 0)
                    ThrowLastWin32Error();
            }

            /// <summary>
            /// Gets the thread's basic information.
            /// </summary>
            /// <returns>A THREAD_BASIC_INFORMATION structure.</returns>
            public THREAD_BASIC_INFORMATION GetBasicInformation()
            {
                THREAD_BASIC_INFORMATION basicInfo = new THREAD_BASIC_INFORMATION();
                int retLen;

                if (ZwQueryInformationThread(this, THREAD_INFORMATION_CLASS.ThreadBasicInformation,
                    ref basicInfo, Marshal.SizeOf(basicInfo), out retLen) < 0)
                    ThrowLastWin32Error();

                return basicInfo;
            }

            /// <summary>
            /// Gets the thread's context.
            /// </summary>
            /// <returns>A CONTEXT struct.</returns>
            public CONTEXT GetContext(CONTEXT_FLAGS flags)
            {
                CONTEXT context = new CONTEXT();

                context.ContextFlags = flags;

                if (!GetThreadContext(this, ref context))
                    ThrowLastWin32Error();

                return context;
            }

            /// <summary>
            /// Gets the number of processor cycles consumed by the thread.
            /// </summary>
            public ulong GetCycleTime()
            {
                ulong cycles;

                if (!QueryThreadCycleTime(this, out cycles))
                    ThrowLastWin32Error();

                return cycles;
            }

            /// <summary>
            /// Gets the thread's exit code.
            /// </summary>
            /// <returns>A number.</returns>
            public int GetExitCode()
            {
                int exitCode;

                if (!GetExitCodeThread(this, out exitCode))
                    ThrowLastWin32Error();

                return exitCode;
            }

            /// <summary>
            /// Gets the thread's priority level.
            /// </summary>
            /// <returns>A ThreadPriorityLevel enum.</returns>
            public System.Diagnostics.ThreadPriorityLevel GetPriorityLevel()
            {
                int priority = GetThreadPriority(this);

                if (priority == 0x7fffffff)
                    ThrowLastWin32Error();

                return (System.Diagnostics.ThreadPriorityLevel)priority;
            }

            /// <summary>
            /// Adds an user-mode asynchronous procedure call (APC) to the thread's APC queue.
            /// This requires the THREAD_SET_CONTEXT permission.
            /// </summary>
            /// <param name="address">The address of the APC procedure.</param>
            /// <param name="parameter">The parameter to pass to the procedure.</param>
            public void QueueAPC(int address, int parameter)
            {
                if (!QueueUserAPC(address, this, parameter))
                    ThrowLastWin32Error();
            }

            /// <summary>
            /// Sets the thread's context.
            /// </summary>
            /// <param name="context">A CONTEXT struct.</param>
            public void SetContext(CONTEXT context)
            {
                if (!SetThreadContext(this, ref context))
                    ThrowLastWin32Error();
            }

            /// <summary>
            /// Sets the thread's priority level.
            /// </summary>
            /// <param name="priority">The priority of the thread.</param>
            public void SetPriorityLevel(System.Diagnostics.ThreadPriorityLevel priority)
            {
                if (!SetThreadPriority(this, (int)priority))
                    ThrowLastWin32Error();
            }

            /// <summary>
            /// Suspends the thread.
            /// </summary>
            public void Suspend()
            {
                if (SuspendThread(this) == -1)
                    ThrowLastWin32Error();
            }

            /// <summary>
            /// Resumes the thread.
            /// </summary>
            public void Resume()
            {
                if (ResumeThread(this) == -1)
                    ThrowLastWin32Error();
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
                if (!TerminateThread(this, ExitCode))
                    ThrowLastWin32Error();
            }

            /// <summary>
            /// Waits for the thread to terminate.
            /// </summary>
            public WaitResult Wait()
            {
                return WaitForSingleObject(this, 0xffffffff);
            }

            /// <summary>
            /// Waits for the thread to terminate.
            /// </summary>
            /// <param name="Timeout">The timeout of the wait.</param>
            public WaitResult Wait(uint timeout)
            {
                return WaitForSingleObject(this, timeout);
            }

            /// <summary>
            /// Opens and returns a handle to the thread's token.
            /// </summary>
            /// <returns>A handle to the thread's token.</returns>
            public TokenHandle GetToken()
            {
                return GetToken(TOKEN_RIGHTS.TOKEN_ALL_ACCESS);
            }

            /// <summary>
            /// Opens and returns a handle to the thread's token.
            /// </summary>
            /// <param name="access">The desired access to the token.</param>
            /// <returns>A handle to the thread's token.</returns>
            public TokenHandle GetToken(TOKEN_RIGHTS access)
            {
                return new TokenHandle(this, access);
            }
        }
    }
}
