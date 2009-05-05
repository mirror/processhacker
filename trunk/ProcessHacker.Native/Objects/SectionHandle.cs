/*
 * Process Hacker - 
 *   section handle
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
using System.Text;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Security;

namespace ProcessHacker.Native.Objects
{
    public class SectionHandle : Win32Handle<SectionAccess>
    {
        public SectionHandle(SectionAccess access, FileHandle fileHandle, SectionAttributes sectionAttributes, MemoryProtection pageAttributes)
        {
            int status;
            IntPtr section;
            LargeInteger largeInteger = new LargeInteger();
            if ((status = Win32.NtCreateSection(
                out section,
                access,
                IntPtr.Zero,
                ref largeInteger,
                (int)pageAttributes,
                (int)sectionAttributes,
                fileHandle)) < 0)
                Win32.ThrowLastError(status);

            this.Handle = section;
        }

        private SectionHandle(IntPtr handle, bool owned)
            : base(handle, owned)
        { }
    }
}
