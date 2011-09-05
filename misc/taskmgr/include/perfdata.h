/*
 *  ReactOS Task Manager
 *
 *  perfdata.h
 *
 *  Copyright (C) 1999 - 2001  Brian Palmer  <brianp@reactos.org>
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
 */

#pragma once

#define Li2Double(x) ((double)((x).HighPart) * 4.294967296E9 + (double)((x).LowPart))

typedef struct _PERFDATA
{
	WCHAR				ImageName[MAX_PATH];
	HANDLE				ProcessId;
	WCHAR				UserName[MAX_PATH];
	ULONG				SessionId;
	ULONG				CPUUsage;
	LARGE_INTEGER		CPUTime;
	SIZE_T				WorkingSetSizeBytes;
	SIZE_T				PeakWorkingSetSizeBytes;
	ULONG				WorkingSetSizeDelta;
	SIZE_T				PageFaultCount;
	ULONG				PageFaultCountDelta;
	ULONG				VirtualMemorySizeBytes;
	SIZE_T				PagedPoolUsagePages;
	SIZE_T				NonPagedPoolUsagePages;
	ULONG				BasePriority;
	ULONG				HandleCount;
	ULONG				ThreadCount;
	ULONG				USERObjectCount;
	ULONG				GDIObjectCount;
	IO_COUNTERS			IOCounters;

	LARGE_INTEGER		UserTime;
	LARGE_INTEGER		KernelTime;
} PERFDATA, *PPERFDATA;

LARGE_INTEGER PerfDataGetCPUTime(ULONG Index);

void PerfDataUninitialize(void);
void PerfDataRefresh(void);

BOOL PerfDataInitialize(void);
BOOL PerfDataGet(ULONG Index, PPERFDATA *lppData);
BOOL PerfDataGetImageName(ULONG Index, LPTSTR lpImageName, int nMaxCount);
BOOL PerfDataGetUserName(ULONG Index, LPTSTR lpUserName, int nMaxCount);
BOOL PerfDataGetIOCounters(ULONG Index, PIO_COUNTERS pIoCounters);

ULONG PerfDataGetProcessIndex(ULONG pid);
ULONG PerfDataGetProcessCount(void);
ULONG PerfDataGetProcessorUsage(void);
ULONG PerfDataGetProcessorSystemUsage(void);
ULONG PerfDataGetProcessId(ULONG Index);
ULONG PerfDataGetSessionId(ULONG Index);
ULONG PerfDataGetCPUUsage(ULONG Index);
SIZE_T PerfDataGetWorkingSetSizeBytes(ULONG Index);
SIZE_T PerfDataGetPeakWorkingSetSizeBytes(ULONG Index);
ULONG PerfDataGetWorkingSetSizeDelta(ULONG Index);
SIZE_T PerfDataGetPageFaultCount(ULONG Index);
ULONG PerfDataGetPageFaultCountDelta(ULONG Index);
ULONG PerfDataGetVirtualMemorySizeBytes(ULONG Index);
SIZE_T PerfDataGetPagedPoolUsagePages(ULONG Index);
SIZE_T PerfDataGetNonPagedPoolUsagePages(ULONG Index);
ULONG PerfDataGetBasePriority(ULONG Index);
ULONG PerfDataGetHandleCount(ULONG Index);
ULONG PerfDataGetThreadCount(ULONG Index);
ULONG PerfDataGetUSERObjectCount(ULONG Index);
ULONG PerfDataGetGDIObjectCount(ULONG Index);
ULONG PerfDataGetCommitChargeTotalK(void);
ULONG PerfDataGetCommitChargeLimitK(void);
ULONG PerfDataGetCommitChargePeakK(void);
ULONG PerfDataGetKernelMemoryTotalK(void);
ULONG PerfDataGetKernelMemoryPagedK(void);
ULONG PerfDataGetKernelMemoryNonPagedK(void);
ULONG PerfDataGetPhysicalMemoryTotalK(void);
ULONG PerfDataGetPhysicalMemoryAvailableK(void);
ULONG PerfDataGetPhysicalMemorySystemCacheK(void);
ULONG PerfDataGetSystemHandleCount(void);
ULONG PerfDataGetTotalThreadCount(void);