/*
 * Process Hacker - 
 *   interfacing code to kernel-mode driver
 * 
 * Copyright (C) 2009 wj32
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
using System.ComponentModel;

namespace ProcessHacker
{
    public class KProcessHacker
    {
        public const uint BaseControlNumber = 0x9999e006;

        public enum Control : uint
        {
            GetObjectName = BaseControlNumber
        }

        private Win32.FileHandle _fileHandle;

        public KProcessHacker()
        {
            _fileHandle = new Win32.FileHandle("\\\\.\\KProcessHacker", 
                Win32.FILE_RIGHTS.FILE_GENERIC_READ | Win32.FILE_RIGHTS.FILE_GENERIC_WRITE);
        }

        public string GetObjectName(Win32.SYSTEM_HANDLE_INFORMATION handle)
        {
            byte[] buffer = new byte[12];
            byte[] outBuffer = new byte[2048];

            Array.Copy(Misc.IntToBytes(handle.Handle, Misc.Endianness.Little), buffer, 4);
            Array.Copy(Misc.IntToBytes(handle.Object, Misc.Endianness.Little), 0, buffer, 4, 4);
            Array.Copy(Misc.IntToBytes(handle.ProcessId, Misc.Endianness.Little), 0, buffer, 8, 4);

            try
            {
                int len = _fileHandle.IoControl((uint)Control.GetObjectName, buffer, outBuffer);

                return UnicodeEncoding.Unicode.GetString(outBuffer, 0, outBuffer.Length / 2);
            }
            catch
            { }

            return null;
        }
    }
}
