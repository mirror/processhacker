using System;
using System.Collections.Generic;
using System.Text;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Security;

namespace ProcessHacker.Native.Objects
{
    public class DesktopHandle : Win32Handle<DesktopAccess>
    {
        public static DesktopHandle GetCurrent()
        {
            return GetThreadDesktop(Win32.GetCurrentThreadId());
        }

        public static DesktopHandle GetThreadDesktop(int threadId)
        {
            int handle = Win32.GetThreadDesktop(threadId);

            if (handle == 0)
                Win32.ThrowLastError();

            return new DesktopHandle(handle, false);
        }

        public DesktopHandle(string name, bool allowOtherAccountHook, DesktopAccess access)
        {
            this.Handle = Win32.OpenDesktop(name, allowOtherAccountHook ? 1 : 0, false, access);

            if (this.Handle == 0)
                Win32.ThrowLastError();
        }

        private DesktopHandle(int handle, bool owned)
            : base(handle, owned)
        { }

        protected override void Close()
        {
            Win32.CloseDesktop(this);
        }

        public void SetCurrent()
        {
            if (!Win32.SetThreadDesktop(this))
                Win32.ThrowLastError();
        }
    }
}
