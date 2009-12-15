using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;

namespace ProcessHacker.Native.Mfs
{
    public unsafe sealed class MemoryFileSystem
    {
        private const int MfsMagic = 0x0053464d;
        private const string MfsMagicString = "MFS\0";

        private const int MfsBlockSize = 0x10000;
        private const int MfsCellSize = 0x400;

        private Section _section;
        private MfsFsHeader* _header;

        public MemoryFileSystem(string fileName, bool readOnly)
        {
            using (var fhandle = FileHandle.CreateWin32(
                fileName,
                FileAccess.GenericRead | (!readOnly ? FileAccess.GenericWrite : 0),
                FileShareMode.Read,
                FileCreationDispositionWin32.OpenAlways
                ))
            {
                _section = new Section(fhandle, !readOnly ? MemoryProtection.ReadWrite : MemoryProtection.ReadOnly);

                if (fhandle.GetSize() < MfsBlockSize)
                    throw new MfsInvalidFileSystemException();
            }
        }
    }
}
