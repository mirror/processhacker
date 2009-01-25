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
using ProcessHacker.PE;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;

namespace ProcessHacker
{
    public class KProcessHacker
    {
        public const string DeviceName = "KProcessHacker";

        private uint _baseControlNumber;
        private Win32.ServiceHandle _service;

        public enum Control : uint
        {
            Read = 0,
            Write,
            GetObjectName,
            GetKiServiceTable,
            GiveKiServiceTable,
            SetKiServiceTableEntry,
            GetServiceLimit,
            RestoreKiServiceTable,
            KphOpenProcess,
            GetProcessProtected,
            SetProcessProtected,
            KphOpenThread
        }

        private Win32.FileHandle _fileHandle;

        public KProcessHacker()
        {
            if (!Properties.Settings.Default.EnableKPH)
                throw new Exception("KProcessHacker is not enabled.");

            // in case the computer crashes, KPH will be disabled the next time 
            // PH is started.
            Properties.Settings.Default.EnableKPH = false;

            try { Properties.Settings.Default.Save(); }
            catch { }

            Properties.Settings.Default.EnableKPH = true; // if the computer crashes, this won't actually be saved

            Win32.ServiceManagerHandle scm = 
                new Win32.ServiceManagerHandle(Win32.SC_MANAGER_RIGHTS.SC_MANAGER_CREATE_SERVICE);

            // delete the service if it exists
            try
            {
                using (var shandle = new Win32.ServiceHandle(DeviceName))
                {
                    shandle.Delete();
                }
            }
            catch
            { }

            try
            {
                _service = scm.CreateService(DeviceName, DeviceName, Win32.SERVICE_TYPE.KernelDriver,
                    Application.StartupPath + "\\kprocesshacker.sys");
                _service.Start();
            }
            catch
            { }

            _fileHandle = new Win32.FileHandle("\\\\.\\" + DeviceName,
                Win32.FILE_RIGHTS.FILE_GENERIC_READ | Win32.FILE_RIGHTS.FILE_GENERIC_WRITE);

            try
            {
                _service.Delete(); // the service will automatically get deleted once it stops
            }
            catch
            { }

            _baseControlNumber = Misc.BytesToUInt(_fileHandle.Read(4), Misc.Endianness.Little);
        }

        public void Close()
        {
            _fileHandle.Dispose();
        }

        private uint CtlCode(Control ctl)
        {
            return _baseControlNumber + ((uint)ctl * 4);
        }

        /// <summary>
        /// Tries to find an original copy of KiServiceTable.
        /// </summary>
        /// <returns>The contents of the original KiServiceTable.</returns>
        /// <remarks>
        /// Technique from http://www.rootkit.com/newsread.php?newsid=176
        /// </remarks>
        public int[] DumpKiServiceTable()
        {
            // initialization
            string kernelFileName = Misc.GetKernelFileName();
            int kernelBase = Misc.GetKernelBase();
            int kernelModule = Win32.LoadLibraryEx(kernelFileName, 0, Win32.DONT_RESOLVE_DLL_REFERENCES);
            PEFile kernelPE = new PEFile(kernelFileName);
            int keServiceDescriptorTable = Win32.GetProcAddress(kernelModule, "KeServiceDescriptorTable");

            if (keServiceDescriptorTable == 0)
                throw new Exception("Can't find the address of KeServiceDescriptorTable.");

            // find KiServiceTable
            int kiServiceTable = 0;

            foreach (var block in kernelPE.RelocData.RelocBlocks)
            {
                foreach (var entry in block.Entries)
                {
                    if (entry.Type == ImageRelocationType.HighLow)
                    {
                        // the instruction we're looking for is:
                        // mov ds:KeServiceDescriptorTable, offset KiServiceTable
                        // c7 05 [KeServiceDescriptorTable] [KiServiceTable]

                        // instr will be set to the address of the instruction.
                        // Note that this is a VA from the start of the kernel file. We must
                        // add kernelMode to it before attempting to read its contents.
                        int instr = block.PageRVA + entry.Offset - 2;
                        int thisKsdt = 
                            Marshal.ReadInt32(new IntPtr(kernelModule + instr + 2)); // read (what we think is) the dest operand

                        // thisKsdt will be an unaltered and un-relocated address; we must subtract
                        // the presumed image base from it and then compare.
                        if (thisKsdt - (int)kernelPE.COFFOptionalHeader.ImageBase == 
                            keServiceDescriptorTable - kernelModule)
                        {
                            // We must now make sure it *is* actually a mov instruction.
                            if (Marshal.ReadInt16(new IntPtr(kernelModule + instr)) == 0x05c7)
                            {
                                kiServiceTable = Marshal.ReadInt32(new IntPtr(kernelModule + instr + 6));
                                break;
                            }
                        }
                    }
                }
            }
            
            if (kiServiceTable == 0)
                throw new Exception("Could not find the address of KiServiceTable.");
            /*
            // find KiServiceLimit
            // the Limit is stored 8 bytes after the address of the Table is stored
            int kiServiceLimit = 0;
            int keServiceDescriptorTableCount = keServiceDescriptorTable + 8;

            // search again...      
            foreach (var block in kernelPE.RelocData.RelocBlocks)
            {
                foreach (var entry in block.Entries)
                {
                    if (entry.Type == ImageRelocationType.HighLow)
                    {
                        // this time we're looking for:
                        // asm: mov ds:KeServiceDescriptorTable+8, ecx
                        // hex: 89 0d [KeServiceDescriptorTable+8]
                        // KiServiceLimit is referenced in the instruction right above it:
                        // asm: mov ecx, ds:KiServiceLimit
                        // hex: 8b 0d [KiServiceLimit]
                        int instr1 = block.PageRVA + entry.Offset - 2;
                        int instr1operand =
                            Marshal.ReadInt32(new 
                                IntPtr(kernelModule + instr1 + 2)); // read KeServiceDescriptorTable+8 so we can check it

                        if (instr1operand - (int)kernelPE.COFFOptionalHeader.ImageBase ==
                            keServiceDescriptorTableCount - kernelModule && // check the value
                            Marshal.ReadInt16(new IntPtr(kernelModule + instr1)) == 0x0d89
                            ) // check that the instruction is indeed mov [mem], ecxs
                        {
                            // start of the instruction before the first one
                            int instr2 = instr1 - 6;

                            if (Marshal.ReadInt16(new IntPtr(kernelModule + instr2)) == 0x0d8b
                                ) // check that the instruction is mov ecx, [mem]
                            {
                                int kiServiceLimitAddr = Marshal.ReadInt32(new IntPtr(kernelModule + instr2 + 2));

                                // we need to convert address
                                kiServiceLimit = Marshal.ReadInt32(new IntPtr(kernelModule + kiServiceLimitAddr -
                                    (int)kernelPE.COFFOptionalHeader.ImageBase));

                                break;
                            }
                        }
                    }
                }
            }

            // if we couldn't get the actual value, make a guess
            if (kiServiceLimit == 0)
                kiServiceLimit = 0x200; */

            int kiServiceLimit = this.GetServiceLimit();
            int[] kiServiceTableContents = new int[kiServiceLimit];

            // read and correct the function pointers!
            for (int i = 0; i < kiServiceLimit; i++)
                kiServiceTableContents[i] =
                    Marshal.ReadInt32(new IntPtr(kernelModule + kiServiceTable -
                        (int)kernelPE.COFFOptionalHeader.ImageBase + i * 4)) - // read the original address
                        (int)kernelPE.COFFOptionalHeader.ImageBase + kernelBase; // correct the value for this system

            Win32.FreeLibrary(kernelModule);

            return kiServiceTableContents;
        }

        public int[] GetKiServiceTable()
        {
            // this isn't exactly high-performance...
            byte[] temp = new byte[4];
            byte[] data = new byte[GetServiceLimit() * 4];
            int[] table = new int[GetServiceLimit()];

            _fileHandle.IoControl(CtlCode(Control.GetKiServiceTable), null, data);

            for (int i = 0; i < table.Length; i++)
            {
                Array.Copy(data, i * 4, temp, 0, 4);
                table[i] = Misc.BytesToInt(temp, Misc.Endianness.Little);
            }

            return table;
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
                int len = _fileHandle.IoControl(CtlCode(Control.GetObjectName), buffer, outBuffer);

                return UnicodeEncoding.Unicode.GetString(outBuffer, 8, len - 8);
            }
            catch
            { }

            return null;
        }

        public bool GetProcessProtected(int pid)
        {
            byte[] result = new byte[1];

            _fileHandle.IoControl(CtlCode(Control.GetProcessProtected),
                Misc.IntToBytes(pid, Misc.Endianness.Little), result);

            return result[0] != 0;
        }

        public int GetServiceLimit()
        {
            byte[] buffer = new byte[4];

            _fileHandle.IoControl(CtlCode(Control.GetServiceLimit), null, buffer);

            return Misc.BytesToInt(buffer, Misc.Endianness.Little);
        }

        public int KphOpenProcess(int pid, Win32.PROCESS_RIGHTS desiredAccess)
        {
            byte[] inData = new byte[8];
            byte[] outData = new byte[4];

            Array.Copy(Misc.IntToBytes(pid, Misc.Endianness.Little), 0, inData, 0, 4);
            Array.Copy(Misc.UIntToBytes((uint)desiredAccess, Misc.Endianness.Little), 0, inData, 4, 4);

            _fileHandle.IoControl(CtlCode(Control.KphOpenProcess), inData, outData);

            return Misc.BytesToInt(outData, Misc.Endianness.Little);
        }

        public int KphOpenThread(int tid, Win32.THREAD_RIGHTS desiredAccess)
        {
            byte[] inData = new byte[8];
            byte[] outData = new byte[4];

            Array.Copy(Misc.IntToBytes(tid, Misc.Endianness.Little), 0, inData, 0, 4);
            Array.Copy(Misc.UIntToBytes((uint)desiredAccess, Misc.Endianness.Little), 0, inData, 4, 4);

            _fileHandle.IoControl(CtlCode(Control.KphOpenThread), inData, outData);

            return Misc.BytesToInt(outData, Misc.Endianness.Little);
        }

        public byte[] Read(int address, int length)
        {
            byte[] buffer = new byte[length];

            _fileHandle.IoControl(CtlCode(Control.Read), Misc.IntToBytes(address, Misc.Endianness.Little), buffer);

            return buffer;
        }

        public void RestoreKiServiceTable()
        {
            _fileHandle.IoControl(CtlCode(Control.RestoreKiServiceTable), null, null);
        }

        public void SendKiServiceTable()
        {
            int[] kiServiceTable = this.DumpKiServiceTable();
            byte[] equivArray = new byte[kiServiceTable.Length * 4];

            for (int i = 0; i < kiServiceTable.Length; i++)
                Array.Copy(Misc.IntToBytes(kiServiceTable[i], Misc.Endianness.Little),
                    0, equivArray, i * 4, 4);

            _fileHandle.IoControl(CtlCode(Control.GiveKiServiceTable), equivArray, null);
        }     

        public void SetProcessProtected(int pid, bool protecte)
        {
            byte[] data = new byte[5];

            Array.Copy(Misc.IntToBytes(pid, Misc.Endianness.Little), 0, data, 0, 4);
            data[4] = (byte)(protecte ? 1 : 0);

            _fileHandle.IoControl(CtlCode(Control.SetProcessProtected), data, null);
        }

        public int Write(int address, byte[] data)
        {
            byte[] newData = new byte[data.Length + 4];

            Array.Copy(Misc.IntToBytes(address, Misc.Endianness.Little), newData, 4);
            Array.Copy(data, 0, newData, 4, data.Length);

            return _fileHandle.IoControl(CtlCode(Control.Write), newData, null);
        }
    }
}
