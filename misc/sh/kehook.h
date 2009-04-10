/*
 * Sh Hooking Library - 
 *   kernel32.dll hooks
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

#ifndef _KEHOOK_H
#define _KEHOOK_H

#include "hook.h"

typedef struct _KE_HOOK
{
    HOOK Hook;
    USHORT ArgumentLength;
} KE_HOOK, *PKE_HOOK;

SHAPI ULONG ShKeCall(
    PKE_HOOK KeHook,
    PVOID FirstArgument
    );

SHAPI NTSTATUS ShKePatchCall(
    PSTR Name,
    PVOID Target,
    USHORT ArgumentLength,
    PKE_HOOK KeHook
    );

SHAPI NTSTATUS ShKeUnpatchCall(
    PKE_HOOK KeHook
    );

#endif