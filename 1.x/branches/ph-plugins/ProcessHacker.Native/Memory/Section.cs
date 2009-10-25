using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;

namespace ProcessHacker.Native
{
    /// <summary>
    /// Represents a section, a memory mapping.
    /// </summary>
    public sealed class Section : NativeObject<SectionHandle>
    {
        private MemoryProtection _originalProtection = MemoryProtection.ReadWrite;

        /// <summary>
        /// Opens an existing section.
        /// </summary>
        /// <param name="name">The name of an existing section.</param>
        /// <param name="access">The desired access to the section.</param>
        public Section(string name, SectionAccess access)
        {
            this.Handle = new SectionHandle(name, access);
        }

        /// <summary>
        /// Creates a section backed by a file.
        /// </summary>
        /// <param name="fileHandle">A file handle.</param>
        public Section(FileHandle fileHandle)
            : this(fileHandle, MemoryProtection.ReadWrite)
        { }

        /// <summary>
        /// Creates a section backed by a file.
        /// </summary>
        /// <param name="fileHandle">A file handle.</param>
        /// <param name="protection">The page protection to apply to mappings.</param>
        public Section(FileHandle fileHandle, MemoryProtection protection)
            : this(fileHandle, false, protection)
        { }

        /// <summary>
        /// Creates a section backed by a file.
        /// </summary>
        /// <param name="fileHandle">A file handle.</param>
        /// <param name="image">Whether to treat the file as an executable image.</param>
        /// <param name="protection">The page protection to apply to mappings.</param>
        public Section(FileHandle fileHandle, bool image, MemoryProtection protection)
            : this(null, fileHandle, image, protection)
        { }

        /// <summary>
        /// Creates a section backed by a file.
        /// </summary>
        /// <param name="name">The name of the section.</param>
        /// <param name="fileHandle">A file handle.</param>
        /// <param name="image">Whether to treat the file as an executable image.</param>
        /// <param name="protection">The page protection to apply to mappings.</param>
        public Section(string name, FileHandle fileHandle, bool image, MemoryProtection protection)
        {
            _originalProtection = protection;

            this.Handle = SectionHandle.Create(
                SectionAccess.All,
                name,
                ObjectFlags.OpenIf,
                null,
                fileHandle.GetSize(),
                image ? SectionAttributes.Image : SectionAttributes.Commit,
                protection,
                fileHandle
                );
        }

        /// <summary>
        /// Creates a section backed by the page file (i.e. in memory).
        /// </summary>
        /// <param name="maximumSize">The maximum size of the section.</param>
        public Section(long maximumSize)
            : this(maximumSize, MemoryProtection.ReadWrite)
        { }

        /// <summary>
        /// Creates a section backed by the page file (i.e. in memory).
        /// </summary>
        /// <param name="maximumSize">The maximum size of the section.</param>
        /// <param name="protection">The page protection to apply to mappings.</param>
        public Section(long maximumSize, MemoryProtection protection)
            : this(null, maximumSize, protection)
        { }

        /// <summary>
        /// Creates a section backed by the page file (i.e. in memory).
        /// </summary>
        /// <param name="name">The name of the section.</param>
        /// <param name="maximumSize">The maximum size of the section.</param>
        /// <param name="protection">The page protection to apply to mappings.</param>
        public Section(string name, long maximumSize, MemoryProtection protection)
        {        
            _originalProtection = protection;

            this.Handle = SectionHandle.Create(
                SectionAccess.All,
                name,
                ObjectFlags.OpenIf,
                null,
                maximumSize,
                SectionAttributes.Commit,
                protection,
                null
                );
        }

        /// <summary>
        /// Extends the size of the section.
        /// </summary>
        /// <param name="newSize">The new size of the section.</param>
        public void Extend(long newSize)
        {
            this.Handle.Extend(newSize);
        }

        /// <summary>
        /// Creates a view of the section.
        /// </summary>
        /// <param name="size">
        /// The number of bytes to map. This value will be rounded up to the 
        /// page size.
        /// </param>
        /// <returns>A view of the section.</returns>
        public SectionView MapView(int size)
        {
            return this.MapView(size, _originalProtection);
        }

        /// <summary>
        /// Creates a view of the section.
        /// </summary>
        /// <param name="size">
        /// The number of bytes to map. This value will be rounded up to the 
        /// page size.
        /// </param>
        /// <param name="protection">The page protection to apply to the mapping.</param>
        /// <returns>A view of the section.</returns>
        public SectionView MapView(int size, MemoryProtection protection)
        {
            return this.Handle.MapView(0, size, protection);
        }

        /// <summary>
        /// Creates a view of the section.
        /// </summary>
        /// <param name="offset">
        /// The offset from the beginning of the section to map. This value 
        /// must be a multiple of 0x10000 (65536).
        /// </param>
        /// <param name="size">
        /// The number of bytes to map. This value will be rounded up to the 
        /// page size.
        /// </param>
        /// <param name="protection">The page protection to apply to the mapping.</param>
        /// <returns>A view of the section.</returns>
        public SectionView MapView(int offset, int size, MemoryProtection protection)
        {
            return this.Handle.MapView(offset, size, protection);
        }
    }
}
