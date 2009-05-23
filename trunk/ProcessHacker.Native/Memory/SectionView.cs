/*
 * Process Hacker - 
 *   mapped view of section
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
using ProcessHacker.Native.Objects;

namespace ProcessHacker.Native
{
    /// <summary>
    /// Represents a mapped view of a section.
    /// </summary>
    public class SectionView : MemoryAlloc
    {
        internal SectionView(IntPtr baseAddress, IntPtr commitSize)
        {
            this.Memory = baseAddress;
            this.Size = commitSize.ToInt32();
        }

        protected override void Free()
        {
            NtStatus status;

            if ((status = Win32.NtUnmapViewOfSection(ProcessHandle.GetCurrent(), this)) >= NtStatus.Error)
                Win32.ThrowLastError(status);
        }

        public bool IsSameFile(SectionView mappedAsFile)
        {
            if ((uint)Win32.NtAreMappedFilesTheSame(this, mappedAsFile) == this.Memory.ToUInt32())
                return true;
            else
                return false;
        }

        public override void Resize(int newSize)
        {
            throw new NotSupportedException();
        }
    }
}
