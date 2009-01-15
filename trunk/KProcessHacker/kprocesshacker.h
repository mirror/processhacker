/*
 * Process Hacker Driver - 
 *   header file
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

#include <ntddk.h>

#define KPROCESSHACKER_DEVICE_NAME L"\\Device\\KProcessHacker"
#define KPROCESSHACKER_DEVICE_DOS_NAME L"\\DosDevices\\KProcessHacker"

NTSTATUS KPHCreate(PDEVICE_OBJECT DeviceObject, PIRP Irp);
NTSTATUS KPHClose(PDEVICE_OBJECT DeviceObject, PIRP Irp);
NTSTATUS KPHIoControl(PDEVICE_OBJECT DeviceObject, PIRP Irp);
NTSTATUS KPHRead(PDEVICE_OBJECT DeviceObject, PIRP Irp);
NTSTATUS KPHWrite(PDEVICE_OBJECT DeviceObject, PIRP Irp);
NTSTATUS KPHUnsupported(PDEVICE_OBJECT DeviceObject, PIRP Irp);
BOOLEAN IsStringNullTerminated(PCHAR String, int Length);

#endif