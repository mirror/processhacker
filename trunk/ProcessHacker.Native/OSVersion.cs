/*
 * Process Hacker - 
 *   operating system version information
 *
 * Copyright (C) 2009 wj32
 * Copyright (C) 2009 dmex
 * 
 * This file is part of Process Hacker.
 * 
 * Process Hacker is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * Process Hacker is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Process Hacker.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using ProcessHacker.Native.Security;

namespace ProcessHacker.Native
{
    public enum OSArch
    {
        I386,
        Amd64
    }

    public enum WindowsVersion
    {
        /// <summary>
        /// Windows 2000 SP0, SP1, SP2, SP3, SP4, Server
        /// </summary>
        Win2000 = 50,

        /// <summary>
        /// Windows XP SP2, SP3.
        /// </summary>
        XP = 51,

        /// <summary>
        /// Windows Server 2003.
        /// </summary>
        Server2003 = 52,

        /// <summary>
        /// Windows Vista SP0, SP1, SP2, Windows Server 2008.
        /// </summary>
        Vista = 60,

        /// <summary>
        /// Windows 7 SP0.
        /// </summary>
        Seven = 61,

        /// <summary>
        /// An unreleased version of Windows.
        /// </summary>
        Unreleased = int.MaxValue
    }

    public enum WindowsType : uint
    {
        /// <summary>
        /// Business Edition
        /// </summary>
        Business = 0x00000006,

        /// <summary>
        /// Business N Edition
        /// </summary>
        BusinessN = 0x00000010,

        /// <summary>
        /// Cluster Server Edition
        /// </summary>
        ClusterServer = 0x00000012,

        /// <summary>
        /// Server Datacenter Edition (full installation)
        /// </summary>
        DatacenterServer = 0x00000008,

        /// <summary>
        /// Server Datacenter Edition (core installation)
        /// </summary>
        DatacenterServerCore = 0x0000000C,

        /// <summary>
        /// Enterprise Edition
        /// </summary>
        Enterprise = 0x00000004,

        /// <summary>
        /// Server Enterprise Edition (full installation)
        /// </summary>
        EnterpriseServer = 0x0000000A,

        /// <summary>
        /// Server Enterprise Edition (core installation)
        /// </summary>
        EnterpriseServerCore = 0x0000000E,

        /// <summary>
        /// Server Enterprise Edition for Itanium-based Systems
        /// </summary>
        EnterpriseServerIa64 = 0x0000000F,

        /// <summary>
        /// Home Basic Edition
        /// </summary>
        HomeBasic = 0x00000002,

        /// <summary>
        /// Home Basic N Edition
        /// </summary>
        HomeBasicN = 0x00000005,

        /// <summary>
        /// Home Premium Edition
        /// </summary>
        HomePremium = 0x00000003,

        /// <summary>
        /// Home Server Edition
        /// </summary>
        HomeServer = 0x00000013,

        /// <summary>
        /// Server for Small Business Edition
        /// </summary>
        ServerForSmallbusiness = 0x00000018,

        /// <summary>
        /// Small Business Server
        /// </summary>
        SmallbusinessServer = 0x00000009,

        /// <summary>
        /// Small Business Server Premium Edition
        /// </summary>
        SmallbusinessServerPremium = 0x00000019,

        /// <summary>
        /// Server Standard Edition (full installation)
        /// </summary>
        StandardServer = 0x00000007,

        /// <summary>
        /// Server Standard Edition (core installation)
        /// </summary>
        StandardServerCore = 0x0000000D,

        /// <summary>
        /// Starter Edition
        /// </summary>
        Starter = 0x0000000B,

        /// <summary>
        /// Storage Server Enterprise Edition
        /// </summary>
        StorageEnterpriseServer = 0x00000017,

        /// <summary>
        /// Storage Server Express Edition
        /// </summary>
        StorageExpressServer = 0x00000014,

        /// <summary>
        /// Storage Server Standard Edition
        /// </summary>
        StorageStandardServer = 0x00000015,

        /// <summary>
        /// Storage Server Workgroup Edition
        /// </summary>
        StorageWorkgroupServer = 0x00000016,

        /// <summary>
        /// An unknown product
        /// </summary>
        Undefined = 0x00000000,

        /// <summary>
        /// Unknown non- activated version that has had its grace period expire
        /// </summary>
        Unlicensed = 0xABCDABCD,

        /// <summary>
        /// Ultimate Edition
        /// </summary>
        Ultimate = 0x00000001,

        /// <summary>
        /// Web Server Edition
        /// </summary>
        WebServer = 0x00000011
    }

    public static class OSVersion
    {
        private static int _bits = IntPtr.Size * 8;
        private static OSArch _arch = IntPtr.Size == 4 ? OSArch.I386 : OSArch.Amd64;
        private static WindowsVersion _windowsVersion;

        private static ProcessAccess _minProcessQueryInfoAccess = ProcessAccess.QueryInformation;
        private static ThreadAccess _minThreadQueryInfoAccess = ThreadAccess.QueryInformation;
        private static ThreadAccess _minThreadSetInfoAccess = ThreadAccess.SetInformation;

        private static bool _hasCycleTime = false;
        private static bool _hasExtendedTaskbar = false;
        private static bool _hasProtectedProcesses = false;
        private static bool _hasPsSuspendResumeProcess = false;
        private static bool _hasQueryLimitedInformation = false;
        private static bool _hasSetAccessToken = false;
        private static bool _hasTaskDialogs = false;
        private static bool _hasUac = false;
        private static bool _hasWin32ImageFileName = false;

        static OSVersion()
        {
            System.Version version = Environment.OSVersion.Version;

            if (version.Major == 5 && version.Minor == 0)
                _windowsVersion = WindowsVersion.Win2000;
            else if (version.Major == 5 && version.Minor == 1)
                _windowsVersion = WindowsVersion.XP;
            else if (version.Major == 5 && version.Minor == 2)
                _windowsVersion = WindowsVersion.Server2003;
            else if (version.Major == 6 && version.Minor == 0)
                _windowsVersion = WindowsVersion.Vista;
            else if (version.Major == 6 && version.Minor == 1)
                _windowsVersion = WindowsVersion.Seven;
            else if ((version.Major == 6 && version.Minor > 1) || version.Major > 6)
                _windowsVersion = WindowsVersion.Unreleased;

            if (IsBelow(WindowsVersion.Vista))
            {
                _hasSetAccessToken = true;
            }

            if (IsAboveOrEqual(WindowsVersion.Vista))
            {
                _minProcessQueryInfoAccess = ProcessAccess.QueryLimitedInformation;
                _minThreadQueryInfoAccess = ThreadAccess.QueryLimitedInformation;
                _minThreadSetInfoAccess = ThreadAccess.SetLimitedInformation;

                _hasCycleTime = true;
                _hasProtectedProcesses = true;
                _hasPsSuspendResumeProcess = true;
                _hasQueryLimitedInformation = true;
                _hasTaskDialogs = true;
                _hasUac = true;
                _hasWin32ImageFileName = true;
            }

            if (IsAboveOrEqual(WindowsVersion.Seven))
            {
                _hasExtendedTaskbar = true;
            }
        }

        public static int Bits
        {
            get { return _bits; }
        }

        public static string BitsString
        {
            get { return _bits.ToString() + "-" + "bit"; }
        }

        public static OSArch Architecture
        {
            get { return _arch; }
        }

        public static WindowsVersion WindowsVersion
        {
            get { return _windowsVersion; }
        }

        public static ProcessAccess MinProcessQueryInfoAccess
        {
            get { return _minProcessQueryInfoAccess; }
        }

        public static ThreadAccess MinThreadQueryInfoAccess
        {
            get { return _minThreadQueryInfoAccess; }
        }

        public static ThreadAccess MinThreadSetInfoAccess
        {
            get { return _minThreadSetInfoAccess; }
        }

        public static bool HasCycleTime
        {
            get { return _hasCycleTime; }
        }

        public static bool HasExtendedTaskbar
        {
            get { return _hasExtendedTaskbar; }
        }

        public static bool HasProtectedProcesses
        {
            get { return _hasProtectedProcesses; }
        }

        public static bool HasPsSuspendResumeProcess
        {
            get { return _hasPsSuspendResumeProcess; }
        }

        public static bool HasQueryLimitedInformation
        {
            get { return _hasQueryLimitedInformation; }
        }

        public static bool HasSetAccessToken
        {
            get { return _hasSetAccessToken; }
        }

        public static bool HasTaskDialogs
        {
            get { return _hasTaskDialogs; }
        }

        public static bool HasUac
        {
            get { return _hasUac; }
        }

        public static bool HasWin32ImageFileName
        {
            get { return _hasWin32ImageFileName; }
        }

        public static bool IsAmd64()
        {
            return _arch == OSArch.Amd64;
        }

        public static bool IsI386()
        {
            return _arch == OSArch.I386;
        }

        public static bool IsAbove(WindowsVersion version)
        {
            return (int)_windowsVersion > (int)version;
        }

        public static bool IsAboveOrEqual(WindowsVersion version)
        {
            return (int)_windowsVersion >= (int)version;
        }

        public static bool IsBelowOrEqual(WindowsVersion version)
        {
            return (int)_windowsVersion <= (int)version;
        }

        public static bool IsBelow(WindowsVersion version)
        {
            return (int)_windowsVersion < (int)version;
        }


        /// <summary>
        /// Returns the product type of the operating system running on this computer.
        /// </summary>
        /// <returns>A string containing the the operating system product type.</returns>
        private static string GetOSProductType()
        {    
            int VER_NT_WORKSTATION = 1;
            int VER_NT_DOMAIN_CONTROLLER = 2;
            int VER_NT_SERVER = 3;
            int VER_SUITE_SMALLBUSINESS = 1;
            int VER_SUITE_ENTERPRISE = 2;
            int VER_SUITE_TERMINAL = 16;
            int VER_SUITE_DATACENTER = 128;
            int VER_SUITE_SINGLEUSERTS = 256;
            int VER_SUITE_PERSONAL = 512;
            int VER_SUITE_BLADE = 1024;
            
            OSVERSIONINFOEX osVersionInfo = new OSVERSIONINFOEX();
            OperatingSystem osInfo = Environment.OSVersion;

            osVersionInfo.dwOSVersionInfoSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(OSVERSIONINFOEX));

            if (!GetVersionEx(ref osVersionInfo))
            {
                return "Unknown";
            }
            else
            {
                if (osInfo.Version.Major == 5)
                {
                    if (osVersionInfo.wProductType == VER_NT_WORKSTATION)
                    {
                        if ((osVersionInfo.wSuiteMask & VER_SUITE_PERSONAL) == VER_SUITE_PERSONAL)
                        { return "Home Edition"; } /* Windows XP Home Edition */
                        else
                        { return "Professional"; } /* Windows XP / Windows 2000 Professional */
                    }
                    else if (osVersionInfo.wProductType == VER_NT_SERVER)
                    {
                        if (osInfo.Version.Minor == 0)
                        {
                            if ((osVersionInfo.wSuiteMask & VER_SUITE_DATACENTER) == VER_SUITE_DATACENTER)
                            { return "Datacenter Server"; } // Windows 2000 Datacenter Server
                            else if ((osVersionInfo.wSuiteMask & VER_SUITE_ENTERPRISE) == VER_SUITE_ENTERPRISE)
                            { return "Advanced Server"; } // Windows 2000 Advanced Server
                            else
                            { return "Server"; } //Windows 2000 Server
                        }
                        else
                        {
                            if ((osVersionInfo.wSuiteMask & VER_SUITE_DATACENTER) == VER_SUITE_DATACENTER)
                            { return "Datacenter Edition"; }//Windows Server 2003 Datacenter Edition
                            else if ((osVersionInfo.wSuiteMask & VER_SUITE_ENTERPRISE) == VER_SUITE_ENTERPRISE)
                            { return "Enterprise Edition"; }//Windows Server 2003 Enterprise Edition
                            else if ((osVersionInfo.wSuiteMask & VER_SUITE_BLADE) == VER_SUITE_BLADE)
                            { return "Web Edition"; } //Windows Server 2003 Web Edition
                            else
                            { return "Standard Edition"; }// Windows Server 2003 Standard Edition
                        }
                    }
                }
            }

            return "";
        }

        /// <summary>
        /// Returns the service pack of the operating system running on this computer.
        /// </summary>
        /// <returns>A string containing the the operating system service pack information.</returns>
        private static string GetOSServicePack()
        {
            OSVERSIONINFOEX osVersionInfo = new OSVERSIONINFOEX();
            osVersionInfo.dwOSVersionInfoSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(OSVERSIONINFOEX));

            if (!GetVersionEx(ref osVersionInfo))
            {
                return "SP0";
            }
            else
            {
                return osVersionInfo.szCSDVersion;
            }
        }

        public static string GetProductType()
        {
            //If IsAboveOrEqual Vista then query API and return OS edition type 
            if (OSVersion.IsAboveOrEqual(WindowsVersion.Vista))
            {
                WindowsType edition = WindowsType.Undefined;

                GetProductInfo(6, 0, 0, 0, out edition);

                return "Windows " + _windowsVersion + " " + edition.ToString() + " " + GetOSServicePack();
            }
            else //return OS type based on VersionEx API output
            {
                return "Windows " + _windowsVersion + " " + GetOSProductType() + " " + GetOSServicePack();
            }
        }

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        private struct OSVERSIONINFOEX
        {
            public int dwOSVersionInfoSize;
            public int dwMajorVersion;
            public int dwMinorVersion;
            public int dwBuildNumber;
            public int dwPlatformId;
            [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 128)]
            public string szCSDVersion;
            public short wServicePackMajor;
            public short wServicePackMinor;
            public short wSuiteMask;
            public byte wProductType;
            public byte wReserved;
        }

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool GetVersionEx(ref OSVERSIONINFOEX osVersionInfo);       


        //API is only supported in version 6.0.0.0 or higher
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool GetProductInfo(int osMajorVersion, int osMinorVersion, int spMajorVersion, int spMinorVersion, out WindowsType edition);

    }
}
