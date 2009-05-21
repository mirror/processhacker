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

#ifndef _KPH_H
#define _KPH_H

#include "nph.h"

#define KPH_DEVICE_TYPE (0x9999)
#define KPH_DEVICE_NAME (L"\\\\.\\KProcessHacker")

#define KPHF_MMCOPYVIRTUALMEMORY 0x1
#define KPHF_EXPGETPROCESSINFORMATION 0x2
#define KPHF_PSTERMINATEPROCESS 0x4
#define KPHF_PSPTERMINATETHREADBPYPOINTER 0x8

#define METHOD_BUFFERED 0
#define METHOD_IN_DIRECT 1
#define METHOD_OUT_DIRECT 2
#define METHOD_NEITHER 3

#ifndef FILE_ANY_ACCESS
#define FILE_ANY_ACCESS 0
#define FILE_SPECIAL_ACCESS (FILE_ANY_ACCESS)
#define FILE_READ_ACCESS (0x0001)
#define FILE_WRITE_ACCESS (0x0002)
#endif

#define CTL_CODE(DeviceType, Function, Method, Access) ( \
    ((DeviceType) << 16) | ((Access) << 14) | ((Function) << 2) | (Method) \
)
#define KPH_CTL_CODE(x) CTL_CODE(KPH_DEVICE_TYPE, 0x800 + x, METHOD_BUFFERED, FILE_ANY_ACCESS)
#define KPH_READ KPH_CTL_CODE(0)
#define KPH_WRITE KPH_CTL_CODE(1)
#define KPH_GETFILEOBJECTNAME KPH_CTL_CODE(2)
#define KPH_OPENPROCESS KPH_CTL_CODE(3)
#define KPH_OPENTHREAD KPH_CTL_CODE(4)
#define KPH_OPENPROCESSTOKEN KPH_CTL_CODE(5)
#define KPH_GETPROCESSPROTECTED KPH_CTL_CODE(6)
#define KPH_SETPROCESSPROTECTED KPH_CTL_CODE(7)
#define KPH_TERMINATEPROCESS KPH_CTL_CODE(8)
#define KPH_SUSPENDPROCESS KPH_CTL_CODE(9)
#define KPH_RESUMEPROCESS KPH_CTL_CODE(10)
#define KPH_READVIRTUALMEMORY KPH_CTL_CODE(11)
#define KPH_WRITEVIRTUALMEMORY KPH_CTL_CODE(12)
#define KPH_SETPROCESSTOKEN KPH_CTL_CODE(13)
#define KPH_GETTHREADSTARTADDRESS KPH_CTL_CODE(14)
#define KPH_SETHANDLEATTRIBUTES KPH_CTL_CODE(15)
#define KPH_GETHANDLEOBJECTNAME KPH_CTL_CODE(16)
#define KPH_OPENPROCESSJOB KPH_CTL_CODE(17)
#define KPH_GETCONTEXTTHREAD KPH_CTL_CODE(18)
#define KPH_SETCONTEXTTHREAD KPH_CTL_CODE(19)
#define KPH_GETTHREADWIN32THREAD KPH_CTL_CODE(20)
#define KPH_DUPLICATEOBJECT KPH_CTL_CODE(21)
#define KPH_ZWQUERYOBJECT KPH_CTL_CODE(22)
#define KPH_GETPROCESSID KPH_CTL_CODE(23)
#define KPH_GETTHREADID KPH_CTL_CODE(24)
#define KPH_TERMINATETHREAD KPH_CTL_CODE(25)
#define KPH_GETFEATURES KPH_CTL_CODE(26)
#define KPH_SETHANDLEGRANTEDACCESS KPH_CTL_CODE(27)
#define KPH_ASSIGNIMPERSONATIONTOKEN KPH_CTL_CODE(28)
#define KPH_PROTECTADD KPH_CTL_CODE(29)
#define KPH_PROTECTREMOVE KPH_CTL_CODE(30)
#define KPH_PROTECTQUERY KPH_CTL_CODE(31)

typedef struct _IO_STATUS_BLOCK
{
    union
    {
        NTSTATUS Status;
        PVOID Pointer;
    };
    ULONG_PTR Information;
} IO_STATUS_BLOCK, *PIO_STATUS_BLOCK;

typedef NTSTATUS (NTAPI *_NtDeviceIoControlFile)(      
    HANDLE FileHandle,
    HANDLE Event,
    PVOID ApcRoutine,
    PVOID ApcContext,
    PIO_STATUS_BLOCK IoStatusBlock,
    ULONG IoControlCode,
    PVOID InputBuffer,
    ULONG InputBufferLength,
    PVOID OutputBuffer,
    ULONG OutputBufferLength
    );

NTSTATUS KphInit();

NPHAPI NTSTATUS KphConnect(PHANDLE KphHandle);

NPHAPI NTSTATUS KphDisconnect(HANDLE KphHandle);

NPHAPI NTSTATUS KphGetFeatures(
    HANDLE KphHandle,
    PULONG Features
    );

NPHAPI NTSTATUS KphRead(
    HANDLE KphHandle,
    PVOID Address,
    PVOID Buffer,
    ULONG BufferLength
    );

NPHAPI NTSTATUS KphWrite(
    HANDLE KphHandle,
    PVOID Address,
    PVOID Buffer,
    ULONG Length
    );

NPHAPI NTSTATUS KphOpenProcess(
    HANDLE KphHandle,
    PHANDLE ProcessHandle,
    ULONG_PTR ProcessId,
    ACCESS_MASK DesiredAccess
    );

NPHAPI NTSTATUS KphOpenThread(
    HANDLE KphHandle,
    PHANDLE ThreadHandle,
    ULONG_PTR ThreadId,
    ACCESS_MASK DesiredAccess
    );

NPHAPI NTSTATUS KphOpenProcessToken(
    HANDLE KphHandle,
    PHANDLE TokenHandle,
    HANDLE ProcessHandle,
    ACCESS_MASK DesiredAccess
    );

NPHAPI NTSTATUS KphGetProcessProtected(
    HANDLE KphHandle,
    ULONG_PTR ProcessId,
    PBOOLEAN IsProtected
    );

NPHAPI NTSTATUS KphSetProcessProtected(
    HANDLE KphHandle,
    ULONG_PTR ProcessId,
    BOOLEAN IsProtected
    );

NPHAPI NTSTATUS KphTerminateProcess(
    HANDLE KphHandle,
    HANDLE ProcessHandle,
    NTSTATUS ExitStatus
    );

NPHAPI NTSTATUS KphSuspendProcess(
    HANDLE KphHandle,
    HANDLE ProcessHandle
    );

NPHAPI NTSTATUS KphResumeProcess(
    HANDLE KphHandle,
    HANDLE ProcessHandle
    );

NPHAPI NTSTATUS KphReadVirtualMemory(
    HANDLE KphHandle,
    HANDLE ProcessHandle,
    PVOID BaseAddress,
    PVOID Buffer,
    ULONG BufferLength,
    PULONG ReturnLength
    );

NPHAPI NTSTATUS KphWriteVirtualMemory(
    HANDLE KphHandle,
    HANDLE ProcessHandle,
    PVOID BaseAddress,
    PVOID Buffer,
    ULONG BufferLength,
    PULONG ReturnLength
    );

NPHAPI NTSTATUS KphOpenProcessJob(
    HANDLE KphHandle,
    PHANDLE JobHandle,
    HANDLE ProcessHandle,
    ACCESS_MASK DesiredAccess
    );

NPHAPI NTSTATUS KphGetContextThread(
    HANDLE KphHandle,
    HANDLE ThreadHandle,
    PCONTEXT ThreadContext
    );

NPHAPI NTSTATUS KphSetContextThread(
    HANDLE KphHandle,
    HANDLE ThreadHandle,
    PCONTEXT ThreadContext
    );

NPHAPI NTSTATUS KphTerminateThread(
    HANDLE KphHandle,
    HANDLE ThreadHandle,
    NTSTATUS ExitStatus
    );

NPHAPI NTSTATUS KphSetHandleGrantedAccess(
    HANDLE KphHandle,
    HANDLE Handle,
    ACCESS_MASK GrantedAccess
    );

NTSTATUS KphProtectAdd(
    HANDLE KphHandle,
    HANDLE ProcessHandle,
    BOOLEAN AllowKernelMode,
    ACCESS_MASK ProcessAllowMask,
    ACCESS_MASK ThreadAllowMask
    );

NTSTATUS KphProtectRemove(
    HANDLE KphHandle,
    HANDLE ProcessHandle
    );

NTSTATUS KphProtectQuery(
    HANDLE KphHandle,
    HANDLE ProcessHandle,
    PBOOLEAN AllowKernelMode,
    PACCESS_MASK ProcessAllowMask,
    PACCESS_MASK ThreadAllowMask
    );

#endif
