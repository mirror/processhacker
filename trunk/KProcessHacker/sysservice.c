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

#include "include/sysservice.h"
#include "include/hook.h"

/* Whether system service logging has been initialized. */
BOOLEAN KphSsInitialized;
/* The KiFastCallEntry hook. */
KPH_HOOK KiFastCallEntryHook;

/* KphSsLogInit
 * 
 * Starts system service logging.
 */
NTSTATUS KphSsLogInit()
{
    NTSTATUS status = STATUS_SUCCESS;
    
    if (KphSsInitialized)
        return STATUS_UNSUCCESSFUL;
    
    /* This will overwrite the inc instruction in KiFastCallEntry with a jmp 
     * to KphpSsNewKiFastCallEntry.
     */
    KphInitializeHook(
        &KiFastCallEntryHook,
        __KiFastCallEntry,
        KphpSsNewKiFastCallEntry
        );
    status = KphHook(&KiFastCallEntryHook);
    KphSsInitialized = TRUE;
    
    return status;
}

/* KphSsLogDeinit
 * 
 * Stops system service logging.
 */
NTSTATUS KphSsLogDeinit()
{
    if (!KphSsInitialized)
        return STATUS_UNSUCCESSFUL;
    
    KphSsInitialized = FALSE;
    
    return KphUnhook(&KiFastCallEntryHook);
}

/* KphpSsLogSystemServiceCall
 * 
 * Logs a system service.
 */
VOID NTAPI KphpSsLogSystemServiceCall(
    __in ULONG Number,
    __in PVOID *Arguments,
    __in ULONG NumberOfArguments,
    __in PKSERVICE_TABLE_DESCRIPTOR ServiceTable,
    __in PKTHREAD Thread
    )
{
    /* First, some checks.
     *   * We can't operate at IRQL > DISPATCH_LEVEL because 
     *     of restrictions on stack traces.
     *   * We can't operate on unknown service tables like the 
     *     shadow service table (yet).
     *   * We have to make sure Thread isn't NULL, as it does 
     *     sometimes happen.
     */
    
    if (KeGetCurrentIrql() > DISPATCH_LEVEL)
        return;
    if (ServiceTable != __KeServiceDescriptorTable)
        return;
    if (!Thread)
        return;
    
    
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
        lea     ebx, KiFastCallEntryHook /* get a pointer to the hook structure */
        mov     ebx, dword ptr [ebx+KPH_HOOK.Bytes+3] /* get the PbSystemCalls offset from the original instruction */
        inc     dword ptr fs:[ebx] /* increment PbSystemCalls in the PRCB */
        
        /* Get the number of arguments to this system service. */
        mov     ebx, dword ptr [edi+KSERVICE_TABLE_DESCRIPTOR.Number] /* ebx = a pointer to the argument table */
        xor     ecx, ecx
        mov     cl, [ebx+eax] /* ecx = size of the arguments, in bytes. */
        shr     ecx, 2 /* divide by 2 to get the number of arguments (all ULONGs) */
        
        /* Call the KiFastCall proc. */
        push    esi /* Thread */
        push    edi /* ServiceTable */
        push    ecx /* NumberOfArguments */
        push    edx /* Arguments */
        push    eax /* Number */
        call    KphpSsLogSystemCall
        
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
