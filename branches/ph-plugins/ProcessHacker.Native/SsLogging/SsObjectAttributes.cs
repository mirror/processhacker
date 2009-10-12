using System;
using System.Collections.Generic;
using System.Text;
using ProcessHacker.Native.Api;

namespace ProcessHacker.Native.SsLogging
{
    public class SsObjectAttributes : SsData
    {
        internal SsObjectAttributes(MemoryRegion data)
        {
            KphSsObjectAttributes oaInfo = data.ReadStruct<KphSsObjectAttributes>();

            if (oaInfo.ObjectNameOffset != 0)
                this.ObjectName = new SsUnicodeString(new MemoryRegion(data, oaInfo.ObjectNameOffset));

            this.Original = oaInfo.ObjectAttributes;

            if (oaInfo.RootDirectoryOffset != 0)
                this.RootDirectory = new SsHandle(new MemoryRegion(data, oaInfo.RootDirectoryOffset));
        }

        public SsUnicodeString ObjectName
        {
            get;
            private set;
        }

        public ObjectAttributes Original
        {
            get;
            private set;
        }

        public SsHandle RootDirectory
        {
            get;
            private set;
        }
    }
}
