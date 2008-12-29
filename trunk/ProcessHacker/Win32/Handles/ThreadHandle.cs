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
using System.Runtime.InteropServices;

namespace ProcessHacker
{
    public partial class Win32
    {
        /// <summary>
        /// Represents a handle to a Windows thread.
        /// </summary>
        public class ThreadHandle : Win32Handle, IWithToken
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
            public ThreadHandle(int TID)
                : this(TID, THREAD_RIGHTS.THREAD_ALL_ACCESS)
            { }

            /// <summary>
            /// Creates a new thread handle.
            /// </summary>
            /// <param name="TID">The ID of the thread to open.</param>
            /// <param name="access">The desired access to the thread.</param>
            public ThreadHandle(int TID, THREAD_RIGHTS access)
            {
                this.Handle = OpenThread(access, 0, TID);

                if (this.Handle == 0)
                    throw new Exception(GetLastErrorMessage());
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
                    ref basicInfo, Marshal.SizeOf(basicInfo), out retLen) != 0)
                    throw new Exception(GetLastErrorMessage());

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
                    throw new Exception(GetLastErrorMessage());

                return context;
            }

            /// <summary>
            /// Gets the thread's priority level.
            /// </summary>
            /// <returns>A ThreadPriorityLevel enum.</returns>
            public System.Diagnostics.ThreadPriorityLevel GetPriorityLevel()
            {
                int priority = GetThreadPriority(this);

                // this is what Microsoft does in its ProcessThread class (found out using Reflector)
                if (priority == 0x7fffffff)
                    throw new Exception(GetLastErrorMessage());

                return (System.Diagnostics.ThreadPriorityLevel)priority;
            }

            /// <summary>
            /// Sets the thread's context.
            /// </summary>
            /// <param name="context">A CONTEXT struct.</param>
            public void SetContext(CONTEXT context)
            {
                if (!SetThreadContext(this, ref context))
                    throw new Exception(GetLastErrorMessage());
            }

            /// <summary>
            /// Suspends the thread.
            /// </summary>
            public void Suspend()
            {
                if (SuspendThread(this.Handle) == -1)
                    throw new Exception(GetLastErrorMessage());
            }

            /// <summary>
            /// Resumes the thread.
            /// </summary>
            public void Resume()
            {
                if (ResumeThread(this.Handle) == -1)
                    throw new Exception(GetLastErrorMessage());
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
                if (!TerminateThread(this.Handle, ExitCode))
                    throw new Exception(GetLastErrorMessage());
            }

            /// <summary>
            /// Waits for the thread.
            /// </summary>
            /// <param name="Timeout">The timeout of the wait.</param>
            /// <returns>Either WAIT_OBJECT_0, WAIT_TIMEOUT or WAIT_FAILED.</returns>
            public int Wait(int Timeout)
            {
                return WaitForSingleObject(this.Handle, Timeout);
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
