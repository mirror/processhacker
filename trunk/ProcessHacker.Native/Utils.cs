using System;
using System.Runtime.InteropServices;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;

namespace ProcessHacker.Native
{
    public static class Utils
    {
        /// <summary>
        /// Reads a Unicode string.
        /// </summary>
        /// <param name="str">A UNICODE_STRING structure.</param>
        /// <returns>A string.</returns>
        /// <remarks>This function is needed because some LSA strings are not 
        /// null-terminated, so we can't use .NET's marshalling.</remarks>
        public static string ReadUnicodeString(UnicodeString str)
        {
            if (str.Length == 0)
                return null;

            return Marshal.PtrToStringUni(new IntPtr(str.Buffer), str.Length / 2);
        }

        public static string ReadUnicodeString(ProcessHandle processHandle, UnicodeString str)
        {
            if (str.Length == 0)
                return null;

            byte[] strData = processHandle.ReadMemory(str.Buffer, str.Length);
            GCHandle strDataHandle = GCHandle.Alloc(strData, GCHandleType.Pinned);

            try
            {
                return Marshal.PtrToStringUni(strDataHandle.AddrOfPinnedObject(), str.Length / 2);
            }
            finally
            {
                strDataHandle.Free();
            }
        }

        /// <summary>
        /// Swaps the order of the bytes in the argument.
        /// </summary>
        /// <param name="v">The number to change.</param>
        /// <returns>A number.</returns>
        public static int ByteSwap(int v)
        {
            byte b1 = (byte)v;
            byte b2 = (byte)(v >> 8);
            byte b3 = (byte)(v >> 16);
            byte b4 = (byte)(v >> 24);

            return b4 | (b3 << 8) | (b2 << 16) | (b1 << 24);
        }

        /// <summary>
        /// Swaps the order of the bytes.
        /// </summary>
        /// <returns>A number.</returns>
        public static int SwapBytes(this int v)
        {
            return ByteSwap(v);
        }

        /// <summary>
        /// Swaps the order of the bytes in the argument.
        /// </summary>
        /// <param name="v">The number to change.</param>
        /// <returns>A number.</returns>
        public static uint ByteSwap(uint v)
        {
            byte b1 = (byte)v;
            byte b2 = (byte)(v >> 8);
            byte b3 = (byte)(v >> 16);
            byte b4 = (byte)(v >> 24);

            return (uint)(b4 | (b3 << 8) | (b2 << 16) | (b1 << 24));
        }

        /// <summary>
        /// Swaps the order of the bytes.
        /// </summary>
        /// <returns>A number.</returns>
        public static uint SwapBytes(this uint v)
        {
            return ByteSwap(v);
        }

        /// <summary>
        /// Swaps the order of the bytes in the argument.
        /// </summary>
        /// <param name="v">The number to change.</param>
        /// <returns>A number.</returns>
        public static ushort ByteSwap(ushort v)
        {
            byte b1 = (byte)v;
            byte b2 = (byte)(v >> 8);

            return (ushort)(b2 | (b1 << 8));
        }

        /// <summary>
        /// Swaps the order of the bytes.
        /// </summary>
        /// <returns>A number.</returns>
        public static ushort SwapBytes(this ushort v)
        {
            return ByteSwap(v);
        }
    }
}
