using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessHacker.Native.SsLogging
{
    public sealed class SsHandle : SsData
    {
        internal SsHandle(MemoryRegion data)
        {
            KphSsHandle handleInfo = data.ReadStruct<KphSsHandle>();

            if (handleInfo.TypeNameOffset != 0)
            {
                this.TypeName = SsLogger.ReadWString(new MemoryRegion(data, handleInfo.TypeNameOffset));
            }

            if (handleInfo.NameOffset != 0)
            {
                this.Name = SsLogger.ReadWString(new MemoryRegion(data, handleInfo.NameOffset));
            }

            this.ProcessId = handleInfo.ClientId.ProcessId;
            this.ThreadId = handleInfo.ClientId.ThreadId;
        }

        public string Name
        {
            get;
            private set;
        }

        public int ProcessId
        {
            get;
            private set;
        }

        public int ThreadId
        {
            get;
            private set;
        }

        public string TypeName
        {
            get;
            private set;
        }
    }
}
