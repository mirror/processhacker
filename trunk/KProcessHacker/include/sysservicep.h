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

#define _SYSSERVICE_PRIVATE
#include "sysservice.h"
#include "ref.h"

/* PKPHPSS_KIFASTCALLENTRYPROC
 * 
 * Represents a function called by KphpSsNewKiFastCallEntry.
 */
typedef VOID (NTAPI *PKPHPSS_KIFASTCALLENTRYPROC)(
    __in ULONG Number,
    __in ULONG *Arguments,
    __in ULONG NumberOfArguments,
    __in PKSERVICE_TABLE_DESCRIPTOR ServiceTable,
    __in PKTHREAD Thread
    );

typedef struct _KPHSS_CLIENT_ENTRY
{
    PEPROCESS Process;
    PKEVENT Event;
    PKSEMAPHORE Semaphore;
    FAST_MUTEX BufferMutex;
    PVOID BufferBase;
    ULONG BufferSize;
    ULONG BufferCursor;
} KPHSS_CLIENT_ENTRY, *PKPHSS_CLIENT_ENTRY;

#define KPHSS_PROCESS_ENTRY(ListEntry) \
    CONTAINING_RECORD((ListEntry), KPHSS_PROCESS_ENTRY, ProcessListEntry)
#define KPHSS_PROCESS_ENTRY_LIMIT 10

typedef struct _KPHSS_PROCESS_ENTRY
{
    LIST_ENTRY ProcessListEntry;
    
    PKPHSS_CLIENT_ENTRY Client;
    PEPROCESS TargetProcess;
    ULONG Flags;
} KPHSS_PROCESS_ENTRY, *PKPHSS_PROCESS_ENTRY;

typedef enum _KPHPSS_BLOCK_TYPE
{
    HeadBlockType,
    ResetBlockType,
    EventBlockType
} KPHPSS_BLOCK_TYPE;

typedef struct _KPHPSS_BLOCK_HEADER
{
    ULONG Size; /* a.k.a. NextEntryOffset */
    ULONG Type;
} KPHPSS_BLOCK_HEADER, *PKPHPSS_BLOCK_HEADER;

typedef struct _KPHPSS_HEAD_BLOCK
{
    KPHPSS_BLOCK_HEADER Header;
} KPHPSS_HEAD_BLOCK, *PKPHPSS_HEAD_BLOCK;

typedef struct _KPHPSS_RESET_BLOCK
{
    KPHPSS_BLOCK_HEADER Header;
} KPHPSS_RESET_BLOCK, *PKPHPSS_RESET_BLOCK;

#define TAG_EVENT_BLOCK ('BEhP')

#define KPHPSS_EVENT_PROBE_ARGUMENTS_FAILED 0x00000001
#define KPHPSS_EVENT_COPY_ARGUMENTS_FAILED 0x00000002

typedef struct _KPHPSS_EVENT_BLOCK
{
    KPHPSS_BLOCK_HEADER Header;
    ULONG Flags;
    LARGE_INTEGER Time;
    CLIENT_ID ClientId;
    
    /* The system service number. */
    ULONG Number;
    /* The number of ULONG arguments to the system service. */
    ULONG NumberOfArguments;
    ULONG ArgumentsOffset;
    
    /* The number of PVOIDs in the trace. */
    ULONG TraceCount;
    ULONG TraceHash;
    ULONG TraceOffset;
} KPHPSS_EVENT_BLOCK, *PKPHPSS_EVENT_BLOCK;

VOID NTAPI KphpSsClientEntryDeleteProcedure(
    __in PVOID Object,
    __in ULONG Flags,
    __in SIZE_T Size
    );

VOID NTAPI KphpSsProcessEntryDeleteProcedure(
    __in PVOID Object,
    __in ULONG Flags,
    __in SIZE_T Size
    );

NTSTATUS KphpSsCreateEventBlock(
    __out PKPHPSS_EVENT_BLOCK *EventBlock,
    __in PKTHREAD Thread,
    __in ULONG Number,
    __in ULONG *Arguments,
    __in ULONG NumberOfArguments
    );

VOID KphpSsFreeEventBlock(
    __in PKPHPSS_EVENT_BLOCK EventBlock
    );

NTSTATUS KphpSsWriteBlock(
    __in PKPHSS_CLIENT_ENTRY ClientEntry,
    __in PKPHPSS_BLOCK_HEADER Block
    );

VOID NTAPI KphpSsLogSystemServiceCall(
    __in ULONG Number,
    __in ULONG *Arguments,
    __in ULONG NumberOfArguments,
    __in PKSERVICE_TABLE_DESCRIPTOR ServiceTable,
    __in PKTHREAD Thread
    );

VOID NTAPI KphpSsNewKiFastCallEntry();

/* KphpSsIsProcessEntryRelevant
 * 
 * Returns whether a system service call should be logged based on 
 * a process entry.
 */
FORCEINLINE BOOLEAN KphpSsIsProcessEntryRelevant(
    __in PKPHSS_PROCESS_ENTRY ProcessEntry,
    __in PEPROCESS Process,
    __in KPROCESSOR_MODE PreviousMode
    )
{
    return
        /* Check if the process entry is referring to the caller. */
        ProcessEntry->TargetProcess == Process && 
        /* Check the mode. */
        (
            ((ProcessEntry->Flags & KPHSS_LOG_USER_MODE) && (PreviousMode == UserMode)) || 
            ((ProcessEntry->Flags & KPHSS_LOG_KERNEL_MODE) && (PreviousMode == KernelMode))
        );
}

#endif
