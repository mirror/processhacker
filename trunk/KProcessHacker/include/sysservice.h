/*
 * Process Hacker Driver - 
 *   system service logging
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

#ifndef _SYSSERVICE_H
#define _SYSSERVICE_H

#include "kph.h"

/* If neither mode flags are specified, both modes are assumed. */
#define KPHSS_LOG_USER_MODE 0x00000001
#define KPHSS_LOG_KERNEL_MODE 0x00000002
#define KPHSS_LOG_VALID_FLAGS 0x00000003

struct _KPHSS_CLIENT_ENTRY;
typedef struct _KPHSS_CLIENT_ENTRY *PKPHSS_CLIENT_ENTRY;
struct _KPHSS_PROCESS_ENTRY;
typedef struct _KPHSS_PROCESS_ENTRY *PKPHSS_PROCESS_ENTRY;

NTSTATUS KphSsLogInit();
NTSTATUS KphSsLogStart();
NTSTATUS KphSsLogStop();

NTSTATUS KphSsCreateClientEntry(
    __out PKPHSS_CLIENT_ENTRY *ClientEntry,
    __in HANDLE ProcessHandle,
    __in HANDLE ReadSemaphoreHandle,
    __in HANDLE WriteSemaphoreHandle,
    __in PVOID BufferBase,
    __in ULONG BufferSize,
    __in KPROCESSOR_MODE AccessMode
    );

NTSTATUS KphSsCreateProcessEntry(
    __out PKPHSS_PROCESS_ENTRY *ProcessEntry,
    __in PKPHSS_CLIENT_ENTRY ClientEntry,
    __in HANDLE TargetProcessHandle,
    __in ULONG Flags
    );

#endif
