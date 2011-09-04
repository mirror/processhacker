/*
 *  ReactOS Task Manager
 *
 *  applicationpage.h
 *
 *  Copyright (C) 1999 - 2001  Brian Palmer  <brianp@reactos.org>
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

#pragma once

extern    HWND        hApplicationPage;                /* Application List Property Page */

extern    HWND        hApplicationPageListCtrl;        /* Application ListCtrl Window */
extern    HWND        hApplicationPageEndTaskButton;    /* Application End Task button */
extern    HWND        hApplicationPageSwitchToButton;    /* Application Switch To button */
extern    HWND        hApplicationPageNewTaskButton;    /* Application New Task button */

INT_PTR CALLBACK    ApplicationPageWndProc(HWND hDlg, UINT message, WPARAM wParam, LPARAM lParam);
void                RefreshApplicationPage(void);
void                UpdateApplicationListControlViewSetting(void);

void                ApplicationPage_OnView(DWORD);
void                ApplicationPage_OnWindowsTile(DWORD);
void                ApplicationPage_OnWindowsMinimize(void);
void                ApplicationPage_OnWindowsMaximize(void);
void                ApplicationPage_OnWindowsCascade(void);
void                ApplicationPage_OnWindowsBringToFront(void);
void                ApplicationPage_OnSwitchTo(void);
void                ApplicationPage_OnEndTask(void);
void                ApplicationPage_OnGotoProcess(void);


typedef struct
{
    HWND    hWnd;
    WCHAR   szTitle[260];
    HICON   hIcon;
    BOOL    bHung;
} APPLICATION_PAGE_LIST_ITEM, *LPAPPLICATION_PAGE_LIST_ITEM;

HWND            hApplicationPage;               /* Application List Property Page */
HWND            hNetworkPage;               /* Application List Property Page */
HWND            hApplicationPageListCtrl;       /* Application ListCtrl Window */
HWND            hApplicationPageEndTaskButton;  /* Application End Task button */
HWND            hApplicationPageSwitchToButton; /* Application Switch To button */
HWND            hApplicationPageNewTaskButton;  /* Application New Task button */
static int      nApplicationPageWidth;
static int      nApplicationPageHeight;
UINT WINAPI    ApplicationPageRefreshThread(void *lpParameter);
BOOL            noApps;
BOOL CALLBACK   EnumWindowsProc(HWND hWnd, LPARAM lParam);
void            AddOrUpdateHwnd(HWND hWnd, WCHAR *szTitle, HICON hIcon, BOOL bHung);
void            ApplicationPageUpdate(void);
void            ApplicationPageOnNotify(WPARAM wParam, LPARAM lParam);
void            ApplicationPageShowContextMenu1(void);
void            ApplicationPageShowContextMenu2(void);
int CALLBACK    ApplicationPageCompareFunc(LPARAM lParam1, LPARAM lParam2, LPARAM lParamSort);
int             ProcGetIndexByProcessId(DWORD dwProcessId);
