/*
 * Process Hacker Library - 
 *   KProcessHacker interface
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

#include "kph.h"

NTSTATUS PHAPI KphpDeviceIoControl(
    HANDLE KphHandle,
    ULONG KphControlCode,
    PVOID InBuffer,
    ULONG InBufferLength,
    PVOID OutBuffer,
    ULONG OutBufferLength,
    PULONG ReturnLength
    );

_NtDeviceIoControlFile NtDeviceIoControlFile = NULL;
_NtTerminateProcess NtTerminateProcess = NULL;
_NtTerminateThread NtTerminateThread = NULL;

NTSTATUS PHAPI KphInit()
{
    if (!(NtDeviceIoControlFile = (_NtDeviceIoControlFile)
        PhGetProcAddress(L"ntdll.dll", "NtDeviceIoControlFile")))
        return STATUS_PROCEDURE_NOT_FOUND;
    if (!(NtTerminateProcess = (_NtTerminateProcess)
        PhGetProcAddress(L"ntdll.dll", "NtTerminateProcess")))
        return STATUS_PROCEDURE_NOT_FOUND;
    if (!(NtTerminateThread = (_NtTerminateThread)
        PhGetProcAddress(L"ntdll.dll", "NtTerminateThread")))
        return STATUS_PROCEDURE_NOT_FOUND;

    return STATUS_SUCCESS;
}

NTSTATUS PHAPI KphConnect(
    __out PHANDLE KphHandle
    )
{
    HANDLE deviceHandle;

    deviceHandle = CreateFileW(
        KPH_DEVICE_NAME,
        FILE_GENERIC_READ | FILE_GENERIC_WRITE,
        FILE_SHARE_READ | FILE_SHARE_WRITE,
        NULL,
        OPEN_EXISTING,
        FILE_ATTRIBUTE_NORMAL,
        NULL
        );

    if (deviceHandle == INVALID_HANDLE_VALUE)
    {
        deviceHandle = NULL;
        return STATUS_UNSUCCESSFUL;
    }

    *KphHandle = deviceHandle;

    return STATUS_SUCCESS;
}

NTSTATUS PHAPI KphDisconnect(
    __in HANDLE KphHandle
    )
{
    if (CloseHandle(KphHandle))
        return STATUS_SUCCESS;
    else
        return STATUS_INVALID_HANDLE;
}

NTSTATUS PHAPI KphpDeviceIoControl(
    HANDLE KphHandle,
    ULONG KphControlCode,
    PVOID InBuffer,
    ULONG InBufferLength,
    PVOID OutBuffer,
    ULONG OutBufferLength,
    PULONG ReturnLength
    )
{
    NTSTATUS status;
    IO_STATUS_BLOCK ioStatusBlock;

    status = NtDeviceIoControlFile(
        KphHandle,
        NULL,
        NULL,
        NULL,
        &ioStatusBlock,
        KphControlCode,
        InBuffer,
        InBufferLength,
        OutBuffer,
        OutBufferLength
        );

    if (NT_SUCCESS(status) && ReturnLength)
        *ReturnLength = ioStatusBlock.Information;

    return status;
}

NTSTATUS PHAPI KphGetFeatures(
    __in HANDLE KphHandle,
    __out PULONG Features
    )
{
    NTSTATUS status;
    ULONG features;

    if (NT_SUCCESS(
        status = KphpDeviceIoControl(
            KphHandle,
            KPH_GETFEATURES,
            NULL,
            0,
            &features,
            sizeof(ULONG),
            NULL)
        ))
        *Features = features;

    return status;
}

NTSTATUS PHAPI KphRead(
    __in HANDLE KphHandle,
    __in PVOID Address,
    __out_bcount(BufferLength) PVOID Buffer,
    __in ULONG BufferLength
    )
{
    return KphpDeviceIoControl(
        KphHandle,
        KPH_READ,
        &Address,
        sizeof(PVOID),
        Buffer,
        BufferLength,
        NULL
        );
}

NTSTATUS PHAPI KphWrite(
    __in HANDLE KphHandle,
    __in PVOID Address,
    __in_bcount(Length) PVOID Buffer,
    __in ULONG Length
    )
{
    NTSTATUS status;
    PVOID data = PhAlloc(Length + sizeof(PVOID));

    *(PVOID *)data = Address;
    memcpy((PCHAR)data + sizeof(PVOID), Buffer, Length);

    status = KphpDeviceIoControl(
        KphHandle,
        KPH_WRITE,
        data,
        Length + sizeof(PVOID),
        NULL,
        0,
        NULL
        );
    PhFree(data);

    return status;
}

NTSTATUS PHAPI KphOpenProcess(
    __in HANDLE KphHandle,
    __out PHANDLE ProcessHandle,
    __in HANDLE ProcessId,
    __in ACCESS_MASK DesiredAccess
    )
{
    NTSTATUS status;

    struct
    {
        HANDLE ProcessId;
        ACCESS_MASK DesiredAccess;
    } args;
    struct
    {
        HANDLE ProcessHandle;
    } ret;

    args.ProcessId = ProcessId;
    args.DesiredAccess = DesiredAccess;

    status = KphpDeviceIoControl(
        KphHandle,
        KPH_OPENPROCESS,
        &args,
        sizeof(args),
        &ret,
        sizeof(ret),
        NULL
        );

    *ProcessHandle = ret.ProcessHandle;

    return status;
}

NTSTATUS PHAPI KphOpenThread(
    __in HANDLE KphHandle,
    __out PHANDLE ThreadHandle,
    __in HANDLE ThreadId,
    __in ACCESS_MASK DesiredAccess
    )
{
    NTSTATUS status;

    struct
    {
        HANDLE ThreadId;
        ACCESS_MASK DesiredAccess;
    } args;
    struct
    {
        HANDLE ThreadHandle;
    } ret;

    args.ThreadId = ThreadId;
    args.DesiredAccess = DesiredAccess;

    status = KphpDeviceIoControl(
        KphHandle,
        KPH_OPENTHREAD,
        &args,
        sizeof(args),
        &ret,
        sizeof(ret),
        NULL
        );

    *ThreadHandle = ret.ThreadHandle;

    return status;
}

NTSTATUS PHAPI KphOpenProcessToken(
    __in HANDLE KphHandle,
    __out PHANDLE TokenHandle,
    __in HANDLE ProcessHandle,
    __in ACCESS_MASK DesiredAccess
    )
{
    NTSTATUS status;

    struct
    {
        HANDLE ProcessHandle;
        ACCESS_MASK DesiredAccess;
    } args;
    struct
    {
        HANDLE TokenHandle;
    } ret;

    args.ProcessHandle = ProcessHandle;
    args.DesiredAccess = DesiredAccess;

    status = KphpDeviceIoControl(
        KphHandle,
        KPH_OPENPROCESSTOKEN,
        &args,
        sizeof(args),
        &ret,
        sizeof(ret),
        NULL
        );

    *TokenHandle = ret.TokenHandle;

    return status;
}

NTSTATUS PHAPI KphGetProcessProtected(
    __in HANDLE KphHandle,
    __in ULONG_PTR ProcessId,
    __out PBOOLEAN IsProtected
    )
{
    NTSTATUS status;

    struct
    {
        HANDLE ProcessId;
    } args;
    struct
    {
        BOOLEAN IsProtected;
    } ret;

    args.ProcessId = (HANDLE)ProcessId;

    status = KphpDeviceIoControl(
        KphHandle,
        KPH_GETPROCESSPROTECTED,
        &args,
        sizeof(args),
        &ret,
        sizeof(ret),
        NULL
        );

    *IsProtected = ret.IsProtected;

    return status;
}

NTSTATUS PHAPI KphSetProcessProtected(
    __in HANDLE KphHandle,
    __in ULONG_PTR ProcessId,
    __in BOOLEAN IsProtected
    )
{
    struct
    {
        HANDLE ProcessId;
        BOOLEAN IsProtected;
    } args;

    args.ProcessId = (HANDLE)ProcessId;
    args.IsProtected = IsProtected;

    return KphpDeviceIoControl(
        KphHandle,
        KPH_SETPROCESSPROTECTED,
        &args,
        sizeof(args),
        NULL,
        0,
        NULL
        );
}

NTSTATUS PHAPI KphTerminateProcess(
    __in HANDLE KphHandle,
    __in HANDLE ProcessHandle,
    __in NTSTATUS ExitStatus
    )
{
    NTSTATUS status = STATUS_SUCCESS;
    struct
    {
        HANDLE ProcessHandle;
        NTSTATUS ExitStatus;
    } args;

    args.ProcessHandle = ProcessHandle;
    args.ExitStatus = ExitStatus;

    status = KphpDeviceIoControl(
        KphHandle,
        KPH_TERMINATEPROCESS,
        &args,
        sizeof(args),
        NULL,
        0,
        NULL
        );

    /* Check if we were trying to terminate the current 
     * process and do it now. */
    if (status == STATUS_CANT_TERMINATE_SELF)
        status = NtTerminateProcess(GetCurrentProcess(), ExitStatus);

    return status;
}

NTSTATUS PHAPI KphSuspendProcess(
    __in HANDLE KphHandle,
    __in HANDLE ProcessHandle
    )
{
    struct
    {
        HANDLE ProcessHandle;
    } args;

    args.ProcessHandle = ProcessHandle;

    return KphpDeviceIoControl(
        KphHandle,
        KPH_SUSPENDPROCESS,
        &args,
        sizeof(args),
        NULL,
        0,
        NULL
        );
}

NTSTATUS PHAPI KphResumeProcess(
    __in HANDLE KphHandle,
    __in HANDLE ProcessHandle
    )
{
    struct
    {
        HANDLE ProcessHandle;
    } args;

    args.ProcessHandle = ProcessHandle;

    return KphpDeviceIoControl(
        KphHandle,
        KPH_RESUMEPROCESS,
        &args,
        sizeof(args),
        NULL,
        0,
        NULL
        );
}

NTSTATUS PHAPI KphReadVirtualMemory(
    __in HANDLE KphHandle,
    __in HANDLE ProcessHandle,
    __in PVOID BaseAddress,
    __out_bcount(BufferLength) PVOID Buffer,
    __in ULONG BufferLength,
    __out_opt PULONG ReturnLength
    )
{
    struct
    {
        HANDLE ProcessHandle;
        PVOID BaseAddress;
        PVOID Buffer;
        ULONG BufferLength;
        PULONG ReturnLength;
    } args;

    args.ProcessHandle = ProcessHandle;
    args.BaseAddress = BaseAddress;
    args.Buffer = Buffer;
    args.BufferLength = BufferLength;
    args.ReturnLength = ReturnLength;

    return KphpDeviceIoControl(
        KphHandle,
        KPH_READVIRTUALMEMORY,
        &args,
        sizeof(args),
        NULL,
        0,
        NULL
        );
}

NTSTATUS PHAPI KphWriteVirtualMemory(
    __in HANDLE KphHandle,
    __in HANDLE ProcessHandle,
    __in PVOID BaseAddress,
    __in_bcount(BufferLength) PVOID Buffer,
    __in ULONG BufferLength,
    __out_opt PULONG ReturnLength
    )
{
    struct
    {
        HANDLE ProcessHandle;
        PVOID BaseAddress;
        PVOID Buffer;
        ULONG BufferLength;
        PULONG ReturnLength;
    } args;

    args.ProcessHandle = ProcessHandle;
    args.BaseAddress = BaseAddress;
    args.Buffer = Buffer;
    args.BufferLength = BufferLength;
    args.ReturnLength = ReturnLength;

    return KphpDeviceIoControl(
        KphHandle,
        KPH_WRITEVIRTUALMEMORY,
        &args,
        sizeof(args),
        NULL,
        0,
        NULL
        );
}

NTSTATUS PHAPI KphOpenProcessJob(
    __in HANDLE KphHandle,
    __out PHANDLE JobHandle,
    __in HANDLE ProcessHandle,
    __in ACCESS_MASK DesiredAccess
    )
{
    NTSTATUS status;

    struct
    {
        HANDLE ProcessHandle;
        ACCESS_MASK DesiredAccess;
    } args;
    struct
    {
        HANDLE JobHandle;
    } ret;

    args.ProcessHandle = ProcessHandle;
    args.DesiredAccess = DesiredAccess;

    status = KphpDeviceIoControl(
        KphHandle,
        KPH_OPENPROCESSJOB,
        &args,
        sizeof(args),
        &ret,
        sizeof(ret),
        NULL
        );

    *JobHandle = ret.JobHandle;

    return status;
}

NTSTATUS PHAPI KphGetContextThread(
    __in HANDLE KphHandle,
    __in HANDLE ThreadHandle,
    __inout PCONTEXT ThreadContext
    )
{
    struct
    {
        HANDLE ThreadHandle;
        PCONTEXT ThreadContext;
    } args;

    args.ThreadHandle = ThreadHandle;
    args.ThreadContext = ThreadContext;

    return KphpDeviceIoControl(
        KphHandle,
        KPH_GETCONTEXTTHREAD,
        &args,
        sizeof(args),
        NULL,
        0,
        NULL
        );
}

NTSTATUS PHAPI KphSetContextThread(
    __in HANDLE KphHandle,
    __in HANDLE ThreadHandle,
    __in PCONTEXT ThreadContext
    )
{
    struct
    {
        HANDLE ThreadHandle;
        PCONTEXT ThreadContext;
    } args;

    args.ThreadHandle = ThreadHandle;
    args.ThreadContext = ThreadContext;

    return KphpDeviceIoControl(
        KphHandle,
        KPH_SETCONTEXTTHREAD,
        &args,
        sizeof(args),
        NULL,
        0,
        NULL
        );
}

NTSTATUS PHAPI KphTerminateThread(
    __in HANDLE KphHandle,
    __in HANDLE ThreadHandle,
    __in NTSTATUS ExitStatus
    )
{
    NTSTATUS status = STATUS_SUCCESS;
    struct
    {
        HANDLE ThreadHandle;
        NTSTATUS ExitStatus;
    } args;

    args.ThreadHandle = ThreadHandle;
    args.ExitStatus = ExitStatus;

    status = KphpDeviceIoControl(
        KphHandle,
        KPH_TERMINATETHREAD,
        &args,
        sizeof(args),
        NULL,
        0,
        NULL
        );

    if (status == STATUS_CANT_TERMINATE_SELF)
        status = NtTerminateThread(GetCurrentThread(), ExitStatus);

    return status;
}

NTSTATUS PHAPI KphSetHandleGrantedAccess(
    __in HANDLE KphHandle,
    __in HANDLE Handle,
    __in ACCESS_MASK GrantedAccess
    )
{
    struct
    {
        HANDLE Handle;
        ACCESS_MASK GrantedAccess;
    } args;

    args.Handle = Handle;
    args.GrantedAccess = GrantedAccess;

    return KphpDeviceIoControl(
        KphHandle,
        KPH_SETHANDLEGRANTEDACCESS,
        &args,
        sizeof(args),
        NULL,
        0,
        NULL
        );
}

NTSTATUS PHAPI KphProtectAdd(
    __in HANDLE KphHandle,
    __in HANDLE ProcessHandle,
    __in BOOLEAN AllowKernelMode,
    __in ACCESS_MASK ProcessAllowMask,
    __in ACCESS_MASK ThreadAllowMask
    )
{
    struct
    {
        HANDLE ProcessHandle;
        LOGICAL AllowKernelMode;
        ACCESS_MASK ProcessAllowMask;
        ACCESS_MASK ThreadAllowMask;
    } args;

    args.ProcessHandle = ProcessHandle;
    args.AllowKernelMode = AllowKernelMode;
    args.ProcessAllowMask = ProcessAllowMask;
    args.ThreadAllowMask = ThreadAllowMask;

    return KphpDeviceIoControl(
        KphHandle,
        KPH_PROTECTADD,
        &args,
        sizeof(args),
        NULL,
        0,
        NULL
        );
}

NTSTATUS PHAPI KphProtectRemove(
    __in HANDLE KphHandle,
    __in HANDLE ProcessHandle
    )
{
    struct
    {
        HANDLE ProcessHandle;
    } args;

    args.ProcessHandle = ProcessHandle;

    return KphpDeviceIoControl(
        KphHandle,
        KPH_PROTECTREMOVE,
        &args,
        sizeof(args),
        NULL,
        0,
        NULL
        );
}

NTSTATUS PHAPI KphProtectQuery(
    __in HANDLE KphHandle,
    __in HANDLE ProcessHandle,
    __out PBOOLEAN AllowKernelMode,
    __out PACCESS_MASK ProcessAllowMask,
    __out PACCESS_MASK ThreadAllowMask
    )
{
    NTSTATUS status = STATUS_SUCCESS;
    struct
    {
        HANDLE ProcessHandle;
        PLOGICAL AllowKernelMode;
        PACCESS_MASK ProcessAllowMask;
        PACCESS_MASK ThreadAllowMask;
    } args;
    LOGICAL allowKernelMode;

    args.ProcessHandle = ProcessHandle;
    args.AllowKernelMode = &allowKernelMode;
    args.ProcessAllowMask = ProcessAllowMask;
    args.ThreadAllowMask = ThreadAllowMask;

    status = KphpDeviceIoControl(
        KphHandle,
        KPH_PROTECTQUERY,
        &args,
        sizeof(args),
        NULL,
        0,
        NULL
        );

    *AllowKernelMode = (BOOLEAN)allowKernelMode;

    return status;
}

NTSTATUS PHAPI KphUnsafeReadVirtualMemory(
    __in HANDLE KphHandle,
    __in HANDLE ProcessHandle,
    __in PVOID BaseAddress,
    __in_bcount(BufferLength) PVOID Buffer,
    __in ULONG BufferLength,
    __out_opt PULONG ReturnLength
    )
{
    struct
    {
        HANDLE ProcessHandle;
        PVOID BaseAddress;
        PVOID Buffer;
        ULONG BufferLength;
        PULONG ReturnLength;
    } args;

    args.ProcessHandle = ProcessHandle;
    args.BaseAddress = BaseAddress;
    args.Buffer = Buffer;
    args.BufferLength = BufferLength;
    args.ReturnLength = ReturnLength;

    return KphpDeviceIoControl(
        KphHandle,
        KPH_UNSAFEREADVIRTUALMEMORY,
        &args,
        sizeof(args),
        NULL,
        0,
        NULL
        );
}

NTSTATUS PHAPI KphSetExecuteOptions(
    __in HANDLE KphHandle,
    __in HANDLE ProcessHandle,
    __in ULONG ExecuteOptions
    )
{
    struct
    {
        HANDLE ProcessHandle;
        ULONG ExecuteOptions;
    } args;

    args.ProcessHandle = ProcessHandle;
    args.ExecuteOptions = ExecuteOptions;

    return KphpDeviceIoControl(
        KphHandle,
        KPH_SETEXECUTEOPTIONS,
        &args,
        sizeof(args),
        NULL,
        0,
        NULL
        );
}

NTSTATUS PHAPI KphQueryProcessHandles(
    __in HANDLE KphHandle,
    __in HANDLE ProcessHandle,
    __out_bcount_opt(BufferLength) PVOID Buffer,
    __in_opt ULONG BufferLength,
    __out_opt PULONG ReturnLength
    )
{
    struct
    {
        HANDLE ProcessHandle;
        PVOID Buffer;
        ULONG BufferLength;
        PULONG ReturnLength;
    } args;

    args.ProcessHandle = ProcessHandle;
    args.Buffer = Buffer;
    args.BufferLength = BufferLength;
    args.ReturnLength = ReturnLength;

    return KphpDeviceIoControl(
        KphHandle,
        KPH_QUERYPROCESSHANDLES,
        &args,
        sizeof(args),
        NULL,
        0,
        NULL
        );
}

NTSTATUS PHAPI KphOpenThreadProcess(
    __in HANDLE KphHandle,
    __out PHANDLE ProcessHandle,
    __in HANDLE ThreadHandle,
    __in ACCESS_MASK DesiredAccess
    )
{
    NTSTATUS status;

    struct
    {
        HANDLE ThreadHandle;
        ACCESS_MASK DesiredAccess;
    } args;
    struct
    {
        HANDLE ProcessHandle;
    } ret;

    args.ThreadHandle = ThreadHandle;
    args.DesiredAccess = DesiredAccess;

    status = KphpDeviceIoControl(
        KphHandle,
        KPH_OPENTHREADPROCESS,
        &args,
        sizeof(args),
        &ret,
        sizeof(ret),
        NULL
        );

    *ProcessHandle = ret.ProcessHandle;

    return status;
}
