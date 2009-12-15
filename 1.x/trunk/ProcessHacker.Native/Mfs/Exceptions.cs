using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessHacker.Native.Mfs
{
    public class MfsException : Exception
    {
        public MfsException()
            : base()
        { }

        public MfsException(string message)
            : base(message)
        { }

        public MfsException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }

    public class MfsInvalidFileSystemException : Exception
    {
        public MfsInvalidFileSystemException()
            : base()
        { }

        public MfsInvalidFileSystemException(string message)
            : base(message)
        { }

        public MfsInvalidFileSystemException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
