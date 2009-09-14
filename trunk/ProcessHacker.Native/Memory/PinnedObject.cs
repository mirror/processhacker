using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using ProcessHacker.Common.Objects;

namespace ProcessHacker.Native
{
    public sealed class PinnedObject<T> : BaseObject
    {
        private T _object;
        private GCHandle _handle;

        public PinnedObject(T obj)
        {
            _object = obj;
            _handle = GCHandle.Alloc(obj, GCHandleType.Pinned);
        }

        protected override void DisposeObject(bool disposing)
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
