#ifndef _COMMON_H
#define _COMMON_H

#include <windows.h>
#include <string.h>
#include <stdlib.h>

typedef enum _CM_TYPE
{
    CmVoid = 0,
    CmBool,
    CmByte,
    CmInt16,
    CmInt32,
    CmPVoid,
    CmBytes,
    CmString,
    CmType = 0x00ffffff,

    CmHex = 0x01000000,
    CmDisplayHint = 0xff000000
} CM_TYPE, *PCM_TYPE;

PBYTE CmMakeDictionary(
    PULONG BufferLength,
    ULONG Length,
    ...
    );

#endif