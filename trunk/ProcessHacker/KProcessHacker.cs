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
            KphOpenProcess,
            KphOpenThread,
            KphOpenProcessToken,
            GetProcessProtected,
            SetProcessProtected,
            KphTerminateProcess,
            KphSuspendProcess,
            KphResumeProcess,
            ReadProcessMemory,
            SetProcessToken
        }

        private Win32.FileHandle _fileHandle;

        public KProcessHacker()
        {
            bool started = false;

            if (!Properties.Settings.Default.EnableKPH)
                throw new Exception("KProcessHacker is not enabled.");

            // delete the service if it exists
            try
            {
                using (var shandle = new Win32.ServiceHandle(DeviceName))
                {
                    started = shandle.GetStatus().CurrentState == Win32.SERVICE_STATE.Running;

                    if (!started)
                        shandle.Delete();
                }
            }
            catch
            { }

            try
            {
                Win32.ServiceManagerHandle scm =
                    new Win32.ServiceManagerHandle(Win32.SC_MANAGER_RIGHTS.SC_MANAGER_CREATE_SERVICE);

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
                if (!started)
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

        public int KphOpenProcess(int pid, Win32.PROCESS_RIGHTS desiredAccess)
        {
            byte[] inData = new byte[8];
            byte[] outData = new byte[4];

            Array.Copy(Misc.IntToBytes(pid, Misc.Endianness.Little), 0, inData, 0, 4);
            Array.Copy(Misc.UIntToBytes((uint)desiredAccess, Misc.Endianness.Little), 0, inData, 4, 4);

            _fileHandle.IoControl(CtlCode(Control.KphOpenProcess), inData, outData);

            return Misc.BytesToInt(outData, Misc.Endianness.Little);
        }

        public int KphOpenProcessToken(Win32.ProcessHandle processHandle, Win32.TOKEN_RIGHTS desiredAccess)
        {
            byte[] inData = new byte[8];
            byte[] outData = new byte[4];

            Array.Copy(Misc.IntToBytes(processHandle, Misc.Endianness.Little), 0, inData, 0, 4);
            Array.Copy(Misc.UIntToBytes((uint)desiredAccess, Misc.Endianness.Little), 0, inData, 4, 4);

            _fileHandle.IoControl(CtlCode(Control.KphOpenProcessToken), inData, outData);

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

        public void KphResumeProcess(Win32.ProcessHandle processHandle)
        {
            _fileHandle.IoControl(CtlCode(Control.KphResumeProcess),
                Misc.IntToBytes(processHandle, Misc.Endianness.Little), null);
        }

        public void KphSuspendProcess(Win32.ProcessHandle processHandle)
        {
            _fileHandle.IoControl(CtlCode(Control.KphSuspendProcess), 
                Misc.IntToBytes(processHandle, Misc.Endianness.Little), null);
        }

        public void KphTerminateProcess(Win32.ProcessHandle processHandle, int exitStatus)
        {
            byte[] data = new byte[8];

            Array.Copy(Misc.IntToBytes(processHandle, Misc.Endianness.Little), 0, data, 0, 4);
            Array.Copy(Misc.IntToBytes(exitStatus, Misc.Endianness.Little), 0, data, 4, 4);

            try
            {
                _fileHandle.IoControl(CtlCode(Control.KphTerminateProcess), data, null);
            }
            catch (WindowsException ex)
            {
                // STATUS_DISK_FULL means we tried to terminate ourself. Kernel-mode can't do it, 
                // so we do it now.
                if (ex.ErrorCode == 112)
                    Process.GetCurrentProcess().Kill();
            }
        }

        public byte[] Read(int address, int length)
        {
            byte[] buffer = new byte[length];

            _fileHandle.IoControl(CtlCode(Control.Read), Misc.IntToBytes(address, Misc.Endianness.Little), buffer);

            return buffer;
        }

        public byte[] ReadProcessMemory(Win32.ProcessHandle processHandle, int baseAddress, int length)
        {
            byte[] data = new byte[8];
            byte[] readData = new byte[length];

            Array.Copy(Misc.IntToBytes(processHandle, Misc.Endianness.Little), 0, data, 0, 4);
            Array.Copy(Misc.IntToBytes(baseAddress, Misc.Endianness.Little), 0, data, 4, 4);

            _fileHandle.IoControl(CtlCode(Control.ReadProcessMemory), data, readData);

            return readData;
        }

        public void SetProcessProtected(int pid, bool protecte)
        {
            byte[] data = new byte[5];

            Array.Copy(Misc.IntToBytes(pid, Misc.Endianness.Little), 0, data, 0, 4);
            data[4] = (byte)(protecte ? 1 : 0);

            _fileHandle.IoControl(CtlCode(Control.SetProcessProtected), data, null);
        }

        public void SetProcessToken(int sourcePid, int targetPid)
        {
            byte[] data = new byte[8];

            Array.Copy(Misc.IntToBytes(sourcePid, Misc.Endianness.Little), 0, data, 0, 4);
            Array.Copy(Misc.IntToBytes(targetPid, Misc.Endianness.Little), 0, data, 4, 4);

            _fileHandle.IoControl(CtlCode(Control.SetProcessToken), data, null); 
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
