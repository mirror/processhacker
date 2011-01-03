/*
 * Process Hacker - 
 *   KProcessHacker interfacing code
 * 
 * Copyright (C) 2009-2011 wj32
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

        public static int KphCtlCode(int x)
        {
            return Win32.CtlCode(KphDeviceType, 0x800 + x, DeviceControlMethod.Neither, DeviceControlAccess.Any);
        }

        public const int KphDeviceType = 0x9999;
        public static readonly int IoCtlGetFeatures = KphCtlCode(0);
        public static readonly int IoCtlOpenProcess = KphCtlCode(50);
        public static readonly int IoCtlOpenProcessToken = KphCtlCode(51);
        public static readonly int IoCtlOpenProcessJob = KphCtlCode(52);
        public static readonly int IoCtlSuspendProcess = KphCtlCode(53);
        public static readonly int IoCtlResumeProcess = KphCtlCode(54);
        public static readonly int IoCtlTerminateProcess = KphCtlCode(55);
        public static readonly int IoCtlReadVirtualMemory = KphCtlCode(56);
        public static readonly int IoCtlWriteVirtualMemory = KphCtlCode(57);
        public static readonly int IoCtlReadVirtualMemoryUnsafe = KphCtlCode(58);
        public static readonly int IoCtlQueryInformationProcess = KphCtlCode(59);
        public static readonly int IoCtlSetInformationProcess = KphCtlCode(60);
        public static readonly int IoCtlOpenThread = KphCtlCode(100);
        public static readonly int IoCtlOpenThreadProcess = KphCtlCode(101);
        public static readonly int IoCtlTerminateThread = KphCtlCode(102);
        public static readonly int IoCtlTerminateThreadUnsafe = KphCtlCode(103);
        public static readonly int IoCtlGetContextThread = KphCtlCode(104);
        public static readonly int IoCtlSetContextThread = KphCtlCode(105);
        public static readonly int IoCtlCaptureStackBackTraceThread = KphCtlCode(106);
        public static readonly int IoCtlQueryInformationThread = KphCtlCode(107);
        public static readonly int IoCtlSetInformationThread = KphCtlCode(108);
        public static readonly int IoCtlEnumerateProcessHandles = KphCtlCode(150);
        public static readonly int IoCtlQueryInformationObject = KphCtlCode(151);
        public static readonly int IoCtlSetInformationObject = KphCtlCode(152);
        public static readonly int IoCtlDuplicateObject = KphCtlCode(153);
        public static readonly int IoCtlOpenDriver = KphCtlCode(200);
        public static readonly int IoCtlQueryInformationDriver = KphCtlCode(201);

        [Flags]
        public enum KphFeatures : int
        {
            None = 0 // none so far
        }

        private string _deviceName;
        private FileHandle _fileHandle;
        private KphFeatures _features;

        /// <summary>
        /// Creates a connection to KProcessHacker.
        /// </summary>
        public KProcessHacker()
            : this("KProcessHacker2")
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
                        using (shandle = new ServiceHandle(deviceName, ServiceAccess.Start))
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
            _features = this.KphGetFeatures();
        }

        public string DeviceName
        {
            get { return _deviceName; }
        }

        public KphFeatures Features
        {
            get { return _features; }
        }

        /// <summary>
        /// Closes the connection to KProcessHacker.
        /// </summary>
        public void Close()
        {
            _fileHandle.SetHandleFlags(Win32HandleFlags.ProtectFromClose, 0);
            _fileHandle.Dispose();
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KphGetFeaturesInput
        {
            public int* Features;
        }

        public KphFeatures KphGetFeatures()
        {
            KphGetFeaturesInput input;
            int features;

            input.Features = &features;
            _fileHandle.IoControl(IoCtlGetFeatures, &input, sizeof(KphGetFeaturesInput), null, 0);

            return (KphFeatures)features;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KphOpenProcessInput
        {
            public IntPtr* ProcessHandle;
            public int DesiredAccess;
            public ClientId* ClientId;
        }

        public IntPtr KphOpenProcess(int pid, ProcessAccess desiredAccess)
        {
            KphOpenProcessInput input;
            IntPtr processHandle;
            ClientId clientId;

            clientId.ProcessId = pid;
            clientId.ThreadId = 0;

            input.ProcessHandle = &processHandle;
            input.DesiredAccess = (int)desiredAccess;
            input.ClientId = &clientId;
            _fileHandle.IoControl(IoCtlOpenProcess, &input, sizeof(KphOpenProcessInput), null, 0);

            return processHandle;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KphOpenProcessTokenInput
        {
            public IntPtr ProcessHandle;
            public int DesiredAccess;
            public IntPtr* TokenHandle;
        }

        public IntPtr KphOpenProcessToken(ProcessHandle processHandle, TokenAccess desiredAccess)
        {
            KphOpenProcessTokenInput input;
            IntPtr tokenHandle;

            input.ProcessHandle = processHandle;
            input.DesiredAccess = (int)desiredAccess;
            input.TokenHandle = &tokenHandle;
            _fileHandle.IoControl(IoCtlOpenProcessToken, &input, sizeof(KphOpenProcessTokenInput), null, 0);

            return tokenHandle;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KphOpenProcessJobInput
        {
            public IntPtr ProcessHandle;
            public int DesiredAccess;
            public IntPtr* JobHandle;
        }

        public IntPtr KphOpenProcessJob(ProcessHandle processHandle, TokenAccess desiredAccess)
        {
            KphOpenProcessJobInput input;
            IntPtr jobHandle;

            input.ProcessHandle = processHandle;
            input.DesiredAccess = (int)desiredAccess;
            input.JobHandle = &jobHandle;
            _fileHandle.IoControl(IoCtlOpenProcessJob, &input, sizeof(KphOpenProcessJobInput), null, 0);

            return jobHandle;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KphSuspendProcessInput
        {
            public IntPtr ProcessHandle;
        }

        public void KphSuspendProcess(ProcessHandle processHandle)
        {
            KphSuspendProcessInput input;

            input.ProcessHandle = processHandle;
            _fileHandle.IoControl(IoCtlSuspendProcess, &input, sizeof(KphSuspendProcessInput), null, 0);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KphResumeProcessInput
        {
            public IntPtr ProcessHandle;
        }

        public void KphResumeProcess(ProcessHandle processHandle)
        {
            KphResumeProcessInput input;

            input.ProcessHandle = processHandle;
            _fileHandle.IoControl(IoCtlResumeProcess, &input, sizeof(KphResumeProcessInput), null, 0);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KphTerminateProcessInput
        {
            public IntPtr ProcessHandle;
            public NtStatus ExitStatus;
        }

        public void KphTerminateProcess(ProcessHandle processHandle, NtStatus exitStatus)
        {
            KphTerminateProcessInput input;

            input.ProcessHandle = processHandle;
            input.ExitStatus = exitStatus;
            _fileHandle.IoControl(IoCtlTerminateProcess, &input, sizeof(KphTerminateProcessInput), null, 0);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KphReadVirtualMemoryInput
        {
            public IntPtr ProcessHandle;
            public IntPtr BaseAddress;
            public IntPtr Buffer;
            public IntPtr BufferSize;
            public IntPtr* NumberOfBytesRead;
        }

        public void KphReadVirtualMemory(ProcessHandle processHandle, IntPtr baseAddress, IntPtr buffer, IntPtr bufferSize, out IntPtr numberOfBytesRead)
        {
            KphReadVirtualMemoryInput input;

            input.ProcessHandle = processHandle;
            input.BaseAddress = baseAddress;
            input.Buffer = buffer;
            input.BufferSize = bufferSize;
            input.NumberOfBytesRead = &numberOfBytesRead;
            _fileHandle.IoControl(IoCtlReadVirtualMemory, &input, sizeof(KphReadVirtualMemoryInput), null, 0);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KphWriteVirtualMemoryInput
        {
            public IntPtr ProcessHandle;
            public IntPtr BaseAddress;
            public IntPtr Buffer;
            public IntPtr BufferSize;
            public IntPtr* NumberOfBytesWritten;
        }

        public void KphWriteVirtualMemory(ProcessHandle processHandle, IntPtr baseAddress, IntPtr buffer, IntPtr bufferSize, out IntPtr numberOfBytesWritten)
        {
            KphWriteVirtualMemoryInput input;

            input.ProcessHandle = processHandle;
            input.BaseAddress = baseAddress;
            input.Buffer = buffer;
            input.BufferSize = bufferSize;
            input.NumberOfBytesWritten = &numberOfBytesWritten;
            _fileHandle.IoControl(IoCtlWriteVirtualMemory, &input, sizeof(KphWriteVirtualMemoryInput), null, 0);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KphReadVirtualMemoryUnsafeInput
        {
            public IntPtr ProcessHandle;
            public IntPtr BaseAddress;
            public IntPtr Buffer;
            public IntPtr BufferSize;
            public IntPtr* NumberOfBytesRead;
        }

        public void KphReadVirtualMemoryUnsafe(ProcessHandle processHandle, IntPtr baseAddress, IntPtr buffer, IntPtr bufferSize, out IntPtr numberOfBytesRead)
        {
            KphReadVirtualMemoryUnsafeInput input;

            input.ProcessHandle = processHandle;
            input.BaseAddress = baseAddress;
            input.Buffer = buffer;
            input.BufferSize = bufferSize;
            input.NumberOfBytesRead = &numberOfBytesRead;
            _fileHandle.IoControl(IoCtlReadVirtualMemoryUnsafe, &input, sizeof(KphReadVirtualMemoryUnsafeInput), null, 0);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KphQueryInformationProcessInput
        {
            public IntPtr ProcessHandle;
            public KphProcessInformationClass ProcessInformationClass;
            public IntPtr ProcessInformation;
            public int ProcessInformationLength;
            public int* ReturnLength;
        }

        public void KphQueryInformationProcess(ProcessHandle processHandle, KphProcessInformationClass processInformationClass, IntPtr processInformation, int processInformationLength, out int returnLength)
        {
            KphQueryInformationProcessInput input;

            input.ProcessHandle = processHandle;
            input.ProcessInformationClass = processInformationClass;
            input.ProcessInformation = processInformation;
            input.ProcessInformationLength = processInformationLength;
            input.ReturnLength = returnLength;
            _fileHandle.IoControl(IoCtlQueryInformationProcess, &input, sizeof(KphQueryInformationProcessInput), null, 0);
        }
    }

    public enum KphSecurityLevel
    {
        KphSecurityNone = 0, // all clients are allowed
        KphSecurityPrivilegeCheck = 1, // require SeDebugPrivilege
        KphMaxSecurityLevel
    }

    public enum KphProcessInformationClass
    {
        KphProcessProtectionInformation = 1,
        KphProcessExecuteFlags = 2,
        KphProcessIoPriority = 3,
        MaxKphProcessInfoClass
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct KphProcessProtectionInformation
    {
        public byte IsProtectedProcess;
    }

    public enum KphThreadInformationClass
    {
        KphThreadWin32Thread = 1,
        KphThreadImpersonationToken = 2,
        KphThreadIoPriority = 3,
        MaxKphThreadInfoClass
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct KphProcessHandle
    {
        public IntPtr Handle;
        public IntPtr Object;
        public int GrantedAccess;
        public short ObjectTypeIndex;
        public short Reserved1;
        public int HandleAttributes;
        private int Reserved2;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct KphProcessHandleInformation
    {
        public static readonly int HandlesOffset =
            Marshal.OffsetOf(typeof(KphProcessHandleInformation), "Handles").ToInt32();

        public int HandleCount;
        public KphProcessHandle Handles;
    }

    public enum KphObjectInformationClass
    {
        KphObjectBasicInformation,
        KphObjectNameInformation,
        KphObjectTypeInformation,
        KphObjectHandleFlagInformation,
        KphObjectProcessBasicInformation,
        KphObjectThreadBasicInformation,
        KphObjectEtwRegBasicInformation,
        MaxKphObjectInfoClass
    }

    public enum DriverInformationClass
    {
        DriverBasicInformation,
        DriverNameInformation,
        DriverServiceKeyNameInformation,
        MaxDriverInfoClass
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DriverBasicInformation
    {
        public int Flags;
        public IntPtr DriverStart;
        public int DriverSize;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DriverBasicInformation
    {
        public UnicodeString DriverName;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DriverServiceKeyNameInformation
    {
        public UnicodeString ServiceKeyName;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct EtwRegBasicInformation
    {
        public Guid Guid;
        public IntPtr SessionId;
    }
}
