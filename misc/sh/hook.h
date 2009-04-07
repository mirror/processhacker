/*
 * Sh Hooking Library - 
 *   Generic hooks
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

#ifndef _HOOK_H
#define _HOOK_H

#include <ntstatus.h>
                                             
#define WIN32_LEAN_AND_MEAN
#define WIN32_NO_STATUS /* Need ntstatus.h instead */
#include <windows.h>
#include <stdlib.h>

#define NTSTATUS LONG
#define NT_SUCCESS(x) ((x) >= STATUS_SUCCESS)

#ifdef SH_EXPORTS
#define SH_API __declspec(dllexport)
#else
#define SH_API __declspec(dllimport)
#endif

typedef struct _CLIENT_ID
{
    ULONG UniqueProcess;
    ULONG UniqueThread;
} CLIENT_ID, *PCLIENT_ID;

typedef struct _UNICODE_STRING
{
    USHORT Length;
    USHORT MaximumLength;
    PWSTR Buffer;
} UNICODE_STRING, *PUNICODE_STRING;

typedef struct _OBJECT_ATTRIBUTES
{
    ULONG Length;
    HANDLE RootDirectory;
    PUNICODE_STRING ObjectName;
    ULONG Attributes;
    PVOID SecurityDescriptor;
    PVOID SecurityQualityOfService;
} OBJECT_ATTRIBUTES, *POBJECT_ATTRIBUTES;

typedef struct _HOOK
{
    PVOID Address;
    BOOLEAN Hooked;
    BYTE ReplacedBytes[16];
    ULONG ReplacedLength;
} HOOK, *PHOOK;

NTSTATUS ShModifyThreads(BOOLEAN Suspend);

NTSTATUS ShUnpatchCall(
    PHOOK Hook
    );

PVOID ShGetProcAddress(
    PSTR LibraryName,
    PSTR ProcName
    );

#endif