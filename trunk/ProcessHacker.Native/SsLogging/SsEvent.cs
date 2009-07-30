using System;
using ProcessHacker.Native.Api;

namespace ProcessHacker.Native.SsLogging
{
    public sealed class SsEvent
    {
        public int[] Arguments
        {
            get;
            internal set;
        }

        public bool ArgumentsCopyFailed
        {
            get;
            internal set;
        }

        public bool ArgumentsProbeFailed
        {
            get;
            internal set;
        }

        public int CallNumber
        {
            get;
            internal set;
        }

        public KProcessorMode Mode
        {
            get;
            internal set;
        }

        public int ProcessId
        {
            get;
            internal set;
        }

        public IntPtr[] StackTrace
        {
            get;
            internal set;
        }

        public int ThreadId
        {
            get;
            internal set;
        }

        public DateTime Time
        {
            get;
            internal set;
        }
    }
}
