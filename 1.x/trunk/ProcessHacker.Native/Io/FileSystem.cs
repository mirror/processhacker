using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using ProcessHacker.Native;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;

namespace ProcessHacker.Native.Io
{
    public static class FileSystem
    {
        public const int MaximumReparseDataBufferSize = 16 * 1024;

        [StructLayout(LayoutKind.Sequential)]
        public struct ReparseDataBuffer
        {
            public static readonly int GenericDataBuffer =
                Marshal.OffsetOf(typeof(ReparseDataBuffer), "SubstituteNameOffset").ToInt32();
            public static readonly int MountPointPathBuffer =
                Marshal.OffsetOf(typeof(ReparseDataBuffer), "Flags").ToInt32();
            public static readonly int SymbolicLinkPathBuffer =
                Marshal.OffsetOf(typeof(ReparseDataBuffer), "PathBuffer").ToInt32();

            public uint ReparseTag;
            public ushort ReparseDataLength;
            public ushort Reserved;

            public ushort SubstituteNameOffset; // GenericReparseBuffer.DataBuffer
            public ushort SubstituteNameLength;
            public ushort PrintNameOffset;
            public ushort PrintNameLength;
            public int Flags; // MountPointReparseBuffer.PathBuffer
            public byte PathBuffer; // SymbolicLinkReparseBuffer.PathBuffer
        }

        public static readonly int FsCtlSetReparsePoint = Win32.CtlCode(DeviceType.FileSystem, 41, DeviceControlMethod.Buffered, DeviceControlAccess.Special);
        public static readonly int FsCtlGetReparsePoint = Win32.CtlCode(DeviceType.FileSystem, 42, DeviceControlMethod.Buffered, DeviceControlAccess.Any);
        public static readonly int FsCtlDeleteReparsePoint = Win32.CtlCode(DeviceType.FileSystem, 43, DeviceControlMethod.Buffered, DeviceControlAccess.Special);
    }
}
