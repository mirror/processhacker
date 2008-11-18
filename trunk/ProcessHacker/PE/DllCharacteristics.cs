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
