/*
 * Process Hacker Driver - 
 *   object manager
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
#include "include/ob.h"

/* KphDuplicateObject
 * 
 * Duplicates a handle from the source process to the target process.
 */
NTSTATUS KphDuplicateObject(
    HANDLE SourceProcessHandle,
    HANDLE SourceHandle,
    HANDLE TargetProcessHandle,
    PHANDLE TargetHandle,
    ACCESS_MASK DesiredAccess,
    ULONG HandleAttributes,
    ULONG Options,
    KPROCESSOR_MODE AccessMode
    )
{
    NTSTATUS status = STATUS_SUCCESS;
    PEPROCESS sourceProcess = NULL;
    PEPROCESS targetProcess = NULL;
    HANDLE targetHandle;
    
    if (TargetHandle && AccessMode != KernelMode)
    {
        __try
        {
            ProbeForWrite(TargetHandle, sizeof(HANDLE), 1);
            *TargetHandle = NULL;
        }
        __except (EXCEPTION_EXECUTE_HANDLER)
        {
            return STATUS_ACCESS_VIOLATION;
        }
    }
    
    status = ObReferenceObjectByHandle(
        SourceProcessHandle,
        PROCESS_DUP_HANDLE,
        *PsProcessType,
        KernelMode,
        &sourceProcess,
        NULL
        );
    
    if (!NT_SUCCESS(status))
        return status;
    
    /* Target handle is optional */
    if (TargetProcessHandle)
    {
        status = ObReferenceObjectByHandle(
            TargetProcessHandle,
            PROCESS_DUP_HANDLE,
            *PsProcessType,
            KernelMode,
            &targetProcess,
            NULL
            );
        
        if (!NT_SUCCESS(status))
            return status;
    }
    
    /* Call the internal function */
    status = ObDuplicateObject(
        sourceProcess,
        targetProcess,
        SourceHandle,
        &targetHandle,
        DesiredAccess,
        HandleAttributes,
        Options,
        AccessMode
        );
    
    if (TargetHandle)
    {
        __try
        {
            *TargetHandle = targetHandle;
        }
        __except (EXCEPTION_EXECUTE_HANDLER)
        {
            status = STATUS_ACCESS_VIOLATION;
        }
    }
    
    ObDereferenceObject(sourceProcess);
    if (targetProcess)
        ObDereferenceObject(targetProcess);
    
    return status;
}

/* KphEnumProcessHandleTable
 * 
 * Enumerates the handles in the specified process' handle table.
 */
BOOLEAN KphEnumProcessHandleTable(
    PEPROCESS Process,
    PEX_ENUM_HANDLE_CALLBACK EnumHandleProcedure,
    PVOID Context,
    PHANDLE Handle
    )
{
    BOOLEAN result = FALSE;
    PHANDLE_TABLE handleTable = NULL;
    
    handleTable = ObReferenceProcessHandleTable(Process);
    
    if (!handleTable)
        return FALSE;
    
    result = ExEnumHandleTable(
        handleTable,
        EnumHandleProcedure,
        Context,
        Handle);
    ObDereferenceProcessHandleTable(Process);
    return result;
}

/* ObDereferenceProcessHandleTable
 * 
 * Allows the process to terminate.
 */
VOID ObDereferenceProcessHandleTable(
    PEPROCESS Process
    )
{
    KphReleaseProcessRundownProtection(Process);
}

/* ObDuplicateObject
 * 
 * Duplicates a handle from the source process to the target process.
 */
NTSTATUS ObDuplicateObject(
    PEPROCESS SourceProcess,
    PEPROCESS TargetProcess,
    HANDLE SourceHandle,
    PHANDLE TargetHandle,
    ACCESS_MASK DesiredAccess,
    ULONG HandleAttributes,
    ULONG Options,
    KPROCESSOR_MODE AccessMode
    )
{
    NTSTATUS status = STATUS_SUCCESS;
    BOOLEAN sourceAttached = FALSE;
    BOOLEAN targetAttached = FALSE;
    KAPC_STATE apcState;
    PVOID object;
    HANDLE objectHandle;
    
    /* Validate the parameters */
    if (!TargetProcess || !TargetHandle)
    {
        if (!(Options & DUPLICATE_CLOSE_SOURCE))
            return STATUS_INVALID_PARAMETER;
    }
    
    /* Check if we need to attach to the source process */
    if (SourceProcess != PsGetCurrentProcess())
    {
        KeStackAttachProcess(SourceProcess, &apcState);
        sourceAttached = TRUE;
    }
    
    /* If the caller wants us to close the source handle, do it now */
    if (Options & DUPLICATE_CLOSE_SOURCE)
    {
        status = NtClose(SourceHandle);
        if (sourceAttached)
            KeUnstackDetachProcess(&apcState);
        
        return status;
    }
    
    /* Reference the object and detach from the source process */
    status = ObReferenceObjectByHandle(
        SourceHandle,
        0,
        NULL,
        KernelMode,
        &object,
        NULL
        );
    if (sourceAttached)
        KeUnstackDetachProcess(&apcState);
    
    if (!NT_SUCCESS(status))
        return status;
    
    /* Check if we need to attach to the target process */
    if (TargetProcess != PsGetCurrentProcess())
    {
        KeStackAttachProcess(TargetProcess, &apcState);
        targetAttached = TRUE;
    }
    
    /* Open the object and detach from the target process */
    {
        POBJECT_TYPE objectType = OBJECT_TO_OBJECT_HEADER(object)->Type;
        ACCESS_STATE accessState;
        char auxData[0x34];
        
        if (!objectType && AccessMode != KernelMode)
        {
            status = STATUS_INVALID_HANDLE;
            goto OpenObjectEnd;
        }
        
        status = SeCreateAccessState(
            &accessState,
            (PAUX_ACCESS_DATA)auxData,
            DesiredAccess,
            (PGENERIC_MAPPING)KVOFF(objectType, OffOtiGenericMapping)
            );
        
        if (!NT_SUCCESS(status))
            goto OpenObjectEnd;
        
        accessState.PreviouslyGrantedAccess |= 0xffffffff; /* HACK, doesn't work properly */
        accessState.RemainingDesiredAccess = 0;
        
        status = ObOpenObjectByPointer(
            object,
            HandleAttributes,
            &accessState,
            DesiredAccess,
            objectType,
            AccessMode,
            &objectHandle
            );
        SeDeleteAccessState(&accessState);
    }
    
OpenObjectEnd:
    ObDereferenceObject(object);
    
    if (targetAttached)
        KeUnstackDetachProcess(&apcState);
    
    if (NT_SUCCESS(status))
        *TargetHandle = objectHandle;
    else
        *TargetHandle = 0;
    
    return status;
}

/* ObReferenceProcessHandleTable
 * 
 * Prevents the process from terminating and returns a pointer 
 * to its handle table.
 */
PHANDLE_TABLE ObReferenceProcessHandleTable(
    PEPROCESS Process
    )
{
    PHANDLE_TABLE handleTable = NULL;
    
    if (KphAcquireProcessRundownProtection(Process))
    {
        handleTable = *(PHANDLE_TABLE *)KVOFF(Process, OffEpObjectTable);
        
        if (!handleTable)
            KphReleaseProcessRundownProtection(Process);
    }
    
    return handleTable;
}
