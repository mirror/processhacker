#include "stdafx.h"	
#include "tlhelp32.h"

BOOL EnableTokenPrivilege(LPCTSTR pszPrivilege)
{
    HANDLE token = 0;
    TOKEN_PRIVILEGES tkp = {0}; 

    // Get a token for this process.

    if (!OpenProcessToken(GetCurrentProcess(), TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, &token))
    {
        return FALSE;
    }

    if (LookupPrivilegeValue(NULL, pszPrivilege, &tkp.Privileges[0].Luid)) 
    {
        tkp.PrivilegeCount = 1; 
															  
        tkp.Privileges[0].Attributes = SE_PRIVILEGE_ENABLED;

        AdjustTokenPrivileges(token, FALSE, &tkp, 0, (PTOKEN_PRIVILEGES)NULL, 0); 

        if (GetLastError() != ERROR_SUCCESS)
           return FALSE;
															
        return TRUE;
    }

    return FALSE;
}

DWORD GetPIDFromName(LPCWSTR name)
{
	PROCESSENTRY32 proc;
	HANDLE snapshot = NULL;

	snapshot = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
		
	proc.dwSize = sizeof(PROCESSENTRY32);

	Process32First(snapshot, &proc);

	do
	{
		if (_wcsicmp(proc.szExeFile, name) == 0)
		{
			return proc.th32ProcessID;
		}
	}  while (Process32Next(snapshot, &proc));

	return -1;
}	   

int _tmain(int argc, _TCHAR* argv[])
{
	HANDLE Proc;
	LPCWSTR DllName = (LPCWSTR) argv[2];	  
	LPCWSTR ExeName = (LPCWSTR) argv[3];
	LPVOID RemoteString, LoadLib, RunAddr;
	DWORD ThreadID;

	if (argc < 3)
		return 0;

	EnableTokenPrivilege(SE_DEBUG_NAME);

	Proc = OpenProcess(PROCESS_CREATE_THREAD | PROCESS_VM_OPERATION | PROCESS_VM_WRITE, FALSE, _wtoi((LPCWSTR) argv[1]));

	LoadLib = (LPVOID)GetProcAddress(GetModuleHandle(_T("kernel32.dll")), (const char *) "LoadLibraryW");

	RemoteString = (LPVOID)VirtualAllocEx(Proc, NULL, wcslen(DllName) * 2 + 1, MEM_RESERVE | MEM_COMMIT, PAGE_READWRITE);
	WriteProcessMemory(Proc, (LPVOID)RemoteString, DllName, wcslen(DllName) * 2 + 1, NULL);
	CreateRemoteThread(Proc, NULL, NULL, (LPTHREAD_START_ROUTINE)LoadLib, (LPVOID)RemoteString, NULL, NULL);   

	Sleep(500);		 
	VirtualFreeEx(Proc, RemoteString, wcslen(DllName) * 2 + 1, 0);
								  
  	LoadLibrary(_T("InjectorDLL.dll"));
	RunAddr = (LPVOID)GetProcAddress(GetModuleHandle(_T("InjectorDLL.dll")), (LPCSTR) 1);  
	RemoteString = (LPVOID)VirtualAllocEx(Proc, NULL, wcslen(ExeName) * 2 + 1, MEM_RESERVE | MEM_COMMIT, PAGE_READWRITE); 
	WriteProcessMemory(Proc, (LPVOID)RemoteString, ExeName, wcslen(ExeName) * 2 + 1, NULL);
	CreateRemoteThread(Proc, NULL, NULL, (LPTHREAD_START_ROUTINE)RunAddr, (LPVOID)RemoteString, NULL, &ThreadID);   

	CloseHandle(Proc); 

	return 0;
}

