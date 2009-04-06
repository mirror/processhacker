#ifndef _APILOG_H
#define _APILOG_H

#include "nthook.h"
#include "common.h"

#define AL_PIPE_NAME (L"\\\\.\\Pipe\\AlLogPipe")

VOID AlLogCall(
    PWSTR Name,
    PNT_HOOK NtHook,
    PBYTE Dictionary,
    ULONG DictionaryLength
    );

VOID AlPatch();
VOID AlUnpatch();
VOID AlInit();
VOID AlDeinit();

#endif