/*
 * Process Hacker - 
 *   DLL characteristics
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
    /// Specifies a DLL's attributes.
    /// </summary>
    [Flags]
    public enum DllCharacteristics : ushort
    {
        /// <summary>
        /// Reserved, must be zero.
        /// </summary>
        Reserved1 = 0x0001,

        /// <summary>
        /// Reserved, must be zero.
        /// </summary>
        Reserved2 = 0x0002,

        /// <summary>
        /// Reserved, must be zero.
        /// </summary>
        Reserved3 = 0x0004,

        /// <summary>
        /// Reserved, must be zero.
        /// </summary>
        Reserved4 = 0x0008,

        /// <summary>
        /// DLL can be relocated at load time.
        /// </summary>
        DynamicBase = 0x0040,

        /// <summary>
        /// Code Integrity checks are enforced.
        /// </summary>
        ForceIntegrity = 0x0080,

        /// <summary>
        /// Image is NX compatible.
        /// </summary>
        NXCompat = 0x0100,

        /// <summary>
        /// Isolation aware, but do not isolate the image.
        /// </summary>
        NoIsolation = 0x0200,

        /// <summary>
        /// Does not use structured exception (SE) handling. No SE 
        /// handler may be called in this image.
        /// </summary>
        NoSEH = 0x0400,

        /// <summary>
        /// Do not bind the image.
        /// </summary>
        NoBind = 0x0800,

        /// <summary>
        /// Reserved, must be zero.
        /// </summary>
        Reserved5 = 0x1000,

        /// <summary>
        /// A WDM driver.
        /// </summary>
        WDMDriver = 0x2000,

        /// <summary>
        /// Terminal Server aware.
        /// </summary>
        TerminalServerAware = 0x8000
    }
}
