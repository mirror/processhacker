/*
 * Process Hacker - 
 *   ProcessHacker Restart and Recovery Extensions
 * 
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
 * 
 */

using System.Runtime.InteropServices;
using ProcessHacker.Native.Api;
using System.Security;
using System;

namespace ProcessHackerRestartRecovery
{
    [SuppressUnmanagedCodeSecurity]
    internal static class AppRestartRecoveryNativeMethods
    {
        #region Application Restart and Recovery Definitions

        internal delegate UInt32 InternalRecoveryCallback(IntPtr state); 
        
        internal static InternalRecoveryCallback internalCallback;

        static AppRestartRecoveryNativeMethods()
        {
            internalCallback = new InternalRecoveryCallback(InternalRecoveryHandler);
        }

        private static UInt32 InternalRecoveryHandler(IntPtr parameter)
        {
            bool cancelled = false;
            ApplicationRecoveryInProgress(out cancelled);

            GCHandle handle = GCHandle.FromIntPtr(parameter);
            RecoveryData data = handle.Target as RecoveryData;
            data.Invoke();
            handle.Free();

            return 0;
        }

        [DllImport("kernel32.dll")]
        internal static extern void ApplicationRecoveryFinished(
           [MarshalAs(UnmanagedType.Bool)] 
            bool success
            );

        [DllImport("kernel32.dll")]
        internal static extern HResult ApplicationRecoveryInProgress(
            [Out, MarshalAs(UnmanagedType.Bool)] 
            out bool canceled
            );

        [DllImport("kernel32.dll")]
        internal static extern HResult GetApplicationRecoveryCallback(
            IntPtr processHandle,
            [Out] RecoveryCallback recoveryCallback,
            [Out] out object state,
            [Out] out uint pingInterval,
            [Out] out uint flags
            );

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        internal static extern HResult RegisterApplicationRecoveryCallback(
            InternalRecoveryCallback callback, IntPtr param,
            uint pingInterval,
            uint flags //Unused
            );


        [DllImport("kernel32.dll")]
        internal static extern HResult RegisterApplicationRestart(
            [MarshalAs(UnmanagedType.LPWStr)] 
            string commandLineArgs,
            RestartRestrictions flags
            );

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern HResult GetApplicationRestartSettings(
            IntPtr process,
            IntPtr commandLine,
            ref uint size,
            [Out] out RestartRestrictions flags
            );

        [DllImport("kernel32.dll")]
        internal static extern HResult UnregisterApplicationRecoveryCallback();

        [DllImport("kernel32.dll")]
        internal static extern HResult UnregisterApplicationRestart();

        #endregion
    }

    /// <summary>
    /// Specifies the conditions when Windows Error Reporting
    /// should not restart an application that has registered
    /// for automatic restart.
    /// </summary>
    [Flags]
    public enum RestartRestrictions
    {
        /// <summary>
        /// Always restart the application.
        /// </summary>
        None = 0,
        /// <summary>
        /// Do not restart when the application has crashed.
        /// </summary>
        NotOnCrash = 1,
        /// <summary>
        /// Do not restart when the application is hung.
        /// </summary>
        NotOnHang = 2,
        /// <summary>
        /// Do not restart when the application is terminated
        /// due to a system update.
        /// </summary>
        NotOnPatch = 4,
        /// <summary>
        /// Do not restart when the application is terminated 
        /// because of a system reboot.
        /// </summary>
        NotOnReboot = 8
    }
}
