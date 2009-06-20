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
    __in struct _HANDLE_TABLE *HandleTable,
    __in PEX_ENUM_HANDLE_CALLBACK EnumHandleProcedure,
    __inout PVOID Context,
    __out_opt PHANDLE Handle
    );

typedef NTSTATUS (NTAPI *_ExpGetProcessInformation)(
    PVOID Buffer,
    ULONG BufferLength,
    PULONG ReturnLength,
    PULONG SessionId, /* NULL to include all processes */
    BOOLEAN ExtendedInformation
    );

#endif