using System;
using System.Runtime.InteropServices;

namespace ProcessHacker.Native
{
    public sealed class PinnedObject<T> : MemoryRegion
    {
        private readonly T _object;
        private GCHandle _handle;

        public PinnedObject(T obj)
        {
            _object = obj;
            _handle = GCHandle.Alloc(obj, GCHandleType.Pinned);
        }

        protected override void Free()
        {
            _handle.Free();
        }

        public IntPtr Address
        {
            get { return _handle.AddrOfPinnedObject(); }
        }

        public T Object
        {
            get { return _object; }
        }
    }
}
