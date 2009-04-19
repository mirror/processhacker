/*
 * Process Hacker Driver - 
 *   Windows version-specific data
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

#ifndef _VERSION_H
#define _VERSION_H

#include "kprocesshacker.h"

#define WINDOWS_XP 51
#define WINDOWS_SERVER_2003 52
#define WINDOWS_VISTA 60
#define WINDOWS_7 61

#define KVOFF(object, offset) ((PCHAR)(object) + offset)

NTSTATUS KvInit();

#ifdef EXT
#undef EXT
#endif

#ifdef _VERSION_PRIVATE
#define EXT
#else
#define EXT extern
#endif

EXT ULONG WindowsVersion;
EXT RTL_OSVERSIONINFOEXW RtlWindowsVersion;
EXT ACCESS_MASK ProcessAllAccess;
EXT ACCESS_MASK ThreadAllAccess;

/* Offsets */
/* Structures
 * Et: ETHREAD
 * Ep: EPROCESS
 * Ot: OBJECT_TYPE
 * Oti: OBJECT_TYPE_INITIALIZER, offset measured from an OBJECT_TYPE
 */
EXT ULONG OffEtClientId;
EXT ULONG OffEtStartAddress;
EXT ULONG OffEtWin32StartAddress;
EXT ULONG OffEpJob;
EXT ULONG OffEpObjectTable;
EXT ULONG OffEpProtectedProcessOff;
EXT ULONG OffEpProtectedProcessBit;
EXT ULONG OffEpRundownProtect;
EXT ULONG OffOtiGenericMapping;

/* Functions
 * These are all offsets from NtClose.
 */
EXT ULONG OffPsTerminateProcess;

#endif
