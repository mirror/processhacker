#ifndef _SH_H
#define _SH_H

#include <ntstatus.h>
                                             
#define WIN32_LEAN_AND_MEAN
#define WIN32_NO_STATUS /* Need ntstatus.h instead */
#include <windows.h>

#define NTSTATUS LONG

#ifdef SH_EXPORTS
#define SH_API __declspec(dllexport)
#else
#define SH_API __declspec(dllimport)
#endif

typedef struct _CLIENT_ID
{
    ULONG UniqueProcess;
    ULONG UniqueThread;
} CLIENT_ID, *PCLIENT_ID;

typedef struct _UNICODE_STRING
{
    USHORT Length;
    USHORT MaximumLength;
    PWSTR Buffer;
} UNICODE_STRING, *PUNICODE_STRING;

typedef struct _OBJECT_ATTRIBUTES
{
    ULONG Length;
    HANDLE RootDirectory;
    PUNICODE_STRING ObjectName;
    ULONG Attributes;
    PVOID SecurityDescriptor;
    PVOID SecurityQualityOfService;
} OBJECT_ATTRIBUTES, *POBJECT_ATTRIBUTES;

typedef struct _HOOK
{
    PVOID Address;
    BOOLEAN Hooked;
    BYTE ReplacedBytes[16];
    ULONG ReplacedLength;
} HOOK, *PHOOK;

VOID ShInit();

VOID ShDeinit();

VOID ShUnpatchCall(
    PHOOK Hook
    );

PVOID ShGetProcAddress(
    PSTR LibraryName,
    PSTR ProcName
    );

#endif