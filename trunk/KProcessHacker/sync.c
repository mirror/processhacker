/*
 * Process Hacker Driver - 
 *   synchronization code
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

#include "include/sync.h"
#include "include/debug.h"

VOID KphpProcessorLockDpc(
    __in PKDPC Dpc,
    __in PVOID DeferredContext,
    __in PVOID SystemArgument1,
    __in PVOID SystemArgument2
    );

FORCEINLINE BOOLEAN KphpIsExclusiveWaitNeededForResource(
    __in ULONG Flags
    )
{
    return (Flags & KPH_RESOURCE_LOCKED) || (Flags & KPH_RESOURCE_WAKING);
}

FORCEINLINE BOOLEAN KphpIsSharedWaitNeededForResource(
    __in ULONG Flags
    )
{
    return ((Flags & KPH_RESOURCE_LOCKED) && (Flags >> KPH_RESOURCE_SHARED_SHIFT == 0)) || 
        (Flags & KPH_RESOURCE_WAKING) || 
        (Flags & KPH_RESOURCE_EXCLUSIVE_WAITERS);
}

FORCEINLINE VOID KphpReleaseResource(
    __inout PKPH_RESOURCE Resource
    )
{
    ULONG flags;
    PKPH_RESOURCE_WAIT_BLOCK waitBlock;
    
    /* Remove a single waiter from the list. */
    KPH_RESOURCE_ACQUIRE_LIST_LOCK(Resource);
    waitBlock = KphRemoveResourceWaitBlock(Resource);
    KPH_RESOURCE_RELEASE_LIST_LOCK(Resource);
    
    /* If we have a waiter, set the waking flag and 
     * wake the waiter. Otherwise, unset the lock bit.
     */
    if (waitBlock)
    {
        while (TRUE)
        {
            flags = Resource->Flags;
            
            if (InterlockedCompareExchange(
                &Resource->Flags,
                (flags & ~KPH_RESOURCE_LOCKED) | KPH_RESOURCE_WAKING,
                flags
                ) == flags)
                break;
        }
        
        waitBlock->Flags |= KPH_RESOURCE_WOKEN;
        KphWakeResourceWaitBlock(waitBlock);
    }
    else
    {
        InterlockedBitTestAndReset(&Resource->Flags, KPH_RESOURCE_LOCKED_SHIFT);
    }
}

/* KphfAcquireGuardedLock
 * 
 * Acquires a guarded lock and raises the IRQL to APC_LEVEL.
 * 
 * IRQL: <= APC_LEVEL
 */
VOID FASTCALL KphfAcquireGuardedLock(
    __inout PKPH_GUARDED_LOCK Lock
    )
{
    KIRQL oldIrql;
    
    ASSERT(KeGetCurrentIrql() <= APC_LEVEL);
    
    /* Raise to APC_LEVEL. */
    oldIrql = KeRaiseIrql(APC_LEVEL, &oldIrql);
    
    /* Acquire the spinlock. */
    KphAcquireBitSpinLock(&Lock->Value, KPH_GUARDED_LOCK_ACTIVE_SHIFT);
    
    /* Now that we have the lock, we must save the old IRQL. */
    /* Clear the old IRQL. */
    Lock->Value &= KPH_GUARDED_LOCK_FLAGS;
    /* Set the new IRQL. */
    Lock->Value |= oldIrql;
}

/* KphfReleaseGuardedLock
 * 
 * Releases a guarded lock and restores the old IRQL.
 * 
 * IRQL: >= APC_LEVEL
 */
VOID FASTCALL KphfReleaseGuardedLock(
    __inout PKPH_GUARDED_LOCK Lock
    )
{
    KIRQL oldIrql;
    
    ASSERT(KeGetCurrentIrql() >= APC_LEVEL);
    
    /* Get the old IRQL. */
    oldIrql = (KIRQL)(Lock->Value & ~KPH_GUARDED_LOCK_FLAGS);
    /* Unlock the spinlock. */
    KphReleaseBitSpinLock(&Lock->Value, KPH_GUARDED_LOCK_ACTIVE_SHIFT);
    /* Restore the old IRQL. */
    KeLowerIrql(oldIrql);
}

/* KphAcquireProcessorLock
 * 
 * Raises the IRQL to DISPATCH_LEVEL and prevents threads from 
 * executing on other processors until the processor lock is released. 
 * Blocks if the supplied processor lock is already in use.
 * 
 * ProcessorLock: A processor lock structure that is present in 
 * non-paged memory.
 * 
 * Comments:
 * Here is how the processor lock works:
 *  1. Tries to acquire the mutex in the processor lock, and 
 *     blocks until it can be obtained.
 *  2. Initializes a DPC for each processor on the computer.
 *  3. Raises the IRQL to DISPATCH_LEVEL to make sure the 
 *     code is not interrupted by a context switch.
 *  4. Queues each of the previously-initialized DPCs, except if 
 *     it is targeted at the current processor.
 *  5. Since DPCs run at DISPATCH_LEVEL, they have exclusive 
 *     control of the processor. As each runs, they increment 
 *     a counter in the processor lock. They then enter a loop.
 *  6. The routine waits for the counter to become n - 1, 
 *     signaling that all (other) processors have been acquired 
 *     (where n is the number of processors).
 *  7. It returns. Any code from here will be running in 
 *     DISPATCH_LEVEL and will be the only code running on the 
 *     machine.
 * Thread safety: Full
 * IRQL: <= APC_LEVEL
 */
BOOLEAN KphAcquireProcessorLock(
    __inout PKPH_PROCESSOR_LOCK ProcessorLock
    )
{
    ULONG i;
    ULONG numberProcessors;
    ULONG currentProcessor;
    
    /* Acquire the processor lock guarded lock. */
    KphAcquireGuardedLock(&ProcessorLock->Lock);
    
    /* Reset some state. */
    ASSERT(ProcessorLock->AcquiredProcessors == 0);
    ProcessorLock->AcquiredProcessors = 0;
    ProcessorLock->ReleaseSignal = 0; /* IMPORTANT */
    
    /* Get the number of processors. */
    numberProcessors = KphCountBits(KeQueryActiveProcessors());
    
    /* If there's only one processor we can simply raise the IRQL and exit. */
    if (numberProcessors == 1)
    {
        dprintf("KphAcquireProcessorLock: Only one processor, raising IRQL and exiting...\n");
        KeRaiseIrql(DISPATCH_LEVEL, &ProcessorLock->OldIrql);
        ProcessorLock->Acquired = TRUE;
        
        return TRUE;
    }
    
    /* Allocate storage for the DPCs. */
    ProcessorLock->Dpcs = ExAllocatePoolWithTag(
        NonPagedPool,
        sizeof(KDPC) * numberProcessors,
        TAG_SYNC_DPC
        );
    
    if (!ProcessorLock->Dpcs)
    {
        dprintf("KphAcquireProcessorLock: Could not allocate storage for DPCs!\n");
        KphReleaseGuardedLock(&ProcessorLock->Lock);
        return FALSE;
    }
    
    /* Initialize the DPCs. */
    for (i = 0; i < numberProcessors; i++)
    {
        KeInitializeDpc(&ProcessorLock->Dpcs[i], KphpProcessorLockDpc, NULL);
        KeSetTargetProcessorDpc(&ProcessorLock->Dpcs[i], (CCHAR)i);
        KeSetImportanceDpc(&ProcessorLock->Dpcs[i], HighImportance);
    }
    
    /* Raise the IRQL to DISPATCH_LEVEL to prevent context switching. */
    KeRaiseIrql(DISPATCH_LEVEL, &ProcessorLock->OldIrql);
    /* Get the current processor number. */
    currentProcessor = KeGetCurrentProcessorNumber();
    
    /* Queue the DPCs (except on the current processor). */
    for (i = 0; i < numberProcessors; i++)
        if (i != currentProcessor)
            KeInsertQueueDpc(&ProcessorLock->Dpcs[i], ProcessorLock, NULL);
    
    /* Spinwait for all (other) processors to be acquired. */
    KphSpinUntilEqual(&ProcessorLock->AcquiredProcessors, numberProcessors - 1);
    
    dprintf("KphAcquireProcessorLock: All processors acquired.\n");
    ProcessorLock->Acquired = TRUE;
    
    return TRUE;
}

/* KphInitializeProcessorLock
 * 
 * Initializes a processor lock.
 * 
 * ProcessorLock: A processor lock structure that is present in 
 * non-paged memory.
 * 
 * IRQL: Any
 */
VOID KphInitializeProcessorLock(
    __out PKPH_PROCESSOR_LOCK ProcessorLock
    )
{
    KphInitializeGuardedLock(&ProcessorLock->Lock, FALSE);
    ProcessorLock->Dpcs = NULL;
    ProcessorLock->AcquiredProcessors = 0;
    ProcessorLock->ReleaseSignal = 0;
    ProcessorLock->OldIrql = PASSIVE_LEVEL;
    ProcessorLock->Acquired = FALSE;
}

/* KphReleaseProcessorLock
 * 
 * Allows threads to execute on other processors and restores the IRQL.
 * 
 * ProcessorLock: A processor lock structure that is present in 
 * non-paged memory.
 * 
 * Comments:
 * Here is how the processor lock is released:
 *  1. Sets the signal to release the processors. The DPCs that are 
 *     currently waiting for the signal will return and decrement 
 *     the acquired processors counter.
 *  2. Waits for the acquired processors counter to become zero.
 *  3. Restores the old IRQL. This will always be APC_LEVEL due to 
 *     the mutex.
 *  4. Frees the storage allocated for the DPCs.
 *  5. Releases the processor lock mutex. This will restore the IRQL 
 *     back to normal.
 * Thread safety: Full
 * IRQL: DISPATCH_LEVEL
 */
VOID KphReleaseProcessorLock(
    __inout PKPH_PROCESSOR_LOCK ProcessorLock
    )
{
    if (!ProcessorLock->Acquired)
        return;
    
    /* Signal for the acquired processors to be released. */
    InterlockedExchange(&ProcessorLock->ReleaseSignal, 1);
    
    /* Spinwait for all acquired processors to be released. */
    KphSpinUntilEqual(&ProcessorLock->AcquiredProcessors, 0);
    
    dprintf("KphReleaseProcessorLock: All processors released.\n");
    
    /* Restore the old IRQL (should always be APC_LEVEL due to the 
     * fast mutex). */
    KeLowerIrql(ProcessorLock->OldIrql);
    
    /* Free the DPCs if necessary. */
    if (ProcessorLock->Dpcs != NULL)
    {
        ExFreePoolWithTag(ProcessorLock->Dpcs, TAG_SYNC_DPC);
        ProcessorLock->Dpcs = NULL;
    }
    
    ProcessorLock->Acquired = FALSE;
    
    /* Release the processor lock guarded lock. This will restore the 
     * IRQL back to what it was before the processor lock was 
     * acquired.
     */
    KphReleaseGuardedLock(&ProcessorLock->Lock);
}

/* KphpProcessorLockDpc
 * 
 * The DPC routine which "locks" processors.
 * 
 * Thread safety: Full
 * IRQL: DISPATCH_LEVEL
 */
VOID KphpProcessorLockDpc(
    __in PKDPC Dpc,
    __in PVOID DeferredContext,
    __in PVOID SystemArgument1,
    __in PVOID SystemArgument2
    )
{
    PKPH_PROCESSOR_LOCK processorLock = (PKPH_PROCESSOR_LOCK)SystemArgument1;
    
    ASSERT(processorLock != NULL);
    
    dprintf("KphpProcessorLockDpc: Acquiring processor %d.\n", KeGetCurrentProcessorNumber());
    
    /* Increase the number of acquired processors. */
    InterlockedIncrement(&processorLock->AcquiredProcessors);
    
    /* Spin until we get the signal to release the processor. */
    KphSpinUntilNotEqual(&processorLock->ReleaseSignal, 0);
    
    /* Decrease the number of acquired processors. */
    InterlockedDecrement(&processorLock->AcquiredProcessors);
    
    dprintf("KphpProcessorLockDpc: Releasing processor %d.\n", KeGetCurrentProcessorNumber());
}

/* KphfAcquireResourceExclusive
 * 
 * Acquires a resource lock for exclusive access.
 * 
 * IRQL: <= APC_LEVEL
 */
VOID FASTCALL KphfAcquireResourceExclusive(
    __inout PKPH_RESOURCE Resource
    )
{
    ULONG flags;
    KPH_RESOURCE_WAIT_BLOCK waitBlock;
    
    while (TRUE)
    {
        flags = Resource->Flags;
        
        /* Check if the resource is held or if someone is waking up. */
        if (KphpIsExclusiveWaitNeededForResource(flags))
        {
            /* Locked for exclusive or shared access. */
            KphInitializeResourceWaitBlock(&waitBlock, KPH_RESOURCE_EXCLUSIVE);
            
            /* Add our wait block to the waiters list (while making sure 
             * we actually need to wait - in essence this is double checking).
             */
            
            KPH_RESOURCE_ACQUIRE_LIST_LOCK(Resource);
            
            if (!KphpIsExclusiveWaitNeededForResource(Resource->Flags))
            {
                KPH_RESOURCE_RELEASE_LIST_LOCK(Resource);
                continue;
            }
            
            KphBlockResource(Resource, &waitBlock);
            
            KPH_RESOURCE_RELEASE_LIST_LOCK(Resource);
            
            KphWaitForResourceWaitBlock(&waitBlock);
            
            /* Did we actually get unblocked? If so, unset the waking flag and 
             * acquire the resource.
             */
            if (waitBlock.Flags & KPH_RESOURCE_WOKEN)
            {
                InterlockedBitTestAndSet(&Resource->Flags, KPH_RESOURCE_LOCKED_SHIFT);
                
                return;
            }
        }
        else
        {
            /* The resource isn't being held. Try to lock it for exclusive access 
             * immediately.
             */
            if (InterlockedCompareExchange(
                &Resource->Flags,
                flags | KPH_RESOURCE_LOCKED,
                flags
                ) == flags)
            {
                /* Success. */
                break;
            }
            
            /* Someone changed the state of the variable. 
             * Go back and try again.
             */
        }
    }
}

/* KphfAcquireResourceShared
 * 
 * Acquires a resource lock for shared access.
 * 
 * IRQL: <= APC_LEVEL
 */
VOID FASTCALL KphfAcquireResourceShared(
    __inout PKPH_RESOURCE Resource
    )
{
    ULONG flags;
    KPH_RESOURCE_WAIT_BLOCK waitBlock;
    
    while (TRUE)
    {
        flags = Resource->Flags;
        
        /* Check if the resource is held exclusively (i.e. the resource is 
         * locked and there are no shared holders), and block. We also need 
         * to block if someone is waking up. For exclusive waiter biasing, 
         * we also need to block if there are exclusive waiters.
         */
        if (KphpIsSharedWaitNeededForResource(flags))
        {
            /* Locked for exclusive access. */
            KphInitializeResourceWaitBlock(&waitBlock, KPH_RESOURCE_SHARED);
            
            /* Add our wait block to the waiters list (while making sure 
             * we actually need to wait).
             */
            
            KPH_RESOURCE_ACQUIRE_LIST_LOCK(Resource);
            
            if (!KphpIsSharedWaitNeededForResource(Resource->Flags))
            {
                KPH_RESOURCE_RELEASE_LIST_LOCK(Resource);
                continue;
            }
            
            KphBlockResource(Resource, &waitBlock);
            
            KPH_RESOURCE_RELEASE_LIST_LOCK(Resource);
            
            KphWaitForResourceWaitBlock(&waitBlock);
            
            /* Did we actually get unblocked? If so, unset the waking flag and 
             * acquire the resource.
             */
            if (waitBlock.Flags & KPH_RESOURCE_WOKEN)
            {
                while (TRUE)
                {
                    flags = Resource->Flags;
                    
                    if (InterlockedCompareExchange(
                        &Resource->Flags,
                        (flags & ~KPH_RESOURCE_WAKING) | (KPH_RESOURCE_LOCKED + KPH_RESOURCE_SHARED_INC),
                        flags
                        ) == flags)
                        break;
                }
                
                return;
            }
        }
        else
        {
            /* The resource isn't being held. Try to lock it for shared access 
             * immediately.
             */
            if (InterlockedCompareExchange(
                &Resource->Flags,
                (flags + KPH_RESOURCE_SHARED_INC) | KPH_RESOURCE_LOCKED,
                flags
                ) == flags)
            {
                /* Success. */
                break;
            }
            
            /* Someone changed the state of the variable. 
             * Go back and try again.
             */
        }
    }
}

/* KphBlockResource
 * 
 * Adds a resource wait block to the waiters list of a resource.
 * 
 * IRQL: <= APC_LEVEL
 */
VOID KphBlockResource(
    __inout PKPH_RESOURCE Resource,
    __inout PKPH_RESOURCE_WAIT_BLOCK WaitBlock
    )
{
    PLIST_ENTRY currentListEntry;
    
    /* Set the waiters flag. */
    InterlockedBitTestAndSet(&Resource->Flags, KPH_RESOURCE_WAITERS_SHIFT);
    
    /* Decide on how to insert our wait block depending on 
     * type of waiter.
     * 
     * If we're a shared waiter, insert our wait block at the end.
     * If we're an exclusive waiter, insert our wait block after 
     * all other exclusive waiters (but before all shared waiters).
     * If the waiter type wasn't specified, insert our wait block 
     * at the end.
     */
    if (WaitBlock->Flags & KPH_RESOURCE_SHARED)
    {
        /* Insert our wait block at the end of the list. */
        InsertTailList(&Resource->WaiterListHead, &WaitBlock->WaiterListEntry);
    }
    else if (WaitBlock->Flags & KPH_RESOURCE_EXCLUSIVE)
    {
        /* Set the exclusive waiters flag. */
        InterlockedBitTestAndSet(&Resource->Flags, KPH_RESOURCE_EXCLUSIVE_WAITERS_SHIFT);
        
        /* Search backwards for the first exclusive wait block and 
         * insert our wait block after it.
         */
        
        currentListEntry = Resource->WaiterListHead.Blink;
        
        while (currentListEntry != &Resource->WaiterListHead)
        {
            PKPH_RESOURCE_WAIT_BLOCK currentWaitBlock = 
                KPH_RESOURCE_WAIT_BLOCK(currentListEntry);
            
            if (currentWaitBlock->Flags & KPH_RESOURCE_EXCLUSIVE)
                break;
            
            currentListEntry = currentListEntry->Blink;
        }
        
        InsertHeadList(currentListEntry, &WaitBlock->WaiterListEntry);
    }
    else
    {
        /* Insert our wait block at the end of the list. */
        InsertTailList(&Resource->WaiterListHead, &WaitBlock->WaiterListEntry);
    }
}

/* KphfReleaseResourceExclusive
 * 
 * Releases a resource previously acquired for exclusive access.
 * 
 * IRQL: <= APC_LEVEL
 */
VOID FASTCALL KphfReleaseResourceExclusive(
    __inout PKPH_RESOURCE Resource
    )
{
    /* Release the resource. */
    KphpReleaseResource(Resource);
}

/* KphfReleaseResourceShared
 * 
 * Releases a resource previously acquired for shared access.
 * 
 * IRQL: <= APC_LEVEL
 */
VOID FASTCALL KphfReleaseResourceShared(
    __inout PKPH_RESOURCE Resource
    )
{
    ULONG oldFlags;
    
    /* Lower the shared holder count. */
    oldFlags = InterlockedExchangeAdd(&Resource->Flags, -KPH_RESOURCE_SHARED_INC);
    
    /* If we were the last shared holder, release the resource. */
    if (oldFlags >> KPH_RESOURCE_SHARED_SHIFT == 1)
    {
        KphpReleaseResource(Resource);
    }
}

/* KphRemoveResourceWaitBlock
 * 
 * Removes a single wait block from a resource.
 * 
 * IRQL: <= DISPATCH_LEVEL
 */
PKPH_RESOURCE_WAIT_BLOCK KphRemoveResourceWaitBlock(
    __inout PKPH_RESOURCE Resource
    )
{
    PKPH_RESOURCE_WAIT_BLOCK waitBlock;
    
    /* Check if we have a waiter. */
    if (Resource->WaiterListHead.Flink == &Resource->WaiterListHead)
        return NULL;
    
    /* Get the wait block and remove it from the waiter list. */
    waitBlock = KPH_RESOURCE_WAIT_BLOCK(Resource->WaiterListHead.Flink);
    RemoveEntryList(Resource->WaiterListHead.Flink);
    
    /* Check if the waiter list is empty and unset the waiters flag. If 
     * the list isn't empty, check if we need to unset the exclusive 
     * waiters flag.
     */
    if (IsListEmpty(&Resource->WaiterListHead))
    {
        InterlockedAnd(&Resource->Flags, ~(KPH_RESOURCE_EXCLUSIVE_WAITERS | KPH_RESOURCE_WAITERS));
    }
    else
    {
        /* If the first wait block isn't an exclusive one, we can assume that there are no 
         * exclusive waiters.
         */
        if (!(KPH_RESOURCE_WAIT_BLOCK(Resource->WaiterListHead.Flink)->Flags & KPH_RESOURCE_EXCLUSIVE))
        {
            InterlockedBitTestAndReset(&Resource->Flags, KPH_RESOURCE_EXCLUSIVE_WAITERS_SHIFT);
        }
    }
    
    return waitBlock;
}

/* KphUnblockResource
 * 
 * Unblocks all waiters blocking on a resource.
 * 
 * IRQL: <= DISPATCH_LEVEL
 */
VOID KphUnblockResource(
    __inout PKPH_RESOURCE Resource
    )
{
    KIRQL oldIrql;
    PLIST_ENTRY currentListEntry;
    
    /* Check if we have at least one waiter to wake. */
    if (Resource->WaiterListHead.Flink == &Resource->WaiterListHead)
        return;
    
    currentListEntry = Resource->WaiterListHead.Flink;
    
    /* Raise IRQL to DISPATCH_LEVEL to prevent rescheduling while 
     * waking multiple waiters.
     */
    KeRaiseIrql(DISPATCH_LEVEL, &oldIrql);
    
    while (currentListEntry != &Resource->WaiterListHead)
    {
        PKPH_RESOURCE_WAIT_BLOCK waitBlock;
        
        waitBlock = KPH_RESOURCE_WAIT_BLOCK(currentListEntry);
        
        /* Wake the waiter. */
        KphWakeResourceWaitBlock(waitBlock);
        
        currentListEntry = currentListEntry->Flink;
    }
    
    /* Restore the old IRQL. */
    KeLowerIrql(oldIrql);
    
    /* Empty the waiter list. */
    InitializeListHead(&Resource->WaiterListHead);
    InterlockedAnd(&Resource->Flags, ~(KPH_RESOURCE_EXCLUSIVE_WAITERS | KPH_RESOURCE_WAITERS));
}
