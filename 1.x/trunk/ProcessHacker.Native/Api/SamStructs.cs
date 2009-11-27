/*
 * Process Hacker - 
 *   SAM structures
 *
 * Copyright (C) 2009 wj32
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
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ProcessHacker.Native.Api
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SamRidEnumeration
    {
        public int RelativeId;
        public UnicodeString Name;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SamSidEnumeration
    {
        public IntPtr Sid; // Sid*
        public UnicodeString Name;
    }
}
