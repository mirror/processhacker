/*
 * Process Hacker Driver - 
 *   executive
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

#ifndef _EX_H
#define _EX_H

#include "types.h"

struct _HANDLE_TABLE;
struct _HANDLE_TABLE_ENTRY;

typedef BOOLEAN (NTAPI *PEX_ENUM_HANDLE_CALLBACK)(
    struct _HANDLE_TABLE_ENTRY *HandleTableEntry,
    HANDLE Handle,
    PVOID Context
    );

BOOLEAN NTAPI ExEnumHandleTable(
    struct _HANDLE_TABLE *HandleTable,
    PEX_ENUM_HANDLE_CALLBACK EnumHandleProcedure,
    PVOID Context,
    PHANDLE Handle
    );

typedef struct _KGDTENTRY
{
    SHORT LimitLow;
    SHORT BaseLow;
    ULONG HighWord;
} KGDTENTRY, *PKGDTENTRY;

typedef struct _KIDTENTRY
{
    SHORT Offset;
    SHORT Selector;
    SHORT Access;
    SHORT ExtendedOffset;
} KIDTENTRY, *PKIDTENTRY;

typedef struct _EX_FAST_REF
{
    union
    {
        PVOID Object;
        ULONG RefCnt: 3;
        ULONG Value;
    };
} EX_FAST_REF, *PEX_FAST_REF;

typedef struct _EX_PUSH_LOCK2
{
    union
    {
        struct
        {
            ULONG_PTR Locked:1;
            ULONG_PTR Waiting:1;
            ULONG_PTR Waking:1;
            ULONG_PTR MultipleShared:1;
            ULONG_PTR Shared: sizeof(ULONG_PTR) * 8 - 4;
        };
        ULONG_PTR Value;
        PVOID Ptr;
    };
} EX_PUSH_LOCK2, *PEX_PUSH_LOCK2;

#endif