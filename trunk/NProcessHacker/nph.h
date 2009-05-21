/*
 * Process Hacker Library - 
 *   main header file
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

#ifndef _NPH_H
#define _NPH_H

/* If the user has already included windows.h, don't include ntstatus.h 
 * to avoid duplicate macro definitions. */
#ifndef _WINDOWS_
#include <ntstatus.h>
#endif

#define WIN32_LEAN_AND_MEAN
#define WIN32_NO_STATUS /* Need ntstatus.h instead */
#include <windows.h>
#include <stdlib.h>

#ifndef LOGICAL
#define LOGICAL ULONG
#define PLOGICAL PULONG
#endif

#ifndef STATUS_SUCCESS
#define STATUS_SUCCESS (0)
#endif

#ifndef NTSTATUS
#define NTSTATUS LONG
#endif

#ifndef NT_SUCCESS
#define NT_SUCCESS(x) ((x) >= STATUS_SUCCESS)
#endif

#ifdef NPH_EXPORTS
#define NPHAPI __declspec(dllexport)
#else
#define NPHAPI __declspec(dllimport)
#endif

#define EXCEPTION_NO_MEMORY STATUS_NO_MEMORY

NPHAPI PVOID PhAlloc(SIZE_T Size);
NPHAPI PVOID PhRealloc(PVOID Memory, SIZE_T Size);
NPHAPI VOID PhFree(PVOID Memory);
PVOID PhGetProcAddress(PWSTR LibraryName, PSTR ProcName);

#endif
