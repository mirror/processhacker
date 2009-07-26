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

/* Client entries
 * 
 * Client entries describe a process and a circular buffer which 
 * receives logging events.
 */

typedef struct _KPHSS_CLIENT_ENTRY
{
    PEPROCESS Process;
    
    /* Buffer */
    PKSEMAPHORE ReadSemaphore;
    PKSEMAPHORE WriteSemaphore;
    FAST_MUTEX BufferMutex;
    PVOID BufferBase;
    ULONG BufferSize;
    ULONG BufferCursor;
    
    /* Statistics */
    ULONG NumberOfBlocksWritten; /* excludes reset blocks */
    ULONG NumberOfBlocksDropped;
} KPHSS_CLIENT_ENTRY, *PKPHSS_CLIENT_ENTRY;

/* Rulesets
 * 
 * Rulesets contain a list of rules and an action to take if a 
 * system service matches the set of rules.
 */

#define KPHSS_RULESET_ENTRY(ListEntry) \
    CONTAINING_RECORD((ListEntry), KPHSS_RULESET_ENTRY, RuleSetListEntry)
#define KPHSS_RULESET_ENTRY_LIMIT 10
#define KPHSS_RULE_HANDLE_INCREMENT 4

typedef struct _KPHSS_RULESET_ENTRY
{
    LIST_ENTRY RuleSetListEntry;
    /* The client is referenced. */
    PKPHSS_CLIENT_ENTRY Client;
    
    KPHSS_RULESET_ACTION Action;
    KPHSS_FILTER_TYPE DefaultFilterType;
    
    ULONG NextRuleHandle;
    FAST_MUTEX RuleListMutex;
    /* A list of rules. Each rule is referenced when stored. */
    LIST_ENTRY RuleListHead;
} KPHSS_RULESET_ENTRY, *PKPHSS_RULESET_ENTRY;

/* Rules */

#define KPHSS_RULE_ENTRY(ListEntry) \
    CONTAINING_RECORD((ListEntry), KPHSS_RULE_ENTRY, RuleListEntry)

typedef struct _KPHSS_RULE_ENTRY
{
    BOOLEAN Initialized;
    HANDLE Handle;
    LIST_ENTRY RuleListEntry;
    
    KPHSS_FILTER_TYPE FilterType;
    KPHSS_RULE_TYPE RuleType;
    
    union
    {
        struct
        {
            HANDLE ProcessId;
        } ProcessIdRule;
        struct
        {
            HANDLE ThreadId;
        } ThreadIdRule;
        struct
        {
            KPROCESSOR_MODE PreviousMode;
        } PreviousModeRule;
        struct
        {
            ULONG Number;
        } NumberRule;
    };
} KPHSS_RULE_ENTRY, *PKPHSS_RULE_ENTRY;

typedef enum _KPHSS_SEQUENCE_MODE
{
    NoSequence,
    StartSequence,
    InSequence,
    EndSequence
} KPHSS_SEQUENCE_MODE;

#define TAG_CAPTURE_TEMP_BUFFER ('tChP')

FORCEINLINE PKPHSS_ARGUMENT_BLOCK KphpSsAllocateArgumentBlock(
    __in ULONG InnerSize,
    __in KPHSS_ARGUMENT_TYPE Type
    )
{
    PKPHSS_ARGUMENT_BLOCK argumentBlock;
    ULONG size;
    
    size = KPHSS_ARGUMENT_BLOCK_SIZE(InnerSize);
    argumentBlock = ExAllocatePoolWithTag(
        PagedPool,
        size,
        TAG_ARGUMENT_BLOCK
        );
    
    if (!argumentBlock)
        return NULL;
    
    argumentBlock->Header.Type = ArgumentBlockType;
    argumentBlock->Header.Size = size;
    argumentBlock->Type = Type;
    
    return argumentBlock;
}

FORCEINLINE NTSTATUS KphpSsCaptureSimple(
    __out PKPHSS_ARGUMENT_BLOCK *ArgumentBlock,
    __in PVOID Argument,
    __in KPHSS_ARGUMENT_TYPE Type
    )
{
    PKPHSS_ARGUMENT_BLOCK argumentBlock;
    ULONG size;
    LARGE_INTEGER value;
    
    switch (Type)
    {
        case Int8Argument:
            size = sizeof(BOOLEAN);
            break;
        case Int16Argument:
            size = sizeof(SHORT);
            break;
        case Int32Argument:
            size = sizeof(LONG);
            break;
        case Int64Argument:
            size = sizeof(LARGE_INTEGER);
            break;
        default:
            return STATUS_INVALID_PARAMETER_3;
    }
    
    __try
    {
        ProbeForRead(Argument, size, 1);
        memcpy(&value, Argument, size);
    }
    __except (EXCEPTION_EXECUTE_HANDLER)
    {
        return GetExceptionCode();
    }
    
    argumentBlock = KphpSsAllocateArgumentBlock(size, Type);
    
    if (!argumentBlock)
        return STATUS_INSUFFICIENT_RESOURCES;
    
    memcpy(&argumentBlock->Simple, &value, size);
    *ArgumentBlock = argumentBlock;
    
    return STATUS_SUCCESS;
}

#define CAPTURE_HANDLE_BUFFER_SIZE 0x400

FORCEINLINE NTSTATUS KphpSsCaptureHandle(
    __out PKPHSS_ARGUMENT_BLOCK *ArgumentBlock,
    __in HANDLE Argument
    )
{
    NTSTATUS status = STATUS_SUCCESS;
    PKPHSS_ARGUMENT_BLOCK argumentBlock;
    PVOID object;
    PUNICODE_STRING objectTypeName;
    PUNICODE_STRING objectNameInfo;
    ULONG returnLength;
    PKPHSS_WSTRING wString;
    
    /* Reference the object. */
    status = ObReferenceObjectByHandle(
        Argument,
        0,
        NULL,
        KernelMode,
        &object,
        NULL
        );
    
    if (!NT_SUCCESS(status))
        return status;
    
    /* Get a pointer to the UNICODE_STRING containing the 
     * object type name.
     */
    objectTypeName = (PUNICODE_STRING)KVOFF(
        OBJECT_TO_OBJECT_HEADER(object)->Type,
        OffOtName
        );
    
    /* Allocate a buffer for name information. */
    objectNameInfo = ExAllocatePoolWithTag(
        PagedPool,
        CAPTURE_HANDLE_BUFFER_SIZE,
        TAG_CAPTURE_TEMP_BUFFER
        );
    
    if (!objectNameInfo)
        goto CleanupObject;
    
    /* Query the name of the object. */
    status = KphQueryNameObject(
        object,
        objectNameInfo,
        CAPTURE_HANDLE_BUFFER_SIZE,
        &returnLength
        );
    
    if (!NT_SUCCESS(status))
        goto CleanupName;
    
    /* Allocate an argument block. */
    argumentBlock = KphpSsAllocateArgumentBlock(
        sizeof(KPHSS_HANDLE) + sizeof(KPHSS_WSTRING) + sizeof(KPHSS_WSTRING) + 
        objectTypeName->Length + objectNameInfo->Length,
        HandleArgument
        );
    
    if (!argumentBlock)
        goto CleanupName;
    
    /* Copy the type name into the block. */
    argumentBlock->Handle.TypeNameOffset = sizeof(KPHSS_HANDLE);
    wString = (PKPHSS_WSTRING)PTR_ADD_OFFSET(&argumentBlock->Handle, argumentBlock->Handle.TypeNameOffset);
    wString->Length = objectTypeName->Length;
    memcpy(&wString->Buffer, objectTypeName->Buffer, wString->Length);
    
    /* Copy the object name into the block. */
    argumentBlock->Handle.NameOffset = 
        argumentBlock->Handle.TypeNameOffset + sizeof(KPHSS_WSTRING) + 
        wString->Length;
    wString = (PKPHSS_WSTRING)PTR_ADD_OFFSET(&argumentBlock->Handle, argumentBlock->Handle.NameOffset);
    wString->Length = objectNameInfo->Length;
    memcpy(&wString->Buffer, objectNameInfo->Buffer, wString->Length);
    
    *ArgumentBlock = argumentBlock;
    
CleanupName:
    ExFreePoolWithTag(objectNameInfo, TAG_CAPTURE_TEMP_BUFFER);
CleanupObject:
    ObDereferenceObject(object);
    
    return status;
}

/* KphpSsMatchRuleSetEntry
 * 
 * Determines if a ruleset is relevant to an event.
 * 
 * Note: This function is inlined for performance reasons.
 */
FORCEINLINE BOOLEAN KphpSsMatchRuleSetEntry(
    __in PKPHSS_RULESET_ENTRY RuleSetEntry,
    __in ULONG Number,
    __in ULONG *Arguments,
    __in ULONG NumberOfArguments,
    __in PKSERVICE_TABLE_DESCRIPTOR ServiceTable,
    __in PKTHREAD Thread,
    __in KPROCESSOR_MODE PreviousMode
    )
{
    PLIST_ENTRY currentListEntry;
    BOOLEAN isRuleSetMatch = FALSE;
    
    /* Get the default filter type. If it is the Include 
     * filter type, we assume the ruleset matches. If it 
     * is the Exclude filter type, we assume it doesn't.
     */
    if (RuleSetEntry->DefaultFilterType == IncludeFilterType)
        isRuleSetMatch = TRUE;
    else if (RuleSetEntry->DefaultFilterType == ExcludeFilterType)
        isRuleSetMatch = FALSE;
    
    ExAcquireFastMutex(&RuleSetEntry->RuleListMutex);
    
    currentListEntry = RuleSetEntry->RuleListHead.Flink;
    
    while (currentListEntry != &RuleSetEntry->RuleListHead)
    {
        PKPHSS_RULE_ENTRY ruleEntry = KPHSS_RULE_ENTRY(currentListEntry);
        BOOLEAN isRuleMatch = FALSE;
        
        /* Check if the rule is initialized. */
        if (!ruleEntry->Initialized)
        {
            currentListEntry = currentListEntry->Flink;
            continue;
        }
        
        /* Attempt to match the rule. All rule types are 
         * considered in this one function.
         */
        switch (ruleEntry->RuleType)
        {
            case ProcessIdRuleType:
                if (PsGetProcessId(IoThreadToProcess(Thread)) == 
                    ruleEntry->ProcessIdRule.ProcessId)
                    isRuleMatch = TRUE;
                break;
            case ThreadIdRuleType:
                if (PsGetThreadId(Thread) == ruleEntry->ThreadIdRule.ThreadId)
                    isRuleMatch = TRUE;
                break;
            case PreviousModeRuleType:
                if (PreviousMode == ruleEntry->PreviousModeRule.PreviousMode)
                    isRuleMatch = TRUE;
                break;
            case NumberRuleType:
                if (Number == ruleEntry->NumberRule.Number)
                    isRuleMatch = TRUE;
                break;
        }
        
        /* Now that we have attempted to match the rule, we 
         * must look at the rule filter type to determine 
         * whether we should continue:
         * 
         * * For the Include filter type, we note that the 
         *   we have a match, but we still have to continue 
         *   going down the rule list since there may be 
         *   Exclude filters.
         * * For the Exclude filter type, we can simply stop 
         *   the matching and return - Exclude filters take 
         *   precedence.
         */
        if (isRuleMatch)
        {
            if (ruleEntry->FilterType == IncludeFilterType)
            {
                isRuleSetMatch = TRUE;
            }
            else if (ruleEntry->FilterType == ExcludeFilterType)
            {
                isRuleSetMatch = FALSE;
                break;
            }
        }
        
        currentListEntry = currentListEntry->Flink;
    }
    
    ExReleaseFastMutex(&RuleSetEntry->RuleListMutex);
    
    return isRuleSetMatch;
}

/* Functions */

VOID NTAPI KphpSsClientEntryDeleteProcedure(
    __in PVOID Object,
    __in ULONG Flags,
    __in SIZE_T Size
    );

VOID NTAPI KphpSsRuleSetEntryDeleteProcedure(
    __in PVOID Object,
    __in ULONG Flags,
    __in SIZE_T Size
    );

NTSTATUS KphpSsAddRule(
    __out PKPHSS_RULE_ENTRY *RuleEntry,
    __in PKPHSS_RULESET_ENTRY RuleSetEntry,
    __in KPHSS_FILTER_TYPE FilterType,
    __in KPHSS_RULE_TYPE RuleType
    );

VOID NTAPI KphpSsRuleEntryDeleteProcedure(
    __in PVOID Object,
    __in ULONG Flags,
    __in SIZE_T Size
    );

NTSTATUS KphpSsCreateEventBlock(
    __out PKPHSS_EVENT_BLOCK *EventBlock,
    __in PKTHREAD Thread,
    __in ULONG Number,
    __in ULONG *Arguments,
    __in ULONG NumberOfArguments
    );

VOID KphpSsFreeEventBlock(
    __in PKPHSS_EVENT_BLOCK EventBlock
    );

NTSTATUS KphpSsCreateArgumentBlock(
    __out PKPHSS_ARGUMENT_BLOCK *ArgumentBlock,
    __in ULONG Number,
    __in ULONG Argument,
    __in ULONG Index
    );

VOID KphpSsFreeArgumentBlock(
    __in PKPHSS_ARGUMENT_BLOCK ArgumentBlock
    );

NTSTATUS KphpSsWriteBlock(
    __in PKPHSS_CLIENT_ENTRY ClientEntry,
    __in_opt PKPHSS_BLOCK_HEADER Block,
    __in KPHSS_SEQUENCE_MODE SequenceMode
    );

VOID NTAPI KphpSsLogSystemServiceCall(
    __in ULONG Number,
    __in ULONG *Arguments,
    __in ULONG NumberOfArguments,
    __in PKSERVICE_TABLE_DESCRIPTOR ServiceTable,
    __in PKTHREAD Thread
    );

VOID NTAPI KphpSsNewKiFastCallEntry();

#endif
