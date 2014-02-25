/*
 * Process Hacker - 
 *   window station handle
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
    public sealed class WindowStationHandle : UserHandle<WindowStationAccess>
    {
        public static WindowStationHandle GetCurrent()
        {
            IntPtr handle = Win32.GetProcessWindowStation();

            if (handle == IntPtr.Zero)
                Win32.Throw();

            return new WindowStationHandle(handle, false);
        }

        public static void SetCurrent(WindowStationHandle windowStationHandle)
        {
            if (!Win32.SetProcessWindowStation(windowStationHandle))
                Win32.Throw();
        }

        public WindowStationHandle(string name, WindowStationAccess access)
        {
            this.Handle = Win32.OpenWindowStation(name, false, access);

            if (this.Handle == System.IntPtr.Zero)
                Win32.Throw();
        }

        private WindowStationHandle(IntPtr handle, bool owned)
            : base(handle, owned)
        { }

        protected override void Close()
        {
            Win32.CloseWindowStation(this);
        }
    }
}
