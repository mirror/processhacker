/*
 * Process Hacker Driver - 
 *   hooks header file
 * 
 * Copyright (C) 2009 wj32
 * 
 * This file is part of Process Hacker.
 * 
 * Process Hacker is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * Process Hacker is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Process Hacker.  If not, see <http://www.gnu.org/licenses/>.
 */

#ifndef _HOOKS_H
#define _HOOKS_H

#include <ntifs.h>

/* ---- CONFIGURATION ---- */
/* If enabled, user-mode processes other than the client 
 * cannot open the client's process or its threads.
 */
//#define PROTECT_CLIENT

/* File hooks - ZwCreateFile, ZwOpenFile, ReadFile, WriteFile, etc. */
//#define HOOK_FILE

/* Key hooks - ZwCreateKey, ZwDeleteKey, etc. */
//#define HOOK_KEY

/* Process hooks - ZwOpenProcess, ZwOpenThread, etc. */
#define HOOK_PROCESS

/* Information hooks - ZwDuplicateObject, ZwQuerySystem*, ZwSetSystem* */
#define HOOK_INFORMATION
/* -------- */

NTSTATUS KPHHook();
void KPHUnhook();

typedef struct _KTHREAD
{
     DISPATCHER_HEADER Header;
     UINT64 CycleTime;
     ULONG HighCycleTime;
     UINT64 QuantumTarget;
     PVOID InitialStack;
     PVOID StackLimit;
     PVOID KernelStack;
     ULONG ThreadLock;
     union
     {
          KAPC_STATE ApcState;
          UCHAR ApcStateFill[23];
     };
     CHAR Priority;
     SHORT NextProcessor;
     SHORT DeferredProcessor;
     ULONG ApcQueueLock;
     ULONG ContextSwitches;
     UCHAR State;
     UCHAR NpxState;
     UCHAR WaitIrql;
     CHAR WaitMode;
     LONG WaitStatus;
     union
     {
          PKWAIT_BLOCK WaitBlockList;
          PKGATE GateObject;
     };
     union
     {
          ULONG KernelStackResident: 1;
          ULONG ReadyTransition: 1;
          ULONG ProcessReadyQueue: 1;
          ULONG WaitNext: 1;
          ULONG SystemAffinityActive: 1;
          ULONG Alertable: 1;
          ULONG GdiFlushActive: 1;
          ULONG Reserved: 25;
          LONG MiscFlags;
     };
     UCHAR WaitReason;
     UCHAR SwapBusy;
     UCHAR Alerted[2];
     union
     {
          LIST_ENTRY WaitListEntry;
          SINGLE_LIST_ENTRY SwapListEntry;
     };
     PKQUEUE Queue;
     ULONG WaitTime;
     union
     {
          struct
          {
               SHORT KernelApcDisable;
               SHORT SpecialApcDisable;
          };
          ULONG CombinedApcDisable;
     };
     PVOID Teb;
     union
     {
          KTIMER Timer;
          UCHAR TimerFill[40];
     };
     union
     {
          ULONG AutoAlignment: 1;
          ULONG DisableBoost: 1;
          ULONG EtwStackTraceApc1Inserted: 1;
          ULONG EtwStackTraceApc2Inserted: 1;
          ULONG CycleChargePending: 1;
          ULONG CalloutActive: 1;
          ULONG ApcQueueable: 1;
          ULONG EnableStackSwap: 1;
          ULONG GuiThread: 1;
          ULONG ReservedFlags: 23;
          LONG ThreadFlags;
     };
     union
     {
          KWAIT_BLOCK WaitBlock[4];
          struct
          {
               UCHAR WaitBlockFill0[23];
               UCHAR IdealProcessor;
          };
          struct
          {
               UCHAR WaitBlockFill1[47];
               CHAR PreviousMode;
          };
          struct
          {
               UCHAR WaitBlockFill2[71];
               UCHAR ResourceIndex;
          };
          UCHAR WaitBlockFill3[95];
     };
     UCHAR LargeStack;
     LIST_ENTRY QueueListEntry;
     PVOID TrapFrame;
     PVOID FirstArgument;
     union
     {
          PVOID CallbackStack;
          ULONG CallbackDepth;
     };
     PVOID ServiceTable;
     UCHAR ApcStateIndex;
     CHAR BasePriority;
     CHAR PriorityDecrement;
     UCHAR Preempted;
     UCHAR AdjustReason;
     CHAR AdjustIncrement;
     UCHAR Spare01;
     CHAR Saturation;
     ULONG SystemCallNumber;
     ULONG Spare02;
     ULONG UserAffinity;
     PKPROCESS Process;
     ULONG Affinity;
     PKAPC_STATE ApcStatePointer[2];
     union
     {
          KAPC_STATE SavedApcState;
          UCHAR SavedApcStateFill[23];
     };
     CHAR FreezeCount;
     CHAR SuspendCount;
     UCHAR UserIdealProcessor;
     UCHAR Spare03;
     UCHAR Iopl;
     PVOID Win32Thread;
     PVOID StackBase;
     union
     {
          KAPC SuspendApc;
          struct
          {
               UCHAR SuspendApcFill0[1];
               CHAR Spare04;
          };
          struct
          {
               UCHAR SuspendApcFill1[3];
               UCHAR QuantumReset;
          };
          struct
          {
               UCHAR SuspendApcFill2[4];
               ULONG KernelTime;
          };
          struct
          {
               UCHAR SuspendApcFill3[36];
               PVOID WaitPrcb;
          };
          struct
          {
               UCHAR SuspendApcFill4[40];
               PVOID LegoData;
          };
          UCHAR SuspendApcFill5[47];
     };
     UCHAR PowerState;
     ULONG UserTime;
     union
     {
          KSEMAPHORE SuspendSemaphore;
          UCHAR SuspendSemaphorefill[20];
     };
     ULONG SListFaultCount;
     LIST_ENTRY ThreadListEntry;
     LIST_ENTRY MutantListHead;
     PVOID SListFaultAddress;
     PVOID MdlForLockedTeb;
} KTHREAD, *PKTHREAD;

typedef NTSTATUS (*_ZwCreateFile)(
    PHANDLE FileHandle,
    ACCESS_MASK DesiredAccess,
    POBJECT_ATTRIBUTES ObjectAttributes,
    PIO_STATUS_BLOCK IoStatusBlock,
    PLARGE_INTEGER AllocationSize,
    ULONG FileAttributes,
    ULONG ShareAccess,
    ULONG CreateDisposition,
    ULONG CreateOptions,
    PVOID EaBuffer,
    ULONG EaLength
    );

typedef NTSTATUS (*_ZwCreateKey)(
    PHANDLE KeyHandle,
    ACCESS_MASK DesiredAccess,
    POBJECT_ATTRIBUTES ObjectAttributes,
    ULONG TitleIndex,
    PUNICODE_STRING Class,
    ULONG CreateOptions,
    PULONG Disposition
    );

typedef NTSTATUS (*_ZwDeleteKey)(
    HANDLE KeyHandle
    );

typedef NTSTATUS (*_ZwDeleteValueKey)(
    HANDLE KeyHandle,
    PUNICODE_STRING ValueName
    );

typedef NTSTATUS (*_ZwDuplicateObject)(
    HANDLE SourceProcessHandle,
    HANDLE SourceHandle,
    HANDLE DestinationProcessHandle,
    PHANDLE DestinationHandle,
    ACCESS_MASK DesiredAccess,
    int Attributes,
    int Options);

typedef NTSTATUS (*_ZwEnumerateKey)(
    HANDLE KeyHandle,
    ULONG Index,
    KEY_INFORMATION_CLASS KeyInformationClass,
    PVOID KeyInformation,
    ULONG Length,
    PULONG ResultLength
    );

typedef NTSTATUS (*_ZwEnumerateValueKey)(
    HANDLE KeyHandle,
    ULONG Index,
    KEY_VALUE_INFORMATION_CLASS KeyValueInformationClass,
    PVOID KeyValueInformation,
    ULONG Length,
    PULONG ResultLength
    );

typedef NTSTATUS (*_ZwOpenFile)(
    PHANDLE FileHandle,
    ACCESS_MASK DesiredAccess,
    POBJECT_ATTRIBUTES ObjectAttributes,
    PIO_STATUS_BLOCK IoStatusBlock,
    ULONG ShareAccess,
    ULONG OpenOptions
    );

typedef NTSTATUS (*_ZwOpenKey)(
    PHANDLE KeyHandle,
    ACCESS_MASK DesiredAccess,
    POBJECT_ATTRIBUTES ObjectAttributes
    );

typedef NTSTATUS (*_ZwOpenProcess)(
    PHANDLE ProcessHandle,
    ACCESS_MASK DesiredAccess,
    POBJECT_ATTRIBUTES ObjectAttributes,
    PCLIENT_ID ClientId
    );

typedef NTSTATUS (*_ZwOpenThread)(
    PHANDLE ThreadHandle,
    ACCESS_MASK DesiredAccess,
    POBJECT_ATTRIBUTES ObjectAttributes,
    PCLIENT_ID ClientId
    );

typedef NTSTATUS (*_ZwQueryInformationFile)(
    HANDLE FileHandle,
    PIO_STATUS_BLOCK IoStatusBlock,
    PVOID FileInformation,
    ULONG Length,
    FILE_INFORMATION_CLASS FileInformationClass
    );

typedef NTSTATUS (*_ZwQueryInformationProcess)(
    HANDLE ProcessHandle,
    int ProcessInformationClass,
    PVOID ProcessInformation,
    int ProcessInformationLength,
    int *ReturnLength
    );

typedef NTSTATUS (*_ZwQueryInformationThread)(
    HANDLE ThreadHandle,
    int ThreadInformationClass,
    PVOID ThreadInformation,
    int ThreadInformationLength,
    int *ReturnLength
    );

typedef NTSTATUS (*_ZwQueryKey)(
    HANDLE KeyHandle,
    KEY_INFORMATION_CLASS KeyInformationClass,
    PVOID KeyInformation,
    ULONG Length,
    PULONG ResultLength
    );

typedef NTSTATUS (*_ZwQuerySystemInformation)(
    int SystemInformationClass,
    PVOID SystemInformation,
    int SystemInformationLength,
    int *ReturnLength
    );

typedef NTSTATUS (*_ZwQueryValueKey)(
    HANDLE KeyHandle,
    PUNICODE_STRING ValueName,
    KEY_VALUE_INFORMATION_CLASS KeyValueInformationClass,
    PVOID KeyValueInformation,
    ULONG Length,
    PULONG ResultLength
    );

typedef NTSTATUS (*_ZwReadFile)(
    HANDLE FileHandle,
    HANDLE Event,
    PIO_APC_ROUTINE ApcRoutine,
    PVOID ApcContext,
    PIO_STATUS_BLOCK IoStatusBlock,
    PVOID Buffer,
    ULONG Length,
    PLARGE_INTEGER ByteOffset,
    PULONG Key
    );

typedef NTSTATUS (*_ZwSetInformationFile)(
    HANDLE FileHandle,
    PIO_STATUS_BLOCK IoStatusBlock,
    PVOID FileInformation,
    ULONG Length,
    FILE_INFORMATION_CLASS FileInformationClass
    );

typedef NTSTATUS (*_ZwSetInformationProcess)(
    HANDLE ProcessHandle,
    int ProcessInformationClass,
    PVOID ProcessInformation,
    ULONG ProcessInformationLength
    );

typedef NTSTATUS (*_ZwSetInformationThread)(
    HANDLE ThreadHandle,
    THREADINFOCLASS ThreadInformationClass,
    PVOID ThreadInformation,
    ULONG ThreadInformationLength
    );

typedef NTSTATUS (*_ZwSetValueKey)(
    HANDLE KeyHandle,
    PUNICODE_STRING ValueName,
    ULONG TitleIndex,
    ULONG Type,
    PVOID Data,
    ULONG DataSize
    );

typedef NTSTATUS (*_ZwTerminateProcess)(
    HANDLE Process,
    ULONG ExitCode
    );

typedef NTSTATUS (*_ZwTerminateThread)(
    HANDLE Thread,
    ULONG ExitCode
    );

typedef NTSTATUS (*_ZwWriteFile)(
    HANDLE FileHandle,
    HANDLE Event,
    PIO_APC_ROUTINE ApcRoutine,
    PVOID ApcContext,
    PIO_STATUS_BLOCK IoStatusBlock,
    PVOID Buffer,
    ULONG Length,
    PLARGE_INTEGER ByteOffset,
    PULONG Key
    );

#define PROCESS_TERMINATE                  (0x0001)  
#define PROCESS_CREATE_THREAD              (0x0002)  
#define PROCESS_SET_SESSIONID              (0x0004)  
#define PROCESS_VM_OPERATION               (0x0008)  
#define PROCESS_VM_READ                    (0x0010)  
#define PROCESS_VM_WRITE                   (0x0020)  
#define PROCESS_DUP_HANDLE                 (0x0040)  
#define PROCESS_CREATE_PROCESS             (0x0080)  
#define PROCESS_SET_QUOTA                  (0x0100)  
#define PROCESS_SET_INFORMATION            (0x0200)  
#define PROCESS_QUERY_INFORMATION          (0x0400)  
#define PROCESS_SUSPEND_RESUME             (0x0800)  
#define PROCESS_QUERY_LIMITED_INFORMATION  (0x1000)  
#if (NTDDI_VERSION >= NTDDI_LONGHORN)
#define PROCESS_ALL_ACCESS        (STANDARD_RIGHTS_REQUIRED | SYNCHRONIZE | \
                                   0xFFFF)
#else
#define PROCESS_ALL_ACCESS        (STANDARD_RIGHTS_REQUIRED | SYNCHRONIZE | \
                                   0xFFF)
#endif

#endif