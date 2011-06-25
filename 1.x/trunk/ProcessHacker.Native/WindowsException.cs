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
        private string _message;

        /// <summary>
        /// Creates an exception with no error.
        /// </summary>
        public WindowsException()
        {
            this.ErrorCode = 0;
            this.IsNtStatus = false;
        }

        /// <summary>
        /// Creates an exception from a Win32 error code.
        /// </summary>
        /// <param name="errorCode">The Win32 error code.</param>
        public WindowsException(Win32Error errorCode)
        {
            this.IsNtStatus = false;
            this.ErrorCode = errorCode;
        }

        /// <summary>
        /// Creates an exception from a NT status value.
        /// </summary>
        /// <param name="status">The NT status value.</param>
        public WindowsException(NtStatus status)
        {
            this.Status = status;
            this.ErrorCode = status.ToDosError();
            this.IsNtStatus = true;
        }

        /// <summary>
        /// Gets whether the NT status value is valid.
        /// </summary>
        public bool IsNtStatus { get; private set; }

        /// <summary>
        /// Gets a Win32 error code which represents the exception.
        /// </summary>
        public Win32Error ErrorCode { get; private set; }

        /// <summary>
        /// Gets a NT status value which represents the exception.
        /// </summary>
        public NtStatus Status { get; private set; }

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
                    // We prefer native status messages because they are usually 
                    // more detailed. However, for some status values we do 
                    // prefer the shorter Win32 error message.

                    if (
                        this.IsNtStatus &&
                        this.Status != NtStatus.AccessDenied &&
                        this.Status != NtStatus.AccessViolation
                        )
                    {
                        string message = this.Status.GetMessage();

                        if (string.IsNullOrEmpty(message))
                            message = "Could not retrieve the error message (0x" + ((int)this.Status).ToString("x") + ").";

                        _message = message;
                    }
                    else
                    {
                        _message = this.ErrorCode.GetMessage();
                    }
                }

                return _message;
            }
        }
    }
}
