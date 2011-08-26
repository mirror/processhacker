/*
 *  ReactOS Task Manager
 *
 *  proclist.c
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

#include "taskmgr.h"

INT_PTR CALLBACK ProcessListWndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam);
WNDPROC OldProcessListWndProc;

INT_PTR CALLBACK ProcessListWndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
    HBRUSH  hbrBackground;
    RECT    rcItem;
    RECT    rcClip;
    HDC     hDC;
    int     DcSave;

    switch (message)
    {
    case WM_ERASEBKGND:

        /*
         * The list control produces a nasty flicker
         * when the user is resizing the window because
         * it erases the background to white, then
         * paints the list items over it.
         */

        return TRUE;
    }

    return CallWindowProc(OldProcessListWndProc, hWnd, message, wParam, lParam);
}
