/*
 * Process Hacker Driver - 
 *   memory manager
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

#ifndef _MM_H
#define _MM_H

#define MI_MAX_TRANSFER_SIZE (0x10000)
#define MI_COPY_STACK_SIZE (0x200)
#define MI_MAPPED_COPY_PAGES (14)
#define MM_POOL_COPY_THRESHOLD (0x1ff)
#define TAG_MM ('PhMm')

/* STRUCTS */
typedef struct _MMADDRESS_NODE
{
    ULONG u1;
    struct _MMADDRESS_NODE *LeftChild;
    struct _MMADDRESS_NODE *RightChild;
    ULONG StartingVpn;
    ULONG EndingVpn;
} MMADDRESS_NODE, *PMMADDRESS_NODE;

typedef struct _MM_AVL_TABLE
{
    MMADDRESS_NODE BalancedRoot;
    ULONG DepthOfTree: 5;
    ULONG Unused: 3;
    ULONG NumberGenericTableElements: 24;
    PVOID NodeHint;
    PVOID NodeFreeHint;
} MM_AVL_TABLE, *PMM_AVL_TABLE;

typedef struct _HARDWARE_PTE
{
    union
    {
        ULONG Valid: 1;
        ULONG Write: 1;
        ULONG Owner: 1;
        ULONG WriteThrough: 1;
        ULONG CacheDisable: 1;
        ULONG Accessed: 1;
        ULONG Dirty: 1;
        ULONG LargePage: 1;
        ULONG Global: 1;
        ULONG CopyOnWrite: 1;
        ULONG Prototype: 1;
        ULONG reserved0: 1;
        ULONG PageFrameNumber: 26;
        ULONG reserved1: 26;
        ULONG LowPart;
    };
    ULONG HighPart;
} HARDWARE_PTE, *PHARDWARE_PTE;

typedef struct _MMSUPPORT_FLAGS
{
    ULONG SessionSpace: 1;
    ULONG ModwriterAttached: 1;
    ULONG TrimHard: 1;
    ULONG MaximumWorkingSetHard: 1;
    ULONG ForceTrim: 1;
    ULONG MinimumWorkingSetHard: 1;
    ULONG SessionMaster: 1;
    ULONG TrimmerAttached: 1;
    ULONG TrimmerDetaching: 1;
    ULONG Reserved: 7;
    ULONG MemoryPriority: 8;
    ULONG WsleDeleted: 1;
    ULONG VmExiting: 1;
    ULONG Available: 6;
} MMSUPPORT_FLAGS, *PMMSUPPORT_FLAGS;

typedef struct _MMSUPPORT
{
    LIST_ENTRY WorkingSetExpansionLinks;
    SHORT LastTrimStamp;
    SHORT NextPageColor;
    MMSUPPORT_FLAGS Flags;
    ULONG PageFaultCount;
    ULONG PeakWorkingSetSize;
    ULONG Spare0;
    ULONG MinimumWorkingSetSize;
    ULONG MaximumWorkingSetSize;
    /* PMMWSL VmWorkingSetList; */
    PVOID VmWorkingSetList;
    ULONG Claim;
    ULONG Spare[1];
    ULONG WorkingSetPrivateSize;
    ULONG WorkingSetSizeOverhead;
    ULONG WorkingSetSize;
    PKEVENT ExitEvent;
    EX_PUSH_LOCK WorkingSetMutex;
    PVOID AccessLog;
} MMSUPPORT, *PMMSUPPORT;

#endif