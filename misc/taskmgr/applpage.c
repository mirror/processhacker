/*
 *  ReactOS Task Manager
 *
 *  applpage.c
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

static uintptr_t hApplicationThread = 0;
static UINT dwApplicationThread = 0;

static INT GetSystemColorDepth(VOID)
{
    DEVMODE pDevMode = { 0 };
    INT ColorDepth = 0;

    pDevMode.dmSize = sizeof(DEVMODE);
    pDevMode.dmDriverExtra = 0;

    if (!EnumDisplaySettings(NULL, ENUM_CURRENT_SETTINGS, &pDevMode))
        return ILC_COLOR;

    switch (pDevMode.dmBitsPerPel)
    {
        case 32: 
            ColorDepth = ILC_COLOR32; 
            break;
        case 24: 
            ColorDepth = ILC_COLOR24; 
            break;
        case 16: 
            ColorDepth = ILC_COLOR16; 
            break;
        case  8: 
            ColorDepth = ILC_COLOR8;  
            break;
        case  4: 
            ColorDepth = ILC_COLOR4;  
            break;
        default: 
            ColorDepth = ILC_COLOR;   
            break;
    }

    return ColorDepth;
}

INT_PTR CALLBACK ApplicationPageWndProc(HWND hDlg, UINT message, WPARAM wParam, LPARAM lParam)
{
    RECT       rc;
    int        nXDifference;
    int        nYDifference;
    LV_COLUMN  column;
    WCHAR      szTemp[256];
    int        cx, cy;

    switch (message) 
    {
    case WM_INITDIALOG:

        /* Save the width and height */
        GetClientRect(hDlg, &rc);
        nApplicationPageWidth = rc.right;
        nApplicationPageHeight = rc.bottom;

        // Get handles to the controls
        hApplicationPageListCtrl = GetDlgItem(hDlg, IDC_APPLIST);
        hApplicationPageEndTaskButton = GetDlgItem(hDlg, IDC_ENDTASK);
        hApplicationPageSwitchToButton = GetDlgItem(hDlg, IDC_SWITCHTO);
        hApplicationPageNewTaskButton = GetDlgItem(hDlg, IDC_NEWTASK);
     
        // Set the title.
        SetWindowText(hApplicationPageListCtrl, TEXT("Tasks"));

        // Set extended window styles for the list control.
        ListView_SetExtendedListViewStyle(
            hApplicationPageListCtrl, 
            ListView_GetExtendedListViewStyle(hApplicationPageListCtrl) | LVS_EX_FULLROWSELECT | LVS_EX_DOUBLEBUFFER | LVS_EX_HEADERDRAGDROP
            );

        /* Initialize the application page's controls */
        column.mask = LVCF_TEXT | LVCF_WIDTH;

        LoadString(hInst, IDS_TAB_TASK, szTemp, 256);
        column.pszText = szTemp;
        column.cx = 250;
       
        ListView_InsertColumn(hApplicationPageListCtrl, 0, &column);    /* Add the "Task" column */

        column.mask = LVCF_TEXT|LVCF_WIDTH;
        LoadString(hInst, IDS_TAB_STATUS, szTemp, 256);
        column.pszText = szTemp;
        column.cx = 95;

        ListView_InsertColumn(hApplicationPageListCtrl, 1, &column);    /* Add the "Status" column */

        ListView_SetImageList(hApplicationPageListCtrl, ImageList_Create(16, 16, GetSystemColorDepth() | ILC_MASK, 0, 1), LVSIL_SMALL);
        //ListView_SetImageList(hApplicationPageListCtrl, ImageList_Create(32, 32, GetSystemColorDepth() | ILC_MASK, 0, 1), LVSIL_NORMAL);

        UpdateApplicationListControlViewSetting();

        /* Start our refresh thread */
        hApplicationThread = _beginthreadex(NULL, 0, ApplicationPageRefreshThread, NULL, 0, &dwApplicationThread);
        return TRUE;
    case WM_COMMAND:

        /* Handle the button clicks */
        switch (LOWORD(wParam))
        {
        case IDC_ENDTASK:
            ApplicationPage_OnEndTask();
            break;
        case IDC_SWITCHTO:
            ApplicationPage_OnSwitchTo();
            break;
        case IDC_NEWTASK:
            SendMessage(hMainWnd, WM_COMMAND, MAKEWPARAM(ID_FILE_NEW, 0), 0);
            break;
        }

        break;

    case WM_SIZE:
        if (wParam == SIZE_MINIMIZED)
            return 0;

        cx = LOWORD(lParam);
        cy = HIWORD(lParam);
        nXDifference = cx - nApplicationPageWidth;
        nYDifference = cy - nApplicationPageHeight;
        nApplicationPageWidth = cx;
        nApplicationPageHeight = cy;

        /* Reposition the application page's controls */
        GetWindowRect(hApplicationPageListCtrl, &rc);
        cx = (rc.right - rc.left) + nXDifference;
        cy = (rc.bottom - rc.top) + nYDifference;
        SetWindowPos(hApplicationPageListCtrl, NULL, 0, 0, cx, cy, SWP_NOACTIVATE|SWP_NOOWNERZORDER|SWP_NOMOVE|SWP_NOZORDER);
        InvalidateRect(hApplicationPageListCtrl, NULL, TRUE);

        GetClientRect(hApplicationPageEndTaskButton, &rc);
        MapWindowPoints(hApplicationPageEndTaskButton, hDlg, (LPPOINT)(PRECT)(&rc), (sizeof(RECT)/sizeof(POINT)) );
        cx = rc.left + nXDifference;
        cy = rc.top + nYDifference;
        SetWindowPos(hApplicationPageEndTaskButton, NULL, cx, cy, 0, 0, SWP_NOACTIVATE|SWP_NOOWNERZORDER|SWP_NOSIZE|SWP_NOZORDER);
        InvalidateRect(hApplicationPageEndTaskButton, NULL, TRUE);

        GetClientRect(hApplicationPageSwitchToButton, &rc);
        MapWindowPoints(hApplicationPageSwitchToButton, hDlg, (LPPOINT)(PRECT)(&rc), (sizeof(RECT)/sizeof(POINT)) );
        cx = rc.left + nXDifference;
        cy = rc.top + nYDifference;
        SetWindowPos(hApplicationPageSwitchToButton, NULL, cx, cy, 0, 0, SWP_NOACTIVATE|SWP_NOOWNERZORDER|SWP_NOSIZE|SWP_NOZORDER);
        InvalidateRect(hApplicationPageSwitchToButton, NULL, TRUE);

        GetClientRect(hApplicationPageNewTaskButton, &rc);
        MapWindowPoints(hApplicationPageNewTaskButton, hDlg, (LPPOINT)(PRECT)(&rc), (sizeof(RECT)/sizeof(POINT)) );
        cx = rc.left + nXDifference;
        cy = rc.top + nYDifference;
        SetWindowPos(hApplicationPageNewTaskButton, NULL, cx, cy, 0, 0, SWP_NOACTIVATE|SWP_NOOWNERZORDER|SWP_NOSIZE|SWP_NOZORDER);
        InvalidateRect(hApplicationPageNewTaskButton, NULL, TRUE);

        break;

    case WM_NOTIFY:
        ApplicationPageOnNotify(wParam, lParam);
        break;

    case WM_KEYDOWN:
        if (wParam == VK_DELETE)
            ProcessPage_OnEndProcess();
        break;
    }

  return 0;
}

void UpdateApplicationListControlViewSetting(void)
{
    LONG_PTR dwStyle = GetWindowLongPtr(hApplicationPageListCtrl, GWL_STYLE);

    dwStyle &= ~(LVS_REPORT | LVS_ICON | LVS_LIST | LVS_SMALLICON);
	dwStyle |= LVS_REPORT; 

    SetWindowLongPtr(hApplicationPageListCtrl, GWL_STYLE, dwStyle);
}

UINT WINAPI ApplicationPageRefreshThread(void *lpParameter)
{
    MSG msg;
    INT i;
    BOOL                            bItemRemoved = FALSE;
    LV_ITEM                         item;
    LPAPPLICATION_PAGE_LIST_ITEM    pAPLI = NULL;
    HIMAGELIST                      hImageListLarge;
    HIMAGELIST                      hImageListSmall;

    /* If we couldn't create the event then exit the thread */
    while (TRUE)
    {
        noApps = TRUE;

        EnumDesktopWindows(NULL, EnumWindowsProc, 0);

        if (noApps)
            ListView_DeleteAllItems(hApplicationPageListCtrl);

        // Get the image lists
        hImageListLarge = ListView_GetImageList(hApplicationPageListCtrl, LVSIL_NORMAL);
        hImageListSmall = ListView_GetImageList(hApplicationPageListCtrl, LVSIL_SMALL);

        // Check to see if we need to remove any items from the list
        for (i =ListView_GetItemCount(hApplicationPageListCtrl)-1; i>=0; i--)
        {
            memset(&item, 0, sizeof(LV_ITEM));

            item.mask = LVIF_IMAGE | LVIF_PARAM;
            item.iItem = i;

            ListView_GetItem(hApplicationPageListCtrl, &item);

            pAPLI = (LPAPPLICATION_PAGE_LIST_ITEM)item.lParam;

            if (!IsWindow(pAPLI->hWnd)||
                (wcslen(pAPLI->szTitle) <= 0) ||
                !IsWindowVisible(pAPLI->hWnd) ||
                (GetParent(pAPLI->hWnd) != NULL) ||
                (GetWindow(pAPLI->hWnd, GW_OWNER) != NULL) ||
                (GetWindowLongPtr(pAPLI->hWnd, GWL_EXSTYLE) & WS_EX_TOOLWINDOW))
            {
                ImageList_Remove(hImageListLarge, item.iItem);
                ImageList_Remove(hImageListSmall, item.iItem);

                ListView_DeleteItem(hApplicationPageListCtrl, item.iItem);
                HeapFree(GetProcessHeap(), 0, pAPLI);
                bItemRemoved = TRUE;
            }
        }

        /*
        * If an item was removed from the list then
        * we need to resync all the items with the
        * image list
        */
        if (bItemRemoved)
        {
            for (i=0; i<ListView_GetItemCount(hApplicationPageListCtrl); i++)
            {
                memset(&item, 0, sizeof(LV_ITEM));
                item.mask = LVIF_IMAGE;
                item.iItem = i;
                item.iImage = i;
                (void)ListView_SetItem(hApplicationPageListCtrl, &item);
            }
            bItemRemoved = FALSE;
        }

        ApplicationPageUpdate();

        Sleep(1000);
    }
}

BOOL CALLBACK EnumWindowsProc(HWND hWnd, LPARAM lParam)
{
    HICON   hIcon;
    WCHAR   szText[MAX_PATH];
    BOOL    bLargeIcon;
    BOOL    bHung = FALSE;
    HICON*  xhIcon = (HICON*)&hIcon;

    /* Skip our window */
    if (hWnd == hMainWnd)
        return TRUE;

    bLargeIcon = FALSE;//(TaskManagerSettings.ViewMode == ID_VIEW_LARGE);

    GetWindowText(hWnd, szText, MAX_PATH); /* Get the window text */

    /* Check and see if this is a top-level app window */
    if ((wcslen(szText) <= 0) ||
        !IsWindowVisible(hWnd) ||
        (GetParent(hWnd) != NULL) ||
        (GetWindow(hWnd, GW_OWNER) != NULL) ||
        (GetWindowLongPtr(hWnd, GWL_EXSTYLE) & WS_EX_TOOLWINDOW))
    {
        return TRUE; /* Skip this window */
    }

    noApps = FALSE;
    /* Get the icon for this window */
    hIcon = NULL;
    SendMessageTimeout(hWnd, WM_GETICON,bLargeIcon ? ICON_BIG : ICON_SMALL, 0, 0, 1000, (PDWORD_PTR)xhIcon);

    if (!hIcon)
    {
        hIcon = (HICON)(LONG_PTR)GetClassLongPtr(hWnd, bLargeIcon ? -14 : -34);
        
        if (!hIcon) 
            hIcon = (HICON)(LONG_PTR)GetClassLongPtr(hWnd, bLargeIcon ? -34 : -14);
       
        if (!hIcon) 
            SendMessageTimeout(hWnd, WM_QUERYDRAGICON, 0, 0, 0, 1000, (PDWORD_PTR)xhIcon);

        if (!hIcon)
            SendMessageTimeout(hWnd, WM_GETICON, bLargeIcon ? ICON_SMALL : ICON_BIG, 0, 0, 1000, (PDWORD_PTR)xhIcon);
    }

    if (!hIcon)
        hIcon = LoadIcon(hInst, bLargeIcon ? MAKEINTRESOURCE(IDI_WINDOW) : MAKEINTRESOURCE(IDI_WINDOWSM));

    bHung = IsHungAppWindow(hWnd);

    AddOrUpdateHwnd(hWnd, szText, hIcon, bHung);

    return TRUE;
}

void AddOrUpdateHwnd(HWND hWnd, WCHAR *szTitle, HICON hIcon, BOOL bHung)
{
    LPAPPLICATION_PAGE_LIST_ITEM    pAPLI = NULL;
    //HIMAGELIST                      hImageListLarge;
    HIMAGELIST                      hImageListSmall;
    LV_ITEM                         item;
    int                             i;
    BOOL                            bAlreadyInList = FALSE;

    memset(&item, 0, sizeof(LV_ITEM));

    /* Get the image lists */
    //hImageListLarge = ListView_GetImageList(hApplicationPageListCtrl, LVSIL_NORMAL);
    hImageListSmall = ListView_GetImageList(hApplicationPageListCtrl, LVSIL_SMALL);

    /* Check to see if it's already in our list */
    for (i = 0; i < ListView_GetItemCount(hApplicationPageListCtrl); i++)
    {
        memset(&item, 0, sizeof(LV_ITEM));
        item.mask = LVIF_IMAGE|LVIF_PARAM;
        item.iItem = i;
        
        ListView_GetItem(hApplicationPageListCtrl, &item);

        pAPLI = (LPAPPLICATION_PAGE_LIST_ITEM)item.lParam;
        if (pAPLI->hWnd == hWnd)
        {
            bAlreadyInList = TRUE;
            break;
        }
    }

    /* If it is already in the list then update it if necessary */
    if (bAlreadyInList)
    {
        /* Check to see if anything needs updating */
        if ((pAPLI->hIcon != hIcon) || (_wcsicmp(pAPLI->szTitle, szTitle) != 0) || (pAPLI->bHung != bHung))
        {
            /* Update the structure */
            pAPLI->hIcon = hIcon;
            pAPLI->bHung = bHung;
           
            wcscpy_s(pAPLI->szTitle, MAX_PATH, szTitle);

            /* Update the image list */
            //ImageList_ReplaceIcon(hImageListLarge, item.iItem, hIcon);
            ImageList_ReplaceIcon(hImageListSmall, item.iItem, hIcon);

            /* Update the list view */
            ListView_RedrawItems(hApplicationPageListCtrl, 0, ListView_GetItemCount(hApplicationPageListCtrl));
            /* UpdateWindow(hApplicationPageListCtrl); */
            InvalidateRect(hApplicationPageListCtrl, NULL, 0);
        }
    }
    /* It is not already in the list so add it */
    else
    {
        pAPLI = (LPAPPLICATION_PAGE_LIST_ITEM)HeapAlloc(GetProcessHeap(), 0, sizeof(APPLICATION_PAGE_LIST_ITEM));

        pAPLI->hWnd = hWnd;
        pAPLI->hIcon = hIcon;
        pAPLI->bHung = bHung;
        
        wcscpy_s(pAPLI->szTitle, MAX_PATH, szTitle);

        /* Add the item to the list */
        memset(&item, 0, sizeof(LV_ITEM));
        item.mask = LVIF_TEXT | LVIF_IMAGE | LVIF_PARAM;
        item.iImage = ImageList_AddIcon(hImageListSmall, hIcon);
        item.iItem = ListView_GetItemCount(hApplicationPageListCtrl);
        item.pszText = LPSTR_TEXTCALLBACK;
        item.lParam = (LPARAM)pAPLI;
        
        ListView_InsertItem(hApplicationPageListCtrl, &item);
    }
    return;
}

void ApplicationPageUpdate(void)
{
    INT selectedCount = ListView_GetSelectedCount(hApplicationPageListCtrl);

    /* Enable or disable the "End Task" & "Switch To" buttons */
    if (selectedCount)
    {
        EnableWindow(hApplicationPageEndTaskButton, TRUE);
    }
    else
    {
        EnableWindow(hApplicationPageEndTaskButton, FALSE);
    }

    /* Enable "Switch To" button only if one app is selected */
    if (selectedCount == 1 )
    {
        EnableWindow(hApplicationPageSwitchToButton, TRUE);
    }
    else
    {
        EnableWindow(hApplicationPageSwitchToButton, FALSE);
    }

    /* If we are on the applications tab the windows menu will be */
    /* present on the menu bar so enable & disable the menu items */
    if (TabCtrl_GetCurSel(hTabWnd) == 0)
    {
        HMENU hMenu = GetMenu(hMainWnd);
        HMENU hWindowsMenu = GetSubMenu(hMenu, 3);

        /* Only one item selected */
        if (selectedCount == 1)
        {
            EnableMenuItem(hWindowsMenu, ID_WINDOWS_TILEHORIZONTALLY, MF_BYCOMMAND|MF_DISABLED|MF_GRAYED);
            EnableMenuItem(hWindowsMenu, ID_WINDOWS_TILEVERTICALLY, MF_BYCOMMAND|MF_DISABLED|MF_GRAYED);
            EnableMenuItem(hWindowsMenu, ID_WINDOWS_MINIMIZE, MF_BYCOMMAND|MF_ENABLED);
            EnableMenuItem(hWindowsMenu, ID_WINDOWS_MAXIMIZE, MF_BYCOMMAND|MF_ENABLED);
            EnableMenuItem(hWindowsMenu, ID_WINDOWS_CASCADE, MF_BYCOMMAND|MF_DISABLED|MF_GRAYED);
            EnableMenuItem(hWindowsMenu, ID_WINDOWS_BRINGTOFRONT, MF_BYCOMMAND|MF_ENABLED);
        }
        /* More than one item selected */
        else if (selectedCount > 1)
        {
            EnableMenuItem(hWindowsMenu, ID_WINDOWS_TILEHORIZONTALLY, MF_BYCOMMAND|MF_ENABLED);
            EnableMenuItem(hWindowsMenu, ID_WINDOWS_TILEVERTICALLY, MF_BYCOMMAND|MF_ENABLED);
            EnableMenuItem(hWindowsMenu, ID_WINDOWS_MINIMIZE, MF_BYCOMMAND|MF_ENABLED);
            EnableMenuItem(hWindowsMenu, ID_WINDOWS_MAXIMIZE, MF_BYCOMMAND|MF_ENABLED);
            EnableMenuItem(hWindowsMenu, ID_WINDOWS_CASCADE, MF_BYCOMMAND|MF_ENABLED);
            EnableMenuItem(hWindowsMenu, ID_WINDOWS_BRINGTOFRONT, MF_BYCOMMAND|MF_DISABLED|MF_GRAYED);
        }
        /* No items selected */
        else
        {
            EnableMenuItem(hWindowsMenu, ID_WINDOWS_TILEHORIZONTALLY, MF_BYCOMMAND|MF_DISABLED|MF_GRAYED);
            EnableMenuItem(hWindowsMenu, ID_WINDOWS_TILEVERTICALLY, MF_BYCOMMAND|MF_DISABLED|MF_GRAYED);
            EnableMenuItem(hWindowsMenu, ID_WINDOWS_MINIMIZE, MF_BYCOMMAND|MF_DISABLED|MF_GRAYED);
            EnableMenuItem(hWindowsMenu, ID_WINDOWS_MAXIMIZE, MF_BYCOMMAND|MF_DISABLED|MF_GRAYED);
            EnableMenuItem(hWindowsMenu, ID_WINDOWS_CASCADE, MF_BYCOMMAND|MF_DISABLED|MF_GRAYED);
            EnableMenuItem(hWindowsMenu, ID_WINDOWS_BRINGTOFRONT, MF_BYCOMMAND|MF_DISABLED|MF_GRAYED);
        }
    }
}

PH_SORT_ORDER bSortAscending = NoSortOrder;

void ApplicationPageOnNotify(WPARAM wParam, LPARAM lParam)
{
    int                           idctrl;
    LPNMHDR                       pnmh;
    LPNM_LISTVIEW                 pnmv;
    LV_DISPINFO*                  pnmdi;
    LPAPPLICATION_PAGE_LIST_ITEM  pAPLI;
    WCHAR                         szMsg[256];


    idctrl = (int) wParam;
    pnmh = (LPNMHDR) lParam;
    pnmv = (LPNM_LISTVIEW) lParam;
    pnmdi = (LV_DISPINFO*) lParam;

    if (pnmh->hwndFrom == hApplicationPageListCtrl) 
	{
        switch (pnmh->code) 
		{
        case LVN_ITEMCHANGED:
            ApplicationPageUpdate();
            break;
        case LVN_GETDISPINFO:
            {
                pAPLI = (LPAPPLICATION_PAGE_LIST_ITEM)pnmdi->item.lParam;
                /* Update the item text */
                if (pnmdi->item.iSubItem == 0)
                {
                    wcsncpy_s(pnmdi->item.pszText, MAX_PATH, pAPLI->szTitle, pnmdi->item.cchTextMax);
                }
                /* Update the item status */
                else if (pnmdi->item.iSubItem == 1)
                {
                    if (pAPLI->bHung)
                    {
                        LoadString(NULL, IDS_Not_Responding , szMsg, NUMBER_OF_ITEMS_IN_ARRAY(szMsg));
                    }
                    else
                    {
                        LoadString(NULL, IDS_Running, (LPWSTR) szMsg, NUMBER_OF_ITEMS_IN_ARRAY(szMsg));
                    }

                    wcsncpy_s(pnmdi->item.pszText, 256, szMsg, pnmdi->item.cchTextMax);
                }
            }
            break;
        case NM_RCLICK:

            if (ListView_GetSelectedCount(hApplicationPageListCtrl) < 1)
            {
                ApplicationPageShowContextMenu1();
            }
            else
            {
                ApplicationPageShowContextMenu2();
            }

            break;

        case NM_DBLCLK:

            ApplicationPage_OnSwitchTo();
            break;

        case LVN_KEYDOWN:

            if (((LPNMLVKEYDOWN)lParam)->wVKey == VK_DELETE)
                ApplicationPage_OnEndTask();

            break;

        }
    }
    else if (pnmh->hwndFrom == ListView_GetHeader(hApplicationPageListCtrl))
    {
        switch (pnmh->code)
        {
        case NM_RCLICK:
            {
                if (ListView_GetSelectedCount(hApplicationPageListCtrl) < 1)
                {
                    ApplicationPageShowContextMenu1();
                }
                else
                {
                    ApplicationPageShowContextMenu2();
                }
            }
            break;
        case HDN_ITEMCLICK:
            {
                HWND headerHandle = ListView_GetHeader(hApplicationPageListCtrl);
                LPNMHDR header = (LPNMHDR)lParam;
                LPNMHEADER header2 = (LPNMHEADER)header;

                //if (header2->iItem == context->SortColumn)
                //{
                //if (context->TriState)
                //{
                //if (bSortAscending == AscendingSortOrder)
                //bSortAscending = DescendingSortOrder;
                //else if (bSortAscending == DescendingSortOrder)
                //bSortAscending = NoSortOrder;
                //else
                //bSortAscending = AscendingSortOrder;
                //}
                //else
                //{
                if (bSortAscending == AscendingSortOrder)
                    bSortAscending = DescendingSortOrder;
                else
                    bSortAscending = AscendingSortOrder;
                //}
                //}
                //else
                //{
                //context->SortColumn = header2->iItem;
                //context->SortOrder = AscendingSortOrder;
                //}

                PhSetHeaderSortIcon(headerHandle, header2->iItem, bSortAscending);
                ListView_SortItems(hApplicationPageListCtrl, ApplicationPageCompareFunc, 0);
            }
            break;
        }

    }
}

/**
 * Visually indicates the sort order of a header control item.
 *
 * \param hwnd A handle to the header control.
 * \param Index The index of the item.
 * \param Order The sort order of the item.
 */
VOID PhSetHeaderSortIcon(
    __in HWND hwnd,
    __in INT Index,
    __in PH_SORT_ORDER Order
    )
{
    ULONG count;
    ULONG i;

    count = Header_GetItemCount(hwnd);

    if (count == -1)
        return;

    for (i = 0; i < count; i++)
    {
        HDITEM item;

        item.mask = HDI_FORMAT;
        Header_GetItem(hwnd, i, &item);

        if (Order != NoSortOrder && i == Index)
        {
            if (Order == AscendingSortOrder)
            {
                item.fmt &= ~HDF_SORTDOWN;
                item.fmt |= HDF_SORTUP;
            }
            else if (Order == DescendingSortOrder)
            {
                item.fmt &= ~HDF_SORTUP;
                item.fmt |= HDF_SORTDOWN;
            }
        }
        else
        {
            item.fmt &= ~(HDF_SORTDOWN | HDF_SORTUP);
        }

        Header_SetItem(hwnd, i, &item);
    }
}

void ApplicationPageShowContextMenu1(void)
{
    HMENU hMenu = NULL;
    HMENU hSubMenu = NULL;
    POINT pt;

    GetCursorPos(&pt);

    hMenu = LoadMenu(hInst, MAKEINTRESOURCE(IDR_APPLICATION_PAGE_CONTEXT1));
    hSubMenu = GetSubMenu(hMenu, 0);

    TrackPopupMenu(hSubMenu, TPM_LEFTALIGN | TPM_TOPALIGN | TPM_LEFTBUTTON, pt.x, pt.y, 0, hMainWnd, NULL);

    DestroyMenu(hMenu);
}

void ApplicationPageShowContextMenu2(void)
{
    HMENU hMenu = NULL, hSubMenu = NULL;
    POINT pt = { 0 };
    UINT selectedCount = 0;

    GetCursorPos(&pt);

    hMenu = LoadMenu(hInst, MAKEINTRESOURCE(IDR_APPLICATION_PAGE_CONTEXT2));
    hSubMenu = GetSubMenu(hMenu, 0);

    selectedCount = ListView_GetSelectedCount(hApplicationPageListCtrl);

    if (selectedCount == 1)
    {
        EnableMenuItem(hSubMenu, ID_WINDOWS_TILEHORIZONTALLY, MF_BYCOMMAND | MF_DISABLED | MF_GRAYED);
        EnableMenuItem(hSubMenu, ID_WINDOWS_TILEVERTICALLY, MF_BYCOMMAND | MF_DISABLED | MF_GRAYED);
        EnableMenuItem(hSubMenu, ID_WINDOWS_MINIMIZE, MF_BYCOMMAND | MF_ENABLED);
        EnableMenuItem(hSubMenu, ID_WINDOWS_MAXIMIZE, MF_BYCOMMAND | MF_ENABLED);
        EnableMenuItem(hSubMenu, ID_WINDOWS_CASCADE, MF_BYCOMMAND | MF_DISABLED | MF_GRAYED);
        EnableMenuItem(hSubMenu, ID_WINDOWS_BRINGTOFRONT, MF_BYCOMMAND | MF_ENABLED);
    }
    else if (selectedCount > 1)
    {
        EnableMenuItem(hSubMenu, ID_WINDOWS_TILEHORIZONTALLY, MF_BYCOMMAND | MF_ENABLED);
        EnableMenuItem(hSubMenu, ID_WINDOWS_TILEVERTICALLY, MF_BYCOMMAND | MF_ENABLED);
        EnableMenuItem(hSubMenu, ID_WINDOWS_MINIMIZE, MF_BYCOMMAND | MF_ENABLED);
        EnableMenuItem(hSubMenu, ID_WINDOWS_MAXIMIZE, MF_BYCOMMAND | MF_ENABLED);
        EnableMenuItem(hSubMenu, ID_WINDOWS_CASCADE, MF_BYCOMMAND | MF_ENABLED);
        EnableMenuItem(hSubMenu, ID_WINDOWS_BRINGTOFRONT, MF_BYCOMMAND | MF_DISABLED | MF_GRAYED);
    }
    else
    {
        EnableMenuItem(hSubMenu, ID_WINDOWS_TILEHORIZONTALLY, MF_BYCOMMAND | MF_DISABLED | MF_GRAYED);
        EnableMenuItem(hSubMenu, ID_WINDOWS_TILEVERTICALLY, MF_BYCOMMAND | MF_DISABLED | MF_GRAYED);
        EnableMenuItem(hSubMenu, ID_WINDOWS_MINIMIZE, MF_BYCOMMAND | MF_DISABLED | MF_GRAYED);
        EnableMenuItem(hSubMenu, ID_WINDOWS_MAXIMIZE, MF_BYCOMMAND | MF_DISABLED | MF_GRAYED);
        EnableMenuItem(hSubMenu, ID_WINDOWS_CASCADE, MF_BYCOMMAND | MF_DISABLED | MF_GRAYED);
        EnableMenuItem(hSubMenu, ID_WINDOWS_BRINGTOFRONT, MF_BYCOMMAND | MF_DISABLED | MF_GRAYED);
    }

    SetMenuDefaultItem(hSubMenu, ID_APPLICATION_PAGE_SWITCHTO, MF_BYCOMMAND);

    TrackPopupMenu(hSubMenu, TPM_LEFTALIGN | TPM_TOPALIGN | TPM_LEFTBUTTON, pt.x, pt.y, 0, hMainWnd, NULL);

    DestroyMenu(hMenu);
}

void ApplicationPage_OnWindowsTile(DWORD dwMode)
{
    LPAPPLICATION_PAGE_LIST_ITEM  pAPLI = NULL;
    LV_ITEM                       item;
    int                           i;
    HWND*                         hWndArray;
    int                           nWndCount;

    hWndArray = (HWND*)HeapAlloc(GetProcessHeap(), 0, sizeof(HWND) * ListView_GetItemCount(hApplicationPageListCtrl));
    nWndCount = 0;

    for (i = 0; i < ListView_GetItemCount(hApplicationPageListCtrl); i++) 
    {
        memset(&item, 0, sizeof(LV_ITEM));
        item.mask = LVIF_STATE|LVIF_PARAM;
        item.iItem = i;
        item.stateMask = (UINT)-1;
        
        ListView_GetItem(hApplicationPageListCtrl, &item);

        if (item.state & LVIS_SELECTED) 
        {
            pAPLI = (LPAPPLICATION_PAGE_LIST_ITEM)item.lParam;

            if (pAPLI) 
            {
                hWndArray[nWndCount] = pAPLI->hWnd;
                nWndCount++;
            }
        }
    }

    TileWindows(NULL, dwMode, NULL, nWndCount, hWndArray);
    HeapFree(GetProcessHeap(), 0, hWndArray);
}

void ApplicationPage_OnWindowsMinimize(void)
{
    LPAPPLICATION_PAGE_LIST_ITEM pAPLI = NULL;
    LV_ITEM item;
    int i = 0;

    for (i = 0; i < ListView_GetItemCount(hApplicationPageListCtrl); i++) 
    {
        memset(&item, 0, sizeof(LV_ITEM));
        item.mask = LVIF_STATE | LVIF_PARAM;
        item.iItem = i;
        item.stateMask = (UINT)-1;
        
        ListView_GetItem(hApplicationPageListCtrl, &item);

        if (item.state & LVIS_SELECTED) 
        {
            pAPLI = (LPAPPLICATION_PAGE_LIST_ITEM)item.lParam;
            
            if (pAPLI) 
                ShowWindow(pAPLI->hWnd, SW_MINIMIZE);
        }
    }
}

void ApplicationPage_OnWindowsMaximize(void)
{
    LPAPPLICATION_PAGE_LIST_ITEM  pAPLI = NULL;
    LV_ITEM                       item;
    int                           i;

    for (i = 0; i < ListView_GetItemCount(hApplicationPageListCtrl); i++) 
    {
        memset(&item, 0, sizeof(LV_ITEM));
        item.mask = LVIF_STATE | LVIF_PARAM;
        item.iItem = i;
        item.stateMask = (UINT)-1;
        
        ListView_GetItem(hApplicationPageListCtrl, &item);

        if (item.state & LVIS_SELECTED) 
        {
            pAPLI = (LPAPPLICATION_PAGE_LIST_ITEM)item.lParam;

            if (pAPLI) 
                ShowWindow(pAPLI->hWnd, SW_MAXIMIZE);
        }
    }
}

void ApplicationPage_OnWindowsCascade(void)
{
    LPAPPLICATION_PAGE_LIST_ITEM  pAPLI = NULL;
    LV_ITEM                       item;
    int                           i;
    HWND*                         hWndArray;
    int                           nWndCount;

    hWndArray = (HWND*)HeapAlloc(GetProcessHeap(), 0, sizeof(HWND) * ListView_GetItemCount(hApplicationPageListCtrl));
    nWndCount = 0;

    for (i=0; i<ListView_GetItemCount(hApplicationPageListCtrl); i++) 
    {
        memset(&item, 0, sizeof(LV_ITEM));
        item.mask = LVIF_STATE|LVIF_PARAM;
        item.iItem = i;
        item.stateMask = (UINT)-1;

        ListView_GetItem(hApplicationPageListCtrl, &item);

        if (item.state & LVIS_SELECTED) 
        {
            pAPLI = (LPAPPLICATION_PAGE_LIST_ITEM)item.lParam;

            if (pAPLI)
            {
                hWndArray[nWndCount] = pAPLI->hWnd;
                nWndCount++;
            }
        }
    }

    CascadeWindows(NULL, 0, NULL, nWndCount, hWndArray);
    HeapFree(GetProcessHeap(), 0, hWndArray);
}

void ApplicationPage_OnWindowsBringToFront(void)
{
    LPAPPLICATION_PAGE_LIST_ITEM  pAPLI = NULL;
    LV_ITEM                       item;
    int                           i;

    for (i = 0; i < ListView_GetItemCount(hApplicationPageListCtrl); i++) 
    {
        memset(&item, 0, sizeof(LV_ITEM));
        item.mask = LVIF_STATE|LVIF_PARAM;
        item.iItem = i;
        item.stateMask = (UINT)-1;

        ListView_GetItem(hApplicationPageListCtrl, &item);

        if (item.state & LVIS_SELECTED)
        {
            pAPLI = (LPAPPLICATION_PAGE_LIST_ITEM)item.lParam;
            break;
        }
    }

    if (pAPLI) 
    {
        if (IsIconic(pAPLI->hWnd))
            ShowWindow(pAPLI->hWnd, SW_RESTORE);

        BringWindowToTop(pAPLI->hWnd);
    }
}

void ApplicationPage_OnSwitchTo(void)
{
    LPAPPLICATION_PAGE_LIST_ITEM  pAPLI = NULL;
    LV_ITEM                       item;
    int                           i;

    for (i = 0; i < ListView_GetItemCount(hApplicationPageListCtrl); i++) 
    {
        memset(&item, 0, sizeof(LV_ITEM));
        item.mask = LVIF_STATE|LVIF_PARAM;
        item.iItem = i;
        item.stateMask = (UINT)-1;
        
        ListView_GetItem(hApplicationPageListCtrl, &item);

        if (item.state & LVIS_SELECTED)
        {
            pAPLI = (LPAPPLICATION_PAGE_LIST_ITEM)item.lParam;
            break;
        }
    }

    if (pAPLI) 
    {
        SwitchToThisWindow(pAPLI->hWnd, TRUE);
   
        if (TaskManagerSettings.MinimizeOnUse)
            ShowWindow(hMainWnd, SW_MINIMIZE);
    }
}

void ApplicationPage_OnEndTask(void)
{
    LPAPPLICATION_PAGE_LIST_ITEM  pAPLI = NULL;
    LV_ITEM                       item;
    int                           i;

    for (i = 0; i < ListView_GetItemCount(hApplicationPageListCtrl); i++) 
    {
        memset(&item, 0, sizeof(LV_ITEM));
        item.mask = LVIF_STATE|LVIF_PARAM;
        item.iItem = i;
        item.stateMask = (UINT)-1;
    
        ListView_GetItem(hApplicationPageListCtrl, &item);

        if (item.state & LVIS_SELECTED) 
        {
            pAPLI = (LPAPPLICATION_PAGE_LIST_ITEM)item.lParam;
           
            if (pAPLI) 
            {
                PostMessage(pAPLI->hWnd, WM_CLOSE, 0, 0);
            }
        }
    }
}

void ApplicationPage_OnGotoProcess(void)
{
    LPAPPLICATION_PAGE_LIST_ITEM  pAPLI = NULL;
    LV_ITEM                       item;
    int                           i;

    for (i = 0; i < ListView_GetItemCount(hApplicationPageListCtrl); i++)
    {
        memset(&item, 0, sizeof(LV_ITEM));
        item.mask = LVIF_STATE|LVIF_PARAM;
        item.iItem = i;
        item.stateMask = (UINT)-1;
        
        ListView_GetItem(hApplicationPageListCtrl, &item);

        if (item.state & LVIS_SELECTED)
        {
            pAPLI = (LPAPPLICATION_PAGE_LIST_ITEM)item.lParam;
            break;
        }
    }

    if (pAPLI) 
    {
        DWORD dwProcessId;

        GetWindowThreadProcessId(pAPLI->hWnd, &dwProcessId);
        /*
         * Switch to the process tab
         */
        TabCtrl_SetCurFocus(hTabWnd, 1);
        /*
         * Select the process item in the list
         */
        i = ProcGetIndexByProcessId(dwProcessId);

        if (i != -1)
        {
            ListView_SetItemState(
                hProcessPageListCtrl,         
                i,              
                LVIS_SELECTED | LVIS_FOCUSED,
                LVIS_SELECTED | LVIS_FOCUSED
                );
            
            ListView_EnsureVisible(
                hProcessPageListCtrl,                   
                i,                     
                FALSE
                );
        }
    }
}

int CALLBACK ApplicationPageCompareFunc(LPARAM lParam1, LPARAM lParam2, LPARAM lParamSort)
{
    LPAPPLICATION_PAGE_LIST_ITEM Param1;
    LPAPPLICATION_PAGE_LIST_ITEM Param2;

    if (bSortAscending == AscendingSortOrder) 
    {
        Param1 = (LPAPPLICATION_PAGE_LIST_ITEM)lParam1;
        Param2 = (LPAPPLICATION_PAGE_LIST_ITEM)lParam2;
    }
    else 
    {
        Param1 = (LPAPPLICATION_PAGE_LIST_ITEM)lParam2;
        Param2 = (LPAPPLICATION_PAGE_LIST_ITEM)lParam1;
    }

    return wcscmp(Param1->szTitle, Param2->szTitle);
}
