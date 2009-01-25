/*
 * Process Hacker Driver - 
 *   custom versions of certain APIs - header file
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

#ifndef _KPH_NT_H
#define _KPH_NT_H

#include "kprocesshacker.h"
#include "kernel_types.h"

NTSTATUS KphOpenProcess(
    PHANDLE ProcessHandle,
    ACCESS_MASK DesiredAccess,
    KPROCESSOR_MODE AccessMode,
    int ProcessId
    );

NTSTATUS KphTerminateProcess(
    HANDLE ProcessHandle,
    NTSTATUS ExitStatus
    );

NTKERNELAPI NTSTATUS NTAPI SeCreateAccessState(
    PACCESS_STATE AccessState,
    PAUX_ACCESS_DATA AuxData,
    ACCESS_MASK DesiredAccess,
    PGENERIC_MAPPING Mapping
    );

NTKERNELAPI VOID NTAPI SeDeleteAccessState(
    PACCESS_STATE AccessState
    );

#endif