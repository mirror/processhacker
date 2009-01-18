/*
 * Process Hacker Driver - 
 *   hooks code
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

#include "ssdt.h"
#include "hooks.h"

/* If enabled, user-mode processes other than the client 
 * cannot open the client's process or its threads.
 */
#define PROTECT_CLIENT

/* File hooks - ZwCreateFile, ZwOpenFile, ReadFile, WriteFile, etc. */
#define HOOK_FILE

/* Key hooks - ZwCreateKey, ZwDeleteKey, etc. */
#define HOOK_KEY

/* Process hooks - ZwOpenProcess, ZwOpenThread, etc. */
#define HOOK_PROCESS

/* Information hooks - ZwDuplicateObject, ZwQuery*, ZwSet* */
#define HOOK_INFORMATION

extern int ClientPID;
extern PVOID *OrigKiServiceTable;

#define OrigEmpty (OrigKiServiceTable == NULL)
#define CallOrig(f, ...) (((_##f)OrigKiServiceTable[SYSCALL_INDEX(f)])(__VA_ARGS__))
#define CallOrigByIndex(f, ...) (((_##f)OrigKiServiceTable[f##Index])(__VA_ARGS__))

/* Hooks a call by reading its index from memory. This is only available for 
 * certain functions that MS allows us to use :(.
 */
#define HOOK_CALL(f) OldNt##f = SsdtModifyEntryByCall(Zw##f, NewNt##f)

/* Hooks a call by a hardcoded index. Not very safe, but it works... */
#define HOOK_INDEX(f) OldNt##f = SsdtModifyEntryByIndex(Zw##f##Index, NewNt##f)
#define UNHOOK_CALL(f) SsdtRestoreEntryByCall(Zw##f, OldNt##f, NewNt##f)
#define UNHOOK_INDEX(f) SsdtRestoreEntryByIndex(Zw##f##Index, OldNt##f, NewNt##f)

_ZwCreateFile OldNtCreateFile;
_ZwCreateKey OldNtCreateKey;
_ZwDeleteKey OldNtDeleteKey;
_ZwDuplicateObject OldNtDuplicateObject;
_ZwOpenProcess OldNtOpenProcess;
_ZwOpenThread OldNtOpenThread;
_ZwTerminateProcess OldNtTerminateProcess;
_ZwTerminateThread OldNtTerminateThread;

NTSTATUS NewNtCreateFile(
    PHANDLE FileHandle,
    ACCESS_MASK DesiredAccess,
    POBJECT_ATTRIBUTES ObjectAttributes,
    PIO_STATUS_BLOCK IoStatusBlock,
    PLARGE_INTEGER AllocationSize,
    ULONG FileAttributes,
    ULONG ShareAccess,
    ULONG CreateDisposition,
    ULONG CreateOptions,
    PVOID EaBuffer,
    ULONG EaLength
    )
{
    if (PsGetProcessId(PsGetCurrentProcess()) == ClientPID && !OrigEmpty)
    {
        return CallOrig(ZwCreateFile, FileHandle, DesiredAccess, ObjectAttributes, 
            IoStatusBlock, AllocationSize, FileAttributes, ShareAccess, 
            CreateDisposition, CreateOptions, EaBuffer, EaLength);
    }
    else
    {
        return OldNtCreateFile(FileHandle, DesiredAccess, ObjectAttributes, 
            IoStatusBlock, AllocationSize, FileAttributes, ShareAccess, 
            CreateDisposition, CreateOptions, EaBuffer, EaLength);
    }
}

NTSTATUS NewNtCreateKey(
    PHANDLE KeyHandle,
    ACCESS_MASK DesiredAccess,
    POBJECT_ATTRIBUTES ObjectAttributes,
    ULONG TitleIndex,
    PUNICODE_STRING Class,
    ULONG CreateOptions,
    PULONG Disposition
    )
{
    if (PsGetProcessId(PsGetCurrentProcess()) == ClientPID && !OrigEmpty)
    {
        return CallOrig(ZwCreateKey, KeyHandle, DesiredAccess, 
            ObjectAttributes, TitleIndex, Class, CreateOptions, Disposition);
    }
    else
    {
        return OldNtCreateKey(KeyHandle, DesiredAccess, 
            ObjectAttributes, TitleIndex, Class, CreateOptions, Disposition);
    }
}

NTSTATUS NewNtDeleteKey(HANDLE KeyHandle)
{
    if (PsGetProcessId(PsGetCurrentProcess()) == ClientPID && !OrigEmpty)
    {
        return CallOrig(ZwDeleteKey, KeyHandle);
    }
    else
    {
        return OldNtDeleteKey(KeyHandle);
    }
}

NTSTATUS NewNtDuplicateObject(
    HANDLE SourceProcessHandle,
    HANDLE SourceHandle,
    HANDLE DestinationProcessHandle,
    PHANDLE DestinationHandle,
    ACCESS_MASK DesiredAccess,
    int Attributes,
    int Options)
{
    if (PsGetProcessId(PsGetCurrentProcess()) == ClientPID && !OrigEmpty)
    {
        return CallOrig(ZwDuplicateObject, SourceProcessHandle, SourceHandle, 
            DestinationProcessHandle, DestinationHandle, DesiredAccess, Attributes, Options);
    }
    else
    {
        return OldNtDuplicateObject(SourceProcessHandle, SourceHandle, 
            DestinationProcessHandle, DestinationHandle, DesiredAccess, Attributes, Options);
    }
}

NTSTATUS NewNtOpenProcess(
    PHANDLE ProcessHandle,
    ACCESS_MASK DesiredAccess,
    POBJECT_ATTRIBUTES ObjectAttributes,
    PCLIENT_ID ClientId
    )
{
#ifdef PROTECT_CLIENT
    if (PsGetProcessId(PsGetCurrentProcess()) != ClientPID && 
        !PsIsSystemThread(PsGetCurrentThread()))
    {
        __try
        {
            ProbeForRead(ClientId, sizeof(CLIENT_ID), 1);
            
            if (ClientId->UniqueProcess == ClientPID)
            {
                return STATUS_NOT_IMPLEMENTED; /* ;) */
            }
        }
        __except (EXCEPTION_EXECUTE_HANDLER)
        {
            return STATUS_ACCESS_VIOLATION;
        }
    }
#endif

    if (PsGetProcessId(PsGetCurrentProcess()) == ClientPID && !OrigEmpty)
    {
        return CallOrig(ZwOpenProcess, ProcessHandle, DesiredAccess, ObjectAttributes, ClientId);
    }
    else
    {
        return OldNtOpenProcess(ProcessHandle, DesiredAccess, ObjectAttributes, ClientId);
    }
}

NTSTATUS NewNtOpenThread(
    PHANDLE ThreadHandle,
    ACCESS_MASK DesiredAccess,
    POBJECT_ATTRIBUTES ObjectAttributes,
    PCLIENT_ID ClientId
    )
{
    if (PsGetProcessId(PsGetCurrentProcess()) == ClientPID && !OrigEmpty)
    {
        return CallOrigByIndex(ZwOpenThread, ThreadHandle, DesiredAccess, ObjectAttributes, ClientId);
    }
    else
    {
        NTSTATUS status = OldNtOpenThread(ThreadHandle, DesiredAccess, ObjectAttributes, ClientId);
        
#ifdef PROTECT_CLIENT
        if (status == STATUS_SUCCESS && !PsIsSystemThread(PsGetCurrentThread()))
        {
            /* check if it's our thread */
            PETHREAD eThread;
            
            status = ObReferenceObjectByHandle(*ThreadHandle, 0, 0, 0, &eThread, 0);
            
            if (status != STATUS_SUCCESS)
                return status;
            
            if (PsGetProcessId(eThread->Process) == ClientPID)
            {
                ObDereferenceObject(eThread);
                *ThreadHandle = 0;
                return STATUS_NOT_IMPLEMENTED;
            }
            else
            {
                ObDereferenceObject(eThread);
                return STATUS_SUCCESS;
            }
        }
        else
        {
            return status;
        }
#else
        return status;
#endif
    }
}

NTSTATUS NewNtTerminateProcess(
    HANDLE ProcessHandle,
    int ExitCode)
{
    if (PsGetProcessId(PsGetCurrentProcess()) == ClientPID && !OrigEmpty)
    {
        return CallOrig(ZwTerminateProcess, ProcessHandle, ExitCode);
    }
    else
    {
        return OldNtTerminateProcess(ProcessHandle, ExitCode);
    }
}

NTSTATUS NewNtTerminateThread(
    HANDLE ThreadHandle,
    int ExitCode)
{
    if (PsGetProcessId(PsGetCurrentProcess()) == ClientPID && !OrigEmpty)
    {
        return CallOrigByIndex(ZwTerminateThread, ThreadHandle, ExitCode);
    }
    else
    {
        return OldNtTerminateThread(ThreadHandle, ExitCode);
    }
}

void KPHHook()
{
#ifdef HOOK_FILE
    HOOK_CALL(CreateFile);
#endif

#ifdef HOOK_KEY
    HOOK_CALL(CreateKey);
    HOOK_CALL(DeleteKey);
#endif

#ifdef HOOK_PROCESS
    HOOK_CALL(OpenProcess);
    HOOK_INDEX(OpenThread);
    HOOK_CALL(TerminateProcess);
    HOOK_INDEX(TerminateThread);
#endif

#ifdef HOOK_INFORMATION
    HOOK_CALL(DuplicateObject);
#endif
}

void KPHUnhook()
{
#ifdef HOOK_FILE
    UNHOOK_CALL(CreateFile);
#endif

#ifdef HOOK_KEY
    UNHOOK_CALL(CreateKey);
    UNHOOK_CALL(DeleteKey);
#endif

#ifdef HOOK_PROCESS
    UNHOOK_CALL(OpenProcess);
    UNHOOK_INDEX(OpenThread);
    UNHOOK_CALL(TerminateProcess);
    UNHOOK_INDEX(TerminateThread);
#endif

#ifdef HOOK_INFORMATION
    UNHOOK_CALL(DuplicateObject);
#endif
}
