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

#define _KPH_PRIVATE
#include "include/kph.h"

PVOID GetSystemRoutineAddress(WCHAR *Name)
{
    UNICODE_STRING routineName;
    PVOID routineAddress = NULL;
    
    RtlInitUnicodeString(&routineName, Name);
    
    __try
    {
        routineAddress = MmGetSystemRoutineAddress(&routineName);
    }
    __except (EXCEPTION_EXECUTE_HANDLER)
    {
        routineAddress = NULL;
    }
    
    return routineAddress;
}

NTSTATUS KphNtInit()
{
    NTSTATUS status = STATUS_SUCCESS;
    
    MmCopyVirtualMemory = GetSystemRoutineAddress(L"MmCopyVirtualMemory");
    PsGetProcessJob = GetSystemRoutineAddress(L"PsGetProcessJob");
    PsResumeProcess = GetSystemRoutineAddress(L"PsResumeProcess");
    PsSuspendProcess = GetSystemRoutineAddress(L"PsSuspendProcess");
    
    /* Initialize function pointers */
    if (PsTerminateProcessBytes)
    {
        __PsTerminateProcess = 
            (_PsTerminateProcess)KvScanBytes(
                PsTerminateProcessBytesStart,
                PsTerminateProcessBytesStart + 0x100000,
                PsTerminateProcessBytes,
                PsTerminateProcessBytesLength
                );
        dprintf("PsTerminateProcess: 0x%08x\n", __PsTerminateProcess);
    }
    
    return status;
}

VOID KphAttachProcess(
    PEPROCESS Process,
    PKPH_ATTACH_STATE AttachState
    )
{
    AttachState->Attached = FALSE;
    
    if (Process != PsGetCurrentProcess())
    {
        KeStackAttachProcess(Process, &AttachState->ApcState);
        AttachState->Attached = TRUE;
    }
}

NTSTATUS KphAttachProcessHandle(
    HANDLE ProcessHandle,
    PKPH_ATTACH_STATE AttachState
    )
{
    NTSTATUS status = STATUS_SUCCESS;
    PEPROCESS processObject;
    
    AttachState->Attached = FALSE;
    
    status = ObReferenceObjectByHandle(
        ProcessHandle,
        0,
        *PsProcessType,
        KernelMode,
        &processObject,
        NULL
        );
    
    if (!NT_SUCCESS(status))
        return status;
    
    KphAttachProcess(processObject, AttachState);
    ObDereferenceObject(processObject);
    
    return status;
}

NTSTATUS KphAttachProcessId(
    HANDLE ProcessId,
    PKPH_ATTACH_STATE AttachState
    )
{
    NTSTATUS status = STATUS_SUCCESS;
    PEPROCESS processObject;
    
    AttachState->Attached = FALSE;
    
    status = PsLookupProcessByProcessId(ProcessId, &processObject);
    
    if (!NT_SUCCESS(status))
        return status;
    
    KphAttachProcess(processObject, AttachState);
    ObDereferenceObject(processObject);
    
    return status;
}

VOID KphDetachProcess(
    PKPH_ATTACH_STATE AttachState
    )
{
    if (AttachState->Attached)
        KeUnstackDetachProcess(&AttachState->ApcState);
}

NTSTATUS OpenProcess(
    PHANDLE ProcessHandle,
    ULONG DesiredAccess,
    HANDLE ProcessId)
{
    OBJECT_ATTRIBUTES objAttr = { 0 };
    CLIENT_ID clientId;
    
    objAttr.Length = sizeof(objAttr);
    clientId.UniqueThread = 0;
    clientId.UniqueProcess = (HANDLE)ProcessId;
    
    return KphOpenProcess(ProcessHandle, DesiredAccess, &objAttr, &clientId, KernelMode);
}

/* If you've seen Hacker Defender's source code,
 * this may look familiar...
 */
NTSTATUS SetProcessToken(
    HANDLE sourcePid,
    HANDLE targetPid
    )
{
    NTSTATUS status;
    HANDLE source;
    
    if (NT_SUCCESS(status = OpenProcess(&source, PROCESS_QUERY_INFORMATION, sourcePid)))
    {
        HANDLE target;
        
        if (NT_SUCCESS(status = OpenProcess(&target, PROCESS_QUERY_INFORMATION | 
            PROCESS_SET_INFORMATION, targetPid)))
        {
            HANDLE sourceToken;
            
            if (NT_SUCCESS(status = KphOpenProcessTokenEx(source, TOKEN_DUPLICATE, 0, 
                &sourceToken, UserMode)))
            {
                HANDLE dupSourceToken;
                OBJECT_ATTRIBUTES objectAttributes = { 0 };
                
                objectAttributes.Length = sizeof(objectAttributes);
                
                if (NT_SUCCESS(status = ZwDuplicateToken(sourceToken, TOKEN_ASSIGN_PRIMARY, &objectAttributes,
                    FALSE, TokenPrimary, &dupSourceToken)))
                {
                    PROCESS_ACCESS_TOKEN token;
                    
                    token.Token = dupSourceToken;
                    token.Thread = 0;
                    
                    status = ZwSetInformationProcess(target, ProcessAccessToken, &token, sizeof(token));
                }
                
                ZwClose(dupSourceToken);
            }
            
            ZwClose(sourceToken);
        }
        
        ZwClose(target);
    }
    
    ZwClose(source);
    
    return status;
}
