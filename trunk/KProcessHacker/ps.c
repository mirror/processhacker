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
#include "include/ke.h"
#include "include/ps.h"

/* KphAcquireProcessRundownProtection
 * 
 * Prevents the process from terminating.
 */
BOOLEAN KphAcquireProcessRundownProtection(
    PEPROCESS Process
    )
{
    return ExAcquireRundownProtection((PEX_RUNDOWN_REF)KVOFF(Process, OffEpRundownProtect));
}

/* KphAssignImpersonationToken
 * 
 * Assigns an impersonation token to the specified thread.
 */
NTSTATUS KphAssignImpersonationToken(
    HANDLE ThreadHandle,
    HANDLE TokenHandle
    )
{
    NTSTATUS status = STATUS_SUCCESS;
    PETHREAD threadObject;
    
    status = ObReferenceObjectByHandle(
        ThreadHandle,
        0,
        *PsThreadType,
        KernelMode,
        &threadObject,
        NULL
        );
    
    if (!NT_SUCCESS(status))
        return status;
    
    status = PsAssignImpersonationToken(threadObject, TokenHandle);
    ObDereferenceObject(threadObject);
    
    return status;
}

/* KphGetContextThread
 * 
 * Gets the context of the specified thread.
 */
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
        NULL
        );
    
    if (!NT_SUCCESS(status))
        return status;
    
    status = PsGetContextThread(threadObject, ThreadContext, AccessMode);
    ObDereferenceObject(threadObject);
    
    return status;
}

/* KphGetProcessId
 * 
 * Gets the ID of the process referenced by the specified handle.
 */
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

/* KphGetThreadId
 * 
 * Gets the ID of the thread referenced by the specified handle, 
 * and optionally the ID of the thread's process.
 */
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
    
    clientId = *(PCLIENT_ID)KVOFF(threadObject, OffEtClientId);
    
    ObDereferenceObject(threadObject);
    
    if (ProcessId)
    {
        *ProcessId = clientId.UniqueProcess;
    }
    
    return clientId.UniqueThread;
}

/* KphGetThreadWin32Thread
 * 
 * Gets a pointer to the WIN32THREAD structure of the specified thread.
 */
NTSTATUS KphGetThreadWin32Thread(
    HANDLE ThreadHandle,
    PVOID *Win32Thread,
    KPROCESSOR_MODE AccessMode
    )
{
    NTSTATUS status = STATUS_SUCCESS;
    PETHREAD threadObject;
    PVOID win32Thread;
    
    if (AccessMode != KernelMode)
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
    
    status = ObReferenceObjectByHandle(
        ThreadHandle,
        0,
        *PsThreadType,
        KernelMode,
        &threadObject,
        NULL
        );
    
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

/* KphOpenProcess
 * 
 * Opens a process.
 */
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
    CHAR auxData[AUX_ACCESS_DATA_SIZE];
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
        (PGENERIC_MAPPING)KVOFF(*PsProcessType, OffOtiGenericMapping)
        );
    
    if (!NT_SUCCESS(status))
    {
        return status;
    }
    
    /* Let's hope our client isn't a virus... */
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

/* KphOpenProcess
 * 
 * Opens the specified process' job object. If the process has 
 * not been assigned to a job object, the function returns 
 * STATUS_PROCESS_NOT_IN_JOB.
 */
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
    CHAR auxData[AUX_ACCESS_DATA_SIZE];
    
    status = SeCreateAccessState(
        &accessState,
        (PAUX_ACCESS_DATA)auxData,
        DesiredAccess,
        (PGENERIC_MAPPING)KVOFF(*PsJobType, OffOtiGenericMapping)
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
    
    /* If we have PsGetProcessJob, use it. Otherwise, read the EPROCESS structure. */
    if (PsGetProcessJob)
    {
        jobObject = PsGetProcessJob(processObject);
    }
    else
    {
        jobObject = *(PVOID *)((PCHAR)processObject + OffEpJob);
    }
    
    ObDereferenceObject(processObject);
    
    if (jobObject == NULL)
    {
        /* No such job. Output a NULL handle and exit. */
        SeDeleteAccessState(&accessState);
        *JobHandle = NULL;
        return STATUS_PROCESS_NOT_IN_JOB;
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

/* KphOpenThread
 * 
 * Opens a thread.
 */
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
    CHAR auxData[AUX_ACCESS_DATA_SIZE];
    PETHREAD threadObject = NULL;
    HANDLE threadHandle = NULL;
    
    if (hasObjectName && ClientId)
        return STATUS_INVALID_PARAMETER_MIX;
    
    status = SeCreateAccessState(
        &accessState,
        (PAUX_ACCESS_DATA)auxData,
        DesiredAccess,
        (PGENERIC_MAPPING)KVOFF(*PsThreadType, OffOtiGenericMapping)
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

/* KphReleaseProcessRundownProtection
 * 
 * Allows the process to terminate.
 */
VOID KphReleaseProcessRundownProtection(
    PEPROCESS Process
    )
{
    ExReleaseRundownProtection((PEX_RUNDOWN_REF)KVOFF(Process, OffEpRundownProtect));
}

/* KphResumeProcess
 * 
 * Resumes the specified process.
 */
NTSTATUS KphResumeProcess(
    HANDLE ProcessHandle
    )
{
    NTSTATUS status = STATUS_SUCCESS;
    PEPROCESS processObject;
    
    if (!PsResumeProcess)
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

/* KphSetContextThread
 * 
 * Sets the context of the specified thread.
 */
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

/* KphSuspendProcess
 * 
 * Suspends the specified process.
 */
NTSTATUS KphSuspendProcess(
    HANDLE ProcessHandle
    )
{
    NTSTATUS status = STATUS_SUCCESS;
    PEPROCESS processObject;
    
    if (!PsSuspendProcess)
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

/* KphTerminateProcess
 * 
 * Terminates the specified process.
 */
NTSTATUS KphTerminateProcess(
    HANDLE ProcessHandle,
    NTSTATUS ExitStatus
    )
{
    NTSTATUS status = STATUS_SUCCESS;
    PEPROCESS processObject;
    
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
    if (processObject == PsGetCurrentProcess())
    {
        ObDereferenceObject(processObject);
        return STATUS_CANT_TERMINATE_SELF;
    }
    
    /* If we have located PsTerminateProcess/PspTerminateProcess, 
       call it. */
    if (__PsTerminateProcess)
    {
        status = PsTerminateProcess(processObject, ExitStatus);
        ObDereferenceObject(processObject);
    }
    else
    {
        /* Otherwise, we'll have to call ZwTerminateProcess - most hooks on this function 
           allow kernel-mode callers through. */
        OBJECT_ATTRIBUTES objectAttributes = { 0 };
        CLIENT_ID clientId;
        HANDLE newProcessHandle;
        
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
    }
    
    return status;
}

/* KphTerminateThread
 * 
 * Terminates the specified thread.
 */
NTSTATUS KphTerminateThread(
    HANDLE ThreadHandle,
    NTSTATUS ExitStatus
    )
{
    NTSTATUS status = STATUS_SUCCESS;
    PETHREAD threadObject;
    
    status = ObReferenceObjectByHandle(
        ThreadHandle,
        THREAD_TERMINATE,
        *PsThreadType,
        KernelMode,
        &threadObject,
        NULL);
    
    if (!NT_SUCCESS(status))
        return status;
    
    if (threadObject != PsGetCurrentThread())
    {
        status = PspTerminateThreadByPointer(threadObject, ExitStatus);
        ObDereferenceObject(threadObject);
    }
    else
    {/*
        ObDereferenceObject(threadObject);
        status = PspTerminateThreadByPointer(PsGetCurrentThread(), ExitStatus); */
        /* Leads to bugs, so don't terminate self. */
        ObDereferenceObject(threadObject);
        return STATUS_CANT_TERMINATE_SELF;
    }
    
    return status;
}

/* PsTerminateProcess
 * 
 * Terminates the specified process. If PsTerminateProcess or 
 * PspTerminateProcess could not be located, the call will fail 
 * with STATUS_NOT_SUPPORTED.
 */
NTSTATUS PsTerminateProcess(
    PEPROCESS Process,
    NTSTATUS ExitStatus
    )
{
    PVOID psTerminateProcess = __PsTerminateProcess;
    NTSTATUS status;
    
    if (!psTerminateProcess)
        return STATUS_NOT_SUPPORTED;
    
    if (WindowsVersion == WINDOWS_XP)
    {
        /* PspTerminateProcess on XP is stdcall. */
        __asm
        {
            push    [ExitStatus]
            push    [Process]
            call    [psTerminateProcess]
            mov     [status], eax
        }
    }
    else if (
        WindowsVersion == WINDOWS_VISTA || 
        WindowsVersion == WINDOWS_7
        )
    {
        /* PsTerminateProcess on Vista and above is thiscall. */
        __asm
        {
            push    [ExitStatus]
            mov     ecx, [Process]
            call    [psTerminateProcess]
            mov     [status], eax
        }
    }
    else
    {
        return STATUS_NOT_SUPPORTED;
    }
    
    return status;
}

/* PspTerminateThreadByPointer
 * 
 * Terminates the specified thread. If PspTerminateThreadByPointer 
 * could not be located, the call will fail with STATUS_NOT_SUPPORTED.
 */
NTSTATUS PspTerminateThreadByPointer(
    PETHREAD Thread,
    NTSTATUS ExitStatus
    )
{
    PVOID pspTerminateThreadByPointer = __PspTerminateThreadByPointer;
    
    if (!pspTerminateThreadByPointer)
        return STATUS_NOT_SUPPORTED;
    
    if (WindowsVersion == WINDOWS_XP)
    {
        return ((_PspTerminateThreadByPointer51)pspTerminateThreadByPointer)(
            Thread,
            ExitStatus
            );
    }
    else if (
        WindowsVersion == WINDOWS_VISTA || 
        WindowsVersion == WINDOWS_7
        )
    {
        return ((_PspTerminateThreadByPointer60)pspTerminateThreadByPointer)(
            Thread,
            ExitStatus,
            Thread == PsGetCurrentThread()
            );
    }
    else
    {
        return STATUS_NOT_SUPPORTED;
    }
}
