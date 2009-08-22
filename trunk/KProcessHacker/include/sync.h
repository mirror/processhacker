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

#ifndef _SYNC_H
#define _SYNC_H

#include "kph.h"
#include "ex.h"

/* General synchronization macros */

/* KphEqualSpin
 * 
 * Spins until the first value is equal to the second 
 * value.
 */
FORCEINLINE VOID KphSpinUntilEqual(
    __inout PLONG Value,
    __in LONG Value2
    )
{
    while (InterlockedCompareExchange(
        Value,
        Value2,
        Value2
        ) != Value2)
        YieldProcessor();
}

/* KphNotEqualSpin
 * 
 * Spins until the first value is not equal to the second 
 * value.
 */
FORCEINLINE VOID KphSpinUntilNotEqual(
    __inout PLONG Value,
    __in LONG Value2
    )
{
    while (InterlockedCompareExchange(
        Value,
        Value2,
        Value2
        ) == Value2)
        YieldProcessor();
}

/* Spin Locks */

/* KphAcquireBitSpinLock
 * 
 * Uses the specified bit as a spinlock and acquires the 
 * lock in the given value.
 */
FORCEINLINE VOID KphAcquireBitSpinLock(
    __inout PLONG Value,
    __in LONG Bit
    )
{
    while (InterlockedBitTestAndSet(Value, Bit))
        YieldProcessor();
}

/* KphReleaseBitSpinLock
 * 
 * Uses the specified bit as a spinlock and releases the 
 * lock in the given value.
 */
FORCEINLINE VOID KphReleaseBitSpinLock(
    __inout PLONG Value,
    __in LONG Bit
    )
{
    InterlockedBitTestAndReset(Value, Bit);
}

/* Guarded Locks */
/* Guarded locks are small spinlocks. Code within 
 * synchronized regions run at APC_LEVEL. They also contain 
 * a signal which can used to implement rundown routines.
 */

#define KPH_GUARDED_LOCK_ACTIVE 0x80000000
#define KPH_GUARDED_LOCK_ACTIVE_SHIFT 31
#define KPH_GUARDED_LOCK_SIGNALED 0x40000000
#define KPH_GUARDED_LOCK_SIGNALED_SHIFT 30
#define KPH_GUARDED_LOCK_FLAGS 0xc0000000

typedef struct _KPH_GUARDED_LOCK
{
    LONG Value;
} KPH_GUARDED_LOCK, *PKPH_GUARDED_LOCK;

#define KphAcquireGuardedLock KphfAcquireGuardedLock
VOID FASTCALL KphfAcquireGuardedLock(
    __inout PKPH_GUARDED_LOCK Lock
    );

#define KphReleaseGuardedLock KphfReleaseGuardedLock
VOID FASTCALL KphfReleaseGuardedLock(
    __inout PKPH_GUARDED_LOCK Lock
    );

/* KphInitializeGuardedLock
 * 
 * Initializes a guarded lock.
 * 
 * IRQL: Any
 */
FORCEINLINE VOID KphInitializeGuardedLock(
    __out PKPH_GUARDED_LOCK Lock,
    __in BOOLEAN Signaled
    )
{
    Lock->Value = 0;
    
    if (Signaled)
        Lock->Value |= KPH_GUARDED_LOCK_SIGNALED;
}

/* KphClearGuardedLock
 * 
 * Clears the signal state of a guarded lock, assuming 
 * that the current thread has acquired it.
 * 
 * IRQL: Any
 */
FORCEINLINE VOID KphClearGuardedLock(
    __in PKPH_GUARDED_LOCK Lock
    )
{
    Lock->Value &= ~KPH_GUARDED_LOCK_SIGNALED;
}

/* KphSignalGuardedLock
 * 
 * Signals a guarded lock.
 * 
 * IRQL: Any
 */
FORCEINLINE VOID KphSignalGuardedLock(
    __in PKPH_GUARDED_LOCK Lock
    )
{
    Lock->Value |= KPH_GUARDED_LOCK_SIGNALED;
}

/* KphSignaledGuardedLock
 * 
 * Determines whether a guarded lock is signaled.
 * 
 * IRQL: Any
 */
FORCEINLINE BOOLEAN KphSignaledGuardedLock(
    __in PKPH_GUARDED_LOCK Lock
    )
{
    return !!(Lock->Value & KPH_GUARDED_LOCK_SIGNALED);
}

/* KphAcquireAndClearGuardedLock
 * 
 * Acquires a guarded lock, clear its signal, and raises the IRQL to APC_LEVEL.
 * 
 * IRQL: <= APC_LEVEL
 */
FORCEINLINE VOID KphAcquireAndClearGuardedLock(
    __inout PKPH_GUARDED_LOCK Lock
    )
{
    KphAcquireGuardedLock(Lock);
    KphClearGuardedLock(Lock);
}

/* KphAcquireAndSignalGuardedLock
 * 
 * Acquires a guarded lock, signals it, and raises the IRQL to APC_LEVEL.
 * 
 * IRQL: <= APC_LEVEL
 */
FORCEINLINE VOID KphAcquireAndSignalGuardedLock(
    __inout PKPH_GUARDED_LOCK Lock
    )
{
    KphAcquireGuardedLock(Lock);
    KphSignalGuardedLock(Lock);
}

/* KphAcquireNonSignaledGuardedLock
 * 
 * Acquires a guarded lock and raises the IRQL to APC_LEVEL, 
 * making sure the lock is not signaled. If it is, the 
 * lock is not acquired.
 * 
 * Return value: whether the lock was acquired.
 * IRQL: <= APC_LEVEL
 */
FORCEINLINE BOOLEAN KphAcquireNonSignaledGuardedLock(
    __inout PKPH_GUARDED_LOCK Lock
    )
{
    KphAcquireGuardedLock(Lock);
    
    if (Lock->Value & KPH_GUARDED_LOCK_SIGNALED)
    {
        KphReleaseGuardedLock(Lock);
        return FALSE;
    }
    
    return TRUE;
}

/* KphAcquireSignaledGuardedLock
 * 
 * Acquires a guarded lock and raises the IRQL to APC_LEVEL, 
 * making sure the lock is signaled. If it is not, the 
 * lock is not acquired.
 * 
 * Return value: whether the lock was acquired.
 * IRQL: <= APC_LEVEL
 */
FORCEINLINE BOOLEAN KphAcquireSignaledGuardedLock(
    __inout PKPH_GUARDED_LOCK Lock
    )
{
    KphAcquireGuardedLock(Lock);
    
    if (!(Lock->Value & KPH_GUARDED_LOCK_SIGNALED))
    {
        KphReleaseGuardedLock(Lock);
        return FALSE;
    }
    
    return TRUE;
}

/* KphReleaseAndClearGuardedLock
 * 
 * Releases a guarded lock, clears its signal, and restores the old IRQL.
 * 
 * IRQL: >= APC_LEVEL
 */
FORCEINLINE VOID KphReleaseAndClearGuardedLock(
    __inout PKPH_GUARDED_LOCK Lock
    )
{
    KphClearGuardedLock(Lock);
    KphReleaseGuardedLock(Lock);
}

/* KphReleaseAndSignalGuardedLock
 * 
 * Releases a guarded lock, signals it, and restores the old IRQL.
 * 
 * IRQL: >= APC_LEVEL
 */
FORCEINLINE VOID KphReleaseAndSignalGuardedLock(
    __inout PKPH_GUARDED_LOCK Lock
    )
{
    KphSignalGuardedLock(Lock);
    KphReleaseGuardedLock(Lock);
}

/* Processor Locks */
/* Processor locks prevent code from executing on all other 
 * processors. Code within synchronized regions run at 
 * DISPATCH_LEVEL.
 */

#define TAG_SYNC_DPC ('DShP')

typedef struct _KPH_PROCESSOR_LOCK
{
    /* Synchronizes access to the processor lock. */
    KPH_GUARDED_LOCK Lock;
    /* Storage allocated for DPCs. */
    PKDPC Dpcs;
    /* The number of currently acquired processors. */
    LONG AcquiredProcessors;
    /* The signal for acquired processors to be released. */
    LONG ReleaseSignal;
    /* The old IRQL. */
    KIRQL OldIrql;
    /* Whether the processor lock has been acquired. */
    BOOLEAN Acquired;
} KPH_PROCESSOR_LOCK, *PKPH_PROCESSOR_LOCK;

BOOLEAN KphAcquireProcessorLock(
    __inout PKPH_PROCESSOR_LOCK ProcessorLock
    );

VOID KphInitializeProcessorLock(
    __out PKPH_PROCESSOR_LOCK ProcessorLock
    );

VOID KphReleaseProcessorLock(
    __inout PKPH_PROCESSOR_LOCK ProcessorLock
    );

/* Resource Locks */
/* Resource locks are reader-writer locks. Speed is emphasized 
 * over lock storage size. Writers are preferred, which means 
 * that as soon as a writer is placed on the waiter list, 
 * new readers block instead of acquiring the lock immediately.
 */

#define KPH_RESOURCE_LOCKED 0x00000001
#define KPH_RESOURCE_LOCKED_SHIFT 0
#define KPH_RESOURCE_WAKING 0x00000002
#define KPH_RESOURCE_WAKING_SHIFT 1
#define KPH_RESOURCE_WAITERS 0x00000004
#define KPH_RESOURCE_WAITERS_SHIFT 2
#define KPH_RESOURCE_EXCLUSIVE_WAITERS 0x00000008
#define KPH_RESOURCE_EXCLUSIVE_WAITERS_SHIFT 3
#define KPH_RESOURCE_SHARED_INC 0x00000010
#define KPH_RESOURCE_SHARED_SHIFT 4

#define KPH_RESOURCE_LIST_LOCKED 0x00000001
#define KPH_RESOURCE_LIST_LOCKED_SHIFT 0

typedef struct _KPH_RESOURCE
{
    /* Flags:
     *  * KPH_RESOURCE_LOCKED: Locked for exclusive or shared access.
     *  * KPH_RESOURCE_WAKING: A waiter has just been woken up. This is a signal to 
     *    acquirers who have just woken up that they can acquire the lock even 
     *    thought it says it is locked already.
     *  * KPH_RESOURCE_WAITERS: There are waiters in the waiter list. Used so that 
     *    the fast paths in this very file do not screw up things by not waking up 
     *    waiters when they should.
     *  * KPH_RESOURCE_EXCLUSIVE_WAITERS: There are exclusive waiters in the waiter 
     *    list. Used for exclusive waiter biasing.
     *  * KPH_RESOURCE_SHARED_INC: Add to increase the number of shared holders.
     */
    ULONG Flags;
    ULONG WaiterListLock;
    LIST_ENTRY WaiterListHead;
} KPH_RESOURCE, *PKPH_RESOURCE;

FORCEINLINE VOID KphInitializeResource(
    __out PKPH_RESOURCE Resource
    )
{
    Resource->Flags = 0;
    Resource->WaiterListLock = 0;
    InitializeListHead(&Resource->WaiterListHead);
}

#define KPH_RESOURCE_ACQUIRE_LIST_LOCK(Resource) \
    KphAcquireBitSpinLock(&Resource->WaiterListLock, KPH_RESOURCE_LIST_LOCKED_SHIFT)
#define KPH_RESOURCE_RELEASE_LIST_LOCK(Resource) \
    KphReleaseBitSpinLock(&Resource->WaiterListLock, KPH_RESOURCE_LIST_LOCKED_SHIFT)

#define KPH_RESOURCE_WAIT_BLOCK(ListEntry) \
    CONTAINING_RECORD(ListEntry, KPH_RESOURCE_WAIT_BLOCK, WaiterListEntry)

#define KPH_RESOURCE_EXCLUSIVE 0x00000001
#define KPH_RESOURCE_SHARED 0x00000002
#define KPH_RESOURCE_WOKEN 0x00000004

typedef struct _KPH_RESOURCE_WAIT_BLOCK
{
    LIST_ENTRY WaiterListEntry;
    ULONG Flags;
    KEVENT WakeEvent;
} KPH_RESOURCE_WAIT_BLOCK, *PKPH_RESOURCE_WAIT_BLOCK;

/* KphInitializeResourceWaitBlock
 * 
 * Initializes a resource wait block.
 */
FORCEINLINE VOID KphInitializeResourceWaitBlock(
    __out PKPH_RESOURCE_WAIT_BLOCK WaitBlock,
    __in ULONG Flags
    )
{
    WaitBlock->Flags = Flags;
    KeInitializeEvent(&WaitBlock->WakeEvent, NotificationEvent, FALSE);
}

/* KphWaitForResourceWaitBlock
 * 
 * Waits for a wait block to be unblocked.
 */
FORCEINLINE VOID KphWaitForResourceWaitBlock(
    __inout PKPH_RESOURCE_WAIT_BLOCK WaitBlock
    )
{
    KeWaitForSingleObject(
        &WaitBlock->WakeEvent,
        Executive,
        KernelMode,
        FALSE,
        NULL
        );
}

/* KphWaitForResourceWaitBlockEx
 * 
 * Waits for a wait block to be unblocked.
 */
FORCEINLINE VOID KphWaitForResourceWaitBlockEx(
    __inout PKPH_RESOURCE_WAIT_BLOCK WaitBlock,
    __in PLARGE_INTEGER Timeout
    )
{
    KeWaitForSingleObject(
        &WaitBlock->WakeEvent,
        Executive,
        KernelMode,
        FALSE,
        Timeout
        );
}

/* KphWakeResourceWaitBlock
 * 
 * Wakes a waiter specified by a wait block.
 */
FORCEINLINE VOID KphWakeResourceWaitBlock(
    __in PKPH_RESOURCE_WAIT_BLOCK WaitBlock
    )
{
    KeSetEvent(&WaitBlock->WakeEvent, 2, FALSE);
}

VOID FASTCALL KphfAcquireResourceExclusive(
    __inout PKPH_RESOURCE Resource
    );

VOID FASTCALL KphfAcquireResourceShared(
    __inout PKPH_RESOURCE Resource
    );

VOID KphBlockResource(
    __inout PKPH_RESOURCE Resource,
    __inout PKPH_RESOURCE_WAIT_BLOCK WaitBlock
    );

VOID FASTCALL KphfReleaseResourceExclusive(
    __inout PKPH_RESOURCE Resource
    );

VOID FASTCALL KphfReleaseResourceShared(
    __inout PKPH_RESOURCE Resource
    );

PKPH_RESOURCE_WAIT_BLOCK KphRemoveResourceWaitBlock(
    __inout PKPH_RESOURCE Resource
    );

VOID KphUnblockResource(
    __inout PKPH_RESOURCE Resource
    );

/* KphAcquireResourceExclusive
 * 
 * Acquires a resource lock for exclusive access.
 * 
 * IRQL: <= APC_LEVEL
 */
FORCEINLINE VOID KphAcquireResourceExclusive(
    __inout PKPH_RESOURCE Resource
    )
{
    /* Fast path. */
    if (InterlockedCompareExchange(
        &Resource->Flags,
        KPH_RESOURCE_LOCKED,
        0
        ) != 0)
    {
        /* Slow path. */
        KphfAcquireResourceExclusive(Resource);
    }
}

/* KphAcquireResourceShared
 * 
 * Acquires a resource lock for shared access.
 * 
 * IRQL: <= APC_LEVEL
 */
FORCEINLINE VOID KphAcquireResourceShared(
    __inout PKPH_RESOURCE Resource
    )
{
    /* Fast path. */
    if (InterlockedCompareExchange(
        &Resource->Flags,
        KPH_RESOURCE_LOCKED + KPH_RESOURCE_SHARED_INC,
        0
        ) != 0)
    {
        /* Slow path. */
        KphfAcquireResourceShared(Resource);
    }
}

/* KphReleaseResourceExclusive
 * 
 * Releases a resource previously acquired for exclusive access.
 * 
 * IRQL: <= APC_LEVEL
 */
FORCEINLINE VOID KphReleaseResourceExclusive(
    __inout PKPH_RESOURCE Resource
    )
{
    /* Fast path. */
    if (InterlockedCompareExchange(
        &Resource->Flags,
        0,
        KPH_RESOURCE_LOCKED
        ) != KPH_RESOURCE_LOCKED)
    {
        /* Slow path. */
        KphfReleaseResourceExclusive(Resource);
    }
}

/* KphReleaseResourceShared
 * 
 * Releases a resource previously acquired for shared access.
 * 
 * IRQL: <= APC_LEVEL
 */
FORCEINLINE VOID KphReleaseResourceShared(
    __inout PKPH_RESOURCE Resource
    )
{
    ULONG flags;
    
    flags = Resource->Flags;
    
    /* Fast path. */
    if (
        (flags & KPH_RESOURCE_WAITERS) || 
        (flags >> KPH_RESOURCE_SHARED_SHIFT == 1) || 
        InterlockedCompareExchange(
            &Resource->Flags,
            flags - KPH_RESOURCE_SHARED_INC,
            flags
            ) != flags
        )
    {
        /* Slow path. */
        KphfReleaseResourceShared(Resource);
    }
}

#endif
