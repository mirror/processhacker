/*
 * Process Hacker Driver - 
 *   process protection
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

#include "include/protect.h"

LIST_ENTRY ProtectedProcessListHead;
KSPIN_LOCK ProtectedProcessListLock;
NPAGED_LOOKASIDE_LIST ProtectedProcessLookasideList;

KPH_HOOK ObOpenObjectByPointerHook = { 0 };

BOOLEAN KphpIsCurrentProcessProtected();

VOID KphpProtectRemoveEntry(
    PKPH_PROCESS_ENTRY Entry
    );

KPH_DEFINE_HOOK_CALL(
    NTSTATUS NTAPI KphOldObOpenObjectByPointer,
    OBOPENOBJECTBYPOINTER_ARGS,
    ObOpenObjectByPointerHook
    );

/* KphProtectInit
 * 
 * Initializes process protection.
 */
NTSTATUS KphProtectInit()
{
    NTSTATUS status;
    
    /* Initialize list structures. */
    InitializeListHead(&ProtectedProcessListHead);
    KeInitializeSpinLock(&ProtectedProcessListLock);
    ExInitializeNPagedLookasideList(
        &ProtectedProcessLookasideList,
        NULL,
        NULL,
        0,
        sizeof(KPH_PROCESS_ENTRY),
        KPH_TAG,
        0
        );
    
    /* Hook various functions. */
    ObOpenObjectByPointerHook.Function = ObOpenObjectByPointer;
    ObOpenObjectByPointerHook.Target = KphNewObOpenObjectByPointer;
    if (!NT_SUCCESS(status = KphHook(&ObOpenObjectByPointerHook)))
        return status;
    
    return STATUS_SUCCESS;
}

/* KphProtectDeinit
 * 
 * Removes process protection and frees associated structures.
 */
NTSTATUS KphProtectDeinit()
{
    KIRQL oldIrql;
    
    /* Unhook. */
    KphUnhook(&ObOpenObjectByPointerHook);
    
    KeAcquireSpinLock(&ProtectedProcessListLock, &oldIrql);
    
    /* Free and remove all process entries from the list. */
    while (!IsListEmpty(&ProtectedProcessListHead))
    {
        PKPH_PROCESS_ENTRY entry = 
            CONTAINING_RECORD(
                RemoveHeadList(&ProtectedProcessListHead),
                KPH_PROCESS_ENTRY, 
                ListEntry
            );
        
        dprintf("KphProtectDeinit: removing entry 0x%08x\n", entry);
        
        ExFreeToNPagedLookasideList(
            &ProtectedProcessLookasideList,
            entry
            );
    }
    
    KeReleaseSpinLock(&ProtectedProcessListLock, oldIrql);
    
    return STATUS_SUCCESS;
}

/* KphNewObOpenObjectByPointer
 * 
 * New ObOpenObjectByPointer function.
 */
NTSTATUS NTAPI KphNewObOpenObjectByPointer(
    OBOPENOBJECTBYPOINTER_ARGS
    )
{
    if (
        AccessMode != KernelMode && /* let kernel-mode callers through */
        !KphpIsCurrentProcessProtected() /* let protected process callers through */
        )
    {
        PEPROCESS processObject = (PEPROCESS)Object;
        BOOLEAN isThread = ObjectType == *PsThreadType;
        ACCESS_MASK access;
        KPH_PROCESS_ENTRY processEntry;
        
        *Handle = NULL;
        access = DesiredAccess;
        
        /* If we have an access state, get the desired access from it. */
        if (PassedAccessState != NULL)
            access = PassedAccessState->OriginalDesiredAccess;
        
        /* If this is a thread, get its parent process. */
        if (isThread)
            processObject = *(PEPROCESS *)KVOFF(Object, OffKtProcess);
        
        if (KphProtectCopyEntry(processObject, &processEntry))
        {
            ACCESS_MASK mask = 
                isThread ? processEntry.ThreadAllowMask : processEntry.ProcessAllowMask;
            
            /* The process/thread is protected. Check if the requested access is allowed. */
            if ((access & mask) != access)
            {
                dprintf(
                    "%d: Access denied: 0x%08x (%s)\n",
                    PsGetCurrentProcessId(),
                    access,
                    isThread ? "Thread" : "Process"
                    );
                /* Access denied. */
                return STATUS_ACCESS_DENIED;
            }
        }
    }
    
    return KphOldObOpenObjectByPointer(
        Object,
        HandleAttributes,
        PassedAccessState,
        DesiredAccess,
        ObjectType,
        AccessMode,
        Handle
        );
}

/* KphProtectAddEntry
 * 
 * Protects the specified process.
 */
PKPH_PROCESS_ENTRY KphProtectAddEntry(
    PEPROCESS Process,
    HANDLE Tag,
    ACCESS_MASK ProcessAllowMask,
    ACCESS_MASK ThreadAllowMask
    )
{
    KIRQL oldIrql;
    PKPH_PROCESS_ENTRY entry = 
        ExAllocateFromNPagedLookasideList(&ProtectedProcessLookasideList);
    
    if (entry == NULL)
        return NULL;
    
    entry->Process = Process;
    entry->Tag = Tag;
    entry->ProcessAllowMask = ProcessAllowMask;
    entry->ThreadAllowMask = ThreadAllowMask;
    
    KeAcquireSpinLock(&ProtectedProcessListLock, &oldIrql);
    InsertHeadList(&ProtectedProcessListHead, &entry->ListEntry);
    KeReleaseSpinLock(&ProtectedProcessListLock, oldIrql);
    
    return entry;
}

/* KphProtectCopyEntry
 * 
 * Copies process protection data for the specified process.
 */
BOOLEAN KphProtectCopyEntry(
    PEPROCESS Process,
    PKPH_PROCESS_ENTRY ProcessEntry
    )
{
    KIRQL oldIrql;
    PLIST_ENTRY entry = ProtectedProcessListHead.Flink;
    
    KeAcquireSpinLock(&ProtectedProcessListLock, &oldIrql);
    
    while (entry != &ProtectedProcessListHead)
    {
        PKPH_PROCESS_ENTRY processEntry = 
            CONTAINING_RECORD(entry, KPH_PROCESS_ENTRY, ListEntry);
        
        if (processEntry->Process == Process)
        {
            memcpy(ProcessEntry, processEntry, sizeof(KPH_PROCESS_ENTRY));
            KeReleaseSpinLock(&ProtectedProcessListLock, oldIrql);
            
            return TRUE;
        }
        
        entry = entry->Flink;
    }
    
    KeReleaseSpinLock(&ProtectedProcessListLock, oldIrql);
    
    return FALSE;
}

/* KphProtectFindEntry
 * 
 * Finds process protection data.
 */
PKPH_PROCESS_ENTRY KphProtectFindEntry(
    PEPROCESS Process,
    HANDLE Tag
    )
{
    KIRQL oldIrql;
    PLIST_ENTRY entry = ProtectedProcessListHead.Flink;
    
    KeAcquireSpinLock(&ProtectedProcessListLock, &oldIrql);
    
    while (entry != &ProtectedProcessListHead)
    {
        PKPH_PROCESS_ENTRY processEntry = 
            CONTAINING_RECORD(entry, KPH_PROCESS_ENTRY, ListEntry);
        
        if (
            (Process != NULL && processEntry->Process == Process) || 
            (Tag != NULL && processEntry->Tag == Tag)
            )
        {
            KeReleaseSpinLock(&ProtectedProcessListLock, oldIrql);
            
            return processEntry;
        }
        
        entry = entry->Flink;
    }
    
    KeReleaseSpinLock(&ProtectedProcessListLock, oldIrql);
    
    return NULL;
}

/* KphProtectRemoveByProcess
 * 
 * Removes protection from the specified process.
 */
BOOLEAN KphProtectRemoveByProcess(
    PEPROCESS Process
    )
{
    PKPH_PROCESS_ENTRY entry = KphProtectFindEntry(Process, NULL);
    
    if (entry == NULL)
        return FALSE;
    
    KphpProtectRemoveEntry(entry);
    
    return TRUE;
}

/* KphProtectRemoveByTag
 * 
 * Removes protection from all processes with the specified tag.
 */
ULONG KphProtectRemoveByTag(
    HANDLE Tag
    )
{
    KIRQL oldIrql;
    ULONG count = 0;
    PKPH_PROCESS_ENTRY entry;
    
    /* Keep removing entries until we can't find any more. */
    while (entry = KphProtectFindEntry(NULL, Tag))
    {
        KphpProtectRemoveEntry(entry);
        count++;
    }
    
    return count;
}

/* KphpIsCurrentProcessProtected
 * 
 * Determines whether the current process is protected.
 */
BOOLEAN KphpIsCurrentProcessProtected()
{
    return KphProtectFindEntry(PsGetCurrentProcess(), NULL) != NULL;
}

/* KphpProtectRemoveEntry
 * 
 * Removes and frees process protection data.
 */
VOID KphpProtectRemoveEntry(
    PKPH_PROCESS_ENTRY Entry
    )
{
    KIRQL oldIrql;
    
    KeAcquireSpinLock(&ProtectedProcessListLock, &oldIrql);
    RemoveEntryList(&Entry->ListEntry);
    ExFreeToNPagedLookasideList(
        &ProtectedProcessLookasideList,
        Entry
        );
    KeReleaseSpinLock(&ProtectedProcessListLock, oldIrql);
}
