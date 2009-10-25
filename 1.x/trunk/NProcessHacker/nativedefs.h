#ifndef _NATIVEDEFS_H
#define _NATIVEDEFS_H

#include "nph.h"

typedef enum _OBJECT_INFORMATION_CLASS
{
    ObjectBasicInformation,
    ObjectNameInformation,
    ObjectTypeInformation,
    ObjectAllInformation,
    ObjectDataInformation
} OBJECT_INFORMATION_CLASS, *POBJECT_INFORMATION_CLASS;

typedef struct _UNICODE_STRING UNICODE_STRING, *PUNICODE_STRING;

typedef struct _CLIENT_ID
{
    HANDLE UniqueProcess;
    HANDLE UniqueThread;
} CLIENT_ID, *PCLIENT_ID;

typedef struct _IO_STATUS_BLOCK
{
    union
    {
        NTSTATUS Status;
        PVOID Pointer;
    };
    ULONG_PTR Information;
} IO_STATUS_BLOCK, *PIO_STATUS_BLOCK;

typedef struct _OBJECT_ATTRIBUTES
{
    ULONG Length;
    PVOID RootDirectory;
    PUNICODE_STRING ObjectName;
    ULONG Attributes;
    PVOID SecurityDescriptor;
    PVOID SecurityQualityOfService;
} OBJECT_ATTRIBUTES, *POBJECT_ATTRIBUTES;

typedef struct _UNICODE_STRING
{
    USHORT Length;
    USHORT MaximumLength;
    PWSTR Buffer;
} UNICODE_STRING, *PUNICODE_STRING;

typedef struct _OBJECT_NAME_INFORMATION
{
    UNICODE_STRING Name;
} OBJECT_NAME_INFORMATION, *POBJECT_NAME_INFORMATION;

#define NTDEVICEIOCONTROLFILE_ARGS \
    HANDLE FileHandle, \
    HANDLE Event, \
    PVOID ApcRoutine, \
    PVOID ApcContext, \
    PIO_STATUS_BLOCK IoStatusBlock, \
    ULONG IoControlCode, \
    PVOID InputBuffer, \
    ULONG InputBufferLength, \
    PVOID OutputBuffer, \
    ULONG OutputBufferLength

typedef NTSTATUS (NTAPI *_NtDeviceIoControlFile)(      
    NTDEVICEIOCONTROLFILE_ARGS
    );

#define NTGETCONTEXTTHREAD_ARGS \
    HANDLE ThreadHandle, \
    PCONTEXT Context

typedef NTSTATUS (NTAPI *_NtGetContextThread)(
    NTGETCONTEXTTHREAD_ARGS
    );

#define NTOPENPROCESS_ARGS \
    PHANDLE ProcessHandle, \
    ACCESS_MASK DesiredAccess, \
    POBJECT_ATTRIBUTES ObjectAttributes, \
    PCLIENT_ID ClientId

typedef NTSTATUS (NTAPI *_NtOpenProcess)(
    NTOPENPROCESS_ARGS
    );

#define NTOPENPROCESSTOKEN_ARGS \
    HANDLE ProcessHandle, \
    ACCESS_MASK DesiredAccess, \
    PHANDLE TokenHandle

typedef NTSTATUS (NTAPI *_NtOpenProcessToken)(
    NTOPENPROCESSTOKEN_ARGS
    );

#define NTOPENPROCESSTOKENEX_ARGS \
    HANDLE ProcessHandle, \
    ACCESS_MASK DesiredAccess, \
    ULONG HandleAttributes, \
    PHANDLE TokenHandle

typedef NTSTATUS (NTAPI *_NtOpenProcessTokenEx)(
    NTOPENPROCESSTOKENEX_ARGS
    );

#define NTOPENTHREAD_ARGS \
    PHANDLE ThreadHandle, \
    ACCESS_MASK DesiredAccess, \
    POBJECT_ATTRIBUTES ObjectAttributes, \
    PCLIENT_ID ClientId

typedef NTSTATUS (NTAPI *_NtOpenThread)(
    NTOPENTHREAD_ARGS
    );

#define NTQUERYOBJECT_ARGS \
    HANDLE Handle, \
    OBJECT_INFORMATION_CLASS ObjectInformationClass, \
    PVOID ObjectInformation, \
    ULONG Length, \
    PULONG ReturnLength

typedef NTSTATUS (NTAPI *_NtQueryObject)(
    NTQUERYOBJECT_ARGS
    );

#define NTREADVIRTUALMEMORY_ARGS \
    HANDLE ProcessHandle, \
    PVOID BaseAddress, \
    PVOID Buffer, \
    ULONG BufferLength, \
    PULONG ReturnLength

typedef NTSTATUS (NTAPI *_NtReadVirtualMemory)(
    NTREADVIRTUALMEMORY_ARGS
    );

#define NTSETCONTEXTTHREAD_ARGS \
    HANDLE ThreadHandle, \
    PCONTEXT Context

typedef NTSTATUS (NTAPI *_NtSetContextThread)(
    NTSETCONTEXTTHREAD_ARGS
    );

#define NTTERMINATEPROCESS_ARGS \
    HANDLE ProcessHandle, \
    NTSTATUS ExitStatus

typedef NTSTATUS (NTAPI *_NtTerminateProcess)(
    NTTERMINATEPROCESS_ARGS
    );

#define NTTERMINATETHREAD_ARGS \
    HANDLE ThreadHandle, \
    NTSTATUS ExitStatus

typedef NTSTATUS (NTAPI *_NtTerminateThread)(
    NTTERMINATETHREAD_ARGS
    );

#define NTWRITEVIRTUALMEMORY_ARGS \
    HANDLE ProcessHandle, \
    PVOID BaseAddress, \
    PVOID Buffer, \
    ULONG BufferLength, \
    PULONG ReturnLength

typedef NTSTATUS (NTAPI *_NtWriteVirtualMemory)(
    NTWRITEVIRTUALMEMORY_ARGS
    );

#endif