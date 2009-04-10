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

#include "hook.h"
#include "tlhelp32.h"

PVOID SHAPI ShGetProcAddress(
    PSTR LibraryName,
    PSTR ProcName
    )
{
    return (PVOID)GetProcAddress(GetModuleHandleA(LibraryName), ProcName);
}

NTSTATUS SHAPI ShModifyThreads(
    BOOLEAN Suspend
    )
{
    HANDLE snapshot = CreateToolhelp32Snapshot(TH32CS_SNAPTHREAD, 0);
    THREADENTRY32 thread;

    if (snapshot == INVALID_HANDLE_VALUE)
        return STATUS_UNSUCCESSFUL;

    thread.dwSize = sizeof(THREADENTRY32);

    if (!Thread32First(snapshot, &thread))
        return STATUS_UNSUCCESSFUL;

    do
    {
        if (thread.th32OwnerProcessID == GetCurrentProcessId() && thread.th32ThreadID != GetCurrentThreadId())
        {
            HANDLE threadHandle = OpenThread(THREAD_SUSPEND_RESUME, FALSE, thread.th32ThreadID);

            if (threadHandle)
            {
                if (Suspend)
                    SuspendThread(threadHandle);
                else
                    ResumeThread(threadHandle);

                CloseHandle(threadHandle);
            }
        }
    } while (Thread32Next(snapshot, &thread));

    return STATUS_SUCCESS;
}

NTSTATUS SHAPI ShUnpatchCall(
    PHOOK Hook
    )
{
    ULONG oldProtection;

    if (!Hook->Hooked)
        return STATUS_NOT_SUPPORTED;

    VirtualProtect(Hook->Address, Hook->ReplacedLength, PAGE_EXECUTE_READWRITE, &oldProtection);

    __try
    {
        memcpy(Hook->Address, Hook->ReplacedBytes, Hook->ReplacedLength);
        Hook->Hooked = FALSE;
    }
    __except (EXCEPTION_EXECUTE_HANDLER)
    {
        VirtualProtect(Hook->Address, Hook->ReplacedLength, oldProtection, NULL);

        return GetExceptionCode();
    }

    VirtualProtect(Hook->Address, Hook->ReplacedLength, oldProtection, NULL);

    return STATUS_SUCCESS;
}
