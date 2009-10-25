#ifndef UNICODE
#define UNICODE
#endif
                    
#include <windows.h>  
#include <stdio.h>
#include "../kph.h"
#include "../kphhook.h"

int wmain(int argc, WCHAR *argv[])
{
    ULONG pid;
    HANDLE processHandle;
    CHAR memory[0x1000];

    if (argc < 2)
    {
        printf("Usage: test [pid to kill]\n");
        return 1;
    }

    pid = _wtoi(argv[1]);

    KphHookInit();
    processHandle = OpenProcess(PROCESS_ALL_ACCESS, FALSE, pid);
    ReadProcessMemory(processHandle, (PVOID)0x10000, memory, 0x1000, NULL);

    return 0;
}
