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

NTSTATUS KphpDeviceIoControl(
    HANDLE KphHandle,
    ULONG KphControlCode,
    PVOID InBuffer,
    ULONG InBufferLength,
    PVOID OutBuffer,
    ULONG OutBufferLength,
    PULONG ReturnLength
    );

_NtDeviceIoControlFile NtDeviceIoControlFile = NULL;

NTSTATUS KphInit()
{
    NtDeviceIoControlFile = (_NtDeviceIoControlFile)
        PhGetProcAddress(L"ntdll.dll", "NtDeviceIoControlFile");
    if (!NtDeviceIoControlFile)
        return STATUS_PROCEDURE_NOT_FOUND;

    return STATUS_SUCCESS;
}

NPHAPI NTSTATUS KphConnect(PHANDLE KphHandle)
{
    HANDLE deviceHandle;

    deviceHandle = CreateFileW(
        KPH_DEVICE_NAME,
        FILE_GENERIC_READ | FILE_GENERIC_WRITE,
        FILE_SHARE_READ | FILE_SHARE_WRITE,
        NULL,
        OPEN_ALWAYS,
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

NPHAPI NTSTATUS KphDisconnect(HANDLE KphHandle)
{
    if (CloseHandle(KphHandle))
        return STATUS_SUCCESS;
    else
        return STATUS_INVALID_HANDLE;
}

NTSTATUS KphpDeviceIoControl(
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

NPHAPI NTSTATUS KphGetFeatures(
    HANDLE KphHandle,
    PULONG Features
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

NPHAPI NTSTATUS KphRead(
    HANDLE KphHandle,
    PVOID Address,
    PVOID Buffer,
    ULONG BufferLength
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

NPHAPI NTSTATUS KphWrite(
    HANDLE KphHandle,
    PVOID Address,
    PVOID Buffer,
    ULONG Length
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

NPHAPI NTSTATUS KphOpenProcess(
    HANDLE KphHandle,
    PHANDLE ProcessHandle,
    ULONG_PTR ProcessId,
    ACCESS_MASK DesiredAccess
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

    args.ProcessId = (HANDLE)ProcessId;
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

NPHAPI NTSTATUS KphOpenThread(
    HANDLE KphHandle,
    PHANDLE ThreadHandle,
    ULONG_PTR ThreadId,
    ACCESS_MASK DesiredAccess
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

    args.ThreadId = (HANDLE)ThreadId;
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

NPHAPI NTSTATUS KphOpenProcessToken(
    HANDLE KphHandle,
    PHANDLE TokenHandle,
    HANDLE ProcessHandle,
    ACCESS_MASK DesiredAccess
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

NTSTATUS KphGetProcessProtected(
    HANDLE KphHandle,
    ULONG_PTR ProcessId,
    PBOOLEAN IsProtected
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

NTSTATUS KphSetProcessProtected(
    HANDLE KphHandle,
    ULONG_PTR ProcessId,
    BOOLEAN IsProtected
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

NTSTATUS KphTerminateProcess(
    HANDLE KphHandle,
    HANDLE ProcessHandle,
    NTSTATUS ExitStatus
    )
{
    struct
    {
        HANDLE ProcessHandle;
        NTSTATUS ExitStatus;
    } args;

    args.ProcessHandle = ProcessHandle;
    args.ExitStatus = ExitStatus;

    return KphpDeviceIoControl(
        KphHandle,
        KPH_TERMINATEPROCESS,
        &args,
        sizeof(args),
        NULL,
        0,
        NULL
        );
}

NTSTATUS KphSuspendProcess(
    HANDLE KphHandle,
    HANDLE ProcessHandle
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

NTSTATUS KphResumeProcess(
    HANDLE KphHandle,
    HANDLE ProcessHandle
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

NTSTATUS KphReadVirtualMemory(
    HANDLE KphHandle,
    HANDLE ProcessHandle,
    PVOID BaseAddress,
    PVOID Buffer,
    ULONG BufferLength,
    PULONG ReturnLength
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

NTSTATUS KphWriteVirtualMemory(
    HANDLE KphHandle,
    HANDLE ProcessHandle,
    PVOID BaseAddress,
    PVOID Buffer,
    ULONG BufferLength,
    PULONG ReturnLength
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

NPHAPI NTSTATUS KphOpenProcessJob(
    HANDLE KphHandle,
    PHANDLE JobHandle,
    HANDLE ProcessHandle,
    ACCESS_MASK DesiredAccess
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

NTSTATUS KphGetContextThread(
    HANDLE KphHandle,
    HANDLE ThreadHandle,
    PCONTEXT ThreadContext
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

NTSTATUS KphSetContextThread(
    HANDLE KphHandle,
    HANDLE ThreadHandle,
    PCONTEXT ThreadContext
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

NTSTATUS KphTerminateThread(
    HANDLE KphHandle,
    HANDLE ThreadHandle,
    NTSTATUS ExitStatus
    )
{
    struct
    {
        HANDLE ThreadHandle;
        NTSTATUS ExitStatus;
    } args;

    args.ThreadHandle = ThreadHandle;
    args.ExitStatus = ExitStatus;

    return KphpDeviceIoControl(
        KphHandle,
        KPH_TERMINATETHREAD,
        &args,
        sizeof(args),
        NULL,
        0,
        NULL
        );
}

NTSTATUS KphSetHandleGrantedAccess(
    HANDLE KphHandle,
    HANDLE Handle,
    ACCESS_MASK GrantedAccess
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
