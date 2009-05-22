/*
 * Process Hacker Driver - 
 *   object manager
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

#ifndef _OB_H
#define _OB_H

#include "types.h"
#include "ex.h"

#define OBJECT_TO_OBJECT_HEADER(o) \
    CONTAINING_RECORD((o), OBJECT_HEADER, Body)

#define OBJ_PROTECT_CLOSE 0x00000001L
#define OBJ_INHERIT 0x00000002L
#define OBJ_AUDIT_OBJECT_CLOSE 0x00000004L
#define OBJ_HANDLE_ATTRIBUTES (OBJ_PROTECT_CLOSE | OBJ_INHERIT | OBJ_AUDIT_OBJECT_CLOSE)

/* FUNCTION DEFS */

struct _OBJECT_HANDLE_FLAG_INFORMATION;
typedef struct _OBJECT_TYPE_INITIALIZER OBJECT_TYPE_INITIALIZER, *POBJECT_TYPE_INITIALIZER;

NTSTATUS NTAPI ObCreateObjectType(
    PUNICODE_STRING TypeName,
    POBJECT_TYPE_INITIALIZER ObjectTypeInitializer,
    PSECURITY_DESCRIPTOR SecurityDescriptor,
    POBJECT_TYPE *ObjectType
    );

NTSTATUS NTAPI ObOpenObjectByName(
    POBJECT_ATTRIBUTES ObjectAttributes,
    POBJECT_TYPE ObjectType,
    KPROCESSOR_MODE PreviousMode,
    PACCESS_STATE AccessState,
    ACCESS_MASK DesiredAccess,
    PVOID ParseContext,
    PHANDLE Handle
    );

NTSTATUS NTAPI ObSetHandleAttributes(
    HANDLE Handle,
    struct _OBJECT_HANDLE_FLAG_INFORMATION *HandleFlags,
    KPROCESSOR_MODE PreviousMode
    );

/* FUNCTION TYPEDEFS */
enum _OB_OPEN_REASON;

typedef NTSTATUS (NTAPI *OB_OPEN_METHOD_51)(
    enum _OB_OPEN_REASON OpenReason,
    PEPROCESS Process,
    PVOID Object,
    ACCESS_MASK GrantedAccess,
    ULONG HandleCount
    );

typedef NTSTATUS (NTAPI *OB_OPEN_METHOD_60)(
    enum _OB_OPEN_REASON OpenReason,
    KPROCESSOR_MODE AccessMode,
    PEPROCESS Process,
    PVOID Object,
    ACCESS_MASK GrantedAccess,
    ULONG HandleCount
    );

/* ENUMS */
typedef enum _OB_OPEN_REASON
{
    ObCreateHandle,
    ObOpenHandle,
    ObDuplicateHandle,
    ObInheritHandle,
    ObMaxOpenReason
} OB_OPEN_REASON, *POB_OPEN_REASON;

/* STRUCTS */

typedef struct _OBP_SET_HANDLE_GRANTED_ACCESS_DATA
{
    HANDLE Handle;
    ACCESS_MASK GrantedAccess;
} OBP_SET_HANDLE_GRANTED_ACCESS_DATA, *POBP_SET_HANDLE_GRANTED_ACCESS_DATA;

typedef struct _OBJECT_HANDLE_FLAG_INFORMATION
{
    BOOLEAN Inherit;
    BOOLEAN ProtectFromClose;
} OBJECT_HANDLE_FLAG_INFORMATION, *POBJECT_HANDLE_FLAG_INFORMATION;

typedef struct _OBJECT_CREATE_INFORMATION OBJECT_CREATE_INFORMATION, *POBJECT_CREATE_INFORMATION;

typedef struct _OBJECT_HEADER
{
    LONG PointerCount;
    union
    {
        LONG HandleCount;
        PVOID NextToFree;
    };
    POBJECT_TYPE Type;
    UCHAR NameInfoOffset;
    UCHAR HandleInfoOffset;
    UCHAR QuotaInfoOffset;
    UCHAR Flags;
    union
    {
        POBJECT_CREATE_INFORMATION ObjectCreateInfo;
        PVOID QuotaBlockCharged;
    };
    PVOID SecurityDescriptor;
    QUAD Body;
} OBJECT_HEADER, *POBJECT_HEADER;

typedef struct _HANDLE_TABLE_ENTRY
{
    union
    {
        PVOID Object;
        ULONG Value;
    };
    ULONG GrantedAccess;
} HANDLE_TABLE_ENTRY, *PHANDLE_TABLE_ENTRY;

typedef struct _HANDLE_TABLE HANDLE_TABLE, *PHANDLE_TABLE;

#endif
