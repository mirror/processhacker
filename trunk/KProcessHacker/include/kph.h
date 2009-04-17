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
#include "mm.h"
#include "ps.h"

extern int WindowsVersion;
extern ACCESS_MASK ProcessAllAccess;
extern ACCESS_MASK ThreadAllAccess;

extern _PsGetProcessJob PsGetProcessJob;
extern _PsSuspendProcess PsSuspendProcess;
extern _PsResumeProcess PsResumeProcess;
extern _MmCopyVirtualMemory MmCopyVirtualMemory;

/* KProcessHacker */
NTSTATUS KphNtInit();

PVOID GetSystemRoutineAddress(
    WCHAR *Name
    );

NTSTATUS OpenProcess(
    PHANDLE ProcessHandle,
    int DesiredAccess,
    HANDLE ProcessId
    );

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

NTSTATUS KphGetContextThread(
    HANDLE ThreadHandle,
    PCONTEXT ThreadContext,
    KPROCESSOR_MODE AccessMode
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

#endif