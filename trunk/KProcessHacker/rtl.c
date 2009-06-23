/*
 * Process Hacker Driver - 
 *   run-time library
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

#include "include/kph.h"

/* KphCaptureStackBackTrace
 * 
 * Walks the stack, capturing the return address from each frame.
 * 
 * Return value: the number of captured addresses in the buffer.
 */
ULONG KphCaptureStackBackTrace(
    __in ULONG FramesToSkip,
    __in ULONG FramesToCapture,
    __in_opt ULONG Flags,
    __out_ecount(FramesToCapture) PVOID *BackTrace,
    __out_opt PULONG BackTraceHash
    )
{
    PVOID backTrace[MAX_STACK_DEPTH * 2];
    ULONG framesFound;
    ULONG hash;
    ULONG i;
    
    /* Skip the current frame (for this function). */
    FramesToSkip++;
    
    /* Check the input. */
    /* Ensure we won't overrun the buffer. */
    if (FramesToCapture + FramesToSkip >= MAX_STACK_DEPTH * 2)
        return 0;
    /* Make sure the flags are correct. */
    if ((Flags & RTL_WALK_VALID_FLAGS) != Flags)
        return 0;
    
    /* Walk the frame chain. */
    framesFound = RtlWalkFrameChain(
        backTrace,
        FramesToCapture + FramesToSkip,
        Flags
        );
    /* Return if we found fewer frames than we wanted to skip. */
    if (framesFound <= FramesToSkip)
        return 0;
    
    /* Copy over the stack trace. 
     * At the same time we calculate the stack trace hash by 
     * summing the addresses.
     */
    for (i = 0, hash = 0; i < FramesToCapture; i++)
    {
        if (FramesToSkip + i >= framesFound)
            break;
        
        BackTrace[i] = backTrace[FramesToSkip + i];
        hash += PtrToUlong(BackTrace[i]);
    }
    
    /* Pass the hash back if the caller requested it. */
    if (BackTraceHash)
        *BackTraceHash = hash;
    
    /* Return the number of addresses we copied. */
    return i;
}

/* KphCaptureAndAddStack
 * 
 * Captures a stack trace and adds it to a trace database.
 */
BOOLEAN KphCaptureAndAddStack(
    __in PRTL_TRACE_DATABASE Database,
    __in KPH_CAPTURE_AND_ADD_STACK_TYPE Type,
    __out_opt PRTL_TRACE_BLOCK *TraceBlock
    )
{
    PVOID trace[MAX_STACK_DEPTH * 4];
    ULONG kmodeFramesFound = 0;
    ULONG umodeFramesFound = 0;
    
    /* Check input. */
    if (Type >= KphCaptureAndAddMaximum)
        return FALSE;
    
    /* Capture the kernel-mode stack if needed. */
    if (
        Type == KphCaptureAndAddKModeStack || 
        Type == KphCaptureAndAddBothStacks
        )
        kmodeFramesFound = KphCaptureStackBackTrace(
            1,
            MAX_STACK_DEPTH * 2 - 2,
            0,
            trace,
            NULL
            );
    /* Capture the user-mode stack if needed. */
    if (
        Type == KphCaptureAndAddUModeStack || 
        Type == KphCaptureAndAddBothStacks
        )
        umodeFramesFound = KphCaptureStackBackTrace(
            0,
            MAX_STACK_DEPTH * 2 - 1,
            RTL_WALK_USER_MODE_STACK,
            &trace[kmodeFramesFound],
            NULL
            );
    /* Add the trace to the database. */
    return RtlTraceDatabaseAdd(
        Database,
        kmodeFramesFound + umodeFramesFound,
        trace,
        TraceBlock
        );
}
