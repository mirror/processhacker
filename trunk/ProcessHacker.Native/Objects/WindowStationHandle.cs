using System;
using System.Collections.Generic;
using System.Text;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Security;

namespace ProcessHacker.Native.Objects
{
    public class WindowStationHandle : Win32Handle<WindowStationAccess>
    {
        public static WindowStationHandle GetCurrent()
        {
            IntPtr handle = Win32.GetProcessWindowStation();

            if (handle == IntPtr.Zero)
                Win32.ThrowLastError();

            return new WindowStationHandle(handle, false);
        }

        public WindowStationHandle(string name, WindowStationAccess access)
        {
            this.Handle = Win32.OpenWindowStation(name, false, access);

            if (this.Handle == System.IntPtr.Zero)
                Win32.ThrowLastError();
        }

        private WindowStationHandle(IntPtr handle, bool owned)
            : base(handle, owned)
        { }

        protected override void Close()
        {
            Win32.CloseWindowStation(this);
        }

        public void SetCurrent()
        {
            if (!Win32.SetProcessWindowStation(this))
                Win32.ThrowLastError();
        }
    }
}
