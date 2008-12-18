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
        public class ThreadHandle : Win32Handle, IWithToken
        {
            public static ThreadHandle FromHandle(int Handle)
            {
                return new ThreadHandle(Handle, false);
            }

            private ThreadHandle(int Handle, bool Owned)
                : base(Handle, Owned)
            { }

            public ThreadHandle(int TID, THREAD_RIGHTS access)
            {
                this.Handle = OpenThread(access, 0, TID);

                if (this.Handle == 0)
                    throw new Exception(GetLastErrorMessage());
            }

            public int Wait(int Timeout)
            {
                return WaitForSingleObject(this.Handle, Timeout);
            }

            public void Suspend()
            {
                if (SuspendThread(this.Handle) == -1)
                    throw new Exception(GetLastErrorMessage());
            }

            public void Resume()
            {
                if (ResumeThread(this.Handle) == -1)
                    throw new Exception(GetLastErrorMessage());
            }

            public void Terminate()
            {
                this.Terminate(0);
            }

            public void Terminate(int ExitCode)
            {
                if (TerminateThread(this.Handle, ExitCode) == 0)
                    throw new Exception(GetLastErrorMessage());
            }

            public TokenHandle GetToken()
            {
                return GetToken(TOKEN_RIGHTS.TOKEN_ALL_ACCESS);
            }

            public TokenHandle GetToken(TOKEN_RIGHTS access)
            {
                return new TokenHandle(this, access);
            }
        }
    }
}
