/*
 * Process Hacker Driver - 
 *   system service logging (data)
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

#define _SYSSERVICEDATA_PRIVATE
#include "include/sysservicedata.h"

PVOID KphpSsCallEntryAllocateRoutine(
    __in PRTL_GENERIC_TABLE Table,
    __in CLONG ByteSize
    );

RTL_GENERIC_COMPARE_RESULTS KphpSsCallEntryCompareRoutine(
    __in PRTL_GENERIC_TABLE Table,
    __in PVOID FirstStruct,
    __in PVOID SecondStruct
    );

VOID KphpSsCallEntryFreeRoutine(
    __in PRTL_GENERIC_TABLE Table,
    __in PVOID Buffer
    );

/* NTSTATUS NtAddAtom(PWSTR String, ULONG StringLength, PUSHORT Atom) */
KPHSS_CALL_ENTRY SsNtAddAtomEntry = { &SsNtAddAtom, "NtAddAtom", 3, { WStringArgument, 0, Int16Argument } };
/* NTSTATUS NtAlertResumeThread(HANDLE ThreadHandle, PULONG PreviousSuspendCount) */
KPHSS_CALL_ENTRY SsNtAlertResumeThreadEntry = { &SsNtAlertResumeThread, "NtAlertResumeThread", 2, { HandleArgument, NormalArgument } };
/* NTSTATUS NtClose(HANDLE Handle) */
KPHSS_CALL_ENTRY SsNtCloseEntry = { &SsNtClose, "NtClose", 1, { HandleArgument } };
/* NTSTATUS NtContinue(PCONTEXT Context, BOOLEAN TestAlert) */
KPHSS_CALL_ENTRY SsNtContinueEntry = { &SsNtContinue, "NtContinue", 2, { ContextArgument, 0 } };

KPHSS_CALL_ENTRY SsEntries[] =
{
    /* NTSTATUS NtAddAtom(PWSTR String, ULONG StringLength, PUSHORT Atom) */
    { &SsNtAddAtom, "NtAddAtom", 3, { WStringArgument, 0, Int16Argument } },
    /* NTSTATUS NtAlertResumeThread(HANDLE ThreadHandle, PULONG PreviousSuspendCount) */
    { &SsNtAlertResumeThread, "NtAlertResumeThread", 2, { HandleArgument, NormalArgument } },
    /* NTSTATUS NtClose(HANDLE Handle) */
    { &SsNtClose, "NtClose", 1, { HandleArgument } },
    /* NTSTATUS NtContinue(PCONTEXT Context, BOOLEAN TestAlert) */
    { &SsNtContinue, "NtContinue", 2, { ContextArgument, 0 } },
    /* NTSTATUS NtDelayExecution(BOOLEAN Alertable, PLARGE_INTEGER Interval) */
    { &SsNtDelayExecution, "NtDelayExecution", 2, { 0, Int64Argument } },
    
    { NULL, "Dummy", 0 }
};

RTL_GENERIC_TABLE KphSsCallTable;

VOID KphSsDataInit()
{
    ULONG i;
    
    RtlInitializeGenericTable(
        &KphSsCallTable,
        KphpSsCallEntryCompareRoutine,
        KphpSsCallEntryAllocateRoutine,
        KphpSsCallEntryFreeRoutine,
        NULL
        );
    
    for (i = 0; i < sizeof(SsEntries) / sizeof(KPHSS_CALL_ENTRY); i++)
    {
        /* Ignore the dummy entry. */
        if (SsEntries[i].Number)
        {
            RtlInsertElementGenericTable(
                &KphSsCallTable,
                &SsEntries[i],
                /* Save some space... */
                FIELD_OFFSET(KPHSS_CALL_ENTRY, Arguments) + 
                    SsEntries[i].NumberOfArguments * sizeof(KPHSS_ARGUMENT_TYPE),
                NULL
                );
        }
    }
}

VOID KphSsDataDeinit()
{
    PKPHSS_CALL_ENTRY callEntry;
    
    while (callEntry = (PKPHSS_CALL_ENTRY)RtlGetElementGenericTable(&KphSsCallTable, 0))
        RtlDeleteElementGenericTable(&KphSsCallTable, callEntry);
}

PKPHSS_CALL_ENTRY KphSsLookupCallEntry(
    __in ULONG Number
    )
{
    KPHSS_CALL_ENTRY callEntry;
    
    callEntry.Number = &Number;
    
    return (PKPHSS_CALL_ENTRY)RtlLookupElementGenericTable(
        &KphSsCallTable,
        &callEntry
        );
}

PVOID KphpSsCallEntryAllocateRoutine(
    __in PRTL_GENERIC_TABLE Table,
    __in CLONG ByteSize
    )
{
    return ExAllocatePoolWithTag(
        PagedPool,
        ByteSize,
        TAG_CALL_ENTRY
        );
}

RTL_GENERIC_COMPARE_RESULTS KphpSsCallEntryCompareRoutine(
    __in PRTL_GENERIC_TABLE Table,
    __in PVOID FirstStruct,
    __in PVOID SecondStruct
    )
{
    PKPHSS_CALL_ENTRY callEntry1, callEntry2;
    
    callEntry1 = (PKPHSS_CALL_ENTRY)FirstStruct;
    callEntry2 = (PKPHSS_CALL_ENTRY)SecondStruct;
    
    if (*(callEntry1->Number) < *(callEntry2->Number))
        return GenericLessThan;
    else if (*(callEntry1->Number) > *(callEntry2->Number))
        return GenericGreaterThan;
    else
        return GenericEqual;
}

VOID KphpSsCallEntryFreeRoutine(
    __in PRTL_GENERIC_TABLE Table,
    __in PVOID Buffer
    )
{
    ExFreePoolWithTag(
        Buffer,
        TAG_CALL_ENTRY
        );
}
