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
#define CAPTURE_HANDLE_BUFFER_SIZE 0x400
#define CAPTURE_UNICODE_STRING_MAX_SIZE 0x400

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

NTSTATUS KphpSsCaptureSimpleArgument(
    __out PKPHSS_ARGUMENT_BLOCK *ArgumentBlock,
    __in PVOID Argument,
    __in KPHSS_ARGUMENT_TYPE Type,
    __in KPROCESSOR_MODE PreviousMode
    );

NTSTATUS KphpSsCaptureHandleArgument(
    __out PKPHSS_ARGUMENT_BLOCK *ArgumentBlock,
    __in HANDLE Argument,
    __in KPROCESSOR_MODE PreviousMode
    );

NTSTATUS KphpSsCaptureUnicodeStringArgument(
    __out PKPHSS_ARGUMENT_BLOCK *ArgumentBlock,
    __in PUNICODE_STRING Argument,
    __in KPROCESSOR_MODE PreviousMode
    );

NTSTATUS KphpSsCaptureObjectAttributesArgument(
    __out PKPHSS_ARGUMENT_BLOCK *ArgumentBlock,
    __in POBJECT_ATTRIBUTES Argument,
    __in KPROCESSOR_MODE PreviousMode
    );

NTSTATUS KphpSsCreateArgumentBlock(
    __out PKPHSS_ARGUMENT_BLOCK *ArgumentBlock,
    __in ULONG Number,
    __in ULONG Argument,
    __in ULONG Index
    );

PKPHSS_ARGUMENT_BLOCK KphpSsAllocateArgumentBlock(
    __in ULONG InnerSize,
    __in KPHSS_ARGUMENT_TYPE Type
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
