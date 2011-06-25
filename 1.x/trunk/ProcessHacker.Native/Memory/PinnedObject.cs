using System;
using System.Runtime.InteropServices;

namespace ProcessHacker.Native
{
    public sealed class PinnedObject<T> : MemoryRegion
    {
        private GCHandle _handle;
        public T Object { get; private set; }

        public PinnedObject(T obj)
        {
            this.Object = obj;
            _handle = GCHandle.Alloc(obj, GCHandleType.Pinned);
        }

        protected override void Free()
        {
            this._handle.Free();
        }

        public IntPtr Address
        {
            get { return _handle.AddrOfPinnedObject(); }
        }
    }
}
