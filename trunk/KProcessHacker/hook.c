/*
 * Process Hacker Driver - 
 *   hooks
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

#include "include/hook.h"
#include "include/sync.h"

typedef struct _MAPPED_MDL
{
    PMDL Mdl;
    PVOID Address;
} MAPPED_MDL, *PMAPPED_MDL;

NTSTATUS KphpCreateMappedMdl(
    PVOID Address,
    ULONG Length,
    PMAPPED_MDL MappedMdl
    );

VOID KphpFreeMappedMdl(
    PMAPPED_MDL MappedMdl
    );

static KPH_PROCESSOR_LOCK HookProcessorLock;

/* KphHook
 * 
 * Hooks a kernel-mode function.
 * WARNING: DO NOT HOOK A FUNCTION THAT IS CALLABLE ABOVE APC_LEVEL.
 * 
 * Thread safety: Full
 * IRQL: <= APC_LEVEL
 */
NTSTATUS KphHook(
    PKPH_HOOK Hook
    )
{
    NTSTATUS status = STATUS_SUCCESS;
    KIRQL oldIrql;
    MAPPED_MDL mappedMdl;
    PCHAR function;
    
    status = KphpCreateMappedMdl(
        Hook->Function,
        5,
        &mappedMdl
        );
    
    if (!NT_SUCCESS(status))
        return status;
    
    function = (PCHAR)mappedMdl.Address;
    
    /* Acquire a lock on all other processors. */
    if (KphAcquireProcessorLock(&HookProcessorLock))
    {
        /* Patch the function. */
        memcpy(Hook->Bytes, function, 5);
        Hook->Hooked = TRUE;
        /* jmp Target */
        *function = 0xe9;
        *(PULONG_PTR)(function + 1) = (ULONG_PTR)Hook->Target - (ULONG_PTR)Hook->Function - 5;
        
        /* Release the processor lock. */
        KphReleaseProcessorLock(&HookProcessorLock);
    }
    else
    {
        dprintf("KphHook: Could not acquire processor lock!\n");
        status = STATUS_INSUFFICIENT_RESOURCES;
    }
    
    KphpFreeMappedMdl(&mappedMdl);
    
    return status;
}

/* KphHookInit
 * 
 * Initializes the hooking module.
 */
NTSTATUS KphHookInit()
{
    KphInitializeProcessorLock(&HookProcessorLock);
    
    return STATUS_SUCCESS;
}

/* KphUnhook
 * 
 * Unhooks a kernel-mode function.
 * WARNING: DO NOT UNHOOK A FUNCTION THAT IS CALLABLE ABOVE APC_LEVEL.
 * 
 * Thread safety: Full
 * IRQL: <= APC_LEVEL
 */
NTSTATUS KphUnhook(
    PKPH_HOOK Hook
    )
{
    NTSTATUS status = STATUS_SUCCESS;
    KIRQL oldIrql;
    MAPPED_MDL mappedMdl;
    
    if (!Hook->Hooked)
        return STATUS_UNSUCCESSFUL;
    
    status = KphpCreateMappedMdl(
        Hook->Function,
        5,
        &mappedMdl
        );
    
    if (!NT_SUCCESS(status))
        return status;
    
    /* Acquire a lock on all other processors. */
    if (KphAcquireProcessorLock(&HookProcessorLock))
    {
        /* Unpatch the function. */
        memcpy(mappedMdl.Address, Hook->Bytes, 5);
        Hook->Hooked = FALSE;
        /* Release the processor lock. */
        KphReleaseProcessorLock(&HookProcessorLock);
    }
    else
    {
        dprintf("KphUnhook: Could not acquire processor lock!\n");
        status = STATUS_INSUFFICIENT_RESOURCES;
    }
    
    KphpFreeMappedMdl(&mappedMdl);
    
    return status;
}

/* KphpCreateMappedMdl
 * 
 * Creates and maps a MDL.
 * 
 * Thread safety: Full
 * IRQL: Any
 */
NTSTATUS KphpCreateMappedMdl(
    PVOID Address,
    ULONG Length,
    PMAPPED_MDL MappedMdl
    )
{
    PMDL mdl;
    
    MappedMdl->Mdl = NULL;
    MappedMdl->Address = NULL;
    
    mdl = IoAllocateMdl(Address, Length, FALSE, FALSE, NULL);
    
    if (mdl == NULL)
        return STATUS_INSUFFICIENT_RESOURCES;
    
    MmBuildMdlForNonPagedPool(mdl);
    mdl->MdlFlags |= MDL_MAPPED_TO_SYSTEM_VA;
    MappedMdl->Address = MmMapLockedPagesSpecifyCache(
        mdl,
        KernelMode,
        MmNonCached,
        NULL,
        FALSE,
        HighPagePriority
        );
    MappedMdl->Mdl = mdl;
    
    if (!MappedMdl->Address)
    {
        KphpFreeMappedMdl(MappedMdl);
        return STATUS_INSUFFICIENT_RESOURCES;
    }
    
    return STATUS_SUCCESS;
}

/* KphpFreeMappedMdl
 * 
 * Unmaps and frees a MDL.
 * 
 * Thread safety: Full
 * IRQL: Any
 */
VOID KphpFreeMappedMdl(
    PMAPPED_MDL MappedMdl
    )
{
    if (MappedMdl->Mdl != NULL)
    {
        if (MappedMdl->Address != NULL)
        {
            MmUnmapLockedPages(MappedMdl->Address, MappedMdl->Mdl);
            MappedMdl->Address = NULL;
        }
        
        IoFreeMdl(MappedMdl->Mdl);
        MappedMdl->Mdl = NULL;
    }
}
