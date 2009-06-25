/*
 * Process Hacker Driver - 
 *   custom APIs
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

#ifndef _KPH_H
#define _KPH_H

#include "kprocesshacker.h"
#include "debug.h"
#include "version.h"

#include "mm.h"
#include "ps.h"
#include "rtl.h"
#include "zw.h"

#ifdef EXT
#undef EXT
#endif

#ifdef _KPH_PRIVATE
#define EXT
#define EQNULL = NULL
#else
#define EXT extern
#define EQNULL
#endif

EXT _NtClose __NtClose EQNULL;
EXT _PsGetProcessJob PsGetProcessJob EQNULL;
EXT _PsResumeProcess PsResumeProcess EQNULL;
EXT _PsSuspendProcess PsSuspendProcess EQNULL;
EXT _PsTerminateProcess __PsTerminateProcess EQNULL;
EXT PVOID __PspTerminateThreadByPointer EQNULL;

typedef enum _KPH_CAPTURE_AND_ADD_STACK_TYPE
{
    KphCaptureAndAddKModeStack,
    KphCaptureAndAddUModeStack,
    KphCaptureAndAddBothStacks,
    KphCaptureAndAddMaximum
} KPH_CAPTURE_AND_ADD_STACK_TYPE, *PKPH_CAPTURE_AND_ADD_STACK_TYPE;

typedef struct _KPH_ATTACH_STATE
{
    BOOLEAN Attached;
    KAPC_STATE ApcState;
} KPH_ATTACH_STATE, *PKPH_ATTACH_STATE;

typedef struct _MAPPED_MDL
{
    PMDL Mdl;
    PVOID Address;
} MAPPED_MDL, *PMAPPED_MDL;

/* Support routines */

NTSTATUS KphNtInit();

PVOID GetSystemRoutineAddress(
    WCHAR *Name
    );

VOID KphAttachProcess(
    __in PEPROCESS Process,
    __out PKPH_ATTACH_STATE AttachState
    );

NTSTATUS KphAttachProcessHandle(
    __in HANDLE ProcessHandle,
    __out PKPH_ATTACH_STATE AttachState
    );

NTSTATUS KphAttachProcessId(
    __in HANDLE ProcessId,
    __out PKPH_ATTACH_STATE AttachState
    );

VOID KphDetachProcess(
    __in PKPH_ATTACH_STATE AttachState
    );

NTSTATUS OpenProcess(
    __out PHANDLE ProcessHandle,
    __in ACCESS_MASK DesiredAccess,
    __in HANDLE ProcessId
    );

NTSTATUS SetProcessToken(
    __in HANDLE sourcePid,
    __in HANDLE targetPid
    );

/* KProcessHacker */

BOOLEAN KphAcquireProcessRundownProtection(
    __in PEPROCESS Process
    );

NTSTATUS KphAssignImpersonationToken(
    __in HANDLE ThreadHandle,
    __in HANDLE TokenHandle
    );

BOOLEAN KphCaptureAndAddStack(
    __in PRTL_TRACE_DATABASE Database,
    __in KPH_CAPTURE_AND_ADD_STACK_TYPE Type,
    __out_opt PRTL_TRACE_BLOCK *TraceBlock
    );

ULONG KphCaptureStackBackTrace(
    __in ULONG FramesToSkip,
    __in ULONG FramesToCapture,
    __in_opt ULONG Flags,
    __out_ecount(FramesToCapture) PVOID *BackTrace,
    __out_opt PULONG BackTraceHash
    );

NTSTATUS KphCaptureStackBackTraceThread(
    __in HANDLE ThreadHandle,
    __in ULONG FramesToSkip,
    __in ULONG FramesToCapture,
    __out_ecount(FramesToCapture) PVOID *BackTrace,
    __out_opt PULONG CapturedFrames,
    __out_opt PULONG BackTraceHash,
    __in KPROCESSOR_MODE AccessMode
    );

NTSTATUS KphDangerousTerminateThread(
    __in HANDLE ThreadHandle,
    __in NTSTATUS ExitStatus
    );

NTSTATUS KphDuplicateObject(
    __in HANDLE SourceProcessHandle,
    __in HANDLE SourceHandle,
    __in_opt HANDLE TargetProcessHandle,
    __out_opt PHANDLE TargetHandle,
    __in ACCESS_MASK DesiredAccess,
    __in ULONG HandleAttributes,
    __in ULONG Options,
    __in KPROCESSOR_MODE AccessMode
    );

BOOLEAN KphEnumProcessHandleTable(
    __in PEPROCESS Process,
    __in PEX_ENUM_HANDLE_CALLBACK EnumHandleProcedure,
    __inout PVOID Context,
    __out_opt PHANDLE Handle
    );

NTSTATUS KphGetContextThread(
    __in HANDLE ThreadHandle,
    __inout PCONTEXT ThreadContext,
    __in KPROCESSOR_MODE AccessMode
    );

HANDLE KphGetProcessId(
    __in HANDLE ProcessHandle
    );

HANDLE KphGetThreadId(
    __in HANDLE ThreadHandle,
    __out_opt PHANDLE ProcessId
    );

NTSTATUS KphGetThreadWin32Thread(
    __in HANDLE ThreadHandle,
    __out PVOID *Win32Thread,
    __in KPROCESSOR_MODE AccessMode
    );

NTSTATUS KphOpenProcess(
    __out PHANDLE ProcessHandle,
    __in ACCESS_MASK DesiredAccess,
    __in POBJECT_ATTRIBUTES ObjectAttributes,
    __in_opt PCLIENT_ID ClientId,
    __in KPROCESSOR_MODE AccessMode
    );

NTSTATUS KphOpenProcessJob(
    __in HANDLE ProcessHandle,
    __in ACCESS_MASK DesiredAccess,
    __out PHANDLE JobHandle,
    __in KPROCESSOR_MODE AccessMode
    );

NTSTATUS KphOpenProcessTokenEx(
    __in HANDLE ProcessHandle,
    __in ACCESS_MASK DesiredAccess,
    __in ULONG ObjectAttributes,
    __out PHANDLE TokenHandle,
    __in KPROCESSOR_MODE AccessMode
    );

NTSTATUS KphOpenThread(
    __out PHANDLE ThreadHandle,
    __in ACCESS_MASK DesiredAccess,
    __in POBJECT_ATTRIBUTES ObjectAttributes,
    __in_opt PCLIENT_ID ClientId,
    __in KPROCESSOR_MODE AccessMode
    );

NTSTATUS KphOpenThreadProcess(
    __in HANDLE ThreadHandle,
    __in ACCESS_MASK DesiredAccess,
    __out PHANDLE ProcessHandle,
    __in KPROCESSOR_MODE AccessMode
    );

NTSTATUS KphQueryProcessHandles(
    __in HANDLE ProcessHandle,
    __out_bcount_opt(BufferLength) PPROCESS_HANDLE_INFORMATION Buffer,
    __in_opt ULONG BufferLength,
    __out_opt PULONG ReturnLength,
    __in KPROCESSOR_MODE AccessMode
    );

NTSTATUS KphReadVirtualMemory(
    __in HANDLE ProcessHandle,
    __in PVOID BaseAddress,
    __out_bcount(BufferLength) PVOID Buffer,
    __in ULONG BufferLength,
    __out_opt PULONG ReturnLength,
    __in KPROCESSOR_MODE AccessMode
    );

VOID KphReleaseProcessRundownProtection(
    __in PEPROCESS Process
    );

NTSTATUS KphResumeProcess(
    __in HANDLE ProcessHandle
    );

NTSTATUS KphSetContextThread(
    __in HANDLE ThreadHandle,
    __in PCONTEXT ThreadContext,
    __in KPROCESSOR_MODE AccessMode
    );

NTSTATUS KphSetHandleGrantedAccess(
    __in PEPROCESS Process,
    __in HANDLE Handle,
    __in ACCESS_MASK GrantedAccess
    );

NTSTATUS KphSuspendProcess(
    __in HANDLE ProcessHandle
    );

NTSTATUS KphTerminateProcess(
    __in HANDLE ProcessHandle,
    __in NTSTATUS ExitStatus
    );

NTSTATUS KphTerminateThread(
    __in HANDLE ThreadHandle,
    __in NTSTATUS ExitStatus
    );

NTSTATUS KphUnsafeReadVirtualMemory(
    __in HANDLE ProcessHandle,
    __in PVOID BaseAddress,
    __out_bcount(BufferLength) PVOID Buffer,
    __in ULONG BufferLength,
    __out_opt PULONG ReturnLength,
    __in KPROCESSOR_MODE AccessMode
    );

NTSTATUS KphWriteVirtualMemory(
    __in HANDLE ProcessHandle,
    __in PVOID BaseAddress,
    __in_bcount(BufferLength) PVOID Buffer,
    __in ULONG BufferLength,
    __out_opt PULONG ReturnLength,
    __in KPROCESSOR_MODE AccessMode
    );

/* MM */

NTSTATUS MiDoMappedCopy(
    __in PEPROCESS FromProcess,
    __in PVOID FromAddress,
    __in PEPROCESS ToProcess,
    __in PVOID ToAddress,
    __in ULONG BufferLength,
    __in KPROCESSOR_MODE AccessMode,
    __out PULONG ReturnLength
    );

NTSTATUS MiDoPoolCopy(
    __in PEPROCESS FromProcess,
    __in PVOID FromAddress,
    __in PEPROCESS ToProcess,
    __in PVOID ToAddress,
    __in ULONG BufferLength,
    __in KPROCESSOR_MODE AccessMode,
    __out PULONG ReturnLength
    );

ULONG MiGetExceptionInfo(
    __in PEXCEPTION_POINTERS ExceptionInfo,
    __out PBOOLEAN HaveBadAddress,
    __out PULONG_PTR BadAddress
    );

NTSTATUS MmCopyVirtualMemory(
    __in PEPROCESS FromProcess,
    __in PVOID FromAddress,
    __in PEPROCESS ToProcess,
    __in PVOID ToAddress,
    __in ULONG BufferLength,
    __in KPROCESSOR_MODE AccessMode,
    __out PULONG ReturnLength
    );

/* KProcessHacker private */

NTSTATUS KphpCaptureStackBackTraceThread(
    __in PETHREAD Thread,
    __in ULONG FramesToSkip,
    __in ULONG FramesToCapture,
    __out_ecount(FramesToCapture) PVOID *BackTrace,
    __out_opt PULONG CapturedFrames,
    __out_opt PULONG BackTraceHash,
    __in KPROCESSOR_MODE AccessMode
    );

NTSTATUS KphpCreateMappedMdl(
    __in PVOID Address,
    __in ULONG Length,
    __out PMAPPED_MDL MappedMdl
    );

VOID KphpFreeMappedMdl(
    __in PMAPPED_MDL MappedMdl
    );

/* OB */

NTSTATUS ObDuplicateObject(
    __in PEPROCESS SourceProcess,
    __in_opt PEPROCESS TargetProcess,
    __in HANDLE SourceHandle,
    __out_opt PHANDLE TargetHandle,
    __in ACCESS_MASK DesiredAccess,
    __in ULONG HandleAttributes,
    __in ULONG Options,
    __in KPROCESSOR_MODE AccessMode
    );

PHANDLE_TABLE ObReferenceProcessHandleTable(
    __in PEPROCESS Process
    );

VOID ObDereferenceProcessHandleTable(
    __in PEPROCESS Process
    );

/* PS */

NTSTATUS PsTerminateProcess(
    __in PEPROCESS Process,
    __in NTSTATUS ExitStatus
    );

NTSTATUS PspTerminateThreadByPointer(
    __in PETHREAD Thread,
    __in NTSTATUS ExitStatus
    );

#endif