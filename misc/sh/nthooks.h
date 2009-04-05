#ifndef _NTHOOKS_H
#define _NTHOOKS_H

#include "sh.h"

typedef struct _NT_HOOK
{
    HOOK Hook;
    ULONG SystemCallIndex;
    USHORT ArgumentLength;
} NT_HOOK, *PNT_HOOK;

VOID ShNtPatch();

VOID ShNtUnpatch();

NTSTATUS ShNtSystemCall(
    PNT_HOOK NtHook,
    PVOID FirstArgument
    );

VOID ShPatchNtCall(
    PSTR Name,
    PVOID Target,
    PNT_HOOK NtHook
    );

VOID ShNtUnpatchCall(
    PNT_HOOK NtHook
    );

#endif
