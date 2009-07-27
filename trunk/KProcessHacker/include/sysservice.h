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

#ifndef _SYSSERVICE_H
#define _SYSSERVICE_H

#include "kph.h"
#include "sysservicedata.h"

/* Define opaque object types */

struct _KPHSS_CLIENT_ENTRY;
typedef struct _KPHSS_CLIENT_ENTRY *PKPHSS_CLIENT_ENTRY;
struct _KPHSS_RULESET_ENTRY;
typedef struct _KPHSS_RULESET_ENTRY *PKPHSS_RULESET_ENTRY;
struct _KPHSS_RULE_ENTRY;
typedef struct _KPHSS_RULE_ENTRY *PKPHSS_RULE_ENTRY;

/* Information types */

typedef struct _KPHSS_CLIENT_INFORMATION
{
    HANDLE ProcessId;
    PVOID BufferBase;
    ULONG BufferSize;
    
    ULONG NumberOfBlocksWritten;
    ULONG NumberOfBlocksDropped;
} KPHSS_CLIENT_INFORMATION, *PKPHSS_CLIENT_INFORMATION;

/* Object types */

#ifndef _SYSSERVICE_PRIVATE
extern PKPH_OBJECT_TYPE KphSsClientEntryType;
extern PKPH_OBJECT_TYPE KphSsRuleSetEntryType;
extern PKPH_OBJECT_TYPE KphSsRuleEntryType;
#endif

/* Ruleset types */

typedef enum _KPHSS_RULESET_ACTION
{
    LogRuleSetAction,
    MaxRuleSetAction
} KPHSS_RULESET_ACTION;

/* Rule types */

typedef enum _KPHSS_FILTER_TYPE
{
    IncludeFilterType,
    ExcludeFilterType,
    MaxFilterType
} KPHSS_FILTER_TYPE;

typedef enum _KPHSS_RULE_TYPE
{
    ProcessIdRuleType,
    ThreadIdRuleType,
    PreviousModeRuleType,
    NumberRuleType,
    MaxRuleType
} KPHSS_RULE_TYPE;

/* Block types */

#define KPHSS_BLOCK_SUCCESS(Status) (NT_SUCCESS(Status) && (Status) != STATUS_TIMEOUT)

typedef enum _KPHSS_BLOCK_TYPE
{
    ResetBlockType,
    EventBlockType,
    ArgumentBlockType
} KPHSS_BLOCK_TYPE;

typedef struct _KPHSS_BLOCK_HEADER
{
    ULONG Size; /* a.k.a. NextEntryOffset */
    ULONG Type;
} KPHSS_BLOCK_HEADER, *PKPHSS_BLOCK_HEADER;

typedef struct _KPHSS_RESET_BLOCK
{
    KPHSS_BLOCK_HEADER Header;
} KPHSS_RESET_BLOCK, *PKPHSS_RESET_BLOCK;

#define TAG_EVENT_BLOCK ('BEhP')

#define KPHSS_EVENT_PROBE_ARGUMENTS_FAILED 0x00000001
#define KPHSS_EVENT_COPY_ARGUMENTS_FAILED 0x00000002
#define KPHSS_EVENT_KERNEL_MODE 0x00000004
#define KPHSS_EVENT_USER_MODE 0x00000008

typedef struct _KPHSS_EVENT_BLOCK
{
    KPHSS_BLOCK_HEADER Header;
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
    ULONG TraceOffset;
} KPHSS_EVENT_BLOCK, *PKPHSS_EVENT_BLOCK;

/* Argument Blocks
 * 
 * These blocks provide additional information about 
 * arguments.
 */

#define TAG_ARGUMENT_BLOCK ('BAhP')

#define KPHSS_ARGUMENT_BLOCK_OVERHEAD \
    FIELD_OFFSET(KPHSS_ARGUMENT_BLOCK, Normal)
#define KPHSS_ARGUMENT_BLOCK_SIZE(InnerSize) \
    (KPHSS_ARGUMENT_BLOCK_OVERHEAD + (InnerSize))

typedef struct _KPHSS_ARGUMENT_BLOCK
{
    KPHSS_BLOCK_HEADER Header;
    ULONG Index;
    KPHSS_ARGUMENT_TYPE Type;
    
    union
    {
        ULONG Normal;
        
        LARGE_INTEGER Simple;
        KPHSS_HANDLE Handle;
        KPHSS_STRING String;
        KPHSS_WSTRING WString;
        KPHSS_ANSI_STRING AnsiString;
        KPHSS_UNICODE_STRING UnicodeString;
        KPHSS_OBJECT_ATTRIBUTES ObjectAttributes;
        CLIENT_ID ClientId;
        CONTEXT Context;
        KPHSS_INITIAL_TEB InitialTeb;
    };
} KPHSS_ARGUMENT_BLOCK, *PKPHSS_ARGUMENT_BLOCK;

/* Functions */

NTSTATUS KphSsLogInit();
NTSTATUS KphSsLogDeinit();
NTSTATUS KphSsLogStart();
NTSTATUS KphSsLogStop();

NTSTATUS KphSsCreateClientEntry(
    __out PKPHSS_CLIENT_ENTRY *ClientEntry,
    __in HANDLE ProcessHandle,
    __in HANDLE ReadSemaphoreHandle,
    __in HANDLE WriteSemaphoreHandle,
    __in PVOID BufferBase,
    __in ULONG BufferSize,
    __in KPROCESSOR_MODE AccessMode
    );

NTSTATUS KphSsQueryClientEntry(
    __in PKPHSS_CLIENT_ENTRY ClientEntry,
    __out_bcount_opt(ClientInformationLength) PKPHSS_CLIENT_INFORMATION ClientInformation,
    __in ULONG ClientInformationLength,
    __out_opt PULONG ReturnLength,
    __in KPROCESSOR_MODE AccessMode
    );

NTSTATUS KphSsCreateRuleSetEntry(
    __out PKPHSS_RULESET_ENTRY *RuleSetEntry,
    __in PKPHSS_CLIENT_ENTRY ClientEntry,
    __in KPHSS_FILTER_TYPE DefaultFilterType,
    __in KPHSS_RULESET_ACTION Action
    );

HANDLE KphSsGetHandleRule(
    __in PKPHSS_RULE_ENTRY RuleEntry
    );

NTSTATUS KphSsRemoveRule(
    __in PKPHSS_RULESET_ENTRY RuleSetEntry,
    __in HANDLE RuleEntryHandle
    );

NTSTATUS KphSsAddProcessIdRule(
    __out PKPHSS_RULE_ENTRY *RuleEntry,
    __in PKPHSS_RULESET_ENTRY RuleSetEntry,
    __in KPHSS_FILTER_TYPE FilterType,
    __in HANDLE ProcessId
    );

NTSTATUS KphSsAddThreadIdRule(
    __out PKPHSS_RULE_ENTRY *RuleEntry,
    __in PKPHSS_RULESET_ENTRY RuleSetEntry,
    __in KPHSS_FILTER_TYPE FilterType,
    __in HANDLE ThreadId
    );

NTSTATUS KphSsAddPreviousModeRule(
    __out PKPHSS_RULE_ENTRY *RuleEntry,
    __in PKPHSS_RULESET_ENTRY RuleSetEntry,
    __in KPHSS_FILTER_TYPE FilterType,
    __in KPROCESSOR_MODE PreviousMode
    );

NTSTATUS KphSsAddNumberRule(
    __out PKPHSS_RULE_ENTRY *RuleEntry,
    __in PKPHSS_RULESET_ENTRY RuleSetEntry,
    __in KPHSS_FILTER_TYPE FilterType,
    __in ULONG Number
    );

#endif
