#include "sh.h"
#include "nthooks.h"

PVOID ShGetProcAddress(
    PSTR LibraryName,
    PSTR ProcName
    )
{
    return (PVOID)GetProcAddress(GetModuleHandleA(LibraryName), ProcName);
}

VOID ShUnpatchCall(
    PHOOK Hook
    )
{
    RtlCopyMemory(Hook->Address, Hook->ReplacedBytes, Hook->ReplacedLength);
    Hook->Hooked = FALSE;
}

VOID ShInit()
{
    ShNtPatch();
}

VOID ShDeinit()
{
    ShNtUnpatch();
}
