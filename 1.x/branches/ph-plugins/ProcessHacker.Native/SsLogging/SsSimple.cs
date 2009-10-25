using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessHacker.Native.SsLogging
{
    public sealed class SsSimple : SsData
    {
        public object Argument
        {
            get;
            internal set;
        }

        public Type Type
        {
            get;
            internal set;
        }
    }
}
