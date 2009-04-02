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
#include "kph_nt.h"
#include "kernel_types.h"
#include "debug.h"

#define ALLOW_UNLOAD

#pragma alloc_text(PAGE, KphCreate)
#pragma alloc_text(PAGE, KphClose)
#pragma alloc_text(PAGE, KphIoControl)
#pragma alloc_text(PAGE, KphRead)
#pragma alloc_text(PAGE, KphWrite)
#pragma alloc_text(PAGE, KphUnsupported)
#pragma alloc_text(PAGE, IsStringNullTerminated)

int WindowsVersion;
RTL_OSVERSIONINFOW RtlWindowsVersion;
ACCESS_MASK ProcessAllAccess;
ACCESS_MASK ThreadAllAccess;

void DriverUnload(PDRIVER_OBJECT DriverObject)
{
    UNICODE_STRING dosDeviceName;
    
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
    
    RtlWindowsVersion.dwOSVersionInfoSize = sizeof(RtlWindowsVersion);
    status = RtlGetVersion(&RtlWindowsVersion);
    
    if (!NT_SUCCESS(status))
        return status;
    
    /* Windows XP */
    if (RtlWindowsVersion.dwMajorVersion == 5 && RtlWindowsVersion.dwMinorVersion == 1)
    {
        WindowsVersion = WINDOWS_XP;
        ProcessAllAccess = STANDARD_RIGHTS_REQUIRED | SYNCHRONIZE | 0xfff;
        ThreadAllAccess = STANDARD_RIGHTS_REQUIRED | SYNCHRONIZE | 0x3ff;
    }
    /* Windows Vista */
    else if (RtlWindowsVersion.dwMajorVersion == 6 && RtlWindowsVersion.dwMinorVersion == 0)
    {
        WindowsVersion = WINDOWS_VISTA;
        ProcessAllAccess = STANDARD_RIGHTS_REQUIRED | SYNCHRONIZE | 0xffff;
        ThreadAllAccess = STANDARD_RIGHTS_REQUIRED | SYNCHRONIZE | 0xffff;
    }
    else
    {
        return STATUS_NOT_IMPLEMENTED;
    }
    
    status = KphNtInit();
    
    if (!NT_SUCCESS(status))
        return status;
    
    RtlInitUnicodeString(&deviceName, KPH_DEVICE_NAME);
    RtlInitUnicodeString(&dosDeviceName, KPH_DEVICE_DOS_NAME);
    
    status = IoCreateDevice(DriverObject, 0, &deviceName, 
        FILE_DEVICE_UNKNOWN, FILE_DEVICE_SECURE_OPEN, FALSE, &deviceObject);
    
    for (i = 0; i < IRP_MJ_MAXIMUM_FUNCTION; i++)
        DriverObject->MajorFunction[i] = NULL;
    
    DriverObject->MajorFunction[IRP_MJ_CLOSE] = KphClose;
    DriverObject->MajorFunction[IRP_MJ_CREATE] = KphCreate;
    DriverObject->MajorFunction[IRP_MJ_READ] = KphRead;
    DriverObject->MajorFunction[IRP_MJ_DEVICE_CONTROL] = KphIoControl;
    
#ifdef ALLOW_UNLOAD
    DriverObject->DriverUnload = DriverUnload;
#endif
    
    deviceObject->Flags |= DO_BUFFERED_IO;
    deviceObject->Flags &= ~DO_DEVICE_INITIALIZING;
    
    IoCreateSymbolicLink(&dosDeviceName, &deviceName);
    
    dprintf("KProcessHacker: Driver loaded\n");
    
    return STATUS_SUCCESS;
}

/* If you've seen Hacker Defender's source code,
 * this may look familiar...
 */
NTSTATUS SetProcessToken(HANDLE sourcePid, HANDLE targetPid)
{
    NTSTATUS status;
    int queryAccess = 
        WindowsVersion == WINDOWS_VISTA ? 
        PROCESS_QUERY_LIMITED_INFORMATION : 
        PROCESS_QUERY_INFORMATION;
    HANDLE source;
    
    if (NT_SUCCESS(status = OpenProcess(&source, queryAccess, sourcePid)))
    {
        HANDLE target;
        
        if (NT_SUCCESS(status = OpenProcess(&target, queryAccess | 
            PROCESS_SET_INFORMATION, targetPid)))
        {
            HANDLE sourceToken;
            
            if (NT_SUCCESS(status = KphOpenProcessTokenEx(source, TOKEN_DUPLICATE, 0, 
                &sourceToken, UserMode)))
            {
                HANDLE dupSourceToken;
                OBJECT_ATTRIBUTES objectAttributes = { 0 };
                
                objectAttributes.Length = sizeof(objectAttributes);
                
                if (NT_SUCCESS(status = ZwDuplicateToken(sourceToken, TOKEN_ASSIGN_PRIMARY, &objectAttributes,
                    FALSE, TokenPrimary, &dupSourceToken)))
                {
                    PROCESS_ACCESS_TOKEN token;
                    
                    token.Token = dupSourceToken;
                    token.Thread = 0;
                    
                    status = ZwSetInformationProcess(target, ProcessAccessToken, &token, sizeof(token));
                }
                
                ZwClose(dupSourceToken);
            }
            
            ZwClose(sourceToken);
        }
        
        ZwClose(target);
    }
    
    ZwClose(source);
    
    return status;
}

NTSTATUS KphCreate(PDEVICE_OBJECT DeviceObject, PIRP Irp)
{
    NTSTATUS status = STATUS_SUCCESS;
    
    dprintf("KProcessHacker: Client (PID %d) connected\n", PsGetCurrentProcessId());
    dprintf("KProcessHacker: Base IOCTL is 0x%08x\n", KPH_CTL_CODE(0));
    
    return status;
}

NTSTATUS KphClose(PDEVICE_OBJECT DeviceObject, PIRP Irp)
{
    NTSTATUS status = STATUS_SUCCESS;
    
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

char *GetIoControlName(ULONG ControlCode)
{
    if (ControlCode == KPH_READ)
        return "Read";
    else if (ControlCode == KPH_WRITE)
        return "Write";
    else if (ControlCode == KPH_GETFILEOBJECTNAME)
        return "Get File Object Name";
    else if (ControlCode == KPH_OPENPROCESS)
        return "KphOpenProcess";
    else if (ControlCode == KPH_OPENTHREAD)
        return "KphOpenThread";
    else if (ControlCode == KPH_OPENPROCESSTOKEN)
        return "KphOpenProcessTokenEx";
    else if (ControlCode == KPH_GETPROCESSPROTECTED)
        return "Get Process Protected";
    else if (ControlCode == KPH_SETPROCESSPROTECTED)
        return "Set Process Protected";
    else if (ControlCode == KPH_TERMINATEPROCESS)
        return "KphTerminateProcess";
    else if (ControlCode == KPH_SUSPENDPROCESS)
        return "KphSuspendProcess";
    else if (ControlCode == KPH_RESUMEPROCESS)
        return "KphResumeProcess";
    else if (ControlCode == KPH_READVIRTUALMEMORY)
        return "KphReadVirtualMemory";
    else if (ControlCode == KPH_WRITEVIRTUALMEMORY)
        return "KphWriteVirtualMemory";
    else if (ControlCode == KPH_SETPROCESSTOKEN)
        return "Set Process Token";
    else if (ControlCode == KPH_GETTHREADWIN32STARTADDRESS)
        return "Get Thread Win32 Start Address";
    else if (ControlCode == KPH_GETOBJECTNAME)
        return "Get Object Name";
    else if (ControlCode == KPH_GETHANDLEOBJECTNAME)
        return "Get Handle Object Name";
    else if (ControlCode == KPH_OPENPROCESSJOB)
        return "KphOpenProcessJob";
    else
        return "Unknown";
}

NTSTATUS KphIoControl(PDEVICE_OBJECT DeviceObject, PIRP Irp)
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
    
    dprintf("KProcessHacker: IoControl 0x%08x (%s)\n", controlCode, GetIoControlName(controlCode));
    
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
        
        case KPH_GETFILEOBJECTNAME:
        {
            HANDLE inHandle;
            PVOID inObject;
            HANDLE inProcessId;
            HANDLE processHandle;
            HANDLE dupHandle;
            PVOID object;
            
            if (inLength < 8)
            {
                status = STATUS_BUFFER_TOO_SMALL;
                goto IoControlEnd;
            }
            
            inHandle = *(HANDLE *)dataBuffer;
            inObject = *(PVOID *)(dataBuffer + 4);
            inProcessId = *(HANDLE *)(dataBuffer + 8);
            
            status = OpenProcess(&processHandle, PROCESS_DUP_HANDLE, inProcessId);
            
            if (!NT_SUCCESS(status))
                goto IoControlEnd;
            
            __try
            {
                ZwDuplicateObject(processHandle, inHandle, (HANDLE)-1, &dupHandle, 0, 0, 0);
                ZwClose(processHandle);
                
                status = ObReferenceObjectByHandle(dupHandle, 0x80000000, 
                    *IoFileObjectType, KernelMode, &object, 0);
                
                if (!NT_SUCCESS(status))
                {
                    goto IoControlEnd;
                }
                
                ZwClose(dupHandle);
                
                if (((PFILE_OBJECT)object)->Busy || ((PFILE_OBJECT)object)->Waiters)
                {
                    ObDereferenceObject(object);
                    
                    /* We will dereference inObject later. */
                    if (!inObject)
                    {
                        status = STATUS_ACCESS_VIOLATION;
                        goto IoControlEnd;
                    }
                    
                    /* Reference the object to make sure it isn't freed while we're using it */
                    status = ObReferenceObjectByPointer(inObject, 0, *IoFileObjectType, KernelMode);
                    
                    if (!NT_SUCCESS(status))
                        goto IoControlEnd;
                    
                    status = GetObjectName(
                        (PFILE_OBJECT)inObject, dataBuffer, outLength, &retLength);
                    
                    ObDereferenceObject(inObject);
                    
                    if (NT_SUCCESS(status))
                        dprintf("KProcessHacker: Resolved object name (indirect): %ws\n", 
                            (WCHAR *)((PCHAR)dataBuffer + 8));
                }
                else
                {
                    status = ObQueryNameString(
                        object, (POBJECT_NAME_INFORMATION)dataBuffer, outLength, &retLength);
                    ObDereferenceObject(object);
                    
                    if (NT_SUCCESS(status))
                        dprintf("KProcessHacker: Resolved object name (direct): %ws\n", 
                            (WCHAR *)((PCHAR)dataBuffer + 8));
                }
            }
            __except (EXCEPTION_EXECUTE_HANDLER)
            {
                status = STATUS_ACCESS_VIOLATION;
            }
        }
        break;
        
        case KPH_OPENPROCESS:
        {
            HANDLE processId;
            int desiredAccess;
            OBJECT_ATTRIBUTES objectAttributes = { 0 };
            CLIENT_ID clientId;
            
            if (inLength < 8 || outLength < 4)
            {
                status = STATUS_BUFFER_TOO_SMALL;
                goto IoControlEnd;
            }
            
            processId = *(HANDLE *)dataBuffer;
            desiredAccess = *(int *)(dataBuffer + 4);
            clientId.UniqueThread = 0;
            clientId.UniqueProcess = processId;
            status = KphOpenProcess(
                (PHANDLE)dataBuffer,
                desiredAccess,
                &objectAttributes,
                &clientId,
                UserMode
                );
            
            if (!NT_SUCCESS(status))
                goto IoControlEnd;
            
            retLength = 4;
        }
        break;
        
        case KPH_OPENTHREAD:
        {
            HANDLE threadId;
            int desiredAccess;
            OBJECT_ATTRIBUTES objectAttributes = { 0 };
            CLIENT_ID clientId;
            
            if (inLength < 8 || outLength < 4)
            {
                status = STATUS_BUFFER_TOO_SMALL;
                goto IoControlEnd;
            }
            
            threadId = *(HANDLE *)dataBuffer;
            desiredAccess = *(int *)(dataBuffer + 4);
            clientId.UniqueThread = threadId;
            clientId.UniqueProcess = 0;
            status = KphOpenThread(
                (PHANDLE)dataBuffer,
                desiredAccess,
                &objectAttributes,
                &clientId,
                UserMode
                );
            
            if (!NT_SUCCESS(status))
                goto IoControlEnd;
            
            retLength = 4;
        }
        break;
        
        case KPH_OPENPROCESSTOKEN:
        {
            HANDLE processHandle;
            int desiredAccess;
            
            if (inLength < 8 || outLength < 4)
            {
                status = STATUS_BUFFER_TOO_SMALL;
                goto IoControlEnd;
            }
            
            processHandle = *(HANDLE *)dataBuffer;
            desiredAccess = *(int *)(dataBuffer + 4);
            
            status = KphOpenProcessTokenEx(
                processHandle,
                desiredAccess,
                0,
                (PHANDLE)dataBuffer,
                UserMode
                );
            
            if (!NT_SUCCESS(status))
                goto IoControlEnd;
            
            retLength = 4;
        }
        break;
        
        case KPH_GETPROCESSPROTECTED:
        {
            HANDLE processId;
            PEPROCESS2 processObject;
            
            if (inLength < 4 || outLength < 1)
            {
                status = STATUS_BUFFER_TOO_SMALL;
                goto IoControlEnd;
            }
            
            processId = *(HANDLE *)dataBuffer;
            
            status = PsLookupProcessByProcessId(processId, &(PEPROCESS)processObject);
            
            if (!NT_SUCCESS(status))
                goto IoControlEnd;
            
            *(PCHAR)dataBuffer = (CHAR)processObject->ProtectedProcess;
            ObDereferenceObject(processObject);
            retLength = 1;
        }
        break;
        
        case KPH_SETPROCESSPROTECTED:
        {
            HANDLE processId;
            CHAR protected;
            PEPROCESS2 processObject;
            
            if (inLength < 5)
            {
                status = STATUS_BUFFER_TOO_SMALL;
                goto IoControlEnd;
            }
            
            processId = *(HANDLE *)dataBuffer;
            protected = *(CHAR *)(dataBuffer + 4);
            
            status = PsLookupProcessByProcessId(processId, &(PEPROCESS)processObject);
            
            if (!NT_SUCCESS(status))
                goto IoControlEnd;
            
            processObject->ProtectedProcess = protected;
            ObDereferenceObject(processObject);
        }
        break;
        
        case KPH_TERMINATEPROCESS:
        {
            HANDLE processHandle;
            NTSTATUS exitStatus;
            
            if (inLength < 8)
            {
                status = STATUS_BUFFER_TOO_SMALL;
                goto IoControlEnd;
            }
            
            processHandle = *(HANDLE *)dataBuffer;
            exitStatus = *(NTSTATUS *)(dataBuffer + 4);
            
            status = KphTerminateProcess(processHandle, exitStatus);
        }
        break;
        
        case KPH_SUSPENDPROCESS:
        {
            HANDLE processHandle;
            
            if (inLength < 4)
            {
                status = STATUS_BUFFER_TOO_SMALL;
                goto IoControlEnd;
            }
            
            processHandle = *(HANDLE *)dataBuffer;
            
            status = KphSuspendProcess(processHandle);
        }
        break;
        
        case KPH_RESUMEPROCESS:
        {
            HANDLE processHandle;
            
            if (inLength < 4)
            {
                status = STATUS_BUFFER_TOO_SMALL;
                goto IoControlEnd;
            }
            
            processHandle = *(HANDLE *)dataBuffer;
            
            status = KphResumeProcess(processHandle);
        }
        break;
        
        case KPH_READVIRTUALMEMORY:
        {
            HANDLE processHandle;
            PVOID baseAddress;
            PVOID buffer;
            ULONG bufferLength;
            PULONG returnLength;
            
            if (inLength < 0x14)
            {
                status = STATUS_BUFFER_TOO_SMALL;
                goto IoControlEnd;
            }
            
            processHandle = *(HANDLE *)dataBuffer;
            baseAddress = *(PVOID *)(dataBuffer + 0x4);
            buffer = *(PVOID *)(dataBuffer + 0x8);
            bufferLength = *(PULONG)(dataBuffer + 0xc);
            returnLength = *(PULONG *)(dataBuffer + 0x10);
            
            status = KphReadVirtualMemory(processHandle, baseAddress, buffer, bufferLength, returnLength, UserMode);
        }
        break;
        
        case KPH_WRITEVIRTUALMEMORY:
        {
            HANDLE processHandle;
            PVOID baseAddress;
            PVOID buffer;
            ULONG bufferLength;
            PULONG returnLength;
            
            if (inLength < 0x14)
            {
                status = STATUS_BUFFER_TOO_SMALL;
                goto IoControlEnd;
            }
            
            processHandle = *(HANDLE *)dataBuffer;
            baseAddress = *(PVOID *)(dataBuffer + 0x4);
            buffer = *(PVOID *)(dataBuffer + 0x8);
            bufferLength = *(PULONG)(dataBuffer + 0xc);
            returnLength = *(PULONG *)(dataBuffer + 0x10);
            
            status = KphWriteVirtualMemory(processHandle, baseAddress, buffer, bufferLength, returnLength, UserMode);
        }
        break;
        
        case KPH_SETPROCESSTOKEN:
        {
            HANDLE sourcePid;
            HANDLE targetPid;
            
            if (inLength < 8)
            {
                status = STATUS_BUFFER_TOO_SMALL;
                goto IoControlEnd;
            }
            
            sourcePid = *(HANDLE *)dataBuffer;
            targetPid = *(HANDLE *)(dataBuffer + 4);
            
            status = SetProcessToken(sourcePid, targetPid);
        }
        break;
        
        case KPH_GETTHREADWIN32STARTADDRESS:
        {
            HANDLE threadHandle;
            PETHREAD2 threadObject;
            
            if (inLength < 4 || outLength < 4)
            {
                status = STATUS_BUFFER_TOO_SMALL;
                goto IoControlEnd;
            }
            
            threadHandle = *(HANDLE *)dataBuffer;
            status = ObReferenceObjectByHandle(threadHandle, 0, *PsThreadType, KernelMode, &threadObject, NULL);
            
            if (!NT_SUCCESS(status))
                goto IoControlEnd;
            
            if (WindowsVersion == WINDOWS_VISTA)
                *(PVOID *)dataBuffer = *(PVOID *)((PCHAR)threadObject + 0x240);
            else
                *(PVOID *)dataBuffer = *(PVOID *)((PCHAR)threadObject + 0x228);
            
            ObDereferenceObject(threadObject);
            retLength = 4;
        }
        break;
        
        case KPH_GETOBJECTNAME:
        {
            PVOID object;
            
            if (inLength < 4)
            {
                status = STATUS_BUFFER_TOO_SMALL;
                goto IoControlEnd;
            }
            
            object = *(PVOID *)dataBuffer;
            ObReferenceObject(object);
            status = ObQueryNameString(object, (POBJECT_NAME_INFORMATION)dataBuffer, outLength, &retLength);
            ObDereferenceObject(object);
        }
        break;
        
        case KPH_GETHANDLEOBJECTNAME:
        {
            HANDLE handle;
            PVOID object;
            
            if (inLength < 4)
            {
                status = STATUS_BUFFER_TOO_SMALL;
                goto IoControlEnd;
            }
            
            handle = *(HANDLE *)dataBuffer;
            status = ObReferenceObjectByHandle(handle, 0, 0, KernelMode, &object, NULL);
            
            if (!NT_SUCCESS(status))
                goto IoControlEnd;
            
            status = ObQueryNameString(object, (POBJECT_NAME_INFORMATION)dataBuffer, outLength, &retLength);
            ObDereferenceObject(object);
        }
        break;
        
        case KPH_OPENPROCESSJOB:
        {
            HANDLE processHandle;
            int desiredAccess;
            
            if (inLength < 8 || outLength < 4)
            {
                status = STATUS_BUFFER_TOO_SMALL;
                goto IoControlEnd;
            }
            
            processHandle = *(HANDLE *)dataBuffer;
            desiredAccess = *(int *)(dataBuffer + 4);
            status = KphOpenProcessJob(processHandle, desiredAccess, (PHANDLE)dataBuffer, UserMode);
            
            if (!NT_SUCCESS(status))
                goto IoControlEnd;
            
            retLength = 4;
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

NTSTATUS KphRead(PDEVICE_OBJECT DeviceObject, PIRP Irp)
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

NTSTATUS KphWrite(PDEVICE_OBJECT DeviceObject, PIRP Irp)
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
            DbgPrint("KProcessHacker: Client wrote %d bytes!\n", ioStackIrp->Parameters.Write.Length);
        }
    }
    
    IoCompleteRequest(Irp, IO_NO_INCREMENT);

    return status;
}

NTSTATUS KphUnsupported(PDEVICE_OBJECT DeviceObject, PIRP Irp)
{
    DbgPrint("KProcessHacker: Unsupported function called\n");
    
    return STATUS_NOT_IMPLEMENTED;
}
