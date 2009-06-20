/*
 * Process Hacker Library
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

#include "process.h"

NTSTATUS PhpQueryProcessWs(
    HANDLE ProcessHandle,
    WS_INFORMATION_CLASS WsInformationClass,
    PVOID WsInformation,
    ULONG WsInformationLength,
    PULONG ReturnLength
    )
{
    switch (WsInformationClass)
    {
    case WsCount:
    case WsPrivateCount:
    case WsSharedCount:
    case WsShareableCount:
        if (WsInformationLength < 4)
            return STATUS_BUFFER_TOO_SMALL;
        goto WsCounters;
    case WsAllCounts:
        if (WsInformationLength < sizeof(WS_ALL_COUNTS))
            return STATUS_BUFFER_TOO_SMALL;
WsCounters:
        {
            PROCESS_MEMORY_COUNTERS procMem;
            ULONG count = 0;
            ULONG privateCount = 0;
            ULONG sharedCount = 0;
            ULONG shareableCount = 0;
            PPSAPI_WORKING_SET_INFORMATION wsInfo;
            ULONG wsInfoLength;
            ULONG i;

            if (!GetProcessMemoryInfo(ProcessHandle, &procMem, sizeof(procMem)))
                return STATUS_UNSUCCESSFUL;

            /* Assume the page size is 4kB */
            wsInfoLength = sizeof(PSAPI_WORKING_SET_INFORMATION) + 
                sizeof(PSAPI_WORKING_SET_BLOCK) * (procMem.WorkingSetSize / 4096);
            wsInfo = (PPSAPI_WORKING_SET_INFORMATION)PhAlloc(wsInfoLength);

            if (!QueryWorkingSet(ProcessHandle, wsInfo, wsInfoLength))
            {
                PhFree(wsInfo);
                return STATUS_UNSUCCESSFUL;
            }

            for (i = 0; i < wsInfo->NumberOfEntries; i++)
            {
                PSAPI_WORKING_SET_BLOCK block = wsInfo->WorkingSetInfo[i];

                count++;

                if (block.ShareCount > 1)
                    sharedCount++;
                if (block.ShareCount == 0)
                    privateCount++;
                if (block.Shared)
                    shareableCount++;
            }

            PhFree(wsInfo);

            switch (WsInformationClass)
            {
            case WsCount:
                *(PULONG)WsInformation = count;
                break;
            case WsPrivateCount:
                *(PULONG)WsInformation = privateCount;
                break;
            case WsSharedCount:
                *(PULONG)WsInformation = sharedCount;
                break;
            case WsShareableCount:
                *(PULONG)WsInformation = shareableCount;
                break;
            case WsAllCounts:
                {
                    PWS_ALL_COUNTS allCounts = (PWS_ALL_COUNTS)WsInformation;

                    allCounts->Count = count;
                    allCounts->PrivateCount = privateCount;
                    allCounts->SharedCount = sharedCount;
                    allCounts->ShareableCount = shareableCount;
                    break;
                }
            }

            return STATUS_SUCCESS;
        }
        break;
    default: 
        return STATUS_INVALID_PARAMETER;
    }
}
