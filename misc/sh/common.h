#ifndef _COMMON_H
#define _COMMON_H

#include <windows.h>
#include <string.h>
#include <stdlib.h>

PBYTE CmMakeDictStringULong(
    PULONG BufferLength,
    ULONG Length,
    ...
    );

#endif