/*
 * Process Hacker
 * - Injector
 *
 * Copyright (C) 2008 wj32
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

#include "stdafx.h"

typedef const wchar_t *(__stdcall *RGetCommandLineW)();
typedef BOOL (__stdcall *RCreateProcessW)(
    __in_opt    LPCWSTR lpApplicationName,
    __inout_opt LPWSTR lpCommandLine,
    __in_opt    LPSECURITY_ATTRIBUTES lpProcessAttributes,
    __in_opt    LPSECURITY_ATTRIBUTES lpThreadAttributes,
    __in        BOOL bInheritHandles,
    __in        DWORD dwCreationFlags,
    __in_opt    LPVOID lpEnvironment,
    __in_opt    LPCWSTR lpCurrentDirectory,
    __in        LPSTARTUPINFOW lpStartupInfo,
    __out       LPPROCESS_INFORMATION lpProcessInformation
	);
typedef BOOL (__stdcall *RCloseHandle)(HANDLE handle);

struct data_struct
{
	DWORD last_error; // error code from thread
	RCreateProcessW fCPW;
	RCloseHandle fCH;
	RGetCommandLineW fGCLW;
	wchar_t winsta_desktop[64];
	wchar_t str[4000];
};

wchar_t *GetWinStaDesktop();
DWORD __stdcall Cp(data_struct *data);
DWORD __stdcall GRcl(data_struct *data);
BOOL EnableTokenPrivilege(LPCTSTR pszPrivilege);

int _tmain(int argc, wchar_t *argv[])
{
	HANDLE handle = 0;
	DWORD pid;
	wchar_t *mode;
	void *local_code = 0;
	void *remote_code = 0;
	data_struct *remote_data = 0;
	data_struct local_data;
	DWORD return_code = 0;
	HMODULE kernel32 = 0;
	HANDLE remote_thread = 0;
	DWORD rc = 0;
	wchar_t *winsta_desktop = GetWinStaDesktop();
	
	if (!EnableTokenPrivilege(SE_DEBUG_NAME))
		printf("Could not get debug privilege!");

	mode = argv[1];
	pid = _wtoi(argv[2]);
	
	if (argc > 3)
		wcscpy_s(local_data.str, 4000, argv[3]);
	else
		local_data.str[0] = 0;

	wcscpy(local_data.winsta_desktop, winsta_desktop);

	if (!(handle = OpenProcess(PROCESS_CREATE_THREAD | PROCESS_VM_OPERATION | 
		PROCESS_VM_WRITE | PROCESS_VM_READ, FALSE, pid)))
	{
		printf("Could not open process!");
		return GetLastError();
	}
	
	if (!(remote_code = VirtualAllocEx(handle, 0, 4096, MEM_COMMIT, PAGE_EXECUTE_READWRITE)))
	{
		printf("Could not allocate memory!");
		return_code = GetLastError();
		goto clean_up;
	}

	if (!(remote_data = (data_struct *)VirtualAllocEx(handle, 0, 
		sizeof(local_data), MEM_COMMIT, PAGE_READWRITE)))
	{
		printf("Could not allocate memory!");
		return_code = GetLastError();
		goto clean_up;
	}

	if (wcscmp(mode, _T("cmdline")) == 0)
		local_code = &GRcl;
	else if (wcscmp(mode, _T("createprocess")) == 0)
		local_code = &Cp;
	
	if (!WriteProcessMemory(handle, remote_code, local_code, 4096, 0))
	{
		printf("Could not write remote code!");
		return_code = GetLastError();
		goto clean_up;
	}

	if (!(kernel32 = LoadLibrary(L"kernel32.dll")))
	{
		printf("Could not load kernel32.dll!");
		return_code = GetLastError();
		goto clean_up;
	}

	local_data.last_error = 0;
	
	if (!(local_data.fGCLW = (RGetCommandLineW)GetProcAddress(kernel32, "GetCommandLineW")))
	{
		printf("Could not get address of GetCommandLineW!");
		return_code = GetLastError();
		goto clean_up;
	}
	
	if (!(local_data.fCPW = (RCreateProcessW)GetProcAddress(kernel32, "CreateProcessW")))
	{
		printf("Could not get address of CreateProcessW!");
		return_code = GetLastError();
		goto clean_up;
	}
	
	if (!(local_data.fCH = (RCloseHandle)GetProcAddress(kernel32, "CloseHandle")))
	{
		printf("Could not get address of CloseHandle!");
		return_code = GetLastError();
		goto clean_up;
	}

	FreeLibrary(kernel32);

	if (!WriteProcessMemory(handle, remote_data, &local_data, sizeof(local_data), 0))
	{
		printf("Could not write remote data!");
		return_code = GetLastError();
		goto clean_up;
	}

	if (!(remote_thread = CreateRemoteThread(handle, 0, 0, (LPTHREAD_START_ROUTINE)remote_code,
		remote_data, 0, &rc)))
	{
		printf("Could not create remote thread!");
		return_code = GetLastError();
		goto clean_up;
	}

	rc = WaitForSingleObject(remote_thread, 5000);

	switch (rc)
	{
	case WAIT_TIMEOUT:
		printf("WaitForSingleObject timed out!");
		goto clean_up;
	case WAIT_FAILED:
		printf("WaitForSingleObject failed!");
		return_code = GetLastError();
		goto clean_up;
	case WAIT_OBJECT_0:
		if (!ReadProcessMemory(handle, remote_data, &local_data, sizeof(local_data), 0))
		{
			printf("Could not read process memory!");
			return_code = GetLastError();
			goto clean_up;
		}

		if (local_data.last_error != 0)
		{
			printf("Error in remote thread!");
			return_code = GetLastError();
		}

		return_code = 0;
		
		break;
	}

	wprintf(_T("%s"), local_data.str);

clean_up:
	if (remote_thread != 0)
		CloseHandle(remote_thread);

	if (remote_code != 0)
		VirtualFreeEx(handle, remote_code, 0, MEM_RELEASE);
	if (remote_data != 0)
		VirtualFreeEx(handle, remote_data, 0, MEM_RELEASE);
	
	if (handle != 0)
		CloseHandle(handle);

	return return_code;
}

wchar_t *GetWinStaDesktop()
{
	HWINSTA winsta = GetProcessWindowStation();
	HDESK desktop = GetThreadDesktop(GetCurrentThreadId());
	wchar_t *result = 0;
	wchar_t *winsta_name = 0;
	wchar_t *desktop_name = 0;
	DWORD required_size = 0;

	GetUserObjectInformation(winsta, UOI_NAME, winsta_name, 0, &required_size);
	winsta_name = (wchar_t *)malloc(required_size);
	GetUserObjectInformation(winsta, UOI_NAME, winsta_name, required_size, 0);

	GetUserObjectInformation(desktop, UOI_NAME, desktop_name, 0, &required_size);
	desktop_name = (wchar_t *)malloc(required_size);
	GetUserObjectInformation(desktop, UOI_NAME, desktop_name, required_size, 0);

	result = (wchar_t *)malloc(wcslen(winsta_name) + wcslen(desktop_name) + 2);
	wcscpy(result, winsta_name);
	result[wcslen(winsta_name)] = '\\';
	wcscpy(result + wcslen(winsta_name) + 1, desktop_name);

	return result;
}

DWORD __stdcall Cp(data_struct *data)
{
	STARTUPINFOW startup_info;
	PROCESS_INFORMATION proc_info;
	
	startup_info.cb = sizeof(startup_info);
	startup_info.lpDesktop = data->winsta_desktop;
	startup_info.dwFlags = STARTF_FORCEONFEEDBACK;

	if (!data->fCPW(NULL, data->str, 0, 0, FALSE, 0, NULL, NULL, &startup_info, 
		&proc_info))
	{
		data->last_error = 1;
		return 1;
	}

	data->fCH(proc_info.hProcess);
	data->fCH(proc_info.hThread);

	return 0;
}

DWORD __stdcall GRcl(data_struct *data)
{
	const wchar_t *src;
	wchar_t *tgt, *end;

	src = data->fGCLW();
	tgt = data->str;
	end = data->str + 3999;

	if (src == 0 || tgt == 0 || end == 0)
	{
		data->last_error = 1;
		return 1;
	}

	for (; *src != 0 && tgt < end; ++src, ++tgt)
		*tgt = *src;

	*tgt = 0;
	data->last_error = 0;

	return 0;
}

BOOL EnableTokenPrivilege(LPCTSTR pszPrivilege)
{
    HANDLE hToken        = 0;
    TOKEN_PRIVILEGES tkp = {0}; 

    // Get a token for this process.

    if (!OpenProcessToken(GetCurrentProcess(),
                          TOKEN_ADJUST_PRIVILEGES |
                          TOKEN_QUERY, &hToken))
    {
        return FALSE;
    }

    // Get the LUID for the privilege. 

    if(LookupPrivilegeValue(NULL, pszPrivilege,
                            &tkp.Privileges[0].Luid)) 
    {
        tkp.PrivilegeCount = 1;  // one privilege to set	  
															  
        tkp.Privileges[0].Attributes = SE_PRIVILEGE_ENABLED;

        // Set the privilege for this process. 

        AdjustTokenPrivileges(hToken, FALSE, &tkp, 0,
                              (PTOKEN_PRIVILEGES)NULL, 0); 

        if (GetLastError() != ERROR_SUCCESS)
           return FALSE;
															
        return TRUE;
    }

    return FALSE;
}
