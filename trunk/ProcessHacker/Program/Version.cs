using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessHacker
{
    public enum WindowsVersion
    {
        /// <summary>
        /// Windows XP SP2 and SP3.
        /// </summary>
        XP = 51,

        /// <summary>
        /// Windows Server 2003.
        /// </summary>
        Server2003 = 52,

        /// <summary>
        /// Windows Vista SP0 and SP1, Windows Server 2008.
        /// </summary>
        Vista = 60,

        /// <summary>
        /// Windows 7 SP0.
        /// </summary>
        Seven = 61
    }

    public static class Version
    {
        private static bool _initialized = false;
        private static WindowsVersion _windowsVersion;
        private static bool _hasCycleTime = false;
        private static bool _hasMmCopyVirtualMemory = false;
        private static bool _hasProtectedProcesses = false;
        private static bool _hasPsSuspendResumeProcess = false;
        private static bool _hasQueryLimitedInformation = false;
        private static bool _hasSetAccessToken = false;
        private static bool _hasTaskDialogs = false;
        private static bool _hasUac = false;
        private static bool _hasWin32ImageFileName = false;

        public static WindowsVersion WindowsVersion
        {
            get { return _windowsVersion; }
        }

        public static bool HasCycleTime
        {
            get { return _hasCycleTime; }
        }

        public static bool HasMmCopyVirtualMemory
        {
            get { return _hasMmCopyVirtualMemory; }
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

        public static void Initialize()
        {
            if (_initialized)
                throw new Exception("Version information cannot be initialized more than once.");

            System.Version version = Environment.OSVersion.Version;

            if (version.Major == 5 && version.Minor == 1)
                _windowsVersion = WindowsVersion.XP;
            else if (version.Major == 5 && version.Minor == 2)
                _windowsVersion = WindowsVersion.Server2003;
            else if (version.Major == 6 && version.Minor == 0)
                _windowsVersion = WindowsVersion.Vista;
            else if (version.Major == 6 && version.Minor == 1)
                _windowsVersion = WindowsVersion.Seven;

            if (IsBelow(WindowsVersion.Vista))
            {
                _hasSetAccessToken = true;
            }

            if (IsAboveOrEqual(WindowsVersion.Vista))
            {
                _hasCycleTime = true;
                _hasMmCopyVirtualMemory = true;
                _hasProtectedProcesses = true;
                _hasPsSuspendResumeProcess = true;
                _hasQueryLimitedInformation = true;
                _hasTaskDialogs = true;
                _hasUac = true;
                _hasWin32ImageFileName = true;
            }

            _initialized = true;
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
    }
}
