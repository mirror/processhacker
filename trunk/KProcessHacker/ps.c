/*
 * Process Hacker Driver - 
 *   processes and threads
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

#include "include/kph.h"
#include "include/ps.h"

NTSTATUS KphGetContextThread(
    HANDLE ThreadHandle,
    PCONTEXT ThreadContext,
    KPROCESSOR_MODE AccessMode
    )
{
    NTSTATUS status = STATUS_SUCCESS;
    PETHREAD threadObject;
    
    status = ObReferenceObjectByHandle(
        ThreadHandle,
        THREAD_GET_CONTEXT,
        *PsThreadType,
        KernelMode,
        &threadObject,
        NULL);
    
    if (!NT_SUCCESS(status))
        return status;
    
    status = PsGetContextThread(threadObject, ThreadContext, AccessMode);
    ObDereferenceObject(threadObject);
    
    return status;
}

NTSTATUS KphGetThreadWin32Thread(
    HANDLE ThreadHandle,
    PVOID *Win32Thread,
    KPROCESSOR_MODE AccessMode
    )
{
    NTSTATUS status = STATUS_SUCCESS;
    PETHREAD threadObject;
    PVOID win32Thread;
    
    if (AccessMode == UserMode)
    {
        __try
        {
            ProbeForWrite(Win32Thread, sizeof(PVOID), 1);
        }
        __except (EXCEPTION_EXECUTE_HANDLER)
        {
            return STATUS_ACCESS_VIOLATION;
        }
    }
    
    status = ObReferenceObjectByHandle(ThreadHandle, 0, *PsThreadType, KernelMode, &threadObject, NULL);
    
    if (!NT_SUCCESS(status))
        return status;
    
    win32Thread = PsGetThreadWin32Thread(threadObject);
    ObDereferenceObject(threadObject);
    
    __try
    {
        *Win32Thread = win32Thread;
    }
    __except (EXCEPTION_EXECUTE_HANDLER)
    {
        return STATUS_ACCESS_VIOLATION;
    }
    
    return status;
}

HANDLE KphGetProcessId(
    HANDLE ProcessHandle
    )
{
    PEPROCESS processObject;
    HANDLE processId;
    
    if (!NT_SUCCESS(ObReferenceObjectByHandle(ProcessHandle, 0,
        *PsProcessType, KernelMode, &processObject, NULL)))
        return 0;
    
    processId = PsGetProcessId(processObject);
    ObDereferenceObject(processObject);
    
    return processId;
}

HANDLE KphGetThreadId(
    HANDLE ThreadHandle,
    PHANDLE ProcessId
    )
{
    PETHREAD threadObject;
    CLIENT_ID clientId;
    
    if (!NT_SUCCESS(ObReferenceObjectByHandle(ThreadHandle, 0, 
        *PsThreadType, KernelMode, &threadObject, NULL)))
        return 0;
    
    if (WindowsVersion == WINDOWS_VISTA)
        clientId = *(PCLIENT_ID)((PCHAR)threadObject + 0x20c);
    else
        clientId = *(PCLIENT_ID)((PCHAR)threadObject + 0x1ec);
    
    ObDereferenceObject(threadObject);
    
    if (ProcessId)
    {
        *ProcessId = clientId.UniqueProcess;
    }
    
    return clientId.UniqueThread;
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
    
    if (PsGetProcessJob)
    {
        jobObject = PsGetProcessJob(processObject);
    }
    else
    {
        if (WindowsVersion == WINDOWS_VISTA)
            jobObject = *(PVOID *)((PCHAR)processObject + 0x10c);
        else
            jobObject = *(PVOID *)((PCHAR)processObject + 0x134);
    }
    
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

NTSTATUS KphResumeProcess(
    HANDLE ProcessHandle
    )
{
    NTSTATUS status = STATUS_SUCCESS;
    PEPROCESS processObject;
    
    if (PsResumeProcess == NULL)
        return STATUS_NOT_SUPPORTED;
    
    status = ObReferenceObjectByHandle(
        ProcessHandle,
        PROCESS_SUSPEND_RESUME,
        *PsProcessType,
        KernelMode,
        &processObject,
        NULL);
    
    if (!NT_SUCCESS(status))
        return status;
    
    status = PsResumeProcess(processObject);
    ObDereferenceObject(processObject);
    
    return status;
}

NTSTATUS KphSetContextThread(
    HANDLE ThreadHandle,
    PCONTEXT ThreadContext,
    KPROCESSOR_MODE AccessMode
    )
{
    NTSTATUS status = STATUS_SUCCESS;
    PETHREAD threadObject;
    
    status = ObReferenceObjectByHandle(
        ThreadHandle,
        THREAD_SET_CONTEXT,
        *PsThreadType,
        KernelMode,
        &threadObject,
        NULL);
    
    if (!NT_SUCCESS(status))
        return status;
    
    status = PsSetContextThread(threadObject, ThreadContext, AccessMode);
    ObDereferenceObject(threadObject);
    
    return status;
}

NTSTATUS KphSuspendProcess(
    HANDLE ProcessHandle
    )
{
    NTSTATUS status = STATUS_SUCCESS;
    PEPROCESS processObject;
    
    if (PsSuspendProcess == NULL)
        return STATUS_NOT_SUPPORTED;
    
    status = ObReferenceObjectByHandle(
        ProcessHandle,
        PROCESS_SUSPEND_RESUME,
        *PsProcessType,
        KernelMode,
        &processObject,
        NULL);
    
    if (!NT_SUCCESS(status))
        return status;
    
    status = PsSuspendProcess(processObject);
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
    
    status = ObReferenceObjectByHandle(
        ProcessHandle,
        PROCESS_TERMINATE,
        *PsProcessType,
        KernelMode,
        &processObject,
        NULL);
    
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
