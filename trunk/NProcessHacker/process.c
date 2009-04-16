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
    case WsValidCount:
    case WsPrivateCount:
    case WsSharedCount:
    case WsShareableCount:
    case WsLockedCount:
        if (WsInformationLength < 4)
            return STATUS_BUFFER_TOO_SMALL;
    case WsAllCounts:
        if (WsInformationLength < sizeof(WS_ALL_COUNTS))
            return STATUS_BUFFER_TOO_SMALL;

        {
            PROCESS_MEMORY_COUNTERS procMem;
            ULONG count = 0;
            ULONG validCount = 0;
            ULONG privateCount = 0;
            ULONG sharedCount = 0;
            ULONG shareableCount = 0;
            ULONG lockedCount = 0;
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

                if (block.
            }
        }
    default:
        return STATUS_INVALID_PARAMETER;
    }
}
