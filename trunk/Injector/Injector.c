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

#include <stdio.h>
#include <tchar.h>
#include <windows.h>

typedef const wchar_t *(__stdcall *RGetCommandLineW)();
typedef BOOL (__stdcall *RCreateProcessW)(
    LPCWSTR lpApplicationName,
    LPWSTR lpCommandLine,
    LPSECURITY_ATTRIBUTES lpProcessAttributes,
    LPSECURITY_ATTRIBUTES lpThreadAttributes,
    BOOL bInheritHandles,
    DWORD dwCreationFlags,
    LPVOID lpEnvironment,
    LPCWSTR lpCurrentDirectory,
    LPSTARTUPINFOW lpStartupInfo,
    LPPROCESS_INFORMATION lpProcessInformation
	);
typedef BOOL (__stdcall *RCloseHandle)(HANDLE handle);
typedef BOOL (__stdcall *RExitProcess)(DWORD ExitCode);

#define MAX_STR 4000
#define MAX_CODE 8192

#define STREQ(x,y) (wcscmp((x), _T(y)) == 0)
#define MODE_CMDLINE 1
#define MODE_CREATEPROCESSA 2
#define MODE_CREATEPROCESSC 3
#define MODE_EXITPROCESS 4

struct data_struct_s
{
	DWORD last_error; // error code from thread
	RCreateProcessW fCPW;
	RCloseHandle fCH;
	RExitProcess fEP;
	RGetCommandLineW fGCLW;
	wchar_t winsta_desktop[64];
	wchar_t str[MAX_STR];
};

typedef struct data_struct_s data_struct;

wchar_t *GetDesktopName();
wchar_t *GetWinStaDesktop();
void Ep(data_struct *data);
DWORD CpApp(data_struct *data);
DWORD CpCmd(data_struct *data);
DWORD GRcl(data_struct *data);
BOOL EnableTokenPrivilege(LPCTSTR pszPrivilege);

int _tmain(int argc, wchar_t *argv[])
{
	HANDLE handle = 0;
	DWORD old_protect;
	DWORD pid;
	wchar_t *mode_str;
	DWORD mode = 0;
	void *local_code = 0;
	void *remote_code = 0;
	data_struct *remote_data = 0;
	data_struct local_data;
	DWORD return_code = 0;
	HMODULE kernel32 = 0;
	HANDLE remote_thread = 0;
	DWORD rc = 0;
	wchar_t *winsta_desktop = GetDesktopName();
	
	if (!EnableTokenPrivilege(SE_DEBUG_NAME))
		printf("Could not get debug privilege!\n");

	if (argc < 3)
	{
		printf("Need more arguments.\n");
		return ERROR_INVALID_PARAMETER;
	}

	mode_str = argv[1];

	if (STREQ(mode_str, "cmdline"))
		mode = MODE_CMDLINE;
	else if (STREQ(mode_str, "createprocess"))
		mode = MODE_CREATEPROCESSA;
	else if (STREQ(mode_str, "createprocessa"))
		mode = MODE_CREATEPROCESSA;
	else if (STREQ(mode_str, "createprocessc"))
		mode = MODE_CREATEPROCESSC;
	else if (STREQ(mode_str, "exitprocess"))
		mode = MODE_EXITPROCESS;
	else
	{
		printf("Invalid mode.\n");
		return ERROR_INVALID_PARAMETER;
	}
	
	pid = _wtoi(argv[2]);
	
	if (argc > 3)
		wcscpy(local_data.str, argv[3]);
	else
		local_data.str[0] = 0;

	wcscpy(local_data.winsta_desktop, winsta_desktop);

	if (!(handle = OpenProcess(PROCESS_CREATE_THREAD | PROCESS_VM_OPERATION | 
		PROCESS_VM_WRITE | PROCESS_VM_READ, FALSE, pid)))
	{
		printf("Could not open process!\n");
		return GetLastError();
	}
	
	if (!(remote_code = VirtualAllocEx(handle, 0, MAX_CODE, MEM_COMMIT, PAGE_EXECUTE_READWRITE)))
	{
		printf("Could not allocate memory!\n");
		return_code = GetLastError();
		goto clean_up;
	}

	if (!(remote_data = (data_struct *)VirtualAllocEx(handle, 0, 
		sizeof(local_data), MEM_COMMIT, PAGE_READWRITE)))
	{
		printf("Could not allocate memory!\n");
		return_code = GetLastError();
		goto clean_up;
	}

	if (mode == MODE_CMDLINE)
		local_code = &GRcl;
	else if (mode == MODE_CREATEPROCESSA)
		local_code = &CpApp;
	else if (mode == MODE_CREATEPROCESSC)
		local_code = &CpCmd;
	else if (mode == MODE_EXITPROCESS)
		local_code = &Ep;
	
	if (!WriteProcessMemory(handle, remote_code, local_code, MAX_CODE, 0))
	{
		printf("Could not write remote code!\n");
		return_code = GetLastError();
		goto clean_up;
	}

	if (!VirtualProtectEx(handle, remote_code, MAX_CODE, PAGE_EXECUTE, &old_protect))
		printf("Warning: could not set page protection on code to PAGE_EXECUTE\n");

	if (!(kernel32 = LoadLibrary(L"kernel32.dll")))
	{
		printf("Could not load kernel32.dll!\n");
		return_code = GetLastError();
		goto clean_up;
	}

	local_data.last_error = 0;
	
	if (!(local_data.fGCLW = (RGetCommandLineW)GetProcAddress(kernel32, "GetCommandLineW")))
	{
		printf("Could not get address of GetCommandLineW!\n");
		return_code = GetLastError();
		goto clean_up;
	}
	
	if (!(local_data.fCPW = (RCreateProcessW)GetProcAddress(kernel32, "CreateProcessW")))
	{
		printf("Could not get address of CreateProcessW!\n");
		return_code = GetLastError();
		goto clean_up;
	}
	
	if (!(local_data.fCH = (RCloseHandle)GetProcAddress(kernel32, "CloseHandle")))
	{
		printf("Could not get address of CloseHandle!\n");
		return_code = GetLastError();
		goto clean_up;
	}
	
	if (!(local_data.fEP = (RExitProcess)GetProcAddress(kernel32, "ExitProcess")))
	{
		printf("Could not get address of ExitProcess!\n");
		return_code = GetLastError();
		goto clean_up;
	}

	if (!WriteProcessMemory(handle, remote_data, &local_data, sizeof(local_data), 0))
	{
		printf("Could not write remote data!\n");
		return_code = GetLastError();
		goto clean_up;
	}

	if (!(remote_thread = CreateRemoteThread(handle, 0, 0, (LPTHREAD_START_ROUTINE)remote_code,
	    remote_data, 0, &rc)))
	{
		printf("Could not create remote thread!\n");
		return_code = GetLastError();
		goto clean_up;
	}
	
	rc = WaitForSingleObject(remote_thread, 5000);

	if (mode == MODE_CMDLINE)
	{
		switch (rc)
		{
		case WAIT_TIMEOUT:
			printf("WaitForSingleObject timed out!\n");
			goto clean_up;
		case WAIT_FAILED:
			printf("WaitForSingleObject failed!\n");
			return_code = GetLastError();
			goto clean_up;
		case WAIT_OBJECT_0:
			if (!ReadProcessMemory(handle, remote_data, &local_data, sizeof(local_data), 0))
			{
				printf("Could not read process memory!\n");
				return_code = GetLastError();
				goto clean_up;
			}

			if (local_data.last_error != 0)
			{
				printf("Error in remote thread!\n");
				return_code = GetLastError();
				goto clean_up;
			}

			return_code = 0;
			
			break;
		}

		wprintf(_T("%s"), local_data.str);
	}

clean_up:
	if (remote_code != 0)
		VirtualFreeEx(handle, remote_code, 0, MEM_RELEASE);
	if (remote_data != 0)
		VirtualFreeEx(handle, remote_data, 0, MEM_RELEASE);

	return return_code;
}

wchar_t *GetDesktopName()
{
	HDESK desktop = GetThreadDesktop(GetCurrentThreadId());
	wchar_t *desktop_name = 0;
	DWORD required_size = 0;

	GetUserObjectInformation(desktop, UOI_NAME, desktop_name, 0, &required_size);
	desktop_name = (wchar_t *)malloc(required_size);
	GetUserObjectInformation(desktop, UOI_NAME, desktop_name, required_size, 0);

	return desktop_name;
}

static void Ep(data_struct *data)
{
	data->fEP(0);
}

static DWORD CpApp(data_struct *data)
{
	STARTUPINFOW startup_info;
	PROCESS_INFORMATION proc_info;
	
	startup_info.cb = sizeof(startup_info);
	startup_info.lpDesktop = data->winsta_desktop;
	startup_info.lpTitle = NULL;
	startup_info.dwFlags = STARTF_FORCEONFEEDBACK;

	if (!data->fCPW(data->str, NULL, 0, 0, FALSE, 0, NULL, NULL, &startup_info, 
		&proc_info))
	{
		data->last_error = 1;
		return 1;
	}

	data->fCH(proc_info.hProcess);
	data->fCH(proc_info.hThread);

	return 0;
}

static DWORD CpCmd(data_struct *data)
{
	STARTUPINFOW startup_info;
	PROCESS_INFORMATION proc_info;
	
	startup_info.cb = sizeof(startup_info);
	startup_info.lpDesktop = data->winsta_desktop;
	startup_info.lpTitle = NULL;
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

static DWORD GRcl(data_struct *data)
{
	const wchar_t *src;
	wchar_t *tgt, *end;

	src = data->fGCLW();
	tgt = data->str;
	end = &data->str[MAX_STR - 1];

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

BOOL EnableTokenPrivilege(LPCTSTR privilege)
{
    HANDLE token_handle = 0;
    TOKEN_PRIVILEGES tkp = {0}; 

    if (!OpenProcessToken(GetCurrentProcess(), TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, &token_handle))
    {
        return FALSE;
    }

    if (LookupPrivilegeValue(NULL, privilege, &tkp.Privileges[0].Luid)) 
    {
        tkp.PrivilegeCount = 1;						  
        tkp.Privileges[0].Attributes = SE_PRIVILEGE_ENABLED;

        AdjustTokenPrivileges(token_handle, FALSE, &tkp, 0, (PTOKEN_PRIVILEGES)NULL, 0); 
		CloseHandle(token_handle);

        if (GetLastError() != ERROR_SUCCESS)
           return FALSE;

        return TRUE;
    }

	CloseHandle(token_handle);

    return FALSE;
}
