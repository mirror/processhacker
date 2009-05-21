/*
 * Process Hacker Driver - 
 *   synchronization code
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

#ifndef _SYNC_H
#define _SYNC_H

#include "kprocesshacker.h"

typedef struct _KPH_PROCESSOR_LOCK
{
    /* Synchronizes access to the processor lock. */
    FAST_MUTEX Mutex;
    /* Storage allocated for DPCs. */
    PKDPC Dpcs;
    /* The number of currently acquired processors. */
    LONG AcquiredProcessors;
    /* The signal for acquired processors to be released. */
    LONG ReleaseSignal;
    /* The old IRQL. */
    KIRQL OldIrql;
    /* Whether the processor lock has been acquired. */
    BOOLEAN Acquired;
} KPH_PROCESSOR_LOCK, *PKPH_PROCESSOR_LOCK;

BOOLEAN KphAcquireProcessorLock(
    PKPH_PROCESSOR_LOCK ProcessorLock
    );

VOID KphInitializeProcessorLock(
    PKPH_PROCESSOR_LOCK ProcessorLock
    );

VOID KphReleaseProcessorLock(
    PKPH_PROCESSOR_LOCK ProcessorLock
    );

#endif
