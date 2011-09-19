/*
 *  ReactOS Task Manager
 *
 *  debug.c
 *
 *  Copyright (C) 1999 - 2001  Brian Palmer  <brianp@reactos.org>
 *                2005         Klemens Friedl <frik85@reactos.at>
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

void ProcessPage_OnDebug(void)
{
    DWORD dwProcessId;
    WCHAR strErrorText[MAX_PATH];
    HKEY hKey;
    WCHAR strDebugPath[MAX_PATH], strDebugger[MAX_PATH];
    DWORD dwDebuggerSize;
    PROCESS_INFORMATION pi;
    STARTUPINFO si;
    HANDLE hDebugEvent;
    WCHAR szTemp[256], szTempA[256];

    dwProcessId = GetSelectedProcessId();

    if (dwProcessId == 0)
        return;

    LoadString(hInst, IDS_MSG_WARNINGDEBUG, szTemp, 256);
    LoadString(hInst, IDS_MSG_TASKMGRWARNING, szTempA, 256);

    if (MessageBox(hMainWnd, szTemp, szTempA, MB_YESNO|MB_ICONWARNING) != IDYES)
    {
        GetLastErrorText(strErrorText, MAX_PATH);
        LoadString(hInst, IDS_MSG_UNABLEDEBUGPROCESS, szTemp, 256);
        MessageBox(hMainWnd, strErrorText, szTemp, MB_OK | MB_ICONSTOP);
        return;
    }

    if (RegOpenKeyEx(HKEY_LOCAL_MACHINE, TEXT("Software\\Microsoft\\Windows NT\\CurrentVersion\\AeDebug"), 0, KEY_READ, &hKey) != ERROR_SUCCESS)
    {
        GetLastErrorText(strErrorText, MAX_PATH);
        LoadString(hInst, IDS_MSG_UNABLEDEBUGPROCESS, szTemp, 256);
        MessageBox(hMainWnd, strErrorText, szTemp, MB_OK | MB_ICONSTOP);
        return;
    }

    dwDebuggerSize = MAX_PATH;

    if (RegQueryValueEx(hKey, TEXT("Debugger"), NULL, NULL, (LPBYTE)strDebugger, &dwDebuggerSize) != ERROR_SUCCESS)
    {
        GetLastErrorText(strErrorText, MAX_PATH);
        LoadString(hInst, IDS_MSG_UNABLEDEBUGPROCESS, szTemp, 256);
        MessageBox(hMainWnd, strErrorText, szTemp, MB_OK | MB_ICONSTOP);
        RegCloseKey(hKey);
        return;
    }

    RegCloseKey(hKey);

    hDebugEvent = CreateEvent(NULL, FALSE, FALSE, NULL);
   
    if (!hDebugEvent)
    {
        GetLastErrorText(strErrorText, MAX_PATH);
        LoadString(hInst, IDS_MSG_UNABLEDEBUGPROCESS, szTemp, 256);
        MessageBox(hMainWnd, strErrorText, szTemp, MB_OK | MB_ICONSTOP);
        return;
    }

    wsprintf(strDebugPath, strDebugger, dwProcessId, hDebugEvent);

    memset(&pi, 0, sizeof(PROCESS_INFORMATION));
    memset(&si, 0, sizeof(STARTUPINFO));
    si.cb = sizeof(STARTUPINFO);

    if (!CreateProcess(NULL, strDebugPath, NULL, NULL, FALSE, 0, NULL, NULL, &si, &pi))
    {
        GetLastErrorText(strErrorText, MAX_PATH);
        LoadString(hInst, IDS_MSG_UNABLEDEBUGPROCESS, szTemp, 256);
        MessageBox(hMainWnd, strErrorText, szTemp, MB_OK | MB_ICONSTOP);
    }
	
	NtClose(pi.hThread);
    NtClose(hDebugEvent);
}