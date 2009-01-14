/*
 * Process Hacker - 
 *   image characteristics
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
    /// Specifies an image file's attributes.
    /// </summary>
    [Flags]
    public enum ImageCharacteristics : ushort
    {
        /// <summary>
        /// Image only, Windows CE, and Windows NT® and later. This indicates that the file does 
        /// not contain base relocations and must therefore be loaded at its preferred base address. 
        /// If the base address is not available, the loader reports an error. The default behavior 
        /// of the linker is to strip base relocations from executable (EXE) files.
        /// </summary>
        RelocsStripped = 0x0001,

        /// <summary>
        /// Image only. This indicates that the image file is valid and can be run. If this flag 
        /// is not set, it indicates a linker error.
        /// </summary>
        ExecutableImage = 0x0002,

        /// <summary>
        /// COFF line numbers have been removed. This flag is deprecated and should be zero.
        /// </summary>
        LineNumsStripped = 0x0004,

        /// <summary>
        /// COFF symbol table entries for local symbols have been removed. This flag is deprecated 
        /// and should be zero.
        /// </summary>
        LocalSymsStripped = 0x0008,

        /// <summary>
        /// Obsolete. Aggressively trim working set. This flag is deprecated for Windows 2000 and later 
        /// and must be zero.
        /// </summary>
        AggressiveWsTrim = 0x0010,

        /// <summary>
        /// Application can handle > 2 GB addresses.
        /// </summary>
        LargeAddressAware = 0x0020,

        /// <summary>
        /// This flag is reserved for future use.
        /// </summary>
        Reserved = 0x0040,

        /// <summary>
        /// Little endian: the least significant bit (LSB) precedes the most significant bit (MSB) in 
        /// memory. This flag is deprecated and should be zero.
        /// </summary>
        BytesReversedLo = 0x0080,

        /// <summary>
        /// Machine is based on a 32-bit-word architecture.
        /// </summary>
        ThirtyTwoBitMachine = 0x0100,

        /// <summary>
        /// Debugging information is removed from the image file.
        /// </summary>
        DebugStripped = 0x0200,

        /// <summary>
        /// If the image is on removable media, fully load it and copy it to the swap file.
        /// </summary>
        RemovableRunFromSwap = 0x0400,

        /// <summary>
        /// If the image is on network media, fully load it and copy it to the swap file.
        /// </summary>
        NetRunFromSwap = 0x0800,

        /// <summary>
        /// The image file is a system file, not a user program.
        /// </summary>
        System = 0x1000,

        /// <summary>
        /// The image file is a dynamic-link library (DLL). Such files are considered 
        /// executable files for almost all purposes, although they cannot be directly run.
        /// </summary>
        DLL = 0x2000,

        /// <summary>
        /// The file should be run only on a uniprocessor machine.
        /// </summary>
        UPSystemOnly = 0x4000,

        /// <summary>
        /// Big endian: the MSB precedes the LSB in memory. This flag is deprecated and should be zero.
        /// </summary>
        BytesReversedHi = 0x8000  
    }       
}
