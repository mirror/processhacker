#ifndef _NTHOOK_H
#define _NTHOOK_H

#include "hook.h"

typedef struct _NT_HOOK
{
    HOOK Hook;
    ULONG SystemCallIndex;
    USHORT ArgumentLength;
} NT_HOOK, *PNT_HOOK;

NTSTATUS ShNtCall(
    PNT_HOOK NtHook,
    PVOID FirstArgument
    );

NTSTATUS ShNtPatchCall(
    PSTR Name,
    PVOID Target,
    PNT_HOOK NtHook
    );

NTSTATUS ShNtUnpatchCall(
    PNT_HOOK NtHook
    );

#endif
