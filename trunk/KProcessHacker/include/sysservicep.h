/*
 * Process Hacker Driver - 
 *   system service logging
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

#ifndef _SYSSERVICEP_H
#define _SYSSERVICEP_H

#include "sysservice.h"
#include "ref.h"

/* PKPHPSS_KIFASTCALLENTRYPROC
 * 
 * Represents a function called by KphpSsNewKiFastCallEntry.
 */
typedef VOID (NTAPI *PKPHPSS_KIFASTCALLENTRYPROC)(
    __in ULONG Number,
    __in PVOID *Arguments,
    __in ULONG NumberOfArguments,
    __in PKSERVICE_TABLE_DESCRIPTOR ServiceTable,
    __in PKTHREAD Thread
    );

typedef struct _KPHPSS_CLIENT_ENTRY
{
    PEPROCESS ClientProcess;
    PVOID ClientBufferBase;
    ULONG ClientBufferSize;
    ULONG ClientBufferCurrentPosition;
} KPHPSS_CLIENT_ENTRY, *PKPHPSS_CLIENT_ENTRY;

#define KPHPSS_PROCESS_ENTRY(ListEntry) \
    CONTAINING_RECORD((ListEntry), KPHPSS_PROCESS_ENTRY, ProcessListEntry)

typedef struct _KPHPSS_PROCESS_ENTRY
{
    LIST_ENTRY ProcessListEntry;
    
    PKPHPSS_CLIENT_ENTRY Client;
    PEPROCESS TargetProcess;
} KPHPSS_PROCESS_ENTRY, *PKPHPSS_PROCESS_ENTRY;

NTSTATUS KphpCreateClientEntry(
    __out PKPHPSS_CLIENT_ENTRY *ClientEntry,
    __in HANDLE ClientProcessHandle,
    __in PVOID ClientBufferBase,
    __in ULONG ClientBufferSize,
    __in KPROCESSOR_MODE AccessMode
    );

NTSTATUS KphpCreateProcessEntry(
    __out PKPHPSS_PROCESS_ENTRY *ProcessEntry,
    __in PKPHPSS_CLIENT_ENTRY ClientEntry,
    __in PEPROCESS TargetProcess
    );

VOID NTAPI KphpClientEntryDeleteProcedure(
    __in PVOID Object,
    __in ULONG Flags,
    __in SIZE_T Size
    );

VOID NTAPI KphpProcessEntryDeleteProcedure(
    __in PVOID Object,
    __in ULONG Flags,
    __in SIZE_T Size
    );

VOID NTAPI KphpSsLogSystemServiceCall(
    __in ULONG Number,
    __in PVOID *Arguments,
    __in ULONG NumberOfArguments,
    __in PKSERVICE_TABLE_DESCRIPTOR ServiceTable,
    __in PKTHREAD Thread
    );

VOID NTAPI KphpSsNewKiFastCallEntry();

#endif
