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

#ifndef _PROCESS_H
#define _PROCESS_H

#include "nph.h"
#include <psapi.h>

typedef enum _WS_INFORMATION_CLASS
{
    WsCount = 0,
    WsPrivateCount,
    WsSharedCount,
    WsShareableCount,
    WsAllCounts
} WS_INFORMATION_CLASS, *PWS_INFORMATION_CLASS;

typedef struct _WS_ALL_COUNTS
{
    ULONG Count;
    ULONG PrivateCount;
    ULONG SharedCount;
    ULONG ShareableCount;
} WS_ALL_COUNTS, *PWS_ALL_COUNTS;

NPHAPI NTSTATUS PHAPI PhQueryProcessWs(
    HANDLE ProcessHandle,
    WS_INFORMATION_CLASS WsInformationClass,
    PVOID WsInformation,
    ULONG WsInformationLength,
    PULONG ReturnLength
    );

#endif