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
#define KPH_GETKISERVICETABLE KPH_CTL_CODE(3)
#define KPH_GIVEKISERVICETABLE KPH_CTL_CODE(4)
#define KPH_SETKISERVICETABLEENTRY KPH_CTL_CODE(5)
#define KPH_GETSERVICELIMIT KPH_CTL_CODE(6)
#define KPH_RESTOREKISERVICETABLE KPH_CTL_CODE(7)
#define KPH_OPENPROCESS KPH_CTL_CODE(8)
#define KPH_GETPROCESSPROTECTED KPH_CTL_CODE(9)
#define KPH_SETPROCESSPROTECTED KPH_CTL_CODE(10)
#define KPH_OPENTHREAD KPH_CTL_CODE(11)

NTSTATUS KPHCreate(PDEVICE_OBJECT DeviceObject, PIRP Irp);
NTSTATUS KPHClose(PDEVICE_OBJECT DeviceObject, PIRP Irp);
NTSTATUS KPHIoControl(PDEVICE_OBJECT DeviceObject, PIRP Irp);
NTSTATUS KPHRead(PDEVICE_OBJECT DeviceObject, PIRP Irp);
NTSTATUS KPHWrite(PDEVICE_OBJECT DeviceObject, PIRP Irp);
NTSTATUS KPHUnsupported(PDEVICE_OBJECT DeviceObject, PIRP Irp);
BOOLEAN IsStringNullTerminated(PCHAR String, int Length);

#endif