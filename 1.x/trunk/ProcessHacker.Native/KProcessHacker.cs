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

using System.Runtime.InteropServices;
using System.Windows.Forms;
using ProcessHacker.Native;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;

namespace System
{
    /// <summary>
    /// Provides an interface to KProcessHacker.
    /// </summary>
    public sealed unsafe class KProcessHacker2 : IDisposable
    {
        public static KProcessHacker2 Instance;

        public bool KphIsConnected
        {
            get { return _fileHandle != null; }
        }

        public static int KphCtlCode(int x)
        {
            return Win32.CtlCode((DeviceType)KphDeviceType, 0x800 + x, DeviceControlMethod.Neither, DeviceControlAccess.Any);
        }

        public const int KphDeviceType = 0x9999;

        // General
        public static readonly int IoCtlGetFeatures = KphCtlCode(0);

        // Processes
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

        // Threads
        public static readonly int IoCtlOpenThread = KphCtlCode(100);
        public static readonly int IoCtlOpenThreadProcess = KphCtlCode(101);
        public static readonly int IoCtlTerminateThread = KphCtlCode(102);
        public static readonly int IoCtlTerminateThreadUnsafe = KphCtlCode(103);
        public static readonly int IoCtlGetContextThread = KphCtlCode(104);
        public static readonly int IoCtlSetContextThread = KphCtlCode(105);
        public static readonly int IoCtlCaptureStackBackTraceThread = KphCtlCode(106);
        public static readonly int IoCtlQueryInformationThread = KphCtlCode(107);
        public static readonly int IoCtlSetInformationThread = KphCtlCode(108);

        // Handles
        public static readonly int IoCtlEnumerateProcessHandles = KphCtlCode(150);
        public static readonly int IoCtlQueryInformationObject = KphCtlCode(151);
        public static readonly int IoCtlSetInformationObject = KphCtlCode(152);
        public static readonly int IoCtlDuplicateObject = KphCtlCode(153);

        // Misc.
        public static readonly int IoCtlOpenDriver = KphCtlCode(200);
        public static readonly int IoCtlQueryInformationDriver = KphCtlCode(201);

        [Flags]
        public enum KphFeatures
        {
            None = 0 // none so far
        }

        private readonly string _deviceName;
        private FileHandle _fileHandle;
        private readonly KphFeatures _features;

        /// <summary>
        /// Creates a connection to KProcessHacker.
        /// </summary>
        public KProcessHacker2()
            : this("KProcessHacker2")
        { }

        /// <summary>
        /// Creates a connection to KProcessHacker.
        /// </summary>
        /// <param name="deviceName">The name of the KProcessHacker service and device.</param>
        public KProcessHacker2(string deviceName)
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
                    LoadService();
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


        public void LoadService()
        {
            // Attempt to load the driver, then try again.
            ServiceHandle shandle;
            bool created = false;

            try
            {
                using (shandle = new ServiceHandle(_deviceName, ServiceAccess.Start))
                {
                    shandle.Start();
                }
            }
            catch
            {
                using (ServiceManagerHandle scm = new ServiceManagerHandle(ScManagerAccess.CreateService))
                {
                    shandle = scm.CreateService(
                        _deviceName,
                        _deviceName,
                        ServiceType.KernelDriver,
                        Application.StartupPath + "\\kprocesshacker.sys"
                        );
                    shandle.Start();
                    created = true;
                }
            }

            try
            {
                _fileHandle = new FileHandle(
                    @"\Device\" + _deviceName,
                    0,
                    FileAccess.GenericRead | FileAccess.GenericWrite
                    );
            }
            finally
            {
                if (created)
                {
                    // The SCM will delete the service when it is stopped.
                    shandle.Delete();
                }

                shandle.Dispose();
            }
        }

        public KphFeatures KphGetFeatures()
        {
            KphGetFeaturesInput input;
            int features;

            input.Features = &features;
            _fileHandle.IoControl(IoCtlGetFeatures, &input, sizeof(KphGetFeaturesInput), null, 0);

            return (KphFeatures)features;
        }

        public IntPtr KphOpenProcess(int pid, ProcessAccess desiredAccess)
        {
            KphOpenProcessInput input;
            IntPtr processHandle;
            ClientId clientId;

            clientId.UniqueProcess = (IntPtr)pid;
            clientId.UniqueThread = IntPtr.Zero;

            input.ProcessHandle = &processHandle;
            input.DesiredAccess = (int)desiredAccess;
            input.ClientId = &clientId;
            _fileHandle.IoControl(IoCtlOpenProcess, &input, sizeof(KphOpenProcessInput), null, 0);

            return processHandle;
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

        public void KphSuspendProcess(ProcessHandle processHandle)
        {
            KphSuspendProcessInput input;

            input.ProcessHandle = processHandle;
            _fileHandle.IoControl(IoCtlSuspendProcess, &input, sizeof(KphSuspendProcessInput), null, 0);
        }


        public void KphResumeProcess(ProcessHandle processHandle)
        {
            KphResumeProcessInput input;

            input.ProcessHandle = processHandle;
            _fileHandle.IoControl(IoCtlResumeProcess, &input, sizeof(KphResumeProcessInput), null, 0);
        }

        public void KphTerminateProcess(ProcessHandle processHandle, NtStatus exitStatus)
        {
            KphTerminateProcessInput input;

            input.ProcessHandle = processHandle;
            input.ExitStatus = exitStatus;
            _fileHandle.IoControl(IoCtlTerminateProcess, &input, sizeof(KphTerminateProcessInput), null, 0);
        }



        public void KphReadVirtualMemory(ProcessHandle processHandle, IntPtr baseAddress, IntPtr buffer, IntPtr bufferSize, void* numberOfBytesRead)
        {
            KphReadVirtualMemoryInput input;

            input.ProcessHandle = processHandle;
            input.BaseAddress = baseAddress;
            input.Buffer = buffer;
            input.BufferSize = bufferSize;
            input.NumberOfBytesRead = (IntPtr*)(&numberOfBytesRead);
            _fileHandle.IoControl(IoCtlReadVirtualMemory, &input, sizeof(KphReadVirtualMemoryInput), null, 0);
        }

        public void KphWriteVirtualMemory(ProcessHandle processHandle, IntPtr baseAddress, IntPtr buffer, IntPtr bufferSize, void* numberOfBytesWritten)
        {
            KphWriteVirtualMemoryInput input;

            input.ProcessHandle = processHandle;
            input.BaseAddress = baseAddress;
            input.Buffer = buffer;
            input.BufferSize = bufferSize;
            input.NumberOfBytesWritten = (IntPtr*)(&numberOfBytesWritten);
            _fileHandle.IoControl(IoCtlWriteVirtualMemory, &input, sizeof(KphWriteVirtualMemoryInput), null, 0);
        }


        public void KphReadVirtualMemoryUnsafe(ProcessHandle processHandle, IntPtr baseAddress, IntPtr buffer, IntPtr bufferSize, void* numberOfBytesRead)
        {
            KphReadVirtualMemoryUnsafeInput input;

            input.ProcessHandle = processHandle;
            input.BaseAddress = baseAddress;
            input.Buffer = buffer;
            input.BufferSize = bufferSize;

            input.NumberOfBytesRead = (IntPtr*)(&numberOfBytesRead);

            _fileHandle.IoControl(IoCtlReadVirtualMemoryUnsafe, &input, sizeof(KphReadVirtualMemoryUnsafeInput), null, 0);
        }


        public void KphQueryInformationProcess(ProcessHandle processHandle, KphProcessInformationClass processInformationClass, IntPtr processInformation, int processInformationLength, out int returnLength)
        {
            KphQueryInformationProcessInput input;

            input.ProcessHandle = processHandle;
            input.ProcessInformationClass = processInformationClass;
            input.ProcessInformation = processInformation;
            input.ProcessInformationLength = processInformationLength;

            returnLength = 0;
            input.ReturnLength = returnLength;

            _fileHandle.IoControl(IoCtlQueryInformationProcess, &input, sizeof(KphQueryInformationProcessInput), null, 0);
        }

        public void Dispose()
        {
            if (_fileHandle != null)
            {
                try
                {
                    _fileHandle.SetHandleFlags(Win32HandleFlags.ProtectFromClose, 0);
                    _fileHandle.Dispose();
                }
                catch (Exception)
                { }
            }
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

    //[StructLayout(LayoutKind.Sequential)]
    //public struct DriverBasicInformation
    //{
    //    public UnicodeString DriverName;
    //}

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


    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct KphOpenProcessJobInput
    {
        public IntPtr ProcessHandle;
        public int DesiredAccess;
        public IntPtr* JobHandle;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct KphGetFeaturesInput
    {
        public int* Features;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct KphOpenProcessInput
    {
        public IntPtr* ProcessHandle;
        public int DesiredAccess;
        public ClientId* ClientId;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct KphOpenProcessTokenInput
    {
        public IntPtr ProcessHandle;
        public int DesiredAccess;
        public IntPtr* TokenHandle;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct KphSuspendProcessInput
    {
        public IntPtr ProcessHandle;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct KphResumeProcessInput
    {
        public IntPtr ProcessHandle;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct KphTerminateProcessInput
    {
        public IntPtr ProcessHandle;
        public NtStatus ExitStatus;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct KphReadVirtualMemoryInput
    {
        public IntPtr ProcessHandle;
        public IntPtr BaseAddress;
        public IntPtr Buffer;
        public IntPtr BufferSize;
        public IntPtr* NumberOfBytesRead;
    }


    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct KphWriteVirtualMemoryInput
    {
        public IntPtr ProcessHandle;
        public IntPtr BaseAddress;
        public IntPtr Buffer;
        public IntPtr BufferSize;
        public IntPtr* NumberOfBytesWritten;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct KphReadVirtualMemoryUnsafeInput
    {
        public IntPtr ProcessHandle;
        public IntPtr BaseAddress;
        public IntPtr Buffer;
        public IntPtr BufferSize;
        public IntPtr* NumberOfBytesRead;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct KphQueryInformationProcessInput
    {
        public IntPtr ProcessHandle;
        public KphProcessInformationClass ProcessInformationClass;
        public IntPtr ProcessInformation;
        public int ProcessInformationLength;
        public int ReturnLength;
    }

}

//PHLIBAPI
//NTSTATUS
//NTAPI
//KphOpenProcessToken(
//    __in HANDLE ProcessHandle,
//    __in ACCESS_MASK DesiredAccess,
//    __out PHANDLE TokenHandle
//    );

//PHLIBAPI
//NTSTATUS
//NTAPI
//KphOpenProcessJob(
//    __in HANDLE ProcessHandle,
//    __in ACCESS_MASK DesiredAccess,
//    __out PHANDLE JobHandle
//    );

//PHLIBAPI
//NTSTATUS
//NTAPI
//KphSuspendProcess(
//    __in HANDLE ProcessHandle
//    );

//PHLIBAPI
//NTSTATUS
//NTAPI
//KphResumeProcess(
//    __in HANDLE ProcessHandle
//    );

//PHLIBAPI
//NTSTATUS
//NTAPI
//KphTerminateProcess(
//    __in HANDLE ProcessHandle,
//    __in NTSTATUS ExitStatus
//    );

//PHLIBAPI
//NTSTATUS
//NTAPI
//KphReadVirtualMemory(
//    __in HANDLE ProcessHandle,
//    __in PVOID BaseAddress,
//    __out_bcount(BufferSize) PVOID Buffer,
//    __in SIZE_T BufferSize,
//    __out_opt PSIZE_T NumberOfBytesRead
//    );

//PHLIBAPI
//NTSTATUS
//NTAPI
//KphWriteVirtualMemory(
//    __in HANDLE ProcessHandle,
//    __in_opt PVOID BaseAddress,
//    __in_bcount(BufferSize) PVOID Buffer,
//    __in SIZE_T BufferSize,
//    __out_opt PSIZE_T NumberOfBytesWritten
//    );

//PHLIBAPI
//NTSTATUS
//NTAPI
//KphReadVirtualMemoryUnsafe(
//    __in_opt HANDLE ProcessHandle,
//    __in PVOID BaseAddress,
//    __out_bcount(BufferSize) PVOID Buffer,
//    __in SIZE_T BufferSize,
//    __out_opt PSIZE_T NumberOfBytesRead
//    );

//PHLIBAPI
//NTSTATUS
//NTAPI
//KphQueryInformationProcess(
//    __in HANDLE ProcessHandle,
//    __in KPH_PROCESS_INFORMATION_CLASS ProcessInformationClass,
//    __out_bcount(ProcessInformationLength) PVOID ProcessInformation,
//    __in ULONG ProcessInformationLength,
//    __out_opt PULONG ReturnLength
//    );

//PHLIBAPI
//NTSTATUS
//NTAPI
//KphSetInformationProcess(
//    __in HANDLE ProcessHandle,
//    __in KPH_PROCESS_INFORMATION_CLASS ProcessInformationClass,
//    __in_bcount(ProcessInformationLength) PVOID ProcessInformation,
//    __in ULONG ProcessInformationLength
//    );

//PHLIBAPI
//NTSTATUS
//NTAPI
//KphOpenThread(
//    __out PHANDLE ThreadHandle,
//    __in ACCESS_MASK DesiredAccess,
//    __in PCLIENT_ID ClientId
//    );

//PHLIBAPI
//NTSTATUS
//NTAPI
//KphOpenThreadProcess(
//    __in HANDLE ThreadHandle,
//    __in ACCESS_MASK DesiredAccess,
//    __out PHANDLE ProcessHandle
//    );

//PHLIBAPI
//NTSTATUS
//NTAPI
//KphTerminateThread(
//    __in HANDLE ThreadHandle,
//    __in NTSTATUS ExitStatus
//    );

//PHLIBAPI
//NTSTATUS
//NTAPI
//KphTerminateThreadUnsafe(
//    __in HANDLE ThreadHandle,
//    __in NTSTATUS ExitStatus
//    );

//PHLIBAPI
//NTSTATUS
//NTAPI
//KphGetContextThread(
//    __in HANDLE ThreadHandle,
//    __inout PCONTEXT ThreadContext
//    );

//PHLIBAPI
//NTSTATUS
//NTAPI
//KphSetContextThread(
//    __in HANDLE ThreadHandle,
//    __in PCONTEXT ThreadContext
//    );

//PHLIBAPI
//NTSTATUS
//NTAPI
//KphCaptureStackBackTraceThread(
//    __in HANDLE ThreadHandle,
//    __in ULONG FramesToSkip,
//    __in ULONG FramesToCapture,
//    __out_ecount(FramesToCapture) PVOID *BackTrace,
//    __out_opt PULONG CapturedFrames,
//    __out_opt PULONG BackTraceHash
//    );

//PHLIBAPI
//NTSTATUS
//NTAPI
//KphQueryInformationThread(
//    __in HANDLE ThreadHandle,
//    __in KPH_THREAD_INFORMATION_CLASS ThreadInformationClass,
//    __out_bcount(ProcessInformationLength) PVOID ThreadInformation,
//    __in ULONG ThreadInformationLength,
//    __out_opt PULONG ReturnLength
//    );

//PHLIBAPI
//NTSTATUS
//NTAPI
//KphSetInformationThread(
//    __in HANDLE ThreadHandle,
//    __in KPH_THREAD_INFORMATION_CLASS ThreadInformationClass,
//    __in_bcount(ThreadInformationLength) PVOID ThreadInformation,
//    __in ULONG ThreadInformationLength
//    );

//PHLIBAPI
//NTSTATUS
//NTAPI
//KphEnumerateProcessHandles(
//    __in HANDLE ProcessHandle,
//    __out_bcount(BufferLength) PVOID Buffer,
//    __in_opt ULONG BufferLength,
//    __out_opt PULONG ReturnLength
//    );

//PHLIBAPI
//NTSTATUS
//NTAPI
//KphQueryInformationObject(
//    __in HANDLE ProcessHandle,
//    __in HANDLE Handle,
//    __in KPH_OBJECT_INFORMATION_CLASS ObjectInformationClass,
//    __out_bcount(ObjectInformationLength) PVOID ObjectInformation,
//    __in ULONG ObjectInformationLength,
//    __out_opt PULONG ReturnLength
//    );

//PHLIBAPI
//NTSTATUS
//NTAPI
//KphSetInformationObject(
//    __in HANDLE ProcessHandle,
//    __in HANDLE Handle,
//    __in KPH_OBJECT_INFORMATION_CLASS ObjectInformationClass,
//    __in_bcount(ObjectInformationLength) PVOID ObjectInformation,
//    __in ULONG ObjectInformationLength
//    );

//PHLIBAPI
//NTSTATUS
//NTAPI
//KphDuplicateObject(
//    __in HANDLE SourceProcessHandle,
//    __in HANDLE SourceHandle,
//    __in_opt HANDLE TargetProcessHandle,
//    __out_opt PHANDLE TargetHandle,
//    __in ACCESS_MASK DesiredAccess,
//    __in ULONG HandleAttributes,
//    __in ULONG Options
//    );

//PHLIBAPI
//NTSTATUS
//NTAPI
//KphOpenDriver(
//    __out PHANDLE DriverHandle,
//    __in POBJECT_ATTRIBUTES ObjectAttributes
//    );

//PHLIBAPI
//NTSTATUS
//NTAPI
//KphQueryInformationDriver(
//    __in HANDLE DriverHandle,
//    __in DRIVER_INFORMATION_CLASS DriverInformationClass,
//    __out_bcount(DriverInformationLength) PVOID DriverInformation,
//    __in ULONG DriverInformationLength,
//    __out_opt PULONG ReturnLength
//    );
