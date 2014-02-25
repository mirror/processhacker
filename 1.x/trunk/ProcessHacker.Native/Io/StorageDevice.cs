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
        [StructLayout(LayoutKind.Sequential)]
        public struct StoragePreventMediaRemoval
        {
            [MarshalAs(UnmanagedType.I1)]
            public bool PreventMediaRemoval;
        }

        // Output for IoCtlGetHotplugInfo
        [StructLayout(LayoutKind.Sequential)]
        public struct StorageHotplugInfo
        {
            public int Size;
            [MarshalAs(UnmanagedType.I1)]
            public bool MediaRemovable;
            [MarshalAs(UnmanagedType.I1)]
            public bool MediaHotplug;
            [MarshalAs(UnmanagedType.I1)]
            public bool DeviceHotplug;
            [MarshalAs(UnmanagedType.I1)]
            public bool WriteCacheEnableOverride;
        }

        public static readonly int IoCtlMediaRemoval = Win32.CtlCode(DeviceType.MassStorage, 0x0201, DeviceControlMethod.Buffered, DeviceControlAccess.Read);
        public static readonly int IoCtlEjectMedia = Win32.CtlCode(DeviceType.MassStorage, 0x0202, DeviceControlMethod.Buffered, DeviceControlAccess.Read);
        public static readonly int IoCtlGetHotplugInfo = Win32.CtlCode(DeviceType.MassStorage, 0x0305, DeviceControlMethod.Buffered, DeviceControlAccess.Any);

        public static void EjectMedia(char driveLetter)
        {
            using (var fhandle = new FileHandle(@"\??\" + driveLetter + ":", FileAccess.GenericRead))
                EjectMedia(fhandle);
        }

        public static void EjectMedia(string fileName)
        {
            using (var fhandle = OpenStorageDevice(fileName, FileAccess.GenericRead))
                EjectMedia(fhandle);
        }

        public static void EjectMedia(FileHandle fileHandle)
        {
            fileHandle.IoControl(IoCtlEjectMedia, IntPtr.Zero, 0, IntPtr.Zero, 0);
        }

        public static StorageHotplugInfo GetHotplugInfo(string fileName)
        {
            using (var fhandle = OpenStorageDevice(fileName, FileAccess.GenericRead))
                return GetHotplugInfo(fhandle);
        }

        public static StorageHotplugInfo GetHotplugInfo(FileHandle fileHandle)
        {
            unsafe
            {
                StorageHotplugInfo hotplugInfo;

                fileHandle.IoControl(IoCtlGetHotplugInfo, null, 0, &hotplugInfo, Marshal.SizeOf(typeof(StorageHotplugInfo)));

                return hotplugInfo;
            }
        }

        public static FileHandle OpenStorageDevice(string fileName, FileAccess access)
        {
            return new FileHandle(fileName, FileShareMode.ReadWrite, access);
        }
    }
}
