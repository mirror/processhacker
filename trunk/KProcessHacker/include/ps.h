/*
 * Process Hacker Driver - 
 *   processes and threads
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

#ifndef _PS_H
#define _PS_H

#include "types.h"
#include "ex.h"
#include "mm.h"
#include "ob.h"
#include "se.h"

#define PROCESS_TERMINATE (0x0001)
#define PROCESS_CREATE_THREAD (0x0002)
#define PROCESS_SET_SESSIONID (0x0004)
#define PROCESS_VM_OPERATION (0x0008)
#define PROCESS_VM_READ (0x0010)
#define PROCESS_VM_WRITE (0x0020)
#define PROCESS_DUP_HANDLE (0x0040)
#define PROCESS_CREATE_PROCESS (0x0080)
#define PROCESS_SET_QUOTA (0x0100)
#define PROCESS_SET_INFORMATION (0x0200)
#define PROCESS_QUERY_INFORMATION (0x0400)
#define PROCESS_SUSPEND_RESUME (0x0800)
#define PROCESS_QUERY_LIMITED_INFORMATION (0x1000)
#ifndef PROCESS_ALL_ACCESS
#define PROCESS_ALL_ACCESS (STANDARD_RIGHTS_REQUIRED | SYNCHRONIZE | 0xffff)
#endif

#define THREAD_TERMINATE (0x0001)
#define THREAD_SUSPEND_RESUME (0x0002)
#define THREAD_ALERT (0x0004)
#define THREAD_GET_CONTEXT (0x0008)
#define THREAD_SET_CONTEXT (0x0010)
#define THREAD_SET_INFORMATION (0x0020)
#define THREAD_QUERY_INFORMATION (0x0040)
#define THREAD_SET_THREAD_TOKEN (0x0080)
#define THREAD_IMPERSONATE (0x0100)
#define THREAD_DIRECT_IMPERSONATION (0x0200)
#ifndef THREAD_ALL_ACCESS
#define THREAD_ALL_ACCESS (STANDARD_RIGHTS_REQUIRED | SYNCHRONIZE | 0x3ff)
#endif

#define JOB_OBJECT_ASSIGN_PROCESS (0x0001)
#define JOB_OBJECT_SET_ATTRIBUTES (0x0002)
#define JOB_OBJECT_QUERY (0x0004)
#define JOB_OBJECT_TERMINATE (0x0008)
#define JOB_OBJECT_SET_SECURITY_ATTRIBUTES (0x0010)
#define JOB_OBJECT_ALL_ACCESS (STANDARD_RIGHTS_REQUIRED | SYNCHRONIZE | 0x1f)

extern POBJECT_TYPE *PsJobType;

/* FUNCTION DEFS */

NTSTATUS NTAPI PsGetContextThread(
    PETHREAD Thread,
    PCONTEXT ThreadContext,
    KPROCESSOR_MODE PreviousMode
    );

PVOID NTAPI PsGetThreadWin32Thread(
    PETHREAD Thread
    );

NTSTATUS NTAPI PsLookupProcessThreadByCid(
    PCLIENT_ID ClientId,
    PEPROCESS *Process,
    PETHREAD *Thread
    );

NTSTATUS NTAPI PsSetContextThread(
    PETHREAD Thread,
    PCONTEXT ThreadContext,
    KPROCESSOR_MODE PreviousMode
    );

/* FUNCTION TYPEDEFS */

typedef PVOID (NTAPI *_PsGetProcessJob)(
    PEPROCESS Process
    );

typedef NTSTATUS (NTAPI *_PsResumeProcess)(
    PEPROCESS Process
    );

typedef NTSTATUS (NTAPI *_PsSuspendProcess)(
    PEPROCESS Process
    );

typedef NTSTATUS (NTAPI *_PsTerminateProcess)(
    PEPROCESS Process,
    NTSTATUS ExitStatus
    );

typedef NTSTATUS (NTAPI *_PspTerminateThreadByPointer51)(
    PETHREAD Thread,
    NTSTATUS ExitStatus
    );

typedef NTSTATUS (NTAPI *_PspTerminateThreadByPointer60)(
    PETHREAD Thread,
    NTSTATUS ExitStatus,
    BOOLEAN DirectTerminate
    );

#endif