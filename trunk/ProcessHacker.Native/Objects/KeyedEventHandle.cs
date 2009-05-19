using System;
using System.Collections.Generic;
using System.Text;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Security;

namespace ProcessHacker.Native.Objects
{
    public class KeyedEventHandle : Win32Handle<KeyedEventAccess>
    {
        private KeyedEventHandle(IntPtr handle, bool owned)
            : base(handle, owned)
        { }
    }
}
