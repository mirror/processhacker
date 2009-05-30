using System;
using System.Collections.Generic;
using System.Text;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;

namespace ProcessHacker.Native
{
    public class Section : NativeObject<SectionHandle>
    {
        private MemoryProtection _originalProtection;

        public Section(FileHandle fileHandle)
            : this(fileHandle, MemoryProtection.ReadWrite)
        { }

        public Section(FileHandle fileHandle, MemoryProtection protection)
            : this(fileHandle, false, protection)
        { }

        public Section(FileHandle fileHandle, bool image, MemoryProtection protection)
            : this(null, fileHandle, image, protection)
        { }

        public Section(string name, FileHandle fileHandle, bool image, MemoryProtection protection)
        {
            _originalProtection = protection;
            this.Handle = SectionHandle.Create(
                SectionAccess.All,
                fileHandle.GetSize(),
                image ? SectionAttributes.Image : 0,
                protection,
                fileHandle
                );
        }

        public Section(long maximumSize)
            : this(maximumSize, MemoryProtection.ReadWrite)
        { }

        public Section(long maximumSize, MemoryProtection protection)
            : this(null, maximumSize, protection)
        { }

        public Section(string name, long maximumSize, MemoryProtection protection)
        {        
            _originalProtection = protection;
            this.Handle = SectionHandle.Create(
                SectionAccess.All,
                maximumSize,
                SectionAttributes.Commit,
                protection
                );
        }

        public void Extend(long newSize)
        {
            this.Handle.Extend(newSize);
        }

        public SectionView MapView(int size)
        {
            return this.MapView(size, _originalProtection);
        }

        public SectionView MapView(int size, MemoryProtection protection)
        {
            return this.Handle.MapView(size, protection);
        }
    }
}
