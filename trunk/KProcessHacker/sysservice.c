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

/* ================ IMPORTANT ================
 * Please read the comments in KphpSsNewKiFastCallEntry to find out how 
 * KiFastCallEntry can be hooked.
 * 
 * Note that the ONLY SUPPORTED METHOD of hooking is KiFastCallEntry, 
 * which means you MUST be using a CPU which supports sysenter.
 * ===========================================
 */

#include "include/sysservicep.h"
#include "include/hook.h"
#include "include/trace.h"

extern PDRIVER_OBJECT KphDriverObject;

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
    NTSTATUS status = STATUS_SUCCESS;
    
    /* Initialize the process list. */
    InitializeListHead(&KphSsProcessListHead);
    ExInitializeFastMutex(&KphSsMutex);
    
    /* Initialize the object types. */
    status = KphCreateObjectType(
        &KphSsClientEntryType,
        NonPagedPool,
        KphpSsClientEntryDeleteProcedure
        );
    
    if (!NT_SUCCESS(status))
        return status;
    
    status = KphCreateObjectType(
        &KphSsProcessEntryType,
        NonPagedPool,
        KphpSsProcessEntryDeleteProcedure
        );
    
    if (!NT_SUCCESS(status))
    {
        KphDereferenceObject(KphSsClientEntryType);
        return status;
    }
    
    return status;
}

/* KphSsLogStart
 * 
 * Starts system service logging.
 */
NTSTATUS KphSsLogStart()
{
    NTSTATUS status = STATUS_SUCCESS;
    
    /* Make sure we have the KiFastCallEntry+x address. */
    if (!__KiFastCallEntry)
        return STATUS_NOT_SUPPORTED;
    
    ExAcquireFastMutex(&KphSsMutex);
    
    if (KphSsInitialized)
    {
        ExReleaseFastMutex(&KphSsMutex);
        return STATUS_UNSUCCESSFUL;
    }
    
    /* (Re-)initialize rundown protection. */
    ExInitializeRundownProtection(&KphSsRundownProtect);
    
    /* Hook KiFastCallEntry. Logging will start from now. */
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

/* KphSsCreateClientEntry
 * 
 * Creates a client entry which describes a client of the 
 * system service logger. Clients receieve system service log events.
 * 
 * ClientEntry: A variable which receives a pointer to the client entry.
 * ProcessHandle: A handle to the client process, with PROCESS_VM_WRITE 
 * access.
 * EventHandle: A handle to an event which is set when an event is 
 * written to the client buffer.
 * SemaphoreHandle: A handle to a semaphore which is acquired when an 
 * event is about to be written to the client buffer. If the semaphore 
 * cannot be acquired immediately, the event is dropped. The client must 
 * continually read the buffer and release the semaphore.
 * BufferBase: A pointer to a buffer in the client process.
 * BufferSize: The size of the buffer, in bytes.
 * AccessMode: The mode to use when probing arguments.
 */
NTSTATUS KphSsCreateClientEntry(
    __out PKPHSS_CLIENT_ENTRY *ClientEntry,
    __in HANDLE ProcessHandle,
    __in HANDLE EventHandle,
    __in HANDLE SemaphoreHandle,
    __in PVOID BufferBase,
    __in ULONG BufferSize,
    __in KPROCESSOR_MODE AccessMode
    )
{
    NTSTATUS status = STATUS_SUCCESS;
    PKPHSS_CLIENT_ENTRY clientEntry;
    PEPROCESS processObject;
    PKEVENT eventObject;
    PKSEMAPHORE semaphoreObject;
    
    /* Probe. */
    if (AccessMode != KernelMode)
    {
        __try
        {
            ProbeForWrite(BufferBase, BufferSize, 1);
        }
        __except (EXCEPTION_EXECUTE_HANDLER)
        {
            return GetExceptionCode();
        }
    }
    
    /* Reference the client process. */
    status = ObReferenceObjectByHandle(
        ProcessHandle,
        PROCESS_VM_WRITE,
        *PsProcessType,
        AccessMode,
        &processObject,
        NULL
        );
    
    if (!NT_SUCCESS(status))
        return status;
    
    /* Reference the event. */
    status = ObReferenceObjectByHandle(
        EventHandle,
        EVENT_MODIFY_STATE,
        *ExEventObjectType,
        AccessMode,
        &eventObject,
        NULL
        );
    
    if (!NT_SUCCESS(status))
    {
        ObDereferenceObject(processObject);
        return status;
    }
    
    /* Reference the semaphore. */
    status = ObReferenceObjectByHandle(
        SemaphoreHandle,
        SEMAPHORE_MODIFY_STATE,
        *ExSemaphoreObjectType,
        AccessMode,
        &semaphoreObject,
        NULL
        );
    
    if (!NT_SUCCESS(status))
    {
        ObDereferenceObject(processObject);
        ObDereferenceObject(eventObject);
        return status;
    }
    
    /* Create the client entry object. */
    status = KphCreateObject(
        &clientEntry,
        sizeof(KPHSS_CLIENT_ENTRY),
        0,
        KphSsClientEntryType,
        0
        );
    
    if (!NT_SUCCESS(status))
    {
        ObDereferenceObject(processObject);
        ObDereferenceObject(eventObject);
        ObDereferenceObject(semaphoreObject);
        
        return status;
    }
    
    clientEntry->Process = processObject;
    clientEntry->Event = eventObject;
    clientEntry->Semaphore = semaphoreObject;
    ExInitializeFastMutex(&clientEntry->BufferMutex);
    clientEntry->BufferBase = BufferBase;
    clientEntry->BufferSize = BufferSize;
    clientEntry->BufferCursor = 0;
    
    *ClientEntry = clientEntry;
    
    return status;
}

/* KphpSsClientEntryDeleteProcedure
 * 
 * Performs cleanup for a client entry.
 */
VOID NTAPI KphpSsClientEntryDeleteProcedure(
    __in PVOID Object,
    __in ULONG Flags,
    __in SIZE_T Size
    )
{
    PKPHSS_CLIENT_ENTRY clientEntry = (PKPHSS_CLIENT_ENTRY)Object;
    
    ObDereferenceObject(clientEntry->Process);
    ObDereferenceObject(clientEntry->Event);
    ObDereferenceObject(clientEntry->Semaphore);
}

/* KphSsCreateProcessEntry
 * 
 * Creates a process entry which describes a process for which 
 * system services will be logged.
 */
NTSTATUS KphSsCreateProcessEntry(
    __out PKPHSS_PROCESS_ENTRY *ProcessEntry,
    __in PKPHSS_CLIENT_ENTRY ClientEntry,
    __in HANDLE TargetProcessHandle,
    __in ULONG Flags
    )
{
    NTSTATUS status = STATUS_SUCCESS;
    PKPHSS_PROCESS_ENTRY processEntry;
    PEPROCESS processObject;
    
    /* Check if the flags are valid. */
    if ((Flags & KPHSS_LOG_VALID_FLAGS) != Flags)
        return STATUS_INVALID_PARAMETER_4;
    
    /* If the caller didn't specify any mode flags, assume both modes. */
    if (!(Flags & (KPHSS_LOG_USER_MODE | KPHSS_LOG_KERNEL_MODE)))
        Flags |= KPHSS_LOG_USER_MODE | KPHSS_LOG_KERNEL_MODE;
    
    /* Reference the process object. Note that we don't actually 
     * need to keep the process object alive since we don't 
     * access it at any point.
     */
    status = ObReferenceObjectByHandle(
        TargetProcessHandle,
        0,
        *PsProcessType,
        KernelMode,
        &processObject,
        NULL
        );
    
    if (!NT_SUCCESS(status))
        return status;
    
    ObDereferenceObject(processObject);
    
    /* Create the process entry object. */
    status = KphCreateObject(
        &processEntry,
        sizeof(KPHSS_PROCESS_ENTRY),
        0,
        KphSsProcessEntryType,
        0
        );
    
    if (!NT_SUCCESS(status))
        return status;
    
    KphReferenceObject(ClientEntry);
    processEntry->Client = ClientEntry;
    processEntry->TargetProcess = processObject;
    processEntry->Flags = Flags;
    
    ExAcquireFastMutex(&KphSsMutex);
    InsertHeadList(&KphSsProcessListHead, &processEntry->ProcessListEntry);
    ExReleaseFastMutex(&KphSsMutex);
    
    *ProcessEntry = processEntry;
    
    return status;
}

/* KphpSsProcessEntryDeleteProcedure
 * 
 * Performs cleanup for a process entry.
 */
VOID NTAPI KphpSsProcessEntryDeleteProcedure(
    __in PVOID Object,
    __in ULONG Flags,
    __in SIZE_T Size
    )
{
    PKPHSS_PROCESS_ENTRY processEntry = (PKPHSS_PROCESS_ENTRY)Object;
    
    KphDereferenceObject(processEntry->Client);
    
    ExAcquireFastMutex(&KphSsMutex);
    RemoveEntryList(&processEntry->ProcessListEntry);
    ExReleaseFastMutex(&KphSsMutex);
}

/* KphpSsCreateEventBlock
 * 
 * Allocates and initializes an event block.
 * 
 * EventBlock: A variable which receives a pointer to the event block.
 * Thread: The thread for which the event is being generated.
 * Number: The system service number.
 * Arguments: A pointer to the caller-supplied arguments.
 * NumberOfArguments: The number of arguments, in ULONGs.
 */
NTSTATUS KphpSsCreateEventBlock(
    __out PKPHPSS_EVENT_BLOCK *EventBlock,
    __in PKTHREAD Thread,
    __in ULONG Number,
    __in ULONG *Arguments,
    __in ULONG NumberOfArguments
    )
{
    PKPHPSS_EVENT_BLOCK eventBlock;
    ULONG eventBlockSize;
    ULONG argumentsSize;
    ULONG traceSize;
    PVOID stackTrace[MAX_STACK_DEPTH];
    ULONG traceHash;
    ULONG capturedFrames;
    
    /* Get a stack trace. */
    capturedFrames = KphCaptureStackBackTrace(
        0,
        MAX_STACK_DEPTH,
        RTL_WALK_USER_MODE_STACK,
        stackTrace,
        &traceHash
        );
    
    /* Calculate the size of the event block. */
    argumentsSize = NumberOfArguments * sizeof(ULONG);
    traceSize = capturedFrames * sizeof(PVOID);
    eventBlockSize = sizeof(KPHPSS_EVENT_BLOCK) + argumentsSize + traceSize;
    
    /* Allocate the event block. */
    eventBlock = ExAllocatePoolWithTag(PagedPool, eventBlockSize, TAG_EVENT_BLOCK);
    
    if (!eventBlock)
        return STATUS_INSUFFICIENT_RESOURCES;
    
    /* Initialize the event block. */
    eventBlock->Header.Size = eventBlockSize;
    eventBlock->Header.Type = EventBlockType;
    eventBlock->Flags = 0;
    KeQuerySystemTime(&eventBlock->Time);
    eventBlock->ClientId.UniqueThread = PsGetThreadId(Thread);
    eventBlock->ClientId.UniqueProcess = PsGetProcessId(IoThreadToProcess(Thread));
    eventBlock->Number = Number;
    eventBlock->NumberOfArguments = NumberOfArguments;
    eventBlock->ArgumentsOffset = sizeof(KPHPSS_EVENT_BLOCK);
    eventBlock->TraceCount = capturedFrames;
    eventBlock->TraceHash = traceHash;
    eventBlock->TraceOffset = sizeof(KPHPSS_EVENT_BLOCK) + argumentsSize;
    
    /* Probe and copy the arguments. */
    if (KeGetPreviousMode() != KernelMode)
    {
        __try
        {
            ProbeForRead(Arguments, argumentsSize, 4);
        }
        __except (EXCEPTION_EXECUTE_HANDLER)
        {
            eventBlock->Flags |= KPHPSS_EVENT_PROBE_ARGUMENTS_FAILED;
        }
    }
    
    __try
    {
        /* Copy the arguments to the space immediately after the event block. */
        memcpy((PCHAR)eventBlock + eventBlock->ArgumentsOffset, Arguments, argumentsSize);
    }
    __except (EXCEPTION_EXECUTE_HANDLER)
    {
        eventBlock->Flags |= KPHPSS_EVENT_COPY_ARGUMENTS_FAILED;
    }
    
    /* Copy the stack trace. */
    memcpy((PCHAR)eventBlock + eventBlock->TraceOffset, stackTrace, traceSize);
    
    /* Pass the pointer to the event block back. */
    *EventBlock = eventBlock;
    
    return STATUS_SUCCESS;
}

/* KphpSsFreeEventBlock
 * 
 * Frees an event block created by KphpSsCreateEventBlock.
 */
VOID KphpSsFreeEventBlock(
    __in PKPHPSS_EVENT_BLOCK EventBlock
    )
{
    ExFreePoolWithTag(EventBlock, TAG_EVENT_BLOCK);
}

/* KphpSsWriteBlock
 * 
 * Writes a block into client memory.
 */
NTSTATUS KphpSsWriteBlock(
    __in PKPHSS_CLIENT_ENTRY ClientEntry,
    __in PKPHPSS_BLOCK_HEADER Block
    )
{
    NTSTATUS status = STATUS_SUCCESS;
    LARGE_INTEGER zeroTimeout;
    KPH_ATTACH_STATE attachState;
    ULONG availableSpace;
    ULONG blockSpaceNeeded;
    
    zeroTimeout.QuadPart = 0;
    
    ExAcquireFastMutex(&ClientEntry->BufferMutex);
    
    /* Try to acquire the client-specified semaphore. If we can't acquire 
     * it immediately, drop the block.
     */
    status = KeWaitForSingleObject(
        ClientEntry->Semaphore,
        Executive,
        KernelMode,
        FALSE,
        &zeroTimeout
        );
    
    if (!NT_SUCCESS(status) || status == STATUS_TIMEOUT)
    {
        if (status == STATUS_TIMEOUT)
            dfprintf("Ss: WARNING: Dropped block (server %#x)\n", ClientEntry->BufferCursor);
        
        ExReleaseFastMutex(&ClientEntry->BufferMutex);
        return status;
    }
    
    availableSpace = ClientEntry->BufferSize - ClientEntry->BufferCursor;
    /* The space we need includes the head block. */
    blockSpaceNeeded = Block->Size - sizeof(KPHPSS_HEAD_BLOCK);
    
    /* Blocks are recorded in a circular buffer. 
     * In the case that there is not enough space for an entire block, 
     * we will record a reset block that tells the client to reset 
     * its read cursor to 0. In the case that there is not enough 
     * space for a block header, it is implied that the client will 
     * reset its read cursor.
     */
    
    /* Check if we have enough space for a block header. */
    if (availableSpace < sizeof(KPHPSS_BLOCK_HEADER))
    {
        /* Not enough space. Reset the cursor. */
        ClientEntry->BufferCursor = 0;
        availableSpace = ClientEntry->BufferSize;
    }
    /* Check if we have enough space for the block. */
    else if (availableSpace < blockSpaceNeeded)
    {
        KPHPSS_RESET_BLOCK resetBlock;
        
        /* Not enough space for the block, but enough space 
         * for a reset block. Write the reset block and reset
         * the cursor.
         */
        resetBlock.Header.Size = sizeof(KPHPSS_RESET_BLOCK);
        resetBlock.Header.Type = ResetBlockType;
        
        /* Attach to the client process and copy the block. */
        KphAttachProcess(ClientEntry->Process, &attachState);
        
        __try
        {
            memcpy(
                PTR_ADD_OFFSET(ClientEntry->BufferBase, ClientEntry->BufferCursor),
                &resetBlock,
                resetBlock.Header.Size
                );
        }
        __except (EXCEPTION_EXECUTE_HANDLER)
        {
            KphDetachProcess(&attachState);
            ExReleaseFastMutex(&ClientEntry->BufferMutex);
            
            return GetExceptionCode();
        }
        
        KphDetachProcess(&attachState);
        ClientEntry->BufferCursor = 0;
        availableSpace = ClientEntry->BufferSize;
    }
    
    /* Now that we have dealt with any end-of-buffer issues, 
     * we still have to check if we have enough space for the 
     * event. We may have a huge event or the client may have a 
     * tiny buffer.
     */
    if (availableSpace < blockSpaceNeeded)
    {
        ExReleaseFastMutex(&ClientEntry->BufferMutex);
        return STATUS_BUFFER_TOO_SMALL;
    }
    
    /* Time to copy the block into the buffer. We also need to write 
     * a head block to tell the client when to stop reading.
     */
    KphAttachProcess(ClientEntry->Process, &attachState);
    
    __try
    {
        KPHPSS_HEAD_BLOCK headBlock;
        
        headBlock.Header.Size = sizeof(KPHPSS_HEAD_BLOCK);
        headBlock.Header.Type = HeadBlockType;
        
        memcpy(
            PTR_ADD_OFFSET(ClientEntry->BufferBase, ClientEntry->BufferCursor),
            Block,
            Block->Size
            );
        memcpy(
            PTR_ADD_OFFSET(ClientEntry->BufferBase, ClientEntry->BufferCursor + Block->Size),
            &headBlock,
            headBlock.Header.Size
            );
    }
    __except (EXCEPTION_EXECUTE_HANDLER)
    {
        KphDetachProcess(&attachState);
        ExReleaseFastMutex(&ClientEntry->BufferMutex);
        return GetExceptionCode();
    }
    
    KphDetachProcess(&attachState);
    
    /* Now that we have succesfully copied the block, we need to 
     * advance the cursor and signal the event.
     */
    ClientEntry->BufferCursor += Block->Size;
    KeSetEvent(ClientEntry->Event, 2, FALSE);
    
    dfprintf("Ss: Wrote block (server %#x).\n", ClientEntry->BufferCursor);
    
    ExReleaseFastMutex(&ClientEntry->BufferMutex);
    
    return status;
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
    __in ULONG *Arguments,
    __in ULONG NumberOfArguments,
    __in PKSERVICE_TABLE_DESCRIPTOR ServiceTable,
    __in PKTHREAD Thread
    )
{
    KPROCESSOR_MODE previousMode;
    PEPROCESS process;
    PLIST_ENTRY currentListEntry;
    PKPHSS_PROCESS_ENTRY processEntryArray[KPHSS_PROCESS_ENTRY_LIMIT];
    ULONG processEntryCount;
    PKPHPSS_EVENT_BLOCK eventBlock;
    ULONG i;
    
    previousMode = KeGetPreviousMode();
    
    /* First, some checks.
     *   * We can't operate at IRQL > APC_LEVEL because 
     *     of restrictions on logging.
     *   * We can't operate on unknown service tables like the 
     *     shadow service table (yet).
     *   * We have to make sure Thread isn't NULL, as it does 
     *     sometimes happen.
     *   * We have to make sure we aren't attempting to log 
     *     a call to ZwContinue because we caused an exception 
     *     last time we were logging something. This will cause 
     *     a deadlock!
     */
    
    if (KeGetCurrentIrql() > APC_LEVEL)
        return;
    if (ServiceTable != __KeServiceDescriptorTable)
        return;
    if (!Thread)
        return;
    
    /* Make sure we aren't logging ZwContinue if it's because 
     * we caused an exception somewhere. */
    if (
        Number == SysCallZwContinue && 
        NumberOfArguments == 2 && 
        previousMode == KernelMode
        )
    {
        /* "Reverse probe" the arguments. */
        if (
            (ULONG_PTR)Arguments > (ULONG_PTR)MmHighestUserAddress && 
            Arguments[0] > (ULONG_PTR)MmHighestUserAddress
            )
        {
            CONTEXT context;
            
            /* The first argument contains the context. */
            memcpy(&context, (PCONTEXT)Arguments[0], sizeof(CONTEXT));
            /* Check if the context Eip points into the KPH module.
             * If so, abort the logging.
             */
            if (
                context.Eip >= (ULONG_PTR)KphDriverObject->DriverStart && 
                context.Eip < (ULONG_PTR)KphDriverObject->DriverStart + KphDriverObject->DriverSize
                )
                return;
        }
    }
    
    /* Build the process entry array by going through the process 
     * list, referencing each relevant one and copying them into 
     * the local array. This we way don't hold the mutex for too 
     * long.
     */
    
    process = IoThreadToProcess(Thread);
    
    if (!process) /* should never happen */
        return;
    
    ExAcquireFastMutex(&KphSsMutex);
    
    currentListEntry = KphSsProcessListHead.Flink;
    processEntryCount = 0;
    
    while (
        currentListEntry != &KphSsProcessListHead && 
        processEntryCount <= KPHSS_PROCESS_ENTRY_LIMIT
        )
    {
        PKPHSS_PROCESS_ENTRY processEntry = KPHSS_PROCESS_ENTRY(currentListEntry);
        
        if (
            KphpSsIsProcessEntryRelevant(processEntry, process, previousMode) && 
            /* Make sure the process entry isn't being destroyed. */
            !KphIsDestroyedObject(processEntry)
            )
        {
            /* Reference and store the process entry in the local array. */
            KphReferenceObject(processEntry);
            processEntryArray[processEntryCount] = processEntry;
            processEntryCount++;
        }
        
        currentListEntry = currentListEntry->Flink;
    }
    
    ExReleaseFastMutex(&KphSsMutex);
    
    /* If we didn't find any process entries, don't bother creating the 
     * event block.
     */
    if (processEntryCount == 0)
        return;
    
    /* We have work to do. Create an event block first. */
    if (!NT_SUCCESS(KphpSsCreateEventBlock(
        &eventBlock,
        Thread,
        Number,
        Arguments,
        NumberOfArguments
        )))
        return;
    
    /* Go through the process entry array and write the block to each 
     * client. While we're doing thing we can also dereference each 
     * process entry.
     */
    for (i = 0; i < processEntryCount; i++)
    {
        KphpSsWriteBlock(processEntryArray[i]->Client, &eventBlock->Header);
        KphDereferenceObject(processEntryArray[i]);
    }
    
    /* Free the event block. */
    KphpSsFreeEventBlock(eventBlock);
}

/* KphpSsNewKiFastCallEntry
 * 
 * The hook function called from within the hooked KiFastCallEntry.
 */
__declspec(naked) VOID NTAPI KphpSsNewKiFastCallEntry()
{
    /* KiFastCallEntry handles system service calls. User-mode applications 
     * will perform system calls like this:
     * 
     * Nt*:
     * mov      eax, SystemServiceNumber
     * mov      edx, 0x7ffe0300 <-- at 0x7ffe0300 we have a pointer to KiFastSystemCall
     * call     [edx]
     * ret
     * 
     * At KiFastSystemCall:
     * mov      edx, esp
     * sysenter
     */
    /* This means that in KiFastCallEntry, eax will contain the system service 
     * number while edx will contain a pointer to the arguments for the system 
     * service. KiFastCallEntry will fill in edi with the service table, and 
     * esi will contain the caller KTHREAD.
     * 
     * We cannot hook KiFastCallEntry from the beginning because it starts on the DPC 
     * stack. KiFastCallEntry switches to the proper thread stack, and we want to 
     * hook it just after it switches to the stack. That way we can avoid having to 
     * manually switch the thread stack ourselves.
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
         * perform the job now - we have to increment the system calls 
         * counter.
         */
        lea     ebx, KphSsKiFastCallEntryHook /* get a pointer to the hook structure */
        mov     ebx, dword ptr [ebx+KPH_HOOK.Bytes+3] /* get the PbSystemCalls offset from the original inc instruction */
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
