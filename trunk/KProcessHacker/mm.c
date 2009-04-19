/*
 * Process Hacker Driver - 
 *   memory manager
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
#include "include/mm.h"

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
    
    if (!MmCopyVirtualMemory)
        return STATUS_NOT_SUPPORTED;
    
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
    
    if (!MmCopyVirtualMemory)
        return STATUS_NOT_SUPPORTED;
    
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
