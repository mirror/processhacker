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
 * its reference count reaches 0.
 * 
 * Object: A pointer to the object to dereference.
 * 
 * Return value: TRUE if the object was freed, otherwise FALSE.
 */
BOOLEAN KphDereferenceObject(
    __in PVOID Object
    )
{
    return KphDereferenceObjectEx(Object, 1) == 0;
}

/* KphDereferenceObjectEx
 * 
 * Dereferences the specified object. The object will be freed if 
 * its reference count reaches 0.
 * 
 * Object: A pointer to the object to dereference.
 * RefCount: The number of references to remove.
 * 
 * Return value: The new reference count of the object.
 */
LONG KphDereferenceObjectEx(
    __in PVOID Object,
    __in LONG RefCount
    )
{
    PKPH_OBJECT_HEADER objectHeader;
    LONG oldRefCount;
    
    /* Make sure we're not subtracting a negative reference count. */
    if (RefCount < 0)
        ExRaiseStatus(STATUS_INVALID_PARAMETER_2);
    
    objectHeader = KphObjectToObjectHeader(Object);
    
    /* Decrease the reference count. */
    oldRefCount = InterlockedExchangeAdd(&objectHeader->RefCount, -RefCount);
    
    /* Free the object if it has 0 references. */
    if (oldRefCount - RefCount == 0)
    {
        /* Remove the object from the global object list. */
        ExAcquireFastMutex(&KphObjectListMutex);
        RemoveEntryList(&objectHeader->GlobalObjectListEntry);
        ExReleaseFastMutex(&KphObjectListMutex);
        
        /* Free the object. */
        KphpFreeObject(objectHeader);
    }
    
    return oldRefCount - RefCount;
}

/* KphGetObjectType
 * 
 * Gets an object's type.
 */
PKPH_OBJECT_TYPE KphGetObjectType(
    __in PVOID Object
    )
{
    return KphObjectToObjectHeader(Object)->Type;
}

/* KphReferenceObject
 * 
 * References the specified object.
 * 
 * Object: A pointer to the object to reference.
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
 * 
 * Object: A pointer to the object to reference.
 * RefCount: The number of references to add.
 * 
 * Return value: The new reference count of the object.
 */
LONG KphReferenceObjectEx(
    __in PVOID Object,
    __in LONG RefCount
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
    
    return oldRefCount + RefCount;
}

/* KphReferenceObjectSafe
 * 
 * Attempts to reference an object and fails if it is being 
 * destroyed.
 * 
 * Object: The object to reference if it is not being deleted.
 * 
 * Return value: TRUE if the object was referenced, FALSE if 
 * it was being deleted and was not referenced.
 * 
 * Remarks:
 * This function is useful if a reference to an object is 
 * held, protected by a mutex, and the delete procedure of 
 * the object's type attempts to acquire the mutex. If this 
 * function is called while the mutex is owned, you can 
 * avoid referencing an object that is being destroyed.
 */
BOOLEAN KphReferenceObjectSafe(
    __in PVOID Object
    )
{
    PKPH_OBJECT_HEADER objectHeader;
    BOOLEAN result;
    
    objectHeader = KphObjectToObjectHeader(Object);
    /* Increase the reference count only if it isn't 0 (atomically). */
    result = KphpInterlockedIncrementSafe(&objectHeader->RefCount);
    
    return result;
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
    /* Object type statistics. */
    InterlockedDecrement(&ObjectHeader->Type->NumberOfObjects);
    
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
