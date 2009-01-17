/*
 * Process Hacker Driver - 
 *   main header file
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

#ifndef KPROCESSHACKER_H
#define KPROCESSHACKER_H

#define DEBUG

#ifdef DEBUG
#define dprintf DbgPrint
#else
#define dprintf
#endif

#include <ntifs.h>

typedef struct _SYSTEM_HANDLE_INFORMATION
{
    ULONG ProcessId;
    UCHAR ObjectTypeNumber;
    UCHAR Flags;
    USHORT Handle;
    PVOID Object;
    ACCESS_MASK GrantedAccess;
} SYSTEM_HANDLE_INFORMATION, *PSYSTEM_HANDLE_INFORMATION;

#define KPH_TAG 'KPHT'
/* I like 0x9999. */
#define KPH_DEVICE_TYPE (0x9999)
#define KPH_DEVICE_NAME (L"\\Device\\KProcessHacker")
#define KPH_DEVICE_DOS_NAME (L"\\DosDevices\\KProcessHacker")

#define KPH_CTL_CODE(x) CTL_CODE(KPH_DEVICE_TYPE, 0x800 + x, METHOD_BUFFERED, FILE_ANY_ACCESS)
#define KPH_READ KPH_CTL_CODE(0)
#define KPH_WRITE KPH_CTL_CODE(1)
#define KPH_GETOBJECTNAME KPH_CTL_CODE(2)
#define KPH_TERMINATEPROCESS KPH_CTL_CODE(3)
#define KPH_GETKISERVICETABLE KPH_CTL_CODE(4)
#define KPH_GIVEKISERVICETABLE KPH_CTL_CODE(5)
#define KPH_SETKISERVICETABLEENTRY KPH_CTL_CODE(6)

NTSTATUS KPHCreate(PDEVICE_OBJECT DeviceObject, PIRP Irp);
NTSTATUS KPHClose(PDEVICE_OBJECT DeviceObject, PIRP Irp);
NTSTATUS KPHIoControl(PDEVICE_OBJECT DeviceObject, PIRP Irp);
NTSTATUS KPHRead(PDEVICE_OBJECT DeviceObject, PIRP Irp);
NTSTATUS KPHWrite(PDEVICE_OBJECT DeviceObject, PIRP Irp);
NTSTATUS KPHUnsupported(PDEVICE_OBJECT DeviceObject, PIRP Irp);
BOOLEAN IsStringNullTerminated(PCHAR String, int Length);

typedef NTSTATUS (*_ZwTerminateProcess)(HANDLE Process, int ExitCode);

#define PROCESS_TERMINATE                  (0x0001)  
#define PROCESS_CREATE_THREAD              (0x0002)  
#define PROCESS_SET_SESSIONID              (0x0004)  
#define PROCESS_VM_OPERATION               (0x0008)  
#define PROCESS_VM_READ                    (0x0010)  
#define PROCESS_VM_WRITE                   (0x0020)  
#define PROCESS_DUP_HANDLE                 (0x0040)  
#define PROCESS_CREATE_PROCESS             (0x0080)  
#define PROCESS_SET_QUOTA                  (0x0100)  
#define PROCESS_SET_INFORMATION            (0x0200)  
#define PROCESS_QUERY_INFORMATION          (0x0400)  
#define PROCESS_SUSPEND_RESUME             (0x0800)  
#define PROCESS_QUERY_LIMITED_INFORMATION  (0x1000)  
#if (NTDDI_VERSION >= NTDDI_LONGHORN)
#define PROCESS_ALL_ACCESS        (STANDARD_RIGHTS_REQUIRED | SYNCHRONIZE | \
                                   0xFFFF)
#else
#define PROCESS_ALL_ACCESS        (STANDARD_RIGHTS_REQUIRED | SYNCHRONIZE | \
                                   0xFFF)
#endif

#endif