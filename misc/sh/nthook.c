/*
 * Sh Hooking Library - 
 *   Native API hooks
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

#include "nthook.h"

__declspec(naked) NTSTATUS NTAPI ShNtCall(
    PNT_HOOK NtHook,
    PVOID FirstArgument
    )
{
    __asm
    {
        push    ebp
        mov     ebp, esp  
        push    esi
        push    edi

        /* Allocate space for the arguments, if necessary. */
        mov     esi, [FirstArgument]
        test    esi, esi
        jz      NoArguments
        mov     eax, [NtHook]
        movzx   ecx, word ptr [eax+NT_HOOK.ArgumentLength]
        sub     esp, ecx
        /* Copy the arguments. */
        mov     edi, esp
        repne   movs [edi], [esi]

NoArguments:
        /* Allocate 4 bytes because that is where the return address normally should be. 
           The system service dispatcher skips it. */
        sub     esp, 4
        /* Move the call index into eax and perform the system call. */
        mov     eax, [NtHook]
        mov     eax, [eax+NT_HOOK.SystemCallIndex]
        mov     edx, 0x7ffe0300
        call    [edx]
        /* Deallocate the 4 bytes */
        add     esp, 4

        /* Deallocate the space we allocated for the arguments, if necessary. */
        mov     esi, [FirstArgument]
        test    esi, esi
        jz      NoArguments2
        mov     edx, [NtHook]
        movzx   ecx, word ptr [edx+NT_HOOK.ArgumentLength]
        add     esp, ecx

NoArguments2:
        pop     edi
        pop     esi
        mov     esp, ebp
        pop     ebp
        ret     0x8
    }
}

NTSTATUS ShNtPatchCall(
    PSTR Name,
    PVOID Target,
    PNT_HOOK NtHook
    )
{
    NTSTATUS status = STATUS_SUCCESS;
    PBYTE callAddress = (PBYTE)ShGetProcAddress("ntdll.dll", Name);
    ULONG oldProtection;

    if (callAddress == 0)
        return STATUS_NOT_FOUND;

    VirtualProtect(callAddress, 10, PAGE_EXECUTE_READWRITE, &oldProtection);

    NtHook->Hook.Address = callAddress;     
    NtHook->Hook.Hooked = FALSE;
    RtlCopyMemory(NtHook->Hook.ReplacedBytes, callAddress, NtHook->Hook.ReplacedLength = 10);
    NtHook->SystemCallIndex = *(PULONG)(callAddress + 1);
    NtHook->ArgumentLength = *(PUSHORT)(callAddress + 13);

    __try
    {
        /* mov eax, Target */
        *callAddress = 0xb8;
        *(PVOID *)(callAddress + 1) = Target;
        /* jmp eax */
        *(callAddress + 5) = 0xff;
        *(callAddress + 6) = 0xe0;
        /* three nops */
        *(callAddress + 7) = 0x90;
        *(callAddress + 8) = 0x90;
        *(callAddress + 9) = 0x90;
    }
    __except (EXCEPTION_EXECUTE_HANDLER)
    {
        __try
        {
            ShNtUnpatchCall(NtHook);
        }
        __except (EXCEPTION_EXECUTE_HANDLER)
        { }

        return GetExceptionCode();
    }

    VirtualProtect(callAddress, 10, oldProtection, NULL);

    NtHook->Hook.Hooked = TRUE;

    return STATUS_SUCCESS;
}

NTSTATUS ShNtUnpatchCall(
    PNT_HOOK NtHook
    )
{
    return ShUnpatchCall(&NtHook->Hook);
}
