/*
 * Process Hacker Driver - 
 *   custom APIs
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

/* All reverse-engineering done in IDA Pro with that X-ray thingy... */
/* Parts taken from ReactOS */

#include "kph_nt.h"
#include "debug.h"

extern RTL_OSVERSIONINFOW WindowsVersion;
extern ACCESS_MASK ProcessAllAccess;
extern ACCESS_MASK ThreadAllAccess;
extern POBJECT_TYPE *PsJobType;
extern POBJECT_TYPE *SeTokenObjectType;

NTSTATUS OpenProcess(PHANDLE ProcessHandle, int DesiredAccess, HANDLE ProcessId)
{
    OBJECT_ATTRIBUTES objAttr = { 0 };
    CLIENT_ID clientId;
    
    objAttr.Length = sizeof(objAttr);
    clientId.UniqueThread = 0;
    clientId.UniqueProcess = (HANDLE)ProcessId;
    
    return KphOpenProcess(ProcessHandle, DesiredAccess, &objAttr, &clientId, KernelMode);
}

NTSTATUS KphOpenProcess(
    PHANDLE ProcessHandle,
    ACCESS_MASK DesiredAccess,
    POBJECT_ATTRIBUTES ObjectAttributes,
    PCLIENT_ID ClientId,
    KPROCESSOR_MODE AccessMode
    )
{
    BOOLEAN hasObjectName = ObjectAttributes->ObjectName != NULL;
    ULONG attributes = ObjectAttributes->Attributes;
    NTSTATUS status = STATUS_SUCCESS;
    ACCESS_STATE accessState;
    
    /* No one seems to know what the format of AUX_ACCESS_DATA is.
     * ReactOS' definition is wrong because there is supposed to be 
     * some sort of security descriptor at +11. Weird. I've inferred 
     * from the stack frame of PsOpenProcess that AUX_ACCESS_DATA has 
     * a size of 0x34 bytes.
     */
    char auxData[0x34];
    PEPROCESS processObject = NULL;
    PETHREAD threadObject = NULL;
    HANDLE processHandle = NULL;
    
    if (hasObjectName && ClientId)
        return STATUS_INVALID_PARAMETER_MIX;
    
    /* ReactOS code cleared this bit up for me :) */
    status = SeCreateAccessState(
        &accessState,
        (PAUX_ACCESS_DATA)auxData,
        DesiredAccess,
        (PGENERIC_MAPPING)((PCHAR)*PsProcessType + 52)
        );
    
    if (!NT_SUCCESS(status))
    {
        return status;
    }
    
    /* let's hope our client isn't a virus... */
    if (accessState.RemainingDesiredAccess & MAXIMUM_ALLOWED)
        accessState.PreviouslyGrantedAccess |= ProcessAllAccess;
    else
        accessState.PreviouslyGrantedAccess |= accessState.RemainingDesiredAccess;
    
    accessState.RemainingDesiredAccess = 0;
    
    if (hasObjectName)
    {
        status = ObOpenObjectByName(
            ObjectAttributes,
            *PsProcessType,
            AccessMode,
            &accessState,
            0,
            NULL,
            &processHandle
            );
        SeDeleteAccessState(&accessState);
    }
    else if (ClientId)
    {
        if (ClientId->UniqueThread)
        {
            status = PsLookupProcessThreadByCid(ClientId, &processObject, &threadObject);
        }
        else
        {
            status = PsLookupProcessByProcessId(ClientId->UniqueProcess, &processObject);
        }
        
        if (!NT_SUCCESS(status))
        {
            SeDeleteAccessState(&accessState);
            return status;
        }
        
        status = ObOpenObjectByPointer(
            processObject,
            attributes,
            &accessState,
            0,
            *PsProcessType,
            AccessMode,
            &processHandle
            );
        
        SeDeleteAccessState(&accessState);
        ObDereferenceObject(processObject);
        
        if (threadObject)
            ObDereferenceObject(threadObject);
    }
    else
    {
        SeDeleteAccessState(&accessState);
        return STATUS_INVALID_PARAMETER_MIX;
    }
    
    if (NT_SUCCESS(status))
    {
        *ProcessHandle = processHandle;
    }
    
    return status;
}

NTSTATUS KphOpenThread(
    PHANDLE ThreadHandle,
    ACCESS_MASK DesiredAccess,
    POBJECT_ATTRIBUTES ObjectAttributes,
    PCLIENT_ID ClientId,
    KPROCESSOR_MODE AccessMode
    )
{
    BOOLEAN hasObjectName = ObjectAttributes->ObjectName != NULL;
    ULONG attributes = ObjectAttributes->Attributes;
    NTSTATUS status = STATUS_SUCCESS;
    ACCESS_STATE accessState;
    char auxData[0x34];
    PETHREAD threadObject = NULL;
    HANDLE threadHandle = NULL;
    
    if (hasObjectName && ClientId)
        return STATUS_INVALID_PARAMETER_MIX;
    
    status = SeCreateAccessState(
        &accessState,
        (PAUX_ACCESS_DATA)auxData,
        DesiredAccess,
        (PGENERIC_MAPPING)((PCHAR)*PsThreadType + 52)
        );
    
    if (!NT_SUCCESS(status))
    {
        return status;
    }
    
    if (accessState.RemainingDesiredAccess & MAXIMUM_ALLOWED)
        accessState.PreviouslyGrantedAccess |= ThreadAllAccess;
    else
        accessState.PreviouslyGrantedAccess |= accessState.RemainingDesiredAccess;
    
    accessState.RemainingDesiredAccess = 0;
    
    if (hasObjectName)
    {
        status = ObOpenObjectByName(
            ObjectAttributes,
            *PsThreadType,
            AccessMode,
            &accessState,
            0,
            NULL,
            &threadHandle
            );
        SeDeleteAccessState(&accessState);
    }
    else if (ClientId)
    {
        if (ClientId->UniqueProcess)
        {
            status = PsLookupProcessThreadByCid(ClientId, NULL, &threadObject);
        }
        else
        {
            status = PsLookupThreadByThreadId(ClientId->UniqueThread, &threadObject);
        }
        
        if (!NT_SUCCESS(status))
        {
            SeDeleteAccessState(&accessState);
            return status;
        }
        
        status = ObOpenObjectByPointer(
            threadObject,
            attributes,
            &accessState,
            0,
            *PsThreadType,
            AccessMode,
            &threadHandle
            );
        
        SeDeleteAccessState(&accessState);
        ObDereferenceObject(threadObject);
    }
    else
    {
        SeDeleteAccessState(&accessState);
        return STATUS_INVALID_PARAMETER_MIX;
    }
    
    if (NT_SUCCESS(status))
    {
        *ThreadHandle = threadHandle;
    }
    
    return status;
}

NTSTATUS KphOpenProcessTokenEx(
    HANDLE ProcessHandle,
    ACCESS_MASK DesiredAccess,
    ULONG ObjectAttributes,
    PHANDLE TokenHandle,
    KPROCESSOR_MODE AccessMode
    )
{
    NTSTATUS status = STATUS_SUCCESS;
    PEPROCESS processObject;
    PVOID tokenObject;
    HANDLE tokenHandle;
    ACCESS_STATE accessState;
    char auxData[0x34];
    
    status = SeCreateAccessState(
        &accessState,
        (PAUX_ACCESS_DATA)auxData,
        DesiredAccess,
        (PGENERIC_MAPPING)((PCHAR)*SeTokenObjectType + 52)
        );
    
    if (!NT_SUCCESS(status))
    {
        return status;
    }
    
    if (accessState.RemainingDesiredAccess & MAXIMUM_ALLOWED)
        accessState.PreviouslyGrantedAccess |= TOKEN_ALL_ACCESS;
    else
        accessState.PreviouslyGrantedAccess |= accessState.RemainingDesiredAccess;
    
    accessState.RemainingDesiredAccess = 0;
    
    status = ObReferenceObjectByHandle(ProcessHandle, 0, *PsProcessType, KernelMode, &processObject, 0);
    
    if (!NT_SUCCESS(status))
    {
        SeDeleteAccessState(&accessState);
        return status;
    }
    
    tokenObject = PsReferencePrimaryToken(processObject);
    ObDereferenceObject(processObject);
    
    status = ObOpenObjectByPointer(
        tokenObject,
        ObjectAttributes,
        &accessState,
        0,
        *SeTokenObjectType,
        AccessMode,
        &tokenHandle
        );
    SeDeleteAccessState(&accessState);
    ObDereferenceObject(tokenObject);
    
    if (NT_SUCCESS(status))
        *TokenHandle = tokenHandle;
    
    return status;
}

NTSTATUS KphOpenProcessJob(
    HANDLE ProcessHandle,
    ACCESS_MASK DesiredAccess,
    PHANDLE JobHandle,
    KPROCESSOR_MODE AccessMode
    )
{
    NTSTATUS status = STATUS_SUCCESS;
    PEPROCESS processObject;
    PVOID jobObject;
    HANDLE jobHandle;
    ACCESS_STATE accessState;
    char auxData[0x34];
    
    status = SeCreateAccessState(
        &accessState,
        (PAUX_ACCESS_DATA)auxData,
        DesiredAccess,
        (PGENERIC_MAPPING)((PCHAR)*PsJobType + 52)
        );
    
    if (!NT_SUCCESS(status))
    {
        return status;
    }
    
    if (accessState.RemainingDesiredAccess & MAXIMUM_ALLOWED)
        accessState.PreviouslyGrantedAccess |= JOB_OBJECT_ALL_ACCESS;
    else
        accessState.PreviouslyGrantedAccess |= accessState.RemainingDesiredAccess;
    
    accessState.RemainingDesiredAccess = 0;
    
    status = ObReferenceObjectByHandle(ProcessHandle, 0, *PsProcessType, KernelMode, &processObject, 0);
    
    if (!NT_SUCCESS(status))
    {
        SeDeleteAccessState(&accessState);
        return status;
    }
    
    if (WindowsVersion.dwMajorVersion == 6 && WindowsVersion.dwMinorVersion == 0)
        jobObject = *(PVOID *)((PCHAR)processObject + 0x10c);
    else
        jobObject = *(PVOID *)((PCHAR)processObject + 0x134);
    
    ObDereferenceObject(processObject);
    
    if (jobObject == NULL)
    {
        SeDeleteAccessState(&accessState);
        return STATUS_NO_SUCH_FILE;
    }
    
    ObReferenceObject(jobObject);
    status = ObOpenObjectByPointer(
        jobObject,
        0,
        &accessState,
        0,
        *PsJobType,
        AccessMode,
        &jobHandle
        );
    SeDeleteAccessState(&accessState);
    ObDereferenceObject(jobObject);
    
    if (NT_SUCCESS(status))
        *JobHandle = jobHandle;
    
    return status;
}

NTSTATUS KphSuspendProcess(
    HANDLE ProcessHandle
    )
{
    NTSTATUS status = STATUS_SUCCESS;
    PEPROCESS processObject;
    
    status = ObReferenceObjectByHandle(ProcessHandle, 0, 0, KernelMode, &processObject, 0);
    
    if (!NT_SUCCESS(status))
        return status;
    
    /* XP ntoskrnl does NOT export this - loading the driver fails because of this */
    /* status = PsSuspendProcess(processObject); */
    ObDereferenceObject(processObject);
    
    return status;
}

NTSTATUS KphResumeProcess(
    HANDLE ProcessHandle
    )
{
    NTSTATUS status = STATUS_SUCCESS;
    PEPROCESS processObject;
    
    status = ObReferenceObjectByHandle(ProcessHandle, 0, 0, KernelMode, &processObject, 0);
    
    if (!NT_SUCCESS(status))
        return status;
    
    /* status = PsResumeProcess(processObject); */
    ObDereferenceObject(processObject);
    
    return status;
}

NTSTATUS KphTerminateProcess(
    HANDLE ProcessHandle,
    NTSTATUS ExitStatus
    )
{
    NTSTATUS status = STATUS_SUCCESS;
    PEPROCESS processObject;
    OBJECT_ATTRIBUTES objectAttributes = { 0 };
    CLIENT_ID clientId;
    HANDLE newProcessHandle;
    
    status = ObReferenceObjectByHandle(ProcessHandle, 0, 0, KernelMode, &processObject, 0);
    
    if (!NT_SUCCESS(status))
        return status;
    
    /* Can't terminate ourself. Get user-mode to do it. */
    if (PsGetProcessId(processObject) == PsGetCurrentProcessId())
    {
        ObDereferenceObject(processObject);
        return STATUS_DISK_FULL;
    }
    
    /* We have to open it again because ZwTerminateProcess only accepts kernel handles. */
    clientId.UniqueThread = 0;
    clientId.UniqueProcess = PsGetProcessId(processObject);
    status = KphOpenProcess(&newProcessHandle, 0x1, &objectAttributes, &clientId, KernelMode);
    ObDereferenceObject(processObject);
    
    if (NT_SUCCESS(status))
    {
        status = ZwTerminateProcess(newProcessHandle, ExitStatus);
        ZwClose(newProcessHandle);
    }
    
    return status;
}

NTSTATUS KphReadVirtualMemory(
    HANDLE ProcessHandle,
    PVOID BaseAddress,
    PVOID Buffer,
    ULONG BufferLength,
    PULONG ReturnLength,
    KPROCESSOR_MODE AccessMode
    )
{
    NTSTATUS status = STATUS_SUCCESS;
    PEPROCESS processObject;
    ULONG returnLength = 0;
    
    if (AccessMode != KernelMode)
    {
        if ((((ULONG_PTR)BaseAddress + BufferLength) < (ULONG_PTR)BaseAddress) || 
            (((ULONG_PTR)Buffer + BufferLength) < (ULONG_PTR)Buffer) || 
            (((ULONG_PTR)BaseAddress + BufferLength) > MmUserProbeAddress) || 
            (((ULONG_PTR)Buffer + BufferLength) > MmUserProbeAddress))
        {
            return STATUS_ACCESS_VIOLATION;
        }
        
        __try
        {
            if (ReturnLength)
                ProbeForWrite(ReturnLength, sizeof(ULONG), 1);
        }
        __except (EXCEPTION_EXECUTE_HANDLER)
        {
            return STATUS_ACCESS_VIOLATION;
        }
    }
    
    if (BufferLength)
    {
        status = ObReferenceObjectByHandle(ProcessHandle, 0, *PsProcessType, KernelMode, &processObject, NULL);
        
        if (!NT_SUCCESS(status))
            return status;
        
        status = MmCopyVirtualMemory(
            processObject,
            BaseAddress,
            PsGetCurrentProcess(),
            Buffer,
            BufferLength,
            AccessMode,
            &returnLength
            );
        ObDereferenceObject(processObject);
    }
    
    if (ReturnLength)
    {
        __try
        {
            *ReturnLength = returnLength;
        }
        __except (EXCEPTION_EXECUTE_HANDLER)
        {
            status = STATUS_ACCESS_VIOLATION;
        }
    }
    
    return status;
}

NTSTATUS KphWriteVirtualMemory(
    HANDLE ProcessHandle,
    PVOID BaseAddress,
    PVOID Buffer,
    ULONG BufferLength,
    PULONG ReturnLength,
    KPROCESSOR_MODE AccessMode
    )
{
    NTSTATUS status = STATUS_SUCCESS;
    PEPROCESS processObject;
    ULONG returnLength = 0;
    
    if (AccessMode != KernelMode)
    {
        if ((((ULONG_PTR)BaseAddress + BufferLength) < (ULONG_PTR)BaseAddress) || 
            (((ULONG_PTR)Buffer + BufferLength) < (ULONG_PTR)Buffer) || 
            (((ULONG_PTR)BaseAddress + BufferLength) > MmUserProbeAddress) || 
            (((ULONG_PTR)Buffer + BufferLength) > MmUserProbeAddress))
        {
            return STATUS_ACCESS_VIOLATION;
        }
        
        __try
        {
            if (ReturnLength)
                ProbeForWrite(ReturnLength, sizeof(ULONG), 1);
        }
        __except (EXCEPTION_EXECUTE_HANDLER)
        {
            return STATUS_ACCESS_VIOLATION;
        }
    }
    
    if (BufferLength)
    {
        status = ObReferenceObjectByHandle(ProcessHandle, 0, *PsProcessType, KernelMode, &processObject, NULL);
        
        if (!NT_SUCCESS(status))
            return status;
        
        status = MmCopyVirtualMemory(
            PsGetCurrentProcess(),
            Buffer,
            processObject,
            BaseAddress,
            BufferLength,
            AccessMode,
            &returnLength
            );
        ObDereferenceObject(processObject);
    }
    
    if (ReturnLength)
    {
        __try
        {
            *ReturnLength = returnLength;
        }
        __except (EXCEPTION_EXECUTE_HANDLER)
        {
            status = STATUS_ACCESS_VIOLATION;
        }
    }
    
    return status;
}
