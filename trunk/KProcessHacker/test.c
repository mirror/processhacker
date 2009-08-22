/*
 * Process Hacker Driver - 
 *   testing code
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

#include "include/kph.h"
#include "include/sync.h"

static ULONG RandomSeed;
static EX_PUSH_LOCK TestLock;
static KPH_RESOURCE TestResource;

VOID KphpTestPushLockThreadStart(
    __in PVOID Context
    );

VOID KphpTestResourceThreadStart(
    __in PVOID Context
    );

FORCEINLINE ULONG KphpRandomNumber()
{
    RandomSeed |= (ULONG)KeQueryInterruptTime();
    
    return RtlRandomEx(&RandomSeed);
}

VOID KphTestPushLock()
{
    ULONG i;
    
    ExInitializePushLock(&TestLock);
    
    for (i = 0; i < 10; i++)
    {
        HANDLE threadHandle;
        OBJECT_ATTRIBUTES objectAttributes;
        
        InitializeObjectAttributes(&objectAttributes, NULL, OBJ_KERNEL_HANDLE, NULL, NULL);
        PsCreateSystemThread(&threadHandle, 0, &objectAttributes, NULL, NULL, KphpTestPushLockThreadStart, NULL);
        ZwClose(threadHandle);
    }
}

VOID KphpTestPushLockThreadStart(
    __in PVOID Context
    )
{
    ULONG i, j;
    
    for (i = 0; i < 10000; i++)
    {
        ExAcquirePushLockShared(&TestLock);
        
        for (j = 0; j < 10000; j++)
            YieldProcessor();
        
        ExReleasePushLock(&TestLock);
        
        if (KphpRandomNumber() % 2)
        {
            ExAcquirePushLockExclusive(&TestLock);
            
            for (j = 0; j < 4000; j++)
                YieldProcessor();
            
            ExReleasePushLock(&TestLock);
        }
    }
    
    PsTerminateSystemThread(STATUS_SUCCESS);
}

VOID KphTestResource()
{
    ULONG i;
    
    KphInitializeResource(&TestResource);
    
    for (i = 0; i < 10; i++)
    {
        HANDLE threadHandle;
        OBJECT_ATTRIBUTES objectAttributes;
        
        InitializeObjectAttributes(&objectAttributes, NULL, OBJ_KERNEL_HANDLE, NULL, NULL);
        PsCreateSystemThread(&threadHandle, 0, &objectAttributes, NULL, NULL, KphpTestResourceThreadStart, NULL);
        ZwClose(threadHandle);
    }
}

VOID KphpTestResourceThreadStart(
    __in PVOID Context
    )
{
    ULONG i, j;
    
    for (i = 0; i < 10000; i++)
    {
        KphAcquireResourceShared(&TestResource);
        
        for (j = 0; j < 10000; j++)
            YieldProcessor();
        
        KphReleaseResourceShared(&TestResource);
        
        if (KphpRandomNumber() % 2)
        {
            KphAcquireResourceExclusive(&TestResource);
            
            for (j = 0; j < 4000; j++)
                YieldProcessor();
            
            KphReleaseResourceExclusive(&TestResource);
        }
    }
    
    dfprintf("Thread exit, flags: %#x\n", TestResource.Flags);
    PsTerminateSystemThread(STATUS_SUCCESS);
}
