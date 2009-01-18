/*
 * Process Hacker Driver - 
 *   main driver code
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

/*
 * This driver has two main purposes:
 * 
 * 1. To enable Process Hacker to (transparently) do anything without the 
 *    restriction of hooked calls, and 
 * 2. To enable Process Hacker to retrieve handle names for any file object.
 */

#include "kprocesshacker.h"
#include "ssdt.h"
#include "hooks.h"

/* It's never safe to allow a hook driver to unload, but it should be 
 * used when debugging to save time.
 */
//#define ALLOW_UNLOAD

#pragma alloc_text(PAGE, KPHCreate) 
#pragma alloc_text(PAGE, KPHClose) 
#pragma alloc_text(PAGE, KPHIoControl) 
#pragma alloc_text(PAGE, KPHRead)
#pragma alloc_text(PAGE, KPHWrite)
#pragma alloc_text(PAGE, KPHUnsupported)
#pragma alloc_text(PAGE, IsStringNullTerminated)

int ClientPID = -1;
PVOID *OrigKiServiceTable = NULL;
extern int CurrentCallCount;

PVOID GetSystemRoutineAddress(WCHAR *Name)
{
    UNICODE_STRING unicodeName;
    
    RtlInitUnicodeString(&unicodeName, Name);
    
    return MmGetSystemRoutineAddress(&unicodeName);
}

void DriverUnload(PDRIVER_OBJECT DriverObject)
{
    LARGE_INTEGER waitTime;
    UNICODE_STRING dosDeviceName;
    int i;
    
    KPHUnhook();
    SsdtDeinit();
    
    /* wait for any syscalls to complete, otherwise it's a BSOD. */
    waitTime.QuadPart = -((signed __int64)20000000); /* 2 seconds */
    KeDelayExecutionThread(KernelMode, FALSE, &waitTime);
    
    if (OrigKiServiceTable != NULL)
    {
        ExFreePool(OrigKiServiceTable);
        OrigKiServiceTable = NULL;
    }
    
    RtlInitUnicodeString(&dosDeviceName, KPH_DEVICE_DOS_NAME);
    IoDeleteSymbolicLink(&dosDeviceName);
    IoDeleteDevice(DriverObject->DeviceObject);
    
    dprintf("KProcessHacker: Driver unloaded\n");
}

NTSTATUS DriverEntry(PDRIVER_OBJECT DriverObject, PUNICODE_STRING RegistryPath)
{
    NTSTATUS status = STATUS_SUCCESS;
    int i;
    PDEVICE_OBJECT deviceObject = NULL;
    UNICODE_STRING deviceName, dosDeviceName;
    
    status = SsdtInit();
    
    if (status != STATUS_SUCCESS)
        return status;
    
    KPHHook();
    
    RtlInitUnicodeString(&deviceName, KPH_DEVICE_NAME);
    RtlInitUnicodeString(&dosDeviceName, KPH_DEVICE_DOS_NAME);
    
    status = IoCreateDevice(DriverObject, 0, &deviceName, 
        FILE_DEVICE_UNKNOWN, FILE_DEVICE_SECURE_OPEN, TRUE, &deviceObject);
    
    for (i = 0; i < IRP_MJ_MAXIMUM_FUNCTION; i++)
        DriverObject->MajorFunction[i] = NULL;
    
    DriverObject->MajorFunction[IRP_MJ_CLOSE] = KPHClose;
    DriverObject->MajorFunction[IRP_MJ_CREATE] = KPHCreate;
    DriverObject->MajorFunction[IRP_MJ_READ] = KPHRead;
    DriverObject->MajorFunction[IRP_MJ_DEVICE_CONTROL] = KPHIoControl;
    
#ifdef ALLOW_UNLOAD
    DriverObject->DriverUnload = DriverUnload;
#endif
    
    deviceObject->Flags |= DO_BUFFERED_IO;
    deviceObject->Flags &= ~DO_DEVICE_INITIALIZING;
    
    IoCreateSymbolicLink(&dosDeviceName, &deviceName);
    
    dprintf("KProcessHacker: Driver loaded\n");
    
    return STATUS_SUCCESS;
}

NTSTATUS KPHCreate(PDEVICE_OBJECT DeviceObject, PIRP Irp)
{
    NTSTATUS status = STATUS_SUCCESS;
    
    ClientPID = PsGetProcessId(PsGetCurrentProcess());
    dprintf("KProcessHacker: Client (PID %d) connected\n", ClientPID);
    dprintf("KProcessHacker: Base IOCTL is 0x%08x\n", KPH_CTL_CODE(0));
    
    return status;
}

NTSTATUS KPHClose(PDEVICE_OBJECT DeviceObject, PIRP Irp)
{
    NTSTATUS status = STATUS_SUCCESS;
    
    ClientPID = -1;
    dprintf("KProcessHacker: Client disconnected\n");
    
    return status;
}

/* from YAPM */
NTSTATUS GetObjectName(PFILE_OBJECT lpObject, PVOID lpBuffer, ULONG nBufferLength, PULONG lpReturnLength)
{
    ULONG nObjectName = 0;
    PFILE_OBJECT lpRelated;
    PVOID lpName = lpBuffer;
    
    if (lpObject->DeviceObject)
    {
        ObQueryNameString((PVOID)lpObject->DeviceObject, lpName, nBufferLength, lpReturnLength);
        (PCHAR)lpName += *lpReturnLength - 2; /* minus the null terminator */
        nBufferLength -= *lpReturnLength - 2;
    }
    else
    {
        /* it's a UNICODE_STRING. we need to subtract the space 
        Length and MaximumLength take up. */
        (PCHAR)lpName += 4;
        nBufferLength -= 4;
    }
    
    if (!lpObject->FileName.Buffer)
        return STATUS_SUCCESS;
    
    lpRelated = lpObject;
    
    do
    {
        nObjectName += lpRelated->FileName.Length;
        lpRelated = lpRelated->RelatedFileObject;
    }
    while (lpRelated);
    
    *lpReturnLength += nObjectName;
    
    if (nObjectName > nBufferLength)
    {
        return STATUS_BUFFER_TOO_SMALL;
    }
    
    (PCHAR)lpName += nObjectName;
    *(unsigned short*)lpName = 0;
    
    lpRelated = lpObject;
    do
    {
        (PCHAR)lpName -= lpRelated->FileName.Length;
        RtlCopyMemory(lpName, lpRelated->FileName.Buffer, lpRelated->FileName.Length);
        lpRelated = lpRelated->RelatedFileObject;
    }
    while (lpRelated);
    
    return STATUS_SUCCESS;
}

NTSTATUS KPHIoControl(PDEVICE_OBJECT DeviceObject, PIRP Irp)
{
    NTSTATUS status = STATUS_SUCCESS;
    PIO_STACK_LOCATION ioStackIrp = NULL;
    PCHAR dataBuffer;
    int controlCode;
    unsigned int inLength, outLength;
    int retLength;
    
    Irp->IoStatus.Status = STATUS_SUCCESS;
    Irp->IoStatus.Information = 0;
    
    ioStackIrp = IoGetCurrentIrpStackLocation(Irp);
    
    if (ioStackIrp == NULL)
    {
        status = STATUS_INTERNAL_ERROR;
        goto IoControlEnd;
    }
    
    dataBuffer = (PCHAR)Irp->AssociatedIrp.SystemBuffer;
    
    if (dataBuffer == NULL)
    {
        status = STATUS_INTERNAL_ERROR;
        goto IoControlEnd;
    }
    
    inLength = ioStackIrp->Parameters.DeviceIoControl.InputBufferLength;
    outLength = ioStackIrp->Parameters.DeviceIoControl.OutputBufferLength;
    controlCode = ioStackIrp->Parameters.DeviceIoControl.IoControlCode;
    
    dprintf("KProcessHacker: IoControl 0x%08x\n", controlCode);
    
    switch (controlCode)
    {
        case KPH_READ:
        {
            PVOID address;
            
            if (inLength < 4)
            {
                status = STATUS_BUFFER_TOO_SMALL;
                goto IoControlEnd;
            }
            
            address = *(PVOID *)dataBuffer;
            
            __try
            {
                RtlCopyMemory(dataBuffer, address, outLength);
                retLength = outLength;
            }
            __except (EXCEPTION_EXECUTE_HANDLER)
            {
                status = STATUS_ACCESS_VIOLATION;
                goto IoControlEnd;
            }
        }
        break;
        
        case KPH_WRITE:
        {
            PVOID address;
            
            if (inLength < 4)
            {
                status = STATUS_BUFFER_TOO_SMALL;
                goto IoControlEnd;
            }
            
            address = *(PVOID *)dataBuffer;
            
            /* any interrupts happening while we're writing is... bad. */
            __asm cli;
            
            __try
            {
                RtlCopyMemory(address, dataBuffer + 4, inLength - 4);
                retLength = inLength;
            }
            __except (EXCEPTION_EXECUTE_HANDLER)
            {
                __asm sti;
                status = STATUS_ACCESS_VIOLATION;
                goto IoControlEnd;
            }
            
            __asm sti;
        }
        break;
        
        case KPH_GETOBJECTNAME:
        {
            HANDLE inHandle;
            int inObject;
            int inProcessId;
            HANDLE processHandle;
            HANDLE dupHandle;
            PVOID object;
            
            if (inLength < 8)
            {
                status = STATUS_BUFFER_TOO_SMALL;
                goto IoControlEnd;
            }
            
            inHandle = *(HANDLE *)dataBuffer;
            inObject = *(int *)(dataBuffer + 4);
            inProcessId = *(int *)(dataBuffer + 8);
            
            {
                OBJECT_ATTRIBUTES objAttr = { 0 };
                CLIENT_ID clientId;
                
                objAttr.Length = sizeof(objAttr);
                clientId.UniqueThread = 0;
                clientId.UniqueProcess = inProcessId;
                
                status = ZwOpenProcess(&processHandle, 0x40, &objAttr, &clientId);
                
                if (status != STATUS_SUCCESS)
                    goto IoControlEnd;
            }
            
            __try
            {
                ZwDuplicateObject(processHandle, inHandle, (HANDLE)-1, &dupHandle, 0, 0, 0);
                ObReferenceObjectByHandle(dupHandle, 0x80000000, 0, 0, &object, 0);
                
                if (((PFILE_OBJECT)object)->Busy || ((PFILE_OBJECT)object)->Waiters)
                {
                    ObDereferenceObject(object);
                    ZwClose(dupHandle);
                    status = GetObjectName(
                        (PFILE_OBJECT)inObject, dataBuffer, outLength, &retLength);
                    
                    if (status == STATUS_SUCCESS)
                        dprintf("KProcessHacker: Resolved object name (indirect): %ws", 
                            (WCHAR *)((PCHAR)dataBuffer + 8));
                }
                else
                {
                    status = ObQueryNameString(
                        object, (POBJECT_NAME_INFORMATION)dataBuffer, outLength, &retLength);
                    ObDereferenceObject(object);
                    ZwClose(dupHandle);
                    
                    if (status == STATUS_SUCCESS)
                        dprintf("KProcessHacker: Resolved object name (direct): %ws", 
                            (WCHAR *)((PCHAR)dataBuffer + 8));
                }
            }
            __except (EXCEPTION_EXECUTE_HANDLER)
            {
                status = STATUS_ACCESS_VIOLATION;
            }
            
            ZwClose(processHandle);
        }
        break;
        
        case KPH_GETKISERVICETABLE:
        {
            if (outLength < SsdtGetCount() * 4)
            {
                status = STATUS_BUFFER_TOO_SMALL;
                goto IoControlEnd;
            }
            
            RtlCopyMemory(dataBuffer, SsdtGetServiceTable(), SsdtGetCount() * 4);
            retLength = SsdtGetCount() * 4;
        }
        break;
        
        case KPH_GIVEKISERVICETABLE:
        {
            if (inLength < SsdtGetCount() * 4)
            {
                status = STATUS_BUFFER_TOO_SMALL;
                retLength = SsdtGetCount() * 4;
                goto IoControlEnd;
            }
            
            if (OrigKiServiceTable != NULL)
            {
                ExFreePool(OrigKiServiceTable);
                OrigKiServiceTable = NULL;
            }
            
            OrigKiServiceTable = ExAllocatePoolWithTag(NonPagedPool, SsdtGetCount() * 4, KPH_TAG);
            
            if (OrigKiServiceTable == NULL)
            {
                status = STATUS_NO_MEMORY;
                goto IoControlEnd;
            }
            
            RtlCopyMemory(OrigKiServiceTable, dataBuffer, SsdtGetCount() * 4);
            
            dprintf("KProcessHacker: Got %d service table entries\n", SsdtGetCount());
            dprintf("KProcessHacker: ZwTerminateProcess points to 0x%08x\n",
                SsdtGetEntryByCall(ZwTerminateProcess));
            dprintf("KProcessHacker: NtTerminateProcess is 0x%08x\n", 
                OrigKiServiceTable[SYSCALL_INDEX(ZwTerminateProcess)]);
        }
        break;
        
        case KPH_SETKISERVICETABLEENTRY:
        {
            int index;
            PVOID function;
            
            if (inLength < SsdtGetCount() * 4)
            {
                status = STATUS_BUFFER_TOO_SMALL;
                goto IoControlEnd;
            }
            
            index = *(int *)dataBuffer;
            function = *(PVOID *)(dataBuffer + 4);
            
            SsdtModifyEntryByIndex(index, function);
        }
        break;
        
        default:
        {
            dprintf("KProcessHacker: unrecognized IOCTL code 0x%08x\n", controlCode);
            status = STATUS_INVALID_DEVICE_REQUEST;
        }
        break;
    }
    
IoControlEnd:
    Irp->IoStatus.Information = retLength;
    Irp->IoStatus.Status = status;
    dprintf("KProcessHacker: IOCTL 0x%08x result was 0x%08x\n", controlCode, status);
    IoCompleteRequest(Irp, IO_NO_INCREMENT);
    
    return status;
}

NTSTATUS KPHRead(PDEVICE_OBJECT DeviceObject, PIRP Irp)
{
    NTSTATUS status = STATUS_SUCCESS;
    PIO_STACK_LOCATION ioStackIrp = NULL;
    int retLength = 0;
    
    ioStackIrp = IoGetCurrentIrpStackLocation(Irp);
    
    if (ioStackIrp != NULL)
    {
        PCHAR readDataBuffer = (PCHAR)Irp->AssociatedIrp.SystemBuffer;
        int readLength = ioStackIrp->Parameters.Read.Length;
        
        if (readDataBuffer != NULL)
        {
            dprintf("KProcessHacker: Client read %d bytes!\n", readLength);
            
            if (readLength == 4)
            {
                *(int *)readDataBuffer = KPH_CTL_CODE(0);
                retLength = 4;
            }
            else
            {
                status = STATUS_INFO_LENGTH_MISMATCH;
            }
        }
    }
    
    Irp->IoStatus.Information = retLength;
    Irp->IoStatus.Status = status;
    IoCompleteRequest(Irp, IO_NO_INCREMENT);
    
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
            int i;
            
            DbgPrint("KProcessHacker: Client wrote %d bytes!\n", ioStackIrp->Parameters.Write.Length);
            DbgPrint("KProcessHacker: Client wrote \"");
            
            for (i = 0; i < ioStackIrp->Parameters.Write.Length; i++)
                DbgPrint("%c", writeDataBuffer[i]);
            
            DbgPrint("\"\n");
        }
    }
    
    IoCompleteRequest(Irp, IO_NO_INCREMENT);

    return status;
}

NTSTATUS KPHUnsupported(PDEVICE_OBJECT DeviceObject, PIRP Irp)
{
    DbgPrint("KProcessHacker: Unsupported function called\n");
    
    return STATUS_NOT_IMPLEMENTED;
}
