using System;
using System.Collections.Generic;
using System.Text;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;

namespace ProcessHacker.Native.Lpc
{
    public class Port : NativeObject<PortHandle>
    {
        public Port(string name)
        {
            this.Handle = PortHandle.Create(
                name,
                ObjectFlags.OpenIf,
                null,
                Win32.PortMessageMaxDataLength,
                Win32.PortMessageMaxLength,
                0
                );
        }
    }
}
