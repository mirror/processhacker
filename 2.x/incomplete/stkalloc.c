/*
 * Process Hacker - 
 *   stack allocator
 * 
 * Copyright (C) 2010 wj32
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

#include <phbase.h>

VOID PhInitializeStackAlloc(
    __out PPH_STACKALLOC StackAlloc
    )
{
    InitializeListHead(&StackAlloc->SegmentListHead);
    StackAlloc->CurrentSegment = NULL;

    StackAlloc->SegmentCount = 0;
    StackAlloc->AllocatedCount = 0;
}

VOID PhDeleteStackAlloc(
    __inout PPH_STACKALLOC StackAlloc
    )
{
    PhFreeStackAlloc(StackAlloc);
}

PVOID PhAllocateStackAlloc(
    __inout PPH_STACKALLOC StackAlloc,
    __in SIZE_T Size
    )
{
    PPH_STACKALLOC_SEGMENT segment;

    if (!StackAlloc->CurrentSegment)
    {
        // Create an initial segment.
        segment = PhpCreateStackAllocSegment(
            StackAlloc,
            PAGE_SIZE,
            PAGE_SIZE * 8,
            FALSE
            );

        if (!segment)
            PhRaiseStatus(STATUS_NO_MEMORY);

        StackAlloc->CurrentSegment = segment;
    }

    // If this allocation is above the threshold, give it its own segment.
    if (Size >= PH_STACKALLOC_LARGE_THRESHOLD)
    {
        segment = PhpCreateStackAllocSegment(
            StackAlloc,
            Size + sizeof(PH_STACKALLOC_SEGMENT),
            0,
            TRUE
            );

        return (PCHAR)segment + sizeof(PH_STACKALLOC_SEGMENT);
    }

    segment = StackAlloc->CurrentSegment;

    // If this allocation is too large, try to extend the current segment.
    
}

VOID PhFreeStackAlloc(
    __inout PPH_STACKALLOC StackAlloc
    )
{
    PLIST_ENTRY listEntry;
    PPH_STACKALLOC_SEGMENT segment;
    PVOID baseAddress;
    SIZE_T regionSize;

    listEntry = StackAlloc->SegmentListHead.Flink;

    while (listEntry != &StackAlloc->SegmentListHead)
    {
        segment = CONTAINING_RECORD(listEntry, PH_STACKALLOC_SEGMENT, ListEntry);

        baseAddress = segment;
        regionSize = 0;
        NtFreeVirtualMemory(NtCurrentProcess(), &baseAddress, &regionSize, MEM_RELEASE);

        listEntry = listEntry->Flink;
    }
}

PPH_STACKALLOC_SEGMENT PhpCreateStackAllocSegment(
    __inout PPH_STACKALLOC StackAlloc,
    __in SIZE_T CommitSize,
    __in_opt SIZE_T ReserveSize,
    __in BOOLEAN StrictSize
    )
{
    NTSTATUS status;
    PVOID reservedBase;
    SIZE_T reserveSize;
    SIZE_T commitSize;
    PPH_STACKALLOC_SEGMENT segment;

    if (!ReserveSize)
        ReserveSize = CommitSize;

    if (ReserveSize < PAGE_SIZE)
    {
        ReserveSize = PAGE_SIZE;
        CommitSize = PAGE_SIZE;
    }
    else if (CommitSize > ReserveSize)
    {
        CommitSize = ReserveSize;
    }

    if (StrictSize)
    {
        if (CommitSize != ReserveSize)
        {
            reserveSize = ReserveSize;
            reservedBase = NULL;
            status = NtAllocateVirtualMemory(
                NtCurrentProcess(),
                &reservedBase,
                0,
                &reserveSize,
                MEM_RESERVE,
                PAGE_READWRITE
                );

            if (!NT_SUCCESS(status))
                return NULL;
        }
        else
        {
            // Just do the commit, because there's nothing else to reserve.
            reserveSize = CommitSize;
            reservedBase = NULL;
        }

        commitSize = CommitSize;
        committedBase = reservedBase;
        status = NtAllocateVirtualMemory(
            NtCurrentProcess(),
            &committedBase,
            0,
            &commitSize,
            MEM_COMMIT,
            PAGE_READWRITE
            );

        if (!NT_SUCCESS(status))
        {
            if (reservedBase)
                NtFreeVirtualMemory(NtCurrentProcess(), &reservedBase, &reserveSize, MEM_RELEASE);

            return NULL;
        }
    }
    else
    {
        reserveSize = ReserveSize;
        reservedBase = NULL;

        // Loop trying to reserve the requested size, and halve the size until we succeed.
        while (reserveSize >= PAGE_SIZE)
        {
            status = NtAllocateVirtualMemory(
                NtCurrentProcess(),
                &reservedBase,
                0,
                &reserveSize,
                MEM_RESERVE,
                PAGE_READWRITE
                );

            if (NT_SUCCESS(status))
                break;

            reserveSize /= 2;
        }

        if (!NT_SUCCESS(status))
            return NULL;

        commitSize = CommitSize;
        committedBase = reservedBase;

        if (commitSize > reserveSize)
            commitSize = reserveSize;

        status = NtAllocateVirtualMemory(
            NtCurrentProcess(),
            &committedBase,
            0,
            &commitSize,
            MEM_COMMIT,
            PAGE_READWRITE
            );

        if (!NT_SUCCESS(status))
        {
            NtFreeVirtualMemory(NtCurrentProcess(), &reservedBase, &reserveSize, MEM_RELEASE);
            return NULL;
        }
    }

    segment = committedBase;

    InsertTailList(&StackAlloc->SegmentListHead, &segment->ListEntry);
    segment->CommitSize = commitSize;
    segment->ReserveSize = reserveSize;
    segment->AvailableBytes = CommitSize - sizeof(PH_STACKALLOC_SEGMENT);

    StackAlloc->SegmentCount++;

    return segment;
}

BOOLEAN PhpExtendStackAllocSegment(
    __inout PPH_STACKALLOC_SEGMENT Segment,
    __in SIZE_T CommitSize
    )
{
    NTSTATUS status;
    PVOID committedBase;
    PVOID reservedBase;
    SIZE_T commitSize;
    SIZE_T reserveSize;

    if (CommitSize < Segment->CommitSize)
        return TRUE; // already have this amount committed
    if (CommitSize > Segment->ReserveSize)
        return FALSE; // not enough space

    // Try to commit some more memory.

    commitSize = CommitSize;
    committedBase = Segment;
    status = NtAllocateVirtualMemory(
        NtCurrentProcess(),
        &committedBase,
        0,
        &commitSize,
        MEM_COMMIT,
        PAGE_READWRITE
        );

    if (!NT_SUCCESS(status))
        return FALSE;

    Segment->CommitSize = commitSize;

    return TRUE;
}
