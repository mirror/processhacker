/*
 * Sh Hooking Library - 
 *   kernel32.dll hooks
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

#include "kehook.h"

/* Standard kernel32.dll export prologue:
   mov      edi, edi
   push     ebp
   mov      ebp, esp
 */
static char ShKePrologue[] = { 0x8b, 0xff, 0x55, 0x8b, 0xec };

SHAPI __declspec(naked) ULONG __stdcall ShKeCall(
    PKE_HOOK KeHook,
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
        mov     eax, [KeHook]
        movzx   ecx, word ptr [eax+KE_HOOK.ArgumentLength]
        sub     esp, ecx
        /* Copy the arguments. */
        mov     edi, esp
        repne   movs [edi], [esi]

NoArguments:
        mov     eax, [KeHook]
        mov     eax, [eax+KE_HOOK.Hook.Address]
        add     eax, 5
        /* Set up a stack frame for the original function. 
           We are assuming that the bytes we patched was in fact the standard
           kernel32.dll prologue.
         */
        push    AfterJump /* return address */
        push    ebp 
        mov     ebp, esp
        jmp     eax

AfterJump:
        pop     edi
        pop     esi
        mov     esp, ebp
        pop     ebp
        ret     0x8
    }
}

SHAPI NTSTATUS ShKePatchCall(
    PSTR Name,
    PVOID Target,
    USHORT ArgumentLength,
    PKE_HOOK KeHook
    )
{
    NTSTATUS status = STATUS_SUCCESS;
    PBYTE callAddress = (PBYTE)ShGetProcAddress("kernel32.dll", Name);
    ULONG oldProtection;

    if (callAddress == 0)
        return STATUS_NOT_FOUND;

    VirtualProtect(callAddress, 5, PAGE_EXECUTE_READWRITE, &oldProtection);

    KeHook->Hook.Address = callAddress;
    KeHook->ArgumentLength = ArgumentLength;
    KeHook->Hook.Hooked = FALSE;
    RtlCopyMemory(KeHook->Hook.ReplacedBytes, callAddress, KeHook->Hook.ReplacedLength = 5);

    __try
    {
        /* jmp Target */
        *callAddress = 0xe9;
        *(PULONG)(callAddress + 1) = (ULONG)Target - (ULONG)callAddress - 5;
    }
    __except (EXCEPTION_EXECUTE_HANDLER)
    {
        VirtualProtect(callAddress, 10, oldProtection, NULL);

        __try
        {
            ShKeUnpatchCall(KeHook);
        }
        __except (EXCEPTION_EXECUTE_HANDLER)
        { }

        return GetExceptionCode();
    }

    VirtualProtect(callAddress, 5, oldProtection, NULL);

    KeHook->Hook.Hooked = TRUE;

    return STATUS_SUCCESS;
}

SHAPI NTSTATUS ShKeUnpatchCall(
    PKE_HOOK KeHook
    )
{
    return ShUnpatchCall(&KeHook->Hook);
}
