#include "hook.h"

PVOID ShGetProcAddress(
    PSTR LibraryName,
    PSTR ProcName
    )
{
    return (PVOID)GetProcAddress(GetModuleHandleA(LibraryName), ProcName);
}

NTSTATUS ShUnpatchCall(
    PHOOK Hook
    )
{
    memcpy(Hook->Address, Hook->ReplacedBytes, Hook->ReplacedLength);
    Hook->Hooked = FALSE;

    return STATUS_SUCCESS;
}
