using System;
using System.Collections.Generic;
using System.Text;
using ProcessHacker.Native.Api;

namespace ProcessHacker.Native.SsLogging
{
    public class SsClientId : SsData
    {
        public SsClientId(MemoryRegion data)
        {
            this.Original = data.ReadStruct<ClientId>();
        }

        public ClientId Original
        {
            get;
            internal set;
        }
    }
}
