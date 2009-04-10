/*
 * Sh Hooking Library - 
 *   API logging
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

#include "apilog.h"

HANDLE AlLogPipeHandle = NULL;

NT_HOOK AlNtOpenProcessHook;
NT_HOOK AlNtOpenProcessTokenExHook;
NT_HOOK AlNtOpenThreadHook;
KE_HOOK AlCreateProcessWHook;

NTSTATUS NTAPI AlNtOpenProcess(
    PHANDLE ProcessHandle,
    ACCESS_MASK DesiredAccess,
    POBJECT_ATTRIBUTES ObjectAttributes,
    PCLIENT_ID ClientId
    )
{
    return ShNtCall(&AlNtOpenProcessHook, &ProcessHandle);
}

NTSTATUS NTAPI AlNewNtOpenProcess(
    PHANDLE ProcessHandle,
    ACCESS_MASK DesiredAccess,
    POBJECT_ATTRIBUTES ObjectAttributes,
    PCLIENT_ID ClientId
    )
{
    AL_LOG_CALL(L"NtOpenProcess", &AlNtOpenProcessHook, 5,
        L"ProcessHandle", ProcessHandle,
        L"DesiredAccess", DesiredAccess,
        L"ObjectAttributes", ObjectAttributes,
        L"ClientId", ClientId,
        L"UniqueProcess", ClientId->UniqueProcess
        );

    return AlNtOpenProcess(ProcessHandle, DesiredAccess, ObjectAttributes, ClientId);
}

NTSTATUS NTAPI AlNtOpenProcessTokenEx(
    HANDLE ProcessHandle,
    ACCESS_MASK DesiredAccess,
    POBJECT_ATTRIBUTES ObjectAttributes,
    PHANDLE TokenHandle
    )
{
    return ShNtCall(&AlNtOpenProcessTokenExHook, &ProcessHandle);
}

NTSTATUS NTAPI AlNewNtOpenProcessTokenEx(
    HANDLE ProcessHandle,
    ACCESS_MASK DesiredAccess,
    POBJECT_ATTRIBUTES ObjectAttributes,
    PHANDLE TokenHandle
    )
{
    AL_LOG_CALL(L"NtOpenProcessTokenEx", &AlNtOpenProcessTokenExHook, 4,
        L"ProcessHandle", ProcessHandle,
        L"DesiredAccess", DesiredAccess,
        L"ObjectAttributes", ObjectAttributes,
        L"TokenHandle", TokenHandle
        );

    return AlNtOpenProcessTokenEx(ProcessHandle, DesiredAccess, ObjectAttributes, TokenHandle);
}

NTSTATUS NTAPI AlNtOpenThread(
    PHANDLE ThreadHandle,
    ACCESS_MASK DesiredAccess,
    POBJECT_ATTRIBUTES ObjectAttributes,
    PCLIENT_ID ClientId
    )
{
    return ShNtCall(&AlNtOpenThreadHook, &ThreadHandle);
}

NTSTATUS NTAPI AlNewNtOpenThread(
    PHANDLE ThreadHandle,
    ACCESS_MASK DesiredAccess,
    POBJECT_ATTRIBUTES ObjectAttributes,
    PCLIENT_ID ClientId
    )
{
    AL_LOG_CALL(L"NtOpenThread", &AlNtOpenThreadHook, 5,
        L"ThreadHandle", ThreadHandle,
        L"DesiredAccess", DesiredAccess,
        L"ObjectAttributes", ObjectAttributes,
        L"ClientId", ClientId,
        L"UniqueThread", ClientId->UniqueThread
        );

    return AlNtOpenThread(ThreadHandle, DesiredAccess, ObjectAttributes, ClientId);
}

BOOL WINAPI AlCreateProcessW(
    LPCTSTR lpApplicationName,
    LPTSTR lpCommandLine,
    LPSECURITY_ATTRIBUTES lpProcessAttributes,
    LPSECURITY_ATTRIBUTES lpThreadAttributes,
    BOOL bInheritHandles,
    DWORD dwCreationFlags,
    LPVOID lpEnvironment,
    LPCTSTR lpCurrentDirectory,
    LPSTARTUPINFO lpStartupInfo,
    LPPROCESS_INFORMATION lpProcessInformation
    )
{
    return (BOOL)ShKeCall(&AlCreateProcessWHook, (PVOID)&lpApplicationName);
}

BOOL WINAPI AlNewCreateProcessW(
    LPCTSTR lpApplicationName,
    LPTSTR lpCommandLine,
    LPSECURITY_ATTRIBUTES lpProcessAttributes,
    LPSECURITY_ATTRIBUTES lpThreadAttributes,
    BOOL bInheritHandles,
    DWORD dwCreationFlags,
    LPVOID lpEnvironment,
    LPCTSTR lpCurrentDirectory,
    LPSTARTUPINFO lpStartupInfo,
    LPPROCESS_INFORMATION lpProcessInformation
    )
{
    int result = MessageBoxW(
        NULL,
        lpApplicationName == NULL ? lpCommandLine : lpApplicationName,
        L"Program execution confirmation", 
        MB_ICONEXCLAMATION | MB_YESNOCANCEL
        );

    if (result == IDNO)
    {
        SetLastError(ERROR_ACCESS_DENIED);

        return FALSE;
    }
    else if (result == IDCANCEL)
    {
        ExitProcess(1);

        return FALSE;
    }

    return AlCreateProcessW(lpApplicationName, lpCommandLine, lpProcessAttributes,
        lpThreadAttributes, bInheritHandles, dwCreationFlags, lpEnvironment,
        lpCurrentDirectory, lpStartupInfo, lpProcessInformation);
}

NTSTATUS AlWriteLogPipe(
    PVOID Buffer,
    ULONG BufferLength
    )
{
    NTSTATUS status = STATUS_SUCCESS;
    ULONG bytesWritten;

    if (!AlLogPipeHandle)
    {
        AlLogPipeHandle = CreateFile(AL_PIPE_NAME, GENERIC_WRITE, 0, NULL, OPEN_EXISTING, 0, NULL);

        if (AlLogPipeHandle == INVALID_HANDLE_VALUE)
        {
            AlLogPipeHandle = NULL;
            return STATUS_UNSUCCESSFUL;
        }
    }

    if (!WriteFile(AlLogPipeHandle, Buffer, BufferLength, &bytesWritten, NULL))
    {
        CloseHandle(AlLogPipeHandle);
        AlLogPipeHandle = NULL;
        return STATUS_UNSUCCESSFUL;
    }

    return status;
}

NTSTATUS AlLogCall(
    PWSTR Name,
    PNT_HOOK NtHook,
    PBYTE Dictionary,
    ULONG DictionaryLength
    )
{
    NTSTATUS status = STATUS_SUCCESS;
    ULONG nameLength = wcslen(Name);
    ULONG bufferLength = sizeof(ULONG) * 2 + (nameLength + 1) * sizeof(WCHAR) + sizeof(NT_HOOK) + DictionaryLength;
    PBYTE buffer = (PBYTE)malloc(bufferLength);

    *(PULONG)buffer = bufferLength;
    *(PULONG)(buffer + sizeof(ULONG)) = GetCurrentProcessId();
    wcscpy((PWSTR)(buffer + sizeof(ULONG) * 2), Name);
    memcpy(buffer + sizeof(ULONG) * 2 + (nameLength + 1) * sizeof(WCHAR), NtHook, sizeof(NT_HOOK));
    memcpy(buffer + sizeof(ULONG) * 2 + (nameLength + 1) * sizeof(WCHAR) + sizeof(NT_HOOK), Dictionary, DictionaryLength);

    status = AlWriteLogPipe(buffer, bufferLength);
    free(buffer);

    return status;
}

NTSTATUS AlPatch()
{
    NTSTATUS status = STATUS_SUCCESS;

    status |= ShNtPatchCall("NtOpenProcess", AlNewNtOpenProcess, &AlNtOpenProcessHook);
    status |= ShNtPatchCall("NtOpenThread", AlNewNtOpenThread, &AlNtOpenThreadHook);
    status |= ShNtPatchCall("NtOpenProcessTokenEx", AlNewNtOpenProcessTokenEx, &AlNtOpenProcessTokenExHook);
    status |= ShKePatchCall("CreateProcessW", AlNewCreateProcessW, 10 * 4, &AlCreateProcessWHook);

    if (!NT_SUCCESS(status))
        return STATUS_UNSUCCESSFUL;

    return status;
}

NTSTATUS AlUnpatch()
{
    NTSTATUS status = STATUS_SUCCESS;

    status |= ShNtUnpatchCall(&AlNtOpenProcessHook);
    status |= ShNtUnpatchCall(&AlNtOpenThreadHook);
    status |= ShNtUnpatchCall(&AlNtOpenProcessTokenExHook);
    status |= ShKeUnpatchCall(&AlCreateProcessWHook);

    if (!NT_SUCCESS(status))
        return STATUS_UNSUCCESSFUL;

    return status;
}

NTSTATUS AlInit()
{
    NTSTATUS status = STATUS_SUCCESS;

    //ShModifyThreads(TRUE);
    status = AlPatch();
    //ShModifyThreads(FALSE);

    return status;
}

NTSTATUS AlDeinit()
{
    NTSTATUS status = STATUS_SUCCESS;

    if (AlLogPipeHandle)
    {
        CloseHandle(AlLogPipeHandle);
        AlLogPipeHandle = NULL;
    }

    //ShModifyThreads(TRUE);
    status = AlUnpatch();
    //ShModifyThreads(FALSE);

    return status;
}
