using System;
using System.Collections.Generic;
using System.Text;
using ProcessHacker.Native.Api;

namespace ProcessHacker.Native.SsLogging
{
    public sealed class SsUnicodeString : SsData
    {
        internal SsUnicodeString(MemoryRegion data)
        {
            KphSsUnicodeString unicodeStringInfo = data.ReadStruct<KphSsUnicodeString>();

            this.Original = new UnicodeString()
            {
                Length = unicodeStringInfo.Length,
                MaximumLength = unicodeStringInfo.MaximumLength,
                Buffer = unicodeStringInfo.Pointer
            };
            this.String = data.ReadUnicodeString(
                KphSsUnicodeString.BufferOffset,
                unicodeStringInfo.Length / 2
                );
        }

        public UnicodeString Original
        {
            get;
            private set;
        }

        public string String
        {
            get;
            private set;
        }
    }
}
