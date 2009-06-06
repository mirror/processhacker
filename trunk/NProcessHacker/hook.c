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

#include "hook.h"

VOID PhInitializeHook(
    PPH_HOOK Hook,
    PVOID Function,
    PVOID Target
    )
{
    memset(Hook, 0, sizeof(PH_HOOK));
    Hook->Function = Function;
    Hook->Target = Target;
}

NTSTATUS PhHook(
    PPH_HOOK Hook
    )
{
    NTSTATUS status = STATUS_SUCCESS;
    ULONG oldProtection;
    PCHAR function;

    /* Change the page protection of the target page so we can write to it. */
    if (!VirtualProtect(Hook->Function, 5, PAGE_EXECUTE_READWRITE, &oldProtection))
        return STATUS_ACCESS_VIOLATION;

    __try
    {
        function = (PCHAR)Hook->Function;
        /* Copy the original five bytes for unhooking. */
        memcpy(Hook->Bytes, function, 5);
        /* Hook the function by writing a jump instruction. */
        Hook->Hooked = TRUE;
        /* jmp Target */
        *function = 0xe9;
        *(PULONG_PTR)(function + 1) = (ULONG_PTR)Hook->Target - (ULONG_PTR)Hook->Function - 5;
    }
    __except (EXCEPTION_EXECUTE_HANDLER)
    {
        status = GetExceptionCode();
    }

    /* Restore the old page protection. */
    VirtualProtect(Hook->Function, 5, oldProtection, NULL);

    return status;
}

NTSTATUS PhUnhook(
    PPH_HOOK Hook
    )
{
    NTSTATUS status = STATUS_SUCCESS;
    ULONG oldProtection;
    PCHAR function;

    /* Change the page protection of the target page so we can write to it. */
    if (!VirtualProtect(Hook->Function, 5, PAGE_EXECUTE_READWRITE, &oldProtection))
        return STATUS_ACCESS_VIOLATION;

    __try
    {
        /* Unpatch the function by restoring the original first 5 bytes. */
        memcpy(Hook->Function, Hook->Bytes, 5);
        Hook->Hooked = FALSE;
    }
    __except (EXCEPTION_EXECUTE_HANDLER)
    {
        status = GetExceptionCode();
    }

    /* Restore the old page protection. */
    VirtualProtect(Hook->Function, 5, oldProtection, NULL);

    return status;
}
