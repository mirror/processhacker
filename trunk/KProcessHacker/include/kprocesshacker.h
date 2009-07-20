/*
 * Process Hacker Driver - 
 *   main header file
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

#ifndef KPROCESSHACKER_H
#define KPROCESSHACKER_H

#include <ntifs.h>

#define BOOL ULONG

/* KPH Configuration */
//#define KPH_REQUIRE_DEBUG_PRIVILEGE

#define TAG_KPH ('gThP')
#define TAG_CLIENT_ENTRY ('lChP')

/* I like 0x9999. */
#define KPH_DEVICE_TYPE (0x9999)
#define KPH_DEVICE_NAME (L"\\Device\\KProcessHacker")
#define KPH_DEVICE_DOS_NAME (L"\\DosDevices\\KProcessHacker")

/* Features */
#define KPHF_PSTERMINATEPROCESS 0x1
#define KPHF_PSPTERMINATETHREADBPYPOINTER 0x2

#define KPH_CTL_CODE(x) CTL_CODE(KPH_DEVICE_TYPE, 0x800 + x, METHOD_BUFFERED, FILE_ANY_ACCESS)
#define KPH_RESERVED_1 KPH_CTL_CODE(0)
#define KPH_RESERVED_2 KPH_CTL_CODE(1)
#define KPH_GETFILEOBJECTNAME KPH_CTL_CODE(2)
#define KPH_OPENPROCESS KPH_CTL_CODE(3)
#define KPH_OPENTHREAD KPH_CTL_CODE(4)
#define KPH_OPENPROCESSTOKEN KPH_CTL_CODE(5)
#define KPH_GETPROCESSPROTECTED KPH_CTL_CODE(6)
#define KPH_SETPROCESSPROTECTED KPH_CTL_CODE(7)
#define KPH_TERMINATEPROCESS KPH_CTL_CODE(8)
#define KPH_SUSPENDPROCESS KPH_CTL_CODE(9)
#define KPH_RESUMEPROCESS KPH_CTL_CODE(10)
#define KPH_READVIRTUALMEMORY KPH_CTL_CODE(11)
#define KPH_WRITEVIRTUALMEMORY KPH_CTL_CODE(12)
#define KPH_SETPROCESSTOKEN KPH_CTL_CODE(13)
#define KPH_GETTHREADSTARTADDRESS KPH_CTL_CODE(14)
#define KPH_SETHANDLEATTRIBUTES KPH_CTL_CODE(15)
#define KPH_GETHANDLEOBJECTNAME KPH_CTL_CODE(16)
#define KPH_OPENPROCESSJOB KPH_CTL_CODE(17)
#define KPH_GETCONTEXTTHREAD KPH_CTL_CODE(18)
#define KPH_SETCONTEXTTHREAD KPH_CTL_CODE(19)
#define KPH_GETTHREADWIN32THREAD KPH_CTL_CODE(20)
#define KPH_DUPLICATEOBJECT KPH_CTL_CODE(21)
#define KPH_ZWQUERYOBJECT KPH_CTL_CODE(22)
#define KPH_GETPROCESSID KPH_CTL_CODE(23)
#define KPH_GETTHREADID KPH_CTL_CODE(24)
#define KPH_TERMINATETHREAD KPH_CTL_CODE(25)
#define KPH_GETFEATURES KPH_CTL_CODE(26)
#define KPH_SETHANDLEGRANTEDACCESS KPH_CTL_CODE(27)
#define KPH_ASSIGNIMPERSONATIONTOKEN KPH_CTL_CODE(28)
#define KPH_PROTECTADD KPH_CTL_CODE(29)
#define KPH_PROTECTREMOVE KPH_CTL_CODE(30)
#define KPH_PROTECTQUERY KPH_CTL_CODE(31)
#define KPH_UNSAFEREADVIRTUALMEMORY KPH_CTL_CODE(32)
#define KPH_SETEXECUTEOPTIONS KPH_CTL_CODE(33)
#define KPH_QUERYPROCESSHANDLES KPH_CTL_CODE(34)
#define KPH_OPENTHREADPROCESS KPH_CTL_CODE(35)
#define KPH_CAPTURESTACKBACKTRACETHREAD KPH_CTL_CODE(36)
#define KPH_DANGEROUSTERMINATETHREAD KPH_CTL_CODE(37)
#define KPH_OPENDEVICE KPH_CTL_CODE(38)
#define KPH_OPENDRIVER KPH_CTL_CODE(39)
#define KPH_QUERYINFORMATIONDRIVER KPH_CTL_CODE(40)
#define KPH_OPENDIRECTORY KPH_CTL_CODE(41)

#define GET_BIT(integer, bit) (((integer) >> (bit)) & 0x1)
#define SET_BIT(integer, bit) ((integer) |= 1 << (bit))
#define CLEAR_BIT(integer, bit) ((integer) &= ~(1 << (bit)))

#define KPH_TIMEOUT_TO_SEC ((LONGLONG) 1 * 10 * 1000 * 1000)
#define KPH_REL_TIMEOUT_IN_SEC(Time) (Time * -1 * KPH_TIMEOUT_TO_SEC)

NTSTATUS DriverEntry(PDRIVER_OBJECT DriverObject, PUNICODE_STRING RegistryPath);
VOID DriverUnload(PDRIVER_OBJECT DriverObject);
NTSTATUS KphDispatchCreate(PDEVICE_OBJECT DeviceObject, PIRP Irp);
NTSTATUS KphDispatchClose(PDEVICE_OBJECT DeviceObject, PIRP Irp);
NTSTATUS KphDispatchDeviceControl(PDEVICE_OBJECT DeviceObject, PIRP Irp);
NTSTATUS KphDispatchRead(PDEVICE_OBJECT DeviceObject, PIRP Irp);
NTSTATUS KphUnsupported(PDEVICE_OBJECT DeviceObject, PIRP Irp);

/* Clients */

#include "include/handle.h"

typedef struct _KPH_CLIENT_ENTRY
{
    LIST_ENTRY ListEntry;
    HANDLE ProcessId;
} KPH_CLIENT_ENTRY, *PKPH_CLIENT_ENTRY;

BOOLEAN AddClientEntry(
    __in HANDLE ProcessId
    );

BOOLEAN FindClientEntry(
    __in HANDLE ProcessId,
    __out_opt PKPH_CLIENT_ENTRY ClientEntryCopy
    );

BOOLEAN RemoveClientEntry(
    __in HANDLE ProcessId
    );

#endif