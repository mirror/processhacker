/*
 *  ReactOS Task Manager
 *
 *  priority.c
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

void DoSetPriority(DWORD priority)
{
    DWORD dwProcessId = 0;
    HANDLE hProcess = NULL;
    WCHAR szText[MAX_PATH], szTitle[256];

    dwProcessId = GetSelectedProcessId();

    if (dwProcessId == 0)
        return;

    LoadString(hInst, IDS_MSG_TASKMGRWARNING, szTitle, 256);
    LoadString(hInst, IDS_MSG_WARNINGCHANGEPRIORITY, szText, MAX_PATH);
    
    if (MessageBox(hMainWnd, szText, szTitle, MB_YESNO | MB_ICONWARNING) != IDYES)
        return;

    hProcess = OpenProcess(PROCESS_SET_INFORMATION, FALSE, dwProcessId);

    if (!hProcess)
    {
        GetLastErrorText(szText, MAX_PATH);
        LoadString(hInst, IDS_MSG_UNABLECHANGEPRIORITY, szTitle, 256);
        MessageBox(hMainWnd, szText, szTitle, MB_OK|MB_ICONSTOP);
        return;
    }

    if (!SetPriorityClass(hProcess, priority))
    {
        GetLastErrorText(szText, MAX_PATH);
        LoadString(hInst, IDS_MSG_UNABLECHANGEPRIORITY, szTitle, 256);
        MessageBox(hMainWnd, szText, szTitle, MB_OK | MB_ICONSTOP);
    }

    NtClose(hProcess);
}
