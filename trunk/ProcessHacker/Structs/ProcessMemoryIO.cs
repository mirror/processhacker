/*
 * Process Hacker
 * 
 * Copyright (C) 2008 wj32
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessHacker.Structs
{
    public class ProcessMemoryIO : IStructIOProvider
    {
        private Win32.ProcessHandle _phandleR;
        private Win32.ProcessHandle _phandleW;

        public ProcessMemoryIO(int pid)
        {
            try { _phandleR = new Win32.ProcessHandle(pid, Win32.PROCESS_RIGHTS.PROCESS_VM_READ); }
            catch { }
            try
            {
                _phandleW = new Win32.ProcessHandle(pid, Win32.PROCESS_RIGHTS.PROCESS_VM_OPERATION |
                    Win32.PROCESS_RIGHTS.PROCESS_VM_WRITE);
            }
            catch { }
        }

        public byte[] ReadBytes(int offset, int length)
        {
            return _phandleR.ReadMemory(offset, length);
        }

        public void WriteBytes(int offset, byte[] bytes)
        {
            _phandleW.WriteMemory(offset, bytes);
        }
    }
}
