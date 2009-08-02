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

        /// <summary>
        /// Creates an exception with no error.
        /// </summary>
        public WindowsException()
        { }

        /// <summary>
        /// Creates an exception from a Win32 error code.
        /// </summary>
        /// <param name="errorCode">The Win32 error code.</param>
        public WindowsException(int errorCode)
        {
            _errorCode = errorCode;
        }

        /// <summary>
        /// Creates an exception from a NT status value.
        /// </summary>
        /// <param name="status">The NT status value.</param>
        public WindowsException(NtStatus status)
        {
            _status = status;
            _errorCode = status.ToDosError();
            _isNtStatus = true;
        }

        /// <summary>
        /// Gets whether the NT status value is valid.
        /// </summary>
        public bool IsNtStatus
        {
            get { return _isNtStatus; }
        }

        /// <summary>
        /// Gets a Win32 error code which represents the exception.
        /// </summary>
        public int ErrorCode
        {
            get { return _errorCode; }
        }

        /// <summary>
        /// Gets a NT status value which represents the exception.
        /// </summary>
        public NtStatus Status
        {
            get { return _status; }
        }

        /// <summary>
        /// Gets a message describing the exception.
        /// </summary>
        public override string Message
        {
            get
            {
                // No locking, for performance reasons. Getting the 
                // message doesn't have any side-effects anyway.
                if (_message == null)
                {
                    if (_isNtStatus)
                    {
                        // Get the real NT status value.

                        NtStatus status;
                        IntPtr messageEntry;

                        status = Win32.RtlFindMessage(
                            Loader.GetDllHandle("ntdll.dll"),
                            0xb,
                            System.Threading.Thread.CurrentThread.CurrentUICulture.LCID,
                            (int)_status,
                            out messageEntry
                            );

                        if (!status.IsError())
                        {
                            var region = new MemoryRegion(messageEntry);
                            var entry = region.ReadStruct<MessageResourceEntry>();

                            // Read the message, depending on format.
                            if ((entry.Flags & MessageResourceFlags.Unicode) == MessageResourceFlags.Unicode)
                            {
                                _message = region.ReadUnicodeString(MessageResourceEntry.TextOffset);
                            }
                            else
                            {
                                _message = region.ReadAnsiString(MessageResourceEntry.TextOffset);
                            }
                        }
                        else
                        {
                            _message = "Could not retrieve the error message (0x" + ((int)status).ToString("x") + ").";
                        }
                    }
                    else
                    {
                        // Use the dumbass Win32 way.
                        _message = Win32.GetErrorMessage(_errorCode);
                    }
                }

                return _message;
            }
        }
    }
}
