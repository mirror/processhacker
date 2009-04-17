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

#include "include/kph.h"

_PsGetProcessJob PsGetProcessJob = NULL;
_PsSuspendProcess PsSuspendProcess = NULL;
_PsResumeProcess PsResumeProcess = NULL;
_MmCopyVirtualMemory MmCopyVirtualMemory = NULL;

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
    
    return status;
}

NTSTATUS OpenProcess(PHANDLE ProcessHandle, int DesiredAccess, HANDLE ProcessId)
{
    OBJECT_ATTRIBUTES objAttr = { 0 };
    CLIENT_ID clientId;
    
    objAttr.Length = sizeof(objAttr);
    clientId.UniqueThread = 0;
    clientId.UniqueProcess = (HANDLE)ProcessId;
    
    return KphOpenProcess(ProcessHandle, DesiredAccess, &objAttr, &clientId, KernelMode);
}
