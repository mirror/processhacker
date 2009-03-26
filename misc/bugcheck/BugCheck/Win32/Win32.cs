/*
 * Process Hacker - 
 *   windows API wrapper code
 * 
 * Copyright (C) 2009 Dean
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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Net;

namespace ProcessHacker
{
    /// <summary>
    /// Provides interfacing to the Win32 and Native APIs.
    /// </summary>
    public partial class Win32
    {
        /// <summary>
        /// Gets the error message associated with the specified error code.
        /// </summary>
        /// <param name="ErrorCode">The error code.</param>
        /// <returns>An error message.</returns>
        public static string GetErrorMessage(int ErrorCode)
        {
            StringBuilder buffer = new StringBuilder(0x100);

            if (FormatMessage(0x3200, 0, ErrorCode, 0, buffer, buffer.Capacity, IntPtr.Zero) == 0)
                return "Unknown error (0x" + ErrorCode.ToString("x") + ")";

            StringBuilder result = new StringBuilder();
            int i = 0;

            while (i < buffer.Length)
            {
                if (!char.IsLetterOrDigit(buffer[i]) && 
                    !char.IsPunctuation(buffer[i]) && 
                    !char.IsSymbol(buffer[i]) && 
                    !char.IsWhiteSpace(buffer[i]))
                    break;

                result.Append(buffer[i]);
                i++;
            }

            return result.ToString();
        }

        /// <summary>
        /// Gets the error message associated with the last error that occured.
        /// </summary>
        /// <returns>An error message.</returns>
        public static string GetLastErrorMessage()
        {
            return GetErrorMessage(Marshal.GetLastWin32Error());
        }

        /// <summary>
        /// Throws a Win32Exception with the last error that occurred.
        /// </summary>
        public static void ThrowLastWin32Error()
        {
            int error = Marshal.GetLastWin32Error();

            if (error != 0)
                throw new WindowsException(error);
        }
    }
}
