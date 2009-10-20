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
    public static class StorageDevice
    {
        // Input for IoCtlMediaRemoval
        public struct StoragePreventMediaRemoval
        {
            [MarshalAs(UnmanagedType.I1)]
            public bool PreventMediaRemoval;
        }

        public static readonly int IoCtlMediaRemoval = Win32.CtlCode(DeviceType.MassStorage, 0x0201, DeviceControlMethod.Buffered, DeviceControlAccess.Read);
        public static readonly int IoCtlEjectMedia = Win32.CtlCode(DeviceType.MassStorage, 0x0202, DeviceControlMethod.Buffered, DeviceControlAccess.Read);

        public static void EjectMedia(char driveLetter)
        {
            EjectMedia(FileUtils.GetPathForDosDrive(driveLetter));
        }

        public static void EjectMedia(string fileName)
        {
            using (var fhandle = new FileHandle(fileName, FileAccess.GenericRead))
                EjectMedia(fhandle);
        }

        public static void EjectMedia(FileHandle fileHandle)
        {
            fileHandle.IoControl(IoCtlEjectMedia, IntPtr.Zero, 0, IntPtr.Zero, 0);
        }
    }
}
