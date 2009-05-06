using System;
using System.Runtime.InteropServices;

namespace ProcessHacker.Native
{
    public static class IntPtrExtensions
    {
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

        public static IntPtr Decrement(this IntPtr ptr, IntPtr ptr2)
        {
            if (IntPtr.Size == sizeof(Int32))
                return new IntPtr(ptr.ToInt32() - ptr2.ToInt32());
            else
                return new IntPtr(ptr.ToInt64() - ptr2.ToInt64());
        }

        public static IntPtr Decrement(this IntPtr ptr, int value)
        {
            return Increment(ptr, -value);
        }

        public static T ElementAt<T>(this IntPtr ptr, int index)
        {
            var offset = Marshal.SizeOf(typeof(T)) * index;
            var offsetPtr = ptr.Increment(offset);
            return (T)Marshal.PtrToStructure(offsetPtr, typeof(T));
        }

        public static IntPtr Increment(this IntPtr ptr, int value)
        {
            unchecked
            {
                if (IntPtr.Size == sizeof(Int32))
                    return new IntPtr(ptr.ToInt32() + value);
                else
                    return new IntPtr(ptr.ToInt64() + value);
            }
        }

        public static IntPtr Increment(this IntPtr ptr, IntPtr ptr2)
        {
            unchecked
            {
                if (IntPtr.Size == sizeof(Int32))
                    return new IntPtr(ptr.ToInt32() + ptr2.ToInt32());
                else
                    return new IntPtr(ptr.ToInt64() + ptr2.ToInt64());
            }
        }

        public static IntPtr Increment<T>(this IntPtr ptr)
        {
            return ptr.Increment(Marshal.SizeOf(typeof(T)));
        }

        public static uint ToUInt32(this IntPtr ptr)
        {
            return (uint)ptr.ToInt32();
        }

        public static ulong ToUInt64(this IntPtr ptr)
        {
            return (ulong)ptr.ToInt64();
        }
    }
}
