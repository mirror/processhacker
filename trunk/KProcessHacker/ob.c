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

BOOLEAN KphpQueryProcessHandlesEnumCallback(
    __inout PHANDLE_TABLE_ENTRY HandleTableEntry,
    __in HANDLE Handle,
    __in POBP_QUERY_PROCESS_HANDLES_DATA Context
    );

BOOLEAN KphpSetHandleGrantedAccessEnumCallback(
    __inout PHANDLE_TABLE_ENTRY HandleTableEntry,
    __in HANDLE Handle,
    __in POBP_SET_HANDLE_GRANTED_ACCESS_DATA Context
    );

#ifdef ALLOC_PRAGMA
#pragma alloc_text(PAGE, KphDuplicateObject)
#pragma alloc_text(PAGE, ObDuplicateObject)
#endif

/* This attribute is now stored in the GrantedAccess field. */
ULONG ObpAccessProtectCloseBit = 0x80000000;

/* KphDuplicateObject
 * 
 * Duplicates a handle from the source process to the target process.
 */
NTSTATUS KphDuplicateObject(
    __in HANDLE SourceProcessHandle,
    __in HANDLE SourceHandle,
    __in_opt HANDLE TargetProcessHandle,
    __out_opt PHANDLE TargetHandle,
    __in ACCESS_MASK DesiredAccess,
    __in ULONG HandleAttributes,
    __in ULONG Options,
    __in KPROCESSOR_MODE AccessMode
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
    
    /* Target handle is optional. */
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
    
    /* Call the internal function. */
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
    __in PEPROCESS Process,
    __in PEX_ENUM_HANDLE_CALLBACK EnumHandleProcedure,
    __inout PVOID Context,
    __out_opt PHANDLE Handle
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
        Handle
        );
    ObDereferenceProcessHandleTable(Process);
    
    return result;
}

/* KphQueryProcessHandles
 * 
 * Queries a process handle table.
 */
NTSTATUS KphQueryProcessHandles(
    __in HANDLE ProcessHandle,
    __out_bcount_opt(BufferLength) PPROCESS_HANDLE_INFORMATION Buffer,
    __in_opt ULONG BufferLength,
    __out_opt PULONG ReturnLength,
    __in KPROCESSOR_MODE AccessMode
    )
{
    NTSTATUS status;
    BOOLEAN result;
    PEPROCESS processObject;
    OBP_QUERY_PROCESS_HANDLES_DATA context;
    
    /* Probe buffer contents. */
    if (AccessMode != KernelMode)
    {
        __try
        {
            if (Buffer)
                ProbeForWrite(Buffer, BufferLength, 1);
            if (ReturnLength)
                ProbeForWrite(ReturnLength, sizeof(ULONG), 1);
        }
        __except (EXCEPTION_EXECUTE_HANDLER)
        {
            return GetExceptionCode();
        }
    }
    
    /* Reference the process object. */
    status = ObReferenceObjectByHandle(
        ProcessHandle,
        PROCESS_QUERY_INFORMATION,
        *PsProcessType,
        KernelMode,
        &processObject,
        NULL
        );
    
    if (!NT_SUCCESS(status))
        return status;
    
    /* Initialize the enumeration context. */
    context.Buffer = Buffer;
    context.BufferLength = BufferLength;
    context.CurrentIndex = 0;
    context.Status = STATUS_SUCCESS;
    
    /* Enumerate the handles. */
    result = KphEnumProcessHandleTable(
        processObject,
        KphpQueryProcessHandlesEnumCallback,
        &context,
        NULL
        );
    ObDereferenceObject(processObject);
    
    /* Write the number of handles (if we have a buffer). */
    if (
        Buffer && 
        BufferLength >= sizeof(ULONG)
        )
    {
        __try
        {
            Buffer->HandleCount = context.CurrentIndex;
        }
        __except (EXCEPTION_EXECUTE_HANDLER)
        {
            return GetExceptionCode();
        }
    }
    
    /* Supply the return length if the caller wanted it. */
    if (ReturnLength)
    {
        __try
        {
            /* CurrentIndex should contain the number of handles, so we simply multiply it 
               by the size of PROCESS_HANDLE. */
            *ReturnLength = sizeof(ULONG) + context.CurrentIndex * sizeof(PROCESS_HANDLE);
        }
        __except (EXCEPTION_EXECUTE_HANDLER)
        {
            return GetExceptionCode();
        }
    }
    
    return context.Status;
}

/* KphpQueryProcessHandlesEnumCallback
 * 
 * The callback for KphEnumProcessHandleTable, used by 
 * KphQueryProcessHandles.
 */
BOOLEAN KphpQueryProcessHandlesEnumCallback(
    __inout PHANDLE_TABLE_ENTRY HandleTableEntry,
    __in HANDLE Handle,
    __in POBP_QUERY_PROCESS_HANDLES_DATA Context
    )
{
    PROCESS_HANDLE handleInfo;
    PPROCESS_HANDLE_INFORMATION buffer = Context->Buffer;
    ULONG i = Context->CurrentIndex;
    
    handleInfo.Handle = Handle;
    handleInfo.Object = ObpDecodeObject(HandleTableEntry->Object);
    handleInfo.GrantedAccess = ObpDecodeGrantedAccess(HandleTableEntry->GrantedAccess);
    handleInfo.HandleAttributes = ObpGetHandleAttributes(HandleTableEntry);
    
    /* Only write if we have a buffer and have not exceeded the buffer length. */
    if (
        buffer && 
        (sizeof(ULONG) + i * sizeof(PROCESS_HANDLE)) <= Context->BufferLength
        )
    {
        __try
        {
            memcpy(&buffer->Handles[i], &handleInfo, sizeof(PROCESS_HANDLE));
        }
        __except (EXCEPTION_EXECUTE_HANDLER)
        {
            /* Report an error. */
            if (Context->Status == STATUS_SUCCESS)
                Context->Status = GetExceptionCode();
        }
    }
    else
    {
        /* Report that the buffer is too small. */
        if (Context->Status == STATUS_SUCCESS)
            Context->Status = STATUS_BUFFER_TOO_SMALL;
    }
    
    /* Increment the index regardless of whether the information was written; 
       this will allow KphQueryProcessHandles to report the correct return length. */
    Context->CurrentIndex++;
    
    return FALSE;
}

/* KphSetHandleGrantedAccess
 * 
 * Sets the granted access of a handle.
 */
NTSTATUS KphSetHandleGrantedAccess(
    __in PEPROCESS Process,
    __in HANDLE Handle,
    __in ACCESS_MASK GrantedAccess
    )
{
    BOOLEAN result;
    OBP_SET_HANDLE_GRANTED_ACCESS_DATA context;
    
    context.Handle = Handle;
    context.GrantedAccess = GrantedAccess;
    
    result = KphEnumProcessHandleTable(
        Process,
        KphpSetHandleGrantedAccessEnumCallback,
        &context,
        NULL
        );
    
    return result ? STATUS_SUCCESS : STATUS_UNSUCCESSFUL;
}

/* KphpSetHandleGrantedAccessEnumCallback
 * 
 * The callback for KphEnumProcessHandleTable, used by 
 * KphSetHandleGrantedAccess.
 */
BOOLEAN KphpSetHandleGrantedAccessEnumCallback(
    __inout PHANDLE_TABLE_ENTRY HandleTableEntry,
    __in HANDLE Handle,
    __in POBP_SET_HANDLE_GRANTED_ACCESS_DATA Context
    )
{
    if (Handle != Context->Handle)
        return FALSE;
    
    HandleTableEntry->GrantedAccess = Context->GrantedAccess;
    
    return TRUE;
}

/* ObDereferenceProcessHandleTable
 * 
 * Allows the process to terminate.
 */
VOID ObDereferenceProcessHandleTable(
    __in PEPROCESS Process
    )
{
    KphReleaseProcessRundownProtection(Process);
}

/* ObDuplicateObject
 * 
 * Duplicates a handle from the source process to the target process.
 * WARNING: This does not actually duplicate a handle. It simply 
 * re-opens an object in another process.
 */
NTSTATUS ObDuplicateObject(
    __in PEPROCESS SourceProcess,
    __in_opt PEPROCESS TargetProcess,
    __in HANDLE SourceHandle,
    __out_opt PHANDLE TargetHandle,
    __in ACCESS_MASK DesiredAccess,
    __in ULONG HandleAttributes,
    __in ULONG Options,
    __in KPROCESSOR_MODE AccessMode
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
        CHAR auxData[AUX_ACCESS_DATA_SIZE];
        
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
            KernelMode,
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
    __in PEPROCESS Process
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
