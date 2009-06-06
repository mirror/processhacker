/*
 * Process Hacker Library - 
 *   KProcessHacker transparency hooking
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

#include "kphhook.h"

#define STD_PREFIX NTSTATUS NTAPI
#define DECLARE_NT_HOOK(Name, Arguments) \
    static _##Name Name; \
    static PH_HOOK Name##Hook; \
    PH_DEFINE_NT_HOOK_CALL(NTSTATUS NTAPI Old##Name, Arguments, Name##Hook); \
    STD_PREFIX New##Name(Arguments)
#define DECLARE_NEW_FUNC(Name, Arguments) \
    STD_PREFIX New##Name(Arguments)
#define INITIALIZE_NT_HOOK(Name) \
    Name = PhGetProcAddress(L"ntdll.dll", #Name); \
    PhInitializeHook(&Name##Hook, Name, New##Name); \
    PhHook(&Name##Hook)
#define DEINITIALIZE_NT_HOOK(Name) \
    PhUnhook(&Name##Hook)

BOOLEAN KphHookInitialized = FALSE;
HANDLE KphHandle = NULL;
DECLARE_NT_HOOK(NtGetContextThread, NTGETCONTEXTTHREAD_ARGS);
DECLARE_NT_HOOK(NtOpenProcess, NTOPENPROCESS_ARGS);
DECLARE_NT_HOOK(NtOpenProcessToken, NTOPENPROCESSTOKEN_ARGS);
DECLARE_NT_HOOK(NtOpenProcessTokenEx, NTOPENPROCESSTOKENEX_ARGS);
DECLARE_NT_HOOK(NtOpenThread, NTOPENTHREAD_ARGS);
DECLARE_NT_HOOK(NtReadVirtualMemory, NTREADVIRTUALMEMORY_ARGS);
DECLARE_NT_HOOK(NtSetContextThread, NTSETCONTEXTTHREAD_ARGS);
DECLARE_NT_HOOK(NtTerminateProcess, NTTERMINATEPROCESS_ARGS);
DECLARE_NT_HOOK(NtTerminateThread, NTTERMINATETHREAD_ARGS);
DECLARE_NT_HOOK(NtWriteVirtualMemory, NTWRITEVIRTUALMEMORY_ARGS);

VOID KphHookInit()
{
    if (KphHookInitialized)
        return;

    if (!NT_SUCCESS(KphConnect(&KphHandle)))
        return;

    INITIALIZE_NT_HOOK(NtGetContextThread);
    INITIALIZE_NT_HOOK(NtOpenProcess);
    INITIALIZE_NT_HOOK(NtOpenProcessToken);
    INITIALIZE_NT_HOOK(NtOpenProcessTokenEx);
    INITIALIZE_NT_HOOK(NtOpenThread);
    INITIALIZE_NT_HOOK(NtReadVirtualMemory);
    INITIALIZE_NT_HOOK(NtSetContextThread);
    INITIALIZE_NT_HOOK(NtTerminateProcess);
    INITIALIZE_NT_HOOK(NtTerminateThread);
    INITIALIZE_NT_HOOK(NtWriteVirtualMemory);

    KphHookInitialized = TRUE;
}

VOID KphHookDeinit()
{
    if (!KphHookInitialized)
        return;

    DEINITIALIZE_NT_HOOK(NtGetContextThread);
    DEINITIALIZE_NT_HOOK(NtOpenProcess);
    DEINITIALIZE_NT_HOOK(NtOpenProcessToken);
    DEINITIALIZE_NT_HOOK(NtOpenProcessTokenEx);
    DEINITIALIZE_NT_HOOK(NtOpenThread);
    DEINITIALIZE_NT_HOOK(NtReadVirtualMemory);
    DEINITIALIZE_NT_HOOK(NtSetContextThread);
    DEINITIALIZE_NT_HOOK(NtTerminateProcess);
    DEINITIALIZE_NT_HOOK(NtTerminateThread);
    DEINITIALIZE_NT_HOOK(NtWriteVirtualMemory);

    KphDisconnect(KphHandle);

    KphHookInitialized = FALSE;
}

DECLARE_NEW_FUNC(NtGetContextThread, NTGETCONTEXTTHREAD_ARGS)
{
    return KphGetContextThread(KphHandle, ThreadHandle, Context);
}

DECLARE_NEW_FUNC(NtOpenProcess, NTOPENPROCESS_ARGS)
{
    /* Use KPH if we only have a CID, no name. */
    if (!ObjectAttributes->ObjectName)
        return KphOpenProcess(KphHandle, ProcessHandle, ClientId->UniqueProcess, DesiredAccess);

    return OldNtOpenProcess(ProcessHandle, DesiredAccess, ObjectAttributes, ClientId);
}

DECLARE_NEW_FUNC(NtOpenProcessToken, NTOPENPROCESSTOKEN_ARGS)
{
    return NtOpenProcessTokenEx(ProcessHandle, DesiredAccess, 0, TokenHandle);
}

DECLARE_NEW_FUNC(NtOpenProcessTokenEx, NTOPENPROCESSTOKEN_ARGS)
{
    /* HandleAttributes is ignored. */
    return KphOpenProcessToken(KphHandle, TokenHandle, ProcessHandle, DesiredAccess);
}

DECLARE_NEW_FUNC(NtOpenThread, NTOPENTHREAD_ARGS)
{
    /* Use KPH if we only have a CID, no name. */
    if (!ObjectAttributes->ObjectName)
        return KphOpenThread(KphHandle, ThreadHandle, ClientId->UniqueThread, DesiredAccess);

    return OldNtOpenThread(ThreadHandle, DesiredAccess, ObjectAttributes, ClientId);
}

DECLARE_NEW_FUNC(NtReadVirtualMemory, NTREADVIRTUALMEMORY_ARGS)
{
    return KphReadVirtualMemory(KphHandle, ProcessHandle, BaseAddress, Buffer, BufferLength, ReturnLength);
}

DECLARE_NEW_FUNC(NtSetContextThread, NTSETCONTEXTTHREAD_ARGS)
{
    return KphSetContextThread(KphHandle, ThreadHandle, Context);
}

DECLARE_NEW_FUNC(NtTerminateProcess, NTTERMINATEPROCESS_ARGS)
{
    /* Call the original NtTerminateProcess if we are terminating self to 
     * avoid infinite recursion with KphTerminateProcess.
     */
    if (ProcessHandle == NULL || ProcessHandle == GetCurrentProcess())
        return NtTerminateProcess(ProcessHandle, ExitStatus);

    return KphTerminateProcess(KphHandle, ProcessHandle, ExitStatus);
}

DECLARE_NEW_FUNC(NtTerminateThread, NTTERMINATETHREAD_ARGS)
{
    if (ThreadHandle == NULL || ThreadHandle == GetCurrentThread())
        return NtTerminateThread(ThreadHandle, ExitStatus);

    return KphTerminateThread(KphHandle, ThreadHandle, ExitStatus);
}

DECLARE_NEW_FUNC(NtWriteVirtualMemory, NTWRITEVIRTUALMEMORY_ARGS)
{
    return KphWriteVirtualMemory(KphHandle, ProcessHandle, BaseAddress, Buffer, BufferLength, ReturnLength);
}
