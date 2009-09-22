using System;
using System.Runtime.InteropServices;
using System.Security;

namespace TaskbarLib
{
    internal class IconHandle : SafeHandle
    {
        internal IconHandle()
            : this(IntPtr.Zero)
        {
        }

        [SecurityCritical, SecurityTreatAsSafe]
        internal IconHandle(IntPtr ptr)
            : base(ptr, true)
        {
        }

        [SecurityTreatAsSafe, SecurityCritical]
        internal IconHandle(IntPtr ptr, bool ownsHandle)
            : base(ptr, ownsHandle)
        {
        }

        public override bool IsInvalid
        {
            [SecurityTreatAsSafe, SecurityCritical]
            get
            {
                return this.handle == IntPtr.Zero;
            }
        }

        public HandleRef MakeHandleRef(object wrapper)
        {
            return new HandleRef(wrapper, this.handle);
        }

        public override bool Equals(object obj)
        {
            var other = obj as IconHandle;

            return other != null && other.handle.Equals(this.handle);
        }

        public override int GetHashCode()
        {
            return this.handle.ToInt32();
        }

        [SecurityCritical]
        protected override bool ReleaseHandle()
        {
            return true;// Win32.DestroyIcon(this.handle);
        }
    }

}