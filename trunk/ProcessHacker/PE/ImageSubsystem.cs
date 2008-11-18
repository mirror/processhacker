/*
 * Process Hacker
 * 
 * Descriptions from the PE/COFF specification v8 from Microsoft.
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
