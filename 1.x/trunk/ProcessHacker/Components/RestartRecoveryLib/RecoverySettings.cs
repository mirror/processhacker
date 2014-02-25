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

namespace ProcessHackerRestartRecovery
{
    /// <summary>
    /// Defines methods and properties for recovery settings, and specifies options for an application that attempts
    /// to perform final actions after a fatal event, such as an unhandled exception.
    /// </summary>
    /// <remarks>This class is used to register for application recovery.
    /// See the ApplicationRestartRecoveryManager class.
    /// </remarks>
    public class RecoverySettings
    {
        private RecoveryData recoveryData;
        private uint pingInterval;

        /// <summary>
        /// Initializes a new instance of the RecoverySettings class.
        /// </summary>
        /// <param name="data">A recovery data object that contains the callback method (invoked by the system
        /// before Windows Error Reporting terminates the application) and an optional state object.</param>
        /// <param name="interval">The time interval within which the 
        /// callback method must invoke ApplicationRestartRecoveryManager.ApplicationRecoveryInProgress to 
        /// prevent WER from terminating the application.</param>
        public RecoverySettings(RecoveryData data, uint interval)
        {
            this.recoveryData = data;
            this.pingInterval = interval;
        }

        /// <summary>
        /// Gets the recovery data object that contains the callback method and an optional
        /// parameter (usually the state of the application) to be passed to the callback method.
        /// </summary>
        /// <value>A RecoveryData object.</value>
        public RecoveryData RecoveryData
        {
            get { return recoveryData; }
        }

        /// <summary>
        /// Gets the time interval for notifying Windows Error Reporting.  
        /// The RecoveryCallback method must invoke ApplicationRestartRecoveryManager.ApplicationRecoveryInProgress 
        /// within this interval to prevent WER from terminating the application.
        /// </summary>
        /// <remarks>        
        /// The recovery ping interval is specified in milliseconds. 
        /// By default, the interval is 5 seconds. 
        /// If you specify zero, the default interval is used. 
        /// </remarks>
        public uint PingInterval
        {
            get { return pingInterval; }
        }

        /// <summary>
        /// Returns a string representation of the current state of this object.
        /// </summary>
        /// <returns>A String object.</returns>
        public override string ToString()
        {
            return String.Format("delegate: {0}, state: {1}, ping: {2}",
                this.recoveryData.Callback.Method.ToString(),
                this.recoveryData.State.ToString(),
                this.PingInterval);
        }
    }
}

