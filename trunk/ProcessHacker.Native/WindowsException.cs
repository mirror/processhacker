/*
 * Process Hacker - 
 *   windows exception
 * 
 * Copyright (C) 2008-2009 wj32
 * 
 * This file is part of Process Hacker.
 * 
 * Process Hacker is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * Process Hacker is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Process Hacker.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Text;
using ProcessHacker.Native.Api;

namespace ProcessHacker.Native
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
        private bool _isNtStatus = false;
        private int _errorCode = 0;
        private NtStatus _status;
        private string _message = null;

        public WindowsException()
        { }

        public WindowsException(int errorCode)
        {
            _errorCode = errorCode;
        }

        public WindowsException(NtStatus status)
        {
            _status = status;
            _errorCode = Win32.RtlNtStatusToDosError(status);
            _isNtStatus = true;
        }

        public bool IsNtStatus
        {
            get { return _isNtStatus; }
        }

        public int ErrorCode
        {
            get { return _errorCode; }
        }

        public NtStatus Status
        {
            get { return _status; }
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
