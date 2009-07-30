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

            this.ObjectName = new SsUnicodeString(new MemoryRegion(data, oaInfo.ObjectNameOffset));
            this.Original = oaInfo.ObjectAttributes;
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
