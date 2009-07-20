/*
 * Process Hacker Driver - 
 *   handle table
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

#ifndef _HANDLE_H
#define _HANDLE_H

#include "kprocesshacker.h"
#include "ref.h"

typedef struct _KPH_HANDLE_TABLE
{
    /* The pool tag used for this descriptor and the table itself. */
    ULONG Tag;
    /* The size of each handle table entry. */
    ULONG SizeOfEntry;
    /* The next handle value to use. */
    HANDLE NextHandle;
    /* The free list of handle table entries. */
    struct _KPH_HANDLE_TABLE_ENTRY *FreeHandle;
    
    /* A fast mutex guarding writes to the handle table. */
    FAST_MUTEX Mutex;
    /* The size of the table, in bytes. */
    ULONG TableSize;
    /* The actual handle table. */
    PVOID Table;
} KPH_HANDLE_TABLE, *PKPH_HANDLE_TABLE;

typedef struct _KPH_HANDLE_TABLE_ENTRY
{
    union
    {
        HANDLE Handle;
        ULONG_PTR Value;
        struct _KPH_HANDLE_TABLE_ENTRY *NextFree;
    };
    PVOID Object;
} KPH_HANDLE_TABLE_ENTRY, *PKPH_HANDLE_TABLE_ENTRY;

NTSTATUS KphCreateHandleTable(
    __out PKPH_HANDLE_TABLE *HandleTable,
    __in ULONG MaximumHandles,
    __in ULONG SizeOfEntry,
    __in ULONG Tag
    );

NTSTATUS KphFreeHandleTable(
    __in PKPH_HANDLE_TABLE HandleTable
    );

NTSTATUS KphCloseHandle(
    __in PKPH_HANDLE_TABLE HandleTable,
    __in HANDLE Handle
    );

NTSTATUS KphCreateHandle(
    __in PKPH_HANDLE_TABLE HandleTable,
    __in PVOID Object,
    __out HANDLE *Handle
    );

NTSTATUS KphReferenceObjectByHandle(
    __in PKPH_HANDLE_TABLE HandleTable,
    __in HANDLE Handle,
    __out PVOID *Object
    );

BOOLEAN KphValidHandle(
    __in PKPH_HANDLE_TABLE HandleTable,
    __in HANDLE Handle,
    __out_opt PKPH_HANDLE_TABLE_ENTRY *Entry
    );

#endif
