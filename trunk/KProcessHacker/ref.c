/*
 * Process Hacker Driver - 
 *   internal object manager
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

#include "include/refp.h"

/* A list of all objects created by the object manager. */
LIST_ENTRY KphObjectListHead;
/* A mutex protecting global data structures. */
FAST_MUTEX KphObjectListMutex;
/* The object type type. */
PKPH_OBJECT_TYPE KphObjectType;

/* KphRefInit
 * 
 * Initializes the KPH object manager.
 */
NTSTATUS KphRefInit()
{
    NTSTATUS status = STATUS_SUCCESS;
    
    /* Initialize the object list. */
    InitializeListHead(&KphObjectListHead);
    /* Initialize the object list mutex. */
    ExInitializeFastMutex(&KphObjectListMutex);
    
    /* Create the fundamental object type. */
    status = KphCreateObjectType(
        &KphObjectType,
        NonPagedPool,
        NULL
        );
    
    if (!NT_SUCCESS(status))
        return status;
    
    /* Now that the fundamental object type exists, fix it up. */
    KphObjectToObjectHeader(KphObjectType)->Type = KphObjectType;
    KphObjectType->NumberOfObjects = 1;
    
    return status;
}

/* KphRefDeinit
 * 
 * Frees all objets created by the KPH object manager.
 */
NTSTATUS KphRefDeinit()
{
    NTSTATUS status = STATUS_SUCCESS;
    PLIST_ENTRY currentEntry;
    
    /* Acquire the object list mutex to make sure no one else 
     * modifies the list. */
    ExAcquireFastMutex(&KphObjectListMutex);
    
    /* Remove and free all objects in the list. */
    while ((currentEntry = RemoveHeadList(&KphObjectListHead)) != &KphObjectListHead)
    {
        PKPH_OBJECT_HEADER objectHeader = 
            CONTAINING_RECORD(currentEntry, KPH_OBJECT_HEADER, GlobalObjectListEntry);
        
        /* Free the object, ignoring its reference count. */
        KphpFreeObject(objectHeader);
    }
    
    /* Release the object list mutex and restore the IRQL. */
    ExReleaseFastMutex(&KphObjectListMutex);
    
    return STATUS_SUCCESS;
}

/* KphCreateObject
 * 
 * Allocates a object.
 * 
 * Object: A variable which receives a pointer to the newly allocated object.
 * ObjectSize: The size of the object.
 * Flags: A combination of flags specifying how the object is to be allocated.
 *   * KPHOBJ_RAISE_ON_FAIL: An exception will be raised if the object could 
 *     not be allocated.
 *   * KPHOBJ_PAGED_POOL: The object will be allocated in the paged pool. If 
 *     this flag is specified, KPHOBJ_NONPAGED_POOL cannot be specified.
 *   * KPHOBJ_NONPAGED_POOL: The object will be allocated in the non-paged pool. 
 *     If this flag is specified, KPHOBJ_PAGED_POOL cannot be specified.
 * ObjectType: The type of the object.
 * AdditionalReferences: The number of references to add to the object. The 
 * object will have a reference count of 1 + AdditionalReferences.
 */
NTSTATUS KphCreateObject(
    __out PVOID *Object,
    __in SIZE_T ObjectSize,
    __in ULONG Flags,
    __in_opt PKPH_OBJECT_TYPE ObjectType,
    __in_opt LONG AdditionalReferences
    )
{
    PKPH_OBJECT_HEADER objectHeader;
    POOL_TYPE poolType;
    
    /* Check the flags. */
    if ((Flags & KPHOBJ_VALID_FLAGS) != Flags) /* Valid flag mask */
        return STATUS_INVALID_PARAMETER_3;
    if ((Flags & KPHOBJ_PAGED_POOL) && (Flags & KPHOBJ_NONPAGED_POOL)) /* Can't be both pools */
        return STATUS_INVALID_PARAMETER_3;
    /* The object type is only optional if the fundamental object type 
     * hasn't been created. */
    if (!ObjectType && KphObjectType)
        return STATUS_INVALID_PARAMETER_4;
    /* Make sure the additional reference count isn't negative. */
    if (AdditionalReferences < 0)
        return STATUS_INVALID_PARAMETER_5;
    
    /* Figure out the pool type. If it wasn't specified in Flags, 
     * get the pool type from the object type. */
    if (Flags & KPHOBJ_PAGED_POOL)
        poolType = PagedPool;
    else if (Flags & KPHOBJ_NONPAGED_POOL)
        poolType = NonPagedPool;
    else if (ObjectType) /* May be null if we're creating the fundamental type */
        poolType = ObjectType->DefaultPoolType;
    else
        poolType = NonPagedPool;
    
    /* Allocate storage for the object. Note that this includes 
     * the object header followed by the object body. */
    objectHeader = KphpAllocateObject(ObjectSize, poolType);
    
    if (!objectHeader)
    {
        if (Flags & KPHOBJ_RAISE_ON_FAIL)
            ExRaiseStatus(STATUS_INSUFFICIENT_RESOURCES);
        else
            return STATUS_INSUFFICIENT_RESOURCES;
    }
    
    /* Object type statistics. */
    if (ObjectType)
    {
        InterlockedIncrement(&ObjectType->NumberOfObjects);
    }
    
    /* Initialize the object header. */
    KphInitializeGuardedLock(&objectHeader->Lock, FALSE);
    objectHeader->RefCount = 1 + AdditionalReferences;
    objectHeader->Flags = Flags;
    objectHeader->Size = ObjectSize;
    objectHeader->Type = ObjectType;
    
    /* Insert the object into the global object list. */
    ExAcquireFastMutex(&KphObjectListMutex);
    InsertHeadList(&KphObjectListHead, &objectHeader->GlobalObjectListEntry);
    ExReleaseFastMutex(&KphObjectListMutex);
    
    /* Pass a pointer to the object body back to the caller. */
    *Object = KphObjectHeaderToObject(objectHeader);
    
    return STATUS_SUCCESS;
}

/* KphCreateObjectType
 * 
 * Creates an object type.
 */
NTSTATUS KphCreateObjectType(
    __out PKPH_OBJECT_TYPE *ObjectType,
    __in POOL_TYPE DefaultPoolType,
    __in PKPH_TYPE_DELETE_PROCEDURE DeleteProcedure
    )
{
    NTSTATUS status = STATUS_SUCCESS;
    PKPH_OBJECT_TYPE objectType;
    
    /* Create the type object. */
    status = KphCreateObject(
        &objectType,
        sizeof(KPH_OBJECT_TYPE),
        0,
        KphObjectType,
        0
        );
    
    if (!NT_SUCCESS(status))
        return status;
    
    /* Initialize the type object. */
    objectType->DefaultPoolType = DefaultPoolType;
    objectType->DeleteProcedure = DeleteProcedure;
    objectType->NumberOfObjects = 0;
    
    *ObjectType = objectType;
    
    return status;
}

/* KphDereferenceObject
 * 
 * Dereferences the specified object. The object will be freed if 
 * its reference count is 0.
 */
BOOLEAN KphDereferenceObject(
    __in PVOID Object
    )
{
    return KphDereferenceObjectEx(Object, 1, NULL);
}

/* KphDereferenceObjectEx
 * 
 * Dereferences the specified object. The object will be freed if 
 * its reference count is 0.
 */
BOOLEAN KphDereferenceObjectEx(
    __in PVOID Object,
    __in LONG RefCount,
    __out_opt PLONG OldRefCount
    )
{
    PKPH_OBJECT_HEADER objectHeader;
    LONG oldRefCount;
    BOOLEAN freed = FALSE;
    
    /* Make sure we're not subtracting a negative reference count. */
    if (RefCount < 0)
        ExRaiseStatus(STATUS_INVALID_PARAMETER_2);
    
    objectHeader = KphObjectToObjectHeader(Object);
    
    /* We must lock the object to signal to others that the object is 
     * being destroyed. This is useful when our client is using a 
     * linked list to store a list of objects and the delete procedure 
     * removes the object from the linked list. In this case the client 
     * must know when an object is about to be freed (but not freed yet).
     */
    /* Acquire the lock to make sure people don't read the wrong signal state. */
    KphAcquireGuardedLock(&objectHeader->Lock);
    
    /* Decrease the reference count. */
    oldRefCount = InterlockedExchangeAdd(&objectHeader->RefCount, -RefCount);
    
    /* Free the object if it has 0 references. */
    if (oldRefCount - RefCount == 0)
    {
        /* Signal the lock to indicate that we're freeing the object. */
        KphSignalGuardedLock(&objectHeader->Lock);
        /* We don't need the lock anymore. If someone calls dereference 
         * more times than they're supposed to, that's their problem.
         */
        KphReleaseGuardedLock(&objectHeader->Lock);
        
        /* Object type statistics. */
        InterlockedDecrement(&objectHeader->Type->NumberOfObjects);
        
        /* Remove the object from the global object list. */
        ExAcquireFastMutex(&KphObjectListMutex);
        RemoveEntryList(&objectHeader->GlobalObjectListEntry);
        ExReleaseFastMutex(&KphObjectListMutex);
        
        /* Free the object. */
        KphpFreeObject(objectHeader);
        freed = TRUE;
    }
    else
    {
        /* Release the lock (we didn't signal it). */
        KphReleaseGuardedLock(&objectHeader->Lock);
    }
    
    /* Pass the old reference count back. */
    if (OldRefCount)
        *OldRefCount = oldRefCount;
    
    return freed;
}

/* KphGetTypeObject
 * 
 * Gets an object's type.
 */
PKPH_OBJECT_TYPE KphGetTypeObject(
    __in PVOID Object
    )
{
    return KphObjectToObjectHeader(Object)->Type;
}

/* KphIsDestroyedObject
 * 
 * Determines whether the specified object is being 
 * freed (or is freed).
 * 
 * This function is necessary if you store a pointer to 
 * an object without referencing it, you have a mutex 
 * guarding accesses to that pointer, and you remove 
 * that pointer in the object's delete procedure. You 
 * must call this function while you hold that mutex and 
 * before you reference the object for usage, because 
 * the object may be in the process of destruction while 
 * you hold the pointer to it. Note that this is the 
 * only safe way to store a pointer to an object without 
 * referencing it.
 * 
 * For example, you may have a linked list containing 
 * pointers to objects, and all accesses to the list 
 * are protected by a mutex. The delete procedures of 
 * the objects acquire the mutex, remove the object being 
 * deleted from the linked list, and release the mutex.
 * 
 * It may happen that while you are traversing the linked 
 * list with the mutex held, all references to the object 
 * are removed and the object begins to be deleted. Its 
 * delete procedure is called and attempts to acquire the 
 * mutex but blocks because you currently hold the mutex. 
 * Since the destruction process has begun, your only 
 * option is to skip the object. This function gives you 
 * the ability to tell whether you should skip an object 
 * in that scenario.
 */
BOOLEAN KphIsDestroyedObject(
    __in PVOID Object
    )
{
    PKPH_OBJECT_HEADER objectHeader;
    BOOLEAN signaled;
    
    objectHeader = KphObjectToObjectHeader(Object);
    
    /* We have to lock the object so that we don't 
     * read the signal state just after the dereference 
     * routine has decided to free the object but before 
     * it has actually signaled the lock.
     */
    if (KphAcquireSignaledGuardedLock(&objectHeader->Lock))
    {
        /* The lock is signaled. Release and return TRUE. */
        KphReleaseGuardedLock(&objectHeader->Lock);
        return TRUE;
    }
    else
    {
        /* The lock isn't signaled. Return FALSE. */
        return FALSE;
    }
}

/* KphReferenceObject
 * 
 * References the specified object.
 */
VOID KphReferenceObject(
    __in PVOID Object
    )
{
    PKPH_OBJECT_HEADER objectHeader;
    
    objectHeader = KphObjectToObjectHeader(Object);
    /* Increment the reference count. */
    InterlockedIncrement(&objectHeader->RefCount);
}

/* KphReferenceObjectEx
 * 
 * References the specified object.
 */
VOID KphReferenceObjectEx(
    __in PVOID Object,
    __in LONG RefCount,
    __out_opt PLONG OldRefCount
    )
{
    PKPH_OBJECT_HEADER objectHeader;
    LONG oldRefCount;
    
    /* Make sure we're not adding a negative reference count. */
    if (RefCount < 0)
        ExRaiseStatus(STATUS_INVALID_PARAMETER_2);
    
    objectHeader = KphObjectToObjectHeader(Object);
    /* Increase the reference count. */
    oldRefCount = InterlockedExchangeAdd(&objectHeader->RefCount, RefCount);
    
    /* Pass the old reference count back. */
    if (OldRefCount)
        *OldRefCount = oldRefCount;
}

/* KphpAllocateObject
 * 
 * Allocates storage for an object.
 * 
 * ObjectSize: The size of the object, excluding the header.
 * PoolType: The pool in which to allocate the object.
 */
PKPH_OBJECT_HEADER KphpAllocateObject(
    __in SIZE_T ObjectSize,
    __in POOL_TYPE PoolType
    )
{
    return ExAllocatePoolWithTag(
        PoolType,
        KphpAddObjectHeaderSize(ObjectSize),
        TAG_KPHOBJ
        );
}

/* KphpFreeObject
 * 
 * Calls the delete procedure for an object and frees its 
 * allocated storage.
 * 
 * ObjectHeader: A pointer to the object header of an allocated object.
 */
VOID KphpFreeObject(
    __in PKPH_OBJECT_HEADER ObjectHeader
    )
{
    /* Call the delete procedure if we have one. */
    if (ObjectHeader->Type->DeleteProcedure)
    {
        ObjectHeader->Type->DeleteProcedure(
            KphObjectHeaderToObject(ObjectHeader),
            ObjectHeader->Flags,
            ObjectHeader->Size
            );
    }
    
    ExFreePoolWithTag(
        ObjectHeader,
        TAG_KPHOBJ
        );
}
