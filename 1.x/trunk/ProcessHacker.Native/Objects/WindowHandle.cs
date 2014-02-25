using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using ProcessHacker.Native.Api;

namespace ProcessHacker.Native.Objects
{
    public delegate bool EnumerateWindowsDelegate(WindowHandle windowHandle);

    public struct WindowHandle : IEquatable<WindowHandle>, IEquatable<IntPtr>, System.Windows.Forms.IWin32Window
    {
        private static WindowHandle _zero = new WindowHandle(IntPtr.Zero);

        public static WindowHandle Zero
        {
            get { return _zero; }
        }

        public static bool Enumerate(EnumerateWindowsDelegate callback)
        {
            return Win32.EnumWindows((hWnd, param) => callback(new WindowHandle(hWnd)), 0);
        }

        public static bool EnumerateByThreadId(int tid, EnumerateWindowsDelegate callback)
        {
            return Win32.EnumThreadWindows(tid, (hWnd, param) => callback(new WindowHandle(hWnd)), 0);
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

        public bool IsInvalid
        {
            get { return _handle == IntPtr.Zero; }
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

        public bool EndTask(bool force)
        {
            return Win32.EndTask(this, false, force);
        }

        public bool EnumerateChildren(EnumerateWindowsDelegate callback)
        {
            return Win32.EnumChildWindows(this, (hWnd, param) => callback(new WindowHandle(hWnd)), 0);
        }

        public bool Equals(WindowHandle other)
        {
            return this.Handle.Equals(other.Handle);
        }

        public bool Equals(IntPtr other)
        {
            return this.Handle.Equals(other);
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

        public WindowPlacement GetPlacement()
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
            int retChars;

            using (var data = new MemoryAlloc(0x200))
            {
                retChars = Win32.InternalGetWindowText(this, data, data.Size / 2);

                return data.ReadUnicodeString(0, retChars);
            }
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

        public bool PostMessage(WindowMessage message, int wParam, int lParam)
        {
            return Win32.PostMessage(this, message, wParam, lParam);
        }

        public IntPtr SendMessage(WindowMessage message, int wParam, int lParam)
        {
            return Win32.SendMessage(this, message, wParam, lParam);
        }

        public IntPtr SendMessageTimeout(WindowMessage message, int wParam, int lParam, SmtoFlags flags, int timeout, out int result)
        {
            return Win32.SendMessageTimeout(this, message, wParam, lParam, flags, timeout, out result);
        }

        public bool SetForeground()
        {
            return Win32.SetForegroundWindow(this);
        }

        public bool Show(ShowWindowType flags)
        {
            return Win32.ShowWindow(this, flags);
        }
    }
}
