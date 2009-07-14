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

#include "obj.h"

ULONG PHAPI PhpQueryFileObjectThreadStart(
    PVOID Parameter
    );

_NtQueryObject NtQueryObject = NULL;

HANDLE QueryFileObjectThreadHandle = NULL;
PVOID QueryFileObjectFiber = NULL;
CRITICAL_SECTION QueryFileObjectCs;
HANDLE QueryFileObjectStartEvent = NULL;
HANDLE QueryFileObjectCompletedEvent = NULL;
HANDLE QueryFileObjectFileHandle;
PH_QUERY_FILE_OBJECT_BUFFER QueryFileObjectBuffer;

NTSTATUS PHAPI PhObjInit()
{
    if (!(NtQueryObject = (_NtQueryObject)
        PhGetProcAddress(L"ntdll.dll", "NtQueryObject")))
        return STATUS_PROCEDURE_NOT_FOUND;

    InitializeCriticalSection(&QueryFileObjectCs);

    return STATUS_SUCCESS;
}

NTSTATUS PHAPI PhQueryNameFileObject(
    HANDLE FileHandle,
    POBJECT_NAME_INFORMATION FileObjectNameInformation,
    ULONG FileObjectNameInformationLength,
    PULONG ReturnLength
    )
{
    ULONG waitResult;

    EnterCriticalSection(&QueryFileObjectCs);

    /* Create a query thread if we don't have one. */
    if (!QueryFileObjectThreadHandle)
    {
        QueryFileObjectThreadHandle = CreateThread(
            NULL, 0, (LPTHREAD_START_ROUTINE)PhpQueryFileObjectThreadStart, NULL, 0, NULL);

        if (!QueryFileObjectThreadHandle)
        {
            LeaveCriticalSection(&QueryFileObjectCs);
            return STATUS_UNSUCCESSFUL;
        }
    }

    /* Create the events if they don't exist. */
    if (!QueryFileObjectStartEvent)
        if (!(QueryFileObjectStartEvent = CreateEvent(NULL, FALSE, FALSE, NULL)))
            return STATUS_UNSUCCESSFUL;
    if (!QueryFileObjectCompletedEvent)
        if (!(QueryFileObjectCompletedEvent = CreateEvent(NULL, FALSE, FALSE, NULL)))
            return STATUS_UNSUCCESSFUL;

    /* Initialize the work context. */
    QueryFileObjectFileHandle = FileHandle;
    QueryFileObjectBuffer.Length = FileObjectNameInformationLength;
    QueryFileObjectBuffer.Name = FileObjectNameInformation;
    QueryFileObjectBuffer.Initialized = TRUE;
    /* Allow the worker thread to start. */
    SetEvent(QueryFileObjectStartEvent);
    /* Wait for the work to complete, with a timeout of 1 second. */
    waitResult = WaitForSingleObject(QueryFileObjectCompletedEvent, 1000);
    /* Set the buffer as uninitialized. */
    QueryFileObjectBuffer.Initialized = FALSE;

    /* Return normally if the work was completed. */
    if (waitResult == WAIT_OBJECT_0)
    {
        NTSTATUS status;
        ULONG returnLength;

        /* Copy the status information before we leave the critical section. */
        status = QueryFileObjectBuffer.Status;
        returnLength = QueryFileObjectBuffer.ReturnLength;
        LeaveCriticalSection(&QueryFileObjectCs);

        if (ReturnLength)
            *ReturnLength = returnLength;

        return status;
    }
    /* Kill the worker thread if it took too long. */
    /* else if (waitResult == WAIT_TIMEOUT) */
    else
    {
        /* Kill the thread. */
        if (TerminateThread(QueryFileObjectThreadHandle, 1))
        {
            QueryFileObjectThreadHandle = NULL;

            /* Delete the fiber (and free the thread stack). */
            DeleteFiber(QueryFileObjectFiber);
            QueryFileObjectFiber = NULL;
        }

        LeaveCriticalSection(&QueryFileObjectCs);
        return STATUS_UNSUCCESSFUL;
    }
}

ULONG PHAPI PhpQueryFileObjectThreadStart(
    PVOID Parameter
    )
{
    QueryFileObjectFiber = ConvertThreadToFiber(Parameter);

    while (TRUE)
    {
        /* Wait for work. */
        if (WaitForSingleObject(QueryFileObjectStartEvent, INFINITE) != WAIT_OBJECT_0)
            continue;

        /* Make sure we actually have work. */
        if (QueryFileObjectBuffer.Initialized)
        {
            QueryFileObjectBuffer.Status = NtQueryObject(
                QueryFileObjectFileHandle,
                ObjectNameInformation,
                QueryFileObjectBuffer.Name,
                QueryFileObjectBuffer.Length,
                &QueryFileObjectBuffer.ReturnLength
                );

            /* Work done. */
            SetEvent(QueryFileObjectCompletedEvent);
        }
    }

    return 0;
}
