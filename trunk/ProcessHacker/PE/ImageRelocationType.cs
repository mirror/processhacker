/*
 * Process Hacker - 
 *   image relocation type
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
    /// Specifies the type of an image relocation.
    /// </summary>
    public enum ImageRelocationType
    {
        /// <summary>
        /// The base relocation is skipped. This type can be used to pad a block.
        /// </summary>
        Absolute = 0,

        /// <summary>
        /// The base relocation adds the high 16 bits of the difference to the 16-bit 
        /// field at offset. The 16-bit field represents the high value of a 32-bit word.
        /// </summary>
        High,

        /// <summary>
        /// The base relocation adds the low 16 bits of the difference to the 16-bit 
        /// field at offset. The 16-bit field represents the low half of a 32-bit word. 
        /// </summary>
        Low,

        /// <summary>
        /// The base relocation applies all 32 bits of the difference to the 32-bit 
        /// field at offset.
        /// </summary>
        HighLow,

        /// <summary>
        /// The base relocation adds the high 16 bits of the difference to the 16-bit 
        /// field at offset. The 16-bit field represents the high value of a 32-bit word. 
        /// The low 16 bits of the 32-bit value are stored in the 16-bit word that follows 
        /// this base relocation. This means that this base relocation occupies two slots.
        /// </summary>
        HighAdj,

        /// <summary>
        /// The base relocation applies to a MIPS jump instruction.
        /// </summary>
        MipsJmpAddr,
        Reserved1,
        Reserved2,

        /// <summary>
        /// The base relocation applies to a MIPS16 jump instruction.
        /// </summary>
        MipsJmpAddr16,

        /// <summary>
        /// The base relocation applies the difference to the 64-bit field at offset.
        /// </summary>
        Dir16
    }
}
