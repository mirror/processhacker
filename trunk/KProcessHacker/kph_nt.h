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

/* Kernel API */
NTSTATUS NTAPI ObOpenObjectByName(
    POBJECT_ATTRIBUTES ObjectAttributes,
    POBJECT_TYPE ObjectType,
    KPROCESSOR_MODE AccessMode,
    PACCESS_STATE AccessState,
    ACCESS_MASK DesiredAccess,
    PVOID ParseContext,
    PHANDLE Handle
    );

NTSTATUS NTAPI PsLookupProcessThreadByCid(
    PCLIENT_ID ClientId,
    PEPROCESS *Process,
    PETHREAD *Thread
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

NTSTATUS PsSuspendProcess(
    PEPROCESS Process
    );

NTSTATUS PsResumeProcess(
    PEPROCESS Process
    );

/* KProcessHacker versions */
NTSTATUS KphOpenProcess(
    PHANDLE ProcessHandle,
    ACCESS_MASK DesiredAccess,
    POBJECT_ATTRIBUTES ObjectAttributes,
    PCLIENT_ID ClientId,
    KPROCESSOR_MODE AccessMode
    );

NTSTATUS KphOpenThread(
    PHANDLE ThreadHandle,
    ACCESS_MASK DesiredAccess,
    POBJECT_ATTRIBUTES ObjectAttributes,
    PCLIENT_ID ClientId,
    KPROCESSOR_MODE AccessMode
    );

NTSTATUS KphOpenProcessTokenEx(
    HANDLE ProcessHandle,
    ACCESS_MASK DesiredAccess,
    ULONG ObjectAttributes,
    PHANDLE TokenHandle,
    KPROCESSOR_MODE AccessMode
    );

NTSTATUS KphSuspendProcess(
    HANDLE ProcessHandle
    );

NTSTATUS KphResumeProcess(
    HANDLE ProcessHandle
    );

NTSTATUS KphTerminateProcess(
    HANDLE ProcessHandle,
    NTSTATUS ExitStatus
    );

NTSTATUS OpenProcess(
    PHANDLE ProcessHandle,
    int DesiredAccess,
    int ProcessId
    );

#endif