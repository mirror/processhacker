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

#pragma alloc_text(PAGE, KphDispatchCreate)
#pragma alloc_text(PAGE, KphDispatchClose)
#pragma alloc_text(PAGE, KphDispatchDeviceControl)
#pragma alloc_text(PAGE, KphDispatchRead)
#pragma alloc_text(PAGE, KphUnsupported)
#pragma pack(1)

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
    {
        if (status == STATUS_NOT_SUPPORTED)
            dprintf("Your operating system is not supported by KProcessHacker\n");
        
        return status;
    }
    
    /* Initialize NT KPH */
    status = KphNtInit();
    
    if (!NT_SUCCESS(status))
        return status;
    
    RtlInitUnicodeString(&deviceName, KPH_DEVICE_NAME);
    RtlInitUnicodeString(&dosDeviceName, KPH_DEVICE_DOS_NAME);
    
    /* Create the KProcessHacker device. */
    status = IoCreateDevice(DriverObject, 0, &deviceName, 
        FILE_DEVICE_UNKNOWN, FILE_DEVICE_SECURE_OPEN, FALSE, &deviceObject);
    
    /* Set up the major functions. */
    for (i = 0; i < IRP_MJ_MAXIMUM_FUNCTION; i++)
        DriverObject->MajorFunction[i] = NULL;
    
    DriverObject->MajorFunction[IRP_MJ_CLOSE] = KphDispatchClose;
    DriverObject->MajorFunction[IRP_MJ_CREATE] = KphDispatchCreate;
    DriverObject->MajorFunction[IRP_MJ_READ] = KphDispatchRead;
    DriverObject->MajorFunction[IRP_MJ_DEVICE_CONTROL] = KphDispatchDeviceControl;
    DriverObject->DriverUnload = DriverUnload;
    
    deviceObject->Flags |= DO_BUFFERED_IO;
    deviceObject->Flags &= ~DO_DEVICE_INITIALIZING;
    
    IoCreateSymbolicLink(&dosDeviceName, &deviceName);
    
    dprintf("Driver loaded\n");
    
    return STATUS_SUCCESS;
}

NTSTATUS KphDispatchCreate(PDEVICE_OBJECT DeviceObject, PIRP Irp)
{
    NTSTATUS status = STATUS_SUCCESS;
    
#ifdef KPH_REQUIRE_DEBUG_PRIVILEGE
    if (!SeSinglePrivilegeCheck(SeExports->SeDebugPrivilege, UserMode))
    {
        dprintf("Client (PID %d) was refused\n", PsGetCurrentProcessId());
        Irp->IoStatus.Status = STATUS_PRIVILEGE_NOT_HELD;
        
        return STATUS_PRIVILEGE_NOT_HELD;
    }
#endif
    
    dprintf("Client (PID %d) connected\n", PsGetCurrentProcessId());
    dprintf("Base IOCTL is 0x%08x\n", KPH_CTL_CODE(0));
    
    return status;
}

NTSTATUS KphDispatchClose(PDEVICE_OBJECT DeviceObject, PIRP Irp)
{
    NTSTATUS status = STATUS_SUCCESS;
    
    dprintf("Client (PID %d) disconnected\n", PsGetCurrentProcessId());
    
    return status;
}

/* from YAPM */
NTSTATUS GetObjectName(PFILE_OBJECT FileObject, PVOID Buffer, ULONG BufferLength, PULONG ReturnLength)
{
    ULONG nameLength = 0;
    PFILE_OBJECT relatedFile;
    PVOID name = Buffer;
    
    if (FileObject->DeviceObject)
    {
        ObQueryNameString((PVOID)FileObject->DeviceObject, name, BufferLength, ReturnLength);
        (PCHAR)name += *ReturnLength - 2; /* minus the null terminator */
        BufferLength -= *ReturnLength - 2;
    }
    else
    {
        /* it's a UNICODE_STRING. we need to subtract the space 
        Length and MaximumLength take up. */
        (PCHAR)name += 4;
        BufferLength -= 4;
    }
    
    if (!FileObject->FileName.Buffer)
        return STATUS_SUCCESS;
    
    relatedFile = FileObject;
    
    do
    {
        nameLength += relatedFile->FileName.Length;
        relatedFile = relatedFile->RelatedFileObject;
    }
    while (relatedFile);
    
    *ReturnLength += nameLength;
    
    if (nameLength > BufferLength)
    {
        return STATUS_BUFFER_TOO_SMALL;
    }
    
    (PCHAR)name += nameLength;
    *(PUSHORT)name = 0;
    
    relatedFile = FileObject;
    do
    {
        (PCHAR)name -= relatedFile->FileName.Length;
        memcpy(name, relatedFile->FileName.Buffer, relatedFile->FileName.Length);
        relatedFile = relatedFile->RelatedFileObject;
    }
    while (relatedFile);
    
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
    else if (ControlCode == KPH_GETTHREADSTARTADDRESS)
        return "Get Thread Start Address";
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
    else if (ControlCode == KPH_TERMINATETHREAD)
        return "KphTerminateThread";
    else if (ControlCode == KPH_GETFEATURES)
        return "Get Features";
    else if (ControlCode == KPH_EXPGETPROCESSINFORMATION)
        return "ExpGetProcessInformation";
    else if (ControlCode == KPH_ASSIGNIMPERSONATIONTOKEN)
        return "KphAssignImpersonationToken";
    else
        return "Unknown";
}

NTSTATUS KphDispatchDeviceControl(PDEVICE_OBJECT DeviceObject, PIRP Irp)
{
    NTSTATUS status = STATUS_SUCCESS;
    PIO_STACK_LOCATION ioStackIrp = NULL;
    PVOID dataBuffer;
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
    
    dataBuffer = Irp->AssociatedIrp.SystemBuffer;
    
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
        /* Read
         * 
         * Reads a number of bytes from the specified address. This call should 
         * never be used because it will cause a bugcheck upon reading invalid 
         * kernel memory.
         */
        case KPH_READ:
        {
            struct
            {
                PVOID Address;
            } *args = dataBuffer;
            
            if (inLength < sizeof(*args))
            {
                status = STATUS_BUFFER_TOO_SMALL;
                goto IoControlEnd;
            }
            
            __try
            {
                RtlCopyMemory(dataBuffer, args->Address, outLength);
                retLength = outLength;
            }
            __except (EXCEPTION_EXECUTE_HANDLER)
            {
                status = STATUS_ACCESS_VIOLATION;
                goto IoControlEnd;
            }
        }
        break;
        
        /* Write
         * 
         * Writes a number of bytes to the specified address. This call should 
         * never be used because it will cause a bugcheck upon writing to invalid 
         * kernel memory.
         */
        case KPH_WRITE:
        {
            struct
            {
                PVOID Address;
                CHAR Data[1];
            } *args = dataBuffer;
            
            if (inLength < sizeof(PVOID))
            {
                status = STATUS_BUFFER_TOO_SMALL;
                goto IoControlEnd;
            }
            
            /* any interrupts happening while we're writing is... bad. */
            __asm cli;
            RtlCopyMemory(args->Address, args->Data, inLength - sizeof(PVOID));
            __asm sti;
            
            retLength = inLength;
        }
        break;
        
        /* Get File Object Name
         * 
         * Gets the file name of the specified handle. The handle can be remote; 
         * in that case the process ID must be specified. Otherwise, specify the 
         * current process ID.
         */
        case KPH_GETFILEOBJECTNAME:
        {
            struct
            {
                HANDLE Handle;
                HANDLE ProcessId;
            } *args = dataBuffer;
            KPH_ATTACH_STATE attachState;
            PVOID object;
            
            if (inLength < sizeof(*args))
            {
                status = STATUS_BUFFER_TOO_SMALL;
                goto IoControlEnd;
            }
            
            status = KphAttachProcessId(args->ProcessId, &attachState);
            
            if (!NT_SUCCESS(status))
                goto IoControlEnd;
            
            status = ObReferenceObjectByHandle(args->Handle, 0, 
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
                    status = GetObjectName((PFILE_OBJECT)object, dataBuffer, outLength, &retLength);
                    ObDereferenceObject(object);
                    
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
        
        /* KphOpenProcess
         * 
         * Opens the specified process. This call will never fail unless:
         * 1. PsLookupProcessByProcessId, ObOpenObjectByPointer or some lower-level 
         *    function is hooked, or 
         * 2. The process is protected.
         */
        case KPH_OPENPROCESS:
        {
            struct
            {
                HANDLE ProcessId;
                ACCESS_MASK DesiredAccess;
            } *args = dataBuffer;
            struct
            {
                HANDLE ProcessHandle;
            } *ret = dataBuffer;
            OBJECT_ATTRIBUTES objectAttributes = { 0 };
            CLIENT_ID clientId;
            
            if (inLength < sizeof(*args) || outLength < sizeof(*ret))
            {
                status = STATUS_BUFFER_TOO_SMALL;
                goto IoControlEnd;
            }
            
            clientId.UniqueThread = 0;
            clientId.UniqueProcess = args->ProcessId;
            status = KphOpenProcess(
                &ret->ProcessHandle,
                args->DesiredAccess,
                &objectAttributes,
                &clientId,
                UserMode
                );
            
            if (!NT_SUCCESS(status))
                goto IoControlEnd;
            
            retLength = sizeof(*ret);
        }
        break;
        
        /* KphOpenThread
         * 
         * Opens the specified thread. This call will never fail unless:
         * 1. PsLookupProcessThreadByCid, ObOpenObjectByPointer or some lower-level 
         *    function is hooked, or 
         * 2. The thread's process is protected.
         */
        case KPH_OPENTHREAD:
        {
            struct
            {
                HANDLE ThreadId;
                ACCESS_MASK DesiredAccess;
            } *args = dataBuffer;
            struct
            {
                HANDLE ThreadHandle;
            } *ret = dataBuffer;
            OBJECT_ATTRIBUTES objectAttributes = { 0 };
            CLIENT_ID clientId;
            
            if (inLength < sizeof(*args) || outLength < sizeof(*ret))
            {
                status = STATUS_BUFFER_TOO_SMALL;
                goto IoControlEnd;
            }
            
            clientId.UniqueThread = args->ThreadId;
            clientId.UniqueProcess = 0;
            status = KphOpenThread(
                &ret->ThreadHandle,
                args->DesiredAccess,
                &objectAttributes,
                &clientId,
                UserMode
                );
            
            if (!NT_SUCCESS(status))
                goto IoControlEnd;
            
            retLength = sizeof(*ret);
        }
        break;
        
        /* KphOpenProcessToken
         * 
         * Opens the specified process' token. This call will never fail unless 
         * a low-level function is hooked.
         */
        case KPH_OPENPROCESSTOKEN:
        {
            struct
            {
                HANDLE ProcessHandle;
                ACCESS_MASK DesiredAccess;
            } *args = dataBuffer;
            struct
            {
                HANDLE TokenHandle;
            } *ret = dataBuffer;
            
            if (inLength < sizeof(*args) || outLength < sizeof(*ret))
            {
                status = STATUS_BUFFER_TOO_SMALL;
                goto IoControlEnd;
            }
            
            status = KphOpenProcessTokenEx(
                args->ProcessHandle,
                args->DesiredAccess,
                0,
                &ret->TokenHandle,
                UserMode
                );
            
            if (!NT_SUCCESS(status))
                goto IoControlEnd;
            
            retLength = sizeof(*ret);
        }
        break;
        
        /* Get Process Protected
         * 
         * Gets whether the process is protected.
         */
        case KPH_GETPROCESSPROTECTED:
        {
            struct
            {
                HANDLE ProcessId;
            } *args = dataBuffer;
            struct
            {
                BOOLEAN IsProtected;
            } *ret = dataBuffer;
            PEPROCESS processObject;
            
            if (inLength < sizeof(*args) || outLength < sizeof(*ret))
            {
                status = STATUS_BUFFER_TOO_SMALL;
                goto IoControlEnd;
            }
            
            status = PsLookupProcessByProcessId(args->ProcessId, &processObject);
            
            if (!NT_SUCCESS(status))
                goto IoControlEnd;
            
            ret->IsProtected = 
                (CHAR)GET_BIT(
                    *(PULONG)KVOFF(processObject, OffEpProtectedProcessOff),
                    OffEpProtectedProcessBit
                    );
            ObDereferenceObject(processObject);
            retLength = sizeof(*ret);
        }
        break;
        
        /* Set Process Protected
         * 
         * Sets whether the process is protected.
         */
        case KPH_SETPROCESSPROTECTED:
        {
            struct
            {
                HANDLE ProcessId;
                BOOLEAN IsProtected;
            } *args = dataBuffer;
            PEPROCESS processObject;
            
            if (inLength < sizeof(*args))
            {dprintf("%d\n", sizeof(*args));
                status = STATUS_BUFFER_TOO_SMALL;
                goto IoControlEnd;
            }
            
            status = PsLookupProcessByProcessId(args->ProcessId, &processObject);
            
            if (!NT_SUCCESS(status))
                goto IoControlEnd;
            
            if (args->IsProtected)
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
        
        /* KphTerminateProcess
         * 
         * Terminates the specified process. This call will never fail unless
         * PsTerminateProcess could not be located and Zw/NtTerminateProcess 
         * is hooked, or an attempt was made to terminate the current process. 
         * In that case, the call will fail with STATUS_DISK_FULL.
         */
        case KPH_TERMINATEPROCESS:
        {
            struct
            {
                HANDLE ProcessHandle;
                NTSTATUS ExitStatus;
            } *args = dataBuffer;
            
            if (inLength < sizeof(*args))
            {
                status = STATUS_BUFFER_TOO_SMALL;
                goto IoControlEnd;
            }
            
            status = KphTerminateProcess(args->ProcessHandle, args->ExitStatus);
        }
        break;
        
        /* KphSuspendProcess
         * 
         * Suspends the specified process. This call will never fail unless 
         * PsSuspendProcess could not be located and Zw/NtSuspendProcess 
         * is hooked.
         */
        case KPH_SUSPENDPROCESS:
        {
            struct
            {
                HANDLE ProcessHandle;
            } *args = dataBuffer;
            
            if (inLength < sizeof(*args))
            {
                status = STATUS_BUFFER_TOO_SMALL;
                goto IoControlEnd;
            }
            
            status = KphSuspendProcess(args->ProcessHandle);
        }
        break;
        
        /* KphResumeProcess
         * 
         * Resumes the specified process. This call will never fail unless 
         * PsResumeProcess could not be located and Zw/NtResumeProcess 
         * is hooked.
         */
        case KPH_RESUMEPROCESS:
        {
            struct
            {
                HANDLE ProcessHandle;
            } *args = dataBuffer;
            
            if (inLength < sizeof(*args))
            {
                status = STATUS_BUFFER_TOO_SMALL;
                goto IoControlEnd;
            }
            
            status = KphResumeProcess(args->ProcessHandle);
        }
        break;
        
        /* KphReadVirtualMemory
         * 
         * Reads process memory. This call will fail if MmCopyVirtualMemory 
         * could not be located.
         */
        case KPH_READVIRTUALMEMORY:
        {
            struct
            {
                HANDLE ProcessHandle;
                PVOID BaseAddress;
                PVOID Buffer;
                ULONG BufferLength;
                PULONG ReturnLength;
            } *args = dataBuffer;
            
            if (inLength < sizeof(*args))
            {
                status = STATUS_BUFFER_TOO_SMALL;
                goto IoControlEnd;
            }
            
            status = KphReadVirtualMemory(
                args->ProcessHandle,
                args->BaseAddress,
                args->Buffer,
                args->BufferLength,
                args->ReturnLength,
                UserMode
                );
        }
        break;
        
        /* KphWriteVirtualMemory
         * 
         * Writes process memory. This call will fail if MmCopyVirtualMemory 
         * could not be located.
         */
        case KPH_WRITEVIRTUALMEMORY:
        {
            struct
            {
                HANDLE ProcessHandle;
                PVOID BaseAddress;
                PVOID Buffer;
                ULONG BufferLength;
                PULONG ReturnLength;
            } *args = dataBuffer;
            
            if (inLength < sizeof(*args))
            {
                status = STATUS_BUFFER_TOO_SMALL;
                goto IoControlEnd;
            }
            
            status = KphWriteVirtualMemory(
                args->ProcessHandle,
                args->BaseAddress,
                args->Buffer,
                args->BufferLength,
                args->ReturnLength,
                UserMode
                );
        }
        break;
        
        /* Set Process Token
         * 
         * Assigns the primary token of a source process to a target process.
         */
        case KPH_SETPROCESSTOKEN:
        {
            struct
            {
                HANDLE SourceProcessId;
                HANDLE TargetProcessId;
            } *args = dataBuffer;
            
            if (inLength < sizeof(*args))
            {
                status = STATUS_BUFFER_TOO_SMALL;
                goto IoControlEnd;
            }
            
            status = SetProcessToken(args->SourceProcessId, args->TargetProcessId);
        }
        break;
        
        /* Get Thread Start Address
         * 
         * Gets the specified thread's start address.
         */
        case KPH_GETTHREADSTARTADDRESS:
        {
            struct
            {
                HANDLE ThreadHandle;
            } *args = dataBuffer;
            struct
            {
                PVOID StartAddress;
            } *ret = dataBuffer;
            PETHREAD2 threadObject;
            
            if (inLength < sizeof(*args) || outLength < sizeof(*ret))
            {
                status = STATUS_BUFFER_TOO_SMALL;
                goto IoControlEnd;
            }
            
            status = ObReferenceObjectByHandle(args->ThreadHandle, 0, *PsThreadType, KernelMode, &threadObject, NULL);
            
            if (!NT_SUCCESS(status))
                goto IoControlEnd;
            
            /* Get the Win32StartAddress */
            if (!(ret->StartAddress = *(PVOID *)KVOFF(threadObject, OffEtWin32StartAddress)))
            {
                /* If that failed, get the StartAddress */
                ret->StartAddress = *(PVOID *)KVOFF(threadObject, OffEtStartAddress);
            }
            
            ObDereferenceObject(threadObject);
            retLength = sizeof(*ret);
        }
        break;
        
        /* Get Handle Object Name
         * 
         * Gets the name of the specified handle. The handle can be remote; in 
         * that case a valid process handle must be passed. Otherwise, set the 
         * process handle to -1.
         */
        case KPH_GETHANDLEOBJECTNAME:
        {
            struct
            {
                HANDLE ProcessHandle;
                HANDLE Handle;
            } *args = dataBuffer;
            KPH_ATTACH_STATE attachState;
            PVOID object;
            
            if (inLength < sizeof(*args))
            {
                status = STATUS_BUFFER_TOO_SMALL;
                goto IoControlEnd;
            }
            
            status = KphAttachProcessHandle(args->ProcessHandle, &attachState);
            
            if (!NT_SUCCESS(status))
                goto IoControlEnd;
            
            status = ObReferenceObjectByHandle(args->Handle, 0, NULL, KernelMode, &object, NULL);
            KphDetachProcess(&attachState);
            
            if (!NT_SUCCESS(status))
                goto IoControlEnd;
            
            status = ObQueryNameString(object, (POBJECT_NAME_INFORMATION)dataBuffer, outLength, &retLength);
            ObDereferenceObject(object);
        }
        break;
        
        /* KphOpenProcessJob
         * 
         * Opens the job object that the process is assigned to. If the process is 
         * not assigned to any job object, the call will fail with STATUS_NO_SUCH_FILE.
         */
        case KPH_OPENPROCESSJOB:
        {
            struct
            {
                HANDLE ProcessHandle;
                ACCESS_MASK DesiredAccess;
            } *args = dataBuffer;
            struct
            {
                HANDLE JobHandle;
            } *ret = dataBuffer;
            
            if (inLength < sizeof(*args) || outLength < sizeof(*ret))
            {
                status = STATUS_BUFFER_TOO_SMALL;
                goto IoControlEnd;
            }
            
            status = KphOpenProcessJob(args->ProcessHandle, args->DesiredAccess, &ret->JobHandle, UserMode);
            
            if (!NT_SUCCESS(status))
                goto IoControlEnd;
            
            retLength = sizeof(*ret);
        }
        break;
        
        /* KphGetContextThread
         * 
         * Gets the context of the specified thread.
         */
        case KPH_GETCONTEXTTHREAD:
        {
            struct
            {
                HANDLE ThreadHandle;
                PCONTEXT ThreadContext;
            } *args = dataBuffer;
            
            if (inLength < sizeof(*args))
            {
                status = STATUS_BUFFER_TOO_SMALL;
                goto IoControlEnd;
            }
            
            status = KphGetContextThread(args->ThreadHandle, args->ThreadContext, UserMode);
        }
        break;
        
        /* KphSetContextThread
         * 
         * Sets the context of the specified thread.
         */
        case KPH_SETCONTEXTTHREAD:
        {
            struct
            {
                HANDLE ThreadHandle;
                PCONTEXT ThreadContext;
            } *args = dataBuffer;
            
            if (inLength < sizeof(*args))
            {
                status = STATUS_BUFFER_TOO_SMALL;
                goto IoControlEnd;
            }
            
            status = KphSetContextThread(args->ThreadHandle, args->ThreadContext, UserMode);
        }
        break;
        
        /* KphGetThreadWin32Thread
         * 
         * Gets a pointer to the specified thread's Win32Thread structure.
         */
        case KPH_GETTHREADWIN32THREAD:
        {
            struct
            {
                HANDLE ThreadHandle;
            } *args = dataBuffer;
            struct
            {
                PVOID Win32Thread;
            } *ret = dataBuffer;
            
            if (inLength < sizeof(*args) || outLength < sizeof(*ret))
            {
                status = STATUS_BUFFER_TOO_SMALL;
                goto IoControlEnd;
            }
            
            status = KphGetThreadWin32Thread(args->ThreadHandle, &ret->Win32Thread, KernelMode);
            
            if (!NT_SUCCESS(status))
                goto IoControlEnd;
            
            retLength = sizeof(*ret);
        }
        break;
        
        /* KphDuplicateObject
         * 
         * Duplicates the specified handle from the source process to the target process. 
         * Do not use this call to duplicate file handles; it may freeze indefinitely if 
         * the file is a named pipe.
         */
        case KPH_DUPLICATEOBJECT:
        {
            struct
            {
                HANDLE SourceProcessHandle;
                HANDLE SourceHandle;
                HANDLE TargetProcessHandle;
                PHANDLE TargetHandle;
                ACCESS_MASK DesiredAccess;
                ULONG HandleAttributes;
                ULONG Options;
            } *args = dataBuffer;
            
            if (inLength < sizeof(*args))
            {
                status = STATUS_BUFFER_TOO_SMALL;
                goto IoControlEnd;
            }
            
            status = KphDuplicateObject(
                args->SourceProcessHandle,
                args->SourceHandle,
                args->TargetProcessHandle,
                args->TargetHandle,
                args->DesiredAccess,
                args->HandleAttributes,
                args->Options,
                UserMode
                );
        }
        break;
        
        /* ZwQueryObject
         * 
         * Performs ZwQueryObject in the context of another process.
         */
        case KPH_ZWQUERYOBJECT:
        {
            struct
            {
                HANDLE ProcessHandle;
                HANDLE Handle;
                ULONG ObjectInformationClass;
            } *args = dataBuffer;
            struct
            {
                NTSTATUS Status;
                ULONG ReturnLength;
                PVOID BufferBase;
                CHAR Buffer[1];
            } *ret = dataBuffer;
            NTSTATUS status2 = STATUS_SUCCESS;
            KPH_ATTACH_STATE attachState;
            
            if (inLength < sizeof(*args) || outLength < sizeof(*ret) - sizeof(CHAR))
            {
                status = STATUS_BUFFER_TOO_SMALL;
                goto IoControlEnd;
            }
            
            status = KphAttachProcessHandle(args->ProcessHandle, &attachState);
            
            if (!NT_SUCCESS(status))
                goto IoControlEnd;
            
            status2 = ZwQueryObject(
                args->Handle,
                args->ObjectInformationClass,
                ret->Buffer,
                outLength - (sizeof(*ret) - sizeof(CHAR)),
                &retLength
                );
            KphDetachProcess(&attachState);
            
            ret->ReturnLength = retLength;
            ret->BufferBase = ret->Buffer;
            
            if (NT_SUCCESS(status2))
                retLength += sizeof(*ret) - sizeof(CHAR);
            else
                retLength = sizeof(*ret) - sizeof(CHAR);
            
            ret->Status = status2;
        }
        break;
        
        /* KphGetProcessId
         * 
         * Gets the process ID of a process handle in the context of another process.
         */
        case KPH_GETPROCESSID:
        {
            struct
            {
                HANDLE ProcessHandle;
                HANDLE Handle;
            } *args = dataBuffer;
            struct
            {
                HANDLE ProcessId;
            } *ret = dataBuffer;
            KPH_ATTACH_STATE attachState;
            
            if (inLength < sizeof(*args) || outLength < sizeof(*ret))
            {
                status = STATUS_BUFFER_TOO_SMALL;
                goto IoControlEnd;
            }
            
            status = KphAttachProcessHandle(args->ProcessHandle, &attachState);
            
            if (!NT_SUCCESS(status))
                goto IoControlEnd;
            
            ret->ProcessId = KphGetProcessId(args->Handle);
            KphDetachProcess(&attachState);
            retLength = sizeof(*ret);
        }
        break;
        
        /* KphGetThreadId
         * 
         * Gets the thread ID of a thread handle in the context of another process.
         */
        case KPH_GETTHREADID:
        {
            struct
            {
                HANDLE ProcessHandle;
                HANDLE Handle;
            } *args = dataBuffer;
            struct
            {
                HANDLE ThreadId;
                HANDLE ProcessId;
            } *ret = dataBuffer;
            KPH_ATTACH_STATE attachState;
            
            if (inLength < sizeof(*args) || outLength < sizeof(*ret))
            {
                status = STATUS_BUFFER_TOO_SMALL;
                goto IoControlEnd;
            }
            
            status = KphAttachProcessHandle(args->ProcessHandle, &attachState);
            
            if (!NT_SUCCESS(status))
                goto IoControlEnd;
            
            ret->ThreadId = KphGetThreadId(args->Handle, &ret->ProcessId);
            KphDetachProcess(&attachState);
            retLength = sizeof(*ret);
        }
        break;
        
        /* KphTerminateThread
         * 
         * Terminates the specified thread. This call will fail if 
         * PspTerminateThreadByPointer could not be located or if an attempt 
         * was made to terminate the current thread. In that case, the call 
         * will return STATUS_DISK_FULL.
         */
        case KPH_TERMINATETHREAD:
        {
            struct
            {
                HANDLE ThreadHandle;
                NTSTATUS ExitStatus;
            } *args = dataBuffer;
            
            if (inLength < sizeof(*args))
            {
                status = STATUS_BUFFER_TOO_SMALL;
                goto IoControlEnd;
            }
            
            status = KphTerminateThread(args->ThreadHandle, args->ExitStatus);
        }
        break;
        
        /* Get Features
         * 
         * Gets the features supported by the driver.
         */
        case KPH_GETFEATURES:
        {
            struct
            {
                ULONG Features;
            } *ret = dataBuffer;
            ULONG features = 0;
            
            if (outLength < sizeof(*ret))
            {
                status = STATUS_BUFFER_TOO_SMALL;
                goto IoControlEnd;
            }
            
            if (ExpGetProcessInformation)
                features |= KPHF_EXPGETPROCESSINFORMATION;
            if (__PsTerminateProcess)
                features |= KPHF_PSTERMINATEPROCESS;
            if (__PspTerminateThreadByPointer)
                features |= KPHF_PSPTERMINATETHREADBPYPOINTER;
            
            ret->Features = features;
            retLength = sizeof(*ret);
        }
        break;
        
        /* ExpGetProcessInformation
         * 
         * Calls ExpGetProcessInformation. This call will fail if 
         * ExpGetProcessInformation could not be located.
         */
        case KPH_EXPGETPROCESSINFORMATION:
        {
            struct
            {
                PVOID Buffer;
                ULONG BufferLength;
                PULONG ReturnLength;
                ULONG SessionId;
                BOOL ExtendedInformation;
            } *args = dataBuffer;
            
            if (!ExpGetProcessInformation)
            {
                status = STATUS_NOT_SUPPORTED;
                goto IoControlEnd;
            }
            
            if (inLength < sizeof(*args))
            {
                status = STATUS_BUFFER_TOO_SMALL;
                goto IoControlEnd;
            }
            
            status = ExpGetProcessInformation(
                args->Buffer,
                args->BufferLength,
                args->ReturnLength,
                args->SessionId,
                !!args->ExtendedInformation
                );
        }
        break;
        
        /* KphAssignImpersonationToken
         * 
         * Assigns an impersonation token to a thread.
         */
        case KPH_ASSIGNIMPERSONATIONTOKEN:
        {
            struct
            {
                HANDLE ThreadHandle;
                HANDLE TokenHandle;
            } *args = dataBuffer;
            
            if (inLength < sizeof(*args))
            {
                status = STATUS_BUFFER_TOO_SMALL;
                goto IoControlEnd;
            }
            
            status = KphAssignImpersonationToken(args->ThreadHandle, args->TokenHandle);
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

NTSTATUS KphDispatchRead(PDEVICE_OBJECT DeviceObject, PIRP Irp)
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
