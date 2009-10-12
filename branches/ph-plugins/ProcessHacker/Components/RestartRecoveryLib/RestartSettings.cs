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
    /// Specifies the options for an application to be automatically restarted by Windows Error Reporting. 
    /// </summary>
    /// <remarks>Regardless of these settings, the application will not be restarted if it executed for 
    /// less than 60 seconds beforeterminating.</remarks>
    public class RestartSettings
    {
        private string command;
        private RestartRestrictions restrictions;

        /// <summary>
        /// Creates a new instance of the RestartSettings class.
        /// </summary>
        /// <param name="commandLine">The command line arguments used to restart the application.</param>
        /// <param name="restrict">A bitwise combination of the RestartRestrictions 
        /// values that specify when the application should not be restarted.
        /// </param>
        public RestartSettings(string commandLine, RestartRestrictions restrict)
        {
            command = commandLine;
            restrictions = restrict;
        }

        /// <summary>
        /// Gets the command line arguments used to restart the application.
        /// </summary>
        /// <value>A String object.</value>
        public string Command
        {
            get { return command; }
        }

        /// <summary>
        /// Gets the set of conditions when the application should not be restarted.
        /// </summary>
        /// <value>A set of RestartRestrictions values.</value>
        public RestartRestrictions Restrictions
        {
            get { return restrictions; }
        }

        /// <summary>
        /// Returns a string representation of the current state of this object.
        /// </summary>
        /// <returns>A String that displays the command line arguments and restrictions for restarting the application.</returns>
        public override string ToString()
        {
            return String.Format("Command: {0} Restrictions: {1}", command, restrictions.ToString());
        }
    }
}

