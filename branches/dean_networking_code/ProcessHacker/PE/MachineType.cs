/*
 * Process Hacker - 
 *   machine type
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
    /// Specifies an executable's target CPU type.
    /// </summary>
    public enum MachineType : ushort
    {
        /// <summary>
        /// Assumed to be applicable to any machine type.
        /// </summary>
        Unknown = 0x0,

        /// <summary>
        /// Matsushita AM33.
        /// </summary>
        AM33 = 0x1d3,

        /// <summary>
        /// x64.
        /// </summary>
        AMD64 = 0x8664,

        /// <summary>
        /// ARM little-endian.
        /// </summary>
        ARM = 0x1c0,

        /// <summary>
        /// EFI byte code.
        /// </summary>
        EBC = 0xebc,

        /// <summary>
        /// Intel 386 or later processors and compatible processors.
        /// </summary>
        i386 = 0x14c,

        /// <summary>
        /// Intel Itanium processor family.
        /// </summary>
        IA64 = 0x200,

        /// <summary>
        /// Mitsubishi M32R little endian.
        /// </summary>
        M32R = 0x9041,

        /// <summary>
        /// MIPS16.
        /// </summary>
        MIPS16 = 0x266,

        /// <summary>
        /// MIPS with FPU.
        /// </summary>
        MIPSFPU = 0x366,

        /// <summary>
        /// MIPS16 with FPU.
        /// </summary>
        MIPSFPU16 = 0x466,

        /// <summary>
        /// PowerPC little-endian.
        /// </summary>
        PowerPC = 0x1f0,

        /// <summary>
        /// PowerPC with floating point support.
        /// </summary>
        PowerPCFP = 0x1f1,

        /// <summary>
        /// MIPS little-endian.
        /// </summary>
        R4000 = 0x166,

        /// <summary>
        /// Hitachi SH3.
        /// </summary>
        SH3 = 0x1a2,

        /// <summary>
        /// Hitachi SH3 DSP.
        /// </summary>
        SH3DSP = 0x1a3,

        /// <summary>
        /// Hitachi SH4.
        /// </summary>
        SH4 = 0x1a6,

        /// <summary>
        /// Hitachi SH5.
        /// </summary>
        SH5 = 0x1a8,

        /// <summary>
        /// Thumb.
        /// </summary>
        Thumb = 0x1c2,

        /// <summary>
        /// MIPS little-endian WCE v2.
        /// </summary>
        WCEMIPSv2 = 0x169
    }
}
