/*
 * Process Hacker -
 *   Process Property Sheet Control
 *
 * Copyright (C) 2011 wj32
 * Copyright (C) 2011 dmex
 *
 * This file is part of Process Hacker.
 *
 * Process Hacker is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * Process Hacker is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Process Hacker.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Runtime.InteropServices;

using ProcessHacker.Native;
using ProcessHacker.Native.Api;

namespace ProcessHacker.Api
{
    public class ProcessPropertySheetPage : PropertySheetPage
    {
        private IntPtr _pageHandle;
        private IntPtr _dialogTemplate;
        private readonly DialogProc _dialogProc;
        private readonly PropSheetPageCallback _pagePageProc;

        public ProcessPropertySheetPage()
        {
            _dialogProc = this.DialogProc;
            _pagePageProc = this.PropPageProc;
        }

        protected override void Dispose(bool disposing)
        {
            MemoryAlloc.PrivateHeap.Free(this._dialogTemplate);

            base.Dispose(disposing);
        }

        public override IntPtr CreatePageHandle()
        {
            if (_pageHandle != IntPtr.Zero)
                return _pageHandle;

            PropSheetPageW psp = new PropSheetPageW();
            
            // *Must* be 260x260. See PhAddPropPageLayoutItem in procprp.c.
            _dialogTemplate = CreateDialogTemplate(260, 260, this.Text, 8, "MS Shell Dlg");

            psp.dwSize = PropSheetPageW.SizeOf;
            psp.dwFlags = PropSheetPageFlags.UseCallback | PropSheetPageFlags.DlgIndirect | PropSheetPageFlags.UseTitle | PropSheetPageFlags.DlgIndirect;
            
            psp.pszTitle = Marshal.StringToHGlobalUni("Details");

            psp.pResource = _dialogTemplate;

            psp.pfnDlgProc = Marshal.GetFunctionPointerForDelegate(_dialogProc);
            psp.pfnCallback = Marshal.GetFunctionPointerForDelegate(_pagePageProc);

            _pageHandle = Win32.CreatePropertySheetPageW(ref psp);

            return _pageHandle;
        }

        protected override bool DialogProc(IntPtr hwndDlg, WindowMessage uMsg, IntPtr wParam, IntPtr lParam)
        {
            switch (uMsg)
            {
                case WindowMessage.InitDialog:
                    {
                        Rect initialSize = new Rect
                        {
                            Left = 0,
                            Top = 0,
                            Right = 260,
                            Bottom = 260
                        };

                        Win32.MapDialogRect(hwndDlg, ref initialSize);

                        this.Size = new System.Drawing.Size(initialSize.Right, initialSize.Bottom);

                        this.Refresh();
                    }
                    break;
            }

            return base.DialogProc(hwndDlg, uMsg, wParam, lParam);
        }
    }
}
