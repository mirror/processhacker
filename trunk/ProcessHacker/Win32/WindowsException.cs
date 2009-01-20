using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessHacker
{
    /// <summary>
    /// Represents a Win32 or Native exception.
    /// </summary>
    /// <remarks>
    /// Unlike the System.ComponentModel.Win32Exception class, 
    /// this class does not get the error's associated 
    /// message unless it is requested.
    /// </remarks>
    public class WindowsException : Exception
    {
        private int _errorCode;
        private string _message = null;

        public WindowsException(int errorCode)
        {
            _errorCode = errorCode;
        }

        public override string Message
        {
            get
            {
                if (_message == null)
                    _message = Win32.GetErrorMessage(_errorCode);

                return _message;
            }
        }
    }
}
