/*
 * Process Hacker - 
 *   ProcessHacker Extended TreeView 
 * 
 * Copyright (C) 2009 dmex
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
 * 
 */

using System;
using System.Runtime.InteropServices;
using ProcessHacker.Native;
using ProcessHacker.Native.Api;

namespace ProcessHacker.Components
{
    public class ExtendedTreeView : System.Windows.Forms.TreeView
    {
        //http://www.danielmoth.com/Blog/2007/01/treeviewvista.html
        //http://www.danielmoth.com/Blog/2006/12/tvsexautohscroll.html

        private const int TV_FIRST = 0x1100;
        private const int TVM_SETEXTENDEDSTYLE = TV_FIRST + 44;
        private const int TVS_EX_AUTOHSCROLL = 0x0020; //autoscroll horizontaly
        private const int TVS_EX_FADEINOUTEXPANDOS = 0x0040; //auto hide the +/- signs

        protected override void OnHandleCreated(System.EventArgs e)
        {
            if (OSVersion.IsAboveOrEqual(WindowsVersion.Vista))
            {
                Win32.SendMessage(this.Handle, (WindowMessage)TVM_SETEXTENDEDSTYLE, 0, TVS_EX_FADEINOUTEXPANDOS);
                ProcessHacker.Common.PhUtils.SetTheme(this, "explorer");
            }

            base.OnHandleCreated(e);
        }
    }
}