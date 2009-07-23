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

#ifndef _REFP_H
#define _REFP_H

#include "ref.h"
#include "sync.h"

#define TAG_KPHOBJ ('bOhP')

#define KphObjectToObjectHeader(Object) ((PKPH_OBJECT_HEADER)CONTAINING_RECORD((PCHAR)(Object), KPH_OBJECT_HEADER, Body))
#define KphObjectHeaderToObject(ObjectHeader) (&((PKPH_OBJECT_HEADER)(ObjectHeader))->Body)
#define KphpAddObjectHeaderSize(Size) ((Size) + sizeof(KPH_OBJECT_HEADER) - sizeof(ULONG))

typedef struct _KPH_OBJECT_HEADER
{
    /* A lock protecting the object. This guarded lock is signaled 
     * when the object is being destroyed, and is never unsignaled.
     */
    KPH_GUARDED_LOCK Lock;
    /* The reference count of the object. */
    LONG RefCount;
    /* The flags that were used to create the object. */
    ULONG Flags;
    /* The size of the object, excluding the header. */
    SIZE_T Size;
    /* The type of the object. */
    PKPH_OBJECT_TYPE Type;
    /* A linked list entry for an optional object manager object list. 
     * For example, this may be used to free all objects when the 
     * driver exits.
     */
    LIST_ENTRY GlobalObjectListEntry;
    
    /* The body of the object. For use by the KphObject(Header)ToObject(Header) macros. */
    ULONG Body;
} KPH_OBJECT_HEADER, *PKPH_OBJECT_HEADER;

PKPH_OBJECT_HEADER KphpAllocateObject(
    __in SIZE_T ObjectSize,
    __in POOL_TYPE PoolType
    );

VOID KphpFreeObject(
    __in PKPH_OBJECT_HEADER ObjectHeader
    );

#endif
