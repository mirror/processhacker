/*
 * Process Hacker Driver - 
 *   custom versions of certain APIs - header file
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
#include "kernel_types.h"

NTSTATUS NTAPI ObOpenObjectByName(
    POBJECT_ATTRIBUTES ObjectAttributes,
    POBJECT_TYPE ObjectType,
    KPROCESSOR_MODE PreviousMode,
    PACCESS_STATE AccessState,
    ACCESS_MASK DesiredAccess,
    PVOID ParseContext,
    PHANDLE Handle
    );

NTSTATUS PsGetContextThread(
    PETHREAD Thread,
    PCONTEXT ThreadContext,
    KPROCESSOR_MODE PreviousMode
    );

NTSTATUS NTAPI PsLookupProcessThreadByCid(
    PCLIENT_ID ClientId,
    PEPROCESS *Process,
    PETHREAD *Thread
    );

NTSTATUS PsSetContextThread(
    PETHREAD Thread,
    PCONTEXT ThreadContext,
    KPROCESSOR_MODE PreviousMode
    );

NTKERNELAPI NTSTATUS NTAPI SeCreateAccessState(
    PACCESS_STATE AccessState,
    PAUX_ACCESS_DATA AuxData,
    ACCESS_MASK DesiredAccess,
    PGENERIC_MAPPING Mapping
    );

NTKERNELAPI VOID NTAPI SeDeleteAccessState(
    PACCESS_STATE AccessState
    );

/* Dynamically linked API defs */

typedef NTSTATUS (NTAPI *_MmCopyVirtualMemory)(
    PEPROCESS FromProcess,
    PVOID FromAddress,
    PEPROCESS ToProcess,
    PVOID ToAddress,
    ULONG BufferLength,
    KPROCESSOR_MODE AccessMode,
    PULONG ReturnLength
    );

typedef PVOID (NTAPI *_PsGetProcessJob)(
    PEPROCESS Process
    );

typedef NTSTATUS (NTAPI *_PsResumeProcess)(
    PEPROCESS Process
    );

typedef NTSTATUS (NTAPI *_PsSuspendProcess)(
    PEPROCESS Process
    );

/* KProcessHacker */
NTSTATUS KphNtInit();

NTSTATUS OpenProcess(
    PHANDLE ProcessHandle,
    int DesiredAccess,
    HANDLE ProcessId
    );

NTSTATUS KphGetContextThread(
    HANDLE ThreadHandle,
    PCONTEXT ThreadContext,
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

#endif