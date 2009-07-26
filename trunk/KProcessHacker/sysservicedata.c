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

KPHSS_CALL_ENTRY SsEntries[] =
{
    /* NTSTATUS NtAddAtom(PWSTR String, ULONG StringLength, PUSHORT Atom) */
    { &SsNtAddAtom, "NtAddAtom", 3, { WStringArgument, 0, Int16Argument } },
    /* NTSTATUS NtAlertResumeThread(HANDLE ThreadHandle, PULONG PreviousSuspendCount) */
    { &SsNtAlertResumeThread, "NtAlertResumeThread", 2, { HandleArgument, 0 } },
    /* NTSTATUS NtAlertThread(HANDLE ThreadHandle) */
    { &SsNtAlertThread, "NtAlertThread", 1, { HandleArgument } },
    /* NTSTATUS NtAllocateLocallyUniqueId(PLUID Luid) */
    { &SsNtAllocateLocallyUniqueId, "NtAllocateLocallyUniqueId", 1, { 0 } },
    /* NTSTATUS NtAllocateUserPhysicalPages(HANDLE ProcessHandle, PULONG NumberOfPages, PULONG PageFrameNumbers) */
    { &SsNtAllocateUserPhysicalPages, "NtAllocateUserPhysicalPages", 3, { HandleArgument, Int32Argument, 0 } },
    /* NTSTATUS NtAllocateUuids(PLARGE_INTEGER UuidLastTimeAllocated, PULONG UuidDeltaTime, PULONG UuidSequenceNumber, 
     *      PUCHAR UuidSeed)
     */
    { &SsNtAllocateUuids, "NtAllocateUuids", 4, { Int64Argument, 0, 0, 0 } },
    /* NTSTATUS NtAllocateVirtualMemory(HANDLE ProcessHandle, PVOID *BaseAddress, ULONG ZeroBits, 
     *      PULONG AllocationSize, ULONG AllocationType, ULONG Protect)
     */
    { &SsNtAllocateVirtualMemory, "NtAllocateVirtualMemory", 6, { HandleArgument, Int32Argument, 0, Int32Argument, 0, 0 } },
    /* NTSTATUS NtApphelpCacheControl(APPHELPCACHECONTROL ApphelpCacheControl, PUNICODE_STRING ApphelpCacheObject) */
    { &SsNtApphelpCacheControl, "NtApphelpCacheControl", 2, { 0, UnicodeStringArgument } },
    /* NTSTATUS NtAreMappedFilesTheSame(PVOID Address1, PVOID Address2) */
    { &SsNtAreMappedFilesTheSame, "NtAreMappedFilesTheSame", 2, { 0, 0 } },
    /* NTSTATUS NtAssignProcessToJobObject(HANDLE JobHandle, HANDLE ProcessHandle) */
    { &SsNtAssignProcessToJobObject, "NtAssignProcessToJobObject", 2, { HandleArgument, HandleArgument } },
    /* NTSTATUS NtCallbackReturn(PVOID Result, ULONG ResultLength, NTSTATUS Status) */
    { &SsNtCallbackReturn, "NtCallbackReturn", 3, { 0, 0, 0 } },
    /* NTSTATUS NtCancelDeviceWakeupRequest(HANDLE DeviceHandle) */
    { &SsNtCancelDeviceWakeupRequest, "NtCancelDeviceWakeupRequest", 1, { HandleArgument } },
    /* NTSTATUS NtCancelIoFile(HANDLE FileHandle, PIO_STATUS_BLOCK IoStatusBlock) */
    { &SsNtCancelIoFile, "NtCancelIoFile", 2, { HandleArgument, 0 } },
    /* NTSTATUS NtCancelTimer(HANDLE TimerHandle, PBOOLEAN CurrentState) */
    { &SsNtCancelTimer, "NtCancelTimer", 2, { HandleArgument, 0 } },
    /* NTSTATUS NtClearEvent(HANDLE EventHandle) */
    { &SsNtClearEvent, "NtClearEvent", 1, { HandleArgument } },
    /* NTSTATUS NtClose(HANDLE Handle) */
    { &SsNtClose, "NtClose", 1, { HandleArgument } },
    /* NTSTATUS NtContinue(PCONTEXT Context, BOOLEAN TestAlert) */
    { &SsNtContinue, "NtContinue", 2, { ContextArgument, 0 } },
    /* NTSTATUS NtCreateFile(PHANDLE FileHandle, ACCESS_MASK DesiredAccess, POBJECT_ATTRIBUTES ObjectAttributes, 
     *      PIO_STATUS_BLOCK IoStatusBlock, PLARGE_INTEGER AllocationSize, ULONG FileAttributes, 
     *      ULONG ShareAccess, ULONG CreateDisposition, ULONG CreateOptions, 
     *      PVOID EaBuffer, ULONG EaLength)
     */
    { &SsNtCreateFile, "NtCreateFile", 11, { 0, 0, ObjectAttributesArgument, 0, Int64Argument, 0, 0, 0, 0, 0, 0 } },
    /* NTSTATUS NtDelayExecution(BOOLEAN Alertable, PLARGE_INTEGER Interval) */
    { &SsNtDelayExecution, "NtDelayExecution", 2, { 0, Int64Argument } },
    /* NTSTATUS NtLoadDriver(PUNICODE_STRING DriverServiceName) */
    { &SsNtLoadDriver, "NtLoadDriver", 1, { UnicodeStringArgument } },
    /* NTSTATUS NtOpenDirectoryObject(PHANDLE DirectoryHandle, ACCESS_MASK DesiredAccess, POBJECT_ATTRIBUTES ObjectAttributes) */
    { &SsNtOpenDirectoryObject, "NtOpenDirectoryObject", 3, { 0, 0, ObjectAttributesArgument } },
    
    { NULL, "Dummy", 0 }
};

RTL_GENERIC_TABLE KphSsCallTable;
FAST_MUTEX KphSsCallTableMutex;

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
    
    ExInitializeFastMutex(&KphSsCallTableMutex);
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
    PKPHSS_CALL_ENTRY foundEntry;
    
    callEntry.Number = &Number;
    
    ExAcquireFastMutex(&KphSsCallTableMutex);
    foundEntry = (PKPHSS_CALL_ENTRY)RtlLookupElementGenericTable(
        &KphSsCallTable,
        &callEntry
        );
    ExReleaseFastMutex(&KphSsCallTableMutex);
    
    return foundEntry;
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
