/*
 * Process Hacker Library
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

#ifndef _OBJ_H
#define _OBJ_H

#include "nph.h"
#include "nativedefs.h"

typedef struct _PH_QUERY_FILE_OBJECT_BUFFER
{
    LOGICAL Initialized;
    NTSTATUS Status;
    ULONG Length;
    ULONG ReturnLength;
    POBJECT_NAME_INFORMATION Name;
} PH_QUERY_FILE_OBJECT_BUFFER, *PPH_QUERY_FILE_OBJECT_BUFFER;

NTSTATUS PHAPI PhObjInit();

NPHAPI NTSTATUS PHAPI PhQueryNameFileObject(
    HANDLE FileHandle,
    POBJECT_NAME_INFORMATION FileObjectNameInformation,
    ULONG FileObjectNameInformationLength,
    PULONG ReturnLength
    );

#endif
