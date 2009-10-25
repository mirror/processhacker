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
using System.Runtime.InteropServices;

namespace ProcessHackerRestartRecovery
{
    /// <summary>
    /// The Delegate that represents the callback method invoked by the system when an application has registered for application recovery. 
    /// </summary>
    /// <param name="state">An application-defined state object that is passed to the callback method.</param>
    /// <remarks>The callback method will be invoked prior to the application being terminated by Windows Error Reporting (WER). 
    /// To keep WER from terminating the application before the callback method completes, the callback method must
    /// periodically call the ApplicationRestartRecoveryManager.ApplicationRecoveryInProgress method.</remarks>
    public delegate int RecoveryCallback(object state);

    /// <summary>
    /// Defines a class that contains a callback delegate and properties of the application as defined by the user.
    /// </summary>
    public class RecoveryData
    {
        /// <summary>
        /// Initializes a recovery data wrapper with a callback method and the current state of the application.
        /// </summary>
        /// <param name="callback">The callback delegate.</param>
        /// <param name="state">The current state of the application.</param>
        public RecoveryData(RecoveryCallback callback, object state)
        {
            Callback = callback;
            State = state;
        }

        /// <summary>
        /// Gets or sets a value that determines the recovery callback function.
        /// </summary>
        public RecoveryCallback Callback { get; set; }

        /// <summary>
        /// Gets or sets a value that determines the application state.
        /// </summary>
        public object State { get; set; }

        /// <summary>
        /// Invokes the recovery callback function.
        /// </summary>
        public void Invoke()
        {
            if(Callback != null)
                Callback(State);
        }
    }
}
