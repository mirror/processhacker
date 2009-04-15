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
KE_HOOK AlCopyFileWHook;
KE_HOOK AlCopyFileExWHook;

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
    AL_LOG_CALL(L"NtOpenProcess", &AlNtOpenProcessHook.Hook, 5,
        CmPVoid, 0, L"ProcessHandle", ProcessHandle,
        CmInt32 | CmHex, 0, L"DesiredAccess", DesiredAccess,
        CmPVoid, 0, L"ObjectAttributes", ObjectAttributes,
        CmPVoid, 0, L"ClientId", ClientId,
        CmInt32, 0, L"UniqueProcess", ClientId->UniqueProcess
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
    AL_LOG_CALL(L"NtOpenProcessTokenEx", &AlNtOpenProcessTokenExHook.Hook, 4,
        CmInt32 | CmHex, 0, L"ProcessHandle", ProcessHandle,
        CmInt32 | CmHex, 0, L"DesiredAccess", DesiredAccess,
        CmPVoid, 0, L"ObjectAttributes", ObjectAttributes,
        CmPVoid, 0, L"TokenHandle", TokenHandle
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
    AL_LOG_CALL(L"NtOpenThread", &AlNtOpenThreadHook.Hook, 5,
        CmPVoid, 0, L"ThreadHandle", ThreadHandle,
        CmInt32 | CmHex, 0, L"DesiredAccess", DesiredAccess,
        CmPVoid, 0, L"ObjectAttributes", ObjectAttributes,
        CmPVoid, 0, L"ClientId", ClientId,
        CmInt32, 0, L"UniqueThread", ClientId->UniqueThread
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
    PWSTR appText = lpApplicationName == NULL ? lpCommandLine : lpApplicationName;
    PWSTR preText = L"Allow the process to execute ";
    PWSTR postText = L"?";
    PWSTR confirmText = (PWSTR)malloc((wcslen(preText) + wcslen(appText) + wcslen(postText) + 1) * sizeof(WCHAR));
    int result;

    *confirmText = 0;
    wcscat(confirmText, preText);
    wcscat(confirmText, appText);
    wcscat(confirmText, postText);

    result = MessageBoxW(
        NULL,
        confirmText,
        L"Program execution confirmation", 
        MB_ICONEXCLAMATION | MB_YESNOCANCEL
        );

    free(confirmText);

    if (result == IDNO)
    {
        SetLastError(ERROR_ACCESS_DENIED);

        return FALSE;
    }
    else if (result == IDCANCEL)
    {
        if (MessageBoxW(
            NULL,
            L"Are you sure you want to terminate the process?",
            L"Process termination confirmation",
            MB_ICONEXCLAMATION | MB_YESNO
            ) == IDYES)
            ExitProcess(1);

        SetLastError(ERROR_CANCELLED);

        return FALSE;
    }

    AL_LOG_CALL(L"CreateProcessW", &AlCreateProcessWHook.Hook, 10,
        CmString, 0, L"lpApplicationName", lpApplicationName,
        CmString, 0, L"lpCommandLine", lpCommandLine,
        CmPVoid, 0, L"lpProcessAttributes", lpProcessAttributes,
        CmPVoid, 0, L"lpThreadAttributes", lpThreadAttributes,
        CmBool, 0, L"bInheritHandles", bInheritHandles,
        CmInt32 | CmHex, 0, L"dwCreationFlags", dwCreationFlags,
        CmPVoid, 0, L"lpEnvironment", lpEnvironment,
        CmString, 0, L"lpCurrentDirectory", lpCurrentDirectory,
        CmPVoid, 0, L"lpStartupInfo", lpStartupInfo,
        CmPVoid, 0, L"lpProcessInformation", lpProcessInformation
        );

    return AlCreateProcessW(lpApplicationName, lpCommandLine, lpProcessAttributes,
        lpThreadAttributes, bInheritHandles, dwCreationFlags, lpEnvironment,
        lpCurrentDirectory, lpStartupInfo, lpProcessInformation);
}

BOOL ConfirmCopyFile(
   LPCTSTR lpExistingFileName,
   LPCTSTR lpNewFileName)
{
    PWSTR preText = L"Allow the process to copy ";
    PWSTR midText = L" to ";
    PWSTR postText = L"?";
    PWSTR confirmText = (PWSTR)malloc(
        (wcslen(preText) + wcslen(lpExistingFileName) + wcslen(midText) + 
        wcslen(lpNewFileName) + wcslen(postText) + 1) * sizeof(WCHAR));

    *confirmText = 0;
    wcscat(confirmText, preText);
    wcscat(confirmText, lpExistingFileName);
    wcscat(confirmText, midText);
    wcscat(confirmText, lpNewFileName);
    wcscat(confirmText, postText);

    if (MessageBoxW(
        NULL,
        confirmText,
        L"File move confirmation",
        MB_ICONEXCLAMATION | MB_YESNO
        ) == IDNO)
        return FALSE;

    free(confirmText);

    return TRUE;
}

BOOL WINAPI AlCopyFileW(
    LPCTSTR lpExistingFileName,
    LPCTSTR lpNewFileName,
    BOOL bFailIfExists
    )
{
    return ShKeCall(&AlCopyFileWHook, (PVOID)&lpExistingFileName);
}

BOOL WINAPI AlNewCopyFileW(
    LPCTSTR lpExistingFileName,
    LPCTSTR lpNewFileName,
    BOOL bFailIfExists
    )
{
    if (!ConfirmCopyFile(lpExistingFileName, lpNewFileName))
        return ERROR_ACCESS_DENIED;

    return AlCopyFileW(lpExistingFileName, lpNewFileName, bFailIfExists);
}

BOOL WINAPI AlCopyFileExW(
    LPCTSTR lpExistingFileName,
    LPCTSTR lpNewFileName,
    LPPROGRESS_ROUTINE lpProgressRoutine,
    LPVOID lpData,
    LPBOOL pbCancel,
    DWORD dwCopyFlags
    )
{
    return ShKeCall(&AlCopyFileExWHook, (PVOID)&lpExistingFileName);
}

BOOL WINAPI AlNewCopyFileExW(
    LPCTSTR lpExistingFileName,
    LPCTSTR lpNewFileName,
    LPPROGRESS_ROUTINE lpProgressRoutine,
    LPVOID lpData,
    LPBOOL pbCancel,
    DWORD dwCopyFlags
    )
{
    if (!ConfirmCopyFile(lpExistingFileName, lpNewFileName))
        return ERROR_ACCESS_DENIED;

    return AlCopyFileExW(lpExistingFileName, lpNewFileName, lpProgressRoutine,
        lpData, pbCancel, dwCopyFlags);
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
    PHOOK Hook,
    PBYTE Dictionary,
    ULONG DictionaryLength
    )
{
    NTSTATUS status = STATUS_SUCCESS;
    ULONG nameLength = wcslen(Name);
    ULONG bufferLength = sizeof(ULONG) * 2 + (nameLength + 1) * sizeof(WCHAR) + sizeof(HOOK) + DictionaryLength;
    PBYTE buffer = (PBYTE)malloc(bufferLength);

    *(PULONG)buffer = bufferLength;
    *(PULONG)(buffer + sizeof(ULONG)) = GetCurrentProcessId();
    wcscpy((PWSTR)(buffer + sizeof(ULONG) * 2), Name);
    memcpy(buffer + sizeof(ULONG) * 2 + (nameLength + 1) * sizeof(WCHAR), Hook, sizeof(HOOK));
    memcpy(buffer + sizeof(ULONG) * 2 + (nameLength + 1) * sizeof(WCHAR) + sizeof(HOOK), Dictionary, DictionaryLength);

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
    status |= ShKePatchCall("CopyFileW", AlNewCopyFileW, 3 * 4, &AlCopyFileWHook);
    status |= ShKePatchCall("CopyFileExW", AlNewCopyFileExW, 6 * 4, &AlCopyFileExWHook);

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
    status |= ShKeUnpatchCall(&AlCopyFileWHook);
    status |= ShKeUnpatchCall(&AlCopyFileExWHook);

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
