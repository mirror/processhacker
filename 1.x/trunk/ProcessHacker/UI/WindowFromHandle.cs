using System;
using System.Windows.Forms;

namespace ProcessHacker.UI
{
    public struct WindowFromHandle : IWin32Window
    {
        private readonly IntPtr _handle;

        public WindowFromHandle(IntPtr handle)
        {
            _handle = handle;
        }

        public IntPtr Handle
        {
            get { return _handle; }
        }
    }
}
