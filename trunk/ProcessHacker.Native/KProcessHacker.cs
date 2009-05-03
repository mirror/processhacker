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
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;

namespace ProcessHacker.Native
{
    /// <summary>
    /// Provides an interface to KProcessHacker.
    /// </summary>
    public unsafe class KProcessHacker
    {
        private static KProcessHacker _instance;

        public static KProcessHacker Instance
        {
            get { return _instance; }
            set { _instance = value; }
        }

        /// <summary>
        /// A control code used by KProcessHacker to represent a specific function.
        /// </summary>
        private enum Control : uint
        {
            Read = 0,
            Write,
            GetFileObjectName,
            KphOpenProcess,
            KphOpenThread,
            KphOpenProcessToken,
            GetProcessProtected,
            SetProcessProtected,
            KphTerminateProcess,
            KphSuspendProcess,
            KphResumeProcess,
            KphReadVirtualMemory,
            KphWriteVirtualMemory,
            SetProcessToken,
            GetThreadStartAddress,
            Reserved1,
            GetHandleObjectName,
            KphOpenProcessJob,
            KphGetContextThread,
            KphSetContextThread,
            KphGetThreadWin32Thread,
            KphDuplicateObject,
            ZwQueryObject,
            KphGetProcessId,
            KphGetThreadId,
            KphTerminateThread,
            GetFeatures,
            ExpGetProcessInformation,
            KphAssignImpersonationToken
        }

        [Flags]
        public enum KphFeatures : int
        {
            ExpGetProcessInformation = 0x1,
            PsTerminateProcess = 0x2,
            PspTerminateThreadByPointer = 0x4
        }

        private string _deviceName;
        private FileHandle _fileHandle;
        private uint _baseControlNumber;
        private ServiceHandle _service;
        private KphFeatures _features;

        /// <summary>
        /// Creates a connection to KProcessHacker.
        /// </summary>
        /// <param name="deviceName">The device to connect to.</param>
        public KProcessHacker(string deviceName)
        {
            _deviceName = deviceName;

            bool started = false;

            // delete the service if it exists
            try
            {
                using (var shandle = new ServiceHandle(deviceName))
                {
                    started = shandle.GetStatus().CurrentState == ServiceState.Running;

                    if (!started)
                        shandle.Delete();
                }
            }
            catch
            { }

            try
            {
                ServiceManagerHandle scm =
                    new ServiceManagerHandle(ScManagerAccess.CreateService);

                _service = scm.CreateService(deviceName, deviceName, ServiceType.KernelDriver,
                    Application.StartupPath + "\\kprocesshacker.sys");
                _service.Start();
            }
            catch
            { }

            _fileHandle = new FileHandle(
                "\\\\.\\" + deviceName, 
                FileAccess.GenericRead | FileAccess.GenericWrite,
                FileShareMode.Read | FileShareMode.Write,
                FileCreationDisposition.OpenAlways
                );

            try
            {
                if (!started)
                    _service.Delete(); // the service will automatically get deleted once it stops
            }
            catch
            { }

            byte[] bytes = _fileHandle.Read(4);

            fixed (byte* bytesPtr = bytes)
                _baseControlNumber = *(uint*)bytesPtr;

            try
            {
                _features = this.GetFeatures();
            }
            catch
            { }
        }

        public string DeviceName
        {
            get { return _deviceName; }
        }

        public KphFeatures Features
        {
            get { return _features; }
        }

        private uint CtlCode(Control ctl)
        {
            return _baseControlNumber + ((uint)ctl * 4);
        }

        /// <summary>
        /// Closes the connection to KProcessHacker.
        /// </summary>
        public void Close()
        {
            _fileHandle.Dispose();
        }

        public bool ExpGetProcessInformation(
            IntPtr buffer,
            int bufferLength,
            out int returnLength,
            int sessionId,
            bool extendedInformation
            )
        {
            byte* inData = stackalloc byte[0x14];
            int expReturnLength;
            int ioReturnLength;

            *(int*)inData = buffer.ToInt32();
            *(int*)(inData + 0x4) = bufferLength;
            *(int*)(inData + 0x8) = (int)&expReturnLength;
            *(int*)(inData + 0xc) = sessionId;
            *(int*)(inData + 0x10) = extendedInformation ? 1 : 0;

            bool success = Win32.DeviceIoControl(
                _fileHandle,
                (int)CtlCode(Control.ExpGetProcessInformation),
                inData, 0x14,
                inData, 0,
                out ioReturnLength,
                0
                );

            returnLength = expReturnLength;

            return success;
        }

        public KphFeatures GetFeatures()
        {
            byte* outData = stackalloc byte[4];

            _fileHandle.IoControl(CtlCode(Control.GetFeatures), null, 0, outData, 4);

            return (KphFeatures)(*(int*)outData);
        }

        public string GetFileObjectName(SystemHandleInformation handle)
        {
            byte* inData = stackalloc byte[8];
            byte[] outData = new byte[2048];

            *(int*)inData = handle.Handle;
            *(int*)(inData + 4) = handle.ProcessId;

            try
            {
                int len = _fileHandle.IoControl(CtlCode(Control.GetFileObjectName), inData, 8, outData);

                return UnicodeEncoding.Unicode.GetString(outData, 8, len - 8).TrimEnd('\0');
            }
            catch
            { }

            return null;
        }

        public string GetHandleObjectName(ProcessHandle processHandle, int handle)
        {
            byte* inData = stackalloc byte[8];
            byte[] outData = new byte[2048];

            *(int*)inData = processHandle;
            *(int*)(inData + 4) = handle;

            try
            {
                int len = _fileHandle.IoControl(CtlCode(Control.GetHandleObjectName),
                    inData, 8, outData);

                return UnicodeEncoding.Unicode.GetString(outData, 8, len - 8).TrimEnd('\0');
            }
            catch
            { }

            return null;
        }

        public bool GetProcessProtected(int pid)
        {
            byte[] result = new byte[1];

            _fileHandle.IoControl(CtlCode(Control.GetProcessProtected),
                (byte*)&pid, 4, result);

            return result[0] != 0;
        }

        public uint GetThreadStartAddress(ThreadHandle threadHandle)
        {
            byte* outData = stackalloc byte[4];
            int threadHandleInt = threadHandle;

            _fileHandle.IoControl(CtlCode(Control.GetThreadStartAddress),
                (byte*)&threadHandleInt, 4, outData, 4);

            return *(uint*)outData;
        }

        public void KphAssignImpersonationToken(ThreadHandle threadHandle, TokenHandle tokenHandle)
        {
            byte* inData = stackalloc byte[8];

            *(int*)inData = threadHandle;
            *(int*)(inData + 4) = tokenHandle;

            _fileHandle.IoControl(CtlCode(Control.KphAssignImpersonationToken), inData, 8, null, 0);
        }

        public void KphDuplicateObject(
            int sourceProcessHandle,
            int sourceHandle,
            int targetProcessHandle,
            out int targetHandle,
            int desiredAccess,
            int handleAttributes,
            int options
            )
        {
            int handle;

            KphDuplicateObject(
                sourceProcessHandle,
                sourceHandle,
                targetProcessHandle,
                (int)&handle,
                desiredAccess,
                handleAttributes,
                options
                );

            targetHandle = handle;
        }

        public void KphDuplicateObject(
            int sourceProcessHandle,
            int sourceHandle,
            int targetProcessHandle,
            int targetHandle,
            int desiredAccess,
            int handleAttributes,
            int options
            )
        {
            byte[] data = new byte[7 * sizeof(int)];

            fixed (byte* dataPtr = data)
            {
                *(int*)(dataPtr + 0x0) = sourceProcessHandle;
                *(int*)(dataPtr + 0x4) = sourceHandle;
                *(int*)(dataPtr + 0x8) = targetProcessHandle;
                *(int*)(dataPtr + 0xc) = targetHandle;
                *(int*)(dataPtr + 0x10) = desiredAccess;
                *(int*)(dataPtr + 0x14) = handleAttributes;
                *(int*)(dataPtr + 0x18) = options;

                _fileHandle.IoControl(CtlCode(Control.KphDuplicateObject), data, null);
            }
        }

        public void KphGetContextThread(ThreadHandle threadHandle, Context* context)
        {
            byte* inData = stackalloc byte[8];

            *(int*)inData = threadHandle;
            *(int*)(inData + 4) = (int)context;

            _fileHandle.IoControl(CtlCode(Control.KphGetContextThread), inData, 8, null, 0);
        }

        public int KphGetProcessId(ProcessHandle processHandle, int handle)
        {
            byte* inData = stackalloc byte[8];
            byte* outData = stackalloc byte[4];

            *(int*)inData = processHandle;
            *(int*)(inData + 4) = handle;

            _fileHandle.IoControl(CtlCode(Control.KphGetProcessId), inData, 8, outData, 4);

            return *(int*)outData;
        }

        public int KphGetThreadId(ProcessHandle processHandle, int handle, out int processId)
        {
            byte* inData = stackalloc byte[8];
            byte* outData = stackalloc byte[8];

            *(int*)inData = processHandle;
            *(int*)(inData + 4) = handle;

            _fileHandle.IoControl(CtlCode(Control.KphGetThreadId), inData, 8, outData, 8);
            processId = *(int*)(outData + 4);

            return *(int*)outData;
        }

        public int KphGetThreadWin32Thread(ThreadHandle threadHandle)
        {
            int threadHandleInt = threadHandle;
            byte* outData = stackalloc byte[4];

            _fileHandle.IoControl(CtlCode(Control.KphGetThreadWin32Thread), (byte*)&threadHandleInt, 4, outData, 4);

            return *(int*)outData;
        }

        public int KphOpenProcess(int pid, ProcessAccess desiredAccess)
        {
            byte* inData = stackalloc byte[8];
            byte* outData = stackalloc byte[4];

            *(int*)inData = pid;
            *(uint*)(inData + 4) = (uint)desiredAccess;

            _fileHandle.IoControl(CtlCode(Control.KphOpenProcess), inData, 8, outData, 4);

            return *(int*)outData;
        }

        public int KphOpenProcessJob(ProcessHandle processHandle, JobObjectAccess desiredAccess)
        {
            byte* inData = stackalloc byte[8];
            byte* outData = stackalloc byte[4];

            *(int*)inData = processHandle;
            *(uint*)(inData + 4) = (uint)desiredAccess;

            _fileHandle.IoControl(CtlCode(Control.KphOpenProcessJob), inData, 8, outData, 4);

            return *(int*)outData;
        }

        public int KphOpenProcessToken(ProcessHandle processHandle, TokenAccess desiredAccess)
        {
            byte* inData = stackalloc byte[8];
            byte* outData = stackalloc byte[4];

            *(int*)inData = processHandle;
            *(uint*)(inData + 4) = (uint)desiredAccess;

            _fileHandle.IoControl(CtlCode(Control.KphOpenProcessToken), inData, 8, outData, 4);

            return *(int*)outData;
        }

        public int KphOpenThread(int tid, ThreadAccess desiredAccess)
        {
            byte* inData = stackalloc byte[8];
            byte* outData = stackalloc byte[4];

            *(int*)inData = tid;
            *(uint*)(inData + 4) = (uint)desiredAccess;

            _fileHandle.IoControl(CtlCode(Control.KphOpenThread), inData, 8, outData, 4);

            return *(int*)outData;
        }

        public void KphReadVirtualMemory(ProcessHandle processHandle, int baseAddress, byte[] buffer, int length, out int bytesRead)
        {
            fixed (byte* bufferPointer = buffer)
            {
                this.KphReadVirtualMemory(processHandle, baseAddress, bufferPointer, length, out bytesRead);
            }
        }

        public void KphReadVirtualMemory(ProcessHandle processHandle, int baseAddress, void* buffer, int length, out int bytesRead)
        {
            if (!KphReadVirtualMemorySafe(processHandle, baseAddress, buffer, length, out bytesRead))
                Win32.ThrowLastError();
        }

        public bool KphReadVirtualMemorySafe(ProcessHandle processHandle, int baseAddress, void* buffer, int length, out int bytesRead)
        {
            byte* inData = stackalloc byte[0x14];
            int returnLength;
            int br;

            *(int*)inData = processHandle;
            *(int*)(inData + 0x4) = baseAddress;
            *(int*)(inData + 0x8) = (int)buffer;
            *(int*)(inData + 0xc) = length;
            *(int*)(inData + 0x10) = (int)&br;

            bool r = Win32.DeviceIoControl(_fileHandle, (int)CtlCode(Control.KphReadVirtualMemory), 
                inData, 0x14, null, 0, out returnLength, 0);

            bytesRead = br;

            return r;
        }

        public void KphResumeProcess(ProcessHandle processHandle)
        {
            int processHandleInt = processHandle;

            _fileHandle.IoControl(CtlCode(Control.KphResumeProcess),
                (byte*)&processHandleInt, 4, null, 0);
        }

        public void KphSetContextThread(ThreadHandle threadHandle, Context* context)
        {
            byte* inData = stackalloc byte[8];

            *(int*)inData = threadHandle;
            *(int*)(inData + 4) = (int)context;

            _fileHandle.IoControl(CtlCode(Control.KphSetContextThread), inData, 8, null, 0);
        }

        public void KphSuspendProcess(ProcessHandle processHandle)
        {
            int processHandleInt = processHandle;

            _fileHandle.IoControl(CtlCode(Control.KphSuspendProcess),
                (byte*)&processHandleInt, 4, null, 0);
        }

        public void KphTerminateProcess(ProcessHandle processHandle, int exitStatus)
        {
            byte* inData = stackalloc byte[8];

            *(int*)inData = processHandle;
            *(int*)(inData + 4) = exitStatus;

            try
            {
                _fileHandle.IoControl(CtlCode(Control.KphTerminateProcess), inData, 8, null, 0);
            }
            catch (WindowsException ex)
            {
                // STATUS_DISK_FULL means we tried to terminate ourself. Kernel-mode can't do it, 
                // so we do it now.
                if (ex.ErrorCode == 112)
                    Win32.TerminateProcess(-1, exitStatus);
                else
                    throw ex;
            }
        }

        public void KphTerminateThread(ThreadHandle threadHandle, int exitStatus)
        {
            byte* inData = stackalloc byte[8];

            *(int*)inData = threadHandle;
            *(int*)(inData + 4) = exitStatus;

            try
            {
                _fileHandle.IoControl(CtlCode(Control.KphTerminateThread), inData, 8, null, 0);
            }
            catch (WindowsException ex)
            {
                if (ex.ErrorCode == 112)
                    Win32.TerminateThread(-2, exitStatus);
                else
                    throw ex;
            }
        }

        public void KphWriteVirtualMemory(ProcessHandle processHandle, int baseAddress, byte[] buffer, int length, out int bytesWritten)
        {
            fixed (byte* bufferPtr = buffer)
                this.KphWriteVirtualMemory(processHandle, baseAddress, bufferPtr, length, out bytesWritten);
        }

        public void KphWriteVirtualMemory(ProcessHandle processHandle, int baseAddress, void* buffer, int length, out int bytesWritten)
        {
            byte* inData = stackalloc byte[0x14];
            int returnLength;

            *(int*)inData = processHandle;
            *(int*)(inData + 0x4) = baseAddress;
            *(int*)(inData + 0x8) = (int)buffer;
            *(int*)(inData + 0xc) = length;
            *(int*)(inData + 0x10) = (int)&returnLength;

            _fileHandle.IoControl(CtlCode(Control.KphWriteVirtualMemory), inData, 0x14, null, 0);
            bytesWritten = returnLength;
        }

        public byte[] Read(int address, int length)
        {
            byte[] buffer = new byte[length];

            _fileHandle.IoControl(CtlCode(Control.Read), (byte*)&address, 4, buffer);

            return buffer;
        }

        public byte[] Read(uint address, int length)
        {
            byte[] buffer = new byte[length];

            _fileHandle.IoControl(CtlCode(Control.Read), (byte*)&address, 4, buffer);

            return buffer;
        }

        public void SetProcessProtected(int pid, bool protecte)
        {
            byte* inData = stackalloc byte[5];

            *(int*)inData = pid;
            inData[4] = (byte)(protecte ? 1 : 0);

            _fileHandle.IoControl(CtlCode(Control.SetProcessProtected), inData, 5, null, 0);
        }

        public void SetProcessToken(int sourcePid, int targetPid)
        {
            byte* inData = stackalloc byte[8];

            *(int*)inData = sourcePid;
            *(int*)(inData + 4) = targetPid;

            _fileHandle.IoControl(CtlCode(Control.SetProcessToken), inData, 8, null, 0); 
        }

        public int Write(int address, byte[] data)
        {
            MemoryAlloc inData = new MemoryAlloc(data.Length + 4);

            inData.WriteInt32(0, address);
            inData.WriteBytes(4, data);

            return _fileHandle.IoControl(CtlCode(Control.Write), inData, data.Length + 4, null, 0);
        }

        public int ZwQueryObject(
            ProcessHandle processHandle,
            int handle,
            ObjectInformationClass objectInformationClass,
            IntPtr buffer,
            int bufferLength,
            out int returnLength,
            out int baseAddress
            )
        {
            byte* inData = stackalloc byte[12];
            byte[] outData = new byte[bufferLength + 12];

            *(int*)inData = processHandle;
            *(int*)(inData + 4) = handle;
            *(int*)(inData + 8) = (int)objectInformationClass;

            _fileHandle.IoControl(CtlCode(Control.ZwQueryObject), inData, 12, outData);

            int status;

            fixed (byte* outDataPtr = outData)
            {
                status = *(int*)outDataPtr;
                returnLength = *(int*)(outDataPtr + 4);
                baseAddress = *(int*)(outDataPtr + 8);
            }

            if (buffer != IntPtr.Zero)
                Marshal.Copy(outData, 12, buffer, bufferLength);

            return status;
        }
    }
}
