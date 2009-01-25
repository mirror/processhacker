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

NTSTATUS KphOpenProcess(
    PHANDLE ProcessHandle,
    ACCESS_MASK DesiredAccess,
    KPROCESSOR_MODE AccessMode,
    int ProcessId
    )
{
    NTSTATUS status = STATUS_SUCCESS;
    ACCESS_STATE accessState;
    AUX_ACCESS_DATA auxData;
    PEPROCESS processObject;
    HANDLE processHandle;
    
    if (ProcessHandle == NULL)
    {
        return STATUS_INVALID_PARAMETER;
    }
    
    /* ReactOS code cleared this bit up for me :) */
    status = SeCreateAccessState(
        &accessState,
        &auxData,
        DesiredAccess,
        (PGENERIC_MAPPING)((char *)PsProcessType + 52)
        );
    
    if (status != STATUS_SUCCESS)
    {
        return status;
    }
    
    /* just give our client full access. */
    /* hopefully our client isn't a virus... */
    accessState.PreviouslyGrantedAccess |= PROCESS_ALL_ACCESS;
    accessState.RemainingDesiredAccess = 0;
    
    status = PsLookupProcessByProcessId((HANDLE)ProcessId, &processObject);
    
    if (status != STATUS_SUCCESS)
    {
        SeDeleteAccessState(&accessState);
        return status;
    }
    
    status = ObOpenObjectByPointer(
        processObject,
        0,
        &accessState,
        0,
        *PsProcessType,
        AccessMode,
        &processHandle
        );
    
    SeDeleteAccessState(&accessState);
    ObDereferenceObject(processObject);
    
    if (status == STATUS_SUCCESS)
    {
        *ProcessHandle = processHandle;
    }
    
    return status;
}
