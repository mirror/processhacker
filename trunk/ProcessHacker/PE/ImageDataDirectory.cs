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
    /// The type of image data.
    /// </summary>
    public enum ImageDataType
    {
        /// <summary>
        /// The export table.
        /// </summary>
        ExportTable = 0,

        /// <summary>
        /// The import table.
        /// </summary>
        ImportTable,

        /// <summary>
        /// The resource table.
        /// </summary>
        ResourceTable,

        /// <summary>
        /// The exception table.
        /// </summary>
        ExceptionTable,

        /// <summary>
        /// The attribute certificate table.
        /// </summary>
        CertificateTable,

        /// <summary>
        /// The base relocation table.
        /// </summary>
        BaseRelocationTable,

        /// <summary>
        /// The debug data.
        /// </summary>
        Debug,

        /// <summary>
        /// Reserved, must be zero.
        /// </summary>
        Architecture,

        /// <summary>
        /// The RVA of the value to be stored in the global pointer register.
        /// </summary>
        GlobalPtr,

        /// <summary>
        /// The thread local storage (TLS) table.
        /// </summary>      
        TLSTable,

        /// <summary>
        /// The load configuration table.
        /// </summary>
        LoadConfigTable,

        /// <summary>
        /// The bound import table.
        /// </summary>
        BoundImport,

        /// <summary>
        /// The import address table.
        /// </summary>
        IAT,

        /// <summary>
        /// The delay import descriptor.
        /// </summary>
        DelayImportDescriptor,

        /// <summary>
        /// The CLR runtime header.
        /// </summary>
        CLRRuntimeHeader,

        /// <summary>
        /// Reserved, must be zero.
        /// </summary>
        Reserved
    }

    /// <summary>
    /// Represents a data directory containing the address and size of a table 
    /// or string in the image.
    /// </summary>
    public struct ImageDataDirectory
    {
        /// <summary>
        /// The relative virtual address (RVA) of the table.
        /// </summary>
        public uint VirtualAddress;

        /// <summary>
        /// The size, in bytes, of the table.
        /// </summary>
        public uint Size;
    }
}
