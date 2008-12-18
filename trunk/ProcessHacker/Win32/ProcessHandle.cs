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
        /// Represents a handle to a Windows process.
        /// </summary>
        public class ProcessHandle : Win32Handle, IWithToken
        {
            /// <summary>
            /// Creates a process handle using an existing handle. 
            /// The handle will not be closed automatically.
            /// </summary>
            /// <param name="Handle">The handle value.</param>
            /// <returns></returns>
            public static ProcessHandle FromHandle(int Handle)
            {
                return new ProcessHandle(Handle, false);
            }

            private ProcessHandle(int Handle, bool Owned)
                : base(Handle, Owned)
            { }

            /// <summary>
            /// Creates a new process handle.
            /// </summary>
            /// <param name="PID">The ID of the process to open.</param>
            public ProcessHandle(int PID)
                : this(PID, PROCESS_RIGHTS.PROCESS_ALL_ACCESS)
            { }

            /// <summary>
            /// Creates a new process handle.
            /// </summary>
            /// <param name="PID">The ID of the process to open.</param>
            /// <param name="access">The desired access to the process.</param>
            public ProcessHandle(int PID, PROCESS_RIGHTS access)
            {
                this.Handle = OpenProcess(access, 0, PID);

                if (this.Handle == 0)
                    throw new Exception(GetLastErrorMessage());
            }

            /// <summary>
            /// Waits for the process.
            /// </summary>
            /// <param name="Timeout">The timeout of the wait.</param>
            /// <returns>Either WAIT_OBJECT_0, WAIT_TIMEOUT or WAIT_FAILED.</returns>
            public int Wait(int Timeout)
            {
                return WaitForSingleObject(this.Handle, Timeout);
            }

            /// <summary>
            /// Terminates the process.
            /// </summary>
            public void Terminate()
            {
                this.Terminate(0);
            }

            /// <summary>
            /// Terminates the process, specifying the exit code.
            /// </summary>
            /// <param name="ExitCode">The exit code.</param>
            public void Terminate(int ExitCode)
            {
                if (TerminateProcess(this.Handle, ExitCode) == 0)
                    throw new Exception(GetLastErrorMessage());
            }

            /// <summary>
            /// Opens and returns a handle to the process' token.
            /// </summary>
            /// <returns>A handle to the process' token.</returns>
            public TokenHandle GetToken()
            {
                return GetToken(TOKEN_RIGHTS.TOKEN_ALL_ACCESS);
            }

            /// <summary>
            /// Opens and returns a handle to the process' token.
            /// </summary>
            /// <param name="access">The desired access to the token.</param>
            /// <returns>A handle to the process' token.</returns>
            public TokenHandle GetToken(TOKEN_RIGHTS access)
            {
                return new TokenHandle(this, access);
            }
        }
    }
}
