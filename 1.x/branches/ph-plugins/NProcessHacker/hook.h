/*
 * Process Hacker Library - 
 *   hooks
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

#include "nph.h"

/* Almost exactly the same as the hooking code in KProcessHacker. */

#ifdef _X86_

#define PH_DEFINE_HOOK_CALL(Name, Arguments, Hook) \
    __declspec(naked) Name(Arguments) \
    { \
        __asm lea   eax, Hook \
        __asm mov   eax, [eax+PH_HOOK.Function] \
        __asm add   eax, 5 \
        __asm push  ebp \
        __asm mov   ebp, esp \
        __asm jmp   eax \
    } \

#define PH_DEFINE_NT_HOOK_CALL(Name, Arguments, Hook) \
    __declspec(naked) Name(Arguments) \
    { \
        __asm lea   eax, Hook \
        __asm mov   edx, [eax+PH_HOOK.Function] \
        __asm add   edx, 5 \
        /* Store the system call number in eax. */ \
        __asm mov   eax, dword ptr [eax+PH_HOOK.Bytes+1] \
        __asm jmp   edx \
    } \

#else

#define PH_DEFINE_HOOK_CALL(Name, Arguments, Hook) \
    Name(Arguments) \
    { \
        RaiseException(STATUS_NOT_SUPPORTED, 0, 0, NULL); \
        return 0; \
    } \

#define PH_DEFINE_NT_HOOK_CALL(Name, Arguments, Hook) \
    Name(Arguments) \
    { \
        RaiseException(STATUS_NOT_SUPPORTED, 0, 0, NULL); \
        return 0; \
    } \

#endif
typedef struct _PH_HOOK
{
    PVOID Function;
    PVOID Target;
    BOOLEAN Hooked;
    CHAR Bytes[5];
} PH_HOOK, *PPH_HOOK;

NPHAPI VOID PHAPI PhInitializeHook(
    PPH_HOOK Hook,
    PVOID Function,
    PVOID Target
    );

NPHAPI NTSTATUS PHAPI PhHook(
    PPH_HOOK Hook
    );

NPHAPI NTSTATUS PHAPI PhUnhook(
    PPH_HOOK Hook
    );

#endif
