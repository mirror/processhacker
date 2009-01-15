/*
 * Process Hacker Driver - 
 *   source file
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

#include "kprocesshacker.h"

#pragma alloc_text(PAGE, KPHCreate) 
#pragma alloc_text(PAGE, KPHClose) 
#pragma alloc_text(PAGE, KPHIoControl) 
#pragma alloc_text(PAGE, KPHRead)
#pragma alloc_text(PAGE, KPHWrite)
#pragma alloc_text(PAGE, KPHUnsupported)
#pragma alloc_text(PAGE, IsStringNullTerminated)

void DriverUnload(PDRIVER_OBJECT DriverObject)
{
    UNICODE_STRING dosDeviceName;
    
    RtlInitUnicodeString(&dosDeviceName, KPROCESSHACKER_DEVICE_DOS_NAME);
    IoDeleteSymbolicLink(&dosDeviceName);
    IoDeleteDevice(DriverObject->DeviceObject);
    
    DbgPrint("KProcessHacker: Driver unloaded");
}

NTSTATUS DriverEntry(PDRIVER_OBJECT DriverObject, PUNICODE_STRING RegistryPath)
{
    NTSTATUS status = STATUS_SUCCESS;
    int i;
    PDEVICE_OBJECT deviceObject = NULL;
    UNICODE_STRING deviceName, dosDeviceName;
    
    RtlInitUnicodeString(&deviceName, KPROCESSHACKER_DEVICE_NAME);
    RtlInitUnicodeString(&dosDeviceName, KPROCESSHACKER_DEVICE_DOS_NAME);
    
    status = IoCreateDevice(DriverObject, 0, &deviceName, 
        FILE_DEVICE_UNKNOWN, FILE_DEVICE_SECURE_OPEN, TRUE, &deviceObject);
    
    for (i = 0; i < IRP_MJ_MAXIMUM_FUNCTION; i++)
        DriverObject->MajorFunction[i] = NULL;
    
    DriverObject->MajorFunction[IRP_MJ_CLOSE] = KPHClose;
    DriverObject->MajorFunction[IRP_MJ_CREATE] = KPHCreate;
    DriverObject->MajorFunction[IRP_MJ_DEVICE_CONTROL] = KPHIoControl;
    DriverObject->MajorFunction[IRP_MJ_READ] = KPHRead;
    DriverObject->MajorFunction[IRP_MJ_WRITE] = KPHWrite;
    
    DriverObject->DriverUnload = DriverUnload;
    
    deviceObject->Flags |= DO_BUFFERED_IO;
    deviceObject->Flags &= ~DO_DEVICE_INITIALIZING;
    
    IoCreateSymbolicLink(&dosDeviceName, &deviceName);
	
    DbgPrint("KProcessHacker: Driver loaded");
	
    return STATUS_SUCCESS;
}

NTSTATUS KPHCreate(PDEVICE_OBJECT DeviceObject, PIRP Irp)
{
    NTSTATUS status = STATUS_SUCCESS;

    return status;
}

NTSTATUS KPHClose(PDEVICE_OBJECT DeviceObject, PIRP Irp)
{
    NTSTATUS status = STATUS_SUCCESS;

    return status;
}

NTSTATUS KPHIoControl(PDEVICE_OBJECT DeviceObject, PIRP Irp)
{
    NTSTATUS status = STATUS_SUCCESS;

    return status;
}

NTSTATUS KPHRead(PDEVICE_OBJECT DeviceObject, PIRP Irp)
{
    NTSTATUS status = STATUS_SUCCESS;

    return status;
}

NTSTATUS KPHWrite(PDEVICE_OBJECT DeviceObject, PIRP Irp)
{
    NTSTATUS status = STATUS_SUCCESS;
    PIO_STACK_LOCATION ioStackIrp = NULL;
    PCHAR writeDataBuffer;
    
    ioStackIrp = IoGetCurrentIrpStackLocation(Irp);
    
    if (ioStackIrp != NULL)
    {
        writeDataBuffer = (PCHAR)Irp->AssociatedIrp.SystemBuffer;
    
        if (writeDataBuffer != NULL)
        {
           if (IsStringNullTerminated(writeDataBuffer, ioStackIrp->Parameters.Write.Length))
           {
                DbgPrint(writeDataBuffer);
           }
        }
    }

    return status;
}

NTSTATUS KPHUnsupported(PDEVICE_OBJECT DeviceObject, PIRP Irp)
{
    DbgPrint("KProcessHacker: Unsupported function called");
    
    return STATUS_NOT_IMPLEMENTED;
}

BOOLEAN IsStringNullTerminated(PCHAR String, int Length)
{
    int i;
    
    for (i = 0; i < Length; i++)
        if (String[i] == 0)
            return TRUE;

    return FALSE;
}
