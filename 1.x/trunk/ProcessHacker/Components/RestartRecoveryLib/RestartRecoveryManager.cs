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

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Diagnostics;
using ProcessHacker.Native;
using ProcessHacker.Native.Api;

namespace ProcessHackerRestartRecovery
{
    /// <summary>
    /// Provides access to the Application Restart and Recovery
    /// features available in Windows Vista or higher. Application Restart and Recovery lets an
    /// application do some recovery work to save data before the process exits.
    /// </summary>
    public static class ApplicationRestartRecoveryManager
    {
        public static void RegisterForRestart()
        {
            // Register for automatic restart if the application was terminated for any reason other than a system reboot or a system update.
            ApplicationRestartRecoveryManager.RegisterForApplicationRestart(
                new RestartSettings("-recovered", 
                    RestartRestrictions.NotOnReboot 
                    | RestartRestrictions.NotOnPatch));
        }

        public static void RegisterForRecovery()
        {
            // Since this registration is being done on application startup, we don't have a state currently.
            // In some cases it might make sense to pass this initial state.
            // Another approach: When doing "auto-save", register for recovery everytime, and pass
            // the current state I.E. data for recovery at that time. 
            RecoveryData data = new RecoveryData(new RecoveryCallback(RecoveryProcedure), null);
            RecoverySettings settings = new RecoverySettings(data, 0);
            ApplicationRestartRecoveryManager.RegisterForApplicationRecovery(settings);
        }

        /// <summary>
        /// This method is invoked by WER.
        /// </summary>
        /// <param name="state">Application state</param>
        /// <returns>A value for WER</returns>
        private static int RecoveryProcedure(object state)
        {
            PingSystem();

            // Do recovery work here.
            // Do {Report Error to SF, SaveData} etc
            // Write the contents to a file, as well as some other data that we need...

            try
            {
                // Remove the icons or they remain in the system try.
                ProcessHacker.Program.HackerWindow.ExecuteOnIcons((icon) => icon.Visible = false);
                ProcessHacker.Program.HackerWindow.ExecuteOnIcons((icon) => icon.Dispose());

                // Make sure KPH connection is closed.
                if (ProcessHacker.Native.KProcessHacker.Instance != null)
                    ProcessHacker.Native.KProcessHacker.Instance.Close();
            }
            catch { }

            // Application is now shutting down...
            // Signal to WER that the recovery has finished, only call this once. 
            // this is the very last call that will be made.
            ApplicationRecoveryFinished(true);

            return 0;
        }

        /// <summary>
        /// This method is called periodically to ensure that WER knows that recovery is still in progress.
        /// </summary>
        private static void PingSystem()
        {
            // Find out if the user canceled recovery.
            bool isCanceled = ApplicationRecoveryInProgress();

            if (isCanceled)
            {
                System.Windows.Forms.MessageBox.Show("Recovery has been canceled by user.");
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// This method gets called by main when the commandline arguments indicate that this application was automatically restarted by WER.
        /// </summary>
        public static void RecoverLastSession()
        {
            // TODO: Perform application state restoration actions here.
            // Do {LoadData, ShowError, ShowRecovered} etc
        }

        /// <summary>
        /// Registers an application for recovery by Application Restart and Recovery.
        /// </summary>
        /// <param name="settings">An object that specifies the callback method, an optional parameter to pass to the callback
        /// method and a time interval.</param>
        /// <remarks>The time interval is the period of time within which the recovery callback method calls 
        /// the ApplicationRecoveryInProgress method to indicate that it is still performing recovery work.</remarks>
        private static void RegisterForApplicationRecovery(RecoverySettings settings)
        {
            if (OSVersion.IsAboveOrEqual(WindowsVersion.Vista))
            {
                if (settings == null)
                    throw new ArgumentNullException("settings");

                GCHandle handle = GCHandle.Alloc(settings.RecoveryData);

                HResult hr = AppRestartRecoveryNativeMethods.RegisterApplicationRecoveryCallback(AppRestartRecoveryNativeMethods.internalCallback, (IntPtr)handle, settings.PingInterval, (uint)0);

                if (hr == HResult.InvalidArgument)
                    throw new ArgumentException("Application was not registered for recovery due to bad parameters.");
                else if (hr == HResult.Fail)
                    throw new ExternalException("Application failed to register for recovery.");
            }
        }

        /// <summary>
        /// Removes an application's recovery registration.
        /// </summary>
        private static void UnregisterApplicationRecovery()
        {
            if (OSVersion.IsAboveOrEqual(WindowsVersion.Vista))
            {
                HResult hr = AppRestartRecoveryNativeMethods.UnregisterApplicationRecoveryCallback();

                if (hr == HResult.Fail)
                    throw new ExternalException("Unregister for recovery failed.");
            }
        }

        /// <summary>
        /// Removes an application's restart registration.
        /// </summary>
        private static void UnregisterApplicationRestart()
        {
            if (OSVersion.IsAboveOrEqual(WindowsVersion.Vista))
            {
                HResult hr = AppRestartRecoveryNativeMethods.UnregisterApplicationRestart();

                if (hr == HResult.Fail)
                    throw new ExternalException("Unregister for restart failed.");
            }
        }

        /// <summary>
        /// Called by an application's RecoveryCallback method 
        /// to indicate that it is still performing recovery work.
        /// </summary>
        /// <returns>A Boolean value indicating whether the user canceled the recovery.</returns>
        private static bool ApplicationRecoveryInProgress()
        {
            if (OSVersion.IsAboveOrEqual(WindowsVersion.Vista))
            {
                bool canceled = false;

                HResult hr = AppRestartRecoveryNativeMethods.ApplicationRecoveryInProgress(out canceled);

                if (hr == HResult.Fail)
                    throw new InvalidOperationException("This method must be called from the registered callback method.");

                return canceled;
            }
            else
                return true;
        }

        /// <summary>
        /// Called by an application's RecoveryCallback method to indicate that the recovery work is complete.
        /// </summary>
        /// <remarks>
        /// This should be the last call made by the RecoveryCallback method because
        /// Windows Error Reporting will terminate the application after this method is invoked.
        /// </remarks>
        /// <param name="success">true to indicate the the program was able to complete its recovery
        /// work before terminating; otherwise false</param>
        private static void ApplicationRecoveryFinished(bool success)
        {
            if (OSVersion.IsAboveOrEqual(WindowsVersion.Vista))
            {
                AppRestartRecoveryNativeMethods.ApplicationRecoveryFinished(success);
            }
        }

        /// <summary>
        /// Registers an application for automatic restart if the application is terminated by Windows Error Reporting.
        /// </summary>
        /// <param name="settings">An object that specifies the command line arguments used to restart the 
        /// application, and  the conditions under which the application should not be  restarted.</param>
        /// <remarks>A registered application will not be restarted if it executed for less than 60 seconds before terminating.</remarks>
        private static void RegisterForApplicationRestart(RestartSettings settings)
        {
            if (OSVersion.IsAboveOrEqual(WindowsVersion.Vista))
            {
                HResult hr = AppRestartRecoveryNativeMethods.RegisterApplicationRestart(settings.Command, settings.Restrictions);

                if (hr == HResult.Fail)
                    throw new InvalidOperationException("Application failed to registered for restart.");
                else if (hr == HResult.InvalidArgument)
                    throw new ArgumentException("Failed to register application for restart due to bad parameters.");
            }
        }   
    }
}

