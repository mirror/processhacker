using System;
using System.Drawing;
using System.Text;
using ProcessHacker.Native.Api;
using System.Runtime.InteropServices;

namespace ProcessHacker.Native.Objects
{
    public struct WindowHandle
    {
        private static WindowHandle _zero = new WindowHandle(IntPtr.Zero);

        public static WindowHandle Zero
        {
            get { return _zero; }
        }

        public static WindowHandle Find(string className, string windowName)
        {
            IntPtr handle = Win32.FindWindow(className, windowName);

            return new WindowHandle(handle);
        }

        public static WindowHandle GetDesktopWindow()
        {
            return new WindowHandle(Win32.GetDesktopWindow());
        }

        public static WindowHandle GetForegroundWindow()
        {
            return new WindowHandle(Win32.GetForegroundWindow());
        }

        public static WindowHandle GetShellWindow()
        {
            return new WindowHandle(Win32.GetShellWindow());
        }

        public static implicit operator IntPtr(WindowHandle windowHandle)
        {
            return windowHandle.Handle;
        }

        private IntPtr _handle;

        public WindowHandle(IntPtr handle)
        {
            _handle = handle;
        }

        public IntPtr Handle
        {
            get { return _handle; }
        }

        public bool BringToTop()
        {
            return Win32.BringWindowToTop(this);
        }

        public bool Close()
        {
            return Win32.CloseWindow(this);
        }

        public bool Destroy()
        {
            return Win32.DestroyWindow(this);
        }

        public ClientId GetClientId()
        {
            int tid, pid;

            tid = Win32.GetWindowThreadProcessId(this, out pid);

            return new ClientId(pid, tid);
        }

        public WindowHandle GetParent()
        {
            return new WindowHandle(Win32.GetParent(this));
        }

        public WindowPlacement GetWindowPlacement()
        {
            WindowPlacement placement = new WindowPlacement();
            placement.Length = Marshal.SizeOf(placement);
            Win32.GetWindowPlacement(this, ref placement);
            return placement;
        }

        public Rectangle GetRectangle()
        {
            Rect rect;

            if (!Win32.GetWindowRect(this, out rect))
                return Rectangle.Empty;
            else
                return rect.ToRectangle();
        }

        public string GetText()
        {
            StringBuilder sb = new StringBuilder(0x1000);
            int retChars;

            retChars = Win32.InternalGetWindowText(this, sb, sb.Capacity - 1);

            return sb.ToString(0, retChars);
        }

        public bool IsHung()
        {
            return Win32.IsHungAppWindow(this);
        }

        public bool IsParent()
        {
            return this.GetParent().Equals(WindowHandle.Zero);
        }

        public bool IsWindow()
        {
            return Win32.IsWindow(this);
        }

        public bool IsVisible()
        {
            return Win32.IsWindowVisible(this);
        }
    }
}
