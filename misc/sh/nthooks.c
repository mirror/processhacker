#include "nthooks.h"

__declspec(naked) NTSTATUS ShNtSystemCall(
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

        /* Allocate space for the arguments. */
        mov     eax, [NtHook]
        movzx   ecx, word ptr [eax+NT_HOOK.ArgumentLength]
        sub     esp, ecx
        /* Copy the arguments. */
        mov     esi, [FirstArgument]
        mov     edi, esp
        repne   movs [edi], [esi]
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
        /* Deallocate the space we allocated for the arguments */
        mov     edx, [NtHook]
        movzx   ecx, word ptr [edx+NT_HOOK.ArgumentLength]
        add     esp, ecx

        pop     edi
        pop     esi
        mov     esp, ebp
        pop     ebp
        ret
    }
}

NTSTATUS ShNtPatchCall(
    PSTR Name,
    PVOID Target,
    PNT_HOOK NtHook
    )
{
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

VOID ShNtUnpatchCall(
    PNT_HOOK NtHook
    )
{
    ShUnpatchCall(&NtHook->Hook);
}

NT_HOOK ShNtOpenProcessHook;

NTSTATUS NTAPI ShNtOpenProcess(
    PHANDLE ProcessHandle,
    ACCESS_MASK DesiredAccess,
    POBJECT_ATTRIBUTES ObjectAttributes,
    PCLIENT_ID ClientId
    )
{
    if (ClientId->UniqueProcess == 4)
        return STATUS_ACCESS_DENIED;

    return ShNtSystemCall(&ShNtOpenProcessHook, &ProcessHandle);
}

VOID ShNtPatch()
{
    ShNtPatchCall("NtOpenProcess", ShNtOpenProcess, &ShNtOpenProcessHook);
}

VOID ShNtUnpatch()
{
    ShNtUnpatchCall(&ShNtOpenProcessHook);
}
