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

    public class MfsInvalidFileSystemException : MfsException
    {
        public MfsInvalidFileSystemException()
            : base("The specified file does not contain a valid file system structure.")
        { }

        public MfsInvalidFileSystemException(string message)
            : base(message)
        { }

        public MfsInvalidFileSystemException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }

    public class MfsInvalidOperationException : MfsException
    {
        public MfsInvalidOperationException()
            : base("The operation is not valid for the mode specified when the file system was opened.")
        { }

        public MfsInvalidOperationException(string message)
            : base(message)
        { }

        public MfsInvalidOperationException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
