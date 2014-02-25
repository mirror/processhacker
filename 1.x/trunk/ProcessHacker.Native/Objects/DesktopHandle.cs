/*
 * Process Hacker - 
 *   desktop handle
 * 
 * Copyright (C) 2009 wj32
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
using System.Text;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Security;

namespace ProcessHacker.Native.Objects
{
    public sealed class DesktopHandle : UserHandle<DesktopAccess>
    {
        public static DesktopHandle GetCurrent()
        {
            return GetThreadDesktop(Win32.GetCurrentThreadId());
        }

        public static DesktopHandle GetThreadDesktop(int threadId)
        {
            IntPtr handle = Win32.GetThreadDesktop(threadId);

            if (handle == IntPtr.Zero)
                Win32.Throw();

            return new DesktopHandle(handle, false);
        }

        public static void SetCurrent(DesktopHandle desktopHandle)
        {
            if (!Win32.SetThreadDesktop(desktopHandle))
                Win32.Throw();
        }

        public DesktopHandle(string name, bool allowOtherAccountHook, DesktopAccess access)
        {
            this.Handle = Win32.OpenDesktop(name, allowOtherAccountHook ? 1 : 0, false, access);

            if (this.Handle == IntPtr.Zero)
                Win32.Throw();
        }

        private DesktopHandle(IntPtr handle, bool owned)
            : base(handle, owned)
        { }

        protected override void Close()
        {
            Win32.CloseDesktop(this);
        }

        public void Switch()
        {
            if (!Win32.SwitchDesktop(this))
                Win32.Throw();
        }
    }
}
