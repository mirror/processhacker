#include "stdafx.h"
#include "InjectorDLL.h"

INJECTORDLL_API void Run(LPCWSTR file)
{		
	try
	{
		STARTUPINFO StartupInfo;
		PROCESS_INFORMATION ProcInfo;

		memset(&StartupInfo, 0, sizeof(STARTUPINFO));

		CreateProcessW(file, _T(""), NULL, NULL, FALSE, 0, NULL, NULL, &StartupInfo, &ProcInfo);
		VirtualFree((LPVOID) file, wcslen(file) * 2 + 1, 0);

		CloseHandle(ProcInfo.hProcess);
		CloseHandle(ProcInfo.hThread);
								
		// Unload this library
		CreateThread(NULL, NULL, (LPTHREAD_START_ROUTINE) GetProcAddress(GetModuleHandle(_T("kernel32.dll")), 
			(const char *) "FreeLibrary"), (LPVOID) GetModuleHandle(_T("InjectorDLL.dll")), NULL, NULL);

		ExitThread(0);
	}
	catch (int ex)
	{
		// stop warning
		ex++;
	}
}
