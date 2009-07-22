/*
 * Process Hacker Driver - 
 *   system service logging
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

#include "include/sysservicep.h"
#include "include/hook.h"

FAST_MUTEX KphSsMutex;
/* Whether system service logging has been initialized. */
BOOLEAN KphSsInitialized;
/* The KiFastCallEntry hook. */
KPH_HOOK KphSsKiFastCallEntryHook;

PKPH_OBJECT_TYPE KphSsClientEntryType;
PKPH_OBJECT_TYPE KphSsProcessEntryType;

LIST_ENTRY KphSsProcessListHead;
EX_RUNDOWN_REF KphSsRundownProtect;

/* KphSsLogInit
 * 
 * Initializes system service logging.
 */
NTSTATUS KphSsLogInit()
{
    /* Initialize the process list. */
    InitializeListHead(&KphSsProcessListHead);
    ExInitializeFastMutex(&KphSsMutex);
    
    return STATUS_SUCCESS;
}

/* KphSsLogStart
 * 
 * Starts system service logging.
 */
NTSTATUS KphSsLogStart()
{
    NTSTATUS status = STATUS_SUCCESS;
    
    ExAcquireFastMutex(&KphSsMutex);
    
    if (KphSsInitialized)
    {
        ExReleaseFastMutex(&KphSsMutex);
        return STATUS_UNSUCCESSFUL;
    }
    
    /* Initialize the object types. */
    if (!KphSsClientEntryType)
    {
        status = KphCreateObjectType(
            &KphSsClientEntryType,
            NonPagedPool,
            KphpClientEntryDeleteProcedure
            );
        
        if (!NT_SUCCESS(status))
        {
            ExReleaseFastMutex(&KphSsMutex);
            return status;
        }
    }
    
    if (!KphSsProcessEntryType)
    {
        status = KphCreateObjectType(
            &KphSsProcessEntryType,
            NonPagedPool,
            KphpProcessEntryDeleteProcedure
            );
        
        if (!NT_SUCCESS(status))
        {
            ExReleaseFastMutex(&KphSsMutex);
            return status;
        }
    }
    
    /* (Re-)initialize rundown protection. */
    ExInitializeRundownProtection(&KphSsRundownProtect);
    
    /* This will overwrite the inc instruction in KiFastCallEntry with a jmp 
     * to KphpSsNewKiFastCallEntry.
     */
    KphInitializeHook(
        &KphSsKiFastCallEntryHook,
        __KiFastCallEntry,
        KphpSsNewKiFastCallEntry
        );
    status = KphHook(&KphSsKiFastCallEntryHook);
    
    if (!NT_SUCCESS(status))
    {
        ExReleaseFastMutex(&KphSsMutex);
        return status;
    }
    
    KphSsInitialized = TRUE;
    
    ExReleaseFastMutex(&KphSsMutex);
    
    return status;
}

/* KphSsLogStop
 * 
 * Stops system service logging.
 */
NTSTATUS KphSsLogStop()
{
    NTSTATUS status = STATUS_SUCCESS;
    
    ExAcquireFastMutex(&KphSsMutex);
    
    if (!KphSsInitialized)
    {
        ExReleaseFastMutex(&KphSsMutex);
        return STATUS_UNSUCCESSFUL;
    }
    
    status = KphUnhook(&KphSsKiFastCallEntryHook);
    
    if (!NT_SUCCESS(status))
    {
        ExReleaseFastMutex(&KphSsMutex);
        return status;
    }
    
    /* Wait for all loggers to finish. */
    ExWaitForRundownProtectionRelease(&KphSsRundownProtect);
    
    KphSsInitialized = FALSE;
    
    ExReleaseFastMutex(&KphSsMutex);
    
    return status;
}

/* KphpCreateClientEntry
 * 
 * Creates a client entry which describes a client of the 
 * system service logger. Clients receieve system service log events.
 */
NTSTATUS KphpCreateClientEntry(
    __out PKPHPSS_CLIENT_ENTRY *ClientEntry,
    __in HANDLE ClientProcessHandle,
    __in PVOID ClientBufferBase,
    __in ULONG ClientBufferSize,
    __in KPROCESSOR_MODE AccessMode
    )
{
    NTSTATUS status = STATUS_SUCCESS;
    PKPHPSS_CLIENT_ENTRY clientEntry;
    PEPROCESS clientProcessObject;
    
    /* Probe. */
    if (AccessMode != KernelMode)
    {
        __try
        {
            ProbeForWrite(ClientBufferBase, ClientBufferSize, 1);
        }
        __except (EXCEPTION_EXECUTE_HANDLER)
        {
            return GetExceptionCode();
        }
    }
    
    /* Reference the client process. */
    status = ObReferenceObjectByHandle(
        ClientProcessHandle,
        PROCESS_VM_WRITE,
        *PsProcessType,
        AccessMode,
        &clientProcessObject,
        NULL
        );
    
    if (!NT_SUCCESS(status))
        return status;
    
    status = KphCreateObject(
        &clientEntry,
        sizeof(KPHPSS_CLIENT_ENTRY),
        0,
        KphSsClientEntryType,
        0
        );
    
    if (!NT_SUCCESS(status))
    {
        ObDereferenceObject(clientProcessObject);
        return status;
    }
    
    clientEntry->ClientProcess = clientProcessObject;
    clientEntry->ClientBufferBase = ClientBufferBase;
    clientEntry->ClientBufferSize = ClientBufferSize;
    
    *ClientEntry = clientEntry;
    
    return status;
}

/* KphpClientEntryDeleteProcedure
 * 
 * Performs cleanup for a client entry.
 */
VOID NTAPI KphpClientEntryDeleteProcedure(
    __in PVOID Object,
    __in ULONG Flags,
    __in SIZE_T Size
    )
{
    PKPHPSS_CLIENT_ENTRY clientEntry = (PKPHPSS_CLIENT_ENTRY)Object;
    
    ObDereferenceObject(clientEntry->ClientProcess);
}

/* KphpCreateProcessEntry
 * 
 * Creates a process entry which describes a process for which 
 * system services will be logged.
 */
NTSTATUS KphpCreateProcessEntry(
    __out PKPHPSS_PROCESS_ENTRY *ProcessEntry,
    __in PKPHPSS_CLIENT_ENTRY ClientEntry,
    __in PEPROCESS TargetProcess
    )
{
    NTSTATUS status = STATUS_SUCCESS;
    PKPHPSS_PROCESS_ENTRY processEntry;
    
    status = KphCreateObject(
        &processEntry,
        sizeof(KPHPSS_PROCESS_ENTRY),
        0,
        KphSsProcessEntryType,
        0
        );
    
    if (!NT_SUCCESS(status))
        return status;
    
    KphReferenceObject(ClientEntry);
    processEntry->Client = ClientEntry;
    processEntry->TargetProcess = TargetProcess;
    
    ExAcquireFastMutex(&KphSsMutex);
    InsertHeadList(&KphSsProcessListHead, &processEntry->ProcessListEntry);
    ExReleaseFastMutex(&KphSsMutex);
    
    *ProcessEntry = processEntry;
    
    return status;
}

VOID NTAPI KphpProcessEntryDeleteProcedure(
    __in PVOID Object,
    __in ULONG Flags,
    __in SIZE_T Size
    )
{
    PKPHPSS_PROCESS_ENTRY processEntry = (PKPHPSS_PROCESS_ENTRY)Object;
    
    KphDereferenceObject(processEntry->Client);
    
    ExAcquireFastMutex(&KphSsMutex);
    RemoveEntryList(&processEntry->ProcessListEntry);
    ExReleaseFastMutex(&KphSsMutex);
}

/* KphpSsLogSystemServiceCall
 * 
 * Logs a system service.
 * 
 * WARNING: This function CANNOT make any system calls.
 * 
 * IRQL: <= APC_LEVEL
 */
VOID NTAPI KphpSsLogSystemServiceCall(
    __in ULONG Number,
    __in PVOID *Arguments,
    __in ULONG NumberOfArguments,
    __in PKSERVICE_TABLE_DESCRIPTOR ServiceTable,
    __in PKTHREAD Thread
    )
{
    PEPROCESS process;
    PLIST_ENTRY currentListEntry;
    BOOLEAN processEntryFound = FALSE;
    
    
    
    /* First, some checks.
     *   * We can't operate at IRQL > APC_LEVEL because 
     *     of restrictions on logging.
     *   * We can't operate on unknown service tables like the 
     *     shadow service table (yet).
     *   * We have to make sure Thread isn't NULL, as it does 
     *     sometimes happen.
     */
    
    if (KeGetCurrentIrql() > APC_LEVEL)
        return;
    if (ServiceTable != __KeServiceDescriptorTable)
        return;
    if (!Thread)
        return;
    
    /* Probe the arguments if we from user-mode. */
    if (KeGetPreviousMode() != KernelMode)
    {
        __try
        {
            ProbeForRead(Arguments, NumberOfArguments * sizeof(ULONG), 4);
        }
        __except (EXCEPTION_EXECUTE_HANDLER)
        {
            return;
        }
    }
    
    /* Check if the thread's process is in the process list. If not, 
     * we can simply return without doing anything.
     */
    
    process = IoThreadToProcess(Thread);
    
    ExAcquireFastMutex(&KphSsMutex);
    
    currentListEntry = KphSsProcessListHead.Flink;
    
    while (currentListEntry != &KphSsProcessListHead)
    {
        PKPHPSS_PROCESS_ENTRY processEntry = KPHPSS_PROCESS_ENTRY(currentListEntry);
        
        if (processEntry->TargetProcess == process)
        {
            processEntryFound = TRUE;
            break;
        }
    }
    
    ExReleaseFastMutex(&KphSsMutex);
    
    if (!processEntryFound)
        return;
    
    dfprintf("Made it this far!\n");
}

/* KphpSsNewKiFastCallEntry
 * 
 * The hook function called from within the hooked KiFastCallEntry.
 */
__declspec(naked) VOID NTAPI KphpSsNewKiFastCallEntry()
{
    /* The hook location for KiFastCallEntry has been chosen so that
     * we don't have to switch the appropriate thread stack because 
     * KiFastCallEntry has already done it for us.
     * 
     * At this point: 
     *   * eax contains the system service number.
     *   * edx contains a pointer to the user-supplied arguments for 
     *     the system service.
     *   * edi contains a pointer to the service table associated with 
     *     the system service number.
     *   * esi contains a pointer to the KTHREAD of the caller.
     */
    /* Some context:
     * 
     * push     edx
     * push     eax
     * call     [_KeGdiFlushUserBatch]
     * pop      eax
     * pop      edx
     * inc      dword ptr fs:[PbSystemCalls] <-- this gets overwritten with a jmp to here
     * mov      edi, edx
     * mov      ebx, [edi+...]
     * ...
     */
    __asm
    {
        /* Save all registers first. */
        push    ebp
        push    edi
        push    esi
        push    edx
        push    ecx
        push    ebx
        push    eax
        
        /* Since we overwrite the inc instruction when we did the hook, 
         * perform the job now.
         */
        lea     ebx, KphSsKiFastCallEntryHook /* get a pointer to the hook structure */
        mov     ebx, dword ptr [ebx+KPH_HOOK.Bytes+3] /* get the PbSystemCalls offset from the original instruction */
        inc     dword ptr fs:[ebx] /* increment PbSystemCalls in the PRCB */
        
        /* Get the number of arguments for this system service. */
        mov     ebx, dword ptr [edi+KSERVICE_TABLE_DESCRIPTOR.Number] /* ebx = a pointer to the argument table */
        xor     ecx, ecx
        mov     cl, [ebx+eax] /* ecx = size of the arguments, in bytes. */
        shr     ecx, 2 /* divide by 2 to get the number of arguments (all ULONGs) */
        
        /* Call the KiFastCallEntry proc. */
        push    esi /* Thread */
        push    edi /* ServiceTable */
        push    ecx /* NumberOfArguments */
        push    edx /* Arguments */
        push    eax /* Number */
        call    KphpSsLogSystemServiceCall
        
        /* Restore the registers and resume execution in KiFastCallEntry. */
        pop     eax
        pop     ebx
        pop     ecx
        pop     edx
        pop     esi
        pop     edi
        pop     ebp
        
        /* Luckily, KiFastCallEntry will overwrite ebx when we jump back, so it's safe to use it. */
        lea     ebx, __KiFastCallEntry
        mov     ebx, [ebx] /* ebx = KiFastCallEntry at the inc instruction */
        add     ebx, 7 /* skip the inc instruction */
        jmp     ebx /* jump back */
    }
}
