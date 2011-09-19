/*
 *  ReactOS Task Manager
 *
 *  graphctl.c
 *
 *  Copyright (C) 2002  Robert Dickenson <robd@reactos.org>
 *
 *  This program is free software; you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation; either version 2 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License along
 *  with this program; if not, write to the Free Software Foundation, Inc.,
 *  51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
 */

#include "taskmgr.h"

WNDPROC OldGraphCtrlWndProc;

static void GraphCtrl_Init(TGraphCtrl* tGraph)
{
    int i;

    tGraph->m_hWnd = 0;
    tGraph->m_hParentWnd = 0;
    tGraph->m_dcGrid = 0;
    tGraph->m_dcPlot = 0;
    tGraph->m_bitmapOldGrid = 0;
    tGraph->m_bitmapOldPlot = 0;
    tGraph->m_bitmapGrid = 0;
    tGraph->m_bitmapPlot = 0;
    tGraph->m_brushBack = 0;

    tGraph->m_penPlot[0] = 0;
    tGraph->m_penPlot[1] = 0;
    tGraph->m_penPlot[2] = 0;
    tGraph->m_penPlot[3] = 0;

    /* since plotting is based on a LineTo for each new point
     * we need a starting point (i.e. a "previous" point)
     * use 0.0 as the default first point.
     * these are public member variables, and can be changed outside
     * (after construction).  Therefore m_perviousPosition could be set to
     * a more appropriate value prior to the first call to SetPosition.
     */
    tGraph->m_dPreviousPosition[0] = 0.0;
    tGraph->m_dPreviousPosition[1] = 0.0;
    tGraph->m_dPreviousPosition[2] = 0.0;
    tGraph->m_dPreviousPosition[3] = 0.0;

    /*  public variable for the number of decimal places on the y axis */
    tGraph->m_nYDecimals = 3;

    /*  set some initial values for the scaling until "SetRange" is called.
     *  these are protected varaibles and must be set with SetRange
     *  in order to ensure that m_dRange is updated accordingly
     */
    /*   m_dLowerLimit = -10.0; */
    /*   m_dUpperLimit =  10.0; */
    tGraph->m_dLowerLimit = 0.0;
    tGraph->m_dUpperLimit = 100.0;
    tGraph->m_dRange      =  tGraph->m_dUpperLimit - tGraph->m_dLowerLimit;   /*  protected member variable */

    /*  m_nShiftPixels determines how much the plot shifts (in terms of pixels)  */
    /*  with the addition of a new data point */
    tGraph->m_nShiftPixels     = 4;
    tGraph->m_nHalfShiftPixels = tGraph->m_nShiftPixels/2;                     /*  protected */
    tGraph->m_nPlotShiftPixels = tGraph->m_nShiftPixels + tGraph->m_nHalfShiftPixels;  /*  protected */

    /*  background, grid and data colors */
    /*  these are public variables and can be set directly */
    tGraph->m_crBackColor = RGB(0, 0, 0);  /*  see also SetBackgroundColor */
    tGraph->m_crGridColor = RGB(0, 128, 64);  /*  see also SetGridColor */
    tGraph->m_crPlotColor[0] = RGB(255, 255, 255);  /*  see also SetPlotColor */
    tGraph->m_crPlotColor[1] = RGB(100, 255, 255);  /*  see also SetPlotColor */
    tGraph->m_crPlotColor[2] = RGB(255, 100, 255);  /*  see also SetPlotColor */
    tGraph->m_crPlotColor[3] = RGB(255, 255, 100);  /*  see also SetPlotColor */

    /*  protected variables */
    for (i = 0; i < MAX_PLOTS; i++)
    {
        tGraph->m_penPlot[i] = CreatePen(PS_SOLID, 0, tGraph->m_crPlotColor[i]);
    }
    tGraph->m_brushBack = CreateSolidBrush(tGraph->m_crBackColor);

    /*  public member variables, can be set directly  */
    strcpy_s(tGraph->m_strXUnitsString, _countof("Samples"), "Samples");  /*  can also be set with SetXUnits */
    strcpy_s(tGraph->m_strYUnitsString, _countof("Samples"), "Samples");  /*  can also be set with SetYUnits */

    /*  protected bitmaps to restore the memory DC's */
    tGraph->m_bitmapOldGrid = NULL;
    tGraph->m_bitmapOldPlot = NULL;
}

void GraphCtrl_Dispose(TGraphCtrl* tGraph)
{
    int plot;

    for (plot = 0; plot < MAX_PLOTS; plot++)
		DeleteObject(tGraph->m_penPlot[plot]);

	/*  just to be picky restore the bitmaps for the two memory dc's */
	/*  (these dc's are being destroyed so there shouldn't be any leaks) */

	if (tGraph->m_bitmapOldGrid != NULL) 
		SelectObject(tGraph->m_dcGrid, tGraph->m_bitmapOldGrid);

	if (tGraph->m_bitmapOldPlot != NULL) 
		SelectObject(tGraph->m_dcPlot, tGraph->m_bitmapOldPlot);

	if (tGraph->m_bitmapGrid != NULL) 
		DeleteObject(tGraph->m_bitmapGrid);

	if (tGraph->m_bitmapPlot != NULL) 
		DeleteObject(tGraph->m_bitmapPlot);

	if (tGraph->m_dcGrid != NULL) 
		DeleteDC(tGraph->m_dcGrid);

	if (tGraph->m_dcPlot != NULL) 
		DeleteDC(tGraph->m_dcPlot);

	if (tGraph->m_brushBack != NULL) 
		DeleteObject(tGraph->m_brushBack);
}

BOOL GraphCtrl_Create(TGraphCtrl* tGraph, HWND hWnd, HWND hParentWnd, UINT nID)
{
    BOOL result = 0;

	UNREFERENCED_PARAMETER(nID);

    GraphCtrl_Init(tGraph);

    tGraph->m_hParentWnd = hParentWnd;
    tGraph->m_hWnd = hWnd;

    GraphCtrl_Resize(tGraph);

    if (result != 0)
        GraphCtrl_InvalidateCtrl(tGraph, FALSE);

    return result;
}

void GraphCtrl_SetRange(TGraphCtrl* tGraph, double dLower, double dUpper, int nDecimalPlaces)
{
    /* ASSERT(dUpper > dLower); */
    tGraph->m_dLowerLimit     = dLower;
    tGraph->m_dUpperLimit     = dUpper;
    tGraph->m_nYDecimals      = nDecimalPlaces;
    tGraph->m_dRange          = tGraph->m_dUpperLimit - tGraph->m_dLowerLimit;
    tGraph->m_dVerticalFactor = (double)tGraph->m_nPlotHeight / tGraph->m_dRange;
    /*  clear out the existing garbage, re-start with a clean plot */
    GraphCtrl_InvalidateCtrl(tGraph, FALSE);
}

#if 0
void TGraphCtrl::SetXUnits(const char* string)
{
    strncpy(m_strXUnitsString, string, sizeof(m_strXUnitsString) - 1);
    /*  clear out the existing garbage, re-start with a clean plot */
    InvalidateCtrl();
}

void TGraphCtrl::SetYUnits(const char* string)
{
    strncpy(m_strYUnitsString, string, sizeof(m_strYUnitsString) - 1);
    /*  clear out the existing garbage, re-start with a clean plot */
    InvalidateCtrl();
}
#endif

void GraphCtrl_SetGridColor(TGraphCtrl* tGraph, COLORREF color)
{
    tGraph->m_crGridColor = color;
    /*  clear out the existing garbage, re-start with a clean plot */
    GraphCtrl_InvalidateCtrl(tGraph, FALSE);
}

void GraphCtrl_SetPlotColor(TGraphCtrl* tGraph, int plot, COLORREF color)
{
    tGraph->m_crPlotColor[plot] = color;
    DeleteObject(tGraph->m_penPlot[plot]);
    tGraph->m_penPlot[plot] = CreatePen(PS_SOLID, 0, tGraph->m_crPlotColor[plot]);
    /*  clear out the existing garbage, re-start with a clean plot */
    GraphCtrl_InvalidateCtrl(tGraph, FALSE);
}

void GraphCtrl_SetBackgroundColor(TGraphCtrl* tGraph, COLORREF color)
{
    tGraph->m_crBackColor = color;
    DeleteObject(tGraph->m_brushBack);
    tGraph->m_brushBack = CreateSolidBrush(tGraph->m_crBackColor);
    /*  clear out the existing garbage, re-start with a clean plot */
    GraphCtrl_InvalidateCtrl(tGraph, FALSE);
}

void GraphCtrl_InvalidateCtrl(TGraphCtrl* tGraph, BOOL bResize)
{
    /*  There is a lot of drawing going on here - particularly in terms of  */
    /*  drawing the grid.  Don't panic, this is all being drawn (only once) */
    /*  to a bitmap.  The result is then BitBlt'd to the control whenever needed. */
    int i;
    int nCharacters;
    //int nTopGridPix, nMidGridPix, nBottomGridPix;

    HPEN oldPen;
    HPEN solidPen = CreatePen(PS_SOLID, 0, tGraph->m_crGridColor);
    /* HFONT axisFont, yUnitFont, oldFont; */
    /* char strTemp[50]; */

    /*  in case we haven't established the memory dc's */
    /* CClientDC dc(tGraph); */
    HDC dc = GetDC(tGraph->m_hParentWnd);

    /*  if we don't have one yet, set up a memory dc for the grid */
    if (tGraph->m_dcGrid == NULL)
    {
        tGraph->m_dcGrid = CreateCompatibleDC(dc);
        tGraph->m_bitmapGrid = CreateCompatibleBitmap(dc, tGraph->m_nClientWidth, tGraph->m_nClientHeight);
        tGraph->m_bitmapOldGrid = (HBITMAP)SelectObject(tGraph->m_dcGrid, tGraph->m_bitmapGrid);
    }
    else if(bResize)
    {
        // the size of the drawing area has changed
        // so create a new bitmap of the appropriate size
        if(tGraph->m_bitmapGrid != NULL)
        {
            tGraph->m_bitmapGrid = (HBITMAP)SelectObject(tGraph->m_dcGrid, tGraph->m_bitmapOldGrid);
            DeleteObject(tGraph->m_bitmapGrid);
            tGraph->m_bitmapGrid = CreateCompatibleBitmap(dc, tGraph->m_nClientWidth, tGraph->m_nClientHeight);
            SelectObject(tGraph->m_dcGrid, tGraph->m_bitmapGrid);
        }
    }

    SetBkColor(tGraph->m_dcGrid, tGraph->m_crBackColor);

    /*  fill the grid background */
    FillRect(tGraph->m_dcGrid, &tGraph->m_rectClient, tGraph->m_brushBack);

    /*  draw the plot rectangle: */
    /*  determine how wide the y axis scaling values are */
    nCharacters = abs((int)log10(fabs(tGraph->m_dUpperLimit)));
    nCharacters = max(nCharacters, abs((int)log10(fabs(tGraph->m_dLowerLimit))));

    /*  add the units digit, decimal point and a minus sign, and an extra space */
    /*  as well as the number of decimal places to display */
    nCharacters = nCharacters + 4 + tGraph->m_nYDecimals;

    /*  adjust the plot rectangle dimensions */
    /*  assume 6 pixels per character (this may need to be adjusted) */
    /*   m_rectPlot.left = m_rectClient.left + 6*(nCharacters); */
    tGraph->m_rectPlot.left = tGraph->m_rectClient.left;
    tGraph->m_nPlotWidth    = tGraph->m_rectPlot.right - tGraph->m_rectPlot.left;/* m_rectPlot.Width(); */

    /*  draw the plot rectangle */
    oldPen = (HPEN)SelectObject(tGraph->m_dcGrid, solidPen);
    MoveToEx(tGraph->m_dcGrid, tGraph->m_rectPlot.left, tGraph->m_rectPlot.top, NULL);
    LineTo(tGraph->m_dcGrid, tGraph->m_rectPlot.right+1, tGraph->m_rectPlot.top);
    LineTo(tGraph->m_dcGrid, tGraph->m_rectPlot.right+1, tGraph->m_rectPlot.bottom+1);
    LineTo(tGraph->m_dcGrid, tGraph->m_rectPlot.left, tGraph->m_rectPlot.bottom+1);
    /*   LineTo(m_dcGrid, m_rectPlot.left, m_rectPlot.top); */

    /*  draw the horizontal axis */
    for (i = tGraph->m_rectPlot.top; i < tGraph->m_rectPlot.bottom; i += 12)
    {
        MoveToEx(tGraph->m_dcGrid, tGraph->m_rectPlot.left, tGraph->m_rectPlot.top + i, NULL);
        LineTo(tGraph->m_dcGrid, tGraph->m_rectPlot.right, tGraph->m_rectPlot.top + i);
    }

    /*  draw the vertical axis */
    for (i = tGraph->m_rectPlot.left; i < tGraph->m_rectPlot.right; i += 12)
    {
        MoveToEx(tGraph->m_dcGrid, tGraph->m_rectPlot.left + i, tGraph->m_rectPlot.bottom, NULL);
        LineTo(tGraph->m_dcGrid, tGraph->m_rectPlot.left + i, tGraph->m_rectPlot.top);
    }

    SelectObject(tGraph->m_dcGrid, oldPen);
    DeleteObject(solidPen);

#if 0
    /*  create some fonts (horizontal and vertical) */
    /*  use a height of 14 pixels and 300 weight  */
    /*  (these may need to be adjusted depending on the display) */
    axisFont = CreateFont (14, 0, 0, 0, 300,
                           FALSE, FALSE, 0, ANSI_CHARSET,
                           OUT_DEFAULT_PRECIS,
                           CLIP_DEFAULT_PRECIS,
                           DEFAULT_QUALITY,
                           DEFAULT_PITCH|FF_SWISS, "Arial");
    yUnitFont = CreateFont (14, 0, 900, 0, 300,
                            FALSE, FALSE, 0, ANSI_CHARSET,
                            OUT_DEFAULT_PRECIS,
                            CLIP_DEFAULT_PRECIS,
                            DEFAULT_QUALITY,
                            DEFAULT_PITCH|FF_SWISS, "Arial");

    /*  grab the horizontal font */
    oldFont = (HFONT)SelectObject(m_dcGrid, axisFont);

    /*  y max */
    SetTextColor(m_dcGrid, m_crGridColor);
    SetTextAlign(m_dcGrid, TA_RIGHT|TA_TOP);
    sprintf(strTemp, "%.*lf", m_nYDecimals, m_dUpperLimit);
    TextOut(m_dcGrid, m_rectPlot.left-4, m_rectPlot.top, strTemp, wcslen(strTemp));

    /*  y min */
    SetTextAlign(m_dcGrid, TA_RIGHT|TA_BASELINE);
    sprintf(strTemp, "%.*lf", m_nYDecimals, m_dLowerLimit);
    TextOut(m_dcGrid, m_rectPlot.left-4, m_rectPlot.bottom, strTemp, wcslen(strTemp));

    /*  x min */
    SetTextAlign(m_dcGrid, TA_LEFT|TA_TOP);
    TextOut(m_dcGrid, m_rectPlot.left, m_rectPlot.bottom+4, "0", 1);

    /*  x max */
    SetTextAlign(m_dcGrid, TA_RIGHT|TA_TOP);
    sprintf(strTemp, "%d", m_nPlotWidth/m_nShiftPixels);
    TextOut(m_dcGrid, m_rectPlot.right, m_rectPlot.bottom+4, strTemp, wcslen(strTemp));

    /*  x units */
    SetTextAlign(m_dcGrid, TA_CENTER|TA_TOP);
    TextOut(m_dcGrid, (m_rectPlot.left+m_rectPlot.right)/2,
            m_rectPlot.bottom+4, m_strXUnitsString, wcslen(m_strXUnitsString));

    /*  restore the font */
    SelectObject(m_dcGrid, oldFont);

    /*  y units */
    oldFont = (HFONT)SelectObject(m_dcGrid, yUnitFont);
    SetTextAlign(m_dcGrid, TA_CENTER|TA_BASELINE);
    TextOut(m_dcGrid, (m_rectClient.left+m_rectPlot.left)/2,
            (m_rectPlot.bottom+m_rectPlot.top)/2, m_strYUnitsString, wcslen(m_strYUnitsString));
    SelectObject(m_dcGrid, oldFont);
#endif
    /*  at this point we are done filling the the grid bitmap,  */
    /*  no more drawing to this bitmap is needed until the setting are changed */

    /*  if we don't have one yet, set up a memory dc for the plot */
    if (tGraph->m_dcPlot == NULL)
    {
        tGraph->m_dcPlot = CreateCompatibleDC(dc);
        tGraph->m_bitmapPlot = CreateCompatibleBitmap(dc, tGraph->m_nClientWidth, tGraph->m_nClientHeight);
        tGraph->m_bitmapOldPlot = (HBITMAP)SelectObject(tGraph->m_dcPlot, tGraph->m_bitmapPlot);
    }
    else if(bResize)
    {
        // the size of the drawing area has changed
        // so create a new bitmap of the appropriate size
        if(tGraph->m_bitmapPlot != NULL)
        {
            tGraph->m_bitmapPlot = (HBITMAP)SelectObject(tGraph->m_dcPlot, tGraph->m_bitmapOldPlot);
            DeleteObject(tGraph->m_bitmapPlot);
            tGraph->m_bitmapPlot = CreateCompatibleBitmap(dc, tGraph->m_nClientWidth, tGraph->m_nClientHeight);
            SelectObject(tGraph->m_dcPlot, tGraph->m_bitmapPlot);
        }
    }

    /*  make sure the plot bitmap is cleared */
    SetBkColor(tGraph->m_dcPlot, tGraph->m_crBackColor);
    FillRect(tGraph->m_dcPlot, &tGraph->m_rectClient, tGraph->m_brushBack);

    /*  finally, force the plot area to redraw */
    InvalidateRect(tGraph->m_hParentWnd, &tGraph->m_rectClient, TRUE);
    ReleaseDC(tGraph->m_hParentWnd, dc);
}

double GraphCtrl_AppendPoint(TGraphCtrl* tGraph, double dNewPoint0, double dNewPoint1, double dNewPoint2, double dNewPoint3)
{
    /*  append a data point to the plot & return the previous point */
    double dPrevious;

    dPrevious = tGraph->m_dCurrentPosition[0];
    tGraph->m_dCurrentPosition[0] = dNewPoint0;
    tGraph->m_dCurrentPosition[1] = dNewPoint1;
    tGraph->m_dCurrentPosition[2] = dNewPoint2;
    tGraph->m_dCurrentPosition[3] = dNewPoint3;
    
	GraphCtrl_DrawPoint(tGraph);

    return dPrevious;
}

void GraphCtrl_Paint(TGraphCtrl* tGraph, HWND hWnd, HDC dc)
{
    HDC memDC;
    HBITMAP memBitmap;
    HBITMAP oldBitmap; /*  bitmap originally found in CMemDC */

	UNREFERENCED_PARAMETER(hWnd);

/*   RECT rcClient; */
/*   GetClientRect(hWnd, &rcClient); */
/*   FillSolidRect(dc, &rcClient, RGB(255, 0, 255)); */
/*   m_nClientWidth = rcClient.right - rcClient.left; */
/*   m_nClientHeight = rcClient.bottom - rcClient.top; */

    /*  no real plotting work is performed here,  */
    /*  just putting the existing bitmaps on the client */

    /*  to avoid flicker, establish a memory dc, draw to it */
    /*  and then BitBlt it to the client */
    memDC = CreateCompatibleDC(dc);
    memBitmap = (HBITMAP)CreateCompatibleBitmap(dc, tGraph->m_nClientWidth, tGraph->m_nClientHeight);
    oldBitmap = (HBITMAP)SelectObject(memDC, memBitmap);

    if (memDC != NULL)
    {
        /*  first drop the grid on the memory dc */
        BitBlt(memDC, 0, 0, tGraph->m_nClientWidth, tGraph->m_nClientHeight, tGraph->m_dcGrid, 0, 0, SRCCOPY);
        /*  now add the plot on top as a "pattern" via SRCPAINT. */
        /*  works well with dark background and a light plot */
        BitBlt(memDC, 0, 0, tGraph->m_nClientWidth, tGraph->m_nClientHeight, tGraph->m_dcPlot, 0, 0, SRCPAINT);  /* SRCPAINT */
        /*  finally send the result to the display */
        BitBlt(dc, 0, 0, tGraph->m_nClientWidth, tGraph->m_nClientHeight, memDC, 0, 0, SRCCOPY);

        SelectObject(memDC, oldBitmap);
        DeleteDC(memDC);
    }

    DeleteObject(memBitmap);
}

void GraphCtrl_DrawPoint(TGraphCtrl* tGraph)
{
    /*  this does the work of "scrolling" the plot to the left
     *  and appending a new data point all of the plotting is
     *  directed to the memory based bitmap associated with m_dcPlot
     *  the will subsequently be BitBlt'd to the client in Paint
     */
    int currX, prevX, currY, prevY;
    HPEN oldPen;
    RECT rectCleanUp;
    int i;

    if (tGraph->m_dcPlot != NULL)
    {
        /*  shift the plot by BitBlt'ing it to itself
         *  note: the m_dcPlot covers the entire client
         *        but we only shift bitmap that is the size
         *        of the plot rectangle
         *  grab the right side of the plot (excluding m_nShiftPixels on the left)
         *  move this grabbed bitmap to the left by m_nShiftPixels
         */
        BitBlt(tGraph->m_dcPlot, tGraph->m_rectPlot.left, tGraph->m_rectPlot.top+1,
               tGraph->m_nPlotWidth, tGraph->m_nPlotHeight, tGraph->m_dcPlot,
               tGraph->m_rectPlot.left+tGraph->m_nShiftPixels, tGraph->m_rectPlot.top+1,
               SRCCOPY);

        /*  establish a rectangle over the right side of plot */
        /*  which now needs to be cleaned up proir to adding the new point */
        rectCleanUp = tGraph->m_rectPlot;
        rectCleanUp.left  = rectCleanUp.right - tGraph->m_nShiftPixels;

        /*  fill the cleanup area with the background */
        FillRect(tGraph->m_dcPlot, &rectCleanUp, tGraph->m_brushBack);

        /*  draw the next line segement */
        for (i = 0; i < MAX_PLOTS; i++)
        {
            /*  grab the plotting pen */
            oldPen = (HPEN)SelectObject(tGraph->m_dcPlot, tGraph->m_penPlot[i]);

            /*  move to the previous point */
            prevX = tGraph->m_rectPlot.right - tGraph->m_nPlotShiftPixels;
            prevY = tGraph->m_rectPlot.bottom - (long)((tGraph->m_dPreviousPosition[i] - tGraph->m_dLowerLimit) * tGraph->m_dVerticalFactor);
            
			MoveToEx(tGraph->m_dcPlot, prevX, prevY, NULL);

            /*  draw to the current point */
            currX = tGraph->m_rectPlot.right - tGraph->m_nHalfShiftPixels;
            currY = tGraph->m_rectPlot.bottom - (long)((tGraph->m_dCurrentPosition[i] - tGraph->m_dLowerLimit) * tGraph->m_dVerticalFactor);
            LineTo(tGraph->m_dcPlot, currX, currY);

            /*  Restore the pen  */
            SelectObject(tGraph->m_dcPlot, oldPen);

            /*  if the data leaks over the upper or lower plot boundaries
             *  fill the upper and lower leakage with the background
             *  this will facilitate clipping on an as needed basis
             *  as opposed to always calling IntersectClipRect
             */
            if ((prevY <= tGraph->m_rectPlot.top) || (currY <= tGraph->m_rectPlot.top))
            {
                RECT rc;
                rc.bottom = tGraph->m_rectPlot.top+1;
                rc.left = prevX;
                rc.right = currX+1;
                rc.top = tGraph->m_rectClient.top;
                FillRect(tGraph->m_dcPlot, &rc, tGraph->m_brushBack);
            }
            if ((prevY >= tGraph->m_rectPlot.bottom) || (currY >= tGraph->m_rectPlot.bottom))
            {
                RECT rc;
                rc.bottom = tGraph->m_rectClient.bottom+1;
                rc.left = prevX;
                rc.right = currX+1;
                rc.top = tGraph->m_rectPlot.bottom+1;
                /* RECT rc(prevX, m_rectPlot.bottom+1, currX+1, m_rectClient.bottom+1); */
                FillRect(tGraph->m_dcPlot, &rc, tGraph->m_brushBack);
            }

            /*  store the current point for connection to the next point */
            tGraph->m_dPreviousPosition[i] = tGraph->m_dCurrentPosition[i];
        }
    }
}

void GraphCtrl_Resize(TGraphCtrl* tGraph)
{
    /*  NOTE: Resize automatically gets called during the setup of the control */
    GetClientRect(tGraph->m_hWnd, &tGraph->m_rectClient);

    /*  set some member variables to avoid multiple function calls */
    tGraph->m_nClientHeight = tGraph->m_rectClient.bottom - tGraph->m_rectClient.top;/* m_rectClient.Height(); */
    tGraph->m_nClientWidth  = tGraph->m_rectClient.right - tGraph->m_rectClient.left;/* m_rectClient.Width(); */

    /*  the "left" coordinate and "width" will be modified in  */
    /*  InvalidateCtrl to be based on the width of the y axis scaling */
#if 0
    tGraph->m_rectPlot.left   = 20;
    tGraph->m_rectPlot.top    = 10;
    tGraph->m_rectPlot.right  = tGraph->m_rectClient.right-10;
    tGraph->m_rectPlot.bottom = tGraph->m_rectClient.bottom-25;
#else
    tGraph->m_rectPlot.left   = 0;
    tGraph->m_rectPlot.top    = -1;
    tGraph->m_rectPlot.right  = tGraph->m_rectClient.right-0;
    tGraph->m_rectPlot.bottom = tGraph->m_rectClient.bottom-0;
#endif

    /*  set some member variables to avoid multiple function calls */
    tGraph->m_nPlotHeight = tGraph->m_rectPlot.bottom - tGraph->m_rectPlot.top;/* m_rectPlot.Height(); */
    tGraph->m_nPlotWidth  = tGraph->m_rectPlot.right - tGraph->m_rectPlot.left;/* m_rectPlot.Width(); */

    /*  set the scaling factor for now, this can be adjusted  */
    /*  in the SetRange functions */
    tGraph->m_dVerticalFactor = (double)tGraph->m_nPlotHeight / tGraph->m_dRange;
}

extern TGraphCtrl PerformancePageCpuUsageHistoryGraph;
extern TGraphCtrl PerformancePageMemUsageHistoryGraph;
extern HWND hPerformancePageCpuUsageHistoryGraph;
extern HWND hPerformancePageMemUsageHistoryGraph;

INT_PTR CALLBACK
GraphCtrl_WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
    RECT        rcClient;
    HDC            hdc;
    PAINTSTRUCT     ps;

    switch (message)
    {
    case WM_ERASEBKGND:
        return TRUE;
    /*
     *  Filter out mouse  & keyboard messages
     */
    /* case WM_APPCOMMAND: */
    case WM_CAPTURECHANGED:
    case WM_LBUTTONDBLCLK:
    case WM_LBUTTONDOWN:
    case WM_LBUTTONUP:
    case WM_MBUTTONDBLCLK:
    case WM_MBUTTONDOWN:
    case WM_MBUTTONUP:
    case WM_MOUSEACTIVATE:
    case WM_MOUSEHOVER:
    case WM_MOUSELEAVE:
    case WM_MOUSEMOVE:
    /* case WM_MOUSEWHEEL: */
    case WM_NCHITTEST:
    case WM_NCLBUTTONDBLCLK:
    case WM_NCLBUTTONDOWN:
    case WM_NCLBUTTONUP:
    case WM_NCMBUTTONDBLCLK:
    case WM_NCMBUTTONDOWN:
    case WM_NCMBUTTONUP:
    /* case WM_NCMOUSEHOVER: */
    /* case WM_NCMOUSELEAVE: */
    case WM_NCMOUSEMOVE:
    case WM_NCRBUTTONDBLCLK:
    case WM_NCRBUTTONDOWN:
    case WM_NCRBUTTONUP:
    /* case WM_NCXBUTTONDBLCLK: */
    /* case WM_NCXBUTTONDOWN: */
    /* case WM_NCXBUTTONUP: */
    case WM_RBUTTONDBLCLK:
    case WM_RBUTTONDOWN:
    case WM_RBUTTONUP:
    /* case WM_XBUTTONDBLCLK: */
    /* case WM_XBUTTONDOWN: */
    /* case WM_XBUTTONUP: */
    case WM_ACTIVATE:
    case WM_CHAR:
    case WM_DEADCHAR:
    case WM_GETHOTKEY:
    case WM_HOTKEY:
    case WM_KEYDOWN:
    case WM_KEYUP:
    case WM_KILLFOCUS:
    case WM_SETFOCUS:
    case WM_SETHOTKEY:
    case WM_SYSCHAR:
    case WM_SYSDEADCHAR:
    case WM_SYSKEYDOWN:
    case WM_SYSKEYUP:
        return 0;

    case WM_NCCALCSIZE:
        return 0;

    case WM_SIZE:
        if (hWnd == hPerformancePageMemUsageHistoryGraph)
        {
            GraphCtrl_Resize(&PerformancePageMemUsageHistoryGraph);
            GraphCtrl_InvalidateCtrl(&PerformancePageMemUsageHistoryGraph, TRUE);
        }
        if (hWnd == hPerformancePageCpuUsageHistoryGraph)
        {
            GraphCtrl_Resize(&PerformancePageCpuUsageHistoryGraph);
            GraphCtrl_InvalidateCtrl(&PerformancePageCpuUsageHistoryGraph, TRUE);
        }
        return 0;

    case WM_PAINT:
        hdc = BeginPaint(hWnd, &ps);
        GetClientRect(hWnd, &rcClient);
        if (hWnd == hPerformancePageMemUsageHistoryGraph)
            GraphCtrl_Paint(&PerformancePageMemUsageHistoryGraph, hWnd, hdc);
        if (hWnd == hPerformancePageCpuUsageHistoryGraph)
            GraphCtrl_Paint(&PerformancePageCpuUsageHistoryGraph, hWnd, hdc);
        EndPaint(hWnd, &ps);
        return 0;
    }

    // We pass on all non-handled messages
    return CallWindowProc(OldGraphCtrlWndProc, hWnd, message, wParam, lParam);
}
