/*
 *  ReactOS Task Manager
 *
 *  taskmgr.h
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

#pragma comment(lib, "comctl32.lib")
#pragma comment(lib, "uxtheme.lib")
#pragma comment(linker,"\"/manifestdependency:type='win32' name='Microsoft.Windows.Common-Controls' version='6.0.0.0' processorArchitecture='*' publicKeyToken='6595b64144ccf1df' language='*'\"")

#include <ctype.h>
#include <math.h>
#include <stdlib.h>
#include <stdio.h>
#include <process.h>
#include <windows.h>
#include <aclapi.h>
#include <commctrl.h>
#include <shellapi.h>
#include <Uxtheme.h>

#include "resource.h"

#include "column.h"
#include "perfdata.h"
#include "perfpage.h"
#include "about.h"
#include "procpage.h"
#include "proclist.h"
#include "affinity.h"
#include "applpage.h"
#include "dbgchnl.h"
#include "debug.h"
#include "endproc.h"
#include "graph.h"
#include "graphctl.h"
#include "optnmenu.h"
#include "priority.h"
#include "run.h"
#include "trayicon.h"

#include "ntwin.h"
#include "phnt.h"

#define STATUS_WINDOW	2001
#define STATUS_SIZE1	80
#define STATUS_SIZE2	210
#define STATUS_SIZE3	400

#define NUMBER_OF_ITEMS_IN_ARRAY(array) (sizeof array / sizeof array[0])

typedef struct
{
	/* Window size & position settings */
	BOOL	Maximized;
	int	Left;
	int	Top;
	int	Right;
	int	Bottom;

	/* Tab settings */
	int	ActiveTabPage;

	/* Options menu settings */
	BOOL	AlwaysOnTop;
	BOOL	MinimizeOnUse;
	BOOL	HideWhenMinimized;
	BOOL	Show16BitTasks;

	/* Update speed settings */
	/* How many half-seconds in between updates (i.e. 0 - Paused, 1 - High, 2 - Normal, 4 - Low) */
	DWORD	UpdateSpeed;

	/* Processes page settings */
	BOOL	ShowProcessesFromAllUsers; /* Server-only? */
	BOOL	Columns[COLUMN_NMAX];
	int		ColumnOrderArray[COLUMN_NMAX];
	int		ColumnSizeArray[COLUMN_NMAX];
	int		SortColumn;
	BOOL	SortAscending;

	/* Performance page settings */
	BOOL	CPUHistory_OneGraphPerCPU;
	BOOL	ShowKernelTimes;

} TASKMANAGER_SETTINGS, *LPTASKMANAGER_SETTINGS;

// Global Variables
extern HINSTANCE hInst;						/* current instance */
extern HWND	hMainWnd;					/* Main Window */
extern HWND	hStatusWnd;					/* Status Bar Window */
extern HWND	hTabWnd;					/* Tab Control Window */
extern int nMinimumWidth;				/* Minimum width of the dialog (OnSize()'s cx) */
extern int nMinimumHeight;				/* Minimum height of the dialog (OnSize()'s cy) */
extern int nOldWidth;					/* Holds the previous client area width */
extern int nOldHeight;					/* Holds the previous client area height */
extern TASKMANAGER_SETTINGS	TaskManagerSettings;

// Foward declarations of functions included in this code module:
INT_PTR CALLBACK TaskManagerWndProc(HWND, UINT, WPARAM, LPARAM);
BOOL OnCreate(HWND hWnd);
void OnSize(WPARAM nType, int cx, int cy);
void FillSolidRect(HDC hDC, LPCRECT lpRect, COLORREF clr);
void FillSolidRect2(HDC hDC, int x, int y, int cx, int cy, COLORREF clr);
void Draw3dRect(HDC hDC, int x, int y, int cx, int cy, COLORREF clrTopLeft, COLORREF clrBottomRight);
void Draw3dRect2(HDC hDC, LPRECT lpRect, COLORREF clrTopLeft, COLORREF clrBottomRight);
void LoadSettings(void);
void SaveSettings(void);
void TaskManager_OnRestoreMainWindow(void);
void TaskManager_OnEnterMenuLoop(HWND hWnd);
void TaskManager_OnExitMenuLoop(HWND hWnd);
void TaskManager_OnMenuSelect(HWND hWnd, UINT nItemID, UINT nFlags, HMENU hSysMenu);
void TaskManager_OnViewUpdateSpeed(DWORD);
void TaskManager_OnTabWndSelChange(void);
LPWSTR GetLastErrorText(LPWSTR lpszBuf, DWORD dwSize);

INT_PTR CALLBACK NetworkPageWndProc(HWND hDlg, UINT message, WPARAM wParam, LPARAM lParam);


// Sorting

typedef enum _PH_SORT_ORDER
{
    NoSortOrder = 0,
    AscendingSortOrder,
    DescendingSortOrder
} PH_SORT_ORDER, *PPH_SORT_ORDER;

VOID PhSetHeaderSortIcon(
    __in HWND hwnd,
    __in INT Index,
    __in PH_SORT_ORDER Order
    );