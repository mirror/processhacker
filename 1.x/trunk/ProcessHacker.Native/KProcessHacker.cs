/*
 * Process Hacker - 
 *   KProcessHacker interfacing code
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

// The private field 'field' is assigned but its value is never used
#pragma warning disable 0414

using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using ProcessHacker.Common.Objects;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;
using ProcessHacker.Native.SsLogging;

namespace ProcessHacker.Native
{
    /// <summary>
    /// Provides an interface to KProcessHacker.
    /// </summary>
    public sealed unsafe class KProcessHacker
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
            ClientCloseHandle = 0,
            SsQueryClientEntry,
            Reserved1,
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
            SetHandleAttributes,
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
            KphSetHandleGrantedAccess,
            KphAssignImpersonationToken,
            ProtectAdd,
            ProtectRemove,
            ProtectQuery,
            KphUnsafeReadVirtualMemory,
            SetExecuteOptions,
            KphQueryProcessHandles,
            KphOpenThreadProcess,
            KphCaptureStackBackTraceThread,
            KphDangerousTerminateThread,
            KphOpenType,
            KphOpenDriver,
            KphQueryInformationDriver,
            KphOpenDirectoryObject,
            SsRef,
            SsUnref,
            SsCreateClientEntry,
            SsCreateRuleSetEntry,
            SsRemoveRule,
            SsAddProcessIdRule,
            SsAddThreadIdRule,
            SsAddPreviousModeRule,
            SsAddNumberRule,
            SsEnableClientEntry,
            KphOpenNamedObject,
            KphQueryInformationProcess,
            KphQueryInformationThread,
            KphSetInformationProcess,
            KphSetInformationThread,
        }

        [Flags]
        public enum KphFeatures : int
        {
            PsTerminateProcess = 0x1,
            PspTerminateThreadByPointer = 0x2
        }

        private string _deviceName;
        private FileHandle _fileHandle;
        private uint _baseControlNumber;
        private KphFeatures _features;

        /// <summary>
        /// Creates a connection to KProcessHacker.
        /// </summary>
        public KProcessHacker()
            : this("KProcessHacker")
        { }

        /// <summary>
        /// Creates a connection to KProcessHacker.
        /// </summary>
        /// <param name="deviceName">The name of the KProcessHacker service and device.</param>
        public KProcessHacker(string deviceName)
            : this(deviceName, Application.StartupPath + "\\kprocesshacker.sys")
        { }

        /// <summary>
        /// Creates a connection to KProcessHacker.
        /// </summary>
        /// <param name="deviceName">The name of the KProcessHacker service and device.</param>
        /// <param name="fileName">The file name of the KProcessHacker driver.</param>
        public KProcessHacker(string deviceName, string fileName)
        {
            _deviceName = deviceName;

            if (OSVersion.Architecture != OSArch.I386)
                throw new NotSupportedException("KProcessHacker does not support 64-bit Windows.");

            try
            {
                _fileHandle = new FileHandle(
                    @"\Device\" + deviceName,
                    0,
                    FileAccess.GenericRead | FileAccess.GenericWrite
                    );
            }
            catch (WindowsException ex)
            {
                if (
                    ex.Status == NtStatus.NoSuchDevice ||
                    ex.Status == NtStatus.NoSuchFile ||
                    ex.Status == NtStatus.ObjectNameNotFound
                    )
                {
                    // Attempt to load the driver, then try again.
                    ServiceHandle shandle;
                    bool created = false;

                    try
                    {
                        using (shandle = new ServiceHandle("KProcessHacker", ServiceAccess.Start))
                        {
                            shandle.Start();
                        }
                    }
                    catch
                    {
                        using (var scm = new ServiceManagerHandle(ScManagerAccess.CreateService))
                        {
                            shandle = scm.CreateService(
                                deviceName,
                                deviceName,
                                ServiceType.KernelDriver,
                                fileName
                                );
                            shandle.Start();
                            created = true;
                        }
                    }

                    try
                    {
                        _fileHandle = new FileHandle(
                            @"\Device\" + deviceName,
                            0,
                            FileAccess.GenericRead | FileAccess.GenericWrite
                            );
                    }
                    finally
                    {
                        if (shandle != null)
                        {
                            if (created)
                            {
                                // The SCM will delete the service when it is stopped.
                                shandle.Delete();
                            }

                            shandle.Dispose();
                        }
                    }
                }
                else
                {
                    throw ex;
                }
            }

            _fileHandle.SetHandleFlags(Win32HandleFlags.ProtectFromClose, Win32HandleFlags.ProtectFromClose);

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

        private int CtlCode(Control ctl)
        {
            return (int)(_baseControlNumber + ((uint)ctl * 4));
        }

        /// <summary>
        /// Closes the connection to KProcessHacker.
        /// </summary>
        public void Close()
        {
            _fileHandle.SetHandleFlags(Win32HandleFlags.ProtectFromClose, 0);
            _fileHandle.Dispose();
        }

        public void ClientCloseHandle(IntPtr handle)
        {
            byte* inData = stackalloc byte[4];

            *(int*)inData = handle.ToInt32();

            _fileHandle.IoControl(CtlCode(Control.ClientCloseHandle), inData, 4, null, 0);
        }

        public KphFeatures GetFeatures()
        {
            byte* outData = stackalloc byte[4];

            _fileHandle.IoControl(CtlCode(Control.GetFeatures), null, 0, outData, 4);

            return (KphFeatures)(*(int*)outData);
        }

        public string GetHandleObjectName(ProcessHandle processHandle, IntPtr handle)
        {
            byte* inData = stackalloc byte[8];
            byte[] outData = new byte[2048];

            *(int*)inData = processHandle;
            *(int*)(inData + 4) = handle.ToInt32();

            try
            {
                int len = _fileHandle.IoControl(CtlCode(Control.GetHandleObjectName),
                    inData, 8, outData);

                return Encoding.Unicode.GetString(outData, 8, len - 8).TrimEnd('\0');
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

        public unsafe int KphCaptureStackBackTraceThread(
            ThreadHandle threadHandle,
            int framesToSkip,
            int framesToCapture,
            IntPtr[] backTrace,
            out int backTraceHash
            )
        {
            byte* inData = stackalloc byte[6 * sizeof(int)];
            int capturedFramesLocal;
            int backTraceHashLocal;

            if (framesToCapture > backTrace.Length)
                throw new ArgumentOutOfRangeException("Back trace buffer is too small.");

            fixed (IntPtr* backTracePtr = backTrace)
            {
                *(int*)inData = threadHandle;
                *(int*)(inData + 0x4) = framesToSkip;
                *(int*)(inData + 0x8) = framesToCapture;
                *(int*)(inData + 0xc) = (int)backTracePtr;
                *(int*)(inData + 0x10) = (int)&capturedFramesLocal;
                *(int*)(inData + 0x14) = (int)&backTraceHashLocal;

                _fileHandle.IoControl(CtlCode(Control.KphCaptureStackBackTraceThread), inData, 6 * sizeof(int), null, 0);
                backTraceHash = backTraceHashLocal;

                return capturedFramesLocal;
            }
        }

        public void KphDangerousTerminateThread(ThreadHandle threadHandle, NtStatus exitStatus)
        {
            byte* inData = stackalloc byte[8];

            *(int*)inData = threadHandle;
            *(int*)(inData + 4) = (int)exitStatus;

            _fileHandle.IoControl(CtlCode(Control.KphDangerousTerminateThread), inData, 8, null, 0);
        }

        public void KphDuplicateObject(
            int sourceProcessHandle,
            int sourceHandle,
            int targetProcessHandle,
            out int targetHandle,
            int desiredAccess,
            HandleFlags handleAttributes,
            DuplicateOptions options
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
            HandleFlags handleAttributes,
            DuplicateOptions options
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
                *(int*)(dataPtr + 0x14) = (int)handleAttributes;
                *(int*)(dataPtr + 0x18) = (int)options;

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

        public int KphGetProcessId(ProcessHandle processHandle, IntPtr handle)
        {
            byte* inData = stackalloc byte[8];
            byte* outData = stackalloc byte[4];

            *(int*)inData = processHandle;
            *(int*)(inData + 4) = handle.ToInt32();

            _fileHandle.IoControl(CtlCode(Control.KphGetProcessId), inData, 8, outData, 4);

            return *(int*)outData;
        }

        public int KphGetThreadId(ProcessHandle processHandle, IntPtr handle, out int processId)
        {
            byte* inData = stackalloc byte[8];
            byte* outData = stackalloc byte[8];

            *(int*)inData = processHandle;
            *(int*)(inData + 4) = handle.ToInt32();

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

        public int KphOpenDirectoryObject(DirectoryAccess access, ObjectAttributes objectAttributes)
        {
            byte* inData = stackalloc byte[0xc];
            int directoryObjectHandle;

            *(int*)inData = (int)&directoryObjectHandle;
            *(int*)(inData + 0x4) = (int)access;
            *(int*)(inData + 0x8) = (int)&objectAttributes;

            _fileHandle.IoControl(CtlCode(Control.KphOpenDirectoryObject), inData, 0xc, null, 0);

            return directoryObjectHandle;
        }

        public int KphOpenDriver(ObjectAttributes objectAttributes)
        {
            byte* inData = stackalloc byte[8];
            int driverHandle;

            *(int*)inData = (int)&driverHandle;
            *(int*)(inData + 4) = (int)&objectAttributes;

            _fileHandle.IoControl(CtlCode(Control.KphOpenDriver), inData, 8, null, 0);

            return driverHandle;
        }

        public int KphOpenNamedObject(int access, ObjectAttributes objectAttributes)
        {
            byte* inData = stackalloc byte[0xc];
            int handle;

            *(int*)inData = (int)&handle;
            *(int*)(inData + 4) = access;
            *(int*)(inData + 8) = (int)&objectAttributes;

            _fileHandle.IoControl(CtlCode(Control.KphOpenNamedObject), inData, 0xc, null, 0);

            return handle;
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

        public int KphOpenThreadProcess(ThreadHandle threadHandle, ProcessAccess desiredAccess)
        {
            byte* inData = stackalloc byte[8];
            byte* outData = stackalloc byte[4];

            *(int*)inData = threadHandle;
            *(uint*)(inData + 4) = (uint)desiredAccess;

            _fileHandle.IoControl(CtlCode(Control.KphOpenThreadProcess), inData, 8, outData, 4);

            return *(int*)outData;
        }

        public int KphOpenType(ObjectAttributes objectAttributes)
        {
            byte* inData = stackalloc byte[8];
            int typeHandle;

            *(int*)inData = (int)&typeHandle;
            *(int*)(inData + 4) = (int)&objectAttributes;

            _fileHandle.IoControl(CtlCode(Control.KphOpenType), inData, 8, null, 0);

            return typeHandle;
        }

        public void KphQueryInformationDriver(
            DriverHandle driverHandle,
            DriverInformationClass driverInformationClass,
            IntPtr driverInformation,
            int driverInformationLength,
            out int returnLength
            )
        {
            byte* inData = stackalloc byte[0x14];
            int returnLengthLocal;

            *(int*)inData = driverHandle;
            *(int*)(inData + 0x4) = (int)driverInformationClass;
            *(int*)(inData + 0x8) = driverInformation.ToInt32();
            *(int*)(inData + 0xc) = driverInformationLength;
            *(int*)(inData + 0x10) = (int)&returnLengthLocal;

            try
            {
                _fileHandle.IoControl(CtlCode(Control.KphQueryInformationDriver), inData, 0x14, null, 0);
            }
            finally
            {
                returnLength = returnLengthLocal;
            }
        }

        public void KphQueryInformationProcess(
            ProcessHandle processHandle,
            ProcessInformationClass processInformationClass,
            IntPtr processInformation,
            int processInformationLength,
            out int returnLength
            )
        {
            byte* inData = stackalloc byte[0x14];
            int returnLengthLocal;

            *(int*)inData = processHandle;
            *(int*)(inData + 0x4) = (int)processInformationClass;
            *(int*)(inData + 0x8) = processInformation.ToInt32();
            *(int*)(inData + 0xc) = processInformationLength;
            *(int*)(inData + 0x10) = (int)&returnLengthLocal;

            try
            {
                _fileHandle.IoControl(CtlCode(Control.KphQueryInformationProcess), inData, 0x14, null, 0);
            }
            finally
            {
                returnLength = returnLengthLocal;
            }
        }

        public void KphQueryInformationThread(
            ThreadHandle threadHandle,
            ThreadInformationClass threadInformationClass,
            IntPtr threadInformation,
            int threadInformationLength,
            out int returnLength
            )
        {
            byte* inData = stackalloc byte[0x14];
            int returnLengthLocal;

            *(int*)inData = threadHandle;
            *(int*)(inData + 0x4) = (int)threadInformationClass;
            *(int*)(inData + 0x8) = threadInformation.ToInt32();
            *(int*)(inData + 0xc) = threadInformationLength;
            *(int*)(inData + 0x10) = (int)&returnLengthLocal;

            try
            {
                _fileHandle.IoControl(CtlCode(Control.KphQueryInformationThread), inData, 0x14, null, 0);
            }
            finally
            {
                returnLength = returnLengthLocal;
            }
        }

        public void KphQueryProcessHandles(ProcessHandle processHandle, IntPtr buffer, int bufferLength, out int returnLength)
        {
            byte* inData = stackalloc byte[0x10];
            int returnLengthLocal;

            *(int*)inData = processHandle;
            *(int*)(inData + 0x4) = buffer.ToInt32();
            *(int*)(inData + 0x8) = bufferLength;
            *(int*)(inData + 0xc) = (int)&returnLengthLocal;

            try
            {
                _fileHandle.IoControl(CtlCode(Control.KphQueryProcessHandles), inData, 0x10, null, 0);
            }
            finally
            {
                returnLength = returnLengthLocal;
            }
        }

        public void KphReadVirtualMemory(ProcessHandle processHandle, int baseAddress, byte[] buffer, int length, out int bytesRead)
        {
            fixed (byte* bufferPtr = buffer)
            {
                this.KphReadVirtualMemory(processHandle, baseAddress, new IntPtr(bufferPtr), length, out bytesRead);
            }
        }

        public void KphReadVirtualMemory(ProcessHandle processHandle, int baseAddress, IntPtr buffer, int length, out int bytesRead)
        {
            NtStatus status;

            status = KphReadVirtualMemorySafe(processHandle, baseAddress, buffer, length, out bytesRead);

            if (status >= NtStatus.Error)
                Win32.Throw(status);
        }

        public NtStatus KphReadVirtualMemorySafe(ProcessHandle processHandle, int baseAddress, IntPtr buffer, int length, out int bytesRead)
        {
            NtStatus status;
            byte* inData = stackalloc byte[0x14];
            int returnLength;
            int br;

            *(int*)inData = processHandle;
            *(int*)(inData + 0x4) = baseAddress;
            *(int*)(inData + 0x8) = (int)buffer;
            *(int*)(inData + 0xc) = length;
            *(int*)(inData + 0x10) = (int)&br;

            status = _fileHandle.IoControl(CtlCode(Control.KphReadVirtualMemory), (IntPtr)inData, 0x14, IntPtr.Zero, 0, out returnLength);

            bytesRead = br;

            return status;
        }

        public NtStatus KphReadVirtualMemoryUnsafe(ProcessHandle processHandle, int baseAddress, void* buffer, int length, out int bytesRead)
        {
            return KphReadVirtualMemoryUnsafe(processHandle, baseAddress, new IntPtr(buffer), length, out bytesRead);
        }

        public NtStatus KphReadVirtualMemoryUnsafe(ProcessHandle processHandle, int baseAddress, IntPtr buffer, int length, out int bytesRead)
        {
            NtStatus status;
            byte* inData = stackalloc byte[0x14];
            int returnLength;
            int br;

            *(int*)inData = processHandle;
            *(int*)(inData + 0x4) = baseAddress;
            *(int*)(inData + 0x8) = (int)buffer;
            *(int*)(inData + 0xc) = length;
            *(int*)(inData + 0x10) = (int)&br;

            status = _fileHandle.IoControl(CtlCode(Control.KphUnsafeReadVirtualMemory), (IntPtr)inData, 0x14, IntPtr.Zero, 0, out returnLength);

            bytesRead = br;

            return status;
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

        public void KphSetHandleGrantedAccess(IntPtr handle, int grantedAccess)
        {
            byte* inData = stackalloc byte[8];

            *(int*)inData = handle.ToInt32();
            *(int*)(inData + 4) = grantedAccess;

            _fileHandle.IoControl(CtlCode(Control.KphSetHandleGrantedAccess), inData, 8, null, 0);
        }

        public void KphSetInformationProcess(
            ProcessHandle processHandle,
            ProcessInformationClass processInformationClass,
            IntPtr processInformation,
            int processInformationLength
            )
        {
            byte* inData = stackalloc byte[0x10];

            *(int*)inData = processHandle;
            *(int*)(inData + 0x4) = (int)processInformationClass;
            *(int*)(inData + 0x8) = processInformation.ToInt32();
            *(int*)(inData + 0xc) = processInformationLength;

            _fileHandle.IoControl(CtlCode(Control.KphSetInformationProcess), inData, 0x10, null, 0);
        }

        public void KphSetInformationThread(
            ThreadHandle threadHandle,
            ThreadInformationClass threadInformationClass,
            IntPtr threadInformation,
            int threadInformationLength
            )
        {
            byte* inData = stackalloc byte[0x10];

            *(int*)inData = threadHandle;
            *(int*)(inData + 0x4) = (int)threadInformationClass;
            *(int*)(inData + 0x8) = threadInformation.ToInt32();
            *(int*)(inData + 0xc) = threadInformationLength;

            _fileHandle.IoControl(CtlCode(Control.KphSetInformationThread), inData, 0x10, null, 0);
        }

        public void KphSuspendProcess(ProcessHandle processHandle)
        {
            int processHandleInt = processHandle;

            _fileHandle.IoControl(CtlCode(Control.KphSuspendProcess),
                (byte*)&processHandleInt, 4, null, 0);
        }

        public void KphTerminateProcess(ProcessHandle processHandle, NtStatus exitStatus)
        {
            byte* inData = stackalloc byte[8];

            *(int*)inData = processHandle;
            *(int*)(inData + 4) = (int)exitStatus;

            try
            {
                _fileHandle.IoControl(CtlCode(Control.KphTerminateProcess), inData, 8, null, 0);
            }
            catch (WindowsException ex)
            {
                // STATUS_CANT_TERMINATE_SELF means we tried to terminate ourself. Kernel-mode can't do it, 
                // so we do it now.
                if (ex.Status == NtStatus.CantTerminateSelf)
                    Win32.TerminateProcess(new IntPtr(-1), (int)exitStatus);
                else
                    throw ex;
            }
        }

        public void KphTerminateThread(ThreadHandle threadHandle, NtStatus exitStatus)
        {
            byte* inData = stackalloc byte[8];

            *(int*)inData = threadHandle;
            *(int*)(inData + 4) = (int)exitStatus;

            try
            {
                _fileHandle.IoControl(CtlCode(Control.KphTerminateThread), inData, 8, null, 0);
            }
            catch (WindowsException ex)
            {
                if (ex.Status == NtStatus.CantTerminateSelf)
                    Win32.TerminateThread(new IntPtr(-2), (int)exitStatus);
                else
                    throw ex;
            }
        }

        public void KphWriteVirtualMemory(ProcessHandle processHandle, int baseAddress, byte[] buffer, int length, out int bytesWritten)
        {
            fixed (byte* bufferPtr = buffer)
                this.KphWriteVirtualMemory(processHandle, baseAddress, new IntPtr(bufferPtr), length, out bytesWritten);
        }

        public void KphWriteVirtualMemory(ProcessHandle processHandle, int baseAddress, IntPtr buffer, int length, out int bytesWritten)
        {
            byte* inData = stackalloc byte[0x14];
            int returnLength;

            *(int*)inData = processHandle;
            *(int*)(inData + 0x4) = baseAddress;
            *(int*)(inData + 0x8) = (int)buffer;
            *(int*)(inData + 0xc) = length;
            *(int*)(inData + 0x10) = (int)&returnLength;

            try
            {
                _fileHandle.IoControl(CtlCode(Control.KphWriteVirtualMemory), inData, 0x14, null, 0);
            }
            finally
            {
                bytesWritten = returnLength;
            }
        }

        public void ProtectAdd(ProcessHandle processHandle, bool allowKernelMode, ProcessAccess ProcessAllowMask, ThreadAccess ThreadAllowMask)
        {
            byte* inData = stackalloc byte[16];

            *(int*)inData = processHandle;
            *(int*)(inData + 0x4) = allowKernelMode ? 1 : 0;
            *(int*)(inData + 0x8) = (int)ProcessAllowMask;
            *(int*)(inData + 0xc) = (int)ThreadAllowMask;

            _fileHandle.IoControl(CtlCode(Control.ProtectAdd), inData, 16, null, 0);
        }

        public void ProtectQuery(ProcessHandle processHandle, out bool AllowKernelMode, out ProcessAccess ProcessAllowMask, out ThreadAccess ThreadAllowMask)
        {
            byte* inData = stackalloc byte[16];
            int allowKernelMode;
            ProcessAccess processAllowMask;
            ThreadAccess threadAllowMask;

            *(int*)inData = processHandle;
            *(int*)(inData + 0x4) = (int)&allowKernelMode;
            *(int*)(inData + 0x8) = (int)&processAllowMask;
            *(int*)(inData + 0xc) = (int)&threadAllowMask;

            _fileHandle.IoControl(CtlCode(Control.ProtectQuery), inData, 16, null, 0);

            AllowKernelMode = allowKernelMode != 0;
            ProcessAllowMask = processAllowMask;
            ThreadAllowMask = threadAllowMask;
        }

        public void ProtectRemove(ProcessHandle processHandle)
        {
            int processHandleInt = processHandle;

            _fileHandle.IoControl(CtlCode(Control.ProtectRemove),
                (byte*)&processHandleInt, 4, null, 0);
        }

        public void SetExecuteOptions(ProcessHandle processHandle, MemExecuteOptions executeOptions)
        {
            byte* inData = stackalloc byte[8];

            *(int*)inData = processHandle;
            *(int*)(inData + 4) = (int)executeOptions;

            _fileHandle.IoControl(CtlCode(Control.SetExecuteOptions), inData, 8, null, 0);
        }

        public void SetHandleAttributes(ProcessHandle processHandle, IntPtr handle, HandleFlags flags)
        {
            byte* inData = stackalloc byte[12];

            *(int*)inData = processHandle;
            *(int*)(inData + 4) = handle.ToInt32();
            *(int*)(inData + 8) = (int)flags;

            _fileHandle.IoControl(CtlCode(Control.SetHandleAttributes), inData, 12, null, 0);
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

        public IntPtr SsAddProcessIdRule(
            KphSsRuleSetEntryHandle ruleSetEntryHandle,
            KphSsFilterType filterType,
            IntPtr processId
            )
        {
            byte* inData = stackalloc byte[0xc];
            byte* outData = stackalloc byte[4];

            *(int*)inData = ruleSetEntryHandle.Handle.ToInt32();
            *(int*)(inData + 0x4) = (int)filterType;
            *(int*)(inData + 0x8) = processId.ToInt32();

            _fileHandle.IoControl(CtlCode(Control.SsAddProcessIdRule), inData, 0xc, outData, 4);

            return (*(int*)outData).ToIntPtr();
        }

        public IntPtr SsAddThreadIdRule(
            KphSsRuleSetEntryHandle ruleSetEntryHandle,
            KphSsFilterType filterType,
            IntPtr threadId
            )
        {
            byte* inData = stackalloc byte[0xc];
            byte* outData = stackalloc byte[4];

            *(int*)inData = ruleSetEntryHandle.Handle.ToInt32();
            *(int*)(inData + 0x4) = (int)filterType;
            *(int*)(inData + 0x8) = threadId.ToInt32();

            _fileHandle.IoControl(CtlCode(Control.SsAddThreadIdRule), inData, 0xc, outData, 4);

            return (*(int*)outData).ToIntPtr();
        }

        public IntPtr SsAddPreviousModeRule(
            KphSsRuleSetEntryHandle ruleSetEntryHandle,
            KphSsFilterType filterType,
            KProcessorMode previousMode
            )
        {
            byte* inData = stackalloc byte[0x9];
            byte* outData = stackalloc byte[4];

            *(int*)inData = ruleSetEntryHandle.Handle.ToInt32();
            *(int*)(inData + 0x4) = (int)filterType;
            *(byte*)(inData + 0x8) = (byte)previousMode;

            _fileHandle.IoControl(CtlCode(Control.SsAddPreviousModeRule), inData, 0x9, outData, 4);

            return (*(int*)outData).ToIntPtr();
        }

        public IntPtr SsAddNumberRule(
            KphSsRuleSetEntryHandle ruleSetEntryHandle,
            KphSsFilterType filterType,
            int number
            )
        {
            byte* inData = stackalloc byte[0xc];
            byte* outData = stackalloc byte[4];

            *(int*)inData = ruleSetEntryHandle.Handle.ToInt32();
            *(int*)(inData + 0x4) = (int)filterType;
            *(int*)(inData + 0x8) = number;

            _fileHandle.IoControl(CtlCode(Control.SsAddNumberRule), inData, 0xc, outData, 4);

            return (*(int*)outData).ToIntPtr();
        }

        public KphSsClientEntryHandle SsCreateClientEntry(
            ProcessHandle processHandle,
            SemaphoreHandle readSemaphoreHandle,
            SemaphoreHandle writeSemaphoreHandle,
            IntPtr bufferBase,
            int bufferSize
            )
        {
            byte* inData = stackalloc byte[0x14];
            byte* outData = stackalloc byte[4];

            *(int*)inData = processHandle;
            *(int*)(inData + 0x4) = readSemaphoreHandle;
            *(int*)(inData + 0x8) = writeSemaphoreHandle;
            *(int*)(inData + 0xc) = bufferBase.ToInt32();
            *(int*)(inData + 0x10) = bufferSize;

            _fileHandle.IoControl(CtlCode(Control.SsCreateClientEntry), inData, 0x14, outData, 4);

            return new KphSsClientEntryHandle((*(int*)outData).ToIntPtr());
        }

        public KphSsRuleSetEntryHandle SsCreateRuleSetEntry(
            KphSsClientEntryHandle clientEntryHandle,
            KphSsFilterType defaultFilterType,
            KphSsRuleSetAction action
            )
        {
            byte* inData = stackalloc byte[0xc];
            byte* outData = stackalloc byte[4];

            *(int*)inData = clientEntryHandle.Handle.ToInt32();
            *(int*)(inData + 0x4) = (int)defaultFilterType;
            *(int*)(inData + 0x8) = (int)action;

            _fileHandle.IoControl(CtlCode(Control.SsCreateRuleSetEntry), inData, 0xc, outData, 4);

            return new KphSsRuleSetEntryHandle((*(int*)outData).ToIntPtr());
        }

        public void SsEnableClientEntry(
            KphSsClientEntryHandle clientEntryHandle,
            bool enable
            )
        {
            byte* inData = stackalloc byte[5];

            *(int*)inData = clientEntryHandle.Handle.ToInt32();
            *(byte*)(inData + 4) = (byte)(enable ? 1 : 0);

            _fileHandle.IoControl(CtlCode(Control.SsEnableClientEntry), inData, 5, null, 0);
        }

        public void SsQueryClientEntry(
            KphSsClientEntryHandle clientEntryHandle,
            out KphSsClientInformation clientInformation,
            int clientInformationLength,
            out int returnLength
            )
        {
            fixed (KphSsClientInformation *clientInfoPtr = &clientInformation)
            fixed (int* retLengthPtr = &returnLength)
            {
                byte* inData = stackalloc byte[0x10];

                *(int*)inData = clientEntryHandle.Handle.ToInt32();
                *(int*)(inData + 0x4) = (int)clientInfoPtr;
                *(int*)(inData + 0x8) = clientInformationLength;
                *(int*)(inData + 0xc) = (int)retLengthPtr;

                _fileHandle.IoControl(CtlCode(Control.SsQueryClientEntry), inData, 0x10, null, 0);
            }
        }

        public void SsRemoveRule(
            KphSsRuleSetEntryHandle ruleSetEntryHandle,
            IntPtr ruleEntryHandle
            )
        {
            byte* inData = stackalloc byte[8];

            *(int*)inData = ruleSetEntryHandle.Handle.ToInt32();
            *(int*)(inData + 4) = ruleEntryHandle.ToInt32();

            _fileHandle.IoControl(CtlCode(Control.SsRemoveRule), inData, 8, null, 0);
        }

        public void SsRef()
        {
            _fileHandle.IoControl(CtlCode(Control.SsRef), null, null);
        }

        public void SsUnref()
        {
            _fileHandle.IoControl(CtlCode(Control.SsUnref), null, null);
        }

        public NtStatus ZwQueryObject(
            ProcessHandle processHandle,
            IntPtr handle,
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
            *(int*)(inData + 4) = handle.ToInt32();
            *(int*)(inData + 8) = (int)objectInformationClass;

            _fileHandle.IoControl(CtlCode(Control.ZwQueryObject), inData, 12, outData);

            NtStatus status;

            fixed (byte* outDataPtr = outData)
            {
                status = *(NtStatus*)outDataPtr;
                returnLength = *(int*)(outDataPtr + 4);
                baseAddress = *(int*)(outDataPtr + 8);
            }

            if (buffer != IntPtr.Zero)
                Marshal.Copy(outData, 12, buffer, bufferLength);

            return status;
        }
    }

    public enum DriverInformationClass
    {
        DriverBasicInformation = 0,
        DriverNameInformation,
        DriverServiceKeyNameInformation
    }

    public class KphHandle : BaseObject
    {
        private IntPtr _handle;

        protected KphHandle(IntPtr handle)
        {
            _handle = handle;
        }

        protected override void DisposeObject(bool disposing)
        {
            KProcessHacker.Instance.ClientCloseHandle(_handle);
        }

        public IntPtr Handle
        {
            get { return _handle; }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DriverBasicInformation
    {
        public int Flags;
        public IntPtr DriverStart;
        public int DriverSize;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ProcessHandleInformation
    {
        public IntPtr Handle;
        public IntPtr Object;
        public int GrantedAccess;
        public HandleFlags HandleAttributes; // should be an int
        private byte Pad1;
        private short Pad2;

        private void Dummy()
        {
            Pad1 = 0;
            Pad2 = 0;
        }
    }
}
