/*
 * Process Hacker - 
 *   image subsystem
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
    /// Specifies the Windows subsystem requied to run an image.
    /// </summary>
    public enum ImageSubsystem : ushort
    {
        /// <summary>
        /// An unknown subsystem.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Device drivers and native Windows processes.
        /// </summary>
        Native = 1,

        /// <summary>
        /// The Windows graphical user interface (GUI) subsystem.
        /// </summary>
        WindowsGUI = 2,

        /// <summary>
        /// The Windows character subsystem.
        /// </summary>
        WindowsCUI = 3,

        /// <summary>
        /// The POSIX character subsystem.
        /// </summary>
        POSIXCUI = 7,

        /// <summary>
        /// Windows CE.
        /// </summary>
        WindowsCEGUI = 9,

        /// <summary>
        /// An Extensible Firmware Interface (EFI) application.
        /// </summary>
        EFIApplication = 10,

        /// <summary>
        /// An EFI driver with boot services.
        /// </summary>
        EFIBootServiceDriver = 11,

        /// <summary>
        /// An EFI driver with run-time services.
        /// </summary>
        EFIRuntimeDriver = 12,

        /// <summary>
        /// An EFI ROM image.
        /// </summary>
        EFIROM = 13,

        /// <summary>
        /// XBOX.
        /// </summary>
        XBOX = 14
    }       
}
