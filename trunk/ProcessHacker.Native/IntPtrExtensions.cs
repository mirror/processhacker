using System;
using System.Runtime.InteropServices;

namespace ProcessHacker.Native
{
    public static class IntPtrExtensions
    {
        public static IntPtr Increment(this IntPtr ptr, int cbSize)
        {
            return new IntPtr(ptr.ToInt64() + cbSize);
        }

        public static IntPtr Increment(this IntPtr ptr, IntPtr ptr2)
        {
            return new IntPtr(ptr.ToInt64() + ptr2.ToInt64());
        }

        public static IntPtr Decrement(this IntPtr ptr, IntPtr ptr2)
        {
            return new IntPtr(ptr.ToInt64() - ptr2.ToInt64());
        }

        public static IntPtr Decrement(this IntPtr ptr, int cbSize)
        {
            return Increment(ptr, -cbSize);
        }

        public static IntPtr Increment<T>(this IntPtr ptr)
        {
            return ptr.Increment(Marshal.SizeOf(typeof(T)));
        }

        public static T ElementAt<T>(this IntPtr ptr, int index)
        {
            var offset = Marshal.SizeOf(typeof(T)) * index;
            var offsetPtr = ptr.Increment(offset);
            return (T)Marshal.PtrToStructure(offsetPtr, typeof(T));
        }

        public static int CompareTo(this IntPtr ptr, IntPtr ptr2)
        {
            if (ptr.ToInt64() > ptr2.ToInt64())
                return 1;
            if (ptr.ToInt64() < ptr2.ToInt64())
                return -1;
            return 0;
        }

        public static int CompareTo(this IntPtr ptr, int ptr2)
        {
            if (ptr.ToInt64() > ptr2)
                return 1;
            if (ptr.ToInt64() < ptr2)
                return -1;
            return 0;
        }
    }
}
