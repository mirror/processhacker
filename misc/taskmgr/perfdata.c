/*
*  ReactOS Task Manager
*
*  perfdata.c
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

#include "taskmgr.h"

CRITICAL_SECTION                           PerfDataCriticalSection;
PPERFDATA                                  pPerfDataOld = NULL;    /* Older perf data (saved to establish delta values) */
PPERFDATA                                  pPerfData = NULL;    /* Most recent copy of perf data */
ULONG                                      ProcessCountOld = 0;
ULONG                                      ProcessCount = 0;
double                                     dbIdleTime = 0;
double                                     dbKernelTime = 0;
double                                     dbSystemTime = 0;
LARGE_INTEGER                              liOldIdleTime = {{0,0}};
double                                     OldKernelTime = 0;
LARGE_INTEGER                              liOldSystemTime = {{0,0}};
SYSTEM_PERFORMANCE_INFORMATION             SystemPerfInfo;
SYSTEM_BASIC_INFORMATION                   SystemBasicInfo;
SYSTEM_FILECACHE_INFORMATION               SystemCacheInfo;
SYSTEM_HANDLE_INFORMATION                  SystemHandleInfo;
PSYSTEM_PROCESSOR_PERFORMANCE_INFORMATION  SystemProcessorTimeInfo = NULL;
PSID                                       SystemUserSid = NULL;

typedef struct _SIDTOUSERNAME
{
    LIST_ENTRY List;
    LPWSTR pszName;
    BYTE Data[1];
} SIDTOUSERNAME, *PSIDTOUSERNAME;

static LIST_ENTRY SidToUserNameHead = {&SidToUserNameHead, &SidToUserNameHead};

BOOL PerfDataInitialize(void)
{
    SID_IDENTIFIER_AUTHORITY NtSidAuthority = {SECURITY_NT_AUTHORITY};
    NTSTATUS    status;

    InitializeCriticalSection(&PerfDataCriticalSection);

    /*
    * Get number of processors in the system
    */
    status = NtQuerySystemInformation(SystemBasicInformation, &SystemBasicInfo, sizeof(SystemBasicInfo), NULL);
    if (!NT_SUCCESS(status))
        return FALSE;

    /*
    * Create the SYSTEM Sid
    */
    AllocateAndInitializeSid(&NtSidAuthority, 1, SECURITY_LOCAL_SYSTEM_RID, 0, 0, 0, 0, 0, 0, 0, &SystemUserSid);
    return TRUE;
}

void PerfDataUninitialize(void)
{
    PLIST_ENTRY pCur;
    PSIDTOUSERNAME pEntry;

    if (pPerfData != NULL)
        HeapFree(GetProcessHeap(), 0, pPerfData);

    DeleteCriticalSection(&PerfDataCriticalSection);

    if (SystemUserSid != NULL)
    {
        FreeSid(SystemUserSid);
        SystemUserSid = NULL;
    }

    /* Free user names cache list */
    pCur = SidToUserNameHead.Flink;

    while (pCur != &SidToUserNameHead)
    {
        pEntry = CONTAINING_RECORD(pCur, SIDTOUSERNAME, List);
        pCur = pCur->Flink;

        HeapFree(GetProcessHeap(), 0, pEntry);
    }
}

static void SidToUserName(PSID Sid, LPWSTR szBuffer, DWORD BufferSize)
{
    static WCHAR szDomainNameUnused[255];
    DWORD DomainNameLen = _countof(szDomainNameUnused);
    SID_NAME_USE Use;

    if (Sid != NULL)
        LookupAccountSid(NULL, Sid, szBuffer, &BufferSize, szDomainNameUnused, &DomainNameLen, &Use);
}

VOID WINAPI CachedGetUserFromSid(PSID pSid, LPWSTR pUserName, PULONG pcwcUserName)
{
    PLIST_ENTRY pCur;
    PSIDTOUSERNAME pEntry;
    ULONG cbSid, cwcUserName;

    cwcUserName = *pcwcUserName;

    /* Walk through the list */
    for(pCur = SidToUserNameHead.Flink; pCur != &SidToUserNameHead; pCur = pCur->Flink)
    {
        pEntry = CONTAINING_RECORD(pCur, SIDTOUSERNAME, List);

        if (EqualSid((PSID)&pEntry->Data, pSid))
        {
            wcsncpy_s(pUserName, cwcUserName, pEntry->pszName, cwcUserName);
            *pcwcUserName = cwcUserName;
            return;
        }
    }

    /* We didn't find the SID in the list, get the name conventional */
    SidToUserName(pSid, pUserName, cwcUserName);

    /* Allocate a new entry */
    *pcwcUserName = (ULONG)wcslen(pUserName);
    cwcUserName = *pcwcUserName + 1;
    cbSid = GetLengthSid(pSid);
    pEntry = HeapAlloc(GetProcessHeap(), 0, sizeof(SIDTOUSERNAME) + cbSid + cwcUserName * sizeof(WCHAR));

    /* Copy the Sid and name to our entry */
    CopySid(cbSid, (PSID)&pEntry->Data, pSid);
    pEntry->pszName = (LPWSTR)(pEntry->Data + cbSid);
    wcsncpy_s(pEntry->pszName, cwcUserName, pUserName, cwcUserName);

    /* Insert the new entry */
    pEntry->List.Flink = &SidToUserNameHead;
    pEntry->List.Blink = SidToUserNameHead.Blink;
    SidToUserNameHead.Blink->Flink = &pEntry->List;
    SidToUserNameHead.Blink = &pEntry->List;

    return;
}

void PerfDataRefresh(void)
{
    SIZE_T ulSize;
    SIZE_T BufferSize;
    NTSTATUS status;
    LPBYTE pBuffer;
    PSYSTEM_PROCESS_INFORMATION pSPI;
    PPERFDATA pPDOld;
    ULONG Idx, Idx2;
    HANDLE hProcess, hProcessToken;
    SYSTEM_PERFORMANCE_INFORMATION SysPerfInfo;
    SYSTEM_TIMEOFDAY_INFORMATION SysTimeInfo;
    SYSTEM_FILECACHE_INFORMATION SysCacheInfo;
    LPBYTE SysHandleInfoData;
    PSYSTEM_PROCESSOR_PERFORMANCE_INFORMATION  SysProcessorTimeInfo;
    double CurrentKernelTime;
    PSECURITY_DESCRIPTOR ProcessSD;
    PSID ProcessUser;
    ULONG Buffer[64]; /* must be 4 bytes aligned! */
    ULONG cwcUserName;

    /* Get new system time */
    status = NtQuerySystemInformation(SystemTimeOfDayInformation, &SysTimeInfo, sizeof(SysTimeInfo), NULL);
    if (!NT_SUCCESS(status))
        return;

    /* Get new CPU's idle time */
    status = NtQuerySystemInformation(SystemPerformanceInformation, &SysPerfInfo, sizeof(SysPerfInfo), NULL);
    if (!NT_SUCCESS(status))
        return;

    /* Get system cache information */
    status = NtQuerySystemInformation(SystemFileCacheInformation, &SysCacheInfo, sizeof(SysCacheInfo), NULL);
    if (!NT_SUCCESS(status))
        return;

    /* Get processor time information */
    SysProcessorTimeInfo = (PSYSTEM_PROCESSOR_PERFORMANCE_INFORMATION)HeapAlloc(GetProcessHeap(), 0, sizeof(SYSTEM_PROCESSOR_PERFORMANCE_INFORMATION) * SystemBasicInfo.NumberOfProcessors);
    status = NtQuerySystemInformation(SystemProcessorPerformanceInformation, SysProcessorTimeInfo, sizeof(SYSTEM_PROCESSOR_PERFORMANCE_INFORMATION) * SystemBasicInfo.NumberOfProcessors, &ulSize);

    if (!NT_SUCCESS(status))
    {
        if (SysProcessorTimeInfo != NULL)
            HeapFree(GetProcessHeap(), 0, SysProcessorTimeInfo);
        
        return;
    }

    /* Get handle information
    * We don't know how much data there is so just keep
    * increasing the buffer size until the call succeeds
    */
    BufferSize = 0;
    do
    {
        BufferSize += 0x10000;
        SysHandleInfoData = (LPBYTE)HeapAlloc(GetProcessHeap(), 0, BufferSize);

        status = NtQuerySystemInformation(SystemHandleInformation, SysHandleInfoData, BufferSize, &ulSize);

        if (status == STATUS_INFO_LENGTH_MISMATCH) 
        {
            HeapFree(GetProcessHeap(), 0, SysHandleInfoData);
        }

    } while (status == STATUS_INFO_LENGTH_MISMATCH);

    /* Get process information
    * We don't know how much data there is so just keep
    * increasing the buffer size until the call succeeds
    */
    BufferSize = 0;
    do
    {
        BufferSize += 0x10000;
        pBuffer = (LPBYTE)HeapAlloc(GetProcessHeap(), 0, BufferSize);

        status = NtQuerySystemInformation(SystemProcessInformation, pBuffer, BufferSize, &ulSize);

        if (status == STATUS_INFO_LENGTH_MISMATCH) 
        {
            HeapFree(GetProcessHeap(), 0, pBuffer);
        }

    } while (status == STATUS_INFO_LENGTH_MISMATCH);

    EnterCriticalSection(&PerfDataCriticalSection);

    // Save system performance info.
    memcpy(&SystemPerfInfo, &SysPerfInfo, sizeof(SYSTEM_PERFORMANCE_INFORMATION));

    // Save system cache info.
    memcpy(&SystemCacheInfo, &SysCacheInfo, sizeof(SYSTEM_FILECACHE_INFORMATION));

    // Save system processor time info.
    if (SystemProcessorTimeInfo) 
    {
        HeapFree(GetProcessHeap(), 0, SystemProcessorTimeInfo);
    }
    SystemProcessorTimeInfo = SysProcessorTimeInfo;

    // Save system handle info.
    memcpy(&SystemHandleInfo, SysHandleInfoData, sizeof(SYSTEM_HANDLE_INFORMATION));
    HeapFree(GetProcessHeap(), 0, SysHandleInfoData);

    for (CurrentKernelTime = 0, Idx = 0; Idx < (ULONG)SystemBasicInfo.NumberOfProcessors; Idx++) 
    {
        CurrentKernelTime += Li2Double(SystemProcessorTimeInfo[Idx].KernelTime);
        CurrentKernelTime += Li2Double(SystemProcessorTimeInfo[Idx].DpcTime);
        CurrentKernelTime += Li2Double(SystemProcessorTimeInfo[Idx].InterruptTime);
    }

    /* If it's a first call - skip idle time calcs */
    if (liOldIdleTime.QuadPart != 0) 
    {
        /*  CurrentValue = NewValue - OldValue */
        dbIdleTime = Li2Double(SysPerfInfo.IdleProcessTime) - Li2Double(liOldIdleTime);
        dbKernelTime = CurrentKernelTime - OldKernelTime;
        dbSystemTime = Li2Double(SysTimeInfo.CurrentTime) - Li2Double(liOldSystemTime);

        /*  CurrentCpuIdle = IdleTime / SystemTime */
        dbIdleTime = dbIdleTime / dbSystemTime;
        dbKernelTime = dbKernelTime / dbSystemTime;

        /*  CurrentCpuUsage% = 100 - (CurrentCpuIdle * 100) / NumberOfProcessors */
        dbIdleTime = 100.0 - dbIdleTime * 100.0 / (double)SystemBasicInfo.NumberOfProcessors; /* + 0.5; */
        dbKernelTime = 100.0 - dbKernelTime * 100.0 / (double)SystemBasicInfo.NumberOfProcessors; /* + 0.5; */
    }

    /* Store new CPU's idle and system time */
    liOldIdleTime = SysPerfInfo.IdleProcessTime;
    liOldSystemTime = SysTimeInfo.CurrentTime;
    OldKernelTime = CurrentKernelTime;

    /* Determine the process count
    * We loop through the data we got from NtQuerySystemInformation
    * and count how many structures there are (until RelativeOffset is 0)
    */
    ProcessCountOld = ProcessCount;
    ProcessCount = 0;
    pSPI = (PSYSTEM_PROCESS_INFORMATION)pBuffer;

    while (pSPI) 
    {
        ProcessCount++;

        if (pSPI->NextEntryOffset == 0)
            break;

        pSPI = (PSYSTEM_PROCESS_INFORMATION)((LPBYTE)pSPI + pSPI->NextEntryOffset);
    }

    /* Now alloc a new PERFDATA array and fill in the data */
    if (pPerfDataOld) 
    {
        HeapFree(GetProcessHeap(), 0, pPerfDataOld);
    }

    pPerfDataOld = pPerfData;
    /* Clear out process perf data structures with HEAP_ZERO_MEMORY flag: */
    pPerfData = (PPERFDATA)HeapAlloc(GetProcessHeap(), HEAP_ZERO_MEMORY, sizeof(PERFDATA) * ProcessCount);
    pSPI = (PSYSTEM_PROCESS_INFORMATION)pBuffer;

    for (Idx = 0; Idx < ProcessCount; Idx++) 
    {
        /* Get the old perf data for this process (if any) */
        /* so that we can establish delta values */
        pPDOld = NULL;

        for (Idx2 = 0; Idx2 < ProcessCountOld; Idx2++) 
        {
            if (pPerfDataOld[Idx2].ProcessId == pSPI->UniqueProcessId) 
            {
                pPDOld = &pPerfDataOld[Idx2];
                break;
            }
        }

        if (pSPI->ImageName.Buffer) 
        {
            // Don't assume a UNICODE_STRING Buffer is zero terminated:
            int len = pSPI->ImageName.Length / 2;
            // Check against max size and allow for terminating zero (already zeroed):
            if(len >= MAX_PATH)
                len=MAX_PATH - 1;
            
            wcsncpy_s(pPerfData[Idx].ImageName, _countof(pPerfData[Idx].ImageName), pSPI->ImageName.Buffer, len);
        } 
        else 
        {
            LoadString(hInst, IDS_IDLE_PROCESS, pPerfData[Idx].ImageName, _countof(pPerfData[Idx].ImageName));
        }

        pPerfData[Idx].ProcessId = pSPI->UniqueProcessId;

        if (pPDOld)    
        {
            double CurTime = Li2Double(pSPI->KernelTime) + Li2Double(pSPI->UserTime);
            double OldTime = Li2Double(pPDOld->KernelTime) + Li2Double(pPDOld->UserTime);
            double CpuTime = (CurTime - OldTime) / dbSystemTime;

            CpuTime = CpuTime * 100.0 / (double)SystemBasicInfo.NumberOfProcessors; /* + 0.5; */
            pPerfData[Idx].CPUUsage = (ULONG)CpuTime;
        }

        pPerfData[Idx].CPUTime.QuadPart = pSPI->UserTime.QuadPart + pSPI->KernelTime.QuadPart;
        pPerfData[Idx].WorkingSetSizeBytes = pSPI->WorkingSetSize;
        pPerfData[Idx].PeakWorkingSetSizeBytes = pSPI->PeakWorkingSetSize;

        if (pPDOld)
            pPerfData[Idx].WorkingSetSizeDelta = labs((LONG)pSPI->WorkingSetSize - (LONG)pPDOld->WorkingSetSizeBytes);
        else
            pPerfData[Idx].WorkingSetSizeDelta = 0;

        pPerfData[Idx].PageFaultCount = pSPI->PageFaultCount;

        if (pPDOld)
            pPerfData[Idx].PageFaultCountDelta = labs((LONG)pSPI->PageFaultCount - (LONG)pPDOld->PageFaultCount);
        else
            pPerfData[Idx].PageFaultCountDelta = 0;

        pPerfData[Idx].VirtualMemorySizeBytes = pSPI->VirtualSize;
        pPerfData[Idx].PagedPoolUsagePages = pSPI->QuotaPeakPagedPoolUsage;
        pPerfData[Idx].NonPagedPoolUsagePages = pSPI->QuotaPeakNonPagedPoolUsage;
        pPerfData[Idx].BasePriority = pSPI->BasePriority;
        pPerfData[Idx].HandleCount = pSPI->HandleCount;
        pPerfData[Idx].ThreadCount = pSPI->NumberOfThreads;
        pPerfData[Idx].SessionId = pSPI->SessionId;
        pPerfData[Idx].UserName[0] = L'\0';
        pPerfData[Idx].USERObjectCount = 0;
        pPerfData[Idx].GDIObjectCount = 0;
        ProcessUser = SystemUserSid;
        ProcessSD = NULL;

        if (pSPI->UniqueProcessId != NULL) 
        {
            hProcess = OpenProcess(PROCESS_QUERY_INFORMATION | READ_CONTROL, FALSE, PtrToUlong(pSPI->UniqueProcessId));

            if (hProcess) 
            {
                //TODO: don't query the information of the system process??? 
                // It's possible but returns Administrators as the owner of the process instead of SYSTEM
                //if (pSPI->UniqueProcessId != (HANDLE)0x4)
                {
                    if (OpenProcessToken(hProcess, TOKEN_QUERY, &hProcessToken))
                    {
                        DWORD RetLen = 0;
                        BOOL Ret = GetTokenInformation(hProcessToken, TokenUser, (LPVOID)Buffer, sizeof(Buffer), &RetLen);
                        
                        NtClose(hProcessToken);

                        if (Ret)
                        {
                            ProcessUser = ((PTOKEN_USER)Buffer)->User.Sid;
                        }
                        else
                        {
                            goto ReadProcOwner;
                        }
                    }
                    else
                    {
ReadProcOwner:
                        GetSecurityInfo(hProcess, SE_KERNEL_OBJECT, OWNER_SECURITY_INFORMATION, &ProcessUser, NULL, NULL, NULL, &ProcessSD);
                    }

                    pPerfData[Idx].USERObjectCount = GetGuiResources(hProcess, GR_USEROBJECTS);
                    pPerfData[Idx].GDIObjectCount = GetGuiResources(hProcess, GR_GDIOBJECTS);
                }

                GetProcessIoCounters(hProcess, &pPerfData[Idx].IOCounters);
                NtClose(hProcess);
            } 
            else 
            {
                goto ClearInfo;
            }
        } 
        else 
        {
ClearInfo:
            /* clear information we were unable to fetch */
            ZeroMemory(&pPerfData[Idx].IOCounters, sizeof(IO_COUNTERS));
        }

        cwcUserName = _countof(pPerfData[0].UserName);
        CachedGetUserFromSid(ProcessUser, pPerfData[Idx].UserName, &cwcUserName);

        if (ProcessSD != NULL)
        {
            LocalFree((HLOCAL)ProcessSD);
        }

        pPerfData[Idx].UserTime.QuadPart = pSPI->UserTime.QuadPart;
        pPerfData[Idx].KernelTime.QuadPart = pSPI->KernelTime.QuadPart;
        pSPI = (PSYSTEM_PROCESS_INFORMATION)((LPBYTE)pSPI + pSPI->NextEntryOffset);
    }

    HeapFree(GetProcessHeap(), 0, pBuffer);

    LeaveCriticalSection(&PerfDataCriticalSection);
}

ULONG PerfDataGetProcessIndex(ULONG pid)
{
    ULONG idx;

    EnterCriticalSection(&PerfDataCriticalSection);

    for (idx = 0; idx < ProcessCount; idx++)
    {
        if (PtrToUlong(pPerfData[idx].ProcessId) == pid)
        {
            break;
        }
    }

    LeaveCriticalSection(&PerfDataCriticalSection);

    if (idx == ProcessCount)
    {
        return 0; //TODO -1
    }

    return idx;
}

ULONG PerfDataGetProcessCount(void)
{
    return ProcessCount;
}

ULONG PerfDataGetProcessorUsage(void)
{
    return (ULONG)dbIdleTime;
}

ULONG PerfDataGetProcessorSystemUsage(void)
{
    return (ULONG)dbKernelTime;
}

BOOL PerfDataGetImageName(ULONG Index, LPWSTR lpImageName, int nMaxCount)
{
    BOOL bSuccessful = FALSE;

    EnterCriticalSection(&PerfDataCriticalSection);

    if (Index < ProcessCount) 
    {
		wcsncpy_s(lpImageName, _countof(pPerfData[Index].ImageName), pPerfData[Index].ImageName, nMaxCount);
        bSuccessful = TRUE;
    }
    else 
    {
        bSuccessful = FALSE;
    }

    LeaveCriticalSection(&PerfDataCriticalSection);

    return bSuccessful;
}

ULONG PerfDataGetProcessId(ULONG Index)
{
    ULONG ProcessId = 0;

    EnterCriticalSection(&PerfDataCriticalSection);

    if (Index < ProcessCount)
        ProcessId = PtrToUlong(pPerfData[Index].ProcessId);
    else
        ProcessId = 0;

    LeaveCriticalSection(&PerfDataCriticalSection);

    return ProcessId;
}

BOOL PerfDataGetUserName(ULONG Index, LPWSTR lpUserName, int nMaxCount)
{
    BOOL bSuccessful = FALSE;

    EnterCriticalSection(&PerfDataCriticalSection);

    if (Index < ProcessCount)
    {
		wcsncpy_s(lpUserName, _countof(pPerfData[Index].UserName), pPerfData[Index].UserName, nMaxCount);

        bSuccessful = TRUE;
    } 
    else 
    {
        bSuccessful = FALSE;
    }

    LeaveCriticalSection(&PerfDataCriticalSection);

    return bSuccessful;
}

ULONG PerfDataGetSessionId(ULONG Index)
{
    ULONG SessionId = 0;

    EnterCriticalSection(&PerfDataCriticalSection);

    if (Index < ProcessCount)
        SessionId = pPerfData[Index].SessionId;
    else
        SessionId = 0;

    LeaveCriticalSection(&PerfDataCriticalSection);

    return SessionId;
}

ULONG PerfDataGetCPUUsage(ULONG Index)
{
    ULONG CpuUsage = 0;

    EnterCriticalSection(&PerfDataCriticalSection);

    if (Index < ProcessCount)
        CpuUsage = pPerfData[Index].CPUUsage;
    else
        CpuUsage = 0;

    LeaveCriticalSection(&PerfDataCriticalSection);

    return CpuUsage;
}

LARGE_INTEGER PerfDataGetCPUTime(ULONG Index)
{
    LARGE_INTEGER CpuTime = {{ 0, 0 }};

    EnterCriticalSection(&PerfDataCriticalSection);

    if (Index < ProcessCount)
        CpuTime = pPerfData[Index].CPUTime;

    LeaveCriticalSection(&PerfDataCriticalSection);

    return CpuTime;
}

SIZE_T PerfDataGetWorkingSetSizeBytes(ULONG Index)
{
    SIZE_T WorkingSetSizeBytes = 0;

    EnterCriticalSection(&PerfDataCriticalSection);

    if (Index < ProcessCount)
        WorkingSetSizeBytes = pPerfData[Index].WorkingSetSizeBytes;
    else
        WorkingSetSizeBytes = 0;

    LeaveCriticalSection(&PerfDataCriticalSection);

    return WorkingSetSizeBytes;
}

SIZE_T PerfDataGetPeakWorkingSetSizeBytes(ULONG Index)
{
    SIZE_T PeakWorkingSetSizeBytes = 0;

    EnterCriticalSection(&PerfDataCriticalSection);

    if (Index < ProcessCount)
        PeakWorkingSetSizeBytes = pPerfData[Index].PeakWorkingSetSizeBytes;
    else
        PeakWorkingSetSizeBytes = 0;

    LeaveCriticalSection(&PerfDataCriticalSection);

    return PeakWorkingSetSizeBytes;
}

ULONG PerfDataGetWorkingSetSizeDelta(ULONG Index)
{
    ULONG WorkingSetSizeDelta = 0;

    EnterCriticalSection(&PerfDataCriticalSection);

    if (Index < ProcessCount)
        WorkingSetSizeDelta = pPerfData[Index].WorkingSetSizeDelta;
    else
        WorkingSetSizeDelta = 0;

    LeaveCriticalSection(&PerfDataCriticalSection);

    return WorkingSetSizeDelta;
}

SIZE_T PerfDataGetPageFaultCount(ULONG Index)
{
    SIZE_T PageFaultCount = 0;

    EnterCriticalSection(&PerfDataCriticalSection);

    if (Index < ProcessCount)
        PageFaultCount = pPerfData[Index].PageFaultCount;
    else
        PageFaultCount = 0;

    LeaveCriticalSection(&PerfDataCriticalSection);

    return PageFaultCount;
}

ULONG PerfDataGetPageFaultCountDelta(ULONG Index)
{
    ULONG PageFaultCountDelta = 0;

    EnterCriticalSection(&PerfDataCriticalSection);

    if (Index < ProcessCount)
        PageFaultCountDelta = pPerfData[Index].PageFaultCountDelta;
    else
        PageFaultCountDelta = 0;

    LeaveCriticalSection(&PerfDataCriticalSection);

    return PageFaultCountDelta;
}

ULONG PerfDataGetVirtualMemorySizeBytes(ULONG Index)
{
    ULONG VirtualMemorySizeBytes = 0;

    EnterCriticalSection(&PerfDataCriticalSection);

    if (Index < ProcessCount)
        VirtualMemorySizeBytes = pPerfData[Index].VirtualMemorySizeBytes;
    else
        VirtualMemorySizeBytes = 0;

    LeaveCriticalSection(&PerfDataCriticalSection);

    return VirtualMemorySizeBytes;
}

SIZE_T PerfDataGetPagedPoolUsagePages(ULONG Index)
{
    SIZE_T PagedPoolUsage = 0;

    EnterCriticalSection(&PerfDataCriticalSection);

    if (Index < ProcessCount)
        PagedPoolUsage = pPerfData[Index].PagedPoolUsagePages;
    else
        PagedPoolUsage = 0;

    LeaveCriticalSection(&PerfDataCriticalSection);

    return PagedPoolUsage;
}

SIZE_T PerfDataGetNonPagedPoolUsagePages(ULONG Index)
{
    SIZE_T NonPagedPoolUsage = 0;

    EnterCriticalSection(&PerfDataCriticalSection);

    if (Index < ProcessCount)
        NonPagedPoolUsage = pPerfData[Index].NonPagedPoolUsagePages;
    else
        NonPagedPoolUsage = 0;

    LeaveCriticalSection(&PerfDataCriticalSection);

    return NonPagedPoolUsage;
}

ULONG PerfDataGetBasePriority(ULONG Index)
{
    ULONG BasePriority = 0;

    EnterCriticalSection(&PerfDataCriticalSection);

    if (Index < ProcessCount)
        BasePriority = pPerfData[Index].BasePriority;
    else
        BasePriority = 0;

    LeaveCriticalSection(&PerfDataCriticalSection);

    return BasePriority;
}

ULONG PerfDataGetHandleCount(ULONG Index)
{
    ULONG HandleCount = 0;

    EnterCriticalSection(&PerfDataCriticalSection);

    if (Index < ProcessCount)
        HandleCount = pPerfData[Index].HandleCount;
    else
        HandleCount = 0;

    LeaveCriticalSection(&PerfDataCriticalSection);

    return HandleCount;
}

ULONG PerfDataGetThreadCount(ULONG Index)
{
    ULONG ThreadCount = 0;

    EnterCriticalSection(&PerfDataCriticalSection);

    if (Index < ProcessCount)
        ThreadCount = pPerfData[Index].ThreadCount;
    else
        ThreadCount = 0;

    LeaveCriticalSection(&PerfDataCriticalSection);

    return ThreadCount;
}

ULONG PerfDataGetUSERObjectCount(ULONG Index)
{
    ULONG USERObjectCount = 0;

    EnterCriticalSection(&PerfDataCriticalSection);

    if (Index < ProcessCount)
        USERObjectCount = pPerfData[Index].USERObjectCount;
    else
        USERObjectCount = 0;

    LeaveCriticalSection(&PerfDataCriticalSection);

    return USERObjectCount;
}

ULONG PerfDataGetGDIObjectCount(ULONG Index)
{
    ULONG GDIObjectCount = 0;

    EnterCriticalSection(&PerfDataCriticalSection);

    if (Index < ProcessCount)
        GDIObjectCount = pPerfData[Index].GDIObjectCount;
    else
        GDIObjectCount = 0;

    LeaveCriticalSection(&PerfDataCriticalSection);

    return GDIObjectCount;
}

BOOL PerfDataGetIOCounters(ULONG Index, PIO_COUNTERS pIoCounters)
{
    BOOL bSuccessful = FALSE;

    EnterCriticalSection(&PerfDataCriticalSection);

    if (Index < ProcessCount)
    {
        memcpy(pIoCounters, &pPerfData[Index].IOCounters, sizeof(IO_COUNTERS));
        bSuccessful = TRUE;
    }
    else
        bSuccessful = FALSE;

    LeaveCriticalSection(&PerfDataCriticalSection);

    return bSuccessful;
}

ULONG PerfDataGetCommitChargeTotalK(void)
{
    ULONG Total = 0;

    EnterCriticalSection(&PerfDataCriticalSection);

    Total = SystemPerfInfo.CommittedPages;

    LeaveCriticalSection(&PerfDataCriticalSection);

    Total = Total * (SystemBasicInfo.PageSize / 1024);

    return Total;
}

ULONG PerfDataGetCommitChargeLimitK(void)
{
    ULONG Limit = 0;

    EnterCriticalSection(&PerfDataCriticalSection);

    Limit = SystemPerfInfo.CommitLimit;

    LeaveCriticalSection(&PerfDataCriticalSection);

    Limit = Limit * (SystemBasicInfo.PageSize / 1024);

    return Limit;
}

ULONG PerfDataGetCommitChargePeakK(void)
{
    ULONG  Peak = 0;

    EnterCriticalSection(&PerfDataCriticalSection);

    Peak = SystemPerfInfo.PeakCommitment;

    LeaveCriticalSection(&PerfDataCriticalSection);

    Peak = Peak * (SystemBasicInfo.PageSize / 1024);

    return Peak;
}

ULONG PerfDataGetKernelMemoryTotalK(void)
{
    ULONG Total = 0, Paged = 0, NonPaged = 0;

    EnterCriticalSection(&PerfDataCriticalSection);

    Paged = SystemPerfInfo.PagedPoolPages;
    NonPaged = SystemPerfInfo.NonPagedPoolPages;

    LeaveCriticalSection(&PerfDataCriticalSection);

    Paged = Paged * (SystemBasicInfo.PageSize / 1024);
    NonPaged = NonPaged * (SystemBasicInfo.PageSize / 1024);

    Total = Paged + NonPaged;

    return Total;
}

ULONG PerfDataGetKernelMemoryPagedK(void)
{
    ULONG Paged = 0;

    EnterCriticalSection(&PerfDataCriticalSection);

    Paged = SystemPerfInfo.PagedPoolPages;

    LeaveCriticalSection(&PerfDataCriticalSection);

    Paged = Paged * (SystemBasicInfo.PageSize / 1024);

    return Paged;
}

ULONG PerfDataGetKernelMemoryNonPagedK(void)
{
    ULONG nonPaged = 0;

    EnterCriticalSection(&PerfDataCriticalSection);

    nonPaged = SystemPerfInfo.NonPagedPoolPages;

    LeaveCriticalSection(&PerfDataCriticalSection);

    nonPaged = nonPaged * (SystemBasicInfo.PageSize / 1024);

    return nonPaged;
}

ULONG PerfDataGetPhysicalMemoryTotalK(void)
{
    ULONG Total = 0;

    EnterCriticalSection(&PerfDataCriticalSection);

    Total = SystemBasicInfo.NumberOfPhysicalPages;

    LeaveCriticalSection(&PerfDataCriticalSection);

    Total = Total * (SystemBasicInfo.PageSize / 1024);

    return Total;
}

ULONG PerfDataGetPhysicalMemoryAvailableK(void)
{
    ULONG Available = 0;

    EnterCriticalSection(&PerfDataCriticalSection);

    Available = SystemPerfInfo.AvailablePages;

    LeaveCriticalSection(&PerfDataCriticalSection);

    Available = Available * (SystemBasicInfo.PageSize / 1024);

    return Available;
}

SIZE_T PerfDataGetPhysicalMemorySystemCacheK(void)
{
    SIZE_T SystemCache = 0;

    EnterCriticalSection(&PerfDataCriticalSection);

    SystemCache = SystemCacheInfo.CurrentSizeIncludingTransitionInPages * SystemBasicInfo.PageSize;

    LeaveCriticalSection(&PerfDataCriticalSection);

    return SystemCache / 1024;
}

ULONG PerfDataGetSystemHandleCount(void)
{
    ULONG HandleCount = 0;

    EnterCriticalSection(&PerfDataCriticalSection);

    HandleCount = SystemHandleInfo.NumberOfHandles;

    LeaveCriticalSection(&PerfDataCriticalSection);

    return HandleCount;
}

ULONG PerfDataGetTotalThreadCount(void)
{
    ULONG ThreadCount = 0, i = 0;

    EnterCriticalSection(&PerfDataCriticalSection);

    for (i = 0; i < ProcessCount; i++)
    {
        ThreadCount += pPerfData[i].ThreadCount;
    }

    LeaveCriticalSection(&PerfDataCriticalSection);

    return ThreadCount;
}

BOOL PerfDataGet(ULONG Index, PPERFDATA *lppData)
{
    BOOL bSuccessful = FALSE;

    EnterCriticalSection(&PerfDataCriticalSection);

    if (Index < ProcessCount)
    {
        *lppData = pPerfData + Index;
        bSuccessful = TRUE;
    }

    LeaveCriticalSection(&PerfDataCriticalSection);

    return bSuccessful;
}

