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
#define KPH_OPENPROCESS KPH_CTL_CODE(3)
#define KPH_OPENTHREAD KPH_CTL_CODE(4)
#define KPH_OPENPROCESSTOKEN KPH_CTL_CODE(5)
#define KPH_GETPROCESSPROTECTED KPH_CTL_CODE(6)
#define KPH_SETPROCESSPROTECTED KPH_CTL_CODE(7)
#define KPH_TERMINATEPROCESS KPH_CTL_CODE(8)
#define KPH_SUSPENDPROCESS KPH_CTL_CODE(9)
#define KPH_RESUMEPROCESS KPH_CTL_CODE(10)
#define KPH_READPROCESSMEMORY KPH_CTL_CODE(11)
#define KPH_SETPROCESSTOKEN KPH_CTL_CODE(12)
#define KPH_GETTHREADWIN32STARTADDRESS KPH_CTL_CODE(13)

NTSTATUS KphCreate(PDEVICE_OBJECT DeviceObject, PIRP Irp);
NTSTATUS KphClose(PDEVICE_OBJECT DeviceObject, PIRP Irp);
NTSTATUS KphIoControl(PDEVICE_OBJECT DeviceObject, PIRP Irp);
NTSTATUS KphRead(PDEVICE_OBJECT DeviceObject, PIRP Irp);
NTSTATUS KphWrite(PDEVICE_OBJECT DeviceObject, PIRP Irp);
NTSTATUS KphUnsupported(PDEVICE_OBJECT DeviceObject, PIRP Irp);
BOOLEAN IsStringNullTerminated(PCHAR String, int Length);

NTSTATUS NTAPI ZwOpenProcessToken(
    HANDLE ProcessHandle,
    ACCESS_MASK DesiredAccess,
    PHANDLE TokenHandle
    );

NTSTATUS NTAPI ZwSetInformationProcess(
    HANDLE ProcessHandle,
    PROCESSINFOCLASS ProcessInformationClass,
    PVOID ProcessInformation,
    ULONG ProcessInformationLength
    );

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