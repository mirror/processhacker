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
//#define PROTECT_CLIENT

/* File hooks - ZwCreateFile, ZwOpenFile, ReadFile, WriteFile, etc. */
#define HOOK_FILE

/* Key hooks - ZwCreateKey, ZwDeleteKey, etc. */
#define HOOK_KEY

/* Process hooks - ZwOpenProcess, ZwOpenThread, etc. */
#define HOOK_PROCESS

/* Information hooks - ZwDuplicateObject, ZwQuerySystem*, ZwSetSystem* */
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

int ZwOpenThreadIndex = -1;
int ZwQueryInformationProcessIndex = -1;
int ZwQueryInformationThreadIndex = -1;
int ZwQuerySystemInformationIndex = -1;
int ZwSetInformationProcessIndex = -1;
int ZwTerminateThreadIndex = -1;

_ZwCreateFile OldNtCreateFile = NULL;
_ZwCreateKey OldNtCreateKey = NULL;
_ZwDeleteKey OldNtDeleteKey = NULL;
_ZwDeleteValueKey OldNtDeleteValueKey = NULL;
_ZwDuplicateObject OldNtDuplicateObject = NULL;
_ZwEnumerateKey OldNtEnumerateKey = NULL;
_ZwEnumerateValueKey OldNtEnumerateValueKey = NULL;
_ZwOpenFile OldNtOpenFile = NULL;
_ZwOpenKey OldNtOpenKey = NULL;
_ZwOpenProcess OldNtOpenProcess = NULL;
_ZwOpenThread OldNtOpenThread = NULL;
_ZwQueryInformationFile OldNtQueryInformationFile = NULL;
_ZwQueryInformationProcess OldNtQueryInformationProcess = NULL;
_ZwQueryInformationThread OldNtQueryInformationThread = NULL;
_ZwQueryKey OldNtQueryKey = NULL;
_ZwQuerySystemInformation OldNtQuerySystemInformation = NULL;
_ZwQueryValueKey OldNtQueryValueKey = NULL;
_ZwReadFile OldNtReadFile = NULL;
_ZwSetInformationFile OldNtSetInformationFile = NULL;
_ZwSetInformationProcess OldNtSetInformationProcess = NULL;
_ZwSetInformationThread OldNtSetInformationThread = NULL;
_ZwSetValueKey OldNtSetValueKey = NULL;
_ZwTerminateProcess OldNtTerminateProcess = NULL;
_ZwTerminateThread OldNtTerminateThread = NULL;
_ZwWriteFile OldNtWriteFile = NULL;

PVOID GetSystemRoutineAddress(WCHAR *Name)
{
    PVOID address = NULL;
    UNICODE_STRING unicodeName;
    
    RtlInitUnicodeString(&unicodeName, Name);
    
    __try
    {
        address = MmGetSystemRoutineAddress(&unicodeName);
    }
    __except (EXCEPTION_EXECUTE_HANDLER)
    {
        address = NULL;
    }
    
    return address;
}

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

NTSTATUS NewNtDeleteValueKey(
    HANDLE KeyHandle,
    PUNICODE_STRING ValueName
    )
{
    if (PsGetProcessId(PsGetCurrentProcess()) == ClientPID && !OrigEmpty)
    {
        return CallOrig(ZwDeleteValueKey, KeyHandle, ValueName);
    }
    else
    {
        return OldNtDeleteValueKey(KeyHandle, ValueName);
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

NTSTATUS NewNtEnumerateKey(
    HANDLE KeyHandle,
    ULONG Index,
    KEY_INFORMATION_CLASS KeyInformationClass,
    PVOID KeyInformation,
    ULONG Length,
    PULONG ResultLength
    )
{
    if (PsGetProcessId(PsGetCurrentProcess()) == ClientPID && !OrigEmpty)
    {
        return CallOrig(ZwEnumerateKey, KeyHandle, Index, KeyInformationClass,
            KeyInformation, Length, ResultLength);
    }
    else
    {
        return OldNtEnumerateKey(KeyHandle, Index, KeyInformationClass,
            KeyInformation, Length, ResultLength);
    }
}

NTSTATUS NewNtEnumerateValueKey(
    HANDLE KeyHandle,
    ULONG Index,
    KEY_VALUE_INFORMATION_CLASS KeyValueInformationClass,
    PVOID KeyValueInformation,
    ULONG Length,
    PULONG ResultLength
    )
{
    if (PsGetProcessId(PsGetCurrentProcess()) == ClientPID && !OrigEmpty)
    {
        return CallOrig(ZwEnumerateValueKey, KeyHandle, Index, KeyValueInformationClass,
            KeyValueInformation, Length, ResultLength);
    }
    else
    {
        return OldNtEnumerateValueKey(KeyHandle, Index, KeyValueInformationClass,
            KeyValueInformation, Length, ResultLength);
    }
}

NTSTATUS NewNtOpenFile(
    PHANDLE FileHandle,
    ACCESS_MASK DesiredAccess,
    POBJECT_ATTRIBUTES ObjectAttributes,
    PIO_STATUS_BLOCK IoStatusBlock,
    ULONG ShareAccess,
    ULONG OpenOptions
    )
{
    if (PsGetProcessId(PsGetCurrentProcess()) == ClientPID && !OrigEmpty)
    {
        return CallOrig(ZwOpenFile, FileHandle, DesiredAccess, ObjectAttributes,
            IoStatusBlock, ShareAccess, OpenOptions);
    }
    else
    {
        return OldNtOpenFile(FileHandle, DesiredAccess, ObjectAttributes,
            IoStatusBlock, ShareAccess, OpenOptions);
    }
}

NTSTATUS NewNtOpenKey(
    PHANDLE KeyHandle,
    ACCESS_MASK DesiredAccess,
    POBJECT_ATTRIBUTES ObjectAttributes
    )
{
    if (PsGetProcessId(PsGetCurrentProcess()) == ClientPID && !OrigEmpty)
    {
        return CallOrig(ZwOpenKey, KeyHandle, DesiredAccess, ObjectAttributes);
    }
    else
    {
        return OldNtOpenKey(KeyHandle, DesiredAccess, ObjectAttributes);
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

NTSTATUS NewNtQueryInformationFile(
    HANDLE FileHandle,
    PIO_STATUS_BLOCK IoStatusBlock,
    PVOID FileInformation,
    ULONG Length,
    FILE_INFORMATION_CLASS FileInformationClass
    )
{
    if (PsGetProcessId(PsGetCurrentProcess()) == ClientPID && !OrigEmpty)
    {
        return CallOrig(ZwQueryInformationFile, FileHandle, IoStatusBlock,
            FileInformation, Length, FileInformationClass);
    }
    else
    {
        return OldNtQueryInformationFile(FileHandle, IoStatusBlock,
            FileInformation, Length, FileInformationClass);
    }
}

NTSTATUS NewNtQueryInformationProcess(
    HANDLE ProcessHandle,
    int ProcessInformationClass,
    PVOID ProcessInformation,
    int ProcessInformationLength,
    int *ReturnLength
    )
{
    if (PsGetProcessId(PsGetCurrentProcess()) == ClientPID && !OrigEmpty)
    {
        return CallOrigByIndex(ZwQueryInformationProcess, ProcessHandle,
            ProcessInformationClass, ProcessInformation, ProcessInformationLength,
            ReturnLength);
    }
    else
    {
        return OldNtQueryInformationProcess(ProcessHandle,
            ProcessInformationClass, ProcessInformation, ProcessInformationLength,
            ReturnLength);
    }
}

NTSTATUS NewNtQueryInformationThread(
    HANDLE ThreadHandle,
    int ThreadInformationClass,
    PVOID ThreadInformation,
    int ThreadInformationLength,
    int *ReturnLength
    )
{
    if (PsGetProcessId(PsGetCurrentProcess()) == ClientPID && !OrigEmpty)
    {
        return CallOrigByIndex(ZwQueryInformationThread, ThreadHandle,
            ThreadInformationClass, ThreadInformation, ThreadInformationLength,
            ReturnLength);
    }
    else
    {
        return OldNtQueryInformationThread(ThreadHandle,
            ThreadInformationClass, ThreadInformation, ThreadInformationLength,
            ReturnLength);
    }
}

NTSTATUS NewNtQueryKey(
    HANDLE KeyHandle,
    KEY_INFORMATION_CLASS KeyInformationClass,
    PVOID KeyInformation,
    ULONG Length,
    PULONG ResultLength
    )
{
    if (PsGetProcessId(PsGetCurrentProcess()) == ClientPID && !OrigEmpty)
    {
        return CallOrig(ZwQueryKey, KeyHandle, KeyInformationClass, KeyInformation,
            Length, ResultLength);
    }
    else
    {
        return OldNtQueryKey(KeyHandle, KeyInformationClass, KeyInformation,
            Length, ResultLength);
    }
}

NTSTATUS NewNtQuerySystemInformation(
    int SystemInformationClass,
    PVOID SystemInformation,
    int SystemInformationLength,
    int *ReturnLength
    )
{
    if (PsGetProcessId(PsGetCurrentProcess()) == ClientPID && !OrigEmpty)
    {
        return CallOrigByIndex(ZwQuerySystemInformation, SystemInformationClass,
            SystemInformation, SystemInformationLength, ReturnLength);
    }
    else
    {
        return OldNtQuerySystemInformation(SystemInformationClass,
            SystemInformation, SystemInformationLength, ReturnLength);
    }
}

NTSTATUS NewNtQueryValueKey(
    HANDLE KeyHandle,
    PUNICODE_STRING ValueName,
    KEY_VALUE_INFORMATION_CLASS KeyValueInformationClass,
    PVOID KeyValueInformation,
    ULONG Length,
    PULONG ResultLength
    )
{
    if (PsGetProcessId(PsGetCurrentProcess()) == ClientPID && !OrigEmpty)
    {
        return CallOrig(ZwQueryValueKey, KeyHandle, ValueName,
            KeyValueInformationClass, KeyValueInformation, Length, ResultLength);
    }
    else
    {
        return OldNtQueryValueKey(KeyHandle, ValueName,
            KeyValueInformationClass, KeyValueInformation, Length, ResultLength);
    }
}

NTSTATUS NewNtReadFile(
    HANDLE FileHandle,
    HANDLE Event,
    PIO_APC_ROUTINE ApcRoutine,
    PVOID ApcContext,
    PIO_STATUS_BLOCK IoStatusBlock,
    PVOID Buffer,
    ULONG Length,
    PLARGE_INTEGER ByteOffset,
    PULONG Key
    )
{
    if (PsGetProcessId(PsGetCurrentProcess()) == ClientPID && !OrigEmpty)
    {
        return CallOrig(ZwReadFile, FileHandle, Event, ApcRoutine, ApcContext,
            IoStatusBlock, Buffer, Length, ByteOffset, Key);
    }
    else
    {
        return OldNtReadFile(FileHandle, Event, ApcRoutine, ApcContext,
            IoStatusBlock, Buffer, Length, ByteOffset, Key);
    }
}

NTSTATUS NewNtSetInformationFile(
    HANDLE FileHandle,
    PIO_STATUS_BLOCK IoStatusBlock,
    PVOID FileInformation,
    ULONG Length,
    FILE_INFORMATION_CLASS FileInformationClass
    )
{
    if (PsGetProcessId(PsGetCurrentProcess()) == ClientPID && !OrigEmpty)
    {
        return CallOrig(ZwSetInformationFile, FileHandle, IoStatusBlock,
            FileInformation, Length, FileInformationClass);
    }
    else
    {
        return OldNtSetInformationFile(FileHandle, IoStatusBlock,
            FileInformation, Length, FileInformationClass);
    }
}

NTSTATUS NewNtSetInformationProcess(
    HANDLE ProcessHandle,
    int ProcessInformationClass,
    PVOID ProcessInformation,
    int ProcessInformationLength
    )
{
    if (PsGetProcessId(PsGetCurrentProcess()) == ClientPID && !OrigEmpty)
    {
        return CallOrigByIndex(ZwSetInformationProcess, ProcessHandle, 
            ProcessInformationClass, ProcessInformation, ProcessInformationLength);
    }
    else
    {
        return OldNtSetInformationProcess(ProcessHandle, 
            ProcessInformationClass, ProcessInformation, ProcessInformationLength);
    }
}

NTSTATUS NewNtSetInformationThread(
    HANDLE ThreadHandle,
    THREADINFOCLASS ThreadInformationClass,
    PVOID ThreadInformation,
    ULONG ThreadInformationLength
    )
{
    if (PsGetProcessId(PsGetCurrentProcess()) == ClientPID && !OrigEmpty)
    {
        return CallOrig(ZwSetInformationThread, ThreadHandle, 
            ThreadInformationClass, ThreadInformation, ThreadInformationLength);
    }
    else
    {
        return OldNtSetInformationThread(ThreadHandle, 
            ThreadInformationClass, ThreadInformation, ThreadInformationLength);
    }
}

NTSTATUS NewNtSetValueKey(
    HANDLE KeyHandle,
    PUNICODE_STRING ValueName,
    ULONG TitleIndex,
    ULONG Type,
    PVOID Data,
    ULONG DataSize
    )
{
    if (PsGetProcessId(PsGetCurrentProcess()) == ClientPID && !OrigEmpty)
    {
        return CallOrig(ZwSetValueKey, KeyHandle, ValueName, TitleIndex,
            Type, Data, DataSize);
    }
    else
    {
        return OldNtSetValueKey(KeyHandle, ValueName, TitleIndex,
            Type, Data, DataSize);
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

NTSTATUS NewNtWriteFile(
    HANDLE FileHandle,
    HANDLE Event,
    PIO_APC_ROUTINE ApcRoutine,
    PVOID ApcContext,
    PIO_STATUS_BLOCK IoStatusBlock,
    PVOID Buffer,
    ULONG Length,
    PLARGE_INTEGER ByteOffset,
    PULONG Key
    )
{
    if (PsGetProcessId(PsGetCurrentProcess()) == ClientPID && !OrigEmpty)
    {
        return CallOrig(ZwWriteFile, FileHandle, Event, ApcRoutine,
            ApcContext, IoStatusBlock, Buffer, Length, ByteOffset, Key);
    }
    else
    {
        return OldNtWriteFile(FileHandle, Event, ApcRoutine,
            ApcContext, IoStatusBlock, Buffer, Length, ByteOffset, Key);
    }
}

NTSTATUS KPHHook()
{
    RTL_OSVERSIONINFOW version;
    
    version.dwOSVersionInfoSize = sizeof(version);
    RtlGetVersion(&version);
    
    if (version.dwMajorVersion == 5 && version.dwMinorVersion == 1)
    {
        // XP
        ZwOpenThreadIndex = 0x80;
        ZwQueryInformationProcessIndex = 0x9a;
        ZwQueryInformationThreadIndex = 0x9b;
        ZwQuerySystemInformationIndex = 0xad;
        ZwSetInformationProcessIndex = 0xe4;
        ZwTerminateThreadIndex = 0x102;
    }
    else if (version.dwMajorVersion == 6 && version.dwMinorVersion == 0)
    {
        // Vista
        ZwOpenThreadIndex = 0xc9;
        ZwQueryInformationProcessIndex = 0xe4;
        ZwQueryInformationThreadIndex = 0xe5;
        ZwQuerySystemInformationIndex = 0xf8;
        ZwSetInformationProcessIndex = 0x131;
        ZwTerminateThreadIndex = 0x14f;
    }
    else
    {
        return STATUS_UNSUCCESSFUL;
    }
    
#ifdef HOOK_FILE
    HOOK_CALL(CreateFile);
    HOOK_CALL(OpenFile);
    HOOK_CALL(QueryInformationFile);
    HOOK_CALL(ReadFile);
    HOOK_CALL(SetInformationFile);
    HOOK_CALL(WriteFile);
#endif

#ifdef HOOK_KEY
    HOOK_CALL(CreateKey);
    HOOK_CALL(DeleteKey);
    HOOK_CALL(DeleteValueKey);
    HOOK_CALL(EnumerateKey);
    HOOK_CALL(EnumerateValueKey);
    HOOK_CALL(OpenKey);
    HOOK_CALL(QueryKey);
    HOOK_CALL(QueryValueKey);
    HOOK_CALL(SetValueKey);
#endif

#ifdef HOOK_PROCESS
    HOOK_CALL(OpenProcess);
    HOOK_INDEX(OpenThread);
    HOOK_INDEX(QueryInformationProcess);
    HOOK_INDEX(QueryInformationThread);
    HOOK_INDEX(SetInformationProcess);
    HOOK_CALL(SetInformationThread);
    HOOK_CALL(TerminateProcess);
    HOOK_INDEX(TerminateThread);
#endif

#ifdef HOOK_INFORMATION
    HOOK_CALL(DuplicateObject);
    HOOK_INDEX(QuerySystemInformation);
#endif
    
    return STATUS_SUCCESS;
}

void KPHUnhook()
{
#ifdef HOOK_FILE
    UNHOOK_CALL(CreateFile);
    UNHOOK_CALL(OpenFile);
    UNHOOK_CALL(QueryInformationFile);
    UNHOOK_CALL(ReadFile);
    UNHOOK_CALL(SetInformationFile);
    UNHOOK_CALL(WriteFile);
#endif

#ifdef HOOK_KEY
    UNHOOK_CALL(CreateKey);
    UNHOOK_CALL(DeleteKey);
    UNHOOK_CALL(DeleteValueKey);
    UNHOOK_CALL(EnumerateKey);
    UNHOOK_CALL(EnumerateValueKey);
    UNHOOK_CALL(OpenKey);
    UNHOOK_CALL(QueryKey);
    UNHOOK_CALL(QueryValueKey);
    UNHOOK_CALL(SetValueKey);
#endif

#ifdef HOOK_PROCESS
    UNHOOK_CALL(OpenProcess);
    UNHOOK_INDEX(OpenThread);
    UNHOOK_INDEX(QueryInformationProcess);
    UNHOOK_INDEX(QueryInformationThread);
    UNHOOK_INDEX(SetInformationProcess);
    UNHOOK_CALL(SetInformationThread);
    UNHOOK_CALL(TerminateProcess);
    UNHOOK_INDEX(TerminateThread);
#endif

#ifdef HOOK_INFORMATION
    UNHOOK_CALL(DuplicateObject);
    UNHOOK_INDEX(QuerySystemInformation);
#endif
}
