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

    deviceHandle = CreateFile(
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
    ULONG ProcessId,
    ACCESS_MASK DesiredAccess
    )
{
    struct
    {
        ULONG ProcessId;
        ACCESS_MASK DesiredAccess;
    } args;

    args.ProcessId = ProcessId;
    args.DesiredAccess = DesiredAccess;

    return KphpDeviceIoControl(
        KphHandle,
        KPH_OPENPROCESS,
        &args,
        sizeof(args),
        NULL,
        0,
        NULL);
}
