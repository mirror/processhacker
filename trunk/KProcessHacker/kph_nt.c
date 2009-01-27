/*
 * Process Hacker Driver - 
 *   custom versions of certain APIs
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

extern ACCESS_MASK ProcessAllAccess;
extern ACCESS_MASK ThreadAllAccess;
extern POBJECT_TYPE *SeTokenObjectType;

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
    
    status = ObReferenceObjectByHandle(ProcessHandle, 0, 0, KernelMode, &processObject, 0);
    
    if (!NT_SUCCESS(status))
        return status;
    
    tokenObject = PsReferencePrimaryToken(processObject);
    ObDereferenceObject(processObject);
    
    status = ObOpenObjectByPointer(
        tokenObject,
        ObjectAttributes,
        0,
        DesiredAccess,
        *SeTokenObjectType,
        AccessMode,
        &tokenHandle
        );
    ObDereferenceObject(tokenObject);
    
    if (NT_SUCCESS(status))
        *TokenHandle = tokenHandle;
    
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
    
    status = PsSuspendProcess(processObject);
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
    
    status = PsResumeProcess(processObject);
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
    
    status = ObReferenceObjectByHandle(ProcessHandle, 0, 0, KernelMode, &processObject, 0);
    
    if (!NT_SUCCESS(status))
        return status;
    
    /* status = PsTerminateProcess(processObject, ExitStatus); */
    ObDereferenceObject(processObject);
    
    return status;
}
