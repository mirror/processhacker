#ifndef UNICODE
#define UNICODE
#endif
                    
#include <windows.h>  
#include <stdio.h>
#include "../kph.h"

int wmain(int argc, WCHAR *argv[])
{
    NTSTATUS status;
    ULONG pid;
    HANDLE kphHandle, processHandle;

    if (argc < 2)
    {
        printf("Usage: test [pid to kill]\n");
        return 1;
    }

    pid = _wtoi(argv[1]);

    status = KphConnect(&kphHandle);

    if (!NT_SUCCESS(status))
    {
        printf("Could not connect to KProcessHacker (0x%08x).\n", status);
        return status;
    }

    status = KphOpenProcess(kphHandle, &processHandle, pid, PROCESS_TERMINATE);

    if (!NT_SUCCESS(status))
    {
        printf("Could not open process with PID %d (0x%08x).\n", pid, status);
        return status;
    }

    status = KphTerminateProcess(kphHandle, processHandle, 0);

    if (!NT_SUCCESS(status))
    {
        printf("Could not terminate process (0x%08x).\n", status);
        return status;
    }

    CloseHandle(processHandle);
    KphDisconnect(kphHandle);

    return 0;
}
