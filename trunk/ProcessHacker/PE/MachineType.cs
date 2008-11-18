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
