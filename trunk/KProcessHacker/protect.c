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

/* ProtectedProcessRundownProtect
 * 
 * Rundown protection making sure this module doesn't deinitialize before all hook targets 
 * have finished executing and no one is accessing the lookaside list.
 */
EX_RUNDOWN_REF ProtectedProcessRundownProtect;
/* ProtectedProcessListHead
 * 
 * The head of the process protection linked list. Each entry stores protection 
 * information for a process.
 */
LIST_ENTRY ProtectedProcessListHead;
/* ProtectedProcessListLock
 * 
 * The spinlock which protects all accesses to the protected process list (even 
 * the individual entries)
 */
KSPIN_LOCK ProtectedProcessListLock;
/* ProtectedProcessLookasideList
 * 
 * The lookaside list for protected process entries.
 */
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
    
    /* Initialize rundown protection. */
    ExInitializeRundownProtection(&ProtectedProcessRundownProtect);
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
    LARGE_INTEGER waitLi;
    
    /* Unhook. */
    KphUnhook(&ObOpenObjectByPointerHook);
    
    /* Wait for all activity to finish. */
    ExWaitForRundownProtectionRelease(&ProtectedProcessRundownProtect);
    /* Wait for a bit (some regions of hook target functions 
       are NOT guarded by rundown protection, e.g. 
       prologues and epilogues). */
    waitLi.QuadPart = KPH_REL_TIMEOUT_IN_SEC(1);
    KeDelayExecutionThread(KernelMode, FALSE, &waitLi);
    
    /* Free all process protection entries. */
    ExDeleteNPagedLookasideList(&ProtectedProcessLookasideList);
    
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
    NTSTATUS status = STATUS_SUCCESS;
    PEPROCESS processObject;
    BOOLEAN isThread;
    
    /* Prevent the driver from unloading while this routine is executing. */
    ExAcquireRundownProtection(&ProtectedProcessRundownProtect);
    
    /* It doesn't matter if it isn't actually a process because we won't be 
       dereferencing it. */
    processObject = (PEPROCESS)Object;
    isThread = ObjectType == *PsThreadType;
    
    /* If this is a thread, get its parent process. */
    if (isThread)
        processObject = *(PEPROCESS *)KVOFF(Object, OffKtProcess);
    
    if (
        processObject != PsGetCurrentProcess() /* let the caller open its own processes/threads */
        )
    {
        ACCESS_MASK access;
        KPH_PROCESS_ENTRY processEntry;
        
        *Handle = NULL;
        access = DesiredAccess;
        
        /* If we have an access state, get the desired access from it. */
        if (PassedAccessState != NULL)
            access = PassedAccessState->OriginalDesiredAccess;
        
        if (KphProtectCopyEntry(processObject, &processEntry))
        {
            ACCESS_MASK mask = 
                isThread ? processEntry.ThreadAllowMask : processEntry.ProcessAllowMask;
            
            /* The process/thread is protected. Check if the requested access is allowed. */
            if (
                /* check if kernel-mode is allowed */
                !(processEntry.AllowKernelMode && AccessMode == KernelMode) && 
                (access & mask) != access
                )
            {
                /* Access denied. */
                dprintf(
                    "%d: Access denied: 0x%08x (%s)\n",
                    PsGetCurrentProcessId(),
                    access,
                    isThread ? "Thread" : "Process"
                    );
                ExReleaseRundownProtection(&ProtectedProcessRundownProtect);
                
                return STATUS_ACCESS_DENIED;
            }
        }
    }
    
    status = KphOldObOpenObjectByPointer(
        Object,
        HandleAttributes,
        PassedAccessState,
        DesiredAccess,
        ObjectType,
        AccessMode,
        Handle
        );
    ExReleaseRundownProtection(&ProtectedProcessRundownProtect);
    
    return status;
}

/* KphProtectAddEntry
 * 
 * Protects the specified process.
 */
PKPH_PROCESS_ENTRY KphProtectAddEntry(
    PEPROCESS Process,
    HANDLE Tag,
    LOGICAL AllowKernelMode,
    ACCESS_MASK ProcessAllowMask,
    ACCESS_MASK ThreadAllowMask
    )
{
    KIRQL oldIrql;
    PKPH_PROCESS_ENTRY entry;
    
    /* Prevent the lookaside list from being freed. */
    if (!ExAcquireRundownProtection(&ProtectedProcessRundownProtect))
        return NULL;
    
    entry = ExAllocateFromNPagedLookasideList(&ProtectedProcessLookasideList);
    /* Lookaside list no longer needed. */
    ExReleaseRundownProtection(&ProtectedProcessRundownProtect);
    
    if (entry == NULL)
        return NULL;
    
    entry->Process = Process;
    entry->Tag = Tag;
    entry->AllowKernelMode = AllowKernelMode;
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
    
    /* Prevent the lookaside list from being destroyed. */
    ExAcquireRundownProtection(&ProtectedProcessRundownProtect);
    ExFreeToNPagedLookasideList(
        &ProtectedProcessLookasideList,
        Entry
        );
    ExReleaseRundownProtection(&ProtectedProcessRundownProtect);
    
    KeReleaseSpinLock(&ProtectedProcessListLock, oldIrql);
}
