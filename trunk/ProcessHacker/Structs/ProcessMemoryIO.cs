/*
 * Process Hacker - 
 *   process memory I/O interface
 * 
 * Copyright (C) 2008 wj32
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

using ProcessHacker.Native.Objects;
using System;

namespace ProcessHacker.Structs
{
    public class ProcessMemoryIO : IStructIOProvider
    {
        private ProcessHandle _phandleR;
        private ProcessHandle _phandleW;

        public ProcessMemoryIO(int pid)
        {
            try { _phandleR = new ProcessHandle(pid, Program.MinProcessReadMemoryRights); }
            catch { }
            try
            {
                _phandleW = new ProcessHandle(pid, Program.MinProcessWriteMemoryRights);
            }
            catch { }
        }

        public byte[] ReadBytes(IntPtr offset, int length)
        {
            return _phandleR.ReadMemory(offset, length);
        }

        public void WriteBytes(IntPtr offset, byte[] bytes)
        {
            _phandleW.WriteMemory(offset, bytes);
        }
    }
}
