/*
 * Process Hacker Driver - 
 *   SSDT header file
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

#ifndef _SSDT_H
#define _SSDT_H

#include <ntifs.h>

#define MDL_MAPPED_TO_SYSTEM_VA     0x0001
#define MDL_PAGES_LOCKED            0x0002
#define MDL_SOURCE_IS_NONPAGED_POOL 0x0004
#define MDL_ALLOCATED_FIXED_SIZE    0x0008
#define MDL_PARTIAL                 0x0010
#define MDL_PARTIAL_HAS_BEEN_MAPPED 0x0020
#define MDL_IO_PAGE_READ            0x0040
#define MDL_WRITE_OPERATION         0x0080
#define MDL_PARENT_MAPPED_SYSTEM_VA 0x0100
#define MDL_LOCK_HELD               0x0200
#define MDL_PHYSICAL_VIEW           0x0400
#define MDL_IO_SPACE                0x0800
#define MDL_NETWORK_HEADER          0x1000
#define MDL_MAPPING_CAN_FAIL        0x2000
#define MDL_ALLOCATED_MUST_SUCCEED  0x4000

#pragma pack(1)
typedef struct _SSDT
{
    PVOID *ServiceTableBase;
    PVOID *ServiceCounterTableBase;
    ULONG NumberOfServices;
    UCHAR *ParamTableBase;
} SSDT;
#pragma pack()

#define SYSCALL_INDEX(f) (*(ULONG *)((UCHAR *)(f) + 1))
#define SYSCALL_NT(f) (KeServiceDescriptorTable.ServiceTableBase[SYSCALL_INDEX(f)])

NTSTATUS SsdtInit();
void SsdtDeinit();
ULONG SsdtGetCount();
PVOID SsdtGetEntryByCall(PVOID zwFunction);
PVOID *SsdtGetServiceTable();
PVOID SsdtModifyEntryByCall(PVOID zwFunction, PVOID ntFunction);
PVOID SsdtModifyEntryByIndex(int index, PVOID ntFunction);
void SsdtRestoreEntryByCall(PVOID zwFunction, PVOID oldNtFunction, PVOID newNtFunction);
void SsdtRestoreEntryByIndex(int index, PVOID oldNtFunction, PVOID newNtFunction);

#endif