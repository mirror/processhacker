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
#include "nativedefs.h"

#define KPH_DEVICE_TYPE (0x9999)
#define KPH_DEVICE_NAME (L"\\\\.\\KProcessHacker")

#define KPHF_PSTERMINATEPROCESS 0x1
#define KPHF_PSPTERMINATETHREADBPYPOINTER 0x2

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
    ((DeviceType) << 16) | ((Access) << 14) | ((Function) << 2) | (Method))
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
#define KPH_UNSAFEREADVIRTUALMEMORY KPH_CTL_CODE(32)
#define KPH_SETEXECUTEOPTIONS KPH_CTL_CODE(33)
#define KPH_QUERYPROCESSHANDLES KPH_CTL_CODE(34)
#define KPH_OPENTHREADPROCESS KPH_CTL_CODE(35)

#ifndef MEM_EXECUTE_OPTION_DISABLE
#define MEM_EXECUTE_OPTION_DISABLE 0x1 
#define MEM_EXECUTE_OPTION_ENABLE 0x2
#define MEM_EXECUTE_OPTION_DISABLE_THUNK_EMULATION 0x4
#define MEM_EXECUTE_OPTION_PERMANENT 0x8
#endif

typedef struct _PROCESS_HANDLE
{
    HANDLE Handle;
    PVOID Object;
    ACCESS_MASK GrantedAccess;
    ULONG HandleAttributes;
} PROCESS_HANDLE, *PPROCESS_HANDLE;

typedef struct _PROCESS_HANDLE_INFORMATION
{
    ULONG HandleCount;
    PROCESS_HANDLE Handles[1];
} PROCESS_HANDLE_INFORMATION, *PPROCESS_HANDLE_INFORMATION;

NTSTATUS KphInit();

NPHAPI NTSTATUS KphConnect(
    __out PHANDLE KphHandle
    );

NPHAPI NTSTATUS KphDisconnect(
    __in HANDLE KphHandle
    );

NPHAPI NTSTATUS KphGetFeatures(
    __in HANDLE KphHandle,
    __out PULONG Features
    );

NPHAPI NTSTATUS KphRead(
    __in HANDLE KphHandle,
    __in PVOID Address,
    __out_bcount(BufferLength) PVOID Buffer,
    __in ULONG BufferLength
    );

NPHAPI NTSTATUS KphWrite(
    __in HANDLE KphHandle,
    __in PVOID Address,
    __in_bcount(Length) PVOID Buffer,
    __in ULONG Length
    );

NPHAPI NTSTATUS KphOpenProcess(
    __in HANDLE KphHandle,
    __out PHANDLE ProcessHandle,
    __in HANDLE ProcessId,
    __in ACCESS_MASK DesiredAccess
    );

NPHAPI NTSTATUS KphOpenThread(
    __in HANDLE KphHandle,
    __out PHANDLE ThreadHandle,
    __in HANDLE ThreadId,
    __in ACCESS_MASK DesiredAccess
    );

NPHAPI NTSTATUS KphOpenProcessToken(
    __in HANDLE KphHandle,
    __out PHANDLE TokenHandle,
    __in HANDLE ProcessHandle,
    __in ACCESS_MASK DesiredAccess
    );

NPHAPI NTSTATUS KphGetProcessProtected(
    __in HANDLE KphHandle,
    __in ULONG_PTR ProcessId,
    __out PBOOLEAN IsProtected
    );

NPHAPI NTSTATUS KphSetProcessProtected(
    __in HANDLE KphHandle,
    __in ULONG_PTR ProcessId,
    __in BOOLEAN IsProtected
    );

NPHAPI NTSTATUS KphTerminateProcess(
    __in HANDLE KphHandle,
    __in HANDLE ProcessHandle,
    __in NTSTATUS ExitStatus
    );

NPHAPI NTSTATUS KphSuspendProcess(
    __in HANDLE KphHandle,
    __in HANDLE ProcessHandle
    );

NPHAPI NTSTATUS KphResumeProcess(
    __in HANDLE KphHandle,
    __in HANDLE ProcessHandle
    );

NPHAPI NTSTATUS KphReadVirtualMemory(
    __in HANDLE KphHandle,
    __in HANDLE ProcessHandle,
    __in PVOID BaseAddress,
    __out_bcount(BufferLength) PVOID Buffer,
    __in ULONG BufferLength,
    __out_opt PULONG ReturnLength
    );

NPHAPI NTSTATUS KphWriteVirtualMemory(
    __in HANDLE KphHandle,
    __in HANDLE ProcessHandle,
    __in PVOID BaseAddress,
    __in_bcount(BufferLength) PVOID Buffer,
    __in ULONG BufferLength,
    __out_opt PULONG ReturnLength
    );

NPHAPI NTSTATUS KphOpenProcessJob(
    __in HANDLE KphHandle,
    __out PHANDLE JobHandle,
    __in HANDLE ProcessHandle,
    __in ACCESS_MASK DesiredAccess
    );

NPHAPI NTSTATUS KphGetContextThread(
    __in HANDLE KphHandle,
    __in HANDLE ThreadHandle,
    __inout PCONTEXT ThreadContext
    );

NPHAPI NTSTATUS KphSetContextThread(
    __in HANDLE KphHandle,
    __in HANDLE ThreadHandle,
    __in PCONTEXT ThreadContext
    );

NPHAPI NTSTATUS KphTerminateThread(
    __in HANDLE KphHandle,
    __in HANDLE ThreadHandle,
    __in NTSTATUS ExitStatus
    );

NPHAPI NTSTATUS KphSetHandleGrantedAccess(
    __in HANDLE KphHandle,
    __in HANDLE Handle,
    __in ACCESS_MASK GrantedAccess
    );

NPHAPI NTSTATUS KphProtectAdd(
    __in HANDLE KphHandle,
    __in HANDLE ProcessHandle,
    __in BOOLEAN AllowKernelMode,
    __in ACCESS_MASK ProcessAllowMask,
    __in ACCESS_MASK ThreadAllowMask
    );

NPHAPI NTSTATUS KphProtectRemove(
    __in HANDLE KphHandle,
    __in HANDLE ProcessHandle
    );

NPHAPI NTSTATUS KphProtectQuery(
    __in HANDLE KphHandle,
    __in HANDLE ProcessHandle,
    __out PBOOLEAN AllowKernelMode,
    __out PACCESS_MASK ProcessAllowMask,
    __out PACCESS_MASK ThreadAllowMask
    );

NPHAPI NTSTATUS KphUnsafeReadVirtualMemory(
    __in HANDLE KphHandle,
    __in HANDLE ProcessHandle,
    __in PVOID BaseAddress,
    __in_bcount(BufferLength) PVOID Buffer,
    __in ULONG BufferLength,
    __out_opt PULONG ReturnLength
    );

NPHAPI NTSTATUS KphSetExecuteOptions(
    __in HANDLE KphHandle,
    __in HANDLE ProcessHandle,
    __in ULONG ExecuteOptions
    );

NPHAPI NTSTATUS KphQueryProcessHandles(
    __in HANDLE KphHandle,
    __in HANDLE ProcessHandle,
    __out_bcount_opt(BufferLength) PVOID Buffer,
    __in_opt ULONG BufferLength,
    __out_opt PULONG ReturnLength
    );

NPHAPI NTSTATUS KphOpenThreadProcess(
    __in HANDLE KphHandle,
    __out PHANDLE ProcessHandle,
    __in HANDLE ThreadHandle,
    __in ACCESS_MASK DesiredAccess
    );

#endif
