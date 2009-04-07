/*
 * Sh Hooking Library - 
 *   Native API hooks
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

#ifndef _NTHOOK_H
#define _NTHOOK_H

#include "hook.h"

typedef struct _NT_HOOK
{
    HOOK Hook;
    ULONG SystemCallIndex;
    USHORT ArgumentLength;
} NT_HOOK, *PNT_HOOK;

NTSTATUS NTAPI ShNtCall(
    PNT_HOOK NtHook,
    PVOID FirstArgument
    );

NTSTATUS ShNtPatchCall(
    PSTR Name,
    PVOID Target,
    PNT_HOOK NtHook
    );

NTSTATUS ShNtUnpatchCall(
    PNT_HOOK NtHook
    );

#endif
