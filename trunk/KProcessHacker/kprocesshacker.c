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

#include "include/kprocesshacker.h"
#include "include/debug.h"
#include "include/kph.h"
#include "include/ps.h"
#include "include/version.h"

#pragma alloc_text(PAGE, KphCreate)
#pragma alloc_text(PAGE, KphClose)
#pragma alloc_text(PAGE, KphIoControl)
#pragma alloc_text(PAGE, KphRead)
#pragma alloc_text(PAGE, KphUnsupported)

VOID DriverUnload(PDRIVER_OBJECT DriverObject)
{
    UNICODE_STRING dosDeviceName;
    
    RtlInitUnicodeString(&dosDeviceName, KPH_DEVICE_DOS_NAME);
    IoDeleteSymbolicLink(&dosDeviceName);
    IoDeleteDevice(DriverObject->DeviceObject);
    
    dprintf("Driver unloaded\n");
}

NTSTATUS DriverEntry(PDRIVER_OBJECT DriverObject, PUNICODE_STRING RegistryPath)
{
    NTSTATUS status = STATUS_SUCCESS;
    int i;
    PDEVICE_OBJECT deviceObject = NULL;
    UNICODE_STRING deviceName, dosDeviceName;
    
    /* Initialize version information */
    status = KvInit();
    if (!NT_SUCCESS(status))
        return status;
    
    /* Initialize NT KPH */
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
    DriverObject->DriverUnload = DriverUnload;
    
    deviceObject->Flags |= DO_BUFFERED_IO;
    deviceObject->Flags &= ~DO_DEVICE_INITIALIZING;
    
    IoCreateSymbolicLink(&dosDeviceName, &deviceName);
    
    dprintf("Driver loaded\n");
    
    return STATUS_SUCCESS;
}

NTSTATUS KphCreate(PDEVICE_OBJECT DeviceObject, PIRP Irp)
{
    NTSTATUS status = STATUS_SUCCESS;
    
    dprintf("Client (PID %d) connected\n", PsGetCurrentProcessId());
    dprintf("Base IOCTL is 0x%08x\n", KPH_CTL_CODE(0));
    
    return status;
}

NTSTATUS KphClose(PDEVICE_OBJECT DeviceObject, PIRP Irp)
{
    NTSTATUS status = STATUS_SUCCESS;
    
    dprintf("Client (PID %d) disconnected\n", PsGetCurrentProcessId());
    
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

PCHAR GetIoControlName(ULONG ControlCode)
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
    else if (ControlCode == KPH_GETCONTEXTTHREAD)
        return "KphGetContextThread";
    else if (ControlCode == KPH_SETCONTEXTTHREAD)
        return "KphSetContextThread";
    else if (ControlCode == KPH_GETTHREADWIN32THREAD)
        return "KphGetThreadWin32Thread";
    else if (ControlCode == KPH_DUPLICATEOBJECT)
        return "KphDuplicateObject";
    else if (ControlCode == KPH_ZWQUERYOBJECT)
        return "ZwQueryObject";
    else if (ControlCode == KPH_GETPROCESSID)
        return "KphGetProcessId";
    else if (ControlCode == KPH_GETTHREADID)
        return "KphGetThreadId";
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
    int retLength = 0;
    
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
    
    dprintf("IoControl 0x%08x (%s)\n", controlCode, GetIoControlName(controlCode));
    
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
            RtlCopyMemory(address, dataBuffer + 4, inLength - 4);
            __asm sti;
            
            retLength = inLength;
        }
        break;
        
        case KPH_GETFILEOBJECTNAME:
        {
            HANDLE inHandle;
            PVOID inObject;
            HANDLE inProcessId;
            KPH_ATTACH_STATE attachState;
            PVOID object;
            
            if (inLength < 0xc)
            {
                status = STATUS_BUFFER_TOO_SMALL;
                goto IoControlEnd;
            }
            
            inHandle = *(HANDLE *)dataBuffer;
            inObject = *(PVOID *)(dataBuffer + 4);
            inProcessId = *(HANDLE *)(dataBuffer + 8);
            
            status = KphAttachProcessId(inProcessId, &attachState);
            
            if (!NT_SUCCESS(status))
                goto IoControlEnd;
            
            status = ObReferenceObjectByHandle(inHandle, 0, 
                *IoFileObjectType, KernelMode, &object, 0);
            KphDetachProcess(&attachState);
            
            if (!NT_SUCCESS(status))
            {
                goto IoControlEnd;
            }
            
            __try
            {
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
                        dprintf("Resolved object name (indirect): %ws\n", 
                            (WCHAR *)((PCHAR)dataBuffer + 8));
                }
                else
                {
                    status = ObQueryNameString(
                        object, (POBJECT_NAME_INFORMATION)dataBuffer, outLength, &retLength);
                    ObDereferenceObject(object);
                    
                    if (NT_SUCCESS(status))
                        dprintf("Resolved object name (direct): %ws\n", 
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
            PEPROCESS processObject;
            
            if (inLength < 4 || outLength < 1)
            {
                status = STATUS_BUFFER_TOO_SMALL;
                goto IoControlEnd;
            }
            
            processId = *(HANDLE *)dataBuffer;
            
            status = PsLookupProcessByProcessId(processId, &processObject);
            
            if (!NT_SUCCESS(status))
                goto IoControlEnd;
            
            *(PCHAR)dataBuffer = 
                (CHAR)GET_BIT(
                    *(PULONG)KVOFF(processObject, OffEpProtectedProcessOff),
                    OffEpProtectedProcessBit
                    );
            ObDereferenceObject(processObject);
            retLength = 1;
        }
        break;
        
        case KPH_SETPROCESSPROTECTED:
        {
            HANDLE processId;
            CHAR protected;
            PEPROCESS processObject;
            
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
            
            if (protected)
            {
                SET_BIT(
                    *(PULONG)KVOFF(processObject, OffEpProtectedProcessOff),
                    OffEpProtectedProcessBit
                    );
            }
            else
            {
                CLEAR_BIT(
                    *(PULONG)KVOFF(processObject, OffEpProtectedProcessOff),
                    OffEpProtectedProcessBit
                    );
            }
            
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
            PVOID startAddress;
            
            if (inLength < 4 || outLength < 4)
            {
                status = STATUS_BUFFER_TOO_SMALL;
                goto IoControlEnd;
            }
            
            threadHandle = *(HANDLE *)dataBuffer;
            status = ObReferenceObjectByHandle(threadHandle, 0, *PsThreadType, KernelMode, &threadObject, NULL);
            
            if (!NT_SUCCESS(status))
                goto IoControlEnd;
            
            /* Get the Win32StartAddress */
            startAddress = *(PVOID *)KVOFF(threadObject, OffEtWin32StartAddress);
            /* If that failed, get the StartAddress */
            if (!startAddress)
                startAddress = *(PVOID *)KVOFF(threadObject, OffEtStartAddress);
            
            *(PVOID *)dataBuffer = startAddress;
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
            HANDLE processHandle;
            HANDLE handle;
            KPH_ATTACH_STATE attachState;
            PVOID object;
            
            if (inLength < 8)
            {
                status = STATUS_BUFFER_TOO_SMALL;
                goto IoControlEnd;
            }
            
            processHandle = *(HANDLE *)dataBuffer;
            handle = *(HANDLE *)(dataBuffer + 4);
            
            status = KphAttachProcessHandle(processHandle, &attachState);
            
            if (!NT_SUCCESS(status))
                goto IoControlEnd;
            
            status = ObReferenceObjectByHandle(handle, 0, NULL, KernelMode, &object, NULL);
            KphDetachProcess(&attachState);
            
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
        
        case KPH_GETCONTEXTTHREAD:
        {
            HANDLE threadHandle;
            PCONTEXT threadContext;
            
            if (inLength < 8)
            {
                status = STATUS_BUFFER_TOO_SMALL;
                goto IoControlEnd;
            }
            
            threadHandle = *(HANDLE *)dataBuffer;
            threadContext = *(PCONTEXT *)(dataBuffer + 4);
            status = KphGetContextThread(threadHandle, threadContext, UserMode);
        }
        break;
        
        case KPH_SETCONTEXTTHREAD:
        {
            HANDLE threadHandle;
            PCONTEXT threadContext;
            
            if (inLength < 8)
            {
                status = STATUS_BUFFER_TOO_SMALL;
                goto IoControlEnd;
            }
            
            threadHandle = *(HANDLE *)dataBuffer;
            threadContext = *(PCONTEXT *)(dataBuffer + 4);
            status = KphSetContextThread(threadHandle, threadContext, UserMode);
        }
        break;
        
        case KPH_GETTHREADWIN32THREAD:
        {
            HANDLE threadHandle;
            
            if (inLength < 4 || outLength < 4)
            {
                status = STATUS_BUFFER_TOO_SMALL;
                goto IoControlEnd;
            }
            
            threadHandle = *(HANDLE *)dataBuffer;
            status = KphGetThreadWin32Thread(threadHandle, (PVOID *)dataBuffer, KernelMode);
            
            if (!NT_SUCCESS(status))
                goto IoControlEnd;
            
            retLength = 4;
        }
        break;
        
        case KPH_DUPLICATEOBJECT:
        {
            HANDLE sourceProcessHandle;
            HANDLE sourceHandle;
            HANDLE targetProcessHandle;
            PHANDLE targetHandle;
            ACCESS_MASK desiredAccess;
            ULONG handleAttributes;
            ULONG options;
            
            if (inLength < 7 * 4)
            {
                status = STATUS_BUFFER_TOO_SMALL;
                goto IoControlEnd;
            }
            
            sourceProcessHandle = *(HANDLE *)dataBuffer;
            sourceHandle = *(HANDLE *)(dataBuffer + 0x4);
            targetProcessHandle = *(HANDLE *)(dataBuffer + 0x8);
            targetHandle = *(PHANDLE *)(dataBuffer + 0xc);
            desiredAccess = *(ACCESS_MASK *)(dataBuffer + 0x10);
            handleAttributes = *(ULONG *)(dataBuffer + 0x14);
            options = *(ULONG *)(dataBuffer + 0x18);
            
            status = KphDuplicateObject(
                sourceProcessHandle,
                sourceHandle,
                targetProcessHandle,
                targetHandle,
                desiredAccess,
                handleAttributes,
                options,
                UserMode
                );
        }
        break;
        
        case KPH_ZWQUERYOBJECT:
        {
            NTSTATUS status2 = STATUS_SUCCESS;
            HANDLE processHandle;
            HANDLE handle;
            ULONG objectInformationClass;
            KPH_ATTACH_STATE attachState;
            
            if (inLength < 0xc || outLength < 12)
            {
                status = STATUS_BUFFER_TOO_SMALL;
                goto IoControlEnd;
            }
            
            processHandle = *(HANDLE *)dataBuffer;
            handle = *(HANDLE *)(dataBuffer + 4);
            objectInformationClass = *(ULONG *)(dataBuffer + 8);
            
            status = KphAttachProcessHandle(processHandle, &attachState);
            
            if (!NT_SUCCESS(status))
                goto IoControlEnd;
            
            status2 = ZwQueryObject(
                handle,
                objectInformationClass,
                (PCHAR)dataBuffer + 12,
                outLength - 12,
                &retLength
                );
            KphDetachProcess(&attachState);
            
            *(PULONG)(dataBuffer + 4) = retLength;
            *(PCHAR *)(dataBuffer + 8) = (PCHAR)dataBuffer + 12;
            
            if (NT_SUCCESS(status2))
                retLength += 12;
            else
                retLength = 12;
            
            *(PNTSTATUS)dataBuffer = status2;
        }
        break;
        
        case KPH_GETPROCESSID:
        {
            HANDLE processHandle;
            HANDLE handle;
            KPH_ATTACH_STATE attachState;
            
            if (inLength < 8 || outLength < 4)
            {
                status = STATUS_BUFFER_TOO_SMALL;
                goto IoControlEnd;
            }
            
            processHandle = *(HANDLE *)dataBuffer;
            handle = *(HANDLE *)(dataBuffer + 4);
            
            status = KphAttachProcessHandle(processHandle, &attachState);
            
            if (!NT_SUCCESS(status))
                goto IoControlEnd;
            
            *(PHANDLE)dataBuffer = KphGetProcessId(handle);
            KphDetachProcess(&attachState);
            retLength = 4;
        }
        break;
        
        case KPH_GETTHREADID:
        {
            HANDLE processHandle;
            HANDLE handle;
            KPH_ATTACH_STATE attachState;
            
            if (inLength < 8 || outLength < 8)
            {
                status = STATUS_BUFFER_TOO_SMALL;
                goto IoControlEnd;
            }
            
            processHandle = *(HANDLE *)dataBuffer;
            handle = *(HANDLE *)(dataBuffer + 4);
            
            status = KphAttachProcessHandle(processHandle, &attachState);
            
            if (!NT_SUCCESS(status))
                goto IoControlEnd;
            
            *(PHANDLE)dataBuffer = KphGetThreadId(handle, (PHANDLE)((PCHAR)dataBuffer + 4));
            KphDetachProcess(&attachState);
            retLength = 8;
        }
        break;
        
        default:
        {
            dprintf("Unrecognized IOCTL code 0x%08x\n", controlCode);
            status = STATUS_INVALID_DEVICE_REQUEST;
        }
        break;
    }
    
IoControlEnd:
    Irp->IoStatus.Information = retLength;
    Irp->IoStatus.Status = status;
    dprintf("IOCTL 0x%08x result was 0x%08x\n", controlCode, status);
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
            dprintf("Client read %d bytes!\n", readLength);
            
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

NTSTATUS KphUnsupported(PDEVICE_OBJECT DeviceObject, PIRP Irp)
{
    DbgPrint("KProcessHacker: Unsupported function called\n");
    
    return STATUS_NOT_IMPLEMENTED;
}
