/*
 * Process Hacker - 
 *   section flags
 * 
 * Copyright (C) 2008 wj32
 * Descriptions from the PE/COFF specification from Microsoft.
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

namespace ProcessHacker.PE
{
    /// <summary>
    /// Specifies the attributes of an image section.
    /// </summary>
    [Flags]
    public enum SectionFlags : uint
    {
        /// <summary>
        /// Reserved for future use.
        /// </summary>
        Reserved1 = 0x00000000,

        /// <summary>
        /// Reserved for future use.
        /// </summary>
        Reserved2 = 0x00000001,

        /// <summary>
        /// Reserved for future use.
        /// </summary>
        Reserved3 = 0x00000002,

        /// <summary>
        /// Reserved, must be zero.
        /// </summary>
        Reserved4 = 0x00000004,

        /// <summary>
        /// The section should not be padded to the next boundary. 
        /// This flag is obsolete and is replaced by IMAGE_SCN_ALIGN_1BYTES. 
        /// This is valid only for object files.
        /// </summary>
        NoPad = 0x00000008,

        /// <summary>
        /// The section contains executable code.
        /// </summary>
        Code = 0x00000020,

        /// <summary>
        /// The section contains initialized data.
        /// </summary>
        InitializedData = 0x00000040,

        /// <summary>
        /// The section contains uninitialized data.
        /// </summary>
        UninitializedData = 0x00000080,

        /// <summary>
        /// Reserved for future use.
        /// </summary>
        Other = 0x00000100,

        /// <summary>
        /// The section contains comments or other information. The 
        /// .drectve section has this type. This is valid for object 
        /// files only.
        /// </summary>
        Info = 0x00000200,

        /// <summary>
        /// Reserved for future use.
        /// </summary>
        Reserved5 = 0x00000400,

        /// <summary>
        /// The section will not become part of the image. This is valid 
        /// only for object files.
        /// </summary>
        Remove = 0x00000800,

        /// <summary>
        /// The section contains COMDAT data. 
        /// </summary>
        COMDAT = 0x00001000,

        /// <summary>
        /// The section contains data referenced through the global pointer (GP).
        /// </summary>
        GPRel = 0x00008000,

        /// <summary>
        /// Reserved for future use.
        /// </summary>
        MemoryPurgeable = 0x00010000,

        /// <summary>
        /// Reserved for future use.
        /// </summary>
        Memory16Bit = 0x00020000,

        /// <summary>
        /// Reserved for future use.
        /// </summary>
        MemoryLocked = 0x00040000,

        /// <summary>
        /// Reserved for future use.
        /// </summary>
        MemoryPeload = 0x00080000,

        Align1Bytes = 0x00100000,
        Align2Bytes = 0x00200000,
        Align4Bytes = 0x00300000,
        Align8Bytes = 0x00400000,
        Align16Bytes = 0x00500000,
        Align32Bytes = 0x00600000,
        Align64Bytes = 0x00700000,
        Align128Bytes = 0x00800000,
        Align256Bytes = 0x00900000,
        Align512Bytes = 0x00a00000,
        Align1024Bytes = 0x00b00000,
        Align2048Bytes = 0x00c00000,
        Align4096Bytes = 0x00d00000,
        Align8192Bytes = 0x00e00000,

        /// <summary>
        /// The section contains extended relocations.
        /// </summary>
        NRelocOvfl = 0x01000000,

        /// <summary>
        /// The section can be discarded as needed.
        /// </summary>
        MemoryDiscardable = 0x02000000,

        /// <summary>
        /// The section cannot be cached.
        /// </summary>
        MemoryNotCached = 0x04000000,

        /// <summary>
        /// The section is not pageable.
        /// </summary>
        MemoryNotPaged = 0x08000000,

        /// <summary>
        /// The section can be shared in memory.
        /// </summary>
        MemoryShared = 0x10000000,

        /// <summary>
        /// The section can be executed as code.
        /// </summary>
        MemoryExecute = 0x20000000,

        /// <summary>
        /// The section can be read.
        /// </summary>
        MemoryRead = 0x40000000,

        /// <summary>
        /// The section can be written to.
        /// </summary>
        MemoryWrite = 0x80000000
    }       
}
