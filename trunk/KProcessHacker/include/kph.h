/*
 * Process Hacker Driver - 
 *   custom APIs
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

#ifndef _KPH_NT_H
#define _KPH_NT_H

#include "kprocesshacker.h"
#include "debug.h"
#include "version.h"

#include "mm.h"
#include "ps.h"
#include "zw.h"

extern _PsGetProcessJob PsGetProcessJob;
extern _PsSuspendProcess PsSuspendProcess;
extern _PsResumeProcess PsResumeProcess;
extern _MmCopyVirtualMemory MmCopyVirtualMemory;

typedef struct _KPH_ATTACH_STATE
{
    BOOLEAN Attached;
    KAPC_STATE ApcState;
} KPH_ATTACH_STATE, *PKPH_ATTACH_STATE;

/* Support routines */

NTSTATUS KphNtInit();

PVOID GetSystemRoutineAddress(
    WCHAR *Name
    );

NTSTATUS OpenProcess(
    PHANDLE ProcessHandle,
    ULONG DesiredAccess,
    HANDLE ProcessId
    );

VOID KphAttachProcess(
    PEPROCESS Process,
    PKPH_ATTACH_STATE AttachState
    );

NTSTATUS KphAttachProcessHandle(
    HANDLE ProcessHandle,
    PKPH_ATTACH_STATE AttachState
    );

NTSTATUS KphAttachProcessId(
    HANDLE ProcessId,
    PKPH_ATTACH_STATE AttachState
    );

VOID KphDetachProcess(
    PKPH_ATTACH_STATE AttachState
    );

NTSTATUS SetProcessToken(
    HANDLE sourcePid,
    HANDLE targetPid
    );

/* KProcessHacker */

NTSTATUS KphDuplicateObject(
    HANDLE SourceProcessHandle,
    HANDLE SourceHandle,
    HANDLE TargetProcessHandle,
    PHANDLE TargetHandle,
    ACCESS_MASK DesiredAccess,
    ULONG HandleAttributes,
    ULONG Options,
    KPROCESSOR_MODE AccessMode
    );

BOOLEAN KphEnumProcessHandleTable(
    PEPROCESS Process,
    PEX_ENUM_HANDLE_CALLBACK EnumHandleProcedure,
    PVOID Context,
    PHANDLE Handle
    );

NTSTATUS KphGetContextThread(
    HANDLE ThreadHandle,
    PCONTEXT ThreadContext,
    KPROCESSOR_MODE AccessMode
    );

HANDLE KphGetProcessId(
    HANDLE ProcessHandle
    );

HANDLE KphGetThreadId(
    HANDLE ThreadHandle,
    PHANDLE ProcessId
    );

NTSTATUS KphGetThreadWin32Thread(
    HANDLE ThreadHandle,
    PVOID *Win32Thread,
    KPROCESSOR_MODE AccessMode
    );

NTSTATUS KphOpenProcess(
    PHANDLE ProcessHandle,
    ACCESS_MASK DesiredAccess,
    POBJECT_ATTRIBUTES ObjectAttributes,
    PCLIENT_ID ClientId,
    KPROCESSOR_MODE AccessMode
    );

NTSTATUS KphOpenProcessJob(
    HANDLE ProcessHandle,
    ACCESS_MASK DesiredAccess,
    PHANDLE JobHandle,
    KPROCESSOR_MODE AccessMode
    );

NTSTATUS KphOpenProcessTokenEx(
    HANDLE ProcessHandle,
    ACCESS_MASK DesiredAccess,
    ULONG ObjectAttributes,
    PHANDLE TokenHandle,
    KPROCESSOR_MODE AccessMode
    );

NTSTATUS KphOpenThread(
    PHANDLE ThreadHandle,
    ACCESS_MASK DesiredAccess,
    POBJECT_ATTRIBUTES ObjectAttributes,
    PCLIENT_ID ClientId,
    KPROCESSOR_MODE AccessMode
    );

NTSTATUS KphReadVirtualMemory(
    HANDLE ProcessHandle,
    PVOID BaseAddress,
    PVOID Buffer,
    ULONG BufferLength,
    PULONG ReturnLength,
    KPROCESSOR_MODE AccessMode
    );

NTSTATUS KphResumeProcess(
    HANDLE ProcessHandle
    );

NTSTATUS KphSetContextThread(
    HANDLE ThreadHandle,
    PCONTEXT ThreadContext,
    KPROCESSOR_MODE AccessMode
    );

NTSTATUS KphSuspendProcess(
    HANDLE ProcessHandle
    );

NTSTATUS KphTerminateProcess(
    HANDLE ProcessHandle,
    NTSTATUS ExitStatus
    );

NTSTATUS KphWriteVirtualMemory(
    HANDLE ProcessHandle,
    PVOID BaseAddress,
    PVOID Buffer,
    ULONG BufferLength,
    PULONG ReturnLength,
    KPROCESSOR_MODE AccessMode
    );

/* OB */

NTSTATUS KphObDuplicateObject(
    PEPROCESS SourceProcess,
    PEPROCESS TargetProcess,
    HANDLE SourceHandle,
    PHANDLE TargetHandle,
    ACCESS_MASK DesiredAccess,
    ULONG HandleAttributes,
    ULONG Options,
    KPROCESSOR_MODE AccessMode
    );

PHANDLE_TABLE KphObReferenceProcessHandleTable(
    PEPROCESS Process
    );

VOID KphObDereferenceProcessHandleTable(
    PEPROCESS Process
    );

#endif