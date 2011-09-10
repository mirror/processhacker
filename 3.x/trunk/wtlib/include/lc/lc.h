#ifndef _LC_LC_H
#define _LC_LC_H

#pragma once

#ifndef LC_NO_DEFAULT_LIB
#pragma comment(lib, "ntdll.lib")
#endif

// nonstandard extension used : nameless struct/union
#pragma warning(disable: 4201)
// nonstandard extension used : bit field types other than int
#pragma warning(disable: 4214)
// 'function': attributes not present on previous declaration
#pragma warning(disable: 4985)
// 'function': was declared deprecated
#pragma warning(disable: 4996)

#if defined(LCLIB_IMPORT)
#define LCLIBAPI __declspec(dllimport)
#else
#define LCLIBAPI
#endif

#include <ntwin.h>
#include <lcnt.h>

#ifdef __cplusplus
extern "C" {
#endif

// string

typedef struct _LC_STRING
{
    SIZE_T Length;
    PWCHAR Buffer;
    ULONG Flags;
    PVOID Data;
} LC_STRING, *PLC_STRING;

#define LC_STRING_INIT(Buffer) { wcslen(Buffer) * sizeof(WCHAR), (Buffer), 0, NULL }

#ifdef __cplusplus
}
#endif

#endif
