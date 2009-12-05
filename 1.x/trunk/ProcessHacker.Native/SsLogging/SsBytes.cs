using System;
using System.Collections.Generic;
using System.Text;
using ProcessHacker.Native.Api;

namespace ProcessHacker.Native.SsLogging
{
    public class SsBytes : SsData
    {
        public SsBytes(MemoryRegion data)
        {
            KphSsBytes bytes;
            byte[] buffer;

            bytes = data.ReadStruct<KphSsBytes>();
            buffer = new byte[bytes.Length];
            System.Runtime.InteropServices.Marshal.Copy(
                data.Memory.Increment(KphSsBytes.BufferOffset),
                buffer,
                0,
                buffer.Length
                );

            this.Bytes = buffer;
        }

        public byte[] Bytes
        {
            get;
            internal set;
        }
    }
}
